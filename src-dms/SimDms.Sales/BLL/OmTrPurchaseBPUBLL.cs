using SimDms.Common;
using SimDms.Sales.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Sales.BLL
{
    public class OmTrPurchaseBPUBLL : BaseBLL
    {
        public enum status { OPEN = 0, PRINTED = 1, CLOSED = 2, CANCELED = 3, FINISHED = 9 };
        public enum bpuTipe { DO = 0, SJ = 1, DO_SJ = 2, SJ_BOOKING = 3 };

        #region -- Initiate --
        public OmTrPurchaseBPUBLL(DataContext _ctx, string _username)
        {
            this.ctx = _ctx;

             //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
        }
        #endregion

        #region -- Public Method --
        public omTrPurchaseBPU GetRecord(string pONo, string bPUNo)
        {
            var record = ctx.omTrPurchaseBPU.Find(CompanyCode, BranchCode, pONo, bPUNo);
            return record;
        }

        public OmTrPurchaseBPULookupView GetRecordView(string pONo, string bPUNo)
        {
            string sql = string.Format(@"
                    select * from OmTrPurchaseBPUView
                    where CompanyCode = '{0}'
                    and BranchCode = '{1}'
                    and PONo = '{2}'                    
                    and BPUNo = '{3}'
                ",CompanyCode, BranchCode, pONo, bPUNo);
            var record = ctx.Database.SqlQuery<OmTrPurchaseBPULookupView>(sql.Trim()).FirstOrDefault();
            sql = null;

            return record;
        }

        public IQueryable<OmTrPurchaseBPULookupView> Select4LookUp()
        {
            var records = ctx.OmTrPurchaseBPULookupView.Where(p => p.CompanyCode == CompanyCode 
                && p.BranchCode == BranchCode)
                .OrderByDescending(p => p.BPUNo);
            
            return records;
        }

        public IEnumerable<OmSelectReffSJTrueViewLookup> SelectReffSJ(bool isSJ)
        {
            if (isSJ)
            {
                string sql = string.Format(@"SELECT * FROM OmSelectReffSJTrueView WHERE CompanyCode = '{0}' AND BranchCode = '{1}'
                        ORDER BY FlagRevisi Desc, BatchNo, SJNo, SKPNo",CompanyCode, BranchCode);
                var records = ctx.Database.SqlQuery<OmSelectReffSJTrueViewLookup>(sql.Trim());

                return records;
            }
            else
            {
                string sql = string.Format(@"SELECT * FROM OmSelectReffSJFalseView WHERE CompanyCode = '{0}' AND BranchCode = '{1}'",
                    CompanyCode, BranchCode);
                var records = ctx.Database.SqlQuery<OmSelectReffSJTrueViewLookup>(sql.Trim());

                return records;
            }
        }

        public IEnumerable<OmSelectReffSJBookingViewLookup> SelectReffSJBooking()
        {
            string sql = string.Format(@"SELECT * FROM OmSelectReffSJBookingView WHERE CompanyCode = '{0}' AND BranchCode = '{1}'",
                CompanyCode, BranchCode);
            var records = ctx.Database.SqlQuery<OmSelectReffSJBookingViewLookup>(sql.Trim());

            return records;
        }

        public IQueryable<omTrPurchaseBPU> select4BPU(string PONo, string DONo)
        {
            var records = ctx.omTrPurchaseBPU.Where(
                p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode
                && p.PONo == PONo
                && p.RefferenceDONo == DONo
            );
            
            return records;
        }

        public IEnumerable<OmSelectReffNoViewLookup> SelectReffNo()
        {
            string sql = string.Format(@"SELECT * FROM OmSelectReffNoView WHERE CompanyCode = '{0}' AND BranchCode = '{1}'",
                CompanyCode, BranchCode);
            var records = ctx.Database.SqlQuery<OmSelectReffNoViewLookup>(sql.Trim());

            return records;
        }

        /// <summary>
        /// check the existance of ReffDONo
        /// </summary>
        /// <param name="reffDONo"></param>
        /// <returns></returns>
        public bool IsExistDONo(string reffDONo)
        {
            var record = ctx.omTrPurchaseBPU.Where(p => p.RefferenceDONo == reffDONo
                && p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                && p.Status != "3");

            if (record.Count() > 0)
                return true;
            else return false;
        }

        /// <summary>
        /// check the existance of ReffSJNo
        /// </summary>
        /// <param name="reffDONo"></param>
        /// <returns></returns>
        public bool IsExistSJNo(string reffSJNo)
        {
            var record = ctx.omTrPurchaseBPU.Where(p => p.RefferenceSJNo == reffSJNo
                && p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                && p.Status != "3");

            if (record.Count() > 0)
                return true;
            else return false;
        }

        public bool Save(omTrPurchaseBPU record, bool isNew)
        {
            bool result = false;
            try
            {
                if (isNew)
                {
                    record.BPUNo = GetNewDocumentNo(GnMstDocumentConstant.BPU, Convert.ToDateTime(record.BPUDate));
                    if (record.BPUNo.EndsWith("X")) throw new Exception(string.Format(GetMessage(SysMessages.MSG_5046), "BPU"));
                }
                result = (isNew) ? p_Insert(record) > 0 : p_Update() >= 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        public int Update()
        {
            return p_Update();
        }
        /// <summary>
        /// Whole DO Process from uploaded file
        /// </summary>
        /// <param name="record"></param>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public bool SaveUpload(omTrPurchaseBPU record, string batchNo)
        {
            bool result = false;
            string DONo = record.RefferenceDONo;
            string PONo = record.PONo;
            try
            {
                record.BPUNo = GetNewDocumentNo(GnMstDocumentConstant.BPU, (DateTime)record.BPUDate);
                if (record.BPUNo.EndsWith("X")) throw new ApplicationException(
                    string.Format(GetMessage(SysMessages.MSG_5046), "BPU"));

                //here, we save everything
                if (p_Insert(record) > 0)
                    if (p_InsertDetail(DONo, PONo, record.BPUNo, batchNo))
                        if (p_InsertDetailModel(DONo, PONo, record.BPUNo, batchNo))
                            result = true;
            
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        public bool SaveSJUpload(omTrPurchaseBPU record, string batchNo, ref string errMsg)
        {
            bool result = false;
            string SJNo = record.RefferenceSJNo;
            //string PONo = record.PONo;
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            try
            {
                //here, we save everything
                errMsg = "";
                Helpers.ReplaceNullable(record);
                if (ctx.SaveChanges() > 0)
                    if (p_InsertSJDetail(SJNo, batchNo, record.PONo, record.BPUNo, ref errMsg))
                        result = true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
          
            return result;
        }

        public int DeleteRecord(string poNo, string bpuNo)
        {
            int result = -1;
            try
            {
                var rec = ctx.omTrPurchaseBPUDetail.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                    && p.PONo == poNo && p.BPUNo == bpuNo && (p.StatusHPP == "1" || p.isReturn == true || p.StatusSJRel == "9"));

                if (rec != null && rec.Count() > 0)
                    result = -1;
                else result = 0;

                if (result > -1)
                {
                    var listString = "0,1".Split(',');
                    var recDetail = ctx.omTrPurchaseBPUDetail.Where(p => p.CompanyCode ==  CompanyCode && p.BranchCode == BranchCode
                        && p.PONo == poNo && p.BPUNo == bpuNo && p.StatusHPP == "0" && p.isReturn == false && listString.Contains(p.StatusHPP));
                    var recDtl = recDetail.ToList();
                    recDetail = null;

                    if (recDtl != null && recDtl.Count() > 0)
                    {
                        var oDtl = ctx.omTrPurchaseBPUDetail.Where(p => p.CompanyCode ==  CompanyCode && p.BranchCode == BranchCode
                            && p.PONo == poNo && p.BPUNo == bpuNo && p.StatusHPP == "0" && p.isReturn == false && listString.Contains(p.StatusHPP))
                            .FirstOrDefault();
                        ctx.omTrPurchaseBPUDetail.Remove(oDtl);
                        result = ctx.SaveChanges();
                        
                        if (result > -1)
                        {
                            foreach (omTrPurchaseBPUDetail recBPU in recDtl)
                            {
                               var recordBPUModel = ctx.OmTrPurchaseBPUDetailModels.Where(p => p.CompanyCode == CompanyCode 
                                   && p.BranchCode == BranchCode && p.PONo == poNo && p.BPUNo == bpuNo && p.SalesModelCode == recBPU.SalesModelCode
                                   && p.SalesModelYear == recBPU.SalesModelYear).FirstOrDefault();
                                if(recordBPUModel != null){
                                    recordBPUModel.QuantityBPU = (recordBPUModel.QuantityBPU == null) ? 0 : recordBPUModel.QuantityBPU;
                                    recordBPUModel.QuantityBPU = recordBPUModel.QuantityBPU -1;
                                    
                                    Helpers.ReplaceNullable(recordBPUModel);
                                    result = ctx.SaveChanges();
                                }
                                if (result > -1)
                                {
                                    string bpuType = "";
                                    omTrPurchaseBPU oOmTrPurchaseBPU = GetRecord(poNo, bpuNo);
                                    if (oOmTrPurchaseBPU != null)
                                        bpuType = oOmTrPurchaseBPU.BPUType;

                                    // Jika user 2W abaikan SalesModelYear
                                    decimal salesModelYear = (decimal)recBPU.SalesModelYear;
                                    if (ProductType.Equals("2W") && bpuType.Equals("2"))
                                    {
                                        var recPOModel = ctx.OmTrPurchasePOModels.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode 
                                            && p.PONo == poNo && p.SalesModelCode == recBPU.SalesModelCode);
                                            
                                        if (recPOModel.Count() > 0)
                                            salesModelYear = recPOModel.FirstOrDefault().SalesModelYear;
                                    }

                                    var objPOModel = ctx.OmTrPurchasePOModels.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                                        && p.PONo == poNo && p.SalesModelCode == recBPU.SalesModelCode && p.SalesModelYear == salesModelYear)
                                        .FirstOrDefault();
                                    if(objPOModel != null){
                                        objPOModel.QuantityBPU = objPOModel.QuantityBPU ?? 0;
                                        objPOModel.QuantityBPU = objPOModel.QuantityBPU -1;

                                        Helpers.ReplaceNullable(objPOModel);
                                        result = ctx.SaveChanges();
                                    }
                                    if (result < 0)
                                        throw new Exception("Gagal update PO Model");
                                }
                                else
                                    throw new Exception("Gagal update BPU Detail Model");
                            }
                        }
                        else
                            throw new Exception("Gagal delere BPU Detail");
                    }

                    if (result > -1)
                    {
                        var objBPU = ctx.omTrPurchaseBPU.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                            && p.PONo == poNo && p.BPUNo == bpuNo).FirstOrDefault();
                        objBPU.Status = "3";

                        Helpers.ReplaceNullable(objBPU);
                        result = ctx.SaveChanges();
                    }
                    else
                        throw new Exception("Gagal update BPU header");
                }
                else
                    throw new Exception("Data sudah HPP atau sudah di Purchase Return");
            }
            catch (Exception ex)
            {
                result = -1;
            }
            
            return result;
        }
        #endregion

        #region -- Private Method --
        private int p_Insert(omTrPurchaseBPU record)
        {
            ctx.omTrPurchaseBPU.Add(record);

            Helpers.ReplaceNullable(record);
            return ctx.SaveChanges();
        }

        private int p_Update()
        {
            return ctx.SaveChanges();
        }
        
        //private static bool isExistsChassisNo(string chassisCode, string chassisNo)
        //{
        //    SysUser user = SysUser.Current;
        //    bool isExists = false;
        //    isExists = OmTrPurchaseBPUDetailBLL.CheckChassisNo(user.CompanyCode, chassisCode, chassisNo);
        //    return isExists;
        //}

        private bool p_InsertDetail(string DONo, string PONo, string BPUNo, string batchNo)
        {
            bool result = false;
            var TrPurchaseBPUDetailBLL = new OmTrPurchaseBPUDetailBLL(ctx, CurrentUser.UserId);
            var OmUtlSDORDDtl3s = p_GetDetailFalse(batchNo, DONo);
            var listOmUtlSDORDDtl3s = OmUtlSDORDDtl3s.ToList();
            OmUtlSDORDDtl3s = null;

            if (listOmUtlSDORDDtl3s.Count() > 0)
            {
                foreach (OmUtlSDORDDtl3 rec in listOmUtlSDORDDtl3s)
                {
                    omTrPurchaseBPUDetail BPUDetail = new omTrPurchaseBPUDetail();
                    BPUDetail.CompanyCode = CompanyCode;
                    BPUDetail.BranchCode = BranchCode;
                    BPUDetail.PONo = PONo;
                    BPUDetail.BPUNo = BPUNo;
                    BPUDetail.BPUSeq = p_GetSeq(BPUNo, PONo);
                    BPUDetail.SalesModelCode = rec.SalesModelCode;
                    BPUDetail.SalesModelYear = rec.SalesModelYear;
                    BPUDetail.ColourCode = rec.ColourCode;
                    BPUDetail.ChassisCode = rec.ChassisCode;
                    BPUDetail.ChassisNo = rec.ChassisNo;
                    BPUDetail.EngineCode = rec.EngineCode;
                    BPUDetail.EngineNo = rec.EngineNo;
                    BPUDetail.ServiceBookNo = rec.ServiceBookNo;
                    BPUDetail.KeyNo = rec.KeyNo;
                    BPUDetail.Remark = "";
                    BPUDetail.StatusSJRel = "0";
                    BPUDetail.SJRelNo = "";
                    BPUDetail.SJRelReff = "0";
                    BPUDetail.StatusDORel = "1";
                    BPUDetail.DORelNo = "";
                    BPUDetail.StatusHPP = "0";
                    BPUDetail.isReturn = false;
                    BPUDetail.CreatedBy = CurrentUser.UserId;
                    BPUDetail.CreatedDate = DateTime.Now;
                    BPUDetail.LastUpdateBy = CurrentUser.UserId;
                    BPUDetail.LastUpdateDate = DateTime.Now;

                    //if (isExistsChassisNo((string)row.ChassisCode, (string)row.ChassisNo))
                    //{
                    //    result = false; break;
                    //}
                    //else
                    //{
                    if (TrPurchaseBPUDetailBLL.Insert(BPUDetail) < 1)
                    {
                        result = false; break;
                    }
                    else
                    {
                        result = true;
                    }
                    //}
                }
            }
            TrPurchaseBPUDetailBLL = null;

            return result;
        }

        private bool p_InsertDetailModel(string DONo, string PONo, string BPUNo, string batchNo)
        {
            bool result = false;
            var omTrPurchaseBPUDetailModelBLL = new OmTrPurchaseBPUDetailModelBLL(ctx, CurrentUser.UserId);

            var utlDtl1 = ctx.OmUtlSDORDDtl1s.Find(CompanyCode, BranchCode, batchNo, DONo);
            if (utlDtl1 != null)
            {
                utlDtl1.Status = "1";
                utlDtl1.LastUpdateBy = CurrentUser.UserId;
                utlDtl1.LastUpdateDate = DateTime.Now;

                Helpers.ReplaceNullable(utlDtl1);
                if (ctx.SaveChanges() > 0)
                {
                    var OmUtlSDORDDtl2s = p_GetDetailModelFalse(batchNo, DONo);
                    var listOmUtlSDORDDtl2s = OmUtlSDORDDtl2s.ToList();
                    OmUtlSDORDDtl2s = null;

                    if (listOmUtlSDORDDtl2s.Count() > 0)
                    {
                        foreach (OmUtlSDORDDtl2 rec in listOmUtlSDORDDtl2s)
                        {
                            rec.Quantity = (rec.Quantity == null) ? 0 : rec.Quantity;

                            OmTrPurchaseBPUDetailModel BPUDetailModel = new OmTrPurchaseBPUDetailModel();
                            BPUDetailModel.CompanyCode = CompanyCode;
                            BPUDetailModel.BranchCode = BranchCode;
                            BPUDetailModel.PONo = PONo;
                            BPUDetailModel.BPUNo = BPUNo;
                            BPUDetailModel.SalesModelCode = rec.SalesModelCode;
                            BPUDetailModel.SalesModelYear = rec.SalesModelYear;
                            BPUDetailModel.QuantityBPU = rec.Quantity.Value;
                            BPUDetailModel.QuantityHPP = 0;
                            BPUDetailModel.CreatedBy = CurrentUser.UserId;
                            BPUDetailModel.CreatedDate = DateTime.Now;
                            BPUDetailModel.LastUpdateBy = CurrentUser.UserId;
                            BPUDetailModel.LastUpdateDate = DateTime.Now;
                            if (omTrPurchaseBPUDetailModelBLL.Insert(BPUDetailModel) < 1)
                            {
                                result = false; break;
                            }
                            else
                            {
                                if (!p_UpdatePurchasePOModel(PONo, BPUNo, BPUDetailModel.SalesModelCode, BPUDetailModel.SalesModelYear))
                                {
                                    result = false; break;
                                }
                                else
                                    result = true;
                            }
                        }
                    }
                }
            }

            omTrPurchaseBPUDetailModelBLL = null;
            return result;
        }

        private bool p_UpdatePurchasePOModel(string PONo, string BPUNo, string salesModelCode, decimal salesModelYear)
        {
            bool result = false;
            decimal poSalesModelYear = salesModelYear;
            string bpuType = "";
            var oOmTrPurchaseBPU = ctx.omTrPurchaseBPU.Find(CompanyCode, BranchCode, PONo, BPUNo);
            if (oOmTrPurchaseBPU != null)
                bpuType = oOmTrPurchaseBPU.BPUType;

            // Jika user 2W abaikan SalesModelYear
            if (ProductType.Equals("2W") && bpuType.Equals("2"))
            {
                var oOmTrPurchasePOModel = ctx.OmTrPurchasePOModels.Where(p => p.CompanyCode ==CompanyCode && p.BranchCode == BranchCode
                    && p.PONo == PONo && p.SalesModelCode == salesModelCode);
                if (oOmTrPurchasePOModel.Count() > 0)
                    poSalesModelYear = oOmTrPurchasePOModel.FirstOrDefault().SalesModelYear;
            }

            OmTrPurchasePOModel purchasePOModel = ctx.OmTrPurchasePOModels.Find(CompanyCode, BranchCode, PONo, salesModelCode, poSalesModelYear);
            if (purchasePOModel != null)
            {
                purchasePOModel.QuantityBPU = purchasePOModel.QuantityBPU ?? 0;
                var countBPUDetail = p_GetCountBPUDetail(PONo, BPUNo, salesModelCode, salesModelYear);
                countBPUDetail = (countBPUDetail == null) ? 0 : countBPUDetail;

                purchasePOModel.QuantityBPU = purchasePOModel.QuantityBPU + p_GetCountBPUDetail(PONo, BPUNo, salesModelCode, salesModelYear);
                purchasePOModel.LastUpdateBy = CurrentUser.UserId;
                purchasePOModel.LastUpdateDate = DateTime.Now;

                purchasePOModel.QuantityPO = purchasePOModel.QuantityPO ?? 0;
                Helpers.ReplaceNullable(purchasePOModel);
                if (purchasePOModel.QuantityBPU <= purchasePOModel.QuantityPO)
                    if (ctx.SaveChanges() > 0)
                        result = true;
            }
            return result;
        }

        private bool p_InsertSJDetail(string SJNo, string batchNo, string PONo, string BPUNo, ref string errMsg)
        {
            bool result = false;
            errMsg = "";
            OmUtlSSJALDtl1 utlDtl1 = ctx.OmUtlSSJALDtl1s.Find(CompanyCode, BranchCode, batchNo, SJNo);
            if (utlDtl1 != null)
            {
                utlDtl1.Status = "1";
                utlDtl1.LastUpdateBy = CurrentUser.UserId;
                utlDtl1.LastUpdateDate = DateTime.Now;

                Helpers.ReplaceNullable(utlDtl1);
                if (ctx.SaveChanges() > 0)
                {
                    var OmUtlSSJALDtl3 = ctx.OmUtlSSJALDtl3s.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                        && p.BatchNo == batchNo && p.SJNo == SJNo);
                    var OmUtlSSJALDtl3s = OmUtlSSJALDtl3.ToList();
                    OmUtlSSJALDtl3 = null;

                    if (OmUtlSSJALDtl3s.Count() > 0)
                    {
                        foreach (OmUtlSSJALDtl3 rec in OmUtlSSJALDtl3s)
                        {
                            omTrPurchaseBPUDetail bpuDetail = p_GetBPUDetail(PONo, BPUNo,rec.SalesModelCode, 
                                rec.SalesModelYear, rec.ColourCode, rec.ChassisCode, rec.EngineCode);
                            if (bpuDetail != null)
                            {
                                bpuDetail.ChassisNo = rec.ChassisNo;
                                bpuDetail.EngineNo = rec.EngineNo;
                                bpuDetail.ServiceBookNo = rec.ServiceBookNo;
                                bpuDetail.KeyNo = rec.KeyNo;
                                bpuDetail.StatusSJRel = "9";
                                bpuDetail.StatusDORel = "0";
                                bpuDetail.LastUpdateBy = CurrentUser.UserId;
                                bpuDetail.LastUpdateDate = DateTime.Now;

                                if (p_IsExistsChassisNo(rec.ChassisCode, rec.ChassisNo))
                                {
                                    result = false; break;
                                }
                                else
                                {
                                    Helpers.ReplaceNullable(bpuDetail);
                                    if (ctx.SaveChanges() < 1)
                                    {
                                        result = false; break;
                                    }
                                    else result = true;
                                }
                            }
                            else
                            {
                                errMsg = "Data Tidak dapat disimpan, gunakan manual entry \r\n";
                                errMsg += " Karena : \r\n";
                                errMsg += " - Warna tidak sama dengan DO \r\n";
                                errMsg += " - Di DO sudah tercantum No.Rangka dan No.Mesin \r\n";
                                result = false; break;
                            }
                        }
                    }
                    else result = false;
                }
            }
            return result;
        }

        private bool p_IsExistsChassisNo(string chassisCode, decimal chassisNo)
        {
            bool isExists = false;
            var omTrPurchaseBPUDetailBLL = new OmTrPurchaseBPUDetailBLL(ctx, CurrentUser.UserId);
            isExists = omTrPurchaseBPUDetailBLL.CheckChassisNo(chassisCode, chassisNo);
            return isExists;
        }

        /// <summary>
        /// Whole DO & SJ Process (from uploaded file)
        /// </summary>
        /// <param name="record"></param>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public bool SaveDOSJUpload(omTrPurchaseBPU record, string batchNo, bool isSJBooking)
        {
            bool result = false;
            string DONo = record.RefferenceDONo;
            string SJNo = record.RefferenceSJNo;
            string PONo = record.PONo;
            try
            {
                record.BPUNo = GetNewDocumentNo(GnMstDocumentConstant.BPU, Convert.ToDateTime(record.BPUDate));
                if (record.BPUNo.EndsWith("X")) throw new Exception(
                    string.Format(GetMessage(SysMessages.MSG_5046), "BPU"));

                //here, we save everything
                if (p_Insert(record) > 0)
                {
                    OmUtlSSJALDtl1 utlDtl1 = ctx.OmUtlSSJALDtl1s.Find(CompanyCode, BranchCode, batchNo, SJNo);
                    if (utlDtl1 != null)
                    {
                        utlDtl1.Status = "1";
                        utlDtl1.LastUpdateBy = CurrentUser.UserId;
                        utlDtl1.LastUpdateDate = DateTime.Now;

                        Helpers.ReplaceNullable(utlDtl1);
                        if (ctx.SaveChanges() > 0)
                        {
                            if (p_InsertDOSJDetail(SJNo, PONo, record.BPUNo, batchNo, isSJBooking))
                                if (p_InsertDOSJDetailModel(SJNo, PONo, record.BPUNo, batchNo))
                                    result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }            

            return result;
        }

        private bool p_InsertDOSJDetailModel(string SJNo, string PONo, string BPUNo, string batchNo)
        {
            bool result = false;
            var OmUtlSSJALDtl2 = p_GetDetailModelTrue(batchNo, SJNo);
            var OmUtlSSJALDtl2s = OmUtlSSJALDtl2.ToList();
            OmUtlSSJALDtl2 = null;

            if (OmUtlSSJALDtl2s.Count() > 0)
            {
                foreach (OmUtlSSJALDtl2 rec in OmUtlSSJALDtl2s)
                {
                    rec.Quantity = (rec.Quantity == null) ? 0 : rec.Quantity;
                    OmTrPurchaseBPUDetailModel BPUDetailModel = new OmTrPurchaseBPUDetailModel();
                    BPUDetailModel.CompanyCode = CompanyCode;
                    BPUDetailModel.BranchCode = BranchCode;
                    BPUDetailModel.PONo = PONo;
                    BPUDetailModel.BPUNo = BPUNo;
                    BPUDetailModel.SalesModelCode = rec.SalesModelCode;
                    BPUDetailModel.SalesModelYear = rec.SalesModelYear;
                    BPUDetailModel.QuantityBPU = rec.Quantity.Value;
                    BPUDetailModel.QuantityHPP = 0;
                    BPUDetailModel.CreatedBy = CurrentUser.UserId;
                    BPUDetailModel.CreatedDate = DateTime.Now;
                    BPUDetailModel.LastUpdateBy = CurrentUser.UserId;
                    BPUDetailModel.LastUpdateDate = DateTime.Now;
                    var omTrPurchaseBPUDetailModelBLL = new OmTrPurchaseBPUDetailModelBLL(ctx, CurrentUser.UserId);
                    if (omTrPurchaseBPUDetailModelBLL.Insert(BPUDetailModel) < 1)
                    {
                        throw new Exception("Gagal save BPU Deatil");
                    }
                    else
                    {
                        if (!p_UpdatePurchasePOModel(PONo, BPUNo, BPUDetailModel.SalesModelCode, BPUDetailModel.SalesModelYear))
                        {
                            //result = false; break;
                            throw new Exception("Gagal update PO Model");
                        }
                        else
                            result = true;
                    }
                    omTrPurchaseBPUDetailModelBLL = null;
                }
            }
            return result;
        }

        private bool p_InsertDOSJDetail(string SJNo, string PONo, string BPUNo, string batchNo, bool isSJBooking)
        {
            bool result = false;
            var OmUtlSSJALDtl3 = p_GetDetailTrue(batchNo, SJNo);
            var OmUtlSSJALDtl3s = OmUtlSSJALDtl3.ToList();
            OmUtlSSJALDtl3 = null;

            if (OmUtlSSJALDtl3s.Count() > 0)
            {
                foreach (OmUtlSSJALDtl3 rec in OmUtlSSJALDtl3s)
                {
                    OmMstVehicle oOmMstVehicle = ctx.OmMstVehicles.Find(CompanyCode, rec.ChassisCode, rec.ChassisNo);
                    if (oOmMstVehicle != null)
                        throw new Exception("Data dengan ChassisCode: " + rec.ChassisCode + " ChassisNo: " +
                            rec.ChassisNo.ToString() + " sudah di Master Vehicle");

                    omTrPurchaseBPUDetail BPUDetail = new omTrPurchaseBPUDetail();
                    BPUDetail.CompanyCode = CompanyCode;
                    BPUDetail.BranchCode = BranchCode;
                    BPUDetail.PONo = PONo;
                    BPUDetail.BPUNo = BPUNo;
                    BPUDetail.BPUSeq = p_GetSeq(BPUDetail.BPUNo, BPUDetail.PONo);
                    BPUDetail.SalesModelCode = rec.SalesModelCode;
                    BPUDetail.SalesModelYear = rec.SalesModelYear;
                    BPUDetail.ColourCode = rec.ColourCode;
                    BPUDetail.ChassisCode = rec.ChassisCode;
                    BPUDetail.ChassisNo = rec.ChassisNo;
                    BPUDetail.EngineCode = rec.EngineCode;
                    BPUDetail.EngineNo = rec.EngineNo;
                    BPUDetail.ServiceBookNo = rec.ServiceBookNo;
                    BPUDetail.KeyNo = rec.KeyNo;
                    BPUDetail.Remark = "";
                    if (isSJBooking) BPUDetail.StatusSJRel = "1";
                    else BPUDetail.StatusSJRel = "0";
                    BPUDetail.SJRelNo = "";
                    BPUDetail.SJRelReff = "0";
                    BPUDetail.StatusDORel = "0";
                    BPUDetail.DORelNo = "";
                    BPUDetail.StatusHPP = "0";
                    BPUDetail.isReturn = false;
                    BPUDetail.CreatedBy = CurrentUser.UserId;
                    BPUDetail.CreatedDate = DateTime.Now;
                    BPUDetail.LastUpdateBy = CurrentUser.UserId;
                    BPUDetail.LastUpdateDate = DateTime.Now;

                    var omTrPurchaseBPUDetailBLL = new OmTrPurchaseBPUDetailBLL(ctx, CurrentUser.UserId);
                    if (omTrPurchaseBPUDetailBLL.Insert(BPUDetail) < 1)
                    {
                        //result = false; break;
                        throw new Exception("Gagal save BPU detail: " + BPUNo);
                    }
                    else result = true;

                    omTrPurchaseBPUDetailBLL = null;
                }
            }

            return result;
        }

        private omTrPurchaseBPUDetail p_GetBPUDetail(string PONo, string BPUNo, string salesModelCode, 
            decimal salesModelYear, string colourCode, string chassisCode, string engineCode)
        {
            var record = ctx.omTrPurchaseBPUDetail.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                && p.PONo == PONo && p.BPUNo == BPUNo && p.SalesModelCode == salesModelCode && p.SalesModelYear == salesModelYear
                && p.ColourCode == colourCode && p.ChassisCode == chassisCode && p.EngineCode == engineCode && p.ChassisNo == 0 && p.EngineNo == 0);
            if (record.Count() > 0)
            {
                return record.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        private decimal p_GetCountBPUDetail(string PONo, string BPUNo, string salesModelCode, decimal salesModelYear)
        {
            var rec = ctx.omTrPurchaseBPUDetail.Where(p => p.CompanyCode == CompanyCode &&  p.BranchCode == BranchCode
                && p.PONo == PONo && p.BPUNo == BPUNo && p.SalesModelCode == salesModelCode && p.SalesModelYear == salesModelYear);
            decimal result = Convert.ToDecimal(rec.Count());
            
            return result;
        }

        private IQueryable<OmUtlSDORDDtl3> p_GetDetailFalse(string batchNo, string DONo)
        {
            IQueryable<OmUtlSDORDDtl3> records;
            var recs = ctx.OmUtlSDORDDtl3s.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                && p.BatchNo == batchNo && p.DONo == DONo);;

            records = recs;
            recs = null;

            return records;
        }

        private IQueryable<OmUtlSSJALDtl3> p_GetDetailTrue(string batchNo, string SJNo)
        {
            return ctx.OmUtlSSJALDtl3s.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                && p.BatchNo == batchNo && p.SJNo == SJNo);
        }

        private IQueryable<OmUtlSDORDDtl2> p_GetDetailModelFalse(string batchNo, string DONo)
        {
            return ctx.OmUtlSDORDDtl2s.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                 && p.BatchNo == batchNo && p.DONo == DONo);
        }

        private IQueryable<OmUtlSSJALDtl2> p_GetDetailModelTrue(string batchNo, string SJNo)
        {
            return ctx.OmUtlSSJALDtl2s.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                 && p.BatchNo == batchNo && p.SJNo == SJNo);
        }

        private int p_GetSeq(string BPUNo, string PONo)
        {
            int No = 0;
            var records = ctx.omTrPurchaseBPUDetail.Where(p => p.CompanyCode == CompanyCode
                && p.BranchCode == BranchCode && p.BPUNo == BPUNo && p.PONo == PONo).ToList();
            
            if (records.Count() > 0){
                No = records.Max(p => p.BPUSeq);
                records = null;
            }

            return No + 1;
        }
        #endregion

    }
}