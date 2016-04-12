using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Sparepart.Models;
using SimDms.Common;
using SimDms.General.Models.Others;
using System.Transactions;

namespace SimDms.General.Controllers
{
    public class SegmentChartController : BaseController
    {
        public JsonResult CheckLookUpDtl(string CodeID, string LookUpValue)
        {
            var titleName = ctx.LookUpDtls.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.CodeID == CodeID && a.LookUpValue == LookUpValue);
            if (titleName == null)
            {
                return Json(new
                {
                    success = false,
                    data = titleName,
                    TitleName = ""
                }, JsonRequestBehavior.AllowGet);
            
            }else{
                return Json(new
                {
                    success = true,
                    data = titleName,
                    TitleName = titleName.LookUpValueName
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(GnMstSegmentAcc model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.GnMstSegmentAccs.Find(CompanyCode, BranchCode, model.TipeSegAcc, model.SegAccNo);

            if (me == null)
            {
                me = new GnMstSegmentAcc();
                me.CreateDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreateBy = userID;
                ctx.GnMstSegmentAccs.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = BranchCode;
            me.TipeSegAcc = model.TipeSegAcc;
            me.SegAccNo = model.SegAccNo;
            me.Description = model.Description;
            me.Parent = model.Parent;
            me.FromDate = model.FromDate;
            me.EndDate = model.EndDate;

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data Segment Chart of Account GL berhasil disimpan.";
                result.data = new
                {
                    TipeSegAcc = me.TipeSegAcc,
                    SegAccNo = me.SegAccNo 
                };
            }
            catch (Exception Ex)
            {
                result.message = "Data Segment Chart of Account GL tidak bisa disimpan.";
            }

            return Json(result);
        }

        public JsonResult Delete(GnMstSegmentAcc model)
        {
            object returnObj = null;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var me = ctx.GnMstSegmentAccs.Find(CompanyCode, BranchCode, model.TipeSegAcc, model.SegAccNo);
                    if (me != null)
                    {
                        ctx.GnMstSegmentAccs.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Segment Chart of Account GL berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Segment Chart of Account GL , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Segment Chart of Account GL , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }
    }
}
