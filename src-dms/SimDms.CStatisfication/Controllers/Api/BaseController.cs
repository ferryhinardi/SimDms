using SimDms.CStatisfication.Models;
using SimDms.CStatisfication.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers
{
    public class BaseController : Controller
    {
        protected DataContext ctx = new DataContext();

        protected string HtmlRender(string jsname)
        {
            return string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/cs/"), jsname);
        }

        protected string HtmlRender(string id, string jsname)
        {
            var jshtml = "";
            if (!string.IsNullOrWhiteSpace(jsname))
            {
                jshtml = string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/cs/"), jsname);
            }
            return string.Format(@"<div id=""{0}"" ></div>", id) + jshtml;
        }

        protected SysUser CurrentUser
        {
            get
            {
                return ctx.SysUsers.Find(User.Identity.Name);
            }
        }

        protected string CompanyCode
        {
            get
            {
                return CurrentUser.CompanyCode;
            }
        }

        protected string CompanyName
        {
            get
            {
                return ctx.GnMstOrganizationHdrs.Find(CurrentUser.CompanyCode).CompanyName;
            }
        }

        protected string BranchCode
        {
            get
            {
                return CurrentUser.BranchCode;
            }
        }

        protected string BranchName
        {
            get
            {
                return ctx.CoProfiles.Find(CompanyCode, BranchCode).CompanyName;
            }
        }

        protected ResultModel InitializeResultModel()
        {
            return new ResultModel()
            {
                success = false,
                message = "",
                details = "",
                data = null
            };
        }

        protected DealerInfo DealerInfo()
        {
            DealerInfo result = (from x in ctx.GnMstOrganizationHdrs
                                 select new DealerInfo()
                                 {
                                     CompanyCode = x.CompanyCode,
                                     CompanyName = x.CompanyName
                                 }).FirstOrDefault();

            return result;
        }

        protected object ObjectMapper(object ObjectFrom, object ObjectTo)
        {
            var listProperty = ObjectFrom.GetType().GetProperties();
            foreach (var prop in listProperty)
            {
                var PropObjB = (from obj in ObjectTo.GetType().GetProperties().ToList()
                                where obj.Name == prop.Name
                                select obj).FirstOrDefault();
                if (PropObjB != null)
                {
                    ObjectTo.GetType().GetProperty(prop.Name).SetValue(ObjectTo, prop.GetValue(ObjectFrom, null), null);
                }
            }
            return ObjectTo;
        }

    }
}
