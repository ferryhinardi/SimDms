using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class MaintainServiceBookNoController : BaseController
    {
        //
        // GET: /MaintainServiceBookNo/
        public JsonResult GetAllServiceBookNo(string flag)
        {
            var query = "exec uspfn_SvGetAllServiceBookNo @p0, @p1, @p2";
            var result = ctx.Database.SqlQuery<MaintainServiceBookNo>(query, CompanyCode, BranchCode, flag);
            
            return Json(result);
        }

        public JsonResult SetServiceBookNo(IEnumerable<MaintainServiceBookNoSave> model)
        {
            var message = "";

            try
            {
                if (model != null)
                {
                    var query = "exec uspfn_SvSetServiceBookNo @p0, @p1, @p2, @p3, @p4, @p5";
                    foreach (var item in model)
                    {
                        ctx.Database.ExecuteSqlCommand(query,
                            CompanyCode, BranchCode, item.ChassisCode, item.ChassisNo, item.NewServiceBookNo, item.Flag);
                    }
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
