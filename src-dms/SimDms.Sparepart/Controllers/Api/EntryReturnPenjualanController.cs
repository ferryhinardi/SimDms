using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using SimDms.Sparepart.Models;
using System.Data;
using SimDms.Common;


namespace SimDms.Sparepart.Controllers.Api
{
    public class EntryReturnPenjualanController : BaseController
    {
        public JsonResult PrintRetur(string ReturnNo)
        {
            var recordHdr = ctx.SpTrnsRturHdrs.Find(CompanyCode, BranchCode, ReturnNo);
            if (recordHdr != null)
            {
                recordHdr.Status = "1";
                recordHdr.PrintSeq += 1;
                recordHdr.LastUpdateDate = DateTime.Now;
                recordHdr.LastUpdateBy = CurrentUser.UserId;
                ctx.SaveChanges();
            }
            else
            {
                return Json(new {success=false, message="Process Print Gagal, Data Tidak Ditemukan !!!" });
            }
            return Json(new {success=true, message="" });
        }

        public JsonResult DeleteReturHdr(string ReturnNo)
        {
            try
            {
                var ReturHdr = ctx.SpTrnsRturHdrs.Find(CompanyCode, BranchCode, ReturnNo);
                if (ReturHdr == null)
                {
                    throw new Exception("Data Retur Header Tidak di temukan");
                }
                else
                {
                    ReturHdr.Status = "3";
                    ReturHdr.LastUpdateBy = CurrentUser.UserId;
                    ReturHdr.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();
                }
                return Json(new { success = true, message = ""});
            }
            catch (Exception ex)
            {
                return Json(new{success=false,message="Error pada saat Delete Retur Header Data, Message="+ex.Message.ToString()});
            }
        }

        /// <summary>
        /// Save data Entry Return Penjualan
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveReturHdr(ReturPenjualanView model)
        {
            object returnObj = null;
            try
            {
                if (string.IsNullOrEmpty(CurrentUser.TypeOfGoods))
                {
                    throw new Exception("Maaf, harap setting type part untuk user anda terlebih dahulu !");
                }

                string errorMsg = IsValidStatus(model.ReturnNo);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    throw new Exception(errorMsg);
                }

                SpTrnSRturHdr oRtrHdr = PrepareHeader(model);

                errorMsg = DateTransValidation(oRtrHdr.ReturnDate.Value);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    throw new Exception(errorMsg);
                }

                SaveRturHdr(oRtrHdr);
                returnObj = new { success = true, Message = "", ReturnNo = oRtrHdr.ReturnNo };
            }
            catch (Exception ex)
            {
                returnObj = new { success = false, Message = "Error pada saat Save Return Perjualan, Message=" + ex.Message.ToString() };
            }
            return Json(returnObj);
        }

        /// <summary>
        /// Populate Customer Details 
        /// </summary>
        /// <param name="model">ReturPenjualanView</param>
        /// <returns></returns>
        public JsonResult PopulateCustomerDetails(ReturPenjualanView model)
        {
            CustomerDetailsTagih custData = new CustomerDetailsTagih();
            var oCust = ctx.GnMstCustomers.Find(CompanyCode, model.CustomerCode);
            if (oCust != null)
            {
                custData.CustomerName = oCust.CustomerName;
                custData.CustomerCode = oCust.CustomerCode;
                custData.Address1 = oCust.Address1;
                custData.Address2 = oCust.Address2;
                custData.Address3 = oCust.Address3;
                custData.PhoneNo = oCust.PhoneNo;
                custData.FaxNo = oCust.FaxNo;
            }

            var oFPJHdr = ctx.SpTrnSFPJHdrs.Find(CompanyCode, BranchCode, model.FPJNo);
            if (oFPJHdr != null)
                custData.PickingSlipNo = oFPJHdr.PickingSlipNo;

            var oLookUpDtl = ctx.LookUpDtls.Find(CompanyCode, "TOPC", oFPJHdr.TOPCode);
            if (oLookUpDtl != null)
                custData.TOPCode = oLookUpDtl.LookUpValueName;

            var oLookTransType = ctx.LookUpDtls.Find(CompanyCode, "TTPJ", oFPJHdr.TransType);
            if (oLookTransType != null)
                custData.OrderType = oLookTransType.LookUpValueName;
            return Json(new { success = true, data = custData });
        }

        /// <summary>
        /// prepare SpTrnRturnHdr object
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SpTrnSRturHdr PrepareHeader(ReturPenjualanView model)
        {
            SpTrnSRturHdr oRtrHdr = new SpTrnSRturHdr();
            oRtrHdr.CompanyCode = CompanyCode;
            oRtrHdr.BranchCode = BranchCode;
            oRtrHdr.ReturnNo = model.ReturnNo;
            oRtrHdr.ReturnDate = model.ReturnDate;
            oRtrHdr.CustomerCode = model.CustomerCode;
            oRtrHdr.FPJNo = model.FPJNo;
            oRtrHdr.FPJDate = model.FPJDate;
            oRtrHdr.FPJCentralNo = "";
            oRtrHdr.FPJCentralDate = DateTime.Now;
            oRtrHdr.ReferenceNo = model.ReferenceNo;
            oRtrHdr.ReferenceDate = model.ReferenceDate;
            oRtrHdr.PrintSeq = 0;
            oRtrHdr.Status = "0";
            oRtrHdr.TypeOfGoods = CurrentUser.TypeOfGoods;

            SpTrnSFPJInfo dtspTrnSFPJInfo = ctx.SpTrnSFPJInfos.Find(CompanyCode, BranchCode, oRtrHdr.FPJNo);
            SpTrnSFPJHdr dtSpTrnSFPJHdr = ctx.SpTrnSFPJHdrs.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode, oRtrHdr.FPJNo);
            oRtrHdr.isPKP = (dtSpTrnSFPJHdr != null) ? dtSpTrnSFPJHdr.IsPKP : false;
            oRtrHdr.NPWPNo = (dtspTrnSFPJInfo != null) ? dtspTrnSFPJInfo.NPWPNo : null;
            return oRtrHdr;
        }

        /// <summary>
        /// Prepare Return data part details
        /// </summary>
        /// <param name="header"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public SpTrnSRturDtl PrepareDetails(SpTrnSRturHdr header, ReturnDetailsView model)
        {
            SpTrnSRturDtl RturDtl = new SpTrnSRturDtl();
            RturDtl.CompanyCode = CompanyCode;
            RturDtl.BranchCode = BranchCode;
            RturDtl.ReturnNo = header.ReturnNo;
            RturDtl.ReturnDate = header.ReturnDate;
            RturDtl.QtyReturn = model.QtyReturn;

            var recordFPJDtl = ctx.SpTrnSFPJDtls.Find(CompanyCode, BranchCode, header.FPJNo, "00", model.PartNo, model.PartNoOriginal, model.DocNo);
            if (recordFPJDtl != null)
            {
                RturDtl.PartNo = recordFPJDtl.PartNo;
                RturDtl.PartNoOriginal = recordFPJDtl.PartNoOriginal;
                RturDtl.DocNo = recordFPJDtl.DocNo;
                RturDtl.WarehouseCode = recordFPJDtl.Warehousecode;
                RturDtl.RetailPriceInclTax = recordFPJDtl.RetailPriceInclTax;
                RturDtl.RetailPrice = recordFPJDtl.RetailPrice;
                RturDtl.CostPrice = recordFPJDtl.CostPrice;

                RturDtl.ReturAmt = RturDtl.QtyReturn * RturDtl.RetailPrice;
                RturDtl.DiscPct = recordFPJDtl.DiscPct;
                RturDtl.DiscAmt = Math.Round((decimal)(RturDtl.ReturAmt * RturDtl.DiscPct / 100), 0, MidpointRounding.AwayFromZero);

                //returnDtl.DiscAmt = returnDtl.DiscPct == 0 ? 0 :
                //    Math.Round((returnDtl.QtyReturn * ((returnDtl.DiscPct / 100) * returnDtl.RetailPrice)), 0);

                RturDtl.NetReturAmt = RturDtl.ReturAmt - RturDtl.DiscAmt;
                RturDtl.CostAmt = RturDtl.QtyReturn * RturDtl.CostPrice;

                // TO DO : PPNAmt set = 0, TotReturAmt should have the same values with TotDPPAmt
                RturDtl.PPNAmt = 0;
                RturDtl.TotReturAmt = RturDtl.NetReturAmt + RturDtl.PPNAmt;
                RturDtl.LocationCode = recordFPJDtl.LocationCode;
                RturDtl.ProductType = recordFPJDtl.ProductType;
                RturDtl.PartCategory = recordFPJDtl.PartCategory;
                RturDtl.MovingCode = recordFPJDtl.MovingCode;
                RturDtl.ABCClass = recordFPJDtl.ABCCLass;
            }
            return RturDtl;
        }

        /// <summary>
        /// Populate part Retur Details
        /// </summary>
        /// <param name="ReturnNo">Return No</param>
        /// <returns></returns>
        public JsonResult PopulatePartReturDetails(string ReturnNo)
        {
            var sql = string.Format("exec uspfn_spGetReturnDtlByReturnNo '{0}','{1}','{2}'", CompanyCode, BranchCode, ReturnNo);
            var dataDtl = ctx.Database.SqlQuery<ReturnDetailsView>(sql).ToList();
            return Json(dataDtl);
        }

        /// <summary>
        /// Delete Part Return Details 
        /// </summary>
        /// <param name="model">ReturnNo</param>
        /// <param name="ReturnNo"></param>
        /// <returns></returns>
        public JsonResult DeletePartReturDetails(ReturnDetailsView model, string ReturnNo)
        {
            object returnObj = null;
            try
            {
                var oRtrDtl = ctx.SpTrnsRturDtls.Find(CompanyCode, BranchCode, ReturnNo, model.PartNo, model.PartNoOriginal, "00", model.DocNo);

                //Delete SpTrnsRturDtl Record
                DeleteReturnDetails(oRtrDtl);
                returnObj = new { success = true, message = "" };
            }
            catch (Exception ex)
            {
                returnObj = new { success = false, message = "Error pada saaat Delete Part Return Details, Message=" + ex.Message.ToString() };

            }
            return Json(returnObj);
        }

        /// <summary>
        /// [MA 08/07/2014]
        /// Save Part Retur Details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReturnNo"></param>
        /// <param name="FPJNo"></param>
        /// <returns></returns>
        public JsonResult SavePartReturDetails(ReturnDetailsView model, string ReturnNo, string FPJNo)
        {
            object returnObj = null;
            string errorMsg = "";
            using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    errorMsg = IsValidStatus(ReturnNo);
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        throw new Exception(errorMsg);
                    }

                    if (model.QtyReturn == 0)
                    {
                        throw new Exception(ctx.SysMsgs.Find("5009").MessageCaption);
                    }
                    var MaxQtyReturPenjualan = ctx.Database.SqlQuery<decimal>(string.Format("exec uspfn_spGetMaxQtyReturPenjualan '{0}','{1}','{2}','{3}','{4}','{5}','{6}'"
                        , CompanyCode, BranchCode, model.PartNo, model.PartNoOriginal, ReturnNo, model.DocNo, FPJNo)).FirstOrDefault();

                    if (model.QtyReturn > MaxQtyReturPenjualan)
                    {
                        throw new Exception(ctx.SysMsgs.Find("5018").MessageCaption + "Jumlah maksimal yang dapat di retur = " + MaxQtyReturPenjualan);
                    }

                    var RtrHdr = ctx.SpTrnsRturHdrs.Find(CompanyCode, BranchCode, ReturnNo);
                    var RtrDtl = PrepareDetails(RtrHdr, model);

                    if (RtrDtl == null)
                    {
                        throw new Exception("Tidak ada data detail untuk disimpan");
                    }

                    if (RtrDtl.RetailPrice == 0)
                    {
                        throw new Exception(ctx.SysMsgs.Find("5029").MessageCaption);
                    }

                    //itemLoc should have been set.
                    var WareHouseCode = ctx.LookUpDtls.Where(m => m.CompanyCode == CompanyCode && m.CodeID == "WRCD" && m.SeqNo == 1).FirstOrDefault().ParaValue;
                    SpMstItemLoc ItemLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, model.PartNo, WareHouseCode);
                    
                    //var queryItemLoc = string.Format(@"select * from {0}..SpMstItemLoc where CompanyCode ='{1}' and BranchCode ='{2}' and PartNo = '{3}' and WarehouseCode = '{4}'",
                    //    GetDbMD(), CompanyMD, BranchMD, model.PartNo, WarehouseMD);
                    //var ItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(queryItemLoc).FirstOrDefault();

                    if (ItemLoc == null)
                    {
                        throw new Exception(string.Format(ctx.SysMsgs.Find("5039").MessageCaption, "simpan detail",
                        "\nData tidak dapat disimpan karena item lokasi belum disetting"));
                    }

                    //Save Part Retur details
                    SavePartRtrDetail(RtrDtl, ReturnNo, FPJNo);
                    trans.Commit();
                    returnObj = new { success = true, message = "" };
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    returnObj = new { success = false, message = "Error pada saat Save Part Return Details, Message=" + ex.Message.ToString() };
                }
            }
            return Json(returnObj);
        }

        /// <summary>
        /// Post Return part Details
        /// </summary>
        /// <returns></returns>
        public JsonResult PostingReturnPartDetails(ReturPenjualanView model)
        {
            object returnObj = null;
            string errorMsg = "";
            try
            {
                if (string.IsNullOrEmpty(CurrentUser.TypeOfGoods))
                {
                    throw new Exception("Maaf, harap setting type part untuk user anda terlebih dahulu sebelum posting return !");
                }

                errorMsg = IsValidStatus(model.ReturnNo);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    throw new Exception(errorMsg);
                }

                //errorMsg = DateTransValidation(model.ReturnDate);
                //if (!string.IsNullOrEmpty(errorMsg))
                //{
                //    throw new Exception(errorMsg);
                //}

                var oRtrHdr = ctx.SpTrnsRturHdrs.Find(CompanyCode, BranchCode, model.ReturnNo);
                if (oRtrHdr.Status.Equals("0"))
                {
                    throw new Exception("Status Adjustment tidak benar, tolong di Periksa");
                }
                
                PostingReturn(oRtrHdr);

                returnObj = new { success = true, message = "" };
            }
            catch (Exception ex)
            {
                returnObj = new { success = false, message = "Error pada saat Posting Return Penjualan, Message=" + ex.Message.ToString() };
            }
            return Json(returnObj);
        }

        /// <summary>
        /// Posting Return
        /// </summary>
        /// <param name="recordHdr"></param>
        private void PostingReturn(SpTrnSRturHdr recordHdr)
        {
            using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted) )
            {
                try
                {
                    //get all data retur details
                    var ListRturDtl = ctx.SpTrnsRturDtls.Where(m => m.CompanyCode == m.CompanyCode && m.BranchCode == BranchCode && m.ReturnNo == recordHdr.ReturnNo).ToList();
                    //Get Data retur Hdr
                    SpTrnSRturHdr recordRturHdr = ctx.SpTrnsRturHdrs.Find(CompanyCode, BranchCode, recordHdr.ReturnNo);
                    if (recordRturHdr == null)
                    {
                        throw new Exception("Data retur tidak ditemukan");
                    }

                    string cust = recordRturHdr.CustomerCode;
                    decimal finalRetur = recordRturHdr.TotFinalReturAmt.Value;

                    if (recordRturHdr.Status == "2")
                    {
                        throw new Exception(string.Format(ctx.SysMsgs.Find("5039").MessageCaption, "posting", "\nNo. Return = " + recordRturHdr.ReturnNo + " telah di posting"));
                    }

//                    //update Retur Hdr
                    recordRturHdr.Status = "2";
                    recordRturHdr.LastUpdateBy = CurrentUser.UserId;
                    recordRturHdr.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();

                    List<RturItem> ListRturItem = new List<RturItem>();
                    RturItem RturItem = null;
                    foreach (var RturDtl in ListRturDtl)
                    {
                        RturItem = new RturItem();
                        RturItem.PartNo = RturDtl.PartNo;
                        RturItem.QtyReturn = RturDtl.QtyReturn.Value;
                        RturItem.CostPrice = RturDtl.CostPrice.Value;
                        ListRturItem.Add(RturItem);
                    }

                    //update table SpMstItemPrice
                    UpdateItemPriceAvgCost(recordRturHdr.ReturnNo, "RJUAL", ListRturItem);
                    int iSeq = 1;

                    foreach (var RturDtl in ListRturDtl)
                    {
                        //Update Stock
                        UpdateStock(RturDtl.PartNo, RturDtl.WarehouseCode, RturDtl.QtyReturn.Value, 0, 0, 0, "");

                        //Insert svSDMovement
                        if (IsMD == false)
                        {
                            //Movement Log
                            MovementLog(recordRturHdr.ReturnNo, recordRturHdr.ReturnDate.Value, RturDtl.PartNo, RturDtl.WarehouseCode,
                                "IN", "RSSLIP", RturDtl.QtyReturn.Value);

                            //Insert svSDMovement
                            var qrDb = "SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'";
                            string DBMD = ctx.Database.SqlQuery<string>(qrDb).FirstOrDefault();

                            var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                                        GetDbMD(), CompanyMD, BranchMD, RturDtl.PartNo);
                            spMstItemPrice oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();

                            // sudah ada di MovementLog
//                            var iQry = @"insert into " + DBMD + @"..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice,   
//	                    TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, 
//	                    Status, ProcessStatus, ProcessDate, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate) 
//                        VALUES('" + CompanyCode + "','" + BranchCode + "','" + recordRturHdr.ReturnNo + "','" + ctx.CurrentTime + "','" + RturDtl.PartNo +
//                            "','" + (iSeq) + "','" + RturDtl.WarehouseCode + "','" + RturDtl.QtyReturn + "','" + RturDtl.QtyReturn + "','" + RturDtl.DiscPct + "','" + RturDtl.CostPrice +
//                            "','" + RturDtl.RetailPrice + "','" + CurrentUser.TypeOfGoods + "','" + CompanyMD + "','" + BranchMD + "','" + RturDtl.WarehouseCode + "','" + RturDtl.RetailPriceInclTax + "','" + RturDtl.RetailPrice +
//                            "','" + RturDtl.CostPrice + "','x','" + RturDtl.ProductType + "','300','0','0','" + ctx.CurrentTime + "','" + CurrentUser.UserId + "','" + ctx.CurrentTime + "','" + CurrentUser.UserId + "','" + ctx.CurrentTime + "')";

//                            ctx.Database.ExecuteSqlCommand(iQry);

//                            iSeq = iSeq + 1;
                        }
                    }

                    //insert ARInterface
                    GenerateAR(recordRturHdr.CustomerCode, recordRturHdr.ReturnNo,
                        recordRturHdr.ReturnDate.Value, recordRturHdr.TotFinalReturAmt.Value, 0, "RETURN", string.Empty,
                        Convert.ToDateTime("1900-01-01 00:00:00.000"), recordRturHdr.TypeOfGoods);

                    // Insert GLInterface
                    JournalSpReturn(recordRturHdr.ReturnNo, recordRturHdr.ReturnDate.Value,
                        recordRturHdr.FPJNo, (recordRturHdr.TotReturAmt == null ? 0 : recordRturHdr.TotReturAmt.Value),
                        (recordRturHdr.TotPPNAmt == null ? 0 : recordRturHdr.TotPPNAmt.Value), (recordRturHdr.TotDiscAmt == null ? 0 : recordRturHdr.TotDiscAmt.Value),
                        (recordRturHdr.TotCostAmt == null ? 0 : recordRturHdr.TotCostAmt.Value), CurrentUser.TypeOfGoods, recordRturHdr.CustomerCode);

                    UpdateBankBook(recordHdr.CustomerCode, finalRetur);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception("Error pada saat PostingReturn, Message=" + ex.Message.ToString());
                }
            }
        }

        private void UpdateBankBook(string customerCode, decimal totalRetur)
        {
            if (CurrentUser.CoProfile.IsLinkToFinance.Value)
            {
                BankBook oBankBook = ctx.BankBooks.Find(CompanyCode, BranchCode, customerCode, ProfitCenter);
                if (oBankBook == null)
                {
                    throw new Exception("Customer belum mempunyai bank book");
                }
                oBankBook.SalesAmt -= totalRetur;
                ctx.SaveChanges();
            }
        }

        private void UpdateReturnQtyOmTrSalesSO(string returnNo, string lmpNo)
        {
            try
            {
                var sql = string.Format("exec uspfn_spInsertReturnQtyUnit '{0}','{1}','{2}','{3}','{4}' ", CompanyCode, BranchCode, lmpNo, returnNo, CurrentUser.UserId);
                ctx.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada Saat UpdateReturnQty");
            }
        }

        private void uspfn_UpdateReturnQtySrvItem(string returnNo, string lmpNo)
        {
            try
            {
                var sql = string.Format("exec uspfn_UpdateReturnQtySrvItem '{0}','{1}','{2}','{3}','{4}' ", CompanyCode, BranchCode, lmpNo, returnNo, CurrentUser.UserId);
                ctx.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada Saat Update Return Qty");
            }
        }

        private void SavePartRtrDetail(SpTrnSRturDtl recordDtl, string ReturnNo, string FPJNo)
        {
            try
            {
                //save Part Return details into database
                SavePartRtrDetail(recordDtl);

                //Recalculate Retur Amount in SpTrnSRturHdr
                ReCalculateRtrAmtHdr(ReturnNo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada Save Part Retur Detail. Error: " + ex.Message);
            }
        }

        private void SavePartRtrDetail(SpTrnSRturDtl recordDtl)
        {
            try
            {
                var tmpRtrDtl = ctx.SpTrnsRturDtls.Find(CompanyCode, BranchCode, recordDtl.ReturnNo, recordDtl.PartNo, recordDtl.PartNoOriginal, recordDtl.WarehouseCode, recordDtl.DocNo);
                if (tmpRtrDtl == null)
                {
                    recordDtl.CreatedBy = CurrentUser.UserId;
                    recordDtl.CreatedDate = DateTime.Now;
                    recordDtl.LastUpdateBy = CurrentUser.UserId;
                    recordDtl.LastUpdateDate = DateTime.Now;

                    Helpers.ReplaceNullable(recordDtl);
                    ctx.SpTrnsRturDtls.Add(recordDtl);
                }
                else
                {
                    tmpRtrDtl.QtyReturn = recordDtl.QtyReturn;
                    tmpRtrDtl.RetailPriceInclTax = recordDtl.RetailPriceInclTax;
                    tmpRtrDtl.RetailPrice = recordDtl.RetailPrice;
                    tmpRtrDtl.CostPrice = recordDtl.CostPrice;

                    tmpRtrDtl.ReturAmt = recordDtl.ReturAmt;
                    tmpRtrDtl.DiscPct = recordDtl.DiscPct;
                    tmpRtrDtl.DiscAmt = recordDtl.DiscAmt;
                    tmpRtrDtl.NetReturAmt = recordDtl.NetReturAmt;
                    tmpRtrDtl.CostAmt = recordDtl.CostAmt;

                    tmpRtrDtl.PPNAmt = recordDtl.PPNAmt;
                    tmpRtrDtl.TotReturAmt = recordDtl.TotReturAmt;

                    tmpRtrDtl.LastUpdateBy = CurrentUser.UserId;
                    tmpRtrDtl.LastUpdateDate = DateTime.Now;

                    Helpers.ReplaceNullable(tmpRtrDtl);
                }
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function Save Part Return Detail, Message=" + ex.Message.ToString());
            }
        }

        private void DeleteReturnDetails(SpTrnSRturDtl RturDtl)
        {
            using (var transScope = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    //delete record SpTrnsRturDtls
                    ctx.SpTrnsRturDtls.Remove(RturDtl);
                    ctx.SaveChanges();

                    //Recalculate Retur Amt in SpTrnsRturHdr
                    ReCalculateRtrAmtHdr(RturDtl.ReturnNo);
                    transScope.Commit();
                }
                catch (Exception ex)
                {
                    transScope.Rollback();
                    throw new Exception("Error pada function DeleteReturnDetails, Message" + ex.Message.ToString());
                }
            }
        }

        private ReturSumAmt GetRturSumAmt(string ReturnNo)
        {
            var sql = string.Format("exec uspfn_spGetRturSumAmt '{0}','{1}','{2}'", CompanyCode, BranchCode, ReturnNo);
            var RtrAmt = ctx.Database.SqlQuery<ReturSumAmt>(sql).FirstOrDefault();
            return RtrAmt;
        }

        private void ReCalculateRtrAmtHdr(string ReturnNo)
        {
            try
            {
                var RturHdr = ctx.SpTrnsRturHdrs.Find(CompanyCode, BranchCode, ReturnNo);
                if (RturHdr != null)
                {
                    ReturSumAmt oRturSumAmt = GetRturSumAmt(RturHdr.ReturnNo);
                    if (oRturSumAmt != null)
                    {
                        RturHdr.TotReturQty = oRturSumAmt.TotReturQty;
                        RturHdr.TotReturAmt = oRturSumAmt.TotReturAmt;
                        RturHdr.TotCostAmt = oRturSumAmt.TotCostAmt;
                        RturHdr.TotDiscAmt = oRturSumAmt.TotDiscAmt;
                        RturHdr.TotDPPAmt = oRturSumAmt.TotDPPAmt;

                        var oProfit = ctx.ProfitCenters.Find(CompanyCode, BranchCode, RturHdr.CustomerCode, ProfitCenter);
                        var recTax = (string.IsNullOrEmpty(oProfit.TaxCode)) ? null : ctx.Taxes.Find(CompanyCode, oProfit.TaxCode);

                        RturHdr.TotPPNAmt = (recTax != null) ? (decimal)(Math.Truncate((decimal)(RturHdr.TotDPPAmt * recTax.TaxPct / 100))) : 0;
                        RturHdr.TotFinalReturAmt = RturHdr.TotDPPAmt + RturHdr.TotPPNAmt;

                    }
                    else
                    {
                        RturHdr.TotReturQty = 0;
                        RturHdr.TotReturAmt = 0;
                        RturHdr.TotDiscAmt = 0;
                        RturHdr.TotDPPAmt = 0;
                        RturHdr.TotPPNAmt = 0;
                        RturHdr.TotFinalReturAmt = 0;
                        RturHdr.TotCostAmt = 0;
                    }

                    RturHdr.Status = "0";
                    RturHdr.LastUpdateDate = DateTime.Now;
                    RturHdr.LastUpdateBy = CurrentUser.UserId;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function ReCalculateRtrAmtHdr, Message" + ex.Message.ToString());
            }
        }

        private void SaveRturHdr(SpTrnSRturHdr RturHdr)
        {
            SpTrnSRturHdr oRturHdr = ctx.SpTrnsRturHdrs.Find(CompanyCode, BranchCode, RturHdr.ReturnNo);
            using (var transScope = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (oRturHdr == null)
                    {
                        oRturHdr = RturHdr;
                        oRturHdr.ReturnNo = GetNewDocumentNo("RTR", RturHdr.ReturnDate.Value);

                        if (oRturHdr.ReturnNo.EndsWith("X"))
                        {
                            var msgs = string.Format(ctx.SysMsgs.Find("5046").MessageCaption, "Return Penjualan");
                            throw new Exception(msgs);
                        }
                        oRturHdr.CreatedBy = CurrentUser.UserId;
                        oRturHdr.CreatedDate = DateTime.Now;
                        oRturHdr.LastUpdateBy = CurrentUser.UserId;
                        oRturHdr.LastUpdateDate = DateTime.Now;
                        ctx.SpTrnsRturHdrs.Add(oRturHdr);
                    }
                    else
                    {
                        oRturHdr.FPJNo = RturHdr.FPJNo;
                        oRturHdr.FPJDate = RturHdr.FPJDate;
                        oRturHdr.CustomerCode = RturHdr.CustomerCode;
                        oRturHdr.isPKP = RturHdr.isPKP;
                        oRturHdr.NPWPNo = RturHdr.NPWPNo;
                        oRturHdr.TypeOfGoods = RturHdr.TypeOfGoods;
                        oRturHdr.ReferenceNo = RturHdr.ReferenceNo;
                        oRturHdr.ReferenceDate = RturHdr.ReferenceDate;
                        oRturHdr.Status = "0";
                        oRturHdr.LastUpdateBy = CurrentUser.UserId;
                        oRturHdr.LastUpdateDate = DateTime.Now;
                    }
                    ctx.SaveChanges();
                    transScope.Commit();
                }
                catch (Exception ex)
                {
                    transScope.Rollback();
                    throw new Exception("Error pada function SaveRturHdr, Message=" + ex.Message.ToString());
                }
            }
        }

        private string IsValidStatus(string ReturnNo)
        {
            var returnMsg = "";
            var SpRturnHdr = ctx.SpTrnsRturHdrs.Find(CompanyCode, BranchCode, ReturnNo);
            if (SpRturnHdr != null)
            {
                if (int.Parse(SpRturnHdr.Status) > 1)
                {
                    returnMsg = "Nomor Dokumen ini telah ter-posting !!";
                }
            }
            return returnMsg;
        }
    }
}
