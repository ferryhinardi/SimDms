using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Sparepart.Controllers.Api
{
    public class ReportInventoryController : BaseController
    {


        public JsonResult Signature()
        {
            var record = ctx.GnMstSignatures.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProfitCenterCode == ProfitCenter && a.DocumentType == "ADJ").FirstOrDefault();
            try
            {
                if (record != null)
                {
                    return Json(new { success = true, signName = record.SignName, titleSign = record.TitleSign });
                }
                else {
                    return Json(new { success = true, signName = "", titleSign = "" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }

        public JsonResult SetID()
        {
            var qr = ctx.GnMstCoProfiles.Find(CompanyCode, BranchCode).CityCode;
            var record = ctx.LookUpDtls.Find(CompanyCode, "CITY", qr).LookUpValueName;
            try
            {
                if (record != null)
                {
                    return Json(new { success = true, cityCode = qr, cityName = record });
                }
                else
                {
                    return Json(new { success = true, cityCode = "", cityName = "" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }

    }
}
