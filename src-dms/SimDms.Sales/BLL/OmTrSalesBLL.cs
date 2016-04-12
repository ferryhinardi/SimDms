using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimDms.Sales.Models;
using TracerX;
using System.ComponentModel;
using System.Data;
using SimDms.Common;

namespace SimDms.Sales.BLL
{
    public class OmTrSalesBLL : BaseBLL
    {
        #region -- Initiate --
        /// <summary>
        /// 
        /// </summary>
        private static OmTrSalesBLL _OmTrSalesBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_username"></param>
        /// <returns></returns>
        public static OmTrSalesBLL Instance(string _username)
        {
            //if (_OmTrPurchaseBPUBLL == null)
            //{
            _OmTrSalesBLL = new OmTrSalesBLL();
            //}
            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
           //}
            return _OmTrSalesBLL;
        }
        #endregion


        #region SO
        public IQueryable<Leasing> Select4Leasing()
        {
            var records = from a in ctx.GnMstCustomer
                          join b in ctx.CustomerProfitCenters on new { a.CompanyCode, a.CustomerCode }
                          equals new { b.CompanyCode, b.CustomerCode }
                          where a.CompanyCode == CompanyCode && b.BranchCode == BranchCode
                          && b.ProfitCenterCode == ProfitCenter
                          && a.CategoryCode == "32"
                          select new Leasing()
                          {
                              LeasingCode = a.CustomerCode,
                              LeasingName = a.CustomerName
                          };

            return records;
        }

        public IQueryable<ITSNoDraftSO> SelectITSNoDraftSO()
        {
            string sql = string.Format(@"
            select a.CompanyCode,a.BranchCode,convert(varchar,a.InquiryNumber) InquiryNo,a.InquiryDate,b.EmployeeName,a.NamaProspek,a.TipeKendaraan
                ,a.EmployeeID
            from pmKDP a
            	left join gnMstEmployee b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
            		and a.EmployeeID=b.EmployeeID
            where a.CompanyCode= '{0}'
            	and a.BranchCode= '{1}'
                and a.LastProgress in ('P', 'HP', 'SPK')
                and not exists (select 1 from OmTrSalesDraftSO where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and ProspectNo = a.InquiryNumber)
            ", CompanyCode, BranchCode);
            var records = ctx.Database.SqlQuery<ITSNoDraftSO>(sql);

            return records.AsQueryable();
        }
        #endregion


        #region DO
        public bool UpdateSOVin(DataContext ctxx, OmTrSalesDO record)
        {
            //ctx.omTrSalesSOVins.
            //var md = DealerCode() == "MD";
            //string Qry = "";
            //string dbMD = ctx.Database.SqlQuery<string>("SELECT DbMD from gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'").FirstOrDefault();

            bool result = false;

            var ovin = ctxx.Database.SqlQuery<omSlsDOUpdateSOVin>(string.Format("exec uspfn_omSlsDoUpdateSOVin {0},{1},'{2}'", CompanyCode, BranchCode, record.DONo)).ToList();
            ovin.ForEach(x =>
            {
                //var ovhcl = md ? ctxMD.OmMstVehicles.Find(CompanyMD, x.ChassisCode, x.ChassisNo) : ctx.OmMstVehicles.Find(CompanyCode, x.ChassisCode, x.ChassisNo);
                //var ovhcl = ctxx.OmMstVehicles.Find(CompanyMD, x.ChassisCode, x.ChassisNo);
                //if (ovhcl == null)
                //{
                //    ovhcl = ctxx.Database.SqlQuery<OmMstVehicle>("SELECT * FROM " + dbMD + "..omMstVehicle WHERE CompanyCode='" + CompanyMD + "' AND ChassisCode='" + x.ChassisCode + "' AND ChassisNo='" + x.ChassisNo + "'").FirstOrDefault();
                //}

                //if (ovhcl != null)
                //{
                //    if (ovhcl.Status != "0" && ovhcl.Status != "3")
                //        throw new Exception("Untuk kendaraan dengan ChassisCode= " + x.ChassisCode + " dan ChassisNo= " + x.ChassisNo.ToString()
                //                + " tidak berstatus Ready atau SO");

                    
                //    ovhcl.Status = "4";
                //    ovhcl.DONo = record.DONo;//x.DONo.ToString();
                //    ovhcl.LastUpdateBy = username;
                //    ovhcl.LastUpdateDate = ctx.CurrentTime;
                //    try
                //    {
                //        //if (md) { ctxMD.SaveChanges(); }
                //        //else
                //        //{
                //            ctxMD.SaveChanges();
                //        //}
                //    }
                //    catch (Exception ex)
                //    {
                //        throw new Exception("Untuk kendaraan dengan ChassisCode= " + x.ChassisCode + " dan ChassisNo= " + x.ChassisNo.ToString()
                //                + " gagal diupdate di Master Kendaraan");
                //    }

                if (result = true)
                {
                    var mdlvin = ctx.omTrSalesSOVins
                            .Where(y => y.CompanyCode == CompanyCode &&
                                        y.BranchCode == BranchCode &&
                                        y.SONo == x.SONo &&
                                        y.SalesModelCode == x.SalesModelCode &&
                                        y.SalesModelYear == x.SalesModelYear &&
                                        y.ColourCode == x.ColourCode)
                            .FirstOrDefault();
                    if (mdlvin != null)
                    {
                        mdlvin.ChassisNo = x.ChassisNo;
                        mdlvin.EngineCode = x.EngineCode;
                        mdlvin.EngineNo = x.EngineNo;
                        mdlvin.ServiceBookNo = x.ServiceBookNo;
                        mdlvin.KeyNo = x.KeyNo;
                        mdlvin.LastUpdateBy = username;
                        mdlvin.LastUpdateDate = ctx.CurrentTime;
                    }

                    try
                    {
                        ctxx.SaveChanges();
                        result = true;
                        //if(md) SDMovementDO(ovhcl, record);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Update SO Vin untuk kendaraan ChassisCode=" + x.ChassisCode + " ChassisNo=" + x.ChassisNo + " gagal");
                        result = false;
                    }
                }

                //}
                //else
                //{
                //    throw new Exception("Untuk kendaraan dengan ChassisCode= " + x.ChassisCode + " dan ChassisNo= " + x.ChassisNo.ToString()
                //                + " belum menjadi stock");
                //}
            });
            return result;
        }
        #endregion

        #region BPK
        public bool insertAllBPK(OmTrSalesBPK hdr)
        {
            int max = 0;
            ctx.OmTrSalesDODetails
            .Where(x => x.CompanyCode == CompanyCode &&
                        x.BranchCode == BranchCode &&
                        x.DONo == hdr.DONo &&
                        x.StatusBPK == "0")
             .ToList()
             .ForEach(y =>
             {
                 max = ctx.OmTrSalesBPKDetails
                    .Where(x => x.CompanyCode == CompanyCode &&
                                x.BranchCode == BranchCode &&
                                x.BPKNo == hdr.BPKNo)
                                .Select(x => x.BPKSeq)
                                .DefaultIfEmpty(0)
                                .Max() + 1;


                 var dtl = new OmTrSalesBPKDetail()
                 {
                     CompanyCode = y.CompanyCode,
                     BranchCode = y.BranchCode,
                     BPKNo = hdr.BPKNo,
                     BPKSeq = max,
                     SalesModelCode = y.SalesModelCode,
                     SalesModelYear = y.SalesModelYear,
                     CreatedDate = ctx.CurrentTime,
                     CreatedBy = username,
                     ChassisCode = y.ChassisCode,
                     ChassisNo = y.ChassisNo,
                     EngineCode = y.EngineCode,
                     EngineNo = y.EngineNo,
                     StatusInvoice = "0",
                     StatusPDI = "1",
                     ColourCode = y.ColourCode,
                     Remark = y.Remark
                 };
                 var vhcl = ctxMD.OmMstVehicles.Find(CompanyMD, y.ChassisCode, y.ChassisNo);
                 if (vhcl != null)
                 {
                     dtl.ServiceBookNo = vhcl.ServiceBookNo;
                     dtl.KeyNo = vhcl.KeyNo;
                     dtl.ReqOutNo = vhcl.ReqOutNo;
                 }
                 dtl.StatusPDI = "0";
                 dtl.LastUpdateBy = username;


                 ctx.OmTrSalesBPKDetails.Add(dtl);
                 ctx.SaveChanges();

                 var dodtl = ctx.OmTrSalesDODetails
                     .Where(z => z.CompanyCode == CompanyCode &&
                              z.BranchCode == BranchCode &&
                              z.DONo == hdr.DONo &&
                              z.SalesModelCode == dtl.SalesModelCode &&
                              z.ChassisCode == dtl.ChassisCode &&
                              z.ChassisNo == dtl.ChassisNo)
                       .FirstOrDefault();
                 if (dodtl != null)
                 {
                     dodtl.StatusBPK = "1";
                     dodtl.LastUpdateBy = username;
                     dodtl.LastUpdateDate = ctx.CurrentTime;
                 }


                 var bpkmdl = ctx.OmTrSalesBPKModels
                             .Find(CompanyCode, BranchCode, hdr.BPKNo, dtl.SalesModelCode, dtl.SalesModelYear);
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
                     bpkmdl.CreatedBy = username;
                     bpkmdl.CreatedDate = ctx.CurrentTime;


                     //var gatbpk = ctx.OmTrSalesBPKDetails
                     //      .Where(z => z.CompanyCode == CompanyCode &&
                     //         z.BranchCode == BranchCode &&
                     //         z.BPKNo == hdr.BPKNo &&
                     //         z.SalesModelCode == dtl.SalesModelCode &&
                     //         z.SalesModelYear == dtl.SalesModelYear)
                     //         .Count(z => z.BPKSeq > 1);

                     bpkmdl.QuantityBPK = ctx.OmTrSalesBPKDetails
                                           .Where(z => z.CompanyCode == CompanyCode &&
                                              z.BranchCode == BranchCode &&
                                              z.BPKNo == hdr.BPKNo &&
                                              z.SalesModelCode == dtl.SalesModelCode &&
                                              z.SalesModelYear == dtl.SalesModelYear)
                                              .Count();
                     bpkmdl.QuantityInvoice = 0;
                     bpkmdl.CreatedBy = username;
                     bpkmdl.CreatedDate = ctx.CurrentTime;

                     ctx.OmTrSalesBPKModels.Add(bpkmdl);
                     ctx.SaveChanges();
                 }else{
                      //var gatbpk = ctx.OmTrSalesBPKDetails
                      //     .Where(z => z.CompanyCode == CompanyCode &&
                      //        z.BranchCode == BranchCode &&
                      //        z.BPKNo == hdr.BPKNo &&
                      //        z.SalesModelCode == dtl.SalesModelCode &&
                      //        z.SalesModelYear == dtl.SalesModelYear)
                      //        .Count(z => z.BPKSeq > 1);

                 bpkmdl.QuantityBPK = ctx.OmTrSalesBPKDetails
                                       .Where(z => z.CompanyCode == CompanyCode &&
                                          z.BranchCode == BranchCode &&
                                          z.BPKNo == hdr.BPKNo &&
                                          z.SalesModelCode == dtl.SalesModelCode &&
                                          z.SalesModelYear == dtl.SalesModelYear)
                                          //.Count(z => z.BPKSeq > 1)
                                          .Count();
                 bpkmdl.QuantityInvoice = 0;
                 bpkmdl.LastUpdateBy = username;
                 bpkmdl.LastUpdateDate = ctx.CurrentTime;
                 ctx.SaveChanges();
                 }
             });

            //ctx.SaveChanges();

            return true;
        }

        public bool insertDtlBpk(OmTrSalesBPKDetail record, bool isNew, string DONo, string SONo, string salesModelCode, decimal salesModelYear,
           OmTrSalesBPK oOmTrSalesBPK, string ChassisCode, decimal ChassisNo)
        {

            return true;
        }


        public bool deleteBPk(OmTrSalesBPK hdr)
        {

            ctx.OmTrSalesBPKDetails
                .Where(x => x.CompanyCode == CompanyCode &&
                           x.BranchCode == BranchCode &&
                           x.BPKNo == hdr.BPKNo)
                .ToList()
                .ForEach(dtl =>
                {
                    var bpkmdl = ctx.OmTrSalesBPKModels
                                 .Find(CompanyCode, BranchCode, hdr.BPKNo, dtl.SalesModelCode, dtl.SalesModelYear);

                    if (bpkmdl != null)
                    {
                        bpkmdl.QuantityBPK = bpkmdl.QuantityBPK - 1;
                        bpkmdl.LastUpdateBy = username;
                        bpkmdl.LastUpdateDate = ctx.CurrentTime;
                    }

                    var dodtl = ctx.OmTrSalesDODetails
                               .Where(y => y.CompanyCode == CompanyCode &&
                                          y.BranchCode == BranchCode &&
                                          y.DONo == hdr.DONo &&
                                          y.SalesModelCode == dtl.SalesModelCode &&
                                          y.ChassisCode == dtl.ChassisCode &&
                                          y.ChassisNo == dtl.ChassisNo)
                               .FirstOrDefault();

                    if (dodtl != null)
                    {
                        dodtl.StatusBPK = "0";
                        dodtl.LastUpdateBy = username;
                        dodtl.LastUpdateDate = ctx.CurrentTime;
                    }

                    ctx.OmTrSalesBPKDetails.Remove(dtl);
                });

            ctx.SaveChanges();
            return true;
        }

        public bool ProcessApproveBPK(OmTrSalesBPK hdr)
        {
            ctx.OmTrSalesBPKDetails
                         .Where(x => x.CompanyCode == CompanyCode &&
                                     x.BranchCode == BranchCode &&
                                     x.BPKNo == hdr.BPKNo)
                         .ToList()
                         .ForEach(dtl =>
                         {
                             var ovhcl = ctx.OmMstVehicles
                                     .Find(CompanyCode, dtl.ChassisCode, dtl.ChassisNo);
                             if (ovhcl != null)
                             {
                                 if (ovhcl.Status != "4")
                                 {
                                     throw new Exception("Untuk kendaraan dengan ChassisCode= " + ovhcl.ChassisCode + " dan ChassisNo= " + ovhcl.ChassisNo.ToString() + " tidak berstatus DO");
                                 }
                                 ovhcl.BPKNo = hdr.BPKNo;
                                 if (dtl.StatusPDI == "1")
                                 {
                                     ovhcl.IsAlreadyPDI = true;
                                 }
                                 else
                                 {
                                     ovhcl.IsAlreadyPDI = false;
                                 }
                                 ovhcl.Status = "5";
                                 ovhcl.LastUpdateBy = username;
                                 ovhcl.LastUpdateDate = ctx.CurrentTime;
                                 ovhcl.BPKDate = hdr.BPKDate;
                                 ctx.SaveChanges();
                             }
                             else
                             {
                                 throw new Exception("Untuk kendaraan dengan ChassisCode= " + ovhcl.ChassisCode + " dan ChassisNo= " + ovhcl.ChassisNo.ToString() + " belum menjadi stock");
                             }



                         });

            return true;


        }

        public bool Update4ITS(DataContext ctxx, string soNo, string lprogress)
        {
            bool result = false;
            int prospectNo = 0;
            var oOmTrSalesSO = ctx.OmTRSalesSOs.Where (x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.SONo == soNo).FirstOrDefault();
            if (oOmTrSalesSO != null)
            {
                if (oOmTrSalesSO.ProspectNo != "")
                {
                    prospectNo = Convert.ToInt32(oOmTrSalesSO.ProspectNo);
                    var pmKDP = ctxx.PmKdps.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.InquiryNumber == prospectNo).FirstOrDefault();
                    if (pmKDP != null)
                    {
                        pmKDP.LastProgress = lprogress;
                        pmKDP.LastUpdateStatus = DateTime.Now;
                        pmKDP.LastUpdateBy = CurrentUser.UserId;
                        pmKDP.LastUpdateDate = DateTime.Now;

                        result = ctxx.SaveChanges() > 0;

                        if (result)
                        {
                            PmStatusHistory oPmStatusHistory = new PmStatusHistory();
                            oPmStatusHistory.CompanyCode = CompanyCode;
                            oPmStatusHistory.BranchCode = BranchCode;
                            oPmStatusHistory.InquiryNumber = pmKDP.InquiryNumber;
                            oPmStatusHistory.SequenceNo = GetSeqHistPm(ctxx, CompanyCode, BranchCode, pmKDP.InquiryNumber) + 1;
                            oPmStatusHistory.LastProgress = lprogress;
                            oPmStatusHistory.UpdateDate = DateTime.Now;
                            oPmStatusHistory.UpdateUser = CurrentUser.UserId;

                            ctxx.PmStatusHistories.Add(oPmStatusHistory);

                            result = ctxx.SaveChanges() > 0;
                        }
                    }
                }else{ result = true; }
            } else { result = true; }

            return result;
        }

        private static int GetSeqHistPm(DataContext ctxx, string companyCode, string branchCode, int inquiryNumber)
        {
            string qry = "SELECT TOP 1 SequenceNo from PmStatusHistory WHERE CompanyCode = '" + companyCode + "' AND BranchCode = '" + branchCode + 
                         "' AND InquiryNumber = '" + inquiryNumber + "' ORDER BY SequenceNo DESC";
            int seqNo = ctxx.Database.SqlQuery<int>(qry).FirstOrDefault();     
            if (seqNo > 0 || seqNo != null)
            {
                return seqNo;
            }
            else
            {
                return 0;
            }
        }


        //public bool Update4ITS(string soNo)
        //{
        //    var slsso = ctx.OmTRSalesSOs.Find(CompanyCode, BranchCode, soNo);
        //    bool result = false;
        //    int prospectNo = 0;
        //    if (slsso != null)
        //    {
        //        if (slsso.ProspectNo != "")
        //            prospectNo = Convert.ToInt32(slsso.ProspectNo);

        //        var pmKDP = ctx.PmKdps.Find(prospectNo, CompanyCode, BranchCode);
        //        if (pmKDP != null)
        //        {
        //            pmKDP.LastProgress = "DELIVERY";
        //            pmKDP.LastUpdateStatus = ctx.CurrentTime;
        //            pmKDP.LastUpdateBy = username;
        //            pmKDP.LastUpdateDate = ctx.CurrentTime;

        //            result = ctx.SaveChanges() > 0;



        //            if (result)
        //            {
        //                PmStatusHistory oPmStatusHistory = new PmStatusHistory();
        //                oPmStatusHistory.CompanyCode = CompanyCode;
        //                oPmStatusHistory.BranchCode = BranchCode;
        //                oPmStatusHistory.InquiryNumber = pmKDP.InquiryNumber;

        //                var i = ctx.PmStatusHistories
        //                    .OrderByDescending(x => x.SequenceNo)
        //                    .FirstOrDefault();
        //                int sq = (i == null ? 0 : i.SequenceNo) + 1;
        //                oPmStatusHistory.SequenceNo = sq;
        //                oPmStatusHistory.LastProgress = "DELIVERY";
        //                oPmStatusHistory.UpdateDate = ctx.CurrentTime;
        //                oPmStatusHistory.UpdateUser = username;
        //                ctx.PmStatusHistories.Add(oPmStatusHistory);
        //                result = ctx.SaveChanges() > 0;
        //            }
        //        }
        //        else
        //        {
        //            result = true;
        //        }
        //    }
        //    return result;

        //}
        #endregion

        #region perlengkapan out

        public IQueryable<OmMstModel> Select4LookupModel(omTrSalesPerlengkapanOut hdr)
        {
            string squery = @"
                            SELECT distinct a.* 
                            FROM omMstModel a ";
            if (hdr.PerlengkapanType == "1")
            {
                squery += @" INNER JOIN omTrSalesBPKModel b
                        ON b.CompanyCode = a.CompanyCode
                        AND b.SalesModelCode = a.SalesModelCode	";
            }

            else if (hdr.PerlengkapanType == "2")
            {
                squery += @" INNER JOIN OmTrInventTransferOutDetail b 
                        ON b.CompanyCode = a.CompanyCode
                        AND b.SalesModelCode = a.SalesModelCode	";
            }
            else if (hdr.PerlengkapanType == "3")
            {
                squery += @" INNER JOIN omTrPurchaseReturnDetailModel b 
                        ON b.CompanyCode = a.CompanyCode
                        AND b.SalesModelCode = a.SalesModelCode	";
            }
            squery += " WHERE a.CompanyCode = {0} and b.BranchCode = {1} ";

            if (hdr.PerlengkapanType == "1")
                squery += " AND b.BPKNo = '{2}' ";
            else if (hdr.PerlengkapanType == "2")
                squery += " AND b.TransferOutNo = '{2}' ";
            else if (hdr.PerlengkapanType == "3")
                squery += " AND b.ReturnNo = '{2}' ";

            squery += " ORDER BY a.SalesModelCode ASC ";

            squery = string.Format(squery, CompanyCode, BranchCode, hdr.SourceDoc);

            return ctx.Database.SqlQuery<OmMstModel>(squery).AsQueryable();
        }

        public IQueryable<OmTrSalesPerlengkapanOutDetail> CekPerlengkapan(OmTrSalesPerlengkapanOutModel mdl)
        {
            return ctx.OmTrSalesPerlengkapanOutDetails
                    .Where(x => x.CompanyCode == CompanyCode &&
                                x.BranchCode == BranchCode &&
                                x.PerlengkapanNo == mdl.PerlengkapanNo &&
                                x.SalesModelCode == mdl.SalesModelCode).AsQueryable();

        }

        public bool plkpDeleteAll( omTrSalesPerlengkapanOut hdr)
        {
            ctx.OmTrSalesPerlengkapanOutDetails
            .Where(x => x.CompanyCode == CompanyCode &&
                        x.BranchCode == BranchCode &&
                        x.PerlengkapanNo == hdr.PerlengkapanNo)
            .ToList()
            .ForEach(plkpdtl =>
            {
                var invQtyPerlengkapan = ctx.OMTrInventQtyPerlengkapan
                                            .Find(CompanyCode, BranchCode, ((DateTime)hdr.PerlengkapanDate).Year, ((DateTime)hdr.PerlengkapanDate).Month, plkpdtl.PerlengkapanCode);

                if (invQtyPerlengkapan != null)
                {
                    invQtyPerlengkapan.QuantityOut -= plkpdtl.Quantity;
                    invQtyPerlengkapan.QuantityEnding = (invQtyPerlengkapan.QuantityBeginning +
                                        invQtyPerlengkapan.QuantityIn) - invQtyPerlengkapan.QuantityOut;
                    invQtyPerlengkapan.LastUpdateBy = CurrentUser.UserId;
                    invQtyPerlengkapan.LastUpdateDate = ctx.CurrentTime;                   
                }

                ctx.OmTrSalesPerlengkapanOutDetails.Remove(plkpdtl);                   
            });


             ctx.OmTrSalesPerlengkapanOutModels
            .Where(x => x.CompanyCode == CompanyCode &&
            x.BranchCode == BranchCode &&
            x.PerlengkapanNo == hdr.PerlengkapanNo)
            .ToList()
            .ForEach(dtmod =>ctx.OmTrSalesPerlengkapanOutModels.Remove(dtmod));            

             ctx.SaveChanges();
             return true;
         

        }

        public bool plkpdtlApprove(omTrSalesPerlengkapanOut hdr)
        {
            ctx.OmTrSalesPerlengkapanOutModels
                .Where(x => x.CompanyCode == CompanyCode &&
                            x.BranchCode == BranchCode &&
                            x.PerlengkapanNo == hdr.PerlengkapanNo)
                .ToList()
                .ForEach(mdl =>
                {
                    ctx.OmTrSalesPerlengkapanOutDetails
                        .Where(x => x.CompanyCode == CompanyCode &&
                            x.BranchCode == BranchCode &&
                             x.PerlengkapanNo == hdr.PerlengkapanNo &&
                             x.SalesModelCode == mdl.SalesModelCode)
                             .ToList()
                             .ForEach(dtl => { 
                                 var tmpdt=(DateTime)hdr.PerlengkapanDate;
                                 var inv = ctx.OMTrInventQtyPerlengkapan.Find(CompanyCode, BranchCode, tmpdt.Year, tmpdt.Month, dtl.PerlengkapanCode);
                                 if (inv != null)
                                 {
                                     inv.QuantityOut = inv.QuantityOut + dtl.Quantity;
                                     inv.QuantityEnding = (inv.QuantityBeginning + inv.QuantityIn) - inv.QuantityOut; ;
                                     if (inv.QuantityEnding < 0)
                                     {
                                         var max = dtl.Quantity + inv.QuantityEnding;
                                         throw new Exception("Input Jumlah Maksimal"+max+" !");
                                     }
                                     else
                                     {
                                         inv.LastUpdateBy = CurrentUser.UserId;
                                         inv.LastUpdateDate = ctx.CurrentTime;
                                         ctx.SaveChanges();
                                     }
                                 }
                             });
                });
            return true;
        }

       
        #endregion

        #region Invoice
        public bool isBPKDateOK(DataContext ctxx, omTrSalesInvoiceBPK mdl)
        {
            Func<string, DateTime?> GetBPKDate = x => {
                var b = ctxx.OmTrSalesBPKs.Find(CompanyCode, BranchCode, x);
                if (b != null)
                {
                    return b.BPKDate;
                }
                return null;               
            };

            string scomp = CompanyCode;

            bool result = true;
            var rowInvBPK = ctxx.omTrSalesInvoiceBPK
                     .Where(x => x.CompanyCode == CompanyCode &&
                                x.BranchCode == BranchCode &&
                                x.InvoiceNo == mdl.InvoiceNo &&
                                x.BPKNo != mdl.BPKNo)
                     .FirstOrDefault();

            if (rowInvBPK != null)
            {
                var date1 = GetBPKDate(rowInvBPK.BPKNo);
                var date2 = GetBPKDate(mdl.BPKNo);
                if (date1 != null && date2 != null)
                {
                    if (Convert.ToDateTime(date1).Date.Equals(Convert.ToDateTime(date2).Date))
                        result = true;
                    else result = false;
                }
                else result = false;
            }
            return result;
        }

        public bool InvInsertAllBPK(DataContext ctxx, omTrSalesInvoiceBPK mdl, OmTrSalesInvoice record)
        {

            bool result = true;
            string scomp = CompanyCode;
            string strUID = "";

            ctxx.OmTrSalesBPKModels
            .Where(x => x.CompanyCode == CompanyCode &&
                        x.BranchCode == BranchCode &&
                        x.BPKNo == mdl.BPKNo &&
                        (x.QuantityBPK - x.QuantityInvoice) > 0)
            .ToList()
            .ForEach(row => {
               var recordInvModel = new omTrSalesInvoiceModel();
               recordInvModel.CompanyCode = CompanyCode;
               recordInvModel.BranchCode = BranchCode;
               recordInvModel.InvoiceNo = record.InvoiceNo;
               recordInvModel.BPKNo = mdl.BPKNo;
               recordInvModel.SalesModelCode = row.SalesModelCode;
               recordInvModel.SalesModelYear = row.SalesModelYear;
               strUID = MyLogger.GetCRC32(CompanyCode + BranchCode + record.InvoiceNo + mdl.BPKNo + row.SalesModelCode + Convert.ToString(row.SalesModelYear)) + CurrentUser.UserId;
               if (strUID.Length > 15)
               {
                   recordInvModel.CreatedBy = strUID.Substring(0, 15);
               }
               else
               {
                   recordInvModel.CreatedBy = strUID;
               }
               recordInvModel.CreatedDate = ctxx.CurrentTime;

               recordInvModel.Quantity = row.QuantityBPK - row.QuantityInvoice;

               var so = ctxx.OmTrSalesSOModels.Find(CompanyCode, BranchCode, record.SONo, row.SalesModelCode, row.SalesModelYear);
               if (so != null)
               {
                   recordInvModel.AfterDiscDPP = so.AfterDiscDPP;
                   recordInvModel.AfterDiscPPn = so.AfterDiscPPn;
                   recordInvModel.AfterDiscPPnBM = so.AfterDiscPPnBM;
                   recordInvModel.AfterDiscTotal = so.AfterDiscTotal;
                   recordInvModel.BeforeDiscDPP = so.BeforeDiscDPP;
                   recordInvModel.DiscExcludePPn = so.DiscExcludePPn;
                   recordInvModel.DiscIncludePPn = so.DiscIncludePPn;
                   recordInvModel.OthersDPP = so.OthersDPP;
                   recordInvModel.OthersPPn = so.OthersPPn;
                   recordInvModel.ShipAmt = so.ShipAmt;
                   recordInvModel.DepositAmt = so.DepositAmt;
                   recordInvModel.OthersAmt = so.OthersAmt;
               }

               recordInvModel.QuantityReturn = 0;               
               recordInvModel.LastUpdateBy = CurrentUser.UserId;
               recordInvModel.LastUpdateDate = ctxx.CurrentTime;

               if (!SaveInvModel(ctxx, recordInvModel, record, true))
               {
                   result = false;                    
               }
            });
            
            return result;
        }
        
        public bool SaveInvModel(DataContext ctxx, omTrSalesInvoiceModel recordInvModel, OmTrSalesInvoice record, bool isNew)
        {            
           
            record.Status = "0";
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = ctxx.CurrentTime;
            
            bool result = false;
            //update OmTrSalesBpkModel
            var bpkModel = ctxx.OmTrSalesBPKModels.Find(CompanyCode, BranchCode, recordInvModel.BPKNo, recordInvModel.SalesModelCode, recordInvModel.SalesModelYear);
            if (bpkModel != null)
            {
                bpkModel.QuantityInvoice = recordInvModel.Quantity;
                bpkModel.LastUpdateBy = CurrentUser.UserId;
                bpkModel.LastUpdateDate = ctxx.CurrentTime;
            }
            //--
            if (isNew)
            {
                ctxx.omTrSalesInvoiceModel.Add(recordInvModel);
                if (ctxx.SaveChanges() > 0)
                {
                    ctxx.OmTrSalesBPKDetails
                    .Where(x => x.CompanyCode == CompanyCode &&
                                x.BranchCode == BranchCode &&
                                x.BPKNo == recordInvModel.BPKNo &&
                                x.SalesModelCode == recordInvModel.SalesModelCode &&
                                x.SalesModelYear == recordInvModel.SalesModelYear)                                
                    .ToList()
                    .ForEach(x=>{
                        x.StatusInvoice="1"; 
                        ctxx.SaveChanges();
                    });
                    if (InsertInvoiceVin(ctxx, recordInvModel, record))
                    {
                        if (InsertInvoiceOther(ctxx, recordInvModel,record.SONo))
                        {
                            result = true;
                        }
                    }               
                }
            }
            else
            {
                result = ctxx.SaveChanges() > 0;
            }
            return result;
        }

        private bool InsertInvoiceVin(DataContext ctxx, omTrSalesInvoiceModel mdl, OmTrSalesInvoice inv)
        {
            bool result = true;
            bool independent = false;
            bool otom = cekOtomatis();

            if (CompanyCode == CompanyMD && BranchCode == BranchMD)
            {
                independent = true;
            }

            var rows = ctxx.OmTrSalesBPKDetails
                .Where(x => x.CompanyCode == CompanyCode &&
                                x.BranchCode == BranchCode &&
                                x.BPKNo == mdl.BPKNo &&
                                x.SalesModelCode == mdl.SalesModelCode &&
                                x.SalesModelYear == mdl.SalesModelYear)
                .ToList();

            foreach (var row in rows)
            {
                var vin = new omTrSalesInvoiceVin();
                vin.CompanyCode = CompanyCode;
                vin.BranchCode = BranchCode;
                vin.InvoiceNo = mdl.InvoiceNo;
                vin.BPKNo = mdl.BPKNo;
                vin.SalesModelCode = mdl.SalesModelCode;
                vin.SalesModelYear = mdl.SalesModelYear;

                vin.InvoiceSeq = row.BPKSeq;
                vin.ColourCode = row.ColourCode;
                vin.ChassisCode = row.ChassisCode;
                vin.ChassisNo = row.ChassisNo;
                vin.EngineCode = row.EngineCode;
                vin.EngineNo = row.EngineNo;


                //var vehicle = ctxMD.OmMstVehicles.Find(CompanyMD, vin.ChassisCode, vin.ChassisNo);

                //
                var qr = "SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'";
                string DBMD = ctxx.Database.SqlQuery<string>(qr).FirstOrDefault();
                //
                var vehicle = new MstVehicleLookup();
                decimal cogs = 0;
                if (!independent && otom)
                {
                    if (DealerCode() == "SD")
                    {
                        //var supplier = ctxMD.OrganizationDtls.FirstOrDefault(a => a.IsBranch == false).BranchCode;
                        string supplier = ctxx.Database.SqlQuery<string>("SELECT BranchCode FROM " + DBMD + "..gnMstOrganizationDtl WHERE IsBranch = 'false'").FirstOrDefault();
                        var groupPrice = "W" + inv.InvoiceDate.Value.ToString("yy") + inv.InvoiceDate.Value.ToString("MM");
                        var query = string.Format(@"select isnull((NetSalesExcludePPN + OthersDPP),0) COGS from {0}..omPriceListBranches 
                                    Where CompanyCode = '{1}' and BranchCode = '{2}' and SupplierCode = '{3}'
                                    and GroupPrice = '{4}' and SalesModelCode = '{5}' and SalesModelYear = {6}
                                    and {7} >=  convert(varchar,EffectiveDate,112) and IsStatus = 1", DBMD, CompanyMD, UnitBranchMD, supplier, groupPrice, row.SalesModelCode, row.SalesModelYear, Convert.ToDateTime(inv.InvoiceDate).ToString("yyyyMMdd"));

                        cogs = ctxx.Database.SqlQuery<decimal>(query).FirstOrDefault();
                    }
                    else
                    {
                        //MD
                        vehicle = ctxx.Database.SqlQuery<MstVehicleLookup>("SELECT COGSUnit, COGSOthers, COGSKaroseri FROM " + DBMD + ".dbo.omMstVehicle WHERE CompanyCode='" + CompanyMD + "' AND  ChassisCode='" + vin.ChassisCode + "' AND ChassisNo='" + vin.ChassisNo + "'").FirstOrDefault();
                        cogs = (decimal)(vehicle.COGSUnit + vehicle.COGSOthers + vehicle.COGSKaroseri);
                    }
                }
                else
                {
                    vehicle = ctxx.Database.SqlQuery<MstVehicleLookup>("SELECT COGSUnit, COGSOthers, COGSKaroseri FROM omMstVehicle WHERE CompanyCode='" + CompanyCode + "' AND  ChassisCode='" + vin.ChassisCode + "' AND ChassisNo='" + vin.ChassisNo + "'").FirstOrDefault();
                    cogs = (decimal)(vehicle.COGSUnit + vehicle.COGSOthers + vehicle.COGSKaroseri);
                }

                if (cogs <= 0)
                {
                    return false;
                }

                string strUID = "";
                if (DealerCode() == "SD" || (vehicle != null && vehicle.COGSUnit > 0))
                {
                    vin.COGS = cogs;
                    vin.IsReturn = false;
                    strUID = MyLogger.GetCRC32(CompanyCode + BranchCode + mdl.InvoiceNo + mdl.BPKNo + mdl.SalesModelCode + mdl.SalesModelYear.ToString() + row.BPKSeq.ToString()) + CurrentUser.UserId;
                    if (strUID.Length > 15)
                    {
                        vin.CreatedBy = strUID.Substring(0, 15);
                    }
                    else
                    {
                        vin.CreatedBy = strUID;
                    }
                    vin.CreatedDate = ctxx.CurrentTime;
                    vin.LastUpdateBy = CurrentUser.UserId;
                    vin.LastUpdateDate = ctxx.CurrentTime;
                    ctxx.omTrSalesInvoiceVin.Add(vin);
                    if (ctxx.SaveChanges() < 0)
                    {
                        result = false;
                        break;
                    }
                    else result = true;
                }
                else result = false;
            }   
            return result;
        }

        private bool InsertInvoiceOther(DataContext ctxx, omTrSalesInvoiceModel mdl,string SONo)
        {
            bool result = true;
            string strUID = "";
            var dtSOModel = ctxx.OmTrSalesSoModelOthers
                    .Where(x => x.CompanyCode == CompanyCode &&
                    x.BranchCode == BranchCode &&
                    x.SONo == SONo &&
                    x.SalesModelCode == mdl.SalesModelCode &&
                    x.SalesModelYear == mdl.SalesModelYear)
                    .ToList();


            if (dtSOModel.Count() > 0)
            {
                foreach (var row in dtSOModel)
                {
                    var Other = new OmTrSalesInvoiceOthers();
                    Other.CompanyCode = CompanyCode;
                    Other.BranchCode = BranchCode;
                    Other.InvoiceNo = mdl.InvoiceNo;
                    Other.BPKNo = mdl.BPKNo;
                    Other.SalesModelCode = mdl.SalesModelCode;
                    Other.SalesModelYear = mdl.SalesModelYear;

                    Other.OtherCode = row.OtherCode;
                    //Other.DPP = decimal.Parse(row["DPP"].ToString());
                    //Other.PPn = decimal.Parse(row["PPn"].ToString());
                    //Other.Total = decimal.Parse(row["Total"].ToString());
                    Other.BeforeDiscDPP = row.BeforeDiscDPP;
                    Other.BeforeDiscPPn = row.BeforeDiscPPn;
                    Other.BeforeDiscTotal = row.BeforeDiscTotal;
                    Other.DiscExcludePPn = row.DiscExcludePPn;
                    Other.DiscIncludePPn = row.DiscIncludePPn;
                    Other.AfterDiscDPP = row.AfterDiscDPP;
                    Other.AfterDiscPPn = row.AfterDiscPPn;
                    Other.AfterDiscTotal = row.AfterDiscTotal;

                    strUID = MyLogger.GetCRC32(CompanyCode + BranchCode + mdl.InvoiceNo + mdl.BPKNo + mdl.SalesModelCode + mdl.SalesModelYear.ToString() + row.OtherCode ) + CurrentUser.UserId;
                    if (strUID.Length > 15)
                    {
                       Other.CreatedBy = strUID.Substring(0, 15);
                    }
                    else
                    {
                        Other.CreatedBy = strUID;
                    }
                    Other.CreatedDate = ctxx.CurrentTime;
                    Other.LastUpdateBy = CurrentUser.UserId;
                    Other.LastUpdateDate = ctxx.CurrentTime;
                    ctxx.omTrSalesInvoiceOthers.Add(Other);
                }

                if (ctxx.SaveChanges() < 0)
                {
                    result = false;
                }
            }
            return result;
        }

        public bool SaveInvBpk(omTrSalesInvoiceBPK recordInvBpk, OmTrSalesInvoice record, bool isNew, bool allCheck)
        {
            return true;
        }

        public bool DeleteInvBPK(DataContext ctxx, omTrSalesInvoiceBPK recordInvBPK, OmTrSalesInvoice record)
        {   
            string invoiceNo = record.InvoiceNo;
            string BPKNo = recordInvBPK.BPKNo;
            
            bool result = false;
           
                record.Status = "0";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = ctxx.CurrentTime;

                if (ctxx.SaveChanges()>-1)
                {
                    if (DelVin(ctxx, record.InvoiceNo, recordInvBPK.BPKNo))
                        if (DelOthers(ctxx, record.InvoiceNo, recordInvBPK.BPKNo))
                            if (UpdateBPKModel(ctxx, BPKNo, 0))
                                if (UpdateBPKDetail(ctxx,  BPKNo, "0"))
                                    if (DelInvModel(ctxx, invoiceNo, BPKNo))
                                    {
                                        ctxx.omTrSalesInvoiceBPK.Remove(recordInvBPK);
                                        return ctxx.SaveChanges() > -1;
                                    }                                        
                }            
            return result;
        }

        private  bool DelInvBPK(DataContext ctxx,  string InvoiceNo)
        {

            ctxx.omTrSalesInvoiceBPK
                 .Where(x => x.CompanyCode == CompanyCode &&
                    x.BranchCode == BranchCode &&
                    x.InvoiceNo == InvoiceNo)
                .ToList()
                .ForEach(x => ctxx.omTrSalesInvoiceBPK.Remove(x));
            
            return ctxx.SaveChanges() > -1;
        }

        private bool DelVin(DataContext ctxx, string InvoiceNo)
        {

            ctxx.omTrSalesInvoiceVin
                .Where(x => x.CompanyCode == CompanyCode &&
                    x.BranchCode == BranchCode &&
                    x.InvoiceNo == InvoiceNo)                    
                .ToList()
                .ForEach(x => ctxx.omTrSalesInvoiceVin.Remove(x));
            return ctxx.SaveChanges() > -1;
        }

        private  bool DelVin(DataContext ctxx, string InvoiceNo,string BPKNo)
        {           

            ctxx.omTrSalesInvoiceVin
                .Where(x => x.CompanyCode == CompanyCode &&
                    x.BranchCode == BranchCode &&
                    x.InvoiceNo == InvoiceNo &&
                    x.BPKNo == BPKNo)
                .ToList()
                .ForEach(x => ctxx.omTrSalesInvoiceVin.Remove(x));
            return ctxx.SaveChanges() > -1;
        }

        private  bool DelVin(DataContext ctxx,  string InvoiceNo,   string BPKNo, string salesModelCode, decimal salesModelYear)
        {
            ctxx.omTrSalesInvoiceVin
                .Where(x => x.CompanyCode == CompanyCode &&
                    x.BranchCode == BranchCode &&
                    x.InvoiceNo == InvoiceNo &&
                    x.BPKNo == BPKNo &&
                    x.SalesModelCode == salesModelCode &&
                    x.SalesModelYear == salesModelYear)
                .ToList()
                .ForEach(x => ctxx.omTrSalesInvoiceVin.Remove(x));
            return ctxx.SaveChanges() > -1;
        }

        private bool DelOthers(DataContext ctxx, string InvoiceNo)
        {

            ctxx.omTrSalesInvoiceOthers
                .Where(x => x.CompanyCode == CompanyCode &&
                    x.BranchCode == BranchCode &&
                     x.InvoiceNo == InvoiceNo )                    
                .ToList()
                .ForEach(x => ctxx.omTrSalesInvoiceOthers.Remove(x));

            return ctxx.SaveChanges() > -1;
        }

        private bool DelOthers(DataContext ctxx,string InvoiceNo, string BPKNo)
        {
            
            ctxx.omTrSalesInvoiceOthers
                .Where(x => x.CompanyCode == CompanyCode &&
                    x.BranchCode == BranchCode &&
                     x.InvoiceNo == InvoiceNo &&
                    x.BPKNo == BPKNo)
                .ToList()
                .ForEach(x => ctxx.omTrSalesInvoiceOthers.Remove(x));

            return ctxx.SaveChanges() > -1;
        }

        private bool DelOthers(DataContext ctxx,string InvoiceNo, string BPKNo, string salesModelCode, decimal salesModelYear)
        {
            ctxx.omTrSalesInvoiceOthers
                .Where(x => x.CompanyCode == CompanyCode &&
                    x.BranchCode == BranchCode &&
                    x.InvoiceNo == InvoiceNo &&
                    x.BPKNo == BPKNo &&
                    x.SalesModelCode == salesModelCode &&
                    x.SalesModelYear == salesModelYear)
                .ToList()
                .ForEach(x => ctxx.omTrSalesInvoiceOthers.Remove(x));
            return ctxx.SaveChanges() > -1;        
        }


        private  bool DelParts(DataContext ctxx,  string InvoiceNo)
        {

            ctxx.OmTrSalesInvoiceAccs
                .Where(x => x.CompanyCode == CompanyCode &&
                            x.BranchCode == BranchCode &&
                            x.InvoiceNo == InvoiceNo)
                .ToList()
                .ForEach(x => ctxx.OmTrSalesInvoiceAccs.Remove(x));

            return ctxx.SaveChanges() > -1;
        }

        private bool UpdateSalesSO(DataContext ctxx, string SONo)
        {

            ctxx.OmTrSalesSOAccses
                .Where(x => x.CompanyCode == CompanyCode &&
                            x.BranchCode == BranchCode &&
                            x.SONo == SONo)
                .ToList()
                .ForEach(x => x.InvoiceQty = 0);
            return ctxx.SaveChanges() > -1;         
        }


        private  bool UpdateBPKModel(DataContext ctxx,  string BPKNo, decimal qtyInvoice)
        {

            ctxx.OmTrSalesBPKModels
                .Where(x => x.CompanyCode == CompanyCode &&
                    x.BranchCode == BranchCode &&
                    x.BPKNo == BPKNo)
                .ToList()
                .ForEach(x => x.QuantityInvoice = qtyInvoice);

            return ctxx.SaveChanges() > -1;
        }

        private  bool UpdateBPKDetail(DataContext ctxx,  string BPKNo, string status)
        {

            ctxx.OmTrSalesBPKDetails
               .Where(x => x.CompanyCode == CompanyCode &&
                    x.BranchCode == BranchCode &&
                    x.BPKNo == BPKNo)
                .ToList()
                .ForEach(x => x.StatusInvoice = status);
            return ctxx.SaveChanges() > -1;
        }

        private  bool UpdateBPKDetail(DataContext ctxx,   string BPKNo, string salesModelCode, decimal salesModelYear, string status)
        {
            ctxx.OmTrSalesBPKDetails
               .Where(x => x.CompanyCode == CompanyCode &&
                    x.BranchCode == BranchCode &&
                    x.BPKNo == BPKNo &&
                    x.SalesModelCode == salesModelCode &&
                    x.SalesModelYear == salesModelYear)
                .ToList()
                .ForEach(x => x.StatusInvoice = status);

            return ctxx.SaveChanges() > -1;            
        }
        
        private bool DelInvModel(DataContext ctxx, string InvoiceNo, string BPKNo)
        {
            ctxx.omTrSalesInvoiceModel
               .Where(x => x.CompanyCode == CompanyCode &&
                     x.BranchCode == BranchCode &&
                     x.InvoiceNo == InvoiceNo &&
                     x.BPKNo == BPKNo)
                 .ToList()
                 .ForEach(x => ctxx.omTrSalesInvoiceModel.Remove(x));

            return ctxx.SaveChanges() > -1;
        }

        private bool DelInvModel(DataContext ctxx,string InvoiceNo)
        {
            
            ctxx.omTrSalesInvoiceModel
                 .Where(x => x.CompanyCode == CompanyCode &&
                     x.BranchCode == BranchCode &&
                     x.InvoiceNo == InvoiceNo)                     
                 .ToList()
                 .ForEach(x => ctxx.omTrSalesInvoiceModel.Remove(x));

            return ctxx.SaveChanges() > -1;
        }


        public bool DeleteInvModel(DataContext ctxx, omTrSalesInvoiceModel recordInvModel, OmTrSalesInvoice record)
        {            

            string companyCode = record.CompanyCode;
            string branchCode = record.BranchCode;
            string SONo = record.SONo;
            string BPKNo = recordInvModel.BPKNo;
            string invoiceNo = record.InvoiceNo;
            string salesModelCode = recordInvModel.SalesModelCode;
            decimal salesModelYear = recordInvModel.SalesModelYear;

            bool result = false;
          
                var  bpkModel = ctxx.OmTrSalesBPKModels
                                .Find(companyCode, branchCode, BPKNo, salesModelCode, salesModelYear);                    
                if (bpkModel != null)
                {
                    bpkModel.QuantityInvoice = 0;
                    bpkModel.LastUpdateBy = CurrentUser.UserId;
                    bpkModel.LastUpdateDate = ctxx.CurrentTime;
                }

                //update OmTrSalesInvoice
                record.Status = "0";
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = ctxx.CurrentTime;

                if (ctxx.SaveChanges() > -1)
                {                
                        if (UpdateBPKDetail(ctxx, BPKNo, salesModelCode, salesModelYear, "0"))
                            if (DelVin(ctxx, invoiceNo, BPKNo, salesModelCode, salesModelYear))
                                if (DelOthers(ctxx, invoiceNo, BPKNo, salesModelCode, salesModelYear))
                                {
                                    ctxx.omTrSalesInvoiceModel.Remove(recordInvModel);
                                    return ctxx.SaveChanges() > -1;
                                }
                }
                return result;        
        }


        public  bool DeleteInv(DataContext ctxx, OmTrSalesInvoice record)
        {
            string companyCode = record.CompanyCode;
            string branchCode = record.BranchCode;
            string invoiceNo = record.InvoiceNo;
            string soNo = record.SONo;

            //update OmTrSalesInvoice
            record.Status = "3";
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = ctxx.CurrentTime;

            bool result = false;
           
                #region Select BPk

                var dt = ctxx.omTrSalesInvoiceBPK
                            .Where(x => x.CompanyCode == companyCode &&
                                        x.BranchCode == branchCode &&
                                        x.InvoiceNo == invoiceNo).ToList();

                #endregion


                if (dt.Count > 0)
                {
                    foreach (var  row in dt)
                    {
                        if (!UpdateBPKDetail(ctxx,row.BPKNo, "0")
                            || !UpdateBPKModel(ctxx, row.BPKNo, 0))
                        {
                            result = false;
                            break;
                        }
                        else result = true;
                    }

                    if (result)
                    {
                        if (DelVin(ctxx,  invoiceNo))
                            if (DelOthers(ctx,invoiceNo))
                                if (DelParts(ctx,invoiceNo))
                                    if (UpdateSalesSO(ctx,  record.SONo))
                                        if (DelInvModel(ctx,  invoiceNo))
                                            if (DelInvBPK(ctx, invoiceNo))
                                            {
                                               result=  ctxx.SaveChanges() > -1;
                                            }
                    }
                }
                else
                {                   
                         result = ctxx.SaveChanges() > -1;
                }               

                result= ctxx.Database.ExecuteSqlCommand("uspfn_omDeleteInvoiceAccSeq '" + CompanyCode + "','" + BranchCode + "','" + invoiceNo + "','" + soNo + "','" + CurrentUser.UserId + "'") > -1;
          
            return result;
        }

        public bool CekItemOut(DataContext ctxx, string SONo)
        {
            bool result = true;
            string SupplySlipNo="";
            decimal? SupplyQty = 0;
            decimal? CostPrice = 0;
            string qry = "";
            ctx.OmTrSalesSOAccsSeqs.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.SONo == SONo).ToList().ForEach(y =>
            {
                SupplyQty = y.SupplyQty;
                SupplySlipNo = y.SupplySlipNo;
                if (SupplyQty == 0 || SupplySlipNo == "")
                {
                    result = false;
                    return;
                }
            });

            return result;
        }

        public bool AllPartIsGet(DataContext ctxx, OmTrSalesInvoice record)
        {
            bool result = false;
            string scom = CompanyCode;
            //IDbContext ctx = DbFactory.Configure(true);

            var SOSparepart = ctxx.OmTrSalesSOAccsSeqs
                                    .Where(x => x.CompanyCode == CompanyCode &&
                                                x.BranchCode == BranchCode &&
                                                x.SONo == record.SONo);

            if (SOSparepart.Count() < 0)
            {
                result = true;
                return result;
            }

            var dt = ctxx.omTrSalesInvoiceModel
                         .Where(x => x.CompanyCode == CompanyCode &&
                                    x.BranchCode == BranchCode &&
                                    x.InvoiceNo == record.InvoiceNo)
                        .ToList();

            if (dt.Count() > 0)
            {
                foreach (var row in dt)
                {
                    var BPKqty = ctxx.OmTrSalesBPKModels
                                 .Where(x => x.CompanyCode == CompanyCode &&
                                    x.BranchCode == BranchCode &&
                                    x.BPKNo == row.BPKNo &&
                                    x.QuantityBPK - x.QuantityInvoice != 0
                                    );
                    if (BPKqty.Count() > 0)
                    {
                        result = true;
                        break;
                    }
                }

                string sql = string.Format(@"
                    select * from dbo.omTrSalesSOAccsSeq 
                    where companyCode = '{0}'
                        and branchCode = '{1}'
                        and SONo = '{2}'
                        and PartNo not in (
	                        select PartNo from omTrSalesInvoiceAccsSeq a 
                            inner join dbo.omTrSalesInvoice b on a.companyCode = b.companyCode 
                                and a.branchCode = b.branchCode 
                                and a.invoiceNo = b.invoiceNo
	                        where b.companyCode = '{0}'
                                and b.branchCode = '{1}'
	                            and b.SONo = '{2}')
                        and SupplyQty - ReturnQty > 0   ", CompanyCode, BranchCode, record.SONo);

                if (ctxx.Database.SqlQuery<OmTrSalesSOAccsSeq>(sql).Count() == 0)
                {
                    result = true;
                }
            }

            return result;
        }

        public  bool CekStatusAccessories(DataContext ctxx,  string sono)
        {
            bool result = false;            
            try
            {
                result = ctxx.Database.ExecuteSqlCommand("exec uspfn_CekLampiranAccessoriesSales {0},{1},'{2}'", CompanyCode, BranchCode, sono) > -1;
                
            }
            catch (Exception ex)
            {
                
                result = false;
                return result;
            }
            return result;
        }


        private string getProdType()
        {
            var s = ctx.CoProfiles.Find(CompanyCode, BranchCode);
            return s.ProductType;
        }

        public bool ApproveInv(DataContext ctxx, OmTrSalesInvoice record, ref string msg)
        {
            string msg1 = "Approve Invoice Berhasil.";
            string msg2 = "Approve Invoice Gagal.";
            string msg3 = "Insert svSDMovement gagal!, Part accesories belum sampai proses LMP";
            bool result = false;
            bool errSVSDM = false;
            bool independent = false;
            var pType = getProdType();
            var user = CurrentUser;//SysUser.Current;
            string companyCode = record.CompanyCode;
            string branchCode = record.BranchCode;
            string invoiceNo = record.InvoiceNo;
            string soNo = record.SONo;
            var md = DealerCode() == "MD";
            bool otom = cekOtomatis();

            var emp = ctxx.HrEmployeeViews.Where(x => x.CompanyCode == companyCode).FirstOrDefault();

            if ((CompanyCode == CompanyMD) && (BranchCode == BranchMD))
            {
                independent = true;
            }

            //IDbContext ctx = DbFactory.Configure(true);
            //OmTrSalesInvoiceDao oOmTrSalesInvoiceDao = new OmTrSalesInvoiceDao(ctx);
            //GnTrnBankBookDao oGnTrnBankBookDao = new GnTrnBankBookDao(ctx);
            //OmTrInventQtyVehicleDao oOmTrInventQtyVehicleDao = new OmTrInventQtyVehicleDao(ctx);
            //OmTrSalesDNVinDao oOmTrSalesDNVinDao = new OmTrSalesDNVinDao(ctx);
            //OmTrSalesSOVinDao oOmTrSalesSOVinDao = new OmTrSalesSOVinDao(ctx);
            //OmMstVehicleDao oOmMstVehicleDao = new OmMstVehicleDao(ctx);
            //OmHstInquirySalesDao OmHstInquirySalesDao = new OmHstInquirySalesDao(ctx);

            record.Status = "2";
            record.LastUpdateBy = user.UserId;
            record.LastUpdateDate = ctxx.CurrentTime;

            bool isNew = false;
            //GnTrnBankBook bankBook = PrepareBankBookRecord(ctx, oGnTrnBankBookDao, companyCode, branchCode,
            //record.CustomerCode, user.ProfitCenterCode, invoiceNo, ref isNew);
            
            string wareHouseCode = "";
            var omtrso= ctxx.OmTRSalesSOs.Find(companyCode,branchCode,record.SONo);
            if (omtrso != null)
                wareHouseCode = omtrso.WareHouseCode.ToString();

            string DNNo = "";
           
                if ((bool)record.isStandard)
                {
                    var recFPJ = ctxx.omFakturPajakHdrs.Find(user.CompanyCode, user.BranchCode, record.InvoiceNo);
                    if (recFPJ == null)
                    {
                        recFPJ = new omFakturPajakHdr();
                        recFPJ.CompanyCode = user.CompanyCode;
                        recFPJ.BranchCode = user.BranchCode;
                        recFPJ.InvoiceNo = record.InvoiceNo;
                        recFPJ.InvoiceDate = record.InvoiceDate;
                        recFPJ.FakturPajakDate = record.InvoiceDate;
                        recFPJ.PrintSeq = 0;
                        recFPJ.Status = "";
                        recFPJ.TaxType = "Standard";
                        recFPJ.DueDate = record.DueDate;
                        recFPJ.CustomerCode = record.BillTo;
                        recFPJ.CreatedBy = user.UserId;
                        recFPJ.CreatedDate = ctxx.CurrentTime;

                        result = OmFakturPajakHdrBLL.Instance(CurrentUser.UserId).Save(ctxx, recFPJ, (DateTime)record.InvoiceDate);
                    }
                }

                if (InsertDN(ctxx, record, ref DNNo))
                {
                    string dbMD = ctxx.Database.SqlQuery<string>("SELECT DbMD from gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'").FirstOrDefault();
                    string Qry = "";

                    if (InsertDNModel(ctxx, record, DNNo))
                    {
                        if (isExistBBNKIR(ctxx,soNo))
                        {
                          var listModel = ctxx.omTrSalesInvoiceVin
                                            .Where(x=> x.CompanyCode== CompanyCode &&
                                                        x.BranchCode== branchCode &&
                                                        x.InvoiceNo== record.InvoiceNo)
                                        .ToList();
                            
                            foreach (omTrSalesInvoiceVin model in listModel)
                            {
                                if (!independent || otom)
                                {
                                    if (!SDMovementInvoice(ctxx, model, wareHouseCode))
                                    {
                                        result = false; break;
                                    }
                                }
                                ////var oOmMstVehicle = md ? ctx.OmMstVehicles.Find(companyCode, model.ChassisCode, model.ChassisNo) : ctxMD.OmMstVehicles.Find(CompanyMD, model.ChassisCode, model.ChassisNo);                                                                  
                                //var oOmMstVehicle = ctxMD.OmMstVehicles.Find(CompanyMD, model.ChassisCode, model.ChassisNo);
                                if (independent && !otom)
                                {
                                    Qry = "SELECT * from omMstVehicle WHERE CompanyCode='" + CompanyCode + "' AND ChassisCode='" + model.ChassisCode + "' AND ChassisNo='" + model.ChassisNo + "'";
                                }
                                else
                                {
                                    Qry = "SELECT * from " + dbMD + "..omMstVehicle WHERE CompanyCode='" + CompanyMD + "' AND ChassisCode='" + model.ChassisCode + "' AND ChassisNo='" + model.ChassisNo + "'";
                                }
                                
                                var oOmMstVehicle = ctxx.Database.SqlQuery<OmMstVehicle>(Qry).FirstOrDefault(); 
                                if (oOmMstVehicle != null)
                                {
                                    //oOmMstVehicle.InvoiceNo = invoiceNo;
                                    //oOmMstVehicle.Status = "6";

                                    decimal? salesNetAmt = 0;
                                    decimal? ppnBmSellPaid = 0;
                                    decimal? ppnBmSell = 0;

                                    var recModel = ctxx.omTrSalesInvoiceModel.Find(CompanyCode, BranchCode, model.InvoiceNo, model.BPKNo, model.SalesModelCode, model.SalesModelYear);
                                    if (recModel != null)
                                    {
                                        

                                        //oOmMstVehicle.SalesNetAmt = recModel.AfterDiscTotal;
                                        //oOmMstVehicle.PpnBmSellPaid = recModel.PPnBMPaid;
                                        //oOmMstVehicle.PpnBmSell = recModel.AfterDiscPPnBM;

                                        salesNetAmt = recModel.AfterDiscTotal == null ? 0 : recModel.AfterDiscTotal;
                                        ppnBmSellPaid = recModel.PPnBMPaid == null ? 0 : recModel.PPnBMPaid;
                                        ppnBmSell = recModel.AfterDiscPPnBM == null ? 0 : recModel.AfterDiscPPnBM;
                                    
                                    
                                    }
                                    //oOmMstVehicle.LastUpdateBy = user.UserId;
                                    //oOmMstVehicle.LastUpdateDate = ctx.CurrentTime;

                                    //var msVhcl = ctxx.OmMstVehicles.Find(CompanyCode, model.ChassisCode, model.ChassisNo);
                                    if (independent && !otom)
                                    {
                                        Qry = "UPDATE omMstVehicle SET InvoiceNo='" + invoiceNo + "', Status='6', SalesNetAmt='" + salesNetAmt + "', PpnBmSellPaid='" + ppnBmSellPaid + "', PpnBmSell='" + ppnBmSell +
                                          "', LastUpdateBy='" + user.UserId + "', LastUpdateDate='" + ctxx.CurrentTime + "' WHERE CompanyCode='" + CompanyCode + "' AND ChassisCode='" + model.ChassisCode + "' AND ChassisNo='" + model.ChassisNo + "'";
                                    }
                                    else
                                    {
                                        Qry = "UPDATE " + dbMD + "..omMstVehicle SET InvoiceNo='" + invoiceNo + "', Status='6', SalesNetAmt='" + salesNetAmt + "', PpnBmSellPaid='" + ppnBmSellPaid + "', PpnBmSell='" + ppnBmSell +
                                              "', LastUpdateBy='" + user.UserId + "', LastUpdateDate='" + ctxx.CurrentTime + "' WHERE CompanyCode='" + CompanyMD + "' AND ChassisCode='" + model.ChassisCode + "' AND ChassisNo='" + model.ChassisNo + "'";
                                    }

                                    try
                                    {
                                        ctxx.Database.ExecuteSqlCommand(Qry);
                                        result = true;
                                    }
                                    catch
                                    {
                                        result = false; break;
                                    }
                                    ////var res = md ? ctx.SaveChanges() : ctxMD.SaveChanges() ;
                                    //var res = ctxMD.SaveChanges();
                                    ////var res = ctxx.SaveChanges();
                                    //if (res <= 0)
                                    //{
                                    //    result = false; break;
                                    //}
                                    //else
                                    //    result = true;
                                }
                                else
                                {
                                    result = false; break;
                                }

                                ////var vehicle = md ? ctx.OmTrInventQtyVehicles
                                ////               .Where(x => x.CompanyCode == CompanyCode &&
                                ////                        x.BranchCode == BranchCode &&
                                ////                        x.SalesModelCode == model.SalesModelCode &&
                                ////                        x.SalesModelYear == model.SalesModelYear &&
                                ////                        x.ColourCode == model.ColourCode &&
                                ////                        x.WarehouseCode == wareHouseCode)
                                ////               .FirstOrDefault() :
                                ////                        ctxMD.OmTrInventQtyVehicles
                                ////               .Where(x => x.CompanyCode == CompanyMD &&
                                ////                        x.BranchCode == BranchMD &&
                                ////                        x.SalesModelCode == model.SalesModelCode &&
                                ////                        x.SalesModelYear == model.SalesModelYear &&
                                ////                        x.ColourCode == model.ColourCode &&
                                ////                        x.WarehouseCode == wareHouseCode)
                                ////                .FirstOrDefault();

                                //var vehicle = ctxMD.OmTrInventQtyVehicles
                                //               .Where(x => x.CompanyCode == CompanyMD &&
                                //                        x.BranchCode == BranchMD &&
                                //                        x.SalesModelCode == model.SalesModelCode &&
                                //                        x.SalesModelYear == model.SalesModelYear &&
                                //                        x.ColourCode == model.ColourCode &&
                                //                        x.WarehouseCode == wareHouseCode).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month)
                                //                .FirstOrDefault();
                                if (independent && !otom)
                                {
                                    Qry = "SELECT TOP 1 * FROM OmTrInventQtyVehicle a WHERE a.CompanyCode='" + CompanyCode + "' AND a.BranchCode='" + BranchCode + "' AND a.SalesModelCode='" + model.SalesModelCode +
                                    "' AND a.SalesModelYear='" + model.SalesModelYear + "' AND a.ColourCode='" + model.ColourCode + "' AND a.WarehouseCode='" + wareHouseCode + "' ORDER BY a.Year DESC, a.Month DESC";
                                }
                                else
                                {
                                    Qry = "SELECT TOP 1 * FROM " + dbMD + "..OmTrInventQtyVehicle a WHERE a.CompanyCode='" + CompanyMD + "' AND a.BranchCode='" + UnitBranchMD + "' AND a.SalesModelCode='" + model.SalesModelCode +
                                    "' AND a.SalesModelYear='" + model.SalesModelYear + "' AND a.ColourCode='" + model.ColourCode + "' AND a.WarehouseCode='" + wareHouseCode + "' ORDER BY a.Year DESC, a.Month DESC";
                                }
                               
                                var vehicle = ctxx.Database.SqlQuery<OmTrInventQtyVehicle>(Qry).FirstOrDefault();
                                                                
                                if (vehicle != null)
                                {
                                    //vehicle.Alocation = vehicle.Alocation - 1;
                                    //vehicle.QtyOut = vehicle.QtyOut + 1;
                                    //vehicle.EndingOH = (vehicle.BeginningOH + vehicle.QtyIn) - vehicle.QtyOut;
                                    //vehicle.EndingAV = ((vehicle.BeginningAV + vehicle.QtyIn) - vehicle.Alocation) - vehicle.QtyOut;

                                    //vehicle.LastUpdateBy = user.UserId;
                                    //vehicle.LastUpdateDate = ctx.CurrentTime;

                                    //var res = ctxMD.SaveChanges();

                                    //if (res < 0)
                                    //{ result = false; break; }
                                    //else
                                    //    result = true;

                                    decimal? alocation = vehicle.Alocation - 1;
                                    decimal? qtyOut = vehicle.QtyOut + 1;
                                    decimal? endingOH = (vehicle.BeginningOH + vehicle.QtyIn) - qtyOut;
                                    decimal? endingAV = ((vehicle.BeginningAV + vehicle.QtyIn) - alocation) - qtyOut;

                                    if (independent && !otom)
                                    {
                                        Qry = "WITH TmpUpdate AS ( SELECT TOP 1 * FROM "
                                                + "OmTrInventQtyVehicle a WHERE a.CompanyCode='" + CompanyCode + "' AND a.BranchCode='" + BranchCode + "' AND a.SalesModelCode='" + model.SalesModelCode +
                                                "' AND a.SalesModelYear='" + model.SalesModelYear + "' AND a.ColourCode='" + model.ColourCode + "' AND a.WarehouseCode='" + wareHouseCode +
                                                "' ORDER BY a.Year DESC, a.Month DESC ) " +
                                                "UPDATE TmpUpdate  SET Alocation='" + alocation + "', QtyOut='" + qtyOut + "', EndingOH='" + endingOH + "', EndingAV='" + endingAV + "', LastUpdateBy='" + user.UserId + "', LastUpdateDate='" + DateTime.Now + "'";
                                    }
                                    else
                                    {
                                        Qry = "WITH TmpUpdate AS ( SELECT TOP 1 * FROM "
                                                + dbMD + "..OmTrInventQtyVehicle a WHERE a.CompanyCode='" + CompanyMD + "' AND a.BranchCode='" + UnitBranchMD + "' AND a.SalesModelCode='" + model.SalesModelCode +
                                                "' AND a.SalesModelYear='" + model.SalesModelYear + "' AND a.ColourCode='" + model.ColourCode + "' AND a.WarehouseCode='" + wareHouseCode +
                                                "' ORDER BY a.Year DESC, a.Month DESC ) " +
                                                "UPDATE TmpUpdate  SET Alocation='" + alocation + "', QtyOut='" + qtyOut + "', EndingOH='" + endingOH + "', EndingAV='" + endingAV + "', LastUpdateBy='" + user.UserId + "', LastUpdateDate='" + DateTime.Now + "'";
                                    }

                                    try
                                    {
                                        ctxx.Database.ExecuteSqlCommand(Qry);
                                        result = true;
                                    }
                                    catch
                                    {
                                        result = false; break;
                                    }
                                }

                                var SOVin = ctxx.omTrSalesSOVins
                                                .Where(x => x.CompanyCode == companyCode &&
                                                    x.BranchCode == branchCode &&
                                                    x.SONo == soNo &&
                                                    x.SalesModelCode == model.SalesModelCode &&
                                                    x.SalesModelYear == model.SalesModelYear &&
                                                    x.ColourCode == model.ColourCode &&
                                                    x.ChassisCode == model.ChassisCode &&
                                                    x.ChassisNo == model.ChassisNo)
                                                .FirstOrDefault();
                                if (SOVin != null)
                                {
                                    var DNVin = new OmTrSalesDNVin();
                                    DNVin.CompanyCode = companyCode;
                                    DNVin.BranchCode = branchCode;
                                    DNVin.DNNo = DNNo;
                                    
                                    DNVin.DNSeq = ctxx.OmTrSalesDNVins
                                                    .Where(x => x.CompanyCode == CompanyCode &&
                                                                x.BranchCode == BranchCode &&
                                                                x.DNNo == DNNo)
                                                    .Select(x => x.DNSeq)
                                                    .DefaultIfEmpty(0)
                                                    .Max() + 1;                                        
                                        
                                    DNVin.ChassisCode = model.ChassisCode;
                                    DNVin.ChassisNo = model.ChassisNo;
                                    DNVin.EngineCode = model.EngineCode;
                                    DNVin.EngineNo = model.EngineNo;
                                    DNVin.BBN = SOVin.BBN;
                                    DNVin.KIR = SOVin.KIR;
                                    DNVin.Remark = SOVin.Remark;
                                    DNVin.CreatedBy = DNVin.LastUpdateBy = user.UserId;
                                    DNVin.CreatedDate = DNVin.LastUpdateDate = ctxx.CurrentTime;
                                    ctxx.OmTrSalesDNVins.Add(DNVin);
                                    if (ctxx.SaveChanges() < 0) { result = false; break; }
                                    else result = true;
                                }

                                if (pType == "4W")
                                {
                                    if (emp != null)
                                    {
                                        if (ctxx.Database.ExecuteSqlCommand("exec uspfn_SaveOmHstInquirySales2 '" + companyCode + "','" + branchCode + "','" + invoiceNo + "','" + CurrentUser.UserId + "'") <= 0) { result = false; break; }
                                        else
                                            result = true;
                                    }
                                    else
                                    {
                                        if (ctxx.Database.ExecuteSqlCommand("exec uspfn_SaveOmHstInquirySales '" + companyCode + "','" + branchCode + "','" + invoiceNo + "','" + CurrentUser.UserId + "'") <= 0) { result = false; break; }
                                        else
                                            result = true;
                                    }

                                }
                                else
                                {
                                    if (emp != null)
                                    {
                                        if (ctxx.Database.ExecuteSqlCommand("exec uspfn_SaveOmHstInquirySales2 '" + companyCode + "','" + branchCode + "','" + invoiceNo + "','" + CurrentUser.UserId + "'") <= 0) { result = false; break; }
                                        else
                                            result = true;
                                    }
                                    else
                                    {
                                        if (ctxx.Database.ExecuteSqlCommand("exec uspfn_SaveOmHstInquirySales '" + companyCode + "','" + branchCode + "','" + invoiceNo + "','" + CurrentUser.UserId + "'") <= 0) { result = false; break; }
                                        else
                                            result = true;
                                    }
                                }                                
                            }
                        }
                        else
                        {
                            var listModel = ctxx.omTrSalesInvoiceVin
                                            .Where(x=> x.CompanyCode== CompanyCode &&
                                                        x.BranchCode== branchCode &&
                                                        x.InvoiceNo== record.InvoiceNo)
                                        .ToList();

                            foreach (omTrSalesInvoiceVin model in listModel)
                            {
                                //var oOmMstVehicle = md ? ctx.OmMstVehicles.Find(companyCode, model.ChassisCode, model.ChassisNo) : 
                                //    ctxMD.OmMstVehicles.Find(CompanyMD, model.ChassisCode, model.ChassisNo);
                                if (!independent && otom)
                                {
                                    if (!SDMovementInvoice(ctxx, model, wareHouseCode))
                                    {
                                        result = false; break;
                                    }
                                }
                                //var oOmMstVehicle = ctxMD.OmMstVehicles.Find(CompanyMD, model.ChassisCode, model.ChassisNo);

                                if (independent && !otom)
                                {
                                    Qry = "SELECT * from omMstVehicle WHERE CompanyCode='" + CompanyCode + "' AND ChassisCode='" + model.ChassisCode + "' AND ChassisNo='" + model.ChassisNo + "'";
                                }
                                else
                                {
                                    Qry = "SELECT * from " + dbMD + "..omMstVehicle WHERE CompanyCode='" + CompanyMD + "' AND ChassisCode='" + model.ChassisCode + "' AND ChassisNo='" + model.ChassisNo + "'";
                                }
                                var oOmMstVehicle = ctxx.Database.SqlQuery<OmMstVehicle>(Qry).FirstOrDefault();
                                if (oOmMstVehicle != null)
                                {
                                    //oOmMstVehicle.InvoiceNo = invoiceNo;
                                    //oOmMstVehicle.Status = "6";
                                    //var recModel = ctx.omTrSalesInvoiceModel.Find(CompanyCode, BranchCode, model.InvoiceNo, model.BPKNo, model.SalesModelCode, model.SalesModelYear);
                                    //if (recModel != null)
                                    //{
                                    //    oOmMstVehicle.SalesNetAmt = recModel.AfterDiscTotal;
                                    //    oOmMstVehicle.PpnBmSellPaid = recModel.PPnBMPaid;
                                    //    oOmMstVehicle.PpnBmSell = recModel.AfterDiscPPnBM;
                                    //}
                                    //oOmMstVehicle.LastUpdateBy = user.UserId;
                                    //oOmMstVehicle.LastUpdateDate = ctx.CurrentTime;
                                    //var res = ctxMD.SaveChanges();
                                    
                                    //if (res <= 0)
                                    //{
                                    //    result = false; break;
                                    //}

                                    decimal? salesNetAmt = 0;
                                    decimal? ppnBmSellPaid = 0;
                                    decimal? ppnBmSell = 0;

                                    var recModel = ctxx.omTrSalesInvoiceModel.Find(CompanyCode, BranchCode, model.InvoiceNo, model.BPKNo, model.SalesModelCode, model.SalesModelYear);
                                    if (recModel != null)
                                    {
                                        salesNetAmt = recModel.AfterDiscTotal == null ? 0 : recModel.AfterDiscTotal;
                                        ppnBmSellPaid = recModel.PPnBMPaid == null ? 0 : recModel.PPnBMPaid;
                                        ppnBmSell = recModel.AfterDiscPPnBM == null ? 0 : recModel.AfterDiscPPnBM;
                                    }

                                    if (independent && !otom)
                                    {
                                        Qry = "UPDATE omMstVehicle SET InvoiceNo='" + invoiceNo + "', Status='6', SalesNetAmt='" + salesNetAmt + "', PpnBmSellPaid='" + ppnBmSellPaid + "', PpnBmSell='" + ppnBmSell +
                                              "', LastUpdateBy='" + user.UserId + "', LastUpdateDate='" + ctxx.CurrentTime + "' WHERE CompanyCode='" + CompanyCode + "' AND ChassisCode='" + model.ChassisCode + "' AND ChassisNo='" + model.ChassisNo + "'";                    
                                    }
                                    else
                                    {
                                        Qry = "UPDATE " + dbMD + "..omMstVehicle SET InvoiceNo='" + invoiceNo + "', Status='6', SalesNetAmt='" + salesNetAmt + "', PpnBmSellPaid='" + ppnBmSellPaid + "', PpnBmSell='" + ppnBmSell +
                                              "', LastUpdateBy='" + user.UserId + "', LastUpdateDate='" + ctxx.CurrentTime + "' WHERE CompanyCode='" + CompanyMD + "' AND ChassisCode='" + model.ChassisCode + "' AND ChassisNo='" + model.ChassisNo + "'";
                                    }
                                    try
                                    {
                                        ctxx.Database.ExecuteSqlCommand(Qry);
                                        result = true;
                                    }
                                    catch
                                    {
                                        result = false; break;
                                    }
                                }
                                else
                                {
                                    result = false; break;
                                }

                                //var vehicle =  ctxMD.OmTrInventQtyVehicles
                                //               .Where(x => x.CompanyCode == CompanyMD &&
                                //                        x.BranchCode == BranchMD &&
                                //                        x.SalesModelCode == model.SalesModelCode &&
                                //                        x.SalesModelYear == model.SalesModelYear &&
                                //                        x.ColourCode == model.ColourCode &&
                                //                        x.WarehouseCode == wareHouseCode).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month)
                                //                .FirstOrDefault();
                                if (independent && !otom)
                                {
                                    Qry = "SELECT TOP 1 * FROM OmTrInventQtyVehicle a WHERE a.CompanyCode='" + CompanyCode + "' AND a.BranchCode='" + BranchCode + "' AND a.SalesModelCode='" + model.SalesModelCode +
                                          "' AND a.SalesModelYear='" + model.SalesModelYear + "' AND a.ColourCode='" + model.ColourCode + "' AND a.WarehouseCode='" + wareHouseCode + "' ORDER BY a.Year DESC, a.Month DESC";
                                }
                                else
                                {
                                    Qry = "SELECT TOP 1 * FROM " + dbMD + "..OmTrInventQtyVehicle a WHERE a.CompanyCode='" + CompanyMD + "' AND a.BranchCode='" + UnitBranchMD + "' AND a.SalesModelCode='" + model.SalesModelCode +
                                          "' AND a.SalesModelYear='" + model.SalesModelYear + "' AND a.ColourCode='" + model.ColourCode + "' AND a.WarehouseCode='" + wareHouseCode + "' ORDER BY a.Year DESC, a.Month DESC";
                                }
                                var vehicle = ctxx.Database.SqlQuery<OmTrInventQtyVehicle>(Qry).FirstOrDefault();

                                if (vehicle != null)
                                {
                                    //vehicle.Alocation = vehicle.Alocation - 1;
                                    //vehicle.QtyOut = vehicle.QtyOut + 1;
                                    //vehicle.EndingOH = (vehicle.BeginningOH + vehicle.QtyIn) - vehicle.QtyOut;
                                    //vehicle.EndingAV = ((vehicle.BeginningAV + vehicle.QtyIn) - vehicle.Alocation) - vehicle.QtyOut;

                                    //vehicle.LastUpdateBy = user.UserId;
                                    //vehicle.LastUpdateDate = ctx.CurrentTime;
                                    //var res = ctxMD.SaveChanges();
                                    //if (res < 0) { result = false; break; }
                                    //else
                                    //    result = true;

                                    decimal? alocation = vehicle.Alocation - 1;
                                    decimal? qtyOut = vehicle.QtyOut + 1;
                                    decimal? endingOH = (vehicle.BeginningOH + vehicle.QtyIn) - qtyOut;
                                    decimal? endingAV = ((vehicle.BeginningAV + vehicle.QtyIn) - alocation) - qtyOut;

                                    if (independent && !otom)
                                    {
                                        Qry = "WITH TmpUpdate AS ( SELECT TOP 1 * FROM "
                                              + "OmTrInventQtyVehicle a WHERE a.CompanyCode='" + CompanyCode + "' AND a.BranchCode='" + BranchCode + "' AND a.SalesModelCode='" + model.SalesModelCode +
                                              "' AND a.SalesModelYear='" + model.SalesModelYear + "' AND a.ColourCode='" + model.ColourCode + "' AND a.WarehouseCode='" + wareHouseCode +
                                              "' ORDER BY a.Year DESC, a.Month DESC ) " +
                                              "UPDATE TmpUpdate  SET Alocation='" + alocation + "', QtyOut='" + qtyOut + "', EndingOH='" + endingOH + "', EndingAV='" + endingAV + "', LastUpdateBy='" + user.UserId + "', LastUpdateDate='" + DateTime.Now + "'";
                                    }
                                    else
                                    {
                                        Qry = "WITH TmpUpdate AS ( SELECT TOP 1 * FROM "
                                              + dbMD + "..OmTrInventQtyVehicle a WHERE a.CompanyCode='" + CompanyMD + "' AND a.BranchCode='" + UnitBranchMD + "' AND a.SalesModelCode='" + model.SalesModelCode +
                                              "' AND a.SalesModelYear='" + model.SalesModelYear + "' AND a.ColourCode='" + model.ColourCode + "' AND a.WarehouseCode='" + wareHouseCode +
                                              "' ORDER BY a.Year DESC, a.Month DESC ) " +
                                              "UPDATE TmpUpdate  SET Alocation='" + alocation + "', QtyOut='" + qtyOut + "', EndingOH='" + endingOH + "', EndingAV='" + endingAV + "', LastUpdateBy='" + user.UserId + "', LastUpdateDate='" + DateTime.Now + "'";
                                    }
                                    try
                                    {
                                        ctxx.Database.ExecuteSqlCommand(Qry);
                                        result = true;
                                    }
                                    catch
                                    {
                                        result = false; break; 
                                    }

                                    if (pType == "4W")
                                    {
                                        if (emp != null)
                                        {
                                            if (ctxx.Database.ExecuteSqlCommand("exec uspfn_SaveOmHstInquirySales2 '" + companyCode + "','" + branchCode + "','" + invoiceNo + "','" + CurrentUser.UserId + "'") <= 0) { result = false; break; }
                                            else
                                                result = true;
                                        }
                                        else
                                        {
                                            if (ctxx.Database.ExecuteSqlCommand("exec uspfn_SaveOmHstInquirySales '" + companyCode + "','" + branchCode + "','" + invoiceNo + "','" + CurrentUser.UserId + "'") <= 0) { result = false; break; }
                                            else
                                                result = true;
                                        }

                                    }
                                    else
                                    {
                                        if (emp != null)
                                        {
                                            if (ctxx.Database.ExecuteSqlCommand("exec uspfn_SaveOmHstInquirySales2 '" + companyCode + "','" + branchCode + "','" + invoiceNo + "','" + CurrentUser.UserId + "'") <= 0) { result = false; break; }
                                            else
                                                result = true;
                                        }
                                        else
                                        {
                                            if (ctxx.Database.ExecuteSqlCommand("exec uspfn_SaveOmHstInquirySales '" + companyCode + "','" + branchCode + "','" + invoiceNo + "','" + CurrentUser.UserId + "'") <= 0) { result = false; break; }
                                            else
                                                result = true;
                                        }
                                    }                                
                                }
                            }
                        }
                    }
                    else result = false;
                }
                else result = false;

                if (result && !independent && otom)
                {
                    int seq = 0;
                    decimal pct = 100;
                    decimal? CostPrice = 0;
                    decimal? CostPriceMD = 0;
                    decimal? RetailPriceMD = 0;
                    decimal? RetailPriceIncTaxMD = 0;
                    var qry = String.Empty;
                    string dbMD = ctxx.Database.SqlQuery<string>("SELECT DbMD from gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'").FirstOrDefault();
                    string Qry = "";

                    ctx.OmTrSalesSOAccsSeqs.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.SONo == record.SONo).ToList()
                    .ForEach(y =>
                    {
                        seq = seq + 1;
                        qry = "SELECT CostPrice FROM spTrnSLmpDtl WHERE CompanyCode='" + CompanyCode + "' AND BranchCode = '" + BranchCode + "' AND DocNo='" + y.SupplySlipNo + "' AND PartNo='" + y.PartNo + "'";
                        CostPrice = ctx.Database.SqlQuery<decimal?>(qry).FirstOrDefault();
            
                        if (CostPrice == null || CostPrice == 0)
                        {
                            result = false;
                            return;
                        }

                        qry = "SELECT RetailPrice, CostPrice, RetailPriceInclTax FROM " + dbMD + "..spMstItemPrice WHERE CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' AND PartNo='" + y.PartNo + "'";
                        var dtPriceMD = ctx.Database.SqlQuery<SpItemPrice>(qry).FirstOrDefault();
                        if (dtPriceMD != null)
                        {
                            CostPriceMD = dtPriceMD.CostPrice;
                            RetailPriceMD = dtPriceMD.RetailPrice;
                            RetailPriceIncTaxMD = dtPriceMD.RetailPriceInclTax;
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
                            CompanyCode, BranchCode, record.InvoiceNo, DateTime.Now, y.PartNo, seq, WarehouseMD
                            , y.Qty, y.Qty, pct, CostPrice, y.RetailPrice, y.TypeOfGoods, CompanyMD
                            , BranchMD, WarehouseMD, RetailPriceIncTaxMD, RetailPriceMD, CostPriceMD
                            , "-", ProductType, "300", "0", "0"
                            , DateTime.Now, CurrentUser.UserId, DateTime.Now, CurrentUser.UserId, DateTime.Now);

                            result = ctxx.Database.ExecuteSqlCommand(Qry) > 0;

                            if (!result)
                            {
                                errSVSDM = false;
                                return;
                            }
                    });
                }
                if (result)
                {
                    //if (ctxx.SaveChanges() > 0) // && oGnTrnBankBookDao.Insert(bankBook) > 0
                    //{ result = true; msg = msg1; }
                    //else
                    //{ result = false; msg = msg2; }
                    result = true; msg = msg1;
                }
                else
                {
                    if (errSVSDM)
                    {
                        msg = msg3;
                    } else
                        msg = msg2;
                }
            
            return result;
        }

        private  bool InsertDN(DataContext ctxx,  OmTrSalesInvoice record, ref string DNNo)
        {
            bool result = true;
            

            if (IsDNModelExist(ctxx, record) || isExistBBNKIR(ctxx, record.SONo))
            {
                var debitNote = new omTrSalesDN();
                debitNote.CompanyCode = record.CompanyCode;
                debitNote.BranchCode = record.BranchCode;
                debitNote.DNNo = DNNo = GetNewDNNo( ctxx,"DNU",(DateTime) record.InvoiceDate);
                if (debitNote.DNNo.EndsWith("X")) throw new ApplicationException
                    ( "Dokumen DNU belum diinput di tabel gnMstDocument");

                debitNote.DNDate = record.InvoiceDate;
                debitNote.InvoiceNo = record.InvoiceNo;
                debitNote.SONo = record.SONo;
                debitNote.CustomerCode = record.CustomerCode;
                debitNote.BillTo = record.BillTo;
                debitNote.DueDate = record.DueDate;
                debitNote.Remark = record.Remark;
                debitNote.Status = record.Status;
                debitNote.CreatedBy = debitNote.LastUpdateBy = CurrentUser.UserId;
                debitNote.CreatedDate = debitNote.LastUpdateDate = ctxx.CurrentTime;
                debitNote.isLocked = false;
                ctxx.omTrSalesDNs.Add(debitNote);
                result = ctxx.SaveChanges() > 0;
            }
            return result;
        }

        private  bool InsertDNModel(DataContext ctxx, OmTrSalesInvoice record, string DNNo)
        {
            bool result = true;
            //OmTrSalesDNModelDao oOmTrSalesDNModelDao = new OmTrSalesDNModelDao(ctx);
            if (IsDNModelExist( ctxx,record))
            {
                #region Select4PrintCheck
                var dtInvoiceModel = ctxx.omTrSalesInvoiceModel
                                        .Where(x => x.CompanyCode == CompanyCode &&
                                            x.BranchCode == BranchCode &&
                                            x.InvoiceNo == record.InvoiceNo)
                                            .ToList();
                        

                #endregion
                if (dtInvoiceModel.Count() > 0)
                {
                    foreach (var row in dtInvoiceModel)
                    {
                        var debitNoteModel = new OmTrSalesDNModel();
                        debitNoteModel.CompanyCode = record.CompanyCode;
                        debitNoteModel.BranchCode = record.BranchCode;
                        debitNoteModel.DNNo = DNNo;
                        debitNoteModel.SalesModelCode = row.SalesModelCode;
                        debitNoteModel.SalesModelYear = row.SalesModelYear;
                        debitNoteModel.Quantity = row.Quantity;
                        debitNoteModel.ShipAmt = row.ShipAmt;
                        debitNoteModel.DepositAmt = row.DepositAmt;
                        debitNoteModel.OthersAmt = row.OthersAmt;
                        debitNoteModel.CreatedBy = debitNoteModel.LastUpdateBy = CurrentUser.UserId;
                        debitNoteModel.CreatedDate = debitNoteModel.LastUpdateDate = ctxx.CurrentTime;

                            ctxx.OmTrSalesDNModels
                                .Add(debitNoteModel);
                        if (ctxx.SaveChanges() < 0)
                        { 
                            result = false; 
                            break; 
                        }
                    }
                }
            }
            return result;
        }

        private  bool IsDNModelExist(DataContext ctxx, OmTrSalesInvoice record)
        {
         string ssql=@"
                select sum(isnull(ShipAmt, 0)+isnull(DepositAmt, 0)+isnull(OthersAmt, 0)) 
                from omtrsalesInvoiceModel where companyCode = {0} and branchCode = {1}
                and InvoiceNo = '{2}'
            ";          
            
            var row = ctxx.Database.SqlQuery<decimal?>(string.Format(ssql,CompanyCode,BranchCode,record.InvoiceNo)).FirstOrDefault();

            try
            {
                if (row > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }         
        }

        private bool isExistBBNKIR(DataContext ctxx, string soNo)
        {
            string sql = @"
                            select sum(isnull(a.BBN,0)+isnull(a.KIR,0)) from OmTrSalesSOVin a 
                            where a.CompanyCode = {0}
                            and a.BranchCode = {1}
                            and a.SONo = '{2}'
                        ";
            sql = string.Format(sql, CompanyCode, BranchCode, soNo);

            var amt = ctxx.Database.SqlQuery<decimal?>(sql).FirstOrDefault();
            try
            {
                return amt > 0;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        private string GetNewDNNo(DataContext ctxx, string doctype, DateTime transdate)
        {
            var sql = "uspfn_GnDocumentGetNew {0}, {1}, {2}, {3}, {4}";
            var result = ctxx.Database.SqlQuery<string>(sql, CompanyCode, BranchCode, doctype, CurrentUser.UserId, transdate);
            return result.First();
        }

        #endregion

        private void SDMovementDO(OmMstVehicle record, OmTrSalesDO recordDO)
        {
            var data = ctxMD.omSDMovements.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == recordDO.DONo);
            var seq = data.FirstOrDefault() == null ? 0 : data.Max(a => a.Seq);

            try
            {
                var oSDMovement = new omSDMovement()
                {
                    CompanyCode = recordDO.CompanyCode,
                    BranchCode = recordDO.BranchCode,
                    DocNo = recordDO.DONo,
                    DocDate = recordDO.DODate,
                    Seq = seq,
                    SalesModelCode = record.SalesModelCode,
                    SalesModelYear = record.SalesModelYear,
                    ChassisCode = record.ChassisCode,
                    ChassisNo = record.ChassisNo,
                    EngineCode = record.EngineCode,
                    EngineNo = record.EngineNo,
                    ColourCode = record.ColourCode,
                    WarehouseCode = record.WarehouseCode,
                    CustomerCode = recordDO.CustomerCode,
                    QtyFlag = "-",
                    CompanyMD = CompanyMD,
                    BranchMD = UnitBranchMD,
                    WarehouseMD = WarehouseMD,
                    Status = recordDO.Status,
                    ProcessStatus = "0",
                    ProcessDate = DateTime.Now,
                    CreatedBy = record.CreatedBy,
                    CreatedDate = record.CreatedDate,
                    LastUpdateBy = record.LastUpdateBy,
                    LastUpdateDate = record.LastUpdateDate
                };
                ctxMD.omSDMovements.Add(oSDMovement);
                ctxMD.SaveChanges();
            }
            catch { }
        }

        private bool svSDMovementInvoice(DataContext ctxx, OmTrSalesInvoiceAccsSeq record, string WhCode, int seq)
        {
            string dbMD = ctxx.Database.SqlQuery<string>("SELECT DbMD from gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'").FirstOrDefault();
            string Qry = "";
            var qry = String.Empty;
            decimal pct = 100;
            decimal? CostPrice = 0;
            decimal? CostPriceMD = 0;
            decimal? RetailPriceMD = 0;

            qry = "SELECT CostPrice FROM spTrnSLmpDtl WHERE CompanyCode='" + CompanyCode + "' AND BranchCode = '" + BranchCode + "' AND DocNo='" + record.SupplySlipNo + "' AND PartNo='" + record.PartNo + "'";
            CostPrice = ctx.Database.SqlQuery<decimal?>(qry).FirstOrDefault();
            
            if (CostPrice == null || CostPrice == 0)
            {
                return false;
            }
            
            qry = "SELECT RetailPrice, CostPrice FROM " + dbMD + "..spMstItemPrice WHERE CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' AND PartNo='" + record.PartNo + "'";
            var dtPriceMD = ctx.Database.SqlQuery<SpItemPrice>(qry).FirstOrDefault();
            if (dtPriceMD != null)
            {
                CostPriceMD = dtPriceMD.CostPrice;
                RetailPriceMD = dtPriceMD.RetailPrice;
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
                CompanyCode, BranchCode, record.InvoiceNo, DateTime.Now, record.PartNo, seq, WarehouseMD
                , record.Quantity.Value, record.Quantity.Value, pct, CostPrice, record.RetailPrice.Value, record.TypeOfGoods, CompanyMD
                , BranchMD, WarehouseMD, record.Total, RetailPriceMD, CostPriceMD
                , "-", ProductType, ProfitCenter, "0", "0"
                , DateTime.Now, CurrentUser.UserId, DateTime.Now, CurrentUser.UserId, DateTime.Now);

            try
            {
                ctxx.Database.ExecuteSqlCommand(Qry);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private bool SDMovementInvoice(DataContext ctxx, omTrSalesInvoiceVin record, string WhCode)
        {
            string dbMD = ctxx.Database.SqlQuery<string>("SELECT DbMD from gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'").FirstOrDefault();
            string Qry = "";
            var invoice = ctxx.OmTrSalesInvoices.FirstOrDefault(a => a.CompanyCode == record.CompanyCode && a.BranchCode == record.BranchCode && a.InvoiceNo == record.InvoiceNo);
            //var data = ctxMD.omSDMovements.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == record.InvoiceNo);
            var data = ctxx.Database.SqlQuery<omSDMovement>("SELECT * FROM " + dbMD + "..omSDMovement WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "' AND DocNo='" + record.InvoiceNo + "'");           
            var seq = data.FirstOrDefault() == null ? 0 : data.Max(a => a.Seq);

            Qry = "INSERT INTO " + dbMD + "..omSDMovement(CompanyCode, BranchCode, DocNo, DocDate, Seq, SalesModelCode, SalesModelYear, ChassisCode, ChassisNo, EngineCode," +
                  "EngineNo, ColourCode, WarehouseCode, CustomerCode, QtyFlag, CompanyMD, BranchMD, WarehouseMD, Status, ProcessStatus, ProcessDate, CreatedBy," +
                  "CreatedDate, LastUpdateBy, LastUpdateDate) Values('" +
                   record.CompanyCode + "','" + record.BranchCode + "','" + record.InvoiceNo + "','" + record.CreatedDate + "','" + seq + "','" + record.SalesModelCode +
                   "','" + record.SalesModelYear + "','" + record.ChassisCode + "','" + record.ChassisNo + "','" + record.EngineCode + "','" + record.EngineNo +
                   "','" + record.ColourCode + "','" + WhCode + "','" + invoice.CustomerCode + "','-','" + CompanyMD + "','" + UnitBranchMD + "','" + WarehouseMD +
                   "','" + invoice.Status + "','0','" + DateTime.Now + "','" + record.CreatedBy + "','" + record.CreatedDate + "','" + record.LastUpdateBy + "','" + record.LastUpdateDate + "')";
            try
            {
                ctxx.Database.ExecuteSqlCommand(Qry);
                return true;
            }
            catch
            {
                return false;
            }
            //try
            //{
            //    var oSDMovement = new omSDMovement()
            //    {
            //        CompanyCode = record.CompanyCode,
            //        BranchCode = record.BranchCode,
            //        DocNo = record.InvoiceNo,
            //        DocDate = record.CreatedDate,
            //        Seq = seq,
            //        SalesModelCode = record.SalesModelCode,
            //        SalesModelYear = record.SalesModelYear,
            //        ChassisCode = record.ChassisCode,
            //        ChassisNo = record.ChassisNo,
            //        EngineCode = record.EngineCode,
            //        EngineNo = record.EngineNo,
            //        ColourCode = record.ColourCode,
            //        WarehouseCode = WhCode,
            //        CustomerCode = invoice.CustomerCode,
            //        QtyFlag = "-",
            //        CompanyMD = CompanyMD,
            //        BranchMD = BranchMD,
            //        WarehouseMD = WarehouseMD,
            //        Status = invoice.Status,
            //        ProcessStatus = "0",
            //        ProcessDate = DateTime.Now,
            //        CreatedBy = record.CreatedBy,
            //        CreatedDate = record.CreatedDate,
            //        LastUpdateBy = record.LastUpdateBy,
            //        LastUpdateDate = record.LastUpdateDate
            //    };
            //    ctxMD.omSDMovements.Add(oSDMovement);
            //    ctxMD.SaveChanges();
            //}
            //catch { }
        }

        public void SDMovementSO(OmTRSalesSO record)
        {

            //var so = ctx.OmTRSalesSO.FirstOrDefault(a => a.CompanyCode == record.CompanyCode && a.BranchCode == record.BranchCode && a.InvoiceNo == record.InvoiceNo);
            //var data = ctxMD.omSDMovements.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.DocNo == record.InvoiceNo);
            //var seq = data.FirstOrDefault() == null ? 0 : data.Max(a => a.Seq);
            int i = 0;
            ctx.omTrSalesSOVins.Where(x => x.SONo == record.SONo)
                       .ToList()
                       .ForEach(x =>
                       {
                           i++;
                           
                           var oSDMovement = new omSDMovement()
                           {
                               CompanyCode = x.CompanyCode,
                               BranchCode = x.BranchCode,
                               DocNo = record.SONo,
                               DocDate = record.SODate,
                               Seq = i,
                               SalesModelCode = x.SalesModelCode,
                               SalesModelYear = x.SalesModelYear,
                               ChassisCode = x.ChassisCode,
                               ChassisNo = x.ChassisNo,
                               EngineCode = x.EngineCode,
                               EngineNo = x.EngineNo,
                               ColourCode = x.ColourCode,
                               WarehouseCode = record.WareHouseCode,
                               CustomerCode = record.CustomerCode,
                               QtyFlag = "-",
                               CompanyMD = CompanyMD,
                               BranchMD = UnitBranchMD,
                               WarehouseMD = WarehouseMD,
                               Status = record.Status,
                               ProcessStatus = "0",
                               ProcessDate = DateTime.Now,
                               CreatedBy = record.CreatedBy,
                               CreatedDate = record.CreatedDate,
                               LastUpdateBy = record.LastUpdateBy,
                               LastUpdateDate = record.LastUpdateDate
                           };
                           ctxMD.omSDMovements.Add(oSDMovement);

                       });

            ctxMD.SaveChanges();

        }

        public void SDMovementSORem(OmTRSalesSO record)
        {


            var mov = ctxMD.omSDMovements.Where(x => x.CompanyCode == record.CompanyCode &&
                                        x.BranchCode == record.BranchCode &&
                                        x.DocNo == record.SONo)
                                        .ToList();

            if (mov.Count > 0)
            {
                mov.ForEach(x => ctxMD.omSDMovements.Remove(x));
                ctxMD.SaveChanges();
            }
        }

        public void svSDMovementSORem(OmTRSalesSO record)
        {
            ctx.Database.ExecuteSqlCommand("DELETE FROM svSDMovement WHERE CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' AND DocNo='" + record.SONo + "'");
        }
    }
}

