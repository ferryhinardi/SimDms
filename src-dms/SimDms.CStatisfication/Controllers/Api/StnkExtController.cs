using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.CStatisfication.Models;
using System.Data.SqlClient;
using System.Data;

namespace SimDms.CStatisfication.Controllers
{
    public class StnkExtController : BaseController
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

        public ActionResult Save(CsStnkExt model)
        {
            var entity = ctx.StnkExtensions.Find(model.CompanyCode, model.CustomerCode, model.Chassis, model.StnkExpiredDate);
            if (entity == null)
            {
                entity = model;
                entity.CreatedBy = CurrentUser.UserId;
                entity.CreatedDate = DateTime.Now;
                ctx.StnkExtensions.Add(entity);
            }
            entity.IsStnkExtend = model.IsStnkExtend;
            entity.ReqKtp = model.ReqKtp;
            entity.ReqStnk = model.ReqStnk;
            entity.ReqBpkb = model.ReqBpkb;
            entity.ReqSuratKuasa = model.ReqSuratKuasa;
            entity.Comment = model.Comment;
            entity.Additional = model.Additional;
            entity.LastUpdatedBy = CurrentUser.UserId;
            entity.LastUpdatedDate = DateTime.Now;
            entity.Ownership = model.Ownership;
            entity.StnkDate = model.StnkDate;
            entity.BpkbDate = model.BpkbDate;
            entity.Status = model.Status;
            entity.Reason = model.Reason;
            //if (entity.IsStnkExtend == false)
            //{
            //    entity.Status = 1;
            //}
            //else
            //{
            //    entity.Status = ((model.ReqBpkb && model.ReqKtp && (model.ReqStnk || model.ReqSuratKuasa)) ? 1 : 0);
            //}

            if (entity.Status == 1)
            {
                if (entity.FinishDate == null)
                {
                    entity.FinishDate = DateTime.Now;
                }
            }

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
                //ctx.Database.ExecuteSqlCommand("exec uspfn_CsSaveCustVehicle @CompanyCode=@p0, @CustomerCode=@p1, @Chassis=p2, @StnkDate=@p3, @BpkbDate=p4, @UserId=p5", CompanyCode, model.CustomerCode, model.Chassis, model.StnkDate.Value.ToString("YYYY-MM-DD hh:mm:ss"), model.BpkbDate.Value.ToString("YYYY-MM-DD hh:mm:ss"), CurrentUser.UserId);
                //ctx.Database.ExecuteSqlCommand("exec uspfn_CsSaveCustVehicle @CompanyCode='" + CompanyCode + "', @CustomerCode='" + model.CustomerCode + "', @Chassis='" + model.Chassis + "', @StnkDate='" + model.StnkDate.Value.ToString("yyyy-MM-dd hh:mm:ss") + "', @BpkbDate='" + model.BpkbDate.Value.ToString("yyyy-MM-dd hh:mm:ss") + "', @UserId='" + CurrentUser.UserId + "'");
                //ctx.Database.ExecuteSqlCommand("exec uspfn_CRUDSTNKExtResource @CompanyCode='" + CompanyCode + "', @CustomerCode='" + model.CustomerCode + "', @Chassis='" + model.Chassis + "'");

                return Json(new { success = true });
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
                cmd.CommandText = "uspfn_CsGetStnkExt";
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

        public ActionResult Delete(CsStnkExt model)
        {
            var entity = ctx.StnkExtensions.Find(model.CompanyCode, model.CustomerCode, model.Chassis, model.StnkExpiredDate);
            if (entity != null)
            {
                ctx.StnkExtensions.Remove(entity);
                try
                {
                    ctx.SaveChanges();
                    //ctx.Database.SqlQuery<object>("exec uspfn_CRUDSTNKExtResource @CompanyCode=@p0, @CustomerCode=@p1, @Chassis=p2", CompanyCode, model.CustomerCode, model.Chassis);
                    //ctx.Database.ExecuteSqlCommand("exec uspfn_CRUDSTNKExtResource @CompanyCode=@p0, @CustomerCode=@p1, @Chassis=p2", CompanyCode, model.CustomerCode, model.Chassis);
                    //ctx.Database.ExecuteSqlCommand("exec uspfn_CRUDSTNKExtResource @CompanyCode='" + CompanyCode + "', @CustomerCode='" + model.CustomerCode + "', @Chassis='" + model.Chassis + "'");

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = true, message = ex.Message });
                }
            }
            return Json(new { success = true, message = "data not valid" });
        }
    }
}
