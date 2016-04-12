using SimDms.Common.Models;
using SimDms.PreSales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using SimDms.Common;

namespace SimDms.PreSales.Controllers.Api
{
    public class KdpController : BaseController
    {
        static readonly object lck = new object();
        public JsonResult Default(string ProductType)
        {
            var companycode = CompanyCode;
            var empl = (from p in ctx.HrEmployees
                        where p.CompanyCode == companycode
                        && p.RelatedUser == CurrentUser.UserId
                        select new
                        {
                            p.EmployeeID,
                            p.EmployeeName,
                            p.Grade,
                            p.TeamLeader,
                            p.Position,
                        }).FirstOrDefault();

            if (empl != null)
            {
                var positions = new string[] { "S", "SC", "SH", "CT", "BM" };
                if (!positions.Contains(empl.Position))
                {
                    return Json(new { success = false, message = "Anda tidak mendapatkan akses untuk menu ini" });
                }

                var jabatan = "";
                var addjob = ctx.HrEmployeeAdditionalJobs.Where(a => a.EmployeeID == empl.EmployeeID && a.Position == "SH").FirstOrDefault();
                if (addjob != null) {
                    if (empl.Position != addjob.Position)
                    {
                        jabatan = empl.Position + addjob.Position;
                    }
                    else
                    {
                        jabatan = empl.Position;
                    }
                } else { 
                    jabatan = empl.Position; 
                }

                string grdSales = "", nikSales = "", nikSalesname = "", nikSHname = "", nikSCname = "", nikSC = "", nikSH = "";
                var oSc = ctx.HrEmployees.Find(companycode, empl.TeamLeader);
                var oSh = ctx.HrEmployees.Find(companycode, empl.TeamLeader);
                dynamic NikSlList = new List<dynamic>() { };
                dynamic NikSCList = new List<dynamic>() { };
                dynamic NikSHList = new List<dynamic>() { };

                if (jabatan == "S")
                {
                    nikSales = empl.EmployeeID;
                    nikSalesname = empl.EmployeeName;
                    grdSales = empl.Grade;
                    NikSlList = new List<dynamic>() { new { value = empl.EmployeeID, text = empl.EmployeeName } };
                    oSc = ctx.HrEmployees.Find(companycode, empl.TeamLeader);
                    if (oSc != null && ProductType == "2W")
                    {
                        nikSC = oSc.EmployeeID;
                        nikSCname = oSc.EmployeeName;
                        NikSCList = new List<dynamic>() { new { value = oSc.EmployeeID, text = oSc.EmployeeName } };
                        //oSh = ctx.HrEmployees.Find(CompanyCode, oSc.TeamLeader);
                    }
                    if (oSc != null && ProductType == "4W")
                    {
                        nikSH = oSc.EmployeeID;
                        nikSHname = oSc.EmployeeName;
                        NikSHList = new List<dynamic>() { new { value = oSc.EmployeeID, text = oSc.EmployeeName } };
                    }
                }
                else if (jabatan == "SC")
                {
                    nikSC = empl.EmployeeID;
                    nikSCname = empl.EmployeeName;
                    NikSCList = new List<dynamic>() { new { value = empl.EmployeeID, text = empl.EmployeeName } };
                    oSh = ctx.HrEmployees.Find(companycode, empl.TeamLeader);
                    if (oSh != null)
                    {
                        nikSH = oSh.EmployeeID;
                        NikSHList = new List<dynamic>() { new { value = oSh.EmployeeID, text = oSh.EmployeeName } };
                    }
                    NikSlList = from p in ctx.HrEmployees
                                where p.CompanyCode == companycode && p.TeamLeader == empl.EmployeeID
                                select new { value = p.EmployeeID, text = p.EmployeeName };
                }
                else if (jabatan == "SH")
                {
                    nikSH = empl.EmployeeID;
                    nikSHname = empl.EmployeeName;
                    nikSC = "";
                    NikSHList = new List<dynamic>() { new { value = empl.EmployeeID, text = empl.EmployeeName } };
                    NikSCList = from p in ctx.HrEmployees
                                where p.CompanyCode == companycode && p.TeamLeader == empl.EmployeeID
                                select new { value = p.EmployeeID, text = p.EmployeeName };
                }
                else if (jabatan == "BM")
                {
                    nikSales = empl.EmployeeID;
                    grdSales = empl.Grade;
                    //NikSlList = new List<dynamic>() { new { value = empl.EmployeeID, text = empl.EmployeeName } };
                    NikSlList = ctx.HrEmployees.Where(x => x.TeamLeader == empl.EmployeeID).Where(x => x.Department=="SALES" && (x.Position=="S" || x.Position=="CT")).Select(x => new { value=x.EmployeeID, text=x.EmployeeName });
                    
                    //oSc = ctx.HrEmployees.Find(CompanyCode, empl.TeamLeader);
                    oSh = ctx.HrEmployees.Find(companycode, empl.EmployeeID);
                    if (oSc != null)
                    {
                        nikSC = oSc.EmployeeID;
                        NikSCList = new List<dynamic>() { new { value = oSc.EmployeeID, text = oSc.EmployeeName } };
                        oSh = ctx.HrEmployees.Find(companycode, oSc.TeamLeader);
                    }
                    if (oSh != null)
                    {
                        nikSH = oSh.EmployeeID;
                        NikSHList = new List<dynamic>() { new { value = oSh.EmployeeID, text = oSh.EmployeeName } };
                    }
                }
                else if (empl.Position == "CT")
                {
                    nikSales = empl.EmployeeID;
                    grdSales = empl.Grade;
                    NikSlList = new List<dynamic>() { new { value = empl.EmployeeID, text = empl.EmployeeName } };
                    //NikSlList = ctx.HrEmployees.Where(x => x.TeamLeader == empl.EmployeeID).Select(x => new { value = x.EmployeeID, text = x.EmployeeName });
                    //oSc = ctx.HrEmployees.Find(CompanyCode, empl.TeamLeader);
                    //if (oSc != null)
                    //{
                    //    nikSC = oSc.EmployeeID;
                    //    NikSCList = new List<dynamic>() { new { value = oSc.EmployeeID, text = oSc.EmployeeName } };
                    //    oSh = ctx.HrEmployees.Find(CompanyCode, oSc.TeamLeader);
                    //}
                    //if (oSh != null)
                    //{
                    //    nikSH = oSh.EmployeeID;
                    //    NikSHList = new List<dynamic>() { new { value = oSh.EmployeeID, text = oSh.EmployeeName } };
                    //}
                }
                else if (jabatan == "BMSH")
                {
                    nikSH = empl.EmployeeID;
                    nikSHname = empl.EmployeeName;
                    nikSC = "";
                    NikSHList = new List<dynamic>() { new { value = empl.EmployeeID, text = empl.EmployeeName } };
                    NikSCList = from p in ctx.HrEmployees
                                where p.CompanyCode == companycode && p.TeamLeader == empl.EmployeeID
                                select new { value = p.EmployeeID, text = p.EmployeeName };
                }

                var oGrade = ctx.LookUpDtls.Find(companycode, "ITSG", grdSales);
                var itsg = ctx.LookUpDtls.Where(p => p.CompanyCode == companycode && p.CodeID == "ITSG").OrderBy(p => p.SeqNo).Select(p => new { value = p.LookUpValue, text = p.LookUpValueName.ToUpper() }).ToList();

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        CompanyCode = companycode,
                        CompanyName = CompanyName,
                        OutletID = BranchCode, 
                        BranchName = BranchName,
                        EmployeeID = empl.EmployeeID,
                        EmployeeName = empl.EmployeeName,
                        Position = empl.Position,
                        Grade = grdSales,
                        GradeName = (oGrade == null) ? "" : oGrade.LookUpValueName,
                        NikSales = nikSales,
                        NikSalesName = nikSalesname,
                        NikSC = nikSC,
                        NikSCName = nikSCname,
                        NikSH = nikSH,
                        NikSHName = nikSHname,
                        NikSlList = NikSlList,
                        NikSCList = NikSCList,
                        NikSHList = NikSHList,
                        CurrentDate = DateTime.Now,
                        QuantityInquiry = 1,
                        ItsgList = itsg,
                    }
                });
            }

            return Json(new { success = false, message = "User tidak/belum link dengan salesman" });
        }

        public JsonResult Get()
        {
            var branch = Request["BranchCode"];
            var inquno = Int32.Parse(Request["InquiryNumber"]);

            var qry = from p in ctx.PmKdps
                      join j1 in ctx.LookUpDtls on new { p.CompanyCode, CodeID = "ITLR", LookUpValue = p.LostCaseReasonID } equals new { j1.CompanyCode, j1.CodeID, j1.LookUpValue } into join1
                      from q in join1.DefaultIfEmpty()
                      join j2 in ctx.MstRefferences on new { p.CompanyCode, p.ColourCode, RefferenceType = "COLO" } equals new { j2.CompanyCode, ColourCode = j2.RefferenceCode, j2.RefferenceType } into join2
                      from r in join2.DefaultIfEmpty()
                      join j3 in ctx.HrEmployees on new { p.CompanyCode, p.SpvEmployeeID } equals new { j3.CompanyCode, SpvEmployeeID = j3.EmployeeID } into join3
                      from s in join3.DefaultIfEmpty()
                      join j4 in ctx.LookUpDtls on new { p.CompanyCode, CodeID = "CITY", LookUpValue = p.CityID } equals new { j4.CompanyCode, j4.CodeID, j4.LookUpValue } into join4
                      from t in join4.DefaultIfEmpty()
                      join j5 in ctx.PmKdpAdditionals on new { p.CompanyCode, p.BranchCode, p.InquiryNumber } equals new { j5.CompanyCode, j5.BranchCode, j5.InquiryNumber } into join5
                      from u in join5.DefaultIfEmpty()
                      join j6 in ctx.HrEmployees on new { p.CompanyCode, p.EmployeeID } equals new { j6.CompanyCode, j6.EmployeeID } into join6
                      from v in join6.DefaultIfEmpty()
                      where p.CompanyCode == CompanyCode
                      && p.BranchCode == branch
                      && p.InquiryNumber == inquno
                      select new
                      {
                          p.InquiryNumber,
                          p.BranchCode,
                          p.CompanyCode,
                          p.EmployeeID,
                          p.SpvEmployeeID,
                          p.InquiryDate,
                          p.OutletID,
                          p.StatusProspek,
                          p.PerolehanData,
                          p.NamaProspek,
                          p.AlamatProspek,
                          p.TelpRumah,
                          p.CityID,
                          p.NamaPerusahaan,
                          p.AlamatPerusahaan,
                          p.Jabatan,
                          p.Handphone,
                          p.Faximile,
                          p.Email,
                          p.TipeKendaraan,
                          p.Variant,
                          p.Transmisi,
                          p.ColourCode,
                          p.CaraPembayaran,
                          p.TestDrive,
                          p.QuantityInquiry,
                          p.LastProgress,
                          p.LastUpdateStatus,
                          p.SPKDate,
                          p.LostCaseDate,
                          p.LostCaseCategory,
                          p.LostCaseReasonID,
                          p.LostCaseOtherReason,
                          p.LostCaseVoiceOfCustomer,
                          p.Leasing,
                          p.DownPayment,
                          p.Tenor,
                          p.MerkLain,
                          v.Grade,
                          Variantstext = p.Variant,
                          Transmisistext = p.Transmisi,
                          ColourCodestext = r.RefferenceDesc1,
                          LostCaseReasonIDstext = q.LookUpValueName,
                          SpvEmployeeName = s.EmployeeName,
                          LeasingCode = p.Leasing,
                          CityName = t.LookUpValueName,
                          StatusVehicle = u.StatusVehicle,
                          BrandCode = u.OthersBrand,
                          ModelName = u.OthersType,
                          NikSales = p.EmployeeID,
                          NikSalesstext = (v.Position == "S" ? v.EmployeeName : "-- SELECT ONE --"),
                          //NikSC = p.SpvEmployeeID,
                      };

            var data = from p in ctx.PmActivites
                       where p.CompanyCode == CompanyCode
                       && p.BranchCode == branch
                       && p.InquiryNumber == inquno
                       select new
                       {
                           p.CompanyCode,
                           p.BranchCode,
                           p.InquiryNumber,
                           p.ActivityID,
                           p.ActivityDate,
                           p.ActivityType,
                           p.ActivityDetail,
                           p.NextFollowUpDate,
                       };

            if (qry.Count() > 0)
            {
                return Json(new { success = true, data = qry.FirstOrDefault(), list = data.ToList() });
            }
            else
            {
                return Json(new { success = false, message = "data not found..." });
            }
        }

        public JsonResult GetGrade(string id = "")
        {
            var model = ctx.HrEmployees.Find(CompanyCode, id);
            return Json(model.Grade);
        }

        //private void Duplicate2(InputKDP model)
        //{
        //    //var listKDP = string.Empty;
        //    //for (int i = 0; i < model.Qty; i++ )
        //    //{
        //        var newNumber = ctx.PmKdps.Select(p => p.InquiryNumber).Max() + 1;
        //        var record = ctx.PmKdps.Find(newNumber, model.BranchCode, CompanyCode);
        //        var userID = CurrentUser.UserId;
        //        var currentDate = DateTime.Now;
        //        if (record == null)
        //        {
        //            //var newNumber = ctx.PmKdps.Select(p => p.InquiryNumber).Max() + 1;
        //            var outlet = ctx.PmBranchOutlets.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == model.BranchCode).FirstOrDefault();
        //            record = new PmKdp
        //            {
        //                InquiryNumber = newNumber,
        //                CompanyCode = CompanyCode,
        //                BranchCode = model.BranchCode,
        //                CreatedBy = userID,
        //                CreationDate = currentDate,
        //                OutletID = (outlet == null) ? "" : outlet.OutletID,
        //            };
        //            ctx.PmKdps.Add(record);

        //        }
        //        record.EmployeeID = model.NikSales;
        //        record.SpvEmployeeID = model.SpvEmployeeID != "" ? model.SpvEmployeeID : model.NikSC ?? "";
        //        record.InquiryDate = model.InquiryDate;
        //        record.StatusProspek = model.StatusProspek;
        //        record.PerolehanData = model.PerolehanData;
        //        record.NamaProspek = model.NamaProspek;
        //        record.AlamatProspek = model.AlamatProspek;
        //        record.TelpRumah = model.TelpRumah;
        //        record.CityID = model.CityID;
        //        record.NamaPerusahaan = model.NamaPerusahaan;
        //        record.AlamatPerusahaan = model.AlamatPerusahaan;
        //        record.Jabatan = model.Jabatan;
        //        record.Handphone = model.Handphone;
        //        record.Faximile = model.Faximile;
        //        record.Email = model.Email;
        //        record.TipeKendaraan = model.TipeKendaraan;
        //        record.Variant = model.Variant;
        //        record.Transmisi = model.Transmisi;
        //        record.ColourCode = model.ColourCode;
        //        record.CaraPembayaran = model.CaraPembayaran;
        //        if (model.CaraPembayaran == "20")
        //        {
        //            record.Leasing = model.Leasing;
        //            record.DownPayment = model.DownPayment;
        //            record.Tenor = model.Tenor;
        //        }
        //        else
        //        {
        //            record.Leasing = "";
        //            record.DownPayment = "";
        //            record.Tenor = "";
        //        }
        //        record.TestDrive = model.TestDrive;
        //        //record.QuantityInquiry = ctx.PmActivites.Where(p => p.CompanyCode == record.CompanyCode && p.BranchCode == record.BranchCode && p.InquiryNumber == record.InquiryNumber).Count();
        //        record.QuantityInquiry = model.QuantityInquiry;
        //        record.LastProgress = model.LastProgress;
        //        record.LastUpdateStatus = model.LastUpdateStatus;
        //        record.SPKDate = model.SPKDate;
        //        record.LostCaseDate = model.LostCaseDate;
        //        record.LostCaseCategory = model.LostCaseCategory;
        //        record.LostCaseReasonID = model.LostCaseReasonID;
        //        record.LostCaseOtherReason = model.LostCaseOtherReason;
        //        record.LostCaseVoiceOfCustomer = model.LostCaseVoiceOfCustomer;
        //        record.MerkLain = model.MerkLain ?? "";
        //        record.LastUpdateBy = userID;
        //        record.LastUpdateDate = currentDate;

        //        Helpers.ReplaceNullable(record);
        //        ctx.SaveChanges();

        //        //if (i == model.Qty - 1)
        //        //    listKDP = listKDP + Convert.ToString(record.InquiryNumber);
        //        //else
        //        //    listKDP = listKDP + Convert.ToString(record.InquiryNumber) + ", ";

        //        var CopyDetail = ctx.Database.ExecuteSqlCommand("uspfn_DuplicateDetailKDP '" + model.InquiryNumber + "','" + record.InquiryNumber + "'");
        //    //}
        //}

        public JsonResult Duplicate(InputKDP model)
        {
            lock (lck)
            {
                var listKDP = string.Empty;
                var inqnoAwal = 0;
                for (int i = 0; i < model.Qty; i++)
                {
                    var newNumber = ctx.PmKdps.Select(p => p.InquiryNumber).Max() + 1;
                    if (i == 0)
                    {
                        inqnoAwal = newNumber;
                    }
                    var record = ctx.PmKdps.Find(newNumber, model.BranchCode, CompanyCode);
                    var userID = CurrentUser.UserId;
                    var currentDate = DateTime.Now;
                    if (record == null)
                    {
                        var outlet = ctx.PmBranchOutlets.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == model.BranchCode).FirstOrDefault();
                        record = new PmKdp
                        {
                            InquiryNumber = newNumber,
                            CompanyCode = CompanyCode,
                            BranchCode = model.BranchCode,
                            CreatedBy = userID,
                            CreationDate = currentDate,
                            OutletID = (outlet == null) ? "" : outlet.OutletID,
                        };
                        ctx.PmKdps.Add(record);

                    }
                    record.EmployeeID = model.NikSales;
                    record.SpvEmployeeID = model.SpvEmployeeID != "" ? model.SpvEmployeeID : model.NikSC ?? "";
                    record.InquiryDate = model.InquiryDate;
                    record.StatusProspek = model.StatusProspek;
                    record.PerolehanData = model.PerolehanData;
                    record.NamaProspek = model.NamaProspek;
                    record.AlamatProspek = model.AlamatProspek;
                    record.TelpRumah = model.TelpRumah;
                    record.CityID = model.CityID;
                    record.NamaPerusahaan = model.NamaPerusahaan;
                    record.AlamatPerusahaan = model.AlamatPerusahaan;
                    record.Jabatan = model.Jabatan;
                    record.Handphone = model.Handphone;
                    record.Faximile = model.Faximile;
                    record.Email = model.Email;
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
                    record.TestDrive = model.TestDrive;
                    record.QuantityInquiry = model.QuantityInquiry;
                    record.LastProgress = model.LastProgress;
                    record.LastUpdateStatus = model.LastUpdateStatus;
                    record.SPKDate = model.SPKDate;
                    record.LostCaseDate = model.LostCaseDate;
                    record.LostCaseCategory = model.LostCaseCategory;
                    record.LostCaseReasonID = model.LostCaseReasonID;
                    record.LostCaseOtherReason = model.LostCaseOtherReason;
                    record.LostCaseVoiceOfCustomer = model.LostCaseVoiceOfCustomer;
                    record.MerkLain = model.MerkLain ?? "";
                    record.LastUpdateBy = userID;
                    record.LastUpdateDate = currentDate;

                    try
                    {
                        Helpers.ReplaceNullable(record);
                        ctx.SaveChanges();
                        if (i == model.Qty - 1)
                            listKDP = listKDP + Convert.ToString(record.InquiryNumber);
                        else
                            listKDP = listKDP + Convert.ToString(record.InquiryNumber) + ", ";
                        var CopyDetail = ctx.Database.ExecuteSqlCommand("uspfn_DuplicateDetailKDP '" + model.InquiryNumber + "','" + record.InquiryNumber + "'");
                    }
                    catch
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Transaksi anda bersamaan dengan transaksi lain, Silahkan Simpan Kembali!"
                        });
                    }
                }
                return Json(new { success = true, message = "Duplicate data berhasil dengan No Inquiry " + listKDP });
            }
        }

        public JsonResult Save(PmKdp model)
        {
            DateTime dtStartRangeInq = DateTime.Now.AddDays(-7);
            DateTime dtEndRangeInq = DateTime.Now.AddDays(7);
            lock (lck)
            {
                var record = ctx.PmKdps.Find(model.InquiryNumber, model.BranchCode, CompanyCode);
                var userID = CurrentUser.UserId;
                var currentDate = DateTime.Now;
                if (record == null)
                {
                    if (model.InquiryDate < dtStartRangeInq || model.InquiryDate > dtEndRangeInq)
                    {
                        return Json(new { success = false, message = "Input Tanggal Inquiry harus berada di range tanggal " + dtStartRangeInq.ToString("dd-MMM-yyyy") + " s/d " + dtEndRangeInq.ToString("dd-MMM-yyyy") });
                    }
                    else if (model.NamaProspek.Trim().Length < 3)
                    {
                        return Json(new { success = false, message = "Nama Customer minimal 3 huruf !" });
                    }
                    else
                    {
                        var PmKdp = ctx.PmKdps;
                        var newNumber = PmKdp != null ? PmKdp.Select(p => p.InquiryNumber).Max() + 1 : 1;
                        var outlet = ctx.PmBranchOutlets.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == model.BranchCode).FirstOrDefault();
                        record = new PmKdp
                        {
                            InquiryNumber = newNumber,
                            CompanyCode = CompanyCode,
                            BranchCode = model.BranchCode,
                            CreatedBy = userID,
                            CreationDate = currentDate,
                            OutletID = (outlet == null) ? "" : outlet.OutletID,
                        };
                        ctx.PmKdps.Add(record);

                    }

                }
                if (model.LastProgress == "SPK")
                {
                    if (model.SPKDate == null)
                    {
                        return Json(new { success = false, message = "Tanggal SPK wajib diisi !" });
                    }
                    if (model.SPKDate.Value.Date < model.InquiryDate.Value.Date)
                    {
                        return Json(new { success = false, message = "Tanggal SPK harus sama atau lebih besar dari Tanggal Inquiry" });
                    }
                }
                if (model.LastProgress == "LOST")
                {
                    if (model.LostCaseDate == null)
                    {
                        return Json(new { success = false, message = "Tanggal LostCase wajib diisi !" });
                    }
                    if (model.LostCaseDate.Value.Date < model.InquiryDate.Value.Date)
                    {
                        return Json(new { success = false, message = "Tanggal Lost Case harus sama atau lebih besar dari Tanggal Inquiry" });
                    }

                }
                record.EmployeeID = model.NikSales;
                record.SpvEmployeeID = model.SpvEmployeeID != "" ? model.SpvEmployeeID : model.NikSC ?? "";
                record.InquiryDate = model.InquiryDate;
                record.StatusProspek = model.StatusProspek;
                record.PerolehanData = model.PerolehanData;
                record.NamaProspek = model.NamaProspek;
                record.AlamatProspek = model.AlamatProspek;
                record.TelpRumah = model.TelpRumah;
                record.CityID = model.CityID;
                record.NamaPerusahaan = model.NamaPerusahaan;
                record.AlamatPerusahaan = model.AlamatPerusahaan;
                record.Jabatan = model.Jabatan;
                record.Handphone = model.Handphone;
                record.Faximile = model.Faximile;
                record.Email = model.Email;
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
                record.TestDrive = model.TestDrive;
                //record.QuantityInquiry = ctx.PmActivites.Where(p => p.CompanyCode == record.CompanyCode && p.BranchCode == record.BranchCode && p.InquiryNumber == record.InquiryNumber).Count();
                record.QuantityInquiry = model.QuantityInquiry;
                record.LastProgress = model.LastProgress;
                record.LastUpdateStatus = currentDate;
                var StatusHistori = ctx.PmStatusHistories
                 .Where(a => a.InquiryNumber == record.InquiryNumber && a.CompanyCode == CompanyCode && a.BranchCode == model.BranchCode)
                 .OrderByDescending(b => b.SequenceNo).FirstOrDefault();
                if (StatusHistori == null)
                {
                    var hist = new PmStatusHistory
                    {

                        InquiryNumber = record.InquiryNumber,
                        CompanyCode = CompanyCode,
                        BranchCode = model.BranchCode,
                        SequenceNo = 1,
                        LastProgress = model.LastProgress,
                        UpdateDate = currentDate,
                        UpdateUser = userID
                    };

                    ctx.PmStatusHistories.Add(hist);
                }
                else if (StatusHistori.LastProgress != model.LastProgress)
                {
                    var newrecord = ctx.LookUpDtls.Where(a => a.CompanyCode == CompanyCode && a.CodeID == "PSTS" && a.LookUpValue == model.LastProgress).FirstOrDefault();
                    var exists = ctx.LookUpDtls.Where(a => a.CompanyCode == CompanyCode && a.CodeID == "PSTS" && a.LookUpValue == StatusHistori.LastProgress).FirstOrDefault();
                    if (newrecord != null && exists != null)
                    {
                        if (newrecord.SeqNo < exists.SeqNo)
                        {
                            return Json(new { success = false, message = " Status terakhir tidak boleh dirubah ke " + newrecord.LookUpValueName });
                        }
                    }
                    record.LastProgress = model.LastProgress;
                    record.LastUpdateStatus = currentDate;

                    var seqs = from p in ctx.PmStatusHistories
                               where p.InquiryNumber == record.InquiryNumber
                               && p.CompanyCode == record.CompanyCode
                               && p.BranchCode == record.BranchCode
                               select p.SequenceNo;

                    var newseqno = (seqs.Count() > 0) ? seqs.Max() + 1 : 1;
                    var hist = new PmStatusHistory
                    {
                        InquiryNumber = record.InquiryNumber,
                        CompanyCode = record.CompanyCode,
                        BranchCode = record.BranchCode,
                        SequenceNo = newseqno,
                        LastProgress = record.LastProgress,
                        UpdateDate = record.LastUpdateStatus,
                        UpdateUser = userID
                    };

                    ctx.PmStatusHistories.Add(hist);
                }
                record.SPKDate = model.SPKDate;
                record.LostCaseDate = model.LostCaseDate;
                record.LostCaseCategory = model.LostCaseCategory;
                record.LostCaseReasonID = model.LostCaseReasonID;
                record.LostCaseOtherReason = model.LostCaseOtherReason;
                record.LostCaseVoiceOfCustomer = model.LostCaseVoiceOfCustomer;
                record.MerkLain = model.MerkLain ?? "";
                record.LastUpdateBy = userID;
                record.LastUpdateDate = currentDate;

                var oKdpAdd = ctx.PmKdpAdditionals.Find(record.CompanyCode, record.BranchCode, record.InquiryNumber);
                if (oKdpAdd == null)
                {
                    oKdpAdd = new PmKdpAdditional
                    {
                        CompanyCode = record.CompanyCode,
                        BranchCode = record.BranchCode,
                        InquiryNumber = record.InquiryNumber,
                        CreatedBy = record.LastUpdateBy,
                        CreatedDate = record.LastUpdateDate,
                    };
                    ctx.PmKdpAdditionals.Add(oKdpAdd);
                }
                oKdpAdd.StatusVehicle = model.StatusVehicle;
                oKdpAdd.OthersBrand = model.BrandCode;
                oKdpAdd.OthersType = model.ModelName;
                oKdpAdd.LastUpdateBy = record.LastUpdateBy;
                oKdpAdd.LastUpdateDate = record.LastUpdateDate;

                try
                {
                    Helpers.ReplaceNullable(record);
                    Helpers.ReplaceNullable(oKdpAdd);
                    ctx.SaveChanges();
                    return Json(new
                    {
                        success = true,
                        data = new { InquiryNumber = record.InquiryNumber, CompanyCode = record.CompanyCode, BranchCode = record.BranchCode }
                    });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        //public JsonResult Save(PmKdp model)
        //{
        //    lock (lck)
        //    {
        //        DateTime dtStartRangeInq = DateTime.Now.AddDays(-7);
        //        DateTime dtEndRangeInq = DateTime.Now.AddDays(7);

        //        var record2 = ctx.PmKdps.Find(model.InquiryNumber, model.BranchCode, CompanyCode);
        //        if (record2 != null)
        //        {
        //            return Save2(model);
        //        }
        //        var record = ctx.PmKdps.Find(model.InquiryNumber, CompanyCode);
        //        var userID = CurrentUser.UserId;
        //        var currentDate = DateTime.Now;
        //        try
        //        {
        //            if (record == null)
        //            {
        //                if (model.InquiryDate.Value.Date < dtStartRangeInq.Date || model.InquiryDate.Value.Date > dtEndRangeInq.Date)
        //                {
        //                    return Json(new { success = false, message = "Input Tanggal Inquiry harus berada di range tanggal " + dtStartRangeInq.ToString("dd-MMM-yyyy") + " s/d " + dtEndRangeInq.ToString("dd-MMM-yyyy") });
        //                }
        //                else if (model.NamaProspek.Trim().Length < 3)
        //                {
        //                    return Json(new { success = false, message = "Nama Customer minimal 3 huruf !" });
        //                }
        //                else
        //                {
        //                    var newNumber = ctx.PmKdps.Select(p => p.InquiryNumber).Max() + 1;
        //                    var outlet = ctx.PmBranchOutlets.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == model.BranchCode).FirstOrDefault();
        //                    record = new PmKdp
        //                    {
        //                        InquiryNumber = newNumber,
        //                        CompanyCode = CompanyCode,
        //                        BranchCode = model.BranchCode,
        //                        CreatedBy = userID,
        //                        CreationDate = currentDate,
        //                        OutletID = (outlet == null) ? "" : outlet.OutletID,
        //                    };
        //                    ctx.PmKdps.Add(record);
        //                }

        //            }
        //            if (model.LastProgress == "SPK")
        //            {
        //                if (model.SPKDate == null)
        //                {
        //                    return Json(new { success = false, message = "Tanggal SPK wajib diisi !" });
        //                }
        //                if (model.SPKDate.Value.Date < model.InquiryDate.Value.Date)
        //                {
        //                    return Json(new { success = false, message = "Tanggal SPK harus sama atau lebih besar dari Tanggal Inquiry" });
        //                }
        //            }
        //            if (model.LastProgress == "LOST")
        //            {
        //                if (model.LostCaseDate == null)
        //                {
        //                    return Json(new { success = false, message = "Tanggal LostCase wajib diisi !" });
        //                }
        //                if (model.LostCaseDate.Value.Date < model.InquiryDate.Value.Date)
        //                {
        //                    return Json(new { success = false, message = "Tanggal Lost Case harus sama atau lebih besar dari Tanggal Inquiry" });
        //                }

        //            }
        //            record.EmployeeID = model.NikSales;
        //            record.SpvEmployeeID = model.SpvEmployeeID != "" ? model.SpvEmployeeID : model.NikSC ?? "";
        //            record.InquiryDate = model.InquiryDate;
        //            record.StatusProspek = model.StatusProspek;
        //            record.PerolehanData = model.PerolehanData;
        //            record.NamaProspek = model.NamaProspek;
        //            record.AlamatProspek = model.AlamatProspek;
        //            record.TelpRumah = model.TelpRumah;
        //            record.CityID = model.CityID;
        //            record.NamaPerusahaan = model.NamaPerusahaan;
        //            record.AlamatPerusahaan = model.AlamatPerusahaan;
        //            record.Jabatan = model.Jabatan;
        //            record.Handphone = model.Handphone;
        //            record.Faximile = model.Faximile;
        //            record.Email = model.Email;
        //            record.TipeKendaraan = model.TipeKendaraan;
        //            record.Variant = model.Variant;
        //            record.Transmisi = model.Transmisi;
        //            record.ColourCode = model.ColourCode;
        //            record.CaraPembayaran = model.CaraPembayaran;
        //            if (model.CaraPembayaran == "20")
        //            {
        //                record.Leasing = model.Leasing;
        //                record.DownPayment = model.DownPayment;
        //                record.Tenor = model.Tenor;
        //            }
        //            else
        //            {
        //                record.Leasing = "";
        //                record.DownPayment = "";
        //                record.Tenor = "";
        //            }
        //            record.TestDrive = model.TestDrive;
        //            //record.QuantityInquiry = ctx.PmActivites.Where(p => p.CompanyCode == record.CompanyCode && p.BranchCode == record.BranchCode && p.InquiryNumber == record.InquiryNumber).Count();
        //            record.QuantityInquiry = model.QuantityInquiry;
        //            record.LastProgress = model.LastProgress;
        //            record.LastUpdateStatus = model.LastUpdateStatus;
        //            record.SPKDate = model.SPKDate;
        //            record.LostCaseDate = model.LostCaseDate;
        //            record.LostCaseCategory = model.LostCaseCategory;
        //            record.LostCaseReasonID = model.LostCaseReasonID;
        //            record.LostCaseOtherReason = model.LostCaseOtherReason;
        //            record.LostCaseVoiceOfCustomer = model.LostCaseVoiceOfCustomer;
        //            record.MerkLain = model.MerkLain ?? "";
        //            record.LastUpdateBy = userID;
        //            record.LastUpdateDate = currentDate;

        //            Helpers.ReplaceNullable(record);
        //            ctx.SaveChanges();
        //            //var query = ctx.Database.SqlQuery<String>("exec uspfn_InsertPmkdp @CompanyCode=@p0, @BranchCode=@p1, @InquiryNumber=@p2, @InquiryNumberDuplicate=@p3",
        //            //CompanyCode, BranchCode, record.InquiryNumber, "");
        //            //var sql = string.Format("exec uspfn_InsertPmkdp '{0}', '{1}', '{2}', '{3}', '{4}'",
        //            //       CompanyCode, BranchCode, record.InquiryNumber, "", 0);
        //            //ctx.Database.ExecuteSqlCommand(sql);
        //            //model.InquiryNumber = record.InquiryNumber;
        //            //Save2(model);
        //            return Json(new
        //            {
        //                success = true,
        //                data = new { InquiryNumber = record.InquiryNumber, CompanyCode = record.CompanyCode, BranchCode = record.BranchCode }
        //            });

        //        }
        //        catch (Exception ex)
        //        {
        //            //model.InquiryNumber = ctx.PmKdps.Select(p => p.InquiryNumber).Max() + 1;
        //            //return Save(model);
        //            return Json(new
        //            {
        //                success = false,
        //                message = "Transaksi anda bersamaan dengan transaksi lain, Silahkan Simpan Kembali!"
        //            });
        //        }
        //    }
        //}

        public JsonResult SaveAct(PmActivity model)
        {
            var record = ctx.PmActivites.Find(CompanyCode, model.BranchCode, model.InquiryNumber, model.ActivityID);
            var userID = CurrentUser.UserId;
            var currentDate = DateTime.Now;
            if (record == null)
            {
                var numbers = from p in ctx.PmActivites
                              where p.CompanyCode == CompanyCode
                              && p.BranchCode == model.BranchCode
                              && p.InquiryNumber == model.InquiryNumber
                              select p.ActivityID;
                var newnumber = (numbers.Count() > 0) ? numbers.Max() + 1 : 1;

                record = new PmActivity
                {
                    CompanyCode = CompanyCode,
                    BranchCode = model.BranchCode,
                    InquiryNumber = model.InquiryNumber,
                    ActivityID = newnumber,
                    CreatedBy = userID,
                    CreationDate = currentDate,
                };
                ctx.PmActivites.Add(record);
            }
            record.ActivityDate = model.ActivityDate;
            record.ActivityType = model.ActivityType;
            record.ActivityDetail = model.ActivityDetail;
            record.NextFollowUpDate = model.NextFollowUpDate;
            record.LastUpdateBy = userID;
            record.LastUpdateDate = currentDate;

            try
            {
                Helpers.ReplaceNullable(record);
                ctx.SaveChanges();

                var kdp = ctx.PmKdps.Find(record.InquiryNumber, record.BranchCode, record.CompanyCode);
                if (kdp != null)
                {
                    var list = ctx.PmActivites.Where(p => p.CompanyCode == record.CompanyCode && p.BranchCode == record.BranchCode && p.InquiryNumber == record.InquiryNumber);
                    kdp.QuantityInquiry = list.Count();
                    kdp.LastUpdateBy = userID;
                    kdp.LastUpdateDate = currentDate;
                }

                var data = from p in ctx.PmActivites
                           where p.CompanyCode == record.CompanyCode
                           && p.BranchCode == record.BranchCode
                           && p.InquiryNumber == record.InquiryNumber
                           select new
                           {
                               p.CompanyCode,
                               p.BranchCode,
                               p.InquiryNumber,
                               p.ActivityID,
                               p.ActivityDate,
                               p.ActivityType,
                               p.ActivityDetail,
                               p.NextFollowUpDate,
                           };
                return Json(new { success = true, list = data.ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult DeleteAct(PmActivity model)
        {
            var userID = CurrentUser.UserId;
            var currentDate = DateTime.Now;

            var record = ctx.PmActivites.Find(model.CompanyCode, model.BranchCode, model.InquiryNumber, model.ActivityID);
            if (record != null) { ctx.PmActivites.Remove(record); }

            try
            {
                ctx.SaveChanges();

                var kdp = ctx.PmKdps.Find(record.InquiryNumber, record.BranchCode, record.CompanyCode);
                if (kdp != null)
                {
                    var list = ctx.PmActivites.Where(p => p.CompanyCode == record.CompanyCode && p.BranchCode == record.BranchCode && p.InquiryNumber == record.InquiryNumber);
                    kdp.QuantityInquiry = list.Count();
                    kdp.LastUpdateBy = userID;
                    kdp.LastUpdateDate = currentDate;
                }

                var data = from p in ctx.PmActivites
                           where p.CompanyCode == record.CompanyCode
                           && p.BranchCode == record.BranchCode
                           && p.InquiryNumber == record.InquiryNumber
                           select new
                           {
                               p.CompanyCode,
                               p.BranchCode,
                               p.InquiryNumber,
                               p.ActivityID,
                               p.ActivityDate,
                               p.ActivityType,
                               p.ActivityDetail,
                               p.NextFollowUpDate,
                           };
                return Json(new { success = true, list = data.ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult DeleteActs(int[] Activities)
        {
            var userID = CurrentUser.UserId;
            var currentDate = DateTime.Now;
            var ccode = Request["CompanyCode"];
            var bcode = Request["BranchCode"];
            var inqnu = Convert.ToInt32(Request["InquiryNumber"]);
            var anums = Request["Activities[]"];

            //if (!string.IsNullOrWhiteSpace(anums))
            if(Activities != null)
            {
                //var activities = anums.Split(',');
                //foreach (var id in anums.Split(','))
                //{
                //    //var record = ctx.PmActivites.Find(ccode, bcode, inqnu, Convert.ToInt32(id));
                //    //if (record != null) { ctx.PmActivites.Remove(record); }
                //}

                foreach (var id in Activities)
                {
                    var record = ctx.PmActivites.Find(ccode, bcode, inqnu, id);
                    if (record != null) { ctx.PmActivites.Remove(record); }
                }
            }

            try
            {
                ctx.SaveChanges();
                var kdp = ctx.PmKdps.Find(inqnu, bcode, ccode);
                if (kdp != null)
                {
                    var list = ctx.PmActivites.Where(p => p.CompanyCode == ccode && p.BranchCode == bcode && p.InquiryNumber == inqnu);
                    kdp.QuantityInquiry = list.Count();
                    kdp.LastUpdateBy = userID;
                    kdp.LastUpdateDate = currentDate;
                }

                var data = from p in ctx.PmActivites
                           where p.CompanyCode == ccode
                           && p.BranchCode == bcode
                           && p.InquiryNumber == inqnu
                           select new
                           {
                               p.CompanyCode,
                               p.BranchCode,
                               p.InquiryNumber,
                               p.ActivityID,
                               p.ActivityDate,
                               p.ActivityType,
                               p.ActivityDetail,
                               p.NextFollowUpDate,
                           };
                return Json(new { success = true, list = data.ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Activities()
        {
            var inquno = Int32.Parse(Request["InquiryNumber"]);
            //var data = from p in ctx.PmActivites
            //           where p.CompanyCode == CompanyCode
            //           && p.InquiryNumber == inquno
            //           select new ActivityView()
            //           {
            //               CompanyCode = p.CompanyCode,
            //               BranchCode = p.BranchCode,
            //               InquiryNumber = p.InquiryNumber,
            //               ActivityID = p.ActivityID,
            //               ActivityDate = String.Format("{0:dd-MMM-yyyy}", p.ActivityDate),
            //               ActivityType = p.ActivityType,
            //               ActivityDetail = p.ActivityDetail,
            //               NextFollowUpDate = String.Format("{0:dd-MMM-yyyy}", p.NextFollowUpDate)
            //           };
            //return Json(data);
            var query = string.Format(@"
                SELECT CompanyCode , BranchCode , InquiryNumber, ActivityID, ActivityType, ActivityDetail, NextFollowUpDate, ActivityDate
                , REPLACE (CONVERT(varchar(250), NextFollowUpDate, 106), ' ','-') as NextFollowUpDates
                , REPLACE (CONVERT(varchar(250), ActivityDate, 106), ' ','-') as ActivityDates
                FROM dbo.pmActivities
                WHERE CompanyCode = '{0}'
                    AND BranchCode = '{1}'
                    AND InquiryNumber = '{2}'
                       ", CompanyCode, BranchCode, inquno);
            return Json(ctx.Database.SqlQuery<ActivityView>(query).AsQueryable());
        }

        public List<dynamic> NikSlList { get; set; }

        private class CboItem
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        public JsonResult SalesmanID(string EmployeeID)
        {
            var qry = string.Format(@"SELECT a.EmployeeID as value, b.EmployeeName as text
                FROM hrEmployeeMutation a
                JOIN (
                    SELECT c.EmployeeId, c.EmployeeName, c.Position, MAX(d.MutationDate) AS MutationDate
	                FROM hrEmployee c
	                JOIN hrEmployeeMutation d
	                ON c.EmployeeId = d.EmployeeId
	                WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
	                AND c.TeamLeader = '{2}'
	                GROUP BY c.EmployeeId, c.EmployeeName, c.Position
                    UNION
	                SELECT c.EmployeeId, c.EmployeeName, c.Position, MAX(d.MutationDate) AS MutationDate
	                FROM hrEmployee c
	                JOIN hrEmployeeMutation d
	                ON c.EmployeeId = d.EmployeeId
	                WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
	                AND c.TeamLeader in (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader = '{2}')
	                GROUP BY c.EmployeeId, c.EmployeeName, c.Position
	                
                ) b
                ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
                JOIN gnMstOrganizationDtl d
                ON a.BranchCode = d.BranchCode
                WHERE a.CompanyCode = '{0}' AND a.BranchCode = '{1}'
                ", CompanyCode, BranchCode, EmployeeID);
            var list = ctx.Database.SqlQuery<CboItem>(qry).AsQueryable();
            return Json(list);

            //var list = ctx.HrEmployees.Where(p => p.CompanyCode == CompanyCode && p.TeamLeader == EmployeeID)
            //    .OrderBy(p => p.EmployeeName )
            //    .Select(p => new { value = p.EmployeeID, text = p.EmployeeName.ToUpper() })
            //    .ToList();
            //return Json(list);
        }
    }
}
