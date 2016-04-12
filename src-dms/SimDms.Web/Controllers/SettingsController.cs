using SimDms.Web.Models;
using SimDms.Web.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Web.Controllers
{
    public class SettingsController : BaseController
    {
        public JsonResult OverriddenSettings()
        {
            string hashLink = Request["HashLink"];
            ResultModel result = InitializeResultModel();

            if (string.IsNullOrEmpty(hashLink) == false)
            {
                string userID = CurrentUser.UserId;
                string roleID = ctx.SysRoleUsers.Where(x => x.UserId==userID).Select(x => x.RoleId).FirstOrDefault();
                int position = IndexOfChar(hashLink, '/', 1);
                string module = hashLink.Substring(0, position);
                string urlLink = hashLink.Substring(position+1, hashLink.Length - module.Length - 1);
                string menuID = ctx.SysMenus.Where(x => x.MenuUrl==urlLink).Select(x => x.MenuId).FirstOrDefault();
                IEnumerable<SysControlDms> controlDms = null;
                if (string.IsNullOrEmpty(menuID) == false)
                {
                    controlDms = ctx.SysControlDmses.Where(x => x.MenuID==menuID && (x.RoleID==roleID  || x.RoleID=="00000000000000000"));
                    int count = controlDms.Count();
                    if (controlDms != null && controlDms.Count() > 0)
                    {
                        result.data = controlDms;
                        result.success = true;
                    }
                }
            }
            else
            {
                result.message = "Sorry, we cannot determine any overridden settings.";
            }

            return Json(result);
        }
    }
}
