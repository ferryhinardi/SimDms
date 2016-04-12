using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using eXpressAPI.Models;
using Kendo.DynamicLinq;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("sm/coprofile")]
    [Route("{action=List}")]
    public class ComprofileController : DefaultController
    {
        private string entityName = "Institusi";

        public JsonResult List(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            string search = Request["search"];
            var list = db.CompanyProfile.AsQueryable();
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

        public JsonResult GridAdd(CompanyProfile data)
        {
            SaveResult ret = new SaveResult(0, false);

            try
            {
                var IsFound = db.CompanyProfile.Where(x => x.Code == data.Code).ToList();

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

        public JsonResult GridUpdate(CompanyProfile data)
        {
            return Json(EntryUpdate(data, string.Format("Update data {0} berhasil",entityName)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridDelete(CompanyProfile data)
        {
            return Json(EntryDelete(data, string.Format("Delete data {0} berhasil",entityName)), JsonRequestBehavior.AllowGet);
        }
        
    }
}