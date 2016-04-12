using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.DataWarehouse.Models;
using SimDms.DataWarehouse.Helpers;
using System.IO;
using Microsoft.Reporting.WebForms;
using System.Web.Script.Serialization;
using System.Transactions;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class SvTransController : BaseController
    {
        public JsonResult FtirList()
        {
            var qry = ctx.SvFtirList.AsQueryable();

            var usname = CurrentUser.Username;
            var dealer = ctx.SysUserDealers.Find(usname);

            if (dealer != null)
            {
                qry = qry.Where(x => x.OutletCode == dealer.OutletCode);
            }

            return Json(qry.KGrid());
        }

        public JsonResult GetFtirByVIN()
        {
            var VIN = Request["VIN"] ?? "0";
            var qry = ctx.SvFtirList.Where(x => x.VinNo == VIN).FirstOrDefault();
            return Json(new {  data = qry }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModelInfo()
        {
            var VIN = Request["VIN"] ?? "0";
            var data = ctx.Database.SqlQuery<VinModel>("exec uspfn_svftir_getmodelinfo '" + VIN + "'").ToList();
            return Json(new {result= data.Count, data = data });
        }

        public JsonResult FtirSave()
        {
            SqlConnection con = ctx.Database.Connection as SqlConnection;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_SvFtirSave";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            var pars = Request.Params;
            var dict = new Dictionary<string, dynamic>();

            foreach (string par in pars)
            {
                if (par.StartsWith("p_"))
                {
                    dict.Add(par.Substring(2), Request[par]);
                    cmd.Parameters.AddWithValue(par.Substring(2), Request[par]);
                }
            }

            cmd.Parameters.AddWithValue("UserID", CurrentUser.Username);

            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Json(new { success = true, data = GetJsonRow(dt) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            ResultModel result = new ResultModel();
            string userId = CurrentUser.Username ;
            DateTime currentTime = DateTime.Now;
            result.status = false;

            if (file != null)
            {
                //byte[] rawData = new byte[file.ContentLength];
                //file.InputStream.Read(rawData, 0, file.ContentLength);

                string fn = System.IO.Path.GetFileName(file.FileName);
                string SaveLocation = Server.MapPath("~/FTIR"); // +"\\" + fn;
                string fileLocation = "";

                string DateEntry = DateTime.Now.ToString("yyyy");

                string dirTarget = SaveLocation + "/" + DateEntry;
                fileLocation += DateEntry;

                if (!System.IO.Directory.Exists(dirTarget))
                {
                    System.IO.Directory.CreateDirectory(dirTarget);
                }

                dirTarget += "/" + DateTime.Now.ToString("MM");
                fileLocation += "/" + DateTime.Now.ToString("MM");

                if (!System.IO.Directory.Exists(dirTarget))
                {
                    System.IO.Directory.CreateDirectory(dirTarget);
                }


                if (CurrentUser.DealerCode != "")
                {
                    dirTarget += "/" + CurrentUser.DealerCode;
                    fileLocation += "/" + CurrentUser.DealerCode;

                    if (!System.IO.Directory.Exists(dirTarget))
                    {
                        System.IO.Directory.CreateDirectory(dirTarget);
                    }
                }

                SaveLocation = dirTarget + "/" + DateTime.Now.ToString("dd-") + fn;
                fileLocation += "/" + DateTime.Now.ToString("dd-") + fn;

                try
                {
                    if (System.IO.File.Exists(SaveLocation))
                    {
                        System.IO.File.Delete(SaveLocation);
                    }

                    file.SaveAs(SaveLocation);

                    result.status = true;
                    result.data = new 
                    {
                        FileID = fileLocation,
                        FileName = fn,
                        FileType = file.ContentType,
                        FileSize = file.ContentLength
                    };
                    result.message = ("The file has been uploaded.");
                }
                catch (Exception ex)
                {
                    result.message = ("Error: " + ex.Message);
                    //Note: Exception.Message returns a detailed message that describes the current exception. 
                    //For security reasons, we do not recommend that you return Exception.Message to end users in 
                    //production environments. It would be better to put a generic error message. 
                }
            }
            else
            {
                result.message = "Sorry, we can't process your request.";
            }

            return Json(result); 
        }

        public JsonResult GetCurrentDealer()
        {
            var usname = CurrentUser.Username;
            var dealer = ctx.SysUserDealers.Find(usname);

            if (dealer != null)
            {
               //dealer.DealerName = ctx.svMasterDealerOutletMappings.Find(dealer.DealerCode, dealer.OutletCode).OutletName;
                var dlr = ctx.svMasterDealerOutletMappings.Where(x => x.GNDealerCode == dealer.DealerCode && x.GNOutletCode == dealer.OutletCode).FirstOrDefault();
                if (dlr != null)
                {
                    dealer.DealerName = dlr.OutletName;
                }
                return Json(dealer);
            }
            else
            {
                return Json(new { });
            }
        }

        [HttpGet]
        public ActionResult ClaimTagWarranty()
        {
            var FTIRNO = Request["FTIRNO"] ?? "";

            var param = new MyReportParameter();

            param.reportType =  "EXCELOPENXML";
            param.sql = "exec uspfn_svFtirReportClaimTag '" + FTIRNO + "'";
            param.filename = Path.Combine( Server.MapPath("~/Reports/rdlc"),  @"sv\ClaimTagWarranty.rdl");
            param.Name = "DataSet1";
            
            var data = MyHelpers.GenerateSQLReport(param);

            //response.Content = new ByteArrayContent(data);
            //response.Content.Headers.Add("Content-Type", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            //response.Content.Headers.Add("Content-Disposition",
            //    string.Format("attachment; filename=" + param.reporttype + "_{0}.xlsx", DatePeriod(param)));
            //response.Content.Headers.Add("Content-Length", data.Length.ToString());
            
             return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [HttpGet]
        public ActionResult RekapFTIR()
        {

            var PeriodStart = Request["PeriodStart"] ?? "";
            var PeriodEnd = Request["PeriodEnd"] ?? "";
            var Area = Request["Area"] ?? "";
            var FilterBy = Request["FilterBy"] ?? "";
            var Dealer = Request["Dealer"] ?? "";
            var Model = Request["Model"] ?? "";
            var Outlet = Request["Outlet"] ?? "";
            var FTIRNO = Request["FTIRNO"] ?? "";

            var AreaName = Request["AreaName"] ?? "";
            var DealerName = Request["DealerName"] ?? "";
            var OutletName = Request["OutletName"] ?? "";
            var FilterByName = Request["FilterByName"] ?? "";

            var userdealer = ctx.SysUserDealers.Find(CurrentUser.Username);

            if (userdealer != null)
            {
                Dealer = userdealer.DealerCode;
                Outlet = userdealer.OutletCode;
                OutletName = ctx.OutletInfos.Find(userdealer.DealerCode, userdealer.OutletCode).BranchName;
                var di = ctx.GnMstDealerMappings.Where(x => x.DealerCode == userdealer.DealerCode).ToList();
                if (di != null && di.Count == 1)
                {
                    DealerName = di[0].DealerName;
                    AreaName = di[0].Area;
                }
            }

            var param = new MyReportParameter();

            param.reportType = "EXCELOPENXML";
            param.sql = string.Format("exec uspfn_svRekapFTIR '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'",
                        PeriodStart, PeriodEnd, FilterBy, Model, FTIRNO, Area, Dealer, Outlet);

            param.filename = Path.Combine(Server.MapPath("~/Reports/rdlc"), @"sv\RekapFTIR.rdl");
            param.Name = "DataSet1";

            ReportParameter p1 = new ReportParameter("Periode", Convert.ToDateTime(PeriodStart).ToString("dd MMM yyyy") + " s.d " + Convert.ToDateTime(PeriodEnd).ToString("dd MMM yyyy"));
            ReportParameter p2 = new ReportParameter("FilterBy", FilterByName);
            ReportParameter p3 = new ReportParameter("ModelType", Model);
            ReportParameter p4 = new ReportParameter("Area", AreaName);
            ReportParameter p5 = new ReportParameter("Dealer", DealerName);
            ReportParameter p6 = new ReportParameter("Outlet", OutletName);

            var paramsReport = new ReportParameter[] { p1, p2, p3, p4, p5, p6 };

            var data = MyHelpers.GenerateSQLReport(param, paramsReport);

            //response.Content = new ByteArrayContent(data);
            //response.Content.Headers.Add("Content-Type", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            //response.Content.Headers.Add("Content-Disposition",
            //    string.Format("attachment; filename=" + param.reporttype + "_{0}.xlsx", DatePeriod(param)));
            //response.Content.Headers.Add("Content-Length", data.Length.ToString());

            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [HttpGet]
        public ActionResult RekapFTIR_Dtl()
        {

            var PeriodStart = Request["PeriodStart"] ?? "";
            var PeriodEnd = Request["PeriodEnd"] ?? "";
            var Area = Request["Area"] ?? "";
            var FilterBy = Request["FilterBy"] ?? "";
            var Dealer = Request["Dealer"] ?? "";
            var Model = Request["Model"] ?? "";
            var Outlet = Request["Outlet"] ?? "";
            var FTIRNO = Request["FTIRNO"] ?? "";

            var AreaName = Request["AreaName"] ?? "";
            var DealerName = Request["DealerName"] ?? "";
            var OutletName = Request["OutletName"] ?? "";
            var FilterByName = Request["FilterByName"] ?? "";

            var userdealer = ctx.SysUserDealers.Find(CurrentUser.Username);

            if (userdealer != null)
            {
                Dealer = userdealer.DealerCode;
                Outlet = userdealer.OutletCode;
                OutletName = ctx.OutletInfos.Find(userdealer.DealerCode, userdealer.OutletCode).BranchName;
                var di = ctx.GnMstDealerMappings.Where(x => x.DealerCode == userdealer.DealerCode).ToList();
                if (di != null && di.Count == 1)
                {
                    DealerName = di[0].DealerName;
                    AreaName = di[0].Area;
                }
            }

            var param = new MyReportParameter();

            param.reportType = "EXCELOPENXML";
            param.sql = string.Format("exec uspfn_svRekapFTIR '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'",
                        PeriodStart, PeriodEnd, FilterBy, Model, FTIRNO, Area, Dealer, Outlet);

            param.filename = Path.Combine(Server.MapPath("~/Reports/rdlc"), @"sv\RekapFTIR_Dtl.rdl");
            param.Name = "DataSet1";

            ReportParameter p1 = new ReportParameter("Periode", Convert.ToDateTime(PeriodStart).ToString("dd MMM yyyy") + " s.d " + Convert.ToDateTime(PeriodEnd).ToString("dd MMM yyyy"));
            ReportParameter p2 = new ReportParameter("FilterBy", FilterByName);
            ReportParameter p3 = new ReportParameter("ModelType", Model);
            ReportParameter p4 = new ReportParameter("Area", AreaName);
            ReportParameter p5 = new ReportParameter("Dealer", DealerName);
            ReportParameter p6 = new ReportParameter("Outlet", OutletName);

            var paramsReport = new ReportParameter[] { p1, p2, p3, p4, p5, p6 };

            var data = MyHelpers.GenerateSQLReport(param, paramsReport);

            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [HttpGet]
        public ActionResult FormFTIR()
        {
            var FTIRNO = Request["FTIRNO"] ?? "";
            var param = new MyReportParameter();

            param.reportType = "PDF";
            param.sql = string.Format("exec uspfn_svFormFTIR '{0}'", FTIRNO);

            param.filename = Path.Combine(Server.MapPath("~/Reports/rdlc"), @"sv\FormFTIR.rdl");
            param.Name = "DataSet1";

            var data = MyHelpers.GenerateSQLReportPdf(param);

            return File(data, "application/pdf");
        }

        #region MRSR Transaksi
        public class dtMRSR{
            public string a { get; set; }
            public string b { get; set; }
            public string c { get; set; }
            public string d { get; set; }
            public string e { get; set; }
            public string f { get; set; }
            public string g { get; set; }
            public string h { get; set; }
            public string i { get; set; }
            public string j { get; set; }
        }
        public JsonResult ReloadMRSR() 
        {
            try
            {
                string qry = String.Format(@"select lower(Description) 
					from svMstRefferenceService 
					where RefferenceType = 'SETMRSR' 
					order by convert(int, RefferenceCode)");

                var data = ctx.Database.SqlQuery<string>(qry);
                qry = String.Format(@"select Description
					from svMstRefferenceService 
					where RefferenceType = 'Month' 
					order by convert(int, RefferenceCode)");
                var data2 = ctx.Database.SqlQuery<string>(qry);
                
                return Json(new { message = "Success", data = data, data2 = data2 });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.InnerException });
            }
        }

        public JsonResult getData() 
        {
            var periodyear = Request["PeriodYear"];
            try
            {
                string qry = String.Format(@"declare @sqlstr varchar(max) = '',@sqlstr1 varchar(max) = '',@sqlstr3 varchar(max) = '', @a int, @b int = 1, @c int ,  @d varchar(2) = 'a'
                set @a = (select max(convert(int,MRSRCODE)) from svtrnMRSR)
                set @c = (select max(convert(int,MRSRCODE)) from svtrnMRSR)
                while @a > 0
                begin
	                if @sqlstr = ''
	                begin
		                set @sqlstr = '[' + convert(varchar(2),@b) + ']'
		                set @sqlstr3 = '[' + convert(varchar(2),@b) + '] as ' + @d
	                end else begin
		                set @sqlstr = @sqlstr + ', [' + convert(varchar(2),@b) + ']'
		                set @sqlstr3 = @sqlstr3 + ', [' + convert(varchar(2),@b) + '] as ' + @d
	                end
	                set @d = CHAR(ASCII(@d) + 1)
	                set @b = @b + 1
	                set @a = @a - 1
                end

                set @b = 1
                while @c > 0 or @b < 13
                begin
                if @sqlstr1 = ''
                begin
	                set @sqlstr1 = '
	                select ' + @sqlstr3 + '
                    from (
	                select MRSRCODE, mrsrData
	                from svtrnMRSR
	                where PeriodYear = {0}
	                and PeriodMonth = '+convert(varchar(2),@b)+'
                  )#
                 pivot (max(mrsrData) for MRSRCODE in ( ' + @sqlstr + ')) as pvt
                '
                end else begin
                set @sqlstr1 = @sqlstr1 + '
                union all
                select *
                    from (
	                select MRSRCODE, mrsrData
	                from svtrnMRSR
	                where PeriodYear = {0}
	                and PeriodMonth = '+convert(varchar(2),@b)+'
                  )#
                 pivot (max(mrsrData) for MRSRCODE in ( ' + @sqlstr + ')) as pvt
                '
                end

	                set @b = @b + 1
	                set @c = @c - 1
                end

                exec(@sqlstr1)", periodyear);

                var data = ctx.Database.SqlQuery<dtMRSR>(qry);
 
                return Json(new { message = "Success", data = data});
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.InnerException });
            }
        }

        public JsonResult MRSRSave() 
        {
            var data = Request["listScore"];
            int n = 0;

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<SvTrnMRSR> listscore = ser.Deserialize<List<SvTrnMRSR>>(data);
            List<SvTrnMRSR> listscoreerror = new List<SvTrnMRSR>();

            using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                foreach (var item in listscore)
                {
                    var ent = ctx.SvTrnMRSRs.Find(item.CompanyCode, item.BranchCode, item.ProductType, item.PeriodYear, item.PeriodMonth, item.MRSRCode);
                    if (ent == null)
                    {
                        ent = new SvTrnMRSR
                        {
                            CompanyCode = item.CompanyCode,
                            BranchCode = item.BranchCode,
                            ProductType = item.ProductType,
                            PeriodYear = item.PeriodYear,
                            PeriodMonth = item.PeriodMonth,
                            MRSRCode = item.MRSRCode,
                            MRSRData = item.MRSRData,
                            CreatedBy = CurrentUser.Username,
                            CreatedDate = DateTime.Now,
                            LastUpdateBy = CurrentUser.Username,
                            LastUpdateDate = DateTime.Now,
                        };

                        ctx.SvTrnMRSRs.Add(ent);

                        //n = ctx.SaveChanges();
                    }
                    else {
                            ent.CompanyCode = item.CompanyCode;
                            ent.BranchCode = item.BranchCode;
                            ent.ProductType = item.ProductType;
                            ent.PeriodYear = item.PeriodYear;
                            ent.PeriodMonth = item.PeriodMonth;
                            ent.MRSRCode = item.MRSRCode;
                            ent.MRSRData = item.MRSRData;
                            ent.LastUpdateBy = CurrentUser.Username;
                            ent.LastUpdateDate = DateTime.Now;
                    }
                }

                try
                {
                    n = ctx.SaveChanges();
                    tranScope.Complete();
                }
                catch (Exception ex)
                {
                    tranScope.Dispose();
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return Json(new { success = true, message = "data berasil disimpan" });
        }
        #endregion
    }
}
