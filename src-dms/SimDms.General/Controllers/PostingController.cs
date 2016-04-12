using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.General.Controllers
{
    public class PostingController : BaseController
    {
        public string Welcome()
        {
            return HtmlRender("posting/welcome.js");
        }
        public string Daily()
        {
            return HtmlRender("posting/daily.js");
        }
    }
}
