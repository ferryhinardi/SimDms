using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using SimDms.General.Models.Others;
using SimDms.Common;
using SimDms.Sparepart.Models;
using TracerX;

namespace SimDms.General.Controllers.Api
{
    public class PeriodeController : BaseController
    {
        [HttpPost]
        public JsonResult lastperiode() {
           ResultModel result = InitializeResultModel();
            var me = ctx.Periodes.Where(x =>
                            x.CompanyCode == CompanyCode
                         && x.BranchCode == BranchCode).
                         OrderByDescending(x => x.FiscalYear).
                         ThenByDescending(y => y.FiscalMonth).
                         ThenByDescending(z => z.PeriodeNum).
                         FirstOrDefault();

            if (me != null)
            {
                DateTime tFromDate = me.FromDate;
                DateTime isFromdate = GetFirstDayInMonth(tFromDate, tFromDate.Month + 1);
                DateTime isEndDate = GetLastDayInMonth(isFromdate);
                
                if (me.PeriodeNum == 13)
                {
                    var data = new Periode {
                                FiscalYear= me.FiscalYear + 1,
                                PeriodeNum = 1,
                                FromDate = isFromdate,
                                EndDate = isEndDate,
                                FiscalMonth = me.FiscalMonth,
                                PeriodeName = isFromdate.ToString("MMM-yy")
                    };
                    return Json(new { success = data != null, data = data });
                }
                else if (me.PeriodeNum == 12)
                {
                     var data = new Periode {
                                FiscalYear = me.FiscalYear,
                                PeriodeNum = me.PeriodeNum + 1,
                                FromDate = tFromDate,
                                EndDate = GetLastDayInMonth(tFromDate),
                                FiscalMonth = me.FiscalMonth,
                                PeriodeName = tFromDate.ToString("MMM-yy")
                     };
                     return Json(new { success = data != null, data = data });
                }
                else
                {
                    //decimal isFiscalYear;
                    //   if (me.PeriodeNum == 9){
                    //       isFiscalYear = me.FiscalYear + 1;
                    //   }else{
                    //       isFiscalYear = me.FiscalYear;
                    //   }
                     var data = new Periode {
                         FiscalYear = me.FiscalYear, 
                                PeriodeNum = me.PeriodeNum + 1,
                                FromDate = isFromdate,
                                FiscalMonth = me.FiscalMonth,
                                EndDate = isEndDate,
                                PeriodeName = isFromdate.ToString("MMM-yy")
                    };
                     return Json(new { success = data != null, data = data });
                }

            }
            else
            {
                DateTime tFromDate = DateTime.Now.Date;
                decimal year = new DateTime().Year;
                 var data = new Periode {
                            FiscalYear = year, 
                            PeriodeNum = 1,
                            FromDate = GetFirstDayInMonth(tFromDate, 1),
                            EndDate = GetLastDayInMonth(tFromDate), 
                            FiscalMonth = 4,
                            PeriodeName = me.FromDate.ToString("MMM-yy")
                };
                 return Json(new { success = data != null, data = data });
            }
        }

        private DateTime GetFirstDayInMonth(DateTime dt, int month)
        {
            int year = dt.Year;
            if (month == 13) { month = 1; year += 1; }
            DateTime dtRet = new DateTime(year, month, 1, 0, 0, 0);
            return dtRet;
        }

        private DateTime GetLastDayInMonth(DateTime dt)
        {
            int month = dt.Month;
            DateTime dtRet = new DateTime(dt.Year, month, 1);
            dtRet = dtRet.AddMonths(1);
            dtRet = dtRet.AddDays(-(dtRet.Day));
            return dtRet;
        }

        [HttpPost]
        public JsonResult Save(Periode model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.Periodes.Find(CompanyCode, BranchCode, model.FiscalYear, model.FiscalMonth, model.PeriodeNum);

            if (me == null)
            {
                me = new Periode(); 
                me.CreateDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreateBy = userID;
                ctx.Periodes.Add(me); 
            }
                else{
                    me.LastUpdateDate = currentTime;
                    me.LastUpdateBy = userID;
            }
            me.CompanyCode= CompanyCode;            me.BranchCode= BranchCode;            me.FiscalYear= model.FiscalYear;            me.FiscalMonth= model.FiscalMonth;            me.PeriodeNum= model.PeriodeNum;            me.PeriodeName= model.PeriodeName;            me.FromDate= model.FromDate;            me.EndDate= model.EndDate;            me.StatusSparepart= 0;            me.StatusSales= 0;            me.StatusService= 0;            me.StatusFinanceAP= 0;            me.StatusFinanceAR= 0;            me.StatusFinanceGL= 0;            me.FiscalStatus= model.FiscalStatus;
              
                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Data Periode berhasil disimpan.";
                    result.data = new
                    {
                        FiscalYear = me.FiscalYear,
                        FiscalMonth = me.FiscalMonth
                    };
                }
                catch (Exception Ex)
                {
                    result.message = "Data Periode tidak bisa disimpan.";
                    MyLogger.Info("Error on Periode saving: " + Ex.Message);
                }
            
            return Json(result);
        }

        public JsonResult Delete(Periode model)
        {
            object returnObj = null;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
            var me = ctx.Periodes.Find(CompanyCode, BranchCode, model.FiscalYear, model.FiscalMonth, model.PeriodeNum);
                    if (me != null)
                    {
                        ctx.Periodes.Remove(me);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Periode berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika mendelete Periode , Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika mendelete Periode , Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }

    }
}
