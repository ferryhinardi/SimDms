using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCHEMON.WebServer.Configs
{
    public class RazorView
    {
        private ExecuteContext context = new ExecuteContext();

        public string TemplateName { get; set; }
        public string Template { get; set; }
        public object Model { get; set; }

        public dynamic ViewBag
        {
            get
            {
                return context.ViewBag;
            }
        }

        public RazorView()
        {
        }

        public RazorView(string templateName)
        {
            this.TemplateName = templateName;
        }

        public RazorView(string templateName, object model)
            : this(templateName)
        {
            this.Model = model;
        }

        public string Run()
        {
            string Url = System.Configuration.ConfigurationManager.AppSettings["views"].ToString();
            Url = Path.Combine(Url.Replace(@"{APPPATH}", Environment.CurrentDirectory), this.TemplateName);
            string ctx = File.ReadAllText(Url);
            return Razor.Parse(ctx, this.Model);
        }
    }

}
