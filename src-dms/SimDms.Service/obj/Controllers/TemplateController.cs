using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers
{
    public class TemplateController : BaseController
    {
        public string Tmpl1()
        {
            return HtmlRender("tmpl/tmpl1.js");
        }
        
        public string Tmpl2()
        {
            return HtmlRender("tmpl/tmpl2.js");
        }
        
        public string Tmpl3()
        {
            return HtmlRender("tmpl/tmpl3.js");
        }
        
        public string Tmpl4()
        {
            return HtmlRender("tmpl/tmpl4.js");
        }

        public string Tmpl5()
        {
            return HtmlRender("tmpl/tmpl5.js");
        }

        public string Tmpl6()
        {
            return HtmlRender("tmpl/tmpl6.js");
        }

        public string Tmpl7()
        {
            return HtmlRender("tmpl/tmpl7.js");
        }

        public string Tmpl8()
        {
            return HtmlRender("tmpl/tmpl8.js");
        }

        public string Tmpl9()
        {
            return HtmlRender("tmpl/tmpl7.js");
        }

    }
}
