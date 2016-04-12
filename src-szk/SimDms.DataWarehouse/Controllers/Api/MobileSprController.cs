using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class MobileSprController : BaseController
    {
        public ContentResult DealerList(string callback)
        {
            var json = Exec(new
            {
                query = "uspm_sprdealer_list",
                result = "table"
            });

            return Content(String.Format("{0}({1});",
                callback,
                new JavaScriptSerializer().Serialize(json.Data)),
                "application/javascript");
        }

        public ContentResult OutletList(string callback)
        {
            var json = Exec(new
            {
                query = "uspm_sproutlet_list",
                param = new List<dynamic>
                {
                    new { key = "DealerCode", value = Request.Params["DealerCode"] },
                },
                result = "table"
            });

            return Content(String.Format("{0}({1});",
                callback,
                new JavaScriptSerializer().Serialize(json.Data)),
                "application/javascript");
        }

        public ContentResult SaveBox(string callback)
        {
            var strBox = Request.Params["box"];
            var objBox = JsonConvert.DeserializeObject<dynamic>(strBox);
            var caseno = objBox.caseno.Value;
            var savedate = objBox.savedate.Value;
            var username = objBox.username.Value;
            var outlet = objBox.outlet.Value;
            var parts = objBox.parts;
            var boxid = Guid.NewGuid().ToString().ToUpper();

            var header = Exec(new
            {
                query = "uspm_box_input",
                param = new List<dynamic>
                {
                    new { key = "BoxID", value = boxid },
                    new { key = "CaseNo", value = caseno },
                    new { key = "SaveDate", value = savedate },
                    new { key = "UserID", value = username },
                    new { key = "BranchCode", value = outlet }
                },
                result = "row"
            });

            foreach (var part in parts)
            {
                var partno = part.PartNo.Value;
                var qty = part.Qty.Value;

                var detail = Exec(new
                {
                    query = "uspm_part_input",
                    param = new List<dynamic>
                    {
                        new { key = "BoxID", value = boxid },
                        new { key = "PartNo", value = partno },
                        new { key = "PartQty", value = qty },
                    },
                    result = "row"
                });
            }

            return Content(String.Format("{0}({1});", callback,
                new JavaScriptSerializer().Serialize(objBox)),
                "application/javascript");
        }
    }
}
