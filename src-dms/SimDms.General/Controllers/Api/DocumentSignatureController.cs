using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
using SimDms.Sparepart.Models;
using SimDms.General.Models.Others;
using SimDms.Common;
using System.Web.Script.Serialization;
using TracerX;
using System.Transactions;
using SimDms.Common.Models;


namespace SimDms.General.Controllers.Api
{
    public class DocumentSignatureController : BaseController 
    {          
        [HttpPost]
        public JsonResult Save(GnMstSignature model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var signature = ctx.GnMstSignatures.Find(companyCode, branchCode, model.ProfitCenterCode, model.DocumentType, model.SeqNo);

            if (signature == null)
            {
                signature = new GnMstSignature();
                signature.CreatedDate = currentTime;
                signature.LastUpdateDate = currentTime;
                signature.CreatedBy = userID;
                ctx.GnMstSignatures.Add(signature);
            }
                else{
                    signature.LastUpdateDate = currentTime;
                    signature.LastUpdateBy = userID;
            }
            signature.CompanyCode = companyCode;
            signature.BranchCode = branchCode;
            signature.ProfitCenterCode = model.ProfitCenterCode;
            signature.DocumentType = model.DocumentType;
            signature.SeqNo = model.SeqNo;
            signature.SignName = model.SignName;
            signature.TitleSign = model.TitleSign;
                        
                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Data Signature berhasil disimpan.";
                    result.data = new
                    {
                        DocumentType = signature.DocumentType,
                        SignName = signature.SignName
                    };
                }
                catch (Exception Ex)
                {
                    result.message = "Data Signature tidak bisa disimpan.";
                    MyLogger.Info("Error on Signature saving: " + Ex.Message);
                }
            
            return Json(result);
        }

        public JsonResult Delete(GnMstSignature model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var signature = ctx.GnMstSignatures.Find(companyCode, branchCode, model.ProfitCenterCode, model.DocumentType, model.SeqNo);
                    if (signature != null)
                    {
                        ctx.GnMstSignatures.Remove(signature);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Signature berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Signature , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Signature , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        public JsonResult CheckLookUpDtl(string DocumentType, string BranchCode, string CodeID, string ProfitCenterCode, int SeqNo) 
        {
            var record = ctx.GnMstSignatures.Find(CompanyCode, BranchCode, ProfitCenterCode, DocumentType, SeqNo);
            var titleName = ctx.LookUpDtls.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.CodeID == CodeID && a.LookUpValue == ProfitCenterCode).LookUpValueName;
            if (record != null)
            {
                return Json(new
                {
                    success = true,
                    data = record,
                    TitleName = titleName 
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true });
        }

        public JsonResult CheckDocument(string DocumentType)
        {
            //var record = ctx.GnMstSignatures.Find(CompanyCode, BranchCode, ProfitCenterCode, DocumentType, SeqNo);
            var titleName = ctx.GnMstDocuments.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocumentType == DocumentType).DocumentName;
            if (titleName != null)
            {
                return Json(new
                {
                    success = true,
                    data = titleName,
                    TitleName = titleName
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true });
        }
    }
}
