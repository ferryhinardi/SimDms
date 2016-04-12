using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Web.Models;

namespace SimDms.Web.Controllers
{
    public class RegisterController : BaseController
    {
        public ActionResult Index()
        {
            return View("Register");
        }

        public ActionResult ApproveUser(RegisteredUser model)
        {
            ResultModel result = InitializeResult();

            var data = ctx.RegisteredUsers.Where(x => x.UserName==model.UserName && x.Email==model.Email && model.DealerCode==model.DealerCode).FirstOrDefault();
            if (data != null)
            {
                data.IsApproved = true;
                data.ApprovedBy = CurrentUser.Username;
                data.ApprovedDate = DateTime.Now;

                var user = ctx.SysUsers.Where(x => x.Username==model.UserName).FirstOrDefault();

                if (user == null)
                {
                    string password = Guid.NewGuid().ToString().Substring(0, 8);

                    user = new SysUser();
                    user.DealerCode = model.DealerCode;
                    user.UserId = Guid.NewGuid();
                    user.Username = model.UserName;
                    user.Email = model.Email;
                    user.Password = Crypto.HashPassword(password);
                    
                }

                try
                {
                    ctx.SaveChanges();

                    result.Status = true;
                    result.Message = "User has been approved.";
                }                                                               
                catch (Exception)
                {
                    result.Message = "Sorry, we cannot process your request.";
                }
            }

            return Json(null);
        }

        public ActionResult IsIDExists()
        {
            ResultModel result = InitializeResult();
            string username = Request["Username"] ?? "";
            string email = Request["Email"] ?? "";
            
            var registeredUser = ctx.SysUsers.Where(x => x.Username==username).FirstOrDefault();
            if (registeredUser != null)
            {
                result.Status = true;
                result.Message = "Username already registered.";
                return Json(result);
            }

            registeredUser = ctx.SysUsers.Where(x => x.Email == email).FirstOrDefault();
            if (registeredUser != null)
            {
                result.Status = true;
                result.Message = "Email already registered.";
                return Json(result);
            }


            result.Status = false;
            result.Message = "This username is valid and not in use.";

            return Json(result);
        }

        public ActionResult IsEmailExists()
        {
            return null;
        }

        public ActionResult Add(RegisteredUser model)
        {
            return null;
        }

        public ActionResult Register()
        {
            return View();
        }
    }
}
