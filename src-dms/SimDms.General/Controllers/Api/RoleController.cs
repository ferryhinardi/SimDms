using SimDms.General.Models;
using SimDms.General.Models.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common.Models;

namespace SimDms.General.Controllers.Api
{
    public class RoleController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                IsAdmin = false,
                IsActive = true,
                IsChangeBranchCode = false
            });
        }

        public JsonResult Save(SysRole model)
        {
            ResultModel result = InitializeResultModel();

            var record = ctx.SysRoles.Where(x => x.RoleId == model.RoleId).FirstOrDefault();
            if (record == null)
            {
                record = model;
                ctx.SysRoles.Add(record);
            }
            record.RoleName = model.RoleName;
            record.Themes = model.Themes ?? "";
            record.IsAdmin = model.IsAdmin;
            record.IsActive = model.IsActive;
            record.IsChangeBranchCode = model.IsChangeBranchCode;

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Role has been saved into database.";
            }
            catch
            {
                result.message = "Role cannot be saved into database.";
            }

            return Json(result);
        }

        public JsonResult Delete(SysRole model)
        {
            ResultModel result = InitializeResultModel();
            var usedRole = ctx.SysRoleUsers.Where(x => x.RoleId==model.RoleId);

            if (usedRole != null && usedRole.Count() > 0)
            {
                result.message = "Sorry, role data still used as user's role.";
                return Json(result);
            }

            var roles = ctx.SysRoles.Where(x => x.RoleId==model.RoleId);

            if (roles != null)
            {
                foreach (var item in roles)
                {
                    ctx.SysRoles.Remove(item);
                }

                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Role data has been deleted.";
                }
                catch (Exception ex)
                {
                    result.message = "Sorry, system cannot delete role data.\nPlease try again later.";
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return Json(result);
        }

        public JsonResult IsChangeBranch()
        {
            string roleID = CurrentRole();
            var role = ctx.SysRoles.Find(roleID);

            return Json(new { 
                IsChangeBranchCode = role.IsChangeBranchCode            
            });
        }
    }
}
