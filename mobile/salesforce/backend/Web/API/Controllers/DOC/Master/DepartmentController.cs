using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using eXpressAPI.Models;
using System.Web.Script.Serialization;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("potaindo/doc/department")]
    [Route("{action=List}")]
    public class DepartmentController : DefaultController
    {
        private string entityName = "Departement";
        private string menuId = "31010";
        
        public JsonResult List()
        {
            if (IsAllowAccess(menuId, "list") != "1")
            {
                SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
                Response.StatusCode = 401;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            string search = Request["search"];
            var list = db.Departements.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.Code.Contains(search) || x.Name.Contains(search));
            }

            return Json(new { Data = list.ToList(), Total = list.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridAdd(Departement data)
        {
            SaveResult ret = new SaveResult(0, false);

            if (IsAllowAccess(menuId, "add") != "1")
            {
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            var isFound = db.Menus.Find(data.Code);
            if (isFound == null)
            {
                ret = EntryAdd(data, string.Format("Buat data {0} berhasil", entityName));
            }
            else
            {
                ret.message = string.Format("Data {0} dengan Id yang Anda masukkan sudah tersedia", entityName);
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridUpdate(Departement data)
        {
            if (IsAllowAccess(menuId, "update") == "1")
                return Json(EntryUpdate(data, string.Format("Update data {0} berhasil", entityName)),
                    JsonRequestBehavior.AllowGet);

            SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridDelete(Departement data)
        {
            if (IsAllowAccess(menuId, "delete") == "1")
                return Json(EntryDelete(data, string.Format("Delete data {0} berhasil", entityName)),
                    JsonRequestBehavior.AllowGet);

            SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
  
        public string Save(string models)
        {
            string callback = Request["callback"];

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Departement> listMenu = ser.Deserialize<List<Departement>>(models);

            Departement mi = listMenu[0];
            Departement m2 = db.Departements.Find(mi.Code);

            if (m2 == null)
            {
                if (mi.Parent == mi.Code)
                {
                    mi.Parent = null;
                }
                mi.CreatedDate = DateTime.Now;
                mi.CreatedBy = UserName;
                db.Departements.Add(mi);
            }
            else
            {
                m2.Name = mi.Name;
                m2.Parent = mi.Parent;
                if (m2.Parent == mi.Code)
                {
                    m2.Parent = null;
                }
                m2.Level = mi.Level;
                m2.Status = mi.Status;
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
                var message = "";
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
        }

        public string Create(string models)
        {
            string message;
            string callback = Request["callback"];

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Departement> listMenu = ser.Deserialize<List<Departement>>(models);

            Departement mi = listMenu[0];
            Departement m2 = db.Departements.Find(mi.Code);

            if (m2 == null)
            {
                if (mi.Parent == mi.Code)
                {
                    mi.Parent = null;
                }
                mi.CreatedDate = DateTime.Now;
                mi.CreatedBy = UserName;
                db.Departements.Add(mi);
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
        }

        public string Delete(string models)
        {
            string callback = Request["callback"];

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Departement> listMenu = ser.Deserialize<List<Departement>>(models);

            Departement mi = listMenu[0];
            Departement m2 = db.Departements.Find(mi.Code);

            if (m2 != null)
            {
                var dataInfo = db.ExecScalar("sp_sm_master_used_departement '" + m2.Code + "'");
                if (dataInfo == "0")
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
    }
}