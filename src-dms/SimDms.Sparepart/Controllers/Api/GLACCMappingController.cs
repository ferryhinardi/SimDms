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

namespace SimDms.Sparepart.Controllers.Api
{
    public class GLAccMappingController : BaseController
    {


        public string GLAccScalar(string id)
        {
            var data = ctx.Database.SqlQuery<spgnMstAccountView>(defaultSpParam("uspfn_spgnMstAccountView", id)).FirstOrDefault();
            string s = "";
            if (data != null)
            {
                s = data.Description;
            }
            return s;
        }

        public JsonResult GLAccScalarAll(spMstAccount model)
        {
            string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "", s6 = "", s7 = "", s8 = "", s9 = "";

            if (model != null) {

            var data = ctx.Database.SqlQuery<spgnMstAccountView>(defaultSpParam("uspfn_spgnMstAccountView", model.SalesAccNo)).FirstOrDefault();
            if (data != null)
            {
                s1 = data.Description;
            }
            data = ctx.Database.SqlQuery<spgnMstAccountView>(defaultSpParam("uspfn_spgnMstAccountView", model.COGSAccNo)).FirstOrDefault();
            if (data != null)
            {
                s2 = data.Description;
            }
            data = ctx.Database.SqlQuery<spgnMstAccountView>(defaultSpParam("uspfn_spgnMstAccountView", model.InventoryAccNo)).FirstOrDefault();
            if (data != null)
            {
                s3 = data.Description;
            }
            data = ctx.Database.SqlQuery<spgnMstAccountView>(defaultSpParam("uspfn_spgnMstAccountView", model.DiscAccNo)).FirstOrDefault();
            if (data != null)
            {
                s4 = data.Description;
            }
            data = ctx.Database.SqlQuery<spgnMstAccountView>(defaultSpParam("uspfn_spgnMstAccountView", model.ReturnAccNo)).FirstOrDefault();
            if (data != null)
            {
                s5 = data.Description;
            }
            data = ctx.Database.SqlQuery<spgnMstAccountView>(defaultSpParam("uspfn_spgnMstAccountView", model.ReturnPybAccNo)).FirstOrDefault();
            if (data != null)
            {
                s6 = data.Description;
            }
            data = ctx.Database.SqlQuery<spgnMstAccountView>(defaultSpParam("uspfn_spgnMstAccountView", model.OtherIncomeAccNo)).FirstOrDefault();
            if (data != null)
            {
                s7 = data.Description;
            }
            data = ctx.Database.SqlQuery<spgnMstAccountView>(defaultSpParam("uspfn_spgnMstAccountView", model.OtherReceivableAccNo)).FirstOrDefault();
            if (data != null)
            {
                s8 = data.Description;
            }
            data = ctx.Database.SqlQuery<spgnMstAccountView>(defaultSpParam("uspfn_spgnMstAccountView", model.InTransitAccNo)).FirstOrDefault();
            if (data != null)
            {
                s9 = data.Description;
            }
            }
 


            return Json(new { SalesAccNoName = s1, COGSAccNoName = s2, InventoryAccNoName = s3, DiscAccNoName = s4, ReturnAccNoName = s5, ReturnPybAccNoName = s6, OtherIncomeAccNoName=s7, OtherReceivableAccNoName=s8, InTransitAccNoName =s9});
        }


        public JsonResult Save(spMstAccount model)
        {
            string msg = "";
            var record = ctx.spMstAccounts.Find(CompanyCode, BranchCode, model.TypeOfGoods);

            if (record == null)
            {
                record = new spMstAccount
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    TypeOfGoods = model.TypeOfGoods,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spMstAccounts.Add(record);
                msg = "New item price added...";
            }
            else
            {
                ctx.spMstAccounts.Attach(record);
                msg = "Item price updated";
            }

            record.SalesAccNo = model.SalesAccNo;
            record.COGSAccNo = model.COGSAccNo;
            record.InventoryAccNo = model.InventoryAccNo;
            record.DiscAccNo = model.DiscAccNo;
            record.ReturnAccNo = model.ReturnAccNo;
            record.ReturnPybAccNo = model.ReturnPybAccNo;
            record.OtherIncomeAccNo = model.OtherIncomeAccNo;
            record.OtherReceivableAccNo = model.OtherReceivableAccNo;
            record.InTransitAccNo = model.InTransitAccNo;


            record.LockingBy = CurrentUser.UserId;
            record.LockingDate = DateTime.Now;
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




        public JsonResult Delete(spMstAccount model)
        {
            var record = ctx.spMstAccounts.Find(CompanyCode, BranchCode, model.TypeOfGoods);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.spMstAccounts.Remove(record);
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

        public JsonResult getRecord(string TypeOfGoods)
        {
            var rec = ctx.spMstAccounts.Find(CompanyCode, BranchCode, TypeOfGoods); 
            
            if (rec != null)
            {
                return Json(new
                {
                    success = true,
                    data = rec
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false });
        }

    }
}
