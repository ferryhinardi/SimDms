using SimDms.SUtility.Models.Dcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.SUtility.Controllers.Api
{
    public class LookupController : BaseController
    {
        [HttpPost]
        public JsonResult Dealers()
        {
            var data = (from x in ctxLink.DealerInfos
                       orderby x.ShortName ascending
                       select new {
                            Text = x.DealerCode + " - " + x.ShortName + " (" + x.ProductType + ")",
                            Value = x.DealerCode                                
                       }).ToList();

            data.Insert(0, new
            {
                Text = "[Select One]",
                Value = ""
            });
            return Json(data);
        }

        [HttpPost]
        public JsonResult DcsCustomerDataID()
        {
            var data = (from x in ctx.DcsDownloads
                        select new
                        {
                            Text = x.DataID.ToUpper(),
                            Value = x.DataID.ToUpper()
                        }).Distinct().ToList();

            return Json(data);
        }
    }
}
