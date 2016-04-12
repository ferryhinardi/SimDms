using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SimDms.Common.Models;
using SimDms.Common;
using System.Data.Entity.Validation;
using System.Text;
using System.Data.Entity.Infrastructure;

namespace SimDms.Sparepart.Models
{
    public partial class DataContext : DbContext
    {
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
        //protected override int SaveChanges()
        //{
        //    int n = 0;

        //    try
        //    {
        //        n = base.SaveChanges();
        //    }
        //    catch (DbEntityValidationException vex)
        //    {
        //        throw HandleDataValidationException(vex);
        //    }
        //    catch (DbUpdateException dbu)
        //    {
        //        throw HandleDataUpdateException(dbu);
        //    }

        //    return n;
        //}

        public IDbSet<SysUser> SysUsers { get; set; }
        public IDbSet<SysRoleUser> SysRoleUsers { get; set; }
        public IDbSet<SysUserView> SysUserViews { get; set; }
        public IDbSet<SysRole> SysRoles { get; set; }
        public IDbSet<SysMenu> SysMenus { get; set; }
        public IDbSet<SysRoleMenu> SysRoleMenus { get; set; }
        public IDbSet<SysParameter> SysParameters { get; set; }
        public IDbSet<SysParam> sysParams  { get; set; }
        public IDbSet<SysReport> SysReports { get; set; }
        public IDbSet<CoProfile> CoProfiles { get; set; }
        public IDbSet<LookUpDtl> LookUpDtls { get; set; }
        public IDbSet<OrganizationHdr> OrganizationHdrs { get; set; }
        public IDbSet<OrganizationDtl> OrganizationDtls { get; set; }
        public IDbSet<SysModule> SysModules { get; set; }
        public IDbSet<SysModuleView> SysModuleViews { get; set; }
        public IDbSet<SysRoleModule> SysRoleModules { get; set; }
        public IDbSet<SysUserProfitCenter> SysUserProfitCenters { get; set; }
        public IDbSet<SupplierView> SupplierViews { get; set; }
        public IDbSet<SpMasterItemView> SpMasterItemViews { get; set; }
        public IDbSet<SpMasterPartView> SpMasterPartViews { get; set; }
        public IDbSet<SpItemPriceView> SpItemPriceViews { get; set; }
        public IDbSet<spMstItemPrice> spMstItemPrices { get; set; }
        public IDbSet<spHstItemPrice> spHstItemPrices { get; set; }
        public IDbSet<spMstAccountView> spMstAccountViews { get; set; }
        public IDbSet<spMstAccount> spMstAccounts { get; set; }
        public IDbSet<spMstMovingCode> spMstMovingCodes { get; set; }
        public IDbSet<spTrnPOrderBalance> spTrnPOrderBalances {get;set;}
        //public IDbSet<spgnMstAccountView> spgnMstAccountViews { get; set; }
        //public IDbSet<spMstMovingCodeView> spMstMovingCodeViews { get; set; }
        public IDbSet<spMstPurchCampaign> spMstPurchCampaigns { get; set; }
        //public IDbSet<spMstSalesCampaignView> spMstSalesCampaignViews { get; set; }
        //public IDbSet<spMstMovingCode> spMstMovingCodes { get; set; }
        public IDbSet<spMstSalesCampaign> spMstSalesCampaigns { get; set; }
        public IDbSet<spMstPurchCampaignView> spMstPurchCampaignViews { get; set; }
        public IDbSet<spMstOrderParam> spMstOrderParams { get; set; }
        //public IDbSet<spMstOrderParamView> spMstOrderParamViews { get; set; }
        public IDbSet<gnMstSupplierView> gnMstSupplierViews { get; set; }

        public IDbSet<spMstSalesTarget> spMstSalesTargets { get; set; }
        public IDbSet<spMstSalesTargetview> spMstSalesTargetviews { get; set; }
        public IDbSet<spMstSalesTargetDtl> spMstSalesTargetDtls { get; set; }
        public IDbSet<SpMstItemConversion> SpMstItemConversions { get; set; }
        public IDbSet<SpMstItemConversionview> SpMstItemConversionviews { get; set; }
        public IDbSet<spMstItemMod> spMstItemMods { get; set; }
        public IDbSet<spMstCompanyAccount> spMstCompanyAccounts { get; set; }
        public IDbSet<spMstCompanyAccountdtl> spMstCompanyAccountdtls { get; set; }
        public IDbSet<spMstCompanyAccountdtlview> spMstCompanyAccountdtlviews { get; set; }
        public IDbSet<SpTrnSFPJInfo> SpTrnSFPJInfos { get; set; }
        public IDbSet<SpTrnSFPJHdr> SpTrnSFPJHdrs { get; set; }
        public IDbSet<SpTrnSFPJHdrLog> SpTrnSFPJHdrLogs { get; set; }
        public IDbSet<SpTrnSInvoiceDtl> SpTrnSInvoiceDtls { get; set; }
        public IDbSet<SpTrnSInvoiceDtlLog> SpTrnSInvoiceDtlLogs { get; set; }
        public IDbSet<SpTrnSInvoiceHdr> SpTrnSInvoiceHdrs { get; set; }
        public IDbSet<SpTrnSInvoiceHdrLog> SpTrnSInvoiceHdrLogs { get; set; }
        public IDbSet<SpTrnSLmpDtl> SpTrnSLmpDtls { get; set; }
        public IDbSet<SpUtlStockTrfDtl> SpUtlStockTrfDtls { get; set; }
        public IDbSet<SpUtlStockTrfHdr> SpUtlStockTrfHdrs { get; set; }

        public IDbSet<SpTrnSPickingHdr> SpTrnSPickingHdrs { get; set; }
        public IDbSet<GnMstCustomer> GnMstCustomers { get; set; }
        public IDbSet<GnMstFPJSignDate> GnMstFPJSignDates { get; set; }

        //public IDbSet<GnMstDealerMapping> GnMstDealerMappings { get; set; }
        public IDbSet<GnMstDealerOutletMapping> GnMstDealerOutletMappings { get; set; }

        public IDbSet<CompanyMapping> CompanyMappings { get; set; }
        public IDbSet<SvSDMovement> SvSDMovements { get; set; }
        public IDbSet<SpTrnSORDHdr> SpTrnSORDHdrs { get; set; }
        public IDbSet<SpTrnSORDDtl> SpTrnSORDDtls { get; set; }
        public IDbSet<SpUtlPORDDHdr> SpUtlPORDDHdrs { get; set; }
        public IDbSet<SpUtlPORDDDtl> SpUtlPORDDDtls { get; set; }
        public IDbSet<SpTrnSFPJDtl> SpTrnSFPJDtls { get; set; }
        public IDbSet<spTrnIWHTrfHdr> spTrnIWHTrfHdrs { get; set; }
        public IDbSet<spTrnIWHTrfDtl> spTrnIWHTrfDtls { get; set; }
        public IDbSet<spTrnIReservedHdr> spTrnIReservedHdrs { get; set; }
        public IDbSet<spTrnIReservedDtl> spTrnIReservedDtls { get; set; }
        public IDbSet<SpTrnIMovement> SpTrnIMovements { get; set; }
        public IDbSet<SpTrnSOSupply> SpTrnSOSupplys { get; set; }
        public IDbSet<SpTrnSPickingDtl> SpTrnSPickingDtls { get; set; }
        public IDbSet<SpHstDemandCust> SpHstDemandCusts { get; set; }
        public IDbSet<GnMstCustomerClass> GnMstCustomerClasses { get; set; }
        public IDbSet<ArInterface> ArInterfaces { get; set; }
        public IDbSet<Employee> Employees { get; set; }
        public IDbSet<ProfitCenter> ProfitCenters { get; set; }
        public IDbSet<Tax> Taxes { get; set; }
        public IDbSet<GnMstReminder> GnMstReminders { get; set; }
        public IDbSet<GnMstApproval> GnMstApprovals { get; set; } 
        public IDbSet<SpTrnPRcvClaimDtl> SpTrnPRcvClaimDtls { get; set; }
        public IDbSet<SpTrnPRcvClaimHdr> SpTrnPRcvClaimHdrs { get; set; }
        public IDbSet<SpTrnSLmpHdr> SpTrnSLmpHdrs { get; set; }
        public IDbSet<SpTrnSFPJDtlLog> SpTrnSFPJDtlLogs { get; set; }
        public IDbSet<ApInterface> ApInterfaces { get; set; }

        

        public IDbSet<SpTrnPHPP> SpTrnPHPPs { get; set; }
        public IDbSet<SpLoadEntryHPP> SpLoadEntryHPPs { get; set; }

        public IDbSet<SpTrnSRturSSHdr> SpTrnSRturSSHdrs { get; set; }
        public IDbSet<SpTrnSRTurSSDtl> SpTrnSRTurSSDtls { get; set; }

        public IDbSet<GridEntryWRS> GridEntryWRSs { get; set; }
        

        public IDbSet<GLInterface> GLInterfaces { get; set; }
        public IDbSet<spTrnPBinnHdr> spTrnPBinnHdrs { get; set; }
        public IDbSet<SpTrnPBinnDtl> SpTrnPBinnDtls { get; set; }
        public IDbSet<SpTrnPRcvDtl> SpTrnPRcvDtls { get; set; }
        public IDbSet<GnMstCustomerDisc> GnMstCustomerDiscs { get; set; }
        public IDbSet<spTrnPPOSHdr> spTrnPPOSHdrs { get; set; }
        public IDbSet<spTrnPPOSDtl> spTrnPPOSDtls { get; set; }
        public IDbSet<Periode> Periodes { get; set; }
        public IDbSet<SpEdpDetail> SpEdpDetails { get; set; }
        public IDbSet<BankBook> BankBooks { get; set; }
        public IDbSet<SpTrnSBPSFHdr> SpTrnSBPSFHdrs { get; set; }
        public IDbSet<SpTrnSBPSFDtl> SpTrnSBPSFDtls { get; set; }
        public IDbSet<SpMstItemInfo> SpMstItemInfos { get; set; }
        public IDbSet<SpHstDemandItem> SpHstDemandItems { get; set; }
        public IDbSet<SvTrnSrvItem> SvTrnSrvItems { get; set; }

        public IDbSet<SpTrnStockTakingHdr> SpTrnStockTakingHdrs { get; set; }
        public IDbSet<SpTrnStockTakingDtl> SpTrnStockTakingDtls { get; set; }
        public IDbSet<SpTrnStockTakingTemp> SpTrnStockTakingTemps { get; set; }
        public IDbSet<SpTrnStockTakingLog> SpTrnStockTakingLogs { get; set; }

        public IDbSet<SupplierProfitCenter> SupplierProfitCenters { get; set; }
        public IDbSet<SpTrnSRturSrvHdr> SpTrnSRturSrvHdrs { get; set; }
        public IDbSet<SvTrnService> SvTrnServices { get; set; }
        public IDbSet<SvTrnInvoice> SvTrnInvoices { get; set; }
        public IDbSet<SvTrnReturn> SvTrnReturns { get; set; }
        public IDbSet<GnMstSupplierClass> GnMstSupplierClasses { get; set; }
        public IDbSet<glJournal> glJournals { get; set; }
        public IDbSet<GlJournalDtl> GlJournalDtls { get; set; }
        public IDbSet<gnMstAccount> gnMstAccounts { get; set; }
        public IDbSet<SpSelectByNoBinning> SpSelectByNoBinnings { get; set; }
        public IDbSet<Supplier> GnMstSuppliers { get; set; }
        public IDbSet<SpSelectByNoWRS> SpSelectByNoWRSes { get; set; }
        public IDbSet<SpClosePeriodPending> SpClosePeriodPendings { get; set; }
        public IDbSet<GnMstCoProfileSales> GnMstCoProfileSalesmant { get; set; }
        public IDbSet<GnMstCoProfileService> GnMstCoProfileServices { get; set; }
        //public IDbSet<GnMstLookUpDtl> GnMstLookUpDtls { get; set; }
        public IDbSet<SpTrnIAdjustHdr> SpTrnIAdjustHdrs { get; set; }
        public IDbSet<SpTrnIAdjustDtl> SpTrnIAdjustDtls { get; set; }

        public IDbSet<SpTrnSRturHdr> SpTrnsRturHdrs { get; set; }
        public IDbSet<SpTrnSRturDtl> SpTrnsRturDtls { get; set; }
        public IDbSet<GnMstSignature> GnMstSignatures { get; set; }

        public IDbSet<spTrnPSUGGORHdr> spTrnPSUGGORHdrs { get; set; }
        public IDbSet<spTrnPSUGGORDtl> spTrnPSUGGORDtls { get; set; }
        public IDbSet<spTrnPSUGGORSubDtl> spTrnPSUGGORSubDtls { get; set; }
        public IDbSet<GnDcsUploadFile> GnDcsUploadFiles { get; set; }
        public IDbSet<GnDcsDownloadFile> GnDcsDownloadFiles { get; set; }
        public IDbSet<SpUtlPINVDHdr> SpUtlPINVDHdrs { get; set; }
        public IDbSet<SpUtlPINVDDtl> SpUtlPINVDDtls { get; set; }
        public IDbSet<GnMstCoProfile> GnMstCoProfiles { get; set; }
        public IDbSet<SpHstPOrderBalance> SpHstPOrderBalances { get; set; }

        public IDbSet<SpLoadDetail_TranStock> SpLoadDetail_TranStocks { get; set; }
        public IDbSet<GnMstSegmentAcc> GnMstSegmentAccs { get; set; }
        
        public IDbSet<GnMstZipCode> GnMstZipCodes { get; set; }
        public IDbSet<gnMstCollector> GnMstCollectors { get; set; }
        public IDbSet<HrEmployeeView> HrEmployeeViews { get; set; }
        public IDbSet<OmMstRefference> OmMstRefferences { get; set; }
        public IDbSet<GnMstDocument> GnMstDocuments { get; set; }
        public IDbSet<SysMessageBoards> SysMessageBoardss { get; set; }
        public IDbSet<LookUpHdr> LookUpHdrs { get; set; }
        public IDbSet<GnMstFPJSeqNo> GnMstFPJSeqNos { get; set; }
        public IDbSet<CoProfileFinance> GnMstCoProfileFinances { get; set; }
        public IDbSet<spHstBOCancels> spHstBOCancels { get; set; }

        public IDbSet<spUtlPINVDtlView> spUtlPINVDtlViews { get; set; }

        public IDbSet<spTrnPREQHdr> spTrnPREQHdr { get; set; }
        public IDbSet<spTrnPREQDtl> spTrnPREQDtl { get; set; }
        public IDbSet<GnMstDealerMapping> DealerMapping { get; set; }
        public IDbSet<SparepartWeekly> SparepartWeekly { get; set; }
        public IDbSet<spUtlItemSetup> spUtlItemSetups { get; set; }
        public IDbSet<SpTrnPSuggorAOS> SpTrnPSuggorAOSs { get; set; }
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

        public IDbSet<spMstItem> spMstItems { get; set; }
        public IDbSet<SpMstItemLoc> SpMstItemLocs { get; set; }
        public IDbSet<spMstItemPrice> spMstItemPrices { get; set; }
        public IDbSet<MasterItemInfo> MasterItemInfos { get; set; }
        public IDbSet<SpTrnIMovement> SpTrnIMovements { get; set; }
        public IDbSet<SpMstItemInfo> SpMstItemInfos { get; set; }
        public IDbSet<SvSDMovement> SvSDMovements { get; set; }
        public IDbSet<ProfitCenter> ProfitCenters { get; set; }
        public IDbSet<GnMstCustomerDisc> GnMstCustomerDiscs { get; set; }
        public IDbSet<spMstSalesCampaign> spMstSalesCampaigns { get; set; }
        public DbSet<GnMstCoProfileSpare> GnMstCoProfileSpares { get; set; }
        public IDbSet<SpTrnSORDHdr> SpTrnSORDHdrs { get; set; }
        public IDbSet<SpTrnSORDDtl> SpTrnSORDDtls { get; set; }
        public IDbSet<GnMstCustomer> GnMstCustomers { get; set; }
        public IDbSet<CoProfile> CoProfiles { get; set; }
        public IDbSet<ArInterface> ArInterfaces { get; set; }
        public IDbSet<Tax> Taxes { get; set; }
        public IDbSet<Periode> Periodes { get; set; }
        public IDbSet<SpUtlPORDDHdr> SpUtlPORDDHdrs { get; set; }
        public IDbSet<SpUtlPORDDDtl> SpUtlPORDDDtls { get; set; }
        public IDbSet<Employee> Employees { get; set; }
        public IDbSet<SpTrnSOSupply> SpTrnSOSupplys { get; set; }
        public IDbSet<gnMstSignature> gnMstSignatures { get; set; }
    }
}