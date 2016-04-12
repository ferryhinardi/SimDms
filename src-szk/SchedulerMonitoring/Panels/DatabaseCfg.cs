using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SCHEMON.Models;
using System.Data.SqlClient;
using Npgsql;
using System.Reflection;
using System.Data.Entity.Validation;
using System.Diagnostics;
using EventScheduler;

namespace SCHEMON.Panels
{
    public partial class DatabaseCfg : UserControl
    {
        public DatabaseCfg()
        {
            InitializeComponent();
        }

        SMSDB db = new SMSDB();
        bool IsProjectDBReady = false, IsEmployeeDBReady = false;

        private void DatabaseCfg_Load(object sender, EventArgs e)
        {
            string smsdbconnstring = System.Configuration.ConfigurationManager.ConnectionStrings["SMSDB"].ToString();
            string prjdbconnstring = System.Configuration.ConfigurationManager.ConnectionStrings["ProjectsDB"].ToString();
            string empdbconnstring = System.Configuration.ConfigurationManager.ConnectionStrings["EmployeeDB"].ToString();

            int start = smsdbconnstring.IndexOf("source=") + 7;
            int nLen = smsdbconnstring.IndexOf(";", start);
            txtSMSSrv.Text = smsdbconnstring.Substring(start, nLen - start);

            start = smsdbconnstring.IndexOf("catalog=") + 8;
            nLen = smsdbconnstring.IndexOf(";", start);
            txtSMSDB.Text = smsdbconnstring.Substring(start, nLen - start);

            start = prjdbconnstring.IndexOf("source=") + 7;
            nLen = prjdbconnstring.IndexOf(";", start);
            txtPrjSrv.Text = prjdbconnstring.Substring(start, nLen - start);

            start = prjdbconnstring.IndexOf("catalog=") + 8;
            nLen = prjdbconnstring.IndexOf(";", start);
            txtPrjDB.Text = prjdbconnstring.Substring(start, nLen - start);

            start = empdbconnstring.IndexOf("server=") + 7;
            nLen = empdbconnstring.IndexOf(";", start);
            txtEmpSrv.Text = empdbconnstring.Substring(start, nLen - start);

            start = empdbconnstring.IndexOf("database=") + 9;
            nLen = empdbconnstring.IndexOf(";", start);
            txtEmpDB.Text = empdbconnstring.Substring(start, nLen - start);

            CreateListTable();

        }

        DataTable resultTable = null;

        private void CheckLastNumber()
        {
            try
            {
                // recreate data table / refresh from database
                CreateListTable();

                // get number of list table
                int n = resultTable.Rows.Count;

                // create/clone connection
                using(var cn = MyShared.ProjectsCN)
                {
                    // open connection
                    cn.Open();

                    for (int i = 0; i < n; i++)
                    {
                        // get current row by index
                        DataRow r = resultTable.Rows[i];

                        // check current row, is projectdb or not? 
                        if (r["Source"].ToString() == "projectdb")
                        {
                            r["SourceNo"] = MyShared.ExecuteScalar(r["SqlGetId"].ToString(), cn);
                            r["Sync"] = (r["SourceNo"].ToString() != r["LocalNo"].ToString());                            
                        }
                    }
                    cn.Close();
                }

                using(var gCN = MyShared.EmployeeCN)
                {
                    gCN.Open();

                    for(int i=0; i < n; i++)
                    {
                        DataRow r = resultTable.Rows[i];
                        if ( r["Source"].ToString() == "employeedb")
                        {
                            r["SourceNo"] = MyShared.ExecuteScalar(r["SqlGetId"].ToString(), gCN);
                            r["Sync"] = (r["SourceNo"].ToString() != r["LocalNo"].ToString());
                        }
                    }
                    gCN.Close();
                }

                dgSynch.Refresh();

            }
            catch (Exception e)
            {
                // Something wrong happened
                MessageBox.Show(e.Message);
            }
        }


        private void CreateListTable()
        {
            try
            {
                using ( SqlConnection cn = MyShared.Conn )
                {
                    cn.Open();

                    using ( var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "select * from IConfigImport";
                        cmd.CommandType = CommandType.Text;
                        resultTable = new DataTable();
                        resultTable.Load(cmd.ExecuteReader());

                        dgSynch.DataSource = null;
                        dgSynch.DataSource = resultTable;

                        dgSynch.Columns[3].Visible = false;
                        dgSynch.Columns[5].Visible = false;
                        dgSynch.Columns[6].Visible = false;

                        dgSynch.Columns[0].Width = 50;
                        dgSynch.Columns[0].HeaderText = "No.";
                        dgSynch.Columns[1].HeaderText = "Name";
                        dgSynch.Columns[2].HeaderText = "Local No.";
                        dgSynch.Columns[4].HeaderText = "Source No.";
                        dgSynch.Columns[7].HeaderText = "Sync";
                        dgSynch.Columns[8].HeaderText = "Last Update";
                        dgSynch.Columns[7].Width = 50;
                        dgSynch.Columns[8].Width = 120;

                        dgSynch.Refresh();
                    }

                    cn.Close();
                }


            }
            catch (Exception e)
            {
                // Something wrong happened
                MessageBox.Show(e.Message);
            }
        }

        // test koneksi ke database employee (PostgreSQL)
        private void btnEmpDbConnect_Click(object sender, EventArgs e)
        {
            try
            {
                // clone koneksi
                using(var pCn = MyShared.EmployeeCN)
                {
                    // open koneksi
                    pCn.Open();

                    // check connection state
                    if (pCn.State == ConnectionState.Open)
                    {
                        lbEmpDbStatus.Text = "CONNECTED";
                        lbEmpDbStatus.ForeColor = lbSmsDBStatus.ForeColor;
                        btnEmpDbConnect.Enabled = false;
                        IsEmployeeDBReady = true;
                        IsAvailableToCheck();
                        pCn.Close();
                    }
                }
            } catch ( Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        // check connection state
        private void IsAvailableToCheck()
        {
            btnCheck.Enabled = IsProjectDBReady && IsEmployeeDBReady;
        }

        // test koneksi ke database project (SQL Server)
        private void btnPrjDbConnect_Click(object sender, EventArgs e)
        {
            try
            {
                // clone a connection
                using(var cn = MyShared.ProjectsCN)
                {
                    // open connection
                    cn.Open();

                    // check connection state
                    if (cn.State == ConnectionState.Open)
                    {
                        lbPrjDbStatus.Text = "CONNECTED";
                        lbPrjDbStatus.ForeColor = lbSmsDBStatus.ForeColor;
                        btnPrjDbConnect.Enabled = false;
                        IsProjectDBReady = true;
                        IsAvailableToCheck();
                        cn.Close();
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            CheckLastNumber();
            btnImport.Enabled = true;
        }


        async Task<int> ImportDataAsync(int Index)
        {
            string localNo = resultTable.Rows[Index]["LocalNo"].ToString();
            string sql_getdata = resultTable.Rows[Index]["SqlGetData"].ToString();
            string sql_getid = resultTable.Rows[Index]["SqlGetId"].ToString();
            string src_num = resultTable.Rows[Index]["SourceNo"].ToString();

            if (localNo != src_num)
            {
                string sql = sql_getdata.Replace("LASTNO", localNo);
                string tableName = sql_getid.Substring(sql_getid.IndexOf("from ") + 5).TrimStart();
                string uSQL = "update IConfigImport set LocalNo='" + src_num + "', SourceNo='" + src_num + "', LastUpdate=getdate() where id=" + (Index + 1);

                if (resultTable.Rows[Index]["source"].ToString()=="projectdb")
                {
                    return await MyShared.AsyncImportDataProjects(sql, tableName, uSQL);
                }
                else
                {
                    return await MyShared.AsyncImportDataEmployees(sql, tableName, uSQL);
                }
            }

            return 0;
            
        }


        private async void btnImport_Click(object sender, EventArgs e)
        {

            int n = resultTable.Rows.Count;

            for (int i = 0; i < n; i++ )
            {
                try
                {
                    await ImportDataAsync(i);
                } 
                catch (Exception eX)
                {
                    MyGlobalVar.DebugInfo("Error on Import Data: " + eX.Message);
                }                
            }

            CheckLastNumber();
            btnImport.Enabled = false;

            MessageBox.Show("Done");

        }


    }
}
