using SimDms.Absence.Models;
using SimDms.Absence.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;

namespace SimDms.Absence.Controllers.Api
{
    public class AchievementController : BaseController
    {
        [HttpPost, Authorize]
        public JsonResult IsHasHistories()
        {
            ResultModel result = InitializeResult();
            string employeeID = Request["employeeID"];

            int historiesCount = ctx.HrEmployeeAchievements.Where(x => x.CompanyCode.Equals(CompanyCode) == true && x.EmployeeID.Equals(employeeID) == true && x.IsDeleted != true).Count();

            if (historiesCount > 0)
            {
                result.status = true;
            }

            return Json(result);
        }

        [HttpPost, Authorize]
        public JsonResult List()
        {
            string employeeID = Request["EmployeeID"];

            IQueryable<HrEmployeeAchievementView> data = ctx.HrEmployeeAchievementViews.Where(x => x.CompanyCode.Equals(CompanyCode) == true ).OrderByDescending(x => x.AssignDate);
            data = data.Where(x => x.EmployeeID.Equals(employeeID ?? "") == true);

            //return Json(GeLang.DataTables<HrEmployeeAchievementView>.Parse(data, Request));
            return Json(data.DGrid());
        }

        [HttpPost, Authorize]
        public JsonResult Save(DateTime? AssignDate)
        {
            ResultModel result = InitializeResult();
            string employeeID = Request["EmployeeID"] ?? "";
            string department = Request["DepartmentAchievement"] ?? "";
            string position = Request["PositionAchievement"] ?? "";
            string grade = Request["GradeAchievement"] ?? "";
            bool isJoinDate  = Convert.ToBoolean(Request["IsJoinDate"]);
            var currentTime = DateTime.Now;

            if (AssignDate < GetJoinDate(CompanyCode, employeeID))
            {
                result.message = "Assigned date cannot less than join date.";
                return Json(result);
            }

            if (AssignDate > GetResignDate(CompanyCode, employeeID) && GetResignDate(CompanyCode, employeeID) != null)
            {
                result.message = "Assigned date cannot more than join date.";
                return Json(result);
            }

            if (string.IsNullOrEmpty(employeeID) == true)
            {
                result.message = "Employee ID cannot be null.";
                return Json(result);
            }

            var data = ctx.HrEmployeeAchievements.Where(x=> 
                x.CompanyCode.Equals(CompanyCode) == true
                &&
                x.EmployeeID.Equals(employeeID) == true
                &&
                x.AssignDate == AssignDate
            ).FirstOrDefault();

            if (data == null)
            {
                data = new HrEmployeeAchievement();
                data.CompanyCode = CompanyCode;
                data.EmployeeID = employeeID;
                data.AssignDate = AssignDate;
                data.CreatedBy = CurrentUser.UserId;
                data.CreatedDate = currentTime;

                ctx.HrEmployeeAchievements.Add(data);
            }

            data.IsDeleted = false;
            data.UpdatedBy = CurrentUser.UserId;
            data.UpdatedDate = currentTime;
            data.IsJoinDate = isJoinDate;
            data.Department = department;
            data.Position = position;
            data.Grade = grade;

            try
            {
                UpdateEmployeeAchievementOnEmployeeRec(employeeID, department, position, grade);
                ctx.SaveChanges();
                result.status = true;
                result.message = "Achievement data has been saved.";
            }
            catch (Exception)
            {
                result.message = "Achievement data cannot be saved.";      
            }

            return Json(result);
        }

        private void UpdateEmployeeAchievementOnEmployeeRec(string employeeID, string department, string position, string grade)
        {
            var employee = ctx.HrEmployees.Where(x =>
                    x.CompanyCode.Equals(CompanyCode) == true
                    &&
                    x.EmployeeID.Equals(employeeID) == true
                ).FirstOrDefault();

            if (employee != null)
            {
                employee.Department = department;
                employee.Position = position;
                employee.Grade = grade;
            }

            ctx.SaveChanges();
        }

        [HttpPost, Authorize]
        public JsonResult Delete(HrEmployeeAchievement model)
        {
            ResultModel result = InitializeResult();

            var data = ctx.HrEmployeeAchievements.Where(x =>
                    x.CompanyCode.Equals(CompanyCode) == true
                    &&
                    x.EmployeeID.Equals(model.EmployeeID) == true
                    &&
                    x.Department.Equals(model.Department) == true
                    &&
                    x.Position.Equals(model.Position) == true
                    &&
                    x.AssignDate == model.AssignDate
                ).FirstOrDefault();

            
            if (data != null)
            {
                //ctx.HrEmployeeAchievements.Remove(data);
                data.IsDeleted = true;
            }

            try
            {
                ctx.SaveChanges();

                var updatedPosition = ctx.HrEmployeeAchievements.Where(x => x.EmployeeID.Equals(model.EmployeeID)).OrderByDescending(x => x.AssignDate).FirstOrDefault();
                var employee = ctx.HrEmployees.Where(x => x.CompanyCode.Equals(CompanyCode) == true && x.EmployeeID.Equals(model.EmployeeID) == true).FirstOrDefault();

                if (updatedPosition != null && employee != null)
                {
                    employee.Department = updatedPosition.Department;
                    employee.Position = updatedPosition.Position;
                    employee.Grade = updatedPosition.Grade;
                }
                else
                {
                    employee.Department = "";
                    employee.Position = "";
                    employee.Grade = "";
                }

                ctx.SaveChanges();

                result.status = true;
                result.message = "Achievement data has been deleted.";
            }
            catch (Exception)
            {
                result.message = "Achievement data cannot be deleted.";
            }

            return Json(result);
        }

        public JsonResult UpdatedAchievement()
        {
            ResultModel result = InitializeResult();
            string employeeID = Request["EmployeeID"] ?? "";

            var data = ctx.HrEmployeeViews.Where(x => x.EmployeeID.Equals(employeeID) == true).FirstOrDefault();
            if (data != null)
            {
                result.status = true;
                result.data = data;
            }

            return Json(result);
        }

        private DateTime? GetJoinDate(string companyCode, string employeeID)
        {
            //var employee = ctx.HrEmployees.Find(companyCode, employeeID);
            var employee = ctx.HrEmployees.Where(x => x.CompanyCode==CompanyCode && x.EmployeeID == employeeID).FirstOrDefault();

            if (employee != null)
            {
                return employee.JoinDate;
            }

            return null;
        }

        private DateTime? GetResignDate(string companyCode, string employeeID)
        {
            var employee = ctx.HrEmployees.Find(companyCode, employeeID);

            if (employee != null)
            {
                return employee.ResignDate;
            }

            return null;
        }
    }
}
