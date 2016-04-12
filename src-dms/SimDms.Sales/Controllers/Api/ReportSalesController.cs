using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Common.Models;
using SimDms.Common.DcsWs;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Data;
using TracerX;

namespace SimDms.Sales.Controllers.Api
{
    public class ReportSalesController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                ProductType = ProductType,
                IsBranch = IsBranch,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year
            });
        }

        public GetPeriod GetPeriod()
        {
            var query = string.Format(@"select gb.* from (
            select 'AP' as Code, CompanyCode, BranchCode, PeriodBeg, PeriodEnd from gnMstCoProfileFinance UNION ALL
            select 'AR' as Code, CompanyCode, BranchCode, PeriodBegAR, PeriodEndAR from gnMstCoProfileFinance UNION ALL
            select 'GL' as Code, CompanyCode, BranchCode, PeriodBegGL, PeriodEndGL from gnMstCoProfileFinance UNION ALL
            select 'SALES' as Code, CompanyCode, BranchCode, PeriodBeg, PeriodEnd from gnMstCoProfileSales)gb
            where gb.CompanyCode = '{0}'
            and gb.BranchCode = '{1}'
            and gb.Code = '{2}'", CompanyCode, BranchCode, "SALES");

            var data = ctx.Database.SqlQuery<GetPeriod>(query).FirstOrDefault();

            return data;
        }

        public JsonResult Periode()
        {
            return Json(new
            {
                DateFrom = GetPeriod().PeriodBeg,
                DateTo = GetPeriod().PeriodEnd
            });
        }

        public JsonResult CheckArea()
        {
            var Area = "";
            var Dealer = "";
            var DealerName = "";
            var isActive = ctx.GnMstDealerMapping.FirstOrDefault().isActive;
            if (isActive == true)
            {
                 Area = ctx.GnMstDealerMapping.FirstOrDefault().Area;
                 Dealer = ctx.GnMstDealerMapping.FirstOrDefault().DealerCode;
                 DealerName = ctx.GnMstDealerMapping.FirstOrDefault().DealerName;
            }

            return Json(new { success = true, Area = Area, Dealer = Dealer, DealerName = DealerName });
        }

        public JsonResult InquiryCustomer(bool AllowPeriod, DateTime StartDate, DateTime EndDate, string Area, string Dealer)
        {
            var isActive = AllowPeriod == false ? '0' : '1';
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_QueryCustomerDealer '" + isActive + "','" + StartDate + "','" + EndDate + "','" + Area + "','" + Dealer + "',''";

            MyLogger.Log.Info("Inquiry Customers: EXEC " + cmd.CommandText);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet dt = new DataSet();
            da.Fill(dt);
            var records = dt.Tables[0];
            var rows = records.Rows.Count;

            return Json(new { success = true, data = records, total = rows }, JsonRequestBehavior.AllowGet);
            //var sqlstr = ctx.Database.SqlQuery<gnMstCustomerDealer>("uspfn_QueryCustomerDealer '" + isActive + "','" + DateFrom + "','" + DateTo + "','" + Area + "','"+Dealer+"',''").AsQueryable();

            //return Json(sqlstr);
        }

        public JsonResult ValidatePrintSO(string SONo, string SONoTo)
        {
            int comparison = string.Compare(SONo, SONoTo, false);

            if (comparison > 0)
            {
                var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "No SO Awal", "No SO Akhir");
                return Json(new { success = false, message = msg });
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult ValidatePrintDO(string DONo, string DONoTo)
        {
            int comparison = string.Compare(DONo, DONoTo, false);

            if (comparison > 0)
            {
                var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "No DO Awal", "No DO Akhir");
                return Json(new { success = false, message = msg });
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult ValidatePrintBPK(string BPKNo, string BPKNoTo)
        {
            int comparison = string.Compare(BPKNo, BPKNoTo, false);

            if (comparison > 0)
            {
                var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "No BPK Awal", "No BPK Akhir");
                return Json(new { success = false, message = msg });
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult ValidatePrintInvoice(string InvoiceNo, string InvoiceNoTo)
        {
            int comparison = string.Compare(InvoiceNo, InvoiceNoTo, false);

            if (comparison > 0)
            {
                var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "No Invoice Awal", "No Invoice Akhir");
                return Json(new { success = false, message = msg });
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult ValidatePrintSalesAchievement(string EmployeeCodeFrom, string EmployeeCodeTo, string LookupCodeFrom, string LookupCodeTo, string ModelCodeFrom, string ModelCodeTo)
        {
            if (EmployeeCodeFrom != "" || EmployeeCodeTo != "")
            {
                int comparison = string.Compare(EmployeeCodeFrom, EmployeeCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Kode Sales Awal", "Kode Sales Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            else if (LookupCodeFrom != "" || LookupCodeTo != "")
            {
                int comparison = string.Compare(LookupCodeFrom, LookupCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Kategori Awal", "Kategori Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            else if (ModelCodeFrom != "" || ModelCodeTo != "")
            {
                int comparison = string.Compare(ModelCodeFrom, ModelCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Model Awal", "Model Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult ValidatePrintRegisterSO(string SONo, string SONoTo, string CustomerCode, string CustomerCodeTo, string ModelCodeFrom, string ModelCodeTo)
        {
            if (SONo != "" || SONoTo != "")
            {
                int comparison = string.Compare(SONo, SONoTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "No SO Awal", "No SO Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            else if (CustomerCode != "" || CustomerCodeTo != "")
            {
                int comparison = string.Compare(CustomerCode, CustomerCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Kode Pelanggan Awal", "Kode Pelanggan Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            else if (ModelCodeFrom != "" || ModelCodeTo != "")
            {
                int comparison = string.Compare(ModelCodeFrom, ModelCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Model Awal", "Model Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult ValidatePrintRegisterDO(string DONo, string DONoTo, string CustomerCode, string CustomerCodeTo, string ModelCodeFrom, string ModelCodeTo)
        {
            if (DONo != "" || DONoTo != "")
            {
                int comparison = string.Compare(DONo, DONoTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "No DO Awal", "No DO Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            else if (CustomerCode != "" || CustomerCodeTo != "")
            {
                int comparison = string.Compare(CustomerCode, CustomerCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Kode Pelanggan Awal", "Kode Pelanggan Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            else if (ModelCodeFrom != "" || ModelCodeTo != "")
            {
                int comparison = string.Compare(ModelCodeFrom, ModelCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Model Awal", "Model Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult ValidatePrintRegisterBPK(string BPKNo, string BPKNoTo, string CustomerCode, string CustomerCodeTo, string ModelCodeFrom, string ModelCodeTo)
        {
            if (BPKNo != "" || BPKNoTo != "")
            {
                int comparison = string.Compare(BPKNo, BPKNoTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "No BPK Awal", "No BPK Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            else if (CustomerCode != "" || CustomerCodeTo != "")
            {
                int comparison = string.Compare(CustomerCode, CustomerCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Kode Pelanggan Awal", "Kode Pelanggan Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            else if (ModelCodeFrom != "" || ModelCodeTo != "")
            {
                int comparison = string.Compare(ModelCodeFrom, ModelCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Model Awal", "Model Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult ValidatePrintRegisterInvoice(string InvoiceNo, string InvoiceNoTo, string CustomerCode, string CustomerCodeTo, string ModelCodeFrom, string ModelCodeTo)
        {
            if (InvoiceNo != "" || InvoiceNoTo != "")
            {
                int comparison = string.Compare(InvoiceNo, InvoiceNoTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "No Invoice Awal", "No Invoice Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            else if (CustomerCode != "" || CustomerCodeTo != "")
            {
                int comparison = string.Compare(CustomerCode, CustomerCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Kode Pelanggan Awal", "Kode Pelanggan Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            else if (ModelCodeFrom != "" || ModelCodeTo != "")
            {
                int comparison = string.Compare(ModelCodeFrom, ModelCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Model Awal", "Model Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult ValidatePrintRegisterReturn(string BranchCode, string BranchCodeTo, string ReturnNo, string ReturnNoTo, string CustomerCode, string CustomerCodeTo, string ModelCodeFrom, string ModelCodeTo)
        {
            if (BranchCode != "" || BranchCodeTo != "")
            {
                int comparison = string.Compare(BranchCode, BranchCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Kode Cabang Awal", "Kode Cabang Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            else if (ReturnNo != "" || ReturnNoTo != "")
            {
                int comparison = string.Compare(ReturnNo, ReturnNoTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "No Return Awal", "No Return Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            else if (CustomerCode != "" || CustomerCodeTo != "")
            {
                int comparison = string.Compare(CustomerCode, CustomerCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Kode Pelanggan Awal", "Kode Pelanggan Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            else if (ModelCodeFrom != "" || ModelCodeTo != "")
            {
                int comparison = string.Compare(ModelCodeFrom, ModelCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Model Awal", "Model Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult ValidatePrintNotaDebet(string InvoiceNo, string InvoiceNoTo, string BranchCode, string BranchCodeTo)
        {
            if (InvoiceNo != "" || InvoiceNoTo != "")
            {
                int comparison = string.Compare(InvoiceNo, InvoiceNoTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "No Invoice Awal", "No Invoice Akhir");
                    return Json(new { success = false, message = msg });
                }
                return Json(new { success = true, message = "" });
            }
            else if (BranchCode != "" || BranchCodeTo != "")
            {
                int comparison = string.Compare(BranchCode, BranchCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Kode Cabang Awal", "Kode Cabang Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult ValidatePrintPelangganLeasing(string LeasingCode, string LeasingCodeTo, string BranchCode, string BranchCodeTo)
        {
            if (LeasingCode != "" || LeasingCodeTo != "")
            {
                int comparison = string.Compare(LeasingCode, LeasingCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Kode Leasing Awal", "Kode Leasing Akhir");
                    return Json(new { success = false, message = msg });
                }
                return Json(new { success = true, message = "" });
            }
            else if (BranchCode != "" || BranchCodeTo != "")
            {
                int comparison = string.Compare(BranchCode, BranchCodeTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "Kode Cabang Awal", "Kode Cabang Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult ValidatePrintFakturPolis(string FakturPolisiNo, string FakturPolisiNoTo)
        {
            if (FakturPolisiNo != "" || FakturPolisiNoTo != "")
            {
                int comparison = string.Compare(FakturPolisiNo, FakturPolisiNoTo, false);

                if (comparison > 0)
                {
                    var msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption, "No Faktur Polis Awal", "No Faktur Polis Akhir");
                    return Json(new { success = false, message = msg });
                }
                return Json(new { success = true, message = "" });
            }
            return Json(new { success = true, message = "" });
        }
    }
}
