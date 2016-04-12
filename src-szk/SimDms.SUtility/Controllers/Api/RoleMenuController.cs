using SimDms.SUtility.Models;
using SimDms.SUtility.Models.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.SUtility.Controllers.Api
{
    public class RoleMenuController : BaseController
    {
        [HttpPost]
        public JsonResult Save(SysRoleMenu model)
        {
            ResultModel result = InitializeResult();
            string roleId = Request["RoleId"];
            
            var data = ctx.SysRoleMenus.Where(x => x.MenuID==model.MenuID && x.RoleID == roleId).FirstOrDefault();
            if (data == null)
            {
                data = new SysRoleMenu();
                data.RoleID = roleId;
                data.MenuID = model.MenuID;

                ctx.SysRoleMenus.Add(data);
            }

            if(model.MenuID.StartsWith("snis"))
            {
                var cisMenuId = ctx.SysMenus.Find(model.MenuID).MenuUrl;

                if(cisMenuId != null)
                {
                    var query = string.Format(@"insert into SdmsCis..CisMenuUserRoles values ('{0}','{1}')",cisMenuId,model.RoleID);
                    ctx.Database.ExecuteSqlCommand(query);
                }
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Menu has been assigned to selected role.";
            }
            catch (Exception)
            {
                result.message = "Menu cannot be assigned to selected role.";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(SysRoleMenu model)
        {
            ResultModel result = InitializeResult();
            string roleId = Request["RoleId"];

            var sysRoleModules = ctx.SysRoleMenus.Where(x => x.MenuID == model.MenuID && x.RoleID == roleId).FirstOrDefault();
            
            if (sysRoleModules != null)
            {
                ctx.SysRoleMenus.Remove(sysRoleModules);
            }

            if (model.MenuID.StartsWith("snis"))
            {
                var cisMenuId = ctx.SysMenus.Find(model.MenuID).MenuUrl;

                if (cisMenuId != null)
                {
                    var query = string.Format(@"delete SdmsCis..CisMenuUserRoles where menuid= '{0}' and roleid ='{1}'", cisMenuId, model.RoleID);
                    ctx.Database.ExecuteSqlCommand(query);
                }
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Menu has been unassigned to selected role.";
            }
            catch (Exception)
            {
                result.message = "Menu cannot be unassigned to selected role.";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult SaveModule(SysModule model)
        {
            ResultModel result = InitializeResult();
            string roleId = Request["RoleId"];

            object[] parameters = {roleId , model.ModuleId};
            var query = "exec uspfn_UtlInsertModule @p0,@p1";

            try
            {
                ctx.Database.ExecuteSqlCommand(query, parameters);
                result.status = true;
                result.message = "Menu has been assigned to selected role.";
            }
            catch (Exception)
            {
                result.message = "Menu cannot be assigned to selected role.";
            }

            return Json(result);
        }
    }
}
