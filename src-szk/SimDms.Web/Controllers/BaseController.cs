using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Web.Controllers
{
    public class BaseController : Controller
    {
        protected DataContext ctx = new DataContext();

        public BaseController()
        {
            ctx.Configuration.AutoDetectChangesEnabled = false;
        }

        protected string HtmlRender(string jsname)
        {
            return string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/"), jsname);
        }

        protected string HtmlRender(string id, string jsname)
        {
            var jshtml = "";
            if (!string.IsNullOrWhiteSpace(jsname))
            {
                jshtml = string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/"), jsname);
            }
            return string.Format(@"<div id=""{0}"" ></div>", id) + jshtml;
        }

        protected SysUser CurrentUser
        {
            get
            {
                var user = ctx.SysUsers.Where(x => x.Username==User.Identity.Name).FirstOrDefault();

                return user;
            }
        }

        protected string CompanyCode
        {
            get
            {
                return (CurrentUser.DealerCode ?? "Suzuki");
            }
        }

        protected ResultModel InitializeResult()
        {
            return new ResultModel()
            {
                Status = false,
                Message = "",
                Details = "",
                Data = null
            };
        }
    }
}
