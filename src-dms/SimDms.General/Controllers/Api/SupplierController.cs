using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using SimDms.Sparepart.Models;
using SimDms.General.Models;
using System.Transactions;
using SimDms.General.Models.Others;
using SimDms.Common;
using SimDms.Sparepart.Models;

namespace SimDms.General.Controllers.Api
{
    public class SupplierController : BaseController
    {
        public JsonResult PopulateSupplierDetails(string SupplierCode)
        {
            var sql = string.Format(@"SELECT  
         a.*, c.LookUpValueName as ProvinceName, d.LookUpValueName as AreaName, e.LookUpValueName as CityName,
         c.LookUpValueName as ProvinceName
        FROM gnMstSupplier a WITH(NOLOCK, NOWAIT)  
        LEFT JOIN gnMstSupplierProfitCenter b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode   
            AND a.SupplierCode = b.SupplierCode AND b.IsBlackList = 0
        LEFT JOIN  gnMstLookUpDtl c ON c.CompanyCode = a.CompanyCode and c.LookUpValue = a.ProvinceCode
        and c.CodeID = 'PROV'
        left JOIN gnMstLookUpDtl d ON d.CompanyCode = a.CompanyCode and d.LookUpValue = a.AreaCode
        and d.CodeID = 'AREA'
        left JOIN gnMstLookUpDtl e ON e.CompanyCode = a.CompanyCode and e.LookUpValue = a.CityCode
        and e.CodeID = 'CITY'
        WHERE a.CompanyCode = '{0}' and a.SupplierCode = '{1}' AND a.Status = 1", CompanyCode, SupplierCode);
            var supplier = ctx.Database.SqlQuery<SupplierDetails>(sql).FirstOrDefault();
            return Json(new { success = true, data = supplier });
        }

        public JsonResult PopulateSupplierPCList(string SupplierCode)
        {
            var sqlSuppPC = string.Format(@"select a.*, b.LookUpValueName as ProfitCenterName from gnMstSupplierProfitCenter a join GnMstLookupDtl b on a.CompanyCode = b.CompanyCode and b.CodeID = 'PFCN' and a.ProfitCenterCode = b.LookUpValue 
                            where a.CompanyCode='{0}' and BranchCode = '{1}' and SupplierCode ='{2}'", CompanyCode, BranchCode, SupplierCode);
            var suppPC = ctx.Database.SqlQuery<SupplierProfitCenterView>(sqlSuppPC).ToList();
            return Json(new { success = true, data = suppPC });
        }

        public JsonResult PopulateSupplierPCDetails(string SupplierCode, string ProfitCenterCode)
        {
            var sqlSuppPC = string.Format(@"select a.*, e.LookUpValueName as SupplierGradeName, b.LookUpValueName  as ProfitCenterName , d.Description as TaxCodeName, c.SupplierClassName
                            from gnMstSupplierProfitCenter a left join GnMstLookupDtl b on a.CompanyCode = b.CompanyCode and b.CodeID = 'PFCN' and a.ProfitCenterCode = b.LookUpValue
                            left join gnMstSupplierClass c on c.CompanyCode = a.CompanyCode and c.BranchCode = a.BranchCode and c.SupplierClass = a.SupplierClass
                            left join gnMstTax d on d.CompanyCode = a.CompanyCode and d.TaxCode = a.TaxCode
                            left JOIN gnMstLookUpDtl e ON e.CompanyCode = a.CompanyCode and e.LookUpValue = a.SupplierGrade and e.CodeID  = 'SPGR'                            
                            where a.CompanyCode='{0}' and a.BranchCode = '{1}' and a.SupplierCode ='{2}' and a.ProfitCenterCode='{3}'", CompanyCode, BranchCode, SupplierCode, ProfitCenterCode);
            var suppPC = ctx.Database.SqlQuery<SupplierProfitCenterView>(sqlSuppPC).FirstOrDefault();
            return Json(new { success = true, data = suppPC });
        }

        public JsonResult PopulateSupplierBankList(string SupplierCode)
        {
            var suppBank = ctx.GnMstSupplierBanks.Where(m => m.CompanyCode == CompanyCode && m.SupplierCode == SupplierCode).ToList();
            return Json(new { success = true, data = suppBank });
        }

        public JsonResult SaveSupplier(SupplierDetails model)
        {
            try
            {
                bool save = false;
                var record = ctx.GnMstSuppliers.Find(CompanyCode, model.SupplierCode);
                GnMstSupplierUtility recordUtl = null;
                var constrainGenerate = p_ConstrainGenerate(ref recordUtl);

                using (var transScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    bool isNewSupplier = false;
                    string newSupplierCode = "";
                    if (record == null)
                    {
                        isNewSupplier = true;
                        if (constrainGenerate)
                        {
                            decimal newSeq = (decimal)recordUtl.Sequence + 1;
                            newSupplierCode = rowGenerator(newSeq.ToString());
                            record = ctx.GnMstSuppliers.Find(CompanyCode, newSupplierCode);
                            if (record != null)
                            {
                                throw new ArgumentException("Harap periksa settingan no sequence utility supplier");
                            }
                            record = new GnMstSupplier();
                            record.SupplierCode = newSupplierCode;
                            record.StandardCode = newSupplierCode;
                        }
                        else
                        {
                            record = new GnMstSupplier();
                            record.SupplierCode = model.SupplierCode;
                            record.StandardCode = model.StandardCode;
                        }
                        record.CompanyCode = CompanyCode;
                        record.CreatedBy = CurrentUser.UserId;
                        record.CreatedDate = DateTime.Now;
                    }

                    //record.StandardCode = model.StandardCode;
                    record.SupplierName = model.SupplierName;
                    record.Address1 = model.Address1;
                    record.Address2 = model.Address2;
                    record.Address3 = model.Address3;
                    record.Address4 = model.Address4;
                    record.PhoneNo = model.PhoneNo;
                    record.FaxNo = model.FaxNo;
                    record.Status = model.Status;
                    record.SupplierGovName = model.SupplierGovName;
                    record.ProvinceCode = model.ProvinceCode;
                    record.AreaCode = model.AreaCode;
                    record.CityCode = model.CityCode;
                    record.ZipNo = model.ZipNo;
                    record.isPKP = model.isPKP;
                    record.NPWPNo = model.NPWPNo;
                    record.NPWPDate = model.NPWPDate;
                    record.HPNo = model.HPNo;
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;
                    record.isLocked = false;

                    Helpers.ReplaceNullable(record);
                    if (isNewSupplier)
                    {
                        try
                        {
                            ctx.GnMstSuppliers.Add(record);
                            ctx.SaveChanges();

                            if (constrainGenerate)
                            {
                                recordUtl.Sequence = recordUtl.Sequence + 1;
                                recordUtl.LastUpdateBy = CurrentUser.UserId;
                                recordUtl.LastUpdateDate = DateTime.Now;

                                Helpers.ReplaceNullable(recordUtl);
                                ctx.SaveChanges();
                            }
                        }
                        catch
                        {
                            throw new Exception("Harap ulangi proses penyimpanan !");
                        }
                    }
                    else
                    {
                        ctx.SaveChanges();
                    }

                    transScope.Complete();
                    save = true;
                }

                return Json(new { success = save, message = "Data Supplier berhasil disimpan", SupplierCode = record.SupplierCode });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error pada saat menyimpan Data, Harap ulangi proses penyimpanan !", error_log = ex.Message });
            }
        }

        private string rowGenerator(string row)
        {
            return row.ToString().PadLeft(8, '0');
        }

        public JsonResult OnPageLoad()
        {
            object returnObj = null;
            string IsDisableSC = "true";
            string MsgDisableSC = "";
            string IsPkp = "false";
            string IsDisableIsPkp = "true";
            //string MsgDisableIsPkp = "";
            bool success = true;

            try
            {
                var OrgDtl = ctx.GnMstOrganizationDtls.Where(m => m.CompanyCode == CompanyCode && m.IsBranch == false).ToList();
                if (OrgDtl.Count > 1)
                {
                    throw new ArgumentException("Periksa setting-an organisasi, jumlah holding lebih dari satu");
                }
                var recorUtil = ctx.GnMstSupplierUtilities.Find(CompanyCode, BranchCode);
                if (recorUtil == null || recorUtil.IsAutoGenerate == null)
                {
                    IsDisableSC = "false";
                }
                else
                {
                    if (recorUtil.IsAutoGenerate.Value)
                    {
                        IsDisableSC = "true";
                    }
                    else
                    {
                        IsDisableSC = "false";
                    }
                }
            }
            catch (Exception ex)
            {
                MsgDisableSC = ex.Message.ToString();
                success = false;
            }

            // Setting IsEnable Set Faktur Pajak Standard
            var oFlag1 = ctx.LookUpDtls.Find(CompanyCode, "FPJSTD", "EDITABLE");
            var oFlag2 = ctx.LookUpDtls.Find(CompanyCode, "FPJSTD", "SUPPLIER");
            if (oFlag1 != null && oFlag1.ParaValue == "1")
            {
                if (oFlag2 == null || oFlag2.ParaValue == "1")
                    IsPkp = "true";
                else
                    IsPkp = "false";
                IsDisableIsPkp = "false";
            }
            else
            {
                if (oFlag2 == null || oFlag2.ParaValue == "1")
                    IsPkp = "true";
                else
                    IsPkp = "false";
                IsDisableIsPkp = "true";
            }
            returnObj = new { success = success, IsDisableSC = IsDisableSC, MsgDisableSC = MsgDisableSC, IsPkp = IsPkp, IsDisableIsPkp = IsDisableIsPkp };
            return Json(returnObj);
        }

        public bool checkSupplierCode()
        {
            var constrainGenerate = false;
            var recordUtl = p_ConstrainGenerate();
            if (recordUtl != null)
            {
                if (Convert.ToBoolean(recordUtl.IsAutoGenerate))
                {
                    constrainGenerate = true;
                }
            }

            return constrainGenerate;
        }

        public JsonResult DeleteSupplier(SupplierDetails datamodel)
        {
            using (TransactionScope trans = new TransactionScope(TransactionScopeOption.RequiresNew,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted}))
            {
                try
                {
                    var supplier = ctx.GnMstSuppliers.Find(CompanyCode, datamodel.SupplierCode);
                    var profitCenters = ctx.GnMstSupplierProfitCenters.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.SupplierCode == datamodel.SupplierCode);
                    var Banks = ctx.GnMstSupplierBanks.Where(m => m.CompanyCode == CompanyCode && m.SupplierCode == datamodel.SupplierCode);
                    if (profitCenters.Count() > 0 || Banks.Count() > 0)
                    {
                        return Json(new { success = false, message = "Gagal hapus data, masih terdapat data details." });
                    }
                    else
                    {
                        ctx.GnMstSuppliers.Remove(supplier);
                        ctx.SaveChanges();
                        trans.Complete();
                        return Json(new { success = true, message = "Data Supplier berhasil dihapus" });
                    }
                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    return Json(new { success = false, message = "Error pada saat menghapus data Supplier", error_log = ex.Message });
                }
            }
        }

        //Profit Center
        public JsonResult SaveProfitCenter(SupplierDetails model, SupplierProfitCenterView supPCModel)
        {
            try
            {
                using (var transScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var oProfitCenter = ctx.GnMstSupplierProfitCenters.Find(CompanyCode, BranchCode, supPCModel.SupplierCode, supPCModel.ProfitCenterCode);
                    if (oProfitCenter == null)
                    {
                        oProfitCenter = new GnMstSupplierProfitCenter();
                        oProfitCenter.CompanyCode = CompanyCode;
                        oProfitCenter.BranchCode = BranchCode;
                        oProfitCenter.SupplierCode = supPCModel.SupplierCode;
                        oProfitCenter.ProfitCenterCode = supPCModel.ProfitCenterCode;
                        oProfitCenter.CreatedBy = CurrentUser.UserId;
                        oProfitCenter.CreatedDate = DateTime.Now;
                        ctx.GnMstSupplierProfitCenters.Add(oProfitCenter);
                    }
                    oProfitCenter.LastUpdateBy = CurrentUser.UserId;
                    oProfitCenter.LastUpdateDate = DateTime.Now;
                    oProfitCenter.ContactPerson = (supPCModel.ContactPerson == null) ? "" : supPCModel.ContactPerson;
                    oProfitCenter.SupplierClass = supPCModel.SupplierClass;
                    oProfitCenter.SupplierGrade = supPCModel.SupplierGrade;
                    oProfitCenter.DiscPct = (supPCModel.DiscPct == null) ? (decimal)0.00 : supPCModel.DiscPct;
                    oProfitCenter.TaxCode = supPCModel.TaxCode;
                    oProfitCenter.TOPCode = supPCModel.TOPCode;
                    oProfitCenter.Status = "1";
                    oProfitCenter.isBlackList = supPCModel.isBlackList ?? false;

                    var record = ctx.GnMstSuppliers.Find(CompanyCode, model.SupplierCode);
                    if (record != null)
                    {
                        record.LastUpdateBy = CurrentUser.UserId;
                        record.LastUpdateDate = DateTime.Now;
                        record.SupplierGovName = model.SupplierGovName;
                        record.ProvinceCode = model.ProvinceCode;
                        record.AreaCode = model.AreaCode;
                        record.CityCode = model.CityCode;
                        record.ZipNo = model.ZipNo;
                        record.isPKP = model.isPKP;
                        record.NPWPNo = model.NPWPNo;
                        record.NPWPDate = model.NPWPDate;
                    }

                    ctx.SaveChanges();
                    transScope.Complete();

                    return Json(new { success = true, message = "Data Profit Center Barhasil disimpan" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error pada saat Save Profit Center", error_log = ex.Message });
            }
        }

        public JsonResult DeleteProfitCenter(SupplierProfitCenterView datamodel)
        {
            try
            {

                var oProfitCenter = ctx.GnMstSupplierProfitCenters.Find(CompanyCode, BranchCode, datamodel.SupplierCode, datamodel.ProfitCenterCode);
                if (oProfitCenter != null)
                {
                    ctx.GnMstSupplierProfitCenters.Remove(oProfitCenter);
                    ctx.SaveChanges();
                    return Json(new { success = true, message = "Profit center berhasil dihapus" });
                }
                else
                {
                    return Json(new { success = true, message = "Data profit center tidak ditemukan" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = true, message = "Error pada saat hapus Profit Center", error_log = ex.Message });
            }
        }

        ///Bank
        public JsonResult SaveBank(GnMstSupplierBank modelBank)
        {
            try
            {
                var bank = ctx.GnMstSupplierBanks.Find(CompanyCode, modelBank.SupplierCode, modelBank.BankCode);
                if (bank == null)
                {
                    bank = new GnMstSupplierBank();
                    bank.CompanyCode = CompanyCode;
                    bank.SupplierCode = modelBank.SupplierCode;
                    bank.BankCode = modelBank.BankCode;
                    bank.CreatedBy = CurrentUser.UserId;
                    bank.CreatedDate = DateTime.Now;
                    ctx.GnMstSupplierBanks.Add(bank);
                }
                bank.BankName = modelBank.BankName;
                bank.AccountBank = modelBank.AccountBank;
                bank.AccountName = modelBank.AccountName;
                bank.LastUpdateBy = CurrentUser.UserId;
                bank.LastUpdateDate = DateTime.Now;

                Helpers.ReplaceNullable(bank);
                ctx.SaveChanges();
                
                return Json(new { success = true, message = "Supplier Bank Berhasil disimpan" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error pada saat menyimpan Supplier Bank", error_log = ex.Message });
            }
        }

        public JsonResult DeleteBank(GnMstSupplierBank modelBank)
        {
            try
            {
                var bank = ctx.GnMstSupplierBanks.Find(CompanyCode, modelBank.SupplierCode, modelBank.BankCode);
                if (bank != null)
                {
                    ctx.GnMstSupplierBanks.Remove(bank);
                    ctx.SaveChanges();
                    return Json(new { success = true, message = "Data Supplier Bank Berhasil dihapus" });
                }
                else
                {
                    return Json(new { success = false, message = "Data Supplier Bank Berhasil dihapus" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error pada saat delete Supplier Bank", error_log = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SetSupplierCode()
        {
            ResultModel result = InitializeResultModel();
            bool constrainGenerate = false;

            var dtHQ = (from p in ctx.GnMstOrganizationDtls
                        where p.CompanyCode == CompanyCode && p.IsBranch == false
                        select new
                        {
                            CompanyCode = p.CompanyCode,
                            BranchCode = p.BranchCode
                        }).ToList();
            if (dtHQ.Count > 1)
            {
                result.message = "Periksa setting-an organisasi, jumlah holding lebih dari satu";
                return Json(result);
            }

            var recordUtl = ctx.GnMstSupplierUtilities.Find(CompanyCode, dtHQ[0].BranchCode);
            if (recordUtl != null)
            {
                if (recordUtl.IsAutoGenerate.Value)
                {
                    constrainGenerate = true;
                }
                else
                {
                    constrainGenerate = false;
                }
            }

            result.data = constrainGenerate;
            return Json(result);
        }

        #region -- Private Method --
        private bool p_ConstrainGenerate(ref GnMstSupplierUtility recordUtl)
        {
            var constrainGenerate = false;
            recordUtl = p_ConstrainGenerate();
            if (recordUtl != null)
            {
                if (Convert.ToBoolean(recordUtl.IsAutoGenerate))
                {
                    constrainGenerate = true;
                }
            }

            return constrainGenerate;
        }

        private GnMstSupplierUtility p_ConstrainGenerate()
        {
            try
            {
                GnMstSupplierUtility recordUtl = new GnMstSupplierUtility();
                var datHQ = ctx.GnMstOrganizationDtls.Where(p => p.CompanyCode == CompanyCode
                    && p.IsBranch == false);

                if (datHQ.Count() > 1)
                {
                    throw new Exception("Periksa setting-an organisasi, jumlah holding lebih dari satu");
                }

                recordUtl = ctx.GnMstSupplierUtilities.Find(CompanyCode, datHQ.FirstOrDefault().BranchCode);

                return recordUtl;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
