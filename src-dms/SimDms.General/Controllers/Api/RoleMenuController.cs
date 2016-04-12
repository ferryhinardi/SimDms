//using SimDms.General.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using SimDms.General.Models.Others;
using System.Data.Entity.Validation;
using System.Diagnostics;
using SimDms.Common.Models;

namespace SimDms.General.Controllers.Api
{
    public class RoleMenuController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                IsAdmin = false,
                IsActive = true
            });
        }

        [HttpPost]
        public JsonResult Search(SysRoleMenu model)
        {
            //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            //cmd.CommandText = "uspfn_GnSearchRoleMenu";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@RoleId", Request["RoleId"]);

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();
            //    cmd.Connection.Close();

            return Json(new { success = true, data = "success" });
            //}
            //catch (Exception ex)
            //{
            //    return Json(new { success = false, message = ex.Message });
            //}
        }

        public JsonResult Save(SysRoleMenuModel model)
        {
            ResultModel result = InitializeResultModel();
            string roleID = Request["RoleId"] ?? "";

            if (model.MenuID != null)
            {
                foreach (var item in model.MenuID)
                {
                    var data = ctx.SysRoleMenus.Where(x => x.RoleId == roleID && x.MenuId == item).FirstOrDefault();
                    if (data == null)
                    {
                        data = new SysRoleMenu();
                        data.RoleId = roleID;
                        data.MenuId = item;
                        ctx.SysRoleMenus.Add(data);
                    }
                }
            }

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Menus has been assigned to this role.";
            }
            //catch (DbEntityValidationException dbEx)
            //{
            //    foreach (var validationErrors in dbEx.EntityValidationErrors)
            //    {
            //        foreach (var validationError in validationErrors.ValidationErrors)
            //        {
            //            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
            //        }
            //    }
            //}
            catch
            {
                result.message = "Cannot assigning into user.\nPlease, try again later!";
            }

            return Json(result);
        }

        public JsonResult Delete(SysRoleMenu model)
        {
            ResultModel result = InitializeResultModel();

            var record = ctx.SysRoleMenus.Find(model.RoleId, model.MenuId);
            if (record != null)
            {
                ctx.SysRoleMenus.Remove(record);

                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Menu has been unsassigned from selected user.";
                }
                catch (Exception)
                {
                    result.message = "Cannot delete assigned menu from seleced user.\nPlease, try again later!";
                }
            }

            return Json(result);
        }

        public JsonResult UnassignMenus(SysRoleMenuModel models)
        {
            ResultModel result = InitializeResultModel();

            if (models.MenuID != null)
            {
                foreach (var item in models.MenuID)
                {
                    var data = ctx.SysRoleMenus.Where(x => x.RoleId == models.RoleID && x.MenuId == item).FirstOrDefault();

                    if (data != null)
                    {
                        ctx.SysRoleMenus.Remove(data);
                    }
                }
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Menus has been unassigned from this user.";
            }
            catch (Exception)
            {
                result.message = "Sorry, menus cannot be unassigned from this role.";
            }

            return Json(result);
        }
    }
}
