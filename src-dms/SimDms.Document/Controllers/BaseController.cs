using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Document.Models;

namespace SimDms.Document.Controllers
{
    public class BaseController : Controller
    {
        protected DataContext ctx = new DataContext();

        
    }
}
