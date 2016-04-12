using SimDms.DataWarehouse.Helpers;
using SimDms.DataWarehouse.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;

namespace SimDms.DataWarehouse.Controllers
{
    public class DataContext : DbContext
    {

        // Override constructor
        public DataContext(string ConnString) : base(ConnString) { }

        // Default constructor
        public DataContext()
            : base(MyHelpers.GetConnString("DataContext")) { }

        //public int AutoSaveChanges(string userId, string companyId)
        //{
        //    int n = 0;

        //    DateTime saveTime = DateTime.Now;

        //    foreach (var entry in this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
        //    {
        //        string createddate = "";
        //        string createdby = "";
        //        string companyid = "";

        //        foreach (var propName in entry.CurrentValues.PropertyNames)
        //        {
        //            if ((propName.ToLower() == "createddate") || (propName.ToLower() == "createdate"))
        //            {
        //                createddate = propName;
        //            }
        //            else if ((propName.ToLower() == "createdby"))
        //            {
        //                createdby = propName;
        //            }
        //            else if ((propName.ToLower() == "companyid"))
        //            {
        //                companyid = propName;
        //            }
        //        }

        //        if (createddate != "")
        //        {
        //            entry.Property(createddate).CurrentValue = saveTime;
        //        }
        //        if (createdby != "")
        //        {
        //            entry.Property(createdby).CurrentValue = userId;
        //        }
        //        if (companyid != "")
        //        {
        //            entry.Property(companyid).CurrentValue = companyId;
        //        }
        //    }

        //    foreach (var entry in this.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
        //    {
        //        string changedate = "";
        //        string changeby = "";

        //        foreach (var propName in entry.CurrentValues.PropertyNames)
        //        {
        //            if ((propName.ToLower() == "changeddate") || (propName.ToLower() == "modifieddate"))
        //            {
        //                changedate = propName;
        //            }
        //            else if ((propName.ToLower() == "changedby") || (propName.ToLower() == "modifiedby"))
        //            {
        //                changeby = propName;
        //            }
        //        }

        //        if (changedate != "")
        //        {
        //            entry.Property(changedate).CurrentValue = saveTime;
        //        }
        //        if (changeby != "")
        //        {
        //            entry.Property(changeby).CurrentValue = userId;
        //        }
        //    }

        //    n = base.SaveChanges();
        //    return n;
        //}

        protected Exception HandleDataUpdateException(DbUpdateException exception)
        {
            Exception innerException = exception.InnerException;

            while (innerException.InnerException != null)
            {
                innerException = innerException.InnerException;
            }

            return new Exception(innerException.Message);
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

            return new Exception(stringBuilder.ToString().Trim());
        }


        public int SaveChanges()
        {
            int svRet = 0;

            try
            {
                svRet = base.SaveChanges();
            }
            catch (DbEntityValidationException vex)
            {
                var exception1 = HandleDataValidationException(vex);
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception1);
                throw exception1;
            }
            catch (DbUpdateException dbu)
            {
                var exception = HandleDataUpdateException(dbu);
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                throw exception;
            }

            return svRet;
        }

        public IDbSet<CompaniesGroupMappingView> CompaniesGroupMappingViews { get; set; }
        public IDbSet<CompaniesGroupMappingView2> CompaniesGroupMapping { get; set; }

        public IDbSet<ListDealerActive> ActiveDealers { get; set; }
        public IDbSet<LastTransDateInfo> TransactionDateInfo { get; set; }

        public IDbSet<MstLookupDtl> MstLookupDtls { get; set; }
        public IDbSet<CoProfile> CoProfiles { get; set; }
        public IDbSet<CoProfileView> BranchList { get; set; }
        public IDbSet<CsLkuTDayCallView> CsLkuTDayCallViews { get; set; }
        public IDbSet<CsLkuBirthdayView> CsLkuBirthdayViews { get; set; }
        public IDbSet<CsLkuStnkExtView> CsLkuStnkExtViews { get; set; }
        public IDbSet<CsLkuBpkbView> CsLkuBpkbViews { get; set; }
        public IDbSet<CsSetting> CsSettings { get; set; }
        public IDbSet<svMstDealerOutletMapping> svMstDealerOutletMappings { get; set; }
        public IDbSet<SvTrnMRSR> SvTrnMRSRs { get; set; } 

        public IDbSet<DealerGroupMappingView> DealerGroupMappingViews { get; set; }
        public IDbSet<DealerInfo> DealerInfos { get; set; }
        public IDbSet<DealerModuleMapping> DealerModuleMappings { get; set; }

        public IDbSet<GenerateITSView> GenerateITSViews { get; set; }
        public IDbSet<GnMstDealerMapping> GnMstDealerMappings { get; set; }
        public IDbSet<GnMstDealerOutletMapping> GnMstDealerOutletMappings { get; set; }
        public IDbSet<GnMstDealerMappingNew> GnMstDealerMappingNews { get; set; }
        public IDbSet<GnMstDealerOutletMappingNew> GnMstDealerOutletMappingNews { get; set; }
        public IDbSet<GroupArea> GroupAreas { get; set; }
        public IDbSet<GroupArea2W> GroupAreas2W { get; set; }
        public IDbSet<SrvGroupArea> SrvGroupAreas { get; set; }
        public IDbSet<GnMstLookUpDtl> GnMstLookUpDtls { get; set; }
        public IDbSet<GnMstPosition> GnMstPositions { get; set; }

        public IDbSet<HrOrgGroup> HrOrgGroups { get; set; }
        public IDbSet<HrPosition> HrPositions { get; set; }
        public IDbSet<HrEmployee> HrEmployees { get; set; }
        public IDbSet<HrEmployeeMutation> HrEmployeeMutations { get; set; }
        public IDbSet<HrDepartmentTraining> HrDepartmentTraining { get; set; }
        public IDbSet<HrMstTraining> HrMstTrainings { get; set; }

        public IDbSet<Organization> Organizations { get; set; }
        public IDbSet<OrganizationDtl> OrganizationDtls { get; set; }
        public IDbSet<OrgGroup> OrgGroups { get; set; }
        public IDbSet<OutletInfo> OutletInfos { get; set; }
        public IDbSet<svMasterDealerOutletMapping> svMasterDealerOutletMappings { get; set; }
        public IDbSet<svMasterDealerMapping> svMasterDealerMappings { get; set; }
        public IDbSet<OmMstModel> OmMstModels { get; set; }
        //public IDbSet<CoProfile> gnMstCoProfiles { get; set; }
        public IDbSet<OmMstModelColour> OmMstModelColours { get; set; }
        public IDbSet<OmMstRefference> OmMstRefferences { get; set; }
        public IDbSet<TargetVIN> TargetVINs { get; set; }

        public IDbSet<PartSalesView> PartSalesViews { get; set; }
        public IDbSet<PmActivity> PmActivities { get; set; }
        public IDbSet<PmBranchOutlet> PmBranchOutlets { get; set; }
        public IDbSet<PmExecSummaryView> PmExecSummaries { get; set; }
        public IDbSet<PmDashboardData> PmDashboardDatas { get; set; }
        public IDbSet<PmGroupTypeSeq> PmGroupTypeSeqs { get; set; }
        public IDbSet<PmItsByTestDrive> PmItsByTestDrives { get; set; }
        public IDbSet<PmItsByLeadTime> PmItsByLeadTimes { get; set; }
        public IDbSet<PmKDP> PmKDPs { get; set; }
        public IDbSet<PmKDPAdditional> PmKDPAdditionals { get; set; }
        public IDbSet<PmStatusHistory> PmStatusHistories { get; set; }
        public IDbSet<PmKDPExhibition> PmKDPExhibitions { get; set; }
        public IDbSet<PmKdpAdditionalExhibition> PmKDPAdditionalExhibitions { get; set; }
        public IDbSet<PmStatusHistoryExhibition> PmStatusHistoryExhibitions { get; set; }
        public IDbSet<Position> Positions { get; set; }
        public IDbSet<pmKDPCoupon> pmKDPCoupons { get; set; }

        public IDbSet<pmQuota> pmQuotas { get; set; }
        public IDbSet<msMstModel> msMstModels { get; set; }
        public IDbSet<pmIndent> pmIndents { get; set; }
        public IDbSet<pmIndentAdditional> pmIndentAdditionals { get; set; }
        public IDbSet<pmStatusHistoryIndent> pmStatusHistoryIndents { get; set; }
        public IDbSet<pmActivitiesIndent> pmActivitiesIndents { get; set; }

        public IDbSet<SvCustomerSatisfactionScore> SvCustomerSatisfactionScores { get; set; }
        public IDbSet<svCustomerSatisfactionScoreLog> svCustomerSatisfactionScoreLogs { get; set; }
        public IDbSet<svMstDealerAndOutletServiceMapping> svMstDealerAndOutletServiceMappings { get; set; }
        public IDbSet<SpHstPartSales> SpHstPartSalesList { get; set; }
        public IDbSet<SvRegisterSpk> SvRegisterSpkList { get; set; }
        public IDbSet<SvFtir> SvFtirList { get; set; }
        public IDbSet<SysUserView> SysUserViews { get; set; }
        public IDbSet<SysUserDealer> SysUserDealers { get; set; }

        public IDbSet<UnitIntakeView> UnitIntakeViews { get; set; }
        public IDbSet<SvUnitIntakeView> SvUnitIntakeViews { get; set; }
        public IDbSet<ViewHrInqPersonalInformation> ViewHrInqPersonalInformations { get; set; }

        public IDbSet<PmITSByLostCase0> PmITSByLostCase0 { get; set; }
        public IDbSet<PmITSByLostCase1> PmITSByLostCase1 { get; set; }
        public IDbSet<PmITSByPerolehanData0> PmITSByPerolehanData0 { get; set; }
        public IDbSet<PmITSByPerolehanData1> PmITSByPerolehanData1 { get; set; }
        public IDbSet<GnSchedulerLog> GnSchedulerLogs { get; set; }

        public IDbSet<qa2MstRefferenceCharModel> qa2MstRefferenceCharModel { get; set; }

        public IDbSet<Qa2RowDataSubModel> Qa2RowDataSubModel { get; set; }

        public IDbSet<DealerGroupMappings> DealerGroupMapping { get; set; }
        public IDbSet<QaMstCompany> QaMstCompanys { get; set; }
        public IDbSet<Qa2MstCompany> Qa2MstCompanys { get; set; }
        public IDbSet<QaMstException> QaMstExceptions { get; set; }
        public IDbSet<QaMstProtection> QaMstProtections { get; set; }
        public IDbSet<SysSQLGateway> SQLGateway { get; set; }

        public IDbSet<msMstGroupModel> MsMstGroupModels { get; set; }

        public IDbSet<svTrnSrvVOR> svTrnSrvVORs { get; set; }
        public IDbSet<ReffService> ReffServices { get; set; }
        public IDbSet<svMstTask> svMstTasks { get; set; }
        public IDbSet<svMstCampaign> svMstCampaigns { get; set; }

        public IDbSet<MsModelMappingHdr> MsModelMappingHdrs { get; set; }
        public IDbSet<MsModelMappingDtl> MsModelMappingDtls { get; set; }
        //public IDbSet<svTrnSrvVOR> svTrnSrvVORs { get; set; }  

        public IDbSet<CsReviews> CsReviews { get; set; }
        public IDbSet<gnMstCalendar> gnMstCalendar { get; set; }
        public IDbSet<spMstAOSWarrantyParts> MstAOSWarrantyParts { get; set; }
        public IDbSet<SpMstItemInfo> MstItemInfos { get; set; }
    }

        public class SuzukiR4Context : DbContext
        {
            // Override constructor
            public SuzukiR4Context(string ConnString) : base(ConnString) { }

            // Default constructor
            public SuzukiR4Context()
                : base(MyHelpers.GetConnString("SuzukiR4Context")) { }
        }
    }
