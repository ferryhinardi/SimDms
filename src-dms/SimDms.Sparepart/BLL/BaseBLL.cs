using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimDms.Sparepart.Models;
using SimDms.Common.Models;
using System.Web.Security;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;

namespace SimDms.Sparepart.BLL
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

        protected string CompanyMD
        {
            get
            {
                var result = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
                if (result != null)
                {
                    return result.CompanyMD;
                }
                else return CompanyCode;
            }
        }

        protected string BranchMD
        {
            get
            {
                var result = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
                if (result != null)
                {
                    return result.BranchMD;
                }
                else return BranchCode;
            }
        }

        protected string WarehouseMD
        {
            get
            {
                var result = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
                if (result != null)
                {
                    return result.WarehouseMD;
                }
                else return "00";
            }
        }

        protected string GetDbMD()
        {
            string sql = string.Format(@"SELECT dbo.GetDbMD('{0}', '{1}')",
                        CompanyCode, BranchCode);

            return ctx.Database.SqlQuery<string>(sql).FirstOrDefault();
        }
    }
}
