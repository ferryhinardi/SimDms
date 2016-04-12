using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Common;
using eXpressAPI.Models;
using Kendo.DynamicLinq;
using eXpressAPP;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("sm/setup/user")]
    [Route("{action=index}")]
    public class UserController : DefaultController
    {
        private string entityName = "User";
        private string menuId = "11200";

        public JsonResult List(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            if (IsAllowAccess(menuId, "list") != "1")
            {
                SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
                Response.StatusCode = 401;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            string search = Request["search"];
            var list = db.VwUsers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.UserId.Contains(search) || x.Name.Contains(search));
            }
            if (sort == null)
            {
                list = list.OrderBy(x => x.UserId);
            }
            return Json(list.ToDataSourceResult(take, skip, sort, filter));
        }
               
        public string Index()
        {
            string callback = Request["callback"];

            var listUser = db.Users;

            var result = listUser.Select(m => new
            {
                UserId = m.UserId,
                Code = m.Code,
                Name = m.Name,
                RoleId = m.RoleId,
                StatusUsers = m.StatusUsers
            }).ToList();

            JavaScriptSerializer ser = new JavaScriptSerializer();

            return callback +  string.Format("({0})",ser.Serialize(result));
        }

        public string Save(string models)
        {
            string message = "Simpan data berhasil";
            string callback = Request["callback"];
            bool success = true;

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Users> listUsers = ser.Deserialize<List<Users>>(models);

            Users mi = listUsers[0];
            Users m2 = db.Users.Find(mi.UserId);

            if (m2 == null)
            {

                string cnStr = Settings.Connection;
                UserManager uman = new UserManager(new UserStore(new Context(cnStr)));
                string defaultPwd = Settings.DefaultPassword; 
                 
                User uid = new User();
                uid.UserName = mi.UserId;

                try
                {
                    var ret = uman.Create(uid, defaultPwd);
                     
                    if (ret.Succeeded)
                    {
                        uman.ChangePassword(uid.UserName, "password", defaultPwd);

                        uid = uman.FindByName(uid.UserName);
                        mi.Pass = uid.Id;
                        //mi.UserId = mi.Code;
                        mi.CreatedDate = DateTime.Now;
                        mi.CreatedBy = UserName;
                        db.Users.Add(mi);
                    }
                    else
                    {
                        message = (ret.Errors.First().ToString());
                        return callback + string.Format("({0})",  MyGlobalVar.SendErrorMsg(message));
                    }

                } catch(Exception ex)
                {
                }
            }
            else
            {
                m2.Code = mi.Code;
                m2.Name = mi.Name;
                m2.RoleId = mi.RoleId;
                m2.StatusUsers = mi.StatusUsers;
                m2.ChangeBy = UserName;
                m2.ChangeDate = DateTime.Now;

                db.Entry(m2).State = System.Data.Entity.EntityState.Modified;
            }


            SaveChanges();
            
            return callback +  string.Format("({0})",ser.Serialize(mi));

        }

        public string Create(string models)
        {
            string message = "Simpan data berhasil";
            string callback = Request["callback"];
            bool success = true;

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Users> listUsers = ser.Deserialize<List<Users>>(models);

            Users mi = listUsers[0];
            Users m2 = db.Users.Find(mi.UserId);

            if (m2 == null)
            {

                string cnStr = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDB"].ToString();
                UserManager uman = new UserManager(new UserStore(new Context(cnStr)));
                string defaultPwd = System.Configuration.ConfigurationManager.AppSettings["config:DefaultPwd"].ToString(); 

                User uid = new User();
                uid.UserName = mi.UserId;

                try
                {
                    var ret = uman.Create(uid, defaultPwd);
                    if (ret.Succeeded)
                    {
                        uid = uman.FindByName(uid.UserName);
                        mi.Pass = uid.Id;
                        mi.Code = CompanyId;
                        //mi.UserId = mi.Code;
                        mi.CreatedDate = DateTime.Now;
                        mi.CreatedBy = UserName;
                        db.Users.Add(mi);
                    }
                    else
                    {
                        message = (ret.Errors.First().ToString());
                        throw new Exception(message);
                        return callback + string.Format("({0})", MyGlobalVar.SendErrorMsg(message));
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                //m2.Code = mi.Code;
                //m2.Name = mi.Name;
                //m2.RoleId = mi.RoleId;
                //m2.StatusUsers = mi.StatusUsers;
                //m2.ChangeBy = UserName;
                //m2.ChangeDate = DateTime.Now;

                //db.Entry(m2).State = System.Data.Entity.EntityState.Modified;
                message = "Data Sudah Ada ! Proses tidak bisa dilanjutkan";
                throw new Exception(message);
                return callback + string.Format("({0})", MyGlobalVar.SendWarningMsg(message));
            }

            SaveChanges();

            //try
            //{
            //    db.SaveChanges(UserName);

            //}
            //catch (DbEntityValidationException ex)
            //{
            //    success = false;
            //    message = "";
            //    foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
            //    {
            //        // Get entry

            //        DbEntityEntry entry = item.Entry;
            //        string entityTypeName = entry.Entity.GetType().Name;

            //        // Display or log error messages

            //        foreach (DbValidationError subItem in item.ValidationErrors)
            //        {
            //            message += string.Format("Error '{0}' occurred in {1} at {2}",
            //                     subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
            //        }
            //    }

            //    Log.Error(message);
            //    return callback + string.Format("({0})", "toastr[\"error\"]('" + message + "','Error')");

            //}

            return callback + string.Format("({0})", ser.Serialize(mi));


            //return Json(new { success = true, message = message });
        }

        public string Delete(string models)
        {
            string message = "Delete data berhasil";
            bool success = true;
            string callback = Request["callback"];

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Users> list1 = ser.Deserialize<List<Users>>(models);

            Users mi = list1[0];
            Users m2 = db.Users.Find(mi.UserId);

            if (m2 != null)
            {
                db.Users.Remove(m2);
            }
            //SaveChanges(false);
            //try
            //{
            //    db.SaveChanges(UserName);

            //}
            //catch (DbUpdateException ex)
            //{
            //    success = false;
            //    message = ex.InnerException.Message;
            //    Log.Error(message);
            //    return callback + string.Format("({0})", "toastr[\"error\"]('" + message + "','Error')");
            //}

            return callback + string.Format("({0})", ser.Serialize(mi));

        }
        
        public JsonResult Roles()
        {
            var roles = db.Database.SqlQuery<ComboList>("sp_combo_roles").ToList();
            return Json(roles, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GridAdd(Users data)
        {
            SaveResult ret = new SaveResult(0, false);

            if (IsAllowAccess(menuId, "add") != "1")
            {
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            var isFound = db.Users.Find(data.UserId);

            if (isFound == null)
            {
                string cnStr = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDB"].ToString();
                UserManager uman = new UserManager(new UserStore(new Context(cnStr)));
                User uid = new User();
                uid.UserName = data.UserId;

                try
                {
                    User userIdExists = uman.FindByName(data.UserId);

                    if (userIdExists != null)
                    {
                        ret.message = "Data User dengan Id yang Anda masukkan sudah tersedia";
                    }
                    else
                    {
                        IdentityResult ret1;

                        if (string.IsNullOrWhiteSpace(data.Google))
                        {
                            ret1 = uman.Create(uid, "password");
                        }
                        else
                        {
                            var data1 = uman.FindById(data.Google);
                            data1.UserName = data.UserId;
                            data1.PasswordHash = uman.PasswordHasher.HashPassword("password");
                            ret1 = uman.Update(data1);
                        }

                        if (ret1.Succeeded)
                        {
                            uid = uman.FindByName(uid.UserName);
                            data.Pass = uid.Id;
                            data.Code = CompanyId;
                            ret = EntryAdd(data, "Create data User berhasil");
                        }
                    }
               }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            else
            {
                ret.message = "Data User dengan Id yang Anda masukkan sudah tersedia";
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GridUpdate(Users data)
        {
            if (IsAllowAccess(menuId, "update") != "1")
            {
                SaveResult ret = new SaveResult(0, false);
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            string cnStr = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDB"].ToString();
            UserManager uman = new UserManager(new UserStore(new Context(cnStr)));

            User userId =  uman.FindByName(data.UserId);

            if (data.StatusUsers == 0)
            {
                if (UserId != null)
                {
                    uman.SetLockoutEnabled(userId.Id, true);
                    uman.SetLockoutEndDate(userId.Id, DateTime.Now.AddYears(20));
                }
            }
            else
            {
                if (UserId != null)
                {
                    uman.SetLockoutEnabled(userId.Id, false);
                    uman.SetLockoutEndDate(userId.Id, DateTime.Now);
                }
            }
            
            return Json(EntryUpdate(data, "Update data User berhasil"), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GridDelete(Users data)
        {
            SaveResult ret = new SaveResult(0, false);

            if (IsAllowAccess(menuId, "delete") != "1")
            {
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            string cnStr = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDB"].ToString();
            UserManager uman = new UserManager(new UserStore(new Context(cnStr)));
            string Id = data.Pass;

            ret = EntryDelete(data, "Delete data User berhasil");

            // delete user from identity server when delete action success
            if (ret.success) {
                uman.Delete(uman.FindById(Id));
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangePassword(string oldPassword, string newPassword)
        {
            string cnStr = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDB"].ToString();
            UserManager uman = new UserManager(new UserStore(new Context(cnStr)));

            var ret = uman.ChangePassword(UserIdentityId, oldPassword, newPassword);
            string msg = "";
            if ( ret.Errors != null) {
                msg = ret.Errors.Aggregate(msg, (current, error) => current + (error.ToString() + Environment.NewLine));
            }
            return Json(new { success = ret.Succeeded, message = msg }, JsonRequestBehavior.AllowGet );
        }
    }
}