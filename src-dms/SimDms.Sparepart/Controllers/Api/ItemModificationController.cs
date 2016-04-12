using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;

namespace SimDms.Sparepart.Controllers.Api
{
    public class ItemModificationController : BaseController
    {

        
        public JsonResult GetObject(string SQL)
        {

            var sqlquery = SQL;
            var data = ctx.Database.SqlQuery<SpMstItemModifInfo>(sqlquery);
            return Json(data, JsonRequestBehavior.AllowGet); 
        }




        public JsonResult Save(spMstItemMod model)
        {
            string msg = "";
            var record = ctx.spMstItemMods.Find(CompanyCode, model.PartNo, model.NewPartNo);

            if (record == null)
            {
                record = new spMstItemMod
                {
                   
                    CompanyCode = CompanyCode,
                  CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spMstItemMods.Add(record);
                msg = "New item convertion added...";
            }
            else
            {
                ctx.spMstItemMods.Attach(record);
                msg = "Item convertion updated";
            }


            record.PartNo = model.PartNo;
            record.NewPartNo = model.NewPartNo;
            record.UnitConversion = model.UnitConversion;
            record.InterChangeCode = model.InterChangeCode;
            record.ProductType = model.ProductType;
            record.PartCategory = model.PartCategory;
            record.EndMark = model.EndMark; 


            try
            {
                ctx.SaveChanges();
              

                return Json(new { success = true, message = msg, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }




        public JsonResult Delete(spMstItemMod model)
        {
            var record = ctx.spMstItemMods.Find(CompanyCode, model.PartNo);

 
            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.spMstItemMods.Remove(record);
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

 
 
 
 
 
    }
}
