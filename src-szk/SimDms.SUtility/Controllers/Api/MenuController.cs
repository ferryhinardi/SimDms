using SimDms.SUtility.Models;
using SimDms.SUtility.Models.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.SUtility.Controllers.Api
{
    public class MenuController : BaseController
    {
        [HttpPost]
        public JsonResult Save(SysMenu model)
        {
            ResultModel result = InitializeResult();

            var data = ctx.SysMenus.Find(model.MenuId);

            if (data == null)
            {
                data = new SysMenu();
                data.MenuId = model.MenuId;

                ctx.SysMenus.Add(data);
            }

            data.MenuCaption = model.MenuCaption;
            data.MenuHeader = model.MenuHeader;
            data.MenuIndex = model.MenuIndex;
            data.MenuLevel = model.MenuLevel;
            data.MenuUrl = model.MenuUrl;

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Menu has been saved into database.";
            }
            catch (Exception)
            {
                result.message = "Sorry, menu cannot be saved into database.\nPlease, try again later!";
            }
            

            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(SysMenu model)
        {
            ResultModel result = InitializeResult();

            var menu = ctx.SysMenus.Where(x => x.MenuId==model.MenuId);
            var roleMenus = ctx.SysRoleMenus.Where(x => x.MenuID==model.MenuId);

            if (menu != null)
            {
                foreach (SysMenu item in menu)
                {
                    ctx.SysMenus.Remove(item);
                }

                foreach (SysRoleMenu roleMenu in roleMenus)
                {
                    ctx.SysRoleMenus.Remove(roleMenu);
                }
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Menu has been removed from database.";
            }
            catch (Exception)
            {
                result.message = "Menu cannot be removed from database.";
            }
            

            return Json(result);
        }

        [HttpPost]
        public JsonResult Headers()
        {
            var list = ctx.SysMenus.Select(p => new { value = p.MenuId, text = p.MenuCaption + " (" + p.MenuId + ")" }).OrderBy(x => x.text);
            return Json(list);
        }
    }
}
