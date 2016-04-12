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
    public class ItemKonversiController : BaseController
    {

        public JsonResult Save(SpMstItemConversion model)
        {
            string msg = "";
            string pT = ProductType;
            var record = ctx.SpMstItemConversions.Find(CompanyCode, pT, model.PartNo);

            if (record == null)
            {
                record = new SpMstItemConversion
                {
                    ProductType = pT,
                    CompanyCode = CompanyCode,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.SpMstItemConversions.Add(record);
                msg = "New item convertion added...";
            }
            else
            {
                ctx.SpMstItemConversions.Attach(record);
                msg = "Item convertion updated";
            }

            record.ProductType = pT;
            record.PartNo = model.PartNo;
            record.FromQty = model.FromQty;
            record.ToQty = model.ToQty;
            record.IsActive = model.IsActive; 

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


        public JsonResult Delete(SpMstItemConversion model)
        {
            var record = ctx.SpMstItemConversions.Find(CompanyCode, ProductType, model.PartNo);
             
            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.SpMstItemConversions.Remove(record);
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
