using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eXpressAPI.Models;
using Kendo.DynamicLinq;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("potaindo/qa/vendor")]
    [Route("{action=List}")]
    public class VendorController : DefaultController
    {
        private string entityName = "Vendor";

        public JsonResult List(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            string search = Request["search"];
            var list = db.Vendors.Where(x => x.CompanyCode == CompanyId).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x =>   x.Name.Contains(search) || x.Code.Contains(search) );
            }
            if (sort == null)
            {
                list = list.OrderByDescending(x => x.Code);
            }
            return Json(list.ToDataSourceResult(take, skip, sort, filter));
        }

        public JsonResult GridAdd(Vendor data)
        {
            SaveResult ret = new SaveResult(0, false);
            var IsFound = db.Vendors.Where(x => x.Code == data.Code && x.CompanyCode == CompanyId).ToList();

            if (IsFound.Count == 0)
            {
                data.CompanyCode = CompanyId;
                ret = EntryAdd(data, string.Format("Buat data {0} berhasil",entityName));
            }
            else
            {
                ret.message = string.Format("Data {0} dengan Id yang Anda masukkan sudah tersedia",entityName);
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridUpdate(Vendor data)
        {
            return Json(EntryUpdate(data, string.Format("Update data {0} berhasil",entityName)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridDelete(Vendor data)
        {
            return Json(EntryDelete(data, string.Format("Delete data {0} berhasil",entityName)), JsonRequestBehavior.AllowGet);
        }
        
    }
}