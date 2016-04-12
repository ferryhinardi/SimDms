using SimDms.Common.Models;
using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class UtilityController : BaseController
    {
        //
        // GET: /Utility/
        #region MaintainChassis
        public JsonResult GetMaintainChassisLinkData(string chassisCode, string chassisNo)
        {
            var query = "exec uspfn_SvUpdateChassisInq @p0, @p1, @p2";
            var result = ctx.Database.SqlQuery<SvUtilMaintainChassisLinkData>(
                    query, CompanyCode, chassisCode, chassisNo).AsQueryable();
            return Json(new { data = result });
        }

        public JsonResult MaintainChassisSave(string chassisCode, string chassisNo, string chassisCodeNew, string chassisNoNew)
        {
            var message = "";
            var query = "exec uspfn_SvUpdateChassis @p0, @p1, @p2, @p3, @p4, @p5";
            try 
            {
                ctx.Database.ExecuteSqlCommand(query,
                    CompanyCode, chassisCode, chassisCodeNew, CurrentUser.UserId, chassisNo, chassisNoNew);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message });
        }
        #endregion
    }
}
