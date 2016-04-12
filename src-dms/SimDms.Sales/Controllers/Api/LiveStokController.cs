using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;

namespace SimDms.Sales.Controllers.Api
{
    public class LiveStokController : BaseController
    {
        public JsonResult loadCombo(string tipeCombo, string tipe, string variant, string colour, bool? isVisible)
        {
            var pTypeCombp = tipeCombo == null ? "" : tipeCombo;
            var pType = tipe == null ? "" : tipe;
            var pVariant = variant == null ? "" : variant;
            var pColour = colour == null ? "" : colour;
            var iSQL = String.Format("EXEC uspfn_spComboLiveStockDealerSales '{0}','{1}','{2}','{3}','{4}'", pTypeCombp, pType, pVariant, pColour, isVisible);
            var data = ctx.Database.SqlQuery<CboItem>(iSQL).AsQueryable();
            if (data.Count()  > 0)
            {
                return Json(new { success = true, data = data });
            }
            return Json(new { success = false });
        }

        public JsonResult LiveStockDealerSalesTable(string type, string variant, string transmisi, string colour)
        {
            string query = "";
            int to = 0; bool fa = true;
            try
            {
                if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

                ctx.Database.CommandTimeout = 21600;
                query = string.Format("exec uspfn_spLoadLiveStockDealer '{0}','{1}','{2}','{3}'", CompanyCode, type, variant, transmisi);
             
                var data = ctx.Database.SqlQuery<LiveStockDealerSales>(query).AsQueryable();

                if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;
                return Json(new { success = true, total = data.Count(), data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult SaveVisiblilityDealer(List<LiveStockDealerSales> data)
        {
            try
            {
                if (data.Count > 0)
                {
                    List<LiveStockDealerSales> temp = new List<LiveStockDealerSales>();
                    foreach (var item in data)
                    {
                        string Type = item.Type,
                            Variant = item.Variant,
                            Colour = item.Colour;
                        bool IsVisible = item.IsVisible;
                        int existsData = ctx.Database.SqlQuery<int>(string.Format("EXEC uspfn_spCheckLiveStockDealerSales '{0}','{1}','{2}','{3}'", CompanyCode, Type, Variant, IsVisible)).SingleOrDefault();
                        if (existsData == 0)
                        {
                            LiveStockDealerSales d = new LiveStockDealerSales();
                            d.CompanyCode = CompanyCode;
                            d.BranchCode = BranchCode;
                            d.Type = Type;
                            d.Variant = Variant;
                            d.IsVisible = IsVisible;
                            temp.Add(d);
                        }
                    }

                    foreach (var i in temp)
                    {
                        ctx.Database.ExecuteSqlCommand(string.Format("EXEC uspfn_spInsertLiveStockDealerSales '{0}','{1}','{2}','{3}','{4}'", i.CompanyCode, i.Type, i.Variant, i.IsVisible, CurrentUser.UserId));
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class LiveStockDealerSales
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string BranchCode { get; set; }
        public string Type { get; set; }
        public string Variant { get; set; }
        public string Transmission { get; set; }
        public string Colour { get; set; }
        public decimal Qty { get; set; }
        public bool IsVisible { get; set; }
    }

    public class CboItem
    {
        public string value { get; set; }
        public string text { get; set; }
    }
}