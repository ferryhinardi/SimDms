using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
using SimDms.Sparepart.Models;
using SimDms.General.Models.Others;
using SimDms.Common;
using System.Web.Script.Serialization;
using TracerX;
using System.Transactions;


namespace SimDms.General.Controllers.Api
{
    public class EmployeeController : BaseController
    {
        public JsonResult EmployeeTrainingLoad(string EmployeeID, string branchCode)   
        {
            var record = ctx.Database.SqlQuery<EmployeeTrainingView>("EmployeeTraining '" + CompanyCode + "' , '" + branchCode + "' ,'" + EmployeeID + "'").AsQueryable();
            return Json(ctx.Database.SqlQuery<EmployeeTrainingView>("EmployeeTraining '" + CompanyCode + "' , '" + branchCode + "' ,'" + EmployeeID + "'").AsQueryable());
        }

        [HttpPost]
        public JsonResult Save(gnMSTEmployee model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var employee = ctx.employees.Find(companyCode, branchCode, model.EmployeeID); 

            if (employee == null)
            {
                employee = new gnMSTEmployee();
                employee.CreatedDate = currentTime;
                employee.LastupdateDate = currentTime;
                employee.CreatedBy = userID;
                ctx.employees.Add(employee);
            }
                else{
                    employee.LastupdateDate = currentTime;
                    employee.LastupdateBy = userID;
            }
            employee.CompanyCode = companyCode;
            employee.BranchCode = branchCode;
            employee.EmployeeID = model.EmployeeID;
            employee.EmployeeName = model.EmployeeName;
            employee.Address1 = model.Address1;
            employee.Address2 = model.Address2;
            employee.Address3 = model.Address3;
            employee.Address4 = model.Address4;
            employee.PhoneNo = model.PhoneNo;
            employee.HpNo = model.HpNo;
            employee.FaxNo = model.FaxNo;
            employee.ProvinceCode = model.ProvinceCode;
            employee.AreaCode = model.AreaCode;
            employee.CityCode = model.CityCode;
            employee.ZipNo = model.ZipNo;
            employee.TitleCode = model.TitleCode;
            employee.JoinDate = model.JoinDate;
            employee.ResignDate = model.ResignDate;
            employee.GenderCode = model.GenderCode;
            employee.BirthPlace = model.BirthPlace;
            employee.BirthDate = model.BirthDate;
            employee.MaritalStatusCode = model.MaritalStatusCode;
            employee.ReligionCode = model.ReligionCode;
            employee.BloodCode = model.BloodCode;
            employee.IdentityNo = model.IdentityNo;
            employee.Height = model.Height;
            employee.Weight = model.Weight;
            employee.UniformSize = model.UniformSize;
            employee.ShoesSize = model.ShoesSize;
            employee.FormalEducation = model.FormalEducation;
            employee.PersonnelStatus = model.PersonnelStatus;
            employee.Nik = model.Nik;
            //employee.EmpPhotoID = model.EmpPhotoID;
            employee.EmpIdentityCardID = model.EmpIdentityCardID;
            employee.EmpImageID = model.EmpImageID;
            employee.EmpIdentityCardImageID = model.EmpIdentityCardImageID;
            employee.IsLocked = false;
               
                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Data Employee berhasil disimpan.";
                    result.data = new
                    {
                        EmployeeID = employee.EmployeeID,
                        EmployeeName = employee.EmployeeName
                    };
                }
                catch (Exception Ex)
                {
                    result.message = "Data Employee tidak bisa disimpan.";
                    MyLogger.Info("Error on Employee saving: " + Ex.Message);
                }
            
            return Json(result);
        }

        public JsonResult Delete(gnMSTEmployee model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var employee = ctx.employees.Find(companyCode, branchCode, model.EmployeeID);
                    if (employee != null)
                    {
                        ctx.employees.Remove(employee);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Employee berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Employee , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Employee , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        public JsonResult Delete2(GnMstEmployeeTraining model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var employee = ctx.employeeTrainings.Find(companyCode, model.BranchCode, model.EmployeeID, model.IsSuzukiTraining, model.TrainingCode);
                    if (employee != null)
                    {
                        ctx.employeeTrainings.Remove(employee);
                        ctx.SaveChanges();
                        var records = ctx.Database.SqlQuery<EmployeeTrainingView>("EmployeeTraining '" + CompanyCode + "' , '" + model.BranchCode + "' ,'" + model.EmployeeID + "'").AsQueryable();

                        returnObj = new { success = true, message = "Data Employee training berhasil di delete.", result = records };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Employee , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete training Employee , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        public JsonResult CheckLookUpDtl(string EmployeeID, string BranchCode, string CodeID, string LookUpValue )
        {
            var record = ctx.employees.Find(CompanyCode, BranchCode, EmployeeID);
            var titleName = ctx.LookUpDtls.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.CodeID == CodeID && a.LookUpValue == LookUpValue).LookUpValueName;
            if (record != null)
            {
                return Json(new
                {
                    success = true,
                    data = record,
                    TitleName = titleName 
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult SaveTraining(GnMstEmployeeTraining model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.employeeTrainings.Find(companyCode, model.BranchCode, model.EmployeeID, model.IsSuzukiTraining, model.TrainingCode);

            if (me == null)
            {
                me = new GnMstEmployeeTraining();
                me.CreatedDate = currentTime;
                me.LastupdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.employeeTrainings.Add(me);
            }
            else
            {
                me.LastupdateDate = currentTime;
                me.LastupdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = model.BranchCode;
            me.EmployeeID = model.EmployeeID;
            me.IsSuzukiTraining = model.IsSuzukiTraining;
            me.TrainingCode = model.TrainingCode;
            me.BeginTrainingDate = model.BeginTrainingDate;
            me.EndTrainingDate = model.EndTrainingDate;
            me.IsActive = model.IsActive;   
            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data Employee Training berhasil disimpan.";
                var records = ctx.Database.SqlQuery<EmployeeTrainingView>("EmployeeTraining '" + CompanyCode + "' , '" + model.BranchCode + "' ,'" + model.EmployeeID + "'").AsQueryable();
                result.data = records;
            }
            catch (Exception Ex)
            {
                result.message = "Data Employee Training tidak bisa disimpan.";
                MyLogger.Info("Error on Employee Training saving: " + Ex.Message);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Save2(EmployeeMutationView model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;
            ctx.Database.ExecuteSqlCommand("uspfn_GnEmployeeMutation'" + CompanyCode + "', '" + model.BranchCode + "', '" + model.EmployeeID + "', '" + model.MutationTo + "', '" + model.PersonnelStatus + "', '" + userID + "'"); 
            return Json(result);
        }
    }
}
