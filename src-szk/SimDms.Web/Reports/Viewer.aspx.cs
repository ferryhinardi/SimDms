using Microsoft.Reporting.WebForms;
using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using SimDms.Web.Controllers;

namespace SimDms.Web.Reports
{
    public partial class Viewer : System.Web.UI.Page
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
                    var type = Request.QueryString["type"];
                    var fileName = (String.IsNullOrWhiteSpace(Request.QueryString["filename"]) ? "ExportReport" : Request.QueryString["filename"].ToString());
                    if (type == "rdlc" || string.IsNullOrEmpty(type))
                    {
                        var ctx = new DataContext();
                        this.ReportViewer1.Reset();
                        var rpt = ctx.SysReports.Find(id);

                        SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

                        //SqlConnection con = (SqlConnection)ctx.Database.Connection;
                        //SqlCommand cmd = con.CreateCommand();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        using (cmd)
                        {
                            cmd.CommandTimeout = 1800;
                            cmd.CommandText = string.Format("{0} {1}", rpt.ReportProc, (string.IsNullOrWhiteSpace(par)) ? "" : string.Format("'{0}'", par.Replace(";", "','").Trim()));
                            da.Fill(ds);
                        }

                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.ReportPath = string.Format(@"reports\rdlc\{0}", rpt.ReportPath); ;
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            var rds = new ReportDataSource(string.Format("DataSet{0}", i + 1), ds.Tables[i]);
                            ReportViewer1.LocalReport.DataSources.Add(rds);
                        }
                    }
                    else if (type == "export")
                    {                        
                        var ctx = new DataContext();
                        this.ReportViewer1.Reset();
                        var rpt = ctx.SysReports.Find(id);

                        SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        using (cmd)
                        {
                            cmd.CommandTimeout = 1800;
                            cmd.CommandText = string.Format("{0} {1}", rpt.ReportProc, (string.IsNullOrWhiteSpace(par)) ? "" : string.Format("'{0}'", par.Replace(";", "','").Trim()));
                            da.Fill(ds);
                        }

                        var reportPath = string.Format(@"reports\rdlc\{0}", rpt.ReportPath); ;
                        var localReport = new LocalReport { ReportPath = reportPath };

                        //ReportViewer1.LocalReport.ReportPath = string.Format(@"reports\rdlc\{0}", rpt.ReportPath); ;
                        ReportDataSource reportDataSource = null;
                        localReport.DataSources.Clear();
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            reportDataSource = new ReportDataSource(string.Format("DataSet{0}", i + 1), ds.Tables[i]);
                            localReport.DataSources.Add(reportDataSource);
                        }

                        var reportType = "excel";
                        string mimeType;
                        string encoding;
                        string fileNameExtension = "xls";

                        //The DeviceInfo settings should be changed based on the reportType
                        //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
                        var deviceInfo =
                            string.Format(@"
                                <DeviceInfo>
                                    <OutputFormat>{0}</OutputFormat>
                                    <PageWidth>12in</PageWidth>
                                    <PageHeight>16in</PageHeight>
                                    <MarginTop>0.5in</MarginTop>
                                    <MarginLeft>1in</MarginLeft>
                                    <MarginRight>0.5in</MarginRight>
                                    <MarginBottom>0.5in</MarginBottom>
                                </DeviceInfo>", reportType);

                        Warning[] warnings;
                        string[] streams;

                        //Render the report
                        var renderedBytes = localReport.Render(
                            reportType,
                            deviceInfo,
                            out mimeType,
                            out encoding,
                            out fileNameExtension,
                            out streams,
                            out warnings);

                        //Clear the response stream and write the bytes to the outputstream
                        //Set content-disposition to "attachment" so that user is prompted to take an action
                        //on the file (open or save)
                        Response.Clear();
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.Buffer = true;
                        Response.ContentType = mimeType;
                        Response.AddHeader("content-disposition", "attachment; filename=" + fileName + "." + fileNameExtension);
                        Response.BinaryWrite(renderedBytes);
                        Response.End();
                    }
                }
                catch (Exception ex)
                {
                    //Label1.Text = ex.Message;
                    Label1.Text = ex.Message;//"No Data Available";
                }
            }
        }
    }
}