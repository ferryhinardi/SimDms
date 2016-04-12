using SimDms.Common;
using SimDms.Common.Models;
using SimDms.Tax.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace SimDms.Tax.BLL
{
    public class BaseBLL
        {
            public DataContext ctx = new DataContext(MyHelpers.GetConnString("DataContext"));

            public static string username = "";

            protected SysUser CurrentUser
            {
                get
                {
                    return ctx.SysUsers.Find(username);
                }
            }

            protected SysUser CurrentUserByUname()
            {
                return ctx.SysUsers.Find(username);
            }

            protected string CompanyCode
            {
                get
                {
                    return CurrentUser.CompanyCode;
                }
            }

            protected string CompanyName
            {
                get
                {
                    return ctx.OrganizationHdrs.Find(CurrentUser.CompanyCode).CompanyName;
                }
            }

            protected string BranchCode
            {
                get
                {
                    return CurrentUser.BranchCode;
                }
            }

            protected string BranchName
            {
                get
                {
                    return ctx.CoProfiles.Find(CompanyCode, BranchCode).CompanyName;
                }
            }

            protected string ProductType
            {
                get
                {
                    return ctx.CoProfiles.Find(CompanyCode, BranchCode).ProductType;
                }
            }

            protected string ProfitCenter
            {
                get
                {
                    string s = "000";
                    var x = ctx.SysUserProfitCenters.Find(CurrentUser.UserId);
                    if (x != null) s = x.ProfitCenter;
                    return s;
                }
            }
        }
}
