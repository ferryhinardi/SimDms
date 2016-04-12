using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;

using Oracle.ManagedDataAccess.Client;
using System.Threading;
using SCHEMON.WebServer.Configs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using System.Web.Hosting;
using System.Xml;
using Newtonsoft.Json;
using System.Data.SqlClient;
using EventScheduler;
using System.Net;
using System.Net.Mail;
using System.Data.SQLite;
using Serilog;

namespace SCHEMON.Panels
{
    public partial class ServiceCfg : UserControl
    {


        public ServiceCfg()
        {
            InitializeComponent();
        }


        private async void button1_Click(object sender, EventArgs e)
        {

            try
            {

                    using (SqlConnection CN =  MyShared.Conn)
                    {
                        // buka koneksi ke database tujuan / async mode
                        await CN.OpenAsync();

                        

                        MyShared.MergeDatabase(@"D:\Projects\6419401_4W_20140908_0835.db3","4W",CN,"343434");

                        MessageBox.Show("Done");
                        



                    }
            }
            catch (Exception eX)
            {
                MessageBox.Show("Error on import data : " + eX.Message);
            }  


        }

        public void OnMyNotificaton(object src, OracleNotificationEventArgs arg)
        {
            IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hub.Clients.All.Test("Update");
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var strId = textBox1.Text;

            MyShared.RunMergeProcess3(strId);

            MessageBox.Show("On Progress");

        }

        private void button3_Click(object sender, EventArgs e)
        {

            var Log = new LoggerConfiguration()
            .WriteTo.Seq("http://localhost:5341")
            .CreateLogger();

            Log.Information("Starting up");

            var rng = new Random();
            while (true)
            {
                var amount = rng.Next() % 100 + 1;

                Log.Debug("Received an debug order totalling " + DateTime.Now.ToString());
                Log.Information("Received an Info order totalling " + DateTime.Now.ToString());
                Log.Error("Received an Error order totalling " + DateTime.Now.ToString());
                Log.Fatal("Received an Fatal order totalling " + DateTime.Now.ToString());
                Log.Warning("Received an Warning order totalling " + DateTime.Now.ToString());

                Thread.Sleep(1000);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            var ret = dlg.ShowDialog();
            if (ret != null){
                textBox1.Text = dlg.SelectedPath;
                var files = System.IO.Directory.GetFiles(textBox1.Text);
                listBox1.Items.Clear();
                foreach (var item in files)
                {
                    listBox1.Items.Add(item);
                }

            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            var files = listBox1.Items;

            var Log = new LoggerConfiguration()
           .WriteTo.Seq("http://localhost:5341")
           .CreateLogger();

            try
            {

                using (SqlConnection CN = MyShared.Conn)
                {

                    await CN.OpenAsync();

                    SqlCommand cmd = CN.CreateCommand();
                    cmd.CommandTimeout = 1800;
                    int nRow = files.Count, i = 0;
                    var id = "MyJob";


                    var SQL = "delete from sysdealerjobs where taskname='" + id + "'; Insert into sysdealerjobs (TaskName, StartDate, JobTotal, Status) values ('" + id +
                              "', getdate()," + nRow.ToString() + ",0);";
                        cmd.CommandText = SQL;
                        cmd.ExecuteNonQuery();

                        Log.Information(SQL);

                        foreach (var item in files)
                        {
                            textBox1.Text = item.ToString();

                            Log.Information(textBox1.Text);

                            try
                            {
                                string optDB = textBox1.Text.Split('_')[1].ToString();
                                MyShared.MergeDatabase(textBox1.Text, optDB, CN, "xxx");
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Error on extract or merge database: " + ex.Message + " >> " + textBox1.Text);
                            }

                            SQL = "update sysdealerjobs set lastupdate=getdate(), onProgress=" + (i++).ToString() +
                                  " where taskname='" + id + "' and status=0";

                            cmd.CommandText = SQL;
                            cmd.ExecuteNonQuery();

                            textBox1.Text = i.ToString();
                            Log.Information(i.ToString());

                        }

                        SQL = "update sysdealerjobs set status=1, lastupdate=getdate(), onProgress=" + nRow.ToString() +
                              " where taskname='" + id + "' and status=0";
                        cmd.CommandText = SQL;
                        cmd.ExecuteNonQuery();                    
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error On Run Merge Manual Process: " + ex.Message);
            }
            finally
            {
                GC.Collect();
            }

        }


    }
}
