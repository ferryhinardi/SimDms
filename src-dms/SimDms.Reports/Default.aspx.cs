using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SimDms.Reports
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    Label1.Text = "";

                    var id = Request.QueryString["rpt"];
                    var par = Request.QueryString["par"];
                    var ctx = new ReportContext();

                    this.ReportViewer1.Reset();
                    var rpt = ctx.SysReports.Find(id);

                    SqlConnection con = (SqlConnection)ctx.Database.Connection;
                    SqlCommand cmd = con.CreateCommand();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    using (cmd)
                    {
                        cmd.CommandText = string.Format("{0} {1}", rpt.ReportProc, (string.IsNullOrWhiteSpace(par)) ? "" : string.Format("'{0}'", par.Replace(";", "','").Trim()));
                        da.Fill(ds);
                    }

                    ReportViewer1.LocalReport.ReportPath = string.Format(@"Rdlc\{0}", rpt.ReportPath); ;
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        var rds = new ReportDataSource(string.Format("DataSet{0}", i + 1), ds.Tables[i]);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                    }
                }
                catch (Exception ex)
                {
                    Label1.Text = ex.Message;
                }
            }
        }
    }
}