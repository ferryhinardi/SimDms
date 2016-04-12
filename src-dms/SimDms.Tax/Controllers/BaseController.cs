using GeLang;
using SimDms.Common;
using SimDms.Common.Models;
using SimDms.Tax.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TracerX;
using System.Text;

namespace SimDms.Tax.Controllers
{
    public class BaseController : Controller
    {

        protected DataContext ctx;       

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            ctx = new DataContext(MyHelpers.GetConnString("DataContext"));

            if (User != null && User.Identity.IsAuthenticated)
            {
                ctx.CurrentUser = User.Identity.Name;
            }
            else
            {
                ctx.CurrentUser = "Guest";
            }
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

        protected string HtmlRender(string jsname)
        {
            return string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/tax/"), jsname);
        }

        protected string HtmlRender(string id, string jsname)
        {
            var jshtml = "";
            if (!string.IsNullOrWhiteSpace(jsname))
            {
                jshtml = string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/tax/"), jsname);
            }
            return string.Format(@"<div id=""{0}"" ></div>", id) + jshtml;
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

        protected SysUser CurrentUser
        {
            get
            {
                var user = ctx.SysUsers.Find(User.Identity.Name);
                user.CoProfile = ctx.CoProfiles.Find(user.CompanyCode, user.BranchCode);
                return user;
            }
            set { }
        }

        protected SysUser CurrentUserByUname(string username)
        {
            return ctx.SysUsers.Find(User.Identity.Name);
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

        protected string BranchName
        {
            get
            {
                return ctx.CoProfiles.Find(CompanyCode, BranchCode).CompanyName;
            }
        }

        protected string ProductType
        {
            get
            {
                return ctx.CoProfiles.Find(CompanyCode, BranchCode).ProductType;
            }
        }

        protected string ProfitCenter
        {
            get
            {
                string s = "000";
                var x = ctx.SysUserProfitCenters.Find(CurrentUser.UserId);
                if (x != null) s = x.ProfitCenter;
                return s;
            }
        }

        protected string ProfitCenterName
        {
            get
            {
                string name = "";
                name = ctx.LookUpDtls.Find(CompanyCode, "PFCN", ProfitCenter).LookUpValueName;
                return name;
            }
        }

        protected string TypeOfGoods
        {
            get
            {
                return CurrentUser.TypeOfGoods;
            }
        }        

        protected string defaultSpParam(string sp, string id = null, bool branch = true)
        {
            string s = " '" + CompanyCode + "'";

            if (branch)
            {
                s += ",'" + BranchCode + "'";
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                s += ",'" + id + "'";
            }
            else
            {
                string p = Request["sSearch"].ToString();

                if (!string.IsNullOrWhiteSpace(p))
                {
                    s += ",'" + p + "'";
                }
            }

            return sp + s;
        }

        public DataResult<T> eXecSp<T>(string spName, string id = null, bool branch = true) where T : class
        {
            string s = spName + " '" + CompanyCode + "'";

            if (branch)
            {
                s += ",'" + BranchCode + "'";
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                s += "," + id;
            }
            else
            {

                if (Request["sSearch"] != null)
                {
                    string p = Request["sSearch"].ToString();

                    if (!string.IsNullOrWhiteSpace(p))
                    {
                        s += ",'" + p + "'";
                    }
                }
            }

            var data = ctx.Database.SqlQuery<T>(s).AsQueryable();
            return DataTableSp<T>.Parse(data, Request);
        }

        public DataResult<T> eXecSQL<T>(string SQL) where T : class
        {
            var data = ctx.Database.SqlQuery<T>(SQL).AsQueryable();
            return DataTableSp<T>.Parse(data, Request);
        }

        public DataResult<T> eXecScalar<T>(string SQL) where T : class
        {

            var data = ctx.Database.SqlQuery<T>(SQL).AsQueryable();
            return DataTableSp<T>.Parse(data, Request);
        }

        protected string GetNewDocumentNo(string doctype, DateTime transdate)
        {
            var sql = "uspfn_GnDocumentGetNew {0}, {1}, {2}, {3}, {4}";
            var result = ctx.Database.SqlQuery<string>(sql, CompanyCode, BranchCode, doctype, CurrentUser.UserId, transdate);
            return result.First();
        }

        protected string GetNewDocumentNoHpp(string doctype, string transdate)
        {
            var sql = "uspfn_GnDocumentGetNew {0}, {1}, {2}, {3}, {4}";
            var result = ctx.Database.SqlQuery<string>(sql, CompanyCode, BranchCode, doctype, CurrentUser.UserId, transdate);
            return result.First();
        }

        //protected string DealerCode()
        //{
        //    var result = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
        //    if (result.CompanyMD == CompanyCode && result.BranchMD == BranchCode) { return "MD"; }
        //    else { return "SD"; }
        //}

        //protected string CompanyMD
        //{
        //    get
        //    {
        //        return ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).CompanyMD;
        //    }
        //}

        //protected string BranchMD
        //{
        //    get
        //    {
        //        return ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).BranchMD;
        //    }
        //}

        //protected string WarehouseMD
        //{
        //    get
        //    {
        //        return ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).WarehouseMD;
        //    }
        //}
       
    }
}
