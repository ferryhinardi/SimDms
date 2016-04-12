using SimDms.Absence.Models;
using SimDms.Absence.Models.Results;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Api
{
    public class VehicleController : BaseController
    {
        public JsonResult Save()
        {
            ResultModel result = this.InitializeResult();

            int vehSeq = Convert.ToInt32(Request["VehSeq"]);
            string brand = Request["Brand"];
            string type = Request["Type"];
            string model = Request["Model"];
            string policeRegNo = Request["PoliceRegNo"];
            string employeeID = Request["EmployeeID"];


            if (string.IsNullOrEmpty(employeeID) == true)
            {
                result.message = "Sorry, your request cannot be processed.";

                return Json(result);
            }

            if (string.IsNullOrEmpty(Request["VehSeq"]) == true)
            {
                vehSeq = ctx.HrEmployeeVehicles.Where(x => (
                    x.CompanyCode.Equals(CompanyCode) == true
                    &&
                    x.EmployeeID.Equals(employeeID) == true
                )).OrderByDescending(x => x.VehSeq).Select(x => x.VehSeq).FirstOrDefault() + 1;
            }

            HrEmployeeVehicle data = ctx.HrEmployeeVehicles.Where(x =>
                x.CompanyCode.Equals(CompanyCode)
                &&
                x.EmployeeID.Equals(employeeID) == true
                &&
                x.VehSeq == vehSeq
            ).FirstOrDefault();

            if (data == null)
            {
                data = new HrEmployeeVehicle();
                data.CreatedBy = this.CurrentUser.UserId;
                data.CreatedDate = DateTime.Now;
                data.CompanyCode = CompanyCode;
                data.EmployeeID = employeeID;
                data.VehSeq = vehSeq;

                ctx.HrEmployeeVehicles.Add(data);
            }

            data.Model = model;
            data.Type = type;
            data.Brand = brand;
            data.PoliceRegNo = policeRegNo;
            data.UpdatedBy = CurrentUser.UserId;
            data.UpdatedDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Vehicle data has been saved.";
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }
            catch (Exception)
            {
                result.message = "Sorry, Vehicle data cannot be saved.";
            }

            return Json(result);
        }

        public JsonResult Delete()
        {
            ResultModel result = this.InitializeResult();

            int vehSeq = Convert.ToInt32(Request["VehSeq"]);
            string brand = Request["Brand"];
            string type = Request["Type"];
            string model = Request["Model"];
            string policeRegNo = Request["PoliceRegNo"];
            string employeeID = Request["EmployeeID"];

            HrEmployeeVehicle data = ctx.HrEmployeeVehicles.Where(x =>
                x.CompanyCode.Equals(CompanyCode)
                &&
                x.EmployeeID.Equals(employeeID) == true
                &&
                x.VehSeq == vehSeq
            ).FirstOrDefault();

            if (data != null)
            {
                ctx.HrEmployeeVehicles.Remove(data);
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Vehicle data has been deleted.";
            }
            catch (Exception)
            {
                result.message = "Sorry, cannot delete vehicle data.";
            }

            return Json(result);
        }

        public JsonResult List()
        {
            string employeeID = Request["EmployeeID"];

            var data = ctx.HrEmployeeVehicles.Where(x => (
                        x.CompanyCode.Equals(CompanyCode) == true
                        &&
                        x.EmployeeID.Equals(employeeID) == true
                    )).OrderBy(x => x.VehSeq);

            return Json(GeLang.DataTables<HrEmployeeVehicle>.Parse(data, Request));
        }
    }
}
