using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SimDms.Web.Controllers
{
    public class LayoutController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            string s = "";
            var f = ctx.SysUserProfitCenters.Find(User.Identity.Name);
            if (f != null) s = f.ProfitCenter;

            ViewBag.BranchCode = BranchCode;
            ViewBag.BranchName = BranchName;
            ViewBag.TypePart = TypeOfGoodsName;
            ViewBag.ShowHideTypePart = (CurrentUser.RoleID.Equals("Admin", StringComparison.InvariantCultureIgnoreCase) || f.ProfitCenter == "300") ? TypeOfGoodsName : "";
                        
            string RptUrl = ConfigurationManager.AppSettings["reportUrl"].ToString() ?? "";
            string appName = Request.ApplicationPath.ToString();

            ViewBag.ReportURL = RptUrl.Replace("sdmsreport", "sdmsreport" + appName.Replace(@"/",""));

            return View(GenerateNavigationMenu());
        }

        [Authorize]
        public ActionResult SDMS()
        {
            ViewBag.BranchCode = BranchCode;
            ViewBag.BranchName = BranchName;
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
            List<SysMenu> menus = null;
            ctx.Configuration.LazyLoadingEnabled = false;

            string roleID = CurrentUser.RoleID;
            if (string.IsNullOrEmpty(roleID))
            {
                menus = ctx.SysMenus.ToList();
            }
            else
            {
                IEnumerable<string> menuIDs = ctx.SysRoleMenus.Where(x => x.RoleId==roleID).Select(x => x.MenuId);
                menus = (from x in ctx.SysMenus
                            where menuIDs.Contains(x.MenuId)
                            select x).ToList();
            }

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

            var strModuleName = ctx.SysModules.Find(id).ModuleCaption;

            return Json(new { success = true, moduleName = strModuleName, data = list }, JsonRequestBehavior.AllowGet );
        }

        public JsonResult ListModules()
        {
            string roleID = CurrentUser.RoleID;
            IEnumerable<string> moduleIDs = ctx.SysRoleModules.Where(x => x.RoleID==roleID).Select(x => x.ModuleID);
            var session = ctx.SysSessions.Where(p => p.SessionUser == User.Identity.Name).OrderBy(p => p.CreateDate).FirstOrDefault();
            //var data = ctx.SysModules.OrderBy(p => p.ModuleIndex).ToList();
            var data = (from x in ctx.SysModules
                        where moduleIDs.Contains(x.ModuleId)
                        select x).ToList();
            return Json(new { id = session.SessionId, data = data, userdata = CurrentUserInfo() });
        }

        public IEnumerable<SysNavigation> GenerateNavigationMenu()
        {
            string roleID = CurrentUser.RoleID;

            IEnumerable<string> moduleIDs = ctx.SysRoleModules.Where(x => x.RoleID == roleID).Select(x => x.ModuleID);
            IEnumerable<string> MenuIDs = ctx.SysRoleMenus.Where(x => x.RoleId == roleID).Select(x => x.MenuId);

            var data = (from x in ctx.SysModules
                        where moduleIDs.Contains(x.ModuleId)
                        select x).Where(x => x.IsPublish == true).OrderBy( x => x.ModuleIndex).ToList();

            var dataMenus = (from x in ctx.SysMenus
                        where MenuIDs.Contains(x.MenuId)
                        select x).ToList();



            List<SysNavigation> Navigator = new List<SysNavigation>();
            List<SysNavigation> QuickNavigator = new List<SysNavigation>();

            foreach (var x in data)
            {
                var y = new SysNavigation();
                y.MenuCaption = x.ModuleCaption;
                y.MenuId = x.ModuleId;
                y.Url = "#";
                y.Icon = x.Icon;
                ListNavMenus(ref y, ref QuickNavigator, dataMenus, x.ModuleId, x.ModuleId, x.ModuleCaption, x.ModuleCaption);
                Navigator.Add(y);  
            }

            ViewBag.menus = QuickNavigator; // (new JsonResult().Data = new { data = QuickNavigator });

            return Navigator;
        }

        public JsonResult ListUserMenus()
        {
            string roleID = CurrentUser.RoleID;

            IEnumerable<string> moduleIDs = ctx.SysRoleModules.Where(x => x.RoleID == roleID).Select(x => x.ModuleID);
            IEnumerable<string> MenuIDs = ctx.SysRoleMenus.Where(x => x.RoleId == roleID).Select(x => x.MenuId);

            var data = (from x in ctx.SysModules
                        where moduleIDs.Contains(x.ModuleId)
                        select x).Where(x => x.IsPublish == true).OrderBy(x => x.ModuleIndex).ToList();

            var dataMenus = (from x in ctx.SysMenus
                             where MenuIDs.Contains(x.MenuId)
                             select x).ToList();



            List<SysNavigation> Navigator = new List<SysNavigation>();
            List<SysNavigation> QuickNavigator = new List<SysNavigation>();

            foreach (var x in data)
            {
                var y = new SysNavigation();
                y.MenuCaption = x.ModuleCaption;
                y.MenuId = x.ModuleId;
                y.Url = "#";
                y.Icon = x.Icon;
                ListNavMenus(ref y, ref QuickNavigator, dataMenus, x.ModuleId, x.ModuleId, x.ModuleCaption, x.ModuleCaption);
                Navigator.Add(y);
            }

            return Json(new { success= true, data= QuickNavigator }, JsonRequestBehavior.AllowGet);
        }

        private void ListNavMenus(ref SysNavigation refMenu, ref List<SysNavigation> OutMenus, List<SysMenu> menus, string ModuleName, string header, string GroupName, string ParentName)
        {
            var myMenus = (from q in menus
                           where q.MenuHeader == header
                           select q).OrderBy( x => x.MenuIndex).ToList();

            foreach (var x in myMenus)
            {
                var y = new SysNavigation();
                y.MenuCaption = x.MenuCaption;
                y.MenuId = x.MenuId;
                y.ModuleName = GroupName;
                y.ParentName = ParentName;

                if (string.IsNullOrEmpty(x.MenuUrl))
                {
                    y.Url = "#";
                }
                else
                {
                    y.Url = "#lnk/" + ModuleName + "/" + x.MenuUrl;
                    OutMenus.Add(y);
                }                

                y.Icon = x.MenuIcon;
                ListNavMenus(ref y, ref OutMenus, menus, ModuleName, y.MenuId, GroupName, ParentName + " / " + y.MenuCaption);

                refMenu.Detail.Add(y);               
            }

        }



        public MyUserInfo CurrentUserInfo()
        {
            var u = ctx.SysUsers.Find(User.Identity.Name);
            string s = "";
            var f = ctx.SysUserProfitCenters.Find(User.Identity.Name);
            if (f != null) s = f.ProfitCenter;

            var info = new MyUserInfo
            {
                UserId = u.UserId,
                FullName = u.FullName,
                CompanyCode = u.CompanyCode,
                BranchCode = u.BranchCode,
                TypeOfGoods = u.TypeOfGoods,
                IsActive = u.IsActive,
                RequiredChange = u.RequiredChange,
                ShowHideTypePart = (CurrentUser.RoleID == "Admin" || f.ProfitCenter == "300") ? TypeOfGoodsName : ""
            };

            return info;
        }

    }
}
