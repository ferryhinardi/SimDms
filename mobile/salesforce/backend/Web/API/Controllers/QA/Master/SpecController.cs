using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eXpressAPI.Models;
using Kendo.DynamicLinq;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("potaindo/qa/spec")]
    [Route("{action=List}")]
    public class specController : DefaultController
    {
        private string entityName = "Item Spec";

        public JsonResult List(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            string search = Request["search"];
            var list = db.SPECS.Where(x => x.CompanyCode == CompanyId).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.Description.Contains(search) ||  x.Value.Contains(search) );
            }



            if (sort == null)
            {
                list = list.OrderBy(x => x.No);
            }
            return Json(list.ToDataSourceResult(take, skip, sort, filter));
        }

        public JsonResult List2(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            string search = Request["search"];
            string itemid = Request["itemid"];

            //var list = db.SPECS.Where(x => x.CompanyCode == CompanyId && x.Status == true && x.TypeId == itemid).AsQueryable();

            //if (!string.IsNullOrEmpty(search))
            //{
            //    list = list.Where(x => x.Description.Contains(search) || x.Value.Contains(search));
            //}

            //var query = from a in list
            //            from b in db.SpecCategories.Where(x => x.CompanyCode == CompanyId && a.SpecCategory == x.Code && x.Status == true)
            //            select new { a.CompanyCode, a.No, a.ItemId, a.TypeId, a.Description, a.Value, a.SpecCategory, b.Name, a.Currency, a.Price };

            //if (sort == null)
            //{
            //    list = list.OrderBy(x => x.No);
            //}

            var ret = SqlQuery("exec sp_get_types_detail '" + CompanyId + "','" + itemid + "','" + search + "'");


            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridAdd(SPECS data)
        {
            SaveResult ret = new SaveResult(0, false);

            //if (data.SpecId == null)
            //{
            //    data.SpecId = MyGlobalVar.GetMD5(CompanyId + data.No + data.TypeId + data.ItemId + data.SpecCategory);
            //}

            var IsFound = db.SPECS.Find(CompanyId, data.No, data.TypeId, data.ItemId, data.SpecCategory);

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

        public JsonResult GridUpdate(SPECS data)
        {
            return Json(EntryUpdate(data, string.Format("Update data {0} berhasil",entityName)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridDelete(SPECS data)
        {
            return Json(EntryDelete(data, string.Format("Delete data {0} berhasil",entityName)), JsonRequestBehavior.AllowGet);
        }
        
    }
}