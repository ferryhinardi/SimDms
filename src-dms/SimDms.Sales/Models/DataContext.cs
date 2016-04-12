using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SimDms.Common.Models;
using SimDms.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Text;

namespace SimDms.Sales.Models
{
    public partial class DataContext : DbContext
    {

        public DataContext(string ConnString)
            : base(ConnString)
        {
        }

        public DataContext() :
            base(MyHelpers.GetConnString("DataContext"))
        {

        } 

        public DateTime CurrentTime
        {
            get
            {
                return this.Database.SqlQuery<DateTime>("select getdate()").FirstOrDefault();
            }
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
        
        public IDbSet<CompanyMapping> CompanyMappings { get; set; }

        public IDbSet<Period> MstPeriod { get; set; }
        public IDbSet<GnMstZipCode> GnMstZipCode { get; set; }
        public IDbSet<CoProfile> CoProfiles { get; set; }
        public IDbSet<HrEmployee> HrEmployees { get; set; }
        public IDbSet<LookUpDtl> LookUpDtls { get; set; }
        public IDbSet<MstModel> MstModels { get; set; }
        public IDbSet<MstModelColour> MstModelColours { get; set; }
        public IDbSet<MstModelYear> MstModelYear { get; set; }
        public IDbSet<MstRefference> MstRefferences { get; set; }
        public IDbSet<RefferenceTypeView> RefferenceTypeView { get; set; }
        public IDbSet<MstPerlengkapan> MstPerlengkapan { get; set; }
        public IDbSet<MstModelPerlengkapan> MstModelPerlengkapan { get; set; }
        public IDbSet<MstKaroseri> MstKaroseri { get; set; }
        public IDbSet<KaroseriBrowseView> KaroseriBrowseView { get; set; }
        public IDbSet<MstPriceListBeli> MstPriceListBeli { get; set; }
        //public IDbSet<OmMstPricelistJual> OmMstPricelistJual { get; set; }
        public IDbSet<MstBBNKIR> MstBBNKIR { get; set; }
        public IDbSet<MstModelAccount> MstModelAccount { get; set; }
        public IDbSet<MstCompanyAccount> MstCompanyAccount { get; set; }
        public IDbSet<omMstSalesTarget> omMstSalesTarget { get; set; }
        public IDbSet<MstOthersInventory> MstOthersInventory { get; set; }
        public IDbSet<OrganizationHdr> OrganizationHdrs { get; set; }
        public IDbSet<OrganizationDtl> OrganizationDtls { get; set; }
        public IDbSet<PmKdp> PmKdps { get; set; }
        public IDbSet<PmStatusHistory> PmStatusHistories { get; set; }
        public IDbSet<Position> Positions { get; set; }
        public IDbSet<SysUser> SysUsers { get; set; }
        public IDbSet<Supplier> Supplier { get; set; }
        public IDbSet<Tax> Taxs { get; set; }
        public IDbSet<GnMstAccount> GnMstAccounts { get; set; }
        public IDbSet<SupplierProfitCenter> SupplierProfitCenter { get; set; }
        public IDbSet<GnMstDealerMapping> GnMstDealerMapping { get; set; }
        public IDbSet<GnMstApproval> GnMstApprovals { get; set; }

        public IDbSet<CoProfileSales> CoProfileSaleses { get; set; }
        public IDbSet<DealerMapping> DealerMappings { get; set; }
        public IDbSet<Employee> Employees { get; set; }
        public IDbSet<PmKdpAdditional> PmKdpAdditionals { get; set; }
        public IDbSet<PmActivity> PmActivites { get; set; }
        public IDbSet<PmBranchOutlet> PmBranchOutlets { get; set; }
        //public IDbSet<PmMstTeamMembers> PmMstTeamMembers { get; set; } 
        public IDbSet<GroupType> GroupTypes { get; set; }
        public IDbSet<ItsMstModel> ItsMstModels { get; set; }
        public IDbSet<HrEmployeeView> HrEmployeeViews { get; set; }

        public IDbSet<SysUserView> SysUserViews { get; set; }
        public IDbSet<TeamMember> TeamMembers { get; set; }
        public IDbSet<SysMessage> SysMsgs { get; set; }
        public IDbSet<GnMstDocument> GnMstDocuments { get; set; }
        public IDbSet<GnMstSignature> GnMstSignature { get; set; }
        
        public IDbSet<GnMstCustomer> GnMstCustomer { get; set; }
        public IDbSet<CustomerProfitCenter> CustomerProfitCenters { get; set; } 
        public IDbSet<PmKdpClnUpView> PmKdpClnUpViews { get; set; }
        public IDbSet<DealerOutletMapping> DealerOutletMappings { get; set; }
        public IDbSet<GroupModels> GroupModels { get; set; }
        public IDbSet<PmHstITS> PmHstITSs { get; set; }
        public IDbSet<SysUserProfitCenter> SysUserProfitCenters { get; set; }
        public IDbSet<GnMstCoProfileSales> GnMstCoProfileSaleses { get; set; }
        public IDbSet<GnMstCoProfileService> GnMstCoProfileServices { get; set; }
        public IDbSet<GnMstCoProfileSpare> GnMstCoProfileSpares { get; set; }
        public IDbSet<CoProfileFinance> CoProfileFinances { get; set; }
        public IDbSet<OmTRSalesSO> OmTRSalesSOs { get; set; }
        public IDbSet<OmTrSalesSOModel> OmTrSalesSOModels { get; set; }
        public IDbSet<OmMstModelColour> OmMstModelColours { get; set; }
        public IDbSet<OmMstPricelistSell> OmMstPricelistSells { get; set; }
        public IDbSet<OmMstModel> OmMstModels { get; set; }
        public IDbSet<OmTrSalesSOModelColour> OmTrSalesSOModelColours { get; set; }
        public IDbSet<omTrSalesSOVin> omTrSalesSOVins { get; set; }
        public IDbSet<OmMstVehicle> OmMstVehicles { get; set; }
        //public IDbSet<InquiryMstVehicleView> MstVehicleView { get; set; }
        public IDbSet<OmMstVehicleTemp> OmMstVehicleTemps { get; set; }
        public IDbSet<OmMstVehicleHistory> OmMstVehicleHistorys { get; set; }
        public IDbSet<OmTrSalesSoModelOther> OmTrSalesSoModelOthers { get; set; }
        public IDbSet<OmTrSalesSOAccs> OmTrSalesSOAccses { get; set; }
        public IDbSet<OmTrSalesSOAccsSeq> OmTrSalesSOAccsSeqs { get; set; }
        public IDbSet<SpMstItemPrice> SpMstItemPrices { get; set; }
        public IDbSet<SpMstItem> SpMstItems { get; set; }
        public IDbSet<OmTrSalesDO> OmTrSalesDOs { get; set; }
        public IDbSet<OmTrSalesBPK> OmTrSalesBPKs { get; set; }
        public IDbSet<omTrSalesDN> omTrSalesDNs { get; set; }
        public IDbSet<OmTrSalesDNVin> OmTrSalesDNVins { get; set; }        
        public IDbSet<OmTrSalesDNModel> OmTrSalesDNModels { get; set; }
        public IDbSet<TrSalesSOView> TrSalesSOViews { get; set; } 

        public IDbSet<OmTrSalesBPKDetail> OmTrSalesBPKDetails { get; set; }
        public IDbSet<OmTrSalesDODetail> OmTrSalesDODetails { get; set; }
        public IDbSet<OmTrInventQtyVehicle> OmTrInventQtyVehicles { get; set; }
        public IDbSet<OMTrInventQtyPerlengkapan> OMTrInventQtyPerlengkapan { get; set; }
        public IDbSet<OmTrSalesSOModelAdditional> OmTrSalesSOModelAdditionals { get; set; }
        public IDbSet<GnTrnBankBook> GnTrnBankBooks { get; set; }
        public IDbSet<OmTrSalesBPKModel> OmTrSalesBPKModels { get; set; }
        
        public IDbSet<OmTrSalesInvoice> OmTrSalesInvoices { get; set; }
        public IDbSet<omTrSalesInvoiceModel> omTrSalesInvoiceModel { get; set; }
        public IDbSet<omTrSalesInvoiceBPK> omTrSalesInvoiceBPK { get; set; }
        public IDbSet<omTrSalesInvoiceVin> omTrSalesInvoiceVin { get; set; }
        public IDbSet<OmTrSalesInvoiceOthers> omTrSalesInvoiceOthers { get; set; }
        public IDbSet<omTrSalesInvoiceAccs> OmTrSalesInvoiceAccs { get; set; }
        public IDbSet<OmTrSalesInvoiceAccsSeq> OmTrSalesInvoiceAccsSeqs { get; set; } 
       
        public IDbSet<omTrSalesReturn> omTrSalesReturn { get; set; }
        public IDbSet<omTrSalesReturnBPK> omTrSalesReturnBPKs { get; set; } 
        public IDbSet<omTrSalesReturnDetailModel> omTrSalesReturnDetailModel { get; set; }
        public IDbSet<omTrSalesReturnOther> omTrSalesReturnOthers { get; set; }
        public IDbSet<omTrSalesReturnVIN> omTrSalesReturnVIN { get; set; }
        public IDbSet<omTrSalesPerlengkapanOut> omTrSalesPerlengkapanOuts { get; set; }
        public IDbSet<OmTrSalesPerlengkapanOutModel> OmTrSalesPerlengkapanOutModels { get; set; }
        public IDbSet<OmTrSalesPerlengkapanOutDetail> OmTrSalesPerlengkapanOutDetails { get; set; }
        public IDbSet<omTrSalesReq> omTrSalesReq { get; set; }
        public IDbSet<omTrSalesReqDetail> omTrSalesReqDetail { get; set; }
        public IDbSet<omTrSalesReqDetailHist> omTrSalesReqDetailHist { get; set; }
        public IDbSet<omTrSalesReqDetailAdditional> omTrSalesReqDetailAdditionals { get; set; }
        public IDbSet<omTrSalesFPolRevision> omTrSalesFPolRevision { get; set; }
        public IDbSet<omTrSalesFPolRevisionDetail> omTrSalesFPolRevisionDetail { get; set; }
        public IDbSet<omTrSalesFPolRevisionHistory> omTrSalesFPolRevisionHistory { get; set; }
        public IDbSet<omTrSalesSPK> omTrSalesSPK { get; set; }
        public IDbSet<omTrSalesSPKDetail> omTrSalesSPKDetail { get; set; }
        public IDbSet<omTrSalesSPKSubDetail> omTrSalesSPKSubDetail { get; set; } 
        public IDbSet<omTrSalesBPKB> omTrSalesBPKB { get; set; }
        public IDbSet<omTrSalesBPKBDetail> omTrSalesBPKBDetail { get; set; }
        public IDbSet<omTrSalesSTNK> omTrSalesSTNK { get; set; }
        public IDbSet<omTrSalesSTNKDetail> omTrSalesSTNKDetail { get; set; }
        public IDbSet<OmTrSalesReceipt> OmTrSalesReceipt { get; set; }


        public IDbSet<OmTrPurchasePO> OmTrPurchasePOs { get; set; }
        public IDbSet<OmTrPurchasePOModel> OmTrPurchasePOModels { get; set; }
        public IDbSet<OmMstPricelistBuy> OmMstPricelistBuys { get; set; }
        public IDbSet<OmTrPurchasePOModelColour> OmTrPurchasePOModelColours { get; set; }
        public IDbSet<omTrStockTakingHdr> omTrStockTakingHdrs { get; set; }
        public IDbSet<omTrStockTakingDtl> omTrStockTakingDtls { get; set; }
        public IDbSet<omTrPurchaseBPU> omTrPurchaseBPU { get; set; }
        public IDbSet<OmTrPurchaseBPULookupView> OmTrPurchaseBPULookupView { get; set; }
        public IDbSet<omTrPurchaseBPUDetail> omTrPurchaseBPUDetail { get; set; }
        public IDbSet<OmTrPurchaseBPUDetailView> OmTrPurchaseBPUDetailViews { get; set; }
        public IDbSet<OmTrPurchaseBPUAttribute> omTrPurchaseBPUAttributes { get; set; }  
        public IDbSet<omTrPurchaseHPP> omTrPurchaseHPP { get; set; }
        public IDbSet<omTrPurchaseHPPDetail> omTrPurchaseHPPDetail { get; set; }
        public IDbSet<omTrPurchaseHPPDetailModel> omTrPurchaseHPPDetailModel { get; set; }
        public IDbSet<omTrPurchaseHPPSubDetail> omTrPurchaseHPPSubDetail { get; set; }
        public IDbSet<omTrPurchaseHPPDetailModelOthers> omTrPurchaseHPPDetailModelOthers { get; set; }
        public IDbSet<omTrPurchaseReturn> omTrPurchaseReturn { get; set; }
        public IDbSet<omTrPurchaseReturnDetail> omTrPurchaseReturnDetail { get; set; }
        public IDbSet<omTrPurchaseReturnDetailModel> omTrPurchaseReturnDetailModel { get; set; }
        public IDbSet<omTrPurchaseReturnSubDetail> omTrPurchaseReturnSubDetail { get; set; }
        public IDbSet<omTrPurchasePerlengkapanIn> omTrPurchasePerlengkapanIn { get; set; }
        public IDbSet<omTrPurchasePerlengkapanInDetail> omTrPurchasePerlengkapanInDetail { get; set; }
        public IDbSet<omTrPurchasePerlengkapanAdjustment> omTrPurchasePerlengkapanAdjustment { get; set; }
        public IDbSet<omTrPurchasePerlengkapanAdjustmentDetail> omTrPurchasePerlengkapanAdjustmentDetail { get; set; }
        public IDbSet<omTrPurchaseKaroseri> omTrPurchaseKaroseri { get; set; }
        public IDbSet<omTrPurchaseKaroseriDetail> omTrPurchaseKaroseriDetail { get; set; }
        public IDbSet<omTrPurchaseKaroseriTerima> omTrPurchaseKaroseriTerima { get; set; }
        public IDbSet<omTrPurchaseKaroseriTerimaDetail> omTrPurchaseKaroseriTerimaDetail { get; set; }
        public IDbSet<OmUtlSPORDHdr> OmUtlSPORDHdrs { get; set; }
        public IDbSet<OmUtlSPORDDtl1> OmUtlSPORDDtl1s { get; set; }
        public IDbSet<OmUtlSPORDDtl2> OmUtlSPORDDtl2s { get; set; }
        public IDbSet<OmUtlSPORDDtl3> OmUtlSPORDDtl3s { get; set; }
        public IDbSet<OmTrInventTransferOutMultiBranch> OmTrInventTransferOutMultiBranch{ get; set; }
        public IDbSet<omTrInventTransferOutDetailMultiBranch> omTrInventTransferOutDetailMultiBranch{ get; set; }
        public IDbSet<OmTrInventTransferInMultiBranch> OmTrInventTransferInMultiBranch{ get; set; }
        public IDbSet<omTrInventTransferInDetailMultiBranch> omTrInventTransferInDetailMultiBranch{ get; set; }
        public IDbSet<OmTrInventTransferOut> OmTrInventTransferOut { get; set; }
        public IDbSet<omTrInventTransferOutDetail> omTrInventTransferOutDetail { get; set; }
        public IDbSet<omTrInventTransferIn> omTrInventTransferIn { get; set; }
        public IDbSet<omTrInventTransferInDetail> omTrInventTransferInDetail { get; set; }
        public IDbSet<OmTrInventColorChange> OmTrInventColorChange { get; set; }
        public IDbSet<OmTrInventColorChangeDetail> OmTrInventColorChangeDetail { get; set; }
        public IDbSet<OmUtlSACCSHdr> OmUtlSACCSHdrs { get; set; } 
        public IDbSet<OmUtlSACCSDtl1> OmUtlSACCSDtl1s { get; set; }
        public IDbSet<OmUtlSACCSDtl2> OmUtlSACCSDtl2s { get; set; }
        public IDbSet<OmUtlSDORDHdr> OmUtlSDORDHdrs { get; set; }
        public IDbSet<OmUtlSDORDDtl1> OmUtlSDORDDtl1s { get; set; }
        public IDbSet<OmUtlSDORDDtl2> OmUtlSDORDDtl2s { get; set; }
        public IDbSet<OmUtlSDORDDtl3> OmUtlSDORDDtl3s { get; set; }
        public IDbSet<OmUtlSSJALHdr> OmUtlSSJALHdrs { get; set; }
        public IDbSet<OmUtlSSJALDtl1> OmUtlSSJALDtl1s { get; set; }       
        public IDbSet<OmUtlSSJALDtl2> OmUtlSSJALDtl2s { get; set; }
        public IDbSet<OmUtlSSJALDtl3> OmUtlSSJALDtl3s { get; set; }
        public IDbSet<OmUtlSHPOKHdr> OmUtlSHPOKHdrs { get; set; }
        public IDbSet<OmUtlSHPOKDtl1> OmUtlSHPOKDtl1s { get; set; }
        public IDbSet<OmUtlSHPOKDtl2> OmUtlSHPOKDtl2s { get; set; }
        public IDbSet<OmUtlSHPOKDtl3> OmUtlSHPOKDtl3s { get; set; }
        public IDbSet<OmUtlSHPOKDtl4> OmUtlSHPOKDtl4s { get; set; }
        public IDbSet<OmUtlSHPOKDtlO> OmUtlSHPOKDtlOs { get; set; }
        public IDbSet<OmUtlSFPOLHdr> OmUtlSFPOLHdrs { get; set; }
        public IDbSet<OmUtlSFPOLDtl1> OmUtlSFPOLDtl1s { get; set; }
        public IDbSet<omUtlFpolReq> omUtlFpolReqs { get; set; }
        public IDbSet<OmUtlFPolReqDetail> OmUtlFPolReqDetails { get; set; }
        public IDbSet<OmUtlFPolReqDetailAdditional> OmUtlFPolReqDetailAdditionals { get; set; }

        public IDbSet<OmTrSalesDraftSO> OmTrSalesDraftSOs { get; set; }
        public IDbSet<OmTrSalesDraftSOAccs> OmTrSalesDraftSOAccs { get; set; }
        public IDbSet<OmTrSalesDraftSOModel> OmTrSalesDraftSOModels { get; set; }
        public IDbSet<OmTrSalesDraftSOModelColour> OmTrSalesDraftSOModelColours { get; set; }
        public IDbSet<OmTrSalesDraftSOModelOthers> OmTrSalesDraftSOModelOthers { get; set; }
        public IDbSet<OmTrSalesDraftSOVin> OmTrSalesDraftSOVins { get; set; }
        public IDbSet<OmTrSalesDraftSOLookupView> OmTrSalesDraftSOLookupViews { get; set; }
        //public IDbSet<ITSNoDraftSO> ITSNoDraftSOs { get; set; }
        public IDbSet<ITSNoDraftSO_2> ITSNoDraftSOs { get; set; }
        public IDbSet<ITSNoDraftSO_4> ITSNoDraftSOs4 { get; set; }
        public IDbSet<OmTrPurchaseBPUDetailModel> OmTrPurchaseBPUDetailModels { get; set; }
        public IDbSet<OmTrSalesFakturPolisi> OmTrSalesFakturPolisi { get; set; }
        public IDbSet<OmMstIndentMapping> OmMstIndentMappings { get; set; }
        public IDbSet<ApUtlInvoiceOthersHdr> ApUtlInvoiceOthersHdrs { get; set; }

        public IDbSet<omFakturPajakHdr> omFakturPajakHdrs { get; set; }
        public IDbSet<omFakturPolda> omFakturPoldas { get; set; }

        public IDbSet<GnDcsUploadFile> GnDcsUploadFiles { get; set; }
        public IDbSet<SysFlatFileHdr> SysFlatFileHdrs { get; set; }
        public IDbSet<SysFlatFileDtl> SysFlatFileDtls { get; set; }

        public IDbSet<omTrInventWH> omTrInventWH { get; set; }
        public IDbSet<omTrInventWHHistory> omTrInventWHHistory { get; set; }
        public IDbSet<omTrInventMovement> omTrInventMovement { get; set; }

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

        public IDbSet<omSDMovement> omSDMovements { get; set; }
        public IDbSet<OmMstVehicle> OmMstVehicles { get; set; }
        public IDbSet<OmTrInventQtyVehicle> OmTrInventQtyVehicles { get; set; }
        public IDbSet<OmTrSalesInvoice> omTrSalesInvoices { get; set; }
        public IDbSet<omTrSalesInvoiceModel> omTrSalesInvoiceModels { get; set; }
        public IDbSet<omTrSalesInvoiceVin> omTrSalesInvoiceVins { get; set; }
        public IDbSet<GnMstCustomer> gnMstCustomers { get; set; }
        public IDbSet<OmTRSalesSO> omTrSalesSOs { get; set; }
        public IDbSet<OmTrSalesBPK> omTrSalesBPKs { get; set; }
        public IDbSet<OmTrSalesDO> omTrSalesDOs { get; set; }
        public IDbSet<MstRefference> MstRefferences { get; set; }
        public IDbSet<LookUpDtl> LookUpDtls { get; set; }
        public IDbSet<MstModel> MstModels { get; set; }
        public IDbSet<OmMstPricelistBuy> OmMstPricelistBuys { get; set; }
        public IDbSet<OrganizationDtl> OrganizationDtls { get; set; }
    }
}