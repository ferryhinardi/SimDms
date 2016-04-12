using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Common.Models;
using System.Transactions;

namespace SimDms.Sales.Controllers.Api
{
    public class FakturPolisiRevisiController : BaseController
    {
        private const string RRF = "RRF"; // Revisi Faktur Polis

        private string getStringStatus(string status)
        {
            var Status = status == "0" ? "OPEN"
                           : status == "1" ? "PRINTED"
                           : status == "2" ? "APPROVED"
                           : status == "3" ? "CANCELED"
                           : status == "9" ? "FINISHED" : "";
            return Status;
        }

        public JsonResult getDataAkhir(string ChassisCode, decimal ChassisNo) 
        {
            var record = ctx.omTrSalesFPolRevision.Where(x => x.ChassisCode == ChassisCode && x.ChassisNo == ChassisNo).OrderByDescending(x => x.LastUpdateDate).FirstOrDefault();
            if (record != null)
            {
                var CityName = ctx.LookUpDtls.Find(CompanyCode, "CITY", record.FakturPolisiCity);
                var revName =  ctx.LookUpDtls.Find(CompanyCode, "REVI", record.RevisionCode);
                return Json(new { success = true, dataAkhir = record, FakturPolisiCityName = CityName.LookUpValueName, RevisionName = revName.LookUpValueName }); 
            }
            else
            {
                return Json(new { success = false, dataAkhir = "" });
            }
        }

        public JsonResult LookUpDtls(string CodeID, string lookupvalue)
        {
            var record = ctx.LookUpDtls.Find(CompanyCode, CodeID, lookupvalue);
            return Json(new { success = record != null, data = record });
        }

        public JsonResult FakturPolisiCity2()
        {
            var CodeID = "CITY";
            string CodeCity = ctx.Database.SqlQuery<string>("SELECT TOP 1 CityCode FROM gnMstCoProfile WHERE CompanyCode='" + CompanyCode + "' ORDER BY BranchCode ASC").FirstOrDefault();
            if (CodeCity != null)
            {
                var record = ctx.LookUpDtls.Find(CompanyCode, CodeID, CodeCity);
                return Json(new { success = record != null, data = record.LookUpValueName });
            }
            return Json(new { success = false, message = "Data kota tidak ada!" });
        }

        public JsonResult Save(omTrSalesFPolRevision model, string reqno)
        {
            try
            {
                string msg = "";
                var currentdate = DateTime.Now;
                var record = ctx.omTrSalesFPolRevision.Find(CompanyCode, BranchCode, model.RevisionNo);
                
                
                if (record == null)
                {
                    SaveRevisionHistory(model, reqno);
                    record = new omTrSalesFPolRevision
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        RevisionNo = GetNewDocumentNo(RRF, currentdate),
                        RevisionDate = currentdate,
                        SubDealerCode = model.SubDealerCode,
                        RevisionSeq = (ctx.omTrSalesFPolRevisionHistory.Where(x => x.ChassisCode == model.ChassisCode && x.ChassisNo == model.ChassisNo).OrderByDescending(y => y.RevisionSeq).FirstOrDefault().RevisionSeq),
                        RevisionCode = model.RevisionCode,
                        ChassisCode = model.ChassisCode,
                        ChassisNo = model.ChassisNo,
                        FakturPolisiName = model.FakturPolisiName,
                        FakturPolisiAddress1 = model.FakturPolisiAddress1,
                        FakturPolisiAddress2 = model.FakturPolisiAddress2,
                        FakturPolisiAddress3 = model.FakturPolisiAddress3,
                        PostalCode = model.PostalCode,
                        PostalCodeDesc = model.PostalCodeDesc,
                        FakturPolisiCity = model.FakturPolisiCity,
                        FakturPolisiTelp1 = model.FakturPolisiTelp1,
                        FakturPolisiTelp2 = model.FakturPolisiTelp2,
                        FakturPolisiHP = model.FakturPolisiHP,
                        FakturPolisiBirthday = model.FakturPolisiBirthday,
                        FakturPolisiNo = model.FakturPolisiNo,
                        FakturPolisiDate = model.FakturPolisiDate,
                        FakturPolisiArea = model.FakturPolisiArea,
                        IDNo = model.IDNo,
                        IsCityTransport = model.IsCityTransport,
                        IsProject = model.IsProject,
                        SendCounter = model.SendCounter,
                        Status = "0",
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = currentdate,
                        LastUpdateBy = CurrentUser.UserId,
                        LastUpdateDate = currentdate,
                    };

                    ctx.omTrSalesFPolRevision.Add(record);
                }
                else
                {
                    record.CompanyCode = CompanyCode;
                    record.BranchCode = BranchCode;
                    record.RevisionNo = model.RevisionNo;
                    record.RevisionDate = model.RevisionDate;
                    record.RevisionSeq = (ctx.omTrSalesFPolRevisionHistory.Where(x => x.ChassisCode == model.ChassisCode && x.ChassisNo == model.ChassisNo).OrderByDescending(y => y.RevisionSeq).FirstOrDefault().RevisionSeq);
                    record.RevisionCode = model.RevisionCode;
                    record.ChassisCode = model.ChassisCode;
                    record.ChassisNo = model.ChassisNo;
                    record.FakturPolisiName = model.FakturPolisiName;
                    record.FakturPolisiAddress1 = model.FakturPolisiAddress1;
                    record.FakturPolisiAddress2 = model.FakturPolisiAddress2;
                    record.FakturPolisiAddress3 = model.FakturPolisiAddress3;
                    record.PostalCode = model.PostalCode;
                    record.PostalCodeDesc = model.PostalCodeDesc;
                    record.FakturPolisiCity = model.FakturPolisiCity;
                    record.FakturPolisiTelp1 = model.FakturPolisiTelp1;
                    record.FakturPolisiTelp2 = model.FakturPolisiTelp2;
                    record.FakturPolisiHP = model.FakturPolisiHP;
                    record.FakturPolisiBirthday = model.FakturPolisiBirthday;
                    record.FakturPolisiNo = model.FakturPolisiNo;
                    record.FakturPolisiDate = model.FakturPolisiDate;
                    record.FakturPolisiArea = model.FakturPolisiArea;
                    record.IDNo = model.IDNo;
                    record.IsCityTransport = model.IsCityTransport;
                    record.IsProject = model.IsProject;
                    record.SubDealerCode = model.SubDealerCode;
                    record.Status = "0";
                    record.SendCounter = model.SendCounter;
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = currentdate;
                }

                Helpers.ReplaceNullable(record);
                ctx.SaveChanges();
                return Json(new { success = true, message = msg, data = record, status = getStringStatus(record.Status) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool SaveRevisionHistory(omTrSalesFPolRevision model, string reqno)
        {
            var md = false;
            var record = ctx.omTrSalesReqDetail.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.ChassisCode == model.ChassisCode && x.ChassisNo == model.ChassisNo).FirstOrDefault();
            var recordHst = ctx.omTrSalesFPolRevisionHistory.Where(x => x.ChassisCode == model.ChassisCode && x.ChassisNo == model.ChassisNo).OrderByDescending(y => y.RevisionSeq).FirstOrDefault();
            String qtbl = @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name LIKE 'omTrSalesFPolRevisionHistory')
                            CREATE TABLE [dbo].[omTrSalesFPolRevisionHistory]( --update sequence 
	                        [ChassisCode] [varchar](15) NOT NULL,
	                        [ChassisNo] [numeric](10, 0) NOT NULL,
	                        [RevisionSeq] [int] NOT NULL,
	                        [RevisionNo] [varchar](15) NULL,
	                        [RevisionCode] [varchar](15) NOT NULL,
	                        [FakturPolisiName] [varchar](100) NULL,
	                        [FakturPolisiAddress1] [varchar](100) NULL,
	                        [FakturPolisiAddress2] [varchar](100) NULL,
	                        [FakturPolisiAddress3] [varchar](100) NULL,
	                        [PostalCode] [varchar](15) NULL,
	                        [PostalCodeDesc] [varchar](100) NULL,
	                        [FakturPolisiCity] [varchar](15) NULL,
	                        [FakturPolisiTelp1] [varchar](15) NULL,
	                        [FakturPolisiTelp2] [varchar](15) NULL,
	                        [FakturPolisiHP] [varchar](15) NULL,
	                        [FakturPolisiBirthday] [datetime] NULL,
	                        [FakturPolisiNo] [varchar](15) NULL,
	                        [FakturPolisiDate] [datetime] NULL,
	                        [FakturPolisiArea] [varchar](15) NULL,
	                        [IDNo] [varchar](100) NULL,
	                        [IsCityTransport] [bit] NOT NULL,
	                        [IsProject] [bit] NULL,
	                        [SubDealerCode] [varchar](15) NULL,
	                        [CreatedBy] [varchar](15) NOT NULL,
	                        [CreatedDate] [datetime] NOT NULL,
	                        [LastUpdateBy] [varchar](15) NULL,
	                        [LastUpdateDate] [datetime]  NOT NULL,
                         CONSTRAINT [PK__omTrSalesFPolRevisionHistory__5D509081] PRIMARY KEY CLUSTERED 
                        (
	                        [ChassisNo] ASC,
	                        [ChassisCode] ASC,
	                        [RevisionSeq] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
                        ) ON [PRIMARY]
";

            ctx.Database.ExecuteSqlCommand(qtbl);

            var vehicle = ctx.OmMstVehicles.Where(a => a.CompanyCode == CompanyCode && a.ChassisNo == model.ChassisNo).FirstOrDefault(); ;

            if (vehicle == null)
            {
                vehicle = ctxMD.OmMstVehicles.Where(a => a.CompanyCode == CompanyMD && a.ChassisCode == model.ChassisCode && a.ChassisNo == model.ChassisNo).FirstOrDefault();
                md = false;
            }
            var seqNo = 1;
            if (record != null && recordHst == null)
            {
                //save log
                recordHst = new omTrSalesFPolRevisionHistory
                {
                    RevisionSeq = seqNo,
                    RevisionNo = "",
                    ChassisCode = record.ChassisCode,
                    ChassisNo = record.ChassisNo,
                    FakturPolisiName = record.FakturPolisiName,
                    FakturPolisiAddress1 = record.FakturPolisiAddress1,
                    FakturPolisiAddress2 = record.FakturPolisiAddress2,
                    FakturPolisiAddress3 = record.FakturPolisiAddress3,
                    PostalCode = record.PostalCode,
                    PostalCodeDesc = record.PostalCodeDesc,
                    FakturPolisiCity = record.FakturPolisiCity,
                    FakturPolisiTelp1 = record.FakturPolisiTelp1,
                    FakturPolisiTelp2 = record.FakturPolisiTelp2,
                    FakturPolisiHP = record.FakturPolisiHP,
                    FakturPolisiBirthday = record.FakturPolisiBirthday,
                    IsCityTransport = record.IsCityTransport,
                    FakturPolisiNo = record.FakturPolisiNo == null ? "" : record.FakturPolisiNo,
                    RevisionCode = model.RevisionCode,
                    FakturPolisiDate = record.FakturPolisiDate,
                    FakturPolisiArea = record.FakturPolisiArea,
                    IDNo = record.IDNo,
                    IsProject = record.IsProject,
                    CreatedBy = record.CreatedBy,
                    CreatedDate = record.CreatedDate,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = DateTime.Now,
                };
                ctx.omTrSalesFPolRevisionHistory.Add(recordHst);
            }
            else
            {
                var recordRev = ctx.omTrSalesFPolRevision.Where(x => x.ChassisCode == model.ChassisCode && x.ChassisNo == model.ChassisNo).OrderByDescending(y => y.RevisionSeq).FirstOrDefault();
                recordHst = new omTrSalesFPolRevisionHistory
                {
                    RevisionSeq = recordHst.RevisionSeq + 1,
                    RevisionNo = recordRev.RevisionNo,
                    ChassisCode = recordRev.ChassisCode,
                    ChassisNo = recordRev.ChassisNo,
                    FakturPolisiName = recordRev.FakturPolisiName,
                    FakturPolisiAddress1 = recordRev.FakturPolisiAddress1,
                    FakturPolisiAddress2 = recordRev.FakturPolisiAddress2,
                    FakturPolisiAddress3 = recordRev.FakturPolisiAddress3,
                    PostalCode = recordRev.PostalCode,
                    PostalCodeDesc = recordRev.PostalCodeDesc,
                    FakturPolisiCity = recordRev.FakturPolisiCity,
                    FakturPolisiTelp1 = recordRev.FakturPolisiTelp1,
                    FakturPolisiTelp2 = recordRev.FakturPolisiTelp2,
                    FakturPolisiHP = recordRev.FakturPolisiHP,
                    FakturPolisiBirthday = recordRev.FakturPolisiBirthday,
                    IsCityTransport = recordRev.IsCityTransport,
                    FakturPolisiNo = recordRev.FakturPolisiNo == null ? "" : recordRev.FakturPolisiNo,
                    RevisionCode = recordRev.RevisionCode,
                    FakturPolisiDate = recordRev.FakturPolisiDate,
                    FakturPolisiArea = recordRev.FakturPolisiArea,
                    IDNo = recordRev.IDNo,
                    IsProject = recordRev.IsProject,
                    CreatedBy = record.CreatedBy,
                    CreatedDate = record.CreatedDate,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = DateTime.Now
                };
                ctx.omTrSalesFPolRevisionHistory.Add(recordHst);
            }

            Helpers.ReplaceNullable(recordHst);
            ctx.SaveChanges();
            //try
            //{
            //    Helpers.ReplaceNullable(record);
            //    //vehicle.ReqOutNo = record.ReqNo;
            //    if (md)
            //    {
            //        ctxMD.SaveChanges();
            //    }
            //    ctx.SaveChanges();
            //    return Json(new { success = true, data = record });
            //}
            //catch (Exception ex)
            //{
            //    return Json(new { success = false, message = ex.Message });
            //}
            return true;
        }

        public JsonResult Delete(omTrSalesReq model)
        {
            var record = ctx.omTrSalesReq.Find(CompanyCode, BranchCode, model.ReqNo);

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.Database.ExecuteSqlCommand(@"UPDATE omTrSalesReq SET Status = 3
                                                WHERE CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode +
                                                "' and ReqNo='" + model.ReqNo + "'");

                ctx.Database.ExecuteSqlCommand(@"DELETE omTrSalesReqDetail 
                                                WHERE CompanyCode='" + CompanyCode + "' and BranchCode='" + BranchCode +
                                                "' and ReqNo='" + model.ReqNo + "'");


            }

            try
            {
                ctx.SaveChanges();

                return Json(new { success = true, data = record, Status = "Canceled" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult prePrint(omTrSalesFPolRevision model)
        {
            var msg = "";
            var Hdr = ctx.omTrSalesFPolRevision.Find(CompanyCode, BranchCode, model.RevisionNo);

            if (Hdr.Status == "0" || Hdr.Status.Trim() == "")
            {
                Hdr.Status = "1";
            }

            Hdr.LastUpdateBy = CurrentUser.UserId;
            Hdr.LastUpdateDate = DateTime.Now;

            var result = ctx.SaveChanges() >= 0;

            return Json(new { success = result, Status = getStringStatus(Hdr.Status), stat = Hdr.Status, Userid= CurrentUser.UserId });
        }

        public JsonResult Approve(omTrSalesReq model, string pType)
        {
            var msg = "";
            bool result = false;
            bool independent = false;
            bool otom = cekOtomatis();
            //var usr = CurrentUser.UserId
            string dbMD = ctx.Database.SqlQuery<string>("SELECT DbMD from gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'").FirstOrDefault();

            if (CompanyCode == CompanyMD && BranchCode == BranchMD)
            {
                independent = true;
            }

            if (dbMD == null && !independent && !otom)
            {
                return Json(new { success = result, message = "Data MD tidak ada!" });
            }
            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    if (pType == "2W")
                    {
                        ctx.omTrSalesReqDetail.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.ReqNo == model.ReqNo).ToList().ForEach(y =>
                        {
                            var dtAngket = ctx.omTrSalesReqDetailAdditionals.Find(CompanyCode, BranchCode, model.ReqNo, y.ChassisCode, Convert.ToString(y.ChassisNo));
                            if (dtAngket == null)
                            {
                                msg = "Kendaraan dengan detail :" + "\n" + "ChassisCode : " + y.ChassisCode + "\n" + "ChassisNo : " + y.ChassisNo + "\n" + "belum memiliki Info Customer" + "\n" + "Silahkan input additional info Customer pada bagian Detail Customer";
                                return;
                            }
                        });
                    }
                    var record = ctx.omTrSalesReq.Find(CompanyCode, BranchCode, model.ReqNo);
                    if (record != null)
                    {
                        record.Status = "2";
                        record.LastUpdateBy = CurrentUser.UserId;
                        record.LastUpdateDate = DateTime.Now;
                        record.ApproveDate = DateTime.Now;
                        bool isNonRegister = true;
                        if (record.StatusFaktur == "1") isNonRegister = false;

                        result = ctx.SaveChanges() >= 0;

                        if (result)
                        {
                            string qry = @"SELECT a.ChassisCode, a.ChassisNo, a.ReqNo, a.FakturPolisiNo, isnull
                                        (a.fakturpolisidate,'1900/01/01')  AS FakturPolisiDate, c.PoliceRegistrationNo, isnull
                                        (c.PoliceRegistrationDate,'1900/01/01')  AS PoliceRegistrationDate, a.SONo
                                        FROM omTrSalesReqDetail a LEFT JOIN OmTrSalesFakturPolisi b ON a.CompanyCode = b.CompanyCode
                                        AND a.BranchCode = b.BranchCode AND a.FakturPolisiNo = b.FakturPolisiNo
                                        LEFT JOIN omTrSalesSPKDetail c ON a.CompanyCode = c.CompanyCode
                                        AND a.BranchCode = c.BranchCode
                                        AND a.ChassisCode = c.ChassisCode
                                        AND a.ChassisNo = c.ChassisNo
                                        WHERE a.CompanyCode = '" + CompanyCode + "' AND a.BranchCode = '" + BranchCode + "' AND a.ReqNo = '" + model.ReqNo + "'";
                            ctx.Database.SqlQuery<fpDetail>(qry).ToList().ForEach(y =>
                            {
                                if (result)
                                {
                                    var vehicle = ctx.OmMstVehicles.Find(CompanyCode, y.ChassisCode, y.ChassisNo);
                                    if (vehicle != null)
                                    {
                                        vehicle.ReqOutNo = y.ReqNo;
                                        vehicle.FakturPolisiDate = y.FakturPolisiDate;
                                        vehicle.IsNonRegister = isNonRegister;
                                        vehicle.LastUpdateBy = CurrentUser.UserId;
                                        vehicle.LastUpdateDate = DateTime.Now;

                                        result = ctx.SaveChanges() >= 0;
                                    }
                                    else
                                    {
                                        //MD
                                        string qr = @"UPDATE " + dbMD + "..OmMstVehicle SET ReqOutNo='" + y.ReqNo +
                                                    "', FakturPolisiDate='" + y.FakturPolisiDate + "', IsNonRegister='" + isNonRegister +
                                                    "', LastUpdateBy='" + CurrentUser.UserId + "', LastUpdateDate='" + DateTime.Now +
                                                    "' WHERE CompanyCode='" + CompanyMD + "' AND ChassisCode='" + y.ChassisCode + "' AND ChassisNo='" + y.ChassisNo + "'";

                                        result = ctx.Database.ExecuteSqlCommand(qr) > 0;
                                    }

                                    if (result)
                                    {
                                        //insert history sales
                                        ctx.Database.ExecuteSqlCommand("EXEC uspfn_InsertOmHstInquirySales '" + CompanyCode + "','" + BranchCode + "','" + y.ChassisCode + "','" + y.ChassisNo + "','" + y.SONo + "','" + CurrentUser.UserId + "'");
                                        //
                                        var fPol = ctx.OmTrSalesFakturPolisi.Find(CompanyCode, BranchCode, y.FakturPolisiNo);
                                        if (fPol != null)
                                        {
                                            fPol.ReqNo = y.ReqNo;
                                            fPol.FakturPolisiDate = y.FakturPolisiDate;
                                            fPol.LastUpdateBy = CurrentUser.UserId;
                                            fPol.LastUpdateDate = DateTime.Now;

                                            result = ctx.SaveChanges() >= 0;

                                            if (!result)
                                            {
                                                msg = "Proses update omTrSalesFakturPolisi gagal!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        msg = "Terdapat kesalahan update omMstVehicle!";
                                    }
                                }
                            });
                        }
                        if (result)
                        {
                            tranScope.Complete();
                            return Json(new { success = result, message = msg, status = getStringStatus(record.Status), Result = record.Status });
                        }
                    }
                    else
                    {
                        msg = "Data permohonan faktur polisi tidak ada!";
                    }
                    return Json(new { success = result, message = msg });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }

            /*
            var record = ctx.omTrSalesReq.Find(CompanyCode, BranchCode, model.ReqNo);
            if (record != null)
            {
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
                record.Status = "2";
                var query = String.Format(@"
                                            UPDATE c 
                                            SET c.ReqOutNo='{0}',
                                                c.FakturPolisiDate=b.FakturPolisiDate, 
											    c.IsNonRegister=CASE WHEN a.StatusFaktur='1' THEN '0' ELSE '1' END,
											    c.LastUpdateBy='{1}'
                                            FROM omTrSalesReq a
                                              INNER JOIN omTrSalesReqDetail b
	                                             ON a.ReqNo=b.ReqNo
                                             INNER JOIN omMstVehicle c
	                                            ON b.ChassisCode=c.ChassisCode 
                                                   and b.ChassisNo=c.ChassisNo
                                            WHERE a.CompanyCode='{2}'
                                            and a.BranchCode='{3}'
                                            and a.ReqNo= '{0}'
                ", model.ReqNo, CurrentUser.UserId, CompanyCode, BranchCode);

                ctx.Database.ExecuteSqlCommand(query);
                result = ctx.SaveChanges() >= 0;
                return Json(new { success = result, message = msg, status = getStringStatus(record.Status), Result = record.Status }); 
              */
        }

        #region not use but must look
        public JsonResult checkOnMstLookup()
        {
            string dt = ctx.Database.SqlQuery<string>("SELECT ParaValue FROM gnMstLookupDtl WHERE CodeID='IsFaktur' AND  CompanyCode='" + CompanyCode + "'").FirstOrDefault();
            if (dt == null || dt == "" || dt == "0")
            {
                return Json(new { success = false, message = "" });
            }
            return Json(new { success = true, message = "" });
        }

        public JsonResult SubDealerCode(SalesReqView model)
        {
            var record = ctx.Database.SqlQuery<SubDealerLookup>("uspfn_PenjualFPLookup '" + CompanyCode + "','" + BranchCode + "','" + ProfitCenter + "','" + model.SubDealerCode + "'");

            return Json(new { success = record != null, data = record });
        }

        public JsonResult SKPKCity(SalesReqDetailView model)
        {
            var CodeID = "CITY";
            var record = ctx.LookUpDtls.Find(CompanyCode, CodeID, model.SKPKCity);
            return Json(new { success = record != null, data = record });
        }

        public JsonResult SaveDetailSD(omTrSalesReq model, omTrSalesReqDetail detailModel)
        {
            var record = ctx.omTrSalesReqDetail.Find(CompanyCode, BranchCode, model.ReqNo, detailModel.ChassisCode, detailModel.ChassisNo);
            var vehicle = ctx.OmMstVehicles.Where(a => a.CompanyCode == CompanyCode && a.ChassisNo == detailModel.ChassisNo).FirstOrDefault(); ;

            if (record == null)
            {
                record = new omTrSalesReqDetail
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ReqNo = model.ReqNo,
                    ChassisCode = detailModel.ChassisCode,
                    ChassisNo = detailModel.ChassisNo,
                    SONo = detailModel.SONo,
                    BPKNo = detailModel.BPKNo,
                    DealerCategory = detailModel.DealerCategory,
                    SKPKNo = detailModel.SKPKNo,
                    SalesmanName = detailModel.SalesmanName,
                    SalesmanCode = detailModel.SalesmanCode,
                    SKPKName = detailModel.SKPKName,
                    SKPKAddress1 = detailModel.SKPKAddress1,
                    SKPKAddress2 = detailModel.SKPKAddress2,
                    SKPKAddress3 = detailModel.SKPKAddress3,
                    SKPKCity = detailModel.SKPKCity,
                    SKPKTelp1 = detailModel.SKPKTelp1,
                    SKPKTelp2 = detailModel.SKPKTelp2,
                    SKPKHP = detailModel.SKPKHP,
                    SKPKBirthday = detailModel.SKPKBirthday,
                    FakturPolisiName = detailModel.FakturPolisiName,
                    FakturPolisiAddress1 = detailModel.FakturPolisiAddress1,
                    FakturPolisiAddress2 = detailModel.FakturPolisiAddress2,
                    FakturPolisiAddress3 = detailModel.FakturPolisiAddress3,
                    PostalCode = detailModel.PostalCode,
                    PostalCodeDesc = detailModel.PostalCodeDesc,
                    FakturPolisiCity = detailModel.FakturPolisiCity,
                    FakturPolisiTelp1 = detailModel.FakturPolisiTelp1,
                    FakturPolisiTelp2 = detailModel.FakturPolisiTelp2,
                    FakturPolisiHP = detailModel.FakturPolisiHP,
                    FakturPolisiBirthday = detailModel.FakturPolisiBirthday,
                    IsCityTransport = detailModel.IsCityTransport,
                    FakturPolisiNo = detailModel.FakturPolisiNo == null ? "" : detailModel.FakturPolisiNo,
                    ReasonCode = detailModel.ReasonCode == null ? "" : detailModel.ReasonCode,
                    ReasonNonFaktur = detailModel.ReasonNonFaktur,
                    FakturPolisiDate = detailModel.FakturPolisiDate,
                    FakturPolisiArea = detailModel.FakturPolisiArea,
                    IDNo = detailModel.IDNo,
                    IsProject = detailModel.IsProject,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                    DOImniNo = vehicle.SuzukiDONo,
                    DOImniDate = vehicle.SuzukiDODate,
                    SJImniNo = vehicle.SuzukiSJNo,
                    SJImniDate = vehicle.SuzukiSJDate
                };

                ctx.omTrSalesReqDetail.Add(record);
            }
            else
            {
                record.ChassisCode = detailModel.ChassisCode;
                record.ChassisNo = detailModel.ChassisNo;
                record.DealerCategory = detailModel.DealerCategory;
                record.SKPKNo = detailModel.SKPKNo;
                record.SalesmanName = detailModel.SalesmanName;
                record.SalesmanCode = detailModel.SalesmanCode;
                record.SKPKName = detailModel.SKPKName;
                record.SKPKAddress1 = detailModel.SKPKAddress1;
                record.SKPKAddress2 = detailModel.SKPKAddress2;
                record.SKPKAddress3 = detailModel.SKPKAddress3;
                record.SKPKCity = detailModel.SKPKCity;
                record.SKPKTelp1 = detailModel.SKPKTelp1;
                record.SKPKTelp2 = detailModel.SKPKTelp2;
                record.SKPKHP = detailModel.SKPKHP;
                record.SKPKBirthday = detailModel.SKPKBirthday;
                record.FakturPolisiName = detailModel.FakturPolisiName;
                record.FakturPolisiAddress1 = detailModel.FakturPolisiAddress1;
                record.FakturPolisiAddress2 = detailModel.FakturPolisiAddress2;
                record.FakturPolisiAddress3 = detailModel.FakturPolisiAddress3;
                record.PostalCode = detailModel.PostalCode;
                record.PostalCodeDesc = detailModel.PostalCodeDesc;
                record.FakturPolisiCity = detailModel.FakturPolisiCity;
                record.FakturPolisiTelp1 = detailModel.FakturPolisiTelp1;
                record.FakturPolisiTelp2 = detailModel.FakturPolisiTelp2;
                record.FakturPolisiHP = detailModel.FakturPolisiHP;
                record.FakturPolisiBirthday = detailModel.FakturPolisiBirthday;
                record.IsCityTransport = detailModel.IsCityTransport;
                record.FakturPolisiNo = detailModel.FakturPolisiNo == null ? "" : detailModel.FakturPolisiNo;
                record.ReasonCode = detailModel.ReasonCode == null ? "" : detailModel.ReasonCode;
                record.ReasonNonFaktur = detailModel.ReasonNonFaktur;
                record.FakturPolisiDate = detailModel.FakturPolisiDate;
                record.FakturPolisiArea = detailModel.FakturPolisiArea;
                record.IDNo = detailModel.IDNo;
                record.IsProject = detailModel.IsProject;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
                //ctx.omTrSalesReqDetail.Attach(record);
            }

            try
            {
                Helpers.ReplaceNullable(record);
                vehicle.ReqOutNo = model.ReqNo;
                ctx.SaveChanges();

                var so = ctx.OmTRSalesSOs.Find(detailModel.CompanyCode, detailModel.BranchCode, detailModel.SONo);
                if (so != null)
                {
                    var customer = ctx.GnMstCustomer.Find(so.CompanyCode, so.CustomerCode);
                    if (customer != null)
                    {
                        var city = ctx.LookUpDtls.Find(CompanyCode, "CITY", detailModel);

                        customer.BirthDate = detailModel.SKPKBirthday;
                        customer.CityCode = detailModel.SKPKCity;
                        if (city != null) customer.KotaKabupaten = city.LookUpValueName;
                        ctx.SaveChanges();
                    }
                }
                return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult DeleteDetailSD(omTrSalesReq model, omTrSalesReqDetail detailModel)
        {
            var record = ctx.omTrSalesReqDetail.Find(CompanyCode, BranchCode, model.ReqNo, detailModel.ChassisCode, detailModel.ChassisNo);
            var vehicle = ctx.OmMstVehicles.Where(a => a.CompanyCode == CompanyCode && a.ChassisNo == detailModel.ChassisNo).FirstOrDefault(); ;

            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.omTrSalesReqDetail.Remove(record);

                vehicle.ReqOutNo = "";
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult getProductType()
        {
            var g = ctx.CoProfiles.Find(CompanyCode, BranchCode);
            string pType = g.ProductType;
            if (pType != "")
            {
                return Json(new { success = true, data = pType });
            }
            return Json(new { success = false, data = "" });
        }

        public JsonResult deleteAngket(string ReqNo, string ChassisCode, string ChassisNo)
        {
            var dt = ctx.omTrSalesReqDetailAdditionals.Find(CompanyCode, BranchCode, ReqNo, ChassisCode, ChassisNo);
            if (dt != null)
            {
                ctx.omTrSalesReqDetailAdditionals.Remove(dt);
                ctx.SaveChanges();
                return Json(new { success = true, message = "Delete angket berhasil!" });
            }
            return Json(new { success = false, data = "" });
        }

        public JsonResult getDataAngket(string ReqNo, string ChassisCode, string ChassisNo)
        {
            var dt = ctx.omTrSalesReqDetailAdditionals.Find(CompanyCode, BranchCode, ReqNo, ChassisCode, ChassisNo);
            if (dt != null)
            {
                var qry = String.Format(@"SELECT a.RefferenceDesc1
                                FROM dbo.omMstRefference a
                                WHERE a.CompanyCode = '{0}'
                                AND a.RefferenceCode = '{1}'
                                AND a.Status != '0'", CompanyCode, dt.ModelYgPernahDimiliki);
                string dt2 = ctx.Database.SqlQuery<string>(qry).FirstOrDefault();
                return Json(new { success = true, data = dt, data2 = dt2 == null ? "" : dt2 });
            }
            return Json(new { success = false, data = "" });
        }

        public JsonResult saveAngket(string ReqNo, omTrSalesReqDetailAdditional mdl)
        {
            var rd = ctx.omTrSalesReqDetailAdditionals.Find(CompanyCode, BranchCode, ReqNo, mdl.ChassisCode, mdl.ChassisNo);
            if (rd == null)
            {
                rd = new omTrSalesReqDetailAdditional
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ReqNo = ReqNo,
                    ChassisCode = mdl.ChassisCode,
                    ChassisNo = mdl.ChassisNo
                };
                ctx.omTrSalesReqDetailAdditionals.Add(rd);
            }
            rd.JenisKelamin = mdl.JenisKelamin;
            rd.TempatPembelian = mdl.TempatPembelian;
            rd.TempatPembelianOther = mdl.TempatPembelianOther == null ? "" : mdl.TempatPembelianOther;
            rd.KendaraanYgPernahDimiliki = mdl.KendaraanYgPernahDimiliki;
            rd.SumberPembelian = mdl.SumberPembelian;
            rd.SumberPembelianOther = mdl.SumberPembelianOther == null ? "" : mdl.SumberPembelianOther;
            rd.AsalPembelian = mdl.AsalPembelian;
            rd.AsalPembelianOther = mdl.AsalPembelianOther == null ? "" : mdl.AsalPembelianOther;
            rd.InfoSuzukiDari = mdl.InfoSuzukiDari;
            rd.InfoSuzukiDariOther = mdl.InfoSuzukiDariOther == null ? "" : mdl.InfoSuzukiDariOther;
            rd.FaktorPentingMemilihMotor = mdl.FaktorPentingMemilihMotor;
            rd.PendidikanTerakhir = mdl.PendidikanTerakhir;
            rd.PendidikanTerakhirOther = mdl.PendidikanTerakhirOther == null ? "" : mdl.PendidikanTerakhirOther;
            rd.PenghasilanPerBulan = mdl.PenghasilanPerBulan;
            rd.Pekerjaan = mdl.Pekerjaan;
            rd.PekerjaanOther = mdl.PekerjaanOther == null ? "" : mdl.PekerjaanOther;
            rd.PenggunaanMotor = mdl.PenggunaanMotor;
            rd.PenggunaanMotorOther = mdl.PenggunaanMotorOther == null ? "" : mdl.PenggunaanMotorOther;
            rd.CaraPembelian = mdl.CaraPembelian;
            rd.Leasing = mdl.Leasing == null ? "" : mdl.Leasing;
            rd.LeasingOther = mdl.LeasingOther == null ? "" : mdl.LeasingOther;
            rd.JangkaWaktuKredit = mdl.JangkaWaktuKredit == null ? "" : mdl.JangkaWaktuKredit;
            rd.JangkaWaktuKreditOther = mdl.JangkaWaktuKreditOther == null ? "" : mdl.JangkaWaktuKreditOther;
            rd.ModelYgPernahDimiliki = mdl.ModelYgPernahDimiliki;
            rd.LastUpdateBy = CurrentUser.UserId;
            rd.LastUpdateDate = DateTime.Now;

            if (ctx.SaveChanges() > 0)
            {
                return Json(new { success = true, message = "Simpan angket berhasil!" });
            }
            return Json(new { success = false, message = "Proses simpan angket gagal!" });
        }

        public JsonResult SaveDetail(omTrSalesReq model, omTrSalesReqDetail detailModel)
        {
            var md = false;
            var record = ctx.omTrSalesReqDetail.Find(CompanyCode, BranchCode, model.ReqNo, detailModel.ChassisCode, detailModel.ChassisNo);
            //var checkSD = ctx.OmMstVehicles.Find(CompanyCode, detailModel.ChassisCode, detailModel.ChassisNo);
            var vehicle = ctx.OmMstVehicles.Where(a => a.CompanyCode == CompanyCode && a.ChassisNo == detailModel.ChassisNo).FirstOrDefault(); ;

            if (vehicle == null)
            {
                vehicle = ctxMD.OmMstVehicles.Where(a => a.CompanyCode == CompanyMD && a.ChassisCode == detailModel.ChassisCode && a.ChassisNo == detailModel.ChassisNo).FirstOrDefault();
                md = true;
            }

            if (record == null)
            {
                record = new omTrSalesReqDetail
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    ReqNo = model.ReqNo,
                    ChassisCode = detailModel.ChassisCode,
                    ChassisNo = detailModel.ChassisNo,
                    SONo = detailModel.SONo,
                    BPKNo = detailModel.BPKNo,
                    DealerCategory = detailModel.DealerCategory,
                    SKPKNo = detailModel.SKPKNo,
                    SalesmanName = detailModel.SalesmanName,
                    SalesmanCode = detailModel.SalesmanCode,
                    SKPKName = detailModel.SKPKName,
                    SKPKAddress1 = detailModel.SKPKAddress1,
                    SKPKAddress2 = detailModel.SKPKAddress2,
                    SKPKAddress3 = detailModel.SKPKAddress3,
                    SKPKCity = detailModel.SKPKCity,
                    SKPKTelp1 = detailModel.SKPKTelp1,
                    SKPKTelp2 = detailModel.SKPKTelp2,
                    SKPKHP = detailModel.SKPKHP,
                    SKPKBirthday = detailModel.SKPKBirthday,
                    FakturPolisiName = detailModel.FakturPolisiName,
                    FakturPolisiAddress1 = detailModel.FakturPolisiAddress1,
                    FakturPolisiAddress2 = detailModel.FakturPolisiAddress2,
                    FakturPolisiAddress3 = detailModel.FakturPolisiAddress3,
                    PostalCode = detailModel.PostalCode,
                    PostalCodeDesc = detailModel.PostalCodeDesc,
                    FakturPolisiCity = detailModel.FakturPolisiCity,
                    FakturPolisiTelp1 = detailModel.FakturPolisiTelp1,
                    FakturPolisiTelp2 = detailModel.FakturPolisiTelp2,
                    FakturPolisiHP = detailModel.FakturPolisiHP,
                    FakturPolisiBirthday = detailModel.FakturPolisiBirthday,
                    IsCityTransport = detailModel.IsCityTransport,
                    FakturPolisiNo = detailModel.FakturPolisiNo == null ? "" : detailModel.FakturPolisiNo,
                    ReasonCode = detailModel.ReasonCode == null ? "" : detailModel.ReasonCode,
                    ReasonNonFaktur = detailModel.ReasonNonFaktur,
                    FakturPolisiDate = detailModel.FakturPolisiDate,
                    FakturPolisiArea = detailModel.FakturPolisiArea,
                    IDNo = detailModel.IDNo,
                    IsProject = detailModel.IsProject,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = ctx.CurrentTime,
                    DOImniNo = vehicle.SuzukiDONo,
                    DOImniDate = vehicle.SuzukiDODate,
                    SJImniNo = vehicle.SuzukiSJNo,
                    SJImniDate = vehicle.SuzukiSJDate
                };

                ctx.omTrSalesReqDetail.Add(record);
            }
            else
            {
                record.ChassisCode = detailModel.ChassisCode;
                record.ChassisNo = detailModel.ChassisNo;
                record.DealerCategory = detailModel.DealerCategory;
                record.SKPKNo = detailModel.SKPKNo;
                record.SalesmanName = detailModel.SalesmanName;
                record.SalesmanCode = detailModel.SalesmanCode;
                record.SKPKName = detailModel.SKPKName;
                record.SKPKAddress1 = detailModel.SKPKAddress1;
                record.SKPKAddress2 = detailModel.SKPKAddress2;
                record.SKPKAddress3 = detailModel.SKPKAddress3;
                record.SKPKCity = detailModel.SKPKCity;
                record.SKPKTelp1 = detailModel.SKPKTelp1;
                record.SKPKTelp2 = detailModel.SKPKTelp2;
                record.SKPKHP = detailModel.SKPKHP;
                record.SKPKBirthday = detailModel.SKPKBirthday;
                record.FakturPolisiName = detailModel.FakturPolisiName;
                record.FakturPolisiAddress1 = detailModel.FakturPolisiAddress1;
                record.FakturPolisiAddress2 = detailModel.FakturPolisiAddress2;
                record.FakturPolisiAddress3 = detailModel.FakturPolisiAddress3;
                record.PostalCode = detailModel.PostalCode;
                record.PostalCodeDesc = detailModel.PostalCodeDesc;
                record.FakturPolisiCity = detailModel.FakturPolisiCity;
                record.FakturPolisiTelp1 = detailModel.FakturPolisiTelp1;
                record.FakturPolisiTelp2 = detailModel.FakturPolisiTelp2;
                record.FakturPolisiHP = detailModel.FakturPolisiHP;
                record.FakturPolisiBirthday = detailModel.FakturPolisiBirthday;
                record.IsCityTransport = detailModel.IsCityTransport;
                record.FakturPolisiNo = detailModel.FakturPolisiNo == null ? "" : detailModel.FakturPolisiNo;
                record.ReasonCode = detailModel.ReasonCode == null ? "" : detailModel.ReasonCode;
                record.ReasonNonFaktur = detailModel.ReasonNonFaktur;
                record.FakturPolisiDate = detailModel.FakturPolisiDate;
                record.FakturPolisiArea = detailModel.FakturPolisiArea;
                record.IDNo = detailModel.IDNo;
                record.IsProject = detailModel.IsProject;
                record.LastUpdateBy = CurrentUser.UserId;
                record.LastUpdateDate = DateTime.Now;
                //ctx.omTrSalesReqDetail.Attach(record);
            }

            try
            {
                Helpers.ReplaceNullable(record);
                vehicle.ReqOutNo = model.ReqNo;
                if (md)
                {
                    ctxMD.SaveChanges();
                }
                ctx.SaveChanges();

                var so = ctx.OmTRSalesSOs.Find(detailModel.CompanyCode, detailModel.BranchCode, detailModel.SONo);
                if (so != null)
                {
                    var customer = ctx.GnMstCustomer.Find(so.CompanyCode, so.CustomerCode);
                    if (customer != null)
                    {
                        var city = ctx.LookUpDtls.Find(CompanyCode, "CITY", detailModel);

                        customer.BirthDate = detailModel.SKPKBirthday;
                        customer.CityCode = detailModel.SKPKCity;
                        if (city != null) customer.KotaKabupaten = city.LookUpValueName;
                        ctx.SaveChanges();
                    }
                }

                return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult DeleteDetail(omTrSalesReq model, omTrSalesReqDetail detailModel)
        {
            var md = false;
            var record = ctx.omTrSalesReqDetail.Find(CompanyCode, BranchCode, model.ReqNo, detailModel.ChassisCode, detailModel.ChassisNo);
            var vehicle = ctx.OmMstVehicles.Where(a => a.CompanyCode == CompanyCode && a.ChassisNo == detailModel.ChassisNo).FirstOrDefault(); ;

            if (vehicle == null)
            {
                vehicle = ctxMD.OmMstVehicles.Where(a => a.CompanyCode == CompanyMD && a.ChassisCode == detailModel.ChassisCode && a.ChassisNo == detailModel.ChassisNo).FirstOrDefault();
                md = true;
            }


            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.omTrSalesReqDetail.Remove(record);

                vehicle.ReqOutNo = "";
            }

            try
            {
                ctx.SaveChanges();

                if (md)
                {
                    ctxMD.SaveChanges();
                }

                return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult SaveHistory(omTrSalesFPolRevision model, string reqno)
        {
            var md = false;
            var record = ctx.omTrSalesReqDetail.Find(CompanyCode, BranchCode, reqno, model.ChassisCode, model.ChassisNo);
            var recordHst = ctx.omTrSalesFPolRevisionHistory.Where(x => x.ChassisCode == model.ChassisCode && x.ChassisNo == model.ChassisNo).OrderByDescending(y => y.RevisionSeq).FirstOrDefault();
            String qtbl = @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name LIKE 'omTrSalesFPolRevisionHistory')
                            CREATE TABLE [dbo].[omTrSalesFPolRevisionHistory]( --update sequence 
	                        [ChassisCode] [varchar](15) NOT NULL,
	                        [ChassisNo] [numeric](10, 0) NOT NULL,
	                        [RevisionSeq] [int] NOT NULL,
	                        [RevisionNo] [varchar](15) NULL,
	                        [RevisionCode] [varchar](15) NOT NULL,
	                        [FakturPolisiName] [varchar](100) NULL,
	                        [FakturPolisiAddress1] [varchar](100) NULL,
	                        [FakturPolisiAddress2] [varchar](100) NULL,
	                        [FakturPolisiAddress3] [varchar](100) NULL,
	                        [PostalCode] [varchar](15) NULL,
	                        [PostalCodeDesc] [varchar](100) NULL,
	                        [FakturPolisiCity] [varchar](15) NULL,
	                        [FakturPolisiTelp1] [varchar](15) NULL,
	                        [FakturPolisiTelp2] [varchar](15) NULL,
	                        [FakturPolisiHP] [varchar](15) NULL,
	                        [FakturPolisiBirthday] [datetime] NULL,
	                        [FakturPolisiNo] [varchar](15) NULL,
	                        [FakturPolisiDate] [datetime] NULL,
	                        [FakturPolisiArea] [varchar](15) NULL,
	                        [IDNo] [varchar](100) NULL,
	                        [IsCityTransport] [bit] NOT NULL,
	                        [IsProject] [bit] NULL,
	                        [SubDealerCode] [varchar](15) NULL,
	                        [CreatedBy] [varchar](15) NOT NULL,
	                        [CreatedDate] [datetime] NOT NULL,
	                        [LastUpdateBy] [varchar](15) NULL,
	                        [LastUpdateDate] [datetime]  NOT NULL,
                         CONSTRAINT [PK__omTrSalesFPolRevisionHistory__5D509081] PRIMARY KEY CLUSTERED 
                        (
	                        [ChassisNo] ASC,
	                        [ChassisCode] ASC,
	                        [RevisionSeq] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
                        ) ON [PRIMARY]
";

            ctx.Database.ExecuteSqlCommand(qtbl);

            var vehicle = ctx.OmMstVehicles.Where(a => a.CompanyCode == CompanyCode && a.ChassisNo == model.ChassisNo).FirstOrDefault(); ;

            if (vehicle == null)
            {
                vehicle = ctxMD.OmMstVehicles.Where(a => a.CompanyCode == CompanyMD && a.ChassisCode == model.ChassisCode && a.ChassisNo == model.ChassisNo).FirstOrDefault();
                md = false;
            }
            var seqNo = 1;
            if (record != null && recordHst == null)
            {
                //save log
                recordHst = new omTrSalesFPolRevisionHistory
                {
                    RevisionSeq = seqNo,
                    RevisionNo = "",
                    ChassisCode = record.ChassisCode,
                    ChassisNo = record.ChassisNo,
                    FakturPolisiName = record.FakturPolisiName,
                    FakturPolisiAddress1 = record.FakturPolisiAddress1,
                    FakturPolisiAddress2 = record.FakturPolisiAddress2,
                    FakturPolisiAddress3 = record.FakturPolisiAddress3,
                    PostalCode = record.PostalCode,
                    PostalCodeDesc = record.PostalCodeDesc,
                    FakturPolisiCity = record.FakturPolisiCity,
                    FakturPolisiTelp1 = record.FakturPolisiTelp1,
                    FakturPolisiTelp2 = record.FakturPolisiTelp2,
                    FakturPolisiHP = record.FakturPolisiHP,
                    FakturPolisiBirthday = record.FakturPolisiBirthday,
                    IsCityTransport = record.IsCityTransport,
                    FakturPolisiNo = record.FakturPolisiNo == null ? "" : record.FakturPolisiNo,
                    RevisionCode = model.RevisionCode,
                    FakturPolisiDate = record.FakturPolisiDate,
                    FakturPolisiArea = record.FakturPolisiArea,
                    IDNo = record.IDNo,
                    IsProject = record.IsProject,
                    CreatedBy = record.CreatedBy,
                    CreatedDate = record.CreatedDate,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = DateTime.Now,
                };
                ctx.omTrSalesFPolRevisionHistory.Add(recordHst);
            }
            else
            {
                recordHst = new omTrSalesFPolRevisionHistory
                {
                    RevisionSeq = recordHst.RevisionSeq + 1,
                    RevisionNo = model.RevisionNo,
                    ChassisCode = model.ChassisCode,
                    ChassisNo = model.ChassisNo,
                    FakturPolisiName = model.FakturPolisiName,
                    FakturPolisiAddress1 = model.FakturPolisiAddress1,
                    FakturPolisiAddress2 = model.FakturPolisiAddress2,
                    FakturPolisiAddress3 = model.FakturPolisiAddress3,
                    PostalCode = model.PostalCode,
                    PostalCodeDesc = model.PostalCodeDesc,
                    FakturPolisiCity = model.FakturPolisiCity,
                    FakturPolisiTelp1 = model.FakturPolisiTelp1,
                    FakturPolisiTelp2 = model.FakturPolisiTelp2,
                    FakturPolisiHP = model.FakturPolisiHP,
                    FakturPolisiBirthday = model.FakturPolisiBirthday,
                    IsCityTransport = model.IsCityTransport,
                    FakturPolisiNo = model.FakturPolisiNo == null ? "" : model.FakturPolisiNo,
                    RevisionCode = model.RevisionCode,
                    FakturPolisiDate = model.FakturPolisiDate,
                    FakturPolisiArea = model.FakturPolisiArea,
                    IDNo = model.IDNo,
                    IsProject = model.IsProject,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = DateTime.Now
                };
                ctx.omTrSalesFPolRevisionHistory.Add(recordHst);
            }

            try
            {
                Helpers.ReplaceNullable(record);
                //vehicle.ReqOutNo = record.ReqNo;
                if (md)
                {
                    ctxMD.SaveChanges();
                }
                ctx.SaveChanges();
                return Json(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult loadFPDetail(string vChassisCode, string vChassisNo)
        {
            if (vChassisCode != null && vChassisNo != null)
            {
                var qry = "exec uspfn_OmFPDetailCustomer '" + vChassisCode + "','" + vChassisNo + "'";
                var data = ctx.Database.SqlQuery<FPDetailCustomer>(qry).FirstOrDefault();
                if (data != null)
                {
                    return Json(new { success = true, data, message = "" });
                }
            }
            else
            {
                return Json(new { success = false, message = "ChassisCode dan ChassisNo null!" });
            }
            return Json(new { success = false, message = "Detail FP Customer tidak ditemukan!" });
        }

        public JsonResult SalesmanCode(SalesReqDetailView model)
        {
            var record = ctx.Employees.Find(CompanyCode, BranchCode, model.SalesmanCode);
            return Json(new { success = record != null, data = record });
        }

        public JsonResult DetailSalesFPolRevision(string RevisiNo)
        {
            var gridDetail = ctx.omTrSalesFPolRevisionDetail.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.RevisiNo == RevisiNo).ToList()
                .Select(m => new SalesFPolRevisionDetailView
                {
                    RevisionNo = m.RevisiNo,
                    ChassisCode = m.ChassisCode,
                    ChassisNo = m.ChassisNo,
                    FakturPolisiName = m.FakturPolisiName,
                    FakturPolisiAddress1 = m.FakturPolisiAddress1,
                    FakturPolisiAddress2 = m.FakturPolisiAddress2,
                    FakturPolisiAddress3 = m.FakturPolisiAddress3,
                    PostalCode = m.PostalCode,
                    PostalCodeDesc = m.PostalCodeDesc,
                    FakturPolisiCity = m.FakturPolisiCity,
                    FakturPolisiCityName = ctx.LookUpDtls.FirstOrDefault(e => e.CompanyCode == CompanyCode && e.ParaValue == m.FakturPolisiCity).LookUpValueName,
                    FakturPolisiTelp1 = m.FakturPolisiTelp1,
                    FakturPolisiTelp2 = m.FakturPolisiTelp2,
                    FakturPolisiHP = m.FakturPolisiHP,
                    FakturPolisiBirthday = m.FakturPolisiBirthday,
                    IsCityTransport = m.IsCityTransport,
                    FakturPolisiNo = m.FakturPolisiNo,
                    RevisionCode = m.RevisionCode,
                    FakturPolisiDate = m.FakturPolisiDate,
                    FakturPolisiArea = m.FakturPolisiArea,
                    IDNo = m.IDNo,
                    IsProject = m.IsProject,
                });
            return Json(new { success = true, grid = gridDetail });
        }
        #endregion
    }
}