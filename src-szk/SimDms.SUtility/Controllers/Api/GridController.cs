using SimDms.SUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;

namespace SimDms.SUtility.Controllers.Api
{
    public class GridController : BaseController
    {
        [HttpPost]
        public JsonResult Modules()
        {
            var queryable = ctx.SysModules.AsQueryable();

            string roleId = Request["RoleId"];

            if (string.IsNullOrEmpty(roleId) == false)
            {
                var assignedModule = ctx.SysRoleModules.Where(x => x.RoleID == roleId).Select(x => x.ModuleID);
                queryable = ctx.SysModules.Where(x => assignedModule.Contains(x.ModuleId) == false).AsQueryable();
            }

            return Json(queryable.DGrid());
            //return Json(queryable);
        }

        [HttpPost]
        public JsonResult Menus()
        {
            var queryable = ctx.SysMenuViews.AsQueryable();

            string roleId = Request["RoleId"];

            if (string.IsNullOrEmpty(roleId) == false)
            {
                var assignedMenu = ctx.SysRoleMenus.Where(x => x.RoleID == roleId).Select(x => x.MenuID).AsQueryable();

                queryable = queryable.Where(x => assignedMenu.Contains(x.MenuId) == false).AsQueryable();
            }

            return Json(GeLang.DataTables<SysMenuView>.Parse(queryable, Request));
            //return Json(queryable);
        }

        [HttpPost]
        public JsonResult Menus_DataTable()
        {
            var queryable = ctx.SysMenuViews.AsQueryable();

            string roleId = Request["RoleId"];

            if (string.IsNullOrEmpty(roleId) == false)
            {
                var assignedMenu = ctx.SysRoleMenus.Where(x => x.RoleID == roleId).Select(x => x.MenuID).AsQueryable();

                queryable = queryable.Where(x => assignedMenu.Contains(x.MenuId) == false).AsQueryable();
            }

            return Json(GeLang.DataTables<SysMenuView>.Parse(queryable, Request));
            //return Json(queryable);
        }

        [HttpPost]
        public JsonResult Modules_DataTable()
        {
            string[] modules = {"ms"};
            var queryable = ctx.SysModules.Where(a=> !modules.Contains(a.ModuleId)).AsQueryable();

            return Json(GeLang.DataTables<SysModule>.Parse(queryable, Request));
        }

        [HttpPost]
        public JsonResult Roles()
        {
            var queryable = ctx.SysRoles;
            //return Json(GeLang.DataTables<SysRole>.Parse(queryable, Request));
            return Json(queryable);
        }

        [HttpPost]
        public JsonResult MenuRoles()
        {
            var queryable = ctx.SysRoles;
            return Json(GeLang.DataTables<SysRole>.Parse(queryable, Request));
        }

        [HttpPost]
        public JsonResult Users()
        {
            var queryable = ctx.SysUserViews;
            return Json(queryable.DGrid());
            //return Json(queryable);
        }

        [HttpPost]
        public JsonResult Tasks()
        {
            var queryable = ctx.SQLGateway.Select(
                    x => new {  TaskNo = x.TaskNo, TaskName = x.TaskName, DealerCode = x.DealerCode,FileName = x.FileName,  UserId = x.UserId , Status = x.Status, CreatedDate = x.CreatedDate }
                );
            return Json(queryable.DGrid());
            //return Json(queryable);
        }


        [HttpPost]
        public JsonResult RoleModules()
        {
            string roleId = Request["RoleId"];
            var queryable = (from x in ctx.SysRoleModuleViews
                             where x.RoleID == roleId
                             select x).Distinct().OrderBy(x => x.ModuleCaption);

            return Json(GeLang.DataTables<SysRoleModuleView>.Parse(queryable, Request));
            //return Json(queryable);
        }

        [HttpPost]
        public JsonResult RoleMenus()
        {
            string roleId = Request["RoleId"];
            var queryable = (from x in ctx.SysRoleMenuViews
                             where x.RoleID == roleId
                             select x).Distinct();

            return Json(GeLang.DataTables<SysRoleMenuView>.Parse(queryable, Request));
        }

        //    [HttpPost]
        //    [Authorize]
        //    public JsonResult DownloadFiles(string DealerCode, string DataID, string Status, DateTime? StartDate, DateTime? EndDate)
        //    {
        //        IQueryable<DcsDownload> list = ctx.DcsDownloads;

        //        if (string.IsNullOrEmpty(DealerCode) == false)
        //        {
        //            list = list.Where(x => x.CustomerCode.Equals(DealerCode));    
        //        }

        //        if (string.IsNullOrEmpty(DataID) == false)
        //        {
        //            list = list.Where(x => x.DataID.Equals(DataID));
        //        }

        //        if (string.IsNullOrEmpty(Status) == false)
        //        {
        //            list = list.Where(x => x.Status.Equals(Status));
        //        }

        //        if (StartDate != null)
        //        {
        //            list = list.Where(x => x.CreatedDate >= StartDate.Value);
        //        }

        //        if (EndDate != null)
        //        {
        //            list = list.Where(x => x.CreatedDate <= EndDate.Value);
        //        }

        //        list = list.OrderBy(x => x.CreatedDate);

        //        return Json(list);
        //    }

        [HttpPost]
        public JsonResult DmsDownloadFile()
        {
            string customerCode = Request["CustomerCode"];
            string status  = Request["Status"];
            string dataID = Request["DataID"];
            string productType = Request["ProductType"];

            var data = ctx.DmsDownloadFileViews.AsQueryable();

            if (string.IsNullOrEmpty(customerCode) == false)
            {
                data = data.Where(x => x.CustomerCode==customerCode);
            }
            if (string.IsNullOrEmpty(status) == false)
            {
                data = data.Where(x => x.Status == status);
            }
            if (string.IsNullOrEmpty(dataID) == false)
            {
                data = data.Where(x => x.DataID == dataID);
            }
            if (string.IsNullOrEmpty(productType) == false)
            {
                data = data.Where(x => x.ProductType == productType);
            }

            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult DmsUploadFile()
        {
            string customerCode = Request["CustomerCode"];
            string status = Request["Status"];
            string dataID = Request["DataID"];
            string productType = Request["ProductType"];

            var data = ctx.DmsUploadFileViews.AsQueryable();

            if (string.IsNullOrEmpty(customerCode) == false)
            {
                data = data.Where(x => x.CustomerCodeBilling == customerCode);
            }
            if (string.IsNullOrEmpty(status) == false)
            {
                data = data.Where(x => x.Status == status);
            }
            if (string.IsNullOrEmpty(dataID) == false)
            {
                data = data.Where(x => x.DataID == dataID);
            }
            if (string.IsNullOrEmpty(productType) == false)
            {
                data = data.Where(x => x.ProductType == productType);
            }

            return Json(data.KGrid());
        }

        public JsonResult RefferenceType() 
        {
            var queryable = ctx.svMstRefferenceService.AsQueryable();

            string refferenceType = Request["RefferenceType"];

            if (string.IsNullOrEmpty(refferenceType) == false)
            {
                queryable = ctx.svMstRefferenceService.Where(x => x.RefferenceType.Contains(refferenceType) == false).AsQueryable();
            }

            return Json(queryable.DGrid());
            //return Json(queryable);
        }
    }
}
