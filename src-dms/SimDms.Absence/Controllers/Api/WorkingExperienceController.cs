using SimDms.Absence.Models;
using SimDms.Absence.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Api
{
    public class WorkingExperienceController : BaseController
    {
        [HttpPost]
        public JsonResult Save(HrEmployeeExperienceModel model)
        {
            ResultModel result = InitializeResult();

            if (string.IsNullOrEmpty(model.WorkingExperienceNameOfCompany) == true && (model.WorkingExperienceJoinDate == null || model.WorkingExperienceJoinDate.Year == 1))
            {
                result.message = "You have to provide 'Name' and 'Join Date' of previous company!";
            }
            else if (model.WorkingExperienceResignDate != null && model.WorkingExperienceResignDate < model.WorkingExperienceJoinDate)
            {
                result.message = "Resign date must be more than Join Date.";
            }
            else
            {
                string companyCode = CompanyCode;
                string userId = CurrentUser.UserId;
                DateTime currentTime = DateTime.Now;

                var workingExperience = ctx.HrEmployeeExperiences.Where(x =>
                        x.CompanyCode == companyCode
                        &&
                        x.EmployeeID == model.EmployeeID
                        &&
                        x.JoinDate.Value.Year == model.WorkingExperienceJoinDate.Year
                        &&
                        x.JoinDate.Value.Month == model.WorkingExperienceJoinDate.Month
                        &&
                        x.JoinDate.Value.Day == model.WorkingExperienceJoinDate.Day
                    ).FirstOrDefault();

                if (workingExperience == null)
                {
                    workingExperience = new HrEmployeeExperience();
                    workingExperience.CompanyCode = companyCode;
                    workingExperience.EmployeeID = model.EmployeeID;
                    workingExperience.JoinDate = model.WorkingExperienceJoinDate;
                    workingExperience.CreatedBy = userId;
                    workingExperience.CreatedDate = currentTime;

                    ctx.HrEmployeeExperiences.Add(workingExperience);
                }

                workingExperience.NameOfCompany = model.WorkingExperienceNameOfCompany;
                workingExperience.ResignDate = model.WorkingExperienceResignDate;
                workingExperience.ReasonOfResign = model.WorkingExperienceReasonOfResign;
                workingExperience.LeaderName = model.WorkingExperienceLeaderName;
                workingExperience.LeaderPhone = model.WorkingExperienceLeaderPhone;
                workingExperience.LeaderHP = model.WorkingExperienceLeaderHP;
                workingExperience.UpdateBy = userId;
                workingExperience.UpdateDate = currentTime;

                try
                {
                    ctx.SaveChanges();

                    result.status = true;
                    result.message = "Working experience data has been saved into database.";
                }
                catch (Exception)
                {
                    result.message = "Working experience data cannot be saved into database.";
                }
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(HrEmployeeExperience model)
        {
            ResultModel result = InitializeResult();

            var workingExperience = ctx.HrEmployeeExperiences.Where(x =>
                    x.CompanyCode == model.CompanyCode
                    &&
                    x.EmployeeID == model.EmployeeID
                    &&
                    x.JoinDate.Value.Year == model.JoinDate.Value.Year
                    &&
                    x.JoinDate.Value.Month == model.JoinDate.Value.Month
                    &&
                    x.JoinDate.Value.Day == model.JoinDate.Value.Day
                );

            foreach (var item in workingExperience)
            {
                ctx.HrEmployeeExperiences.Remove(item);
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Working experience data has been deleted.";
            }
            catch (Exception)
            {
                result.message = "Sorry, working experience data cannot be deleted.\nPlease, try again later.";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Find(HrEmployeeExperience model)
        {
            ResultModel result = InitializeResult();

            var workingExperience = ctx.HrEmployeeExperiences.Where(x =>
                    x.CompanyCode == model.CompanyCode
                    &&
                    x.EmployeeID == model.EmployeeID
                    &&
                    x.NameOfCompany == model.NameOfCompany
                    &&
                    x.JoinDate == model.JoinDate
                ).FirstOrDefault();

            if (workingExperience != null)
            {
                result.status = true;
                result.data = workingExperience;
            }

            return Json(result);
        }


        [HttpPost]
        public JsonResult List()
        {
            string employeeID = Request["EmployeeID"] ?? "";
            string companyCode = CompanyCode;
            var data = ctx.HrEmployeeExperiences.Where(x => x.CompanyCode == x.CompanyCode && x.EmployeeID == employeeID);

            return Json(GeLang.DataTables<HrEmployeeExperience>.Parse(data, Request));
        }
    }
}
