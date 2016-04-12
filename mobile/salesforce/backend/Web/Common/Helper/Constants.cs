using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helper
{
    public static class ControllerName
    {
        public const string Error = "Error";
        public const string Home = "Home";
    }

    public static class ErrorControllerAction
    {
        public const string BadRequest = "BadRequest";
        public const string Forbidden = "Forbidden";
        public const string InternalServerError = "InternalServerError";
        public const string MethodNotAllowed = "MethodNotAllowed";
        public const string NotFound = "NotFound";
        public const string Unauthorized = "Unauthorized";
    }

    public static class ErrorControllerRoute
    {
        public const string GetBadRequest = ControllerName.Error + "GetBadRequest";
        public const string GetForbidden = ControllerName.Error + "GetForbidden";
        public const string GetInternalServerError = ControllerName.Error + "GetInternalServerError";
        public const string GetMethodNotAllowed = ControllerName.Error + "GetMethodNotAllowed";
        public const string GetNotFound = ControllerName.Error + "GetNotFound";
        public const string GetUnauthorized = ControllerName.Error + "Unauthorized";
    }

    public static class CacheProfileName
    {
        public const string BadRequest = "BadRequest";
        public const string BrowserConfigXml = "BrowserConfigXml";
        public const string Feed = "Feed";
        public const string Forbidden = "Forbidden";
        public const string InternalServerError = "InternalServerError";
        public const string ManifestJson = "ManifestJson";
        public const string MethodNotAllowed = "MethodNotAllowed";
        public const string NotFound = "NotFound";
        public const string OpenSearchXml = "OpenSearchXml";
        public const string RobotsText = "RobotsText";
        public const string Unauthorized = "Unauthorized";
    }

}
