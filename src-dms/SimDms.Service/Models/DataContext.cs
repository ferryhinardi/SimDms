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
using TracerX;

namespace SimDms.Service.Models
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

        //public IDbSet<SysMessage> SysMsgs { get; set; }
        public IDbSet<ARInterface> ARInterfaces { get; set; }

        public IDbSet<CoProfile> CoProfiles { get; set; }
        public IDbSet<svCoProfileService> CoProfileServices { get; set; }
        public IDbSet<GnMstCustomer> Customers { get; set; }
        public IDbSet<CustomerView> CustomerViews { get; set; }
        public IDbSet<CompanyMapping> CompanyMappings { get; set; }
        public IDbSet<SysMessage> Messages { get; set; }
        public IDbSet<GnMstDealerMapping> GnMstDealerMapping { get; set; }
        public IDbSet<GnMstDealerOutletMapping> GnMstDealerOutletMapping { get; set; } 


        public IDbSet<Document> Documents { get; set; }
        public IDbSet<Employee> Employees { get; set; }
        public IDbSet<EmployeeLookup> EmployeeLookups { get; set; }

        public IDbSet<GLInterface> GLInterfaces { get; set; }

        public IDbSet<Invoice> Invoices { get; set; }
        public IDbSet<InvoiceView> InvoiceViews { get; set; }
        public IDbSet<InvoiceCancelView> InvoiceCancelViews { get; set; }
        public IDbSet<InvoiceLUView> InvoiceLUViews { get; set; }

        public IDbSet<JobOrderView> JobOrderViews { get; set; }
        public IDbSet<CustomerVehicleView> CustomerVehicleViews { get; set; }
        public IDbSet<CustomerVehicle> CustomerVehicles { get; set; }
        public IDbSet<JobTypeView> JobTypeViews { get; set; }
        public IDbSet<SaView> SaViews { get; set; }
        public IDbSet<FmView> FmViews { get; set; }

        public IDbSet<OmMstRefference> OmMstRefferences { get; set; }

        public IDbSet<LookUpDtl> LookUpDtls { get; set; }

        public IDbSet<OrganizationHdr> OrganizationHdrs { get; set; }
        public IDbSet<OrganizationDtl> OrganizationDtls { get; set; }

        public IDbSet<Package> Packages { get; set; }
        public IDbSet<Periode> Periodes { get; set; }
        public IDbSet<PdiFsc> PdiFscs { get; set; }
        public IDbSet<PdiFscView> PdiFscViews { get; set; }
        public IDbSet<PdiFscApplication> PdiFscApplications { get; set; }
        public IDbSet<PdiFscRate> PdiFscRates { get; set; }

        public IDbSet<SysRole> SysRoles { get; set; }
        public IDbSet<SysRoleUser> SysRoleUsers { get; set; }
        public IDbSet<SysUser> SysUsers { get; set; }
        public IDbSet<SysUserView> SysUserViews { get; set; }

        public IDbSet<SubCon> SubCons { get; set; }
        public IDbSet<SubConTask> SubConTasks { get; set; }
        public IDbSet<Supplier> Suppliers { get; set; }
        public IDbSet<TrnService> Services { get; set; }
        public IDbSet<TrnServiceMechanicView> TrnServiceMechanicViews { get; set; }
        public IDbSet<TrnSrvTask> ServiceTasks { get; set; }
        public IDbSet<TrnSrvItem> ServiceItems { get; set; }
        public IDbSet<Mechanic> Mechanics { get; set; }
        public IDbSet<MechanicDetail> MechanicViews { get; set; }
        public IDbSet<MechanicTask> MechanicTasks { get; set; }
        public IDbSet<SupplierProfitCenter> SupplierProfitCenters { get; set; }
        public IDbSet<SubConView> SubConViews { get; set; }
        public IDbSet<SubConRcvView> SubConRcvViews { get; set; }
        public IDbSet<SupplierView> SupplierViews { get; set; }
        public IDbSet<SubConTaskView> SubConTaskViews { get; set; }
        public IDbSet<Return> Returns { get; set; }
        public IDbSet<ReturnView> ReturnViews { get; set; }
        public IDbSet<ReturnInvoice> ReturnInvoices { get; set; }
        public IDbSet<InvoiceDetail> ReturnInvoiceDetails { get; set; }
        public IDbSet<Signature> Signatures { get; set; }
        public IDbSet<SignatureView> SignatureViews { get; set; }
        public IDbSet<SenderDealerView> SenderDealerViews { get; set; }
       
        //public IDbSet<Task> Tasks { get; set; }
        public IDbSet<Tax> Taxes { get; set; }
        public IDbSet<TaxInvoiceView> TaxInvoiceViews { get; set; }
        public IDbSet<TaxInvoice> TaxInvoices { get; set; }
        public IDbSet<TaxInvoiceHQView> TaxInvoiceHQViews { get; set; }
        public IDbSet<svMstRefferenceService> svMstRefferenceServices { get; set; }
        public IDbSet<RefferenceServiceView> RefferenceServiceViews { get; set; }
        public IDbSet<svMstRefferenceServiceView> svMstRefferenceServiceViews { get; set; }
        public IDbSet<TaxInvoiceStdView> TaxInvoiceStdViews { get; set; }
        public IDbSet<TaxInvoiceLookUpView> TaxInvoiceLookUpViews { get; set; }

        public IDbSet<BankBook> BankBooks { get; set; }
        public IDbSet<BasicModelView> BasicModelViews { get; set; }
        public IDbSet<TaskTypeView> TaskTypeViews { get; set; }
        public IDbSet<TaskNoView> TaskNoViews { get; set; }
        public IDbSet<svUploadedFile> UploadedFiles { get; set; }

        public IDbSet<MaintainSPKView> MaintainSPKViews { get; set; }
        public IDbSet<svMstCustomerVehicleHist> svMstCustomerVehicleHists { get; set; }

        public IDbSet<Outstanding> Outstandings { get; set; }
        public IDbSet<AllBranchFromSPK> AllBranchFromSPKs { get; set; }
        public IDbSet<svMstBillType> svMstBillTypes { get; set; }
        public IDbSet<svMstBillTypeView> svMstBillTypeViews { get; set; }
        public IDbSet<svMstGetCusDet> svMstGetCusDets { get; set; }
        public IDbSet<svMstStall> svMstStalls { get; set; }
        public IDbSet<svMstStallView> svMstStallViews { get; set; }
        public IDbSet<SvMstMecAvb> SvMstMecAvbs { get; set; }
        public IDbSet<SvMstMecAvbView> SvMstMecAvbViews { get; set; }
        public IDbSet<SvGetMekanik> SvGetMekaniks { get; set; }
        public IDbSet<SvMstWarranty> SvMstWarranties { get; set; }
        public IDbSet<SvMstGaransiView> SvMstGaransiViews { get; set; }
        public IDbSet<SvBMView> SvBMViews { get; set; }
        public IDbSet<SvJTView> SvJTViews { get; set; }
        public IDbSet<SvMstCampaign> SvMstCampaigns { get; set; }
        public IDbSet<svMstCampaignTrans> SvMstCampaignTrans { get; set; }
        public IDbSet<SvCampaignView> SvCampaignViews { get; set; }
        public IDbSet<SvComplainView> SvComplainViews { get; set; }
        public IDbSet<SvDefectView> SvDefectViews { get; set; }
        public IDbSet<SvOperationView> SvOperationViews { get; set; }
        public IDbSet<svMstPdiFscView> svMstPdiFscViews { get; set; }
        public IDbSet<SvBasicKsgView> SvBasicKsgViews { get; set; }
        public IDbSet<SvMstTarifJasa> SvMstTarifJasas { get; set; }
        public IDbSet<SvLaborCode> SvLaborCodes { get; set; }
        public IDbSet<SvMstWaktuKerja> SvMstWaktuKerjas { get; set; }
        public IDbSet<SvMstWaktuKerjaView> SvMstWaktuKerjaViews { get; set; }
        public IDbSet<SvTarifJasaView> SvTarifJasaViews { get; set; }
        public IDbSet<SvMstPekerjaanBrowse> SvMstPekerjaanBrowses { get; set; }
        public IDbSet<SvBasicModelPekerjaan> SvBasicModelPekerjaans { get; set; }
        public IDbSet<SvJobView> SvJobViews { get; set; }
        public IDbSet<SvGroupJobView> SvGroupJobViews { get; set; }
        public IDbSet<SvNomorAccView> SvNomorAccViews { get; set; }
        public IDbSet<SvRincianJob> SvRincianJobs { get; set; }
        public IDbSet<SvRincianBrowser> SvRincianBrowsers { get; set; }
        public IDbSet<SvRincianPart> SvRincianParts { get; set; }
        public IDbSet<svMstJob> svMstJobs { get; set; }
        public IDbSet<svMstTask> svMstTasks { get; set; }
        public IDbSet<svMstTaskPart> svMstTaskParts { get; set; }
        public IDbSet<svMstTaskPrice> svMstTaskPrices { get; set; }
        public IDbSet<SvPartNoView> SvPartNoViews { get; set; }
        public IDbSet<SvBasicCopy> SvBasicCopys { get; set; }
        public IDbSet<SvClubView> SvClubViews { get; set; }
        public IDbSet<SvNoPolisi> SvNoPolisis { get; set; }
        public IDbSet<SvClubTable> SvClubTables { get; set; }
        public IDbSet<svMstClub> svMstClubs { get; set; }

        public IDbSet<Claim> Claims { get; set; }
        public IDbSet<ClaimApplication> ClaimApplications { get; set; }
        public IDbSet<ClaimView> ClaimViews { get; set; }
        public IDbSet<BasicCodeView> BasicCodeViews { get; set; }
        public IDbSet<OmMstModel> Models { get; set; }
        public IDbSet<ClaimPart> ClaimParts { get; set; }
        public IDbSet<ClaimBatch> ClaimBatchs { get; set; }

        public IDbSet<SvKontrakServiceView> SvKontrakServiceViews { get; set; }
        public IDbSet<SvCustomerDetailView> SvCustomerDetailViews { get; set; }
        public IDbSet<SvVehicleDetailView> SvVehicleDetailViews { get; set; }
        public IDbSet<svMstContract> svMstContracts { get; set; }
        public IDbSet<SvKendaraanPel> SvKendaraanPels { get; set; }
        public IDbSet<SvChassicView> SvChassicViews { get; set; }
        public IDbSet<SvCBasmodView> SvCBasmodViews { get; set; }
        public IDbSet<SvColorView> SvColorViews { get; set; }
        public IDbSet<SvGetTableChassis> SvGetTableChassiss { get; set; }
        public IDbSet<SvMstDealerView> SvMstDealerViews { get; set; }
        public IDbSet<SvEventView> SvEventViews { get; set; }
        public IDbSet<svMstEvent> svMstEvents { get; set; }
        public IDbSet<SvPaymentPackage> SvPaymentPackages { get; set; }
        public IDbSet<svMstPackage> svMstPackages { get; set; }
        public IDbSet<svMstPackageTask> svMstPackageTasks { get; set; }
        public IDbSet<svMstPackagePart> svMstPackageParts { get; set; }
        public IDbSet<SvEventBM> SvEventBMs { get; set; }
        public IDbSet<SvRegPackageView> SvRegPackageViews { get; set; }
        public IDbSet<svMstPackageContract> svMstPackageContracts { get; set; }
        public IDbSet<SvTargetView> SvTargetViews { get; set; }
        public IDbSet<svMstTarget> svMstTargets { get; set; }
        public IDbSet<svMstAccount> svMstAccounts { get; set; }
        public IDbSet<SvRegCampaign> SvRegCampaigns { get; set; }
        public IDbSet<svMstFscCampaign> svMstFscCampaigns { get; set; }
        public IDbSet<svTrnInvClaim> svTrnInvClaims { get; set; }
        public IDbSet<SvTrnCatCode> SvTrnCatCodes { get; set; }
        public IDbSet<SvTrnComCode> SvTrnComCodes { get; set; }
        public IDbSet<SvTrnDefCode> SvTrnDefCodes { get; set; }
        public IDbSet<SvTrnOpNo> SvTrnOpNos { get; set; }
        public IDbSet<GnMstLookUpDtl> GnMstLookUpDtls { get; set; }

        public IDbSet<Item> Items { get; set; }
        public IDbSet<ItemLoc> ItemLocs { get; set; }
        public IDbSet<ItemInfo> ItemInfos { get; set; }
        public IDbSet<ItemPrice> ItemPrices { get; set; }
        public IDbSet<ItemModel> ItemModels { get; set; }

        public IDbSet<SysParameter> SysParameters { get; set; }
        public IDbSet<VehicleServiceView> VehicleServiceViews { get; set; }

        public IDbSet<SvTrnInvItem> SvTrnInvItems { get; set; }
        public IDbSet<SvTrnSPKGeneralView> SvTrnSPKGeneralViews { get; set; }
        public IDbSet<SvTrnClaimSPK> SvTrnClaimSPKs { get; set; }
        public IDbSet<SvTrnClaimJudgement> SvTrnClaimJudgements { get; set; }
        //Master General
        public IDbSet<svGnMstCustomerProfitCenter> svGnMstCustomerProfitCenters { get; set; }
        public IDbSet<SysUserProfitCenter> SysUserProfitCenters { get; set; }
       
        public IDbSet<VOR> VORs { get; set; }
        public IDbSet<VORDtl> VORDtls { get; set; }
        public IDbSet<svTrnSrvNoVOR> NoVORs { get; set; }
        public IDbSet<spTrnPPOSHdr> spTrnPPOSHdrs { get; set; }
        public IDbSet<spTrnPPOSDtl> spTrnPPOSDtls { get; set; }

        public IDbSet<SvUtlJudgementCode> SvUtlJudgementCodes { get; set; }
        public IDbSet<SvUtlJudgementDescription> SvUtlJudgementDescriptions { get; set; }
        public IDbSet<SvUtlTroubleCode> SvUtlTroubleCodes { get; set; }
        public IDbSet<SvUtlTroubleDescription> SvUtlTroubleDescriptions { get; set; }
        public IDbSet<SvUtlSectionCode> SvUtlSectionCodes { get; set; }
        public IDbSet<SvUtlSectionDescription> SvUtlSectionDescriptions { get; set; }
        public IDbSet<SvUtlFlatRate> SvUtlFlatRates { get; set; }
        public IDbSet<SvUtlWarranty> SvUtlWarrantys { get; set; }
        public IDbSet<SvUtlWarrantyTime> SvUtlWarrantyTimes { get; set; }
        public IDbSet<SvUtlCampaign> SvUtlCampaigns { get; set; }
        public IDbSet<SvUtlCampaignRange> SvUtlCampaignRanges { get; set; }
        public IDbSet<SvUtlPdiFsc> SvUtlPdiFscs { get; set; }
        public IDbSet<SvUtlPdiFscAmount> SvUtlPdiFscAmounts { get; set; }
        public IDbSet<SvTrnPdiFscBatch> SvTrnPdiFscBatchs { get; set; }

        public IDbSet<SvTrnDailyRetention> SvTrnDailyRetention { get; set; }
        public IDbSet<SvHstDailyRetention> SvHstDailyRetentions { get; set; }

        public IDbSet<GnDcsUploadFile> GnDcsUploadFiles { get; set; }

        public IDbSet<LookUpHdr> LookUpHdrs { get; set; }
        //public IDbSet<LookUpDetail> LookUpDetails { get; set; }
    }

    public class MDContext : DbContext
    {

        // Override constructor
        public MDContext(string ConnString) : base(ConnString) { }

        // Default constructor
        public MDContext()
            : base(MyHelpers.GetConnString("MDContext"))
        {

        } 

        public IDbSet<Item> Items { get; set; }
        public IDbSet<Invoice> Invoices { get; set; }
        public IDbSet<CustomerVehicle> CustomerVehicles { get; set; }
        public IDbSet<TaxInvoice> TaxInvoices { get; set; }
        public IDbSet<BankBook> BankBooks { get; set; }
        public IDbSet<GnMstCustomer> Customers { get; set; }
        public IDbSet<InvoiceView> InvoiceViews { get; set; }
        public IDbSet<TrnService> Services { get; set; }
        public IDbSet<TrnSrvItem> ServiceItems { get; set; }
        public IDbSet<TrnSrvTask> ServiceTasks { get; set; }
        public IDbSet<Document> Documents { get; set; }
        public IDbSet<ItemPrice> ItemPrices { get; set; }
        public IDbSet<svCoProfileService> CoProfileServices { get; set; }
        public IDbSet<JobOrderView> JobOrderViews { get; set; }
        public IDbSet<LookUpDtl> LookUpDtls { get; set; }
        public IDbSet<SvSDMovement> SvSDMovements { get; set; }
    }
}