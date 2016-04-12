using SimDms.Common.Models;
using SimDms.PreSales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.PreSales.Controllers.Api
{
    public class PositionController : BaseController
    {
        public JsonResult ComboPosition()
        {
            var result = ctx.LookUpDtls.Where(x => x.CompanyCode == CompanyCode && x.CodeID == "SPOS")
                    .Select(x => new
                    {
                        value = x.LookUpValue,
                        text = x.LookUpValueName
                    });
            return Json(result);
        }

        public JsonResult Save(PositionItem data)
        {
            var message = "";

            try
            {
                var isNew = false;
                var employee = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode &&
                    x.EmployeeID == data.EmployeeID);
                if (employee == null)
                {
                    var employeeUser = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode &&
                        x.RelatedUser == data.UserID);
                    if (employeeUser != null) throw new Exception("User ID sudah terdaftar!");
                    
                    isNew = true;
                    employee = new HrEmployee
                    {
                        CompanyCode = CompanyCode,
                        EmployeeID = data.EmployeeID,
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = DateTime.Now
                    };
                }
                employee.RelatedUser = data.UserID;
                employee.Position = data.Position;
                employee.UpdatedBy = CurrentUser.UserId;
                employee.UpdatedDate = DateTime.Now;

                if (isNew) ctx.HrEmployees.Add(employee);
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