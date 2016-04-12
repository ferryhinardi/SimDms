using SimDms.CStatisfication.Models;
using SimDms.CStatisfication.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.CStatisfication.Utilities
{
    public class UtilHelper
    {
        private static DataContext ctx;

        public static void InitializeContext()
        {
            ctx = new DataContext();
        }

        public static ResultModel InitializeResultModel()
        {
            return new ResultModel()
            {
                success = false,
                message = "",
                details = "",
                data = null
            };
        }

        public static DealerInfo DealerInfo()
        {
            DealerInfo result = (from x in ctx.GnMstOrganizationHdrs
                                 select new DealerInfo()
                                 {
                                    CompanyCode = x.CompanyCode,
                                    CompanyName = x.CompanyName
                                 }).FirstOrDefault();

            return result;
        }
    }
}