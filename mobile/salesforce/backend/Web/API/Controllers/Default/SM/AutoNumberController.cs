using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eXpressAPI.Models;
using Kendo.DynamicLinq;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("sm/setupnumber")]
    [Route("{action=List}")]
    public class AutoNumberController : DefaultController
    {
        private string entityName = "Config Auto Number";

        public JsonResult List(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            string search = Request["search"];
            var list = db.SetupNumber.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.Code.Contains(search) || x.Name.Contains(search));
            }
            if (sort == null)
            {
                list = list.OrderBy(x => x.Code);
            }
            return Json(list.ToDataSourceResult(take, skip, sort, filter));
        }

        public JsonResult GridAdd(ConfigNumber data)
        {
            SaveResult ret = new SaveResult(0, false);

            try
            {
                var IsFound = db.SetupNumber.Where(x => x.CompanyCode == CompanyId && x.Code == data.Code ).ToList();

                if (IsFound.Count == 0)
                {
                    ret = EntryAdd(data, string.Format("Buat data {0} berhasil",entityName));
                }
                else
                {
                    ret.message = string.Format("Data {0} dengan code yang Anda masukkan sudah tersedia",entityName);
                }

            } catch (Exception ex)
            {

            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridUpdate(ConfigNumber data)
        {
            return Json(EntryUpdate(data, string.Format("Update data {0} berhasil",entityName)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridDelete(ConfigNumber data)
        {
            return Json(EntryDelete(data, string.Format("Delete data {0} berhasil",entityName)), JsonRequestBehavior.AllowGet);
        }
        
    }
}