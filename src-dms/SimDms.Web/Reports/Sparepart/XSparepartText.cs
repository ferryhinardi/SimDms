using SimDms.Common;
using SimDms.Common.Models;
using SimDms.Sparepart.Models;
using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class XSparepartText : IXText
    {
        #region Print Text Router
        SimDms.Web.Models.SysUser CurrentUser;
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

        public XSparepartText(string reportIDText, SimDms.Web.Models.SysUser usr, params object[] pparamText)
        {


            reportID = reportIDText;
            CurrentUser = usr;
            paramText = pparamText;
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
                    if (x[i].ToString() == "typeofgoods")
                        x[i] = CurrentUser.TypeOfGoods;

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
            string signName = "";
            string titleSign = "";
            string transType = "";
            string documentType = "";
            string documentFlagType = "";

            switch (reportID)
            {
                #region ** SpRpTrn001 : ordering-pso **

                case "SpRpTrn001":
                    procrpt = new txtSpRpTrn001();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, false, setTextParameter, paramReport);

                #endregion


                #region ** sprptrn002 : ordering-entryorder **

                case "sprptrn002srt":
                    procrpt = new txtSpRpTrn002();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, false, setTextParameter, paramReport);                

                #endregion

                #region ** SpRpTrn003 : receiving-edp **

                case "SpRpTrn003A":
                    procrpt = new txtSpRpTrn003();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, false, setTextParameter, paramReport);

                #endregion

                #region ** SpRpTrn004 : receiving-entrywrs **

                case "SpRpTrn004":
                    procrpt = new txtSpRpTrn004();
                    procrpt.CurrentUser = CurrentUser;
                    string wrsNo = pparam[2].ToString();
                    var recHeader = ctx.Database.SqlQuery<SpTrnPRcvHdr>(string.Format("select companycode,branchCode,wrsNo,transtype from spTrnPRcvHdr " +
                                                                        " where CompanyCode =  '{0}' " +
                                                                        " and BranchCode = '{1}' " +
                                                                        " and WRSNo='{2}'", CurrentUser.CompanyCode, CurrentUser.BranchCode, wrsNo)).ToList();
                    var recSignature = ctx.Database.SqlQuery<gnMstSignature>(string.Format("select CompanyCode,BranchCode,SignName,TitleSign from gnMstSignature " +
                                                                        " where CompanyCode =  '{0}' " +
                                                                        " and BranchCode =  '{1}' " +
                                                                        " and ProfitCenterCode='300' " +
                                                                        " and DocumentType='WRN' " +
                                                                        " and seqNo=1", CurrentUser.CompanyCode, CurrentUser.BranchCode)).ToList();

                    signName = (recSignature.Count > 0) ? recSignature[0].SignName.ToString() : string.Empty;
                    titleSign = (recSignature.Count > 0) ? recSignature[0].TitleSign.ToString() : string.Empty;
                    transType = (recHeader != null) ? recHeader[0].TransType.ToString() : string.Empty;

                    documentType = (recHeader != null) ? (recHeader[0].TransType.ToString() == "4" ? "WRL" : "WRN") : string.Empty;
                    documentFlagType = (recHeader != null) ? (recHeader[0].TransType.ToString() == "4" ? "WRLN" : "WRNN") : string.Empty;

                    SetTextParameter(signName, titleSign, transType);

                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, false, setTextParameter, paramReport);

                #endregion

                #region ** SpRpTrn005 : receiving-entryclaimsupplier **

                case "SpRpTrn005":
                    procrpt = new txtSpRpTrn005();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, false, setTextParameter, paramReport);

                #endregion

                #region ** SpRpTrn006 : inventory-inventoryAdjustment **

                case "SpRpTrn006":
                    procrpt = new txtSpRpTrn006();
                    procrpt.CurrentUser = CurrentUser;
                    var gnMstDocs = ctx.Database.SqlQuery<GnMstDocument>(string.Format("select CompanyCode,BranchCode,DocumentType " +
                                                                        "	from gnMstDocument " +
                                                                        "	where CompanyCode='{0}' " +
                                                                        "	and BranchCode='{1}' " +
                                                                        "	and DocumentType='ADJ' ", CurrentUser.CompanyCode, CurrentUser.BranchCode)).ToList();
                    string docType = gnMstDocs[0].DocumentType.ToString();
                    var ttd = ctx.Database.SqlQuery<SignatureLkp>(string.Format("SELECT SignName , TitleSign " +
                                                                        " FROM gnMstSignature  WHERE CompanyCode = '{0}' AND BranchCode = '{1}' " +
                                                                        " AND DocumentType = '{2}' " +
                                                                        " ORDER BY SeqNo ASC", CurrentUser.CompanyCode, CurrentUser.BranchCode, docType)).ToList();

                    var recCoProfiles = ctx.Database.SqlQuery<GnMstCoProfile>(string.Format("select companyCode,BranchCode,CityCode from gnMstCoProfile WHERE CompanyCode='{0}' and BranchCode='{1}'", CurrentUser.CompanyCode, CurrentUser.BranchCode)).ToList();
                    string cityCode = recCoProfiles[0].CityCode.ToString();

                    var recMstLkpDtls = ctx.Database.SqlQuery<Web.Models.LookUpDtl>(string.Format("SELECT companyCode,CodeId,LookUpValue,ParaValue,LookUpValueName FROM GnMstLookUpDtl WHERE CompanyCode='{0}' AND CodeID = 'CITY' AND LookUpValue = '{1}'", CurrentUser.CompanyCode, cityCode)).ToList();
                    string cityName = recMstLkpDtls[0].LookUpValueName.ToString();
                    string dateAndCity = string.Format("{0}, ", cityName.ToUpper());

                    signName = ttd[0].SignName.ToString();
                    titleSign = ttd[0].TitleSign.ToString();

                    SetTextParameter(dateAndCity, signName, titleSign);

                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, false, setTextParameter, paramReport);

                #endregion

                #region ** SpRpTrn009 : sales-entrypengeluaranstock nonsales(bps) **

                case "SpRpTrn007":
                    procrpt = new txtSpRpTrn007();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, false, setTextParameter, paramReport);

                #endregion

                #region ** SpRpTrn009 : sales-entrypengeluaranstock nonsales(bps) **

                case "SpRpTrn009":
                    procrpt = new txtSpRpTrn009();
                    procrpt.CurrentUser = CurrentUser;
                    SetTextParameter("SPAREPART");
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, false, setTextParameter, paramReport);

                #endregion

                #region ** SpRpTrn011 : sales-generateinnvoice(pembuatan faktur penjualan) **

                case "SpRpTrn011Short":
                    procrpt = new txtSpRpTrn011();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, false, setTextParameter, paramReport);

                case "SpRpTrn011Long":
                    procrpt = new txtSpRpTrn011();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, true, setTextParameter, paramReport);

                #endregion

                #region ** SpRpTrn012 : sales-entryreturnpenjualan, sales-entryreturnservice **

                case "SpRpTrn012":
                    procrpt = new txtSpRpTrn012();
                    procrpt.CurrentUser = CurrentUser;
                    SetTextParameter(reportID);
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, true, setTextParameter, paramReport);

                case "SpRpTrn012A":
                    procrpt = new txtSpRpTrn012();
                    procrpt.CurrentUser = CurrentUser;
                    SetTextParameter(reportID);
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, true, setTextParameter, paramReport);

                #endregion

                #region ** SpRpTrn013 : sales-entryreturnsuplyslip **

                case "SpRpTrn013A":
                    procrpt = new txtSpRpTrn013();
                    procrpt.CurrentUser = CurrentUser;
                    SetTextParameter(reportID);
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, true, setTextParameter, paramReport);

                #endregion

                #region ** SpRpTrn026 : receiving-entryhpp **

                case "SpRpTrn026":
                    procrpt = new txtSpRpTrn026();
                    procrpt.CurrentUser = CurrentUser;
                    string note = "HARGA SATUAN BELUM TERMASUK PPN";

                    SetTextParameter(note, "", true);
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, true, setTextParameter, paramReport);

                #endregion

                #region ** SpRpTrn027 : receiving-claimvendor **

                case "SpRpTrn027":
                    procrpt = new txtSpRpTrn027();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, true, setTextParameter, paramReport);

                #endregion

                #region ** SpRpTrn028 : sales-GenerateDocumentNonSales, Sales-LampDocumentService **

                case "SpRpTrn028":
                    procrpt = new txtSpRpTrn028();
                    procrpt.CurrentUser = CurrentUser;
                    string lmp = pparam[3].ToString();
                    var signature = ctx.Database.SqlQuery<SignatureLkp>(string.Format("select a.SignName,a.TitleSign " +
                                                                            " from gnMstSignature a " +
                                                                            "	inner join ( " +
                                                                            "		select x.CompanyCode,x.BranchCode,x.ProfitCenterCode,x.DocumentType " +
                                                                            "		from gnMstDocument x" +
                                                                            "			inner join spTrnSLmpHdr y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode " +
                                                                            "				and x.ProfitCenterCode='300' and x.DocumentPrefix=substring(y.LmpNo,1,3) " +
                                                                            "	where x.CompanyCode='{0}' " +
                                                                            "			and x.BranchCode='{1}' " +
                                                                            "			and y.LmpNo='{2}' " +
                                                                            "	) b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode  " +
                                                                            "		and a.ProfitCenterCode=b.ProfitCenterCode and a.DocumentType=b.DocumentType " +
                                                                            "order by a.SeqNo asc", CurrentUser.CompanyCode, CurrentUser.BranchCode, lmp)).ToList();
                    signName = signature[0].SignName.ToString();
                    titleSign = signature[0].TitleSign.ToString();

                    SetTextParameter(signName, titleSign, true);
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, true, setTextParameter, paramReport);

                case "SpRpTrn028A":
                    procrpt = new txtSpRpTrn028();
                    procrpt.CurrentUser = CurrentUser;
                    string lmp28A = pparam[3].ToString();
                    var signature28A = ctx.Database.SqlQuery<SignatureLkp>(string.Format("select a.SignName,a.TitleSign " +
                                                                            " from gnMstSignature a " +
                                                                            "	inner join ( " +
                                                                            "		select x.CompanyCode,x.BranchCode,x.ProfitCenterCode,x.DocumentType " +
                                                                            "		from gnMstDocument x" +
                                                                            "			inner join spTrnSLmpHdr y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode " +
                                                                            "				and x.ProfitCenterCode='300' and x.DocumentPrefix=substring(y.LmpNo,1,3) " +
                                                                            "	where x.CompanyCode='{0}' " +
                                                                            "			and x.BranchCode='{1}' " +
                                                                            "			and y.LmpNo='{2}' " +
                                                                            "	) b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode  " +
                                                                            "		and a.ProfitCenterCode=b.ProfitCenterCode and a.DocumentType=b.DocumentType " +
                                                                            "order by a.SeqNo asc", CurrentUser.CompanyCode, CurrentUser.BranchCode, lmp28A)).ToList();
                    signName = signature28A[0].SignName.ToString();
                    titleSign = signature28A[0].TitleSign.ToString();

                    SetTextParameter(signName, titleSign, true);
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, true, setTextParameter, paramReport);

                #endregion    

                #region ** SpRpTrn027 : receiving-claimvendor **

                case "SpRpTrn030":
                    procrpt = new txtSpRpTrn030();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, true, setTextParameter, paramReport);

                #endregion

                #region ** SpRpTrn031 : sales-entrypengeluaranstock sales-sales(pordd) **

                case "SpRpTrn031":
                    procrpt = new txtSpRpTrn031();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, true, setTextParameter, paramReport);
                    
                #endregion

                #region ** SpRpTrn033 : sales-PLandEntryPickedQtyPL, Sales-LampDocumentService **

                case "SpRpTrn033":
                    procrpt = new txtSpRpTrn033();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, false, setTextParameter, paramReport);

                case "SpRpTrn033Long":
                    procrpt = new txtSpRpTrn033();
                    procrpt.CurrentUser = CurrentUser;
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, false, setTextParameter, paramReport);

                #endregion

                #region ** SpRpTrn039 : Sales-LampDocumentService, sales-entrypengeluaranstock service-unitorder **

                case "SpRpTrn039":
                    procrpt = new txtSpRpTrn039();
                    procrpt.CurrentUser = CurrentUser;
                    SetTextParameter("SERVICE");
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, true, setTextParameter, paramReport);

                case "SpRpTrn039A":
                    procrpt = new txtSpRpTrn039();
                    procrpt.CurrentUser = CurrentUser;
                    SetTextParameter("SERVICE");
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, true, setTextParameter, paramReport);

                case "SpRpTrn039U":
                    procrpt = new txtSpRpTrn039();
                    procrpt.CurrentUser = CurrentUser;
                    SetTextParameter("UNIT");
                    return procrpt.CreateReport(reportID, sproc, sparam.Replace("profitcenter", "300"), printerLoc, print, true, setTextParameter, paramReport);

                #endregion

                default:
                    throw new Exception("Report text file belum tersedia");

            }
        }

        #endregion
    }
}