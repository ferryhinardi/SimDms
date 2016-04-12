using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SimDms.Common.Models;

namespace SimDms.Sales.Controllers
{
    public class DataContext : DbContext
    {
        public DataContext(string ConnString)
            : base(ConnString)
        {
        }

        public DataContext() :
            base("name=DataContext")
        {

        }

        public DateTime CurrentTime
        {
            get
            {
                return this.Database.SqlQuery<DateTime>("select getdate()").FirstOrDefault();
            }
        }

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

        public IDbSet<CoProfileSales> CoProfileSaleses { get; set; }
        public IDbSet<DealerMapping> DealerMappings { get; set; }
        public IDbSet<Employee> Employees { get; set; }
        public IDbSet<PmKdpAdditional> PmKdpAdditionals { get; set; }
        public IDbSet<PmActivity> PmActivites { get; set; }
        public IDbSet<PmBranchOutlet> PmBranchOutlets { get; set; }
        public IDbSet<GroupType> GroupTypes { get; set; }
        public IDbSet<ItsMstModel> ItsMstModels { get; set; }
        public IDbSet<HrEmployeeView> HrEmployeeViews { get; set; }

        public IDbSet<SysUserView> SysUserViews { get; set; }
        public IDbSet<TeamMember> TeamMembers { get; set; }

        public IDbSet<PmKdpClnUpView> PmKdpClnUpViews { get; set; }
        public IDbSet<DealerOutletMapping> DealerOutletMappings { get; set; }
        public IDbSet<GroupModels> GroupModels { get; set; }
        public IDbSet<PmHstITS> PmHstITSs { get; set; }
        public IDbSet<SysUserProfitCenter> SysUserProfitCenters { get; set; }
        public IDbSet<GnMstCoProfileSales> GnMstCoProfileSalesmans { get; set; }
        public IDbSet<GnMstCoProfileService> GnMstCoProfileServices { get; set; }
        public IDbSet<GnMstCoProfileSpare> GnMstCoProfileSpares { get; set; }
        public IDbSet<OmTRSalesSO> OmTRSalesSOs { get; set; }
        public IDbSet<OmTrSalesSOModel> OmTrSalesSOModels { get; set; }
        public IDbSet<OmMstModelColour> OmMstModelColours { get; set; }
        public IDbSet<OmMstPricelistSell> OmMstPricelistSells { get; set; }
        public IDbSet<OmMstModel> OmMstModels { get; set; }
        public IDbSet<OmTrSalesSOModelColour> OmTrSalesSOModelColours { get; set; }
        public IDbSet<omTrSalesSOVin> omTrSalesSOVins { get; set; }
        public IDbSet<OmMstVehicle> OmMstVehicles { get; set; }
        public IDbSet<OmTrSalesSoModelOther> OmTrSalesSoModelOthers { get; set; }
        public IDbSet<OmTrSalesSOAccs> OmTrSalesSOAccses { get; set; }
        public IDbSet<OmTrSalesSOAccsSeq> OmTrSalesSOAccsSeqs { get; set; }
        public IDbSet<SpMstItemPrice> SpMstItemPrices { get; set; }
        public IDbSet<SpMstItem> SpMstItems { get; set; }
        public IDbSet<OmTrSalesDO> OmTrSalesDOs { get; set; }
        public IDbSet<OmTrSalesBPK> OmTrSalesBPKs { get; set; }
        public IDbSet<OmTrSalesBPKDetail> OmTrSalesBPKDetails { get; set; }
        public IDbSet<OmTrSalesDODetail> OmTrSalesDODetails { get; set; }
        public IDbSet<OmTrInventQtyVehicle> OmTrInventQtyVehicles { get; set; }
        public IDbSet<OmTrSalesSOModelAdditional> OmTrSalesSOModelAdditionals { get; set; }
        public IDbSet<GnTrnBankBook> GnTrnBankBooks { get; set; }
        public IDbSet<OmTrSalesBPKModel> OmTrSalesBPKModels { get; set; }
        public IDbSet<OmTrSalesInvoice> OmTrSalesInvoices { get; set; }
        public IDbSet<OmTrPurchasePO> OmTrPurchasePOs { get; set; }
        public IDbSet<OmTrPurchasePOModel> OmTrPurchasePOModels { get; set; }
        public IDbSet<OmMstPricelistBuy> OmMstPricelistBuys { get; set; }
        public IDbSet<OmTrPurchasePOModelColour> OmTrPurchasePOModelColours { get; set; }
        public IDbSet<omTrStockTakingHdr> omTrStockTakingHdrs { get; set; }
        public IDbSet<omTrPurchaseBPU> omTrPurchaseBPUs { get; set; }    

    }
}