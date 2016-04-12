using SimDms.CStatisfication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class TDayCallController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
            });
        }

        public JsonResult Save(TDayCallModel model)
        {
            var record = ctx.TDayCalls.Find(model.CompanyCode, model.CustomerCode, model.Chassis);
            if (record == null)
            {
                record = new TDayCall();
                record.CompanyCode = model.CompanyCode;
                record.CustomerCode = model.CustomerCode;
                record.Chassis = model.Chassis;
                record.CreatedBy = CurrentUser.UserId;
                record.CreatedDate = DateTime.Now;
                ctx.TDayCalls.Add(record);
            }
            record.IsDeliveredA = model.IsDeliveredA;
            record.IsDeliveredB = model.IsDeliveredB;
            record.IsDeliveredC = model.IsDeliveredC;
            record.IsDeliveredD = model.IsDeliveredD;
            record.IsDeliveredE = model.IsDeliveredE;
            record.IsDeliveredF = model.IsDeliveredF;
            record.IsDeliveredG = model.IsDeliveredG;
            record.Comment = model.Comment;
            record.Additional = model.Additional;
            record.Status = model.Status;
            //record.Status = ((model.IsDeliveredA.Value
            //    && model.IsDeliveredB.Value
            //    && model.IsDeliveredC.Value
            //    && model.IsDeliveredD.Value
            //    && model.IsDeliveredE.Value
            //    && model.IsDeliveredF.Value
            //    && model.IsDeliveredG.Value) ? 2 : 0);
            record.UpdatedBy = CurrentUser.UserId;
            record.UpdatedDate = DateTime.Now;

            if (record.Status == 1)
            {
                record.FinishDate = DateTime.Now;
                record.Reason = null;
            }
            else
            {
                record.Reason = model.Reason;
                record.FinishDate = null;
            }

            var cust = ctx.GnMstCustomers.Find(model.CompanyCode, model.CustomerCode);
            if (cust != null)
            {
                cust.BirthDate = model.BirthDate;
                cust.Spare02 = model.ReligionCode;
                cust.Spare03 = model.AddPhone1;
                cust.Spare04 = model.AddPhone2;
            }

            var custdata = ctx.CsCustDatas.Find(model.CompanyCode, model.CustomerCode);
            if (custdata == null)
            {
                custdata = new CsCustData
                {
                    CompanyCode = model.CompanyCode,
                    CustomerCode = model.CustomerCode,
                    IsDeleted = false,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };
                ctx.CsCustDatas.Add(custdata);
            }
            
            custdata.AddPhone1 = model.AddPhone1;
            custdata.AddPhone2 = model.AddPhone2;
            custdata.ReligionCode = model.ReligionCode;
            custdata.UpdatedBy = CurrentUser.UserId;
            custdata.UpdatedDate = DateTime.Now;

            var custView = ctx.CustomerViews.Where(cv => cv.CompanyCode ==  model.CompanyCode && cv.CustomerCode == model.CustomerCode).FirstOrDefault();
            if (custView != null)
            {
                if (custView.CustomerCode == model.CustomerCode)
                {
                    custView.BirthDate = model.BirthDate;
                    custView.AddPhone1 = model.AddPhone1;
                    custView.AddPhone2 = model.AddPhone2;
                    custView.ReligionCode = model.ReligionCode;
                }
            }
            try
            {
                ctx.SaveChanges();
                //var result3 = ctx.Database.ExecuteSqlCommand("exec uspfn_CRUD3DaysCallResource  @CompanyCode=@p0, @CustomerCode=@p1, @Chassis=@p2", CompanyCode, model.CustomerCode, model.Chassis);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Delete(TDayCall model)
        {
            var record = ctx.TDayCalls.Find(model.CompanyCode, model.CustomerCode, model.Chassis);
            if (record != null)
            {
                ctx.TDayCalls.Remove(record);

                try
                {
                    ctx.SaveChanges();
                    var result3 = ctx.Database.ExecuteSqlCommand("exec uspfn_CRUD3DaysCallResource  @CompanyCode=@p0, @CustomerCode=@p1, @Chassis=@p2", CompanyCode, model.CustomerCode, model.Chassis);

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return Json(new { success = false, message = "data not valid" });
        }

        public JsonResult Get(TDayCall model)
        {
            var record = ctx.CsLkuTDayCallViews.Where(p =>
                p.CompanyCode == model.CompanyCode &&
                p.CustomerCode == model.CustomerCode &&
                p.Chassis == model.Chassis).FirstOrDefault();
            if (record != null)
            {
                return Json(new { success = true, data = record, isNew = false });
            }
            else
            {
                var data = ctx.CustomerBuyViews.Find(model.CompanyCode, model.CustomerCode, model.Chassis);
                return Json(new { success = true, data = data, isNew = true }); ;
            }
        }
    }
}
