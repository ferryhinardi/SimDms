using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.Reporting.WebForms;
using SimDms.Sales.Models;
using SimDms.Sales.Models.Result;
using SimDms.Common;
using SimDms.Common.Models;
using System.Text;
using System.Data.SqlClient;
using System.ComponentModel;

namespace SimDms.Sales.Controllers
{
    public class BaseController : Controller
    {
        protected DataContext ctx;
        protected MDContext ctxMD; 

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ctx = new DataContext(MyHelpers.GetConnString("DataContext"));
            ctxMD = new MDContext(MyHelpers.GetConnString("MDContext"));
        }

        protected string HtmlRender(string jsname)
        {
            return string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/om/"), jsname);
        }

        protected string HtmlRender(string id, string jsname)
        {
            var jshtml = "";
            if (!string.IsNullOrWhiteSpace(jsname))
            {
                jshtml = string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/om/"), jsname);
            }
            return string.Format(@"<div id=""{0}"" ></div>", id) + jshtml;
        }

        protected ResultModel InitializeResult()
        {
            return new ResultModel()
            {
                status = false,
                message = "",
                details = "",
                data = null
            };
        }

        protected List<Dictionary<string, object>> GetJson(DataTable dt)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row = null;

            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName.Trim(), dr[col]);
                }
                rows.Add(row);
            }
            return rows;
        }

        protected SysUser CurrentUser
        {
            get
            {
                return ctx.SysUsers.Find(User.Identity.Name);
            }
        }

        protected string getCompanySD(string NoPlg)
        {
            string CompanySD = ctx.Database.SqlQuery<string>("SELECT CompanyCode FROM gnmstcompanymapping WHERE BranchCode='" + NoPlg + "'").FirstOrDefault();
            return CompanySD;
        }

        protected string getDbSD(string CCode, string BCode)
        {
            string DbSD = ctx.Database.SqlQuery<string>("SELECT DbName FROM gnmstcompanymapping WHERE CompanyCode='" + CCode + "' AND BranchCode='" + BCode + "'").FirstOrDefault();
            return DbSD;
        }

        protected string CompanyCode
        {
            get
            {
                return CurrentUser.CompanyCode;
            }
        }

        protected string CompanyCodeMD  
        {
            get
            {
                var rd = ctx.CompanyMappings.Where(p => p.CompanyCode== CompanyCode && p.BranchCode == BranchCode).FirstOrDefault();
                if (rd == null)
                {
                    return CompanyCode;
                }
                return rd.CompanyMD;
            }
        }

        protected string CompanyName
        {
            get
            {
                return ctx.OrganizationHdrs.Find(CurrentUser.CompanyCode).CompanyName;
            }
        }

        protected string CompanyNameMD   
        {
            get
            {
                var rd = ctx.CompanyMappings.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).FirstOrDefault();
                if (rd == null)
                {
                    return CompanyName;
                }
                return rd.BranchNameMD;
            }
        }

        protected string BranchCode
        {
            get
            {
                return CurrentUser.BranchCode;
            }
        }

        protected string BranchCodeMD  
        {
            get
            {
                var rd = ctx.CompanyMappings.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).FirstOrDefault();
                if (rd == null)
                {
                    return BranchCode;
                }
                return rd.BranchMD;
            }
        }

        protected string UnitBranchCodeMD  // khusus unit 
        {
            get
            {
                var rd = ctx.CompanyMappings.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).FirstOrDefault();
                if (rd == null)
                {
                    return BranchCode;
                }
                return rd.UnitBranchMD;
            }
        }

        protected string NationalSLS
        {
            get
            {
                return ctx.LookUpDtls.Find(CurrentUser.CompanyCode, "QSLS", "NATIONAL").ParaValue;
            }
        }

        protected string BranchName
        {
            get
            {
                return ctx.CoProfiles.Find(CompanyCode, BranchCode).CompanyName;
            }
        }

        protected bool IsBranch
        {
            get
            {
                return ctx.OrganizationDtls.Find(CompanyCode, BranchCode).IsBranch;
            }
        }

        protected string SALESMAN
        {
            get
            {
                return "10";
            }
        }

        protected string SALES_COORDINATOR
        {
            get
            {
                return "20";
            }
        }

        protected string SALES_HEAD
        {
            get
            {
                return "30";
            }
        }

        protected string BRANCH_MANAGER
        {
            get
            {
                return "40";
            }
        }

        protected string COO
        {
            get
            {
                return "50";
            }
        }

        protected string SALES_ADMIN
        {
            get
            {
                return "60";
            }
        }

        protected void RenderReport(string reportPath, string fileName, double reportWidth, double reportHeight, string reportType, params IEnumerable<object>[] dataSources)
        {
            LocalReport report = new LocalReport()
            {
                ReportPath = reportPath
            };
            report.DataSources.Clear();
            //report.DataSources.Add(new ReportDataSource("MonthlyActivities", dataSource.ToArray()));

            string dataSourcePrefix = "ds";
            int iterator = 0;
            foreach (var item in dataSources)
            {
                report.DataSources.Add(new ReportDataSource((dataSourcePrefix + "_" + iterator), item.ToArray()));
                iterator++;
            }

            string mimeType;
            string encoding;
            string fileNameExtension;

            if (reportHeight == 0)
            {
                string strReportHeight = ConfigurationManager.AppSettings["defaultReportHeight"] ?? "8.3";
                reportHeight = Convert.ToInt32(strReportHeight);
            }

            if (reportWidth == 0)
            {
                string strReportWidth = ConfigurationManager.AppSettings["defaultReportWidth"] ?? "11.7";
                reportWidth = Convert.ToInt32(strReportWidth);
            }

            var deviceInfo =
            string.Format(@""
            + "<DeviceInfo>"
            + "    <OutputFormat>{0}</OutputFormat>"
                + "    <PageWidth>" + reportWidth + "in</PageWidth>"
                + "    <PageHeight>" + reportHeight + "in</PageHeight>"
            + "    <MarginTop>0.5in</MarginTop>"
            + "    <MarginLeft>0.5in</MarginLeft>"
            + "    <MarginRight>0.5in</MarginRight>"
            + "    <MarginBottom>0.5in</MarginBottom>"
            + "</DeviceInfo>", reportType);

            Warning[] warnings;
            string[] streams;

            //Render the report
            var renderedBytes = report.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName + "." + fileNameExtension);
            Response.BinaryWrite(renderedBytes);
            Response.End();

            //RenderReport(Server.MapPath("~/Reports/rdlc/its/followup.rdlc"), "MonthlyActivities", 10, 11.7, "pdf", ds_0, ds_1, ds_2);
            //return null;
        }

        protected string GetNewDocumentNo(string doctype, DateTime transdate)
        {
            var sql = "uspfn_GnDocumentGetNew {0}, {1}, {2}, {3}, {4}";
            var result = ctx.Database.SqlQuery<string>(sql, CompanyCode, BranchCode, doctype, CurrentUser.UserId, transdate);
            return result.First();
        }

        protected string GetNewDocumentNoHpp(string doctype, string transdate)
        {
            var sql = "uspfn_GnDocumentGetNew {0}, {1}, {2}, {3}, {4}";
            var result = ctx.Database.SqlQuery<string>(sql, CompanyCode, BranchCode, doctype, CurrentUser.UserId, transdate);
            return result.First();

        }

        //protected ResultModel DateTransValidation(DateTime paramDate)
        //{
        //    ResultModel result = InitializeResult();
        //    DateTime currentDate = DateTime.Now;

        //    string errMsg1 = string.Format("{0} tidak sesuai dengan {1}", "Tanggal transaksi", "periode transaksi");
        //    string errMsg2 = string.Format("{0} tidak sesuai dengan {1}", "Tanggal Transaksi", "Tanggal Server");
        //    string errMsg3 = string.Format("Periode sedang di locked");
        //    string errMsg4 = string.Format("Tanggal Transaksi lebih kecil dari tanggal [TransDate]");

        //    paramDate = paramDate.Date;

        //    var oSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
        //    if (oSpare != null)
        //    {
        //        if (oSpare.TransDate.Equals(DBNull.Value) || oSpare.TransDate < new DateTime(1900, 1, 2)) oSpare.TransDate = currentDate;

        //        if (paramDate >= oSpare.PeriodBeg.Date && paramDate <= oSpare.PeriodEnd.Date)
        //        {
        //            if (paramDate <= currentDate)
        //            {
        //                if (paramDate >= oSpare.TransDate.Date)
        //                {
        //                    if (oSpare.isLocked == true)
        //                    {
        //                        result.message = errMsg3;
        //                        result.status = false;
        //                    }
        //                }
        //                else
        //                {
        //                    errMsg4 = errMsg4.Replace("[TransDate]", oSpare.TransDate.Date.ToString("dd-MMM-yyyy"));
        //                    result.message = errMsg4;
        //                    result.status = false;
        //                }
        //                result.status = true;
        //            }
        //            else
        //            {
        //                result.message = errMsg2;
        //                result.status = false;
        //            }
        //        }
        //        else
        //        {
        //            result.message = errMsg1;
        //            result.status = false;
        //        }

        //    }
        //    return result;

        //}

        protected string GetMessage(string MessageCode){
            var record  = ctx.SysMsgs.Find(MessageCode);
            if (record != null)
            {
                return record.MessageCaption;
            }
            else
            {
                return string.Empty;
            }
        }

        public bool DateTransValidation(DateTime date, ref string msg)
        {
            if (CurrentUser == null)
            {
                msg = GetMessage(SysMessages.MSG_1200);
                return false;
            }

            string pcentre = ProfitCenter;
            DateTime currDate = DateTime.Now.Date;
            string errMsg1 = string.Format(GetMessage(SysMessages.MSG_5006), "Tanggal transaksi", "periode transaksi");
            string errMsg2 = string.Format(GetMessage(SysMessages.MSG_5006), "Tanggal Transaksi", "Tanggal Server");
            string errMsg3 = string.Format("Periode sedang di locked");
            string errMsg4 = string.Format("Tanggal Transaksi lebih kecil dari tanggal [TransDate]");

            date = date.Date;

            // 100 : Check for Unit (Sales) 
            if (pcentre.Equals("100"))
            {
                var oSales = ctx.GnMstCoProfileSaleses.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode);
 
                if (oSales != null)
                {
                    if (oSales.TransDate.Equals(DBNull.Value) || oSales.TransDate < new DateTime(1900, 1, 2)) oSales.TransDate = currDate;
                    if (date >= ((DateTime)oSales.PeriodBeg).Date && date <= ((DateTime)oSales.PeriodEnd).Date)
                    {
                        if (date <= currDate)
                        {
                            if (date >= ((DateTime)oSales.TransDate).Date)
                            {
                                if (oSales.isLocked == true)
                                {
                                    msg = errMsg3;
                                    return false;
                                }
                            }
                            else
                            {
                                errMsg4 = errMsg4.Replace("[TransDate]", ((DateTime)oSales.TransDate).ToString("dd-MMM-yyyy"));
                                msg = errMsg4;
                                return false;
                            }
                            return true;
                        }
                        else
                        {
                            msg = errMsg2;
                            return false;
                        }
                    }
                    else
                    {
                        msg = errMsg1;
                        return false;
                    }
                }
            }

            msg = GetMessage(SysMessages.MSG_5007);
            return false;
        }

        protected string ProfitCenter
        {
            get
            {
                string s = "100";
                //var x = ctx.SysUserProfitCenters.Find(CurrentUser.UserId);
                //if (x != null) s = x.ProfitCenter;
                return s;
            }
        }

        protected string ProductType
        {
            get
            {
                return ctx.CoProfiles.Find(CompanyCode, BranchCode).ProductType;
            }
        }

        protected string ProfitCenterName
        {
            get
            {
                string name = "";
                name = ctx.LookUpDtls.Find(CompanyCode, "PFCN", ProfitCenter).LookUpValueName;
                return name;
            }
        }

        public string getStringStatus(string status)
        {
            var Status = status == "0" ? "OPEN" : status == "1" ? "PRINTED"
                           : status == "2" ? "APPROVED"
                           : status == "3" ? "CANCELED"
                           : status == "9" ? "FINISHED" : "";
            return Status;
        }

        protected JsonResult ThrowException(string msg)
        {
            return Json(new { success = false, message = msg }, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult JsonException(string message)
        {
            return Json(new { success = false, message = "Terjadi Kesalahan, Hubungi SDMS Support", error_log = message });
        }


        protected string DealerCode()
        {
            var result = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
            if (result != null)
            {
                if (result.CompanyMD == CompanyCode && result.BranchMD == BranchCode) { return "MD"; }
                else { return "SD"; }
            }
            else return "MD";
        }

        protected string CompanyMD
        {
            get
            {
                var rd = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
                if (rd == null)
                {
                    return CompanyCode;
                }
                return rd.CompanyMD;
            }
        }

        protected string BranchMD
        {
            get
            {
                var rd = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
                if (rd == null)
                {
                    return BranchCode;
                }
                return rd.BranchMD;

            }
        }

        protected string UnitBranchMD
        {
            get
            {
                var rd = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
                if (rd == null)
                {
                    return BranchCode;
                }
                return rd.UnitBranchMD;
            }
        }

        protected string WarehouseMD
        {
            get
            {
                var rd = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
                if (rd == null)
                {
                    return "";
                }
                return rd.WarehouseMD;
            }
        }

        protected decimal GetCostPrice(string PartNo)
        {
            var parCompanyCode = new SqlParameter("CompanyCode", SqlDbType.VarChar);
            parCompanyCode.Value = CompanyCode;
            var parBranchCode = new SqlParameter("BranchCode", SqlDbType.VarChar);
            parBranchCode.Value = BranchCode;
            var parPartNo = new SqlParameter("PartNo", SqlDbType.VarChar);
            parPartNo.Value = PartNo;
            var CostPrice = new SqlParameter("CostPrice", SqlDbType.Decimal)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            object[] parameters = { parCompanyCode, parBranchCode, parPartNo, CostPrice };
            ctx.Database.ExecuteSqlCommand("exec uspfn_GetCostPrice @CompanyCode, @BranchCode, @PartNo, @CostPrice OUTPUT", parameters);

            decimal? val = CostPrice.Value as decimal?;
            var decCostPrice = val ?? 0;

            return decCostPrice;
        }

        protected bool cekOtomatis()
        {
            bool otom = true;
            string rcd = ctx.Database.SqlQuery<string>("SELECT ParaValue from gnMstLookUpDtl WHERE CodeID='OTOM' AND LookUpValue='UNIT' AND CompanyCode='" + CompanyCode + "'").FirstOrDefault();
            if (rcd != null && rcd == "0")
            {
                otom = false;
            }

            return otom;
        }

    }
}
