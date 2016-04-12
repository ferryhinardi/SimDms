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
    public class CompanyaccController : BaseController
    {
        public JsonResult CompanyaccDetailsLoad(string strCompanyCodeTo)
        {
            return Json( ctx.Database.SqlQuery<spMstCompanyAccountdtlview>("uspfn_spMstCompanyAccountDtl2 '" + CompanyCode + "','" + strCompanyCodeTo + "'").AsQueryable());
         }

        public JsonResult Save(spMstCompanyAccount model)
        {
            string msg = "";
            var record = ctx.spMstCompanyAccounts.Find(CompanyCode, model.CompanyCodeTo);

            if (record == null)
            {
                record = new spMstCompanyAccount
                {
                    CompanyCode = CompanyCode,
                    CompanyCodeTo = model.CompanyCodeTo ?? "",
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now

                };

                ctx.spMstCompanyAccounts.Add(record);
                msg = "New company account added...";
            }
            else
            {
                ctx.spMstCompanyAccounts.Attach(record);
                msg = "company account updated";
            }

                    record.CompanyCodeToDesc = model.CompanyCodeToDesc ?? "";
                    record.BranchCodeTo = model.BranchCodeTo ?? "";
                    record.BranchCodeToDesc = model.BranchCodeToDesc ?? "";
                    record.WarehouseCodeTo = model.WarehouseCodeTo ?? "";
                    record.WarehouseCodeToDesc = model.WarehouseCodeToDesc ?? "";
                    record.UrlAddress = model.UrlAddress;
                    record.isActive = model.isActive;
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;
        
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

        public JsonResult Delete(spMstCompanyAccount model)
        {

            var query = string.Format(@"
                        select distinct d.lookupvaluename TPGO, a.InterCompanyAccNoTo AccountNo
                    from spMstCompanyAccountDtl a
                    left join gnmstaccount c on c.companycode=a.companycode
                        and c.branchcode={1}
                        and c.accountno=a.intercompanyaccnoto
                    left join gnMstLookUpDtl d on d.companycode=a.companycode
                        and d.codeid='TPGO'
                        and d.lookupvalue=a.TPGO
                    where a.companycode = {0}
                            and a.companycodeto = {2} ", CompanyCode, BranchCode, model.CompanyCodeTo);

            var recordDetail = ctx.Database.SqlQuery<spMstCompanyAccountdtl2>(query);
            if (recordDetail.Count() > 0)
            {
                return Json(new { success = false, message = "Data tidak bisa dihapus, karena masih ada detail !" });
            }
            
            var record = ctx.spMstCompanyAccounts.Find(CompanyCode, model.CompanyCodeTo);
 
            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.spMstCompanyAccounts.Remove(record);
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

        public JsonResult Save2(spMstCompanyAccountdtl2 model)
        {
            string msg = "";
            var record = ctx.spMstCompanyAccountdtls.Find(CompanyCode,model.CompanyCodeTo , model.TPGO);

            if (record == null)
            {
                record = new spMstCompanyAccountdtl
                {
                    CompanyCode = CompanyCode,
                    CompanyCodeTo = model.CompanyCodeTo ?? "",
                    TPGO = model.TPGO,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

              ctx.spMstCompanyAccountdtls.Add(record);
                msg = "New company account details added...";
            }
            else
            {
                ctx.spMstCompanyAccountdtls.Attach(record);
                msg = "company account details updated";
            }
               
            record.InterCompanyAccNoTo = model.AccountNo;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
 
            try
            {
                ctx.SaveChanges();

                var records = ctx.Database.SqlQuery<spMstCompanyAccountdtlview>("uspfn_spMstCompanyAccountDtl2 '" + CompanyCode + "','" + model.CompanyCodeTo + "'").AsQueryable();

                return Json(new { success = true, message = msg, data = record, result = records });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete2(spMstCompanyAccountdtl2 model)
        {
            var record = ctx.spMstCompanyAccountdtls.Find(CompanyCode, model.CompanyCodeTo, model.TPGO);
             
            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.spMstCompanyAccountdtls.Remove(record);
            }

            try
            {
                ctx.SaveChanges();

                var records = ctx.Database.SqlQuery<spMstCompanyAccountdtlview>("uspfn_spMstCompanyAccountDtl2 '" + CompanyCode + "','" + model.CompanyCodeTo + "'").AsQueryable();

                return Json(new { success = true, result = records });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult getThisRecord(string CompanyCodeTo)
        {
            //var record = ctx.spMstCompanyAccounts.Find(CompanyCode, CompanyCodeTo);
            var record = from p in ctx.spMstCompanyAccounts
                         join p1 in ctx.spMstCompanyAccountdtls
                            on p.CompanyCodeTo equals p1.CompanyCodeTo
                         join p2 in ctx.gnMstAccounts
                            on p1.InterCompanyAccNoTo equals p2.AccountNo
                         join p3 in ctx.LookUpDtls
                            on p1.TPGO equals p3.LookUpValue
                         where p.CompanyCodeTo == CompanyCodeTo && p3.CodeID == "TPGO"
                         select new {
                                p.CompanyCodeTo, p.CompanyCodeToDesc, p.BranchCodeTo, p.BranchCodeToDesc,
                                p.WarehouseCodeTo,
                                p.WarehouseCodeToDesc,
                                p.CompanyCode,
                                p.UrlAddress,
                                p1.TPGO, 
                                TPGOName = p3.LookUpValueName,
                                AccountNo = p1.InterCompanyAccNoTo,
                                AccountName = p2.Description,
                            };
            if (record != null)
            {
                return Json(new { success = true, data = record.FirstOrDefault() });
            }
            else 
            {
                return Json(new { success = false });
            }
            
        }
    }
}
