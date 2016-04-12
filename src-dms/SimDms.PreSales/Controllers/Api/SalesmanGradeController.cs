using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common.Models;
using SimDms.PreSales.Models;

namespace SimDms.PreSales.Controllers.Api
{
    public class SalesmanGradeController : BaseController
    {
        public JsonResult ComboGrade()
        {
            var data = ctx.LookUpDtls.Where(x => x.CompanyCode == CompanyCode && x.CodeID == "ITSG")
                .Select(x => new 
                {
                    value = x.LookUpValue,
                    text = x.LookUpValueName
                });
                
            return Json(data);
        }

        public JsonResult Save(PositionItem data)
        {
            var message = "";
            
            try
            {
                var employee = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode &&
                    x.EmployeeID == data.EmployeeID);
                if (employee == null) throw new Exception("Error. Karyawan tidak terdaftar");
                employee.Grade = data.Grade;
                employee.UpdatedBy = CurrentUser.UserId;
                employee.UpdatedDate = DateTime.Now;
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message });
        }
    }
}