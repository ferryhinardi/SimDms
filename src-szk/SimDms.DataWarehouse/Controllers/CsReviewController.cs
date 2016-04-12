using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers
{
    public class CsReviewController : BaseController
    {
        public string Reviews() 
        {
            return HtmlRender("cs/review/Reviews.js");
        }

        public string inputReview()
        {
            return HtmlRender("cs/review/input-review.js");
        }
    }
}