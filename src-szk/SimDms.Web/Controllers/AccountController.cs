using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SimDms.Web.Controllers
{
    public class AccountController : BaseController
    {
        public ActionResult LogOn()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LogInfo");
            }
            return View();
        }

        [HttpPost]
        public JsonResult UserValidation(LogOnModel data)
        {
            bool IsValid = Membership.ValidateUser(data.UserName, data.Password);
            string role = "guest";

            if(string.IsNullOrEmpty(data.UserName) || string.IsNullOrEmpty(data.Password))
            {
                IsValid = false;
            }
            else
            {
                if (IsValid)
                {
                    role = ctx.Database.SqlQuery<string>("exec uspfn_sysGetUserRole '" + data.UserName + "'").FirstOrDefault();
                }
                else
                {
                    IsValid = ValidateUserLDAP(data.UserName, data.Password);
                    if (IsValid)
                    {
                        role = ctx.Database.SqlQuery<string>("exec uspfn_sysGetUserRole '" + data.UserName + "'").FirstOrDefault();
                    }
                }
            }

            return Json(new { success = IsValid, role = role }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

                string useLDAP = System.Configuration.ConfigurationManager.AppSettings["UseLDAP"].ToString();
                bool IsValid = false;

                IsValid = Membership.ValidateUser(model.UserName, model.Password);

                if (useLDAP != "" && !IsValid)
                {
                    if (Convert.ToBoolean(useLDAP)==true)
                    {
                        IsValid = ValidateUserLDAP(model.UserName, model.Password);
                    }
                }

                if (IsValid)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    SessionLogin(model.UserName);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                              && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return new RedirectResult("~/Account/LogInfo");
                    }
                }
            }

            return View(model);
        }

        public ActionResult MenuValidation()
        {
            var valid = false;
            if (!User.Identity.IsAuthenticated) return Json(valid);
            if (CurrentUser == null) return Json(valid);

            var UserMenu = new
            {
                UserId = CurrentUser.UserId,
                Menu = Request.Params["menu"] ?? ""
            };

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_ValidateUserMenu";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UserId", UserMenu.UserId);
            cmd.Parameters.AddWithValue("@Menu", UserMenu.Menu);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(dt.Rows.Count > 0);
        }

        public bool ValidateUserLDAP(string username, string password)
        {
            bool result = false;
            var dse = new DirectoryEntry("LDAP://suzuki.co.id/rootDSE", username, password);
            try
            {
                var name = dse.Name;
                if (dse.Name.Length > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            SessionLogout();
            return new RedirectResult("~/Account/LogOn");
        }

        [Authorize]
        public ActionResult LogInfo()
        {
            return View();
        }

        public void SessionLogin(string userName)
        {
            var model = new SysSession
            {
                SessionId = Guid.NewGuid().ToString(),
                SessionUser = userName,
                CreateDate = DateTime.Now,
                IsLogout = false,
            };
            try
            {
                ctx.SysSessions.Add(model);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
        }

        public void SessionLogout()
        {
            var model = ctx.SysSessions.Where(p => p.SessionUser == User.Identity.Name).OrderBy(p => p.CreateDate).FirstOrDefault();
            if (model != null)
            {
                try
                {
                    FormsAuthentication.SignOut();
                    model.IsLogout = true;
                    model.LogoutTime = DateTime.Now;
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }
        }

        [HttpPost]
        public JsonResult ChangePassword(ChangePasswordModel model)
        {
            ResultModel result = InitializeResult();
            string username = CurrentUser.Username;
            string password = CurrentUser.Password;

            var data = ctx.SysUsers.Where(x => x.Username == username).FirstOrDefault();
            if (data != null)
            {
                if (Crypto.VerifyHashedPassword(password, model.OldPassword) == false)
                {
                    result.Message = "Sorry, entered current password do not match with our database.";
                }
                else
                {
                    if (model.NewPassword != model.ConfirmPassword)
                    {
                        result.Message = "Sorry, entered new password do not match with password confirmation.";
                    }
                    else
                    {
                        //string regexPassword = @"^(?=.*[A-Z])(?=.*\d)(?!.*(.)\1\1)[a-zA-Z0-9@]{6,12}$";
                        //string regexPassword = @"^(?=.*[A-Z])(?=.*\d)(?!.*()\1\1)[a-zA-Z0-9@]{6,12}$";
                        //if (Regex.IsMatch(model.NewPassword, regexPassword))
                        //{
                        string hashedPassword = Crypto.HashPassword(model.NewPassword);
                        DataContext context = new DataContext();
                        var user = context.SysUsers.Where(x => x.Username == username).FirstOrDefault();

                        if (user != null)
                        {
                            user.Password = hashedPassword;
                        }

                        try
                        {
                            context.SaveChanges();

                            result.Status = true;

                            result.Message = "Your password has been changed.";
                        }
                        catch (Exception ex)
                        {
                            result.Message = "Sorry, we cannot process your request : " + ex;
                        }
                        //}
                        //else
                        //{
                        //    result.Message = "Your password doestn't meet the requirements.\n\nRequirements: " +
                        //                     "\n1. Special chararcters is not allowed." +
                        //                     "\n2. Spaces - Not Allowed." +
                        //                     "\n3. Minimum and Maximum Length of field - 6 to 12 Characters." +
                        //                     "\n4. Numeric Character - At least one character." +
                        //                     "\n5. At least one Capital Letter." +
                        //                     "\n6. Repetitive Characters - Allowed only two repetitive characters.";
                        //}
                    }
                }
            }

            return Json(result);
        }

        public void RedirectToDoc()
        {
            string currentUserID = CurrentUser.Username.ToString();
            var session = ctx.SysSessions.Where(m => m.SessionUser == currentUserID && m.IsLogout == false).FirstOrDefault();
            string baseUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped);
            var DocumentURL = baseUrl + ConfigurationManager.AppSettings["DOC_URL_VALIDATE"].ToString();
            Response.Redirect(DocumentURL + session.SessionId);
        }
    }
}
