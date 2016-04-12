using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Sales.Models;
using SimDms.Sales.Models.Result;
using System.Data.Entity.Validation;
using System.Diagnostics;
using SimDms.Common.Models;
using System.Transactions;
using SimDms.Common;
using SimDms.Sales.BLL;
using TracerX;

namespace SimDms.Sales.Controllers.Api
{
    public class SalesOrderController : BaseController
    {
        private string salesCoor = "";
        private string salesHead = "";
        private string branchManager = "";
        private string statusVehicle = "A";
        //owner ship add
        [HttpPost]
        public JsonResult GridOwnerShip(string SONo)
        {
            var hdr = from p in ctx.OmTrSalesSOModels
                      where p.SONo == SONo
                      select new OWSalesModel { 
                        SalesModelCode = p.SalesModelCode,
                        SalesModelYear = p.SalesModelYear,
                        StatusVehicle = "",
                        ModelName =  "",
                        BrandCode = ""
                      };
            return Json(new { success = true, data = hdr });
        }

        [HttpPost]
        public JsonResult Default()
        {
            var transactionDate = DateTime.Now.Date;
            string WareHouseCode = "";
            string WareHouseName = "";
            if (!cekOtomatis())
            {
                WareHouseCode = ctx.LookUpDtls.Where(p => p.CodeID == "MPWH" && p.CompanyCode == CompanyCode && p.ParaValue == BranchCode).OrderBy(p => p.SeqNo).FirstOrDefault().LookUpValue;
                WareHouseName = ctx.LookUpDtls.Where(p => p.CodeID == "MPWH" && p.CompanyCode == CompanyCode && p.ParaValue == BranchCode).OrderBy(p => p.SeqNo).FirstOrDefault().LookUpValueName;
            }
            else
            {
                WareHouseCode = ctxMD.LookUpDtls.Where(p => p.CodeID == "MPWH" && p.CompanyCode == CompanyMD && p.ParaValue == BranchMD).OrderBy(p => p.SeqNo).FirstOrDefault().LookUpValue;
                WareHouseName = ctxMD.LookUpDtls.Where(p => p.CodeID == "MPWH" && p.CompanyCode == CompanyMD && p.ParaValue == BranchMD).OrderBy(p => p.SeqNo).FirstOrDefault().LookUpValueName;
            }

            return Json(new
            {
                CompanyCode = CompanyCode,
                BranchCode = BranchCode,
                SODate = transactionDate,
                ReffDate = transactionDate,
                ProfitCenterCode = ProfitCenter,
                isDSOL = ctx.LookUpDtls.Where(p => p.CodeID == "DSOL").FirstOrDefault().ParaValue,
                isITSFL = ctx.LookUpDtls.Where(p => p.CodeID == "ITSFL").FirstOrDefault().ParaValue,
                WareHouseCode = WareHouseCode,
                WareHouseName = WareHouseName,
                ProductType = ProductType
            });
        }

        [HttpPost]
        public JsonResult Blur(string IsApa, string Key, string key2)
        {
            var json =Json(new{});
            if (IsApa == "SalesModelCode")
            {
                 json = Json(new
                {
                    SalesModelCode = ctx.OmMstModels.Where(p => p.SalesModelCode == Key).FirstOrDefault().SalesModelCode
                });
            }

            if (IsApa == "SalesModelYear")
            {
                json = Json(new
                {
                    SalesModelYear = ctx.MstModelYear.Where(p => p.SalesModelCode == Key && p.SalesModelYear == Convert.ToDecimal(key2)).FirstOrDefault().SalesModelYear,
                    SalesModelDesc = ctx.MstModelYear.Where(p => p.SalesModelCode == Key && p.SalesModelYear == Convert.ToDecimal(key2)).FirstOrDefault().SalesModelDesc 
                });
            }

            if (IsApa == "ColourCode")
            {
                json = Json(new
                {
                    ColourCode = ctx.MstRefferences.Where(p => p.RefferenceType == Key && p.RefferenceCode == key2).FirstOrDefault().RefferenceCode,
                    ColourName = ctx.MstRefferences.Where(p => p.RefferenceType == Key && p.RefferenceCode == key2).FirstOrDefault().RefferenceDesc1
                });
            }

            //if (IsApa == "Pemasok")
            //{
            //    json = Json(new
            //    {
            //        CustomerCode = ctx.GnMstCustomer.Where(p => p.CustomerCode == Key).FirstOrDefault().CustomerCode,
            //        CustomerName = ctx.GnMstCustomer.Where(p => p.CustomerCode == Key).FirstOrDefault().CustomerName 
            //    });
            //}

            return json;
        }


        [HttpPost]
        public JsonResult Status(string SONo) 
        {
            var transactionDate = DateTime.Now.Date;

            return Json(new
            {
                Status = ctx.OmTRSalesSOs.Where(p => p.SONo == SONo).FirstOrDefault().Status
            });
        }

        public JsonResult SalesModelLoad(string SONo)
        {
            var query = string.Format(@"
                SELECT * , b.SalesModelDesc
                FROM dbo.omTrSalesSOModel a
                left join ommstmodel b
                on a.CompanyCode =b.CompanyCode and a.SalesModelCode = b.SalesModelCode
                WHERE a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.SONo = '{2}'
                       ", CompanyCode, BranchCode, SONo);
            return Json(ctx.Database.SqlQuery<OmSalesModelView>(query).AsQueryable());
        }

        public JsonResult SalesModelColorLoad(OmTrSalesSOModelColour model)  
        {
            var query = string.Format(@"
               SELECT a.*, b.RefferenceDesc1 as ColourDescription
                FROM dbo.OmTrSalesSOModelColour a
                inner join omMstRefference b
                    on b.RefferenceCode = a.ColourCode
                WHERE a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.SONo = '{2}'
                    AND a.SalesModelCode = '{3}'
                    AND a.SalesModelYear = '{4}'
                       ", CompanyCode, BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear);
            return Json(ctx.Database.SqlQuery<SalesSOModelColourView>(query).AsQueryable());
        }

        public JsonResult SalesSOVinLoad(omTrSalesSOVin model)   
        {
            var query = string.Format(@"
                 SELECT a.* ,b.SupplierName,c.LookUpValueName CityDesc
                FROM dbo.omTrSalesSOVin a
                left join GnMstSupplier b on a.CompanyCode=b.CompanyCode  
                and a.SupplierBBN=b.SupplierCode
                left join gnMstLookUpDtl c on a.CompanyCode=c.CompanyCode and a.CityCode=c.LookUpValue
                and c.CodeID='CITY'
                WHERE 1=1
					AND a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'                
                    AND a.SONo = '{2}'
                    AND a.salesmodelcode = '{3}'
                    AND a.salesmodelyear = '{4}'
                    AND a.colourcode = '{5}'
                       ", CompanyCode, BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear, model.ColourCode);
            return Json(ctx.Database.SqlQuery<omTrSalesSOVinDtl>(query).AsQueryable());
        }

        public JsonResult SalesSOAccsSeqLoad(OmTrSalesSOAccsSeq model) 
        {
            var query = string.Format(@"
                 SELECT a.* , b.PartName, c.LookUpValueName ProductType, (a.DemandQty * a.RetailPrice) as Total
                FROM dbo.OmTrSalesSOAccsSeq a
                inner join spMstItemInfo b
					on a.PartNo = b.PartNo
				inner join gnMstLookUpDtl c
					on c.codeid='TPGO' and a.TypeOfGoods = c.LookUpValue
                WHERE a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.SONo = '{2}'
                       ", CompanyCode, BranchCode, model.SONo);
            return Json(ctx.Database.SqlQuery<SalesSOAccsSeq>(query).AsQueryable());
        }

        public JsonResult SalesModelOthersLoad(OmTrSalesSoModelOther model)
        {
            var query = string.Format(@"
                 SELECT a.* , b.RefferenceDesc1 as OtherDesc
                FROM dbo.OmTrSalesSoModelOthers a
				inner join omMstRefference b
					on a.OtherCode = b.RefferenceCode
                WHERE a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.SONo = '{2}'
                    AND a.salesmodelcode = '{3}'
                    AND a.salesmodelyear = '{4}'
                       ", CompanyCode, BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear);
            return Json(ctx.Database.SqlQuery<SalesSoModelOther>(query).AsQueryable());
        }

        public JsonResult DetilWarnaLoad(string SONo) 
        {
            var query = string.Format(@"
                SELECT * 
                FROM dbo.omTrSalesSOModel a
                WHERE a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.SONo = '{2}'
                       ", CompanyCode, BranchCode, SONo);
            return Json(ctx.Database.SqlQuery<OmSalesModelView>(query).AsQueryable());
        }

        public JsonResult GetCustomer(string CustomerCode) 
        {
            var data = ctx.GnMstCustomer.Where(x => x.CustomerCode == CustomerCode).FirstOrDefault();
            if (data != null)
            {
                return Json(data);
            }
            return Json(null);
        }
        
        private string updateStsSO(string SONo){
            var companyCode = CompanyCode;
            var branchCode = BranchCode;
            var me = ctx.OmTRSalesSOs.Find(companyCode, branchCode, SONo);
            var sts = me.Status;
            if (me.Status == "1")
            {
                me.Status = "0";
                ctx.SaveChanges();
                sts = "0";
            }
            return sts;
        }

        [HttpPost]
        public JsonResult Save(OmTRSalesSO model)
        {
            ResultModel result = InitializeResult();
            string userID = CurrentUser.UserId;
            DateTime currentDate = DateTime.Now;
            string itsNumber = "";
            bool isNew = true;
            string CompanySD = getCompanySD(model.CustomerCode);
            string DbSD = getDbSD(CompanySD, model.CustomerCode);

            if (model.SODate == null)
            {
                result.message = "Sales Order Date belum diisi.";
                return Json(result);
            }

            if ((model.RefferenceNo == null && model.isC1 == true) || (model.RefferenceNo != null && model.isC1 == false))
            {
                result.message = "No Reff dan Tgl Reff harus diisi";
                return Json(result);
            }
            OmTRSalesSO me = ctx.OmTRSalesSOs.Find(CompanyCode, BranchCode, model.SONo);
            ResultModel soDateValidation = ValidateSODate(model.SODate.Value.Date);
            if (ProfitCenter != "000"){
                if (!soDateValidation.status && me == null)
                {
                    result.status = soDateValidation.status;
                    result.message = soDateValidation.message;
                    return Json(result);
                }
            }
            
            if (!string.IsNullOrWhiteSpace(model.RefferenceNo))
            {

                if (model.RefferenceDate == null || model.RefferenceDate.Value.Year == 1900 || model.RefferenceDate.Value.Year == 1)
                {
                    result.message = "Jika No. Reff diisi, maka tanggal Reff juga harus diisi.";
                    return Json(result);
                }
            }

            if (model.isLeasing == true)
            {
                if (model.LeasingCo == null)
                {
                     result.message = "Jika Leasing dicek, maka leasing co harus diisi";
                     return Json(result);
                }
                if (model.Installment == null)
                {
                    result.message = "Angsuran harus dipilih!";
                    return Json(result);
                }
            }

            var TOP = ctx.LookUpDtls.Where(x => x.CodeID == "TOPC").FirstOrDefault();
            if (TOP == null)
            {
                result.status = false;
                result.message = "Data TOP tidak terdapat di dalam database.";

                return Json(result);
            }

            if (model.isC2 != null)
            {
                if (model.isC2.Value)
                {
                    if (string.IsNullOrWhiteSpace(model.SKPKNo))
                    {
                        result.message = "No. ITS / No. Prospek belum diisi";
                        return Json(result);
                    }
                }
            }

            ResultModel itsValidation = CheckItsOrganization(model.Salesman, model.SalesmanName);

            
            
            //ITSValidationResult itsValidation = ctx.Database.SqlQuery<ITSValidationResult>("exec uspfn_CheckITSOrganization @CompanyCode=@p0, @BranchCode=@p1, @EmployeeID=@p2", CompanyCode, BranchCode, model.Salesman).FirstOrDefault();
            //if (!itsValidation.Status.Value)
            //{
            //    result.status = itsValidation.Status.Value;
            //    result.message = itsValidation.Message;

            //    return Json(result);
            //}

            if (string.IsNullOrWhiteSpace(model.SKPKNo))
            {
                result.message = "No. SKPK harus diisi.";
            }

            if (me == null) 
            {
                me = new OmTRSalesSO();
                me.CompanyCode = CompanyCode;
                me.BranchCode = BranchCode;
                me.SONo = GetNewSONumber(model.SODate);
                me.Status = "0";
                me.CreatedBy = CurrentUser.UserId;
                me.CreatedDate = DateTime.Now;

                ctx.OmTRSalesSOs.Add(me);
                isNew = true;
            }
            else
            {
                isNew = false;
                itsNumber = me.ProspectNo;
            }

            //if (isNew == false)
            //{
            //    if (me.ProspectNo != model.ProspectNo && model.ProspectNo !=null )
            //    {
            //        result.message = "Nomor ITS tidak boleh diubah";
            //        return Json(result);
            //    }
            //}

            me.SalesCoordinator = salesCoor ?? "";
            me.SalesHead = salesHead ?? "";
            me.BranchManager = branchManager ?? "";
            me.SODate = model.SODate;
            me.SalesType = model.isC2 == true ? "1" : "0";
            me.RefferenceNo = model.RefferenceNo;
            me.RefferenceDate = model.RefferenceDate;
            me.CustomerCode = model.CustomerCode;
            me.TOPCode = model.TOPCode;
            me.TOPDays = model.TOPDays;
            me.BillTo = model.BillTo;
            me.ShipTo = model.ShipTo;
            me.ProspectNo = model.ProspectNo;
            me.SKPKNo = model.SKPKNo;
            me.Salesman = model.Salesman;
            me.WareHouseCode = model.WareHouseCode;
            me.isLeasing = model.isLeasing;
            me.LeasingCo = model.LeasingCo;
            me.GroupPriceCode = model.GroupPriceCode;
            me.Insurance = model.Insurance;
            me.PaymentType = model.PaymentType;
            me.PrePaymentAmt = model.PrePaymentAmt;
            me.PrePaymentDate = model.PrePaymentDate;
            me.PrePaymentBy = model.PrePaymentBy;
            me.CommissionBy = model.CommissionBy;
            me.CommissionAmt = model.CommissionAmt;
            me.PONo = model.PONo;
            me.ContractNo = model.ContractNo;
            me.RequestDate = model.RequestDate;
            me.Remark = model.Remark;
            me.RejectBy = null;
            me.RejectDate = null;
            me.isLocked = null;
            me.LockingBy = null;
            me.LockingDate = null;
            me.SalesCode = model.SalesCode;
            me.Installment = model.Installment;
            me.FinalPaymentDate = model.FinalPaymentDate;
            me.LastUpdateBy = CurrentUser.UserId;
            me.LastUpdateDate = DateTime.Now;

            //if (me.Status == "0")
            //{
            //    model.Status = "OPEN";
            //}
            //else if (me.Status == "1")
            //{
            //    model.Status = "PRINTED";
            //}
            //else if (me.Status == "2")
            //{
            //    model.Status = "APPROVED";
            //}
            //else if (me.Status == "3")
            //{
            //    model.Status = "REJECTED";
            //}

            try
            {
                Helpers.ReplaceNullable(me);
                ctx.SaveChanges();
                ctx.Database.ExecuteSqlCommand("exec uspfn_GnChangeCustStatus @CompanyCode=@p0, @CustomerCode=@p1, @FuncCode=@p2, @UserID=@p3", CompanyCode, model.CustomerCode, "SO", userID);

                result.status = true;
                result.data = new
                {
                    SONumber = me.SONo,
                    SOStatus = ctx.OmTRSalesSOs.Find(CompanyCode, BranchCode, me.SONo).Status
                };

//                if (model.RefferenceNo != null || model.RefferenceNo != "")
//                {
//                    if (model.RefferenceNo.Substring(0, 2) == "PO")
//                    {
//                        var qr = String.Format(@"SELECT Count(*) FROM omTrSalesSO a  INNER JOIN omTrSalesSOModel b ON
//                                                b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.SONo = a.SONo AND a.RefferenceNo='{2}'
//                                                WHERE a.CompanyCode='{0}' AND a.BranchCode='{1}'", CompanyCode, BranchCode, model.RefferenceNo );
//                        Int32 qtyMdl = ctx.Database.SqlQuery<Int32>(qr).FirstOrDefault();
//                        Int32 qtyPO = ctx.Database.SqlQuery<Int32>(String.Format(@"SELECT Count(*) FROM {2}..omTrPurchasePOModel WHERE CompanyCode='{0}' AND BranchCode='{1}' AND PONo='{3}'",
//                                                                                 CompanySD, model.CustomerCode, DbSD, model.RefferenceNo)).FirstOrDefault();
//                        if (qtyPO == qtyMdl)
//                        {
//                            ctx.Database.ExecuteSqlCommand("UPDATE " + DbSD + "..omTrPurchasePO SET IsLocked=1 WHERE CompanyCode='" + CompanySD + "' AND BranchCode='" + model.CustomerCode + "' AND PONo='" + model.RefferenceNo + "'");
//                        }
     
//                    }
//                } 
             
                result.message = "Data SO berhasil disimpan.";
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                result.message = "Data SO gagal disimpan.";
            }
            catch (Exception)
            {
                result.message = "Data SO gagal disimpan.";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(OmTRSalesSO model) 
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.OmTRSalesSOs.Find(CompanyCode, BranchCode, model.SONo);
                    if (me != null)
                    {
                        me.Status = "3";
                        me.LastUpdateBy = CurrentUser.UserId;
                        me.LastUpdateDate = ctx.CurrentTime;

                        ctx.SaveChanges();

                        #region sovin
                        var lst = ctx.omTrSalesSOVins
                                    .Where(x => x.CompanyCode == companyCode &&
                                        x.BranchCode == BranchCode &&
                                        x.SONo == model.SONo)
                                        .ToList();
                        if(lst.Count>0)
                        {
                            lst.ForEach(x=>{
                                ctx.omTrSalesSOVins.Remove(x);                            
                            });
                            ctx.SaveChanges();
                        }

                        var lstmdlclr = ctx.OmTrSalesSOModelColours
                                .Where(x => x.CompanyCode == companyCode && x.BranchCode == BranchCode & x.SONo == model.SONo)
                                .ToList();
                        if (lstmdlclr.Count > 0)
                        {
                            lstmdlclr.ForEach(x => ctx.OmTrSalesSOModelColours.Remove(x));
                            ctx.SaveChanges();
                        }



                        var lstmdlothr = ctx.OmTrSalesSoModelOthers
                          .Where(x => x.CompanyCode == companyCode && x.BranchCode == BranchCode & x.SONo == model.SONo)
                          .ToList();
                        if (lstmdlothr.Count > 0)
                        {
                            lstmdlothr.ForEach(x => ctx.OmTrSalesSoModelOthers.Remove(x));
                            ctx.SaveChanges();
                        }



                        var lstmdl = ctx.OmTrSalesSOModels
                                    .Where(x => x.CompanyCode == companyCode && x.BranchCode == BranchCode & x.SONo == model.SONo)
                                    .ToList();
                        if (lstmdl.Count>0)
                        {
                            lstmdl.ForEach(x => ctx.OmTrSalesSOModels.Remove(x));
                            ctx.SaveChanges();
                        }

                        var lstmdlacc = ctx.OmTrSalesSOAccses
                          .Where(x => x.CompanyCode == companyCode && x.BranchCode == BranchCode & x.SONo == model.SONo)
                          .ToList();
                        if (lstmdlacc.Count > 0)
                        {
                            lstmdlacc.ForEach(x => ctx.OmTrSalesSOAccses.Remove(x));
                            ctx.SaveChanges();
                        }
                        

                        #endregion  

                        //ctx.OmTRSalesSOs.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data SO berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete SO, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete SO, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
            
        }

        [HttpPost]
        public JsonResult Save2(OmTrSalesSOModel model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;



            var hdr=ctx.OmTRSalesSOs.Find(CompanyCode,BranchCode,model.SONo);
            if(hdr==null)
                return Json(new { success = false, message = "SO Tidak Ada" });

            var pjl = ctx.OmMstPricelistSells.Find(CompanyCode, BranchCode, model.GroupPriceCode, model.SalesModelCode, model.SalesModelYear);

            //if (model.AfterDiscTotal == 0 || model.AfterDiscTotal == null  || model.AfterDiscTotal<pjl.TotalMinStaff)
            if (model.AfterDiscTotal == 0 || model.AfterDiscTotal == null || model.AfterDiscDPP == 0 || model.AfterDiscDPP == null)
            {
                var message = "Harga Total/ DPP tidak boleh kurang atau sama dengan nol";
                return Json(new { success = false, message = message });
            }

            var sell = ctx.OmMstPricelistSells.Find(CompanyCode, BranchCode, model.GroupPriceCode, model.SalesModelCode, model.SalesModelYear);
            if (sell == null)
            {
                var message = "Pricelist jual belum ada";
                return Json(new { success = false, message = message });
            }

            var me = ctx.OmTrSalesSOModels.Find(CompanyCode, BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear);

            if (me == null)
            {
                me = new OmTrSalesSOModel();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                me.QuantitySO = 0;
                me.QuantityDO = 0;
                ctx.OmTrSalesSOModels.Add(me);
            }
            
            me.LastUpdateDate = currentTime;
            me.LastUpdateBy = userID;
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.SONo = model.SONo;
            me.SalesModelCode = model.SalesModelCode;
            me.SalesModelYear = model.SalesModelYear;
            me.ChassisCode = model.ChassisCode;

            me.BeforeDiscTotal = model.BeforeDiscTotal != null ? model.BeforeDiscTotal : 0;
            //me.BeforeDiscDPP = model.BeforeDiscDPP != null ? model.BeforeDiscDPP : 0;
            //me.BeforeDiscPPn = model.BeforeDiscPPn != null ? model.BeforeDiscPPn : 0;
            me.BeforeDiscDPP = sell.DPP;
            me.BeforeDiscPPn = sell.PPn;
            me.BeforeDiscPPnBM = sell.PPnBM;  //model.AfterDiscPPnBM != null ? model.BeforeDiscPPnBM : 0;
            
            //me.DiscExcludePPn = model.DiscExcludePPn != null ? model.DiscExcludePPn : 0;
            
            me.DiscIncludePPn = model.DiscIncludePPn != null ? model.DiscIncludePPn : 0;
            me.AfterDiscDPP = model.AfterDiscDPP != null ? model.AfterDiscDPP : 0;
            me.AfterDiscPPn = model.AfterDiscPPn != null ? model.AfterDiscPPn : 0;
            me.AfterDiscPPnBM = model.AfterDiscPPnBM != null ? model.AfterDiscPPnBM : 0;
            me.DiscExcludePPn = me.BeforeDiscDPP - me.AfterDiscDPP;

            me.AfterDiscTotal = model.AfterDiscTotal != null ? model.AfterDiscTotal : 0;
            
            me.OthersDPP = model.OthersDPP != null? model.OthersDPP:0;
            me.OthersPPn = model.OthersPPn != null ? model.OthersPPn : 0;
            me.Remark = model.Remark;
            me.ShipAmt = model.ShipAmt != null ? model.ShipAmt : 0;
            me.DepositAmt = model.DepositAmt != null ? model.DepositAmt : 0;
            me.OthersAmt = model.OthersAmt != null ? model.OthersAmt : 0;

            try
            {
                Helpers.ReplaceNullable(me);
                ctx.SaveChanges();
                string status = updateStsSO(model.SONo);
                //Update omTrPurchasePOModel
                var SoHdr = ctx.OmTRSalesSOs.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.SONo == model.SONo).FirstOrDefault();
                if (SoHdr.RefferenceNo != null && SoHdr.RefferenceNo != "")
                {
                    if (SoHdr.RefferenceNo.Substring(0, 2) == "PO")
                    {
                        string CompanySD = getCompanySD(SoHdr.CustomerCode);
                        string DbSD = getDbSD(CompanySD, SoHdr.CustomerCode);
                        string rmk = model.SONo + ", " + SoHdr.SODate;

//                        var qr = String.Format(@"SELECT Count(*) FROM omTrSalesSO a  INNER JOIN omTrSalesSOModel b ON
//                                                b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.SONo = a.SONo AND a.RefferenceNo='{2}'
//                                                WHERE a.CompanyCode='{0}' AND a.BranchCode='{1}'", CompanyCode, BranchCode, SoHdr.RefferenceNo);
//                        Int32 qtyMdl = ctx.Database.SqlQuery<Int32>(qr).FirstOrDefault();
//                        Int32 qtyPO = ctx.Database.SqlQuery<Int32>(String.Format(@"SELECT Count(*) FROM {2}..omTrPurchasePOModel WHERE CompanyCode='{0}' AND BranchCode='{1}' AND PONo='{3}'",
//                                                                                 CompanySD, SoHdr.CustomerCode, DbSD, SoHdr.RefferenceNo)).FirstOrDefault();
//                        if (qtyPO == qtyMdl)
//                        {
//                            ctx.Database.ExecuteSqlCommand("UPDATE " + DbSD + "..omTrPurchasePO SET IsLocked=1 WHERE CompanyCode='" + CompanySD + "' AND BranchCode='" + SoHdr.CustomerCode + "' AND PONo='" + SoHdr.RefferenceNo + "'");
//                        }

                        ctx.Database.ExecuteSqlCommand("UPDATE " + DbSD + "..omTrPurchasePOModel SET Remark='" + rmk + "' WHERE CompanyCode='" + CompanySD + "' AND BranchCode='" + SoHdr.CustomerCode + "' AND PONo='" + SoHdr.RefferenceNo + "' AND SalesModelCode='" + model.SalesModelCode + "' AND SalesModelYear='" + model.SalesModelYear + "'");
                    
                    }
                }
                
                return Json(new { success = true, data = me, message = "Save Detail Successfully", Status = status });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete2(OmTrSalesSOModel model) 
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.OmTrSalesSOModels.Find(CompanyCode, BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear);
                    var meDtl = ctx.OmTrSalesSOModelColours.Where(m => m.CompanyCode == companyCode && m.BranchCode == BranchCode && m.SONo == model.SONo && m.SalesModelCode == model.SalesModelCode && m.SalesModelYear == model.SalesModelYear).ToArray();
                    var DtlUnit = ctx.omTrSalesSOVins.Where(z => z.CompanyCode == CompanyCode && z.BranchCode == BranchCode && z.SONo == me.SONo && z.SalesModelCode == me.SalesModelCode
                                          && z.SalesModelYear == me.SalesModelYear ).FirstOrDefault();
                    if (DtlUnit != null)
                    {
                        returnObj = new { success = false, message = "Masih terdapat detail kendaraan pada colour model!, Silahkan hapus terlebih dahulu detail kendaraan tersebut" };
                        trans.Dispose();
                    }
                    else
                    {
                        if (me != null)
                        {
                            var x = meDtl.Length;
                            for (var i = 0; i < x; i++)
                            {
                                ctx.OmTrSalesSOModelColours.Remove(meDtl[i]);
                                ctx.SaveChanges();
                            }
                            ctx.OmTrSalesSOModels.Remove(me);
                            ctx.SaveChanges();
                            string status = updateStsSO(model.SONo);


                            //Update omTrPurchasePOModel
                            var SoHdr = ctx.OmTRSalesSOs.Where(z => z.CompanyCode == CompanyCode && z.BranchCode == BranchCode && z.SONo == model.SONo).FirstOrDefault();
                            if (SoHdr.RefferenceNo != "")
                            {
                                if (SoHdr.RefferenceNo.Substring(0, 2) == "PO")
                                {
                                    string CompanySD = getCompanySD(SoHdr.CustomerCode);
                                    string DbSD = getDbSD(CompanySD, SoHdr.CustomerCode);
                                    //string rmk = model.SONo + ", " + SoHdr.SODate;
                                    ctx.Database.ExecuteSqlCommand("UPDATE " + DbSD + "..omTrPurchasePOModel SET Remark='' WHERE CompanyCode='" + CompanySD + "' AND BranchCode='" + SoHdr.CustomerCode + "' AND PONo='" + SoHdr.RefferenceNo + "' AND SalesModelCode='" + model.SalesModelCode + "' AND SalesModelYear='" + model.SalesModelYear + "'");
                                }
                            }

                            returnObj = new { success = true, message = "Data SO Detail Model berhasil di delete.", Status = status };
                            trans.Complete();
                        }
                        else
                        {
                            returnObj = new { success = false, message = "Error ketika mendelete SO Detail Model, Message=Data tidak ditemukan" };
                            trans.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete SO Detail Model, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);

        }

        [HttpPost]
        public JsonResult Save3(OmTrSalesSOModelColour model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            //var SoHdr = ctx.OmTRSalesSOs.Find(CompanyCode, BranchCode, model.SONo);
            //string CompanySD = getCompanySD(SoHdr.CustomerCode);
            //string dbSD = getDbSD(CompanySD, SoHdr.CustomerCode);
           
            DateTime currentTime = DateTime.Now;
//            Int32 maxPO = ctx.Database.SqlQuery<Int32>(String.Format(@"SELECT QuantityPO FROM {0}..omTrPurchasePOModel WHERE CompanyCode='{1}' AND BranchCode='{2}' AND 
//                                                                     PONo='{3}'", dbSD, CompanySD, SoHdr.CustomerCode, SoHdr.RefferenceNo)).FirstOrDefault();
//            //Int32 countPO = ctx.Database.SqlQuery<Int32>(String.Format(@"SELECT Count(*) FROM ")).FirstOrDefault();

            var me = ctx.OmTrSalesSOModelColours.Find(CompanyCode, BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear, model.ColourCode);

            if (me == null)
            {
                me = new OmTrSalesSOModelColour();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.OmTrSalesSOModelColours.Add(me);
            }
            
            me.LastUpdateDate = currentTime;
            me.LastUpdateBy = userID;
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.SONo = model.SONo;
            me.SalesModelCode = model.SalesModelCode;
            me.SalesModelYear = model.SalesModelYear;
            me.ColourCode = model.ColourCode;
            me.Quantity = model.Quantity;
            me.Remark = model.Remark;

            try
            {
                Helpers.ReplaceNullable(me);
                ctx.SaveChanges();
                var meUpDtl = ctx.OmTrSalesSOModels.Find(CompanyCode, BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear);
                var iqty = ctx.Database.SqlQuery<decimal?>
                    (string.Format("select sum(quantity)  from omTrSalesSOModelColour where sono='{0}' and SalesModelCode='{1}' and SalesModelYear='{2}' group by sono", model.SONo,model.SalesModelCode,model.SalesModelYear)).FirstOrDefault();
                iqty = iqty ?? 0;
               
                if (meUpDtl != null) {
                    meUpDtl.QuantitySO = iqty;
                    Helpers.ReplaceNullable(meUpDtl);
                    ctx.SaveChanges();
                }
                string status = updateStsSO(model.SONo);

                var SoHdr = ctx.OmTRSalesSOs.Find(CompanyCode, BranchCode, model.SONo);
                if (SoHdr.RefferenceNo != null && SoHdr.RefferenceNo != "")
                {
                    if (SoHdr.RefferenceNo.Substring(0, 2) == "PO")
                    {
                        string CompanySD = getCompanySD(SoHdr.CustomerCode);
                        string DbSD = getDbSD(CompanySD, SoHdr.CustomerCode);

                        string Qry = String.Format(@"SELECT ISNULL(SUM(b.Quantity),0) FROM omTrSalesSO a INNER JOIN omTrSalesSOModelColour b
                                            ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.SONo = a.SONo
                                            WHERE a.CompanyCode='{0}' AND a.BranchCode ='{1}' AND a.RefferenceNo='{2}'", CompanyCode, BranchCode, SoHdr.RefferenceNo);
                        Decimal qtySaved = ctx.Database.SqlQuery<Decimal>(Qry).FirstOrDefault();
                        Decimal qtyPO = ctx.Database.SqlQuery<Decimal>(String.Format(@"SELECT QuantityPO FROM {2}..omTrPurchasePOModel WHERE CompanyCode='{0}' AND BranchCode='{1}' AND PONo='{3}'",
                                                                                   CompanySD, SoHdr.CustomerCode, DbSD, SoHdr.RefferenceNo)).FirstOrDefault();
                        if (qtyPO == qtySaved)
                        {
                            ctx.Database.ExecuteSqlCommand("UPDATE " + DbSD + "..omTrPurchasePO SET IsLocked=1 WHERE CompanyCode='" + CompanySD + "' AND BranchCode='" + SoHdr.CustomerCode + "' AND PONo='" + SoHdr.RefferenceNo + "'");
                        }
                    }
                }
                return Json(new { success = true, data = me, Status = status });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete3(OmTrSalesSOModelColour model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.OmTrSalesSOModelColours.Find(CompanyCode, BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear, model.ColourCode);
                    if (me != null)
                    {
                        var meSOVins = ctx.omTrSalesSOVins.Where(x => x.CompanyCode == companyCode && x.BranchCode == BranchCode && x.SalesModelCode == model.SalesModelCode && x.SalesModelYear == model.SalesModelYear && x.ColourCode == model.ColourCode && x.SONo == model.SONo).ToList();
                        foreach (var row in meSOVins)
                        {
                            ctx.omTrSalesSOVins.Remove(row);
                            ctx.SaveChanges();
                        }
                        ctx.OmTrSalesSOModelColours.Remove(me);
                        ctx.SaveChanges();

                        var meUpDtl = ctx.OmTrSalesSOModels.Find(CompanyCode, BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear);
                        var iqty = ctx.Database.SqlQuery<decimal?>
                            (string.Format("select sum(quantity)  from omTrSalesSOModelColour where sono='{0}' and salesmodelcode='{1}' and salesmodelyear='{2}'  group by sono", model.SONo,model.SalesModelCode,model.SalesModelYear)).FirstOrDefault();
                        iqty = iqty ?? 0;

                        if (meUpDtl != null)
                        {
                            if (meUpDtl.QuantitySO <= 0)
                            {
                                meUpDtl.QuantitySO = 0;
                            }
                            else
                            {
                                meUpDtl.QuantitySO = meUpDtl.QuantitySO - me.Quantity;
                            }
                            Helpers.ReplaceNullable(meUpDtl);
                            ctx.SaveChanges();
                        }
                        string status = updateStsSO(model.SONo);

                        var SoHdr = ctx.OmTRSalesSOs.Find(CompanyCode, BranchCode, model.SONo);

                        if (SoHdr.RefferenceNo != null && SoHdr.RefferenceNo != "")
                        {
                            if (SoHdr.RefferenceNo.Substring(0, 2) == "PO")
                            {
                                string CompanySD = getCompanySD(SoHdr.CustomerCode);
                                string DbSD = getDbSD(CompanySD, SoHdr.CustomerCode);

                                ctx.Database.ExecuteSqlCommand("UPDATE " + DbSD + "..omTrPurchasePO SET IsLocked=0 WHERE CompanyCode='" + CompanySD + "' AND BranchCode='" + SoHdr.CustomerCode + "' AND PONo='" + SoHdr.RefferenceNo + "'");
                            }
                        }

                        returnObj = new { success = true, message = "Data SO Detail Model Colour berhasil di delete.", Status = status };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete SO Detail Model Colour, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete SO Detail Model Colour, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);

        }

        [HttpPost]
        public JsonResult Save4(omTrSalesSOVin model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            //if (model.ChassisNo == null)
            //{
            //    return Json(new { success = false, message = "Tentukan no rangka terlebih dahulu!" });
            //}

            var qty = ctx.OmTrSalesSOModelColours.Find(CompanyCode, BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear, model.ColourCode).Quantity;
            var qtyVin = ctx.omTrSalesSOVins.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.SONo == model.SONo && x.SalesModelCode == model.SalesModelCode && x.SalesModelYear == model.SalesModelYear && x.ColourCode == model.ColourCode).Count();

            if (qtyVin >= qty) { return Json(new { success = false, message = "Jumlah Chassis melebihi jumlah maksimal quantity!" }); }

            if (model.SOSeq == 0)
            {
                int iseq = (from x in ctx.omTrSalesSOVins
                          where x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.SONo == model.SONo
                          orderby x.SOSeq descending
                          select x.SOSeq).FirstOrDefault();
                    model.SOSeq = iseq + 1;
            }
            var me = ctx.omTrSalesSOVins.Find(CompanyCode, BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear, model.ColourCode, model.SOSeq);

            if (me == null)
            {
                me = new omTrSalesSOVin();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.omTrSalesSOVins.Add(me);
            }

            var chkExist = ctx.omTrSalesSOVins.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.SalesModelCode == model.SalesModelCode &&
                           x.SalesModelYear == model.SalesModelYear && x.ChassisCode == model.ChassisCode && x.ChassisNo == model.ChassisNo).FirstOrDefault();
            var chkRetur = ctx.omTrSalesReturnVIN.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.SalesModelCode == model.SalesModelCode &&
                x.SalesModelYear == model.SalesModelYear && x.ChassisCode == model.ChassisCode && x.ChassisNo == model.ChassisNo).FirstOrDefault();
            if (chkExist != null && chkExist.SOSeq != model.SOSeq && chkRetur == null)
            {
                return Json(new { success = false, message = "Chassis no yg dimasukan sudah ada dalam list!" });
            }
            me.LastUpdateDate = currentTime;
            me.LastUpdateBy = userID;
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.SONo = model.SONo;
            me.SalesModelCode = model.SalesModelCode;
            me.SalesModelYear = model.SalesModelYear;
            me.ColourCode = model.ColourCode;
            me.SOSeq = model.SOSeq;
            me.ChassisCode = model.ChassisCode;
            me.ChassisNo = model.ChassisNo;
            me.EngineCode = model.EngineCode;
            me.EngineNo = model.EngineNo;
            me.ServiceBookNo = model.ServiceBookNo;
            me.KeyNo = model.KeyNo;
            me.EndUserName = model.EndUserName;
            me.EndUserAddress1 = model.EndUserAddress1;
            me.EndUserAddress2 = model.EndUserAddress2;
            me.EndUserAddress3 = model.EndUserAddress3;
            me.SupplierBBN = model.SupplierBBN;
            me.CityCode = model.CityCode;
            me.BBN = model.BBN;
            me.KIR = model.KIR;
            me.Remark = model.Remark;
            me.StatusReq = model.StatusReq;
            
            try
            {
                //Helpers.ReplaceNullable(me);
                ctx.SaveChanges();
                string status = updateStsSO(model.SONo);

                return Json(new { success = true, data = me, Status = status });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete4(omTrSalesSOVin model)
        { 
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.omTrSalesSOVins.Find(CompanyCode, BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear, model.ColourCode, model.SOSeq);
                    if (me != null)
                    {
                        ctx.omTrSalesSOVins.Remove(me);
                        ctx.SaveChanges();
                        string status = updateStsSO(model.SONo);
                        returnObj = new { success = true, message = "Data SO Detail Vin berhasil di delete.", Status = status };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete SO Detail Vin, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete SO Detail Vin, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);

        }

        [HttpPost]
        public JsonResult Save5(OmTrSalesSoModelOther model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.OmTrSalesSoModelOthers.Find(CompanyCode, BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear, model.OtherCode);

            if (me == null)
            {
                me = new OmTrSalesSoModelOther();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.OmTrSalesSoModelOthers.Add(me);
            }
            
            me.LastUpdateDate = currentTime;
            me.LastUpdateBy = userID;
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.SONo = model.SONo;
            me.SalesModelCode = model.SalesModelCode;
            me.SalesModelYear = model.SalesModelYear;
            me.OtherCode = model.OtherCode;
            me.BeforeDiscDPP = model.BeforeDiscDPP != null ? model.BeforeDiscDPP : 0;
            me.BeforeDiscPPn = model.BeforeDiscPPn != null ? model.BeforeDiscPPn : 0;
            me.BeforeDiscTotal = model.BeforeDiscTotal != null ? model.BeforeDiscTotal : 0;
            me.DiscExcludePPn = model.DiscExcludePPn != null ? model.DiscExcludePPn : 0;
            me.DiscIncludePPn = model.DiscIncludePPn != null ? model.DiscIncludePPn : 0;
            me.AfterDiscDPP = model.AfterDiscDPP != null ? model.BeforeDiscDPP : 0;
            me.AfterDiscPPn = model.AfterDiscPPn != null ? model.AfterDiscPPn : 0;
            me.AfterDiscTotal = model.AfterDiscTotal != null ? model.AfterDiscTotal : 0;
            me.DPP = model.DPP != null ? model.DPP : 0;
            me.PPn = model.PPn != null ? model.PPn : 0;
            me.Total = model.Total != null ? model.Total : 0;
            me.Remark = model.Remark;
            try
            {
                Helpers.ReplaceNullable(me);
                ctx.SaveChanges();
                string status = updateStsSO(model.SONo);
                return Json(new { success = true, data = me, Status = status });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete5(OmTrSalesSoModelOther model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.OmTrSalesSoModelOthers.Find(CompanyCode, BranchCode, model.SONo, model.SalesModelCode, model.SalesModelYear, model.OtherCode);
                    if (me != null)
                    {
                        ctx.OmTrSalesSoModelOthers.Remove(me);
                        ctx.SaveChanges();
                        string status = updateStsSO(model.SONo);
                        returnObj = new { success = true, message = "Data SO Detail Vin berhasil di delete.", Status = status };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete SO Detail Vin, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete SO Detail Vin, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);

        }

        [HttpPost]
        public JsonResult Save6(OmTrSalesSOAccsSeq model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string Qry = String.Empty;
            DateTime currentTime = DateTime.Now;
            Qry = string.Format(@"SELECT (OnHand - (AllocationSP + AllocationSR + AllocationSL)- (ReservedSP + ReservedSR + ReservedSL))  AS Available
		            FROM spMstItems WHERE PartNo = '{0}' AND CompanyCode = '{1}' AND BranchCode = '{2}'", model.PartNo, CompanyMD, BranchMD);
            decimal? qtyAvailable = ctxMD.Database.SqlQuery<decimal?>(Qry).FirstOrDefault();


            model.Qty = model.Qty == null ? 0 : model.Qty;
            qtyAvailable = qtyAvailable == null ? 0 : qtyAvailable;
            if (model.Qty > qtyAvailable) 
            {
                return Json(new { success = false, message = "Quantity yang dimasukkan melebihi stok, Stok saat ini = " + Convert.ToInt32(qtyAvailable) }); 
            }

            var me = ctx.OmTrSalesSOAccsSeqs.Find(CompanyCode, BranchCode, model.SONo, model.PartNo, model.PartSeq );

            if (me == null)
            {
                me = new OmTrSalesSOAccsSeq();
                me.CompanyCode = CompanyCode;
                me.BranchCode = BranchCode;
                me.SONo = model.SONo;
                me.PartNo = model.PartNo;
                me.PartSeq = 1;

                me.CreatedBy = userID;
                me.CreatedDate = currentTime;
                ctx.OmTrSalesSOAccsSeqs.Add(me);
            }

            decimal demand = Convert.ToDecimal(model.DemandQty);

            try
            {
                me.DemandQty = demand;
            }
            catch
            {
                me.DemandQty = 0;
            }

            me.Qty = Convert.ToDecimal(model.Qty);
            me.SupplyQty = 0;
            me.ReturnQty = 0;

            SpMstItemPrice price = ctx.SpMstItemPrices.Find(CompanyCode, BranchCode, model.PartNo);
            if (price != null)
            {
                //me.CostPrice = price.CostPrice;
                me.CostPrice = GetCostPrice(model.PartNo);
                me.RetailPrice = price.RetailPrice;
            }
            else
            {
                return Json(new { success = false, message = "Data Belum Ada di Master Item Price, Silahkan dilengkapi!" });
                //me.CostPrice = 0;
                //me.RetailPrice = 0;
            }

            me.RetailPrice = Convert.ToDecimal(model.RetailPrice);//txtTotPartBefDisc.Text);
            me.DiscExcludePPn = model.DiscExcludePPn != 0 ? Convert.ToDecimal(model.DiscExcludePPn) : price.RetailPrice;//(txtDiscPart.Text);
            me.AfterDiscDPP    = Convert.ToDecimal(model.AfterDiscDPP);//txtDPPPartAftDisc.Text);
            me.AfterDiscPPn    = Convert.ToDecimal(model.AfterDiscPPn);//txtPPnPartAftDisc.Text);
            me.AfterDiscTotal  = Convert.ToDecimal(model.AfterDiscTotal);//txtTotPartAftDisc.Text);

            SpMstItem item = ctx.SpMstItems.Find(CompanyCode, BranchCode, model.PartNo);
            if (item != null)
            {
                me.TypeOfGoods = item.TypeOfGoods;
            }

            //me.IsSubstitution = false;

            me.LastUpdateBy = userID;
            me.LastUpdateDate = currentTime;
            try
            {
                Helpers.ReplaceNullable(me);
                ctx.SaveChanges();
                string status = updateStsSO(model.SONo);
                return Json(new { success = true, data = me, Status = status });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete6(OmTrSalesSOAccsSeq model) 
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            //string branchCode = BranchCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.OmTrSalesSOAccsSeqs.Find(CompanyCode, BranchCode, model.SONo, model.PartNo, model.PartSeq);
                    if (me != null)
                    {
                        ctx.OmTrSalesSOAccsSeqs.Remove(me);
                        ctx.SaveChanges();
                        string status = updateStsSO(model.SONo);
                        returnObj = new { success = true, message = "Data SO Detail Vin berhasil di delete.", Status = status };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete SO Detail Vin, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete SO Detail Vin, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);

        }

        private ResultModel ValidateSODate(DateTime paramDate)
        {
            paramDate = paramDate.Date;
            ResultModel result = InitializeResult();
            DateTime currentDate = DateTime.Now;
            currentDate = currentDate.Date;
            //string profitCenter = ctx.SysUserProfitCenters.Where(x => x.UserId == CurrentUser.UserId).Select(x => x.ProfitCenter).FirstOrDefault();
            string profitCenter = ProfitCenter;
            if (!string.IsNullOrWhiteSpace(profitCenter))
            {
                switch (profitCenter)
                {
                    case "100":
                        //var oSales = ctx.GnMstCoProfileSalesmans.Find(CompanyCode, BranchCode);
                        var oSales = ctx.GnMstCoProfileSaleses.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).FirstOrDefault();
                        if (oSales != null)
                        {
                            oSales.TransDate = Convert.ToDateTime(oSales.TransDate).Date;
                            oSales.PeriodBeg = Convert.ToDateTime(oSales.PeriodBeg).Date;
                            oSales.PeriodEnd = Convert.ToDateTime(oSales.PeriodEnd).Date;

                            if (oSales.TransDate == null || oSales.TransDate.Value < new DateTime(1900, 1, 1))
                            {
                                oSales.TransDate = currentDate;
                            }

                            if (paramDate.Date >= oSales.PeriodBeg && paramDate <= oSales.PeriodEnd)
                            {
                                if (paramDate.Date <= currentDate)
                                {
                                    result.status = true;
                                    if (paramDate.Date >= oSales.TransDate.Value.Date)
                                    {
                                        if (oSales.isLocked.Value)
                                        {
                                            result.status = false;
                                            result.message = "Periode sedang dikunci.";
                                        }
                                    }
                                    else
                                    {
                                        result.status = false;
                                        result.message = "Tanggal transaksi lebih kecil dari tanggal " + oSales.TransDate.Value.ToShortDateString();
                                    }
                                }
                                else
                                {
                                    result.status = false;
                                    result.message = "Tanggal transaksi kurang dari tanggal server.";
                                }
                            }
                            else
                            {
                                if (paramDate.Date < oSales.PeriodBeg)
                                {
                                    result.message = "Tanggal transaksi kurang dari periode transaksi.";
                                }

                                if (paramDate.Date > oSales.PeriodEnd)
                                {
                                    result.message = "Tanggal transaksi lebih dari periode transaksi.";
                                }

                                result.status = false;
                            }
                        }

                        break;

                    case "200":
                        //var oService = ctx.GnMstCoProfileServices.Find(CompanyCode, BranchCode);
                        var oService = ctx.GnMstCoProfileServices.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).FirstOrDefault();
                        if (oService != null)
                        {
                            oService.TransDate = Convert.ToDateTime(oService.TransDate).Date;
                            oService.PeriodBeg = Convert.ToDateTime(oService.PeriodBeg).Date;
                            oService.PeriodEnd = Convert.ToDateTime(oService.PeriodEnd).Date;

                            if (oService.TransDate == null || oService.TransDate.Value <= new DateTime(1900, 1, 1))
                            {
                                oService.TransDate = currentDate;
                            }

                            if (paramDate >= oService.PeriodBeg && paramDate <= oService.PeriodEnd)
                            {
                                if (paramDate <= currentDate)
                                {
                                    if (paramDate >= oService.TransDate)
                                    {
                                        result.status = true;
                                        if (oService.isLocked.Value)
                                        {
                                            result.message = "Periode sedang terkunci.";
                                        }
                                    }
                                    else
                                    {
                                        result.message = "Tanggal transaksi lebih kecil dari " + oService.TransDate.Value.ToShortDateString();
                                    }
                                }
                                else
                                {
                                    result.message = "Tanggal server kurang dari tanggal transaksi.";
                                }
                            }
                            else
                            {
                                if (paramDate < oService.PeriodBeg)
                                {
                                    result.message = "Tanggal transaksi kurang dari periode transaksi.";
                                }

                                if (paramDate > oService.PeriodEnd)
                                {
                                    result.message = "Tanggal transaksi lebih dari periode transaksi.";
                                }
                            }
                        }

                        break;

                    case "300":
                        //var oSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
                        var oSpare = ctx.GnMstCoProfileSpares.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).FirstOrDefault();
                        if (oSpare != null)
                        {
                            oSpare.TransDate = Convert.ToDateTime(oSpare.TransDate).Date;
                            oSpare.PeriodBeg = Convert.ToDateTime(oSpare.PeriodBeg).Date;
                            oSpare.PeriodEnd = Convert.ToDateTime(oSpare.PeriodEnd).Date;

                            if (oSpare.TransDate == null || oSpare.TransDate == new DateTime(1900, 1, 1))
                            {
                                oSpare.TransDate = currentDate;
                            }

                            if (paramDate >= oSpare.PeriodBeg && paramDate <= oSpare.PeriodEnd)
                            {
                                if (paramDate <= currentDate)
                                {
                                    if (paramDate >= oSpare.TransDate)
                                    {
                                        result.status = true;

                                        if (oSpare.isLocked.Value)
                                        {
                                            result.message = "";
                                        }
                                    }
                                    else
                                    {
                                        result.message = "Tanggal transaksi kurang dari " + oSpare.TransDate.ToShortDateString();
                                    }
                                }
                                else
                                {
                                    result.message = "Tanggal transaksi kurang dari tanggal server.";
                                }
                            }
                        }

                        break;
                }
            }

            try
            {
                ctx.SaveChanges();
            }
            catch (Exception)
            {
                result.status = false;
                result.message = "Tidak dapat memvalidasi tanggal transaksi.";
            }

            return result;
        }

        private ResultModel CheckItsOrganization(string SalesCode, string salesmanName) 
        {
            ResultModel result = InitializeResult();
            string name = "";
            //string salesCoor = "";

            var oGnMstLookUpDtl = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "ITSFL" && p.LookUpValue == "STATUS");
            if (oGnMstLookUpDtl != null && oGnMstLookUpDtl.FirstOrDefault().ParaValue.ToString() == "1")
            {
                //// Sales Coordinator
                //var sc = ctx.PmMstTeamMembers.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.MemberID == SalesCode);
                //if (sc == null)
                //{
                //    result.status = false;
                //    result.message = "Salesman " + salesmanName + " belum memiliki Sales Koordinator di Master Organisasi ITS !";
                //}
                //else
                //{
                //    // Sales Coor
                //    salesCoor = sc.FirstOrDefault().EmployeeID;
                //    name = ctx.Employees.Where(p => p.EmployeeID == salesCoor).FirstOrDefault();

                //    var sh = ctx.PmMstTeamMembers.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && sc.Rows[0]["employeeid"].ToString());
                //    if (sh.Rows.Count == 0)
                //    {
                //        XMessageBox.ShowWarning("Sales Koordinator " + name + " belum memiliki Sales Head di Master Organisasi ITS !");
                //        return result;
                //    }
                //    else
                //    {
                //        // Sales Head
                //        salesHead = sh.Rows[0]["employeeid"].ToString();
                //        name = sh.Rows[0]["employeename"].ToString();

                //        DataTable bm = PmMstTeamMembersBLL.GetSalesNextPosition(user.CompanyCode, user.BranchCode
                //                        , sh.Rows[0]["employeeid"].ToString());

                //        if (bm.Rows.Count == 0)
                //        {
                //            XMessageBox.ShowWarning("Sales Head " + name + " belum memiliki Branch Manager di Master Organisasi ITS !");
                //            return result;
                //        }
                //        else
                //        {
                //            // Branch Manager
                //            branchManager = bm.Rows[0]["employeeid"].ToString();
                //            result = true;
                //            return result;
                //        }
                //    }
                //}
            }
            else
            {
                salesCoor = SalesCode;
                salesHead = SalesCode;
                var query = string.Format(@"
                SELECT TOP 1 EmployeeID
                FROM gnMstEmployee WITH (NOLOCK, NOWAIT)
                WHERE CompanyCode = '{0}'
                  AND BranchCode = '{1}'
                  AND TitleCode = '1'
                  AND PersonnelStatus = '1'
                ORDER BY EmployeeID 
                       ", CompanyCode,BranchCode);
                var queryable = ctx.Database.SqlQuery<string>(query).FirstOrDefault();

                if (queryable != null)
                    branchManager = queryable;
                else
                    branchManager = SalesCode;

                result.status = true;
            }

            return result;
        }

        private string GetNewSONumber(DateTime? SODate)
        {
            string newSONumber = ctx.Database.SqlQuery<string>("Exec uspfn_GnDocumentGetNew @CompanyCode=@p0, @BranchCode=@p1, @DocumentType=@p2, @UserID=@p3, @TransDate=@p4", CompanyCode, BranchCode, "SOR", CurrentUser.UserId, SODate.Value).FirstOrDefault();
            return newSONumber;
        }

        public JsonResult GetDefaultWarehouse()
        {
            var data = ctx.LookUpDtls.Where(x => x.CodeID == "MPWH" && x.ParaValue==BranchCode).OrderByDescending(x => x.SeqNo).FirstOrDefault();
            if (data != null)
            {
                return Json(new { 
                    WarehouseCode = data.LookUpValue,
                    WarehouseName = data.LookUpValueName
                });
            }

            return Json(null);
        }

        public JsonResult cekPriceListBranch(string SModelCode, string SModelYear, DateTime GrpPrice)
        {
            bool independent = false;
            var cMap = ctx.CompanyMappings.Where(c => c.CompanyCode == CompanyCode && c.BranchCode == BranchCode).FirstOrDefault();
            string gprice = "W" + GrpPrice.ToString("yy") + GrpPrice.ToString("MM");
            if ((CompanyCode == CompanyMD) && (BranchCode == BranchMD))
            {
                independent = true;
            }
            if (cMap != null && !independent && cekOtomatis())
            {
                var qry = @"SELECT COUNT(*) FROM " + cMap.DbMD + "..omPriceListBranches WHERE CompanyCode='" + cMap.CompanyMD + "' AND BranchCode='" + cMap.UnitBranchMD + "' AND SupplierCode='" + cMap.UnitBranchMD +"' AND SalesModelCode='" + SModelCode + "' AND SalesModelYear='" + SModelYear +
                    "' AND RetailPriceIncludePPN > 0 AND NetSalesIncludePPN > 0 AND RetailPriceExcludePPN > 0 AND NetSalesExcludePPN > 0 AND GroupPrice='" + gprice +
                    "' AND '" + Convert.ToDateTime(GrpPrice).ToString("yyyyMMdd") + "' >= convert(varchar,EffectiveDate,112) and IsStatus = 1";

                int plb = ctx.Database.SqlQuery<Int32>(qry).FirstOrDefault();
                if (plb > 0)
                {
                    return Json(new { success = true, message = "" });
                }
                else
                {
                    return Json(new { success = false, message = "Sales model ini tidak ada di Price List Branch atau memiliki nilai 0" });
                }
            }
            else
            {
                var qry2 = @"SELECT COUNT(*) FROM omMstPriceListSell WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "' AND SalesModelCode='" + SModelCode + "' AND SalesModelYear='" + SModelYear +
                    "' AND Total > 0 AND DPP > 0 AND PPN > 0";

                int plb = ctx.Database.SqlQuery<Int32>(qry2).FirstOrDefault();
                if (plb > 0)
                {
                    return Json(new { success = true, message = "" });
                }
                else
                {
                    return Json(new { success = false, message = "Sales model ini tidak ada di Price List Sell" });
                }
            }
        }

        public JsonResult Select4Tax(string customerCode)
        {
            var qry = @"SELECT a.TaxPct FROM gnMstTax a
                        INNER JOIN dbo.gnMstCustomerProfitCenter b
                        ON a.CompanyCode = b.CompanyCode AND a.TaxCode = b.TaxCode
                        AND a.CompanyCode = '" + CompanyCode + "' AND b.CustomerCode = '" + customerCode +
                        "' AND b.ProfitCenterCode = '100'";
            decimal taxpct = ctx.Database.SqlQuery<decimal>(qry).FirstOrDefault();
            
            if (taxpct != null)
            {
                return Json(new { success = true, data = taxpct, message = "" });
            }
            return Json(new { success = false, message = "Nilai Tax tidak ada!" });
        }

        public JsonResult getQtyUnit(string SONo)
        {
            decimal qty = ctx.Database.SqlQuery<decimal>("select quantityso from omtrsalessomodel where SONo ='" + SONo + "'").FirstOrDefault();
            return Json(new { success = true, qty = qty });
        }


        public JsonResult doCountPart(string CustCode, decimal TotPartAftDisc, decimal TotPartBefDisc)
        {
            decimal aftDiscTotal = 0;
            decimal befDiscTotal = 0;
            decimal discExcPPn = 0;
            decimal aftDiscDPP = 0;
            decimal aftDiscPPn = 0;
            decimal ppnPct = 0;

            aftDiscTotal = TotPartAftDisc;
            befDiscTotal = TotPartBefDisc;

            var qry = @"SELECT a.TaxPct FROM gnMstTax a
                       INNER JOIN dbo.gnMstCustomerProfitCenter b
                        ON a.CompanyCode = b.CompanyCode AND a.TaxCode = b.TaxCode
                        AND a.CompanyCode = '" + CompanyCode + "' AND b.CustomerCode = '" + CustCode +
                        "' AND b.ProfitCenterCode = '100'";
            ppnPct = ctx.Database.SqlQuery<decimal>(qry).FirstOrDefault();
            aftDiscDPP = Math.Round(aftDiscTotal / ((100 + ppnPct) / 100), MidpointRounding.AwayFromZero);
            discExcPPn = befDiscTotal - aftDiscDPP;
            if (discExcPPn < 0)
            {
                discExcPPn = 0;
                befDiscTotal = aftDiscDPP;
            }
            aftDiscPPn = aftDiscTotal - aftDiscDPP;
            //txtTotPartBefDisc.Text = befDiscTotal.ToString("n0");
            //txtDiscPart.Text = discExcPPn.ToString("n0");
            //txtTotPartAftDisc.Text = aftDiscTotal.ToString("n0");
            //txtDPPPartAftDisc.Text = aftDiscDPP.ToString("n0");
            //txtPPnPartAftDisc.Text = aftDiscPPn.ToString("n0");

            var dt = new { befDiscTotal, discExcPPn, aftDiscTotal, aftDiscDPP, aftDiscPPn };
            return Json(new { success = true, data = dt, message = "" });
        }

        public JsonResult doCount(string CustCode, string SlsModelCode, bool val, decimal beforeDiscTotal, decimal afterDiscTotal, decimal vdpp)
        {
            decimal ppnPct = 0;
            decimal ppnBmPct = 0;
            decimal dpp = 0;
            decimal ppnBm = 0;
            decimal ppn = 0;
            decimal disc = 0;
            decimal totalPrice = 0;

            var qry = @"SELECT a.TaxPct FROM gnMstTax a
                        INNER JOIN dbo.gnMstCustomerProfitCenter b
                        ON a.CompanyCode = b.CompanyCode AND a.TaxCode = b.TaxCode
                        AND a.CompanyCode = '" + CompanyCode + "' AND b.CustomerCode = '" + CustCode +
                        "' AND b.ProfitCenterCode = '100'";
            decimal taxpct = ctx.Database.SqlQuery<decimal>(qry).FirstOrDefault();
            
            if (taxpct == null || taxpct == 0)
            {
                return Json(new { success = false, message = "Customer belum di set pajak/nilai tax = 0" });
            }
            ppnPct = taxpct;
            OmMstModel mdl = ctx.OmMstModels.Where(x => x.CompanyCode == CompanyCode && x.SalesModelCode == SlsModelCode).FirstOrDefault();      
            if (mdl != null) ppnBmPct = (decimal)mdl.PpnBmPctSell;
            if (val == true)
            {
                totalPrice = afterDiscTotal;
                dpp = Math.Round(totalPrice / ((100 + ppnPct + ppnBmPct) / 100), MidpointRounding.AwayFromZero);
                ppnBm = Math.Floor(dpp * (ppnBmPct / 100));
                ppn = totalPrice - dpp - ppnBm;
                disc = beforeDiscTotal - totalPrice;

                //txtTotAftDisc.Text = totalPrice.ToString("n0");
                //txtDPPAftDisc.Text = dpp.ToString("n0");
                //txtPPnAftDisc.Text = ppn.ToString("n0");
                //txtPPnBMAftDisc.Text = ppnBm.ToString("n0");
                //txtDiscPPn.Text = (disc < 0) ? "0" : disc.ToString("n0");
            }
            else
            {
                dpp = vdpp;
                ppn = dpp * (ppnPct / 100);
                ppnBm = dpp * (ppnBmPct / 100);
                totalPrice = dpp + ppn + ppnBm;
                disc = beforeDiscTotal - totalPrice;

                //txtDPPAftDisc.Text = dpp.ToString("n0");
                //txtPPnAftDisc.Text = ppn.ToString("n0");
                //txtPPnBMAftDisc.Text = ppnBm.ToString("n0");
                //txtTotAftDisc.Text = totalPrice.ToString("n0");
                //txtDiscPPn.Text = (disc < 0) ? "0" : disc.ToString("n0");
            }
            var dt = new { dpp, ppn, ppnBm, disc, totalPrice};             
            return Json(new { success = true, data = dt, message = "" });
        }

        public JsonResult Approve(OmTRSalesSO model)
        {
            ResultModel result = InitializeResult();
            string userID = CurrentUser.UserId;
            DateTime currentDate = DateTime.Now;

            bool checkChassisNoMatch = ctx.Database.SqlQuery<bool>("exec uspfn_CheckChassisNoMatch @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2", CompanyCode, BranchCode, model.SONo).FirstOrDefault();

            if (!checkChassisNoMatch)
            {
                result.message = "Ada detail SO yang tidak memiliki nomor rangka atau SO tidak memiliki detail model.";
                return Json(result);
            }

            OmTRSalesSO omTrSalesSO = ctx.OmTRSalesSOs.Find(CompanyCode, BranchCode, model.SONo);
            //if (omTrSalesSO != null)
            //{
            //    omTrSalesSO.ApproveBy = userID;
            //    omTrSalesSO.ApproveDate = currentDate;
            //    omTrSalesSO.Status = "2";
            //    omTrSalesSO.LastUpdateBy = userID;
            //    omTrSalesSO.LastUpdateDate = currentDate;
            //}

            if (!string.IsNullOrWhiteSpace(omTrSalesSO.ProspectNo))
            {
                //Perlu ditanyakan kenapa di R2 tidak ada CodeID='Indent'      
            }

            if (ProductType == "4W")
            {
                result.status = true;
                result.data = new
                {
                    ProductType = "4W"
                };
            }
            else {
                result.status = true;
                result.data = new
                {
                    ProductType = "2W"
                };
            }
            return Json(result);
        }

        public JsonResult UnApproveCheck(SalesOrderForm model)
        {
            ResultModel result = InitializeResult();
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            bool isDO = false;
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            int cekDOBasedSONo = ctx.OmTrSalesDOs.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.SONo == model.SONumber).Count();
            if (cekDOBasedSONo > 0)
            {

                int cekInvBasedSONo = ctx.Database.SqlQuery<int>(
                                string.Format(@"
                select count(*) 
                from omTrSalesBPK a
                inner join omTrSalesBPKDetail b
                on a.CompanyCode = b.CompanyCode
                and a.BranchCode = b.BranchCode
                and a.BPKNo = b.BPKNo
                    where a.CompanyCode = {0}
                    and a.BranchCode  = {1}
                    and a.SONo = '{2}'
                    and b.StatusInvoice = 1
                    and a.Status <> 3", companyCode, branchCode, model.SONumber))
                .FirstOrDefault();


                //int cekInvBasedSONo = (
                //                            from x in ctx.OmTrSalesBPKs
                //                            from y in ctx.OmTrSalesBPKDetails
                //                            where
                //                            x.CompanyCode == y.CompanyCode
                //                            &&
                //                            x.BranchCode == y.BranchCode
                //                            &&
                //                            x.CompanyCode == companyCode
                //                            &&
                //                            x.BranchCode == branchCode
                //                            &&
                //                            x.SONo == model.SONumber
                //                            &&
                //                            y.StatusInvoice=="1"
                //                            &&
                //                            x.Status != "3"
                //                            select x.SONo
                //                       ).Count();

                if (cekInvBasedSONo > 0)
                {
                    result.message = "Batal unapprove, karna sudah ada invoice!";
                    result.data = new
                    {
                        state = "warning"
                    };
                    return Json(result);
                }
                else
                {
                    isDO = true;

                    int cekBPKBasedSONo = ctx.OmTrSalesBPKs.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.SONo == model.SONumber).Count();
                    if (cekBPKBasedSONo > 0)
                    {
                        result.message = "DO sudah ada, apakah semua DO akan dihapus?";
                        result.data = new
                        {
                            state = "confirmation",
                             isDO = true
                        };
                        return Json(result);
                    }
                    else
                    {
                        result.message = "BPK sudah ada, apakah semua BPK termasuk DO akan dihapus?";
                        result.data = new
                        {
                            state = "confirmation",
                            isDO = true
                        };
                        return Json(result);
                    }
                }
            }
            else
            {
                isDO = false;

                result.message = "DO belum ada, apakah unapprove dilanjutkan?";
                result.data = new
                {
                    state = "confirmation",
                    isDO = false
                };
                return Json(result);
            }
        }

        public JsonResult UnApprove(OmTRSalesSO model, bool isDO)
        {
            ResultModel result = InitializeResult();
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;
            string warehouseCode = "";
            string profitCenterCode = ctx.SysUserProfitCenters.Where(x => x.UserId == userID).Select(x => x.ProfitCenter).FirstOrDefault();
            bool vMD = false;

            if (string.IsNullOrWhiteSpace(profitCenterCode))
            {
                result.message = "User belum memiliki profit center.";
                return Json(result);
            }

            OmTRSalesSO omTrSalesSO = ctx.OmTRSalesSOs.Find(companyCode, branchCode, model.SONo);
            if (omTrSalesSO == null)
            {
                result.message = "SO tidak ditemukan.";
                return Json(result);
            }
            else
            {
                omTrSalesSO.Status = "0";
                omTrSalesSO.LastUpdateBy = userID;
                omTrSalesSO.LastUpdateDate = currentTime;
            }
            warehouseCode = omTrSalesSO.WareHouseCode;

            if (isDO)
            {
                var whdo = ctx.Database.SqlQuery<WHDOList>("exec uspfn_SelectWHDO @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2", companyCode, branchCode, model.SONo).ToList();
                foreach (var rWhdo in whdo)
                {
                    OmTrInventQtyVehicle omTrInventQtyVehicle = ctxMD.OmTrInventQtyVehicles.Where(x =>
                            x.CompanyCode == CompanyMD
                            &&
                            x.BranchCode == UnitBranchMD
                            &&
                            x.SalesModelCode == rWhdo.SalesModelCode
                            &&
                            x.SalesModelYear == rWhdo.SalesModelYear
                            &&
                            x.ColourCode == rWhdo.ColourCode
                            &&                            
                            x.WarehouseCode == rWhdo.WarehouseCode

                        )
                        .OrderByDescending(x=>x.Year)
                        .ThenByDescending(x=>x.Month)
                        .FirstOrDefault();

                    if (omTrInventQtyVehicle != null)
                    {
                        omTrInventQtyVehicle.Alocation = omTrInventQtyVehicle.Alocation - rWhdo.QtyDO;                        
                        omTrInventQtyVehicle.EndingOH = omTrInventQtyVehicle.BeginningOH + omTrInventQtyVehicle.QtyIn - omTrInventQtyVehicle.QtyOut;
                        omTrInventQtyVehicle.EndingAV = omTrInventQtyVehicle.BeginningAV + omTrInventQtyVehicle.QtyIn - omTrInventQtyVehicle.Alocation - omTrInventQtyVehicle.QtyOut;
                        omTrInventQtyVehicle.LastUpdateBy = userID;
                        omTrInventQtyVehicle.LastUpdateDate = currentTime;
                    }
                }
            }
            else
            {
                var colourModels = ctx.OmTrSalesSOModelColours.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.SONo == model.SONo).ToList();
                string salesModelCode = "";
                decimal salesModelYear = 0;
                foreach (var colourModel in colourModels)
                {
                    OmTrInventQtyVehicle omTrInventQtyVehicle = ctxMD.OmTrInventQtyVehicles.Where(x =>
                            x.CompanyCode == CompanyMD
                            &&
                            x.BranchCode == UnitBranchMD
                            &&
                            x.SalesModelCode == colourModel.SalesModelCode
                            &&
                            x.SalesModelYear == colourModel.SalesModelYear
                            &&
                            x.ColourCode == colourModel.ColourCode
                            &&
                            x.WarehouseCode == warehouseCode
                        )
                        .OrderByDescending(x => x.Year)
                        .ThenByDescending(x=>x.Month)
                        .FirstOrDefault();

                    if (omTrInventQtyVehicle != null)
                    {
                        salesModelCode = omTrInventQtyVehicle.SalesModelCode;
                        salesModelYear = omTrInventQtyVehicle.SalesModelYear;
                        omTrInventQtyVehicle.Alocation = omTrInventQtyVehicle.Alocation - colourModel.Quantity;
                        omTrInventQtyVehicle.EndingOH = omTrInventQtyVehicle.BeginningOH + omTrInventQtyVehicle.QtyIn - omTrInventQtyVehicle.QtyOut;
                        omTrInventQtyVehicle.EndingAV = omTrInventQtyVehicle.BeginningAV + omTrInventQtyVehicle.QtyIn - omTrInventQtyVehicle.Alocation - omTrInventQtyVehicle.QtyOut;
                        omTrInventQtyVehicle.LastUpdateBy = userID;
                        omTrInventQtyVehicle.LastUpdateDate = currentTime;
                    }
                }

                var additionalInfos = ctx.OmTrSalesSOModelAdditionals.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.SONo == model.SONo && x.SalesModelCode == salesModelCode && x.SalesModelYear == salesModelYear);
                foreach (var additionalInfo in additionalInfos)
                {
                    ctx.OmTrSalesSOModelAdditionals.Remove(additionalInfo);
                }
            }

            var bankBook = ctx.GnTrnBankBooks.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.CustomerCode == model.CustomerCode && x.ProfitCenterCode == ProfitCenter).FirstOrDefault();
            if (bankBook == null)
            {
                bankBook = new GnTrnBankBook();
                bankBook.CompanyCode = companyCode;
                bankBook.BranchCode = branchCode;
                bankBook.CustomerCode = model.CustomerCode;
                bankBook.ProfitCenterCode = profitCenterCode;
                bankBook.ReceivedAmt = 0;

                ctx.GnTrnBankBooks.Add(bankBook);
            }
            var salesAmt = ctx.OmTrSalesSOModels.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.SONo == model.SONo).Sum(x => (x.QuantitySO * x.AfterDiscTotal));
            bankBook.SalesAmt = bankBook.SalesAmt + (salesAmt ?? 0);

            if (string.IsNullOrWhiteSpace(model.ProspectNo) == false)
            {
                int itsNumber = Convert.ToInt32(model.ProspectNo);
                var pmKDP = ctx.PmKdps.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.InquiryNumber == itsNumber).FirstOrDefault();
                if (pmKDP != null)
                {
                    pmKDP.LastProgress = "HP";
                    pmKDP.LastUpdateBy = userID;
                    pmKDP.LastUpdateDate = currentTime;
                }
            }

            if (isDO)
            {
                var chassisBySO = ctx.Database.SqlQuery<ChassisCheckModel>("exec uspfn_GetChassisBySO @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2", companyCode, branchCode, model.SONo).ToList();
                foreach (var item in chassisBySO)
                {
                    decimal chassisNo = Convert.ToDecimal(item.ChassisNo);

                    var omMstVehicle = ctx.OmMstVehicles.Where(x => x.CompanyCode == companyCode && x.ChassisNo == item.ChassisNo && x.ChassisCode == item.ChassisCode).FirstOrDefault();
                    if (omMstVehicle == null)
                    {
                        omMstVehicle = ctxMD.OmMstVehicles.Where(x => x.CompanyCode == CompanyMD && x.ChassisNo == item.ChassisNo && x.ChassisCode == item.ChassisCode).FirstOrDefault();
                        vMD = true;
                    }
                    else
                    {
                        vMD = false;
                    }
                    //var omMstVehicle = ctxMD.OmMstVehicles.Where(x => x.CompanyCode == CompanyMD && x.ChassisCode == item.ChassisCode && x.ChassisNo == chassisNo).FirstOrDefault();
                    if (omMstVehicle != null)
                    {
                        omMstVehicle.BPKNo = "";
                        omMstVehicle.DONo = "";
                        omMstVehicle.SONo = "";
                        omMstVehicle.IsAlreadyPDI = false;
                        omMstVehicle.Status = "0";
                        omMstVehicle.LastUpdateBy = userID;
                        omMstVehicle.LastUpdateDate = currentTime;
                        omMstVehicle.BPKDate = DateTime.Now;
                    }
                    //if (vMD)
                    //{
                    //    ctxMD.SaveChanges();
                    //}
                }

                var getBPKModel = ctx.Database.SqlQuery<BpkSeqModel>("exec uspfn_GetBPKModelbySO @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2", companyCode, branchCode, model.SONo).ToList();
                foreach (var item in getBPKModel)
                {
                    var omTrSalesBPKModel = ctx.OmTrSalesBPKModels.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.BPKNo == item.BPKNo && x.SalesModelCode == item.SalesModelCode && x.SalesModelYear == item.SalesModelYear).FirstOrDefault();
                    if (omTrSalesBPKModel != null)
                    {
                        ctx.OmTrSalesBPKModels.Remove(omTrSalesBPKModel);
                    }
                }

                var getBPKSeq = ctx.Database.SqlQuery<BpkSeqModel>("exec uspfn_GetBPKDetSeqbySO @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2", companyCode, branchCode, model.SONo).ToList();
                foreach (var item in getBPKSeq)
                {
                    var omTrSalesBPKDetail = ctx.OmTrSalesBPKDetails.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.BPKNo == item.BPKNo && x.BPKSeq == item.BPKSeq).FirstOrDefault();
                    if (omTrSalesBPKDetail != null)
                    {
                        ctx.OmTrSalesBPKDetails.Remove(omTrSalesBPKDetail);
                    }
                }


                var getBPKNo = ctx.Database.SqlQuery<BpkSeqModel>("exec uspfn_GetBPKNobySO @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2", companyCode, branchCode, model.SONo).ToList();
                foreach (var item in getBPKNo)
                {
                    var omTrSalesBPK = ctx.OmTrSalesBPKs.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.BPKNo == item.BPKNo).FirstOrDefault();
                    if (omTrSalesBPK != null)
                    {
                        omTrSalesBPK.Status = "3";
                        omTrSalesBPK.LastUpdateBy = userID;
                        omTrSalesBPK.LastUpdateDate = DateTime.Now;
                    }
                }

                var omTrSalesSOModel = ctx.OmTrSalesSOModels.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.SONo == model.SONo);
                foreach (var item in omTrSalesSOModel)
                {
                    item.QuantityDO = 0;
                    item.LastUpdateBy = userID;
                    item.LastUpdateDate = currentTime;
                }

                var getDOSeq = ctx.Database.SqlQuery<DOSeqModel>("exec uspfn_getDODetSeqbySO @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2", companyCode, branchCode, model.SONo).ToList();
                foreach (var item in getDOSeq)
                {
                    var omTrSalesDODetail = ctx.OmTrSalesDODetails.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.DONo == item.DONo).FirstOrDefault();
                    if (omTrSalesDODetail != null)
                    {
                        ctx.OmTrSalesDODetails.Remove(omTrSalesDODetail);
                    }
                }

                var getDONobySO = ctx.Database.SqlQuery<DOSeqModel>("exec uspfn_GetDONobySO @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2", companyCode, branchCode, model.SONo).ToList();
                foreach (var item in getDONobySO)
                {
                    var omTrSalesDO = ctx.OmTrSalesDOs.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.DONo == item.DONo).FirstOrDefault();
                    if (omTrSalesDO != null)
                    {
                        omTrSalesDO.Status = "3";
                    }
                }

                var getInvoice = ctx.OmTrSalesInvoices.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.SONo == model.SONo).ToList();
                foreach (var item in getInvoice)
                {
                    item.Status = "3";
                    item.LastUpdateBy = userID;
                    item.LastUpdateDate = currentTime;
                }
            }
            else
            {
                var omTrSalesSOVin = ctx.omTrSalesSOVins.Where(x => x.CompanyCode == companyCode && x.BranchCode == branchCode && x.SONo == model.SONo).ToList();
                foreach (var item in omTrSalesSOVin)
                {
                    
                    var omMstVehicle = ctx.OmMstVehicles.Where(x => x.CompanyCode == companyCode && x.ChassisNo == item.ChassisNo && x.ChassisCode == item.ChassisCode).FirstOrDefault();
                    if (omMstVehicle == null)
                    {
                        omMstVehicle = ctxMD.OmMstVehicles.Where(x => x.CompanyCode == CompanyMD && x.ChassisNo == item.ChassisNo && x.ChassisCode == item.ChassisCode).FirstOrDefault();
                        vMD = true;
                    }
                    else
                    {
                        vMD = false;
                    }
                    if (omMstVehicle != null)
                    {
                        omMstVehicle.SONo = "";
                        omMstVehicle.IsAlreadyPDI = false;
                        omMstVehicle.Status = "0";
                        omMstVehicle.BPKDate = currentTime;
                        omMstVehicle.LastUpdateBy = userID;
                        omMstVehicle.LastUpdateDate = currentTime;
                    }

                    //if (vMD)
                    //{
                    //    ctxMD.SaveChanges();
                    //}
                }
            }

            try
            {
                ctxMD.Database.ExecuteSqlCommand("DELETE FROM svSDMovement WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "' AND DocNo='" + model.SONo + "'");
                ctxMD.SaveChanges();
                ctx.SaveChanges();
                OmTrSalesBLL.Instance(CurrentUser.UserId).SDMovementSORem(model);
            
                result.status = true;
                result.message = "Unapprove berhasil.";
                result.data = new {
                    SOStatus = "UNAPPROVED"
                };
            }
            catch (Exception)
            {
                result.message = "Maaf, Unapprove gagal.";
            }

            return Json(result);
        }

        public JsonResult Reject(OmTRSalesSO model)
        {
            ResultModel result = InitializeResult();
            string userID = CurrentUser.UserId;
            DateTime currentDate = DateTime.Now;

            int cekDOBasedSONo = ctx.Database.SqlQuery<int>("exec uspfn_IsDOExists @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2", CompanyCode, BranchCode, model.SONo).FirstOrDefault();
            if (cekDOBasedSONo > 0)
            {
                result.message = "DO sudah ada, tidak bisa melakukan reject.";
                return Json(result);
            }

            OmTRSalesSO omTrSalesSO = ctx.OmTRSalesSOs.Find(CompanyCode, BranchCode, model.SONo);
            if (omTrSalesSO != null)
            {
                omTrSalesSO.ApproveBy = userID;
                omTrSalesSO.ApproveDate = currentDate;
                omTrSalesSO.Status = "4";
                omTrSalesSO.LastUpdateBy = userID;
                omTrSalesSO.LastUpdateDate = currentDate;
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "SO berhasil direject.";
                result.data = new { Status = 4 }; 
            }
            catch (Exception)
            {
                result.message = "SO gagal direject.";
            }

            return Json(result);
        }

        public JsonResult PrintSO(string SONo)  
        {
            try
            {
                //check whether detail SO data doesn't exist
                var uid = CurrentUser.UserId;
                var recModel = ctx.OmTrSalesSOModels.Where(p => p.SONo == SONo);
                if (recModel.Count() < 1)
                {
                    return Json(new { success = true, message = "Dokumen tidak dapat dicetak karena tidak memiliki data detail", isDataExist = false });
                }

                var record = ctx.OmTRSalesSOs.Find(CompanyCode, BranchCode, SONo);
                if (record.Status != "0")
                    return Json(new { success = true, message = "", isDataExist = false });
                else
                {
                    if (record.Status == "0" || record.Status.Trim() == "") record.Status = "1";
                }

                record.LastUpdateBy = uid;
                record.LastUpdateDate = ctx.CurrentTime;
                bool result = ctx.SaveChanges() > 0;

                if (result)
                {
                    return Json(new { success = true, Status = record.Status, isDataExist = true });
                }
                else
                {
                    return Json(new { success = true, message = "", isDataExist = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Terjadi kesalahan pada saat Print data, silahkan hubungi SDMS support",
                    error_log = ex.Message
                });
            }
        }

        public JsonResult getRecord(string ChassisCode, decimal ChassisNo) 
        {
            var data = ctx.OmMstVehicles.Where(x => x.CompanyCode == CompanyCode && x.ChassisCode == ChassisCode && x.ChassisNo == ChassisNo).FirstOrDefault();
            if (data != null)
            {
                return Json(new
                {
                    data = data
                });
            }

            return Json(null);
        }

        private bool UpdateBankBook(DataContext ctxx, string SONo, string customerCode, string profitCenterCode, bool isReject)
        {
            bool result = false;
            bool isNew = false;
            string companycode = CompanyCode;
            string branchcode = BranchCode;
            var bankBook = ctxx.GnTrnBankBooks.Where(p => p.CompanyCode == companycode && p.BranchCode == branchcode && p.CustomerCode == customerCode && p.ProfitCenterCode == "100").FirstOrDefault();//companycode, branchcode, customerCode, profitCenterCode);
            if (bankBook == null)
            {
                isNew = true;
                bankBook = new GnTrnBankBook();
                bankBook.CompanyCode = CompanyCode;
                bankBook.BranchCode = BranchCode;
                bankBook.CustomerCode = customerCode;
                //bankBook.ProfitCenterCode = profitCenterCode;
                bankBook.ProfitCenterCode = "100";
                bankBook.ReceivedAmt = 0;
                bankBook.SalesAmt = 0;
            }
            if (!isReject)
                bankBook.SalesAmt = bankBook.SalesAmt + GetSalesAmt(CompanyCode, BranchCode, SONo);
            else bankBook.SalesAmt = bankBook.SalesAmt - GetSalesAmt(CompanyCode, BranchCode, SONo);

            if (isNew)
            {
                ctxx.GnTrnBankBooks.Add(bankBook);
                try
                {
                    ctxx.SaveChanges();
                    result = true;
                }
                catch (Exception Ex)
                {
                    result = false;
                }
            }
            else
            {
                ctxx.SaveChanges();
                result = true;
            }

            return result;
        }

        private void UpdateModelOwnerShip(string SONo, string salesModelCode, decimal salesModelYear, string statusVehicle, string othersBrand, string othersType)
        {
            string userID = CurrentUser.UserId;
            OmTrSalesSOModelAdditional oOmTrSalesModelAdditional = new OmTrSalesSOModelAdditional();
            OmTrSalesSOModelAdditional model = ctx.OmTrSalesSOModelAdditionals.Find(CompanyCode,BranchCode, SONo, salesModelCode, salesModelYear);
            //GetRecord(companyCode, branchCode, SONo, salesModelCode, salesModelYear);
            if (model == null)
            {
                model = new OmTrSalesSOModelAdditional();
                model.CompanyCode = CompanyCode;
                model.BranchCode = BranchCode;
                model.CreatedBy = userID;
                model.CreatedDate = DateTime.Now;
                model.LastUpdateBy = userID; 
                model.LastUpdateDate = DateTime.Now;
                model.StatusVehicle = statusVehicle;
                model.OthersBrand = othersBrand;
                model.OthersType = othersType;
                model.SalesModelCode = salesModelCode;
                model.SalesModelYear = salesModelYear;
                model.SONo = SONo;
            }
            else
            {
                model.StatusVehicle = statusVehicle;
                model.OthersType = othersType;
                model.OthersBrand = othersBrand;
                model.LastUpdateDate = DateTime.Now;
                model.LastUpdateBy = userID; 
            }
            ctx.OmTrSalesSOModelAdditionals.Add(model);
        }

        public List<OmTrInventQtyVehicle> getVehicle(string CompanyCode, string BranchCode, string SalesModelCode, decimal SalesModelYear, string ColourCode, string wareHouseCode) 
        {
            var query = string.Format(@"
                        SELECT    TOP 1
                            * 
                          FROM omTrInventQtyVehicle a
                         WHERE a.CompanyCode = '{0}'
                               AND a.BranchCode ='{1}'
                               AND a.SalesModelCode = '{2}'
                               AND a.SalesModelYear = '{3}'
                               AND a.ColourCode = '{4}'
                               AND a.WarehouseCode = '{5}'
                        ORDER BY a.[Year] DESC, a.[Month] DESC
                       ", CompanyCode, BranchCode, SalesModelCode, SalesModelYear, ColourCode, wareHouseCode);
            var vehicle = ctx.Database.SqlQuery<OmTrInventQtyVehicle>(query).ToList();
            return vehicle;
        }

        private decimal GetSalesAmt(string companyCode, string branchCode, string SONo)
        {
            decimal unitAmt, accAmt, spareAmt;
            unitAmt = accAmt = spareAmt = 0;
            var query = string.Format(@"
                 select sum(a.QuantitySO * a.AfterDiscTotal) as TotSO from OmTrSalesSOModel a
                WHERE a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.SONo = '{2}'
                       ", companyCode, branchCode, SONo);
            var obj = ctx.Database.SqlQuery<decimal>(query).FirstOrDefault(); 
            if (obj != null)
            {
                unitAmt = Convert.ToDecimal(obj.ToString());
            }
            var query1 = string.Format(@"
                 select isnull(sum(a.AfterDiscTotal * b.QuantitySO), 0) from OmTrSalesSOModelOthers a 
                inner join OmTrSalesSOModel b 
                    on a.companyCode = b.companyCode and a.branchCode = b.branchCode and a.SONo = b.SONo
                         and a.salesModelCode = b.salesModelCode and a.SalesModelYear = b.salesModelYear
                WHERE a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.SONo = '{2}'
                       ", companyCode, branchCode, SONo);
            var obj1 = ctx.Database.SqlQuery<decimal>(query1).FirstOrDefault();  
            
            if (obj1 != null)
            {
                accAmt = Convert.ToDecimal(obj1.ToString());
            }

            var query2 = string.Format(@"
                 select isnull(sum(a.DemandQty * a.RetailPrice), 0) from OmTrSalesSoAccs a
                WHERE a.CompanyCode = '{0}'
                    AND a.BranchCode = '{1}'
                    AND a.SONo = '{2}'
                       ", companyCode, branchCode, SONo);
            var obj2 = ctx.Database.SqlQuery<decimal>(query2).FirstOrDefault();  
          
            if (obj2 != null)
            {
                spareAmt = Convert.ToDecimal(obj2.ToString());
            }
            return unitAmt + accAmt + spareAmt;
        }

        public JsonResult ApproveSO(string SONo, bool islinkITS, List<OWSalesModel> additionalOwnership)
        {
            ResultModel resultto = InitializeResult();
            bool result = false;
            bool independent = false;
            var json = Json(new { });
            var msg = "";
            var Qry = string.Empty;
            string dbMD = ctx.Database.SqlQuery<string>("SELECT DbMD from gnMstCompanyMapping WHERE CompanyCode='" + CompanyCode + "' AND BranchCode='" + BranchCode + "'").FirstOrDefault();
            var userID = CurrentUser.UserId;
            var OmTrSalesSO = new OmTrSalesSOBLL(ctx, userID);
            var OmTrSalesSOVin = new OmTrSalesSOVinBLL(ctx, userID);
            OmTRSalesSO record = OmTrSalesSO.GetRecord(SONo);
            string companyCode = record.CompanyCode;
            string branchCode = record.BranchCode;
            string companymd = CompanyCodeMD;
            string unitbranch = UnitBranchCodeMD;
            string profitcenter = ProfitCenter;
            string warehouseCode = record.WareHouseCode;
            string dbmd = "";
            bool otom = cekOtomatis();

            if ((CompanyCode == CompanyMD) && (BranchCode == BranchMD))
            {
                independent = true;
            }
            if (!independent)
            {
                dbmd = ctx.CompanyMappings.Where(q => q.CompanyCode == companyCode && q.BranchCode == branchCode).FirstOrDefault().DbMD;
            }
            
            List<OmTrSalesSOModelColour> listModelColour = ctx.OmTrSalesSOModelColours.Where(p => p.CompanyCode == companyCode && p.BranchCode == branchCode && p.SONo == SONo).ToList();
            List<omTrSalesSOVin> dt = ctx.omTrSalesSOVins.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.SONo == SONo && p.ChassisNo != 0).ToList();
                       
            foreach (var row in additionalOwnership)
            {
                if (row.StatusVehicle != "A")
                {
                    if (row.BrandCode == null)
                    {
                        resultto.message = "Approved gagal" + "\n" + "Silahkan isi tipe / model terlebih dahulu untuk setiap kendaraan";
                        return Json(resultto);
                    }
                }
            }

            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    if (record == null)
                        return Json(resultto);
                    else
                    {
                        record.ApproveBy = userID;
                        record.ApproveDate = DateTime.Now;
                        record.LastUpdateDate = DateTime.Now;
                        record.LastUpdateBy = userID;
                        record.Status = "2";
                    }

                    foreach (var row in dt)
                    {
                        string oOmMstVehicle = "";
                        if (independent || !otom)
                        {
                            oOmMstVehicle = ctx.Database.SqlQuery<string>("select status from OmMstVehicle where ChassisCode=@p0 and ChassisNo=@p1"
                                , row.ChassisCode, row.ChassisNo).FirstOrDefault();
                        }
                        else
                        {
                            oOmMstVehicle = ctx.Database.SqlQuery<string>("select status from " + dbmd + ".dbo.OmMstVehicle where ChassisCode=@p0 and ChassisNo=@p1"
                               , row.ChassisCode, row.ChassisNo).FirstOrDefault();
                        }
                        if (oOmMstVehicle != null)
                        {
                            if (oOmMstVehicle != "0")
                                msg = "Untuk kendaraan dengan ChassisCode= " + row.ChassisCode + " dan ChassisNo= " + row.ChassisNo.ToString()
                                    + " tidak berstatus Ready";
                            bool updateMstVehicle = ctx.Database.SqlQuery<bool>("exec sp_updateOmMstVehicleSO @CompanyCode=@p0, @BranchCode=@p1, @ChassisCode=@p2,@ChassisNo=@p3,@SONO=@p4, @UserId=@p5"
                                , companyCode, branchCode, row.ChassisCode, row.ChassisNo, record.SONo, userID).FirstOrDefault();

                            if (!updateMstVehicle)
                            {
                                msg = "Untuk kendaraan dengan ChassisCode= " + row.ChassisCode + " dan ChassisNo= " + row.ChassisNo.ToString()
                                   + " tidak ada terupdate di Master Kendaraan";
                            }
                            else result = true;

                            //OmMstVehicle oOmMstVehicleDao = new OmMstVehicle();
                            //oOmMstVehicle.Status = "3";
                            //oOmMstVehicle.SONo = record.SONo;
                            //oOmMstVehicle.LastUpdateBy = userID;
                            //oOmMstVehicle.LastUpdateDate = DateTime.Now;
                            //Helpers.ReplaceNullable(oOmMstVehicle);
                            //if (ctxMD.SaveChanges() >= 0)
                            //    msg = "Untuk kendaraan dengan ChassisCode= " + oOmMstVehicle.ChassisCode + " dan ChassisNo= " + oOmMstVehicle.ChassisNo.ToString()
                            //        + " tidak ada terupdate di Master Kendaraan";
                            //else result = true;
                        }
                        else
                            msg = "Untuk kendaraan ini belum menjadi stock";


                    }

                    foreach (OmTrSalesSOModelColour modelColour in listModelColour)
                    {
                        //List<OmTrInventQtyVehicle> vehicle = getVehicle(CompanyCode, BranchCode,modelColour.SalesModelCode, modelColour.SalesModelYear, modelColour.ColourCode, warehouseCode);
                        //var vehicle = ctxMD.OmTrInventQtyVehicles.Where(p =>
                        //    p.CompanyCode == companymd &&
                        //    p.BranchCode == unitbranch &&
                        //    p.SalesModelCode == modelColour.SalesModelCode &&
                        //    p.SalesModelYear == modelColour.SalesModelYear &&
                        //    p.ColourCode == modelColour.ColourCode &&
                        //    p.WarehouseCode == warehouseCode).FirstOrDefault();

                        if (independent || !otom)
                        {
                            Qry = "SELECT TOP 1 * FROM OmTrInventQtyVehicle a WHERE a.CompanyCode='" + CompanyCode + "' AND a.BranchCode='" + BranchCode + "' AND a.SalesModelCode='" + modelColour.SalesModelCode +
                                  "' AND a.SalesModelYear='" + modelColour.SalesModelYear + "' AND a.ColourCode='" + modelColour.ColourCode + "' AND a.WarehouseCode='" + record.WareHouseCode + "' ORDER BY a.Year DESC, a.Month DESC";
                        }
                        else
                        {
                            Qry = "SELECT TOP 1 * FROM " + dbMD + "..OmTrInventQtyVehicle a WHERE a.CompanyCode='" + CompanyMD + "' AND a.BranchCode='" + UnitBranchMD + "' AND a.SalesModelCode='" + modelColour.SalesModelCode +
                                  "' AND a.SalesModelYear='" + modelColour.SalesModelYear + "' AND a.ColourCode='" + modelColour.ColourCode + "' AND a.WarehouseCode='" + record.WareHouseCode + "' ORDER BY a.Year DESC, a.Month DESC";
                        }

                        //var vehicle = ctxMD.OmTrInventQtyVehicles
                        //        .Where(x => x.CompanyCode == CompanyMD &&
                        //                x.BranchCode == BranchMD &&
                        //                x.SalesModelCode == modelColour.SalesModelCode &&
                        //                x.SalesModelYear == modelColour.SalesModelYear &&
                        //                x.ColourCode == modelColour.ColourCode &&
                        //                x.WarehouseCode == record.WareHouseCode
                        //        )
                        //        .OrderByDescending(x => x.Year)
                        //        .ThenByDescending(x => x.Month)
                        //        .FirstOrDefault();
                        
                        var vehicle = ctx.Database.SqlQuery<OmTrInventQtyVehicle>(Qry).FirstOrDefault(); 
                            
                            ////ctx.Database.SqlQuery<string>("select SalesModelCode from " + dbmd +".dbo.OmTrInventQtyVehicle where CompanyCode=@p0 and BranchCode=@p1 and SalesModelCode=@p2 and SalesModelYear=@p3 and ColourCode=@p4 and WareHousecode = @p5 "
                            ////   , companymd, unitbranch, modelColour.SalesModelCode, modelColour.SalesModelYear, modelColour.ColourCode, warehouseCode).FirstOrDefault();
                        if (vehicle != null)
                        {
                            ////int updateTrInventQtyVehicle = ctx.Database.SqlQuery<int>("exec sp_updateOmTrInventQtyVehicleSO @CompanyCode=@p0, @BranchCode=@p1, @SalesModelCode=@p2,@SalesModelYear=@p3,@ColourCode=@p4, @WarehouseCode=@p5, @Quantity=@p6, @UserId=@p7 "
                            ////    , companyCode, branchCode, modelColour.SalesModelCode, modelColour.SalesModelYear, modelColour.ColourCode, warehouseCode, modelColour.Quantity, userID).FirstOrDefault();

                            ////if (updateTrInventQtyVehicle == 1)
                            ////{
                            ////    msg = "Untuk kendaraan dengan Model= " + modelColour.SalesModelCode + " dan Warna= " + modelColour.ColourCode
                            ////        + " tidak mempunyai cukup available unit";
                            ////}
                            ////else if (updateTrInventQtyVehicle == 0)
                            ////{
                            ////    msg = "Untuk kendaraan dengan Model= " + modelColour.SalesModelCode + " dan Warna= " + modelColour.ColourCode
                            ////        + " tidak ada terupdate di Stock Inventory";
                            ////}
                            ////else result = true;

                            //Blok by Irfan test coiy...
                            //vehicle.Alocation = vehicle.Alocation + modelColour.Quantity;
                            //vehicle.EndingOH = vehicle.BeginningOH + vehicle.QtyIn - vehicle.QtyOut;
                            //vehicle.EndingAV = vehicle.BeginningAV + vehicle.QtyIn - vehicle.Alocation - vehicle.QtyOut;
                            
                            ////decimal i = CalculateQuantitySO(ctx, companyCode, branchCode, modelColour.SONo, modelColour.SalesModelCode, modelColour.SalesModelYear, modelColour.ColourCode, warehouseCode);
                            //if (vehicle.EndingAV < 0)
                            //    msg = "Untuk kendaraan dengan Model= " + modelColour.SalesModelCode + " dan Warna= " + modelColour.ColourCode
                            //        + " tidak mempunyai cukup available unit";

                            //vehicle.LastUpdateBy = userID;
                            //vehicle.LastUpdateDate = DateTime.Now;
                            
                            //end by irfan


                            decimal? alocation = vehicle.Alocation + modelColour.Quantity;
                            decimal? endingOH = (vehicle.BeginningOH + vehicle.QtyIn) - vehicle.QtyOut;
                            decimal? endingAV = ((vehicle.BeginningAV + vehicle.QtyIn) - alocation) - vehicle.QtyOut;
                            if (endingAV < 0)
                            {
                                msg = "Untuk kendaraan dengan Model= " + modelColour.SalesModelCode + " dan Warna= " + modelColour.ColourCode
                                    + " tidak mempunyai cukup available unit";
                                result = false;
                            }else{
                                if (independent || !otom)
                                {
                                    Qry = "WITH TmpUpdate AS ( SELECT TOP 1 * FROM "
                                     + "OmTrInventQtyVehicle a WHERE a.CompanyCode='" + CompanyCode + "' AND a.BranchCode='" + BranchCode + "' AND a.SalesModelCode='" + modelColour.SalesModelCode +
                                     "' AND a.SalesModelYear='" + modelColour.SalesModelYear + "' AND a.ColourCode='" + modelColour.ColourCode + "' AND a.WarehouseCode='" + record.WareHouseCode +
                                     "' ORDER BY a.Year DESC, a.Month DESC )  UPDATE TmpUpdate  SET Alocation='" + alocation + "', EndingOH='" + endingOH + "', EndingAV='" + endingAV + "', LastUpdateBy='" + userID + "', LastUpdateDate=getdate()";
                                }
                                else
                                {
                                    Qry = "WITH TmpUpdate AS ( SELECT TOP 1 * FROM "
                                         + dbMD + "..OmTrInventQtyVehicle a WHERE a.CompanyCode='" + CompanyMD + "' AND a.BranchCode='" + UnitBranchMD + "' AND a.SalesModelCode='" + modelColour.SalesModelCode +
                                         "' AND a.SalesModelYear='" + modelColour.SalesModelYear + "' AND a.ColourCode='" + modelColour.ColourCode + "' AND a.WarehouseCode='" + record.WareHouseCode +
                                         "' ORDER BY a.Year DESC, a.Month DESC )  UPDATE TmpUpdate  SET Alocation='" + alocation + "', EndingOH='" + endingOH + "', EndingAV='" + endingAV + "', LastUpdateBy='" + userID + "', LastUpdateDate=getdate()";
                                }

                                //try
                                //{
                                    result = ctx.Database.ExecuteSqlCommand(Qry) > 0;
                                    //result = true;
                                //}
                                //catch
                                //{
                                //    result = false; break;
                                //}
                            }
                                
                            //if (ctxMD.SaveChanges() <= 0)
                            //    msg = "Untuk kendaraan dengan Model= " + modelColour.SalesModelCode + " dan Warna= " + modelColour.ColourCode
                            //         + " tidak ada terupdate di Stock Inventory";
                            //else
                            //    result = true;
                        }
                    }

                    if (result)
                    {
                        if (UpdateBankBook(ctx, SONo, record.CustomerCode, profitcenter, false))
                        {
                            if (ctx.SaveChanges() >= 0)
                            {
                                if (ProductType == "4W")
                                {
                                    foreach (OWSalesModel row in additionalOwnership)
                                        UpdateModelOwnerShip(SONo, row.SalesModelCode, Convert.ToInt16(row.SalesModelYear), row.StatusVehicle, row.BrandCode, row.ModelName);
                                }

                                if (islinkITS)
                                {
                                    if (record.ProspectNo != "")
                                    {
                                        //if (!Update4ITSAdditional(ctx, Convert.ToInt32(record.ProspectNo), user.CompanyCode, user.BranchCode, user.UserId, "SPK", additionalOwnership))
                                        //    throw new Exception("Update ITS Gagal");
                                    }
                                }
                            }
                            else
                            {
                                msg = "Update SO Gagal";
                                result = false;
                            }
                        }
                        else
                        {
                            msg = "Update Bank Book Gagal";
                            result = false;
                        }
                    }
                    else
                    {
                        if (msg == "") msg = "Approve SO Gagal";
                        result = false;
                    }
                    
                    if (result && !independent && otom) //Insertsdmovement
                    {
                        ctx.omTrSalesSOVins.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.SONo == record.SONo)
                        .ToList().ForEach( y => {
                            if (result)
                            {
                                Qry = "INSERT INTO " + dbMD + "..omSDMovement(CompanyCode, BranchCode, DocNo, DocDate, Seq, SalesModelCode, SalesModelYear, ChassisCode, ChassisNo, EngineCode," +
                                      "EngineNo, ColourCode, WarehouseCode, CustomerCode, QtyFlag, CompanyMD, BranchMD, WarehouseMD, Status, ProcessStatus, ProcessDate, CreatedBy," +
                                      "CreatedDate, LastUpdateBy, LastUpdateDate) Values('" +
                                      record.CompanyCode + "','" + record.BranchCode + "','" + record.SONo + "','" + record.SODate + "','" + y.SOSeq + "','" + y.SalesModelCode +
                                      "','" + y.SalesModelYear + "','" + y.ChassisCode + "','" + y.ChassisNo + "','" + y.EngineCode + "','" + y.EngineNo +
                                      "','" + y.ColourCode + "','" + record.WareHouseCode + "','" + record.CustomerCode + "','-','" + CompanyMD + "','" + UnitBranchMD + "','" + WarehouseMD +
                                      "','" + record.Status + "','0','" + DateTime.Now + "','" + CurrentUser.UserId + "','" + DateTime.Now + "','" + CurrentUser.UserId + "','" + DateTime.Now + "')";
                                try
                                {
                                    ctx.Database.ExecuteSqlCommand(Qry);
                                    //update mstVehicle
                                    var oVhcl = ctx.OmMstVehicles.Find(CompanyCode, y.ChassisCode, y.ChassisNo);
                                    if (oVhcl == null)
                                    {
                                        Qry = "UPDATE " + dbMD + "..omMstVehicle SET status='3', SoNo='" + y.SONo + "', LastUpdateBy='" + CurrentUser.UserId + "', LastUpdateDate='" + DateTime.Now +
                                              "' WHERE CompanyCode='" + CompanyMD + "' AND ChassisCode='" + y.ChassisCode + "' AND ChassisNo='" + y.ChassisNo + "'";
                                    }
                                    else
                                    {
                                        Qry = "UPDATE omMstVehicle SET status='3', SoNo='" + y.SONo + "', LastUpdateBy='" + CurrentUser.UserId + "', LastUpdateDate='" + DateTime.Now +
                                              "' WHERE CompanyCode='" + CompanyCode + "' AND ChassisCode='" + y.ChassisCode + "' AND ChassisNo='" + y.ChassisNo + "'";
                                    }

                                    try
                                    {
                                        ctx.Database.ExecuteSqlCommand(Qry);
                                        result = true;
                                    }
                                    catch
                                    {
                                        msg = "Insert omSDMovement gagal!";
                                        result = false;
                                    }
                                }
                                catch
                                {
                                    msg = "Insert omSDMovement gagal!";
                                    result = false;
                                }
                            }
                        });
                    }

                    if (result && !independent && otom)
                    {
                        int seq = 0;
                        var qry = String.Empty;
                        decimal? CostPrice = 0;
                        decimal? CostPriceMD = 0;
                        decimal? RetailPriceMD = 0;
                        decimal? RetailPriceIncTaxMD = 0;

                        decimal? pct = 100;

                        ctx.OmTrSalesSOAccsSeqs.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.SONo == record.SONo).ToList()
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
                                        CompanyCode, BranchCode, y.SONo, DateTime.Now, y.PartNo, seq, WarehouseMD
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

                    if (result)
                    {
                        //ctx.Database.ExecuteSqlCommand(string.Format("exec uspfn_omsdmovementsoinsert {0},{1},'{2}'", companyCode, branchCode, record.SONo));
                        tranScope.Complete();
                        
                        OmTRSalesSO record2 = OmTrSalesSO.GetRecord(SONo);
                        return Json(new { success = true, message = "Proses Approve SO berhasil.", data = record2 });
                    }
                    else
                    {
                        if (msg == "") { msg = "Terdapat error silahkan hubungi SDMS Support!"; }
                        return Json(new { success = false, message = msg });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                    //message = "Approved Sales Order tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support",
                    //error_log = ex.Message
                });
            }
            return Json(resultto);
        }

        public JsonResult checkBottomPrice(string SONo)
        {
            bool status = false;
            string message = "";
            decimal? minStaff = 0;

            var SO = ctx.OmTRSalesSOs.Find(CompanyCode, BranchCode, SONo);

            if (SO == null)
            {
                return Json(new { success = status, message = "Data SO tidak di temukan!" });
            }

            ctx.OmTrSalesSOModels.Where(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.SONo == SONo).ToList()
                .ForEach(y =>
            {
                var sell = ctx.OmMstPricelistSells.Find(CompanyCode, BranchCode, SO.GroupPriceCode, y.SalesModelCode, y.SalesModelYear);
                if (sell != null)
                {
                    minStaff = sell.TotalMinStaff;
                    if(y.AfterDiscTotal < minStaff)
                    {
                        var sql = @"select COUNT(UserID) from gnMstApproval
                                    where CompanyCode = '" + CompanyCode + "' and BranchCode = '" + BranchCode +
                                    "' and DocumentType = 'SOR' and UserID = '" + CurrentUser.UserId + "' and IsActive = '1'";
                        Int32 Result = ctx.Database.SqlQuery<Int32>(sql).FirstOrDefault();

                        if (Result > 0 )
                        {
                            status = true;
                            return;
                        }
                        else
                        {
                            message += "Total harga model " + y.SalesModelCode +" lebih kecil dari harga batas bawah!";
                            status = false;
                            return;
                        }
                    }
                    else
                    {
                        status = true;
                        return;
                    }
                }
                else
                {
                    message = "Harga jual model " + y.SalesModelCode + " tidak ada!";
                    status = false;
                    return;
                } 
            });

            return Json(new { success = status, message = message });
        }

        public JsonResult CheckApproval() {
            var sql = @"select COUNT(UserID) from gnMstApproval
                    where CompanyCode = '" + CompanyCode + "' and BranchCode = '" + BranchCode + 
                    "' and DocumentType = 'SOR' and UserID = '" + CurrentUser.UserId + "' and IsActive = '1'";

            int Result = ctx.Database.SqlQuery<Int32>(sql).FirstOrDefault();
            return Json(Result);
        }

        public JsonResult getMaxQty(string SalesModelCode, string SalesModelYear, string PONo, string ColourCode, string NOPlg)
        {
//            string Qry = String.Format(@"SELECT ISNULL(SUM(b.Quantity),0) FROM omTrSalesSO a  
//                                        INNER JOIN omTrSalesSOModelColour b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode 
//                                        AND b.SONo = a.SONo WHERE a.RefferenceNo='{2}' AND a.CompanyCode='{0}' AND a.BranchCode='{1}' AND 
//                                        b.SalesModelCode='{3}' AND b.SalesModelYear='{4}' AND b.ColourCode='{5}'"
//                                        ,CompanyCode, BranchCode, PONo, SalesModelCode, SalesModelYear, ColourCode);
            string CompanySD = getCompanySD(NOPlg);
            string DbSD = getDbSD(CompanySD, NOPlg);
            string Qry = String.Format(@"SELECT (a.Quantity - (SELECT ISNULL(SUM(d.Quantity),0) FROM omTrSalesSO c  
                                    INNER JOIN omTrSalesSOModelColour d ON d.CompanyCode = c.CompanyCode AND d.BranchCode = c.BranchCode AND d.SONo = c.SONo WHERE c.RefferenceNo=a.PONo AND 
                                    c.CompanyCode='{0}' AND c.BranchCode='{1}' AND d.SalesModelCode=a.SalesModelCode AND d.SalesModelYear=a.SalesModelYear AND d.ColourCode=a.ColourCode)) AS Quantity 
                                    FROM SBTSBY..omTrPurchasePOModelColour a 
                                    WHERE a.CompanyCode='{4}' AND a.BranchCode='{5}' AND a.PONo='{6}' AND a.SalesModelCode='{2}' AND a.SalesModelYear='{3}' AND a.ColourCode='{7}'"
                                    , CompanyCode, BranchCode, SalesModelCode, SalesModelYear, CompanySD, NOPlg, PONo, ColourCode);


            Decimal Result = ctx.Database.SqlQuery<Decimal>(Qry).FirstOrDefault();
            return Json(new { success = true, data = Result });
        }

        public JsonResult getQtyITS(int NoIts)
        {
            int? qty = ctx.PmKdps.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.InquiryNumber == NoIts).FirstOrDefault().QuantityInquiry;
            if (qty == null) { qty = 0; };
            return Json(new { success = true, qty = qty });
        }

        public JsonResult getRemarkPO(string PONo, string NOPlg)
        {
            string remark = "";
            string CompanySD = ctx.Database.SqlQuery<string>("SELECT CompanyCode FROM gnmstcompanymapping WHERE BranchCode='" + NOPlg + "'").FirstOrDefault();
            string dbSD = ctx.Database.SqlQuery<string>("SELECT DbName FROM gnmstcompanymapping WHERE CompanyCode='" + CompanySD + "' AND BranchCode='" + NOPlg + "'").FirstOrDefault();
            if (dbSD != null || dbSD != "")
            {
                remark = ctx.Database.SqlQuery<string>("SELECT remark FROM "+ dbSD + "..omTrpurchasePO WHERE CompanyCode='" + CompanySD + "' AND BranchCode='" + NOPlg + "' AND PONo='" + PONo + "'").FirstOrDefault();
            }
            return Json(new { success = true, data = remark });
        }

        //public JsonResult TestOtom()
        //{
        //    return Json(cekOtomatis());
        //}


        //public JsonResult ApproveSO(string SONo, bool islinkITS, List<OWSalesModel> additionalOwnership) 
        //{
        //    ResultModel resultto = InitializeResult();
        //    bool result = false;
        //    var json = Json(new { });
        //    var msg = "";
        //    var userID = CurrentUser.UserId;
        //    var OmTrSalesSO = new OmTrSalesSOBLL(ctx, userID);
        //    var OmTrSalesSOVin = new OmTrSalesSOVinBLL(ctx, userID);
        //    OmTRSalesSO record = OmTrSalesSO.GetRecord(SONo); 
        //    string companyCode = record.CompanyCode;
        //    string branchCode = record.BranchCode;
        //    string companymd = CompanyCodeMD;
        //    string unitbranch = UnitBranchCodeMD;
        //    string profitcenter = ProfitCenter;
        //    string warehouseCode = record.WareHouseCode;
        //    List<OmTrSalesSOModelColour> listModelColour = ctx.OmTrSalesSOModelColours.Where(p => p.CompanyCode == companyCode && p.BranchCode == branchCode && p.SONo == SONo).ToList();
        //    List<omTrSalesSOVin> dt = ctx.omTrSalesSOVins.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.SONo == SONo && p.ChassisNo != 0).ToList();
        //    foreach (var row in additionalOwnership)
        //    {
        //        if (row.StatusVehicle != "A")
        //        {
        //            if (row.BrandCode == null)
        //            {
        //                resultto.message = "Approved gagal" + "\n" + "Silahkan isi tipe / model terlebih dahulu untuk setiap kendaraan";
        //                return Json(resultto);
        //            }
        //        }
        //    }

        //    try
        //    {
        //        using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
        //          new TransactionOptions()))
        //        {
        //            if (record == null)
        //                return Json(resultto);
        //            else
        //            {
        //                record.ApproveBy = userID;
        //                record.ApproveDate = DateTime.Now;
        //                record.LastUpdateDate = DateTime.Now;
        //                record.LastUpdateBy = userID;
        //                record.Status = "2";
        //            }

        //            foreach (var row in dt)
        //            {
        //                //var oOmMstVehicle = ctx.OmMstVehicles.Where(p => p.ChassisCode == row.ChassisCode && p.ChassisNo == row.ChassisNo).FirstOrDefault();
        //                //var oOmMstVehicle = ctxMD.OmMstVehicles.Where(p => p.ChassisCode == row.ChassisCode && p.ChassisNo == row.ChassisNo).FirstOrDefault();
        //                string oOmMstVehicle = ctx.Database.SqlQuery<string>("select status from BAT_UAT.dbo.OmMstVehicle where ChassisCode=@p0 and ChassisNo=@p1"
        //                       , row.ChassisCode, row.ChassisNo).FirstOrDefault();
        //                if (oOmMstVehicle != null)
        //                {
        //                    if (oOmMstVehicle != "0")
        //                        msg = "Untuk kendaraan dengan ChassisCode= " + row.ChassisCode + " dan ChassisNo= " + row.ChassisNo.ToString()
        //                            + " tidak berstatus Ready";
        //                    bool updateMstVehicle = ctx.Database.SqlQuery<bool>("exec sp_updateOmMstVehicleSO @CompanyCode=@p0, @BranchCode=@p1, @ChassisCode=@p2,@ChassisNo=@p3,@SONO=@p4, @UserId=@p5"
        //                        , companymd, unitbranch, row.ChassisCode, row.ChassisNo, record.SONo, userID).FirstOrDefault();

        //                    if (!updateMstVehicle)
        //                    {
        //                        msg = "Untuk kendaraan dengan ChassisCode= " + row.ChassisCode + " dan ChassisNo= " + row.ChassisNo.ToString()
        //                           + " tidak ada terupdate di Master Kendaraan";
        //                    }
        //                    else result = true;

        //                    //OmMstVehicle oOmMstVehicleDao = new OmMstVehicle();
        //                    //oOmMstVehicle.Status = "3";
        //                    //oOmMstVehicle.SONo = record.SONo;
        //                    //oOmMstVehicle.LastUpdateBy = userID;
        //                    //oOmMstVehicle.LastUpdateDate = DateTime.Now;
        //                    //Helpers.ReplaceNullable(oOmMstVehicle);
        //                    //if (ctxMD.SaveChanges() >= 0)
        //                    //    msg = "Untuk kendaraan dengan ChassisCode= " + oOmMstVehicle.ChassisCode + " dan ChassisNo= " + oOmMstVehicle.ChassisNo.ToString()
        //                    //        + " tidak ada terupdate di Master Kendaraan";
        //                    //else result = true;
        //                }
        //                else
        //                    msg = "Untuk kendaraan ini belum menjadi stock";
        //            }

        //            foreach (OmTrSalesSOModelColour modelColour in listModelColour)
        //            {
        //                //List<OmTrInventQtyVehicle> vehicle = getVehicle(CompanyCode, BranchCode,modelColour.SalesModelCode, modelColour.SalesModelYear, modelColour.ColourCode, warehouseCode);
        //                //var vehicle = ctxMD.OmTrInventQtyVehicles.Where(p =>
        //                //    p.CompanyCode == companymd &&
        //                //    p.BranchCode == unitbranch &&
        //                //    p.SalesModelCode == modelColour.SalesModelCode &&
        //                //    p.SalesModelYear == modelColour.SalesModelYear &&
        //                //    p.ColourCode == modelColour.ColourCode &&
        //                //    p.WarehouseCode == warehouseCode).FirstOrDefault();
        //                string vehicle = ctx.Database.SqlQuery<string>("select SalesModelCode from BAT_UAT.dbo.OmTrInventQtyVehicle where CompanyCode=@p0 and BranchCode=@p1 and SalesModelCode=@p2 and SalesModelYear=@p3 and ColourCode=@p4 and WareHousecode = @p5 "
        //                       ,companymd, unitbranch, modelColour.SalesModelCode, modelColour.SalesModelYear, modelColour.ColourCode, warehouseCode).FirstOrDefault();
        //                if (vehicle != null)
        //                {
        //                    int updateTrInventQtyVehicle = ctx.Database.SqlQuery<int>("exec sp_updateOmTrInventQtyVehicleSO @CompanyCode=@p0, @BranchCode=@p1, @SalesModelCode=@p2,@SalesModelYear=@p3,@ColourCode=@p4, @WarehouseCode=@p5, @Quantity=@p6, @UserId=@p7 "
        //                        , companymd, unitbranch, modelColour.SalesModelCode, modelColour.SalesModelYear, modelColour.ColourCode, warehouseCode, modelColour.Quantity, userID).FirstOrDefault();

        //                    if (updateTrInventQtyVehicle == 1) 
        //                    {
        //                        msg = "Untuk kendaraan dengan Model= " + modelColour.SalesModelCode + " dan Warna= " + modelColour.ColourCode
        //                            + " tidak mempunyai cukup available unit";
        //                    }
        //                    else if (updateTrInventQtyVehicle == 0) {
        //                        msg = "Untuk kendaraan dengan Model= " + modelColour.SalesModelCode + " dan Warna= " + modelColour.ColourCode
        //                            + " tidak ada terupdate di Stock Inventory";
        //                    }
        //                    else result = true;

        //                    //vehicle.Alocation = vehicle.Alocation + modelColour.Quantity;
        //                    //vehicle.EndingOH = vehicle.BeginningOH + vehicle.QtyIn - vehicle.QtyOut;
        //                    //vehicle.EndingAV = vehicle.BeginningAV + vehicle.QtyIn - vehicle.Alocation - vehicle.QtyOut;
        //                    //decimal i = CalculateQuantitySO(ctx, companyCode, branchCode, modelColour.SONo, modelColour.SalesModelCode, modelColour.SalesModelYear, modelColour.ColourCode, warehouseCode);
        //                    //if (vehicle.EndingAV < 0)
        //                    //    msg = "Untuk kendaraan dengan Model= " + modelColour.SalesModelCode + " dan Warna= " + modelColour.ColourCode
        //                    //        + " tidak mempunyai cukup available unit";

        //                    //vehicle.LastUpdateBy = userID;
        //                    //vehicle.LastUpdateDate = DateTime.Now;

        //                    //if (ctxMD.SaveChanges() <= 0)
        //                    //   msg = "Untuk kendaraan dengan Model= " + modelColour.SalesModelCode + " dan Warna= " + modelColour.ColourCode
        //                    //        + " tidak ada terupdate di Stock Inventory";
        //                    //else
        //                    //    result = true;
        //                }
        //            }

        //            if (result)
        //            {
        //                if (UpdateBankBook(SONo, record.CustomerCode, profitcenter, false))
        //                {
        //                    if (ctx.SaveChanges() >= 0)
        //                    {
        //                        if (ProductType == "4W")
        //                        {
        //                            foreach (OWSalesModel row in additionalOwnership)
        //                                UpdateModelOwnerShip(SONo, row.SalesModelCode, Convert.ToInt16(row.SalesModelYear), row.StatusVehicle, row.BrandCode, row.ModelName);
        //                        }

        //                        if (islinkITS)
        //                        {
        //                            if (record.ProspectNo != "")
        //                            {
        //                                //if (!Update4ITSAdditional(ctx, Convert.ToInt32(record.ProspectNo), user.CompanyCode, user.BranchCode, user.UserId, "SPK", additionalOwnership))
        //                                //    throw new Exception("Update ITS Gagal");
        //                            }
        //                        }
        //                    }
        //                    else msg = "Update SO Gagal";
        //                }
        //                else msg = "Update Bank Book Gagal";
        //            }
        //            else msg = "Approve SO Gagal";
        //            if (result)
        //            {
        //                OmTrSalesBLL.Instance(CurrentUser.UserId).SDMovementSO(record);
        //                tranScope.Complete();
                        
        //                OmTRSalesSO record2 = OmTrSalesSO.GetRecord(SONo);  
        //                return Json(new { success = true, message = "Proses Approve SO berhasil.", data = record2 });
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = msg });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = "Approved Sales Order tidak berhasil. Periksa kembali inputan anda atau hubungi SDMS support",
        //            error_log = ex.Message
        //        });
        //    }
        //    return Json(resultto);
        //}
    }
   
}
