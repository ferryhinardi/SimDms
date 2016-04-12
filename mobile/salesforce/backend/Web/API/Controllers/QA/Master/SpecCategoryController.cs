﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eXpressAPI.Models;
using Kendo.DynamicLinq;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("potaindo/qa/speccategory")]
    [Route("{action=List}")]
    public class SPecCategoryController : DefaultController
    {
        private string entityName = "Kategori Spec";

        public JsonResult List(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            string search = Request["search"];
            var list = db.SpecCategories.Where(x => x.CompanyCode == CompanyId).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.Description.Contains(search) || x.Name.Contains(search) || x.Code.Contains(search) );
            }
            if (sort == null)
            {
                list = list.OrderBy(x => x.GroupNo).ThenBy(x => x.Seq);
            }
            return Json(list.ToDataSourceResult(take, skip, sort, filter));
        }

        public JsonResult GridAdd(SpecCategory data)
        {
            SaveResult ret = new SaveResult(0, false);
            var IsFound = db.SpecCategories.Where(x => x.Code == data.Code && x.CompanyCode == CompanyId).ToList();
            if (IsFound.Count == 0)
            {
                ret = EntryAdd(data, string.Format("Buat data {0} berhasil",entityName));
            }
            else
            {
                ret.message = string.Format("Data {0} dengan Id yang Anda masukkan sudah tersedia",entityName);
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridUpdate(SpecCategory data)
        {
            return Json(EntryUpdate(data, string.Format("Update data {0} berhasil",entityName)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridDelete(SpecCategory data)
        {
            return Json(EntryDelete(data, string.Format("Delete data {0} berhasil",entityName)), JsonRequestBehavior.AllowGet);
        }
        
    }


}