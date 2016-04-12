using SimDms.Absence.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Api
{
    public class LookupController : BaseController
    {
        [HttpPost]
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName
            });
        }

        [HttpPost]
        public JsonResult Save()
        {
            ResultModel result = InitializeResult();
            string codeID = Request["CodeID"] ?? "";
            string lookUpValue = Request["LookupValue"] ?? "";
            string lookupValueName = Request["LookUpValueName"] ?? "";
            string codeDescription = Request["CodeDescription"] ?? "";
            string paraValue = Request["ParaValue"] ?? "";
            string strSeqNo = Request["SeqNo"] ?? "0";
            int seqNo = Convert.ToInt32(strSeqNo);

            var mapdata = ctx.HrLookupMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode
                                            && x.CodeID == codeID);

            if (mapdata == null)
            {
                mapdata = new Models.HrLookupMapping();
                mapdata.CompanyCode = CompanyCode;
                mapdata.CodeID = codeID;
                mapdata.CodeDescription = codeDescription;
                ctx.HrLookupMappings.Add(mapdata);
            }

            var data = ctx.GnMstLookupDtls.Where(x =>
                    x.CompanyCode.Equals(CurrentUser.CompanyCode) == true
                    &&
                    x.CodeID.Equals(codeID) == true
                    &&
                    x.LookUpValue.Equals(lookUpValue) == true
                ).FirstOrDefault();

            if (data == null)
            {
                data = new Models.GnMstLookupDtl();
                data.CompanyCode = CompanyCode;
                data.CodeID = codeID;
                data.LookUpValue = lookUpValue;
                data.CreatedBy = CurrentUser.UserId;
                data.CreatedDate = DateTime.Now;

                ctx.GnMstLookupDtls.Add(data);
            }
                data.LookupValueName = lookupValueName;
                data.ParaValue = paraValue;
                data.SeqNo = seqNo;
                data.LastUpdateBy = CurrentUser.UserId;
                data.LastUpdateDate = DateTime.Now;
            
            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete()
        {
            ResultModel result = InitializeResult();
            string codeID = Request["CodeID"] ?? "";
            string lookUpValue = Request["LookupValue"] ?? "";
            string lookupValueName = Request["LookUpValueName"] ?? "";
            string codeDescription = Request["CodeDescription"] ?? "";
            string paraValue = Request["ParaValue"] ?? "";
            string strSeqNo = Request["SeqNo"] ?? "0";
            int seqNo = Convert.ToInt32(strSeqNo);

            var data = ctx.GnMstLookupDtls.Where(x =>
                    x.CompanyCode.Equals(CurrentUser.CompanyCode) == true
                    &&
                    x.CodeID.Equals(codeID) == true
                    &&
                    x.LookUpValue.Equals(lookUpValue) == true
                ).FirstOrDefault();

            if (data != null)
            {
                ctx.GnMstLookupDtls.Remove(data);
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
