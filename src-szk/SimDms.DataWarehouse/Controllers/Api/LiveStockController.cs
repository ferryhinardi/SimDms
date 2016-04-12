using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using SimDms.DataWarehouse.Models;
using System.Web.Script.Serialization;
using System.Data;
using SimDms.DataWarehouse.Helpers;
using GeLang;
using System.Data.Entity.Core.Objects;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class LiveStockController : BaseController
    {
        public JsonResult LiveStockSalesTable()
        {
            string type = Request["Type"];
            string transmission = Request["Transmission"];
            string variant = Request["Variant"];
            string colour = Request["Colour"];
            bool IsVisible;
            string query = "";
            
            if (!bool.TryParse(Request["IsVisible"], out IsVisible)) 
            {
                IsVisible = true;
            }

            int to = 0; bool fa = true;
            try
            {
                if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

                ctx.Database.CommandTimeout = 10800;
                if (Request["IsVisible"] == null)
                {
                    query = string.Format("exec uspfn_spLiveStockSales '{0}', '{1}', '{2}', '{3}'", type, variant, transmission, colour);
                }
                else
                {
                    query = string.Format("exec uspfn_spLiveStockSales '{0}', '{1}', '{2}', '{3}', {4}", type, variant, transmission, colour, IsVisible);
                }

                var data = ctx.Database.SqlQuery<LiveStockSales>(query).AsQueryable();

                if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;
                return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult SelectCombo()
        {
            var typeCombo = Request["TypeCombo"] ?? "";
            var type = Request["Type"] ?? "";
            var variant = Request["Variant"] ?? "";
            var colour = Request["Colour"] ?? "";
            var isVisible = Request["IsVisible"] ?? "";

            var boolVisible = (dynamic)null;

            if (isVisible == "")
            {
                boolVisible = DBNull.Value;
            }
            else 
            {
                boolVisible = isVisible;
            }
 
            try
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spComboLiveStockSales";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@TypeCombo", typeCombo);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Variant", variant);
                cmd.Parameters.AddWithValue("@Colour", colour);
                cmd.Parameters.AddWithValue("@IsVisible", boolVisible);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult SaveVisiblility(List<LiveStockSales> data)
        {
            try
            {
                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        string Type = item.Type,
                            Variant = item.Variant,
                            Transmission = item.Transmission,
                            Colour = item.Colour;
                        bool IsVisible = item.IsVisible;
                        ctx.Database.ExecuteSqlCommand(string.Format("EXEC uspfn_spInsertLiveStockSales '{0}', '{1}', '{2}', '{3}', {4}", Type, Variant, Transmission, Colour, IsVisible));
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #region -- Live Stock Sales Dealer --
        public JsonResult SelectComboDealer()
        {
            var typeCombo = Request["TypeCombo"] ?? "";
            var type = Request["Type"] ?? "";
            var variant = Request["Variant"] ?? "";
            var colour = Request["Colour"] ?? "";
            var isVisible = Request["IsVisible"] ?? "";

            var boolVisible = (dynamic)null;

            if (isVisible == "")
            {
                boolVisible = DBNull.Value;
            }
            else
            {
                boolVisible = isVisible;
            }

            try
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spComboLiveStockDealerSales";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@TypeCombo", typeCombo);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Variant", variant);
                cmd.Parameters.AddWithValue("@Colour", colour);
                cmd.Parameters.AddWithValue("@IsVisible", boolVisible);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult LiveStockDealerSalesTable()
        {
            string type = Request["Type"];
            string transmission = Request["Transmission"];
            string variant = Request["Variant"];
            string colour = Request["Colour"];
            bool IsVisible, IsMaintain;
            string query = "";

            if (!bool.TryParse(Request["IsVisible"], out IsVisible))
            {
                IsVisible = true;
            }
            if (!bool.TryParse(Request["IsMaintain"], out IsMaintain))
            {
                IsMaintain = false;
            }

            int to = 0; bool fa = true;
            try
            {
                if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

                ctx.Database.CommandTimeout = 21600;
                if (Request["IsVisible"] == null)
                {
                    query = string.Format("exec uspfn_spLiveStockDealerSales {0}, '{1}', '{2}', '{3}', '{4}'", IsMaintain, type, variant, transmission, colour);
                }
                else
                {
                    query = string.Format("exec uspfn_spLiveStockDealerSales {0}, '{1}', '{2}', '{3}', '{4}', {5}", IsMaintain, type, variant, transmission, colour, IsVisible);
                }

                var data = ctx.Database.SqlQuery<LiveStockDealerSales>(query).AsQueryable();

                if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;
                return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
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
                        int existsData = ctx.Database.SqlQuery<int>(string.Format("EXEC uspfn_spCheckLiveStockDealerSales '{0}', '{1}', '{2}', {3}", Type, Variant, Colour, IsVisible)).SingleOrDefault();
                        if (existsData == 0)
                        {
                            LiveStockDealerSales d = new LiveStockDealerSales();
                            d.Type = Type;
                            d.Variant = Variant;
                            d.IsVisible = IsVisible;
                            temp.Add(d);
                        }
                    }

                    foreach (var i in temp)
                    {
                        ctx.Database.ExecuteSqlCommand(string.Format("EXEC uspfn_spInsertLiveStockDealerSales '{0}', '{1}', '{2}'", i.Type, i.Variant, i.IsVisible));
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }
}
