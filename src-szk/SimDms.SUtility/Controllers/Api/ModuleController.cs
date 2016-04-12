using SimDms.SUtility.Models;
using SimDms.SUtility.Models.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.SUtility.Controllers.Api
{
    public class ModuleController : BaseController
    {
        [HttpPost]
        public JsonResult Save(SysModule model)
        {
            ResultModel result = InitializeResult();

            var data = ctx.SysModules.Find(model.ModuleId);

            if (data == null)
            {
                data = new SysModule();
                data.ModuleId = model.ModuleId;

                ctx.SysModules.Add(data);
            }

            data.ModuleCaption = model.ModuleCaption;
            data.ModuleIndex = model.ModuleIndex;
            data.ModuleUrl = model.ModuleUrl;
            data.IsPublish = model.IsPublish;
            data.InternalLink = model.InternalLink;

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Module has been saved.";
            }
            catch (Exception)
            {
                result.message = "Sorry, module cannot be saved into database.\nPlease, try again later!";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(SysModule model)
        {
            ResultModel result = InitializeResult();

            var module = ctx.SysModules.Find(model.ModuleId);
            var roleModule = ctx.SysRoleModules.Where(x => x.ModuleID==model.ModuleId);
            if (module != null)
            {
                ctx.SysModules.Remove(module);

                foreach (var item in roleModule)
                {
                    ctx.SysRoleModules.Remove(item);
                }

                try
                {
                    ctx.SaveChanges();

                    result.status = true;
                    result.message = "Module has been deleted.";
                }
                catch (Exception)
                {
                    result.message = "Sorry, module cannot be removed from database.\nPlease, try again later!";
                }
            }
            else
            {
                result.message = "There is no data to be deleted.";
            }

            return Json(result);
        }
    }
}
