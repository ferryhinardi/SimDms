using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models.Others;
using SimDms.Sparepart.Models;
using SimDms.Common.Models;
using SimDms.General.Models;
using SimDms.Common;
using System.Transactions;
using TracerX;

namespace SimDms.General.Controllers.Api
{
    public class ChartController : BaseController
    {

        [HttpPost]
        public JsonResult Company()
        {
            ResultModel result = InitializeResultModel();
            var me = ctx.OrganizationHdrs
                    .FirstOrDefault();

            var my = ctx.GnMstSegmentAccs
                    .Where(m => m.BranchCode == BranchCode && m.TipeSegAcc == "100" && m.SegAccNo == me.CompanyAccNo)
                    .FirstOrDefault();
            
            var mi = ctx.GnMstSegmentAccs
                    .Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.TipeSegAcc == "200")
                    .FirstOrDefault();

            SegmentAcc data = new SegmentAcc
                    {
                        SegAccNo = my.SegAccNo,
                        Description = my.Description,
                        SegAccNo1 = mi.SegAccNo,
                        Description1 = mi.Description
                    };
                    return Json(new { success = data != null, data = data });
        }

        [HttpPost]
        public JsonResult AccDesc(string tipe, string segacc) 
        {
            ResultModel result = InitializeResultModel();
            var me = ctx.GnMstSegmentAccs.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.TipeSegAcc == tipe && p.SegAccNo == segacc)
                .FirstOrDefault();
            SegmentAcc data = new SegmentAcc
                            {
                                Description = me.Description 
                            };
            return Json(new { success = data != null, data = data });
        }

        [HttpPost]
        public JsonResult CostCentreDesc(string lookupvalue)
        {
            ResultModel result = InitializeResultModel();
            var my = ctx.LookUpDtls
                     .Where(m => m.LookUpValue == lookupvalue && m.CodeID == "CSTR")
                     .FirstOrDefault();
            LookUpDtl data = new LookUpDtl
            {
                LookUpValueName = my.LookUpValueName,
            };
            return Json(new { success = data != null, data = data });
        }
        //[HttpPost]
        //public JsonResult Branch()
        //{
        //    ResultModel result = InitializeResultModel();
            

        //    GnMstSegmentAcc data = new GnMstSegmentAcc
        //    {
        //        SegAccNo = me.SegAccNo,
        //        Description = me.Description
        //    };
        //    return Json(new { success = data != null, data = data });
        //}

        [HttpPost]
        public JsonResult AccountDesc(string lookupvalue) 
        {
            ResultModel result = InitializeResultModel();
            var me = ctx.LookUpDtls
                    .Where(m => m.CompanyCode == CompanyCode && m.CodeID == "ACCT" && m.LookUpValue == lookupvalue)
                    .FirstOrDefault();

            LookUpDtl data = new LookUpDtl
            {
                LookUpValueName = me.LookUpValueName
            };
            return Json(new { success = data != null, data = data });
        }

        [HttpPost]
        public JsonResult Save(gnMstAccount model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.GnMstAccounts.Find(CompanyCode, BranchCode, model.AccountNo);

            if (me == null)
            {
                me = new gnMstAccount();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.GnMstAccounts.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.AccountNo = model.AccountNo;
            me.Description = model.Description;
            me.AccountType = model.AccountType;
            me.Company = model.Company;
            me.Branch = model.Branch;
            me.CostCtrCode = model.CostCtrCode;
            me.ProductType = model.ProductType;
            me.NaturalAccount = model.NaturalAccount;
            me.InterCompany = model.InterCompany;
            me.Futureuse = model.Futureuse;
            me.Consol = model.Consol;
            me.FromDate = model.FromDate;
            me.EndDate = model.EndDate;
            me.Balance = model.Balance;

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data Account berhasil disimpan.";
                result.data = new
                {
                    AccountNo = me.AccountNo,
                    AccountType = me.AccountType,
                    Description = me.Description
                };
            }
            catch (Exception Ex)
            {
                result.message = "Data Account tidak bisa disimpan.";
                MyLogger.Info("Error on Account saving: " + Ex.Message);
            }

            return Json(result);
        }

        public JsonResult Delete(gnMstAccount model)
        {
            object returnObj = null;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.GnMstAccounts.Find(CompanyCode, BranchCode, model.AccountNo);
                    if (me != null)
                    {
                        ctx.GnMstAccounts.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Account berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Account , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Account , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        public JsonResult CheckAccNo(string AccountNo)
        {
            var titleName = ctx.GnMstAccounts.Where(a => a.AccountNo == AccountNo).FirstOrDefault();
            if (titleName != null)
            {
                return Json(new
                {
                    success = true,
                    data = titleName
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = false,
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
