using SimDms.SUtility.Models;
using SimDms.SUtility.Models.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.SUtility.Controllers.Api
{
    public class RoleController : BaseController
    {
        [HttpPost]
        public JsonResult Default()
        {
            return Json(new
            {
                IsAdmin = false,
                IsActive = true
            });
        }

        [HttpPost]
        public JsonResult Save(SysRole model)
        {
            ResultModel result = InitializeResult();

            var data = ctx.SysRoles.Find(model.RoleId);

            if (data == null)
            {
                data = new SysRole();
                data.RoleId = model.RoleId;

                ctx.SysRoles.Add(data);
            }

            data.RoleName = model.RoleName;
            data.Description = model.Description;

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Role has been saved into database.";
            }
            catch (Exception)
            {
                result.message = "Role cannot be saved into database.\nPlease, try again later!";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(SysRole model)
        {
            ResultModel result = InitializeResult();

            var role = ctx.SysRoles.Find(model.RoleId);
            var roleMenus = ctx.SysRoleMenus.Where(x => x.RoleID==model.RoleId);
            var roleModules = ctx.SysRoleModules.Where(x => x.RoleID==model.RoleId);
            var roleUsers = ctx.SysRoleUsers.Where(x => x.RoleId==model.RoleId);

            if (role != null)
            {
                if(roleMenus.Where(a=>a.MenuID.StartsWith("snis")).Count() > 0)
                {
                    var query = string.Format(@"delete SdmsCis..CisMenuUserRoles where roleid ='{0}'", model.RoleId);
                    ctx.Database.ExecuteSqlCommand(query);
                }

                ctx.SysRoles.Remove(role);

                foreach (var item in roleMenus)
                {
                    ctx.SysRoleMenus.Remove(item);
                }

                foreach (var item in roleModules)
                {
                    ctx.SysRoleModules.Remove(item);
                }

                foreach (var item in roleUsers)
                {
                    ctx.SysRoleUsers.Remove(item);
                }
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Role has been deleted from database.";
            }
            catch (Exception)
            {
                result.message = "Role canoot be removed from database.\nPlease, try again later!";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult List()
        {
            var list = ctx.SysRoles.Select(p => new { value = p.RoleId, text = p.RoleName }).OrderBy(x => x.text);
            return Json(list);
        }
    }
}
