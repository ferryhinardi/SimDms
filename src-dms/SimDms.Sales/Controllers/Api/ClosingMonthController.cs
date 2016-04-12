using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common.Models;
using SimDms.Sales.Models.Reports;
//using SimDms.Sales.Models;

namespace SimDms.Sales.Controllers.Api
{
    public class ClosingMonthController : BaseController
    {
        private static string msg = "";
        private static bool IsPrint = false;

        public JsonResult Default()
        {
            var data = ctx.CoProfileSaleses.Find(CompanyCode, BranchCode);
            if (data != null)
            {
                return Json(new
                {
                    Year = data.FiscalYear,
                    Period = data.FiscalYear + data.FiscalPeriod.ToString().PadLeft(2, '0'),
                    PeriodDesc = "Dari Tanggal " + data.PeriodBeg.Value.ToString("dd-MMM-yyyy") + " S/D " + data.PeriodEnd.Value.ToString("dd-MMM-yyyy"),
                    PeriodBeg = data.PeriodBeg,
                    PeriodEnd = data.PeriodEnd,
                    PeriodeNum = data.FiscalPeriod
                });
            }

            return Json(null);
        }

        public JsonResult ValidateClosing()
        {
            bool result = false;
            int statusOM = 0; bool statusFiscal = false;
            var data = ctx.CoProfileSaleses.Find(CompanyCode, BranchCode);

            if (!ValidateB4Closing()) return Json(new { success = result });

            if (GetPeriod(data.PeriodBeg.Value) != null)
            {
                statusOM = GetPeriod(data.PeriodBeg.Value).StatusSales.Value;
                statusFiscal = GetPeriod(data.PeriodBeg.Value).FiscalStatus.Value;
            }

            if (statusOM == 2)
            {
                msg = "Status Periode Sales sudah di tutup";
                return Json(new { success = result, message = msg });
            }
            if (statusFiscal == false)
            {
                msg = "Periode tidak aktif";
                return Json(new { success = result, message = msg });
            }
            if (statusOM == 0)
            {
                msg = "Periode belum aktif";
                return Json(new { success = result, message = msg });
            }

            if (GetPeriod(data.PeriodBeg.Value.AddMonths(1).Date) == null)
            {
                msg = "Periode berikutnya belum aktif";
                return Json(new { success = result, message = msg });
            }

            if (!CheckTransaction(data.PeriodBeg.Value))
            {
                return Json(new { success = result, message = msg, print = IsPrint });
            }

            if (!CheckBPUHPP(data.PeriodBeg.Value, data.PeriodEnd.Value))
            {
                return Json(new { success = result, message = msg, print = IsPrint });
            }

            return Json(new { success = true });
        }

        public JsonResult ClosingMonth()
        {
            var data = ctx.CoProfileSaleses.Find(CompanyCode, BranchCode);

            int fiscalYear = (int)data.FiscalYear;
            int periodeNum = (int)data.FiscalPeriod;
            DateTime closingDate = data.PeriodBeg.Value;
            bool result = false;
            var rec = ctx.CoProfileSaleses.Find(CompanyCode, BranchCode);

            if (rec != null)
            {
                var rowPeriod = GetNextPeriod(fiscalYear, periodeNum);
                if (rowPeriod != null)
                {
                    using (var tran = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            rec.FiscalYear = rowPeriod.FiscalYear;
                            rec.FiscalMonth = rowPeriod.FiscalMonth;
                            rec.FiscalPeriod = rowPeriod.PeriodeNum;
                            rec.PeriodBeg = rowPeriod.FromDate;
                            rec.PeriodEnd = rowPeriod.EndDate;
                            rec.TransDate = rowPeriod.FromDate;
                            rec.LastUpdateBy = CurrentUser.UserId;
                            rec.LastUpdateDate = DateTime.Now;
                            result = ctx.SaveChanges() >= 0;
                            if (result == true)
                            {
                                //Update Inventory Perlengkapan
                                string qSql = "";
                                qSql = "INSERT INTO omTrInventQtyPerlengkapan(CompanyCode, BranchCode, [Year], [Month], PerlengkapanCode, QuantityBeginning, QuantityIn, QuantityOut";
                                qSql += ", QuantityEnding, Remark, [Status], CreatedBy, CreatedDate, IsLocked)";
                                qSql += " SELECT CompanyCode, BranchCode, @Fiscal, @Month, PerlengkapanCode, QuantityBeginning = QuantityEnding, 0 AS QuantityIn, 0 AS QuantityOut";
                                qSql += ", QuantityEnding, '', '', @UserID, GETDATE(), 0  FROM OmTrInventQtyPerlengkapan";
                                qSql += " WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND [Year] = @Year AND [Month] = @PeriodeNum";
                                qSql = qSql.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
                                qSql = qSql.Replace("@BranchCode", string.Format("'{0}'", BranchCode));
                                qSql = qSql.Replace("@Year", string.Format("'{0}'", closingDate.Year.ToString()));
                                qSql = qSql.Replace("@PeriodeNum", string.Format("'{0}'", closingDate.Month.ToString()));
                                qSql = qSql.Replace("@UserID", string.Format("'{0}'", CurrentUser.UserId));
                                int fiscal = Convert.ToInt16(fiscalYear);
                                int perNum = Convert.ToInt16(periodeNum) + 1;
                                if (periodeNum == 12)
                                {
                                    fiscal = Convert.ToInt16(fiscalYear) + 1;
                                    perNum = 1;
                                }
                                qSql = qSql.Replace("@Fiscal", string.Format("'{0}'", rec.PeriodBeg.Value.Year.ToString()));
                                qSql = qSql.Replace("@Month", string.Format("'{0}'", rec.PeriodBeg.Value.Month.ToString()));
                                result = ctx.Database.ExecuteSqlCommand(qSql) > 0;

                                //Update Inventory Vehicle

                                qSql = "INSERT INTO omTrInventQtyVehicle(CompanyCode, BranchCode, [Year], [Month], SalesModelCode, SalesModelYear, ColourCode, WarehouseCode";
                                qSql += ", QtyIn, Alocation, QtyOut, BeginningOH, EndingOH, BeginningAV, EndingAV, Remark, Status, CreatedBy, CreatedDate, IsLocked)";
                                qSql += " SELECT CompanyCode, BranchCode, @Fiscal, @Month, SalesModelCode, SalesModelYear, ColourCode, WarehouseCode";
                                qSql += ", 0 AS QtyIn, 0 AS Alocation, 0 AS QtyOut, BeginningOH = EndingOH, EndingOH, BeginningAV = EndingAV, EndingAV";
                                qSql += ", '', '', @UserID, GETDATE(), 0  FROM omTrInventQtyVehicle";
                                qSql += " WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND [Year] = @Year AND [Month] = @PeriodeNum";
                                qSql = qSql.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
                                qSql = qSql.Replace("@BranchCode", string.Format("'{0}'", BranchCode));
                                qSql = qSql.Replace("@Year", string.Format("'{0}'", closingDate.Year.ToString()));
                                qSql = qSql.Replace("@PeriodeNum", string.Format("'{0}'", closingDate.Month.ToString()));
                                qSql = qSql.Replace("@UserID", string.Format("'{0}'", CurrentUser.UserId));
                                qSql = qSql.Replace("@Fiscal", string.Format("'{0}'", rec.PeriodBeg.Value.Year.ToString()));
                                qSql = qSql.Replace("@Month", string.Format("'{0}'", rec.PeriodBeg.Value.Month.ToString()));
                                result = ctx.Database.ExecuteSqlCommand(qSql) > 0;

                                qSql = "UPDATE GnMstPeriode SET StatusSales = '2' WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode";
                                qSql += " AND FiscalYear = @FiscalYear AND PeriodeNum = @PeriodeNum";
                                qSql = qSql.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
                                qSql = qSql.Replace("@BranchCode", string.Format("'{0}'", BranchCode));
                                qSql = qSql.Replace("@FiscalYear", string.Format("'{0}'", fiscalYear));
                                qSql = qSql.Replace("@PeriodeNum", string.Format("'{0}'", periodeNum));
                                result = ctx.Database.ExecuteSqlCommand(qSql) > 0;

                                //Input Dealer Stock History

                                object[] parameters = { CompanyCode, BranchCode, closingDate.Year.ToString(), closingDate.Month.ToString(), CurrentUser.UserId };
                                var query = "exec uspfn_InsertHstDealerStock @p0,@p1,@p2,@p3,@p4";
                                result = ctx.Database.ExecuteSqlCommand(query, parameters) > 0;

                                //Update Document Sequence
                                if (rec.PeriodBeg.Value.Month == 1)
                                {
                                    var recDoc = ctx.GnMstDocuments.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.ProfitCenterCode == "100");
                                    recDoc.DocumentYear = rec.PeriodBeg.Value.Year;
                                    recDoc.DocumentSequence = 0;

                                    result = ctx.SaveChanges() > 0;
                                }

                                msg = "Proses Tutup Bulan berhasil";
                            }
                            if (result)
                            {
                                tran.Commit();
                            }
                            else
                            {
                                tran.Rollback();
                            }

                            return Json(new { success = result, message = msg });
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            result = false;
                            msg = "Proses Tutup Bulan gagal / Periode berikutnya belum aktif";
                        }
                    }
                }
                else
                {
                    msg = "Data Period Null";
                }
            }
            else
            {
                msg = "Data Master CoProfileSales tidak ditemukan.";
            }

            return Json(new { success = result, message = msg });
        }

        private bool ValidateB4Closing()
        {
            bool result = false;
            try
            {
                var query = string.Format(@"exec uspfn_OmUtlCheckB4Closing {0},{1},'{2}'", CompanyCode, BranchCode, CurrentUser.UserId);
                ctx.Database.ExecuteSqlCommand(query);
                result = true;
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        private Period GetPeriod(DateTime docDate)
        {
            var data = ctx.MstPeriod.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && docDate >= a.FromDate && docDate <= a.EndDate);

            return data;
        }

        private bool CheckTransaction(DateTime date)
        {
            bool isValid = true;
            if (CreateNotValidTransaction(date.ToString("dd-MMM-yyyy")))
            {
                isValid = false;
                msg = "Tidak dapat tutup bulan \r\n  ada transaksi yang masih pending. \r\n Lihat Laporan Daftar Dokumen Pending ?";
                IsPrint = true;
            }
            return isValid;
        }

        private bool CreateNotValidTransaction(string date)
        {
            bool result = false;
            try
            {
                var query = string.Format("exec usprpt_OmRpDocPending {0},{1},'{2}'", CompanyCode, BranchCode, date);

                var qObj = ctx.Database.SqlQuery<OmRpDocPending>(query);
                // = (int)qObj;
                result = (qObj.ToList().Count == 0) ? false : true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private bool CheckBPUHPP(DateTime PeriodBeg, DateTime PeriodEnd)
        {
            bool isValid = true;
            if (!IsBPUinHPP(PeriodBeg, PeriodEnd))
            {
                isValid = CreateBPUNotInHPP(PeriodBeg, PeriodEnd);
                IsPrint = true;
            }
            return isValid;
        }

        private bool IsBPUinHPP(DateTime fromDate, DateTime endDate)
        {
            string sSql = ""; bool result = false;
            sSql += " SELECT BPUNo FROM OmTrPurchaseBPU";
            sSql += " WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode";
            sSql += " AND Status = '2'";
            sSql += " AND CONVERT(VARCHAR, BPUDate, 112) BETWEEN @FromDate AND @EndDate";
            sSql += " AND BPUNo NOT IN (SELECT BPUNo FROM OmTrPurchaseHPPDetail WHERE CompanyCode = OmTrPurchaseBPU.CompanyCode";
            sSql += " AND BranchCode = OmTrPurchaseBPU.BranchCode)";

            sSql = sSql.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
            sSql = sSql.Replace("@BranchCode", string.Format("'{0}'", BranchCode));
            sSql = sSql.Replace("@FromDate", string.Format("'{0}'", fromDate.ToString("yyyyMMdd")));
            sSql = sSql.Replace("@EndDate", string.Format("'{0}'", endDate.ToString("yyyyMMdd")));

            object obj = ctx.Database.ExecuteSqlCommand(sSql);
            int intObj = (int)obj;
            result = (obj == null || obj.ToString() == string.Empty || intObj == -1) ? true : false;
            return result;
        }

        private bool CreateBPUNotInHPP(DateTime fromDate, DateTime endDate)
        {
            string sSql = ""; bool result = false;
            sSql = "SELECT * INTO #f1 FROM (";
            sSql += " SELECT CompanyCode, BranchCode, CONVERT(VARCHAR(20), BPUNo) AS DocNo, BPUDate AS DocDate";
            sSql += " , 'DO : ' + RefferenceDONo + ' SJ : ' + RefferenceSJNo AS RefNo, RefferenceDODate AS RefDate";
            sSql += ", 'Belum Ada HPP' AS StatusClosed, CONVERT(VARCHAR(30), 'BPU') AS TransType";
            sSql += " FROM OmTrPurchaseBPU";
            sSql += " WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode";
            sSql += " AND Status = '2'";
            sSql += " AND CONVERT(VARCHAR, BPUDate, 112) BETWEEN @FromDate AND @EndDate";
            sSql += " AND BPUNo NOT IN (SELECT BPUNo FROM OmTrPurchaseHPPDetail WHERE CompanyCode = OmTrPurchaseBPU.CompanyCode";
            sSql += " AND BranchCode = OmTrPurchaseBPU.BranchCode)";
            sSql += "  ) #f1";

            sSql += "  DELETE OmListClosingMonth WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode";
            sSql += "  AND CONVERT(VARCHAR, DocDate, 112) BETWEEN @FromDate AND @EndDate";

            sSql += "  INSERT INTO OmListClosingMonth(CompanyCode, BranchCode, DocNo, DocDate, RefNo, RefDate, TransType, StatusClosed)";
            sSql += "  SELECT CompanyCode, BranchCode, DocNo, DocDate, RefNo, RefDate, TransType, StatusClosed FROM #f1";

            sSql += " DROP TABLE #f1";

            sSql = sSql.Replace("@CompanyCode", string.Format("'{0}'", CompanyCode));
            sSql = sSql.Replace("@BranchCode", string.Format("'{0}'", BranchCode));
            sSql = sSql.Replace("@FromDate", string.Format("'{0}'", fromDate.ToString("yyyyMMdd")));
            sSql = sSql.Replace("@EndDate", string.Format("'{0}'", endDate.ToString("yyyyMMdd")));

            result = ctx.Database.ExecuteSqlCommand(sSql) > 0;
            return result;
        }

        private PeriodViewModel GetNextPeriod(int fiscalYear, int periodeNum)
        {
            //var p0 = new SqlParameter("@p0", CompanyCode);
            //p0.DbType = DbType.String;

            //var p1 = new SqlParameter("@p1", BranchCode);
            //p1.DbType = DbType.String;

            //var p2 = new SqlParameter("@p2", fiscalYear);
            //p2.DbType = DbType.Int16;

            //var p3 = new SqlParameter("@p3", periodeNum);
            //p3.DbType = DbType.Int16;



            object[] parameters = { CompanyCode, BranchCode, fiscalYear, periodeNum == 12 ? 1 : periodeNum };

            string s = "";
            s = "SELECT TOP 1 FiscalYear, FiscalMonth, PeriodeNum, FromDate, EndDate";
            s += " FROM gnMstPeriode WITH (NoLock, NoWait)";
            s += " WHERE CompanyCode = @p0 AND BranchCode = @p1";
            if (periodeNum == 12)
                s += " AND FiscalYear > @p2 AND PeriodeNum = @p3";
            else s += " AND FiscalYear = @p2 AND PeriodeNum > @p3";
            s += " AND FiscalStatus = '1'";
            s += " ORDER BY FiscalYear, PeriodeNum";

            var data = ctx.Database.SqlQuery<PeriodViewModel>(s, parameters);
            //.FirstOrDefault();

            var period = new PeriodViewModel();
            if (data.Count() > 0 )
                period = data.FirstOrDefault();

            return period;
        }
    }
}