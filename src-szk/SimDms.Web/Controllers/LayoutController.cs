using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SimDms.Web.Controllers
{
    public class LayoutController : BaseController
    {
        public ActionResult Index()
        {
            var UseRS = System.Configuration.ConfigurationManager.AppSettings["UseReportServer"].ToString();
            string Url = System.Configuration.ConfigurationManager.AppSettings["LocalReportServer"].ToString();
            if (UseRS == "true")
            {
                Url = Request.Url.ToString().ToLower();
                Url = Url.Contains("dms.suzuki.co.id") ? "http://dms.suzuki.co.id/reportserver/" : "http://tbsdmsap01/reportserver/";
            }

            ViewBag.ReportAddress = Url;
            ViewBag.CurrentUser = "";

            if (CurrentUser != null)
            {
            ViewBag.CurrentUser = CurrentUser.Username;

            }

            return View();
        }

        public JsonResult GetMenu(string id)
        {
            var menu = ctx.SysMenus.Find(id);
            if (menu == null)
            {
                return Json(new { success = false, message = "menu not define yet" });
            }
            else
            {
                return Json(new { success = true, data = menu });
            }
        }

        public ActionResult ListMenu(string id)
        {
            var list = new List<SysMenu>();
            ctx.Configuration.LazyLoadingEnabled = false;
            var userID = CurrentUser.UserId;
            var roleUser = ctx.SysRoleUsers.Where(x => x.UserId == userID).FirstOrDefault();

            string role = "";
            if (roleUser != null)
            {
                role = roleUser.RoleId.ToString();                
            }
            var availabeMenus = ctx.SysRoleMenus.Where(x => x.RoleID==role).Select(x => x.MenuID);
            var menus = ctx.SysMenus.Where(x =>
                    availabeMenus.Contains(x.MenuId)
                );


            foreach (var item in menus.Where(p => p.MenuHeader == id).OrderBy(p => p.MenuIndex).ToList())
            {
                list.Add(item);

                // add detail level 1
                if (menus.Where(p => p.MenuHeader == item.MenuId).Count() > 0)
                {
                    item.Detail = new List<SysMenu>();
                    foreach (var child1 in menus.Where(p => p.MenuHeader == item.MenuId).OrderBy(p => p.MenuIndex).ToList())
                    {
                        item.Detail.Add(child1);

                        // add detail level 2
                        if (menus.Where(p => p.MenuHeader == child1.MenuId).Count() > 0)
                        {
                            child1.Detail = new List<SysMenu>();
                            foreach (var child2 in menus.Where(p => p.MenuHeader == child1.MenuId).OrderBy(p => p.MenuIndex).ToList())
                            {
                                child1.Detail.Add(child2);

                                // add detail level 3
                                if (menus.Where(p => p.MenuHeader == child2.MenuId).Count() > 0)
                                {
                                    child2.Detail = new List<SysMenu>();
                                    foreach (var child3 in menus.Where(p => p.MenuHeader == child2.MenuId).OrderBy(p => p.MenuIndex).ToList())
                                    {
                                        child2.Detail.Add(child3);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return Json(new { success = true, data = list });
        }

        public JsonResult ListModules()
        {
            List<SysModule> data = null;
            string SessionId = "";

            try
            {
            var session = ctx.SysSessions.Where(p => p.SessionUser == User.Identity.Name).OrderBy(p => p.CreateDate).FirstOrDefault();
            if (session != null)
            {
                SessionId = session.SessionId;
            }
            var userID = CurrentUser.UserId;
            var roleUser = ctx.SysRoleUsers.Where(x => x.UserId == userID).FirstOrDefault();
            string role = "";
            if (roleUser != null)
            {
                role = roleUser.RoleId.ToString();
            }
            var availableModules = ctx.SysRoleModules.Where(x => x.RoleID==role).Select(x => x.ModuleID);

            data = ctx.SysModules.Where(p => availableModules.Contains(p.ModuleId)).OrderBy(p => p.ModuleIndex).ToList();
            string baseUrl = Url.Content("~/").Replace("SimDms/", "");

            foreach (var item in data)
            {

                if (item.InternalLink)
                {
                    item.ModuleUrl = baseUrl + item.ModuleUrl;
                }
                else
                {
                    item.ModuleUrl = "/" + item.ModuleUrl; 
                }
            }
            } catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return Json(new { id = SessionId, data = data });
        }
    }
}
