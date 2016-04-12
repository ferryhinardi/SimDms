using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Sales.BLL;
using SimDms.Sales.Models;
using SimDms.Common;
using System.Transactions;

namespace SimDms.Sales.Controllers.Api
{
    public class SalesBPKController : BaseController
    {
        public JsonResult Save(OmTrSalesBPK mdl, bool isAll = false)
        {
            try
            {
                string serr = "";
                if (!DateTransValidation((DateTime)mdl.BPKDate, ref serr))
                {
                    return Json(new { success = false, message = serr });
                }

                if (ctx.Database.SqlQuery<omSlsBPkLkpDO>(string.Format("exec uspfn_omSlsBPKLkpDO {0},{1},{2}", CompanyCode, BranchCode, ProfitCenter))
                      .Where(x => x.DONo == mdl.DONo).Count() < 1)
                {
                    return Json(new { success = false, browse = "do", message = "DONo Not Valid " });
                }

                bool isNew = false;
                var recordHdr = ctx.OmTrSalesBPKs.Find(CompanyCode, BranchCode, mdl.BPKNo);
                if (recordHdr == null)
                {
                    isNew = true;
                    recordHdr = new OmTrSalesBPK();
                    recordHdr.CompanyCode = CompanyCode;
                    recordHdr.BranchCode = BranchCode;
                    recordHdr.BPKDate = mdl.BPKDate;
                    recordHdr.BPKNo = GetNewDocumentNo("BPK", (DateTime)mdl.BPKDate);
                    recordHdr.CreatedBy = CurrentUser.UserId;
                    recordHdr.CreatedDate = ctx.CurrentTime;
                    recordHdr.isLocked = false;
                    ctx.OmTrSalesBPKs.Add(recordHdr);
                }
                recordHdr.DONo = mdl.DONo;
                recordHdr.SONo = mdl.SONo;
                recordHdr.CustomerCode = mdl.CustomerCode;
                recordHdr.ShipTo = mdl.ShipTo;
                recordHdr.WareHouseCode = mdl.WareHouseCode;
                recordHdr.Expedition = mdl.Expedition;
                recordHdr.Remark = mdl.Remark;
                recordHdr.Status = "0";
                recordHdr.LastUpdateBy = CurrentUser.UserId;
                recordHdr.LastUpdateDate = ctx.CurrentTime;

                Helpers.ReplaceNullable(recordHdr);
                ctx.SaveChanges();
                if (isAll)
                {
                    OmTrSalesBLL.Instance(CurrentUser.UserId).insertAllBPK(recordHdr);
                }

                return Json(new { success = true, BPKNo = recordHdr.BPKNo, Status = "0", StatusDsc = getStringStatus("0") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        public JsonResult Delete(OmTrSalesBPK mdl)
        {
            try
            {
                var itm = ctx.OmTrSalesBPKs.Find(CompanyCode, BranchCode, mdl.BPKNo);

                if (itm != null)
                {
                    if (OmTrSalesBLL.Instance(CurrentUser.UserId).deleteBPk(itm))
                    {
                        itm.Status = "3";
                        itm.LastUpdateBy = CurrentUser.UserId;
                        itm.LastUpdateDate = ctx.CurrentTime;

                        Helpers.ReplaceNullable(itm);

                        ctx.SaveChanges();
                    }

                    return Json(new { success = true, message = "Deleted", Status = "3", StatusDsc = getStringStatus("3") }); ;
                }
                return Json(new { success = false, message = "BPKNo Not Found" });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }); ;
            }
        }

        public JsonResult Approve(OmTrSalesBPK mdl)
        {
            bool result = false;
            bool independent = false;
            string Qry = "";
            string dbMD = ctx.Database.SqlQuery<string>("SELECT DbMD from gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'").FirstOrDefault();

            if ((CompanyCode == CompanyMD) && (BranchCode == BranchMD) || !cekOtomatis())
            {
                independent = true;
            }

            if ((dbMD == "" || dbMD == null) && !independent)
            {
                return Json(new { success = false, message = "Approved BPk gagal!! Database MD tidak ada silahkan cek gnMstCompanyMapping" }); 
            }

            

            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    var sobll = OmTrSalesBLL.Instance(CurrentUser.UserId);
                    var itm = ctx.OmTrSalesBPKs.Find(CompanyCode, BranchCode, mdl.BPKNo);

                    if (itm.Status == "2")
                    {
                        return Json(new { success = false, message = "BPK Sudah di Approve" });
                    }

                    if (itm != null)
                    {
                        if (sobll.Update4ITS(ctx, itm.SONo, "DELIVERY"))
                        {
                            itm.Status = "2";
                            itm.LastUpdateBy = CurrentUser.UserId;
                            itm.LastUpdateDate = ctx.CurrentTime;

                            Helpers.ReplaceNullable(itm);

                            result = ctx.SaveChanges() > 0;
                            if (result)
                            {
                                ctx.OmTrSalesBPKDetails
                               .Where(x => x.CompanyCode == CompanyCode &&
                                   x.BranchCode == BranchCode &&
                                   x.BPKNo == itm.BPKNo)
                               .ToList()
                               .ForEach(x =>
                               {
                                   if (result)
                                   {
                                       Qry = "INSERT INTO " + dbMD + "..omSDMovement(CompanyCode, BranchCode, DocNo, DocDate, Seq, SalesModelCode, SalesModelYear, ChassisCode, ChassisNo, EngineCode," +
                                             "EngineNo, ColourCode, WarehouseCode, CustomerCode, QtyFlag, CompanyMD, BranchMD, WarehouseMD, Status, ProcessStatus, ProcessDate, CreatedBy," +
                                             "CreatedDate, LastUpdateBy, LastUpdateDate) Values('" +
                                             itm.CompanyCode + "','" + itm.BranchCode + "','" + itm.BPKNo + "','" + itm.CreatedDate + "','" + x.BPKSeq + "','" + x.SalesModelCode +
                                             "','" + x.SalesModelYear + "','" + x.ChassisCode + "','" + x.ChassisNo + "','" + x.EngineCode + "','" + x.EngineNo +
                                             "','" + x.ColourCode + "','" + itm.WareHouseCode + "','" + itm.CustomerCode + "','-','" + CompanyMD + "','" + UnitBranchMD + "','" + WarehouseMD +
                                             "','" + itm.Status + "','0','" + DateTime.Now + "','" + CurrentUser.UserId + "','" + DateTime.Now + "','" + CurrentUser.UserId + "','" + DateTime.Now + "')";
                                       try
                                       {
                                           if (!independent)
                                           {
                                               ctx.Database.ExecuteSqlCommand(Qry);
                                           }
                                  
                                           //update mstVehicle
                                           var oVhcl = ctx.OmMstVehicles.Find(CompanyCode, x.ChassisCode, x.ChassisNo);
                                           if (oVhcl == null && !independent)
                                           {
                                               Qry = "UPDATE " + dbMD + "..omMstVehicle SET status='5', BPKNO='" + x.BPKNo + "', LastUpdateBy='" + CurrentUser.UserId + "', LastUpdateDate='" + DateTime.Now +
                                                     "' WHERE CompanyCode='" + CompanyMD + "' AND ChassisCode='" + x.ChassisCode + "' AND ChassisNo='" + x.ChassisNo + "'";
                                           }
                                           else
                                           {
                                               Qry = "UPDATE omMstVehicle SET status='5', BPKNO='" + x.BPKNo + "', LastUpdateBy='" + CurrentUser.UserId + "', LastUpdateDate='" + DateTime.Now +
                                                     "' WHERE CompanyCode='" + CompanyCode + "' AND ChassisCode='" + x.ChassisCode + "' AND ChassisNo='" + x.ChassisNo + "'";
                                           }

                                           try
                                           {
                                               ctx.Database.ExecuteSqlCommand(Qry);
                                               result = true;
                                           }
                                           catch
                                           {
                                               result = false;
                                           }
                                       }
                                       catch
                                       {
                                           result = false;
                                       }
                                   }
                               });
                            }

                            if (result && !independent)
                            {
                                int seq = 0;
                                var qry = String.Empty;
                                decimal pct = 100;
                                decimal? CostPrice = 0;
                                decimal? CostPriceMD = 0;
                                decimal? RetailPriceMD = 0;
                                decimal? RetailPriceIncTaxMD = 0;
                                var dtSO = ctx.OmTrSalesDOs.Find(CompanyCode, BranchCode, mdl.DONo);
                                if (dtSO != null)
                                {
                                    ctx.OmTrSalesSOAccsSeqs.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.SONo == dtSO.SONo).ToList()
                                    .ForEach(y =>
                                    {

                                        if (result)
                                        {
                                            seq = seq + 1;
                                            qry = "SELECT RetailPrice, CostPrice, RetailPriceInclTax FROM " + dbMD + "..spMstItemPrice WHERE CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' AND PartNo='" + y.PartNo + "'";
                                            var dtPriceMD = ctx.Database.SqlQuery<SpItemPrice>(qry).FirstOrDefault();
                                            if (dtPriceMD != null)
                                            {
                                                CostPriceMD = dtPriceMD.CostPrice;
                                                RetailPriceMD = dtPriceMD.RetailPrice;
                                                RetailPriceIncTaxMD = dtPriceMD.RetailPriceInclTax;
                                                if (y.TypeOfGoods == "2")
                                                {
                                                    Double x = Convert.ToDouble(RetailPriceMD.ToString()) * 0.75;
                                                    CostPrice = Convert.ToDecimal(Math.Floor(x));
                                                }
                                                else if (y.TypeOfGoods == "5")
                                                {
                                                    CostPrice = RetailPriceMD;
                                                }
                                            }                                                                               
                                            Qry = string.Format(@"INSERT INTO {0}..svSDMovement(
                                          CompanyCode,BranchCode,DocNo,DocDate,PartNo,PartSeq,WarehouseCode
                                            ,QtyOrder,Qty,DiscPct,CostPrice,RetailPrice,TypeOfGoods,CompanyMD
                                            ,BranchMD,WarehouseMD,RetailPriceInclTaxMD,RetailPriceMD,CostPriceMD
                                            ,QtyFlag,ProductType,ProfitCenterCode,Status,ProcessStatus
                                            ,ProcessDate,CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate)
                                        VALUES(
                                            '{1}','{2}','{3}','{4}','{5}','{6}','{7}'
                                            ,{8},{9},{10},{11},{12},'{13}','{14}'
                                            ,'{15}','{16}',{17},{18},{19}
                                            ,'{20}','{21}','{22}','{23}','{24}'
                                            ,'{25}','{26}','{27}','{28}','{29}')",
                                                    dbMD,
                                                    CompanyCode, BranchCode, mdl.BPKNo, DateTime.Now, y.PartNo, seq, WarehouseMD
                                                    , y.DemandQty.Value, y.Qty.Value, pct, CostPrice, y.RetailPrice.Value, y.TypeOfGoods, CompanyMD
                                                    , BranchMD, WarehouseMD, RetailPriceIncTaxMD, RetailPriceMD, CostPriceMD
                                                    , "-", ProductType, "300", "0", "0"
                                                    , DateTime.Now, CurrentUser.UserId, DateTime.Now, CurrentUser.UserId, DateTime.Now);

                                            result = ctx.Database.ExecuteSqlCommand(Qry) > 0;
                                        }
                                    });
                                }
                            }

                            if (result)
                            {
                                tranScope.Complete();
                                return Json(new { success = true, message = "Approved BPK Berhasil", Status = "2", StatusDsc = getStringStatus("2") });
                            }
                            else
                            {
                                return Json(new { success = false, message = "Approved BPk gagal" });
                            }
                        }
                        else
                        {
                            return Json(new { success = false, message = "Approved BPk gagal" });
                        }
                    }
                    return Json(new { success = false, message = "BPKNo Not Found" }); ;
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }); ;
            }
        }


        public JsonResult PrintBpk(OmTrSalesBPK mdl)
        {
            try
            {
                //if (mdl.Status == "0" || mdl.Status == "")
                //{
                    var itm = ctx.OmTrSalesBPKs.Find(CompanyCode, BranchCode, mdl.BPKNo);
                    var pType = ctx.CoProfiles.Find(CompanyCode, BranchCode).ProductType;

                    if (itm != null)
                    {
                        string sflg = "";
                        var flg = ctx.Database.SqlQuery<omSlsBPKMstlkpDtl>("select CodeID,LookUPValue,ParaValue,LookUpValueName FROM GnMstLookUpDtl WHERE CompanyCode=" + CompanyCode + " AND CodeID = 'INV_FRM' AND LookUpValue = 'FRM_SLS'").FirstOrDefault();
                        if (flg != null)
                            sflg = flg.ParaValue;

                        if (itm.Status == "0" || mdl.Status == "")
                        {
                            itm.Status = "1";
                            itm.LastUpdateBy = CurrentUser.UserId;
                            itm.LastUpdateDate = ctx.CurrentTime;
                            ctx.SaveChanges();
                        }

                        return Json(new { success = true, message = "Printed", Status = itm.Status, StatusDsc = getStringStatus(itm.Status), Flag = sflg, pType = pType }); ;
                    }
                    return Json(new { success = false, message = "DONo Not Found" }); ;
                //}
                //else
                //{
                //    return Json(new { success = false, message = "Invalid Status" }); ;
                //}
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }); ;
            }

        }


        public JsonResult SaveDetail(OmTrSalesBPKDetail mdl)
        {
            int max = 1;
            try
            {
                var hdr = ctx.OmTrSalesBPKs.Find(CompanyCode, BranchCode, mdl.BPKNo);
                hdr.Status = "0";
                if (hdr == null)
                {
                    return Json(new { success = false, message = "BPKNo Not Found" });
                }


                //if (ctx.Database.SqlQuery<OmMstModel>(string.Format("exec uspfn_omSlsBPKLkpMdlCode {0},{1},'{2}'",
                //       CompanyCode, BranchCode, hdr.DONo)).FirstOrDefault() == null)
                if(string.IsNullOrEmpty(mdl.SalesModelCode))
                {
                    return Json(new { success = false, browse = "slsmdlcode", message = "Sales Model Code cannot be null" });
                }


                if (ctx.Database.SqlQuery<MstModelYear>(string.Format("exec uspfn_omSlsBPKLkpMdlYear {0},{1},'{2}','{3}'",
                      CompanyCode, BranchCode, hdr.DONo, mdl.SalesModelYear)) == null)
                {
                    return Json(new { success = false, browse = "slsmdlyear", message = "Sales Model Year cannot be null" });
                }


                if (ctx.Database.SqlQuery<omSLsBPKLkpChasisNo>(string.Format("exec uspfn_omSlsBPKLkpChassisNo {0},{1},'{2}','{3}',{4}",
                     CompanyCode, BranchCode, hdr.DONo, mdl.BPKNo, mdl.ChassisNo)) == null)
                {
                    return Json(new { success = false, browse = "ChasisNo", message = "ChasisNo cannot be null" });
                }

                var dtl = ctx.OmTrSalesBPKDetails
                            .Where(x => x.CompanyCode == CompanyCode &&
                                    x.BranchCode == BranchCode &&
                                    x.BPKNo == mdl.BPKNo &&
                                    x.BPKSeq == mdl.BPKSeq &&
                                    x.ChassisNo == mdl.ChassisNo
                                    )
                         .FirstOrDefault();

                if (dtl == null)
                {
                    max = ctx.OmTrSalesBPKDetails
                       .Where(x => x.CompanyCode == CompanyCode &&
                                   x.BranchCode == BranchCode &&
                                   x.BPKNo == mdl.BPKNo)
                                   .Select(x => x.BPKSeq)
                                   .DefaultIfEmpty(0)
                                   .Max() + 1;


                    dtl = new OmTrSalesBPKDetail()
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        BPKNo = mdl.BPKNo,
                        BPKSeq = max,
                        SalesModelCode = mdl.SalesModelCode,
                        SalesModelYear = mdl.SalesModelYear,
                        StatusInvoice = "0",
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = ctx.CurrentTime
                    };
                    ctx.OmTrSalesBPKDetails.Add(dtl);
                }

                dtl.ChassisCode = mdl.ChassisCode;
                dtl.ChassisNo = mdl.ChassisNo;
                dtl.EngineCode = mdl.EngineCode;
                dtl.EngineNo = mdl.EngineNo;
                dtl.ColourCode = mdl.ColourCode;
                dtl.Remark = mdl.Remark == null ? "" : mdl.Remark;


                var ovhcl = cekOtomatis() ? ctxMD.OmMstVehicles.Find(CompanyMD, mdl.ChassisCode, mdl.ChassisNo) : ctx.OmMstVehicles.Find(CompanyMD, mdl.ChassisCode, mdl.ChassisNo);
                if (ovhcl != null)
                {
                    dtl.ServiceBookNo = ovhcl.ServiceBookNo;
                    dtl.KeyNo = ovhcl.KeyNo;
                    dtl.ReqOutNo = ovhcl.ReqOutNo;
                }

                if (dtl.StatusPDI != null)
                {
                    dtl.StatusPDI = (mdl.StatusPDI.ToLower() == "true") ? "1" : "0";
                }
                else
                {
                    dtl.StatusPDI = "0";
                }
                dtl.LastUpdateDate = ctx.CurrentTime;
                dtl.LastUpdateBy = CurrentUser.UserId;

                var dtlDo = ctx.OmTrSalesDODetails
                           .Where(x => x.CompanyCode == CompanyCode &&
                                   x.BranchCode == BranchCode &&
                                   x.DONo == hdr.DONo &&
                                   x.SalesModelYear == mdl.SalesModelYear &&
                                   x.ChassisCode == mdl.ChassisCode &&
                                   x.ChassisNo == mdl.ChassisNo
                                   )
                        .FirstOrDefault();

                if (dtlDo != null)
                {
                    dtlDo.StatusBPK = "1";
                }

                var bpkMdl = ctx.OmTrSalesBPKModels
                            .Where(x => x.CompanyCode == CompanyCode &&
                            x.BranchCode == BranchCode &&
                            x.BPKNo == hdr.BPKNo &&
                            x.SalesModelCode == mdl.SalesModelCode &&
                            x.SalesModelYear == mdl.SalesModelYear
                            ).FirstOrDefault();
                            
                if (bpkMdl != null)
                {
                    var qMdl = ctx.OmTrSalesBPKDetails
                       .Where(x => x.CompanyCode == CompanyCode &&
                                   x.BranchCode == BranchCode &&
                                   x.BPKNo == mdl.BPKNo &&
                                   x.SalesModelCode == mdl.SalesModelCode &&
                                   x.SalesModelYear == mdl.SalesModelYear
                                   ).Count();
                   
                    bpkMdl.QuantityBPK = qMdl; 
                    bpkMdl.LastUpdateBy = CurrentUser.UserId;
                    bpkMdl.LastUpdateDate = ctx.CurrentTime;
                }
                else
                {
                    bpkMdl = new OmTrSalesBPKModel()
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        BPKNo = mdl.BPKNo,
                        SalesModelCode = mdl.SalesModelCode,
                        SalesModelYear = (decimal)mdl.SalesModelYear,
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = ctx.CurrentTime,
                        QuantityBPK = 1,
                        QuantityInvoice = 0
                    };
                    ctx.OmTrSalesBPKModels.Add(bpkMdl);
                }

                Helpers.ReplaceNullable(bpkMdl);

                ctx.SaveChanges();

                //if (ctx.SaveChanges() > -1)
                //{
                //    //var hdr2 = ctx.OmTrSalesBPKs.Find(CompanyCode, BranchCode, mdl.BPKNo);
                //    //var statBpk = ctx.OmTrSalesDODetails.Find(CompanyCode, BranchCode, hdr2.DONo, );
                   
                //}

                return Json(new { success = true, hdrStatus = "0", hdrStatusDsc = getStringStatus("0"), BPKSeq = max });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }); ;
            }
        }

        public JsonResult DeleteDetail(OmTrSalesBPKDetail mdl)
        {
            try
            {
                var hdr = ctx.OmTrSalesBPKs.Find(CompanyCode, BranchCode, mdl.BPKNo);
                hdr.Status = "0";
                if (hdr == null)
                {
                    return Json(new { success = false, message = "BPKNo Not Found" });
                }


                var dtl = ctx.OmTrSalesBPKDetails
                       .Where(x => x.CompanyCode == CompanyCode &&
                                  x.BranchCode == BranchCode &&
                                  x.BPKNo == mdl.BPKNo &&
                                  x.BPKSeq == mdl.BPKSeq
                                  )
                       .FirstOrDefault();

                if (dtl != null)
                {
                    ctx.OmTrSalesBPKDetails.Remove(dtl);
                    ctx.SaveChanges();

                    var dodtl = ctx.OmTrSalesDODetails
                                .Where(x => x.CompanyCode == CompanyCode &&
                                            x.BranchCode == BranchCode &&
                                            x.DONo == hdr.DONo &&
                                            x.SalesModelCode == dtl.SalesModelCode &&
                                            x.ChassisCode == dtl.ChassisCode &&
                                            x.ChassisNo == dtl.ChassisNo)
                                .FirstOrDefault();
                    if (dodtl != null)
                    {
                        dodtl.StatusBPK = "0";
                        dodtl.LastUpdateBy = CurrentUser.UserId;
                        dodtl.LastUpdateDate = ctx.CurrentTime;
                    }

                    var bpkmdl = ctx.OmTrSalesBPKModels.Find(CompanyCode, BranchCode, hdr.BPKNo, dtl.SalesModelCode, dtl.SalesModelYear);

                    bool isNewModel = false;
                    if (bpkmdl == null)
                    {
                        isNewModel = true;
                        bpkmdl = new OmTrSalesBPKModel();
                        bpkmdl.CompanyCode = CompanyCode;
                        bpkmdl.BranchCode = BranchCode;
                        bpkmdl.BPKNo = hdr.BPKNo;
                        bpkmdl.SalesModelCode = dtl.SalesModelCode;
                        bpkmdl.SalesModelYear = (decimal)dtl.SalesModelYear;
                        bpkmdl.CreatedBy = CurrentUser.UserId;
                        bpkmdl.CreatedDate = ctx.CurrentTime;
                    }
                    bpkmdl.QuantityBPK = ctx.OmTrSalesBPKDetails
                                        .Where(x => x.CompanyCode == CompanyCode &&
                                                   x.BranchCode == BranchCode &&
                                                   x.BPKNo == hdr.BPKNo &&
                                                   x.SalesModelCode == dtl.SalesModelCode &&
                                                   x.SalesModelYear == dtl.SalesModelYear)
                                       .ToList()
                                       .Count();

                    bpkmdl.QuantityInvoice = 0;
                    bpkmdl.LastUpdateBy = CurrentUser.UserId;
                    bpkmdl.LastUpdateDate = ctx.CurrentTime;

                    var ovhcl = ctx.OmMstVehicles.Find(CompanyCode, dtl.ChassisCode, dtl.ChassisNo);
                    if (ovhcl != null)
                    {
                        ovhcl.BPKNo = "";
                        if (dtl.StatusPDI == "1")
                        {
                            ovhcl.IsAlreadyPDI = true;
                        }
                        else
                        {
                            ovhcl.IsAlreadyPDI = false;
                        }
                        ovhcl.Status = "4";
                        ovhcl.LastUpdateBy = CurrentUser.UserId;
                        ovhcl.LastUpdateDate = ctx.CurrentTime;
                        ovhcl.BPKDate = hdr.BPKDate;
                    }

                    //var dtlDo = ctx.OmTrSalesDODetails
                    //      .Where(x => x.CompanyCode == CompanyCode &&
                    //              x.BranchCode == BranchCode &&
                    //              x.DONo == hdr.DONo &&
                    //              x.SalesModelYear == mdl.SalesModelYear &&
                    //              x.ChassisCode == mdl.ChassisCode &&
                    //              x.ChassisNo == mdl.ChassisNo
                    //              )
                    //   .FirstOrDefault();

                    //if (dtlDo != null)
                    //{
                    //    dtlDo.StatusBPK = "0";
                    //}

                    //var bpkMdl = ctx.OmTrSalesBPKModels
                    //        .Where(x => x.CompanyCode == CompanyCode &&
                    //        x.BranchCode == BranchCode &&
                    //        x.BPKNo == hdr.BPKNo &&
                    //        x.SalesModelCode == mdl.SalesModelCode &&
                    //        x.SalesModelYear == mdl.SalesModelYear
                    //        ).FirstOrDefault();

                    //if (bpkMdl != null)
                    //{
                    //    if (bpkMdl.QuantityBPK > 0)
                    //    {
                    //        bpkMdl.QuantityBPK = bpkMdl.QuantityBPK - 1;
                    //        bpkMdl.LastUpdateBy = CurrentUser.UserId;
                    //        bpkMdl.LastUpdateDate = ctx.CurrentTime;
                    //    }
                    //    else
                    //    {
                    //        bpkMdl.QuantityBPK = 0;
                    //    }
                    //}

                    ctx.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "detail not found" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }

        }

    }
}
