using SimDms.General.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Web.Script.Serialization;
using SimDms.General.Models.Others;
using SimDms.Common;
using SimDms.Common.Models;
using System.Text;

namespace SimDms.General.Controllers
{
    public class BaseController : Controller
    {
        protected DataContext ctx;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ctx = new DataContext(MyHelpers.GetConnString("DataContext"));
        }

        protected string HtmlRender(string jsname)
        {
            return string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/gn/"), jsname);
        }

        protected string HtmlRender(string id, string jsname)
        {
            var jshtml = "";
            if (!string.IsNullOrWhiteSpace(jsname))
            {
                jshtml = string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/gn/"), jsname);
            }
            return string.Format(@"<div id=""{0}"" ></div>", id) + jshtml;
        }

        protected Dictionary<string, object> GetJson(DataRow dr)
        {
            Dictionary<string, object> row = new Dictionary<string, object>();
            foreach (DataColumn col in dr.Table.Columns)
            {
                row.Add(col.ColumnName.Trim(), dr[col]);
            }

            return row;
        }


        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        protected List<Dictionary<string, object>> GetJson(DataTable dt)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row = null;

            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName.Trim(), dr[col]);
                }
                rows.Add(row);
            }
            return rows;
        }

        protected dynamic GetJson(DataSet ds)
        {
            var result = new List<dynamic>();
            foreach (DataTable dt in ds.Tables)
            {
                var rows = GetJson(dt);
                result.Add(rows);
            }
            return result;
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
                return ctx.OrganizationHdrs.Find(CurrentUser.CompanyCode).CompanyName;
            }
        }

        protected string BranchCode
        {
            get
            {
                return CurrentUser.BranchCode;
            }
        }
       
        protected ResultModel InitializeResultModel()
        {
            return new ResultModel()
            {
                status = false,
                message = "",
                details = "",
                data = null
            };
        }

        protected string CurrentRole()
        {
            string userID = CurrentUser.UserId;
            string roleID = ctx.SysRoleUsers.Where(x => x.UserId==userID).Select(x => x.RoleId).FirstOrDefault();
            return roleID;
        }

        protected object GetSessionValue(string key)
        {
            return Session[key] ?? "";
        }

        //protected DealerInfo DealerInfo()
        //{
        //    DealerInfo result = (from x in ctx.GnMstOrganizationHdrs
        //                         select new DealerInfo()
        //                         {
        //                             CompanyCode = x.CompanyCode,
        //                             CompanyName = x.CompanyName
        //                         }).FirstOrDefault();

        //    return result;
        //}
    }
}
