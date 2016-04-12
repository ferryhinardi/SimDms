using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Sales.BLL;
using SimDms.Sales.Models;
using System.Transactions;
using SimDms.Common;

namespace SimDms.Sales.Controllers.Api
{
    public class SalesDOController : BaseController
    {
        public JsonResult Save(OmTrSalesDO mdl)
        {
            try
            {
                string serr = "";
                if (!DateTransValidation((DateTime)mdl.DODate, ref serr))
                {
                    return Json(new { success = false, message = serr });
                }


                //if (ctx.Database.SqlQuery<omSlsDOLkpSO>(string.Format("exec uspfn_omSlsDoLkpSO {0},{1},{2}", CompanyCode, BranchCode, "WARE"))
                //      .Where(x => x.SONo == mdl.SONo).Count() < 1)
                //{
                //    return Json(new { success = false, browse = "so", message = "SONo Not Valid " });
                //}


                //if (ctxMD.MstRefferences
                //    .Where(x => x.CompanyCode == CompanyCode &&
                //               x.RefferenceType == "WARE" &&
                //               x.RefferenceCode == mdl.WareHouseCode &&
                //               x.Status != "0")
                //    .FirstOrDefault() == null)
                //{
                //    return Json(new { success = false, browse = "wrh", message = "WareHouseCode Not valid" });
                //}

                bool isNew = false;
                var recordHdr = ctx.OmTrSalesDOs.Find(CompanyCode, BranchCode, mdl.DONo);
                if (recordHdr == null)
                {
                    isNew = true;
                    recordHdr = new OmTrSalesDO();
                    recordHdr.CompanyCode = CompanyCode;
                    recordHdr.BranchCode = BranchCode;
                    recordHdr.DODate = mdl.DODate;
                    recordHdr.DONo = GetNewDocumentNo("DOS", (DateTime)mdl.DODate);
                    recordHdr.CreatedBy = CurrentUser.UserId;
                    recordHdr.CreatedDate = ctx.CurrentTime;
                    recordHdr.isLocked = false;

                    ctx.OmTrSalesDOs.Add(recordHdr);
                }

                recordHdr.SONo = mdl.SONo;
                recordHdr.CustomerCode = mdl.CustomerCode;
                recordHdr.ShipTo = mdl.ShipTo;
                recordHdr.WareHouseCode = mdl.WareHouseCode;
                recordHdr.Expedition = mdl.Expedition;
                recordHdr.Remark = mdl.Remark;
                recordHdr.Status = "0";
                recordHdr.LastUpdateBy = CurrentUser.UserId;
                recordHdr.LastUpdateDate = ctx.CurrentTime;
                ctx.SaveChanges();
                return Json(new { success = true, DONo = recordHdr.DONo, Status = "0", StatusDsc = getStringStatus("0") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        public JsonResult Delete(OmTrSalesDO mdl)
        {
            try
            {
                var itm = ctx.OmTrSalesDOs.Find(CompanyCode, BranchCode, mdl.DONo);

                if (itm != null)
                {
                    itm.Status = "3";
                    itm.LastUpdateBy = CurrentUser.UserId;
                    itm.LastUpdateDate = ctx.CurrentTime;
                    ctx.SaveChanges();
                    return Json(new { success = true, message = "Deleted", Status = "3", StatusDsc = getStringStatus("3") }); ;
                }
                return Json(new { success = false, message = "DONo Not Found" }); ;

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }); ;
            }
        }

        public JsonResult Approve(string doNo)
        {
            bool result = false;
            bool independent = false;
            string msg = "";
            string Qry = "";
            string dbMD = ctx.Database.SqlQuery<string>("SELECT DbMD from gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'").FirstOrDefault();
            bool Otom = cekOtomatis();

            if ((CompanyCode == CompanyMD) && (BranchCode == BranchMD))
            {
                independent = true;
            }

            if ((dbMD == "" || dbMD == null) && (!independent))
            {
                return Json(new { success = false, message = "Approved DO gagal!! Database MD tidak ada silahkan cek gnMstCompanyMapping" });
            }

            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    var sobll = OmTrSalesBLL.Instance(CurrentUser.UserId);
                    var itm = ctx.OmTrSalesDOs.Find(CompanyCode, BranchCode, doNo);

                    if (itm != null)
                    {
                        if (sobll.UpdateSOVin(ctx, itm))
                        {
                            if (sobll.Update4ITS(ctx, itm.SONo, "DO"))
                            {
                                itm.Status = "2";
                                itm.LastUpdateBy = CurrentUser.UserId;
                                itm.LastUpdateDate = ctx.CurrentTime;
                                Helpers.ReplaceNullable(itm);

                                result = ctx.SaveChanges() > 0;
                                if (result)
                                {
                                    ctx.OmTrSalesDODetails
                                    .Where(x => x.CompanyCode == CompanyCode &&
                                        x.BranchCode == BranchCode &&
                                        x.DONo == itm.DONo)
                                    .ToList()
                                    .ForEach(x =>
                                    {
                                        if (result)
                                        {
                                            if (!independent)
                                            {
                                                if (Otom)
                                                {
                                                    Qry = "INSERT INTO " + dbMD + "..omSDMovement(CompanyCode, BranchCode, DocNo, DocDate, Seq, SalesModelCode, SalesModelYear, ChassisCode, ChassisNo, EngineCode," +
                                                      "EngineNo, ColourCode, WarehouseCode, CustomerCode, QtyFlag, CompanyMD, BranchMD, WarehouseMD, Status, ProcessStatus, ProcessDate, CreatedBy," +
                                                      "CreatedDate, LastUpdateBy, LastUpdateDate) Values('" +
                                                      itm.CompanyCode + "','" + itm.BranchCode + "','" + itm.DONo + "','" + itm.CreatedDate + "','" + x.DOSeq + "','" + x.SalesModelCode +
                                                      "','" + x.SalesModelYear + "','" + x.ChassisCode + "','" + x.ChassisNo + "','" + x.EngineCode + "','" + x.EngineNo +
                                                      "','" + x.ColourCode + "','" + itm.WareHouseCode + "','" + itm.CustomerCode + "','-','" + CompanyMD + "','" + UnitBranchMD + "','" + WarehouseMD +
                                                      "','" + itm.Status + "','0','" + DateTime.Now + "','" + CurrentUser.UserId + "','" + DateTime.Now + "','" + CurrentUser.UserId + "','" + DateTime.Now + "')";
                                                    result = ctx.Database.ExecuteSqlCommand(Qry) > 0;
                                                    if (!result)
                                                    {
                                                        msg += "Error : Gagal saat insert di " + dbMD + "..omSDMovement untuk ChassisCode : " + x.ChassisCode + " ChassisNo : " + x.ChassisNo;
                                                    }
                                                }
                                            }
                                            //update mstVehicle
                                            var oVhcl = ctx.OmMstVehicles.Find(CompanyCode, x.ChassisCode, x.ChassisNo);
                                            if (oVhcl == null && !independent)
                                            {
                                                Qry = "UPDATE " + dbMD + "..omMstVehicle SET status='4', DONo='" + x.DONo + "', LastUpdateBy='" + CurrentUser.UserId + "', LastUpdateDate='" + DateTime.Now +
                                                      "' WHERE CompanyCode='" + CompanyMD + "' AND ChassisCode='" + x.ChassisCode + "' AND ChassisNo='" + x.ChassisNo + "'";
                                            }
                                            else
                                            {
                                                Qry = "UPDATE omMstVehicle SET status='4', DONo='" + x.DONo + "', LastUpdateBy='" + CurrentUser.UserId + "', LastUpdateDate='" + DateTime.Now +
                                                      "' WHERE CompanyCode='" + CompanyCode + "' AND ChassisCode='" + x.ChassisCode + "' AND ChassisNo='" + x.ChassisNo + "'";
                                            }

                                            result = ctx.Database.ExecuteSqlCommand(Qry) > 0;
                                            if (!result)
                                            {
                                                msg += "Error : Gagal saat update di " + dbMD + "..omMstVehicle untuk ChassisCode : " + x.ChassisCode + " dan ChassisNo : " + x.ChassisNo;
                                            }                                            
                                        }

                                    });
                                }
                            }
                            else
                            {
                                result = false;
                            }

                            if(result && !independent)
                            {
                                if(Otom){
                                    int seq = 0;
                                    var qry = String.Empty;
                                    decimal pct = 100;
                                    decimal? CostPrice = 0;
                                    decimal? CostPriceMD = 0;
                                    decimal? RetailPriceMD = 0;
                                    decimal? RetailPriceIncTaxMD = 0;
                                    var dtSO = ctx.OmTrSalesDOs.Find(CompanyCode, BranchCode, doNo);
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
                                                else
                                                {
                                                    result = false;
                                                    return;
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
                                                        CompanyCode, BranchCode, doNo, DateTime.Now, y.PartNo, seq, WarehouseMD
                                                        , y.DemandQty.Value, y.Qty.Value, pct, CostPrice, y.RetailPrice.Value, y.TypeOfGoods, CompanyMD
                                                        , BranchMD, WarehouseMD, RetailPriceIncTaxMD, RetailPriceMD, CostPriceMD
                                                        , "-", ProductType, "300", "0", "0"
                                                        , DateTime.Now, CurrentUser.UserId, DateTime.Now, CurrentUser.UserId, DateTime.Now);

                                                result = ctx.Database.ExecuteSqlCommand(Qry) > 0;
                                                //result = true;
                                                if (!result)
                                                {
                                                    msg = "Insert svSDMovement gagal!";
                                                }
                                            }
                                        });
                                    }
                                }
                            }

                            if (result)
                            {
                                tranScope.Complete();
                                return Json(new { success = true, message = "Approved DO Berhasil", Status = "2", StatusDsc = getStringStatus("2") });
                            }
                            else
                            {
                                return Json(new { success = false, message = "Approve Gagal!" + msg });
                            }
                        }
                        else
                        {
                            return Json(new { success = false, message = "Update So Vin Gagal!" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "DONo Not Found!" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }); ;
            }
        }


        public JsonResult PrintDO(OmTrSalesDO mdl)
        {
            try
            {
                var itm = ctx.OmTrSalesDOs.Find(CompanyCode, BranchCode, mdl.DONo);

                if (itm != null)
                {
                    if (itm.Status == "0")
                    {
                        itm.Status = "1";
                        itm.LastUpdateBy = CurrentUser.UserId;
                        itm.LastUpdateDate = ctx.CurrentTime;
                        ctx.SaveChanges();
                    }
                    return Json(new { success = true, message = "Printed", Status = itm.Status, StatusDsc = getStringStatus(itm.Status) }); ;
                }
                return Json(new { success = false, message = "DONo Not Found" }); ;

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }); ;
            }

        }

        public JsonResult SaveDetail(OmTrSalesDODetail mdl)
        {
            int max = 1;
            try
            {
                var dtl = ctx.OmTrSalesDODetails
                         .Where(x => x.CompanyCode == CompanyCode &&
                                    x.BranchCode == BranchCode &&
                                    x.DONo == mdl.DONo &&
                                    x.DOSeq == mdl.DOSeq)
                         .FirstOrDefault();
                if (dtl == null)
                {
                    max = ctx.OmTrSalesDODetails
                       .Where(x => x.CompanyCode == CompanyCode &&
                                   x.BranchCode == BranchCode &&
                                   x.DONo == mdl.DONo)
                                   .Select(x => x.DOSeq)
                                   .DefaultIfEmpty(0)
                                   .Max() + 1;

                    dtl = new OmTrSalesDODetail()
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = ctx.CurrentTime,
                        DONo = mdl.DONo,
                        DOSeq = max
                    };
                    ctx.OmTrSalesDODetails.Add(dtl);
                }
                else
                {
                    dtl.LastUpdateDate = ctx.CurrentTime;
                    dtl.LastUpdateBy = CurrentUser.UserId;
                }

                dtl.ChassisNo = mdl.ChassisNo;
                dtl.ColourCode = mdl.ColourCode;
                dtl.ChassisCode = mdl.ChassisCode;
                dtl.EngineCode = mdl.EngineCode;
                dtl.EngineNo = mdl.EngineNo;
                dtl.LastUpdateBy = CurrentUser.UserId;
                dtl.LastUpdateDate = ctx.CurrentTime;
                dtl.Remark = mdl.Remark;
                dtl.SalesModelCode = mdl.SalesModelCode;
                dtl.SalesModelYear = mdl.SalesModelYear;
                dtl.StatusBPK = "0";

                //Add QuantityDo

                var hdr = ctx.OmTrSalesDOs.Find(CompanyCode, BranchCode, mdl.DONo);
                var soMdl = ctx.OmTrSalesSOModels
                           .Where(x => x.CompanyCode == CompanyCode &&
                                   x.BranchCode == BranchCode &&
                                   x.SONo == hdr.SONo &&
                                   x.SalesModelCode == dtl.SalesModelCode
                                   )
                        .FirstOrDefault();

                if (soMdl != null)
                {
                    soMdl.QuantityDO = soMdl.QuantityDO + 1;
                    soMdl.LastUpdateBy = CurrentUser.UserId;
                    soMdl.LastUpdateDate = ctx.CurrentTime;
                }

                //If not exist, Insert into SOVin
                /*
                string SoNo = "";
                string EndUserName = "";
                string EndUserAddress1 = "";
                string EndUserAddress2 = "";
                string EndUserAddress3 = "";

                var SoInfo = ctx.OmTrSalesDOs.Find(CompanyCode, BranchCode, mdl.DONo);
                if (SoInfo != null)
                {
                    SoNo = SoInfo.SONo;
                    var addrInfo = ctx.GnMstCustomer.Find(CompanyCode, SoInfo.CustomerCode);
                    if (addrInfo != null)
                    {
                        EndUserName = addrInfo.CustomerName == null ? "" : addrInfo.CustomerName;
                        EndUserAddress1 = addrInfo.Address1 == null ? "" : addrInfo.Address1;
                        EndUserAddress2 = addrInfo.Address2 == null ? "" : addrInfo.Address2;
                        EndUserAddress3 = addrInfo.Address3 == null ? "" : addrInfo.Address3;
                    }
                }
                
                var soVin = ctx.omTrSalesSOVins.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.SalesModelCode == mdl.SalesModelCode && x.SalesModelYear == mdl.SalesModelYear
                                           && x.ChassisCode == mdl.ChassisCode && x.ChassisNo == mdl.ChassisNo).FirstOrDefault();
                if (soVin == null)
                {
                    soVin = new omTrSalesSOVin();
                    soVin.CompanyCode = CompanyCode;
                    soVin.BranchCode = BranchCode;
                    soVin.SONo = SoNo;
                    soVin.SalesModelCode = mdl.SalesModelCode;
                    soVin.SalesModelYear = (decimal)mdl.SalesModelYear;
                    soVin.ColourCode = mdl.ColourCode;
                    soVin.SOSeq = mdl.DOSeq;
                    soVin.ChassisCode = mdl.ChassisCode;
                    soVin.ChassisNo = mdl.ChassisNo;
                    soVin.EngineCode = mdl.EngineCode;
                    soVin.EngineNo = mdl.EngineNo;
                    soVin.EndUserName = EndUserName;
                    soVin.EndUserAddress1 = EndUserAddress1;
                    soVin.EndUserAddress2 = EndUserAddress2;
                    soVin.EndUserAddress3 = EndUserAddress3;
                    soVin.CreatedBy = CurrentUser.UserId;
                    soVin.CreatedDate = DateTime.Now;
                    soVin.LastUpdateDate = DateTime.Now;
                    soVin.CreatedBy = CurrentUser.UserId;

                    ctx.omTrSalesSOVins.Add(soVin);
                }

                 */
                
                ctx.SaveChanges();
                return Json(new { success = true, DOSeq = max });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }); ;
            }
        }

        public JsonResult DeleteDetail(OmTrSalesDODetail mdl)
        {
            try
            {
                var dtl = ctx.OmTrSalesDODetails
                       .Where(x => x.CompanyCode == CompanyCode &&
                                  x.BranchCode == BranchCode &&
                                  x.DONo == mdl.DONo &&
                                  x.DOSeq == mdl.DOSeq)
                       .FirstOrDefault();

                if (dtl != null)
                {
                    ctx.OmTrSalesDODetails.Remove(dtl);

                    var hdr = ctx.OmTrSalesDOs.Find(CompanyCode, BranchCode, mdl.DONo);
                    var soMdl = ctx.OmTrSalesSOModels
                               .Where(x => x.CompanyCode == CompanyCode &&
                                       x.BranchCode == BranchCode &&
                                       x.SONo == hdr.SONo &&
                                       x.SalesModelCode == dtl.SalesModelCode
                                       )
                            .FirstOrDefault();

                    if (soMdl != null)
                    {
                        if (soMdl.QuantityDO > 0)
                        {
                            soMdl.QuantityDO = soMdl.QuantityDO - 1;
                            soMdl.LastUpdateBy = CurrentUser.UserId;
                            soMdl.LastUpdateDate = ctx.CurrentTime;
                        }
                    }

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
