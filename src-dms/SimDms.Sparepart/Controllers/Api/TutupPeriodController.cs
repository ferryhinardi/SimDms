using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Sparepart.Controllers.Api
{
    public class TutupPeriodController : BaseController
    {

        public JsonResult ReloadAll()
        {
            var oCoProfileSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
            var isFiscalExist = Convert.ToDecimal(oCoProfileSpare.FiscalMonth.Equals(string.Empty) ? 0 : oCoProfileSpare.FiscalMonth) > 0 &&
                            Convert.ToDecimal(oCoProfileSpare.FiscalYear.Equals(string.Empty) ? 0 : oCoProfileSpare.FiscalYear) > 0 &&
                            Convert.ToDecimal(oCoProfileSpare.FiscalPeriod.Equals(string.Empty) ? 0 : oCoProfileSpare.FiscalPeriod) > 0;
            if (isFiscalExist)
            {
                var oPeriode = ctx.Periodes.Find(CompanyCode, BranchCode, oCoProfileSpare.FiscalYear, oCoProfileSpare.FiscalMonth, oCoProfileSpare.FiscalPeriod);
                
                return Json(new
                {
                    success = true,
                    YearClosed = oCoProfileSpare.FiscalYear.ToString(),
                    MonthClosed = oCoProfileSpare.FiscalMonth.ToString(),
                    Periode = oCoProfileSpare.FiscalPeriod.ToString(),
                    NmPeriode = oPeriode.PeriodeName.ToString(),
                    //dataGrid = record
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                });
            }
            
        }

        public JsonResult LoadGrid() {
            var oCoProfileSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
            var record = ctx.SpClosePeriodPendings.Where(a => a.FiscalYear == oCoProfileSpare.FiscalYear && a.FiscalMonth == oCoProfileSpare.FiscalMonth && a.ProfitCenter == ProfitCenter);
            try
            {
                return Json(new { success = true, record = record });
            }
            catch (Exception e) {
                return Json(new { success = false, message = e.Message});
            }
        }

        public JsonResult ClosePeriod() 
        {
                try
                {
                    string fail1 = "Closing month gagal karena terdapat transaksi yang belum close";
                    var pesanStr = "";

                    var oCoProfileSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
                    var isFiscalExist = Convert.ToDecimal(oCoProfileSpare.FiscalMonth.Equals(string.Empty) ? 0 : oCoProfileSpare.FiscalMonth) > 0 &&
                                    Convert.ToDecimal(oCoProfileSpare.FiscalYear.Equals(string.Empty) ? 0 : oCoProfileSpare.FiscalYear) > 0 &&
                                    Convert.ToDecimal(oCoProfileSpare.FiscalPeriod.Equals(string.Empty) ? 0 : oCoProfileSpare.FiscalPeriod) > 0;

                    if (isFiscalExist)
                    {
                        if (oCoProfileSpare != null && CheckValidTrans(oCoProfileSpare))
                        {
                            using (var tran = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                            {
                                try 
	                            {
	                                PostingDelegate pd = new PostingDelegate();
                                    string pesan = ClosingMonth(oCoProfileSpare.PeriodEnd.Year, oCoProfileSpare.PeriodEnd.Month, oCoProfileSpare, pd);

                                    if (pesan.Length > 5)
                                    {
                                        pesanStr = string.Format("Tutup bulan gagal, disebabkan {0}", pesan);

                                        tran.Rollback();
                                        return Json(new { success = false, message = pesanStr });
                                    }
                                    else
                                    {
                                        pesanStr = "Tutup bulan selesai/berhasil";

                                        tran.Commit();
                                        return Json(new { success = true, message = pesanStr });
                                    }
	                            }
	                            catch (Exception ex)
	                            {
                                    tran.Rollback();
                                    return Json(new { success = false, message = ex.Message });
	                            }
                            }
                        }
                        else
                        {
                            return Json(new { success = false, message = fail1 });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Tutup bulan tidak dapat diproses. Periode Fiscal belum di-setting" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
        }

        private string ClosingMonth(int year, int month, GnMstCoProfileSpare oCoProfileSpare, PostingDelegate pd)
        {
            bool result = false;
            string msg = "1";
            //try
            //{
                object[] parameters = {CompanyCode, BranchCode, CurrentUser.UserId};

                var query = "exec uspfn_SpUtlCheckB4Closing @p0,@p1,@p2";

                result = ctx.Database.ExecuteSqlCommand(query, parameters) > 0;

                //SqlConnection con = (SqlConnection)ctx.Database.Connection;
                //SqlCommand cmd = con.CreateCommand();

                //cmd.CommandText = "uspfn_SpUtlCheckB4Closing";
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                //cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);
                ////try
                ////{
                //    cmd.Connection.Open();
                //    cmd.ExecuteNonQuery();
                //    cmd.Connection.Close();
                //    cmd.Connection.Dispose();
                ////}
                //catch (Exception e)
                //{
                //    throw new Exception(e.Message);
                //}
                if (!CheckTransactPeriod(oCoProfileSpare))
                {
                    //LockedPeriod(true);
                    pd.Update(string.Format("Proses 1 / 11 : jam {0}", DateTime.Now.ToString("hh:mm:ss")));
                    if (UpdateBORegisterTuning())
                    {
                        pd.Update(string.Format("Proses 2 / 11 : jam {0}", DateTime.Now.ToString("hh:mm:ss")));
                        if (InsertHistoryItemStatusTuning(year, month))
                        {
                            pd.Update(string.Format("Proses 3 / 11 : jam {0}", DateTime.Now.ToString("hh:mm:ss")));
                            if (UpdateDemandAverageTuning(year, month))
                            {
                                pd.Update(string.Format("Proses 4 / 11 : jam {0}", DateTime.Now.ToString("hh:mm:ss")));
                                if (UpdateABCClassTuning(year, month))
                                {
                                    pd.Update(string.Format("Proses 5 / 11  : jam {0}", DateTime.Now.ToString("hh:mm:ss")));
                                    if (UpdateMovingCodeTuningV2(year, month))
                                    {
                                        pd.Update(string.Format("Proses 6 / 11  : jam {0}", DateTime.Now.ToString("hh:mm:ss")));
                                        if (InsertHistoryMovementTuning(year, month))
                                        {
                                            pd.Update(string.Format("Proses 7 / 11  : jam {0}", DateTime.Now.ToString("hh:mm:ss")));
                                            if (UpdateBOMTuning())
                                            {
                                                pd.Update(string.Format("Proses 8 / 11  : jam {0}", DateTime.Now.ToString("hh:mm:ss")));
                                                if (UpdateOrderPointTuning())
                                                {
                                                    pd.Update(string.Format("Proses 9 / 11  : jam {0}", DateTime.Now.ToString("hh:mm:ss")));
                                                    if (InsertTransactionHistoryTuning(year, month))
                                                    {
                                                        pd.Update(string.Format("Proses 10 / 11 : jam {0}", DateTime.Now.ToString("hh:mm:ss")));
                                                        if (UpdateDemandHistoryTuning(year, month))
                                                        {
                                                            pd.Update(string.Format("Proses 11 / 11  : jam {0}", DateTime.Now.ToString("hh:mm:ss")));
                                                            if (UpdateTransactPeriod(oCoProfileSpare))
                                                            {
                                                                pd.Update(string.Format("Proses Tutup Bulan Berhasil jam {0}", DateTime.Now.ToString("hh:mm:ss")));
                                                                msg = "1";
                                                                result = true;

                                                            }
                                                            else
                                                            {
                                                                pd.Update("Proses Tutup Bulan Gagal");
                                                                msg = "Update Periode Transaksi/reset No.Dokumen gagal";
                                                                result = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            msg = "Proses Update Demand History gagal";
                                                            result = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        msg = "Proses Insert History transaksi gagal";
                                                        result = false;
                                                    }
                                                }
                                                else
                                                {
                                                    msg = "Proses Insert History Items gagal";
                                                    result = false;
                                                }
                                            }
                                            else
                                            {
                                                msg = "Proses Update Moving Code gagal";
                                                result = false;
                                            }
                                        }
                                        else
                                        {
                                            msg = "Proses Update data Qty awal bulan gagal";
                                            result = false;
                                        }
                                    }
                                    else
                                    {
                                        msg = "Proses insert history movement gagal";
                                        result = false;
                                    }
                                }
                                else
                                {
                                    msg = "Proses Update data Order Point gagal";
                                    result = false;
                                }
                            }
                            else
                            {
                                msg = "Proses Update ABC Class gagal";
                                result = false;
                            }
                        }
                        else
                        {
                            msg = "Proses Update Demand Average gagal";
                            result = false;
                        }
                    }
                    else
                    {
                        msg = "Proses Update BO Register gagal";
                        result = false;
                    }
                    // LockedPeriod(ctx, user, false);

                    object[] paramSp = { CompanyCode, BranchCode, year, month, month, CurrentUser.UserId };

                    var querySp = "exec uspfn_spPostingSparepartAnalysis @p0,@p1,@p2,@p3,@p4,@p5";

                    result = ctx.Database.ExecuteSqlCommand(querySp, paramSp) > 0;

                    //cmd.Parameters.Clear();
                    //cmd.CommandText = "uspfn_spPostingSparepartAnalysis";
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                    //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                    //cmd.Parameters.AddWithValue("@PeriodYear", year);
                    //cmd.Parameters.AddWithValue("@PeriodStart", month);
                    //cmd.Parameters.AddWithValue("@PeriodEnd", month);
                    //cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);
                    ////try
                    ////{
                    //    cmd.Connection.Open();
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Connection.Close();
                    //    cmd.Connection.Dispose();
                    //}
                    //catch (Exception e)
                    //{
                    //    throw new Exception(e.Message);
                    //}
                }
                else
                {
                    msg = "Periode Transaksi Tidak Benar \r\n[Blm diseting/Blm diaktifkan/Sudah di Tutup]";
                    result = false;
                }
            //}
            //catch (Exception e)
            //{
            //    throw new Exception(e.Message);
            //}
            if (result)
            {
                return msg;
            }
            return msg;

        }

        private bool UpdateTransactPeriod(GnMstCoProfileSpare oCoProfileSpare)
        {
            bool result = false;
            int Year, oldYear, Month, newPeriod, oldPeriod;

            if (oCoProfileSpare.FiscalPeriod != 12)
            {
                oldYear = Convert.ToInt32(oCoProfileSpare.FiscalYear);
                Year = Convert.ToInt32(oCoProfileSpare.FiscalYear);
                Month = Convert.ToInt32(oCoProfileSpare.FiscalMonth);
                oldPeriod = Convert.ToInt32(oCoProfileSpare.FiscalPeriod);
                newPeriod = Convert.ToInt32(oCoProfileSpare.FiscalPeriod) + 1;
            }
            else
            {
                oldYear = Convert.ToInt32(oCoProfileSpare.FiscalYear);
                Year = Convert.ToInt32(oCoProfileSpare.FiscalYear) + 1;
                Month = Convert.ToInt32(oCoProfileSpare.FiscalMonth);
                oldPeriod = Convert.ToInt32(oCoProfileSpare.FiscalPeriod);
                newPeriod = 1;
            }

            //try
            //{
                var q1 = string.Format(@"
                    SELECT 
                    FiscalYear,
                    FiscalMonth,
                    PeriodeNum,
                    FromDate,
                    EndDate
                    FROM gnMstPeriode
                    WHERE CompanyCode = '{0}'
                    AND BranchCode    = '{1}'
                    AND FiscalYear    = '{2}'
                    AND FiscalMonth   = '{3}'
                    AND PeriodeNum    = '{4}'
                    ", CompanyCode, BranchCode, Year, Month, newPeriod);

                var rowPeriode = ctx.Database.SqlQuery<QueryTutupPeriod2>(q1).ToList();

                if (rowPeriode != null)
                {
                    var q2 = string.Format(@"
                        UPDATE gnMstPeriode 
                        SET StatusSparepart='{5}'
                        WHERE CompanyCode='{0}'
                        AND BranchCode='{1}'
                        AND FiscalYear='{2}'
                        AND FiscalMonth='{3}'
                        AND PeriodeNum='{4}'", CompanyCode, BranchCode, oldYear, Month, oldPeriod, "2");
                    
                    result = (ctx.Database.ExecuteSqlCommand(q2) > 0);

                    if (result)
                    {
                        var q3 = string.Format(@"
                        UPDATE gnMstPeriode 
                        SET StatusSparepart='{5}'
                        WHERE CompanyCode='{0}'
                        AND BranchCode='{1}'
                        AND FiscalYear='{2}'
                        AND FiscalMonth='{3}'
                        AND PeriodeNum='{4}'", CompanyCode, BranchCode, Year, Month, newPeriod, "1");

                        result = (ctx.Database.ExecuteSqlCommand(q3) > 0);

                        if (result)
                        {
                            var q4 = string.Format(@"
                                UPDATE gnMstCoProfileSpare
                                SET FiscalYear = '{2}',
                                    FiscalMonth = '{3}',
                                    FiscalPeriod = '{4}',
                                    PeriodBeg = '{5}',
                                    PeriodEnd = '{6}',
                                    LastUpdateBy = '{7}',
                                    LastUpdateDate = GetDate() 
                                WHERE CompanyCode = '{0}'
                                AND BranchCode = '{1}'
                                ", CompanyCode, BranchCode, rowPeriode[0].FiscalYear, rowPeriode[0].FiscalMonth, rowPeriode[0].PeriodeNum, rowPeriode[0].FromDate, rowPeriode[0].EndDate, CurrentUser.UserId);


                            result = (ctx.Database.ExecuteSqlCommand(q4) > 0);
                            if (oCoProfileSpare.PeriodBeg.Month == 12)
                            {
                                if (result)
                                {
                                    var q5 = string.Format(@"
                                    UPDATE gnMstDocument 
                                    SET DocumentYear = '{3}', 
                                        DocumentSequence = '{4}',
                                        LastUpdateBy = '{5}', 
                                        LastUpdateDate = GetDate() 
                                    WHERE CompanyCode = '{0}'
                                    AND BranchCode = '{1}'
                                    AND ProfitCenterCode = '{2}'
                                ", CompanyCode, BranchCode, ProfitCenter, oCoProfileSpare.PeriodBeg.Year + 1, "0", CurrentUser.UserId);

                                    result = (ctx.Database.ExecuteSqlCommand(q5) > 0);
                                }
                            }

                        }
                    }
                }
                else
                {
                    result = true;
                }
            //}
            //catch
            //{
            //    result = true;
            //}
            return result;
        }

        private bool UpdateDemandHistoryTuning(int year, int month)
        {
            object[] parameters = { CompanyCode, BranchCode, (month + 1 > 12 ? year + 1 : year), (month + 1 > 12 ? 1 : month + 1), ProfitCenter, CurrentUser.UserId };

            var query = "exec sp_UpdateDemandHistoryTuning @p0,@p1,@p2,@p3,@p4,@p5";

            bool result = ctx.Database.ExecuteSqlCommand(query, parameters) > 0;

            return result;

            //SqlConnection con = (SqlConnection)ctx.Database.Connection;
            //SqlCommand cmd = con.CreateCommand();
            //cmd.CommandText = "sp_UpdateDemandHistoryTuning";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            //cmd.Parameters.AddWithValue("@Year", year);
            //cmd.Parameters.AddWithValue("@Month", month);
            //cmd.Parameters.AddWithValue("@ProfitCenterCode", ProfitCenter);
            //cmd.Parameters.AddWithValue("@LastUpdateBy", CurrentUser.UserId);

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();
            //    cmd.Connection.Close();
            //    cmd.Connection.Dispose();

            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }

        private bool InsertTransactionHistoryTuning(int year, int month)
        {
            object[] parameters = { CompanyCode, BranchCode, month, year, CurrentUser.UserId, DateTime.Now };

            var query = "exec sp_InsertTransactionHistoryTuning @p0,@p1,@p2,@p3,@p4,@p5";

            bool result = ctx.Database.ExecuteSqlCommand(query, parameters) > 0;

            return result;

            //SqlConnection con = (SqlConnection)ctx.Database.Connection;
            //SqlCommand cmd = con.CreateCommand();
            //cmd.CommandText = "sp_InsertTransactionHistoryTuning";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            //cmd.Parameters.AddWithValue("@Month", month);
            //cmd.Parameters.AddWithValue("@Year", year);
            //cmd.Parameters.AddWithValue("@CreatedBy", CurrentUser.UserId);
            //cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();
            //    cmd.Connection.Close();
            //    cmd.Connection.Dispose();

            //    return true;
            //}
            //catch 
            //{
            //    return false;
            //}
        }

        private bool UpdateOrderPointTuning()
        {
            object[] parameters = { CompanyCode, BranchCode, CurrentUser.UserId };

            var query = "exec sp_UpdateOrderPointTuning @p0,@p1,@p2";

            bool result = ctx.Database.ExecuteSqlCommand(query, parameters) > 0;

            return result;

            //SqlConnection con = (SqlConnection)ctx.Database.Connection;
            //SqlCommand cmd = con.CreateCommand();
            //cmd.CommandText = "sp_UpdateOrderPointTuning";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            //cmd.Parameters.AddWithValue("@LastUpdateBy", CurrentUser.UserId);

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();
            //    cmd.Connection.Close();
            //    cmd.Connection.Dispose();

            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }

        private bool UpdateBOMTuning()
        {
            object[] parameters = { CompanyCode, BranchCode, CurrentUser.UserId };

            var query = "exec sp_UpdateBOMTuning @p0,@p1,@p2";

            bool result = ctx.Database.ExecuteSqlCommand(query, parameters) > 0;

            return result;

            //SqlConnection con = (SqlConnection)ctx.Database.Connection;
            //SqlCommand cmd = con.CreateCommand();
            //cmd.CommandText = "sp_UpdateBOMTuning";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            //cmd.Parameters.AddWithValue("@User", CurrentUser.UserId);

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();
            //    cmd.Connection.Close();
            //    cmd.Connection.Dispose();

            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }

        private bool InsertHistoryMovementTuning(int year, int month)
        {
            object[] parameters = { CompanyCode, BranchCode, year, month, CurrentUser.UserId };

            var query = "exec sp_InsertHistoryMovementTuning @p0,@p1,@p2,@p3,@p4";

            bool result = ctx.Database.ExecuteSqlCommand(query, parameters) > 0;

            return result;
            
            //SqlConnection con = (SqlConnection)ctx.Database.Connection;
            //SqlCommand cmd = con.CreateCommand();
            //cmd.CommandText = "sp_InsertHistoryMovementTuning";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            //cmd.Parameters.AddWithValue("@Year", year);
            //cmd.Parameters.AddWithValue("@Month", month);
            //cmd.Parameters.AddWithValue("@PID", CurrentUser.UserId);

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();
            //    cmd.Connection.Close();
            //    cmd.Connection.Dispose();

            //    return true;
            //}
            //catch 
            //{
            //    return false;
            //}
        }

        private bool UpdateMovingCodeTuningV2(int year, int month)
        {
            DateTime transDate = new DateTime(year, month, 1);

            object[] parameters = { CompanyCode, BranchCode, transDate };

            var query = "exec uspfn_SpUpdateMovingCode @p0,@p1,@p2";

            bool result = ctx.Database.ExecuteSqlCommand(query, parameters) > 0;

            return result;
            
            //SqlConnection con = (SqlConnection)ctx.Database.Connection;
            //SqlCommand cmd = con.CreateCommand();
            //cmd.CommandText = "uspfn_SpUpdateMovingCode";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            //cmd.Parameters.AddWithValue("@TransDate", transDate);

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();
            //    cmd.Connection.Close();
            //    cmd.Connection.Dispose();

            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }

        private bool UpdateABCClassTuning(int year, int month)
        {
            object[] parameters = { CompanyCode, BranchCode, year, month, CurrentUser.UserId };

            var query = "exec sp_UpdateABCClassTuning @p0,@p1,@p2,@p3,@p4";

            bool result = ctx.Database.ExecuteSqlCommand(query, parameters) > 0;

            return result;
            //SqlConnection con = (SqlConnection)ctx.Database.Connection;
            //SqlCommand cmd = con.CreateCommand();
            //cmd.CommandText = "sp_UpdateABCClassTuning";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            //cmd.Parameters.AddWithValue("@Year", year);
            //cmd.Parameters.AddWithValue("@Month", month);
            //cmd.Parameters.AddWithValue("@PID", CurrentUser.UserId);

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();
            //    cmd.Connection.Close();
            //    cmd.Connection.Dispose();

            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }

        private bool UpdateDemandAverageTuning(int year, int month)
        {
            bool b = true;
            //int result = -1;
            DateTime transDate = new DateTime(year, month, 1);

            object[] parameters = { CompanyCode, BranchCode, transDate };

            var query = "exec sp_UpdateDemandAverageTuning @p0,@p1,@p2";

            b = ctx.Database.ExecuteSqlCommand(query, parameters) > 0;

            //SqlConnection con = (SqlConnection)ctx.Database.Connection;
            //SqlCommand cmd = con.CreateCommand();
            //cmd.CommandText = "sp_UpdateDemandAverageTuning";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            //cmd.Parameters.AddWithValue("@TransDate", transDate);

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();
            //    cmd.Connection.Close();
            //    cmd.Connection.Dispose();

            //    result = 0;
            //}
            //catch (Exception ex)
            //{
            //    result = -1;
            //}

            //if (result >= 0)
            //{

//                var q1 = string.Format(@"
//                     update spMstItems 
// set DemandAverage = spMstItems.DemandAverage + a.DemandAverage
// from 
// (select a.PartNo, b.NewPartNo, a.DemandAverage
//                from spMstItems a
//                inner join spMstItemMod b
//                  on b.CompanyCode = a.CompanyCode
//                 and b.PartNo = a.PartNo
//                where a.DemandAverage > 0 and b.NewPartNo <> ''
//                  and a.CompanyCode= '{0}'
//                  and a.BranchCode= '{1}') a ", CompanyCode, BranchCode);

//                b = ctx.Database.ExecuteSqlCommand(q1) > 0;

//                var q1 = string.Format(@"
//                select 
//                 a.PartNo, b.NewPartNo, a.DemandAverage
//                from spMstItems a
//                left join spMstItemMod b
//                  on b.CompanyCode = a.CompanyCode
//                 and b.PartNo = a.PartNo
//                where a.DemandAverage > 0 and b.NewPartNo <> ''
//                  and a.CompanyCode = '{0}'
//                  and a.BranchCode = '{1}'", CompanyCode, BranchCode);
//                var dt = ctx.Database.SqlQuery<QueryTutupPeriod1>(q1);

//                foreach (var row in dt)
//                {
//                    if (!row.PartNo.Equals(row.NewPartNo))
//                    {
//                        var q2 = string.Format(@"
//                update spMstItems set DemandAverage = DemandAverage + '{2}'
//                where CompanyCode = '{0}'
//                  and BranchCode = '{1}'
//                  and PartNo = '{3}'", CompanyCode, BranchCode, row.DemandAverage, row.NewPartNo);

//                       b = ctx.Database.ExecuteSqlCommand(q2) > 0;
//                    }
//                }
//                //b = false;
//            }
            return b;
        }

        private bool InsertHistoryItemStatusTuning(int year, int month)
        {
            object[] parameters = { CompanyCode, BranchCode, year, month, CurrentUser.UserId };

            var query = "exec sp_InsertHistoryItemStatusTuning @p0,@p1,@p2,@p3,@p4";

            bool result = ctx.Database.ExecuteSqlCommand(query, parameters) > 0;

            return result;

            //SqlConnection con = (SqlConnection)ctx.Database.Connection;
            //SqlCommand cmd = con.CreateCommand();
            //cmd.CommandText = "sp_InsertHistoryItemStatusTuning";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            //cmd.Parameters.AddWithValue("@Year", year);
            //cmd.Parameters.AddWithValue("@Month", month);
            //cmd.Parameters.AddWithValue("@CreatedBy", CurrentUser.UserId);

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();
            //    cmd.Connection.Close();
            //    cmd.Connection.Dispose();

            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }

        private bool UpdateBORegisterTuning()
        {
            object[] parameters = { CompanyCode, BranchCode, CurrentUser.UserId };

            var query = "exec sp_UpdateBORegisterTuning @p0,@p1,@p2";

            bool result = ctx.Database.ExecuteSqlCommand(query, parameters) > 0;

            return result;

            //SqlConnection con = (SqlConnection)ctx.Database.Connection;
            //SqlCommand cmd = con.CreateCommand();
            //cmd.CommandText = "sp_UpdateBORegisterTuning";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            //cmd.Parameters.AddWithValue("@LastUpdateBy", CurrentUser.UserId);

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();
            //    cmd.Connection.Close();
            //    cmd.Connection.Dispose();

            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }

        private bool CheckTransactPeriod(GnMstCoProfileSpare oCoProfileSpare)
        {
            bool result = false;
            int Year, Month, newPeriod;

            if (oCoProfileSpare.FiscalPeriod != 12)
            {
                Year = Convert.ToInt32(oCoProfileSpare.FiscalYear);
                Month = Convert.ToInt32(oCoProfileSpare.FiscalMonth);
                newPeriod = Convert.ToInt32(oCoProfileSpare.FiscalPeriod) + 1;
            }
            else
            {
                Year = Convert.ToInt32(oCoProfileSpare.FiscalYear) + 1;
                Month = Convert.ToInt32(oCoProfileSpare.FiscalMonth);
                newPeriod = 1;
            }

            //try
            //{
                var q3 = string.Format(@"
                    SELECT 
                    FiscalYear,
                    FiscalMonth,
                    PeriodeNum,
                    FromDate,
                    EndDate
                    FROM gnMstPeriode
                    WHERE CompanyCode = '{0}'
                    AND BranchCode = '{1}'
                    AND FiscalYear = '{2}'
                    AND FiscalMonth = '{3}'
                    AND PeriodeNum = '{4}'",
                    CompanyCode, BranchCode, Year, Month, newPeriod);

                var rowPeriode = ctx.Database.ExecuteSqlCommand(q3);
                if (rowPeriode == null) 
                {
                    result = true;
                }
            //}
            //catch {
            //    result = false;
            //}
            return result;
        }

        public class PostingDelegate
        {
            public delegate void MassageChangeEvent(object sender, string text);
            public event MassageChangeEvent MassageChanged;

            public void Update(string message)
            {
                if (MassageChanged != null) MassageChanged(this, message);
            }
        }

        private bool CheckValidTrans(GnMstCoProfileSpare oCoProfileSpare)
        {
            bool result = false;
            //try
            //{

                //Clear previous error data
                var q1 = string.Format(@"DELETE FROM gnErrorRaiseDtl");
                ctx.Database.ExecuteSqlCommand(q1);

                var q2 = string.Format(@"DELETE FROM gnErrorRaiseHdr");
                ctx.Database.ExecuteSqlCommand(q2);

                DataSet ds = new DataSet();
                SqlConnection con = (SqlConnection)ctx.Database.Connection;
                SqlCommand cmd = con.CreateCommand();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                cmd.CommandText = "sp_CheckValidTrans";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                cmd.Parameters.AddWithValue("@BegDate", oCoProfileSpare.PeriodBeg.ToString("yyyy/MM/dd"));
                cmd.Parameters.AddWithValue("@EndDate", oCoProfileSpare.PeriodEnd.ToString("yyyy/MM/dd"));

                da.Fill(ds);
                // object[] paramSp = { CompanyCode, BranchCode, oCoProfileSpare.PeriodBeg.ToString("yyyy/MM/dd"), oCoProfileSpare.PeriodEnd.ToString("yyyy/MM/dd") };

                //var querySp = "exec sp_CheckValidTrans @p0,@p1,@p2,@p3";

                //result = ctx.Database.(querySp, paramSp) > 0;

                bool istrue = true;
                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Rows.Count > 0)
                    {
                        istrue = false;
                        break;
                    }
                }
                if (istrue)
                {
                    result = true;
                }
                else
                {
                    //Clear previous error data
                    q1 = string.Format(@"DELETE FROM gnErrorRaiseDtl");
                    ctx.Database.ExecuteSqlCommand(q1);

                    q2 = string.Format(@"DELETE FROM gnErrorRaiseHdr");
                    ctx.Database.ExecuteSqlCommand(q2);

                    //Insert ke gnErrorRaiseHdr
                    var q3 = string.Format(@"
                    INSERT INTO gnErrorRaiseHdr 
                    (FiscalYear, FiscalMonth, ProfitCenter, SeqNo, CompanyCode, BranchCode)
                    VALUES 
                    ('{2}', '{3}', '{4}', '{5}', '{0}', '{1}')",
                    CompanyCode, BranchCode, oCoProfileSpare.FiscalYear, oCoProfileSpare.FiscalMonth, ProfitCenter, 1);
                    ctx.Database.ExecuteSqlCommand(q3);

                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            // insert ke gnErrorRaiseDtl
                            foreach (DataRow row in dt.Rows)
                            {
                                var q4 = string.Format(@"
                                INSERT INTO gnErrorRaiseDtl 
                                (FiscalYear, FiscalMonth, ProfitCenter, SeqNo, TableName, DocumentNo, Status, TypeOfGoods)
                                VALUES 
                                ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')"
                                , oCoProfileSpare.FiscalYear, oCoProfileSpare.FiscalMonth, ProfitCenter, 1, row["TableName"], row["DocNo"], row["Status"], row["TypeOfGoods"]);
                                ctx.Database.ExecuteSqlCommand(q4);
                            }
                        }
                    }
                }

                return result;
            }
            //catch (Exception e) {
            //    throw new Exception(e.Message);
            //}
            
        //}

        public JsonResult Print() 
        {
            var oCoProfileSpare = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
            return Json(oCoProfileSpare);
        }
        

    }
}
