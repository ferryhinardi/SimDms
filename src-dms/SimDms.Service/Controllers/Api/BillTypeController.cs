using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;
using SimDms.Common.Models;

namespace SimDms.Service.Controllers.Api
{
    public class BillTypeController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {

                CompanyCode = CompanyCode,
                //CompanyName = CompanyName,
                //BranchCode = BranchCode,
                //BranchName = BranchName,
                //ServiceType = 2,
                //JobOrderDate = DateTime.Now,
                //StartService = DateTime.Now,
                //FinishService = DateTime.Now
            });
        }

        public JsonResult Save(svMstBillType model)
        {
            var record = ctx.svMstBillTypes.Find(CompanyCode, model.BillType);
            if (record == null)
            {
                record = new  svMstBillType
                {
                    CompanyCode = CompanyCode,
                    BillType = model.BillType.ToUpper(),
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                };
                ctx.svMstBillTypes.Add(record);
            }

            record.CustomerCode = (model.CustomerCode == null) ? "" : model.CustomerCode;
            record.Description = model.Description.ToUpper();
            record.IsActive = model.IsActive;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            record.IsLocked = false;
            record.LockingBy = "";
            record.LockingDate = new DateTime(1900,1,1);

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

        public JsonResult deleteData(svMstBillType model)
        {

            var record = ctx.svMstBillTypes.Find(CompanyCode, model.BillType);
            if (record != null)
            {
                ctx.svMstBillTypes.Remove(record);
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

        public JsonResult Get(svMstBillType model)
        {
            var record = ctx.svMstBillTypes.Find(CompanyCode, model.BillType);

            if (record != null)
            {
                if (record.CustomerCode != "")
                {
                    var recordCust = ctx.Customers.Find(CompanyCode, record.CustomerCode);
                    if (recordCust != null)
                    {
                        return Json(new { success = true, data = record, dataCust = recordCust });
                    }
                    else return Json(new { success = true, data = record });

                }
                else
                    return Json(new { success = true, data = record });
            }
            else
                return Json(new { success = false });
        }

        public JsonResult GetCust(svMstBillType model)
        {
            var record = ctx.Customers.Find(CompanyCode, model.CustomerCode);

            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }
       
    }
}
