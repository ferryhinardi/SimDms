using SimDms.Tax;
using SimDms.Tax.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Diagnostics;
using ClosedXML.Excel;
using System.Data;
using System.Data.SqlClient;

namespace SimDms.Tax.Controllers.Api
{
    public class UtilityController : BaseController
    {
        public JsonResult FpjConfig(string FpjID)
        {
            var FpjValue = ctx.TxFpjConfigs.SingleOrDefault(f => f.FpjID == FpjID).FpjValue;
            return Json(new { success = true, FpjValue = FpjValue });
        }

        public JsonResult Save(TxFpjConfig model)
        {
            var record = ctx.TxFpjConfigs.Find(model.FpjID);

            if (record == null)
            {
                record = new TxFpjConfig
                {
                    FpjID = model.FpjID,
                    FpjValue = model.FpjValue,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = ctx.CurrentTime,
                    UpdateBy = CurrentUser.UserId,
                    UpdateDate = ctx.CurrentTime
                };
                ctx.TxFpjConfigs.Add(record);
            }
            else
            {
                record.FpjValue = model.FpjValue;
                record.UpdateBy = CurrentUser.UserId;
                record.UpdateDate = ctx.CurrentTime;
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult GetListTax(DateTime DateFrom, DateTime DateTo)
        {
            var sqlstr = ctx.Database.SqlQuery<GenerateTaxView>("uspfn_GnListTax '" + CompanyCode + "','" + DateFrom + "','" + DateTo + "'").AsQueryable();
            return Json(sqlstr);
        }

        public JsonResult save2(string FPJGovNo, string FPJGovNoNew, string DocNo)
        {
            try
            {
                var sqlstr = ctx.Database.ExecuteSqlCommand("uspfn_GnUpdateTax '" + CompanyCode + "','" + FPJGovNo + "','" + FPJGovNoNew + "','" + DocNo + "','" + CurrentUser.UserId + "'");
                return Json(new { success = true, result = sqlstr });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
                
        }
    }
}
