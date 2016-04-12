using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;
 
namespace SimDms.Service.Controllers.Api
{
    public class PartMaterialController : BaseController
    {
        //
        // GET: /PartMaterial/

        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
            });
        }

        public JsonResult Get(string PartNo)
        {
            var items = ctx.Items.Find(CompanyCode, BranchCode, PartNo);
            var itemlocs = ctx.ItemLocs.Find(CompanyCode, BranchCode, PartNo, "00");
            var iteminfos = ctx.ItemInfos.Find(CompanyCode, PartNo);
            var itemprices = ctx.ItemPrices.Find(CompanyCode, BranchCode, PartNo);

            var modelInfo = from a in ctx.ItemModels
                            join d in ctx.LookUpDtls
                            on new { a.ModelCode, a.CompanyCode }
                            equals new { ModelCode = d.LookUpValue, d.CompanyCode }
                            where a.CompanyCode == CompanyCode 
                            && a.PartNo == PartNo
                            && d.CodeID == "MODL"
                            select new
                            {
                                Code = a.ModelCode,
                                Description = d.LookUpValueName
                            };

            var func = string.Format(@"select * from GetModifikasi ('{0}')", PartNo);
            var list = ctx.Database.SqlQuery<ItemMod>(func);

            int num = 0;

            var interchange = from a in list
                            where a.ID != PartNo
                            select new
                            {
                                No = ++num,
                                InterChangeCode = a.InterChangeCode,
                                NewPartNo = a.ID,
                                UnitConversion = "1"
                            };

            return Json(new { item = items, itemloc = itemlocs, iteminfo = iteminfos, itemprice = itemprices, modelinfo = modelInfo, itemmod = interchange });
        }
    }
}
