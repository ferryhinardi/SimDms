using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class CalenderController : BaseController
    {
        public JsonResult Default()
        {
            var year = Request["year"];
            var type = Request["type"]??"";
            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<string>("select convert(varchar(20),CalendarDate,112) from GnMstCalendar").AsQueryable();
            if (type != "")
            {
                data = ctx.Database.SqlQuery<string>("select convert(varchar(20),CalendarDate,112) from GnMstCalendar where YEAR(CalendarDate) = @p0 and CalendarDescription='holiday'", year).AsQueryable();
            }
            return Json(data);
        }

        public JsonResult AddSabtuMinggu() 
        {
            var year = Request["year"];
            ctx.Database.CommandTimeout = 3600;
            string data = ctx.Database.ExecuteSqlCommand("exec uspfn_addSabtuMinggu @Year=@p0, @userId=@p1", year, CurrentUser.Username).ToString();
            return Json(data);
        }

        public JsonResult save() 
        {
            var MultiDates = Request["MultiDates"];
            ctx.Database.CommandTimeout = 3600;
            try
            {
                string data = ctx.Database.ExecuteSqlCommand("exec uspfn_addHoliday @MultiDates=@p0, @userId=@p1", MultiDates, CurrentUser.Username).ToString();
                return Json(new { success = true, message = "success" });
            }
            catch (Exception ex)
            {
                {
                    return Json(new { success = false, message = (ex.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message });
                }
            }
        }

        public JsonResult delete()
        {
            var date = Request["DateAct"]; 
            ctx.Database.CommandTimeout = 3600;
            try
            {
                string data = ctx.Database.ExecuteSqlCommand("delete GnMstCalendar where CalendarDate = @p0", date).ToString();
                return Json(new { success = true, message = "success" });
            }
            catch (Exception ex)
            {
                {
                    return Json(new { success = false, message = (ex.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message });
                }
            }
        }
    }
}
