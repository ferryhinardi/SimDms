using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using eXpressAPI.Models;
using Kendo.DynamicLinq;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("sm/setup/menu")]
    [Route("{action=list}")]
    public class MenuController : DefaultController
    {

        private string entityName = "Menu";
        //public JsonResult List(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        public JsonResult List()
        {
            string search = Request["search"];
            var list = db.Menus.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.MenuId.Contains(search) || x.Name.Contains(search));
            }
            //if (sort == null)
            //{
            //    list = list.OrderBy(x => x.MenuId);
            //}
            return Json(new { Data = list.ToList(), Total = list.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridAdd(Menus data)
        {
            SaveResult ret = new SaveResult(0, false);
            var IsFound = db.Menus.Find(data.MenuId);

            if (IsFound == null)
            {
                ret = EntryAdd(data, string.Format("Buat data {0} berhasil", entityName));
            }
            else
            {
                ret.message = string.Format("Data {0} dengan Id yang Anda masukkan sudah tersedia", entityName);
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridUpdate(Menus data)
        {
            return Json(EntryUpdate(data, string.Format("Update data {0} berhasil", entityName)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridDelete(Menus data)
        {
            return Json(EntryDelete(data, string.Format("Delete data {0} berhasil", entityName)), JsonRequestBehavior.AllowGet);
        }

        public string MyUser()
        {
            return UserName;
        }

        public string Index(string MenuId)
        {
            string callback = Request["callback"];

            var listmenus = db.Menus;
                            //.Where(m =>  !string.IsNullOrEmpty(MenuId) ? m.Parent == MenuId : m.Parent == null);

            var result = listmenus.Select(m => new
            {
                MenuId = m.MenuId,
                Name = m.Name,
                Parent = m.Parent,
                Link = m.Link,
                Seq = m.Seq,
                MenuLevel = m.MenuLevel,
                MenuPict = m.MenuPict,
                StatusMenu = m.StatusMenu,
                //hasChildren = db.Menus.Where(x => x.Parent = m.MenuId).Any()
            }).ToList();

            JavaScriptSerializer ser = new JavaScriptSerializer();

            return callback +  string.Format("({0})",ser.Serialize(result));
        }

        public string Save(string models)
        {
            string message = "Simpan data berhasil";
            bool success = true;
            string callback = Request["callback"];

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Menus> listMenu = ser.Deserialize<List<Menus>>(models);

            Menus mi = listMenu[0];
            Menus m2 = db.Menus.Find(mi.MenuId);

            if (m2 == null)
            {
                if (mi.Parent == mi.MenuId)
                {
                    mi.Parent = null;
                }
                mi.CreatedDate = DateTime.Now;
                mi.CreatedBy = UserName;
                db.Menus.Add(mi);
            }
            else
            {
                m2.Name = mi.Name;
                m2.Parent = mi.Parent;
                if (m2.Parent == mi.MenuId)
                {
                    m2.Parent = null;
                }
                m2.Seq = mi.Seq;
                m2.MenuLevel = mi.MenuLevel;
                m2.MenuPict = mi.MenuPict;
                m2.Link = mi.Link;
                m2.StatusMenu = mi.StatusMenu;
                m2.ChangeBy = UserName;
                m2.ChangeDate = DateTime.Now;

                db.Entry(m2).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                db.SaveChanges(UserName);

            }
            catch (DbEntityValidationException ex)
            {
                success = false;
                message = "";
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    // Get entry

                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    // Display or log error messages

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        message += string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                    }
                }
                return callback + string.Format("({0})", MyGlobalVar.SendErrorMsg(message));
            }

            return callback +  string.Format("({0})",ser.Serialize(mi));


            //return Json(new { success = true, message = message });
        }

        public string Create(string models)
        {
            string message = "Simpan data berhasil";
            bool success = true;
            string callback = Request["callback"];

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Menus> listMenu = ser.Deserialize<List<Menus>>(models);

            Menus mi = listMenu[0];
            Menus m2 = db.Menus.Find(mi.MenuId);

            if (m2 == null)
            {
                if (mi.Parent == mi.MenuId)
                {
                    mi.Parent = null;
                }
                mi.CreatedDate = DateTime.Now;
                mi.CreatedBy = UserName;
                db.Menus.Add(mi);
            }
            else
            {
                //m2.Name = mi.Name;
                //m2.Parent = mi.Parent;
                //m2.Seq = mi.Seq;
                //m2.MenuLevel = mi.MenuLevel;
                //m2.MenuPict = mi.MenuPict;
                //m2.Link = mi.Link;
                //m2.StatusMenu = mi.StatusMenu;
                //m2.ChangeBy = UserName;
                //m2.ChangeDate = DateTime.Now;

                //db.Entry(m2).State = System.Data.Entity.EntityState.Modified;
                message = "Data Sudah Ada ! Proses tidak bisa dilanjutkan";
                return callback + string.Format("({0})", MyGlobalVar.SendWarningMsg(message));
            }

            try
            {
                db.SaveChanges(UserName);

            }
            catch (DbEntityValidationException ex)
            {
                success = false;
                message = "";
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    // Get entry

                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    // Display or log error messages

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        message += string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                    }
                }
                return callback + string.Format("({0})", MyGlobalVar.SendErrorMsg(message));
            }

            
            return callback + string.Format("({0})", ser.Serialize(mi));


            //return Json(new { success = true, message = message });
        }

        public string Delete(string models)
        {
            string message = "Delete data berhasil";
            bool success = true;
            string callback = Request["callback"];

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Menus> listMenu = ser.Deserialize<List<Menus>>(models);

            Menus mi = listMenu[0];
            Menus m2 = db.Menus.Find(mi.MenuId);

            if (m2 != null)
            {
                var dataInfo = db.ExecScalar("sp_sm_master_used_menu '" + m2.MenuId + "'");
                //string ret = db.ExecScalar("sp_sm_master_used_menu '" + m2.MenuId + "'");
                if (dataInfo == "0" )
                {
                    base.EntryDelete(m2);
                    SaveChanges();
                }
                else
                {
                    return callback + string.Format("({0})", MyGlobalVar.SendErrorMsg("Menu yang Anda pilih sudah terpakai oleh role system!!!"));
                }                
            }            

            return callback + string.Format("({0})", ser.Serialize(mi));

        }

        public JsonResult GenerateNavigationMenu()
        {

            var data = (from x in db.Menus
                        where x.MenuLevel == 0
                        select x).Where(x => x.StatusMenu == 1).OrderBy(x => x.Seq).ToList();

            var dataMenus = (from x in db.Menus
                             where x.MenuLevel > 0
                             select x).ToList();


            List<SysNavigation> Navigator = new List<SysNavigation>();
            List<SysNavigation> QuickNavigator = new List<SysNavigation>();

            foreach (var x in data)
            {
                var y = new SysNavigation();
                y.MenuCaption = x.Name;
                y.MenuId = x.MenuId;
                y.Url = "#";
                y.Icon = x.MenuPict;
                ListNavMenus(ref y, ref QuickNavigator, dataMenus, x.MenuId, x.MenuId, x.Name, x.Name);
                Navigator.Add(y);
            }

            return  Json(Navigator, JsonRequestBehavior.AllowGet);
        }

        private void ListNavMenus(ref SysNavigation refMenu, ref List<SysNavigation> OutMenus, 
            List<Menus> menus, string ModuleName, 
            string header, string GroupName, string ParentName)
        {
            var myMenus = (from q in menus
                           where q.Parent == header
                           select q).OrderBy(x => x.Seq).ToList();

            foreach (var x in myMenus)
            {
                var y = new SysNavigation();
                y.MenuCaption = x.Name;
                y.MenuId = x.MenuId;
                y.ModuleName = GroupName;
                y.ParentName = ParentName;

                if (string.IsNullOrEmpty(x.Link))
                {
                    y.Url = "#";
                }
                else
                {
                    y.Url = "#" + x.Link;
                    OutMenus.Add(y);
                }

                y.Icon = x.MenuPict;
                ListNavMenus(ref y, ref OutMenus, menus, ModuleName, y.MenuId, GroupName, ParentName + " / " + y.MenuCaption);

                refMenu.Detail.Add(y);
            }

        }

        
    
    
    }
}