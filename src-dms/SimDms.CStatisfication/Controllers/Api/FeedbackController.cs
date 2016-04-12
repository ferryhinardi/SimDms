using SimDms.CStatisfication.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class FeedbackController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
            });
        }

        public ActionResult Save(CsCustFeedback model)
        {
            var record = ctx.CsCustFeedbacks.Find(model.CompanyCode, model.CustomerCode, model.Chassis);
            if (record == null)
            {
                record = model;
                record.CreatedBy = CurrentUser.UserId;
                record.CreatedDate = DateTime.Now;
                ctx.CsCustFeedbacks.Add(record);
            }
            record.IsManual = model.IsManual;
            record.FeedbackA = model.FeedbackA;
            record.FeedbackB = model.FeedbackB;
            record.FeedbackC = model.FeedbackC;
            record.FeedbackD = model.FeedbackD;
            record.Status = model.Status;
            record.Reason = model.Reason;
            record.UpdatedBy = CurrentUser.UserId;
            record.UpdatedDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult Get(CsCustFeedback model)
        {
            try
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "uspfn_CsGetFeedback";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CompanyCode", model.CompanyCode);
                cmd.Parameters.AddWithValue("@CustomerCode", model.CustomerCode);
                cmd.Parameters.AddWithValue("@Chassis", model.Chassis);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return Json(new { success = true, data = GetJsonRow(dt) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(CsCustFeedback model)
        {
            var record = ctx.CsCustFeedbacks.Find(CompanyCode, model.CustomerCode, model.Chassis);
            if (record != null)
            {
                ctx.CsCustFeedbacks.Remove(record);

                try
                {
                    ctx.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return Json(new { success = false, message = "data not valid" });
        }
    }
}
