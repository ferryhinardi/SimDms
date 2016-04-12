using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Service.Controllers.Api
{
    public class PekerjaanController : BaseController
    {
        //
        // GET: /Pekerjaan/

        public JsonResult Default()
        {
            return Json(new
            {

                CompanyCode = CompanyCode,
                ProductType = ProductType,
                PdiFscSeq = 0,
                WarrantyOdometer = 0,
                WarrantyTimePeriod = 0,
                OperationHour=0,
                ClaimHour = 0,
                LaborCost = 0,
                LaborPrice = 0,
                Quantity = 0,
                RetailPrice = 0,
                IsActive = true, 
                WarrantyTimeDim = "D"
            });
        }

        public JsonResult AccountNo(string ReceivableAccountNo)
        {
            var queryable = ctx.SvNomorAccViews.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.AccountNo == ReceivableAccountNo);
            if (queryable != null)
                return Json(new { success = true, data = queryable });
            else
                return Json(new { success = false });
        }

        public JsonResult GroupJobType(string GroupJobType)
        {
            var queryable = ctx.SvGroupJobViews.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType && x.GroupJobType == GroupJobType);
            if (queryable != null)
                return Json(new { success = true, data = queryable });
            else
                return Json(new { success = false });
        }

        public JsonResult JobType(string JobType)
        {
            var queryable = ctx.svMstRefferenceServices.Find(CompanyCode, ProductType, "JOBSTYPE", JobType);
            if (queryable != null)
                return Json(new { success = true, data = queryable });
            else
                return Json(new { success = false });
        }

        public JsonResult SavePertama(svMstJob model)
        {
            var record = ctx.svMstJobs.Find(CompanyCode, ProductType, model.BasicModel, model.JobType);

            if (record == null)
            {
                record = new svMstJob
                {
                    CompanyCode = CompanyCode,
                    ProductType = ProductType,
                    BasicModel = model.BasicModel,
                    JobType = model.JobType,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,

                };
                ctx.svMstJobs.Add(record);
            }

            record.GroupJobType = model.GroupJobType;
            record.IsPdiFsc = model.IsPdiFsc;
            record.PdiFscSeq = model.PdiFscSeq;
            record.WarrantyOdometer = model.WarrantyOdometer;
            record.WarrantyTimePeriod = model.WarrantyTimePeriod;
            record.WarrantyTimeDim = model.WarrantyTimeDim;
            record.CounterOperationNo = model.CounterOperationNo;
            record.ReceivableAccountNo = model.ReceivableAccountNo;
            record.IsActive = model.IsActive;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            record.IsLocked = false;
            record.LockingBy = CurrentUser.UserId;
            record.LockingDate = DateTime.Now;

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

        public JsonResult SaveKedua(svMstTask model)
        {

            var recordJob = ctx.svMstJobs.Find(CompanyCode, ProductType,model.BasicModel,model.JobType);
            var IsFscType = recordJob != null && recordJob.GroupJobType.Equals("FSC");

            var record = ctx.svMstTasks.Find(CompanyCode, ProductType, model.BasicModel, model.JobType, model.OperationNo);

            if (record == null)
            {
                record = new svMstTask
                {
                    CompanyCode = CompanyCode,
                    ProductType = ProductType,
                    BasicModel = model.BasicModel,
                    JobType = model.JobType,
                    OperationNo = model.OperationNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                };
                ctx.svMstTasks.Add(record);
            }

            record.Description = model.Description;
            record.OperationHour = model.OperationHour;
            record.ClaimHour = model.ClaimHour;
            record.LaborCost = IsFscType ? 0 : model.LaborCost;
            record.LaborPrice = model.LaborPrice;
            record.TechnicalModelCode = model.TechnicalModelCode;
            record.IsSubCon = IsFscType ? false : model.IsSubCon;
            record.IsCampaign = model.IsCampaign;
            record.IsActive = model.IsActive;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            record.IsLocked = false;
            record.LockingBy = CurrentUser.UserId;
            record.LockingDate = DateTime.Now;
            record.BillType = model.BillType;


            //if (record.IsSubCon.Value && record.LaborCost <= 0)
            //{
            //    return Json(new { success = false, message = "Biaya Pekerjaan Luar harus lebih besar dari nol" });
            //}

            //if (record != null && record.IsActive)
            //{
            //    //DataTable dtSvTrnSrvTask = SvTrnSrvTaskBLL.SelectOutstdSPKByOperationNo(recordTask.OperationNo);
            //    string[] srvStat = {"0","1","2","3","4"};

            //    var svtrnsrvtask = from e in ctx.ServiceTasks
            //                       join d in ctx.Services
            //                       on new { e.CompanyCode, e.BranchCode, e.ProductType, e.ServiceNo }
            //                       equals new { d.CompanyCode, d.BranchCode, d.ProductType, d.ServiceNo } into _d
            //                       from d in _d.DefaultIfEmpty()
            //                       where srvStat.Contains(d.ServiceStatus) && e.OperationNo == model.OperationNo
            //                       && e.CompanyCode == CompanyCode && e.BranchCode == BranchCode
            //                       && e.ProductType == ProductType && d.ServiceType == "2"
            //                       select new
            //                       {
            //                           d,
            //                           e.OperationNo
            //                       };

            //    if (svtrnsrvtask.Count() > 0)
            //    {
            //        return Json(new { success = false, message = "Data tidak dapat diupdate karena sudah dipakai di proses SPK" });
            //    }
            //}

            //if (record == null)
            //{
            //    string[] JobType = { "OTHER", "CLAIM" };
                
            //    var data = ctx.svMstTasks.Where(e=>e.CompanyCode == CompanyCode && e.ProductType == ProductType && e.BasicModel == model.BasicModel 
            //        && JobType.Contains(e.JobType) && e.OperationNo == model.OperationNo);
            //    if (data.Count() > 0)
            //    {
            //        return Json(new { success = false, message = string.Format("Data tidak dapat insert karena sudah dipakai di JobType : {0}", data.First().JobType) });
            //    }
            //}

            //if (record.IsSubCon.Value)
            //{
            //    decimal newcost = record.OperationHour.Value * record.LaborPrice.Value;
            //    object[] parameters = { CompanyCode, BranchCode, ProductType, record.BasicModel, record.JobType, record.OperationNo, newcost };
            //    var query = "exec uspfn_SvTrnServiceOutstandingSubCon @p0,@p1,@p2,@p3,@p4,@p5,@p6";

            //    var OutstandingSubCon = ctx.Database.SqlQuery<OutstandingSubCon>(query, parameters);

            //    if (OutstandingSubCon.Count() > 0)
            //    {

            //        foreach (var subCon in OutstandingSubCon)
            //        {
            //            if (subCon.IsSelected.Value)
            //            {
            //                var oTrnTask = ctx.ServiceTasks.Find(CompanyCode, BranchCode, ProductType, subCon.ServiceNo, record.OperationNo);
            //                if (oTrnTask != null)
            //                {
            //                    oTrnTask.OperationHour = record.OperationHour;
            //                    oTrnTask.OperationCost = record.LaborPrice;
            //                    oTrnTask.BillType = record.BillType;
            //                    oTrnTask.LastupdateBy = CurrentUser.UserId;
            //                    oTrnTask.LastupdateDate = DateTime.Now;
            //                    var result = ctx.SaveChanges() > 1;
            //                    if (result) ctx.Database.ExecuteSqlCommand("exec uspfn_SvTrnServiceReCalculate @p0,@p1,@p2,@p3", CompanyCode, BranchCode, ProductType, subCon.ServiceNo);
            //                }
            //            }
            //        }
            //    }
            //}

            try
            {
                ctx.SaveChanges();

                object[] parameters = { CompanyCode, BranchCode, ProductType, record.JobType, record.BasicModel, record.OperationNo };
                var query = "exec uspfn_SvMstSaveTaskPrice @p0,@p1,@p2,@p3,@p4,@p5";
                ctx.Database.ExecuteSqlCommand(query, parameters);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult SaveKetiga(svMstTaskPart model)
        {
           var record = ctx.svMstTaskParts.Find(CompanyCode, ProductType, model.BasicModel, model.JobType, model.OperationNo, model.PartNo);

            if (record == null)
            {
                record = new svMstTaskPart
                {
                    CompanyCode = CompanyCode,
                    ProductType = ProductType,
                    BasicModel = model.BasicModel,
                    JobType = model.JobType,
                    OperationNo = model.OperationNo,
                    PartNo = model.PartNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,

                };
                ctx.svMstTaskParts.Add(record);
            }

            record.Quantity = model.Quantity;
            record.RetailPrice = model.RetailPrice;
            record.BillType = model.BillType;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            
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

        public JsonResult deletePertama(svMstJob model)
        {

            var record = ctx.svMstJobs.Find(CompanyCode, ProductType, model.BasicModel, model.JobType);
            if (record != null)
            {
                ctx.svMstJobs.Remove(record);
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

        public JsonResult deleteKedua(svMstTask model)
        {

            var record = ctx.svMstTasks.Find(CompanyCode, ProductType, model.BasicModel, model.JobType, model.OperationNo);
            if (record != null)
            {
                ctx.svMstTasks.Remove(record);
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

        public JsonResult deleteKetiga(svMstTaskPart model)
        {

            var record = ctx.svMstTaskParts.Find(CompanyCode, ProductType, model.BasicModel, model.JobType, model.OperationNo, model.PartNo);
            if (record != null)
            {
                ctx.svMstTaskParts.Remove(record);
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

        public JsonResult getData(SvRincianJob model)
        {
                var record = ctx.svMstTasks.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.ProductType == ProductType && x.BasicModel == model.BasicModel
                    && x.JobType == model.JobType);
                if (record == null)
                {
                    return Json(new { success = false, data = record });
                }
                else {
                    return Json(new { success = true, data = record });
                }
                
        }

        public JsonResult GetTaskPrice(string BasicModel, string JobType, string OperationNo)
        {
            var sql = string.Format(@"
                        select * from svMstTaskPrice
                         where 1 = 1
                           and CompanyCode = '{0}'
                           and BranchCode  = '{1}'
                           and ProductType = '{2}'
                           and BasicModel  = '{3}'
                           and JobType     = '{4}'
                           and OperationNo = '{5}'
                        ", CompanyCode, BranchCode, ProductType, BasicModel, JobType, OperationNo);
            var data = ctx.Database.SqlQuery<svMstTaskPrice>(sql).AsQueryable();

            return Json(data);

        }

        public JsonResult getDataTable(SvRincianPart model)
        {

            var record = ctx.SvRincianParts.Where(x => x.CompanyCode == CompanyCode
                && x.ProductType == ProductType && x.BranchCode == BranchCode && x.BasicModel == model.BasicModel
                && x.JobType == model.JobType && x.OperationNo == model.OperationNo).Select(x => new {
                    PartNo = x.PartNo,
                    Quantity = x.Quantity,
                    RetailPrice = x.RetailPrice,
                    PartName = x.PartName,
                    BillTypeDesc = x.BillTypeDesc,
                    BillType = x.BillTypePart
                });
            return Json(record);


        }

        public JsonResult ProsesBasicModel(string BasicModelSumber, string BasicModelTarget)
        {

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CopyBasicModel";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
           // cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@BasicModelSource", BasicModelSumber);
            cmd.Parameters.AddWithValue("@BasicModelTarget", BasicModelTarget);
            cmd.Parameters.AddWithValue("@UserId", CurrentUser.UserId);

            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                cmd.Connection.Dispose();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult get(svMstJob model)
        {
            //var record = ctx.svMstJobs.FirstOrDefault(x => x.CompanyCode == CompanyCode
            //       && x.ProductType == ProductType && x.BasicModel == model.BasicModel
            //       && x.JobType == model.JobType);
            var record = (from p in ctx.svMstJobs
                         join p1 in ctx.SvJobViews
                         on p.JobType equals p1.JobType
                         join p2 in ctx.SvGroupJobViews
                         on p.GroupJobType equals p2.GroupJobType
                          join p3 in ctx.SvNomorAccViews
                                 on p.ReceivableAccountNo equals p3.AccountNo into _p1
                          from p3 in _p1.DefaultIfEmpty()
                         where p.BasicModel == model.BasicModel &&  p.JobType == model.JobType 
                         select new svMstJobView
                         {
                             BasicModel = p.BasicModel,
                             JobType = p.JobType,
                             GroupJobType = p.GroupJobType,
                             IsPdiFsc = p.IsPdiFsc,
                             PdiFscSeq = p.PdiFscSeq,
                             WarrantyOdometer = p.WarrantyOdometer,
                             WarrantyTimePeriod = p.WarrantyTimePeriod,
                             WarrantyTimeDim = p.WarrantyTimeDim,
                             CounterOperationNo = p.CounterOperationNo,
                             ReceivableAccountNo = p.ReceivableAccountNo,
                             IsActive = p.IsActive,
                             JobDescription = p1.JobDescription,
                             GroupJobDescription = p2.GroupJobDescription,
                             AccDescription = p3.AccDescription
                         }).FirstOrDefault();
            if (record == null)
            {
                return Json(new { success = false, data = record });
            }
            else
            {
                return Json(new { success = true, data = record });
            }
           
        }

        public JsonResult laborprice(string LaborCode) 
        {
            var record = ctx.SvMstTarifJasas.FirstOrDefault(x => x.CompanyCode == CompanyCode
                && x.BranchCode == BranchCode && x.ProductType == ProductType
                && x.LaborCode == LaborCode);
            if (record == null)
            {
                return Json(new { success = false, data = record });
            }
            else
            {
                return Json(new { success = true, data = record });
            }

        }

        public JsonResult tehnicalmodel(string BasicModel) 
        {
            var record = ctx.SvBasicModelPekerjaans.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType && x.BasicModel == BasicModel).FirstOrDefault().TechnicalModelCode;
            if (record == null)
            {
                return Json(new { success = false, data = record });
            }
            else
            {
                return Json(new { success = true, data = record });
            }

        }

        public JsonResult GetBasicModel(svMstJob model)
        {
            var record = ctx.svMstRefferenceServices.Where(p => p.CompanyCode == CompanyCode && p.ProductType == ProductType && p.RefferenceType == "BASMODEL" && p.RefferenceCode == model.BasicModel).FirstOrDefault();

            if (record != null)
                return Json (new { success = true, data = record});
            else
                return Json(new { success = false });
        }

        public JsonResult LookupBasicModelPrint()
        {
            var models = ctx.Models.Where(p => p.CompanyCode == CompanyCode).Distinct();
            var queryable = from p in models
                        select new
                        {
                            p.BasicModel,
                            p.TechnicalModelCode,
                            Status = p.Status == "1" ? "Aktif" : "Tidak Aktif",
                            ModelDescription = ctx.Models.Where(q => q.CompanyCode == p.CompanyCode && q.BasicModel == p.BasicModel && q.TechnicalModelCode == p.TechnicalModelCode).OrderByDescending(q => q.LastUpdateDate).FirstOrDefault()
                        };

            return Json(queryable.Distinct().AsQueryable().toKG());
        }

        public JsonResult LookupReffSrvPrint(string reffType)
        {
            var queryable = ctx.svMstRefferenceServices.Where(p => p.CompanyCode == CompanyCode && p.ProductType == ProductType && p.RefferenceType == reffType)
                .Select(p => new 
                {
                    p.RefferenceType, 
                    p.RefferenceCode,
                    p.Description,
                    p.DescriptionEng,
                    IsActive = p.IsActive == true ? "Aktif" : "Tidak Aktif" 
                });

            return Json(queryable.AsQueryable().toKG());
        }

        public JsonResult LookupOperationNoPrint()
        {
            var queryable = ctx.svMstTasks.ToList();

            return Json(queryable.AsQueryable().toKG());
        }

        public JsonResult LookupPartNoPrint()
        {
            var queryable = from p in ctx.ItemInfos
                            join p1 in ctx.ItemPrices on new { p.CompanyCode, p.PartNo } equals new { p1.CompanyCode, p1.PartNo }
                            where p.CompanyCode == CurrentUser.CompanyCode
                            && p1.BranchCode == CurrentUser.BranchCode
                            select new
                            {
                                p.PartNo,
                                p.PartName,
                                p1.RetailPriceInclTax,
                                Status = p.Status == "1" ? "Aktif" : "Tidak Aktif"
                            };

            return Json(queryable.AsQueryable().toKG());
        }

        public JsonResult GetOperationNo(string operationNo)
        {
            var record = ctx.svMstTasks.Where(p => p.CompanyCode == CompanyCode && p.OperationNo == operationNo).FirstOrDefault();

            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }
    }
}