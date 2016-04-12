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
    public class GantiWarnaController : BaseController
    {

        public JsonResult DetailLoad(string DocNo) 
        {
            var query = string.Format(@"
                SELECT a.SalesModelCode,
            Convert(varchar,a.SalesModelYear) as SalesModelYear,
            b.SalesModelDesc,a.ChassisCode,
            Convert(varchar,a.ChassisNo) as ChassisNo,
            a.EngineCode,Convert(varchar,a.EngineNo) as EngineNo,
            a.ColourCodeFrom, a.WarehouseCode, c.RefferenceDesc1 as WarehouseName, 
            d.RefferenceDesc1 as ColourNameFrom, a.ColourCodeTo, e.RefferenceDesc1 as ColourNameTo, a.RemarkDtl
            FROM OmTrInventColorChangeDetail a
            LEFT JOIN omMstModel b
            ON a.CompanyCode = b.CompanyCode
            AND a.SalesModelCode = b.SalesModelCode
            LEFT JOIN omMstRefference c
            ON a.CompanyCode = c.CompanyCode
            AND a.WarehouseCode = c.RefferenceCode
			AND c.RefferenceType = 'WARE'
			LEFT JOIN omMstRefference d
            ON a.CompanyCode = d.CompanyCode
            AND a.ColourCodeFrom = d.RefferenceCode
			AND d.RefferenceType = 'COLO'
			LEFT JOIN omMstRefference e
            ON a.CompanyCode = e.CompanyCode
            AND a.ColourCodeTo = e.RefferenceCode
			AND e.RefferenceType = 'COLO'
            WHERE a.CompanyCode = '{0}'
            AND a.BranchCode = '{1}'
            AND a.DocNo = '{2}'
            ORDER BY a.SalesModelCode,a.SalesModelYear,a.ChassisCode,a.ChassisNo ASC
                       ", CompanyCode, BranchCode, DocNo);
            return Json(ctx.Database.SqlQuery<ColorChangeDetailView>(query).AsQueryable());
        }

        public JsonResult updateHdr(OmTrInventColorChange model)
        {
            var me = ctx.OmTrInventColorChange.Find(CompanyCode, BranchCode, model.DocNo);
            var data = new OmTrInventColorChange();
            if (me != null)
            {
                var meBPU = ctx.OmTrInventColorChangeDetail.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.DocNo == model.DocNo).FirstOrDefault();


                if (meBPU != null && me.Status == "0")
                {
                    var meDSM = ctx.OmTrInventColorChangeDetail
                        .Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.DocNo == model.DocNo && p.ChassisCode == meBPU.ChassisCode)
                        .FirstOrDefault();
                    if (meDSM != null)
                    {
                        me.Status = "1";
                        ctx.SaveChanges();
                        data = ctx.OmTrInventColorChange.Find(CompanyCode, BranchCode, model.DocNo);
                        return Json(new { success = true, data = data });
                    }
                    else
                    {
                        return Json(new { success = false, message = meBPU.DocNo + " : do not have table detail model in Colour Change!" });
                    }
                }
                else if (meBPU != null && me.Status == "1")
                {
                    me.Status = "2";
                    ctx.SaveChanges();
                    data = ctx.OmTrInventColorChange.Find(CompanyCode, BranchCode, model.DocNo);
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
        public JsonResult Save(OmTrInventColorChange model)
        {
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.OmTrInventColorChange.Find(companyCode, BranchCode, model.DocNo);

            if (me == null)
            {
                me = new OmTrInventColorChange();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                var DocNo = GetNewDocumentNo("VTI", model.DocDate.Value);
                me.DocNo = DocNo;
                me.Status = "0";
                ctx.OmTrInventColorChange.Add(me); 
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.DocDate = model.DocDate;
            me.ReferenceNo = model.ReferenceNo;
            me.ReferenceDate = model.ReferenceDate;
            me.Remark = model.Remark;
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

        public JsonResult Delete(OmTrInventColorChange model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.OmTrInventColorChange.Find(CompanyCode, BranchCode, model.DocNo);
                    var meDtl = ctx.OmTrInventColorChangeDetail.Where(m => m.CompanyCode == companyCode && m.BranchCode == BranchCode && m.DocNo == model.DocNo).ToArray();
                    if (me != null)
                    {
                        var x = meDtl.Length;
                        for (var i = 0; i < x; i++)
                        {
                            ctx.OmTrInventColorChangeDetail.Remove(meDtl[i]);
                            ctx.SaveChanges();
                        }
                        ctx.OmTrInventColorChange.Remove(me);
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
        public JsonResult Save2(OmTrInventColorChangeDetail model)
        {
            //ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.OmTrInventColorChangeDetail.Find(CompanyCode, BranchCode, model.DocNo, model.ChassisCode, model.ChassisNo);

            if (me == null)
            {
                me = new OmTrInventColorChangeDetail();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.OmTrInventColorChangeDetail.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.DocNo = model.DocNo;
            me.ChassisCode = model.ChassisCode;
            me.ChassisNo = model.ChassisNo;
            me.EngineCode = model.EngineCode;
            me.EngineNo = model.EngineNo;
            me.SalesModelCode = model.SalesModelCode;
            me.SalesModelYear = model.SalesModelYear;
            me.ColourCodeFrom = model.ColourCodeFrom;
            me.ColourCodeTo = model.ColourCodeTo;
            me.WarehouseCode = model.WarehouseCode;
            me.RemarkDtl = model.RemarkDtl;
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

        public JsonResult Delete2(OmTrInventColorChangeDetail model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.OmTrInventColorChangeDetail.Find(CompanyCode, BranchCode, model.DocNo, model.ChassisCode, model.ChassisNo);
                    if (me != null)
                    {
                        ctx.OmTrInventColorChangeDetail.Remove(me);
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
