using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.CStatisfication.Models;
using System.Data.SqlClient;
using System.Data;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class BpkbController : BaseController
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

        public ActionResult Save(CsCustBpkb model, DateTime? StnkDate, DateTime? BpkbDate)
        {
            DateTime currentTime = DateTime.Now;

            //if (model.BpkbPickUp < model.BpkbReadyDate)
            //{
            //    return Json(new { success = false, message = "BPKB pickup date cannot less than BPKB ready date " });
            //}

            if (//(model.Status > 0) && (!model.ReqKtp || !model.ReqStnk || model.BpkbPickUp == null) || 
                (model.Status < 1 && model.Reason == ""))
            {
                return Json(new { success = false, message = "Data cannot be saved " });
            }

            var entity = ctx.CustBPKBs.Find(model.CompanyCode, model.CustomerCode, model.Chassis);
            if (entity == null)
            {
                entity = model;
                entity.CreatedBy = CurrentUser.UserId;
                entity.CreatedDate = currentTime;
                ctx.CustBPKBs.Add(entity);
            }
            entity.BpkbReadyDate = model.BpkbReadyDate;
            entity.BpkbPickUp = model.BpkbPickUp;
            entity.ReqInfoLeasing = model.ReqInfoLeasing;
            entity.ReqInfoCust = model.ReqInfoCust;
            entity.ReqKtp = model.ReqKtp;
            entity.ReqStnk = model.ReqStnk;
            entity.ReqSuratKuasa = model.ReqSuratKuasa;
            entity.Comment = model.Comment;
            entity.Additional = model.Additional;
            entity.UpdatedBy = CurrentUser.UserId;
            entity.UpdatedDate = currentTime;
            entity.Status = model.Status;

            if (entity.Status == 1)
            {
                if (entity.FinishDate == null)
                {
                    entity.FinishDate = currentTime;
                }
                entity.Reason = null;
            }
            else
            {
                entity.Reason = model.Reason;
                entity.BpkbPickUp = null;
            }

            //if (DelayedRetrievalDate != null)
            //{
            //    var pendingRetrieval = ctx.CsBpkbRetrievalInformations.Find(CompanyCode, model.CustomerCode, DelayedRetrievalDate);
            //    if (pendingRetrieval == null)
            //    {
            //        pendingRetrieval = new CsBpkbRetrievalInformation();
            //        pendingRetrieval.CompanyCode = CompanyCode;
            //        pendingRetrieval.CustomerCode = model.CustomerCode;
            //        pendingRetrieval.RetrievalEstimationDate = DelayedRetrievalDate;
            //        pendingRetrieval.CreatedBy = CurrentUser.UserId;
            //        pendingRetrieval.CreatedDate = currentTime;

            //        ctx.CsBpkbRetrievalInformations.Add(pendingRetrieval);
            //    }

            //    pendingRetrieval.Notes = DelayedRetrievalNote;
            //    pendingRetrieval.UpdatedBy = CurrentUser.UserId;
            //    pendingRetrieval.UpdatedDate = currentTime;
            //}

            try
            {
                ctx.SaveChanges();
                //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                //cmd.Connection.Open();
                //cmd.CommandText = "uspfn_CsSaveCustVehicle";
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.Clear();
                //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                //cmd.Parameters.AddWithValue("@CustomerCode", model.CustomerCode);
                //cmd.Parameters.AddWithValue("@Chassis", model.Chassis);
                //cmd.Parameters.AddWithValue("@StnkDate", StnkDate);
                //cmd.Parameters.AddWithValue("@BpkbDate", BpkbDate);
                //cmd.Parameters.AddWithValue("@UserId", CurrentUser.UserId);
                //cmd.ExecuteNonQuery();
                //cmd.Connection.Close();
                //cmd.Connection.Dispose();

                var result1 = ctx.Database.ExecuteSqlCommand("exec uspfn_CsSaveCustVehicle @CompanyCode=@p0, @CustomerCode=@p1, @Chassis=@p2, @StnkDate=@p3, @BpkbDate=@p4, @UserId=@p5", CompanyCode, model.CustomerCode, model.Chassis, StnkDate, BpkbDate, CurrentUser.UserId);
                var result2 = ctx.Database.ExecuteSqlCommand("exec uspfn_UpdateValidityCsBpkbRetrievalInformation @CompanyCode=@p0, @CustomerCode=@p1, @BpkbReadyDate=@p2", CompanyCode, model.CustomerCode, model.BpkbReadyDate);
                var result3 = ctx.Database.ExecuteSqlCommand("exec uspfn_CRUDBPKPReminderResource  @CompanyCode=@p0, @CustomerCode=@p1, @Chassis=@p2", CompanyCode, model.CustomerCode, model.Chassis);

                return Json(new { success = true, message = "Data has been saved." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult Get(CsStnkExt model)
        {
            try
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "uspfn_CsGetBpkb";
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

        public ActionResult GetRetrievalEstimation(string CompanyCode, string CustomerCode)
        {
            try
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = @"select top 1 * from CsBpkbRetrievalInformation a
	                             where a.CompanyCode = @CompanyCode
	                               and a.CustomerCode = @CustomerCode
	                               and ( a.IsDeleted = 0 or a.IsDeleted is null)
                                   and a.RetrievalEstimationDate > getdate()";

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                cmd.Parameters.AddWithValue("@CustomerCode", CustomerCode);

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

        public JsonResult Delete(CsCustBpkb model, DateTime? DelayedRetrievalDate)
        {
            var csCustBpkb = ctx.CustBPKBs.Find(model.CompanyCode, model.CustomerCode, model.Chassis);
            //var pendingRetrieval = ctx.CsBpkbRetrievalInformations.Find(model.CompanyCode, model.CustomerCode, DelayedRetrievalDate);

            if (csCustBpkb != null)
            {
                ctx.CustBPKBs.Remove(csCustBpkb);
                //csCustBpkb.IsDeleted = true;
            }

            //if (pendingRetrieval != null)
            //{
            //    //pendingRetrieval.IsDeleted = true;
            //    ctx.CsBpkbRetrievalInformations.Remove(pendingRetrieval);
            //}

            try
            {
                ctx.SaveChanges();
                ctx.Database.ExecuteSqlCommand("exec uspfn_CsRemovePendingBpkbRetrieval @CompanyCode=@p0, @CustomerCode=@p1", CompanyCode, model.CustomerCode);
                ctx.Database.ExecuteSqlCommand("exec uspfn_CRUDBPKPReminderResource @CompanyCode=@p0, @CustomerCode=@p1, @Chassis=@p2", CompanyCode, model.CustomerCode, model.Chassis);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            //return Json(new { success = false, message = "data not valid" });
        }
    }
}
