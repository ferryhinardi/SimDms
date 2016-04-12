using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.SUtility.Controllers.Api
{
    public class ComboController : BaseController
    {
        public JsonResult DataTypes()
        {
            var list = ctxDealer.SysDataTypes.Select(p => new { value = p.DataType, text = p.DataTypeDesc });
            return Json(list);
        }

        [HttpPost]
        public JsonResult Roles()
        {
            var list = ctx.SysRoles.Select(p => new { value = p.RoleId, text = p.RoleName });
            return Json(list); 
        }
        
        [HttpPost]
        public JsonResult DmsDownloadFileDataID()
        {
            var list = ctx.DmsDownloads.Select(x => new
            {
                value = x.DataID,
                text = x.DataID
            }).OrderBy(x => x.text).Distinct();

            return Json(list);
        }

        [HttpPost]
        public JsonResult DmsDownloadFileStatus()
        {
            var list = ctx.DmsDownloads.Select(x => new
            {
                value = x.Status,
                text = x.Status
            }).OrderBy(x => x.text).Distinct();

            return Json(list);
        }

        [HttpPost]
        public JsonResult DmsUploadFileDataID()
        {
            var list = ctx.DmsUploads.Select(x => new
            {
                value = x.DataID,
                text = x.DataID
            }).OrderBy(x => x.text).Distinct();

            return Json(list);
        }

        [HttpPost]
        public JsonResult DmsUploadFileStatus()
        {
            var list = ctx.DmsUploads.Select(x => new
            {
                value = x.Status,
                text = x.Status
            }).OrderBy(x => x.text).Distinct();

            return Json(list);
        }

        [HttpPost]
        public JsonResult DmsDownloadFileProductType()
        {
            var list = ctx.DmsDownloads.Select(x => new
            {
                value = x.ProductType,
                text = x.ProductType
            }).OrderBy(x => x.text).Distinct();

            return Json(list);
        }

        [HttpPost]
        public JsonResult DmsUploadFileProductType()
        {
            var list = ctx.DmsDownloads.Select(x => new
            {
                value = x.ProductType,
                text = x.ProductType
            }).OrderBy(x => x.text).Distinct();

            return Json(list);
        }
    }
}
