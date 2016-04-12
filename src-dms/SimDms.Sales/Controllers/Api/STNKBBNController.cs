using SimDms.Sales.BLL;
using SimDms.Sales.Models;
//using SimDms.Sales.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using System.Transactions;

namespace SimDms.Sales.Controllers.Api
{
    public class STNKBBNController : BaseController
    {

        public JsonResult DetailLoad(string SPKNo) 
        {
            var query = string.Format(@"
                SELECT a.CompanyCode, a.BranchCode, a.SPKNo, a.ChassisCode, a.ChassisNo, a.ReqInNo, a.FakturPolisiNo, 
                a.PoliceRegistrationNo, CONVERT(VARCHAR(50),a.PoliceRegistrationDate,111) PoliceRegistrationDate, 
                CONVERT(VARCHAR(50),a.STNKInDate,111) STNKInDate, a.STNKInBy, CONVERT(VARCHAR(50),a.STNKOutDate,111) STNKOutDate, 
                a.STNKOutBy, CONVERT(VARCHAR(50),a.BPKBInDate,111) BPKBInDate, a.BPKBInBy, CONVERT(VARCHAR(50),a.BPKBOutDate,111) BPKBOutDate, 
                a.BPKBOutBy, CONVERT(VARCHAR(50),a.KIRInDate,111) KIRInDate, a.KIRInBy, CONVERT(VARCHAR(50),a.KIROutDate,111) KIROutDate, 
                a.KIROutBy, a.Remark, a.BPKBNo, b.SKPKName CustomerName, c.CustomerCode,
                b.SKPKAddress1 + b.SKPKAddress2+' '+ b.SKPKAddress3 as Address, c.leasingCo as Leasing
                  from omTrSalesSPKDetail a
                    inner join omTrSalesReqDetail b
	                    on a.ChassisCode = b.ChassisCode and a.ChassisNo = b.ChassisNo
                    inner join omTrSalesSO c 
	                    on b.SONo=c.SONo and b.BranchCode=c.BranchCode
            WHERE a.CompanyCode = '{0}'
            AND a.BranchCode = '{1}'
            AND a.SPKNo = '{2}'
                       ", CompanyCode, BranchCode, SPKNo);
            return Json(ctx.Database.SqlQuery<SPKDetailView>(query).AsQueryable());
        }

        public JsonResult SubDetailLoad(string SPKNo, string ChassisCode, decimal ChassisNo)
        {
            var query = string.Format(@"
                SELECT  CONVERT(VARCHAR(50),a.BPKBOutDate,111) as BPKBOutDate, CONVERT(VARCHAR(50),a.BPKBOutDate,106) as BPKBOutDateDesc , (CASE ISNULL(a.BPKBOutType, 0) WHEN '0' THEN 'Leasing' WHEN '1' THEN 'Cabang' WHEN '2' THEN 'Pelanggan' END) AS tipe
                    , (CASE ISNULL(a.BPKBOutType, 0) WHEN '0' THEN (SELECT CustomerName FROM GnMstCustomer WHERE CompanyCode = a.CompanyCode AND CustomerCode = a.BPKBOutBy)
                    WHEN '1' THEN (SELECT RefferenceDesc1 FROM OmMstRefference WHERE CompanyCode = a.CompanyCode AND RefferenceType = 'WARE' AND RefferenceCode = a.BPKBOutBy)
                    WHEN '2' THEN (SELECT CustomerName FROM GnMstCustomer WHERE CompanyCode = a.CompanyCode AND CustomerCode = a.BPKBOutBy) END) AS Nama, a.BPKBOutType, BPKBOutBy
                  FROM omTrSalesSPKSubDetail a
                 WHERE a.CompanyCode = '{0}'
            AND a.BranchCode = '{1}'
                   AND a.SPKNo = '{2}'
                    AND a.ChassisCode = '{3}'
                    AND a.ChassisNo = '{4}'
                       ", CompanyCode, BranchCode, SPKNo, ChassisCode, ChassisNo);
            return Json(ctx.Database.SqlQuery<SPKSubDetailView>(query).AsQueryable());
        }
        
        public JsonResult updateHdr(omTrSalesSPK model)
        {
            var me = ctx.omTrSalesSPK.Find(CompanyCode, BranchCode, model.SPKNo);
            var data = new omTrSalesSPK();
            if (me != null)
            {
                var meBPU = ctx.omTrSalesSPKDetail.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.SPKNo == model.SPKNo).FirstOrDefault();


                if (meBPU != null && me.Status == "0")
                {
                    var meDSM = ctx.omTrSalesSPKDetail
                        .Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.SPKNo == model.SPKNo && p.ChassisCode == meBPU.ChassisCode && p.ChassisNo == meBPU.ChassisNo)
                        .FirstOrDefault();
                    if (meDSM != null)
                    {
                        me.Status = "1";
                        ctx.SaveChanges();
                        data = ctx.omTrSalesSPK.Find(CompanyCode, BranchCode, model.SPKNo);
                        return Json(new { success = true, data = data });
                    }
                    else
                    {
                        return Json(new { success = false, message = meBPU.SPKNo + " : do not have table detail model in Colour Change!" });
                    }
                }
                else if (meBPU != null && me.Status == "1")
                {
                    me.Status = "2";
                    ctx.SaveChanges();
                    data = ctx.omTrSalesSPK.Find(CompanyCode, BranchCode, model.SPKNo);
                    return Json(new { success = true, data = data });
                }
                else
                {
                    return Json(new { success = false, message = "You must fill table detail!" });
                }

            }
            else
            {
                return Json(new { success = false, data = data });
            }

        }

        [HttpPost]
        public JsonResult Save(omTrSalesSPK model)
        {
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            DateTime currentTime = DateTime.Now;
            try
            {

                var me = ctx.omTrSalesSPK.Find(companyCode, BranchCode, model.SPKNo);

                if (me == null)
                {
                    me = new omTrSalesSPK();
                    me.CreatedDate = currentTime;
                    me.LastUpdateDate = currentTime;
                    me.CreatedBy = userID;
                    var SPKNo = GetNewDocumentNo("SPF", model.SPKDate.Value);
                    me.SPKNo = SPKNo;
                    me.Status = "0";
                    ctx.omTrSalesSPK.Add(me); 
                }
                else
                {
                    me.LastUpdateDate = currentTime;
                    me.LastUpdateBy = userID;
                }
                me.CompanyCode = CompanyCode;
                me.BranchCode = BranchCode;
                me.SPKDate = model.SPKDate;
                me.RefferenceNo = model.RefferenceNo;
                me.RefferenceDate = model.RefferenceDate;
                me.SupplierCode = model.SupplierCode;
                me.Remark = model.Remark;
            
                Helpers.ReplaceNullable(me);
                ctx.SaveChanges();
                return Json(new { success = true, data = me });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        public JsonResult Delete(omTrSalesSPK model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.omTrSalesSPK.Find(CompanyCode, BranchCode, model.SPKNo);
                    var meDtl = ctx.omTrSalesSPKDetail.Where(m => m.CompanyCode == companyCode && m.BranchCode == BranchCode && m.SPKNo == model.SPKNo).ToArray();
                    if (me != null)
                    {
                        var x = meDtl.Length;
                        for (var i = 0; i < x; i++)
                        {
                            ctx.omTrSalesSPKDetail.Remove(meDtl[i]);
                            ctx.SaveChanges();
                        }
                        ctx.omTrSalesSPK.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Colour Change berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Colour Change, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Colour Change, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        [HttpPost]
        public JsonResult Save2(omTrSalesSPKDetail model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.omTrSalesSPKDetail.Find(CompanyCode, BranchCode, model.SPKNo, model.ChassisCode, model.ChassisNo);

            if (me == null)
            {
                me = new omTrSalesSPKDetail();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.omTrSalesSPKDetail.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.SPKNo = model.SPKNo;
            me.ChassisCode = model.ChassisCode;
            me.ChassisNo = model.ChassisNo;
            me.ReqInNo = model.ReqInNo;
            me.FakturPolisiNo = model.FakturPolisiNo;
            me.PoliceRegistrationNo = model.PoliceRegistrationNo;
            me.PoliceRegistrationDate = model.PoliceRegistrationDate;
            me.STNKInDate = model.STNKInDate;
            me.STNKInBy = model.STNKInBy;
            me.STNKOutDate = model.STNKOutDate;
            me.STNKOutBy = model.STNKOutBy;
            me.BPKBInDate = model.BPKBInDate;
            me.BPKBInBy = model.BPKBInBy;
            me.BPKBOutDate = model.BPKBOutDate;
            me.BPKBOutBy = model.BPKBOutBy;
            me.KIRInDate = model.KIRInDate;
            me.KIRInBy = model.KIRInBy;
            me.KIROutDate = model.KIROutDate;
            me.KIROutBy = model.KIROutBy;
            me.Remark = model.Remark;
            me.BPKBNo = model.BPKBNo;
            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = me });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Save3(omTrSalesSPKSubDetail model)
        {
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.omTrSalesSPKSubDetail.Find(companyCode, BranchCode, model.SPKNo, model.ChassisCode, model.ChassisNo, model.BPKBOutType, model.BPKBOutBy);

            if (me == null)
            {
                me = new omTrSalesSPKSubDetail();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.omTrSalesSPKSubDetail.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.SPKNo = model.SPKNo;
            me.ChassisCode = model.ChassisCode;
            me.ChassisNo = model.ChassisNo;
            me.BPKBOutDate = model.BPKBOutDate;
            me.BPKBOutType = model.BPKBOutType;
            me.BPKBOutBy = model.BPKBOutBy;
            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = me });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        public JsonResult Delete2(omTrSalesSPKDetail model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.omTrSalesSPKDetail.Find(CompanyCode, BranchCode, model.SPKNo, model.ChassisCode, model.ChassisNo);
                    if (me != null)
                    {
                        ctx.omTrSalesSPKDetail.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Colour ChangeDetail berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Colour ChangeDetail, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Colour ChangeDetail, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

        public JsonResult Delete3(omTrSalesSPKSubDetail model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.omTrSalesSPKSubDetail.Find(companyCode, BranchCode, model.SPKNo, model.ChassisCode, model.ChassisNo, model.BPKBOutType, model.BPKBOutBy);
                    if (me != null)
                    {
                        ctx.omTrSalesSPKSubDetail.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Colour ChangeDetail berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Colour ChangeDetail, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Colour ChangeDetail, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }
    }
}
