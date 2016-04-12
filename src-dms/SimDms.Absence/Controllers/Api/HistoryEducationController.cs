using SimDms.Absence.Models;
using SimDms.Absence.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Api
{
    public class HistoryEducationController : BaseController
    {
        [HttpPost]
        public JsonResult List()
        {
            string companyCode = CompanyCode;
            string employeeID = Request["EmployeeID"] ?? "";

            var data = ctx.HrEmployeeEducations.Where(x => x.CompanyCode==CompanyCode && x.EmployeeID==employeeID);

            return Json(GeLang.DataTables<HrEmployeeEducation>.Parse(data, Request));
        }

        [HttpPost]
        public JsonResult Save(HrEmployeeEducationModel model)
        {
            ResultModel result = InitializeResult();
            string companyCode = CompanyCode;
            string userId = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;
            Regex numbersOnly = new Regex(@"^[0-9]{4}");

            //if (string.IsNullOrEmpty(model.HistoryEducationYearBegin)==false && numbersOnly.Match(model.HistoryEducationYearBegin).Success == false)
            //{
            //    result.message = "Year begin only received numbers.";
            //    return Json(result);
            //}

            //if (string.IsNullOrEmpty(model.HistoryEducationYearFinish)==false && numbersOnly.Match(model.HistoryEducationYearBegin).Success == false)
            //{
            //    result.message = "Year finish only received numbers.";
            //    return Json(result);
            //}

            var historyEducation = ctx.HrEmployeeEducations.Where(x => 
                    x.CompanyCode == companyCode
                    &&
                    x.EmployeeID==model.EmployeeID
                    &&
                    x.College==model.HistoryEducationCollege
                ).FirstOrDefault();

            if (historyEducation == null)
            {
                historyEducation = new HrEmployeeEducation();
                historyEducation.CompanyCode = companyCode;
                historyEducation.EmployeeID = model.EmployeeID;
                historyEducation.College = model.HistoryEducationCollege;
                historyEducation.CreatedBy = userId;
                historyEducation.CreatedDate = currentTime;

                ctx.HrEmployeeEducations.Add(historyEducation);
            }

            historyEducation.YearBegin = model.HistoryEducationYearBegin;
            historyEducation.YearFinish = model.HistoryEducationYearFinish;
            historyEducation.CreatedBy = userId;
            historyEducation.CreatedDate = currentTime;

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "History education data has been saved into database.";
            }
            catch (Exception)
            {
                result.message = "History education data cannot be saved into database.";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(HrEmployeeEducation model)
        {
            ResultModel result = InitializeResult();
            string companyCode = CompanyCode;

            var historyEducation = ctx.HrEmployeeEducations.Where(x =>
                    x.CompanyCode == companyCode
                    &&
                    x.EmployeeID == model.EmployeeID
                    &&
                    x.College == model.College
                );

            foreach (var item in historyEducation)
            {
                ctx.HrEmployeeEducations.Remove(item);
            }

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "History education data has been deleted from database.";
            }
            catch (Exception)
            {
                result.message = "History education data cannot be deleted from database.";
            }

            return Json(result);
        }
    }
}
