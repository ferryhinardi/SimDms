using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sales.Controllers
{
    public class UtilityController : BaseController
    {
        public string uploadfile()
        {
            return HtmlRender("utility/uploadfile.js");
        }

        public string sendfile()
        {
            return HtmlRender("utility/sendfile.js");
        }

        public string postingjurnal()
        {
            return HtmlRender("utility/postingjurnal.js");
        }

        public string repostingjurnal()
        {
            return HtmlRender("utility/repostingjurnal.js");
        }

        public string tutupbulan()
        {
            return HtmlRender("utility/tutupbulan.js");
        }

        public string uploadfromdcs()
        {
            return HtmlRender("utility/uploadfromdcs.js");
        }

        public string uploaddatadealer()
        {
            return HtmlRender("utility/uploaddatadealer.js");
        }

        public string uploadpolda()
        {
            return HtmlRender("utility/uploadpolda.js");
        }

        public string generatepolda()
        {
            return HtmlRender("utility/generatepolda.js");
        }

    }
}
