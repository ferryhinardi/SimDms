using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class PackageController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {

                IntervalYear = "3",
                IntervalKM = "50,000",
                PackageSrvSeq = "0.00",
                DiscPct = "0.00"
                
            });
        }

      /*  public string getData()
        {
            var transdate = ctx.CoProfileServices.Find(CompanyCode, BranchCode).TransDate;
            return GetNewDocumentNo("KTK", transdate.Value);
        }
        */
        public JsonResult Save(svMstPackage model)
        {

            var record = ctx.svMstPackages.Find(CompanyCode, model.PackageCode);
            if (record == null)
            {
                record = new svMstPackage
                {
                    CompanyCode = CompanyCode,
                    PackageCode = model.PackageCode,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,

                };

                ctx.svMstPackages.Add(record);
            }
            record.PackageName = model.PackageName;
            record.PackageSrvSeq = model.PackageSrvSeq;
            record.PackageDesc = model.PackageDesc;
            record.BasicModel = model.BasicModel;
            record.JobType = model.JobType;
            record.BillTo = model.BillTo;
            record.IntervalYear = model.IntervalYear;
            record.IntervalKM = model.IntervalKM;
            record.LastUpdatedBy = CurrentUser.UserId;
            record.LastUpdatedDate = DateTime.Now;

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

        public JsonResult SaveTask(svMstPackage model, svMstPackageTask TasksModel)
        {

            var record = ctx.svMstPackageTasks.Find(CompanyCode, model.PackageCode, model.BasicModel, TasksModel.OperationNo);
            if (record == null)
            {
                record = new svMstPackageTask
                {
                    CompanyCode = CompanyCode,
                    PackageCode = model.PackageCode,
                    BasicModel = model.BasicModel,
                    OperationNo = TasksModel.OperationNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,

                };

                ctx.svMstPackageTasks.Add(record);
            }
            record.DiscPct = TasksModel.DiscPct == null ? 0 : TasksModel.DiscPct;
            record.LastUpdatedBy = CurrentUser.UserId;
            record.LastUpdatedDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult SavePart(svMstPackage model, svMstPackagePart PartModel)
        {

            var record = ctx.svMstPackageParts.Find(CompanyCode, model.PackageCode, model.BasicModel, PartModel.PartNo);
            if (record == null)
            {
                record = new svMstPackagePart
                {
                    CompanyCode = CompanyCode,
                    PackageCode = model.PackageCode,
                    BasicModel = model.BasicModel,
                    PartNo = PartModel.PartNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,

                };

                ctx.svMstPackageParts.Add(record);
            }
            record.DiscPct = PartModel.DiscPct == null ? 0 : PartModel.DiscPct;
            record.LastUpdatedBy = CurrentUser.UserId;
            record.LastUpdatedDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult deleteData(svMstPackage model)
        {

            var record = ctx.svMstPackages.Find(CompanyCode, model.PackageCode);
            var record2 = ctx.svMstPackageTasks.Where(a => a.CompanyCode == CompanyCode && a.BasicModel == model.BasicModel);
            if (record != null)
            {
                ctx.svMstPackages.Remove(record);
                ctx.Database.ExecuteSqlCommand("DELETE svMstPackageTask WHERE CompanyCode='" + CompanyCode + "' and PackageCode='" + model.PackageCode + "' and BasicModel='" + model.BasicModel + "'");
                ctx.Database.ExecuteSqlCommand("DELETE svMstPackagePart WHERE CompanyCode='" + CompanyCode + "' and PackageCode='" + model.PackageCode + "' and BasicModel='" + model.BasicModel + "'");
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

        public JsonResult deleteTask(svMstPackage model, svMstPackageTask TasksModel)
        {

            var record = ctx.svMstPackageTasks.Find(CompanyCode, model.PackageCode, model.BasicModel, TasksModel.OperationNo);
            var record2 = ctx.svMstPackageTasks.Where(a => a.CompanyCode == CompanyCode && a.BasicModel == model.BasicModel);
            if (record != null)
            {
                ctx.svMstPackageTasks.Remove(record);
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = record2 });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }


        }

        public JsonResult deletePart(svMstPackage model, svMstPackagePart PartModel)
        {

            var record = ctx.svMstPackageParts.Find(CompanyCode, model.PackageCode, model.BasicModel, PartModel.PartNo);
            var record2 = ctx.svMstPackageParts.Where(a => a.CompanyCode == CompanyCode && a.BasicModel == model.BasicModel);
            if (record != null)
            {
                ctx.svMstPackageParts.Remove(record);
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = record2 });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }


        }

        public JsonResult BasmodDetail(string BasicModel)
        {
            var record = ctx.SvCBasmodViews.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType && a.BasicModel == BasicModel) ;
            return Json(record);
        }

        public JsonResult JobDetail(string JobType)
        {
            var record = ctx.SvJobViews.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType && x.JobType == JobType);
            return Json(record);
        }
        

            public JsonResult PayDetail(string BillTo)
        {
            var record = ctx.SvPaymentPackages.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProfitCenterCode == ProfitCenter && a.CustomerCode == BillTo);
            return Json(record);
        }

        public JsonResult LoadTask(string PackageCode)
        {

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvGetPackageTask";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@PackageCode", PackageCode);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));

        }

        public JsonResult LoadPart(string PackageCode)
        {

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvGetPackagePart";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@PackageCode", PackageCode);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

    }
}
