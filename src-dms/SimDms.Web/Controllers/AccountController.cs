using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SimDms.Web.Models.General;
using SimDms.Absence.Models;
using System.Configuration;
using TracerX;
using System.Data.SqlClient;
using System.Data;

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
            var record = ctx.OrganizationHdrs.FirstOrDefault();
            ViewBag.CompanyProfile = record.CompanyName;
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            var record = ctx.OrganizationHdrs.FirstOrDefault();
            ViewBag.CompanyProfile = record.CompanyName;
            ViewBag.Message = "";
            
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
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

                        return RedirectToAction("LogInfo", "Account");
                    }
                }
                else
                {
                    ViewBag.Message = ("Invalid username or password!");
                }
            }

            // If we got this far, something failed, redisplay form
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
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UserId", UserMenu.UserId);
            cmd.Parameters.AddWithValue("@Menu", UserMenu.Menu);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(dt.Rows.Count > 0);
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();            
            //SessionLogout();
            return RedirectToAction("LogOn", "Account");
        }

        [Authorize]
        public ActionResult LogInfo()
        {
            var record = ctx.OrganizationHdrs.FirstOrDefault();
            ViewBag.CompanyProfile = record.CompanyName;
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

            string username = CurrentUser.UserId;
            var data = ctx.SysUsers.Find(username);
            string password = ctx.SysUsers.Find(username).Password;

            if (data != null)
            {                                                                  
                //if (Crypto.VerifyHashedPassword(password, model.OldPassword) == false)
                if (FormsAuthentication.HashPasswordForStoringInConfigFile(model.OldPassword, "md5") != password)
                {
                    result.message = "Sorry, entered current password do not match with our database.";
                }
                else
                {
                    if (model.NewPassword != model.ConfirmPassword)
                    {
                        result.message = "Sorry, entered new password do not match with password confirmation.";
                    }
                    else
                    {
                        //string regexPassword = @"^(?=.*[A-Z])(?=.*\d)(?!.*(.)\1\1)[a-zA-Z0-9@]{6,12}$";
                        //string regexPassword = @"^(?=.*[A-Z])(?=.*\d)(?!.*()\1\1)[a-zA-Z0-9@]{6,12}$";
                        //if (Regex.IsMatch(model.NewPassword, regexPassword))
                        //{
                        //string hashedPassword = Crypto.HashPassword(model.NewPassword);
                        string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(model.NewPassword, "md5");
                        DataContext context = new DataContext();
                        var user = context.SysUsers.Find(username);

                        if (user != null)
                        {
                            user.Password = hashedPassword;
                        }

                        try
                        {
                            context.SaveChanges();
                            // create history
                            context.Database.ExecuteSqlCommand("EXEC uspfn_ChangePassword '" + user.UserId + "','" + user.Password + "'");
                            result.success = true;
                            result.message = "Your password has been changed.";
                        }
                        catch
                        {
                            result.message = "Sorry, we cannot process your request.";
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
            var session = ctx.SysSessions.Where(m=>m.SessionUser == CurrentUser.UserId && m.IsLogout == false).FirstOrDefault();
            string baseUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped);
            var DocumentURL = baseUrl + ConfigurationManager.AppSettings["DOC_URL_VALIDATE"].ToString();
            Response.Redirect(DocumentURL + session.SessionId);
        }
    }
}
