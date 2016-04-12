using SimDms.Absence.Models;
using SimDms.Absence.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Api
{
    public class TrainingController : BaseController
    {
        [HttpPost]
        public JsonResult List()
        {
            ResultModel result = this.InitializeResult();

            string employeeID = Request["EmployeeID"];

            IQueryable<HrEmployeeTrainingView> data = ctx.HrEmployeeTrainingViews
                                .Where(x => (
                                    x.CompanyCode.Equals(CompanyCode) == true
                                    &&
                                    x.EmployeeID.Equals(employeeID) == true
                                ));
            result.status = true;

            return Json(GeLang.DataTables<HrEmployeeTrainingView>.Parse(data, Request));
        }

        [HttpPost]
        public JsonResult Save(HrEmployeeTraining model)
        {
            ResultModel result = this.InitializeResult();

            if (model.TrainingCode == null)
            {
                result.message = "Training Code must be filled ...";
                return Json(result);
            }

            HrEmployeeTraining data = ctx.HrEmployeeTrainings
                                        .Where(x => (
                                            x.CompanyCode.Equals(CompanyCode) == true
                                            &&
                                            x.EmployeeID.Equals(model.EmployeeID) == true
                                            &&
                                            x.TrainingDate == model.TrainingDate
                                        )).FirstOrDefault();

            if (isMoreThanJoinDate(model.EmployeeID, model.TrainingDate) == false)
            {
                result.message = "Sorry, Training date must be more than join date.";
                return Json(result);
            }

            if (data == null)
            {
                data = new HrEmployeeTraining();
                data.CompanyCode = CompanyCode;
                data.EmployeeID = model.EmployeeID;
                data.TrainingDate = model.TrainingDate;
                data.CreatedBy = CurrentUser.UserId;
                data.CreatedDate = DateTime.Now;

                ctx.HrEmployeeTrainings.Add(data);
            }

            data.IsDeleted = false;
            data.TrainingCode = model.TrainingCode;
            data.TrainingDuration = model.TrainingDuration;
            data.PostTest = model.PostTest;
            data.PostTestAlt = model.PostTestAlt;
            data.PreTest = model.PreTest;
            data.PreTestAlt = model.PreTestAlt;
            data.UpdatedBy = CurrentUser.UserId;
            data.UpdatedDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data has been saved.";
            }
            catch (Exception)
            {
                result.message = "Data cannot be saved.";
            }
            
            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(HrEmployeeTraining model)
        {
            ResultModel result = this.InitializeResult();

            HrEmployeeTraining data = ctx.HrEmployeeTrainings
                                        .Where(x => (
                                            x.CompanyCode.Equals(CompanyCode) == true
                                            &&
                                            x.EmployeeID.Equals(model.EmployeeID) == true
                                            &&
                                            x.TrainingDate == model.TrainingDate
                                        )).FirstOrDefault();

            if (data != null)
            {
                //ctx.HrEmployeeTrainings.Remove(data);
                data.IsDeleted = true;
            }

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data has been deleted.";
            }
            catch (Exception)
            {
                result.message = "Cannot delete data.";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult DepartmentList()
        {
            ResultModel result = this.InitializeResult();
            string employeeID = Request["EmployeeID"];

            List<ComboModel> data = ctx.Database.SqlQuery<ComboModel>("exec uspfn_HrTrainingDepartmentList @CompanyCode=@p0, @EmployeeID=@p1", CompanyCode, employeeID).ToList();            
                       
            return Json(data);
        }

        [HttpPost]
        public JsonResult PositionList()
        {
            ResultModel result = this.InitializeResult();
            string employeeID = Request["employeeID"];
            string department = Request["id"];

            List<ComboModel> data = ctx.Database.SqlQuery<ComboModel>("exec uspfn_HrTrainingPositionList @CompanyCode=@p0, @EmployeeID=@p1, @Department=@p2", CompanyCode, employeeID, department).ToList();

            return Json(data);
        }

        public JsonResult TrainingList()
        {
            string employeeID = Request["EmployeeID"];
            string strAssignDate = Request["TrainingDate"];
            DateTime? assignDate = DateTime.ParseExact(strAssignDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            HrEmployeePositionDetails employeePositionDetails = ctx.Database.SqlQuery<HrEmployeePositionDetails>("exec uspfn_HrGetDetailsEmployeePosition @CompanyCode=@p0, @EmployeeID=@p1, @ValidDate=@p2", CompanyCode, employeeID, assignDate.Value.ToString("yyyy-MM-dd")).FirstOrDefault();

            if (isMoreThanJoinDate(employeeID, assignDate) == false)
            {
                return Json(new ResultModel()
                {
                    status = false,
                    message = "Sorry, training date must be more than join date."
                });
            }

            if (isLessThanResignDate(employeeID, assignDate) == false)
            {
                return Json(new ResultModel()
                {
                    status = false,
                    message = "Sorry, training date must be less than resign date."
                });
            }

            List<ComboModel> result = new List<ComboModel>();

            if (employeePositionDetails != null)
            {
                result = ctx.Database.SqlQuery<ComboModel>("exec uspfn_HrGetValidEmployeeTraining @CompanyCode=@p0, @Department=@p1, @Position=@p2, @Grade=@p3", CompanyCode, employeePositionDetails.DepartmentCode, employeePositionDetails.PositionCode, employeePositionDetails.GradeCode).ToList();
            }

            return Json(result);
        }

        private bool isMoreThanJoinDate(string employeeID, DateTime? comparedDate)
        {
            bool result = false;

            var data = (from x in ctx.HrEmployees
                        where
                        x.CompanyCode.Equals(CompanyCode) == true
                        &&
                        x.EmployeeID.Equals(employeeID) == true
                        select x.JoinDate).FirstOrDefault();

            if (data != null && comparedDate > data)
            {
                result = true;
            }

            return result;
        }

        private bool isLessThanResignDate(string employeeID, DateTime? comparedDate)
        {
            bool result = false;

            var data = (from x in ctx.HrEmployees
                        where
                        x.CompanyCode.Equals(CompanyCode) == true
                        &&
                        x.EmployeeID.Equals(employeeID) == true
                        select x.ResignDate).FirstOrDefault();

            if ((data != null && comparedDate < data) || data == null)
            {
                result = true;
            }

            return result;
        }
    }
}
