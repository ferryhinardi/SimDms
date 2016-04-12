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
using SimDms.Common.Models;

namespace SimDms.Sparepart.Controllers.Api
{
    public class MaintainHargaPokokController : BaseController
    {
        public JsonResult Save(MaintainSave model)
        {
            try
            {
                var query = string.Format(@"
                   exec uspfn_MaintainItemAvgCost '{0}','{1}','{2}','{3}','{4}'
                ", CompanyCode, BranchCode, model.PartNo, model.NewCostPrice, CurrentUser.UserId);

                ctx.Database.ExecuteSqlCommand(query);
                return Json(new { success = true});
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        public JsonResult PopulateData(string PartNo)
        {
            try
            {
                var query = string.Format(@"
                exec sp_MaintainAvgCostItem '{0}','{1}', '{2}', '{3}', 'B' 
                ", CompanyCode, BranchCode, ProductType, PartNo);
                var row = ctx.Database.SqlQuery<InquiryAvgMainRow>(query);
                
                return Json(new { success = true, dt = row });
            }
            catch
            {
                return Json(new { success = false, message = "Terjadi Kesalahan, Fail!!!" });
            }
        }

        
        /* Maintain Tipe Part */
        public JsonResult FisrtLoad() 
        {
            bool result = false;
            var dtOrg = ctx.OrganizationDtls.ToList();
            if (dtOrg.Count >= 1)
            {
                var recOrgDtl = ctx.OrganizationDtls.Find(CompanyCode, BranchCode);
                if (recOrgDtl.IsBranch)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }

            return Json(new { success = result });
        }
        
        public JsonResult SaveMaintainTypeOfGoods(SaveMaintainTypeOfGoods model)
        {
            var pesan = "";
            var dtOrg = ctx.OrganizationDtls.ToList();
            try
            {
                foreach (var row in dtOrg)
                {
                    var recItems = ctx.spMstItems.Find(CompanyCode, row.BranchCode.ToString(), model.PartNo);
                    if (recItems != null)
                    {
                        decimal validQty = Convert.ToDecimal(recItems.OnHand + recItems.AllocationSL + recItems.AllocationSP + recItems.AllocationSR + recItems.OnOrder + recItems.InTransit);
                        if (validQty > 0)
                        {
                            pesan = "Maintain Tipe Part Gagal ! Periksa onhand, alokasi, onorder dan intransit untuk part " + model.PartNo + " di cabang " + row.BranchName.ToString() + " [" + row.BranchCode.ToString() + "] !";
                            return Json(new { success = false, message = pesan });
                        }
                        else
                        {
                            recItems.TypeOfGoods = model.TypeOfGoods;
                            recItems.LastUpdateBy = CurrentUser.UserId;
                            recItems.LastUpdateDate = DateTime.Now;
                            
                        }
                    }
                }
                ctx.SaveChanges();
                return Json(new { success = true});
            }
            catch {
                return Json(new { success = false, message = "Terjadi Kesalahan!!!" });
            }
        }


        public JsonResult Print(printing model) 
        {
            var strbln = "";
            switch (model.MonthPeriod.ToString()) { 
                case "1": strbln = "Jan".ToString();
                    break;
                case "2": strbln = "Feb".ToString();
                    break;
                case "3": strbln = "Mar".ToString();
                    break;
                case "4": strbln = "Apr".ToString();
                    break;
                case "5": strbln = "Mei".ToString();
                    break;
                case "6": strbln = "Jun".ToString();
                    break;
                case "7": strbln = "Jul".ToString();
                    break;
                case "8": strbln = "Aug".ToString();
                    break;
                case "9": strbln = "Sep".ToString();
                    break;
                case "10": strbln = "Okt".ToString();
                    break;
                case "11": strbln = "Nov".ToString();
                    break;
                case "12": strbln = "Des".ToString();
                    break;
            }
            var bln = (model.IsPeriod == true) ? model.MonthPeriod : 0;
            var thn = model.YearPeriod;
            var prd = (model.IsPeriod == true) ? strbln + " " + model.YearPeriod : "ALL";
            var tog = (model.IsType == true) ? model.PartType : "%";
            try
            {
                return Json(new {
                    success = true,
                    month = bln,
                    year = thn,
                    periode = prd,
                    typeOfGoods = tog
                });
            }
            catch {
                return Json(new { success = false, message = "Terjadi Kesalahan"});
            }
        }
    }
}
