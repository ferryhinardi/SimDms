using SimDms.Absence.Models;
using GeLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Api
{
    public class KGridController : BaseController
    {
        public JsonResult Employees()
        {

            IQueryable<HrEmployeeView> qry = ctx.HrEmployeeViews.Where(x => x.CompanyCode != "" && x.EmployeeID != "");
            qry = qry.Where(p => p.CompanyCode == CompanyCode);

            string dept = Request["Department"];
            string status = Request["Status"];
            string fltNik = Request["fltNik"];
            string fltSlsID = Request["fltSlsID"];
            string fltEmplName = Request["fltEmplName"];
            string fltDepartment = Request["fltDepartment"];
            string fltPosition = Request["fltPosition"];
            string fltPersonnelStatus = Request["fltPersonnelStatus"];

            if (!string.IsNullOrWhiteSpace(dept)) qry = qry.Where(x => x.Department == dept);
            if (!string.IsNullOrWhiteSpace(status)) qry = qry.Where(x => x.Status == status);
            if (!string.IsNullOrWhiteSpace(fltNik)) qry = qry.Where(x => x.EmployeeID.Contains(fltNik));
            if (!string.IsNullOrWhiteSpace(fltSlsID)) qry = qry.Where(x => x.SalesID.Contains(fltSlsID));
            if (!string.IsNullOrWhiteSpace(fltEmplName)) qry = qry.Where(x => x.EmployeeName.Contains(fltEmplName));
            if (!string.IsNullOrWhiteSpace(fltDepartment)) qry = qry.Where(x => x.DepartmentName.Contains(fltDepartment));
            if (!string.IsNullOrWhiteSpace(fltPosition)) qry = qry.Where(x => x.PositionName.Contains(fltPosition));
            if (!string.IsNullOrWhiteSpace(fltPersonnelStatus)) qry = qry.Where(x => x.PersonnelStatus==fltPersonnelStatus);

            return Json(qry.KGrid());
        }
    }
}
