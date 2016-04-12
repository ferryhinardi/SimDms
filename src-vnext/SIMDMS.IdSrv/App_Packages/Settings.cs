#region Using

using System;
using System.Configuration;
using System.Web.Configuration;
using System.Web.WebPages;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using System.Reflection;
using System.Text;
using System.Web.Mvc.Html;
using Microsoft.Ajax.Utilities;
using eXpressAPI.Models;
using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;


#endregion

namespace eXpressAPP
{
    /// <summary>
    ///     Provides access to the current application's configuration file.
    /// </summary>
    public static class Settings
    {
        public static CodeEditorRepository myRepo = new CodeEditorRepository();

        /// <summary>
        ///     Specifies the default entry prefix value ("config").
        /// </summary>
        private const string Prefix = "config";

        /// <summary>
        ///     Retrieves the entry value for the following composed key: "config:Company" as a string.
        /// </summary>
        //public static string Company = GetValue<string>("Company");
        public static string Company
        {
            get
            {
                return GetValue<string>("Company");// DateTime.Now.ToString();
            }
        }

        /// <summary>
        ///     Retrieves the entry value for the following composed key: "config:Project" as a string.
        /// </summary>
        //public static string Project = GetValue<string>("Project");
        public static string Project
        {
            get
            {
                return GetValue<string>("Project");
            }
        }

        public static string DevelopmentMode
        {
            get
            {
                return GetValue<string>("DEVELOPMENT_MODE");
            }
        }

        /// <summary>
        ///     Retrieves the entry value for the following composed key: "config:EnableTiles" as a boolean.
        /// </summary>
        //public static  bool EnableTiles = GetValue<bool>("EnableTiles");
        public static bool EnableTiles
        {
            get
            {
                return GetValue<bool>("EnableTiles");
            }
        }

        /// <summary>
        ///     Retrieves the entry value for the following composed key: "config:EnableLoader" as a boolean.
        /// </summary>
        //public static  bool EnableLoader = GetValue<bool>("EnableLoader");
        public static bool EnableLoader
        {
            get
            {
                return GetValue<bool>("EnableLoader");
            }
        }

        /// <summary>
        ///     Retrieves the entry value for the following composed key: "config:CurrentTheme" as a string.
        /// </summary>
        //public static string CurrentTheme = GetValue<string>("CurrentTheme");
        public static string CurrentTheme
        {
            get
            {
                return GetValue<string>("CurrentTheme");
            }
        }

        //public static string CompanyLogo = GetValue<string>("CompanyLogo");
        public static string CompanyLogo
        {
            get
            {
                return GetValue<string>("CompanyLogo");
            }
        }

        //public static string Home = GetValue<string>("Home");
        public static string Home
        {
            get
            {
                var getHome = System.Configuration.ConfigurationManager.AppSettings["config:home"].ToString();
                return getHome;
            }
        }

        //public static string Connection = GetValue<string>("Connection");
        public static string Connection
        {
            get
            {
                return GetValue<string>("Connection");
            }
        }

        //public static string CertFile = GetValue<string>("CertFile");
        public static string CertFile
        {
            get
            {
                return GetValue<string>("CertFile");
            }
        }

        //public static string CertPwd = GetValue<string>("CertPwd");
        public static string CertPwd
        {
            get
            {
                return GetValue<string>("CertPwd");
            }
        }

        //public static string UseSSL = GetValue<string>("useSSL");
        public static string UseSSL
        {
            get
            {
                return GetValue<string>("useSSL");
            }
        }

        //public static string FullPath = GetValue<string>("fullPath");
        public static string FullPath
        {
            get
            {
                return GetValue<string>("fullPath");
            }
        }

        //public static string DisabledUI = GetValue<string>("Disabled-UI");
        public static string DisabledUI
        {
            get
            {
                return GetValue<string>("Disabled-UI");
            }
        }

        //public static string AppIdentity = GetValue<string>("AppIdentity");
        public static string AppIdentity
        {
            get
            {
                return GetValue<string>("AppIdentity");
            }
        }

        //public static string AppName = GetValue<string>("AppName");
        public static string AppName
        {
            get
            {
                return GetValue<string>("AppName");
            }
        }

        //public static string Copyright = GetValue<string>("Copyright");
        public static string Copyright
        {
            get
            {
                return GetValue<string>("Copyright");
            }
        }

        //public static string IssueUrl = GetValue<string>("IssueUrl");
        public static string IssueUrl
        {
            get
            {
                var getHome = System.Configuration.ConfigurationManager.AppSettings["config:issueurl"].ToString();
                return getHome;
            }
        }

        //public static string GoogleClientId = GetValue<string>("GoogleClientId");
        public static string GoogleClientId
        {
            get
            {
                return GetValue<string>("GoogleClientId");
            }
        }

        //public static string GoogleClientKey = GetValue<string>("GoogleClientKey");
        public static string GoogleClientKey
        {
            get
            {
                return GetValue<string>("GoogleClientKey");
            }
        }

        //public static string FacebookClientId = GetValue<string>("FacebookClientId");
        public static string FacebookClientId
        {
            get
            {
                return GetValue<string>("FacebookClientId");
            }
        }

        //public static string FacebookClientKey = GetValue<string>("FacebookClientKey");
        public static string FacebookClientKey
        {
            get
            {
                return GetValue<string>("FacebookClientKey");
            }
        }

        //public static string ConfigDB = GetValue<string>("configdb");
        public static string ConfigDB
        {
            get
            {
                return GetValue<string>("configdb");
            }
        }

        //public static string ReffDir = GetValue<string>("reffdir");
        public static string ReffDir
        {
            get
            {
                return GetValue<string>("reffdir");
            }
        }

        //public static string UnderContraction = GetValue<string>("UC_IMG");
        public static string UnderContraction
        {
            get
            {
                return GetValue<string>("UC_IMG");
            }
        }

        //public static string Favicon = GetValue<string>("FAVICON");
        public static string Favicon
        {
            get
            {
                return GetValue<string>("FAVICON");
            }
        }

        //public static string DefaultUser = GetValue<string>("DefaultUser");
        public static string DefaultUser
        {
            get
            {
                return GetValue<string>("DefaultUser");
            }
        }

        //public static string DefaultPassword = GetValue<string>("DefaultPassword");
        public static string DefaultPassword
        {
            get
            {
                return GetValue<string>("DefaultPassword");
            }
        }

        //public static string ReportUrl = GetValue<string>("ReportUrl");
        public static string ReportUrl
        {
            get
            {
                return GetValue<string>("ReportUrl");
            }
        }

        //public static string AttachmentDB = GetValue<string>("attachmentdb");
        public static string AttachmentDB
        {
            get
            {
                return GetValue<string>("attachmentdb");
            }
        }

        //public static string HeaderQuoMess = GetValue<string>("HeaderQuoMess");
        public static string HeaderQuoMess
        {
            get
            {
                return GetValue<string>("HeaderQuoMess");
            }
        }

        //public static string FooterQuoMess = GetValue<string>("FooterQuoMess");
        public static string FooterQuoMess
        {
            get
            {
                return GetValue<string>("FooterQuoMess");
            }
        }

        public static string MongoDbData
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["MongoDBData"].ConnectionString ?? "";
            }
        }

        //public static string MongoDbUIFiles
        //{
        //    get
        //    {
        //        return ConfigurationManager.ConnectionStrings["MongoServerSettings"].ConnectionString ?? "";
        //    }
        //}

        //public static string MongoDbCDN
        //{
        //    get
        //    {
        //        return ConfigurationManager.ConnectionStrings["CDN"].ConnectionString ?? "";
        //    }
        //}

        public static string DynamicIssueUrl
        {
            get
            {
                return HttpContext.Current.Request.Url.ToString();
            }
        }

        public static string CalculateMD5(string source)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }

            return "";
        }

        public static string Concat(params object[] args)
        {
            if (args.Length > 0)
            {
                string ret = "";
                foreach (var item in args)
                {
                    ret += " " + item;
                }
                return ret;
            }

            return "";
        }

        public static string GetSetting(string id)
        {
            return myRepo.GetSetting(id);
        }

        public static string CurrentUser()
        {
            var User = HttpContext.Current.User;

            if (!User.Identity.IsAuthenticated)
            {
                return "";
            }

            var myclaims = (User as System.Security.Claims.ClaimsPrincipal).Claims;
            var userid = myclaims.First(x => x.Type == "user_id").Value;
            return userid;
        }

        public static int CacheDuration
        {
            get
            {
                var result = 10;
                var ret = GetValue<string>("cache-duration");
                if (!string.IsNullOrEmpty(ret))
                {
                    result = Convert.ToInt32(ret);
                }
                return result;
            }
        }

        /// <summary>
        ///     Gets the entry for the given key and prefix and retrieves its value as the specified type.
        ///     <para>If no prefix is specified the default prefix value ("config") will be used.</para>
        ///     <para>
        ///         <example>e.g. GetValue&lt;string&gt;("config", "SettingName")</example>
        ///     </para>
        ///     Would result in checking the configuration file for a key named: "config:SettingName"
        /// </summary>
        /// <typeparam name="T">The type of which the value will be converted into and returned.</typeparam>
        /// <param name="prefix">The prefix of the entry to locate.</param>
        /// <param name="key">The key of the entry to locate.</param>
        /// <returns>The value of the entry, or the default value, as the specified type.</returns>
        public static T GetValue<T>(string key, string prefix = Prefix)
        {
            var entry = key;  // string.Format("{0}:{1}", prefix, key); -- key only on mongodb configuration

            // Make sure the key represents a possible valid entry
            if (entry.IsNullOrWhiteSpace())
                return default(T);

            var value = myRepo.GetSetting(entry); //ConfigurationManager.AppSettings[entry];

            // If the key is available but does not contain any value, return the default value of the specfied type
            if (value.IsNullOrWhiteSpace())
                return default(T);

            // In case the specified type is an enum, try to parse the entry as an enum value
            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), value, true);

            // In case the specified type is a bool and the entry value represents an integer
            if (typeof(T) == typeof(bool) && value.Is<int>())
                // We convert to value to an integer first before changing the entry value to the specified type
                return (T)Convert.ChangeType(value.As<int>(), typeof(T));

            // Change the entry value to the specified type
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        ///     To dynamical replace the appSettings "config:CurrentTheme"
        ///     Calling function such as Settings.SetValue<string>("config:CurrentTheme", "smart-style-0");
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">the key of the theme === config:CurrentTheme</param>
        /// <param name="value">the value of the theme === smart-style-{0}</param>
        /// <returns></returns>
        public static T SetValue<T>(string key, string value)
        {
            //var config = WebConfigurationManager.OpenWebConfiguration("~");

            //config.AppSettings.Settings[key].Value = value;
            //config.Save(ConfigurationSaveMode.Modified);
            //ConfigurationManager.RefreshSection("appSettings");
            var item = new KeyValues();
            item.key = key;
            item.value = value;

            myRepo.AddOrUpdateConfig(item);

            return default(T);
        }
    }

    public static class StringExtensions
    {
        /// <summary>
        ///     Removes dashes ("-") from the given object value represented as a string and returns an empty string ("")
        ///     when the instance type could not be represented as a string.
        ///     <para>
        ///         Note: This will return the type name of given isntance if the runtime type of the given isntance is not a
        ///         string!
        ///     </para>
        /// </summary>
        /// <param name="value">The object instance to undash when represented as its string value.</param>
        /// <returns></returns>
        public static string UnDash(this object value)
        {
            return ((value as string) ?? string.Empty).UnDash();
        }

        /// <summary>
        ///     Removes dashes ("-") from the given string value.
        /// </summary>
        /// <param name="value">The string value that optionally contains dashes.</param>
        /// <returns></returns>
        public static string UnDash(this string value)
        {
            return (value ?? string.Empty).Replace("-", string.Empty);
        }

        public static string Left(this string str, int length)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            if (length > str.Length)
            {
                length = str.Length;
            }
            return str.Substring(0, length);
        }

        public static string Right(this string str, int length)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return str.Substring(str.Length - length, length);
        }
    }

    public class DashRouteHandler : MvcRouteHandler
    {
        /// <summary>
        ///     Custom route handler that removes any dashes from the route before handling it.
        /// </summary>
        /// <param name="requestContext">The context of the given (current) request.</param>
        /// <returns></returns>
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var routeValues = requestContext.RouteData.Values;

            routeValues["action"] = routeValues["action"].UnDash();
            routeValues["controller"] = routeValues["controller"].UnDash();

            return base.GetHttpHandler(requestContext);
        }
    }

    public static class HtmlHelperExtensions
    {
        private static string _displayVersion;

        /// <summary>
        ///     Retrieves a non-HTML encoded string containing the assembly version as a formatted string.
        ///     <para>If a project name is specified in the application configuration settings it will be prefixed to this value.</para>
        ///     <para>
        ///         e.g.
        ///         <code>1.0 (build 100)</code>
        ///     </para>
        ///     <para>
        ///         e.g.
        ///         <code>ProjectName 1.0 (build 100)</code>
        ///     </para>
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static IHtmlString AssemblyVersion(this HtmlHelper helper)
        {
            if (_displayVersion.IsNullOrWhiteSpace())
                SetDisplayVersion();

            return helper.Raw(_displayVersion);
        }

        /// <summary>
        ///     Compares the requested route with the given <paramref name="value" /> value, if a match is found the
        ///     <paramref name="attribute" /> value is returned.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="value">The action value to compare to the requested route action.</param>
        /// <param name="attribute">The attribute value to return in the current action matches the given action value.</param>
        /// <returns>A HtmlString containing the given attribute value; otherwise an empty string.</returns>
        public static IHtmlString RouteIf(this HtmlHelper helper, string value, string attribute)
        {
            var currentController =
                (helper.ViewContext.RequestContext.RouteData.Values["controller"] ?? string.Empty).ToString().UnDash();
            var currentAction =
                (helper.ViewContext.RequestContext.RouteData.Values["action"] ?? string.Empty).ToString().UnDash();

            var hasController = value.Equals(currentController, StringComparison.InvariantCultureIgnoreCase);
            var hasAction = value.Equals(currentAction, StringComparison.InvariantCultureIgnoreCase);

            return hasAction || hasController ? new HtmlString(attribute) : new HtmlString(string.Empty);
        }

        /// <summary>
        ///     Renders the specified partial view with the parent's view data and model if the given setting entry is found and
        ///     represents the equivalent of true.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="partialViewName">The name of the partial view.</param>
        /// <param name="appSetting">The key value of the entry point to look for.</param>
        public static void RenderPartialIf(this HtmlHelper htmlHelper, string partialViewName, string appSetting)
        {
            var setting = Settings.GetValue<bool>(appSetting);

            htmlHelper.RenderPartialIf(partialViewName, setting);
        }

        /// <summary>
        ///     Renders the specified partial view with the parent's view data and model if the given setting entry is found and
        ///     represents the equivalent of true.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="partialViewName">The name of the partial view.</param>
        /// <param name="condition">The boolean value that determines if the partial view should be rendered.</param>
        public static void RenderPartialIf(this HtmlHelper htmlHelper, string partialViewName, bool condition)
        {
            if (!condition)
                return;

            htmlHelper.RenderPartial(partialViewName);
        }

        /// <summary>
        ///     Retrieves a non-HTML encoded string containing the assembly version and the application copyright as a formatted
        ///     string.
        ///     <para>If a company name is specified in the application configuration settings it will be suffixed to this value.</para>
        ///     <para>
        ///         e.g.
        ///         <code>1.0 (build 100) © 2015</code>
        ///     </para>
        ///     <para>
        ///         e.g.
        ///         <code>1.0 (build 100) © 2015 CompanyName</code>
        ///     </para>
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static IHtmlString Copyright(this HtmlHelper helper)
        {
            var copyright =
                string.Format("{0} &copy; {1} {2}", helper.AssemblyVersion(), DateTime.Now.Year, Settings.Company)
                    .Trim();

            return helper.Raw(copyright);
        }

        private static void SetDisplayVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            _displayVersion =
                string.Format("{4} {0}.{1}.{2} (build {3})", version.Major, version.Minor, version.Build,
                    version.Revision, Settings.Project).Trim();
        }

        /// <summary>
        ///     Returns an unordered list (ul element) of validation messages that utilizes bootstrap markup and styling.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="alertType">The alert type styling rule to apply to the summary element.</param>
        /// <param name="heading">The optional value for the heading of the summary element.</param>
        /// <returns></returns>
        public static HtmlString ValidationBootstrap(this HtmlHelper htmlHelper, string alertType = "danger",
            string heading = "")
        {
            if (htmlHelper.ViewData.ModelState.IsValid)
                return new HtmlString(string.Empty);

            var sb = new StringBuilder();

            sb.AppendFormat("<div class=\"alert alert-{0} alert-block\">", alertType);
            sb.Append("<button class=\"close\" data-dismiss=\"alert\" aria-hidden=\"true\">&times;</button>");

            if (!heading.IsNullOrWhiteSpace())
            {
                sb.AppendFormat("<h4 class=\"alert-heading\">{0}</h4>", heading);
            }

            sb.Append(htmlHelper.ValidationSummary());
            sb.Append("</div>");

            return new HtmlString(sb.ToString());
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

    public class MyOutputCacheAttribute : OutputCacheAttribute
    {
        public MyOutputCacheAttribute()
        {
            this.Duration = Settings.CacheDuration;
        }
    }
}