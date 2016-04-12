using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers
{
    public class ReviewController : BaseController
    {
        public string InputReview()
        {
            return HtmlRender("review/inputreview.js");
        }

        public string Reviews()
        {
            return HtmlRender("review/reviews.js");
        }
    }
}