using Microsoft.Reporting.WebForms;
using Microsoft.Reporting.Common;
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
//using System.IO;

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
                    var rparam = Request.QueryString["rparam"];
                    var fileName = (String.IsNullOrWhiteSpace(Request.QueryString["filename"]) ? "ExportReport" : Request.QueryString["filename"].ToString());
                    if (type == "rdlc" || string.IsNullOrEmpty(type))
                    {
                        var ctx = new LayoutContext();
                        this.ReportViewer1.Reset();
                        var rpt = ctx.SysReports.Find(id);

                        SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

                        //SqlConnection con = (SqlConnection)ctx.Database.Connection;
                        //SqlCommand cmd = con.CreateCommand();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        using (cmd)
                        {
                            cmd.CommandText = string.Format("{0} {1}", rpt.ReportProc, (string.IsNullOrWhiteSpace(par)) ? "" : string.Format("'{0}'", par.Replace(";", "','").Trim()));
                            da.Fill(ds);
                        }

                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.ReportPath = string.Format(@"reports\rdlc\{0}", rpt.ReportPath);
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            var rds = new ReportDataSource(string.Format("DataSet{0}", i + 1), ds.Tables[i]);
                            ReportViewer1.LocalReport.DataSources.Add(rds);
                            //StreamReader sr = File.OpenText(string.Format(@"reports\rdlc\{0}", rpt.ReportPath));
                            //ReportViewer1.LocalReport.LoadSubreportDefinition("Subreport", sr);
                            //ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
                            //AddHandler report.SubreportProcessing, AddressOf ReportUseCaseSubreportProcessingEventHandler;
                        }
                    }
                    else if (type == "devex")
                    {
                        string RptUrl = ConfigurationManager.AppSettings["reportUrl"].ToString().ToLower() ?? "";
                        
                        string appName = Request.ApplicationPath.ToString().ToLower();
                                                
                        string reportUrl = RptUrl.Replace("sdmsreport", "sdmsreport" + appName.Replace(@"/",""));                        

                        reportUrl = reportUrl.Replace('-', '&');
                        
                        string rParam = (string.IsNullOrEmpty(rparam) || rparam == "undefined" ? User.Identity.Name : rparam);
                        string[] parameters = par.Split(',');
                        
                        string url = string.Format(reportUrl, id, BuildStringParameter(id, parameters), rParam);
                        
                        url = url.Replace("localhost", HttpContext.Current.Request.Url.Host);                        

                        Response.Redirect(url+"&uid="+User.Identity.Name, false);
                    }
                    else if (type == "export")
                    {
                        var ctx = new LayoutContext();
                        this.ReportViewer1.Reset();
                        var rpt = ctx.SysReports.Find(id);

                        SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        using (cmd)
                        {
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
                    string infoMsg = string.Empty;
                    string innerMsg = (ex.InnerException != null) ? ", Inner Exception : " + ex.InnerException.InnerException.Message : "";
                    string errorMsg = "Error Message: " + ex.Message + innerMsg;
                    string style = "color:Red;font-family:Tahoma;font-size:24pt;";

                    var ctx = new LayoutContext();
                    var role = ctx.SysUserViews.FirstOrDefault(a => a.UserId == User.Identity.Name).RoleID;
                    if (role.ToUpper() == "DEV-ADMIN")
                    {
                        infoMsg = string.Format("<div><span style='{0}' id='{1}'>{2}</span></div>",
                            style, "Label1", errorMsg);
                    }
                    else
                    {
                        infoMsg = string.Format("<div><span style='{0}' id='{1}'>{2}</span></div>",
                            style, "Label1", "Tidak ada data untuk ditampilkan<br />Silahkan hubungi SDMS support!");
                    }

                    Response.Write(infoMsg);
                }
            }
        }

        private void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("DataSet1"));
            //e.DataSources.Add(new ReportDataSource("DataSet1", ));
        }

        private string BuildStringParameter(string reportID, string[] reportParam)
        {
            LayoutContext ctx = new LayoutContext();
            bool noCode = reportParam.Contains("nocode");
            string pparam = "";
            string productType = "", profitCenter = "", typeofgoods = "";
            var user = ctx.SysUsers.Find(User.Identity.Name);
            profitCenter = ctx.Database.SqlQuery<string>(string.Format("select profitcenter from sysUserProfitCenter where UserID='{0}'", user.UserId)).FirstOrDefault();
            productType = ctx.Database.SqlQuery<string>(string.Format("select ProductType from gnMstCoProfile where CompanyCode={0} and BranchCode={1}", user.CompanyCode, user.BranchCode)).FirstOrDefault();

            //build parameter CompanyCode anda BranchCode 
            if (!reportParam.Contains("companycode") || reportParam.Contains("branchcode"))
            {
                if (!noCode)
                {
                    pparam += "'" + user.CompanyCode + "',";
                    pparam += "'" + user.BranchCode + "',";
                }
            }

            foreach (var param in reportParam)
            {
                if (!string.IsNullOrWhiteSpace(param))
                {
                    switch (param.ToLower().Trim())
                    {
                        case "companycode":
                            pparam += "'" + user.CompanyCode + "',";
                            break;
                        case "branchcode":
                            pparam += "'" + user.BranchCode + "',";
                            break;
                        case "profitcenter":
                            pparam += "'" + profitCenter + "',";
                            break;
                        case "producttype":
                            pparam += "'" + productType + "',";
                            break;
                        case "typeofgoods":
                            pparam += "'" + user.TypeOfGoods + "',";
                            break;
                        case "default" :
                            break;
                        default:
                            try
                            {
                                if (param.Substring(3, 1) != "." && param.Substring(2, 1) != "." && param.Substring(3, 1) != "-")
                                {
                                    var paramConvert = Convert.ToDateTime(param).ToString("yyyyMMdd");
                                    pparam += "'" + paramConvert + "',";
                                }
                                else {
                                    pparam += "'" + param + "',";
                                }
                            }
                            catch (Exception)
                            {
                                switch (param.Split(new string[] { "|" }, StringSplitOptions.None)[0].ToLower()){
                                    //case for movingCode with sparator comas data
                                    case "movingcode":
                                        pparam += "'" + param.Replace("movingCode", "").Remove(0,1).Replace("|",",") + "',";
                                        break;
                                    default:
                                        pparam += "'" + param + "',";
                                        break;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    pparam += "'" + param + "',";
                }
            }
            pparam = pparam.Substring(0, pparam.Length - 1);

            if(noCode){
                pparam = pparam.Replace("'nocode',", "");
            }
            return pparam;
        }
    }
}