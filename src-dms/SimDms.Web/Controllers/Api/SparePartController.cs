using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Breeze.WebApi;
using Newtonsoft.Json.Linq;
using SimDms.Common.Models;
using SimDms.Sparepart.Models;
using Breeze.WebApi.EF;
using SimDms.Common;

namespace SimDms.Web.Controllers.Api
{
    [BreezeController]
    public class SparePartController : ApiController
    {

        readonly EFContextProvider<DataContext> ctx = new EFContextProvider<DataContext>();

        [HttpGet]
        public String Metadata()
        {
            return ctx.Metadata();
        }

        [HttpGet]
        public SysUser CurrentUser()
        {
            var user = ctx.Context.SysUsers.Find(User.Identity.Name);
            user.CoProfile = ctx.Context.CoProfiles.Find(user.CompanyCode, user.BranchCode);
            return user;
        }

        public string GetLookupValueName(string CompanyCode, string VarGroup, string varCode)
        {
            string s = "";
            var x = ctx.Context.LookUpDtls.Find(CompanyCode, VarGroup, varCode);
            if (x != null) s = x.LookUpValueName;
            return s;
        }

        [HttpGet]
        public MyUserInfo CurrentUserInfo()
        {
            string s = "";
            var f = ctx.Context.SysUserProfitCenters.Find(User.Identity.Name);
            if (f != null) s = f.ProfitCenter;
            var u = ctx.Context.SysUsers.Find(User.Identity.Name);
            var g = ctx.Context.CoProfiles.Find(u.CompanyCode, u.BranchCode);
            u.CoProfile = ctx.Context.CoProfiles.Find(u.CompanyCode, u.BranchCode);
            var uv = ctx.Context.SysUserViews.Find(User.Identity.Name);

            var info = new MyUserInfo
            {
                UserId = u.UserId,
                FullName = u.FullName,
                CompanyCode = u.CompanyCode,
                CompanyGovName = g.CompanyGovName,
                BranchCode = u.BranchCode,
                CompanyName = g.CompanyName,
                TypeOfGoods = u.TypeOfGoods,
                ProductType = g.ProductType,
                ProfitCenter = s,
                TypeOfGoodsName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.TypeOfGoods, u.TypeOfGoods),
                ProductTypeName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProductType, g.ProductType),
                ProfitCenterName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProfitCenter, s),
                IsActive = u.IsActive,
                RequiredChange = u.RequiredChange,
                SimDmsVersion = GetType().Assembly.GetName().Version.ToString(),
                ShowHideTypePart = (uv.RoleId.Equals("Admin", StringComparison.InvariantCultureIgnoreCase) || f.ProfitCenter == "300") ? GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.TypeOfGoods, u.TypeOfGoods) : ""
            };

            return info;
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return ctx.SaveChanges(saveBundle);
        }

        [HttpGet]
        public IQueryable<spMstAccount> MasterAccount()
        {
            return ctx.Context.spMstAccounts;
        }

        [HttpGet]
        public IQueryable<spMasterPartLookup> PartLookup()
        {
            return ctx.Context.spMasterPartLookups;
        }

        [HttpGet]
        public IQueryable<spMstMovingCode> MovingCode()
        {
            return ctx.Context.spMstMovingCodes;
        }

        [HttpGet]
        public IQueryable<spMstOrderParamView> ParamCode()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<spMstOrderParamView>(" sp_spMstOrderParamView '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<gnMstSupplierView> Suppliers()
        {
            var uid = CurrentUser();
            var queryable = ctx.Context.gnMstSupplierViews.Where(x => x.CompanyCode == uid.CompanyCode);
            return queryable;
        }

        [HttpGet]
        public IQueryable<GnMstCoProfile> Branches()
        {
            var uid = CurrentUser();
            var queryable = ctx.Context.GnMstCoProfiles.Where(x => x.CompanyCode == uid.CompanyCode);
            return queryable;
        }

        [HttpGet]
        public IQueryable<sp_spMstCompanyAccount> CompanyaccBrowse()
        {

            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<sp_spMstCompanyAccount>(" uspfn_spMstCompanyAccount '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<spgnMstAccountViewAccX> GLAccBrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<spgnMstAccountViewAccX>(" uspfn_spgnMstAccountView '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<LookUpCompanyView> ShowCompanyLookup()
        {

            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<LookUpCompanyView>("SELECT CompanyCode ,CompanyName   FROM gnMstOrganizationHdr").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<LookUpBranchView> ShowBranchLookup()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<LookUpBranchView>("SELECT CompanyCode ,BranchCode,CompanyName   FROM gnMstCoProfile  where CompanyCode='" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<LookUpDtlView> ShowWarehouseLookup()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<LookUpDtlView>("select CompanyCode,CodeID,LookUpValue,LookUpValueName from gnMstLookUpDtl where CompanyCode='" + Uid.CompanyCode + "' and CodeID='WRCD'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<spMstAccountView> GLAccMappingBrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<spMstAccountView>("sp_spMstAccountView '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<spMstSalesTargetview> TargetPenjualanBrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<spMstSalesTargetview>("sp_spMstSalesTargetview '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<sp_spCategoryCodeview> CategoryCodebrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<sp_spCategoryCodeview>("sp_spCategoryCodeview '" + Uid.CompanyCode + "'").AsQueryable();

            return (queryable);



        }

        [HttpGet]
        public IQueryable<SpMstItemConversionview> Itemkonversibrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SpMstItemConversionview>("sp_SpMstItemConversionview '" + Uid.CompanyCode + "'").AsQueryable();

            return (queryable);

        }


        [HttpGet]
        public IQueryable<SpMasterPartView> MasterPartView()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SpMasterPartView>("sp_SpMasterPartView '" + Uid.CompanyCode + "', '" + Uid.BranchCode + "'").AsQueryable();

            return (queryable);

        }
       
        [HttpGet]
        public IQueryable<spMstSalesCampaignView> mastersalescampaignbrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<spMstSalesCampaignView>("sp_spMstSalesCampaignView '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }
        [HttpGet]
        public IQueryable<spMstPurchCampaignView> masterpurchcampaignbrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<spMstPurchCampaignView>("sp_spMstPurchCampaignView '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<SpMstItemlocView> MasterItemLocationBrowse()
        {
            var Uid = CurrentUser();
            var g = ctx.Context.CoProfiles.Find(Uid.CompanyCode, Uid.BranchCode);
            var queryable = ctx.Context.Database.SqlQuery<SpMstItemlocView>("uspfn_SpMstItemLocView '" + Uid.CompanyCode + "','" + Uid.BranchCode + "','" + Uid.TypeOfGoods + "','" + g.ProductType + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpMasterItemView> MasterItemBrowse()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SpMasterItemView>("sp_SpMasteritemview '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpItemPriceView> ItemPriceBrowse()
        {

            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SpItemPriceView>("sp_SpItemPriceView '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<SpMstItemconditionView> ItemPartModifBrowse()
        {

            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SpMstItemconditionView>("sp_SpMstItemcondition '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<SpMstItemModifInfo> ItemPartLookup()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SpMstItemModifInfo>("sp_SpMstItemModifInfo '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);


        }
        [HttpGet]
        public IQueryable<spMstMovingCodeView> MovingCodeLookup()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<spMstMovingCodeView>("sp_spMstMovingCodeView '" + Uid.CompanyCode + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<supplierLookUp> SupplierLookup()
        {

            var Uid = CurrentUserInfo();
            var queryable = ctx.Context.Database.SqlQuery<supplierLookUp>("uspfn_spSuppliers '" + Uid.CompanyCode + "','" + Uid.BranchCode + "','" + Uid.ProfitCenter + "'").AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SuggorLookup> SuggorLookup()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SuggorLookup>("uspfn_spTrnPSUGGORHdr '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);

        }
        [HttpGet]
        public IQueryable<OrderSparepartview> OrderSparepartLookup()
        {
            var Uid = CurrentUserInfo();
            var query = string.Format("exec uspfn_SpOrderSparepartView '{0}', '{1}', '{2}', '{3}' ", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods, Uid.ProductType);
            var queryable = ctx.Context.Database.SqlQuery<OrderSparepartview>(query).AsQueryable();
            //var queryable = ctx.Context.Database.SqlQuery<OrderSparepartview>("uspfn_SpOrderSparepartView '" + Uid.CompanyCode + "','" + Uid.BranchCode + "'").AsQueryable();
            return (queryable);

        }
        [HttpGet]
        public IQueryable<sp_spMstSalesTargetDtlview> SalesTargetDetail()
        {
            var uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<sp_spMstSalesTargetDtlview>("sp_spMstSalesTargetDetil '" + uid.CompanyCode + "','" + uid.BranchCode + "'").AsQueryable();
            return queryable;
        }


        [HttpGet]
        public IQueryable<Posview> PosLookup()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<Posview>("uspfn_SpposView '" + Uid.CompanyCode + "','" + Uid.BranchCode + "', '" + Uid.TypeOfGoods + "'").AsQueryable();

            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpTrnPPOSHdrView> PosHdrLookup()
        {
            var Uid = CurrentUser();
            var queryable = ctx.Context.Database.SqlQuery<SpTrnPPOSHdrView>("sp_SpPosLkp '" + Uid.CompanyCode + "','" + Uid.BranchCode + "', '" + Uid.TypeOfGoods + "'").AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<orderPartview> OrderPartLookup(string PosNo)
        {
            var Uid = CurrentUser();
            //string posNo = Request["PosNo"] ?? "";
            var queryable = ctx.Context.Database.SqlQuery<orderPartview>("uspfn_SppartView '" + Uid.CompanyCode + "','" + Uid.BranchCode + "','" + PosNo + "'").AsQueryable();
            return (queryable);

        }
        [HttpGet]
        public IQueryable<orderPartviewByPart> OrderPartLookupByPart(string PartNo)
        {
            var Uid = CurrentUser();
            //string posNo = Request["PosNo"] ?? "";
            var queryable = ctx.Context.Database.SqlQuery<orderPartviewByPart>(" SELECT ID 'PartNo',spMstItemInfo.PartName 'PartName'  FROM GetSUGGORModifikasi  ('" + PartNo + "') inner join  spMstItemInfo on GetSUGGORModifikasi.ID=spMstItemInfo.PartNo where GetSUGGORModifikasi.ID<>'" + PartNo + "'").AsQueryable();
            return (queryable);

        }
        [HttpGet]
        public IQueryable<PickingList> PickingList()
        {
            var Uid = CurrentUserInfo();
            string sql = string.Format("exec uspfn_spGetPickingList '{0}', '{1}', '{2}', '{3}', '{4}'", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods, Uid.ProductType, Uid.ProfitCenter);
            var queryable = ctx.Context.Database.SqlQuery<PickingList>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<FPJLookup> FPJLookup()
        {
            var Uid = CurrentUser();
            string sql = string.Format("exec uspfn_spGetFPJLookUp '{0}', '{1}', '{2}', '{3}'", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods, "0");
            var queryable = ctx.Context.Database.SqlQuery<FPJLookup>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<LookupNonPenjualan> LookupLMP4Srv(string SalesType)
        {
            var Uid = CurrentUser();
            var parameters = SalesType.Split('?');
            string sql = string.Format("exec uspfn_spGetLookupLMP4Srv '{0}', '{1}', '{2}', '{3}', '{4}'", Uid.CompanyCode, Uid.BranchCode, parameters[0], Uid.TypeOfGoods, Uid.CoProfile.ProductType);
            var queryable = ctx.Context.Database.SqlQuery<LookupNonPenjualan>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<LookupNonPenjualan> LookupLMP(string SalesType)
        {
            var Uid = CurrentUser();
            var parameters = SalesType.Split('?');
            var transType = parameters[0];
            string CodeID = "";
            switch (transType)
            {
                case "1":
                    CodeID = "TTNP";
                    break;
                case "2":
                    CodeID = "TTSR";
                    break;
                case "3":
                    CodeID = "TTSL";
                    break;
                default:
                    break;
            }
            string sql = string.Format("exec uspfn_GetLookupLMP '{0}', '{1}', '{2}', '{3}', '{4}','{5}'", Uid.CompanyCode, Uid.BranchCode, transType, CodeID, Uid.TypeOfGoods, Uid.CoProfile.ProductType);
            var queryable = ctx.Context.Database.SqlQuery<LookupNonPenjualan>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<GetLMPHdr> GetLampiranDokumen(string SalesType)
        {
            var Uid = CurrentUser();
            var parameters = SalesType.Split('?');
            var transType = parameters[0];
            string CodeID = "";
            switch (transType)
            {
                case "1":
                    CodeID = "TTNP";
                    break;
                case "2":
                    CodeID = "TTSR";
                    break;
                case "3":
                    CodeID = "TTSL";
                    break;
                default:
                    break;
            }
            transType += "%";
            var coProfileSpare = ctx.Context.GnMstCoProfileSpares.Find(Uid.CompanyCode, Uid.BranchCode);
            string sql = string.Format("exec uspfn_GetLmpDoc '{0}', '{1}', '{2}', '{3}', '{4}', '{5}','{6}'", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods, transType, CodeID, coProfileSpare.PeriodBeg.Date.ToString("yyyy/MM/dd"), coProfileSpare.PeriodEnd.Date.ToString("yyyy/MM/dd"));
            var queryable = ctx.Context.Database.SqlQuery<GetLMPHdr>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<FPJLookup> GetSpTrnSFPJHdr()
        {
            var Uid = CurrentUserInfo();
            string sql = string.Format("exec uspfn_GetSpTrnSFPJHdr '{0}', '{1}'", Uid.CompanyCode, Uid.BranchCode);
            var queryable = ctx.Context.Database.SqlQuery<FPJLookup>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<CustomerDetailsTagih> GetCustomer()
        {
            var Uid = CurrentUserInfo();
            string sql = string.Format("exec uspfn_GetCustomerByProfitCenterCodeId '{0}', '{1}', '{2}'", Uid.CompanyCode, Uid.BranchCode, Uid.ProfitCenter);
            var queryable = ctx.Context.Database.SqlQuery<CustomerDetailsTagih>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<MasterItemBrowse> SparePartLookup()
        {
            var Uid = CurrentUserInfo();
            var sql = string.Format("uspfn_spMasterPartLookup '{0}', '{1}', '{2}', '{3}'", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods, Uid.ProductType);
            var sqlexec = ctx.Context.Database.SqlQuery<MasterItemBrowse>(sql).AsQueryable();
            return sqlexec;
        }

        [HttpGet]
        public IQueryable<MasterItemBrowse> SparePartLookupNew()
        {
            var Uid = CurrentUserInfo();
            var sql = string.Format("uspfn_spMasterPartLookupNew '{0}', '{1}', '{2}'", Uid.CompanyCode, Uid.TypeOfGoods, Uid.ProductType);
            var sqlexec = ctx.Context.Database.SqlQuery<MasterItemBrowse>(sql).AsQueryable();
            return sqlexec;
        }

        [HttpGet]
        public IQueryable<SparePartLocationLookup> SparePartLocationLookup()
        {
            var Uid = CurrentUserInfo();
            var sql = string.Format("uspfn_spMasterPartLocationLookup '{0}', '{1}', '{2}', '{3}'", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods, Uid.ProductType);
            var sqlexec = ctx.Context.Database.SqlQuery<SparePartLocationLookup>(sql).AsQueryable();
            return sqlexec;
        }

        [HttpGet]
        public IQueryable<MasterModelBrowse> ModelLookup()
        {
            var sql = string.Format("uspfn_spModelGridLookup '{0}', '{1}'", CurrentUser().CompanyCode, GnMstLookUpHdr.ModelVehicle);
            return ctx.Context.Database.SqlQuery<MasterModelBrowse>(sql).AsQueryable();
        }

        [HttpGet]
        public IQueryable<GetLMPHdr> ReturLampiranLookup()
        {
            var Uid = CurrentUser();
            var sql = string.Format("exec uspfn_spGetNoLampiranReturSupplySlip '{0}','{1}','{2}','{3}'", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods, Uid.CoProfile.ProductType);
            return ctx.Context.Database.SqlQuery<GetLMPHdr>(sql).AsQueryable();
        }

        [HttpGet]
        public IQueryable<EntryReturnSupplySlipModel> GetReturnSupplySlipBrowse()
        {
            var Uid = CurrentUser();
            var sql = string.Format("exec uspfn_spGetReturnSupplySlip '{0}','{1}','{2}'", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods);
            return ctx.Context.Database.SqlQuery<EntryReturnSupplySlipModel>(sql).AsQueryable();
        }

        [HttpGet]
        public IQueryable<ReturnSSDetails> GetNoPartReturnSupplySlip(string DocNo)
        {
            var Uid = CurrentUser();
            var parameters = DocNo.Split('?');
            var docNo = parameters[0];
            var sql = string.Format("exec uspfn_spGetNoPartsReturnSS '{0}','{1}','{2}','{3}','{4}'", Uid.CompanyCode, Uid.BranchCode, Uid.CoProfile.ProductType, Uid.TypeOfGoods, docNo);
            return ctx.Context.Database.SqlQuery<ReturnSSDetails>(sql).AsQueryable();
        }


        [HttpGet]
        public Object eXecSQL(string SQL)
        {
            return ctx.Context.Database.SqlQuery<Object>(SQL).FirstOrDefault();
        }

        [HttpGet]
        public IQueryable<PartInquiry> SparePartInquiry()
        {
            var Uid = CurrentUserInfo();
            var sql = string.Format("uspfn_sp_partinquiry '{0}', '{1}', '{2}', '{3}'", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods, Uid.ProductType);
            return ctx.Context.Database.SqlQuery<PartInquiry>(sql).AsQueryable();
        }

        [HttpGet]
        public IQueryable<PickingListBrowse> PickingListBrowse()
        {
            string isBORelease = "";
            var uid = CurrentUserInfo();
            string sql = string.Format("exec uspfn_GetTrnPickingHdr '{0}','{1}','{2}',{3}", uid.CompanyCode, uid.BranchCode, uid.TypeOfGoods, isBORelease);
            var queryable = ctx.Context.Database.SqlQuery<PickingListBrowse>(sql).AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<PickingListHdrLkp> PickingListHdrBrowse()
        {
            var uid = CurrentUserInfo();
            var queryable = ctx.Context.SpTrnSPickingHdrs
                           .Where(x => x.CompanyCode == uid.CompanyCode &&
                                     x.BranchCode == uid.BranchCode &&
                                     x.TypeOfGoods == uid.TypeOfGoods)
                           .Select(y => new PickingListHdrLkp()
                                           {
                                               PickingSlipNo = y.PickingSlipNo,
                                               PickingSlipDate = y.PickingSlipDate,
                                               Remark = y.Remark
                                           })
                           .AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpTrnSRturSSHdrLkp> SpTrnSRturSSHdrLkp()
        {
            string isBORelease = "";
            var uid = CurrentUserInfo();
            var queryable = ctx.Context.SpTrnSRturSSHdrs
                            .Where(x => x.CompanyCode == uid.CompanyCode &&
                                        x.BranchCode == uid.BranchCode &&
                                        x.TypeOfGoods == uid.TypeOfGoods)
                           .Select(x => new SpTrnSRturSSHdrLkp() { ReturnNo = x.ReturnNo, ReturnDate = x.ReturnDate });
            return (queryable);
        }

        [HttpGet]
        public IQueryable<CustSOPickingList> CustPickingListLookupNewOrder()
        {
            var uid = CurrentUserInfo();
            string sql = string.Format("exec uspfn_spCustSOPickListNewOrder '{0}','{1}','{2}','{3}'", uid.CompanyCode, uid.BranchCode, "300", uid.TypeOfGoods);
            var queryable = ctx.Context.Database.SqlQuery<CustSOPickingList>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<CustSOPickingList> CustPickingListLookupBackOrder()
        {
            var uid = CurrentUserInfo();
            string sql = string.Format("exec uspfn_spCustSOPickListBackOrder '{0}','{1}','{2}','{3}'", uid.CompanyCode, uid.BranchCode, "300", uid.TypeOfGoods);
            var queryable = ctx.Context.Database.SqlQuery<CustSOPickingList>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<Employee> EmployeePickedByLookup()
        {

            var uid = CurrentUserInfo();
            var queryable = ctx.Context.Employees.Where(m => m.CompanyCode == uid.CompanyCode && m.BranchCode == uid.BranchCode && m.PersonnelStatus == "1" && m.TitleCode == "7").AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<SpTrnSORDHdrLkp> SORDHdrLkp(string salestype)
        {
            var uid = CurrentUserInfo();
            var queryable = ctx.Context.SpTrnSORDHdrs
                        .Where(m => m.CompanyCode == uid.CompanyCode &&
                                    m.BranchCode == uid.BranchCode &&
                                    m.SalesType == salestype &&
                                    m.TypeOfGoods == uid.TypeOfGoods)
                        .Select(x => new SpTrnSORDHdrLkp { DocNo = x.DocNo, DocDate = x.DocDate })
                        .AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<SpTrnSORDHdrLkp> SPkNoLstOutStandingLkp()
        {
            var uid = CurrentUserInfo();
            var queryable = ctx.Context.SpTrnSORDHdrs
                        .Where(m => m.CompanyCode == uid.CompanyCode &&
                                    m.BranchCode == uid.BranchCode &&
                                    m.SalesType == "2" &&
                                    m.UsageDocNo != "" &&
                                    m.TypeOfGoods == uid.TypeOfGoods)
                        .Select(x => new SpTrnSORDHdrLkp { DocNo = x.DocNo, DocDate = x.DocDate, UsageDocNo = x.UsageDocNo, UsageDocDate = x.UsageDocDate })
                        .AsQueryable();
            return (queryable);
        }




        [HttpGet]
        public IQueryable<spTrnSFPJHdrLKP> FakturLkp()
        {
            var uid = CurrentUserInfo();
            var queryable = ctx.Context.SpTrnSFPJHdrs
                        .Where(m => m.CompanyCode == uid.CompanyCode &&
                                    m.BranchCode == uid.BranchCode &&
                                    m.TypeOfGoods == uid.TypeOfGoods)
                        .Select(x => new spTrnSFPJHdrLKP
                        {
                            FPJNo = x.FPJNo,
                            FPJDate = x.FPJDate,
                            PickingSlipNo = x.PickingSlipNo,
                            PickingSlipDate = x.PickingSlipDate,
                            InvoiceNo = x.InvoiceNo,
                            InvoiceDate = x.InvoiceDate
                        })
                        .AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SignatureLkp> SignatureLkp(string doctype)
        {
            var uid = CurrentUserInfo();
            var queryable = ctx.Context.GnMstSignatures
                        .Where(m => m.CompanyCode == uid.CompanyCode &&
                                    m.BranchCode == uid.BranchCode &&
                                    m.ProfitCenterCode == uid.ProfitCenter &&
                                    m.DocumentType == doctype)
                        .Select(x => new SignatureLkp
                        {
                            SeqNo = x.SeqNo,
                            SignName = x.SignName,
                            TitleSign = x.TitleSign
                        })
                        .AsQueryable();
            return (queryable);
        }


        [HttpGet]
        public IQueryable<LMPHdrLKP> SpSrvLampLkp(string salestype)
        {
            var uid = CurrentUser();
            var sql = string.Format("exec sp_SpSrvLampLkp '{0}','{1}',{2}", uid.CompanyCode, uid.BranchCode, salestype, 1);
            var queryable = ctx.Context.Database.SqlQuery<LMPHdrLKP>(sql).AsQueryable();
            return (queryable);
        }
        [HttpGet]
        public IQueryable<LMPHdrLKP> SpRptLampLkp(string salestype)
        {
            var uid = CurrentUser();
            var queryable = ctx.Context.SpTrnSLmpHdrs.Where(x => x.CompanyCode == uid.CompanyCode && x.BranchCode == uid.BranchCode && x.TypeOfGoods == uid.TypeOfGoods)
                     .Join(ctx.Context.SpTrnSBPSFHdrs, a => new { a.BPSFNo }, b => new { b.BPSFNo }, (a, b) => new { a, b })
                     .Where(y => y.b.SalesType == salestype)
                     .Select(y => new LMPHdrLKP()
                     {
                         LmpNo = y.a.LmpNo,
                         LmpDate = y.a.LmpDate,
                         BPSFNo = y.b.BPSFNo,
                         BPSFDate = y.b.BPSFDate,
                         PickingSlipNo = y.a.PickingSlipNo,
                         PickingSlipDate = y.a.PickingSlipDate
                     })
                     .AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpTrnSRturHdrView> SpRptSlsReturNoLkp()
        {
            var uid = CurrentUser();
            var queryable = ctx.Context.SpTrnsRturHdrs.Where(x => x.CompanyCode == uid.CompanyCode && x.BranchCode == uid.BranchCode && x.TypeOfGoods == uid.TypeOfGoods)
                     .Join(ctx.Context.SpTrnsRturDtls, a => new { a.ReturnNo }, b => new { b.ReturnNo }, (a, b) => new { a, b })
                     .Select(y => new SpTrnSRturHdrView()
                     {
                         ReturnNo = y.a.ReturnNo,
                         ReturnDate = y.a.ReturnDate,
                         FPJNo = y.a.FPJNo,
                         FPJDate = y.a.FPJDate
                     })
                     .AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpTrnSRturSrvHdrLookUp> FakturSrvLkp()
        {
            var Uid = CurrentUserInfo();
            var queryable = from p in ctx.Context.SpTrnSRturSrvHdrs
                            where p.CompanyCode == Uid.CompanyCode && p.BranchCode == Uid.BranchCode
                            select new SpTrnSRturSrvHdrLookUp()
                            {
                                InvoiceNo = p.InvoiceNo,
                                InvoiceDate = p.InvoiceDate,
                                ReferenceNo = p.ReferenceNo,
                                ReferenceDate = p.ReferenceDate
                            };
            return (queryable);
        }



        [HttpGet]
        public IQueryable<PickingHdrBrowse> BrowsePickingHdrNewOrder()
        {
            var uid = CurrentUser();
            var sql = string.Format("exec uspfn_spBrowseSpTrnSPickingHdr '{0}','{1}','{2}',{3}", uid.CompanyCode, uid.BranchCode, uid.TypeOfGoods, 1);
            var queryable = ctx.Context.Database.SqlQuery<PickingHdrBrowse>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<PickingHdrBrowse> BrowsePickingHdrBackOrder()
        {
            var uid = CurrentUser();
            var sql = string.Format("exec uspfn_spBrowseSpTrnSPickingHdr '{0}','{1}','{2}',{3}", uid.CompanyCode, uid.BranchCode, uid.TypeOfGoods, 0);
            var queryable = ctx.Context.Database.SqlQuery<PickingHdrBrowse>(sql).AsQueryable();
            return (queryable);
        }

        #region "Lampiran Dokumen Service"
        [HttpGet]
        public IQueryable<SelectSPKNoEnhance> SPKNoLookUp()
        {
            var Uid = CurrentUser();
            var sql = string.Format("exec  uspfn_spSelectSPKNoEnhance '{0}','{1}','{2}'", Uid.CompanyCode, Uid.BranchCode, Uid.CoProfile.ProductType);
            var queryable = ctx.Context.Database.SqlQuery<SelectSPKNoEnhance>(sql).AsQueryable();
            return (queryable);
        }
        #endregion

        #region "Entry Retur Penjualan"
        [HttpGet]
        public IQueryable<ReturPenjualanView> Get4FakturPenjualan()
        {
            var Uid = CurrentUser();
            var sql = string.Format("exec uspfn_spGet4FakturRetur '{0}','{1}','{2}'", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods);
            var queryable = ctx.Context.Database.SqlQuery<ReturPenjualanView>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<SpTrnSRturHdrView> Get4BrowseRtrFakturPenjualan()
        {
            var Uid = CurrentUser();
            var sql = string.Format("exec uspfn_getBrowseEntryRtrPenjualan '{0}','{1}','{2}'", Uid.CompanyCode, Uid.BranchCode, Uid.TypeOfGoods);
            var queryable = ctx.Context.Database.SqlQuery<SpTrnSRturHdrView>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<ReturPart> GetPartReturDetails(string fpjNo)
        {
            var Uid = CurrentUser();
            var sql = string.Format("exec uspfn_spGetPartReturDetails '{0}','{1}','{2}','{3}','{4}'", Uid.CompanyCode, Uid.BranchCode, fpjNo, Uid.TypeOfGoods, Uid.CoProfile.ProductType);
            var queryable = ctx.Context.Database.SqlQuery<ReturPart>(sql).AsQueryable();
            return (queryable);
        }
        #endregion

        [HttpGet]
        public IQueryable<Select4LookupSalesman> GetEmployeeAllBranch()
        {
            var Uid = CurrentUserInfo();
            var sql = string.Format("exec uspfn_spGetEmployeeAllBranchs '{0}'", Uid.CompanyCode);
            var queryable = ctx.Context.Database.SqlQuery<Select4LookupSalesman>(sql).AsQueryable();
            return (queryable);

        }

        [HttpGet]
        public IQueryable<Select4LookupSalesman> GetEmployeeBranch()
        {
            var Uid = CurrentUserInfo();
            var sql = string.Format("exec uspfn_spGetEmployeeBranchs '{0}','{1}'", Uid.CompanyCode, Uid.BranchCode);
            var queryable = ctx.Context.Database.SqlQuery<Select4LookupSalesman>(sql).AsQueryable();
            return (queryable);
        }

        [HttpGet]
        public IQueryable<GetInquiryMapping> GetInquirySpBtn(string Area, string Dealer, string Outlet, string detail)
        {

            var Uid = CurrentUser();
            var query = string.Format("exec uspfn_gnInquiryBtn '{0}','{1}','{2}','{3}'", Area, Dealer, Outlet, detail);
            var queryable = ctx.Context.Database.SqlQuery<GetInquiryMapping>(query).AsQueryable();
            return (queryable);

        }
        
    }

}
