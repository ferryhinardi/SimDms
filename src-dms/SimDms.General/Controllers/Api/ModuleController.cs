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
    public class ModuleController : BaseController
    {
        [HttpPost]
        public JsonResult Save(SysModule model)
        {
            ResultModel result = InitializeResultModel();

            var data = ctx.SysModules.Find(model.ModuleID);

            if (data == null)
            {
                data = new SysModule();
                data.ModuleID = model.ModuleID;
                ctx.SysModules.Add(data);
            }

            data.ModuleCaption = model.ModuleCaption;
            data.ModuleIndex = model.ModuleIndex;
            data.ModuleUrl = model.ModuleUrl;
            data.InternalLink = model.InternalLink;
            data.IsPublish = model.IsPublish;

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Your data has been saved.";
            }
            catch (Exception)
            {
                result.message = "Sorry, your data cannot be saved.\nPlease, try again later.";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(SysModule model)
        {
            ResultModel result = InitializeResultModel();
            var data = ctx.SysModules.Find(model.ModuleID);

            if (data != null)
            {
                ctx.SysModules.Remove(data);
            }

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Your data has been deleted.";
            }
            catch (Exception)
            {
                result.message = "Sorry, cannot delete module data.\nPlease, try again later.";
            }

            return Json(result);
        }
    }
}
