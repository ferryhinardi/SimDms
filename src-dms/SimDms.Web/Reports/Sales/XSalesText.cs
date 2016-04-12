using SimDms.Common;
using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sales
{
    public class XSalesText : IXText
    {
        #region Print Text Router
        SysUser CurrentUser;
        private string rparam = "";


        private object[] setTextParameter, paramText;
        private string paramReport = "", printerLoc = "", fileLocation = "", reportID = "";
        private bool print = true, fullPage = true;

        private SimDms.Service.Models.DataContext ctx = new SimDms.Service.Models.DataContext(MyHelpers.GetConnString("DataContext"));

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


        public XSalesText(string reportIDText, SysUser usr, params object[] pparamText)
        {


            reportID = reportIDText;
            CurrentUser = usr;
            paramText = pparamText;
        }

        public string Print()
        {
            return PrintTextFile(reportID, paramReport, printerLoc, print, fileLocation, fullPage, paramText, setTextParameter);
        }

        #endregion


        private string PrintTextFile(string reportID, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] pparam, object[] textParam)
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
            IRptProc procrpt;
            var vals = ctx.CoProfiles.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode);
            string prodType = vals.ProductType.ToString();

            switch (reportID)
            {
                #region OmRpSalesTrn...
                #region SO (OmRpSalesTrn001/OmRpSalesTrn001A/OmRpSalesTrn001B/OmRpSalesTrn001C/OmRpSalesTrn001D/OmRpSalesTrn001E/OmRpSalesTrn001F/OmRpSalesTrn001G)
                case "OmRpSalesTrn001":
                    procrpt = new txtOmRpSalesTrn001();//yan
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpSalesTrn001A":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn0001();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpSalesTrn001B":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn0001();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpSalesTrn001C":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn0001();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpSalesTrn001D":
                    procrpt = new txtOmRpSalesTrn001();//yan
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, false, setTextParameter, paramReport);
                case "OmRpSalesTrn001E":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn0001();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpSalesTrn001F":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn0001();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpSalesTrn001G":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn0001();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #region DO (OmRpSalesTrn002/OmRpSalesTrn002A)
                case "OmRpSalesTrn002":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W80");
                    procrpt = new txtOmRpSalesTrn002();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);   
                    break;
                case "OmRpSalesTrn002A":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W80");
                    procrpt = new txtOmRpSalesTrn002();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);   
                    break;
                #endregion

                #region ** BPK (OmRpSalesTrn003A/OmRpSalesTrn003B/OmRpSalesTrn003C/OmRpSalesTrn003D) **
                case "OmRpSalesTrn003A":
                    procrpt = new txtOmRpSalesTrn003();
                    procrpt.CurrentUser = CurrentUser;
                    SetTextParameter("", "", prodType, true);
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);

                case "OmRpSalesTrn003B":
                    procrpt = new txtOmRpSalesTrn003();
                    procrpt.CurrentUser = CurrentUser;
                    SetTextParameter("", "", prodType, true);
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpSalesTrn003C":
                    procrpt = new txtOmRpSalesTrn003();
                    procrpt.CurrentUser = CurrentUser;
                    SetTextParameter("", "", prodType, true);
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpSalesTrn003D":
                    procrpt = new txtOmRpSalesTrn003();
                    procrpt.CurrentUser = CurrentUser;
                    SetTextParameter("", "", prodType, true);
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #region Invoice / Nota Debet (OmRpSalesTrn004/OmRpSalesTrn004A/OmRpSalesTrn009/OmRpSalesTrn009A)
                case "OmRpSalesTrn004":
                    procrpt = new txtOmRpSalesTrn004();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);

                case "OmRpSalesTrn004A":
                    procrpt = new txtOmRpSalesTrn004();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpSalesTrn009":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn009();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);   
                    break;
                case "OmRpSalesTrn009A":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn009();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);   
                    break;
                #endregion

                #region Return (OmRpSalesTrn005/OmRpSalesTrn005A)
                case "OmRpSalesTrn005":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn005();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);   
                    break;
                case "OmRpSalesTrn005A":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn005();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);   
                    break;
                #endregion

                #region SPK & Tracking BBN (OmRpSalesTrn006/OmRpSalesTrn006A)
                case "OmRpSalesTrn006":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn006();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);   
                    break;
                case "OmRpSalesTrn006A":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn006();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);   
                    break;
                #endregion

                #region Faktur Polisi (OmRpSalesTrn007DNew/OmRpSalesTrn007/OmRpSalesTrn007C/OmRpSalesTrn007A/OmRpSalesTrn007B)
                case "OmRpSalesTrn007DNewWeb":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn007();
                    procrpt.CurrentUser = CurrentUser;
                    if(setTextParameter==null)
                    {
                        setTextParameter = rparam.Split(',');
                    }
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);   
                    break;
                case "OmRpSalesTrn007":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn007();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);   
                    break;
                case "OmRpSalesTrn007C":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                   procrpt = new txtOmRpSalesTrn007();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);   
                    break;
                case "OmRpSalesTrn007A":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn007();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);   
                    break;
                case "OmRpSalesTrn007B":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn007();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);   
                    break;
                #endregion

                #region Perlengkapan Out (OmRpSalesTrn008)
                case "OmRpSalesTrn008":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn008();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Tanda Terima BPKB (OmRpSalesTrn010/OmRpSalesTrn010A)
                case "OmRpSalesTrn010":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn010();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                case "OmRpSalesTrn010A":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalesTrn010();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion
                #endregion

                #region OmRpSalRgs...

                #region Register Sales Order (OmRpSalesRgs001/OmRpSalesRgs001A)
                case "OmRpSalesRgs001":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpSalRgs001(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                case "OmRpSalesRgs001A":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpSalRgs001(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Register Delivery Order (OmRpSalRgs002/OmRpSalRgs002HQ)
                case "OmRpSalRgs002":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W136");
                     procrpt = new txtOmRpSalRgs002(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                case "OmRpSalRgs002HQ":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W136");
                     procrpt = new txtOmRpSalRgs002(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Register Bukti Pengiriman Kendaraan (OmRpSalesRgs003/OmRpSalesRgs003A)
                case "OmRpSalesRgs003":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                     procrpt = new txtOmRpSalRgs003(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                case "OmRpSalesRgs003A":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                     procrpt = new txtOmRpSalRgs003(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Register Faktur Penjualan Unit (OmRpSalRgs004/OmRpSalRgs004HQ)
                case "OmRpSalRgs004":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                    procrpt = new txtOmRpSalRgs004(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                case "OmRpSalRgs004HQ":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                    procrpt = new txtOmRpSalRgs004(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Register Retur Invoice (OmRpSalRgs005/OmRpSalRgs005HQ)
                case "OmRpSalRgs005":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                    procrpt = new txtOmRpSalRgs005(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Register Pembatalan Sales Order (OmRpSalesRgs006)
                case "OmRpSalesRgs006":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W136");
                     procrpt = new txtOmRpSalRgs006(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Register Nota Debet (OmRpSalRgs007/OmRpSalRgs007HQ)
                case "OmRpSalRgs007":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                     procrpt = new txtOmRpSalRgs007(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                case "OmRpSalRgs007HQ":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                     procrpt = new txtOmRpSalRgs007(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Register Pelanggan Perleasing (OmRpSalRgs008)
                case "OmRpSalRgs008":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpSalRgs008(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Daftar Rincian Kendaraan Bermotor (OmRpSalRgs009)
                case "OmRpSalRgs009":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                     procrpt = new txtOmRpSalRgs009(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Laporan Penjualan Terbaik (OmRpSalRgs010/OmRpSalRgs011/OmRpSalRgs012)
                case "OmRpSalRgs010":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W80");
                     procrpt = new txtOmRpSalRgs010(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                case "OmRpSalRgs011":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W80");
                     procrpt = new txtOmRpSalRgs010(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                case "OmRpSalRgs012":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W80");
                     procrpt = new txtOmRpSalRgs010(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Faktur-faktur yang Sudah Digenerate (OmRpSalRgs013)
                case "OmRpSalRgs013":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                     procrpt = new txtOmRpSalRgs013(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Summary Permohonan Faktur Polisi (OmRpSalRgs014)
                case "OmRpSalRgs014":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W80");
                     procrpt = new txtOmRpSalRgs014(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Register Faktur Polisi (OmRpSalRgs015)
                case "OmRpSalRgs015":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                     procrpt = new txtOmRpSalRgs015(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Register Harian Penerbitan Faktur Polisi (OmRpSalRgs017)
                case "OmRpSalRgs017":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                     procrpt = new txtOmRpSalRgs017(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Outstanding Blanko Faktur Polisi (OmRpSalRgs018/OmRpSalRgs019)
                case "OmRpSalRgs018":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                     procrpt = new txtOmRpSalRgs018(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                case "OmRpSalRgs019":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalRgs018(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                    break;
                #endregion

                #region Penggunaan Blanko (OmRpSalRgs025)
                case "OmRpSalRgs025":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                     procrpt = new txtOmRpSalRgs025(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Rekapitulasi Harian (OmRpSalRgs026)
                case "OmRpSalRgs026":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                     procrpt = new txtOmRpSalRgs026(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Tanda Terima BPKB (OmRpSalRgs028)
                case "OmRpSalRgs028":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpSalRgs028(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Daftar BPKB (OmRpSalRgs029)
                case "OmRpSalRgs029":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                     procrpt = new txtOmRpSalRgs029(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Daftar BPKB per Lokasi (OmRpSalRgs030A/OmRpSalRgs030B)
                case "OmRpSalRgs030A":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                     procrpt = new txtOmRpSalRgs030(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                case "OmRpSalRgs030B":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                     procrpt = new txtOmRpSalRgs030(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Outstanding STNK per Biro Jasa (OmRpSalRgs031)
                case "OmRpSalRgs031":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W136");
                     procrpt = new txtOmRpSalRgs031(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #region Tanda Terima Faktur / Pengajuan Pengurusan BBN (OmRpSalRgs032)
                case "OmRpSalRgs032":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                     procrpt = new txtOmRpSalRgs032(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                    break;
                #endregion

                #endregion

                #region Sales Achievement Record (OmRpSalAch)
                case "OmRpSalAch":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpSalAch();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion                
                #region next

                #region Stock Taking (OmRpStock..)
                case "OmRpStock001": //txtOmRpStock001
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpStock001(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpStock002":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpStock002(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpStock003":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                    procrpt = new txtOmRpStock002(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #region Inventory (OmRpInvRgs.../ OmRpInventTrn...)

                #region OmRpInvRgs...
                #region Perincian Stok (OmRpInvRgs001/OmRpInvRgs001A/OmRpInvRgs002/OmRpInvRgs002A)
                case "OmRpInvRgs001":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpInvRgs001(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpInvRgs001A":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpInvRgs001(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpInvRgs002":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpInvRgs001(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpInvRgs002A":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpInvRgs001(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #region Stok Inventory (OmRpInvRgs003/OmRpInvRgs003A/OmRpInvRgs003B/OmRpInvRgs003C/OmRpInvRgs003D/OmRpInvRgs003E/OmRpInvRgs003F/OmRpInvRgs003G)
                case "OmRpInvRgs003":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                    procrpt = new txtOmRpInvRgs003(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpInvRgs003A":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                    procrpt = new txtOmRpInvRgs003(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpInvRgs003B":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                    procrpt = new txtOmRpInvRgs003(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpInvRgs003C":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                    procrpt = new txtOmRpInvRgs003(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpInvRgs003D":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                    procrpt = new txtOmRpInvRgs003(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpInvRgs003E":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpInvRgs003(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpInvRgs003F":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpInvRgs003(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpInvRgs003G":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                    procrpt = new txtOmRpInvRgs003(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #region Stok Kendaraan (OmRpInvRgs004B/OmRpInvRgs004)
                case "OmRpInvRgs004B":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                    procrpt = new txtOmRpInvRgs004(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpInvRgs004":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                    procrpt = new txtOmRpInvRgs004(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #region Transfer In (OmRpInvRgs005)
                case "OmRpInvRgs005":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpInvRgs005(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #region Transfer In COGS (OmRpInvRgs005COGS)
                case "OmRpInvRgs005COGS":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpInvRgs005COGS(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #region Transfer Out (OmRpInvRgs006)
                case "OmRpInvRgs006":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpInvRgs005(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #region Transfer Out COGS (OmRpInvRgs006COGS)
                case "OmRpInvRgs006COGS":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpInvRgs005COGS(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #region Ganti Warna (OmRpInvRgs007)
                case "OmRpInvRgs007":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W163");
                    procrpt = new txtOmRpInvRgs007(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #endregion

                #region OmRpInventTrn...

                #region Transfer Out (OmRpInventTrn001)
                case "OmRpInventTrn001":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpInventTrn001(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpInventTrn002":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpInventTrn001(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpInventTrn003":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpInventTrn003(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #endregion

                #endregion

                #region Dokumen Pending (OmRpDocPending)
                case "OmRpDocPending":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpDocPending(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #region Report Statistik Salesman (OmRpStatSal/OmRpStatSalA/OmRpStatSalB/OmRpStatSalC)
                case "OmRpStatSal":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1400, 1100, "W233");
                    procrpt = new txtOmRpStatSal(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpStatSalA":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpStatSal(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpStatSalB":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpStatSal(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpStatSalC":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W233");
                    procrpt = new txtOmRpStatSal(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #region Report Laba Rugi (OmRpLabaRugi001/OmRpLabaRugi002/OmRpLabaRugi003)
                case "OmRpLabaRugi001":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1400, 1100, "W233");
                    procrpt = new txtOmRpLabaRugi001(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpLabaRugi002":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W96");
                    procrpt = new txtOmRpLabaRugi002(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                case "OmRpLabaRugi003":
                    //CreateReport(reportID, dt, paramReport, printerLoc, print, fileLocation, fullPage, textParam, 1100, 850, "W136");
                    procrpt = new txtOmRpLabaRugi003(); 
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "100"), printerLoc, print, true, setTextParameter, paramReport);
                #endregion

                #endregion
                default:
                    throw new Exception("Report text file belum tersedia");

            }
            return "";
        }

    }

}