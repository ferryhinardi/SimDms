using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.SUtility.Models;
using System.Text;
using Newtonsoft.Json;
using System.Threading;

namespace SimDms.SUtility.Controllers.Api
{
    public class SendSheduleController : BaseController
    {
        //public JsonResult SaveScheduleDataDms(GnMstScheduleData model, string TokenID, string ComputerName, string LastSend, string callback = "")
        //{
        //    try
        //    {
        //        if (CheckTokenAccess(model.CompanyCode, ComputerName, TokenID))
        //        {
        //            var entity = ctxDealer.GnMstScheduleDatas.Find(model.UniqueID);
        //            if (entity == null)
        //            {
        //                entity = new GnMstScheduleData { UniqueID = model.UniqueID };
        //                ctxDealer.GnMstScheduleDatas.Add(entity);
        //            }
        //            entity.CompanyCode = model.CompanyCode;
        //            entity.Data = model.Data;
        //            entity.DataType = model.DataType;
        //            entity.LastSendDate = model.LastSendDate;
        //            entity.Segment = model.Segment;
        //            entity.Status = "A";
        //            entity.CreatedDate = DateTime.Now;
        //            entity.UpdatedDate = DateTime.Now;
        //            try
        //            {
        //                //ctxDealer.SaveChanges();
        //                ProcessParsing(entity.UniqueID);

        //                return Jsonp(new
        //                {
        //                    Success = true,
        //                    CompanyCode = model.CompanyCode,
        //                    DataType = model.DataType
        //                }, callback, JsonRequestBehavior.AllowGet);
        //            }
        //            catch (Exception ex)
        //            {
        //                return Jsonp(new
        //                {
        //                    Success = false,
        //                    Message = ex.ToString(),
        //                    CompanyCode = model.CompanyCode,
        //                    DataType = model.DataType
        //                }, callback, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Jsonp(new
        //        {
        //            Success = false,
        //            Message = ex.ToString(),
        //            CompanyCode = model.CompanyCode,
        //            DataType = model.DataType
        //        }, callback, JsonRequestBehavior.AllowGet);
        //    }
        //    return Jsonp(new
        //    {
        //        Success = true,
        //        CompanyCode = model.CompanyCode,
        //        DataType = model.DataType
        //    }, callback, JsonRequestBehavior.AllowGet);
        //}

        //private void ProcessParsing(string pid)
        //{
        //    var data = ctxDealer.GnMstScheduleDatas.Find(pid);
        //    if (data != null)
        //    {
        //        switch (data.DataType)
        //        {
        //            case "EMPLY":
        //                var lstem01 = JsonConvert.DeserializeObject<List<HrEmployee>>(data.Data);
        //                foreach (var row in lstem01)
        //                {
        //                    var model = ctx.HrEmployees.Find(row.CompanyCode, row.EmployeeID);
        //                    if (model == null) ctx.HrEmployees.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "EMACH":
        //                var lstem02 = JsonConvert.DeserializeObject<List<HrEmployeeAchievement>>(data.Data);
        //                foreach (var row in lstem02)
        //                {
        //                    var model = ctx.HrEmployeeAchievements.Find(row.CompanyCode, row.EmployeeID, row.AssignDate);
        //                    if (model == null) ctx.HrEmployeeAchievements.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "EMUTA":
        //                var lstem03 = JsonConvert.DeserializeObject<List<HrEmployeeMutation>>(data.Data);
        //                foreach (var row in lstem03)
        //                {
        //                    var model = ctx.HrEmployeeMutations.Find(row.CompanyCode, row.EmployeeID, row.MutationDate);
        //                    if (model == null) ctx.HrEmployeeMutations.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "EMSFM":
        //                var lstem04 = JsonConvert.DeserializeObject<List<HrEmployeeSales>>(data.Data);
        //                foreach (var row in lstem04)
        //                {
        //                    var model = ctx.HrEmployeeSales.Find(row.CompanyCode, row.EmployeeID);
        //                    if (model == null) ctx.HrEmployeeSales.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;

        //            case "SVSPK":
        //                var lstsv01 = JsonConvert.DeserializeObject<List<SvTrnService>>(data.Data);
        //                foreach (var row in lstsv01)
        //                {
        //                    var model = ctx.SvTrnServices.Find(row.CompanyCode, row.BranchCode, row.ProductType, row.ServiceNo);
        //                    if (model == null) ctx.SvTrnServices.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "SVINV":
        //                var lstsv02 = JsonConvert.DeserializeObject<List<SvTrnInvoice>>(data.Data);
        //                foreach (var row in lstsv02)
        //                {
        //                    var model = ctx.SvTrnInvoices.Find(row.CompanyCode, row.BranchCode, row.ProductType, row.InvoiceNo);
        //                    if (model == null) ctx.SvTrnInvoices.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "SVMSI":
        //                var lstsv03 = JsonConvert.DeserializeObject<List<SvHstSzkMsi>>(data.Data);
        //                foreach (var row in lstsv03)
        //                {
        //                    var model = ctx.SvHstSzkMsies.Find(row.CompanyCode, row.BranchCode, row.PeriodYear, row.PeriodMonth, row.SeqNo);
        //                    if (model == null) ctx.SvHstSzkMsies.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;

        //            case "PMKDP":
        //                var lstpm01 = JsonConvert.DeserializeObject<List<PmKdp>>(data.Data);
        //                foreach (var row in lstpm01)
        //                {
        //                    var model = ctx.PmKdps.Find(row.InquiryNumber, row.BranchCode, row.CompanyCode);
        //                    if (model == null) ctx.PmKdps.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "PMSHS":
        //                var lstpm02 = JsonConvert.DeserializeObject<List<PmStatusHistory>>(data.Data);
        //                foreach (var row in lstpm02)
        //                {
        //                    var model = ctx.PmStatusHistories.Find(row.InquiryNumber, row.CompanyCode, row.BranchCode, row.SequenceNo);
        //                    if (model == null) ctx.PmStatusHistories.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "PMACT":
        //                var lstpm03 = JsonConvert.DeserializeObject<List<PmActivity>>(data.Data);
        //                foreach (var row in lstpm03)
        //                {
        //                    var model = ctx.PmActivities.Find(row.CompanyCode, row.BranchCode, row.InquiryNumber, row.ActivityID);
        //                    if (model == null) ctx.PmActivities.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "CDTATRANS":
        //                var lstpm04 = JsonConvert.DeserializeObject<List<GnMstCustDealer>>(data.Data);
        //                foreach (var row in lstpm04)
        //                {
        //                    var model = ctx.GnMstCustDealers.Find(row.CompanyCode, row.BranchCode, row.SelectCode, row.Year, row.Month);
        //                    if (model == null) ctx.GnMstCustDealers.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "CSZKDATA":
        //                var lstpm05 = JsonConvert.DeserializeObject<List<GnMstCustDealer>>(data.Data);
        //                foreach (var row in lstpm05)
        //                {
        //                    var model = ctx.GnMstCustDealers.Find(row.CompanyCode, row.BranchCode, row.SelectCode, row.Year, row.Month);
        //                    if (model == null) ctx.GnMstCustDealers.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "CUSTDATA":
        //                var lstpm06 = JsonConvert.DeserializeObject<List<GnMstCustDealer>>(data.Data);
        //                foreach (var row in lstpm06)
        //                {
        //                    var model = ctx.GnMstCustDealers.Find(row.CompanyCode, row.BranchCode, row.SelectCode, row.Year, row.Month);
        //                    if (model == null) ctx.GnMstCustDealers.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "CSZKTRDET":
        //                var lstpm07 = JsonConvert.DeserializeObject<List<GnMstCustDealerDtl>>(data.Data);
        //                foreach (var row in lstpm07)
        //                {
        //                    var model = ctx.GnMstCustDealerDtls.Find(row.CompanyCode, row.BranchCode, row.CustomerCode, row.LastTransactionDate, row.TransType);
        //                    if (model == null) ctx.GnMstCustDealerDtls.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;


        //            case "CS3DCALL":
        //                var lstpm08 = JsonConvert.DeserializeObject<List<CsTDayCall>>(data.Data);
        //                foreach (var row in lstpm08)
        //                {
        //                    var model = ctx.CsTDayCalls.Find(row.CompanyCode, row.CustomerCode, row.Chassis);
        //                    if (model == null) ctx.CsTDayCalls.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "CSBDAY":
        //                var lstpm09 = JsonConvert.DeserializeObject<List<CsCustBirthDay>>(data.Data);
        //                foreach (var row in lstpm09)
        //                {
        //                    var model = ctx.CsCustBirthDays.Find(row.CompanyCode, row.CustomerCode);
        //                    if (model == null) ctx.CsCustBirthDays.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "CSBKPB":
        //                var lstpm10 = JsonConvert.DeserializeObject<List<CsCustBpkb>>(data.Data);
        //                foreach (var row in lstpm10)
        //                {
        //                    var model = ctx.CsCustBpkbs.Find(row.CompanyCode, row.CustomerCode, row.Chassis);
        //                    if (model == null) ctx.CsCustBpkbs.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "CSCUSTDATA":
        //                var lstpm11 = JsonConvert.DeserializeObject<List<CsCustData>>(data.Data);
        //                foreach (var row in lstpm11)
        //                {
        //                    var model = ctx.CsCustDatas.Find(row.CompanyCode, row.CustomerCode);
        //                    if (model == null) ctx.CsCustDatas.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "CSFEEDBACK":
        //                var lstpm12 = JsonConvert.DeserializeObject<List<CsCustFeedback>>(data.Data);
        //                foreach (var row in lstpm12)
        //                {
        //                    var model = ctx.CsCustFeedbacks.Find(row.CompanyCode, row.CustomerCode, row.Chassis);
        //                    if (model == null) ctx.CsCustFeedbacks.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "CSHLDAY":
        //                var lstpm13 = JsonConvert.DeserializeObject<List<CsCustHoliday>>(data.Data);
        //                foreach (var row in lstpm13)
        //                {
        //                    var model = ctx.CsCustHolidays.Find(row.CompanyCode, row.CustomerCode, row.PeriodYear, row.GiftSeq);
        //                    if (model == null) ctx.CsCustHolidays.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "CSMSTHLDAY":
        //                var lstpm14 = JsonConvert.DeserializeObject<List<CsMstHoliday>>(data.Data);
        //                foreach (var row in lstpm14)
        //                {
        //                    var model = ctx.CsMstHolidays.Find(row.CompanyCode, row.HolidayYear, row.HolidayCode);
        //                    if (model == null) ctx.CsMstHolidays.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "CSRLTN":
        //                var lstpm15 = JsonConvert.DeserializeObject<List<CsCustRelation>>(data.Data);
        //                foreach (var row in lstpm15)
        //                {
        //                    var model = ctx.CsCustRelations.Find(row.CompanyCode, row.CustomerCode, row.RelationType);
        //                    if (model == null) ctx.CsCustRelations.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "CSSTNKEXT":
        //                var lstpm16 = JsonConvert.DeserializeObject<List<CsStnkExt>>(data.Data);
        //                foreach (var row in lstpm16)
        //                {
        //                    var model = ctx.CsStnkExts.Find(row.CompanyCode, row.CustomerCode, row.Chassis);
        //                    if (model == null) ctx.CsStnkExts.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "CSVHCL":
        //                var lstpm17 = JsonConvert.DeserializeObject<List<CsCustomerVehicle>>(data.Data);
        //                foreach (var row in lstpm17)
        //                {
        //                    var model = ctx.CsCustomerVehicles.Find(row.CompanyCode, row.CustomerCode, row.Chassis);
        //                    if (model == null) ctx.CsCustomerVehicles.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "MSTCUST":
        //                var lstpm18 = JsonConvert.DeserializeObject<List<gnMstCustomer>>(data.Data);
        //                foreach (var row in lstpm18)
        //                {
        //                    var model = ctx.gnMstCustomers.Find(row.CompanyCode, row.CustomerCode);
        //                    if (model == null) ctx.gnMstCustomers.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "SVMSTCVHCL":
        //                var lstpm19 = JsonConvert.DeserializeObject<List<svMstCustomerVehicle>>(data.Data);
        //                foreach (var row in lstpm19)
        //                {
        //                    var model = ctx.svMstCustomerVehicles.Find(row.CompanyCode, row.ChassisCode, row.ChassisNo);
        //                    if (model == null) ctx.svMstCustomerVehicles.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "TRSLSBPK":
        //                var lstpm20 = JsonConvert.DeserializeObject<List<omTrSalesBPK>>(data.Data);
        //                foreach (var row in lstpm20)
        //                {
        //                    var model = ctx.omTrSalesBPKs.Find(row.CompanyCode, row.BranchCode, row.BPKNo);
        //                    if (model == null) ctx.omTrSalesBPKs.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "TRSLSDO":
        //                var lstpm21 = JsonConvert.DeserializeObject<List<omTrSalesDO>>(data.Data);
        //                foreach (var row in lstpm21)
        //                {
        //                    var model = ctx.omTrSalesDOs.Find(row.CompanyCode, row.BranchCode, row.DONo);
        //                    if (model == null) ctx.omTrSalesDOs.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "TRSLSDODTL":
        //                var lstpm22 = JsonConvert.DeserializeObject<List<omTrSalesDODetail>>(data.Data);
        //                foreach (var row in lstpm22)
        //                {
        //                    var model = ctx.omTrSalesDODetails.Find(row.CompanyCode, row.BranchCode, row.DONo, row.DOSeq);
        //                    if (model == null) ctx.omTrSalesDODetails.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "TRSLSINV":
        //                var lstpm23 = JsonConvert.DeserializeObject<List<omTrSalesInvoice>>(data.Data);
        //                foreach (var row in lstpm23)
        //                {
        //                    var model = ctx.omTrSalesInvoices.Find(row.CompanyCode, row.BranchCode, row.InvoiceNo);
        //                    if (model == null) ctx.omTrSalesInvoices.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "TRSLSINVIN":
        //                var lstpm24 = JsonConvert.DeserializeObject<List<omTrSalesInvoiceVin>>(data.Data);
        //                foreach (var row in lstpm24)
        //                {
        //                    var model = ctx.omTrSalesInvoiceVins.Find(row.CompanyCode, row.BranchCode, row.InvoiceNo, row.BPKNo, row.SalesModelCode, row.SalesModelYear, row.InvoiceSeq);
        //                    if (model == null) ctx.omTrSalesInvoiceVins.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "TRSLSSOVIN":
        //                var omTrSalesSoVin = JsonConvert.DeserializeObject<List<OmTrSalesSoVin>>(data.Data);
        //                foreach (var row in omTrSalesSoVin)
        //                {
        //                    var model = ctx.OmTrSalesSoVins.Find(row.CompanyCode, row.BranchCode, row.SONo, row.SalesModelCode, row.SalesModelYear, row.ColourCode, row.SOSeq);
        //                    if (model == null) ctx.OmTrSalesSoVins.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //            case "TRSLSSO":
        //                var lstpm25 = JsonConvert.DeserializeObject<List<omTrSalesSO>>(data.Data);
        //                foreach (var row in lstpm25)
        //                {
        //                    var model = ctx.omTrSalesSOs.Find(row.CompanyCode, row.BranchCode, row.SONo);
        //                    if (model == null) ctx.omTrSalesSOs.Add(row);
        //                    else extend(model, row);
        //                }
        //                UpdateSchedule(pid);
        //                break;
        //                //SavePmHstIts
        //            case "SITSH":
        //                var lstpm26 = JsonConvert.DeserializeObject<List<pmHstITS>>(data.Data);
        //                var addLocalTime = 7;
        //                var lastUpdateDate = new DateTime(1900, 1, 1).AddHours(addLocalTime);
        //                bool isNew = false;
        //                foreach (var item in lstpm26)
        //                {
        //                    var entity = ctx.pmHstITSes.Find(item.CompanyCode, item.BranchCode, item.InquiryNumber);
        //                    if (entity == null)
        //                    {
        //                        entity = new pmHstITS();
        //                        entity.InquiryNumber = item.InquiryNumber;
        //                        entity.CompanyCode = item.CompanyCode;
        //                        entity.BranchCode = item.BranchCode;
        //                        entity.CreatedBy = item.CreatedBy;
        //                        entity.CreatedDate = item.CreatedDate.AddHours(addLocalTime);
        //                        isNew = true;
        //                    }
        //                    entity.AlamatPerusahaan = item.AlamatPerusahaan != null ? item.AlamatPerusahaan : string.Empty;
        //                    entity.AlamatProspek = item.AlamatProspek != null ? item.AlamatProspek : string.Empty;
        //                    entity.BranchHead = item.BranchHead != null ? item.BranchHead : string.Empty;
        //                    entity.CaraPembayaran = item.CaraPembayaran != null ? item.CaraPembayaran : string.Empty;
        //                    entity.City = item.City != null ? item.City : string.Empty;
        //                    entity.ColourCode = item.ColourCode != null ? item.ColourCode : string.Empty;
        //                    entity.ColourDescription = item.ColourDescription != null ? item.ColourDescription : string.Empty;

        //                    entity.DeliveryDate = item.DeliveryDate.HasValue == true ? item.DeliveryDate.Value.AddHours(addLocalTime) : new DateTime(1900, 01, 10).AddHours(addLocalTime);
        //                    entity.DownPayment = item.DownPayment != null ? item.DownPayment : string.Empty;
        //                    entity.Email = item.Email != null ? item.Email : string.Empty;
        //                    entity.Faximile = item.Faximile != null ? item.Faximile : string.Empty;
        //                    entity.Handphone = item.Handphone != null ? item.Handphone : string.Empty;
        //                    entity.HotDate = item.HotDate.HasValue == true ? item.HotDate.Value.AddHours(addLocalTime) : new DateTime(1900, 01, 10).AddHours(addLocalTime);
        //                    entity.InquiryDate = item.InquiryDate.HasValue == true ? item.InquiryDate.Value.AddHours(addLocalTime) : new DateTime(1900, 01, 10).AddHours(addLocalTime);
        //                    entity.Jabatan = item.Jabatan != null ? item.Jabatan : string.Empty;
        //                    entity.LastProgress = item.LastProgress != null ? item.LastProgress : string.Empty;
        //                    entity.LastUpdateBy = item.LastUpdateBy != null ? item.LastUpdateBy : string.Empty;
        //                    entity.LastUpdateDate = item.LastUpdateDate.AddHours(addLocalTime);
        //                    entity.LastUpdateStatus = item.LastUpdateStatus.HasValue == true ? item.LastUpdateStatus.Value.AddHours(addLocalTime) : new DateTime(1900, 01, 10).AddHours(addLocalTime);
        //                    entity.Leasing = item.Leasing != null ? item.Leasing : string.Empty;
        //                    entity.LostCaseCategory = item.LostCaseCategory != null ? item.LostCaseCategory : string.Empty;
        //                    entity.LostCaseDate = item.LostCaseDate.HasValue == true ? item.LostCaseDate.Value.AddHours(addLocalTime) : new DateTime(1900, 01, 10).AddHours(addLocalTime);
        //                    entity.LostCaseReasonID = item.LostCaseReasonID != null ? item.LostCaseReasonID : string.Empty;
        //                    entity.LostCaseVoiceOfCustomer = item.LostCaseVoiceOfCustomer != null ? item.LostCaseVoiceOfCustomer : string.Empty;
        //                    entity.MerkLain = item.MerkLain != null ? item.MerkLain : string.Empty;
        //                    entity.NamaPerusahaan = item.NamaPerusahaan != null ? item.NamaPerusahaan : string.Empty;
        //                    entity.NamaProspek = item.NamaProspek != null ? item.NamaProspek : string.Empty;
        //                    entity.OutletID = item.OutletID != null ? item.OutletID : string.Empty;
        //                    entity.PerolehanData = item.PerolehanData != null ? item.PerolehanData : string.Empty;
        //                    entity.ProspectDate = item.ProspectDate.HasValue == true ? item.ProspectDate.Value.AddHours(addLocalTime) : new DateTime(1900, 01, 10).AddHours(addLocalTime);
        //                    entity.QuantityInquiry = item.QuantityInquiry != null ? item.QuantityInquiry : 0;
        //                    entity.SalesCoordinator = item.SalesCoordinator != null ? item.SalesCoordinator : string.Empty;
        //                    entity.SalesHead = item.SalesHead != null ? item.SalesHead : string.Empty;
        //                    entity.SPKDate = item.SPKDate.HasValue == true ? item.SPKDate.Value.AddHours(addLocalTime) : new DateTime(1900, 01, 10).AddHours(addLocalTime);
        //                    entity.StatusProspek = item.StatusProspek != null ? item.StatusProspek : string.Empty;
        //                    entity.TelpRumah = item.TelpRumah != null ? item.TelpRumah : string.Empty;
        //                    entity.Tenor = item.Tenor != null ? item.Tenor : string.Empty;
        //                    entity.TestDrive = item.TestDrive != null ? item.TestDrive : string.Empty;
        //                    entity.TipeKendaraan = item.TipeKendaraan != null ? item.TipeKendaraan : string.Empty;
        //                    entity.Transmisi = item.Transmisi != null ? item.Transmisi : string.Empty;
        //                    entity.Variant = item.Variant != null ? item.Variant : string.Empty;
        //                    entity.Wiraniaga = item.Wiraniaga != null ? item.Wiraniaga : string.Empty;
        //                    lastUpdateDate = item.LastUpdateDate > lastUpdateDate ? item.LastUpdateDate : lastUpdateDate;

        //                    if (isNew)
        //                        ctx.pmHstITSes.Add(entity);

        //                    isNew = false;

        //                    UpdateSchedule(pid);
        //                }
        //                break;
        //                //SaveSHIST
        //            case "SHIST":
        //                var companyCode = "0";
        //                var objSHIST = JsonConvert.DeserializeObject<List<omHstInquirySales>>(data.Data);
        //                var callback = string.Empty;
        //                addLocalTime = 7;
        //                //var lastUpdateDate = DateTime.Now;
        //                try
        //                {
        //                    foreach (var item in objSHIST)
        //                    {
        //                        companyCode = item.CompanyCode + "-HI";
        //                        isNew = false;
        //                        var soNo = item.SoNo.Length > 13 ? item.SoNo.Substring(0, 13) : item.SoNo;
        //                        var entity = ctx.omHstInquirySalesses.Where(m => m.CompanyCode == item.CompanyCode && m.ChassisCode == item.ChassisCode && m.ChassisNo == item.ChassisNo && m.SoNo.Contains(soNo) && m.Status == true).FirstOrDefault();
        //                        if (entity == null)
        //                        {
        //                            entity = new omHstInquirySales();
        //                            entity.BranchCode = item.BranchCode;
        //                            entity.CompanyCode = item.CompanyCode;
        //                            entity.Year = item.Year;
        //                            entity.Area = item.Area != null ? item.Area : string.Empty;
        //                            entity.BranchHeadID = item.BranchHeadID != null ? item.BranchHeadID : string.Empty;
        //                            entity.BranchHeadName = item.BranchHeadName != null ? item.BranchHeadName : string.Empty;
        //                            entity.BranchName = item.BranchName != null ? item.BranchName : string.Empty;
        //                            entity.ChassisCode = item.ChassisCode != null ? item.ChassisCode : string.Empty;
        //                            entity.ChassisNo = item.ChassisNo != null ? item.ChassisNo : 0;
        //                            entity.ColourCode = item.ColourCode != null ? item.ColourCode : string.Empty;
        //                            entity.ColourName = item.ColourName != null ? item.ColourName : string.Empty;
        //                            entity.ColumnMarketModel = item.ColumnMarketModel != null ? item.ColumnMarketModel : string.Empty;
        //                            entity.CompanyName = item.CompanyName != null ? item.CompanyName : string.Empty;
        //                            entity.CreatedBy = item.CreatedBy != null ? item.CreatedBy : string.Empty;
        //                            entity.CreatedDate = item.CreatedDate != null ? item.CreatedDate.Value.AddHours(addLocalTime) : new DateTime(1900, 1, 1).AddHours(addLocalTime);
        //                            entity.EngineCode = item.EngineCode != null ? item.EngineCode : string.Empty;
        //                            entity.EngineNo = item.EngineNo != null ? item.EngineNo : 0;
        //                            entity.FakturPolisiDesc = item.FakturPolisiDesc != null ? item.FakturPolisiDesc : string.Empty;
        //                            entity.Grade = item.Grade != null ? item.Grade : string.Empty;
        //                            entity.GradeDate = item.GradeDate != null ? item.GradeDate.Value.AddHours(addLocalTime) : new DateTime(1900, 1, 1).AddHours(addLocalTime);
        //                            entity.GroupMarketModel = item.GroupMarketModel != null ? item.GroupMarketModel : string.Empty;
        //                            entity.InvoiceDate = item.InvoiceDate != null ? item.InvoiceDate.Value.AddHours(addLocalTime) : new DateTime(1900, 1, 1).AddHours(addLocalTime);
        //                            entity.InvoiceNo = item.InvoiceNo != null ? item.InvoiceNo : string.Empty;
        //                            entity.JoinDate = item.JoinDate != null ? item.JoinDate.Value.AddHours(addLocalTime) : new DateTime(1900, 1, 1).AddHours(addLocalTime);
        //                            entity.MarketModel = item.MarketModel != null ? item.MarketModel : string.Empty;
        //                            entity.ModelCatagory = item.ModelCatagory != null ? item.ModelCatagory : string.Empty;
        //                            entity.Month = item.Month;
        //                            entity.ResignDate = item.ResignDate != null ? item.ResignDate.Value.AddHours(addLocalTime) : new DateTime(1900, 1, 1).AddHours(addLocalTime);
        //                            entity.SalesCoordinatorID = item.SalesCoordinatorID != null ? item.SalesCoordinatorID : string.Empty;
        //                            entity.SalesCoordinatorName = item.SalesCoordinatorName != null ? item.SalesCoordinatorName : string.Empty;
        //                            entity.SalesHeadID = item.SalesHeadID != null ? item.SalesHeadID : string.Empty;
        //                            entity.SalesHeadName = item.SalesHeadName != null ? item.SalesHeadName : string.Empty;
        //                            entity.SalesmanID = item.SalesmanID != null ? item.SalesmanID : string.Empty;
        //                            entity.SalesmanName = item.SalesmanName != null ? item.SalesmanName : string.Empty;
        //                            entity.SalesModelCode = item.SalesModelCode != null ? item.SalesModelCode : string.Empty;
        //                            entity.SalesModelDesc = item.SalesModelDesc != null ? item.SalesModelDesc : string.Empty;
        //                            entity.SalesModelYear = item.SalesModelYear != null ? item.SalesModelYear : 0;
        //                            entity.SalesType = item.SalesType != null ? item.SalesType : string.Empty;
        //                            entity.AfterDiscDPP = item.AfterDiscDPP != null ? item.AfterDiscDPP : 0;
        //                            entity.AfterDiscPPn = item.AfterDiscPPn != null ? item.AfterDiscPPn : 0;
        //                            entity.AfterDiscPPnBM = item.AfterDiscPPnBM != null ? item.AfterDiscPPnBM : 0;
        //                            entity.AfterDiscTotal = item.AfterDiscTotal != null ? item.AfterDiscTotal : 0;
        //                            entity.BeforeDiscDPP = item.BeforeDiscDPP != null ? item.BeforeDiscDPP : 0;
        //                            entity.COGS = item.COGS != null ? item.COGS : 0;
        //                            entity.DepositAmt = item.DepositAmt != null ? item.DepositAmt : 0;
        //                            entity.DiscExcludePPn = item.DiscExcludePPn != null ? item.DiscExcludePPn : 0;
        //                            entity.DiscIncludePPn = item.DiscIncludePPn != null ? item.DiscIncludePPn : 0;
        //                            entity.OthersAmt = item.OthersAmt != null ? item.OthersAmt : 0;
        //                            entity.OthersDPP = item.OthersDPP != null ? item.OthersDPP : 0;
        //                            entity.OthersPPn = item.OthersPPn != null ? item.OthersPPn : 0;
        //                            entity.PPnBMPaid = item.PPnBMPaid != null ? item.PPnBMPaid : 0;
        //                            entity.ShipAmt = item.ShipAmt != null ? item.ShipAmt : 0;
        //                            entity.SuzukiDODate = item.SuzukiDODate != null ? item.SuzukiDODate.Value.AddHours(addLocalTime) : new DateTime(1900, 1, 1).AddHours(addLocalTime);
        //                            entity.SuzukiFPolDate = item.SuzukiFPolDate != null ? item.SuzukiFPolDate.Value.AddHours(addLocalTime) : new DateTime(1900, 1, 1).AddHours(addLocalTime);
        //                            isNew = true;
        //                        }
        //                        entity.Status = item.Status != null ? item.Status : false;
        //                        entity.SoNo = item.SoNo != null ? item.SoNo : string.Empty;
        //                        entity.SODate = item.SODate != null ? item.SODate.Value.AddHours(addLocalTime) : new DateTime(1900, 1, 1).AddHours(addLocalTime);
        //                        entity.FakturPolisiNo = item.FakturPolisiNo != null ? item.FakturPolisiNo : string.Empty;
        //                        entity.FakturPolisiDate = item.FakturPolisiDate != null ? item.FakturPolisiDate.Value.AddHours(addLocalTime) : new DateTime(1900, 1, 1).AddHours(addLocalTime);
        //                        entity.LastUpdateBy = item.LastUpdateBy != null ? item.LastUpdateBy : string.Empty;
        //                        entity.LastUpdateDate = item.LastUpdateDate != null ? item.LastUpdateDate.Value.AddHours(addLocalTime) : new DateTime(1900, 1, 1).AddHours(addLocalTime);

        //                        if (isNew)
        //                            ctx.omHstInquirySalesses.Add(entity);


        //                        isNew = false;
        //                        UpdateSchedule(pid);
        //                        break;
        //                    }
        //                }
        //                catch
        //                {

        //                }
        //                break;
        //                //SaveSITSS
        //            case "SITSS":
        //            var companycode = "0";
        //            var objSITSS = JsonConvert.DeserializeObject<List<PmStatusHistory>>(data.Data);
        //            addLocalTime = 7;
        //            callback = string.Empty;
        //            try
        //            {
        //                foreach (var item in objSITSS)
        //                {
        //                    companycode = item.CompanyCode + "-SS";
        //                    isNew = false;
        //                    var entity = ctx.PmStatusHistories.Where(m => m.InquiryNumber == item.InquiryNumber && m.CompanyCode == item.CompanyCode && m.BranchCode == item.BranchCode && m.SequenceNo == item.SequenceNo).FirstOrDefault();
        //                    if (entity == null)
        //                    {
        //                        entity = new PmStatusHistory();
        //                        entity.BranchCode = item.BranchCode;
        //                        entity.CompanyCode = item.CompanyCode;
        //                        entity.InquiryNumber = item.InquiryNumber;
        //                        entity.SequenceNo = item.SequenceNo;
        //                        isNew = true;
        //                    }
        //                    entity.LastProgress = item.LastProgress != null ? item.LastProgress : string.Empty;
        //                    entity.UpdateUser = item.UpdateUser != null ? item.UpdateUser : string.Empty;
        //                    entity.UpdateDate = item.UpdateDate.Value.AddHours(addLocalTime);

        //                    if (isNew)
        //                        ctx.PmStatusHistories.Add(entity);

        //                    UpdateSchedule(pid);
        //                }
        //            }
        //            catch
        //            {

        //            }
        //            break;
        //            default:
        //                break;
        //        }
        //    }
        //}

        //private void UpdateSchedule(string pid)
        //{
        //    var schedule = ctxDealer.GnMstScheduleDatas.Find(pid);
        //    schedule.Status = "P";
        //    schedule.UpdatedDate = DateTime.Now;
        //    try
        //    {
        //        ctx.SaveChanges();
        //        ctxDealer.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
    }
}
