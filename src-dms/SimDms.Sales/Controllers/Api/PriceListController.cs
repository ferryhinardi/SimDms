using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Sales.Models;
using System.Web.Script.Serialization;

namespace SimDms.Sales.Controllers.Api
{
    public class PriceListController : BaseController
    {

        public JsonResult Save(string BranchCode, string SupplierCode, string Data)
        {                        
            var items = Data;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<ListPriceData> listprice = ser.Deserialize<List<ListPriceData>>(Data);

            if (string.IsNullOrEmpty(BranchCode))
            {
                BranchCode = "";
            }

            foreach(var item in listprice)
            {
                SqlParameter p0 = new SqlParameter("@CompanyCode", CompanyCode);
                SqlParameter p1 = new SqlParameter("@BranchCode", BranchCode);
                SqlParameter p2 = new SqlParameter("@SupplierCode", SupplierCode);
                SqlParameter p3 = new SqlParameter("@GroupPrice", item.GroupPrice);
                SqlParameter p4 = new SqlParameter("@SalesModelCode", item.SalesModelCode);
                SqlParameter p5 = new SqlParameter("@SalesModelYear", item.SalesModelYear);
                SqlParameter p6 = new SqlParameter("@RetailPriceIncludePPN", item.RetailPriceIncludePPN);
                SqlParameter p7 = new SqlParameter("@DiscPriceIncludePPN", item.DiscPriceIncludePPN);
                SqlParameter p8 = new SqlParameter("@NetSalesIncludePPN", item.NetPriceIncludePPN);
                SqlParameter p9 = new SqlParameter("@RetailPriceExcludePPN", item.RetailPriceExcludePPN);
                SqlParameter p10 = new SqlParameter("@DiscPriceExcludePPN", item.DiscPriceExcludePPN);
                SqlParameter p11 = new SqlParameter("@NetSalesExcludePPN", item.NetPriceExcludePPN);
                SqlParameter p12 = new SqlParameter("@PPNBeforeDisc", item.PPNBeforeDisc);
                 SqlParameter p13 = new SqlParameter("@PPNAfterDisc", item.PPNAfterDisc);
                SqlParameter p14 = new SqlParameter("@PPNBMPaid", item.PPNBMPaid ?? 0);
                SqlParameter p15 = new SqlParameter("@EffectiveDate", item.EffectiveDate ?? DateTime.Now);
                SqlParameter p16 = new SqlParameter("@isStatus", item.isStatus ?? true);
                SqlParameter p17 = new SqlParameter("@UserId", CurrentUser.UserId);
                SqlParameter p18 = new SqlParameter("@OthersDPP", item.OthersDPP);
                SqlParameter p19 = new SqlParameter("@OthersPPN", item.OthersPPN);

                Object[] oPeams = new Object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14 , p15, p16, p17, p18, p19};

                ctx.Database.ExecuteSqlCommand("EXEC uspfn_omUpdatePriceList @companycode,@BranchCode,@SupplierCode,@GroupPrice,@SalesModelCode,@SalesModelYear,@EffectiveDate,@RetailPriceIncludePPN,@DiscPriceIncludePPN,@NetSalesIncludePPN,@RetailPriceExcludePPN,@DiscPriceExcludePPN,@NetSalesExcludePPN,@PPNBeforeDisc,@PPNAfterDisc,@PPNBMPaid,@isStatus,@UserId,@OthersDPP,@OthersPPN", oPeams);
            }

            return Json(new { success = true });
        }

        public JsonResult LoadTable(String BranchCode, string SalesModelCode, String SalesModelYear, bool IsAllStatus, bool IsActive, string SupplierCode)
        {
            int Status = 2;

            if (!IsAllStatus)
            {
                Status = IsActive ? 1 : 0;
            }

            if (SalesModelYear == "")
            {
                SalesModelYear = "0";
            }

            //var listprice = ctx.Database.SqlQuery<ListPriceData>(string.Format("EXEC uspfn_omGetPriceList '{0}','{1}','{2}',{3},{4},'{5}'", CompanyCode, BranchCode, SalesModelCode, SalesModelYear, Status, SupplierCode));
           
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "uspfn_omGetPriceList";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@SalesModelCode", SalesModelCode);
            cmd.Parameters.AddWithValue("@SalesModelYear", SalesModelYear);
            cmd.Parameters.AddWithValue("@Status", Status);
            cmd.Parameters.AddWithValue("@SupplierCode", SupplierCode);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);

            return Json(new { success = true, data = dt }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult PivotDemo()
        {

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usppvt_SvMsiP29";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@StartDate", "20130701");
            cmd.Parameters.AddWithValue("@EndDate", "20130731");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[1];

            return Json(new { success = true, data = dt }, JsonRequestBehavior.AllowGet);
        }
    
    }

    public class ListPriceData
    {
        public string SalesModelCode { get; set; }
        public int SalesModelYear { get; set; }
        public string SalesModelDesc { get; set; }

        public string GroupPrice { get; set; }

        public System.Int64 RetailPriceIncludePPN { get; set; }
        public System.Int64 DiscPriceIncludePPN { get; set; }
        public System.Int64 NetPriceIncludePPN { get; set; }

        public System.Int64 RetailPriceExcludePPN { get; set; }
        public System.Int64 DiscPriceExcludePPN { get; set; }
        public System.Int64 NetPriceExcludePPN { get; set; }

        public System.Int64 PPNBeforeDisc { get; set; }
        public System.Int64 PPNAfterDisc { get; set; }      
  
        public System.Int64 OthersDPP { get; set; }
        public System.Int64 OthersPPN { get; set; }

        public System.Int64? PPNBMPaid { get; set; }

        public DateTime? EffectiveDate { get; set; }
        public bool? isStatus { get; set; }
    }

}