using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;
using SimDms.Common;

namespace SimDms.Sparepart.Controllers.Api
{
    public class MasterItemController : BaseController
    {
        //
        // GET: /MasterItemController/
        public JsonResult Default()
        {
            string s = "", p = "";

            var trans = ctx.LookUpDtls
                .Where(x => (
                    x.CompanyCode == CompanyCode && 
                    x.CodeID == "TPGO" && 
                    x.LookUpValue == TypeOfGoods))
                .FirstOrDefault();

            if (trans != null)
            {
                s = trans.LookUpValueName;
            }

            var pz = ctx.LookUpDtls
                .Where(x => (
                    x.CompanyCode == CompanyCode &&
                    x.CodeID == GnMstLookUpHdr.ProductType  &&
                    x.LookUpValue == ProductType))
                .FirstOrDefault();

            if (pz != null)
            {
                p = pz.LookUpValueName;
            }

            return Json(new
            {
                CompanyCode = CompanyCode,
                BornDate = DateTime.Now,
                LastPurchaseDate = DateTime.Now,
                LastDemandDate = DateTime.Now,
                LastSalesDate = DateTime.Now,
                DiscPct=0,
                PurcDiscPct=0,
                IsGenuinePart = 1,
                PartType = s ,
                ProductType = ProductType,
                ProductTypeDesc = p,
                MovingCode = 0,
                UserInfo = CurrentUser
            });
        }


        public JsonResult getDataTableAlokasi(SpMasteritemStockAlokasiView model, string PartNo)
        {
            var queryable = ctx.Database.SqlQuery<SpMasteritemStockAlokasiView>("exec sp_SpMasteritemStockAlokasiView '" + CompanyCode + "','" + BranchCode + "'").AsQueryable();
            return Json(GeLang.DataTables<SpMasteritemStockAlokasiView>.Parse(queryable, Request));
        }

        public JsonResult CheckPartCount(string PartNo)
        {
            var result = ctx.Database.SqlQuery<System.Decimal>("exec [uspfn_SpMstItemCheckPartCount]  @CompanyCode=@p0, @BranchCode=@p1, @PartNo=@p2", CompanyCode, BranchCode, PartNo);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(SpMasterItemView model)
        {
            string msg = "";
            var record = ctx.spMstItems.Find(CompanyCode, BranchCode, model.PartNo);

            if (record == null)
            {
                record = new spMstItem();
                record.CompanyCode = CompanyCode;
                record.BranchCode = BranchCode;
                record.PartNo = model.PartNo;
                record.MovingCode = "0";
                record.ABCClass = "C";
                record.TypeOfGoods = TypeOfGoods;
                record.CreatedBy = CurrentUser.UserId;
                record.CreatedDate = DateTime.Now;
                record.OrderPointQty = 0;
                record.SafetyStockQty = 0;
                ctx.spMstItems.Add(record);
            }
            else
            {
                ctx.spMstItems.Attach(record);
            }

            record.LastDemandDate = model.LastDemandDate;
            record.LastPurchaseDate = model.LastPurchaseDate;
            record.LastSalesDate = model.LastSalesDate;
            record.Status = model.Status;
            record.ProductType = model.ProductType;
            record.PartCategory =  model.PartCategory ?? "";
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;
            record.DemandAverage = model.DemandAverage;

            record.SafetyStock = model.SafetyStock;
            record.OrderCycle = model.OrderCycle;
            record.LeadTime = model.LeadTime;

            record.SafetyStockQty = model.SafetyStock * model.DemandAverage;
            record.OrderPointQty = (model.DemandAverage * (model.OrderCycle + model.LeadTime)) + model.SafetyStock;

            record.Utility1 = model.Utility1;
            record.Utility2 = model.Utility2;
            record.Utility3 = model.Utility3;
            record.Utility4 = model.Utility4;

            record.SalesUnit = model.SalesUnit;
            record.OrderUnit = model.OrderUnit;
            record.BornDate = model.BornDate;

            //Kondisi
            //Jika SD dan Part = SGP maka PurcDiscPct = mengambil dari gnMstSupplierProfitCenter.disPct dengan kondisi SupplierCode = Main Dealer dan ProfitCenterCode = Sparepart/300
            //if (IsMD == false)
            //{
            //    var recSupProfitCenter = ctx.SupplierProfitCenters.Where(x => x.SupplierCode == BranchMD && x.ProfitCenterCode == "300");
            //    decimal? decPurcDiscPct = 0;
            //    if (recSupProfitCenter != null)
            //    {
            //        decPurcDiscPct = recSupProfitCenter.FirstOrDefault().DiscPct.Value; 
            //    }

            //    record.PurcDiscPct = decPurcDiscPct;
            //}
            //else
            //{
                record.PurcDiscPct = model.PurcDiscPct;
            //}
            //EOF Kondisi

            Helpers.ReplaceNullable(record);
            ctx.SaveChanges();

            var info = ctx.MasterItemInfos.Find(CompanyCode,  model.PartNo);
            if (info == null)
            {
                info = new MasterItemInfo();
                info.CompanyCode = CompanyCode;
                info.PartNo = model.PartNo;
                info.CreatedBy = CurrentUser.UserId;
                info.CreatedDate = DateTime.Now;
                ctx.MasterItemInfos.Add(info);
            }
            else
            {
                ctx.MasterItemInfos.Attach(info);
            }

            info.SupplierCode = model.SupplierCode;
            info.PartName = model.PartName;
            info.IsGenuinePart = model.IsGenuinePart;
            info.DiscPct = model.DiscPct;
            info.UOMCode = model.UOMCode;
            info.Status = model.Status;
            info.ProductType = model.ProductType ?? "";
            info.PartCategory = model.PartCategory ?? "";
            info.LastUpdateBy = CurrentUser.UserId;
            info.LastUpdateDate = DateTime.Now;
            info.isLocked = false;

            Helpers.ReplaceNullable(info);
            ctx.SaveChanges();

            var nwItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, model.PartNo, "00");

            if (nwItemLoc == null)
            {
                nwItemLoc = new SpMstItemLoc();
                nwItemLoc.CompanyCode = CompanyCode;
                nwItemLoc.BranchCode = BranchCode;
                nwItemLoc.PartNo = model.PartNo;
                nwItemLoc.WarehouseCode = "00";
                nwItemLoc.LocationCode = "-";
                nwItemLoc.OnHand = 0;
                nwItemLoc.AllocationSP = 0;
                nwItemLoc.AllocationSR = 0;
                nwItemLoc.AllocationSL = 0;
                nwItemLoc.BackOrderSP = 0;
                nwItemLoc.BackOrderSR = 0;
                nwItemLoc.BackOrderSL = 0;
                nwItemLoc.ReservedSP = 0;
                nwItemLoc.ReservedSR = 0;
                nwItemLoc.ReservedSL = 0;
                nwItemLoc.CreatedBy = CurrentUser.UserId;
                nwItemLoc.CreatedDate = DateTime.Now;
                ctx.SpMstItemLocs.Add(nwItemLoc);
            }

            try
            {
                Helpers.ReplaceNullable(nwItemLoc);
                ctx.SaveChanges();
                return Json(new { success = true, message = msg, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        
        }

        private MasterItemInfo _ItemInfo(string PartNo)
        {
            return ctx.MasterItemInfos.Find(CompanyCode, PartNo);
        }

        private spMstOrderParam _OrderParam(string spCode, string MovingCode)
        {
            return ctx.spMstOrderParams.Find(CompanyCode, BranchCode, spCode, MovingCode);
        }

        private MstSupplierProfitCenter _SplProfitCenter(string spCode)
        {
            return ctx.MstSupplierProfitCenters.Find(CompanyCode, BranchCode, spCode, ProfitCenter);
        }

        private spMstPurchCampaign _SplPurchaseCampaign(string spCode,string PartNo)
        {
            return ctx.Database.SqlQuery<spMstPurchCampaign>("uspfn_spGetPurchaseCampaign '" + CompanyCode + "','" + BranchCode + "','" + spCode + "','" + PartNo + "'").FirstOrDefault();
        }

        private spMstItemPrice _SplGetPrice(string PartNo)
        {
            return ctx.spMstItemPrices.Find(CompanyCode, BranchCode, PartNo);
        }

        private IList<SpMasteritemStockAlokasiView> ListItemAllocation(string PartNo)
        {
            return ctx.Database.SqlQuery<SpMasteritemStockAlokasiView>("uspfn_spGetItemAlocation '" + CompanyCode + "','" + BranchCode + "','" + PartNo + "'").ToList();
        }

        private IList<MasterModelBrowse> ListModel(string PartNo)
        {
            return ctx.Database.SqlQuery<MasterModelBrowse>("uspfn_spModelGridLookup '" + CompanyCode + "','" + GnMstLookUpHdr.ModelVehicle + "','" + PartNo + "'").ToList();
        }

        public string GetLookupValue(string VarGroup, string varCode)
        {
            string s = "";
            var x = ctx.LookUpDtls.Find(CompanyCode, VarGroup, varCode);
            if (x != null) s = x.ParaValue;
            return s;
        }

        public string GetLookupValueName(string VarGroup, string varCode)
        {
            string s = "";
            var x = ctx.LookUpDtls.Find(CompanyCode, VarGroup, varCode);
            if (x != null) s = x.LookUpValueName;
            return s;
        }

        public JsonResult CheckItem(string PartNo)
        {
            string msg = "Item not found";
            var record = ctx.spMstItems.Find(CompanyCode, BranchCode, PartNo);
            //var info = ctx.MasterItemInfos.Find(CompanyCode, PartNo);
            //var supliername = ctx.GnMstSuppliers.Where(a => a.CompanyCode == CompanyCode && a.SupplierCode == info.SupplierCode).FirstOrDefault();
            if (record != null)
            {
                var info = ctx.MasterItemInfos.Find(CompanyCode, PartNo);
                var supliername = ctx.GnMstSuppliers.Where(a => a.CompanyCode == CompanyCode && a.SupplierCode == info.SupplierCode).FirstOrDefault();

                if (record.TypeOfGoods.Equals(TypeOfGoods) && record.ProductType.Equals(ProductType))
                {
                    if (info != null)
                    {
                        return Json(new { success = true, 
                            data = record, 
                            masterinfo = info, 
                            profit = _SplProfitCenter(info.SupplierCode), 
                            discount2 = GetLookupValue(GnMstLookUpHdr.OrderType,"S"),
                            campaign = _SplPurchaseCampaign(info.SupplierCode, PartNo),
                            price = _SplGetPrice(PartNo),
                            alokasi = ListItemAllocation(PartNo),
                            model = ListModel(PartNo),
                            mode = 3,
                                          SupplierName = supliername.SupplierName
                        }, JsonRequestBehavior.AllowGet);
                    } else
                    return Json(new { success = true, 
                        data = record,
                        price = _SplGetPrice(PartNo),
                        alokasi = ListItemAllocation(PartNo),
                        model = ListModel(PartNo),
                        masterinfo = info, 
                        mode = 1 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    msg = "Part yang bersangkutan mempunyai TypeOfGoods yang berbeda dengan user.login anda !";
                }
            }
            else
            {
                var info = ctx.MasterItemInfos.Find(CompanyCode, PartNo);
                if (info != null)
                {
                    return Json(new { success = true, 
                        masterinfo = info, 
                        orderparam = _OrderParam(info.SupplierCode,"0"),
                        mode = 2 }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json( new { success = false, message = msg, mode = 0 } , JsonRequestBehavior.AllowGet);

        }

        public JsonResult SparePartlookup()
        {
            string sql, s = "";
            if (Request["sSearch"] != null)
            {
                s = Request["sSearch"].ToString();
            }

            sql = string.Format("uspfn_spMasterPartLookup '{0}', '{1}', '{2}', '{3}', '{4}'", CompanyCode, BranchCode, TypeOfGoods, ProductType, s);
            
            return Json(eXecSQL<MasterItemBrowse>(sql), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModelLookUp()
        {
            string sql, s = "";
            if (Request["sSearch"] != null)
            {
                s = Request["sSearch"].ToString();
            }
            sql = string.Format("uspfn_spModelGridLookup '{0}', '{1}', '', '{2}'", CompanyCode, GnMstLookUpHdr.ModelVehicle, s);
            return Json(eXecSQL<MasterModelBrowse>(sql));
        }

        public JsonResult SaveModel()
        {
            string PartNo = Request["PartNo"].ToString();
            string ModelCode = Request["ModelCode"].ToString();
            string PartCtg = Request["PartCtg"].ToString();

            var oData = ctx.SpMstItemModels.Find(CompanyCode, PartNo, ModelCode);

            if (oData == null)
            {
                var X = new SpMstItemModel();
                X.CompanyCode = CompanyCode;
                X.PartCategory = PartCtg;
                X.PartNo = PartNo;
                X.ModelCode = ModelCode;
                X.ProductType = ProductType;
                ctx.SpMstItemModels.Add(X);
                
                Helpers.ReplaceNullable(X);
            }

            try
            {
                ctx.SaveChanges();
                var data = ctx.Database.SqlQuery<MasterModelBrowse>("uspfn_spModelGridLookup '" + CompanyCode + "','" + GnMstLookUpHdr.ModelVehicle + "','" + PartNo + "'").ToList();
                return Json(new { success = true, result = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        public JsonResult UpdateModel()
        {
            string PartNo = Request["PartNo"].ToString();
            string ModelCode = Request["ModelCode"].ToString();
            string OldModelCode = Request["OldModel"].ToString();
            string PartCtg = Request["PartCtg"].ToString();

            var G = ctx.SpMstItemModels.Find(CompanyCode, PartNo, OldModelCode);

            if (G != null)
            {
                ctx.SpMstItemModels.Remove(G);
                var X = new SpMstItemModel();
                X.CompanyCode = CompanyCode;
                X.PartCategory = PartCtg;
                X.PartNo = PartNo;
                X.ModelCode = ModelCode;
                X.ProductType = ProductType;
                ctx.SpMstItemModels.Add(X);
                
                Helpers.ReplaceNullable(X);
            }

            try
            {
                ctx.SaveChanges();
                var data = ctx.Database.SqlQuery<MasterModelBrowse>("uspfn_spModelGridLookup '" + CompanyCode + "','" + GnMstLookUpHdr.ModelVehicle + "','" + PartNo + "'").ToList();
                return Json(new { success = true, result = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        public JsonResult DeleteModel()
        {
            string PartNo = Request["PartNo"].ToString();
            string ModelCode = Request["ModelCode"].ToString();
            string PartCtg = Request["PartCtg"].ToString();

            var oData = ctx.SpMstItemModels.Find(CompanyCode, PartNo, ModelCode);

            if (oData != null)
            {
                ctx.SpMstItemModels.Remove(oData);
            }

            try
            {
                ctx.SaveChanges();
                var data = ctx.Database.SqlQuery<MasterModelBrowse>("uspfn_spModelGridLookup '" + CompanyCode + "','" + GnMstLookUpHdr.ModelVehicle + "','" + PartNo + "'").ToList();
                return Json(new { success = true, result = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult GetSupplierKontrak()
        {
            string supplierID = Request["SPCODE"].ToString();
            string cityID = Request["CITYCODE"].ToString();

            try
            {
                var odata = ctx.MstSupplierProfitCenters.Find(CompanyCode, BranchCode, supplierID, ProfitCenter);
                if (odata != null)
                {
                    var city = GetLookupValueName("CITY", cityID);
                    return Json(new { success = true, message = "data found", kontrak = odata.ContactPerson, kota = city  });
                }
                else
                {
                    return Json(new { success = false, message = "record not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }


        public JsonResult GetCustomerKontrak()
        {
            string custID = Request["CUSCODE"].ToString();
            string cityID = Request["CITYCODE"].ToString();

            try
            {
                var odata = ctx.MstCustomerProfitCenters.Find(CompanyCode, BranchCode, custID, ProfitCenter);
                if (odata != null)
                {
                    var city = GetLookupValueName("CITY", cityID);
                    return Json(new { success = true, message = "data found", kontrak = odata.ContactPerson , kota = city, kelas = odata.CustomerClass  });
                }
                else
                {
                    return Json(new { success = false, message = "record not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }


    }
}
