using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eXpressAPI.Models;
using Kendo.DynamicLinq;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("potaindo/qa/customer")]
    [Route("{action=List}")]
    public class CustomerController : DefaultController
    {
        private string entityName = "Konsumen";
        private string menuId = "21200";

        public JsonResult List(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            if (IsAllowAccess(menuId, "list") != "1")
            {
                SaveResult ret = new SaveResult(0, false) {message = "Access denied"};
                Response.StatusCode = 401;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            string search = Request["search"];
            var list = db.Customers.Where(x => x.CompanyCode == CompanyId).AsQueryable();

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

        public JsonResult GridAdd(Customer data)
        {
            SaveResult ret = new SaveResult(0, false);

            if (IsAllowAccess(menuId, "add") != "1")
            {
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            var isFound = db.Customers.Where(x => x.Code == data.Code && x.CompanyCode == CompanyId).ToList();

            if (isFound.Count == 0)
            {
                data.CompanyCode = CompanyId;
                data.Code = GetAutoNo("CUSTOMER");
                ret = EntryAdd(data, string.Format("Buat data {0} berhasil",entityName));
            }
            else
            {
                ret.message = string.Format("Data {0} dengan Id yang Anda masukkan sudah tersedia",entityName);
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridUpdate(Customer data)
        {
            if (IsAllowAccess(menuId, "update") == "1")
                return Json(EntryUpdate(data, string.Format("Update data {0} berhasil", entityName)),
                    JsonRequestBehavior.AllowGet);

            SaveResult ret = new SaveResult(0, false) {message = "Access denied"};
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridDelete(Customer data)
        {
            if (IsAllowAccess(menuId, "delete") == "1")
                return Json(EntryDelete(data, string.Format("Delete data {0} berhasil", entityName)),
                    JsonRequestBehavior.AllowGet);

            SaveResult ret = new SaveResult(0, false) {message = "Access denied"};
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
    }
}