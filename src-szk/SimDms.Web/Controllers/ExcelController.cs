using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Web.Controllers
{
    public class ExcelController : Controller
    {
        public string Export()
        {
            var name = Request["name"];
            var html = Request["html"];

            if (string.IsNullOrWhiteSpace(name)) name = "XlsData";

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";

            Response.Output.Write(HttpUtility.HtmlDecode(html));
            Response.Flush();
            Response.End();

            return name;
        }
    }
}
