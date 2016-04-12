using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SimDms.Web.Controllers
{
    public class Layout3Controller : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.BranchCode = BranchCode;
            ViewBag.BranchName = BranchName;
            return View();
        }

        public JsonResult Test()
        {
            List<extField> myfields = new List<extField> { 
                new extField() { name="id", type="int" } ,
                new extField() { name="name", type="string" } ,
                new extField() { name="phone", type="string" } ,
                new extField() { name="email", type="string" } 
            };

            List<extGridColumn> myColumns = new List<extGridColumn> { 
                new extGridColumn() { dataIndex = "id", header="ID", width=100 } ,
                new extGridColumn() { dataIndex = "name", header="Nama", width=100 } ,
                new extGridColumn() { dataIndex = "phone", header="Telepon", width=100 } ,
                new extGridColumn() { dataIndex = "email", header="Email", width=100 } 
            };

            return Json(new
            {
                metadData = new { idProperty = "id", totalProperty = "total", successProperty = "success", root = "data", fields = myfields },
                columns = myColumns,
                total = 7,
                success = true,
                message = "berhasil",
                data = new List<extData>
                {
                    new extData (){id=1,name="Osen Kusnadi", phone="081808153100", email="support@osenxpsuite.net" },
                    new extData (){id=2,name="Osen Kusnadi 1", phone="081808153100", email="support@osenxpsuite.net" },
                    new extData (){id=3,name="Osen Kusnadi 2", phone="081808153100", email="support@osenxpsuite.net" },
                    new extData (){id=4,name="Osen Kusnadi 3", phone="081808153100", email="support@osenxpsuite.net" },
                    new extData (){id=5,name="Osen Kusnadi 4", phone="081808153100", email="support@osenxpsuite.net" },
                    new extData (){id=6,name="Osen Kusnadi 5", phone="081808153100", email="support@osenxpsuite.net" },
                    new extData (){id=7,name="Osen Kusnadi 6", phone="081808153100", email="support@osenxpsuite.net" }
                }
            }, JsonRequestBehavior.AllowGet );

        }

    }

public class extField
{
    public string name { get; set; }
    public string type { get; set; }
}

public class extData
{
    public int id { get; set; }
    public string name { get; set; }
    public string phone { get; set; }
    public string email { get; set; }
}

public class extGridColumn
{
    public string dataIndex { get; set; }
    public string header { get; set; }
    public int width { get; set; }
}

}
