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

using Oracle.ManagedDataAccess.Client;
using System.Threading;
using SVNMON.WebServer.Configs;
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
using SVNMON.Models;

namespace SVNMON.Panels
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

                SimDMSDB db = new SimDMSDB();
                
                var data = db.SysRepositories.ToList();

                foreach (var item in data)
                {
                    try
                    {
                        string msg = MyShared.RunExternalExe(item.PathName, "svn", "update --username sdms --password sdms");
                        string revision = msg.Substring(msg.LastIndexOf(' ') + 1 );
                        item.LastMessage = msg;
                        item.Revision = revision;
                        item.LastUpdate = DateTime.Now;
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MyShared.Log.Info(ex.Message);
                    }
                }


                dataGridView1.DataSource = data;
                dataGridView1.Refresh();

                
                //textBox1.Text = MyShared.RunExternalExe("fossil");
                    //using (SqlConnection CN =  MyShared.Conn)
                    //{
                    //    // buka koneksi ke database tujuan / async mode
                    //    await CN.OpenAsync();

                        

                    //    MyShared.MergeDatabase(@"D:\Projects\6419401_4W_20140908_0835.db3","4W",CN,"343434");

                    //    MessageBox.Show("Done");
                        



                    //}
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

        private void button2_Click(object sender, EventArgs e)
        {


            using (SQLiteConnection db3 = new SQLiteConnection())
            {
                db3.ConnectionString = "Data Source=d:/6419401_4W_20141111_CsTDayCall.db3";
                db3.Open();
                string a = "Asas";
                a.Replace("T", " ");
                using (SQLiteCommand cmd = db3.CreateCommand())
                
                {
                    cmd.CommandText = "select * from cstdaycall";
                    var dba = new SQLiteDataAdapter(cmd);
                    var dt = new DataTable();
                    dba.Fill(dt);

                    dataGridView1.DataSource = dt;

                }
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = MyGlobalVar.Encrypt(textBox1.Text,"20151015-fvXt9FzImqs3p6zfxdzBoQ==","fvXt9FzImqs3p6zfxdzBoQ==20151015");
            textBox3.Text = MyGlobalVar.Decrypt(textBox2.Text, "20151015-fvXt9FzImqs3p6zfxdzBoQ==", "fvXt9FzImqs3p6zfxdzBoQ==20151015");

        }


    }
}
