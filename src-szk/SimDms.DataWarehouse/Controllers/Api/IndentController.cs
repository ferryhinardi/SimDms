using SimDms.DataWarehouse.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Transactions;
using ClosedXML;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using GemBox.Spreadsheet;
using System.Globalization;
using GeLang;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class IndentController : BaseController
    {
        static readonly object lck = new object();
        public JsonResult Default()
        {
            var groupArea = String.Empty;
            var OutletCode = BranchCode;
            var CoProfiles = ctx.CoProfiles.FirstOrDefault();
            var roleid = CurrentUser.RoleId;

            if (CompanyCode != null && CompanyCode != "")
            {
                CoProfiles = ctx.CoProfiles.Where(a => a.CompanyCode == CompanyCode).FirstOrDefault();
                if (BranchCode == null)
                {
                    OutletCode = CoProfiles.BranchCode;
                }
                var Outlet = ctx.GnMstDealerOutletMappingNews.Where(a => a.DealerCode == CompanyCode && a.OutletCode == OutletCode).FirstOrDefault();
                groupArea = Outlet.GroupNo.ToString();
            }
            
            return Json(new
            {
                success = true,
                data = new
                {
                    CompanyCode = CompanyCode,
                    pType = CoProfiles.ProductType,
                    IndentDate = DateTime.Now,
                    QuantityInquiry = 1,
                    Date = DateTime.Now,
                    DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    DateTo = DateTime.Now,
                    LastProgress = "SPK",
                    LastUpdateStatus = DateTime.Now,
                    ActivityDate = DateTime.Now,
                    NextFollowUpDate = DateTime.Now,
                    Outlet = BranchCode,
                    GroupArea = groupArea == String.Empty ? "" : groupArea,
                    RoleID = roleid
                }
            });
        }

        public JsonResult LastStatus()
        {
            List<object> LastStatus = new List<object>();

            LastStatus.Add(new { value = "SPK", text = "SPK" });
            LastStatus.Add(new { value = "LOST", text = "LOST" });

            return Json(LastStatus);
        }

        public JsonResult GetTeamLeader(string Company, string id)
        {
            var list = ctx.HrEmployees.Where(p => p.CompanyCode == Company && p.TeamLeader == id && p.PersonnelStatus == "1")
                .Select(p => new { value = p.EmployeeID, text = p.EmployeeName })
                .ToList();
            return Json(list);

        }

        public JsonResult Save(pmIndent model)
        {
            var Quota = ctx.pmQuotas.Where(a => a.CompanyCode == CompanyCode && a.PeriodYear == DateTime.Now.Year).FirstOrDefault();

            if (Quota == null) { return Json(new { success = false, message = "Master Quota masih kosong!" }); }

            var QuotaBy = Quota.QuotaBy;
            var MonthDesc = "";
            var userID = CurrentUser.Username;
            var newNumber = 1;
            DateTime dtStartRangeInq = DateTime.Now.AddDays(-7);
            DateTime dtEndRangeInq = DateTime.Now.AddDays(7);
            var messageDev = "";

            using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
               new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                bool bolNew = false;

                lock (lck)
                {
                    var record = ctx.pmIndents.Find(model.IndentNumber, model.BranchCode, CompanyCode);
                    var currentDate = DateTime.Now;
                    if (record == null)
                    {
                        //var CekData = ctx.pmIndentAdditionals.Where(a => a.CompanyCode == model.CompanyCode && a.SPKNo == model.SPKNo).FirstOrDefault();
                        var DealerName = ctx.GnMstDealerMappings.Where(a => a.DealerCode == model.CompanyCode).FirstOrDefault().DealerName;

                        //if (CekData != null)
                        //{
                        //    return Json(new { success = false, message = "No SPK " + model.SPKNo + " Sudah ada di Dealer " + DealerName + " !" });
                        //}

                        if (model.IndentDate < dtStartRangeInq || model.IndentDate > dtEndRangeInq)
                        {
                            return Json(new { success = false, message = "Input Tanggal Inquiry harus berada di range tanggal " + dtStartRangeInq.ToString("dd-MMM-yyyy") + " s/d " + dtEndRangeInq.ToString("dd-MMM-yyyy") });
                        }
                        else if (model.NamaProspek.Trim().Length < 3)
                        {
                            return Json(new { success = false, message = "Nama Customer minimal 3 huruf !" });
                        }
                        else
                        {
                            var PmKdp = ctx.pmIndents.FirstOrDefault();
                            if (PmKdp != null) { newNumber = ctx.pmIndents.Select(p => p.IndentNumber).Max() + 1; }
                            var outlet = ctx.PmBranchOutlets.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == model.BranchCode).FirstOrDefault();
                            record = new pmIndent
                            {
                                IndentNumber = newNumber,
                                CompanyCode = CompanyCode,
                                BranchCode = model.BranchCode,
                                CreatedBy = userID,
                                CreationDate = currentDate,
                                OutletID = (outlet == null) ? "" : outlet.OutletID,
                            };
                            ctx.pmIndents.Add(record);
                            bolNew = true;
                        }

                    }

                    //*Cek Qty Qouta di pmQouta
                    if (model.DeliveryMonth != null && model.DeliveryYear != null)
                    {
                        if (QuotaBy == "TYP")
                        {
                            #region QuotaBy=TYP
                            if (model.TipeKendaraan != record.TipeKendaraan)
                            {
                                var tahun = model.IndentDate.Value.Year;
                                var bulan = model.IndentDate.Value.Month;

                                var oldyear = record.DeliveryYear;
                                var oldmonth = record.DeliveryMonth;

                                record.DeliveryYear = null;
                                record.DeliveryMonth = null;

                                ctx.Database.ExecuteSqlCommand(@"update pmQuota
                                                         set IndentQty = IndentQty - 1
                                                         where CompanyCode = '" + CompanyCode + "' and TipeKendaraan = '" + record.TipeKendaraan +
                                                              "' and PeriodYear = '" + oldyear + "' and PeriodMonth = '" + oldmonth + "'");

                                var quota = ctx.pmQuotas.Where(a => a.CompanyCode == CompanyCode && a.TipeKendaraan == model.TipeKendaraan && a.PeriodYear == tahun && a.PeriodMonth >= bulan)
                                                            .OrderBy(a => a.PeriodMonth).ToList();
                                foreach (pmQuota Qty in quota)
                                {
                                    if (Qty.QuotaQty != Qty.IndentQty && Qty.IndentQty < Qty.QuotaQty)
                                    {
                                        int month = Convert.ToInt32(Qty.PeriodMonth);
                                        switch (month)
                                        {
                                            case 1: MonthDesc = "Januari"; bulan = 1; break;
                                            case 2: MonthDesc = "Februari"; bulan = 2; break;
                                            case 3: MonthDesc = "Maret"; bulan = 3; break;
                                            case 4: MonthDesc = "April"; bulan = 4; break;
                                            case 5: MonthDesc = "Mei"; bulan = 5; break;
                                            case 6: MonthDesc = "Juni"; bulan = 6; break;
                                            case 7: MonthDesc = "Juli"; bulan = 7; break;
                                            case 8: MonthDesc = "Agustus"; bulan = 8; break;
                                            case 9: MonthDesc = "September"; bulan = 9; break;
                                            case 10: MonthDesc = "Oktober"; bulan = 10; break;
                                            case 11: MonthDesc = "November"; bulan = 11; break;
                                            case 12: MonthDesc = "Desember"; bulan = 12; break;
                                        }

                                        record.DeliveryYear = Qty.PeriodYear;
                                        record.DeliveryMonth = Qty.PeriodMonth;

                                        ctx.Database.ExecuteSqlCommand(@"update pmQuota
                                                         set IndentQty = IndentQty + 1
                                                         where CompanyCode = '" + CompanyCode + "' and TipeKendaraan = '" + model.TipeKendaraan +
                                                                      "' and PeriodYear = '" + tahun + "' and PeriodMonth = '" + bulan + "'");
                                        break;
                                    }
                                }
                            }
                            #endregion
                        }
                        else if (QuotaBy == "VAR")
                        {
                            #region QuotaBy=VAR
                            if (model.TipeKendaraan != record.TipeKendaraan || model.Variant != record.Variant)
                            {
                                var tahun = model.IndentDate.Value.Year;
                                var bulan = model.IndentDate.Value.Month;

                                var oldyear = record.DeliveryYear;
                                var oldmonth = record.DeliveryMonth;

                                record.DeliveryYear = null;
                                record.DeliveryMonth = null;

//                                ctx.Database.ExecuteSqlCommand(@"update pmQuota
//                                                         set IndentQty = IndentQty - 1
//                                                         where CompanyCode = '" + CompanyCode + "' and TipeKendaraan = '" + record.TipeKendaraan +
//                                                              "' and Variant = '" + record.Variant +
//                                                              "' and PeriodYear = '" + oldyear + "' and PeriodMonth = '" + oldmonth + "'");

//                                ctx.Database.ExecuteSqlCommand("exec uspfn_RecalculateFromPmQuota '" + model.CompanyCode + "','" + record.TipeKendaraan + "','" + record.Variant + "'," + record.DeliveryMonth + "," + record.DeliveryYear + "");


                                var quota = ctx.pmQuotas.Where(a => a.CompanyCode == CompanyCode && a.TipeKendaraan == model.TipeKendaraan && a.Variant == model.Variant
                                                            && a.PeriodYear == tahun && a.PeriodMonth >= bulan)
                                                            .OrderBy(a => a.PeriodMonth).ToList();
                                foreach (pmQuota Qty in quota)
                                {
                                    if (Qty.QuotaQty != Qty.IndentQty && Qty.IndentQty < Qty.QuotaQty)
                                    {
                                        int month = Convert.ToInt32(Qty.PeriodMonth);
                                        switch (month)
                                        {
                                            case 1: MonthDesc = "Januari"; bulan = 1; break;
                                            case 2: MonthDesc = "Februari"; bulan = 2; break;
                                            case 3: MonthDesc = "Maret"; bulan = 3; break;
                                            case 4: MonthDesc = "April"; bulan = 4; break;
                                            case 5: MonthDesc = "Mei"; bulan = 5; break;
                                            case 6: MonthDesc = "Juni"; bulan = 6; break;
                                            case 7: MonthDesc = "Juli"; bulan = 7; break;
                                            case 8: MonthDesc = "Agustus"; bulan = 8; break;
                                            case 9: MonthDesc = "September"; bulan = 9; break;
                                            case 10: MonthDesc = "Oktober"; bulan = 10; break;
                                            case 11: MonthDesc = "November"; bulan = 11; break;
                                            case 12: MonthDesc = "Desember"; bulan = 12; break;
                                        }

//                                        record.DeliveryYear = Qty.PeriodYear;
//                                        record.DeliveryMonth = Qty.PeriodMonth;

//                                        ctx.Database.ExecuteSqlCommand(@"update pmQuota
//                                                         set IndentQty = IndentQty + 1
//                                                         where CompanyCode = '" + CompanyCode + "' and TipeKendaraan = '" + model.TipeKendaraan +
//                                                                      "' and Variant = '" + model.Variant +
//                                                                      "' and PeriodYear = '" + tahun + "' and PeriodMonth = '" + bulan + "'");

                                        //ctx.Database.ExecuteSqlCommand("exec uspfn_RecalculateFromPmQuota '" + model.CompanyCode + "','" + model.TipeKendaraan + "','" + model.Variant + "'," + bulan + "," + tahun + "");
                                        break;
                                    }
                                }

                                var oldTipeKendaraan = record.TipeKendaraan;
                                var oldTipeVariant = record.Variant;
                                
                                record.TipeKendaraan = model.TipeKendaraan;
                                record.Variant = model.Variant;
                                record.Transmisi = model.Transmisi;
                                record.ColourCode = model.ColourCode;

                                ctx.SaveChanges();

                                ctx.Database.ExecuteSqlCommand("exec uspfn_RecalculateFromPmQuota '" + record.CompanyCode + "','" + oldTipeKendaraan + "','" + oldTipeVariant + "'," + oldmonth + "," + oldyear + "");
                                ctx.Database.ExecuteSqlCommand("exec uspfn_RecalculateFromPmQuota '" + record.CompanyCode + "','" + record.TipeKendaraan + "','" + record.Variant + "'," + bulan + "," + tahun + "");
                            }
                            #endregion
                        }
                        else if (QuotaBy == "COL")
                        {
                            #region QuotaBy=COL
                            if (model.TipeKendaraan != record.TipeKendaraan || model.Variant != record.Variant || model.ColourCode != record.ColourCode)
                            {
                                var tahun = model.IndentDate.Value.Year;
                                var bulan = model.IndentDate.Value.Month;

                                var oldyear = record.DeliveryYear;
                                var oldmonth = record.DeliveryMonth;

                                record.DeliveryYear = null;
                                record.DeliveryMonth = null;

                                ctx.Database.ExecuteSqlCommand(@"update pmQuota
                                                         set IndentQty = IndentQty - 1
                                                         where CompanyCode = '" + CompanyCode + "' and TipeKendaraan = '" + record.TipeKendaraan +
                                                              "' and Variant = '" + record.Variant + "' and ColourCode = '" + record.ColourCode +
                                                              "' and PeriodYear = '" + oldyear + "' and PeriodMonth = '" + oldmonth + "'");

                                var quota = ctx.pmQuotas.Where(a => a.CompanyCode == CompanyCode && a.TipeKendaraan == model.TipeKendaraan && a.Variant == model.Variant
                                                            && a.ColourCode == model.ColourCode && a.PeriodYear == tahun && a.PeriodMonth >= bulan)
                                                            .OrderBy(a => a.PeriodMonth).ToList();
                                foreach (pmQuota Qty in quota)
                                {
                                    if (Qty.QuotaQty != Qty.IndentQty && Qty.IndentQty < Qty.QuotaQty)
                                    {
                                        int month = Convert.ToInt32(Qty.PeriodMonth);
                                        switch (month)
                                        {
                                            case 1: MonthDesc = "Januari"; bulan = 1; break;
                                            case 2: MonthDesc = "Februari"; bulan = 2; break;
                                            case 3: MonthDesc = "Maret"; bulan = 3; break;
                                            case 4: MonthDesc = "April"; bulan = 4; break;
                                            case 5: MonthDesc = "Mei"; bulan = 5; break;
                                            case 6: MonthDesc = "Juni"; bulan = 6; break;
                                            case 7: MonthDesc = "Juli"; bulan = 7; break;
                                            case 8: MonthDesc = "Agustus"; bulan = 8; break;
                                            case 9: MonthDesc = "September"; bulan = 9; break;
                                            case 10: MonthDesc = "Oktober"; bulan = 10; break;
                                            case 11: MonthDesc = "November"; bulan = 11; break;
                                            case 12: MonthDesc = "Desember"; bulan = 12; break;
                                        }

                                        record.DeliveryYear = Qty.PeriodYear;
                                        record.DeliveryMonth = Qty.PeriodMonth;

                                        ctx.Database.ExecuteSqlCommand(@"update pmQuota
                                                         set IndentQty = IndentQty + 1
                                                         where CompanyCode = '" + CompanyCode + "' and TipeKendaraan = '" + model.TipeKendaraan +
                                                                      "' and Variant = '" + model.Variant + "' and ColourCode = '" + model.ColourCode +
                                                                      "' and PeriodYear = '" + tahun + "' and PeriodMonth = '" + bulan + "'");
                                        break;
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        if (QuotaBy == "TYP")
                        {
                            #region QuotaBy=TYP
                            var tahun = model.IndentDate.Value.Year;
                            var bulan = model.IndentDate.Value.Month;

                            var quota = ctx.pmQuotas.Where(a => a.CompanyCode == CompanyCode && a.TipeKendaraan == model.TipeKendaraan
                                                        && a.PeriodYear == tahun && a.PeriodMonth >= bulan)
                                                        .OrderBy(a => a.PeriodMonth).ToList();
                            foreach (pmQuota Qty in quota)
                            {
                                if (Qty.QuotaQty != Qty.IndentQty && Qty.IndentQty < Qty.QuotaQty)
                                {
                                    int month = Convert.ToInt32(Qty.PeriodMonth);
                                    switch (month)
                                    {
                                        case 1: MonthDesc = "Januari"; break;
                                        case 2: MonthDesc = "Februari"; break;
                                        case 3: MonthDesc = "Maret"; break;
                                        case 4: MonthDesc = "April"; break;
                                        case 5: MonthDesc = "Mei"; break;
                                        case 6: MonthDesc = "Juni"; break;
                                        case 7: MonthDesc = "Juli"; break;
                                        case 8: MonthDesc = "Agustus"; break;
                                        case 9: MonthDesc = "September"; break;
                                        case 10: MonthDesc = "Oktober"; break;
                                        case 11: MonthDesc = "November"; break;
                                        case 12: MonthDesc = "Desember"; break;
                                    }

                                    record.DeliveryYear = Qty.PeriodYear;
                                    record.DeliveryMonth = Qty.PeriodMonth;

                                    ctx.Database.ExecuteSqlCommand(@"update pmQuota
                                                         set IndentQty = IndentQty + 1
                                                         where CompanyCode = '" + CompanyCode + "' and TipeKendaraan = '" + model.TipeKendaraan +
                                                                  "' and PeriodYear = '" + tahun + "' and PeriodMonth = '" + bulan + "'");
                                    break;
                                }
                            }
                            #endregion
                        }
                        else if (QuotaBy == "VAR")
                        {
                            #region QuotaBy=VAR
                            var tahun = model.IndentDate.Value.Year;
                            var bulan = model.IndentDate.Value.Month;

                            var quota = ctx.pmQuotas.Where(a => a.CompanyCode == CompanyCode && a.TipeKendaraan == model.TipeKendaraan && a.Variant == model.Variant
                                                        && a.PeriodYear == tahun && a.PeriodMonth >= bulan)
                                                        .OrderBy(a => a.PeriodMonth).ToList();
                            foreach (pmQuota Qty in quota)
                            {
                                if (Qty.QuotaQty != Qty.IndentQty && Qty.IndentQty < Qty.QuotaQty)
                                {
                                    int month = Convert.ToInt32(Qty.PeriodMonth);
                                    switch (month)
                                    {
                                        case 1: MonthDesc = "Januari"; bulan = 1; break;
                                        case 2: MonthDesc = "Februari"; bulan = 2; break;
                                        case 3: MonthDesc = "Maret"; bulan = 3; break;
                                        case 4: MonthDesc = "April"; bulan = 4; break;
                                        case 5: MonthDesc = "Mei"; bulan = 5; break;
                                        case 6: MonthDesc = "Juni"; bulan = 6; break;
                                        case 7: MonthDesc = "Juli"; bulan = 7; break;
                                        case 8: MonthDesc = "Agustus"; bulan = 8; break;
                                        case 9: MonthDesc = "September"; bulan = 9; break;
                                        case 10: MonthDesc = "Oktober"; bulan = 10; break;
                                        case 11: MonthDesc = "November"; bulan = 11; break;
                                        case 12: MonthDesc = "Desember"; bulan = 12; break;
                                    }

                                    record.DeliveryYear = Qty.PeriodYear;
                                    record.DeliveryMonth = Qty.PeriodMonth;

                                    ctx.Database.ExecuteSqlCommand(@"update pmQuota
                                                         set IndentQty = IndentQty + 1
                                                         where CompanyCode = '" + CompanyCode + "' and TipeKendaraan = '" + model.TipeKendaraan +
                                                                  "' and Variant = '" + model.Variant +
                                                                  "' and PeriodYear = '" + tahun + "' and PeriodMonth = '" + bulan + "'");
                                    break;
                                }
                            }
                            #endregion
                        }
                        else if (QuotaBy == "COL")
                        {
                            #region QuotaBy=COL
                            var tahun = model.IndentDate.Value.Year;
                            var bulan = model.IndentDate.Value.Month;

                            var quota = ctx.pmQuotas.Where(a => a.CompanyCode == CompanyCode && a.TipeKendaraan == model.TipeKendaraan && a.Variant == model.Variant
                                                        && a.Transmisi == model.Transmisi && a.ColourCode == model.ColourCode && a.PeriodYear == tahun && a.PeriodMonth >= bulan)
                                                        .OrderBy(a => a.PeriodMonth).ToList();
                            foreach (pmQuota Qty in quota)
                            {
                                if (Qty.QuotaQty != Qty.IndentQty && Qty.IndentQty < Qty.QuotaQty)
                                {
                                    int month = Convert.ToInt32(Qty.PeriodMonth);
                                    switch (month)
                                    {
                                        case 1: MonthDesc = "Januari"; bulan = 1; break;
                                        case 2: MonthDesc = "Februari"; bulan = 2; break;
                                        case 3: MonthDesc = "Maret"; bulan = 3; break;
                                        case 4: MonthDesc = "April"; bulan = 4; break;
                                        case 5: MonthDesc = "Mei"; bulan = 5; break;
                                        case 6: MonthDesc = "Juni"; bulan = 6; break;
                                        case 7: MonthDesc = "Juli"; bulan = 7; break;
                                        case 8: MonthDesc = "Agustus"; bulan = 8; break;
                                        case 9: MonthDesc = "September"; bulan = 9; break;
                                        case 10: MonthDesc = "Oktober"; bulan = 10; break;
                                        case 11: MonthDesc = "November"; bulan = 11; break;
                                        case 12: MonthDesc = "Desember"; bulan = 12; break;
                                    }

                                    record.DeliveryYear = Qty.PeriodYear;
                                    record.DeliveryMonth = Qty.PeriodMonth;

                                    ctx.Database.ExecuteSqlCommand(@"update pmQuota
                                                         set IndentQty = IndentQty + 1
                                                         where CompanyCode = '" + CompanyCode + "' and TipeKendaraan = '" + model.TipeKendaraan +
                                                                  "' and Variant = '" + model.Variant + "' and ColourCode = '" + model.ColourCode +
                                                                  "' and PeriodYear = '" + tahun + "' and PeriodMonth = '" + bulan + "'");
                                    break;
                                }
                            }
                            #endregion
                        }
                    }

                    record.InquiryNumber = model.InquiryNumber;
                    record.EmployeeID = model.NikSales;
                    record.SpvEmployeeID = model.SpvEmployeeID != "" ? model.SpvEmployeeID : model.NikSC ?? "";
                    record.IndentDate = model.IndentDate;
                    record.StatusProspek = model.StatusProspek;
                    record.PerolehanData = model.PerolehanData;
                    record.NamaProspek = model.NamaProspek;
                    record.AlamatProspek = model.AlamatProspek == null ? "" : model.AlamatProspek;
                    record.TelpRumah = model.TelpRumah;
                    record.CityID = model.CityID == null ? "" : model.CityID;
                    record.NamaPerusahaan = model.NamaPerusahaan == null ? "" : model.NamaPerusahaan;
                    record.AlamatPerusahaan = model.AlamatPerusahaan == null ? "" : model.AlamatPerusahaan;
                    record.Jabatan = model.Jabatan;
                    record.Handphone = model.Handphone == null ? "" : model.Handphone;
                    record.Faximile = model.Faximile == null ? "" : model.Faximile;
                    record.Email = model.Email == null ? "" : model.Email;
                    record.TipeKendaraan = model.TipeKendaraan;
                    record.Variant = model.Variant;
                    record.Transmisi = model.Transmisi;
                    record.ColourCode = model.ColourCode;
                    record.CaraPembayaran = model.CaraPembayaran;
                    if (model.CaraPembayaran == "20")
                    {
                        record.Leasing = model.Leasing;
                        record.DownPayment = model.DownPayment;
                        record.Tenor = model.Tenor;
                    }
                    else
                    {
                        record.Leasing = "";
                        record.DownPayment = "";
                        record.Tenor = "";
                    }
                    record.TestDrive = model.TestDrive;//record.QuantityInquiry = ctx.PmActivites.Where(p => p.CompanyCode == record.CompanyCode && p.BranchCode == record.BranchCode && p.InquiryNumber == record.InquiryNumber).Count();
                    record.QuantityInquiry = model.QuantityInquiry;
                    record.LastProgress = model.LastProgress;
                    record.LastUpdateStatus = model.LastUpdateStatus;
                    var hist = ctx.pmStatusHistoryIndents
                     .Where(a => a.IndentNumber == record.IndentNumber && a.CompanyCode == CompanyCode && a.BranchCode == model.BranchCode)
                     .OrderByDescending(b => b.SequenceNo).FirstOrDefault();
                    if (hist == null)
                    {
                        hist = new pmStatusHistoryIndent
                        {

                            IndentNumber = record.IndentNumber,
                            CompanyCode = CompanyCode,
                            BranchCode = model.BranchCode,
                            SequenceNo = 1,
                            LastProgress = model.LastProgress,
                            UpdateDate = model.LastUpdateStatus,
                            UpdateUser = userID
                        };

                        ctx.pmStatusHistoryIndents.Add(hist);
                    }
                    else if (hist.LastProgress != model.LastProgress)
                    {
                        var newrecord = ctx.GnMstLookUpDtls.Where(a => a.CompanyCode == "0000000" && a.CodeID == "PSTS" && a.LookUpValue == model.LastProgress).FirstOrDefault();
                        var exists = ctx.GnMstLookUpDtls.Where(a => a.CompanyCode == "0000000" && a.CodeID == "PSTS" && a.LookUpValue == hist.LastProgress).FirstOrDefault();
                        if (newrecord != null && exists != null)
                        {
                            if (newrecord.SeqNo < exists.SeqNo)
                            {
                                return Json(new { success = false, message = " Status terakhir tidak boleh dirubah ke " + newrecord.LookUpValueName });
                            }
                        }
                        record.LastProgress = model.LastProgress;
                        record.LastUpdateStatus = currentDate;

                        var seqs = from p in ctx.pmStatusHistoryIndents
                                   where p.IndentNumber == record.IndentNumber
                                   && p.CompanyCode == record.CompanyCode
                                   && p.BranchCode == record.BranchCode
                                   select p.SequenceNo;

                        var newseqno = (seqs.Count() > 0) ? seqs.Max() + 1 : 1;
                        hist = new pmStatusHistoryIndent
                        {
                            IndentNumber = record.IndentNumber,
                            CompanyCode = record.CompanyCode,
                            BranchCode = record.BranchCode,
                            SequenceNo = newseqno,
                            LastProgress = record.LastProgress,
                            UpdateDate = record.LastUpdateStatus,
                            UpdateUser = userID
                        };

                        ctx.pmStatusHistoryIndents.Add(hist);
                    }
                    record.SPKDate = model.SPKDate == null ? Convert.ToDateTime("1900/01/01") : model.SPKDate;
                    record.LostCaseDate = model.LostCaseDate == null ? Convert.ToDateTime("1900/01/01") : model.LostCaseDate;
                    record.LostCaseCategory = model.LostCaseCategory == null ? "" : model.LostCaseCategory;
                    record.LostCaseReasonID = model.LostCaseReasonID == null ? "" : model.LostCaseReasonID;
                    record.LostCaseOtherReason = model.LostCaseOtherReason == null ? "" : model.LostCaseOtherReason;
                    record.LostCaseVoiceOfCustomer = model.LostCaseVoiceOfCustomer == null ? "" : model.LostCaseVoiceOfCustomer;
                    record.MerkLain = model.MerkLain ?? "";
                    record.LastUpdateBy = userID;
                    record.LastUpdateDate = currentDate;

                    //var giftRefferenceCode = "GIFT";
                    //var giftRefferenceDesc = ctx.GnMstLookUpDtls.Where(
                    //        p => p.CompanyCode == "0000000" && p.CodeID == giftRefferenceCode && p.LookUpValue == model.Hadiah)
                    //        .FirstOrDefault().LookUpValueName;

                    var oKdpAdd = ctx.pmIndentAdditionals.Find(record.CompanyCode, record.BranchCode, record.IndentNumber);
                    if (oKdpAdd == null)
                    {
                        oKdpAdd = new pmIndentAdditional
                        {
                            CompanyCode = record.CompanyCode,
                            BranchCode = record.BranchCode,
                            IndentNumber = record.IndentNumber,
                            CreatedBy = record.LastUpdateBy,
                            CreatedDate = record.LastUpdateDate,
                        };
                        ctx.pmIndentAdditionals.Add(oKdpAdd);
                    }
                    oKdpAdd.SPKNo = model.SPKNo;
                    oKdpAdd.SPKDate = model.SPKDate;
                    oKdpAdd.GiftRefferenceCode = ""; // giftRefferenceCode;
                    oKdpAdd.GiftRefferenceValue = model.Hadiah;
                    oKdpAdd.GiftRefferenceDesc = ""; // giftRefferenceDesc;
                    oKdpAdd.ShiftCode = model.ShiftCode;
                    //
                    oKdpAdd.StatusVehicle = model.StatusVehicle;
                    oKdpAdd.OthersBrand = model.BrandCode;
                    oKdpAdd.OthersType = model.ModelName;
                    oKdpAdd.LastUpdateBy = record.LastUpdateBy;
                    oKdpAdd.LastUpdateDate = record.LastUpdateDate;


                    if (model.LastProgress == "LOST")
                    {
                        if (model.InquiryNumber == null && model.SPKNo == null)
                        {
                            GenerateInsPmKdp(record, oKdpAdd, hist, bolNew);
                        }

                        if (QuotaBy == "VAR")
                        {
                            ctx.Database.ExecuteSqlCommand("exec uspfn_RecalculateFromIndentLost '" + model.CompanyCode + "','" + model.TipeKendaraan + "','" + model.Variant + "'," + model.IndentNumber + "," + model.DeliveryMonth + "," + model.DeliveryYear + "");
                            //record.DeliveryMonth = null;
                            //record.DeliveryYear = null;
                            ctx.SaveChanges();
                            tranScope.Complete();

                            return Json(new
                            {
                                success = true,
                                messageDev = "Nomor Indent: \"" + model.IndentNumber + "\" telah di batalkan",
                                data = new
                                {
                                    IndentNumber = record.IndentNumber,
                                    CompanyCode = record.CompanyCode,
                                    BranchCode = record.BranchCode,
                                    DeliveryMonth = record.DeliveryMonth,
                                    DeliveryYear = record.DeliveryYear
                                }
                            });
                        }
                        else if (QuotaBy == "COL")
                        {
                            ctx.Database.ExecuteSqlCommand("exec uspfn_RecalculateFromIndentLostByCol '" + model.CompanyCode + "','" + model.TipeKendaraan + "','" + model.Variant + "','" + model.ColourCode + "'," + model.IndentNumber + "," + model.DeliveryMonth + "," + model.DeliveryYear + "");
                            //record.DeliveryMonth = null;
                            //record.DeliveryYear = null;
                            ctx.SaveChanges();
                            tranScope.Complete();

                            return Json(new
                            {
                                success = true,
                                messageDev = "Nomor Indent: \"" + model.IndentNumber + "\" telah di batalkan",
                                data = new
                                {
                                    IndentNumber = record.IndentNumber,
                                    CompanyCode = record.CompanyCode,
                                    BranchCode = record.BranchCode,
                                    DeliveryMonth = record.DeliveryMonth,
                                    DeliveryYear = record.DeliveryYear
                                }
                            });
                        }
                        else
                        {
                            tranScope.Dispose();
                            return Json(new { success = false, message = "StoreProsedur belum dibuat, silakan hubungi bagian IT !" });
                        }

                    }

                    try
                    {
                        ctx.SaveChanges();
                        if (model.InquiryNumber == null && model.SPKNo == null)
                        {
                            GenerateInsPmKdp(record, oKdpAdd, hist, bolNew);
                        }
                        tranScope.Complete();
                        
                        if (record.DeliveryMonth != null && record.DeliveryYear != null)
                        {
                            messageDev = "Inquiry#: " + record.IndentNumber + ", SPK#: " + oKdpAdd.SPKNo + ", Kendaraan akan di delivery pada bulan " + MonthDesc + " " + record.DeliveryYear + ".";
                        }
                        else
                        {
                            //messageDev = "Inquiry#: " + record.IndentNumber + ", SPK#: " + oKdpAdd.SPKNo + ", delivery belum bisa di tentukan karena quota belum tersedia";
                            messageDev = "QUOTA HABIS... HUBUNGI PIC LOGISTIC DEALER";
                        }
                        return Json(new
                        {
                            success = true,
                            message = "Inquiry#: " + record.IndentNumber + ",Nomor SPK: \"" + model.SPKNo + "\" berhasil disimpan ke database...",
                            messageDev = messageDev,
                            data = new
                            {
                                IndentNumber = record.IndentNumber,
                                CompanyCode = record.CompanyCode,
                                BranchCode = record.BranchCode,
                                DeliveryMonth = record.DeliveryMonth,
                                DeliveryYear = record.DeliveryYear
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        tranScope.Dispose();
                        return Json(new { success = false, message = ex.Message });
                    }
                }
            }
        }

        public JsonResult SaveAct(pmActivitiesIndent model)
        {
            var record = ctx.pmActivitiesIndents.Find(CompanyCode, model.BranchCode, model.IndentNumber, model.ActivityID);
            var userID = CurrentUser.Username;
            var currentDate = DateTime.Now;
            if (record == null)
            {
                var numbers = from p in ctx.pmActivitiesIndents
                              where p.CompanyCode == CompanyCode
                              && p.BranchCode == model.BranchCode
                              && p.IndentNumber == model.IndentNumber
                              select p.ActivityID;
                var newnumber = (numbers.Count() > 0) ? numbers.Max() + 1 : 1;

                record = new pmActivitiesIndent
                {
                    CompanyCode = CompanyCode,
                    BranchCode = model.BranchCode,
                    IndentNumber = model.IndentNumber,
                    ActivityID = newnumber,
                    CreatedBy = userID,
                    CreationDate = currentDate,
                };
                ctx.pmActivitiesIndents.Add(record);
            }
            record.ActivityDate = model.ActivityDate;
            record.ActivityType = model.ActivityType;
            record.ActivityDetail = model.ActivityDetail;
            record.NextFollowUpDate = model.NextFollowUpDate;
            record.LastUpdateBy = userID;
            record.LastUpdateDate = currentDate;

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

        public JsonResult DeleteAct(pmActivitiesIndent model)
        {
            var userID = CurrentUser.UserId;
            var currentDate = DateTime.Now;

            var record = ctx.pmActivitiesIndents.Find(model.CompanyCode, model.BranchCode, model.IndentNumber, model.ActivityID);
            var data = ctx.pmActivitiesIndents.Where(a => a.CompanyCode == model.CompanyCode && a.BranchCode == model.BranchCode && a.IndentNumber == model.IndentNumber).FirstOrDefault();
            if (record != null) { ctx.pmActivitiesIndents.Remove(record); }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Get()
        {
            try
            {
                var branch = Request["BranchCode"];
                var inquno = Int32.Parse(Request["InquiryNumber"]);
                DateTime tanggal = Convert.ToDateTime("1900/01/01");

                var qry = from p in ctx.pmIndents
                          join q in ctx.HrEmployees on new { p.CompanyCode, p.EmployeeID } equals new { q.CompanyCode, q.EmployeeID }
                          join j1 in ctx.OmMstRefferences on new { p.ColourCode, RefferenceType = "COLO" } equals new { ColourCode = j1.RefferenceCode, j1.RefferenceType } into join1
                          from r in join1.DefaultIfEmpty()
                          join j2 in ctx.GnMstLookUpDtls on new { p.TestDrive, CodeID = "PMOP" } equals new { TestDrive = j2.LookUpValue, j2.CodeID } into join2
                          from s in join2.DefaultIfEmpty()
                          join j3 in ctx.GnMstLookUpDtls on new { p.CityID, CodeID = "CITY" } equals new { CityID = j3.LookUpValue, j3.CodeID } into join3
                          from t in join3.DefaultIfEmpty()
                          join j4 in ctx.pmIndentAdditionals on new { p.CompanyCode, p.BranchCode, p.IndentNumber } equals new { j4.CompanyCode, j4.BranchCode, j4.IndentNumber } into join4
                          from u in join4.DefaultIfEmpty()
                          where
                          p.CompanyCode == CompanyCode
                          &&
                          p.BranchCode == branch
                          &&
                          p.IndentNumber == inquno
                          select new
                          {
                              p.InquiryNumber,                             
                              p.CompanyCode,
                              p.IndentNumber,
                              p.IndentDate,
                              p.BranchCode,
                              p.TipeKendaraan,
                              p.Variant,
                              p.Transmisi,
                              p.ColourCode,
                              ColourName = r.RefferenceDesc1,
                              p.NamaProspek,
                              p.EmployeeID,
                              p.SpvEmployeeID,
                              NikSales = p.EmployeeID,
                              NikSH = p.SpvEmployeeID,
                              q.EmployeeName,
                              PerolehanData = p.PerolehanData.ToUpper(),
                              p.LastProgress,
                              p.QuantityInquiry,
                              TestDrive = s.LookUpValue,
                              p.TelpRumah,
                              p.NamaPerusahaan,
                              p.AlamatProspek,
                              p.AlamatPerusahaan,
                              p.Handphone,
                              p.CityID,
                              CityName = t.LookUpValueName,
                              p.Jabatan,
                              p.CaraPembayaran,
                              p.DownPayment,
                              p.Faximile,
                              p.LastUpdateStatus,
                              p.SPKDate,
                              p.Leasing,
                              p.StatusProspek,
                              p.Tenor,
                              p.Email,
                              p.LostCaseDate,
                              p.LostCaseCategory,
                              p.LostCaseOtherReason,
                              p.LostCaseReasonID,
                              p.LostCaseVoiceOfCustomer,
                              p.MerkLain,
                              q.Grade,
                              u.StatusVehicle,
                              u.OthersBrand,
                              u.OthersType,
                              u.SPKNo,
                              Hadiah = u.GiftRefferenceValue,
                              p.DeliveryMonth,
                              p.DeliveryYear
                          };
                qry = qry.Distinct();

                var data = from p in ctx.pmActivitiesIndents
                           where p.CompanyCode == CompanyCode
                           && p.BranchCode == branch
                           && p.IndentNumber == inquno
                           select new
                           {
                               p.CompanyCode,
                               p.BranchCode,
                               p.IndentNumber,
                               p.ActivityID,
                               p.ActivityDate,
                               p.ActivityType,
                               p.ActivityDetail,
                               p.NextFollowUpDate,
                           };

                if (qry.Count() > 0)
                {
                    return Json(new { success = true, data = qry.FirstOrDefault(), list = data.ToList() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "data not found..." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                {
                    return Json(new { success = false, message = (ex.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult pmActivitiesIndent()
        {
            var branch = Request["BranchCode"];
            var inquno = Int32.Parse(Request["InquiryNumber"]);

            var data = from p in ctx.pmActivitiesIndents
                       where p.CompanyCode == CompanyCode
                       && p.BranchCode == branch
                       && p.IndentNumber == inquno
                       select new
                       {
                           p.CompanyCode,
                           p.BranchCode,
                           p.IndentNumber,
                           p.ActivityID,
                           p.ActivityDate,
                           p.ActivityType,
                           p.ActivityDetail,
                           p.NextFollowUpDate,
                       };

            var result = data;
            return Json(data.KGrid());
        }

        public JsonResult GetDetail()
        {
            var branch = Request["BranchCode"];
            var inquno = Int32.Parse(Request["IndentNumber"]);
            var activityid = Int32.Parse(Request["ActivityID"]);

            var data = from p in ctx.pmActivitiesIndents
                       where p.CompanyCode == CompanyCode
                       && p.BranchCode == branch
                       && p.IndentNumber == inquno
                       && p.ActivityID == activityid
                       select new
                       {
                           p.CompanyCode,
                           p.BranchCode,
                           p.IndentNumber,
                           p.ActivityID,
                           p.ActivityDate,
                           p.ActivityType,
                           p.ActivityDetail,
                           p.NextFollowUpDate,
                       };

            return Json(new { success = true, data = data.FirstOrDefault() });
        }

        public JsonResult ModelList(string fltStatus)
        {
            //var fltBrand = Request["fltBrand"];
            //var fltModel = Request["fltModel"];
            //var fltStatu = Request["fltStatus"];

            var qry = from p in ctx.msMstModels
                      select new
                      {
                          p.ModelType,
                          p.Variant,
                          p.BrandCode,
                          p.ModelName,
                          p.isSuzukiClass
                      };

            //if (!string.IsNullOrWhiteSpace(fltBrand)) qry = qry.Where(p => p.BrandCode.Contains(fltBrand));
            //if (!string.IsNullOrWhiteSpace(fltModel)) qry = qry.Where(p => p.ModelName.Contains(fltModel));
            if (!string.IsNullOrWhiteSpace(fltStatus))
            {
                if ((new string[] { "B", "D" }).Contains(fltStatus))
                {
                    qry = qry.Where(p => p.isSuzukiClass == true);
                }

                if ((new string[] { "C", "E" }).Contains(fltStatus))
                {
                    qry = qry.Where(p => p.isSuzukiClass == false);
                }
            }

            return Json(qry.KGrid());
        }

        public JsonResult SpkNo(string BranchCode)
        {
            var sql = ctx.pmIndentAdditionals.Where(a=> a.CompanyCode == CompanyCode && a.BranchCode == BranchCode).Select(a => a.SPKNo).ToList();
            var qry = from p in ctx.PmKDPAdditionalExhibitions
                      where p.CompanyCode == CompanyCode
                      && p.BranchCode == BranchCode
                      && !sql.Contains(p.SPKNo)
                      select new
                      {
                          p.SPKDate,
                          p.SPKNo,
                          p.GiftRefferenceCode,
                          p.GiftRefferenceDesc,
                          p.GiftRefferenceValue,
                          p.ShiftCode
                      };

            return Json(qry.KGrid());
        }

        public JsonResult Exhibition(string NikSales, string BranchCode)
        {
            var sql = ctx.pmIndents.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode).Select(a => a.InquiryNumber).ToList();
            var qry = from p in ctx.PmKDPExhibitions
                      join j1 in ctx.OmMstRefferences on new { p.ColourCode, RefferenceType = "COLO" } equals new { ColourCode = j1.RefferenceCode, j1.RefferenceType } into join1
                      from r in join1.DefaultIfEmpty()
                      join b in ctx.GnMstLookUpDtls on new { LookUpValue = p.TipeKendaraan, CodeID = "INDENT" } equals new { b.LookUpValue, b.CodeID }
                      where p.CompanyCode == CompanyCode 
                      && p.BranchCode == BranchCode
                      && p.EmployeeID == NikSales
                      && !sql.Contains(p.InquiryNumber)
                      select new
                      {
                          p.CompanyCode,
                          p.BranchCode,
                          p.InquiryNumber,
                          p.InquiryDate,
                          p.TipeKendaraan,
                          p.Variant,
                          p.Transmisi,
                          p.ColourCode,
                          ColourName = r.RefferenceDesc1,
                      };
            qry = qry.Distinct();
            qry = qry.OrderByDescending(p => p.InquiryNumber);
            return Json(qry.KGrid());
        }

        public JsonResult GetExhibition()
        {
            try
            {
                var branch = Request["BranchCode"];
                var inquno = Int32.Parse(Request["InquiryNumber"]);

                var qry = from p in ctx.PmKDPExhibitions
                          join q in ctx.HrEmployees on new { p.CompanyCode, p.EmployeeID } equals new { q.CompanyCode, q.EmployeeID }
                          join j1 in ctx.OmMstRefferences on new { p.ColourCode, RefferenceType = "COLO" } equals new { ColourCode = j1.RefferenceCode, j1.RefferenceType } into join1
                          from r in join1.DefaultIfEmpty()
                          join j2 in ctx.GnMstLookUpDtls on new { p.TestDrive, CodeID = "PMOP" } equals new { TestDrive = j2.LookUpValue, j2.CodeID } into join2
                          from s in join2.DefaultIfEmpty()
                          join j3 in ctx.GnMstLookUpDtls on new { p.CityID, CodeID = "CITY" } equals new { CityID = j3.LookUpValue, j3.CodeID } into join3
                          from t in join3.DefaultIfEmpty()
                          join j4 in ctx.PmKDPAdditionalExhibitions on new { p.CompanyCode, p.BranchCode, p.InquiryNumber } equals new { j4.CompanyCode, j4.BranchCode, j4.InquiryNumber } into join4
                          from u in join4.DefaultIfEmpty()
                          where
                          p.CompanyCode == CompanyCode
                          &&
                          p.BranchCode == branch
                          &&
                          p.InquiryNumber == inquno
                          select new
                          {
                              p.CompanyCode,
                              p.InquiryNumber,
                              p.InquiryDate,
                              p.BranchCode,
                              p.TipeKendaraan,
                              p.Variant,
                              p.Transmisi,
                              p.ColourCode,
                              ColourName = r.RefferenceDesc1,
                              p.NamaProspek,
                              p.EmployeeID,
                              SpvEmployeeID = ctx.HrEmployees.Where(a=> a.CompanyCode == CompanyCode && a.EmployeeID == p.EmployeeID).FirstOrDefault().TeamLeader,
                              NikSales = p.EmployeeID,
                              NikSH = ctx.HrEmployees.Where(a => a.CompanyCode == CompanyCode && a.EmployeeID == p.EmployeeID).FirstOrDefault().TeamLeader,
                              q.EmployeeName,
                              PerolehanData = p.PerolehanData.ToUpper(),
                              p.LastProgress,
                              p.QuantityInquiry,
                              TestDrive = s.LookUpValue,
                              p.TelpRumah,
                              p.NamaPerusahaan,
                              p.AlamatProspek,
                              p.AlamatPerusahaan,
                              p.Handphone,
                              p.CityID,
                              CityName = t.LookUpValueName,
                              p.Jabatan,
                              p.CaraPembayaran,
                              p.DownPayment,
                              p.Faximile,
                              p.LastUpdateStatus,
                              p.SPKDate,
                              p.Leasing,
                              p.StatusProspek,
                              p.Tenor,
                              p.Email,
                              p.LostCaseDate,
                              p.LostCaseCategory,
                              p.LostCaseOtherReason,
                              p.LostCaseReasonID,
                              p.LostCaseVoiceOfCustomer,
                              p.MerkLain,
                              q.Grade,
                              u.StatusVehicle,
                              u.OthersBrand,
                              u.OthersType,
                              u.SPKNo,
                              Hadiah = u.GiftRefferenceValue,
                          };
                qry = qry.Distinct();

                if (qry.Count() > 0)
                {
                    return Json(new { success = true, data = qry.FirstOrDefault() });
                }
                else
                {
                    return Json(new { success = false, message = "data not found..." });
                }
            }
            catch (Exception ex)
            {
                {
                    return Json(new { success = false, message = (ex.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message });
                }
            }
        }

        public IEnumerable<dynamic> CarMappingList()
        {
            ctx.Database.CommandTimeout = 3600;
            var qry = from g in ctx.PmGroupTypeSeqs
                      join m in ctx.OmMstModels on new { g.GroupCode, g.TypeCode } equals new { m.GroupCode, m.TypeCode } into m2
                      from m in m2.DefaultIfEmpty()
                      join c in ctx.OmMstModelColours on m.SalesModelCode equals c.SalesModelCode into c2
                      from c in c2.DefaultIfEmpty()
                      join b in ctx.GnMstLookUpDtls on new { LookUpValue = g.GroupCode, CodeID = "INDENT", ParaValue = "1" } equals new { b.LookUpValue, b.CodeID, b.ParaValue }
                      where c.ColourCode != null
                             && m.Status == "1"
                      orderby g.GroupCode, g.TypeCode, c.ColourCode
                      select new
                      {
                          TipeKendaraan = g.GroupCode,
                          Variant = g.TypeCode,
                          TransmissionType = m != null ? m.TransmissionType : null,
                          ColorCode = c != null ? c.ColourCode : null,
                          ColorName = (from r in ctx.OmMstRefferences
                                       where r.RefferenceType == "COLO" && r.RefferenceCode == c.ColourCode
                                       select r.RefferenceDesc1).FirstOrDefault()
                      };

            return qry.Distinct();
        }

        public JsonResult CarTypes()
        {
            return Json(CarMappingList().Select(p => new
            {
                value = p.TipeKendaraan,
                text = p.TipeKendaraan
            }).Distinct());
        }

        public JsonResult CarVariants(string id = "")
        {
            return Json(CarMappingList().Where(p => p.TipeKendaraan == id).Select(p => new
            {
                value = p.Variant,
                text = p.Variant
            }).Distinct());
        }

        public JsonResult Transmissions()
        {
            var cartype = Request["CarType"];
            var carvari = Request["CarVariant"];
            var list = from c in CarMappingList().ToList()
                       join l in ctx.GnMstLookUpDtls on c.TransmissionType equals l.LookUpValue
                       where l.CodeID == "TRTY" && c.TipeKendaraan == cartype && c.Variant == carvari
                       select new
                       {
                           value = c.TransmissionType,
                           text = c.TransmissionType + " - " + l.LookUpValueName
                       };

            return Json(list.Distinct());
        }

        public JsonResult ModelColors()
        {
            var cartype = Request["CarType"];
            var carvari = Request["CarVariant"];
            var cartran = Request["CarTrans"];
            var list = CarMappingList().Where(p => p.TipeKendaraan == cartype
                    && p.Variant == carvari && p.TransmissionType == cartran).Select(p => new
                    {
                        value = p.ColorCode,
                        text = p.ColorCode + " - " + p.ColorName
                    }).Distinct();

            return Json(list);
        }

        public void GenerateInsPmKdp(pmIndent model, pmIndentAdditional model2, pmStatusHistoryIndent model3, bool bolNew)
        {
            string sqls = "declare @inqNo int, @branchCode varchar(15)" + System.Environment.NewLine +
                "set @branchCode = (select top 1 BranchCode from HrEmployeeMutation where CompanyCode = '" + model.CompanyCode + "' and EmployeeID = '" + model.EmployeeID + "' order by MutationDate desc)" + System.Environment.NewLine;
            if (bolNew)
            {
                sqls += "begin tran " + System.Environment.NewLine +
                    "set @inqNo = (select isnull(max(InquiryNumber), 0) + 1 from pmKDP with (tablockx))" + System.Environment.NewLine +

                    "INSERT INTO [dbo].[pmKDP] ([InquiryNumber], [CompanyCode], [BranchCode], [EmployeeID], [SpvEmployeeID], [InquiryDate], [OutletID], [StatusProspek], [PerolehanData], [NamaProspek], [AlamatProspek], [TelpRumah], [CityID], [NamaPerusahaan], [AlamatPerusahaan], [Jabatan], [Handphone], [Faximile], [Email], [TipeKendaraan], [Variant], [Transmisi], [ColourCode], [CaraPembayaran], [TestDrive], [QuantityInquiry], [LastProgress], [LastUpdateStatus], [SPKDate], [LostCaseDate], [LostCaseCategory], [LostCaseReasonID], [LostCaseOtherReason], [LostCaseVoiceOfCustomer], [CreationDate], [CreatedBy], [LastUpdateBy], [LastUpdateDate], [Leasing], [DownPayment], [Tenor], [MerkLain])" + System.Environment.NewLine +
                    "VALUES (@inqNo,'" + model.CompanyCode +
                        "',@branchCode,'" + model.EmployeeID +
                        "',(select top 1 TeamLeader from HrEmployee where EmployeeId = '" +
                                model.EmployeeID + "'),'" + model.IndentDate +
                        "','" + model.OutletID + "','" + model.StatusProspek +
                        "','" + model.PerolehanData + "','" + model.NamaProspek +
                        "','" + model.AlamatProspek + "','" + model.TelpRumah +
                        "','" + model.CityID + "','" + model.NamaPerusahaan +
                        "','" + model.AlamatPerusahaan + "','" + model.Jabatan +
                        "','" + model.Handphone + "','" + model.Faximile +
                        "','" + model.Email + "','" + model.TipeKendaraan +
                        "','" + model.Variant + "','" + model.Transmisi +
                        "','" + model.ColourCode + "','" + model.CaraPembayaran +
                        "','" + model.TestDrive + "','" + model.QuantityInquiry +
                        "','" + model.LastProgress + "','" + model.LastUpdateStatus +
                        "','" + model.SPKDate + "','" + model.LostCaseDate +
                        "','" + model.LostCaseCategory + "','" + model.LostCaseReasonID +
                        "','" + model.LostCaseOtherReason + "','" + model.LostCaseVoiceOfCustomer +
                        "','" + model.CreationDate + "','" + GetUserEx(model.CreatedBy, model.IndentNumber) +
                        "','" + GetUserEx(model.LastUpdateBy, model.IndentNumber) + "','" + model.LastUpdateDate +
                        "','" + model.Leasing + "','" + model.DownPayment +
                        "','" + model.Tenor + "','" + model.MerkLain +
                        "')" + System.Environment.NewLine +

                    "commit tran" + System.Environment.NewLine +

                    "INSERT INTO [dbo].[pmKdpAdditional] ([CompanyCode], [BranchCode], [InquiryNumber], [StatusVehicle], [OthersBrand], [OthersType], [CreatedBy], [CreatedDate], [LastUpdateBy], [LastUpdateDate])" + System.Environment.NewLine +
                    "VALUES ('" +
                        model2.CompanyCode + "'," +
                        "@branchCode" + "," +
                        "@inqNo" + ",'" +
                        model2.StatusVehicle + "','" +
                        model2.OthersBrand + "','" +
                        model2.OthersType + "','" +
                        model.IndentNumber + "','" +
                        model2.CreatedDate + "','" +
                       GetUserEx(model2.LastUpdateBy, model.IndentNumber) + "','" +
                        model2.LastUpdateDate + "')" + System.Environment.NewLine +
                    "INSERT INTO [dbo].[pmStatusHistory] ([InquiryNumber], [CompanyCode], [BranchCode], [SequenceNo], [LastProgress], [UpdateDate], [UpdateUser])" + System.Environment.NewLine +
                    "VALUES (@inqNo,'" +
                        model3.CompanyCode + "'," +
                        "@branchCode" + ",'" +
                        "1" + "','" +
                        "P" + "','" +
                        model3.UpdateDate + "','" +
                        GetUserEx(model3.UpdateUser, model.IndentNumber) +
                        "')" + System.Environment.NewLine +
                    "INSERT INTO [dbo].[pmStatusHistory] ([InquiryNumber], [CompanyCode], [BranchCode], [SequenceNo], [LastProgress], [UpdateDate], [UpdateUser])" + System.Environment.NewLine +
                    "VALUES (@inqNo,'" +
                        model3.CompanyCode + "'," +
                        "@branchCode" + ",'" +
                        "2" + "','" +
                        "HP" + "','" +
                        model3.UpdateDate + "','" +
                        GetUserEx(model3.UpdateUser, model.IndentNumber) +
                        "')" + System.Environment.NewLine +
                    "INSERT INTO [dbo].[pmStatusHistory] ([InquiryNumber], [CompanyCode], [BranchCode], [SequenceNo], [LastProgress], [UpdateDate], [UpdateUser])" + System.Environment.NewLine +
                    "VALUES (@inqNo,'" +
                        model3.CompanyCode + "'," +
                        "@branchCode" + ",'" +
                        "3" + "','" +
                        "SPK" + "','" +
                        model3.UpdateDate + "','" +
                       GetUserEx(model3.UpdateUser, model.IndentNumber) +
                        "')" + System.Environment.NewLine;
            }
            else
            {
                sqls += "SET @inqNo = (SELECT InquiryNumber from pmKdpAdditional where CreatedBy = '" + model.IndentNumber + "')" + System.Environment.NewLine +
                "UPDATE [dbo].[pmKDP]" + System.Environment.NewLine +
                "SET " +
                    "EmployeeID = '" + model.EmployeeID +
                    "', SpvEmployeeID = (select top 1 TeamLeader from HrEmployee where EmployeeId = '" + model.EmployeeID +
                    "'), InquiryDate = '" + model.IndentDate +
                    "', OutletID = '" + model.OutletID +
                    "', StatusProspek = '" + model.StatusProspek +
                    "', PerolehanData = '" + model.PerolehanData +
                    "', NamaProspek = '" + model.NamaProspek +
                    "', AlamatProspek = '" + model.AlamatProspek +
                    "', TelpRumah = '" + model.TelpRumah +
                    "', CityID = '" + model.CityID +
                    "', NamaPerusahaan = '" + model.NamaPerusahaan +
                    "', AlamatPerusahaan = '" + model.AlamatPerusahaan +
                    "', Jabatan = '" + model.Jabatan +
                    "', Handphone = '" + model.Handphone +
                    "', Faximile = '" + model.Faximile +
                    "', Email = '" + model.Email +
                    "', TipeKendaraan = '" + model.TipeKendaraan +
                    "', Variant = '" + model.Variant +
                    "', Transmisi = '" + model.Transmisi +
                    "', ColourCode = '" + model.ColourCode +
                    "', CaraPembayaran = '" + model.CaraPembayaran +
                    "', TestDrive = '" + model.TestDrive +
                    "', QuantityInquiry = '" + model.QuantityInquiry +
                    "', LastProgress = '" + model.LastProgress +
                    "', LastUpdateStatus = '" + model.LastUpdateStatus +
                    "', SPKDate = '" + model.SPKDate +
                    "', LostCaseDate = '" + model.LostCaseDate +
                    "', LostCaseCategory = '" + model.LostCaseCategory +
                    "', LostCaseReasonID = '" + model.LostCaseReasonID +
                    "', LostCaseOtherReason = '" + model.LostCaseOtherReason +
                    "', LostCaseVoiceOfCustomer = '" + model.LostCaseVoiceOfCustomer +
                    "', LastUpdateBy = '" + GetUserEx(model.LastUpdateBy, model.IndentNumber) +
                    "', LastUpdateDate = '" + model.LastUpdateDate +
                    "', Leasing = '" + model.Leasing +
                    "', DownPayment = '" + model.DownPayment +
                    "', Tenor = '" + model.Tenor +
                    "', MerkLain = '" + model.MerkLain +
                    "'" + System.Environment.NewLine +
                "WHERE InquiryNumber = @inqNo " + System.Environment.NewLine +
                    "AND CompanyCode = '" + model3.CompanyCode +
                    "' AND  BranchCode = @branchCode" + System.Environment.NewLine +

                //"GO" + System.Environment.NewLine +
                "UPDATE [dbo].[pmKdpAdditional]" + System.Environment.NewLine +
                "SET " +
                    "StatusVehicle = '" + model2.StatusVehicle +
                    "', OthersBrand = '" + model2.OthersBrand +
                    "', OthersType = '" + model2.OthersType +
                    "', LastUpdateBy = '" + GetUserEx(model2.LastUpdateBy, model.IndentNumber) +
                    "', LastUpdateDate = '" + model2.LastUpdateDate +
                    "'" + System.Environment.NewLine +
                "WHERE InquiryNumber = @inqNo" + System.Environment.NewLine +
                    "AND CompanyCode = '" + model3.CompanyCode +
                    "' AND  BranchCode = @branchCode" + System.Environment.NewLine +

                 "INSERT INTO [dbo].[pmStatusHistory] ([InquiryNumber], [CompanyCode], [BranchCode], [SequenceNo], [LastProgress], [UpdateDate], [UpdateUser])" + System.Environment.NewLine +
                    "VALUES (@inqNo,'" +
                        model3.CompanyCode + "'," +
                        "@branchCode" + ",'" +
                        "4" + "','" +
                        "LOST" + "','" +
                        model3.UpdateDate + "','" +
                       GetUserEx(model3.UpdateUser, model.IndentNumber) +
                        "')" + System.Environment.NewLine;   
            }
            GenerateSQL(new SysSQLGateway() { TaskNo = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + model.IndentNumber, TaskName = model.CompanyCode + "_" + model.SPKNo, SQL = sqls, DealerCode = model.CompanyCode });
        }

        private string GetUserEx(string UserLogin, int idx)
        {
            var str = "ID" + idx.ToString() + "_"  + UserLogin;

            if (str.Length > 15)
            {
                return str.Substring(0, 15);
            }
            else
                return str;
        }
    }
}