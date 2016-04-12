using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class SvAccountController : BaseController
    {
        public JsonResult Default()
        {
            var sales = Sales();
            var discount = Discount();
            var retur = Retur();
            var hpp = HPP();
            var pyb = PYB();
            return Json(new
            {

                SalesAccNo = sales[0],
                COGSAccNo = hpp[0],
                DiscAccNo = discount[0],
                ReturnAccNo = retur[0],
                ReturnPybAccNo = pyb[0],
                DescriptionSales = SalesNo(sales[0]),
                DescriptionDiscount = DiscNo(discount[0]),
                DescriptionReturn = ReturnNo(retur[0]),
                DescriptionCOGS = COGSNo(hpp[0]),
                DescriptionPyb = PybNo(pyb[0]),
                TypeOfGoods = "L"
            });
        }

        public JsonResult SalesAccNo(NomorAccount model)
        {
            var queryable = ctx.SvNomorAccViews.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.AccountNo == model.SalesAccNo).FirstOrDefault();
            return Json(new { success = queryable != null ? true : false, data = queryable });
        }

        public JsonResult DiscAccNo(NomorAccount model)
        {
            var queryable = ctx.SvNomorAccViews.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.AccountNo == model.DiscAccNo).FirstOrDefault();
            return Json(new { success = queryable != null ? true : false, data = queryable });
        }

        public JsonResult ReturnAccNo(NomorAccount model)
        {
            var queryable = ctx.SvNomorAccViews.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.AccountNo == model.ReturnAccNo).FirstOrDefault();
            return Json(new { success = queryable != null ? true : false, data = queryable });
        }

        public JsonResult COGSAccNo(NomorAccount model)
        {
            var queryable = ctx.SvNomorAccViews.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.AccountNo == model.COGSAccNo).FirstOrDefault();
            return Json(new { success = queryable != null ? true : false, data = queryable });
        }

        public JsonResult ReturnPybAccNo(NomorAccount model)
        {
            var queryable = ctx.SvNomorAccViews.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.AccountNo == model.ReturnPybAccNo).FirstOrDefault();
            return Json(new { success = queryable != null ? true : false, data = queryable });
        }

        public JsonResult Save(svMstAccount model)
        {

            var record = ctx.svMstAccounts.Find(CompanyCode, BranchCode, model.TypeOfGoods);
            if (record == null)
            {
                record = new svMstAccount
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    TypeOfGoods = model.TypeOfGoods,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,

                };
                ctx.svMstAccounts.Add(record);
            }
            record.SalesAccNo = model.SalesAccNo;
            record.COGSAccNo = model.COGSAccNo;
            record.InventoryAccNo = "";
            record.DiscAccNo = model.DiscAccNo;
            record.ReturnAccNo = model.ReturnAccNo;
            record.ReturnPybAccNo = model.ReturnPybAccNo;
            record.OtherIncomeAccNo = "";
            record.OtherReceivableAccNo = "";
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            
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

        
        public List<string> Sales()
        {
            var record = ctx.svMstAccounts.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode).Select(a => a.SalesAccNo).ToList();
            return record;
        }
        public List<string> Discount()
        {
            var record = ctx.svMstAccounts.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode).Select(a => a.DiscAccNo).ToList();
            return record;
        }
        public List<string> Retur()
        {
            var record = ctx.svMstAccounts.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode).Select(a => a.ReturnAccNo).ToList();
            return record;
        }
        public List<string> HPP()
        {
            var record = ctx.svMstAccounts.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode).Select(a => a.COGSAccNo).ToList();
            return record;
        }
        public List<string> PYB()
        {
            var record = ctx.svMstAccounts.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode).Select(a => a.ReturnPybAccNo).ToList();
            return record;
        }

        public List<string> SalesNo(string SalesAccNo)
        {
            var record = ctx.SvNomorAccViews.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.AccountNo == SalesAccNo).Select(a => a.AccDescription).ToList();
            return record;
        }
        public List<string> DiscNo(string DiscAccNo)
        {
            var record = ctx.SvNomorAccViews.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.AccountNo == DiscAccNo).Select(a=>a.AccDescription).ToList();
            return record;
        }
        public List<string> ReturnNo(string ReturnAccNo)
        {
            var record = ctx.SvNomorAccViews.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.AccountNo == ReturnAccNo).Select(a => a.AccDescription).ToList();
            return record;
        }
        public List<string> COGSNo(string COGSAccNo)
        {
            var record = ctx.SvNomorAccViews.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.AccountNo == COGSAccNo).Select(a => a.AccDescription).ToList();
            return record;
        }
        public List<string> PybNo(string ReturnPybAccNo)
        {
            var record = ctx.SvNomorAccViews.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.AccountNo == ReturnPybAccNo).Select(a => a.AccDescription).ToList();
            return record;
        }

        
    }
}
