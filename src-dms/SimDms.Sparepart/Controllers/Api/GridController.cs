using System;
using SimDms.Sparepart.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Common.Models;
using SimDms.Common;

namespace SimDms.Sparepart.Controllers.Api
{
    public class GridController : BaseController
    {

        public JsonResult MasterPartView(string refcode)
        {
           // var queryable = ctx.SpMasterPartViews.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
         //   //   var queryable = ctx.SpMasterItemViews.Where(a => a.CompanyCode == CompanyCode).Select(a => new SpMasterItemView  { PartNo = a.PartNo }).Distinct().OrderBy(a => a.PartNo);
         ////   var queryable = ctx.Database.SqlQuery<SpMasterPartView>("exec sp_SpMasterPartView '" + CompanyCode + "','" + BranchCode + "'").AsQueryable();
       //return Json(GeLang.DataTables<SpMasterPartView>.Parse(queryable, Request));
    return Json(eXecSp<SpMasterPartView>("sp_SpMasterPartView"));
        }

        public JsonResult MasterPartBrowse(string refcode)
        {
//            var sql = string.Format(@"
//            SELECT TOP 1500
//             ItemPrice.PartNo
//            ,ItemInfo.PartName
//            ,ItemInfo.SupplierCode
//            ,ItemPrice.PurchasePrice
//            ,ItemPrice.RetailPriceInclTax
//            ,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
//            ,Items.PartCategory
//            ,(SELECT LookupValueName 
//                FROM gnMstLookupDtl 
//                WHERE CodeID = 'PRCT' AND 
//                LookUpValue = Items.PartCategory AND 
//                CompanyCode = '{0}') CategoryName
//            FROM spMstItemPrice ItemPrice 
//            INNER JOIN spMstItems Items 
//                ON ItemPrice.CompanyCode=Items.CompanyCode 
//                AND ItemPrice.BranchCode=Items.BranchCode
//                AND ItemPrice.PartNo=Items.PartNo
//            INNER JOIN spMstItemInfo ItemInfo 
//                ON ItemPrice.CompanyCode = ItemInfo.CompanyCode 
//                AND ItemPrice.PartNo = ItemInfo.PartNo
//            WHERE ItemPrice.CompanyCode= '{0}'
//                AND ItemPrice.BranchCode= '{1}'
//                AND Items.TypeOfGoods= '{2}'
//                AND Items.ProductType= '{3}'", CompanyCode, BranchCode,  TypeOfGoods,ProductType);
//            var data = ctx.Database.SqlQuery<LoadPart>(sql);

//            return Json(GeLang.DataTables<LoadPart>.Parse(data.AsQueryable(), Request));
            return Json(eXecSp<LoadPart>("sp_spmasterpart", "'" + TypeOfGoods + "','" + ProductType +"'"));

        }
        //public JsonResult MasterSupplierBrowse(string refcode)
        //{
        //    var queryable = ctx.gnMstSupplierViews.Where(x => x.CompanyCode == CompanyCode);
        //    return Json(GeLang.DataTables<gnMstSupplierView>.Parse(queryable, Request));
        //}
 


        public JsonResult MasterItemLocationBrowse(string refcode)
        {
            var queryable = ctx.SpMstItemlocViews
                .Where(x => (x.CompanyCode == CompanyCode &&
                    x.BranchCode == BranchCode &&
                    x.ProductType == ProductType &&
                    x.TypeOfGoods == TypeOfGoods));

            return Json(GeLang.DataTables<SpMstItemlocView>.Parse(queryable, Request));
        }

        public JsonResult MasterItemLocationItemLookup(string refcode)
        {
            var queryable = ctx.SpMstItemLocItemLookupViews
                .Where(x => (x.CompanyCode == CompanyCode &&
                    x.BranchCode == BranchCode &&
                    x.ProductType == ProductType &&
                    x.TypeOfGoods == TypeOfGoods));
            return Json(GeLang.DataTables<SpMstItemLocItemLookupView>.Parse(queryable, Request));
        }


        public JsonResult MasterItemLocationWarehouseLookup(string refcode)
        {
            var queryable = ctx.LookUpDtls
                .Where(x => (x.CompanyCode == CompanyCode &&
                    x.CodeID == "WRCD"));
            return Json(GeLang.DataTables<LookUpDtl>.Parse(queryable, Request));
        }

        public string WareHouseName(string id)
        {
            var queryable = ctx.LookUpDtls
                .Where(x => (x.CompanyCode == CompanyCode &&
                    x.CodeID == "WRCD" && x.LookUpValue == id)).FirstOrDefault();

            string s = "";

            if (queryable != null)
            {
                s = queryable.LookUpValueName;
            }

            return s;
        }

        public JsonResult MstLookupDetail(string id)
        {
            var queryable = ctx.LookUpDtls
                .Where(x => (x.CompanyCode == CompanyCode && x.CodeID == id));
            return Json(GeLang.DataTables<LookUpDtl>.Parse(queryable, Request));
        }

        public JsonResult PickingList()
        {
            string sql = string.Format("exec GetFPJLookUp {0}, {1}, {2}, {3}",CompanyCode,BranchCode, TypeOfGoods,ProductType);
            return Json(eXecSp<PickingList>(sql));
        }

        public JsonResult ItemPartLookupGrid()
        {
            var field = "";
            var value = "";
            string dynamicFilter = "";

            for (int i = 0; i < 7; i++)
            {
                field = Request["filter[filters][" + i + "][field]"] ?? "";
                value = Request["filter[filters][" + i + "][value]"] ?? "";

                if (dynamicFilter == "")
                {
                    dynamicFilter += value != "" ? " AND " + field + " LIKE ''%" + value + "%'" : "";
                }
                else
                {
                    dynamicFilter += value != "" ? "' AND " + field + " LIKE ''%" + value + "%'" : "";
                }
            }

            dynamicFilter = dynamicFilter != "" ? dynamicFilter += "'" : "";

            string sql = string.Format(@"EXEC sp_SpMstItemModifInfoWeb '{0}', '{1}'",
                CompanyCode, dynamicFilter);
            
            
            var rslt = ctx.Database.SqlQuery<SpMstItemModifInfo>(sql).AsQueryable();



            return Json(rslt.toKG());
        }

        public JsonResult MovingCodeLookup()
        {
            string sql = string.Format(@"EXEC sp_spMstMovingCodeView_Web '{0}', '{1}'",
                CompanyCode, Helpers.GetDynamicFilter(Request), 500);

            var records = ctx.Database.SqlQuery<spMstMovingCodeView>(sql).AsQueryable();
            return Json(records.toKG(ApplyFilterKendoGrid.False));
        }

        public JsonResult SupplierLookup()
        {
            string sql = string.Format(@"EXEC uspfn_spSuppliers_Web '{0}', '{1}', '{2}', '{3}', '{4}'", 
                CompanyCode, BranchCode, ProfitCenter, Helpers.GetDynamicFilter(Request), 500);

            var records = ctx.Database.SqlQuery<supplierLookUp>(sql).AsQueryable();
            return Json(records.toKG(ApplyFilterKendoGrid.False));
        }

        public JsonResult SupplierReqLookup()
        {
            string sql = string.Format(@"SELECT a.SupplierCode, a.SupplierName, ISNULL((a.address1+''+a.address2+''+a.address3+''+a.address4),'') as Alamat,
	                (Case a.Status when 0 then 'Tidak Aktif' else 'Aktif' end) as [Status]
                from gnMstSupplier a
                inner JOIN gnMstOrganizationDtl b
                on a.CompanyCode = b.CompanyCode
                AND a.SupplierCode = b.BranchCode
                WHERE a.SupplierCode <> '{0}'",
                BranchCode);

            var records = ctx.Database.SqlQuery<supplierLookUp>(sql).AsQueryable();
            return Json(records.toKG(ApplyFilterKendoGrid.False));
        }

        public JsonResult SparePartLookupNewV2(int cols)
        {
            string dynamicFilter = "";
            dynamicFilter = GetDynFilters(cols);

            dynamicFilter = dynamicFilter != "" ? dynamicFilter += "'" : "";

            var query = string.Format(@"exec uspfn_spMasterPartLookupNewV2 '{0}', '{1}' {2}", CompanyCode, ProductType, dynamicFilter);
            var queryable = ctx.Database.SqlQuery<MasterItemBrowse>(query).AsQueryable();
            return Json(queryable.toKG());
        }

        public JsonResult MasterItemLocationBrowseV2(int cols)
        {
            string dynamicFilter = "";
            dynamicFilter = GetDynFilters(cols);

            dynamicFilter = dynamicFilter != "" ? dynamicFilter += "'" : "";

            var query = string.Format(@"exec uspfn_SpMstItemLocViewV2 '{0}', '{1}', '{2}' ,'{3}' {4}", CompanyCode, BranchCode, TypeOfGoods, ProductType, dynamicFilter);
            var queryable = ctx.Database.SqlQuery<SpMstItemlocView>(query).AsQueryable();
            return Json(queryable.toKG());
        }

        public JsonResult SparePartLocationLookupV2(int cols)
        {
            string dynamicFilter = "";
            dynamicFilter = GetDynFilters(cols);

            dynamicFilter = dynamicFilter != "" ? dynamicFilter += "'" : "";

            var query = string.Format(@"exec uspfn_spMasterPartLocationLookupV2 '{0}', '{1}', '{2}' ,'{3}' {4}", CompanyCode, BranchCode, TypeOfGoods, ProductType, dynamicFilter);
            var queryable = ctx.Database.SqlQuery<SparePartLocationLookup>(query).AsQueryable();
            return Json(queryable.toKG());
        }

        public JsonResult OrderSparepartLookup()
        {
            string sql = string.Format(@"EXEC uspfn_SpOrderSparepartView_Web '{0}', '{1}', '{2}', '{3}', '{4}', '{5}'",
                CompanyCode, BranchCode, TypeOfGoods, ProductType, Helpers.GetDynamicFilter(Request), 500);

            var records = ctx.Database.SqlQuery<OrderSparepartview>(sql).AsQueryable();
            return Json(records.toKG(ApplyFilterKendoGrid.False));            
        }
    }
}
