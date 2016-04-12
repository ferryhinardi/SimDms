using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;

namespace SimDms.Service.Controllers.Api
{
    public class MechanicController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                ProductType = ProductType,
                ServiceType = 2,
                JobOrderDate = DateTime.Now,
                StartService = DateTime.Now,
                FinishService = DateTime.Now
            });
        }

        public JsonResult GetTaskDetail(TrnService model)
        {
            try
            {
                var claim = ctx.Services.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                    && p.ProductType == ProductType && p.JobOrderNo == model.JobOrderNo).FirstOrDefault();
                
                long serviceNo = 0;
                if (claim != null)
                {
                    serviceNo = claim.ServiceNo;
                }
                var details = ctx.Database.SqlQuery<MechanicTask>(
                    "exec uspfn_SvTrnServiceSelectDtl @p0, @p1, @p2, @p3",
                    CompanyCode, BranchCode, ProductType, serviceNo);
                details = details.Where(x => !(x.IsSubCon ?? true) && x.TypeOfGoods == "L");

                return Json(new { Message = "", List = details });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        public JsonResult GetTaskMechanic(MechanicTask model)
        {
            try
            {
                var mechanics = ctx.Database.SqlQuery<MechanicDetail>(
                    "exec uspfn_SvTrnGetTaskMechanic @p0, @p1, @p2, @p3",
                    CompanyCode, BranchCode, model.ServiceNo, model.TaskPartNo);
                return Json(new { Message = "", List = mechanics });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult InsertMechanic(Mechanic model)
        {
            try
            {
                var mechanic = ctx.Mechanics.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.ServiceNo == model.ServiceNo && x.OperationNo == model.OperationNo
                    && x.MechanicID == model.MechanicID);
                var isNew = false;
                if (mechanic == null)
                {
                    isNew = true;
                    mechanic = new Mechanic
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        ProductType = ProductType,
                        ServiceNo = model.ServiceNo,
                        OperationNo = model.OperationNo,
                        MechanicID = model.MechanicID,
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now
                    };
                }

                mechanic.ChiefMechanicID = model.MechanicID;
                mechanic.StartService = model.StartService;
                mechanic.FinishService = model.FinishService;
                mechanic.MechanicStatus = model.MechanicStatus;
                mechanic.LastupdateBy = User.Identity.Name;
                mechanic.LastupdateDate = DateTime.Now;

                if (isNew) ctx.Mechanics.Add(mechanic);
                ctx.SaveChanges();

                var task = ctx.ServiceTasks.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.ServiceNo == mechanic.ServiceNo && x.OperationNo == mechanic.OperationNo);

                var mechanics = ctx.Mechanics.Where(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.ServiceNo == mechanic.ServiceNo && x.OperationNo == mechanic.OperationNo);

                if (mechanics.Count() > 0)
                {
                    task.TaskStatus = mechanics.Min(x => x.MechanicStatus);
                    task.SharingTask = mechanics.Count();
                    task.StartService = mechanics.Min(x => x.StartService.Value);
                    task.FinishService = mechanics.Max(x => x.FinishService.Value);
                }
                else
                {
                    task.StartService = new DateTime(1900, 1, 1);
                    task.FinishService = new DateTime(1900, 1, 1);
                }

                task.LastupdateBy = User.Identity.Name;
                task.LastupdateDate = DateTime.Now;

                var trnService = ctx.Services.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.ServiceNo == mechanic.ServiceNo);
                if (trnService.ServiceStatus == "0") trnService.ServiceStatus = "1";
                ctx.SaveChanges();

                return Json(new
                {
                    Message = "",
                    List = new
                    {
                        ServiceNo = model.ServiceNo,
                        TaskPartNo = model.OperationNo
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult RemoveTaskMechanic(Mechanic model)
        {
            try
            {
                var mechanic = ctx.Mechanics.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.ServiceNo == model.ServiceNo && x.OperationNo == model.OperationNo
                    && x.MechanicID == model.MechanicID);
                if (mechanic == null) throw new Exception("Mekanik tidak ditemukan. Coba kembali");
                ctx.Mechanics.Remove(mechanic);
                ctx.SaveChanges();

                if (GetStatusSPK(model.ServiceNo))
                {
                    var service = ctx.Services.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.ServiceNo == model.ServiceNo);
                    if (service.ServiceStatus == "1") service.ServiceStatus = "0";
                    ctx.SaveChanges();
                }

                var task = ctx.ServiceTasks.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.ServiceNo == model.ServiceNo && x.OperationNo == model.OperationNo);

                var mechanics = ctx.Mechanics.Where(x => x.CompanyCode == CompanyCode
                    && x.BranchCode == BranchCode && x.ProductType == ProductType
                    && x.ServiceNo == mechanic.ServiceNo && x.OperationNo == mechanic.OperationNo);

                if (mechanics.Count() > 0)
                {
                    task.TaskStatus = mechanics.Min(x => x.MechanicStatus);
                    task.SharingTask = mechanics.Count();
                    task.StartService = mechanics.Min(x => x.StartService.Value);
                    task.FinishService = mechanics.Max(x => x.FinishService.Value);
                }
                else
                {
                    task.StartService = new DateTime(1900, 1, 1);
                    task.FinishService = new DateTime(1900, 1, 1);
                }

                task.LastupdateBy = User.Identity.Name;
                task.LastupdateDate = DateTime.Now;
                ctx.SaveChanges();
                
                return Json(new
                {
                    Message = "",
                    List = new
                    {
                        ServiceNo = model.ServiceNo,
                        TaskPartNo = model.OperationNo
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        private bool GetStatusSPK(long serviceNo)
        {
            var status = false;
            var tasks = ctx.ServiceTasks.Where(x => x.CompanyCode == CompanyCode
                && x.BranchCode == BranchCode && x.ServiceNo == serviceNo);
            var items = ctx.ServiceItems.Where(x => x.CompanyCode == CompanyCode
                && x.BranchCode == BranchCode && x.ServiceNo == serviceNo);
            if (tasks.Count() == 1 && items.Count() == 0) status = true;
            return status;
        }
    }
}
