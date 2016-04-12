using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eXpressAPI.Models;
using Kendo.DynamicLinq;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("potaindo/qa/contact")]
    [Route("{action=List}")]
    public class ContactController : DefaultController
    {
        private string entityName = "Contact";
        
        public JsonResult List(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            string search = Request["search"];
            string code = Request["code"];
            string tipe = Request["tipe"];

            var menuId = tipe == "CONTACT"
                ? "21200"
                : tipe == "SALES" ? "12100" : "";

            if (IsAllowAccess(menuId, "listdetail") != "1")
            {
                SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
                Response.StatusCode = 401;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            var list = db.Contacts.Where(x => x.CompanyCode == CompanyId && x.Code == code && x.ContactType == tipe).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.Name.Contains(search) || x.Code.Contains(search));
            }
            if (sort == null)
            {
                list = list.OrderBy(x => x.Code);
            }
            return Json(list.ToDataSourceResult(take, skip, sort, filter));
        }

        public JsonResult GridAdd(Contact data)
        {
            SaveResult ret = new SaveResult(0, false);

            var menuId = data.ContactType == "CONTACT"
                ? "21200"
                : data.ContactType == "SALES" ? "12100" : "";

            if (IsAllowAccess(menuId, "adddetail") != "1")
            {
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            if (data.No == "<AUTO>")
            {
                data.No = GetAutoNo(data.ContactType);
            }
            data.ContactId = MyGlobalVar.GetCRC32(data.CompanyCode + data.No + data.Code + data.ContactType);
            var IsFound = db.Contacts.Find(CompanyId, data.No, data.Code);
            if (IsFound == null)
            {

                ret = EntryAdd(data, string.Format("Buat data {0} berhasil",entityName));
            }
            else
            {
                ret.message = string.Format("Data {0} dengan Id yang Anda masukkan sudah tersedia",entityName);
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridUpdate(Contact data)
        {
            var menuId = data.ContactType == "CONTACT"
                ? "21200"
                : data.ContactType == "SALES" ? "12100" : "";

            if (IsAllowAccess(menuId, "updatedetail") == "1")
                return Json(EntryUpdate(data, string.Format("Update data {0} berhasil", entityName)),
                    JsonRequestBehavior.AllowGet);

            SaveResult ret = new SaveResult(0, false) {message = "Access denied"};
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridDelete(Contact data)
        {
            var menuId = data.ContactType == "CONTACT"
                ? "21200"
                : data.ContactType == "SALES" ? "12100" : "";

            if (IsAllowAccess(menuId, "deletedetail") == "1")
                return Json(EntryDelete(data, string.Format("Delete data {0} berhasil", entityName)),
                    JsonRequestBehavior.AllowGet);

            SaveResult ret = new SaveResult(0, false) {message = "Access denied"};
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
    }
}