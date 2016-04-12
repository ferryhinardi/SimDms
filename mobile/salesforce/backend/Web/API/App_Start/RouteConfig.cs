﻿using System.Web.Mvc;
using System.Web.Routing;

namespace eXpressAPI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "DefaultAPIController",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "MyApi", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}