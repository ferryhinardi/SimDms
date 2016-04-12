using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using eXpressAPI.Models;
using eXpressAPI.Models.Core;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace eXpressAPI.Controllers
{

    [Authorize]
    public class DefaultController : Controller
    {
        protected DataAccessContext db = new DataAccessContext();

        protected string UserName
        {
            get {
                if (!User.Identity.IsAuthenticated)
                {
                    return "";
                }
                var myclaims = (User as ClaimsPrincipal).Claims;
                var _UserName = myclaims.First(x => x.Type == "user_name").Value;
                return _UserName;
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

        protected string UserId
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                {                         
                    return "";
                }
                var myclaims = (User as ClaimsPrincipal).Claims;
                var userid = myclaims.First(x => x.Type == "user_id").Value;
                return userid;
            }
        }


        protected string UserIdentityId
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return "";
                }
                var myclaims = (User as ClaimsPrincipal).Claims;
                var userid = myclaims.First(x => x.Type == "sub").Value;
                return userid;
            }
        }


        protected Exception HandleDataUpdateException(DbUpdateException exception)
        {
            Exception innerException = exception.InnerException;
            while (innerException.InnerException != null)
            {
                innerException = innerException.InnerException;
            }
            return new Exception(innerException.Message);
        }

        protected Exception HandleDataValidationException(DbEntityValidationException exception)
        {
            var stringBuilder = new StringBuilder();
            foreach (DbEntityValidationResult result in exception.EntityValidationErrors)
            {
                foreach (DbValidationError error in result.ValidationErrors)
                {
                    stringBuilder.AppendFormat("{0} [{1}]: {2}",
                        result.Entry.Entity.ToString().Split('.').Last(), error.PropertyName, error.ErrorMessage);
                    stringBuilder.AppendLine();
                }
            }
            return new Exception(stringBuilder.ToString().Trim());
        }


        public SaveResult SaveChanges()
        {
            var svRet = new SaveResult(0, false);
            try
            {
                db.AutoSaveChanges(UserId, CompanyId);
                svRet.success = true;
            }
            catch (DbEntityValidationException vex)
            {
                var exception1 = HandleDataValidationException(vex);
                svRet.message = exception1.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception1);
            }
            catch (DbUpdateException dbu)
            {
                var exception = HandleDataUpdateException(dbu);
                svRet.message = exception.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
            }
            return svRet;
        }

        public SaveResult EntryAdd(object t,string success_message = "")
        {
            db.Entry(t).State = System.Data.Entity.EntityState.Added;
            var ret = SaveChanges();
            if (ret.success)
            {
                ret.data = t;
                ret.message = success_message;
            }
            return ret;
        }

        public SaveResult EntryDelete(object t, string success_message = "")
        {
            db.Entry(t).State = System.Data.Entity.EntityState.Deleted;
            var ret = SaveChanges();
            if (ret.success)
            {
                ret.data = t;
                ret.message = success_message;
            }
            return ret;
        }

        public SaveResult EntryUpdate(object t, string success_message = "")
        {
            db.Entry(t).State = System.Data.Entity.EntityState.Modified;
            var ret = SaveChanges();
            if (ret.success)
            {
                ret.data = t;
                ret.message = success_message;
            }
            return ret;
        }

        public int GetRecordCount(object data, int index = 0)
        {
            int x = 0;
            try
            {
                DataSet dt = (DataSet)data;
                if (dt != null && dt.Tables.Count > 0)
                {
                    x = dt.Tables[index].Rows.Count;
                }
            } catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return x;
        }

        public List<ComboList> ListRoles()
        {
            List<ComboList> ret = new List<ComboList>();
            var listCo = db.Roles.Select(x => new { value = x.RoleId, text = x.Name }).ToList();
            foreach(var item in listCo)
            {
                var data = new ComboList(item.value, item.text);
                ret.Add(data);
            }
            return ret;
        }

        public List<ComboList> ListCompany()
        {
            List<ComboList> ret = new List<ComboList>();
            var listCo = db.CompanyProfile.Select(x => new { value = x.Code, text = x.Name }).ToList();
            foreach (var item in listCo)
            {
                var data = new ComboList(item.value, item.text);
                ret.Add(data);
            }
            return ret;
        }

        public List<ComboList> ListCompanyByRole()
        {
            return ListCompany();
        }



        protected bool HaveRole(string rolename)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return false;
            }
            var myclaims = (User as ClaimsPrincipal).Claims;
            bool allow = false;

            foreach (var item in myclaims)
            {
                if (item.Type == "role" && item.Value.ToLower() == rolename.ToLower())
                {
                    allow = true;
                    break;
                }
            }

            return allow;
        }

        public SaveResult SqlQuery(string sql)
        {
            string DataSourceDB = System.Configuration.ConfigurationManager.ConnectionStrings["DataAccessContext"].ToString();

            SaveResult ret = new SaveResult(0, false);
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            using (var connection = factory.CreateConnection())
            {
                using (var command = factory.CreateCommand())
                {
                    try
                    {
                        connection.ConnectionString = DataSourceDB;
                        command.Connection = connection;
                        command.CommandTimeout = 3600;
                        command.CommandText = sql;
                        connection.Open();
                        DataSet dt = new DataSet();
                        var da = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);
                        ret.success = true;
                        ret.data = dt;
                        if (dt != null && dt.Tables.Count > 0)
                        {
                            ret.Count = dt.Tables[0].Rows.Count;
                        }
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        ret.message = ex.Message;
                        if (ex.InnerException != null)
                        {
                            if (!string.IsNullOrEmpty(ex.InnerException.Message))
                            {
                                ret.message += System.Environment.NewLine + ex.InnerException.Message;
                            }
                        }
                    }
                    finally
                    {
                        if (command.Connection.State == ConnectionState.Open)
                            command.Connection.Close();
                    }
                }
            }

            return ret;
        }

        protected string CompanyId
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return "";
                }
                var myclaims = (User as ClaimsPrincipal).Claims;
                var userid = myclaims.First(x => x.Type == "user_id").Value;
                var userInfo = db.Users.Find(userid);
                if (userInfo != null)
                {
                    return userInfo.Code;
                }
                return "1";
            }
        }

        public void UserLogout()
        {            
            if (!User.Identity.IsAuthenticated)
            {
                return;
            }

            var myclaims = (User as ClaimsPrincipal).Claims;
            var userid = myclaims.First(x => x.Type == "user_id").Value;
            var userInfo = db.Users.Find(userid);
            if (userInfo != null)
            {
                userInfo.IsLogin = 0;
                db.Entry(userInfo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            HttpContext.GetOwinContext().Authentication.SignOut();
        }

        protected string RoleId
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return "";
                }
                var myclaims = (User as ClaimsPrincipal).Claims;
                var _RoleId = myclaims.First(x => x.Type == "user_role").Value;
                return _RoleId;
            }
        }

        protected bool IsEditable(string MenuId)
        {
            return db.IsEditable(MenuId, RoleId);
        }

        public IEnumerable<SysNavigation> GenerateNavigationMenu(bool IsDeveloper = false)
        {
            var data = db.Database.SqlQuery<Menus>("exec sp_role_checkmenu '" + RoleId + "'").ToList();

            var dataMenus = (from x in data
                             where x.MenuLevel > 0
                             select x).ToList();

            var rootMenu = (from x in data where x.MenuLevel == 0 select x).ToList();
                      
            List<SysNavigation> Navigator = new List<SysNavigation>();
            List<SysNavigation> QuickNavigator = new List<SysNavigation>();

            foreach (var x in rootMenu)
            {
                var y = new SysNavigation();
                y.MenuCaption = x.Name;
                y.MenuId = x.MenuId;
                y.Url = "#";
                y.Icon = x.MenuPict;
                ListNavMenus(ref y, ref QuickNavigator, dataMenus, x.MenuId, x.MenuId, x.Name, x.Name, IsDeveloper);
                Navigator.Add(y);
            }

            ViewBag.menus = QuickNavigator; // (new JsonResult().Data = new { data = QuickNavigator });

            return Navigator;

        }

        private void ListNavMenus(ref SysNavigation refMenu, ref List<SysNavigation> OutMenus,
            List<Menus> menus, string ModuleName,
            string header, string GroupName, string ParentName, bool IsDeveloper = false)
        {
            var myMenus = (from q in menus
                           where q.Parent == header
                           select q).OrderBy(x => x.Seq).ToList();

            foreach (var x in myMenus)
            {
                var y = new SysNavigation();
                y.MenuCaption = x.Name;
                y.MenuId = x.MenuId;
                y.ModuleName = GroupName;
                y.ParentName = ParentName;

                if (string.IsNullOrEmpty(x.Link))
                {
                    y.Url = "#";
                }
                else
                {
                    if (IsDeveloper)
                    {
                        y.Url = "#" + x.Link;
                    }
                    else
                    {
                        y.Url = "#/" + x.Link;
                    }                    
                    OutMenus.Add(y);
                }

                y.Icon = x.MenuPict;
                ListNavMenus(ref y, ref OutMenus, menus, ModuleName, y.MenuId, GroupName, ParentName + " / " + y.MenuCaption, IsDeveloper);

                refMenu.Detail.Add(y);
            }
        }        
    }


    class ElmahResult : ActionResult
    {
        private string _resouceType;

        public ElmahResult(string resouceType)
        {
            _resouceType = resouceType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var factory = new Elmah.ErrorLogPageFactory();
            if (!string.IsNullOrEmpty(_resouceType))
            {
                var pathInfo = "." + _resouceType;
                HttpContext.Current.RewritePath(HttpContext.Current.Request.Path, pathInfo, HttpContext.Current.Request.QueryString.ToString());
            }

            var handler = factory.GetHandler(HttpContext.Current, null, null, null);

            handler.ProcessRequest(HttpContext.Current);
        }
    }

    public class ErrorHandlingActionInvoker : ControllerActionInvoker
    {
        private readonly IExceptionFilter filter;

        public ErrorHandlingActionInvoker(IExceptionFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            this.filter = filter;
        }

        protected override FilterInfo GetFilters(
        ControllerContext controllerContext,
        ActionDescriptor actionDescriptor)
        {
            var filterInfo =
            base.GetFilters(controllerContext,
            actionDescriptor);

            filterInfo.ExceptionFilters.Add(this.filter);

            return filterInfo;
        }
    }

    public class ErrorHandlingControllerFactory : DefaultControllerFactory
    {
        public override IController CreateController(
        RequestContext requestContext,
        string controllerName)
        {
            var controller =
            base.CreateController(requestContext,
            controllerName);

            var c = controller as Controller;

            if (c != null)
            {
                c.ActionInvoker =
                new ErrorHandlingActionInvoker(
                new HandleErrorWithELMAHAttribute());
            }

            return controller;
        }
    }
}