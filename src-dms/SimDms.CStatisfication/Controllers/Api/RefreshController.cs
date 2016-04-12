using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class RefreshController : BaseController
    {
        public JsonResult Reload3DaysCall()
        {
            ctx.Database.ExecuteSqlCommand("exec uspfn_ReloadCSTdayCallResource");
            return null;
        }

        public JsonResult ReloadStnkExtension()
        {
            ctx.Database.ExecuteSqlCommand("exec uspfn_ReloadSTNKExtResource");
            return null;
        }

        public JsonResult ReloadBpkbReminder()
        {
            ctx.Database.ExecuteSqlCommand("exec uspfn_ReloadBPKBReminderResource");
            return null;
        }

        public JsonResult ReloadCustomerBirthDay()
        {
            ctx.Database.ExecuteSqlCommand("exec uspfn_ReloadCustBDayResource");
            return null;
        }
    }
}
