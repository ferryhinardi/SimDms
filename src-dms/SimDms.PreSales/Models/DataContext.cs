using SimDms.Common;
using SimDms.Common.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;

namespace SimDms.PreSales.Models
{
    public class DataContext : DbContext
    {
        public DataContext(string ConnString)
            : base(ConnString)
        {
        }

        public DataContext() :
            base(MyHelpers.GetConnString("DataContext"))
        {

        }

        protected Exception HandleDataUpdateException(DbUpdateException exception)
        {
            Exception innerException = exception.InnerException;

            while (innerException.InnerException != null)
            {
                innerException = innerException.InnerException;
            }

            Elmah.ErrorSignal.FromCurrentContext().Raise(innerException);
            return innerException;
        }

        protected Exception HandleDataValidationException(DbEntityValidationException exception)
        {
            var stringBuilder = new StringBuilder();

            foreach (DbEntityValidationResult result in exception.EntityValidationErrors)
            {
                foreach (DbValidationError error in result.ValidationErrors)
                {
                    stringBuilder.AppendFormat("{0} [{1}]: {2}",
                        result.Entry.Entity.ToString().Split('.').Last(), error.PropertyName, error.ErrorMessage);
                    stringBuilder.AppendLine();
                }
            }

            var ex = new Exception(stringBuilder.ToString().Trim());
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            return ex;
        }

        public override int SaveChanges()
        {
            int n = 0;

            try
            {
                n = base.SaveChanges();
            }
            catch (DbEntityValidationException vex)
            {
                throw HandleDataValidationException(vex);
            }
            catch (DbUpdateException dbu)
            {
                throw HandleDataUpdateException(dbu);
            }

            return n;
        }

        public IDbSet<SysRoleUser> SysRoleUsers { get; set; }
        public IDbSet<CoProfile> CoProfiles { get; set; }
        public IDbSet<CoProfileSales> CoProfileSaleses { get; set; }
        public IDbSet<DealerMapping> DealerMappings { get; set; }
        public IDbSet<Employee> Employees { get; set; }
        public IDbSet<pmKDPCoupon> pmKDPCoupon { get; set; }
        public IDbSet<pmMstCoupon> pmMstCoupon { get; set; }
        public IDbSet<PmKdp> PmKdps { get; set; }
        public IDbSet<PmKdpTemp> PmKdpTemps { get; set; } 
        public IDbSet<PmKdpAdditional> PmKdpAdditionals { get; set; }
        public IDbSet<PmActivity> PmActivites { get; set; }
        public IDbSet<PmBranchOutlet> PmBranchOutlets { get; set; }
        public IDbSet<PmStatusHistory> PmStatusHistories { get; set; }
        public IDbSet<LookUpDtl> LookUpDtls { get; set; }
        public IDbSet<OrganizationHdr> OrganizationHdrs { get; set; }
        public IDbSet<OrganizationDtl> OrganizationDtls { get; set; }
        public IDbSet<Position> Positions { get; set; }
        public IDbSet<GroupType> GroupTypes { get; set; }
        public IDbSet<MstModel> MstModels { get; set; }
        public IDbSet<MstModelColour> MstModelColours { get; set; }
        public IDbSet<MstRefference> MstRefferences { get; set; }
        public IDbSet<ItsMstModel> ItsMstModels { get; set; }
        public IDbSet<HrEmployee> HrEmployees { get; set; }
        public IDbSet<HrEmployeeSales> HrEmployeeSales { get; set; }
        public IDbSet<HrEmployeeView> HrEmployeeViews { get; set; }
        public IDbSet<HrEmployeeMutation> HrEmployeeMutations { get; set; }
        public IDbSet<HrEmployeeAchievement> HrEmployeeAchievements { get; set; }
        public IDbSet<HrEmployeeAdditionalJob> HrEmployeeAdditionalJobs { get; set; }

        public IDbSet<SysUser> SysUsers { get; set; }
        public IDbSet<SysUserView> SysUserViews { get; set; }
        public IDbSet<TeamMember> TeamMembers { get; set; }
        public IDbSet<Team> Teams { get; set; }

        public IDbSet<PmKdpClnUpView> PmKdpClnUpViews { get; set; }
        public IDbSet<DealerOutletMapping> DealerOutletMappings { get; set; }
        public IDbSet<GroupModels> GroupModels { get; set; }
        public IDbSet<PmHstITS> PmHstITSs { get; set; }
        public IDbSet<SysUserProfitCenter> SysUserProfitCenters { get; set; }
        public IDbSet<GnMstCoProfileService> GnMstCoProfileServices { get; set; }
        public IDbSet<GnMstCoProfileSales> GnMstCoProfileSales { get; set; }  
        public IDbSet<GnMstCoProfileSpare> GnMstCoProfileSpares { get; set; }
        public IDbSet<OmTRSalesSO> OmTRSalesSOs { get; set; }
        public IDbSet<OmTrSalesSOModel> OmTrSalesSOModels { get; set; }
        public IDbSet<OmMstModelColour> OmMstModelColours { get; set; }
        public IDbSet<OmMstPricelistSell> OmMstPricelistSells { get; set; }
        public IDbSet<OmMstModel> OmMstModels { get; set; }
        public IDbSet<OmTrSalesSOModelColour> OmTrSalesSOModelColours { get; set; }
        public IDbSet<omTrSalesSOVin> omTrSalesSOVins { get; set; }
        public IDbSet<OmMstVehicle> OmMstVehicles { get; set; }        
    }
}