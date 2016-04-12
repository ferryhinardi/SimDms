using SimDms.Service.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class MaintainVehicleController : BaseController
    {
        //
        // GET: /MaintainVehicle/

        public JsonResult GetVehicleTypeData(int vehicleType)
        {
            var query = "exec uspfn_GetDataJenisKendaraan @p0, @p1, @p2, @p3";
            var result = ctx.Database.SqlQuery<SvUtilMaintainVehicle>(query,
                CompanyCode, BranchCode, ProductType, vehicleType).ToList().Select(x => new
                {
                    IsSelected = x.IsSelected ? 1 : 0,
                    x.BasicModel,
                    x.Description
                });

            return Json(result);
        }

        public JsonResult ChangeVehicleType(List<SvUtilMaintainVehicle> model)
        {
            var message = "";
            
            try
            {
                foreach (var row in model)
                {
                    var service = ctx.svMstRefferenceServices.FirstOrDefault(x => x.CompanyCode == CompanyCode &&
                        x.ProductType == ProductType &&
                        x.RefferenceType == "BASMODEL" && x.RefferenceCode == row.BasicModel);
                    if (service == null) throw new Exception("Item is not found");
                    service.IsLocked = !service.IsLocked;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message });
        }
    }
}
