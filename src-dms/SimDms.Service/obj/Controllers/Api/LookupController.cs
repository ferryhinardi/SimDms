using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Service.Models;
using SimDms.Common;

namespace SimDms.Service.Controllers.Api
{
    public class LookupController : BaseController
    {
        public ActionResult TaskPart()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvLkuTaskPart";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", Request["CompanyCode"]);
            cmd.Parameters.AddWithValue("@BranchCode", Request["BranchCode"]);
            cmd.Parameters.AddWithValue("@BasicModel", Request["BasicModel"]);
            cmd.Parameters.AddWithValue("@JobType", Request["JobType"]);
            cmd.Parameters.AddWithValue("@ChassisCode", Request["ChassisCode"]);
            cmd.Parameters.AddWithValue("@ChassisNo", Request["ChassisNo"]);
            cmd.Parameters.AddWithValue("@TransType", Request["TransType"]);
            cmd.Parameters.AddWithValue("@ItemType", Request["ItemType"]);
            cmd.Parameters.AddWithValue("@BillType", Request["BillType"]);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(GetJson(dt));
        }

        public JsonResult SupplierRegPekerjaanLuar(DateTime? dateFrom, DateTime? dateTo)
        {
            //var record = from p in ctx.SubCons
            //             join p1 in ctx.Suppliers on new { p.CompanyCode, p.SupplierCode } equals new { p1.CompanyCode, p1.SupplierCode } into tSupplier
            //             where p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProductType == ProductType && p.PODate >= dateFrom
            //             select new ListSupplier
            //             {
            //                 SupplierCode = p.SupplierCode,
            //                 SupplierName = p.SupplierCode 
            //             };

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = @"
                select distinct a.SupplierCode, b.SupplierName from 
                svTrnPOSubCon a
                left join gnMstSupplier b on a.CompanyCode = b.CompanyCode and a.SupplierCode= b.SupplierCode
                where
	                a.CompanyCode = @CompanyCode 
                    and a.BranchCode= @BranchCode 
	                and a.ProductType= @ProductType 
                	and convert(varchar,a.PODate,112) between @StartDate and @EndDate
                order by a.SupplierCode";
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@StartDate",dateFrom);
            cmd.Parameters.AddWithValue("@EndDate", dateTo);
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var list = GetJson(dt);

            return Json(list.AsQueryable().toKG());
        }

        public JsonResult SqlBatchLookUp(BatchLookUpFilter filter)
        {
            var user = CurrentUser;
            var coProfile = ctx.CoProfiles.Where(m => m.CompanyCode == user.CompanyCode && m.BranchCode == user.BranchCode).FirstOrDefault();

            string sql = @"
select (row_number() over (order by a.BatchNo)) seq
,a.BatchNo
,a.BatchDate
,a.ReceiptNo
,a.ReceiptDate
,a.FPJNo
,a.FPJDate
,a.FPJGovNo
,a.IsCampaign
,IsCampaignDesc = (case a.IsCampaign when 1 then 'Ya' else 'Tidak' end)
,a.ProcessSeq
,ItemTotal = isnull((
  select sum(b.TotalNoOfItem) from SvTrnPdiFsc b where 1 = 1
  and b.CompanyCode = a.CompanyCode
  and b.BranchCode = a.BranchCode
  and b.ProductType = a.ProductType
  and b.BatchNo = a.BatchNo
 ),0)
,ItemTotalAmt = isnull((
  select sum(b.TotalAmt) from SvTrnPdiFsc b where 1 = 1
  and b.CompanyCode = a.CompanyCode
  and b.BranchCode = a.BranchCode
  and b.ProductType = a.ProductType
  and b.BatchNo = a.BatchNo
 ),0)
from svTrnPdiFscBatch a where 1 = 1
 and a.CompanyCode = @p0
 and a.BranchCode = @p1
 and a.ProductType = @p2
";
            //ctx.Database.Connection.ConnectionString += "; MultipleActiveResultSets=true ";

            var qry = ctx.Database.SqlQuery<BatchLookUpView>(sql, CurrentUser.CompanyCode, CurrentUser.BranchCode, coProfile.ProductType).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.NoBatchlk))
                qry = qry.Where(x => x.BatchNo == filter.NoBatchlk);

            if (!string.IsNullOrWhiteSpace(filter.NoKwitansilk))
                qry = qry.Where(x => x.ReceiptNo == filter.NoKwitansilk);

            if (!string.IsNullOrWhiteSpace(filter.NoFakturPajaklk))
                qry = qry.Where(x => x.FPJNo == filter.NoFakturPajaklk);

            if (!string.IsNullOrWhiteSpace(filter.SeriPajak))
                qry = qry.Where(x => x.FPJGovNo == filter.SeriPajak);

            if (filter.TglBatchlk.HasValue && filter.sdTglBatchlk.HasValue)
                qry = qry.Where(x => filter.TglBatchlk <= x.BatchDate && filter.sdTglBatchlk >= x.BatchDate);

            if (filter.TglKwitansilk.HasValue && filter.sdTglKwitansilk.HasValue)
                qry = qry.Where(x => filter.TglKwitansilk <= x.ReceiptDate && filter.sdTglKwitansilk >= x.ReceiptDate);

            if (filter.TglFakturPajaklk.HasValue && filter.sdTglFakturPajaklk.HasValue)
                qry = qry.Where(x => filter.TglFakturPajaklk <= x.FPJDate && filter.sdTglFakturPajaklk >= x.FPJDate);

            return Json(qry.KGrid());
        }

        private class BatchLookUpView
        {
            public Int64 seq { get; set; }
            public string BatchNo { get; set; }
            public DateTime? BatchDate { get; set; }
            public string ReceiptNo { get; set; }
            public DateTime? ReceiptDate { get; set; }
            public string FPJNo { get; set; }
            public DateTime? FPJDate { get; set; }
            public string FPJGovNo { get; set; }
            public bool IsCampaign { get; set; }
            public decimal ProcessSeq { get; set; }
            public decimal ItemTotal { get; set; }
            public decimal ItemTotalAmt { get; set; }
        }

        public class BatchLookUpFilter
        {
            public string NoBatchlk { get; set; }
            public string NoKwitansilk { get; set; }
            public string NoFakturPajaklk { get; set; }
            public string SeriPajak { get; set; }
            public DateTime? TglBatchlk { get; set; }
            public DateTime? sdTglBatchlk { get; set; }
            public DateTime? TglKwitansilk { get; set; }
            public DateTime? sdTglKwitansilk { get; set; }
            public DateTime? TglFakturPajaklk { get; set; }
            public DateTime? sdTglFakturPajaklk { get; set; }
        }

        public ActionResult InvoiceFakturPajak()
        {
            var data = ctx.Database.SqlQuery<InvoiceFP>(string.Format("EXEC uspfn_InvoiceFakturPajak '{0}','{1}','{2}'",CurrentUser.CompanyCode,CurrentUser.BranchCode, ProductType));
            return Json(data.AsQueryable().KGrid());
        }

        public ActionResult Signer()
        {
            var sql = "select distinct SignName, TitleSign from gnMstSignature "+
                      "where 1 = 1 and CompanyCode = '{0}' "+
                      "and BranchCode = '{1}' and ProfitCenterCode = '200' "+
                      "and DocumentType like 'IN%'";
            sql = string.Format(sql,CurrentUser.CompanyCode, CurrentUser.BranchCode);
            var data = ctx.Database.SqlQuery<Signer>(sql);
            return Json(data.AsQueryable().KGrid());
        }

        public JsonResult reffcode(string refcode)
        {
            var queryable = ctx.svMstRefferenceServiceViews.Where(x => x.CompanyCode == CompanyCode
                && x.ProductType == ProductType && x.RefferenceType == refcode);
            if (queryable != null)
            {
                return Json(new
                {
                    success = true,
                    data = queryable
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = false,
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
