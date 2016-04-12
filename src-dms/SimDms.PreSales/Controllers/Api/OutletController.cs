using SimDms.PreSales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SimDms.PreSales.Controllers.Api
{
    public class OutletController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                UserID = CurrentUser.UserId
            });
        }

        public JsonResult GetGrid(string BranchCode)
        {
            var data = ctx.PmBranchOutlets.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode);
            return Json(data);
        }

        public JsonResult SaveDetail(string BranchCode, string OutletID, string OutletName)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var outletRec = ctx.PmBranchOutlets.Find(CompanyCode, BranchCode, OutletID);
                    if (outletRec != null)
                    {
                        outletRec.OutletName = OutletName.ToUpper();
                        outletRec.LastUpdateDate = DateTime.Now;
                        outletRec.LastUpdateBy = CurrentUser.UserId;
                    }
                    else
                    {
                        var outletIDs = ctx.PmBranchOutlets.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode).OrderBy(a => a.OutletID);
                        string outletID = string.Empty;

                        if (outletIDs.Count() > 0)
                        {
                            int inc = 1;
                            foreach (var row in outletIDs)
                            {
                                outletID = row.OutletID.Substring(2, 2);
                                if (inc != Convert.ToInt32(outletID)) break;
                                inc++;
                            }
                            outletID = BranchCode.Substring(BranchCode.Length - 2, 2) + inc.ToString().PadLeft(2, '0');
                        }
                        else
                        {
                            outletID = BranchCode.Substring(BranchCode.Length - 2, 2) + "01";
                        }

                        outletRec = new PmBranchOutlet();
                        outletRec.CompanyCode = CompanyCode;
                        outletRec.BranchCode = BranchCode;
                        outletRec.OutletName = OutletName.ToUpper();
                        outletRec.OutletID = outletID;
                        outletRec.CreatedBy = CurrentUser.UserId;
                        outletRec.CreatedDate = DateTime.Now;
                        outletRec.LastUpdateBy = outletRec.CreatedBy = CurrentUser.UserId;
                        outletRec.LastUpdateDate = outletRec.CreatedDate = DateTime.Now;
                        ctx.PmBranchOutlets.Add(outletRec);
                    }
                    ctx.SaveChanges();
                    trans.Complete();
                    return Json(new { success = true, data = outletRec, message = "Proses Save berhasil!" });
                }
                catch
                {
                    trans.Dispose();
                    return Json(new { success = false, message = "Proses Save gagal!" });
                }
            }
        }

        public JsonResult DeleteDetail(string BranchCode, string OutletID)
        {
            var outletRec = ctx.PmBranchOutlets.Find(CompanyCode, BranchCode, OutletID);
            try
            {
                if (outletRec != null)
                {
                    ctx.PmBranchOutlets.Remove(outletRec);
                    ctx.SaveChanges();
                }
                return Json(new { success = true, message = "Proses Delete berhasil!" });
            }
            catch
            {
                return Json(new { success = false, message = "Proses Delete gagal!" });
            }
        }
            
    }
}