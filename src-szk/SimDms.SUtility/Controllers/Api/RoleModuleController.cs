using SimDms.SUtility.Models;
using SimDms.SUtility.Models.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.SUtility.Controllers.Api
{
    public class RoleModuleController : BaseController
    {
        [HttpPost]
        public JsonResult Save(SysRoleModule model)
        {
            ResultModel result = InitializeResult();
            string roleId = Request["RoleId"];

            var data = ctx.SysRoleModules.Where(x => x.ModuleID==model.ModuleID && x.RoleID == roleId).FirstOrDefault();
            if (data == null)
            {
                data = new SysRoleModule();
                data.RoleID = roleId;
                data.ModuleID = model.ModuleID;

                ctx.SysRoleModules.Add(data);
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Module has been assigned to selected role.";
            }
            catch (Exception)
            {
                result.message = "Module cannot be assigned to selected role.";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(SysRoleModule model)
        {
            ResultModel result = InitializeResult();
            string roleId = Request["RoleId"];

            var sysRoleModules = ctx.SysRoleModules.Where(x => x.ModuleID == model.ModuleID && x.RoleID == roleId).FirstOrDefault();
            
            if (sysRoleModules != null /* && sysRoleModules.Count() > 0 */)
            {
                //foreach (var item in sysRoleModules)
                //{
                //    ctx.SysRoleModules.Remove(item);
                //}

                ctx.SysRoleModules.Remove(sysRoleModules);

            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Module has been unassigned to selected role.";
            }
            catch (Exception)
            {
                result.message = "Module cannot be unassigned to selected role.";
            }

            return Json(result);
        }

    }
}
