using SimDms.Common;
using SimDms.Service.Models.Reports;
using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Service
{

    public class XServiceText : IXText
    {

        SysUser CurrentUser;
        #region Print Text Router
        
        private object[] setTextParameter, paramText;

        private string rparam = "";
        private string paramReport = "", printerLoc = "", fileLocation = "", reportID = "";
        private bool print = true, fullPage = true;

        private SimDms.Service.Models.DataContext ctx = new SimDms.Service.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        // private DataSet dsSource = null;
        //private DataTable dt = null;

        public void SetTextParameter(params object[] textParam)
        {
            setTextParameter = textParam;
        }

        public void SetDefaultParameter(string paramReportText, string printerLocText, string fileLocationText, bool printText, bool fullPageText, string srparam)
        {
            paramReport = paramReportText;
            printerLoc = printerLocText;
            fileLocation = fileLocationText;
            print = printText;
            fullPage = fullPageText;
            rparam = srparam;
        }

        public XServiceText(string reportIDText, SysUser usr, params object[] pparamText)
        {
            reportID = reportIDText;
            CurrentUser = usr;
            paramText = pparamText;
        }

        public XServiceText(string reportIDText)//, DataSet ds)
        {
            //dsSource = ds;
            reportID = reportIDText;
        }

        public string Print()
        {
            return PrintTextFile(reportID, paramReport, printerLoc, print, fileLocation, fullPage, paramText);
        }

        #endregion

        #region Print Text File


        private string PrintTextFile(string reportID, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] pparam)
        {
            //DataTable dtReport = SysReportBLL.GetReportByReportID(reportID);

            string productType = ctx.Database.SqlQuery<string>(string.Format("select ProductType from gnMstCoProfile where CompanyCode={0} and BranchCode={1}", CurrentUser.CompanyCode, CurrentUser.BranchCode)).FirstOrDefault();

            Func<object[], string> stringparam = x =>
            {
                string s = "";
                for (int i = 0; i < x.Length; i++)
                {
                    if (i > 0) s += ",";
                    if (x[i].ToString() == "producttype")
                        x[i] = productType;

                    s += "'" + x[i].ToString() + "'";
                    
                }
                //s = "'";

                return s;
            };

            string sproc = ctx.Database.SqlQuery<string>(string.Format("select ReportProc from sysreport where reportid='{0}'", reportID)).FirstOrDefault();
            if (string.IsNullOrEmpty(sproc))
                throw new Exception("Report ID Not Found");

            string sparam = stringparam(pparam);

            string stitle = "";


            IRptProc procrpt;
            if (reportID.Substring(0, 10) == "SvRpReport")
            {
                procrpt = new txtSvRpReport();
                procrpt.CurrentUser = CurrentUser;
                stitle = ctx.Database.SqlQuery<string>(string.Format("select TitleSign from gnMstSignature where  CompanyCode = {0} and BranchCode = {1} and SignName='{2}' and DocumentType like 'IN%'", CurrentUser.CompanyCode, CurrentUser.BranchCode, rparam)).FirstOrDefault();
                SetTextParameter(rparam, stitle ?? "", false);
                return procrpt.CreateReport(reportID, sproc, sparam, printerLoc, print, fullPage, setTextParameter, paramReport);
            }
            else
            {
                switch (reportID)
                {
                    #region SPK SvRpTrn001

                    case "SvRpTrn001":
                        procrpt = new txtSvRpTrn001();
                        procrpt.CurrentUser = CurrentUser;
                        return procrpt.CreateReport(reportID, sproc, sparam.Substring(0, sparam.Length - 3) + "1", printerLoc, print, fullPage, setTextParameter, paramReport);
                    //break;
                    case "SvRpTrn001PrePrinted":
                        procrpt = new txtSvRpTrn001PrePrinted();
                        procrpt.CurrentUser = CurrentUser;
                        return procrpt.CreateReport(reportID, sproc, sparam.Substring(0, sparam.Length - 3) + "1", printerLoc, print, fullPage, setTextParameter, paramReport);
                    case "SvRpTrn00101":
                        procrpt = new txtSvRpTrn00101();
                        procrpt.CurrentUser = CurrentUser;
                        return procrpt.CreateReport(reportID, sproc, sparam, printerLoc, print, fullPage, setTextParameter, rparam );
                    //break;

                    #endregion

                    #region SvRpTrn002
                    case "SvRpTrn002":
                        //dt = GnMstPrinterHdrBLL.GetDataTableForTextFile(dtReport.Rows[0]["ReportProc"].ToString(), pparam);
                        //if (dt.Rows.Count != 0)
                        //    CreateReportSvRpTrn002(reportID, dt, paramReport, printerLoc, print, fileLocation);
                        //else
                        //    XMessageBox.ShowInformation("Data Tidak Tersedia");
                        procrpt = new txtSvRpTrn002();
                        procrpt.CurrentUser = CurrentUser;
                        stitle = ctx.Database.SqlQuery<string>(string.Format("select TitleSign from gnMstSignature where  CompanyCode = {0} and BranchCode = {1} and SignName='{2}' and DocumentType like 'IN%'", CurrentUser.CompanyCode, CurrentUser.BranchCode, rparam)).FirstOrDefault();
                        SetTextParameter(rparam, stitle ?? "", false);
                        return procrpt.CreateReport(reportID, sproc, sparam, printerLoc, print, fullPage, setTextParameter, paramReport);
                    #endregion

                    #region Invoice SvRpTrn004

                    case "SvRpTrn004":
                        procrpt = new txtSvRpTrn004();
                        procrpt.CurrentUser = CurrentUser;
                        stitle = ctx.Database.SqlQuery<string>(string.Format("select TitleSign from gnMstSignature where  CompanyCode = {0} and BranchCode = {1} and SignName='{2}' and DocumentType like 'IN%'", CurrentUser.CompanyCode, CurrentUser.BranchCode, rparam)).FirstOrDefault();
                        var prm = rparam.Split(',');
                        SetTextParameter(prm[0], prm[1], false);
                        return procrpt.CreateReport(reportID, sproc, sparam, printerLoc, print, fullPage, setTextParameter, paramReport);

                    case "SvRpTrn004H":
                        var dtih = ctx.Database.SqlQuery<SvRpTrn004>(string.Format("exec {0} {1}", sproc, sparam));
                        SetTextParameter(rparam, "KEPALA CABANG", true);
                        //if (dtih.Rows.Count != 0) { }
                        ////CreateReportSvRpTrn004(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage);
                        //else
                        //    return ("Data Tidak Tersedia");
                        break;
                    case "SvRpTrn004001":
                        //dt = GnMstPrinterHdrBLL.GetDataTableForTextFile(dtReport.Rows[0]["ReportProc"].ToString(), pparam);
                        //if (dt.Rows.Count != 0)
                        //    CreateReportSvRpTrn004001(reportID, dt, paramReport, printerLoc, print, fileLocation);
                        //else
                        //    XMessageBox.ShowInformation("Data Tidak Tersedia");
                        procrpt = new txtSvRpTrn004();
                        procrpt.CurrentUser = CurrentUser;
                        stitle = ctx.Database.SqlQuery<string>(string.Format("select TitleSign from gnMstSignature where  CompanyCode = {0} and BranchCode = {1} and SignName='{2}' and DocumentType like 'IN%'", CurrentUser.CompanyCode, CurrentUser.BranchCode, rparam)).FirstOrDefault();
                        SetTextParameter(rparam, stitle ?? "", false);
                        return procrpt.CreateReport(reportID, sproc, sparam, printerLoc, print, fullPage, setTextParameter, paramReport);
                    case "SvRpTrn004001H":
                        //dt = GnMstPrinterHdrBLL.GetDataTableForTextFile(dtReport.Rows[0]["ReportProc"].ToString(), pparam);
                        //if (dt.Rows.Count != 0)
                        //    CreateReportSvRpTrn004001(reportID, dt, paramReport, printerLoc, print, fileLocation);
                        //else
                        //    XMessageBox.ShowInformation("Data Tidak Tersedia");
                        procrpt = new txtSvRpTrn004();
                        procrpt.CurrentUser = CurrentUser;
                        stitle = ctx.Database.SqlQuery<string>(string.Format("select TitleSign from gnMstSignature where  CompanyCode = {0} and BranchCode = {1} and SignName='{2}' and DocumentType like 'IN%'", CurrentUser.CompanyCode, CurrentUser.BranchCode, rparam)).FirstOrDefault();
                        SetTextParameter(rparam, stitle ?? "", false);
                        return procrpt.CreateReport(reportID, sproc, sparam, printerLoc, print, false, setTextParameter, paramReport);
                    case "SvRpTrn004002H":
                        //dt = GnMstPrinterHdrBLL.GetDataTableForTextFile(dtReport.Rows[0]["ReportProc"].ToString(), pparam);
                        //if (dt.Rows.Count != 0)
                        //    CreateReportSvRpTrn004002(reportID, dt, paramReport, printerLoc, print, fileLocation);
                        //else
                        //    XMessageBox.ShowInformation("Data Tidak Tersedia");
                        procrpt = new txtSvRpTrn004();
                        procrpt.CurrentUser = CurrentUser;
                        stitle = ctx.Database.SqlQuery<string>(string.Format("select TitleSign from gnMstSignature where  CompanyCode = {0} and BranchCode = {1} and SignName='{2}' and DocumentType like 'IN%'", CurrentUser.CompanyCode, CurrentUser.BranchCode, rparam)).FirstOrDefault();
                        SetTextParameter(rparam, stitle ?? "", false);
                        return procrpt.CreateReport(reportID, sproc, sparam, printerLoc, print, false, setTextParameter, paramReport);


                    case "SvRpTrn004002":
                        //dt = GnMstPrinterHdrBLL.GetDataTableForTextFile(dtReport.Rows[0]["ReportProc"].ToString(), pparam);
                        //if (dt.Rows.Count != 0)
                        //    CreateReportSvRpTrn004002(reportID, dt, paramReport, printerLoc, print, fileLocation);
                        //else
                        //    XMessageBox.ShowInformation("Data Tidak Tersedia");
                        procrpt = new txtSvRpTrn004();
                        procrpt.CurrentUser = CurrentUser;
                        stitle = ctx.Database.SqlQuery<string>(string.Format("select TitleSign from gnMstSignature where  CompanyCode = {0} and BranchCode = {1} and SignName='{2}' and DocumentType like 'IN%'", CurrentUser.CompanyCode, CurrentUser.BranchCode, rparam)).FirstOrDefault();
                        SetTextParameter(rparam, stitle ?? "", false);
                        return procrpt.CreateReport(reportID, sproc, sparam, printerLoc, print, fullPage, setTextParameter, paramReport);

                    case "SvRpTrn004AGJ":
                        //dt = GnMstPrinterHdrBLL.GetDataTableForTextFile(dtReport.Rows[0]["ReportProc"].ToString(), pparam);
                        //if (dt.Rows.Count != 0)
                        //    CreateReportSvRpTrn004(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage);
                        //else
                        //    XMessageBox.ShowInformation("Data Tidak Tersedia");
                        procrpt = new txtSvRpTrn004();
                        procrpt.CurrentUser = CurrentUser;
                        stitle = ctx.Database.SqlQuery<string>(string.Format("select TitleSign from gnMstSignature where  CompanyCode = {0} and BranchCode = {1} and SignName='{2}' and DocumentType like 'IN%'", CurrentUser.CompanyCode, CurrentUser.BranchCode, rparam)).FirstOrDefault();
                        SetTextParameter(rparam, stitle ?? "", false);
                        return procrpt.CreateReport(reportID, sproc, sparam, printerLoc, print, fullPage, setTextParameter, paramReport);
                    #endregion

                    #region SvRpTrn010
                    case "SvRpTrn010":
                        //if (dsSource.Tables[0].Rows.Count != 0)
                        //    CreateReportSvRpTrn010(reportID, dsSource, paramReport, printerLoc, print, fileLocation);
                        //else
                        //    XMessageBox.ShowInformation("Data Tidak Tersedia");
                        procrpt = new txtSvRpTrn010();
                        procrpt.CurrentUser = CurrentUser;
                        stitle = ctx.Database.SqlQuery<string>(string.Format("select TitleSign from gnMstSignature where  CompanyCode = {0} and BranchCode = {1} and SignName='{2}' and DocumentType like 'IN%'", CurrentUser.CompanyCode, CurrentUser.BranchCode, rparam)).FirstOrDefault();
                        SetTextParameter(rparam, stitle ?? "", false);
                        return procrpt.CreateReport(reportID, sproc, sparam, printerLoc, print, fullPage, setTextParameter, paramReport);
                    #endregion

                    #region SvRpTrn018
                    case "SvRpTrn018":
                        //dt = GnMstPrinterHdrBLL.GetDataTableForTextFile(dtReport.Rows[0]["ReportProc"].ToString(), pparam);
                        //if (dt.Rows.Count != 0)
                        //    CreateReportSvRpTrn018(reportID, dt, paramReport, printerLoc, print, fileLocation);
                        //else
                        //    XMessageBox.ShowInformation("Data Tidak Tersedia");
                        procrpt = new txtSvRpTrn018();
                        procrpt.CurrentUser = CurrentUser;
                        stitle = ctx.Database.SqlQuery<string>(string.Format("select TitleSign from gnMstSignature where  CompanyCode = {0} and BranchCode = {1} and SignName='{2}' and DocumentType like 'IN%'", CurrentUser.CompanyCode, CurrentUser.BranchCode, rparam)).FirstOrDefault();
                        SetTextParameter(rparam, stitle ?? "", false);
                        return procrpt.CreateReport(reportID, sproc, sparam, printerLoc, print, fullPage, setTextParameter, paramReport);
                    #endregion

                    default:
                        return "Report ID " + reportID + "Tidak Tersedia ";//XMessageBox.ShowWarning("Report text file belum tersedia")
                    //break;
                }
            }
            return "";
        }




       
        #endregion


       
    }
}