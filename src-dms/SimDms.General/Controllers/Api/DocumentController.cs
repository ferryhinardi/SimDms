using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
using SimDms.Sparepart.Models;
using SimDms.General.Models.Others;
using SimDms.Common;
using SimDms.Common.Models;
using System.Web.Script.Serialization;
using TracerX;
using System.Transactions;


namespace SimDms.General.Controllers.Api
{
    public class DocumentController : BaseController   
    {          
        [HttpPost]
        public JsonResult Save(GnMstDocument model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var document = ctx.GnMstDocuments.Find(companyCode, branchCode, model.DocumentType);

            if (document == null)
            {
                document = new GnMstDocument();
                document.CreatedDate = currentTime;
                document.LastUpdateDate = currentTime;
                document.CreatedBy = userID;
                ctx.GnMstDocuments.Add(document);
            }
                else{
                    document.LastUpdateDate = currentTime;
                    document.LastUpdateBy = userID;
            }               
                document.DocumentType = model.DocumentType;
                document.DocumentPrefix = model.DocumentPrefix;
                document.DocumentName = model.DocumentName;
                document.ProfitCenterCode = model.ProfitCenterCode;
                document.DocumentYear = model.DocumentYear;
                document.DocumentSequence = model.DocumentSequence;
                document.CompanyCode = companyCode;
                document.BranchCode = branchCode;
               
                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Data document Class berhasil disimpan.";
                    result.data = new
                    {
                        DocumentType = document.DocumentType,
                        DocumentName = document.DocumentName 
                    };
                }
                catch (Exception Ex)
                {
                    result.message = "Data document tidak bisa disimpan.";
                    MyLogger.Info("Error on document saving: " + Ex.Message);
                }
            
            return Json(result);
        }

        public JsonResult Delete(GnMstDocument model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var document = ctx.GnMstDocuments.Find(companyCode, branchCode, model.DocumentType);
                    if (document != null)
                    {
                        ctx.GnMstDocuments.Remove(document);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data document class berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete document , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete customer class , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        public JsonResult Get(string documentType)
        {
            try
            {
                var document = ctx.GnMstDocuments.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode, documentType);
                if (document != null)
                {
                    return Json(new { result = true, data = document }); 
                }
                else
                {
                    return Json(new { result = false, message = "" }); 
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = ex.Message });    
            }
        }
    }
}
