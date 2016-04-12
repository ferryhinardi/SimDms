using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;
using SimDms.Common;

namespace SimDms.Sparepart.Controllers.Api
{
    public class ItemPriceController : BaseController
    {
        public JsonResult Save(spMstItemPrice model)
        {
            string msg = "";
            var record = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, model.PartNo);
            var sqlItem = string.Format("SELECT * FROM {0}..spMstItems WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
              GetDbMD(), CompanyMD, BranchMD, model.PartNo);
            spMstItem oItem = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();
 
            if (record == null)
            {
                record = new spMstItemPrice
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    PartNo = model.PartNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spMstItemPrices.Add(record);
                msg = "New item price added...";
            }
            else
            {
                ctx.spMstItemPrices.Attach(record);
                msg = "Item price updated";
            }

            record.RetailPrice = model.RetailPrice;
            record.RetailPriceInclTax = model.RetailPriceInclTax;
            record.PurchasePrice = model.PurchasePrice;
            record.CostPrice = model.PurchasePrice;
            record.OldRetailPrice = model.OldRetailPrice;
            record.OldPurchasePrice = model.OldPurchasePrice;
            record.OldCostPrice = model.OldCostPrice;
            record.LastPurchaseUpdate = model.LastPurchaseUpdate;
            record.LastRetailPriceUpdate = model.LastRetailPriceUpdate;

            record.LockingBy = CurrentUser.UserId;
            record.LockingDate = DateTime.Now;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            Helpers.ReplaceNullable(record);

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = msg, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public JsonResult SaveHistory(spHstItemPrice model)
        {
            string msg = "";
            var record = ctx.spHstItemPrices.Find(CompanyCode, BranchCode, model.PartNo, model.UpdateDate);

            if (record == null)
            {
                record = new spHstItemPrice
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    PartNo = model.PartNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spHstItemPrices.Add(record);
                msg = "New item price added...";
            }
            else
            {
                ctx.spHstItemPrices.Attach(record);
                msg = "Item price updated";
            }
          
            
            record.RetailPrice = model.RetailPrice;
            record.RetailPriceInclTax = model.RetailPriceInclTax;
            record.PurchasePrice = model.PurchasePrice;
            record.CostPrice = model.CostPrice;
            record.OldRetailPrice = model.OldRetailPrice;
            record.OldPurchasePrice = model.OldPurchasePrice;
            record.OldCostPirce = model.OldCostPirce;
            record.LastPurchaseUpdate = model.LastPurchaseUpdate;
            record.LastRetailPriceUpdate = model.LastRetailPriceUpdate;

            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = msg, data = record });
            }
            catch (Exception ex)
            {
                   return Json(new { success = false, message = ex.Message });
            }
        }
        
        public JsonResult Delete(spMstItemPrice model)
        {
            var record = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, model.PartNo);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.spMstItemPrices.Remove(record);
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public string CheckRecord(string PartNo)
        {
            var data = ctx.spMstItems.Find(CompanyCode, BranchCode, PartNo);
            if (data == null)
            {
                return "0";
            }
            return "1";
        }

        public JsonResult getRecord(string PartNo)
        {
            var PartNoCompare = PartNo;
            var record =  ctx.spMstItemPrices.Find(CompanyCode, BranchCode, PartNo);
            if (record != null)
            {
                var oItems = ctx.spMstItems.Find(CompanyCode, BranchCode, record.PartNo);
                if (oItems != null)
                {
                    if (oItems.ProductType.Equals(ProductType)&& oItems.TypeOfGoods.Equals(TypeOfGoods))
                    {
                        SpMstItemInfo oItemInfo = ctx.SpMstItemInfos.Find(CompanyCode, PartNo);
                        var partName = "";
                        if (oItemInfo != null)
                        {
                            partName = oItemInfo.PartName;
                        }

                        return Json(new { success = true, data = record, PartName = partName });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            else
            {
                SpMstItemInfo oItemInfo = ctx.SpMstItemInfos.Find(CompanyCode, PartNo);
                if (oItemInfo == null)
                    return Json(new { success = false });
                else
                {
                    var partName = oItemInfo.PartName;
                    bool isRetailPriceIncPPN = false;
                    bool isPurchasePriceIncPPN = false;
                    decimal taxPct = 0;
                    var model = new spMstItemPrice();
                    model.PartNo = PartNo;

                    var recCo = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
                    if(recCo != null){
                        isRetailPriceIncPPN = recCo.isRetailPriceIncPPN;
                        isPurchasePriceIncPPN = recCo.isPurchasePriceIncPPN;
                    }

                    var recTax = ctx.Taxes.Find(CompanyCode, "PPN");
                    if(recTax != null){
                        taxPct = recTax.TaxPct ?? 0;
                    }

                    if (isPurchasePriceIncPPN)
                    {
                        var purchasePrice = oItemInfo.PurchasePrice ?? 0;
                        decimal realpurchasePrice = Math.Truncate(purchasePrice / (1 + (taxPct / 100)));
                        model.PurchasePrice = Convert.ToDecimal(realpurchasePrice.ToString("n0"));
                        model.CostPrice = Convert.ToDecimal(realpurchasePrice.ToString("n0"));
                    }
                    else
                    {
                        decimal realpurchasePrice = oItemInfo.PurchasePrice ?? 0;
                        model.PurchasePrice = Convert.ToDecimal(realpurchasePrice.ToString("n0"));
                        model.CostPrice = Convert.ToDecimal(realpurchasePrice.ToString("n0"));
                    }

                    if (isRetailPriceIncPPN)
                    {
                        var purchasePrice = oItemInfo.PurchasePrice ?? 0;
                        decimal retailPriceInclTax = oItemInfo.PurchasePrice ?? 0;
                        decimal retailPrice = Math.Truncate(purchasePrice / (1 + (taxPct / 100)));
                        model.RetailPriceInclTax = Convert.ToDecimal(retailPriceInclTax.ToString("n0"));
                        model.RetailPrice = Convert.ToDecimal(retailPrice.ToString("n0"));
                    }
                    else
                    {
                        var purchasePrice = oItemInfo.PurchasePrice ?? 0;
                        decimal retailPrice = oItemInfo.PurchasePrice ?? 0;
                        decimal retailPriceInclTax = Math.Truncate(purchasePrice * (1 + (taxPct / 100)));
                        model.RetailPriceInclTax = Convert.ToDecimal(retailPriceInclTax.ToString("n0"));
                        model.RetailPrice = Convert.ToDecimal(retailPrice.ToString("n0"));
                    }

                    Helpers.ReplaceNullable(model);
                    return Json(new { success = true, data = model, PartName = partName });
                }
            }
        }

        public JsonResult Select4Lookup()
        {
            try
            {
                var queryable = ctx.Database.SqlQuery<SpMasterPartSelect4Lookup>(@"exec sp_spMasterPartSelect4Lookup 
                        @CompanyCode=@p0, @BranchCode=@p1, @TypeOfGoods=@p2, @ProductType=@p3",
                        CompanyCode, BranchCode, TypeOfGoods, ProductType
                    ).AsQueryable();
                
                return Json(queryable.toKG());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Terjadi Kesalahan, Hubungi SDMS Support", error_log = ex.Message });
            }
        }

    }
}
