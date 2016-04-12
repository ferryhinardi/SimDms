using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SCHEMON.WebServer.Configs
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { action = RouteParameter.Optional ,  id = RouteParameter.Optional }
            );

            //var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            //jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }

    public class UserX
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ChatHub : Hub
    {
        public static List<UserX> UsersOnline = new List<UserX>();
        public void UserSignin(string username, string connectionId)
        {
            UsersOnline.Add(new UserX
            {
                Id = connectionId,
                Name = username
            });

            Clients.All.receiveUser(UsersOnline);
        }

        public void Test(string username)
        {
            Clients.All.test(username);
        }
    }

}
