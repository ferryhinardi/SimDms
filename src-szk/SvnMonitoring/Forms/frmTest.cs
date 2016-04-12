using JSMSCentreSvc.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSMSCentreSvc
{
    public partial class frmTest : Form
    {
        public frmTest()
        {
            InitializeComponent();
        }

        //EmployeeDB db = new EmployeeDB();
        NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["EmployeeConnString"].ToString());

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //var emp = db.Database.SqlQuery<msemployee>("select * from msemployee").ToList();

                //dataGridView1.DataSource = emp;
                //dataGridView1.Refresh();
                //stopwatch.Stop();
                //button1.Text = stopwatch.Elapsed.ToString();
            
        }


        private void frmTest_Load(object sender, EventArgs e)
        {
            conn.Open();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            NpgsqlCommand command = new NpgsqlCommand("select * from msemployee", conn);
            try
            {
                NpgsqlDataReader dr = command.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                dataGridView2.DataSource = dt;
                dataGridView2.Refresh();
            }
            finally
            {                
            }
            stopwatch.Stop();
            button2.Text = stopwatch.Elapsed.ToString();
        }

        private void frmTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            conn.Close();
        }
    }
}
