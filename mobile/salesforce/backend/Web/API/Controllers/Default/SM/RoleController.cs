using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using eXpressAPI.Models;
using Kendo.DynamicLinq;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("sm/setup/role")]
    [Route("{action=index}")]
    public class RoleController : DefaultController
    {
        private string entityName = "Role";
        private string menuId = "11300";

        public JsonResult List(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            if (IsAllowAccess(menuId, "list") != "1")
            {
                SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
                Response.StatusCode = 401;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            string search = Request["search"];
            var list = db.Roles.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.RoleId.Contains(search) || x.Name.Contains(search));
            }
            if (sort == null)
            {
                list = list.OrderBy(x => x.RoleId);
            }
            return Json(list.ToDataSourceResult(take, skip, sort, filter));
        }

        public JsonResult GridAdd(Roles data)
        {
            SaveResult ret = new SaveResult(0, false);

            if (IsAllowAccess(menuId, "add") != "1")
            {
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            var isFound = db.Roles.Find(data.RoleId);

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

        public JsonResult GridUpdate(Roles data)
        {
            if (IsAllowAccess(menuId, "update") == "1")
                return Json(EntryUpdate(data, string.Format("Update data {0} berhasil", entityName)),
                    JsonRequestBehavior.AllowGet);

            SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridClone(Roles data)
        {
            if (IsAllowAccess(menuId, "clone") != "1")
            {
                SaveResult ret1 = new SaveResult(0, false) { message = "Access denied" };
                return Json(ret1, JsonRequestBehavior.AllowGet);
            }

            var ret = SqlQuery("exec sp_clone_role '" + CompanyId + "','" + UserId + "','" + data.RoleId + "','" + data.Name + "'");
            ret.message = "Clonning Role success ( " + data.RoleId + " >> " + ret.Value() + " ) ";
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridDelete(Roles data)
        {
            SaveResult ret = new SaveResult(0, false);

            if (IsAllowAccess(menuId, "delete") != "1")
            {   
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            ret = EntryDelete(data, string.Format("Delete data {0} berhasil", entityName));
            if (ret.success == false)
            {
                ret.message = "Maaf, role yang anda akan hapus sudah digunakan";
            }
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        
        public string Index()
        {
            string callback = Request["callback"];

            var list = db.Roles;

            var result = list.Select(m => new
            {
                RoleId = m.RoleId,
                Name = m.Name,
                RoleByCompany = m.RoleByCompany,
                RoleByGrade = m.RoleByGrade,
                RoleByLocation = m.RoleByLocation,
                RoleByOutsource = m.RoleByOutsource,
                StatusRole = m.StatusRole,
                IsSalary = m.IsSalary,
                RoleByOrganization = m.RoleByOrganization
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
            List<Roles> list = ser.Deserialize<List<Roles>>(models);

            Roles m = list[0];
            Roles m2 = db.Roles.Find(m.RoleId);

            if (m2 == null)
            {
                m.CreatedDate = DateTime.Now;
                m.CreatedBy = UserName;
                db.Roles.Add(m);
            }
            else
            {
                m2.Name = m.Name;
                m2.ChangeBy = UserName;
                m2.ChangeDate = DateTime.Now;
                m2.RoleByCompany = m.RoleByCompany;
                m2.RoleByGrade = m.RoleByGrade;
                m2.RoleByLocation = m.RoleByLocation;
                m2.RoleByOutsource = m.RoleByOutsource;
                m2.StatusRole = m.StatusRole;
                m2.IsSalary = m.IsSalary;
                m2.RoleByOrganization = m.RoleByOrganization;
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

                return callback + string.Format("({0})", "toastr[\"error\"]('" + message + "','Error')");
            }

            return callback +  string.Format("({0})",ser.Serialize(m));


            //return Json(new { success = true, message = message });
        }
        
        public string Create(string models)
        {
            string message = "Simpan data berhasil";
            bool success = true;
            string callback = Request["callback"];

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Roles> list = ser.Deserialize<List<Roles>>(models);

            Roles m = list[0];
            Roles m2 = db.Roles.Find(m.RoleId);

            if (m2 == null)
            {
                m.CreatedDate = DateTime.Now;
                m.CreatedBy = UserName;
                db.Roles.Add(m);
            }
            else
            {
                //m2.Name = m.Name;
                //m2.ChangeBy = UserName;
                //m2.ChangeDate = DateTime.Now;
                //m2.RoleByCompany = m.RoleByCompany;
                //m2.RoleByGrade = m.RoleByGrade;
                //m2.RoleByLocation = m.RoleByLocation;
                //m2.RoleByOutsource = m.RoleByOutsource;
                //m2.StatusRole = m.StatusRole;
                //m2.IsSalary = m.IsSalary;
                //m2.RoleByOrganization = m.RoleByOrganization;
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

                return callback + string.Format("({0})", "toastr[\"error\"]('" + message + "','Error')");
            }

            return callback + string.Format("({0})", ser.Serialize(m));


            //return Json(new { success = true, message = message });
        }

        public string Delete(string models)
        {
            string message = "Delete data berhasil";
            bool success = true;
            string callback = Request["callback"];

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Roles> listMenu = ser.Deserialize<List<Roles>>(models);

            Roles mi = listMenu[0];
            Roles m2 = db.Roles.Find(mi.RoleId);

            if (m2 != null)
            {
                db.Roles.Remove(m2);
            }

            try
            {
                db.SaveChanges(UserName);
            }
            catch (DbUpdateException ex)
            {
                success = false;
                message = ex.InnerException.Message;
                return callback + string.Format("({0})", "toastr[\"error\"]('" + message + "','Error')");
            }

            return callback + string.Format("({0})", ser.Serialize(mi));
        }

        public JsonResult ListRoleMenu(string roleid)
        {

            var data = db.Database.SqlQuery<TreeRoleMenu>("exec sp_role_checkpermision '" + roleid + "'").ToList();

            var data1 = (from q in data
                         where q.Level == 0
                         select q).OrderBy(x => x.Seq).ToList();

            TreeData root = new TreeData();
            root.Id = "0";

            foreach (var x in data1)
            {
                var y = new TreeData();
                y.Id = x.MenuId;
                y.Name = x.Name;
                y.value1 = x.value1;
                y.value2 = x.value2;
                ListNavMenus(ref y,  data, x.MenuId);
                root.Children.Add(y);
            }

            return Json(root, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListRoleDept(string roleid)
        {

            var data = db.Database.SqlQuery<TreeDeptMenu>("exec [sp_role_department] '" + roleid + "'").ToList();

            var data1 = (from q in data
                         where q.Level == 0
                         select q).ToList();

            TreeData root = new TreeData();
            root.Id = "0";

            foreach (var x in data1)
            {
                var y = new TreeData();
                y.Id = x.CodeId;
                y.Name = x.Name;
                y.value1 = x.Status;
                ListNavDept(ref y, data, x.CodeId);
                root.Children.Add(y);
            }

            return Json(root, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ListRoleDocType(string roleid)
        {
            //var data = SqlQuery("exec [sp_role_doctype] '" + roleid + "'");
            var data = db.Database.SqlQuery<TreeDeptMenu>("exec [sp_role_doctype] '" + roleid + "'").ToList();

            var data1 = (from q in data
                         where q.Level == 0
                         select q).ToList();

            TreeData root = new TreeData();
            root.Id = "0";

            foreach (var x in data1)
            {
                var y = new TreeData();
                y.Id = x.CodeId;
                y.Name = x.Name;
                y.value1 = x.Status;
                root.Children.Add(y);
            }

            return Json(root, JsonRequestBehavior.AllowGet);

        }

        private void ListNavDept(ref TreeData refMenu, List<TreeDeptMenu> menus, string ParentId)
        {
            var myMenus = (from q in menus
                           where q.ParentId == ParentId
                           select q).ToList();

            foreach (var x in myMenus)
            {
                var y = new TreeData();
                y.Id = x.CodeId;
                y.Name = x.Name;
                y.value1 = x.Status;
 
                ListNavDept(ref y, menus, x.CodeId);
                refMenu.Children.Add(y);
            }
        }

        private void ListNavMenus(ref TreeData refMenu, List<TreeRoleMenu> menus, string ParentId)
        {
            var myMenus = (from q in menus
                           where q.ParentId == ParentId
                           select q).OrderBy(x => x.Seq).ToList();

            foreach (var x in myMenus)
            {
                var y = new TreeData();
                y.Id = x.MenuId;
                y.Name = x.Name;
                y.value1 = x.value1 ;
                y.value2 = x.value2 ;
                ListNavMenus(ref y, menus, x.MenuId);
                refMenu.Children.Add(y);
            }
        }

        private void TreeToString(ref StringBuilder ret, TreeData node)
        {
            ret.Append("|" + node.Id + "," + (node.value1 ? "1," : "0,") + (node.value2 ? "1" : "0"));
            if (node.Children.Count > 0 )
            {
                int n = node.Children.Count;
                for (int i = 0; i < n; i++ )
                {
                    TreeToString(ref ret, node.Children[i]);
                }
            }
        }
        
        public JsonResult Edit(string roleid)
        {
            var list = db.Roles.Find(roleid);
            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveRoleMenu(RoleMapping rMap)
        {
            if (IsAllowAccess(menuId, "update") != "1")
                return Json(new { error = "Access denied to update role" }, JsonRequestBehavior.AllowGet);

            JavaScriptSerializer ser = new JavaScriptSerializer();
            TreeData list = ser.Deserialize<TreeData>(rMap.models);
            StringBuilder sRet = new StringBuilder();
            int n = list.Children.Count;

            for (int i = 0; i < n; i++)
            {
                TreeToString(ref sRet, list.Children[i]);
            }

            var ret = db.Database.ExecuteSqlCommand(string.Format("exec sp_role_mappingmenu '{0}','{1}','{2}'", rMap.roleid, sRet.ToString().Substring(1), UserName));

            return Json(new { success = ret }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveRoleDept(RoleMapping rMap)
        {
            if (IsAllowAccess(menuId, "update") != "1")
                return Json(new { error = "Access denied to update role" }, JsonRequestBehavior.AllowGet);

            JavaScriptSerializer ser = new JavaScriptSerializer();
            TreeData list = ser.Deserialize<TreeData>(rMap.models);
            StringBuilder sRet = new StringBuilder();
            int n = list.Children.Count;

            for (int i = 0; i < n; i++)
            {
                TreeToString(ref sRet, list.Children[i]);
            }

            var ret = db.Database.ExecuteSqlCommand(string.Format("exec sp_role_mappingdept '{0}','{1}','{2}'", rMap.roleid, sRet.ToString().Substring(1), UserName));

            return Json(new { success = ret }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveRoleDocType(RoleMapping rMap)
        {
            if (IsAllowAccess(menuId, "update") != "1")
                return Json(new { error = "Access denied to update role" }, JsonRequestBehavior.AllowGet);

            JavaScriptSerializer ser = new JavaScriptSerializer();
            TreeData list = ser.Deserialize<TreeData>(rMap.models);
            StringBuilder sRet = new StringBuilder();
            int n = list.Children.Count;

            for (int i = 0; i < n; i++)
            {
                TreeToString(ref sRet, list.Children[i]);
            }

            var ret = db.Database.ExecuteSqlCommand(string.Format("exec sp_role_mappingdoctype '{0}','{1}','{2}'", rMap.roleid, sRet.ToString().Substring(1), UserName));

            return Json(new { success = ret }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveRoleOrg(RoleMapping rMap)
        {
            var ret = db.Database.ExecuteSqlCommand(string.Format("exec sp_role_mapping_org '{0}','{1}','{2}'", rMap.roleid, rMap.models, UserName));
            return Json(new { success = ret }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveRoleLocation(RoleMapping rMap)
        {
            var ret = db.Database.ExecuteSqlCommand(string.Format("exec sp_role_mapping_loc '{0}','{1}','{2}'", rMap.roleid, rMap.models, UserName));

            return Json(new { success = ret }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult SaveRoleVendor(RoleMapping rMap)
        {
            var ret = db.Database.ExecuteSqlCommand(string.Format("exec sp_role_mapping_vendor '{0}','{1}','{2}'", rMap.roleid, rMap.models, UserName));
            return Json(new { success = ret }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult SaveRoleGrade(RoleMapping rMap)
        {
            var ret = db.Database.ExecuteSqlCommand(string.Format("exec sp_role_mapping_grade '{0}','{1}','{2}'", rMap.roleid, rMap.models, UserName));
            return Json(new { success = ret }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult SaveRoleCompany(RoleMapping rMap)
        {
            var ret = db.Database.ExecuteSqlCommand(string.Format("exec sp_role_mapping_company '{0}','{1}','{2}'", rMap.roleid, rMap.models, UserName));
            return Json(new { success = ret }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListRoleCompany(string roleid)
        {
            var ret = db.Database.SqlQuery<RoleMapping>(string.Format("exec sp_role_list_company '{0}'", roleid));
            return Json(new { data = ret }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListRoleLocation(string roleid)
        {
            var ret = db.Database.SqlQuery<RoleMapping>(string.Format("exec sp_role_list_location '{0}'", roleid));
            return Json(new { data = ret }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult ListRoleGrade(string roleid)
        {
            var ret = db.Database.SqlQuery<RoleMapping>(string.Format("exec sp_role_list_grade '{0}'", roleid));
            return Json(new { data = ret }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListRoleVendor(string roleid)
        {
            var ret = db.Database.SqlQuery<RoleMapping>(string.Format("exec sp_role_list_vendor '{0}'", roleid));
            return Json(new { data = ret }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListRoleOrg(string roleid)
        {

            var data = db.Database.SqlQuery<TreeRoleMenu>("exec sp_role_checkorg '" + roleid + "'").ToList();

            var data1 = (from q in data
                         where q.Level == 0
                         select q).OrderBy(x => x.Name).ToList();

            TreeData root = new TreeData();
            root.Id = "0";

            foreach (var x in data1)
            {
                var y = new TreeData();
                y.Id = x.MenuId;
                y.Name = x.Name;
                y.value1 = x.value1  ;
                y.value2 = x.value2  ;
                ListNavOrg(ref y, data, x.MenuId);
                root.Children.Add(y);
            }

            return Json(root, JsonRequestBehavior.AllowGet);
        }

        private void ListNavOrg(ref TreeData refMenu, List<TreeRoleMenu> menus, string ParentId)
        {
            var myMenus = (from q in menus
                           where q.ParentId == ParentId
                           select q).OrderBy(x => x.Name).ToList();

            foreach (var x in myMenus)
            {
                var y = new TreeData();
                y.Id = x.MenuId;
                y.Name = x.Name;
                y.value1 = x.value1  ;
                y.value2 = x.value2  ;
                ListNavOrg(ref y, menus, x.MenuId);
                refMenu.Children.Add(y);
            }
        }

        public string CompanyMapping()
        {
            string callback = Request["callback"];
            string roleId = Request["roleid"];

            var list = db.Database.SqlQuery<RoleMappingList>("exec sp_role_mapping_list_company '" + roleId + "'").ToList();

            JavaScriptSerializer ser = new JavaScriptSerializer();

            return callback + string.Format("({0})", ser.Serialize(list));
        }

        public string OrganizationMapping()
        {
            string callback = Request["callback"];
            string roleId = Request["roleid"];

            var list = db.Database.SqlQuery<RoleMappingList>("exec sp_role_mapping_list_org '" + roleId + "'").ToList();

            JavaScriptSerializer ser = new JavaScriptSerializer();

            return callback + string.Format("({0})", ser.Serialize(list));
        }

        public string LocationMapping()
        {
            string callback = Request["callback"];
            string roleId = Request["roleid"];

            var list = db.Database.SqlQuery<RoleMappingList>("exec sp_role_mapping_list_location '" + roleId + "'").ToList();

            JavaScriptSerializer ser = new JavaScriptSerializer();

            return callback + string.Format("({0})", ser.Serialize(list));
        }

        public string VendorMapping()
        {
            string callback = Request["callback"];
            string roleId = Request["roleid"];

            var list = db.Database.SqlQuery<RoleMappingList>("exec sp_role_mapping_list_vendor '" + roleId + "'").ToList();

            JavaScriptSerializer ser = new JavaScriptSerializer();

            return callback + string.Format("({0})", ser.Serialize(list));
        }

        public string GradeMapping()
        {
            string callback = Request["callback"];
            string roleId = Request["roleid"];

            var list = db.Database.SqlQuery<RoleMappingList>("exec sp_role_mapping_list_grade '" + roleId + "'").ToList();

            JavaScriptSerializer ser = new JavaScriptSerializer();

            return callback + string.Format("({0})", ser.Serialize(list));
        }

        public JsonResult DeleteCompanyMapping(RoleMappingList model)
        {
            int i = db.Database.ExecuteSqlCommand("exec sp_role_mapping_delete_company '" + model.IDX + "'");
            return Json(new { success = true, changes = i }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult DeleteGradeMapping(RoleMappingList model)
        {
            int i = db.Database.ExecuteSqlCommand("exec sp_role_mapping_delete_grade '" + model.IDX + "'");
            return Json(new { success = true, changes = i }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteLocationMapping(RoleMappingList model)
        {
            int i = db.Database.ExecuteSqlCommand("exec sp_role_mapping_delete_location '" + model.IDX + "'");
            return Json(new { success = true, changes = i }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteOrganizationMapping(RoleMappingList model)
        {
            int i = db.Database.ExecuteSqlCommand("exec sp_role_mapping_delete_org '" + model.IDX + "'");
            return Json(new { success = true, changes = i }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteVendorMapping(RoleMappingList model)
        {
            int i = db.Database.ExecuteSqlCommand("exec sp_role_mapping_delete_vendor '" + model.IDX + "'");
            return Json(new { success = true, changes = i }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListCompanyByRole(string roleid)
        {
            var data = db.Database.SqlQuery<ComboList>("exec sp_role_combo_list_company '" + roleid + "'").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListVendorByRole()
        {
            string filter = Request["filter[filters][1][value]"] ?? "";
            string companyid = Request["filter[filters][0][value]"] ?? "";
            string roleId = Request["roleid"];
            var data = db.Database.SqlQuery<ComboList>("exec sp_role_combo_list_vendor '" + roleId + "','" + companyid + "','" + filter + "'").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListGradeByRole()
        {
            string filter = Request["filter[filters][1][value]"] ?? "";
            string companyid = Request["filter[filters][0][value]"] ?? "";
            string roleId = Request["roleid"];
            var data = db.Database.SqlQuery<ComboList>("exec sp_role_combo_list_grade '" + roleId + "','" + companyid + "','" + filter + "'").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        public string Update(Roles m)
        {
            if (IsAllowAccess(menuId, "update") != "1")
                return "{\"error\":\"Access denied to update role\"}";

            string message = "Simpan data berhasil";
            bool success = true;
            string callback = Request["callback"];
            
            Roles m2 = db.Roles.Find(m.RoleId);

            if (m2 == null)
            {
                m.CreatedDate = DateTime.Now;
                m.CreatedBy = UserName;
                db.Roles.Add(m);
            }
            else
            {
                m2.Name = m.Name;
                m2.ChangeBy = UserName;
                m2.ChangeDate = DateTime.Now;
                m2.RoleByCompany = m.RoleByCompany;
                m2.RoleByGrade = m.RoleByGrade;
                m2.RoleByLocation = m.RoleByLocation;
                m2.RoleByOutsource = m.RoleByOutsource;
                m2.StatusRole = m.StatusRole;
                m2.IsSalary = m.IsSalary;
                m2.RoleByOrganization = m.RoleByOrganization;
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

                return callback + string.Format("({0})", "toastr[\"error\"]('" + message + "','Error')");
            }

            JavaScriptSerializer ser = new JavaScriptSerializer();

            return callback + string.Format("({0})", ser.Serialize(m));
        }

        public JsonResult ManageAccess(AccessPermissionDetail data)
        {
            SaveResult ret = new SaveResult(0, false);

            if (IsAllowAccess(menuId, "update") != "1")
            {
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            ret =
                SqlQuery("exec [sp_manage_permission_detail] '" + data.roleid + "','" + data.menuid + "','" +
                         data.accessname + "','" + data.action + "'");
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPermissionByMenu(AccessPermissionDetail data)
        {
            var ret = SqlQuery("exec [sp_get_permission_detail] '" + data.roleid + "','" + data.menuid + "'");
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
    }
}