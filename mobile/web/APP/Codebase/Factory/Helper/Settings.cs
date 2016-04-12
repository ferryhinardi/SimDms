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

#endregion

namespace eXpressAPP
{
    /// <summary>
    ///     Provides access to the current application's configuration file.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        ///     Specifies the default entry prefix value ("config").
        /// </summary>
        private const string Prefix = "config";

        /// <summary>
        ///     Retrieves the entry value for the following composed key: "config:Company" as a string.
        /// </summary>
        public static readonly string Company = GetValue<string>("Company");

        /// <summary>
        ///     Retrieves the entry value for the following composed key: "config:Project" as a string.
        /// </summary>
        public static readonly string Project = GetValue<string>("Project");

        /// <summary>
        ///     Retrieves the entry value for the following composed key: "config:EnableTiles" as a boolean.
        /// </summary>
        public static readonly bool EnableTiles = GetValue<bool>("EnableTiles");

        /// <summary>
        ///     Retrieves the entry value for the following composed key: "config:EnableLoader" as a boolean.
        /// </summary>
        public static readonly bool EnableLoader = GetValue<bool>("EnableLoader");

        /// <summary>
        ///     Retrieves the entry value for the following composed key: "config:CurrentTheme" as a string.
        /// </summary>
        public static readonly string CurrentTheme = GetValue<string>("CurrentTheme");

        public static readonly string CompanyLogo = GetValue<string>("CompanyLogo");

        public static readonly string Home = GetValue<string>("Home");

        public static readonly string Connection = GetValue<string>("Connection");

        public static readonly string CertFile = GetValue<string>("CertFile");

        public static readonly string CertPwd = GetValue<string>("CertPwd");

        public static readonly string UseSSL = GetValue<string>("useSSL");

        public static readonly string FullPath = GetValue<string>("fullPath");

        public static readonly string DisabledUI = GetValue<string>("Disabled-UI");

        public static readonly string AppIdentity = GetValue<string>("AppIdentity");

        public static readonly string AppName = GetValue<string>("AppName");

        public static readonly string Copyright = GetValue<string>("Copyright");

        public static readonly string IssueUrl = GetValue<string>("IssueUrl");

        public static readonly string GoogleClientId = GetValue<string>("GoogleClientId");

        public static readonly string GoogleClientKey = GetValue<string>("GoogleClientKey");

        public static readonly string FacebookClientId = GetValue<string>("FacebookClientId");

        public static readonly string FacebookClientKey = GetValue<string>("FacebookClientKey");

        public static readonly string ConfigDB = GetValue<string>("configdb");

        public static readonly string ReffDir = GetValue<string>("reffdir");

        public static readonly string UnderContraction = GetValue<string>("UC_IMG");

        public static readonly string Favicon = GetValue<string>("FAVICON");

        public static readonly string DefaultUser = GetValue<string>("DefaultUser");

        public static readonly string DefaultPassword = GetValue<string>("DefaultPwd");

        public static readonly string JobViewer = GetValue<string>("JobViewer");

        public static readonly string LDAP = GetValue<string>("LDAP");

        public static readonly string Domain = GetValue<string>("Domain");

        public static readonly string ExternalLogin = GetValue<string>("ExternalLogin");

        public static readonly string TokenURL = GetValue<string>("TokenURL");

        public static readonly string GlimpseRole = GetValue<string>("GlimpseRole"); 

        public static string VersionInfo
        {
            get {
                return "";// DateTime.Now.ToString();
            }
        }

        public static string DynamicIssueUrl
        {
            get
            {
                return HttpContext.Current.Request.Url.ToString();
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
            var entry = string.Format("{0}:{1}", prefix, key);

            // Make sure the key represents a possible valid entry
            if (entry.IsNullOrWhiteSpace())
                return default(T);

            
            var value = ConfigurationManager.AppSettings[entry];

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
            var config = WebConfigurationManager.OpenWebConfiguration("~");

            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

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

    public class ErrorHandlingControllerFactory : DefaultControllerFactory
    {
        public override IController CreateController(
        RequestContext requestContext,
        string controllerName)
        {

            if (requestContext == null)
            {
                return null;
            }

            try
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
            } catch(Exception ex)
            {
                return null;
            }

        }
    }

}