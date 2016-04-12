using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Oracle2SQLServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(txtSrc.Text))
                {
                    conn.Open();
 
                    using (OracleCommand cmd = new OracleCommand(txtSQL.Text , conn))
                    {                        
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(reader);
                                dataGridView1.DataSource = null;
                                dataGridView1.Refresh();
                                dataGridView1.DataSource = dataTable;
                                lbCount.Text = dataTable.Rows.Count.ToString();                                 
                                Application.DoEvents();
                            }
                        }
                    }                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                var db = new SqlConnection(txtDst.Text);

                try
                {
                    db.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;  
                }
                using (SqlBulkCopy bcp = new SqlBulkCopy(db))
                {

                    using (OracleConnection conn = new OracleConnection(txtSrc.Text))
                    {
                        conn.Open();

                        int n = int.Parse(txtTotal.Text);
                        int m = int.Parse(txtSize.Text);
                        int x = n / m;

                        pBar.Maximum = x;
                        pBar.Value = 0;

                        OracleCommand cmd = conn.CreateCommand();
                        OracleDataReader reader = null;
                        DataTable dataTable = new DataTable();



                        //for (int i = 0; i < x; i++)
                        //{

                            //pBar.Value = i;
                            //lbCount.Text = i.ToString();
                            Application.DoEvents();

                            string SQL = txtSQL.Text; // +" WHERE " + txtFilter.Text + " > " + (i * m).ToString() + " AND " + txtFilter.Text + " <= " + ((i + 1) * m).ToString();


                            OracleDataAdapter adapter = new OracleDataAdapter(SQL, conn);
                            OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
                            DataSet dataset = new DataSet();
                            adapter.Fill(dataset);
                            DataTable table = dataset.Tables[0];

                            //dataTable.Clear();
                            //cmd.CommandText = SQL;
                             
                            //dataTable.Load(cmd.ExecuteReader());
                            //dataGridView1.DataSource = null;
                            //dataGridView1.Refresh();
                            //dataGridView1.DataSource = dataTable;
                            //lbCount.Text = dataTable.Rows.Count.ToString();
                            //pBar.Value = i;
                            //Application.DoEvents();


                            bcp.DestinationTableName = txtTDst.Text;
                            //bcp.NotifyAfter = dataTable.Rows.Count;
                            bcp.BatchSize = table.Rows.Count;
                            bcp.BulkCopyTimeout = 600;
                            try
                            {
                                bcp.WriteToServer(table);
                            }
                            catch (Exception ex)
                            {

                            }


                        //}


                        //string SQL2 = txtSQL.Text + " WHERE " + txtFilter.Text + " > " + (x * m).ToString();

                        //var adapter2 = new OracleDataAdapter(SQL2, conn);
                        //var builder2 = new OracleCommandBuilder(adapter2);
                        //DataSet dataset2 = new DataSet();
                        //adapter2.Fill(dataset2);
                        //DataTable table2 = dataset2.Tables[0];


                        //pBar.Value = x;
                        //Application.DoEvents();


                        //bcp.DestinationTableName = txtTDst.Text;
                        ////bcp.NotifyAfter = dataTable.Rows.Count;
                        //bcp.BatchSize = dataTable.Rows.Count;
                        //bcp.BulkCopyTimeout = 600;

                        //try
                        //{
                        //    //bcp.WriteToServer(dataTable);
                        //}
                        //catch (Exception ex)
                        //{

                        //}




                    }



                }

                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
