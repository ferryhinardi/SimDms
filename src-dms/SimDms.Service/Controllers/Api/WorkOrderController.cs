using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;

namespace SimDms.Service.Controllers.Api
{
    public class WorkOrderController : BaseController
    {
        //
        // GET: /WorkOrder/

        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                ProductType = ProductType
            });
        }

        public JsonResult Get(JobOrder model)
        {
            var jobOrder = ctx.JobOrderViews.FirstOrDefault(x => x.CompanyCode == CompanyCode
                      && x.BranchCode == BranchCode
                      && x.ProductType == ProductType
                      && x.ServiceNo == model.ServiceNo);
            if (jobOrder == null) return Json(new { success = false, message = "data not found" });

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            cmd.CommandText = "uspfn_SvTrnInvoiceDraft";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@JobOrderNo", (jobOrder.ServiceType == "0" ? jobOrder.EstimationNo :
                jobOrder.ServiceType == "1" ? jobOrder.BookingNo : jobOrder.JobOrderNo));
            SqlDataAdapter daHdr = new SqlDataAdapter(cmd);
            DataTable dtHdr = new DataTable();
            daHdr.Fill(dtHdr);

            cmd.CommandText = "uspfn_SvTrnServiceSelectDtl";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", dtHdr.Rows[0]["ProductType"]);
            cmd.Parameters.AddWithValue("@ServiceNo", dtHdr.Rows[0]["ServiceNo"]);
            SqlDataAdapter daDtl = new SqlDataAdapter(cmd);
            DataTable dtDtl = new DataTable();
            daDtl.Fill(dtDtl);

            var header = GetJson(dtHdr)[0];
            var detail = GetJson(dtDtl);

            return Json(new { success = true, data = header, list = detail });
        }

    }
}
