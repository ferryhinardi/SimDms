using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;

namespace SimDms.Sparepart.Controllers.Api
{
    public class PembelianEntryOrderSparepartController : BaseController
    {
        /// <summary>
        ///   The <c>PembelianEntryOrderSparepartController</c> type 
        ///   Fungsi save pada Entry Pesanan Sparepart.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Pembelian Entry Order Sparepart Controller<br></br>
        ///     Execute Model <t></t>: <br></br>
        ///     Primary Key<t></t>: (CompanyCode, BranchCode, model.POSNo) <br></br>
        ///     Tabel<t></t>: spTrnPPOSHdr <br></br>
        ///     Link Tabel<t></t>: spTrnPPOSdtl <br></br>
        ///     Update Link ref<t></t>: <br></br>
        ///   </para>
        /// </remarks>

        protected string GetNewDocumentNo(string doctype, DateTime transdate)
        {
            var sql = "uspfn_GnDocumentGetNew {0}, {1}, {2}, {3}, {4}";
            var result = ctx.Database.SqlQuery<string>(sql, CompanyCode, BranchCode, doctype, CurrentUser.UserId, transdate.ToString("yyyy-MM-dd")).FirstOrDefault();
            return result;
        }


        //public string getDataNO(string Code)
        //{
        //    // var transdate = ctx.CoProfileServices.Find(CompanyCode, BranchCode).TransDate;


        //    try
        //    {
        //        return GetNewDocumentNo(Code, DateTime.Now);
        //    }
        //    catch (Exception ex)
        //    {
        //        return 
        //    }




        //}



        public JsonResult Save(spTrnPPOSHdr model)
        {
            string msg = "";
            string PosNo = "";

            if (string.IsNullOrEmpty(model.POSNo))
                PosNo = GetNewDocumentNo("POS", DateTime.Now);
            else
                PosNo = model.POSNo;


            var record = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, PosNo);


            if (record == null)
            {
                record = new spTrnPPOSHdr
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    POSNo = PosNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spTrnPPOSHdrs.Add(record);
                msg = "New Trasnactiopn POS added...";
            }
            else
            {
                ctx.spTrnPPOSHdrs.Attach(record);
                msg = "Transaction Updated";
            }

            model.isSubstution = true;
            model.isSuggorProcess = true;

            record.POSDate = model.POSDate;
            record.SupplierCode = model.SupplierCode;
            record.OrderType = model.OrderType;
            record.isBO = model.isBO;
            record.isSubstution = model.isSubstution;
            record.isSuggorProcess = model.isSuggorProcess;
            record.Remark = model.Remark;
            record.ProductType = model.ProductType;
            record.ExPickingSlipNo = model.ExPickingSlipNo;
            record.ExPickingSlipDate = model.ExPickingSlipDate;
            record.Transportation = model.Transportation;
            record.TypeOfGoods = model.TypeOfGoods;
            record.isGenPORDD = model.isGenPORDD;



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




        public JsonResult Delete(spTrnPPOSHdr model)
        {
            var record = ctx.spTrnPPOSHdrs.Find(CompanyCode, BranchCode, model.POSNo);


            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.spTrnPPOSHdrs.Remove(record);
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


        public JsonResult SaveDetails(spTrnPPOSDtl model)
        {
            string msg = "";

            var record = ctx.spTrnPPOSDtls.Find(CompanyCode, BranchCode, model.POSNo, model.PartNo);

            if (record == null)
            {
                record = new spTrnPPOSDtl
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    POSNo = model.POSNo,
                    PartNo = model.PartNo,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spTrnPPOSDtls.Add(record);
                msg = "New Transaction Order Sparepart Details added...";
            }
            else
            {
                ctx.spTrnPPOSDtls.Attach(record);
                msg = "Transaction Order Sparepart Details updated";
            }


            record.SeqNo = model.SeqNo;
            record.OrderQty = model.OrderQty;
            record.SuggorQty = model.SuggorQty;
            record.PurchasePrice = model.PurchasePrice;
            record.DiscPct = model.DiscPct;
            record.PurchasePriceNett = model.PurchasePriceNett;
            record.CostPrice = model.CostPrice;
            record.TotalAmount = model.TotalAmount;
            record.ABCClass = model.ABCClass;
            record.MovingCode = model.MovingCode;
            record.ProductType = model.ProductType;
            record.PartCategory = model.PartCategory;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();

                var queryable = ctx.Database.SqlQuery<spTrnPPOSHdrView>("uspfn_spTrnPPOSHdrView '" + CompanyCode + "','" + BranchCode + "','" + TypeOfGoods + "'").AsQueryable();

                if (queryable != null)
                {
                    var records = queryable.Select(x => new
                    {
                        POSNo = x.POSNo,
                        PosDate = x.PosDate,
                        Status = x.Status,
                        SupplierCode = x.SupplierCode,
                        SupplierName = x.SupplierName
                    }).ToList();



                    return Json(new { success = true, data = records, count = records.Count });
                }
                else
                {
                    return Json(new { success = true, count = 0 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }




    }
}
