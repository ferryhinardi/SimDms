using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace SimDms.SUtility.Controllers.Api
{
    public class PublicController : BaseController
    {
        public ContentResult Login(string callback)
        {
            var userName = Request.Params["UserName"];
            var password = Request.Params["Password"];

            if (Membership.ValidateUser(userName, password))
            {
                FormsAuthentication.SetAuthCookie(userName, false);
                return Content(String.Format("{0}({1});",
                    callback,
                    new JavaScriptSerializer().Serialize(new { success = true })),
                    "application/javascript");
            }
            else
            {
                return Content(String.Format("{0}({1});",
                    callback,
                    new JavaScriptSerializer().Serialize(new { success = false, message = "Invalid UserName / Password", User = User.Identity.Name })),
                    "application/javascript");
            }
        }

        public ContentResult Logout(string callback)
        {
            var userName = Request.Params["UserName"];

            if (User.Identity.IsAuthenticated && User.Identity.Name == userName)
            {
                FormsAuthentication.SignOut();
            }

            return Content(String.Format("{0}({1});",
                callback,
                new JavaScriptSerializer().Serialize(new { success = true, User = User.Identity.Name })),
                "application/javascript");
        }

        public ContentResult TableList(string callback)
        {
            var userName = Request.Params["UserName"];
            var dealerCode = Request.Params["DealerCode"];

            if (User.Identity.IsAuthenticated && User.Identity.Name == userName)
            {
                var json = Exec(new
                {
                    query = "uspm_table_list",
                    param = new List<dynamic>
                    {
                        new { key = "DealerCode", value = dealerCode },
                    },
                    result = "table"
                });

                return Content(String.Format("{0}({1});",
                    callback,
                    new JavaScriptSerializer().Serialize(json.Data)),
                    "application/javascript");
            }
            else
            {
                return Content(String.Format("{0}({1});",
                    callback,
                    new JavaScriptSerializer().Serialize(new
                    {
                        success = false,
                        IsAuthenticated = User.Identity.IsAuthenticated,
                        Type = User.Identity.AuthenticationType,
                        Name = User.Identity.Name,
                        Pars = new { UserName = userName, DealerCode = dealerCode }
                    })),
                    "application/javascript");
            }
        }

    }
}
