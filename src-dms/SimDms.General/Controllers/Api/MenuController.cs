using SimDms.General.Models;
using SimDms.General.Models.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.General.Controllers.Api
{
    public class MenuController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName
            });
        }

        public JsonResult Save(SysMenu model)
        {
            ResultModel result = InitializeResultModel();

            var record = ctx.SysMenus.Find(model.MenuId);
            if (record == null)
            {
                record = model;
                ctx.SysMenus.Add(record);
            }
            record.MenuCaption = model.MenuCaption;
            record.MenuHeader = model.MenuHeader;
            record.MenuIndex = model.MenuIndex;
            record.MenuLevel = model.MenuLevel;
            record.MenuUrl = model.MenuUrl;
            record.MenuIcon = model.MenuIcon;

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Menu has been saved into database.";
            }
            catch
            {
                result.message = "Sorry, menu cannot be saved into database.\nPlease, try again later!";
            }
            return Json(result);
        }

        public JsonResult Delete(SysMenu model)
        {
            ResultModel result = InitializeResultModel();

            var record = ctx.SysMenus.Find(model.MenuId);
            if (record != null)
            {
                ctx.SysMenus.Remove(record);

                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Menu has been deleted.";
                }
                catch (Exception)
                {
                    result.message = "Sorry, cannot delete menu.\nPlease, try again later!";
                }
            }

            return Json(result);
        }
    }
}
