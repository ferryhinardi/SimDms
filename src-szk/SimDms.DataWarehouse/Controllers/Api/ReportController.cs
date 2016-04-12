using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.DataWarehouse.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Web.Script.Serialization;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class ReportController : BaseController
    {
        public JsonResult Cs3DaysCall()
        {
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string companyCode = Request["CompanyCode"];
            var data = ctx.Database.SqlQuery<Cs3DaysCallModel>("exec uspfn_CsInqTDayCall @CompanyCode=@p0, @DateFrom=@p1, @DateTo=@p2", companyCode, dateFrom, dateTo).AsQueryable();
            return Json(data);
        }

        public JsonResult StnkExtension()
        {
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string companyCode = Request["CompanyCode"];
            var data = ctx.Database.SqlQuery<StnkExtensionModel>("exec uspfn_CsInqStnkExt @CompanyCode=@p0, @DateFrom=@p1, @DateTo=@p2", companyCode, dateFrom, dateTo).AsQueryable();
            return Json(data);
        }

        public JsonResult BpkbReminder()
        {
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string companyCode = Request["CompanyCode"];
            var data = ctx.Database.SqlQuery<BpkbReminderModel>("exec uspfn_CsInqBpkbReminder @CompanyCode=@p0, @DateFrom=@p1, @DateTo=@p2", companyCode, dateFrom, dateTo).AsQueryable();
            return Json(data);
        }

        public JsonResult CsFeedback()
        {
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string companyCode = Request["CompanyCode"];
            var data = ctx.Database.SqlQuery<FeedbackModel>("exec uspfn_CsInqCustFeedback @CompanyCode=@p0, @DateFrom=@p1, @DateTo=@p2", companyCode, dateFrom, dateTo).AsQueryable();
            return Json(data);
        }

        public JsonResult CsBirthday()
        {
            string dateFrom = Request["MonthFrom"];
            string dateTo = Request["MonthTo"];
            string companyCode = Request["CompanyCode"];
            var data = ctx.Database.SqlQuery<CustomerBirthdayModel>("exec uspfn_CsInqCustBirthday @CompanyCode=@p0, @MonthFrom=@p1, @MonthTo=@p2", companyCode, dateFrom, dateTo).AsQueryable();
            return Json(data);
        }

        public JsonResult TDayCalls()
        {
            string CompanyCode = Request["CompanyCode"] ?? "";
            string BranchCode = Request["BranchCode"] ?? "%";

            var oustanding = Request["OutStanding"];
            var qry = ctx.CsLkuTDayCallViews.Where(p => p.CompanyCode == CompanyCode);

            if (string.IsNullOrEmpty(BranchCode) == false)
            {
                qry = qry.Where(x => x.BranchCode == BranchCode);
            }

            if (oustanding == "Y")
            {
                var setting = ctx.CsSettings.Find(CompanyCode, "REM3DAYSCALL") ?? new CsSetting();
                var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
                var date1 = DateTime.Now.AddMonths(-month);
                var date2 = new DateTime(date1.Year, date1.Month, 1);
                qry = qry.Where(p => p.OutStanding == "Y" && p.DODate >= date2);
            }
            else
            {
                var setting = ctx.CsSettings.Find(CompanyCode, "REM3DAYSCALL") ?? new CsSetting();
                var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
                var date1 = DateTime.Now.AddMonths(-month);
                var date2 = new DateTime(date1.Year, date1.Month, 1);
                qry = qry.Where(p => p.OutStanding == "N" && p.DODate >= date2);
            }
            return Json(qry);
        }

        public JsonResult CustomerBirthDays()
        {
            string CompanyCode = Request["CompanyCode"] ?? "";
            string BranchCode = Request["BranchCode"] ?? "%";

            string customerName = Request["CustomerName"] ?? "";
            string customerCode = Request["CustomerCode"] ?? "";
            string branchCode = Request["BranchCode"] ?? "";
            string outStanding = Request["OutStanding"] ?? "";

            var qry = ctx.CsLkuBirthdayViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode.Contains(branchCode));
            if (!string.IsNullOrWhiteSpace(outStanding)) { qry = qry.Where(p => p.OutStanding == outStanding); };
            if (!string.IsNullOrWhiteSpace(customerCode)) { qry = qry.Where(p => p.CustomerCode.Contains(customerCode)); };
            if (!string.IsNullOrWhiteSpace(customerName)) { qry = qry.Where(p => p.CustomerName.Contains(customerName)); };
            //if (!string.IsNullOrWhiteSpace(branchCode)) { qry = qry.Where(p => p.BranchCode == branchCode); };

            return Json(qry);
        }

        public JsonResult StnkExt()
        {
            string CompanyCode = Request["CompanyCode"] ?? "";
            string BranchCode = Request["BranchCode"] ?? "%";

            var oustanding = Request["OutStanding"];
            var qry = ctx.CsLkuStnkExtViews.Where(p => p.CompanyCode == CompanyCode);

            if (string.IsNullOrEmpty(BranchCode) == false)
            {
                qry = qry.Where(x => x.BranchCode == BranchCode);
            }

            if (oustanding == "Y")
            {
                var setting = ctx.CsSettings.Find(CompanyCode, "REMSTNKEXT") ?? new CsSetting();
                var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
                var date1 = DateTime.Now.AddMonths(-month);
                var date2 = new DateTime(date1.Year, date1.Month, 1);
                qry = qry.Where(p => p.OutStanding == "Y" && p.StnkExpiredDate >= date2);
            }
            else
            {
                var setting = ctx.CsSettings.Find(CompanyCode, "REMSTNKEXT") ?? new CsSetting();
                var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
                var date1 = DateTime.Now.AddMonths(-month);
                var date2 = new DateTime(date1.Year, date1.Month, 1);
                qry = qry.Where(p => p.OutStanding == "N" && p.StnkExpiredDate >= date2);
            }
            return Json(qry);
        }

        public JsonResult Bpkbs()
        {
            string CompanyCode = Request["CompanyCode"] ?? "";
            string BranchCode = Request["BranchCode"] ?? "%";

            var oustanding = Request["OutStanding"];
            var qry = ctx.CsLkuBpkbViews.Where(p => p.CompanyCode == CompanyCode);

            if (string.IsNullOrEmpty(BranchCode) == false)
            {
                qry = qry.Where(x => x.BranchCode == BranchCode);
            }

            if (oustanding == "Y")
            {
                var setting = ctx.CsSettings.Find(CompanyCode, "REMBPKB") ?? new CsSetting();
                var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
                var date1 = DateTime.Now.AddMonths(-month);
                var date2 = new DateTime(date1.Year, date1.Month, 1);
                qry = qry.Where(p => p.OutStanding == "Y" && p.BpkbDate >= date2);
            }
            else
            {
                var setting = ctx.CsSettings.Find(CompanyCode, "REMBPKB") ?? new CsSetting();
                var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
                var date1 = DateTime.Now.AddMonths(-month);
                var date2 = new DateTime(date1.Year, date1.Month, 1);
                qry = qry.Where(p => p.OutStanding == "N" && p.BpkbDate >= date2);
            }
            return Json(qry);
        }

        public JsonResult SfmTurnOver()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];

            var message = "";
            try
            {
                //var data = ctx.Database.SqlQuery<TurnOver>("exec uspfn_InqTurnOver @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2, @ByDate=@p3", groupArea, companyCode, branchCode, byDate).ToList();
                var data = ctx.Database.SqlQuery<TurnOver>("exec uspfn_InqTurnOver_New @GroupArea=@p0, @Company=@p1, @BranchCode=@p2, @ByDate=@p3", groupArea, companyCode, branchCode, byDate).ToList();
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                package = GESfmTurnOver(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult SfmReview()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];

            var message = "";
            try
            {
                var data = ctx.Database.SqlQuery<ReviewSfm>("exec uspfn_InqReviewSFM @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2, @ByDate=@p3", groupArea, companyCode, branchCode, byDate).ToList();
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                package = GESfmReview(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult SfmReviewNew()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = ParamDealerCode;//Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];
            var groupNoNew = ParamGroupNoNew;

            var message = "";
            try
            {
                var data = ctx.Database.SqlQuery<ReviewSfm>("exec uspfn_InqReviewSFMNew @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2, @ByDate=@p3, @GroupNoNew=@p4",
                    groupArea, companyCode, branchCode, byDate, groupNoNew).ToList();
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                package = GESfmReview(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult SfmScShBmTraining()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];

            var message = "";
            try
            {
                var data = ctx.Database.SqlQuery<ScShBmTraining>("exec uspfn_InqSCSHBMTraining @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2, @ByDate=@p3", groupArea, companyCode, branchCode, byDate).ToList();
                //return Json(data);
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                package = GEScShBmTraining(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult SfmScShBmTrainingNew()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = ParamDealerCode; //Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];
            var groupNoNew = ParamGroupNoNew;

            var message = "";
            try
            {
                var data = ctx.Database.SqlQuery<ScShBmTraining>("exec uspfn_InqSCSHBMTrainingNew @p0, @p1, @p2, @p3, @p4", 
                    groupArea, companyCode, branchCode, byDate, groupNoNew).ToList();
                //return Json(data);
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                package = GEScShBmTraining(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult SfmSalesmanTrainingNew()
        {
            var message = "";
            string groupArea = Request["GroupArea"];
            string companyCode = ParamDealerCode; //Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];
            var groupNoNew = ParamGroupNoNew;

            try
            {
                var data = ctx.Database.SqlQuery<SalesmanTraining>("exec uspfn_InqSalesmanTrainingNew @p0, @p1, @p2, @p3, @p4", 
                    groupArea, companyCode, branchCode, byDate, groupNoNew).ToList();
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                package = GenerateExcelSalesmanTraining(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult SfmSalesmanTraining()
        {
            var message = "";
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];

            try
            {
                var data = ctx.Database.SqlQuery<SalesmanTraining>("exec uspfn_InqSalesmanTraining @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2, @ByDate=@p3", groupArea, companyCode, branchCode, byDate).ToList();
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                package = GenerateExcelSalesmanTraining(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        
        public JsonResult SfmOutstandingTraining()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];

            var message = "";
            try
            {
                var data = ctx.Database.SqlQuery<OutstandingTraining>("exec uspfn_InqOutstandingTraining @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2, @ByDate=@p3", groupArea, companyCode, branchCode, byDate).ToList();
                //return Json(data);
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                package = GEOutstandingTraining(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult SfmOutstandingTrainingNew()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = ParamDealerCode; //Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];
            var groupNoNew = ParamGroupNoNew;

            var message = "";
            try
            {
                var data = ctx.Database.SqlQuery<OutstandingTraining>("exec uspfn_InqOutstandingTrainingNew @p0, @p1, @p2, @p3, @p4", 
                    groupArea, companyCode, branchCode, byDate, groupNoNew).ToList();
                //return Json(data);
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                package = GEOutstandingTraining(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult SfmSalesTeamHeader()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var data = ctx.Database.SqlQuery<SalesTeamHeader>("exec uspfn_InqSalesTeamHeader @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2", groupArea, companyCode, branchCode).AsQueryable();
            return Json(data);
        }

        public JsonResult SfmSalesTeam()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var data = ctx.Database.SqlQuery<SalesTeamHeader>("exec uspfn_InqSalesTeam @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2", groupArea, companyCode, branchCode).AsQueryable();
            return Json(data);
        }

        public JsonResult SfmMutation()
        {
            string area = Request["GroupArea"];// ?? "--";
            string comp = Request["CompanyCode"];// ?? "--";
            string dept = "SALES";
            string posi = Request["Position"] ?? "";
            string date = Request["MutaDate"];

            var message = "";
            try
            {
                //var data = ctx.Database.SqlQuery<HrInqMutation>("exec uspfn_HrInqMutation @GroupNo=@p0, @CompanyCode=@p1, @DeptCode=@p2, @Position=@p3, @MutaDate=@p4", area, comp, dept, posi, date).ToList();
                var data = ctx.Database.SqlQuery<HrInqMutation>("exec uspfn_HrInqMutation_New @GroupNo=@p0, @Company=@p1, @DeptCode=@p2, @Position=@p3, @MutaDate=@p4", area, comp, dept, posi, date).ToList();
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                package = GESfmMutation(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult SfmTrend()
        {
            string area = Request["GroupArea"];// ?? "--";
            string comp = Request["CompanyCode"];// ?? "--";
            string dept = "SALES";
            string posi = Request["Position"] ?? "";
            string year = Request["Year"];

            var message = "";
            try
            {
                //var data = ctx.Database.SqlQuery<HrInqTrend>("exec uspfn_HrInqTrend @GroupNo=@p0, @CompanyCode=@p1, @DeptCode=@p2, @Position=@p3, @Year=@p4", area, comp, dept, posi, (year == "" ? DateTime.Now.Year.ToString() : year)).ToList();
                var data = ctx.Database.SqlQuery<HrInqTrend>("exec uspfn_HrInqTrend_New @GroupNo=@p0, @Company=@p1, @DeptCode=@p2, @Position=@p3, @Year=@p4", area, comp, dept, posi, (year == "" ? DateTime.Now.Year.ToString() : year)).ToList();
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                package = GESfmTrend(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult InqEmployees(string type)
        {
            var message = "";
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string department = Request["Department"];
            string post = Request["Position"];
            string status = Request["Status"];
            try
            {
                var data = ctx.Database.SqlQuery<HrInqEmployee>("exec uspfn_HrInqEmployee @GroupNo=@p0, @CompanyCode=@p1, @DeptCode=@p2, @PosCode=@p3, @Status=@p4", groupArea, companyCode, department, post, status).ToList();
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                if (type == "mutation")
                    package = GenerateExcelMutation(package, data);
                else
                    package = GenerateExcel(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult InqEmployeesNew(string type)
        {
            var groupNoWithCompany = !string.IsNullOrEmpty(Request["CompanyCode"]) ? Request["CompanyCode"].Split('|') : ("" + "|" + "").Split('|');

            var message = "";
            string groupNoNew = groupNoWithCompany[0];
            string groupArea = Request["GroupArea"];
            string companyCode = groupNoWithCompany[1]; //Request["CompanyCode"];
            string department = Request["Department"];
            string post = Request["Position"];
            string status = Request["Status"];
            try
            {
                var data = ctx.Database.SqlQuery<HrInqEmployee>("exec uspfn_HrInqEmployeeNew @GroupNo=@p0, @CompanyCode=@p1, @DeptCode=@p2, @PosCode=@p3, @Status=@p4, @GroupNoNew=@p5", groupArea, companyCode, department, post, status, groupNoNew).ToList();
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                if (type == "mutation")
                    package = GenerateExcelMutation(package, data);
                else
                    package = GenerateExcel(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult UploadCSIScore()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonth = Request["PeriodMonth"];
            var data = Request["listScore"];

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<SvCustomerSatisfactionScore> listscore = ser.Deserialize<List<SvCustomerSatisfactionScore>>(data);
            List<SvCustomerSatisfactionScore> listscoreerror = new List<SvCustomerSatisfactionScore>();
            
            foreach (var item in listscore)
            {
                if (ctx.svMstDealerAndOutletServiceMappings.Find(item.ServiceCode) == null)
                {
                    listscoreerror.Add(item);
                    continue;
                }
                item.PeriodYear = Convert.ToInt32(PeriodYear);
                item.PeriodMonth = Convert.ToInt32(PeriodMonth);
                item.isStatus = true;

                var ent2 = new svCustomerSatisfactionScoreLog();
                ent2.ServiceCode = item.ServiceCode;
                ent2.PeriodYear = item.PeriodYear;
                ent2.PeriodMonth = item.PeriodMonth;
                ent2.SeqNo = ctx.svCustomerSatisfactionScoreLogs.Count(x => x.ServiceCode == item.ServiceCode && x.PeriodYear == item.PeriodYear && x.PeriodMonth == item.PeriodMonth) + 1;
                ent2.CompanyCode = item.CompanyCode;
                ent2.BranchCode = item.BranchCode;
                ent2.ServiceInitiation = item.ServiceInitiation;
                ent2.ServiceAdvisor = item.ServiceAdvisor;
                ent2.ServiceFaciltiy = item.ServiceFaciltiy;
                ent2.VehiclePickup = item.VehiclePickup;
                ent2.ServiceQuality = item.ServiceQuality;
                ent2.Score = item.Score;
                ent2.LastUpdateBy = CurrentUser.Username;
                ent2.LastUpdateDate = DateTime.Now;

                var ent = ctx.SvCustomerSatisfactionScores.Find(item.ServiceCode, item.PeriodYear, item.PeriodMonth);
                if (ent != null)
                {
                    ent.ServiceInitiation = item.ServiceInitiation;
                    ent.ServiceAdvisor = item.ServiceAdvisor;
                    ent.ServiceFaciltiy = item.ServiceFaciltiy;
                    ent.VehiclePickup = item.VehiclePickup;
                    ent.ServiceQuality = item.ServiceQuality;
                    ent.Score = item.Score;
                    ent.LastUpdateBy = CurrentUser.Username;
                    ent.LastUpdateDate = DateTime.Now;
                }
                else
                {
                    item.CreatedBy = CurrentUser.Username;
                    item.CreatedDate = DateTime.Now;
                    ctx.SvCustomerSatisfactionScores.Add(item);
                }

                ctx.svCustomerSatisfactionScoreLogs.Add(ent2);
            }

            int n = 0;

            try
            {
                n = ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Arithmetic overflow error converting numeric to data type numeric."))
                {
                    return Json(new { success = false, message = "Tidak boleh lebih dari 3 digit pada data score" });
                }
                return Json(new { success = false, message = ex.InnerException.Message });
            }

            if (listscoreerror.Count() == 0 && n > 0)
            {
                var package = new ExcelPackage();
 
                
                if (listscore == null) throw new Exception("Harap hubungi IT Support");
                package = GenerateExcelCSIScore(package, listscore);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { success = true, value = guid });
            }
            return Json(new { success = false, errorlist = listscoreerror });
        }

        public JsonResult GetUnitRevenue(string PeriodYear)
        {
            var message = "";
            decimal PeriodYearParam;
            if (!Decimal.TryParse(PeriodYear, out PeriodYearParam))
                PeriodYearParam = 0;

            try
            {
                var data = ctx.Database.SqlQuery<svMstUnitRevenueTarget>(string.Format("EXEC uspfn_svGetUnitRevenueTarget {0}", PeriodYearParam)).ToList();
                var dataUnit = data.Where(x => x.TargetFlag == "U").ToList();
                var dataRevenue = data.Where(x => x.TargetFlag == "R").ToList();
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");

                package = GenerateExcelUnitRevenueTarget(package, dataUnit, dataRevenue);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult VORReport()

        {
            var message = "";
            string groupArea = Request["Area"];
            string companyCode = Request["Dealer"];
            string branchCode = Request["Outlet"];
            string ReffProblem = Request["ProblemService"];
            string ReffModel = Request["Model"];
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string WIP = Request["WIP"];
            int status = (Request["Open"] != null ? 1 : 0) + (Request["Closed"] != null ? 2 : 0);

            string DealerName = "";
            string dealerType = Request["DealerType"] ?? "";
            string linkedModule = Request["LinkedModule"] ?? "";

            if (string.IsNullOrWhiteSpace(dealerType))
            {
                dealerType = DealerType;
            }
            try
            {
                var Dealers = ctx.Database.SqlQuery<DealerList>("exec uspfn_DealerList @DealerType=@p0, @LinkedModule=@p1, @GroupArea=@p2", dealerType, linkedModule, groupArea).Where(p => p.CompanyCode == companyCode).Select(p => new { text = p.CompanyName.ToUpper() }).ToList();
                if (Dealers.Count() == 0)
                    DealerName = "";
                else
                    DealerName = Dealers[0].text;
                var data = ctx.Database.SqlQuery<VORPart>("exec usprpt_SvRpTrn021ver2 @GroupNo=@p0, @CompanyCode=@p1, @BranchCode=@p2, @RefferenceCodeDelay=@p3, @RefferenceCodeModel=@p4, @StartDate=@p5, @EndDate=@p6, @WIP=@p7, @Status=@p8", groupArea, companyCode, branchCode, ReffProblem, ReffModel, dateFrom, dateTo, WIP, status).ToList();
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");

                package = GenerateExcelVOR(package, DealerName, dateFrom, dateTo, data);
                
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult VORSummaryReport()
        {
            var message = "";
            string groupArea = Request["Area"];
            string companyCode = Request["Dealer"];
            string branchCode = Request["Outlet"];
            string ReffProblem = Request["ProblemService"];
            string ReffModel = Request["Model"];
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string WIP = Request["WIP"];
            int status = (Request["Open"] != null ? 1 : 0) + (Request["Closed"] != null ? 2 : 0);

            string DealerName = "";
            string dealerType = Request["DealerType"] ?? "";
            string linkedModule = Request["LinkedModule"] ?? "";

            if (string.IsNullOrWhiteSpace(dealerType))
            {
                dealerType = DealerType;
            }
            try
            {
                var Dealers = ctx.Database.SqlQuery<DealerList>("exec uspfn_DealerList @DealerType=@p0, @LinkedModule=@p1, @GroupArea=@p2", dealerType, linkedModule, groupArea).Where(p => p.CompanyCode == companyCode).Select(p => new { text = p.CompanyName.ToUpper() }).ToList();
                if (Dealers.Count() == 0)
                    DealerName = "";
                else
                    DealerName = Dealers[0].text;
                var data = ctx.Database.SqlQuery<VORSummary>("exec usprpt_SvRpTrn021ver2 @GroupNo=@p0, @CompanyCode=@p1, @BranchCode=@p2, @RefferenceCodeDelay=@p3, @RefferenceCodeModel=@p4, @StartDate=@p5, @EndDate=@p6, @WIP=@p7, @Status=@p8", groupArea, companyCode, branchCode, ReffProblem, ReffModel, dateFrom, dateTo, WIP, status).ToList();
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");

                package = GenerateExcelVORSummary(package, DealerName, dateFrom, dateTo, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult VORConsistencyReport()
        {
            var message = "";
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            string tempYear = Request["Year"];
            string tempMonth = Request["Month"];

            int year, month;
            if (!Int32.TryParse(tempYear, out year))
                year = 0;
            if (!Int32.TryParse(tempMonth, out month))
                month = 0;

            var Model = new
            {
                GroupAreasParam = groupArea,
                CompanyCodeParam = companyCode,
                BranchCodeParam = branchCode,
                Year = year,
                Month = month
            };
            try
            {
                var data = ctx.Database.SqlQuery<VORConsistencyReportV2>(string.Format("exec uspfn_svGetVORReportConsistencyV2 '{0}', '{1}', '{2}', {3}, {4}", groupArea, companyCode, branchCode, year, month)).ToList(); 
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");

                package = GenerateExcelVORConsistencyV2(package, data, Model);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult SfmPersInfo()
        {
            var message = "";
            string dept = "SALES";
            string area = Request["GroupArea"];
            string comp = Request["CompanyCode"] ?? "--";
            string post = Request["Position"];
            string status = Request["Status"];
            string branch = Request["Branch"];

            try
            {
                var data = ctx.Database.SqlQuery<HrInqPersInfoDetail>("exec uspfn_HrInqPersInfoDetail @GroupNo=@p0, @CompanyCode=@p1, @DeptCode=@p2, @PosCode=@p3, @Status=@p4, @BranchCode=@p5", area, comp, dept, post, status, branch).ToList();
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                package = GenerateExcelPersonalInfoSF(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult SfmPersInfoNew()
        {
            var message = "";
            string dept = "SALES";
            string area = Request["GroupArea"];
            string comp = ParamDealerCode; //Request["CompanyCode"] ?? "--";
            string post = Request["Position"];
            string status = Request["Status"];
            string branch = Request["Branch"];
            string groupNoNew = ParamGroupNoNew;

            try
            {
                var data = ctx.Database.SqlQuery<HrInqPersInfoDetail>("exec uspfn_HrInqPersInfoDetailNew @GroupNo=@p0, @CompanyCode=@p1, @DeptCode=@p2, @PosCode=@p3, @Status=@p4, @BranchCode=@p5, @GroupNoNew=@p6", 
                    area, comp, dept, post, status, branch, ParamGroupNoNew).ToList();
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                package = GenerateExcelPersonalInfoSF(package, data);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult CsBirthdayMonitorReport()
        {
            var message = "";
            var GroupArea = Request["GroupArea"];
            var CompanyCode = Request["CompanyCode"];
            var BranchCode = Request["BranchCode"];
            var PeriodYear = Request["PeriodYear"];
            var ParMonth1 = Request["ParMonth1"];
            var ParMonth2 = Request["ParMonth2"];
            var ParStatus = Request["ParStatus"];

            try
            {
                var dataMonthly = ctx.Database.SqlQuery<CustomerBirthdayMonitoring>(string.Format("exec uspfn_GetMonitoringCustBirthday '{0}', '{1}', '{2}', {3}, {4}, {5}, {6}, {7}", GroupArea, CompanyCode, BranchCode, PeriodYear, ParMonth1, ParMonth2, ParStatus, 0)).ToList();
                var dataWeekly = ctx.Database.SqlQuery<CustomerBirthdayMonitoring>(string.Format("exec uspfn_GetMonitoringCustBirthday '{0}', '{1}', '{2}', {3}, {4}, {5}, {6}, {7}", GroupArea, CompanyCode, BranchCode, PeriodYear, ParMonth1, ParMonth2, ParStatus, 1)).ToList();
                var package = new ExcelPackage();
                if (dataMonthly == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                if (dataWeekly == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");

                package = GenerateExcelCsBirthMonitoring(package, dataMonthly, dataWeekly);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult GenerateDRH()
        {
            var message = "";
            var GroupArea = Request["GroupArea"];
            var CompanyCode = Request["CompanyCode"];
            var BranchCode = Request["BranchCode"];
            var DateVisitFrom = Request["DateVisitFrom"];
            var DateVisitTo = Request["DateVisitTo"];
            var PMVisit = Request["PMVisit"];
            var Activity = Request["Activity"];
            var DateFrom = Request["DateFrom"];
            var DateTo = Request["DateTo"];

            var Model = new
            {
                GroupAreasParam = GroupArea,
                CompanyCodeParam = CompanyCode,
                BranchCodeParam = BranchCode
            };

            try
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_svRetensiHarianReport";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@GroupNo", GroupArea);
                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                cmd.Parameters.AddWithValue("@StartVisitedDate", DateVisitFrom);
                cmd.Parameters.AddWithValue("@EndVisitedDate", DateVisitTo);
                cmd.Parameters.AddWithValue("@PMNext", PMVisit);
                cmd.Parameters.AddWithValue("@Activity", Activity);
                cmd.Parameters.AddWithValue("@StartPeriode", DateFrom);
                cmd.Parameters.AddWithValue("@EndPeriode", DateTo);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable data = new DataTable();
                da.Fill(data);
                //var data = ctx.Database.SqlQuery<DataRetentionReport>(string.Format("exec uspfn_svRetensiHarianReport '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}'", GroupArea, CompanyCode, BranchCode, DateVisitFrom, DateVisitTo, PMVisit, Activity, DateFrom, DateTo)).ToList();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                var package = new ExcelPackage();
                package = GenerateExcelDRH(package, data, Model);
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult HistJobDelayVOR()
        {
            var message = "";
            var GroupArea = Request["GroupArea"];
            var CompanyCode = Request["CompanyCode"];
            var BranchCode = Request["BranchCode"];
            var Model = new
            {
                GroupAreasParam = GroupArea,
                CompanyCodeParam = CompanyCode,
                BranchCodeParam = BranchCode
            };
            try
            {
                var data = ctx.Database.SqlQuery<HistJobDelayVOR>(string.Format("exec usprpt_SvRpTrn021ver2Hist '{0}', '{1}', '{2}'", GroupArea, CompanyCode, BranchCode)).ToList();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                var package = new ExcelPackage();
                package = GenerateExcelHistJobDelayVOR(package, data, Model);
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);
                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        public JsonResult MappingSrvGnReport()
        {
            var message = "";
            var GroupNo = Request["GroupNo"];
            var CompanyCode = Request["CompanyCode"];
            var BranchCode = Request["BranchCode"];
            var Model = new
            {
                GroupNoParam = GroupNo,
                CompanyCodeParam = CompanyCode,
                BranchCodeParam = BranchCode
            };
            try
            {
                var data = ctx.Database.SqlQuery<MappingSrvGn>(string.Format("exec uspfn_GetMappingSrvGn '{0}', '{1}', '{2}'", GroupNo, CompanyCode, BranchCode)).ToList();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                var package = new ExcelPackage();
                package = GenerateExcelMappingSrvGn(package, data, Model);
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);
                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        #region Generate Excels Area

        private static ExcelPackage GenerateExcel(ExcelPackage package, List<HrInqEmployee> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 6,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 4;
            double
                wDealerCode = GetTrueColWidth(10.86),
                wBranchCode = GetTrueColWidth(10.86),
                wDealer = GetTrueColWidth(30.00),
                wBranch = GetTrueColWidth(30.00),
                wDepartment = GetTrueColWidth(12.43),
                wLastPosition = GetTrueColWidth(12.43),
                wNIK = GetTrueColWidth(11.43),
                wName = GetTrueColWidth(30.00),
                wLeaderName = GetTrueColWidth(30.00),
                wSubs = GetTrueColWidth(30.00),
                wStatus = GetTrueColWidth(14.00),
                wSubordinates = GetTrueColWidth(14.00),
                wMutationTimes = GetTrueColWidth(14.00),
                wAchiveTime = GetTrueColWidth(14.00),
                wLastBranch = GetTrueColWidth(10.86),
                wTeamLeader = GetTrueColWidth(14.00),
                wJoinDate = GetTrueColWidth(14.00),
                wResignDate = GetTrueColWidth(14.00),
                wResignDescription = GetTrueColWidth(20.00),
                wMaritalStatus = GetTrueColWidth(14.00),
                wReligion = GetTrueColWidth(14.00),
                wGender = GetTrueColWidth(14.00),
                wEducation = GetTrueColWidth(18.00),
                wBirthPlace = GetTrueColWidth(18.00),
                wBirthDate = GetTrueColWidth(14.00),
                wAddress = GetTrueColWidth(30.00),
                wProvince = GetTrueColWidth(19.00),
                wDistrict = GetTrueColWidth(14.00),
                wVillage = GetTrueColWidth(18.00),
                wIdentityNo = GetTrueColWidth(18.00),
                wNPWPNo = GetTrueColWidth(18.00),
                wEmail = GetTrueColWidth(30.00),
                wDrivingLicense = GetTrueColWidth(16.00);

            double cWidth = GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Personal List Data";
            sheet.Cells[x].Style.Font.Size = 20;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = wDealerCode, Value = "Dealer Code" },
                new ExcelCellItem { Column = 2, Width = wBranchCode, Value = "Branch Code" },
                new ExcelCellItem { Column = 3, Width = wDealer, Value = "Dealer" },
                new ExcelCellItem { Column = 4, Width = wBranch, Value = "Branch" },
                new ExcelCellItem { Column = 5, Width = wDepartment, Value = "Department" },
                new ExcelCellItem { Column = 6, Width = wLastPosition, Value = "Last Position" },
                new ExcelCellItem { Column = 7, Width = wNIK, Value = "NIK" },
                new ExcelCellItem { Column = 8, Width = wName, Value = "Name" },
                new ExcelCellItem { Column = 9, Width = wLeaderName, Value = "Leader Name" },
                new ExcelCellItem { Column = 10, Width = wSubs, Value = "Subs" },
                new ExcelCellItem { Column = 11, Width = wStatus, Value = "Status" },
                new ExcelCellItem { Column = 12, Width = wSubordinates, Value = "Subordinates" },
                new ExcelCellItem { Column = 13, Width = wMutationTimes, Value = "Mutation Times" },
                new ExcelCellItem { Column = 14, Width = wAchiveTime, Value = "Achive Times" },
                new ExcelCellItem { Column = 15, Width = wLastBranch, Value = "Last Branch" },
                new ExcelCellItem { Column = 16, Width = wTeamLeader, Value = "Team Leader" },
                new ExcelCellItem { Column = 17, Width = wJoinDate, Value = "Join Date" },
                new ExcelCellItem { Column = 18, Width = wResignDate, Value = "Resign Date" },
                new ExcelCellItem { Column = 19, Width = wResignDescription, Value = "Resign Description" },
                new ExcelCellItem { Column = 20, Width = wMaritalStatus, Value = "Marital Status" },
                new ExcelCellItem { Column = 21, Width = wReligion, Value = "Religion" },
                new ExcelCellItem { Column = 22, Width = wGender, Value = "Gender" },
                new ExcelCellItem { Column = 23, Width = wEducation, Value = "Education" },
                new ExcelCellItem { Column = 24, Width = wBirthPlace, Value = "Birth Place" },
                new ExcelCellItem { Column = 25, Width = wBirthDate, Value = "Birth Date" },
                new ExcelCellItem { Column = 26, Width = wAddress, Value = "Address" },
                new ExcelCellItem { Column = 27, Width = wProvince, Value = "Province" },
                new ExcelCellItem { Column = 28, Width = wDistrict, Value = "District" },
                new ExcelCellItem { Column = 29, Width = wDistrict, Value = "Sub District" },
                new ExcelCellItem { Column = 30, Width = cWidth, Value = "Village" },
                new ExcelCellItem { Column = 31, Width = cWidth, Value = "Zip Code" },
                new ExcelCellItem { Column = 32, Width = wIdentityNo, Value = "Identity Number" },
                new ExcelCellItem { Column = 33, Width = wNPWPNo, Value = "NPWP No" },
                new ExcelCellItem { Column = 34, Width = cWidth, Value = "NPWP Date" },
                new ExcelCellItem { Column = 35, Width = wEmail, Value = "Email" },
                new ExcelCellItem { Column = 36, Width = cWidth, Value = "Telephone 1" },
                new ExcelCellItem { Column = 37, Width = cWidth, Value = "Telephone 2" },
                new ExcelCellItem { Column = 38, Width = cWidth, Value = "Handphone 1" },
                new ExcelCellItem { Column = 39, Width = cWidth, Value = "Handphone 2" },
                new ExcelCellItem { Column = 40, Width = cWidth, Value = "Handphone 3" },
                new ExcelCellItem { Column = 41, Width = cWidth, Value = "Handphone 4" },
                new ExcelCellItem { Column = 42, Width = wDrivingLicense, Value = "Driving License 1" },
                new ExcelCellItem { Column = 43, Width = wDrivingLicense, Value = "Driving License 2" },
                new ExcelCellItem { Column = 44, Width = cWidth, Value = "Height" },
                new ExcelCellItem { Column = 45, Width = cWidth, Value = "Weight" },
                new ExcelCellItem { Column = 46, Width = cWidth, Value = "Blood Type" },
                new ExcelCellItem { Column = 47, Width = cWidth, Value = "Uniforms Size" },
                new ExcelCellItem { Column = 48, Width = cWidth, Value = "Uniform Size Alt" },
                new ExcelCellItem { Column = 49, Width = cWidth, Value = "Shoes Size" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            #endregion

            //DATA
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].CompanyCode },
                    new ExcelCellItem { Column = 2, Value = data[i].LastBranch },
                    new ExcelCellItem { Column = 3, Value = data[i].DealerName },
                    new ExcelCellItem { Column = 4, Value = data[i].LastBranchName },
                    new ExcelCellItem { Column = 5, Value = data[i].Department },
                    new ExcelCellItem { Column = 6, Value = data[i].Position },
                    new ExcelCellItem { Column = 7, Value = data[i].EmployeeID },
                    new ExcelCellItem { Column = 8, Value = data[i].EmployeeName },
                    new ExcelCellItem { Column = 9, Value = data[i].TeamLeaderName },
                    new ExcelCellItem { Column = 10, Value = data[i].SubDistrict },
                    new ExcelCellItem { Column = 11, Value = data[i].Status },
                    new ExcelCellItem { Column = 12, Value = data[i].SubOrdinates },
                    new ExcelCellItem { Column = 13, Value = data[i].MutationTimes },
                    new ExcelCellItem { Column = 14, Value = data[i].AchieveTimes },
                    new ExcelCellItem { Column = 15, Value = data[i].LastBranch },
                    new ExcelCellItem { Column = 16, Value = data[i].TeamLeader },
                    new ExcelCellItem { Column = 17, Value = data[i].JoinDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 18, Value = data[i].ResignDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 19, Value = data[i].ResignDescription },
                    new ExcelCellItem { Column = 20, Value = data[i].MaritalStatus },
                    new ExcelCellItem { Column = 21, Value = data[i].Religion },
                    new ExcelCellItem { Column = 22, Value = data[i].Gender },
                    new ExcelCellItem { Column = 23, Value = data[i].Education },
                    new ExcelCellItem { Column = 24, Value = data[i].BirthPlace },
                    new ExcelCellItem { Column = 25, Value = data[i].BirthDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 26, Value = data[i].Address },
                    new ExcelCellItem { Column = 27, Value = data[i].Province },
                    new ExcelCellItem { Column = 28, Value = data[i].District },
                    new ExcelCellItem { Column = 29, Value = data[i].SubDistrict },
                    new ExcelCellItem { Column = 30, Value = data[i].Village },
                    new ExcelCellItem { Column = 31, Value = data[i].ZipCode },
                    new ExcelCellItem { Column = 32, Value = data[i].IdentityNo },
                    new ExcelCellItem { Column = 33, Value = data[i].NPWPNo },
                    new ExcelCellItem { Column = 34, Value = data[i].NPWPDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 35, Value = data[i].Email },
                    new ExcelCellItem { Column = 36, Value = data[i].Telephone1 },
                    new ExcelCellItem { Column = 37, Value = data[i].Telephone2 },
                    new ExcelCellItem { Column = 38, Value = data[i].Handphone1 },
                    new ExcelCellItem { Column = 39, Value = data[i].Handphone2 },
                    new ExcelCellItem { Column = 40, Value = data[i].Handphone3 },
                    new ExcelCellItem { Column = 41, Value = data[i].Handphone4 },
                    new ExcelCellItem { Column = 42, Value = data[i].DrivingLicense1 },
                    new ExcelCellItem { Column = 43, Value = data[i].DrivingLicense2 },
                    new ExcelCellItem { Column = 44, Value = data[i].Height },
                    new ExcelCellItem { Column = 45, Value = data[i].Weight },
                    new ExcelCellItem { Column = 46, Value = data[i].BloodCode },
                    new ExcelCellItem { Column = 47, Value = data[i].UniformSize },
                    new ExcelCellItem { Column = 48, Value = data[i].UniformSizeAlt },
                    new ExcelCellItem { Column = 49, Value = data[i].ShoesSize },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            return package;
        }

        private static ExcelPackage GEOutstandingTraining(ExcelPackage package, List<OutstandingTraining> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 6,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00);

            double cWidth = GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Outstanding Training Data";
            sheet.Cells[x].Style.Font.Size = 20;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w1143, Value = "Department" },
                new ExcelCellItem { Column = 2, Width = cWidth, Value = "Position" },
                new ExcelCellItem { Column = 3, Width = cWidth, Value = "Grade" },
                new ExcelCellItem { Column = 4, Width = w1600, Value = "Training Name" },
                new ExcelCellItem { Column = 5, Width = w2000, Value = "Training Description" },
                new ExcelCellItem { Column = 6, Width = w1086, Value = "Man Power" },
                new ExcelCellItem { Column = 7, Width = w1086, Value = "Trained" },
                new ExcelCellItem { Column = 8, Width = w1086, Value = "Not Trained" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            #endregion

            //DATA
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].Department },
                    new ExcelCellItem { Column = 2, Value = data[i].Position },
                    new ExcelCellItem { Column = 3, Value = data[i].GradeName },
                    new ExcelCellItem { Column = 4, Value = data[i].TrainingName },
                    new ExcelCellItem { Column = 5, Value = data[i].TrainingDescription },
                    new ExcelCellItem { Column = 6, Value = data[i].ManPower },
                    new ExcelCellItem { Column = 7, Value = data[i].Trained },
                    new ExcelCellItem { Column = 8, Value = data[i].NotTrained }
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            return package;
        }

        private static ExcelPackage GEScShBmTraining(ExcelPackage package, List<ScShBmTraining> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 6,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00);

            double cWidth = GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Outstanding Training Data";
            sheet.Cells[x].Style.Font.Size = 20;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w3000, Value = "Outlet Name" },
                new ExcelCellItem { Column = 2, Width = w1086, Value = "SC" },
                new ExcelCellItem { Column = 3, Width = w1086, Value = "SH" },
                new ExcelCellItem { Column = 4, Width = w1086, Value = "BM" },
                new ExcelCellItem { Column = 5, Width = w1086, Value = "SC BASIC" },
                new ExcelCellItem { Column = 6, Width = w1086, Value = "SC ADVANCE" },
                new ExcelCellItem { Column = 7, Width = w1086, Value = "SH BASIC" },
                new ExcelCellItem { Column = 8, Width = cWidth, Value = "SH INTERMERDIATE" },
                new ExcelCellItem { Column = 9, Width = w1086, Value = "SH ADVANCE" },
                new ExcelCellItem { Column = 10, Width = w1086, Value = "BM BASIC" },
                new ExcelCellItem { Column = 11, Width = cWidth, Value = "BM INTERMERDIATE" },
                new ExcelCellItem { Column = 12, Width = w1086, Value = "BM ADVANCE" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            #endregion

            //DATA
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].BranchName },
                    new ExcelCellItem { Column = 2, Value = data[i].SC },
                    new ExcelCellItem { Column = 3, Value = data[i].SH },
                    new ExcelCellItem { Column = 4, Value = data[i].BM },
                    new ExcelCellItem { Column = 5, Value = data[i].SCBasic },
                    new ExcelCellItem { Column = 6, Value = data[i].SCAdvance },
                    new ExcelCellItem { Column = 7, Value = data[i].SHBasic },
                    new ExcelCellItem { Column = 8, Value = data[i].SHIntermediate },
                    new ExcelCellItem { Column = 9, Value = data[i].SHAdvance },
                    new ExcelCellItem { Column = 10, Value = data[i].BMBasic },
                    new ExcelCellItem { Column = 11, Value = data[i].BMIntermediate },
                    new ExcelCellItem { Column = 12, Value = data[i].BMAdvance },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            return package;
        }

        private static ExcelPackage GESfmMutation(ExcelPackage package, List<HrInqMutation> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 6,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00),
                w6000 = GetTrueColWidth(60.00);

            double cWidth = GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Mutation Data";
            sheet.Cells[x].Style.Font.Size = 20;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w6000, Value = "(Branch) Outlet" },
                new ExcelCellItem { Column = 2, Width = w1086, Value = "Awal" },
                new ExcelCellItem { Column = 3, Width = w1086, Value = "Join" },
                new ExcelCellItem { Column = 4, Width = w1243, Value = "Mutasi(In)" },
                new ExcelCellItem { Column = 5, Width = w1086, Value = "Resign" },
                new ExcelCellItem { Column = 6, Width = w1243, Value = "Mutasi(Out)" },
                new ExcelCellItem { Column = 7, Width = w1086, Value = "Akhir" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            #endregion

            //DATA
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].BranchName },
                    new ExcelCellItem { Column = 2, Value = data[i].Muta01 },
                    new ExcelCellItem { Column = 3, Value = data[i].Muta02 },
                    new ExcelCellItem { Column = 4, Value = data[i].Muta03 },
                    new ExcelCellItem { Column = 5, Value = data[i].Muta04 },
                    new ExcelCellItem { Column = 6, Value = data[i].Muta05 },
                    new ExcelCellItem { Column = 7, Value = data[i].Muta06 }
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            return package;
        }

        private static ExcelPackage GESfmTrend(ExcelPackage package, List<HrInqTrend> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 6,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00),
                w6000 = GetTrueColWidth(60.00);

            double cWidth = GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Trend Mutation Data";
            sheet.Cells[x].Style.Font.Size = 20;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w6000, Value = "(Branch) Outlet" },
                new ExcelCellItem { Column = 2, Width = w1086, Value = "Jan" },
                new ExcelCellItem { Column = 3, Width = w1086, Value = "Feb" },
                new ExcelCellItem { Column = 4, Width = w1086, Value = "Mar" },
                new ExcelCellItem { Column = 5, Width = w1086, Value = "Apr" },
                new ExcelCellItem { Column = 6, Width = w1086, Value = "May" },
                new ExcelCellItem { Column = 7, Width = w1086, Value = "Jun" },
                new ExcelCellItem { Column = 8, Width = w1086, Value = "Jul" },
                new ExcelCellItem { Column = 9, Width = w1086, Value = "Aug" },
                new ExcelCellItem { Column = 10, Width = w1086, Value = "Sep" },
                new ExcelCellItem { Column = 11, Width = w1086, Value = "Oct" },
                new ExcelCellItem { Column = 12, Width = w1086, Value = "Nov" },
                new ExcelCellItem { Column = 13, Width = w1086, Value = "Dec" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            #endregion

            //DATA
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].BranchName },
                    new ExcelCellItem { Column = 2, Value =  data[i].Month01 },
                    new ExcelCellItem { Column = 3, Value =  data[i].Month02 },
                    new ExcelCellItem { Column = 4, Value =  data[i].Month03 },
                    new ExcelCellItem { Column = 5, Value =  data[i].Month04 },
                    new ExcelCellItem { Column = 6, Value =  data[i].Month05 },
                    new ExcelCellItem { Column = 7, Value =  data[i].Month06 },
                    new ExcelCellItem { Column = 8, Value =  data[i].Month07 },
                    new ExcelCellItem { Column = 9, Value =  data[i].Month08 },
                    new ExcelCellItem { Column = 10, Value = data[i].Month09 },
                    new ExcelCellItem { Column = 11, Value = data[i].Month10 },
                    new ExcelCellItem { Column = 12, Value = data[i].Month11 },
                    new ExcelCellItem { Column = 13, Value = data[i].Month12 }
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            return package;
        }

        private static ExcelPackage GESfmTurnOver(ExcelPackage package, List<TurnOver> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 6,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00),
                w6000 = GetTrueColWidth(60.00);

            double cWidth = GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            //sheet.Cells[x].Value = "Report Mutation Data";
            sheet.Cells[x].Value = "Report Turn-Over Data";
            sheet.Cells[x].Style.Font.Size = 20;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w6000, Value = "(Branch) Outlet" },
                new ExcelCellItem { Column = 2, Width = w1243, Value = "Trainee" },
                new ExcelCellItem { Column = 3, Width = w1243, Value = "Silver" },
                new ExcelCellItem { Column = 4, Width = w1243, Value = "Gold" },
                new ExcelCellItem { Column = 5, Width = w1243, Value = "Platinum" },
                new ExcelCellItem { Column = 6, Width = w2000, Value = "Trainee Terminated" },
                new ExcelCellItem { Column = 7, Width = w2000, Value = "Silver Terminated" },
                new ExcelCellItem { Column = 8, Width = w2000, Value = "Gold Terminated" },
                new ExcelCellItem { Column = 9, Width = w2000, Value = "Platinum Terminated" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            #endregion

            //DATA
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].BranchName },
                    new ExcelCellItem { Column = 2, Value =  data[i].Trainee },
                    new ExcelCellItem { Column = 3, Value =  data[i].Silver },
                    new ExcelCellItem { Column = 4, Value =  data[i].Gold },
                    new ExcelCellItem { Column = 5, Value =  data[i].Platinum },
                    new ExcelCellItem { Column = 6, Value =  data[i].TraineeTerminated },
                    new ExcelCellItem { Column = 7, Value =  data[i].SilverTerminated },
                    new ExcelCellItem { Column = 8, Value =  data[i].GoldTerminated },
                    new ExcelCellItem { Column = 9, Value = data[i].PlatinumTerminated },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            return package;
        }

        private static ExcelPackage GESfmReview(ExcelPackage package, List<ReviewSfm> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 6,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00),
                w6000 = GetTrueColWidth(60.00);

            double cWidth = GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Mutation Data";
            sheet.Cells[x].Style.Font.Size = 20;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w6000, Value = "Outlet Name" },
                new ExcelCellItem { Column = 2, Width = cWidth, Value = "Total Wiraniaga" },
                new ExcelCellItem { Column = 3, Width = w1243, Value = "Trainee" },
                new ExcelCellItem { Column = 4, Width = w1243, Value = "Silver" },
                new ExcelCellItem { Column = 5, Width = w1243, Value = "Gold" },
                new ExcelCellItem { Column = 6, Width = w1243, Value = "Platinum" },
                new ExcelCellItem { Column = 7, Width = w1243, Value = "SC" },
                new ExcelCellItem { Column = 8, Width = w1243, Value = "SH" },
                new ExcelCellItem { Column = 9, Width = w1243, Value = "BM" },
                new ExcelCellItem { Column = 10, Width = cWidth, Value = "Total SC SH BM" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            #endregion

            //DATA
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].BranchName },
                    new ExcelCellItem { Column = 2, Value =  data[i].TotalSalesman },
                    new ExcelCellItem { Column = 3, Value =  data[i].Trainee },
                    new ExcelCellItem { Column = 4, Value =  data[i].Silver },
                    new ExcelCellItem { Column = 5, Value =  data[i].Gold },
                    new ExcelCellItem { Column = 6, Value =  data[i].Platinum },
                    new ExcelCellItem { Column = 7, Value =  data[i].SC },
                    new ExcelCellItem { Column = 8, Value =  data[i].SH },
                    new ExcelCellItem { Column = 9, Value =  data[i].BM },
                    new ExcelCellItem { Column = 10, Value = data[i].TotalSCSHBM }
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            return package;
        }

        private static ExcelPackage GenerateExcelMutation(ExcelPackage package, List<HrInqEmployee> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 6,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 4;
            double
                wNIK = GetTrueColWidth(11.43),
                wName = GetTrueColWidth(30.00),
                wJoinDate = GetTrueColWidth(14.00),
                wLastBranch = GetTrueColWidth(10.86),
                wLastPosition = GetTrueColWidth(12.43),
                wStatus = GetTrueColWidth(14.00),
                wIsValid = GetTrueColWidth(10.86);
            #endregion
            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Personal Mutation Data";
            sheet.Cells[x].Style.Font.Size = 20;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = wNIK, Value = "NIK" },
                new ExcelCellItem { Column = 2, Width = wName, Value = "Name" },
                new ExcelCellItem { Column = 3, Width = wJoinDate, Value = "Join Date" },
                new ExcelCellItem { Column = 4, Width = wLastBranch, Value = "Last Branch" },
                new ExcelCellItem { Column = 5, Width = wLastPosition, Value = "Last Position" },
                new ExcelCellItem { Column = 6, Width = wStatus, Value = "Status" },
                new ExcelCellItem { Column = 7, Width = wIsValid, Value = "Is Valid" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            #endregion

            //DATA
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].EmployeeID },
                    new ExcelCellItem { Column = 2, Value = data[i].EmployeeName },
                    new ExcelCellItem { Column = 3, Value = data[i].JoinDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 4, Value = data[i].LastBranch },
                    new ExcelCellItem { Column = 5, Value = data[i].Position },
                    new ExcelCellItem { Column = 6, Value = data[i].Status },
                    new ExcelCellItem { Column = 7, Value = data[i].IsValid },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            return package;
        }

        private static ExcelPackage GenerateExcelPersonalInfoSF(ExcelPackage package, List<HrInqPersInfoDetail> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 6,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w3000 = GetTrueColWidth(30.00),
                w1243 = GetTrueColWidth(12.43),
                w1143 = GetTrueColWidth(11.43),
                w2000 = GetTrueColWidth(20.00),
                w1800 = GetTrueColWidth(18.00),
                w1900 = GetTrueColWidth(19.00),
                w1600 = GetTrueColWidth(16.00);

            double cWidth = GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Personal List Sales Force";
            sheet.Cells[x].Style.Font.Size = 20;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w1086, Value = "Dealer Code" },
                new ExcelCellItem { Column = 2, Width = w1086, Value = "Branch Code" },
                new ExcelCellItem { Column = 3, Width = w3000, Value = "Dealer" },
                new ExcelCellItem { Column = 4, Width = w3000, Value = "Branch" },
                new ExcelCellItem { Column = 5, Width = w1243, Value = "Department" },
                new ExcelCellItem { Column = 6, Width = w2000, Value = "ATPM ID" },
                new ExcelCellItem { Column = 7, Width = w1143, Value = "NIK" },
                new ExcelCellItem { Column = 8, Width = w3000, Value = "Name" },
                new ExcelCellItem { Column = 9, Width = w1600, Value = "Position" },
                new ExcelCellItem { Column = 10, Width = w1243, Value = "Grade" },
                new ExcelCellItem { Column = 11, Width = w1243, Value = "Additional Job" },
                new ExcelCellItem { Column = 12, Width = cWidth, Value = "Status" },
                new ExcelCellItem { Column = 13, Width = cWidth, Value = "Team Leader" },
                new ExcelCellItem { Column = 14, Width = cWidth, Value = "Join Date" },
                new ExcelCellItem { Column = 15, Width = cWidth, Value = "Resign Date" },
                new ExcelCellItem { Column = 16, Width = w2000, Value = "Resign Description" },
                new ExcelCellItem { Column = 17, Width = cWidth, Value = "Marital Status" },
                new ExcelCellItem { Column = 18, Width = cWidth, Value = "Religion" },
                new ExcelCellItem { Column = 19, Width = cWidth, Value = "Gender" },
                new ExcelCellItem { Column = 20, Width = w1800, Value = "Education" },
                new ExcelCellItem { Column = 21, Width = w1800, Value = "Birth Place" },
                new ExcelCellItem { Column = 22, Width = cWidth, Value = "Birth Date" },
                new ExcelCellItem { Column = 23, Width = w3000, Value = "Address" },
                new ExcelCellItem { Column = 24, Width = w1900, Value = "Province" },
                new ExcelCellItem { Column = 25, Width = cWidth, Value = "City" },
                new ExcelCellItem { Column = 26, Width = cWidth, Value = "Zip Code" },
                new ExcelCellItem { Column = 27, Width = w1800, Value = "Identity Number" },
                new ExcelCellItem { Column = 28, Width = w1800, Value = "NPWP No" },
                new ExcelCellItem { Column = 29, Width = cWidth, Value = "NPWP Date" },
                new ExcelCellItem { Column = 30, Width = w3000, Value = "Email" },
                new ExcelCellItem { Column = 31, Width = cWidth, Value = "Telephone 1" },
                new ExcelCellItem { Column = 32, Width = cWidth, Value = "Telephone 2" },
                new ExcelCellItem { Column = 33, Width = cWidth, Value = "Handphone 1" },
                new ExcelCellItem { Column = 34, Width = cWidth, Value = "Handphone 2" },
                new ExcelCellItem { Column = 35, Width = cWidth, Value = "Handphone 3" },
                new ExcelCellItem { Column = 36, Width = cWidth, Value = "Handphone 4" },
                new ExcelCellItem { Column = 37, Width = cWidth, Value = "Height" },
                new ExcelCellItem { Column = 38, Width = cWidth, Value = "Weight" },
                new ExcelCellItem { Column = 39, Width = cWidth, Value = "Uniforms Size" },
                new ExcelCellItem { Column = 40, Width = cWidth, Value = "Uniform Size Alt" },
                new ExcelCellItem { Column = 41, Width = cWidth, Value = "PreTraining" },
                new ExcelCellItem { Column = 42, Width = cWidth, Value = "PreTrainingPostTest" },
                new ExcelCellItem { Column = 43, Width = cWidth, Value = "Pembekalan" },
                new ExcelCellItem { Column = 44, Width = cWidth, Value = "Nilai Akhir Pembekalan" },
                new ExcelCellItem { Column = 45, Width = cWidth, Value = "Salesmanship" },
                new ExcelCellItem { Column = 46, Width = cWidth, Value = "Nilai Akhir Salesmanship" },
                new ExcelCellItem { Column = 47, Width = cWidth, Value = "OJT" },
                new ExcelCellItem { Column = 48, Width = cWidth, Value = "Nilai Akhir Ojt" },
                new ExcelCellItem { Column = 49, Width = cWidth, Value = "Final Review" },
                new ExcelCellItem { Column = 50, Width = cWidth, Value = "Nilai Akhir Final Review" },
                new ExcelCellItem { Column = 51, Width = cWidth, Value = "Sales Silver" },
                new ExcelCellItem { Column = 52, Width = cWidth, Value = "Sales Silver PostTest" },
                new ExcelCellItem { Column = 53, Width = cWidth, Value = "Sales Gold" },
                new ExcelCellItem { Column = 54, Width = cWidth, Value = "Sales Gold PostTest" },
                new ExcelCellItem { Column = 55, Width = cWidth, Value = "Sales Platinum" },
                new ExcelCellItem { Column = 56, Width = cWidth, Value = "Sales Platinum PostTest" },
                new ExcelCellItem { Column = 57, Width = cWidth, Value = "SC Basic" },
                new ExcelCellItem { Column = 58, Width = cWidth, Value = "SC Advance" },
                new ExcelCellItem { Column = 59, Width = cWidth, Value = "SH Basic" },
                new ExcelCellItem { Column = 60, Width = cWidth, Value = "SH Intermediate" },
                new ExcelCellItem { Column = 61, Width = cWidth, Value = "BM Basic" },
                new ExcelCellItem { Column = 62, Width = cWidth, Value = "BM Intermediate" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            #endregion

            //DATA
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].CompanyCode },
                    new ExcelCellItem { Column = 2, Value = data[i].BranchCode },
                    new ExcelCellItem { Column = 3, Value = data[i].DealerName },
                    new ExcelCellItem { Column = 4, Value = data[i].BranchName },
                    new ExcelCellItem { Column = 5, Value = data[i].DeptCode },
                    new ExcelCellItem { Column = 6, Value = data[i].SalesID },
                    new ExcelCellItem { Column = 7, Value = data[i].EmployeeID },
                    new ExcelCellItem { Column = 8, Value = data[i].EmployeeName },
                    new ExcelCellItem { Column = 9, Value = data[i].Position },
                    new ExcelCellItem { Column = 10, Value = data[i].Grade },
                    new ExcelCellItem { Column = 11, Value = data[i].AdditionalJob },
                    new ExcelCellItem { Column = 12, Value = data[i].Status },
                    new ExcelCellItem { Column = 13, Value = data[i].TeamLeader },
                    new ExcelCellItem { Column = 14, Value = data[i].JoinDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 15, Value = data[i].ResignDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 16, Value = data[i].ResignDescription },
                    new ExcelCellItem { Column = 17, Value = data[i].MaritalStatus },
                    new ExcelCellItem { Column = 18, Value = data[i].Religion },
                    new ExcelCellItem { Column = 19, Value = data[i].Gender },
                    new ExcelCellItem { Column = 20, Value = data[i].Education },
                    new ExcelCellItem { Column = 21, Value = data[i].BirthPlace },
                    new ExcelCellItem { Column = 22, Value = data[i].BirthDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 23, Value = data[i].Address },
                    new ExcelCellItem { Column = 24, Value = data[i].Province },
                    new ExcelCellItem { Column = 25, Value = data[i].City },
                    new ExcelCellItem { Column = 26, Value = data[i].ZipCode },
                    new ExcelCellItem { Column = 27, Value = data[i].IdentityNo },
                    new ExcelCellItem { Column = 28, Value = data[i].NPWPNo },
                    new ExcelCellItem { Column = 29, Value = data[i].NPWPDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 30, Value = data[i].Email },
                    new ExcelCellItem { Column = 31, Value = data[i].Telephone1 },
                    new ExcelCellItem { Column = 32, Value = data[i].Telephone2 },
                    new ExcelCellItem { Column = 33, Value = data[i].Handphone1 },
                    new ExcelCellItem { Column = 34, Value = data[i].Handphone2 },
                    new ExcelCellItem { Column = 35, Value = data[i].Handphone3 },
                    new ExcelCellItem { Column = 36, Value = data[i].Handphone4 },
                    new ExcelCellItem { Column = 37, Value = data[i].Height },
                    new ExcelCellItem { Column = 38, Value = data[i].Weight },
                    new ExcelCellItem { Column = 39, Value = data[i].UniformSize },
                    new ExcelCellItem { Column = 40, Value = data[i].UniformSizeAlt },
                    new ExcelCellItem { Column = 41, Value = data[i].PreTraining, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 42, Value = data[i].PreTrainingPostTest },
                    new ExcelCellItem { Column = 43, Value = data[i].Pembekalan, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 44, Value = data[i].PembekalanPostTest },
                    new ExcelCellItem { Column = 45, Value = data[i].Salesmanship, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 46, Value = data[i].SalesmanshipPostTest },
                    new ExcelCellItem { Column = 47, Value = data[i].OJT, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 48, Value = data[i].OjtPostTest },
                    new ExcelCellItem { Column = 49, Value = data[i].FinalReview, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 50, Value = data[i].FinalReviewPostTest },
                    new ExcelCellItem { Column = 51, Value = data[i].SpsSlv, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 52, Value = data[i].SpsSlvPostTest },
                    new ExcelCellItem { Column = 53, Value = data[i].SpsGld, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 54, Value = data[i].SpsGldPostTest },
                    new ExcelCellItem { Column = 55, Value = data[i].SpsPlt, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 56, Value = data[i].SpsPltPostTest },
                    new ExcelCellItem { Column = 57, Value = data[i].SCBsc, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 58, Value = data[i].SCAdv, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 59, Value = data[i].SHBsc, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 60, Value = data[i].SHInt, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 61, Value = data[i].BMBsc, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 62, Value = data[i].BMInt, Format = "dd-MMM-YYYY" },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }
            return package;
        }

        private static ExcelPackage GenerateExcelSalesmanTraining(ExcelPackage package, List<SalesmanTraining> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 6,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w3200 = GetTrueColWidth(32.00),
                w1243 = GetTrueColWidth(12.43),
                w1900 = GetTrueColWidth(19.00),
                w1600 = GetTrueColWidth(16.00);

            double cWidth = GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Salesman Training";
            sheet.Cells[x].Style.Font.Size = 20;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w3200, Value = "Dealer Name" },
                new ExcelCellItem { Column = 2, Width = w3200, Value = "Outlet Name" },
                new ExcelCellItem { Column = 3, Width = w1243, Value = "Trainee" },
                new ExcelCellItem { Column = 4, Width = w1243, Value = "Silver" },
                new ExcelCellItem { Column = 5, Width = w1243, Value = "Gold" },
                new ExcelCellItem { Column = 6, Width = w1243, Value = "Platinum" },
                new ExcelCellItem { Column = 7, Width = w1900, Value = "Total Wiraniaga" },
                new ExcelCellItem { Column = 8, Width = w1900, Value = "Gold Terminated" },
                new ExcelCellItem { Column = 9, Width = w1900, Value = "Platinum Terminated" },
                new ExcelCellItem { Column = 10, Width = w1086, Value = "STDP 1" },
                new ExcelCellItem { Column = 11, Width = w1086, Value = "STDP 2" },
                new ExcelCellItem { Column = 12, Width = w1086, Value = "STDP 3" },
                new ExcelCellItem { Column = 13, Width = w1086, Value = "STDP 4" },
                new ExcelCellItem { Column = 14, Width = w1086, Value = "STDP 5" },
                new ExcelCellItem { Column = 15, Width = w1086, Value = "STDP 6" },
                new ExcelCellItem { Column = 16, Width = w1086, Value = "STDP 7" },
                new ExcelCellItem { Column = 17, Width = w1600, Value = "Total STDP" },
                new ExcelCellItem { Column = 18, Width = w1600, Value = "SPS Silver" },
                new ExcelCellItem { Column = 19, Width = w1600, Value = "SPS Gold" },
                new ExcelCellItem { Column = 20, Width = w1600, Value = "SPS Platinum" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            #endregion

            //DATA
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].CompanyName },
                    new ExcelCellItem { Column = 2, Value = data[i].BranchName },
                    new ExcelCellItem { Column = 3, Value = data[i].Trainee },
                    new ExcelCellItem { Column = 4, Value = data[i].Silver },
                    new ExcelCellItem { Column = 5, Value = data[i].Gold },
                    new ExcelCellItem { Column = 6, Value = data[i].Platinum },
                    new ExcelCellItem { Column = 7, Value = data[i].TotalSalesman },
                    new ExcelCellItem { Column = 8, Value = data[i].GoldTerminated },
                    new ExcelCellItem { Column = 9, Value = data[i].PlatinumTerminated },
                    new ExcelCellItem { Column = 10, Value = data[i].STDP1 },
                    new ExcelCellItem { Column = 11, Value = data[i].STDP2 },
                    new ExcelCellItem { Column = 12, Value = data[i].STDP3 },
                    new ExcelCellItem { Column = 13, Value = data[i].STDP4 },
                    new ExcelCellItem { Column = 14, Value = data[i].STDP5 },
                    new ExcelCellItem { Column = 15, Value = data[i].STDP6 },
                    new ExcelCellItem { Column = 16, Value = data[i].STDP7 },
                    new ExcelCellItem { Column = 17, Value = data[i].TotalSTDP },
                    new ExcelCellItem { Column = 18, Value = data[i].SPSSilver },
                    new ExcelCellItem { Column = 19, Value = data[i].SPSGold },
                    new ExcelCellItem { Column = 20, Value = data[i].SPSPlatinum },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            return package;
        }

        private static ExcelPackage GenerateExcelCSIScore(ExcelPackage package, List<SvCustomerSatisfactionScore> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 6,
                rPeriod = 4,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1243 = GetTrueColWidth(12.43),
                w1900 = GetTrueColWidth(19.00),
                w1600 = GetTrueColWidth(16.00),
                w3200 = GetTrueColWidth(32.00),
                w7000 = GetTrueColWidth(70.00);

            double cWidth = GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report CSI Score";
            sheet.Cells[x].Style.Font.Size = 20;

            x = GetCell(rPeriod, cStart);
            sheet.Cells[x].Value = "Period";

            x = GetCell(rPeriod, 2);
            sheet.Cells[x].Value = new DateTime((int)data.FirstOrDefault().PeriodYear, (int)data.FirstOrDefault().PeriodMonth, 1);
            sheet.Cells[x].Style.Numberformat.Format = "MMM-YYYY";

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w1086, Value = "No" },
                new ExcelCellItem { Column = 2, Width = w1243, Value = "Nama" },
                new ExcelCellItem { Column = 3, Width = w7000, Value = "Dealer" },
                new ExcelCellItem { Column = 4, Width = w1243, Value = "SI" },
                new ExcelCellItem { Column = 5, Width = w1243, Value = "SA" },
                new ExcelCellItem { Column = 6, Width = w1243, Value = "SF" },
                new ExcelCellItem { Column = 7, Width = w1243, Value = "VP" },
                new ExcelCellItem { Column = 8, Width = w1243, Value = "SQ" },
                new ExcelCellItem { Column = 9, Width = w1243, Value = "Total Score" }
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            #endregion

            //DATA
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                string sCode = data[i].ServiceCode;
                string name = ctx.svMstDealerAndOutletServiceMappings.FirstOrDefault(o => o.ServiceCode == sCode).ServiceName;
                x = GetCell(row, 3);
                if (ctx.GnMstDealerMappings.Where(o => o.DealerCode == sCode).Count() > 0)
                {
                    sheet.Cells[x].Style.Font.Bold = true;
                }
                else
                {
                    name = "        " + name;
                }

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = i + 1 },
                    new ExcelCellItem { Column = 2, Value = data[i].ServiceCode },
                    new ExcelCellItem { Column = 3, Value = name },
                    new ExcelCellItem { Column = 4, Value = data[i].ServiceInitiation  }, //, Format = "##,###0.00" },
                    new ExcelCellItem { Column = 5, Value = data[i].ServiceAdvisor     }, //, Format = "##,###0.00" },
                    new ExcelCellItem { Column = 6, Value = data[i].ServiceFaciltiy    }, //, Format = "##,###0.00" },
                    new ExcelCellItem { Column = 7, Value = data[i].VehiclePickup      }, //, Format = "##,###0.00" },
                    new ExcelCellItem { Column = 8, Value = data[i].ServiceQuality     }, //, Format = "##,###0.00" },
                    new ExcelCellItem { Column = 9, Value = data[i].Score              }  //, Format = "##,###0.00" }
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            return package;
        }

        private static ExcelPackage GenerateExcelUnitRevenueTarget(ExcelPackage package, List<svMstUnitRevenueTarget> dataUnit, List<svMstUnitRevenueTarget> dataRevenue)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rSubtitle = 3,
                rHdrFirst = 6,
                rDataStart = 8,

                cStart = 1,
                cSubtitle = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w1900 = GetTrueColWidth(19.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00);
            #endregion
            var sheet = package.Workbook.Worksheets.Add("Unit & Revenue Service Target");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Unit & Revenue Service Target";
            sheet.Cells[x].Style.Font.Size = 20;

            x = GetCell(rSubtitle, cSubtitle);
            sheet.Cells[x].Value = "Periode Year : " + dataUnit[0].PeriodYear;
            sheet.Cells[x].Style.Font.Size = 13;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w3000, Value = "Region" },
                new ExcelCellItem { Column = 2, Width = w1243, Value = "Target Unit" },
                new ExcelCellItem { Column = 3, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 4, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 5, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 6, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 7, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 8, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 9, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 10, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 11, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 12, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 13, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 14, Width = w1900, Value = "Total Target" },
                new ExcelCellItem { Column = 15, Width = w1243, Value = "Target Revenue" },
                new ExcelCellItem { Column = 16, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 17, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 18, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 19, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 20, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 21, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 22, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 23, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 24, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 25, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 26, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 27, Width = w1900, Value = "Total Target" },
            };

            sheet.Cells[GetRange(rSubtitle, cSubtitle, rSubtitle, (cSubtitle + 1))].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 2, rHdrFirst, 13)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, cStart, (rHdrFirst + 1), cStart)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 14, (rHdrFirst + 1), 14)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 15, rHdrFirst, 26)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 27, (rHdrFirst + 1), 27)].Merge = true;

            var headers2 = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w3000, Value = "" },
                new ExcelCellItem { Column = 2, Width = w1243, Value = "Jan" },
                new ExcelCellItem { Column = 3, Width = w1243, Value = "Feb" },
                new ExcelCellItem { Column = 4, Width = w1243, Value = "Mar" },
                new ExcelCellItem { Column = 5, Width = w1243, Value = "Apr" },
                new ExcelCellItem { Column = 6, Width = w1243, Value = "May" },
                new ExcelCellItem { Column = 7, Width = w1243, Value = "Jun" },
                new ExcelCellItem { Column = 8, Width = w1243, Value = "Jul" },
                new ExcelCellItem { Column = 9, Width = w1243, Value = "Aug" },
                new ExcelCellItem { Column = 10, Width = w1243, Value = "Sep" },
                new ExcelCellItem { Column = 11, Width = w1243, Value = "Oct" },
                new ExcelCellItem { Column = 12, Width = w1243, Value = "Nov" },
                new ExcelCellItem { Column = 13, Width = w1243, Value = "Des" },
                new ExcelCellItem { Column = 14, Width = w1900, Value = "" },
                new ExcelCellItem { Column = 15, Width = w1243, Value = "Jan" },
                new ExcelCellItem { Column = 16, Width = w1243, Value = "Feb" },
                new ExcelCellItem { Column = 17, Width = w1243, Value = "Mar" },
                new ExcelCellItem { Column = 18, Width = w1243, Value = "Apr" },
                new ExcelCellItem { Column = 19, Width = w1243, Value = "May" },
                new ExcelCellItem { Column = 20, Width = w1243, Value = "Jun" },
                new ExcelCellItem { Column = 21, Width = w1243, Value = "Jul" },
                new ExcelCellItem { Column = 22, Width = w1243, Value = "Aug" },
                new ExcelCellItem { Column = 23, Width = w1243, Value = "Sep" },
                new ExcelCellItem { Column = 24, Width = w1243, Value = "Oct" },
                new ExcelCellItem { Column = 25, Width = w1243, Value = "Nov" },
                new ExcelCellItem { Column = 26, Width = w1243, Value = "Des" },
                new ExcelCellItem { Column = 27, Width = w1900, Value = "" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            foreach (var header in headers2)
            {
                x = GetCell((rHdrFirst+1), header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            #endregion

            #region -- DATA --
            if (dataUnit.Count() == 0) return package;
            for (int i = 0; i < dataUnit.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = dataUnit[i].AreaDealer },
                    new ExcelCellItem { Column = 2, Value = dataUnit[i].Target01, Format = "#,##0" },
                    new ExcelCellItem { Column = 3, Value = dataUnit[i].Target02, Format = "#,##0" },
                    new ExcelCellItem { Column = 4, Value = dataUnit[i].Target03, Format = "#,##0" },
                    new ExcelCellItem { Column = 5, Value = dataUnit[i].Target04, Format = "#,##0" },
                    new ExcelCellItem { Column = 6, Value = dataUnit[i].Target05, Format = "#,##0" },
                    new ExcelCellItem { Column = 7, Value = dataUnit[i].Target06, Format = "#,##0" },
                    new ExcelCellItem { Column = 8, Value = dataUnit[i].Target07, Format = "#,##0" },
                    new ExcelCellItem { Column = 9, Value = dataUnit[i].Target08, Format = "#,##0" },
                    new ExcelCellItem { Column = 10, Value = dataUnit[i].Target09, Format = "#,##0" },
                    new ExcelCellItem { Column = 11, Value = dataUnit[i].Target10, Format = "#,##0" },
                    new ExcelCellItem { Column = 12, Value = dataUnit[i].Target11, Format = "#,##0" },
                    new ExcelCellItem { Column = 13, Value = dataUnit[i].Target12, Format = "#,##0" },
                    new ExcelCellItem { Column = 14, Value = dataUnit[i].TotalTarget, Format = "#,##0" },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            if (dataRevenue.Count() == 0) return package;
            for (int i = 0; i < dataRevenue.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 15, Value = dataRevenue[i].Target01, Format = "#,##0" },
                    new ExcelCellItem { Column = 16, Value = dataRevenue[i].Target02, Format = "#,##0" },
                    new ExcelCellItem { Column = 17, Value = dataRevenue[i].Target03, Format = "#,##0" },
                    new ExcelCellItem { Column = 18, Value = dataRevenue[i].Target04, Format = "#,##0" },
                    new ExcelCellItem { Column = 19, Value = dataRevenue[i].Target05, Format = "#,##0" },
                    new ExcelCellItem { Column = 20, Value = dataRevenue[i].Target06, Format = "#,##0" },
                    new ExcelCellItem { Column = 21, Value = dataRevenue[i].Target07, Format = "#,##0" },
                    new ExcelCellItem { Column = 22, Value = dataRevenue[i].Target08, Format = "#,##0" },
                    new ExcelCellItem { Column = 23, Value = dataRevenue[i].Target09, Format = "#,##0" },
                    new ExcelCellItem { Column = 24, Value = dataRevenue[i].Target10, Format = "#,##0" },
                    new ExcelCellItem { Column = 25, Value = dataRevenue[i].Target11, Format = "#,##0" },
                    new ExcelCellItem { Column = 26, Value = dataRevenue[i].Target12, Format = "#,##0" },
                    new ExcelCellItem { Column = 27, Value = dataRevenue[i].TotalTarget, Format = "#,##0" },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }
            #endregion

            return package;
        }

        private static ExcelPackage GenerateExcelVOR(ExcelPackage package, string DealerName, string startDate, string endDate, List<VORPart> data)
        {
            DateTime Start = Convert.ToDateTime(startDate);
            DateTime End = Convert.ToDateTime(endDate);
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rSubtitle = 3,
                rHdrFirst = 6,
                rDataStart = 8,

                cStart = 1,
                cSubtitle = 1,
                cTitleVal = 21;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w1900 = GetTrueColWidth(19.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");
            
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle + 1, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "VEHICLE OFF THE ROAD REPORT ( VOR REPORT ) / LAPORAN PEKERJAAN TUNDA";
            sheet.Cells[x].Style.Font.Size = 16;
            sheet.Cells[x].Style.Font.Name = "SuzukiPROHeadline";
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            x = GetCell(rSubtitle, cSubtitle);
            sheet.Cells[x].Value = "Periode Laporan : " + Start.ToString("dd MMM yyyy") + " - " + End.ToString("dd MMM yyyy");
            sheet.Cells[x].Style.Font.Size = 13;

            //x = GetCell(rSubtitle + 1, cSubtitle);
            //sheet.Cells[x].Value = "Nama Dealer : " + DealerName;
            //sheet.Cells[x].Style.Font.Size = 13;
            
            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w1600, Value = "Ticket No." },
                new ExcelCellItem { Column = 2, Width = w1086, Value = "Kode Dealer" },
                new ExcelCellItem { Column = 3, Width = w3000, Value = "Nama Dealer" },
                new ExcelCellItem { Column = 4, Width = w1600, Value = "Tanggal Laporan" },
                new ExcelCellItem { Column = 5, Width = w1900, Value = "No Atap / No. Urut Service" },
                new ExcelCellItem { Column = 6, Width = w1086, Value = "No. Polisi" },
                new ExcelCellItem { Column = 7, Width = w1086, Value = "Model" },
                new ExcelCellItem { Column = 8, Width = w3000, Value = "Nama Pelanggan" },
                new ExcelCellItem { Column = 9, Width = w1900, Value = "Nama Petugas" },
                new ExcelCellItem { Column = 10, Width = w1086, Value = "" },
                new ExcelCellItem { Column = 11, Width = w1086, Value = "" },
                new ExcelCellItem { Column = 12, Width = w1086, Value = "Tanggal Masuk" },
                new ExcelCellItem { Column = 13, Width = w1086, Value = "Jam Masuk" },
                new ExcelCellItem { Column = 14, Width = w3000, Value = "Jenis Pekerjaan" },
                new ExcelCellItem { Column = 15, Width = w2000, Value = "Uraian Pekerjaan" },
                new ExcelCellItem { Column = 16, Width = w2000, Value = "Penyebab pekerjaan tunda" },
                new ExcelCellItem { Column = 17, Width = w1086, Value = "Jika Penyebabnya Spare Parts" },
                new ExcelCellItem { Column = 18, Width = w1086, Value = "" },
                new ExcelCellItem { Column = 19, Width = w2000, Value = "Alasan  Penyebab Tertunda" },
                new ExcelCellItem { Column = 20, Width = w1143, Value = "Tanggal Selesai / Close" },
                new ExcelCellItem { Column = 21, Width = w1143, Value = "Cancel VOR" },
            };
            
            // Colspan
            sheet.Cells[GetRange(rHdrFirst, 9, rHdrFirst, 11)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 17, rHdrFirst, 18)].Merge = true;
            // Rowspan
            sheet.Cells[GetRange(rHdrFirst, 1, (rHdrFirst + 1), 1)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 2, (rHdrFirst + 1), 2)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 3, (rHdrFirst + 1), 3)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 4, (rHdrFirst + 1), 4)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 5, (rHdrFirst + 1), 5)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 6, (rHdrFirst + 1), 6)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 7, (rHdrFirst + 1), 7)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 8, (rHdrFirst + 1), 8)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 12, (rHdrFirst + 1), 12)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 13, (rHdrFirst + 1), 13)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 14, (rHdrFirst + 1), 14)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 15, (rHdrFirst + 1), 15)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 16, (rHdrFirst + 1), 16)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 19, (rHdrFirst + 1), 19)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 20, (rHdrFirst + 1), 20)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 21, (rHdrFirst + 1), 21)].Merge = true;

            var headers2 = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Value = "" },
                new ExcelCellItem { Column = 2, Value = "" },
                new ExcelCellItem { Column = 3, Value = "" },
                new ExcelCellItem { Column = 4, Value = "" },
                new ExcelCellItem { Column = 5, Value = "" },
                new ExcelCellItem { Column = 6, Value = "" },
                new ExcelCellItem { Column = 7, Value = "" },
                new ExcelCellItem { Column = 8, Value = "" },
                new ExcelCellItem { Column = 9, Value = "SA" },
                new ExcelCellItem { Column = 10, Value = "Foreman" },
                new ExcelCellItem { Column = 11, Value = "Teknisi" },
                new ExcelCellItem { Column = 12, Value = "" },
                new ExcelCellItem { Column = 13, Value = "" },
                new ExcelCellItem { Column = 14, Value = "" },
                new ExcelCellItem { Column = 15, Value = "" },
                new ExcelCellItem { Column = 16, Value = "" },
                new ExcelCellItem { Column = 17, Value = "No. Suku Cadang" },
                new ExcelCellItem { Column = 18, Value = "Nama Suku Cadang" },
                new ExcelCellItem { Column = 19, Value = "" },
                new ExcelCellItem { Column = 20, Value = "" },
                new ExcelCellItem { Column = 21, Value = "" },
            };
            
            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            
            foreach (var header in headers2)
            {
                x = GetCell((rHdrFirst + 1), header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            /* * */
            #endregion
            
            #region -- Data --

            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].TicketNo },
                    new ExcelCellItem { Column = 2, Value = data[i].DealerCode },
                    new ExcelCellItem { Column = 3, Value = data[i].DealerName },
                    new ExcelCellItem { Column = 4, Value = data[i].SPKDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 5, Value = data[i].SPKNo },
                    new ExcelCellItem { Column = 6, Value = data[i].PoliceRegNo },
                    new ExcelCellItem { Column = 7, Value = data[i].BasicModel },
                    new ExcelCellItem { Column = 8, Value = data[i].Customer },
                    new ExcelCellItem { Column = 9, Value = data[i].SA },
                    new ExcelCellItem { Column = 10, Value = data[i].FM },
                    new ExcelCellItem { Column = 11, Value = data[i].Mech },
                    new ExcelCellItem { Column = 12, Value = data[i].CreatedDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 13, Value = data[i].CreatedDate, Format = "hh:mm" },
                    new ExcelCellItem { Column = 14, Value = data[i].Job },
                    new ExcelCellItem { Column = 15, Value = data[i].ServiceRequestDesc },
                    new ExcelCellItem { Column = 16, Value = data[i].JobDelayDesc },
                    new ExcelCellItem { Column = 17, Value = data[i].PartNo },
                    new ExcelCellItem { Column = 18, Value = data[i].PartName },
                    new ExcelCellItem { Column = 19, Value = data[i].JobReasonDesc },
                    new ExcelCellItem { Column = 20, Value = data[i].ClosedDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 21, Value = data[i].StatusVOR },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.WrapText = true;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                    // sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
            }

            #endregion

            return package;
        }

        private static ExcelPackage GenerateExcelVORSummary(ExcelPackage package, string DealerName, string startDate, string endDate, List<VORSummary> data)
        {
            DateTime Start = Convert.ToDateTime(startDate);
            DateTime End = Convert.ToDateTime(endDate);
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            string x = "";
            const int
                rTitle = 1,
                rHdrFirst = 2,
                rDataStart = 3,

                cStart = 1;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w1900 = GetTrueColWidth(19.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            #region -- Title & Header --
            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 204, 153));

            x = GetCell(rTitle, cStart + 1);
            sheet.Cells[x].Value = "Closed";

            x = GetCell(rTitle, cStart + 2);
            sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 0, 0));

            x = GetCell(rTitle, cStart + 3);
            sheet.Cells[x].Value = "Batal";

            x = GetCell(rTitle, cStart + 4);
            sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 0, 255));

            x = GetCell(rTitle, cStart + 5);
            sheet.Cells[x].Value = "Menggantung";

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w1600, Value = "Ticket No." },
                new ExcelCellItem { Column = 2, Width = w1600, Value = "Area Representative" },
                new ExcelCellItem { Column = 3, Width = w1600, Value = "Tanggal Laporan" },
                new ExcelCellItem { Column = 4, Width = w3000, Value = "Nama Beres" },
                new ExcelCellItem { Column = 5, Width = w1900, Value = "Kota" },
                new ExcelCellItem { Column = 6, Width = w3000, Value = "Region" },
                new ExcelCellItem { Column = 7, Width = w1900, Value = "Kelas Beres" },
                new ExcelCellItem { Column = 8, Width = w1086, Value = "Month" },
                new ExcelCellItem { Column = 9, Width = w1086, Value = "Year" },
                new ExcelCellItem { Column = 10, Width = w1086, Value = "NO" },
                new ExcelCellItem { Column = 11, Width = w1086, Value = "No Polisi" },
                new ExcelCellItem { Column = 12, Width = w3000, Value = "Model" },
                new ExcelCellItem { Column = 13, Width = w2000, Value = "Nama Pelanggan" },
                new ExcelCellItem { Column = 14, Width = w2000, Value = "Service Advisor" },
                new ExcelCellItem { Column = 15, Width = w1086, Value = "Foreman" },
                new ExcelCellItem { Column = 16, Width = w1086, Value = "Teknisi" },
                new ExcelCellItem { Column = 17, Width = w2000, Value = "Tanggal Masuk" },
                new ExcelCellItem { Column = 18, Width = w1143, Value = "Jam Masuk" },
                new ExcelCellItem { Column = 19, Width = w1143, Value = "Jenis Pekerjaan" },
                new ExcelCellItem { Column = 20, Width = w3000, Value = "Uraian Pekerjaan" },
                new ExcelCellItem { Column = 21, Width = w3000, Value = "Penyebab Pekerjaan Tunda" },
                new ExcelCellItem { Column = 22, Width = w3000, Value = "Penyebabnya Sparepart - No.Suku Cadang" },
                new ExcelCellItem { Column = 23, Width = w3000, Value = "Penyebabnya Sparepart - Nama Suku Cadang" },
                new ExcelCellItem { Column = 24, Width = w3000, Value = "Alasan Penyebabnya" },
                new ExcelCellItem { Column = 25, Width = w1143, Value = "Tanggal Selesai / Close" },
                new ExcelCellItem { Column = 26, Width = w1143, Value = "Teknisi yang hadir" },
                new ExcelCellItem { Column = 27, Width = w1143, Value = "Teknisi tidak hadir" },
                new ExcelCellItem { Column = 28, Width = w1143, Value = "Teknisi lembur" },
                new ExcelCellItem { Column = 29, Width = w1143, Value = "Teknisi pekerjaan tunda" },
                new ExcelCellItem { Column = 30, Width = w1143, Value = "Teknisi siap di hari berikutnya" },
                new ExcelCellItem { Column = 31, Width = w1143, Value = "Lama Proses Selesai (Hari)" },
                new ExcelCellItem { Column = 32, Width = w1143, Value = "WIP (Work In Proses)" },
                new ExcelCellItem { Column = 33, Width = w1143, Value = "OPEN / CLOSE" },
                new ExcelCellItem { Column = 34, Width = w1143, Value = "Spare parts note" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            sheet.Cells[GetCell(rHdrFirst, headers[0].Column) + ":" + GetCell(rHdrFirst, headers[headers.Count-1].Column)].AutoFilter = true;
            
            #endregion
            
            #region -- Data --

            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].TicketNo },
                    new ExcelCellItem { Column = 2, Value = "" },
                    new ExcelCellItem { Column = 3, Value = data[i].SPKDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 4, Value = data[i].OutletName },
                    new ExcelCellItem { Column = 5, Value = data[i].OutletArea },
                    new ExcelCellItem { Column = 6, Value = data[i].AreaDealer },
                    new ExcelCellItem { Column = 7, Value = data[i].KalasBeres },
                    new ExcelCellItem { Column = 8, Value = data[i].Month },
                    new ExcelCellItem { Column = 9, Value = data[i].Year },
                    new ExcelCellItem { Column = 10, Value = "" },
                    new ExcelCellItem { Column = 11, Value = data[i].PoliceRegNo },
                    new ExcelCellItem { Column = 12, Value = data[i].BasicModel },
                    new ExcelCellItem { Column = 13, Value = data[i].Customer },
                    new ExcelCellItem { Column = 14, Value = data[i].SA },
                    new ExcelCellItem { Column = 15, Value = data[i].FM },
                    new ExcelCellItem { Column = 16, Value = data[i].Mech },
                    new ExcelCellItem { Column = 17, Value = data[i].CreatedDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 18, Value = data[i].CreatedDate, Format = "hh:mm" },
                    new ExcelCellItem { Column = 19, Value = data[i].Job },
                    new ExcelCellItem { Column = 20, Value = data[i].ServiceRequestDesc },
                    new ExcelCellItem { Column = 21, Value = data[i].JobDelayDesc },
                    new ExcelCellItem { Column = 22, Value = data[i].PartNo },
                    new ExcelCellItem { Column = 23, Value = data[i].PartName },
                    new ExcelCellItem { Column = 24, Value = data[i].JobReasonDesc },
                    new ExcelCellItem { Column = 25, Value = data[i].ClosedDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 26, Value = data[i].CountMechAbs },
                    new ExcelCellItem { Column = 27, Value = data[i].CountMechAlpha },
                    new ExcelCellItem { Column = 28, Value = data[i].CountMechOvrTime },
                    new ExcelCellItem { Column = 29, Value = data[i].CountMechDelay },
                    new ExcelCellItem { Column = 30, Value = data[i].CountMechNextDelay },
                    new ExcelCellItem { Column = 31, Value = data[i].Process },
                    new ExcelCellItem { Column = 32, Value = data[i].Process },
                    new ExcelCellItem { Column = 33, Value = "" },
                    new ExcelCellItem { Column = 34, Value = "" },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.WrapText = true;
                    //sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 204, 153));
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                    sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
            }

            #endregion
            
            return package;
        }

        /*semua tanggal*/
        private static ExcelPackage GenerateExcelVORConsistency(ExcelPackage package, List<VORConsistencyReport> data, dynamic param)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            string z = "", x = "";
            const int
                rTitle = 1,
                rSubtitle = 3,
                rHdrFirst = 9,
                rDataStart = 11,

                cStart = 1,
                cSubtitle = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w1900 = GetTrueColWidth(19.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");
            
            #region -- Title & Header --
            
            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "VEHICLE OFF THE ROAD";
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[x].Style.Font.Size = 16;

            x = GetCell(rSubtitle, cSubtitle);
            sheet.Cells[x].Value = "Area ";
            sheet.Cells[x].Style.Font.Size = 11;

            string GroupNo = "", Area = "";
            if (!param.GroupAreasParam.Equals(""))
            {
                GroupNo = param.GroupAreasParam;
                var GroupArea = ctx.GroupAreas.Where(a => a.GroupNo == GroupNo).Select(a => new { Area = a.AreaDealer }).ToList();
                if (GroupArea.Count > 0)
                    Area = GroupArea[0].Area;
                else
                    Area = data[0].Area;
            }
            else
                Area = "-- ALL --";

            x = GetCell(rSubtitle, (cSubtitle + 1));
            sheet.Cells[x].Value = ": " + Area;
            sheet.Cells[x].Style.Font.Size = 11;

            x = GetCell((rSubtitle + 1), cSubtitle);
            sheet.Cells[x].Value = "Dealer ";
            sheet.Cells[x].Style.Font.Size = 11;

            string CompanyCode = "", BranchCode = "", CompanyName = "";
            var Company = (dynamic) null;
            if (!param.CompanyCodeParam.Equals(""))
            {
                CompanyCode = param.CompanyCodeParam;
                BranchCode = param.BranchCodeParam;
                if (BranchCode.Equals(""))
                    Company = ctx.CoProfiles.Where(a => a.CompanyCode == CompanyCode).ToList();
                else
                    Company = ctx.CoProfiles.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode).ToList();

                if (Company.Count > 0)
                    CompanyName = Company[0].CompanyGovName;
                else 
                    CompanyName = data[0].CompanyName;
            }
            else
                CompanyName = "-- ALL --";

            x = GetCell((rSubtitle + 1), (cSubtitle + 1));
            sheet.Cells[x].Value = ": " + CompanyName;
            sheet.Cells[x].Style.Font.Size = 11;

            x = GetCell((rSubtitle + 2), cSubtitle);
            sheet.Cells[x].Value = "Outlet ";
            sheet.Cells[x].Style.Font.Size = 11;

            string OutletName = "";
            if (!param.BranchCodeParam.Equals(""))
            {
                CompanyCode = param.CompanyCodeParam;
                BranchCode = param.BranchCodeParam;
                var Outlet = ctx.GnMstDealerOutletMappings.Where(a => a.DealerCode == CompanyCode && a.OutletCode == BranchCode).ToList();

                if (Outlet.Count > 0)
                    OutletName = Outlet[0].OutletName;
                else
                    OutletName = data[0].OutletName;
            }
            else
                OutletName = "-- ALL --";

            x = GetCell((rSubtitle + 2), (cSubtitle + 1));
            sheet.Cells[x].Value = ": " + OutletName;
            sheet.Cells[x].Style.Font.Size = 11;

            x = GetCell((rSubtitle + 3), cSubtitle);
            sheet.Cells[x].Value = "Year ";
            sheet.Cells[x].Style.Font.Size = 11;

            x = GetCell((rSubtitle + 3), (cSubtitle + 1));
            sheet.Cells[x].Value = ": " + param.Year;
            sheet.Cells[x].Style.Font.Size = 11;
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            x = GetCell((rSubtitle + 4), cSubtitle);
            sheet.Cells[x].Value = "Month ";
            sheet.Cells[x].Style.Font.Size = 11;

            x = GetCell((rSubtitle + 4), (cSubtitle + 1));
            sheet.Cells[x].Value = ": " + DateTimeFormatInfo.CurrentInfo.GetMonthName(param.Month);
            sheet.Cells[x].Style.Font.Size = 11;

            //TABLE HEADER
            int colStart = 1, maxDateOfMonth = DateTime.DaysInMonth(param.Year, param.Month);
            var headers = new List<ExcelCellItem>();
            headers.Add(new ExcelCellItem { Column = colStart++, Width = GetTrueColWidth(5.56), Value = "No" });
            headers.Add(new ExcelCellItem { Column = colStart++, Width = w1600, Value = "Kode Dealer" });
            headers.Add(new ExcelCellItem { Column = colStart++, Width = w3000, Value = "Nama Dealer" });
            headers.Add(new ExcelCellItem { Column = colStart++, Width = w1600, Value = DateTimeFormatInfo.CurrentInfo.GetMonthName(param.Month) + " / " + param.Year });
            for (int i = 1; i <= maxDateOfMonth; i++)
            {
                headers.Add(new ExcelCellItem { Column = colStart++, Width = w1143, Value = "" });
            }
            headers.Add(new ExcelCellItem { Column = colStart-1, Width = w1900, Value = "Total Pengiriman" });

            z = GetRange(rTitle, cStart, rTitle, ((cStart + 3) + maxDateOfMonth));
            sheet.Cells[z].Merge = true;

            sheet.Cells[GetRange(rHdrFirst, 1, (rHdrFirst + 1), 1)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, (cStart + 1), (rHdrFirst + 1), (cStart + 1))].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, (cStart + 2), (rHdrFirst + 1), (cStart + 2))].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, (cStart + 3), rHdrFirst, ((cStart + 2) + maxDateOfMonth))].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, (colStart-1), (rHdrFirst + 1), (colStart-1))].Merge = true;

            colStart = 1;
            var headers2 = new List<ExcelCellItem>();
            headers2.Add(new ExcelCellItem { Column = colStart++, Width = w1600, Value = "" });
            headers2.Add(new ExcelCellItem { Column = colStart++, Width = w1600, Value = "" });
            headers2.Add(new ExcelCellItem { Column = colStart++, Width = w3000, Value = "" });
            for (int i = 1; i <= maxDateOfMonth; i++)
            {
                headers2.Add(new ExcelCellItem { Column = colStart++, Width = w1143, Value = i });
            }
            headers2.Add(new ExcelCellItem { Column = colStart++, Width = w1900, Value = "" });

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            foreach (var header in headers2)
            {
                x = GetCell(rHdrFirst + 1, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            #endregion
            
            #region -- Data --
            int tempRow = 1, TotalPengiriman = 0, idx = 0;
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var items = new List<ExcelCellItem>();
                var row = 0;
                colStart = 1;
                if (i != 0)
                {
                    if (data[i].CompanyCode == data[i - 1].CompanyCode && data[i].BranchCode == data[i - 1].BranchCode && data[i].DayOfVOR != data[i-1].DayOfVOR)
                    {
                        TotalPengiriman += data[i].TotalPengiriman;
                        row = rDataStart + i - tempRow;
                        for (int j = 1; j <= maxDateOfMonth; j++)
                        {
                            if (j == data[i].DayOfVOR)
                            {
                                items.Add(new ExcelCellItem { Column = (j + 3), Value = data[i].DateSPK, Format = "dd-MMM-yyyy" });
                            }
                        }
                        idx++;
                        tempRow++;
                        items.Add(new ExcelCellItem { Column = (maxDateOfMonth + 4), Width = w1900, Value = TotalPengiriman });
                    }
                    else
                    {
                        TotalPengiriman = data[i].TotalPengiriman;
                        tempRow = 1;
                        row = rDataStart + i;
                        items.Add(new ExcelCellItem { Column = colStart++, Value = (i + 1 - idx) });
                        items.Add(new ExcelCellItem { Column = colStart++, Value = data[i].BranchCode });
                        items.Add(new ExcelCellItem { Column = colStart++, Value = data[i].OutletName });
                        for (int j = 1; j <= maxDateOfMonth; j++)
                        {
                            if (j == data[i].DayOfVOR)
                            {
                                items.Add(new ExcelCellItem { Column = colStart++, Value = data[i].DateSPK, Format = "dd-MMM-yyyy" });
                            }
                            else
                            {
                                items.Add(new ExcelCellItem { Column = colStart++, Value = "" });
                            }
                        }
                        items.Add(new ExcelCellItem { Column = colStart, Width = w1900, Value = TotalPengiriman });
                    }
                }
                else
                {
                    TotalPengiriman = data[i].TotalPengiriman;
                    tempRow = 1;
                    row = rDataStart + i;
                    items.Add(new ExcelCellItem { Column = colStart++, Value = (i + 1) });
                    items.Add(new ExcelCellItem { Column = colStart++, Value = data[i].BranchCode });
                    items.Add(new ExcelCellItem { Column = colStart++, Value = data[i].OutletName });
                    for (int j = 1; j <= maxDateOfMonth; j++)
                    {
                        if (j == data[i].DayOfVOR)
                        {
                            items.Add(new ExcelCellItem { Column = colStart++, Value = data[i].DateSPK, Format = "dd-MMM-yyyy" });
                        }
                        else
                        {
                            items.Add(new ExcelCellItem { Column = colStart++, Value = "" });
                        }
                    }
                    items.Add(new ExcelCellItem { Column = colStart, Width = w1900, Value = TotalPengiriman });
                }

                foreach (var item in items)
                {
                    x = GetCell(rDataStart + (i - idx), item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.WrapText = true;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                    sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
            }

            #endregion

            return package;
        }

        /*total per bulan*/
        private static ExcelPackage GenerateExcelVORConsistencyV2(ExcelPackage package, List<VORConsistencyReportV2> data, dynamic param)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            string z = "", x = "";
            const int
                rTitle = 1,
                rSubtitle = 3,
                rHdrFirst = 9,
                rDataStart = 11,

                cStart = 1,
                cSubtitle = 1;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w1900 = GetTrueColWidth(19.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00);
            int
                rFooter = rDataStart + 1;

            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            #region -- Title & Header --

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "VEHICLE OFF THE ROAD";
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[x].Style.Font.Size = 16;

            x = GetCell(rSubtitle, cSubtitle);
            sheet.Cells[x].Value = "Area ";
            sheet.Cells[x].Style.Font.Size = 11;

            string GroupNo = "", Area = "";
            if (!param.GroupAreasParam.Equals(""))
            {
                GroupNo = param.GroupAreasParam;
                var GroupArea = ctx.SrvGroupAreas.Where(a => a.GroupNo == GroupNo).Select(a => new { Area = a.AreaDealer }).ToList();
                if (GroupArea.Count > 0)
                    Area = GroupArea[0].Area;
                else
                    Area = data[0].Area;
            }
            else
                Area = "-- ALL --";

            x = GetCell(rSubtitle, (cSubtitle + 1));
            sheet.Cells[x].Value = ": " + Area;
            sheet.Cells[x].Style.Font.Size = 11;

            x = GetCell((rSubtitle + 1), cSubtitle);
            sheet.Cells[x].Value = "Dealer ";
            sheet.Cells[x].Style.Font.Size = 11;

            string CompanyCode = "", BranchCode = "", CompanyName = "";
            var Company = (dynamic)null;
            if (!param.CompanyCodeParam.Equals(""))
            {
                CompanyCode = param.CompanyCodeParam;
                BranchCode = param.BranchCodeParam;
                if (BranchCode.Equals(""))
                    Company = ctx.CoProfiles.Select(a => new { CompanyCode = a.CompanyCode, CompanyName = a.CompanyGovName }).Where(a => a.CompanyCode == CompanyCode).ToList();
                else
                    Company = ctx.Database.SqlQuery<DealerList>("exec uspfn_SrvDealerList @DealerType=@p0, @LinkedModule=@p1, @GroupArea=@p2", null, null, GroupNo).Where(p => p.CompanyCode == CompanyCode).ToList();

                if (Company.Count > 0)
                    CompanyName = Company[0].CompanyName;
                else
                    CompanyName = data[0].CompanyName;
            }
            else
                CompanyName = "-- ALL --";

            x = GetCell((rSubtitle + 1), (cSubtitle + 1));
            sheet.Cells[x].Value = ": " + CompanyName;
            sheet.Cells[x].Style.Font.Size = 11;

            x = GetCell((rSubtitle + 2), cSubtitle);
            sheet.Cells[x].Value = "Outlet ";
            sheet.Cells[x].Style.Font.Size = 11;

            string OutletName = "";
            if (!param.BranchCodeParam.Equals(""))
            {
                CompanyCode = param.CompanyCodeParam;
                BranchCode = param.BranchCodeParam;
                var Outlet = ctx.Database.SqlQuery<DealerList>("exec uspfn_SrvBranchList @CompanyCode=@p0, @GroupArea=@p1", CompanyCode, GroupNo).Where(a => a.CompanyCode == BranchCode).ToList();

                if (Outlet.Count > 0)
                    OutletName = Outlet[0].CompanyName;
                else
                    OutletName = data[0].OutletName;
            }
            else
                OutletName = "-- ALL --";

            x = GetCell((rSubtitle + 2), (cSubtitle + 1));
            sheet.Cells[x].Value = ": " + OutletName;
            sheet.Cells[x].Style.Font.Size = 11;

            x = GetCell((rSubtitle + 3), cSubtitle);
            sheet.Cells[x].Value = "Year ";
            sheet.Cells[x].Style.Font.Size = 11;

            x = GetCell((rSubtitle + 3), (cSubtitle + 1));
            sheet.Cells[x].Value = ": " + param.Year;
            sheet.Cells[x].Style.Font.Size = 11;
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            x = GetCell((rSubtitle + 4), cSubtitle);
            sheet.Cells[x].Value = "Month ";
            sheet.Cells[x].Style.Font.Size = 11;

            x = GetCell((rSubtitle + 4), (cSubtitle + 1));
            sheet.Cells[x].Value = ": " + DateTimeFormatInfo.CurrentInfo.GetMonthName(param.Month);
            sheet.Cells[x].Style.Font.Size = 11;

            //TABLE HEADER
            int colStart = 1, maxDateOfMonth = DateTime.DaysInMonth(param.Year, param.Month);
            var headers = new List<ExcelCellItem>();
            headers.Add(new ExcelCellItem { Column = colStart++, Width = GetTrueColWidth(5.56), Value = "No" });
            headers.Add(new ExcelCellItem { Column = colStart++, Width = w1600, Value = "Kode Dealer" });
            headers.Add(new ExcelCellItem { Column = colStart++, Width = w3000, Value = "Nama Dealer" });
            headers.Add(new ExcelCellItem { Column = colStart++, Width = w1600, Value = DateTimeFormatInfo.CurrentInfo.GetMonthName(param.Month) + " / " + param.Year });
            for (int i = 1; i <= maxDateOfMonth; i++)
            {
                headers.Add(new ExcelCellItem { Column = colStart++, Width = w1143, Value = "" });
            }
            headers.Add(new ExcelCellItem { Column = colStart - 1, Width = w1900, Value = "Total Pengiriman" });

            z = GetRange(rTitle, cStart, rTitle, ((cStart + 3) + maxDateOfMonth));
            sheet.Cells[z].Merge = true;

            sheet.Cells[GetRange(rHdrFirst, 1, (rHdrFirst + 1), 1)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, (cStart + 1), (rHdrFirst + 1), (cStart + 1))].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, (cStart + 2), (rHdrFirst + 1), (cStart + 2))].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, (cStart + 3), rHdrFirst, ((cStart + 2) + maxDateOfMonth))].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, (colStart - 1), (rHdrFirst + 1), (colStart - 1))].Merge = true;

            colStart = 1;
            var headers2 = new List<ExcelCellItem>();
            headers2.Add(new ExcelCellItem { Column = colStart++, Width = w1600, Value = "" });
            headers2.Add(new ExcelCellItem { Column = colStart++, Width = w1600, Value = "" });
            headers2.Add(new ExcelCellItem { Column = colStart++, Width = w3000, Value = "" });
            for (int i = 1; i <= maxDateOfMonth; i++)
            {
                headers2.Add(new ExcelCellItem { Column = colStart++, Width = w1143, Value = i });
            }
            headers2.Add(new ExcelCellItem { Column = colStart++, Width = w1900, Value = "" });

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            foreach (var header in headers2)
            {
                x = GetCell(rHdrFirst + 1, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            #endregion

            #region -- Data --
            int tempRow = 1, TotalPengiriman = 0, idx = 0;
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var items = new List<ExcelCellItem>();
                var row = 0;
                colStart = 1;

                TotalPengiriman = data[i].TotalPengiriman;
                if (i != 0)
                {
                    if (data[i].CompanyCode == data[i - 1].CompanyCode && data[i].BranchCode == data[i - 1].BranchCode && data[i].DayOfVOR != data[i - 1].DayOfVOR)
                    {
                        row = rDataStart + i - tempRow;
                        for (int j = 1; j <= maxDateOfMonth; j++)
                        {
                            if (j == data[i].DayOfVOR)
                            {
                                if (data[i].NoVOR == 0) //check ada VOR atau tidak
                                {
                                    items.Add(new ExcelCellItem { Column = (j + 3), Value = data[i].TransactionDay });                                    
                                }
                                else
                                {
                                    items.Add(new ExcelCellItem { Column = (j + 3), Value = "X" });
                                }
                            }
                        }
                        idx++;
                        tempRow++;
                        items.Add(new ExcelCellItem { Column = (maxDateOfMonth + 4), Width = w1900, Value = TotalPengiriman });
                    }
                    else
                    {
                        tempRow = 1;
                        row = rDataStart + i;
                        items.Add(new ExcelCellItem { Column = colStart++, Value = (i + 1 - idx) });
                        items.Add(new ExcelCellItem { Column = colStart++, Value = data[i].BranchCode });
                        items.Add(new ExcelCellItem { Column = colStart++, Value = data[i].OutletName });
                        for (int j = 1; j <= maxDateOfMonth; j++)
                        {
                            if (j == data[i].DayOfVOR)
                            {
                                if (data[i].NoVOR == 0) //check ada VOR atau tidak
                                {
                                    items.Add(new ExcelCellItem { Column = colStart++, Value = data[i].TransactionDay });
                                }
                                else
                                {
                                    items.Add(new ExcelCellItem { Column = colStart++, Value = "X" });
                                }
                            }
                            else
                            {
                                items.Add(new ExcelCellItem { Column = colStart++, Value = "" });
                            }
                        }
                        items.Add(new ExcelCellItem { Column = colStart, Width = w1900, Value = TotalPengiriman });
                        rFooter++;
                    }
                }
                else
                {
                    tempRow = 1;
                    row = rDataStart + i;
                    items.Add(new ExcelCellItem { Column = colStart++, Value = (i + 1) });
                    items.Add(new ExcelCellItem { Column = colStart++, Value = data[i].BranchCode });
                    items.Add(new ExcelCellItem { Column = colStart++, Value = data[i].OutletName });
                    for (int j = 1; j <= maxDateOfMonth; j++)
                    {
                        if (j == data[i].DayOfVOR)
                        {
                            items.Add(new ExcelCellItem { Column = colStart++, Value = data[i].TransactionDay });
                        }
                        else
                        {
                            items.Add(new ExcelCellItem { Column = colStart++, Value = "" });
                        }
                    }
                    items.Add(new ExcelCellItem { Column = colStart, Width = w1900, Value = TotalPengiriman });
                    rFooter++;
                }

                foreach (var item in items)
                {
                    x = GetCell(rDataStart + (i - idx), item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.WrapText = true;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                    sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
            }

            #endregion

            #region -- Footer --
            x = GetCell(rFooter, cStart);
            sheet.Cells[x].Value = "Nb : X => Tidak ada VOR";
            sheet.Cells[x].Style.Font.Size = 9;
            #endregion

            return package;
        }

        private static ExcelPackage GenerateExcelCsBirthMonitoring(ExcelPackage package, List<CustomerBirthdayMonitoring> dataMonthly, List<CustomerBirthdayMonitoring> dataWeekly)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 1,
                rDataStart = 2,

                cStart = 1,
                cSubtitle = 1,
                cTitleVal = 1;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w1900 = GetTrueColWidth(19.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00);
            #endregion
            var sheet = package.Workbook.Worksheets.Add("Customer Birthday Monitoring - 1");
            //var sheet1 = package.Workbook.Worksheets.Add("Customer Birthday Monitoring - 2");

            #region -- Title & Header --
            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w1086, Value = "No" },
                new ExcelCellItem { Column = 2, Width = w1800, Value = "Periode" },
                new ExcelCellItem { Column = 3, Width = w1800, Value = "Jumlah Customer" },
                new ExcelCellItem { Column = 4, Width = w1900, Value = "Input by CRO" },
                new ExcelCellItem { Column = 5, Width = w1243, Value = "Gift" },
                new ExcelCellItem { Column = 6, Width = w1243, Value = "SMS" },
                new ExcelCellItem { Column = 7, Width = w1600, Value = "Telephone" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            /*
            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet1.Cells[x].Value = header.Value;
                sheet1.Column(header.Column).Width = header.Width;
                sheet1.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet1.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet1.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet1.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet1.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            */
            #endregion

            #region -- DATA --

            // MONTHLY
            if (dataMonthly.Count() == 0) return package;
            int row = 0, weekly = 1, totalCustomer = 0, totalReminder = 0, totalGift = 0, totalSMS = 0, totalTelephone = 0;
            for (int i = 0; i < dataMonthly.Count(); i++)
            {
                totalCustomer += dataMonthly[i].TotalCustomer;
                totalReminder += dataMonthly[i].Reminder;
                totalGift += dataMonthly[i].Gift;
                totalSMS += dataMonthly[i].SMS;
                totalTelephone += dataMonthly[i].Telephone;
                row = rDataStart + i;
                if (i != 0)
                {
                    if (dataMonthly[i].Month != dataMonthly[i-1].Month)
                    {
                        weekly = 1;
                    }
                }

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = (i+1) },
                    new ExcelCellItem { Column = 2, Value = DateTimeFormatInfo.CurrentInfo.GetMonthName(dataMonthly[i].Month) + " W " + weekly },
                    new ExcelCellItem { Column = 3, Value = dataMonthly[i].TotalCustomer },
                    new ExcelCellItem { Column = 4, Value = dataMonthly[i].Reminder },
                    new ExcelCellItem { Column = 5, Value = dataMonthly[i].Gift },
                    new ExcelCellItem { Column = 6, Value = dataMonthly[i].SMS },
                    new ExcelCellItem { Column = 7, Value = dataMonthly[i].Telephone },
                };

                weekly++;

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            // Footer
            row += 1;
            x = GetCell(row, 1);
            sheet.Cells[GetRange(row, 1, row, 2)].Merge = true;
            sheet.Cells[x].Value = "Total";
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            x = GetCell(row, 2);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            x = GetCell(row, 3);
            sheet.Cells[x].Value = totalCustomer;
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            x = GetCell(row, 4);
            sheet.Cells[x].Value = totalReminder;
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            x = GetCell(row, 5);
            sheet.Cells[x].Value = totalGift;
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            x = GetCell(row, 6);
            sheet.Cells[x].Value = totalSMS;
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            x = GetCell(row, 7);
            sheet.Cells[x].Value = totalTelephone;
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // WEEKLY
            /*
            if (dataWeekly.Count() == 0) return package;
            totalCustomer = 0;
            totalReminder = 0;
            totalGift = 0;
            totalSMS = 0;
            totalTelephone = 0;
            for (int i = 0; i < dataWeekly.Count(); i++)
            {
                totalCustomer += dataMonthly[i].TotalCustomer;
                totalReminder += dataMonthly[i].Reminder;
                totalGift += dataMonthly[i].Gift;
                totalSMS += dataMonthly[i].SMS;
                totalTelephone += dataMonthly[i].Telephone;
                row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = (i+1) },
                    new ExcelCellItem { Column = 2, Value = " Week - " + dataWeekly[i].Periode },
                    new ExcelCellItem { Column = 3, Value = dataWeekly[i].TotalCustomer },
                    new ExcelCellItem { Column = 4, Value = dataWeekly[i].Reminder },
                    new ExcelCellItem { Column = 5, Value = dataWeekly[i].Gift },
                    new ExcelCellItem { Column = 6, Value = dataWeekly[i].SMS },
                    new ExcelCellItem { Column = 7, Value = dataWeekly[i].Telephone },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet1.Cells[x].Value = item.Value;
                    sheet1.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    sheet1.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    if (item.Format != null) sheet1.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            // Footer
            row += 1;
            x = GetCell(row, 1);
            sheet1.Cells[GetRange(row, 1, row, 2)].Merge = true;
            sheet1.Cells[x].Value = "Total";
            sheet1.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet1.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            x = GetCell(row, 2);
            sheet1.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            x = GetCell(row, 3);
            sheet1.Cells[x].Value = totalCustomer;
            sheet1.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet1.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            x = GetCell(row, 4);
            sheet1.Cells[x].Value = totalReminder;
            sheet1.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet1.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            x = GetCell(row, 5);
            sheet1.Cells[x].Value = totalGift;
            sheet1.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet1.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            x = GetCell(row, 6);
            sheet1.Cells[x].Value = totalSMS;
            sheet1.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet1.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            x = GetCell(row, 7);
            sheet1.Cells[x].Value = totalTelephone;
            sheet1.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet1.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            */
            #endregion

            return package;
        }

        private static ExcelPackage GenerateExcelDRH(ExcelPackage package, DataTable data, dynamic param)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            string z = "", x = "";
            const int
                rTitle = 1,
                rSubtitle = 3,
                rHdrFirst = 6,
                rDataStart = 8,

                cStart = 1,
                cSubtitle = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w1900 = GetTrueColWidth(19.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00);
            #endregion
            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Data Retensi Harian";
            sheet.Cells[x].Style.Font.Size = 20;

            string CompanyCode = "", BranchCode = "", CompanyName = "";
            var Company = (dynamic)null;
            if (!param.CompanyCodeParam.Equals(""))
            {
                CompanyCode = param.CompanyCodeParam;
                BranchCode = param.BranchCodeParam;
                if (BranchCode.Equals(""))
                    Company = ctx.CoProfiles.Where(a => a.CompanyCode == CompanyCode).ToList();
                else
                    Company = ctx.CoProfiles.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode).ToList();

                if (Company.Count > 0)
                    CompanyName = Company[0].CompanyGovName;
                //else
                //    CompanyName = data[0].CompanyName;
            }
            else
                CompanyName = "-- ALL --";

            string OutletName = "";
            if (!param.BranchCodeParam.Equals(""))
            {
                CompanyCode = param.CompanyCodeParam;
                BranchCode = param.BranchCodeParam;
                var Outlet = ctx.GnMstDealerOutletMappings.Where(a => a.DealerCode == CompanyCode && a.OutletCode == BranchCode).ToList();

                if (Outlet.Count > 0)
                    OutletName = Outlet[0].OutletName;
                //else
                //    OutletName = data[0].OutletName;
            }
            else
                OutletName = "-- ALL --";

            x = GetCell(rSubtitle, cSubtitle);
            sheet.Cells[x].Value = "Nama Dealer : " + CompanyName;
            sheet.Cells[x].Style.Font.Size = 12;

            x = GetCell((rSubtitle + 1), cSubtitle);
            sheet.Cells[x].Value = "Nama Outlet : " + OutletName;
            sheet.Cells[x].Style.Font.Size = 12;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w1086, Value = "" }, // Kode Dealer
                new ExcelCellItem { Column = 2, Width = w3000, Value = "" }, // Nama Dealer
                new ExcelCellItem { Column = 3, Width = w1086, Value = "" }, // Kode Outlet
                new ExcelCellItem { Column = 4, Width = w3000, Value = "" }, // Nama Outlet
                new ExcelCellItem { Column = 5, Width = w1086, Value = "" }, // Inisial
                new ExcelCellItem { Column = 6, Width = w1086, Value = "" }, // Type
                new ExcelCellItem { Column = 7, Width = w1086, Value = "" }, // No Polisi
                new ExcelCellItem { Column = 8, Width = w1086, Value = "" }, // TM
                new ExcelCellItem { Column = 9, Width = w1086, Value = "" }, // Tahun
                new ExcelCellItem { Column = 10, Width = w1086, Value = "" }, // Kode Mesin
                new ExcelCellItem { Column = 11, Width = w1086, Value = "" }, // No Mesin
                new ExcelCellItem { Column = 12, Width = w1800, Value = "" }, // Kode Rangka
                new ExcelCellItem { Column = 13, Width = w1086, Value = "" }, // No Rangka
                new ExcelCellItem { Column = 14, Width = w1800, Value = "" }, // Nama Pelanggan
                new ExcelCellItem { Column = 15, Width = w3000, Value = "" }, // Alamat Pelanggan
                new ExcelCellItem { Column = 16, Width = w1600, Value = "" }, // Telpon Rumah
                new ExcelCellItem { Column = 17, Width = w1800, Value = "" }, // Additional Phone 1
                new ExcelCellItem { Column = 18, Width = w1800, Value = "" }, // Additional Phone 2
                new ExcelCellItem { Column = 19, Width = w1600, Value = "" }, // Telpon Kantor
                new ExcelCellItem { Column = 20, Width = w1900, Value = "" }, // Tanggal Kunjungan
                new ExcelCellItem { Column = 21, Width = w1600, Value = "" }, // HP
                new ExcelCellItem { Column = 22, Width = w1086, Value = "" }, // RM
                new ExcelCellItem { Column = 23, Width = w1086, Value = "" }, // P/M Saat Ini
                new ExcelCellItem { Column = 24, Width = w1086, Value = "" }, // P/M Berikut
                new ExcelCellItem { Column = 25, Width = w1900, Value = "Reminder" }, // Estimasi Berikut
                new ExcelCellItem { Column = 26, Width = w1900, Value = "" }, // Tgl.Reminder
                new ExcelCellItem { Column = 27, Width = w1086, Value = "" }, // Berhasil Dihubungi
                new ExcelCellItem { Column = 28, Width = w1086, Value = "" }, // Booking
                new ExcelCellItem { Column = 29, Width = w1900, Value = "" }, // Tgl. Booking
                new ExcelCellItem { Column = 30, Width = w1086, Value = "Follow Up" }, // Konsumen Datang
                new ExcelCellItem { Column = 31, Width = w1900, Value = "" }, // Tgl. Follow Up
                new ExcelCellItem { Column = 32, Width = w1086, Value = "" }, // Puas / Tidak
                new ExcelCellItem { Column = 33, Width = w2000, Value = "" }, // Alasan
                new ExcelCellItem { Column = 34, Width = w1086, Value = "Kontak" }, // Nama Kontak
                new ExcelCellItem { Column = 35, Width = w3000, Value = "" }, // Alamat Kontak
                new ExcelCellItem { Column = 36, Width = w1900, Value = "" }, // No Telp Kontak
                new ExcelCellItem { Column = 37, Width = w2000, Value = "" }, // Nama SA
                new ExcelCellItem { Column = 38, Width = w2000, Value = "" }, // Nama Mekanik
                new ExcelCellItem { Column = 39, Width = w3000, Value = "" }, // Permintaan Perawatan
                new ExcelCellItem { Column = 40, Width = w3000, Value = "" }, // Rekomendasi
            };

            var headers2 = new List<ExcelCellItem>();
            for (int i = 0; i < data.Columns.Count; i++)
            {
                headers2.Add(new ExcelCellItem { Column = (i + 1), Value = data.Columns[i].Caption.ToString() });
            }

            sheet.Cells[GetRange(rHdrFirst, 25, rHdrFirst, 29)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 30, rHdrFirst, 33)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 34, rHdrFirst, 36)].Merge = true;

            sheet.Cells[GetRange(rHdrFirst, 25, rHdrFirst, 29)].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[GetRange(rHdrFirst, 30, rHdrFirst, 33)].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[GetRange(rHdrFirst, 34, rHdrFirst, 36)].Style.Border.BorderAround(ExcelBorderStyle.Medium);

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.WrapText = true;
                if (header.Value.ToString() != "")
                {
                    //sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                    sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                }
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            foreach (var header in headers2)
            {
                x = GetCell((rHdrFirst + 1), header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            #endregion


            #region -- Data --

            if (data.Rows.Count == 0) return package;
            for (int i = 0; i < data.Rows.Count; i++)
            {
                var row = rDataStart + i;

                var items = new List<ExcelCellItem>();
                for (int j = 0; j < data.Columns.Count; j++)
                {
                    if (j == 19 || j == 24 || j == 25 || j == 28 || j == 30)
                        items.Add(new ExcelCellItem { Column = (j + 1), Value = data.Rows[i][data.Columns[j].Caption], Format = "dd-MMM-YYYY" });
                    else
                        items.Add(new ExcelCellItem { Column = (j + 1), Value = data.Rows[i][data.Columns[j].Caption] });
                }

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.WrapText = true;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                    sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
            }

            #endregion
            return package;
        }

        private static ExcelPackage GenerateExcelHistJobDelayVOR(ExcelPackage package, List<HistJobDelayVOR> data, dynamic param)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            string z = "", x = "";
            const int
                rTitle = 1,
                rSubtitle = 3,
                rHdrFirst = 6,
                rDataStart = 7,

                cStart = 1,
                cSubtitle = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w1900 = GetTrueColWidth(19.00),
                w3000 = GetTrueColWidth(30.00),
                w4000 = GetTrueColWidth(40.00);
            #endregion
            var sheet = package.Workbook.Worksheets.Add("book1");


            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report History Job Delay";
            sheet.Cells[x].Style.Font.Size = 20;

            string CompanyCode = "", BranchCode = "", CompanyName = "";
            var Company = (dynamic)null;
            if (!param.CompanyCodeParam.Equals(""))
            {
                CompanyCode = param.CompanyCodeParam;
                BranchCode = param.BranchCodeParam;
                if (BranchCode.Equals(""))
                    Company = ctx.CoProfiles.Where(a => a.CompanyCode == CompanyCode).ToList();
                else
                    Company = ctx.CoProfiles.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode).ToList();

                if (Company.Count > 0)
                    CompanyName = Company[0].CompanyGovName;
            }
            else
                CompanyName = "-- ALL --";

            string OutletName = "";
            if (!param.BranchCodeParam.Equals(""))
            {
                CompanyCode = param.CompanyCodeParam;
                BranchCode = param.BranchCodeParam;
                var Outlet = ctx.GnMstDealerOutletMappings.Where(a => a.DealerCode == CompanyCode && a.OutletCode == BranchCode).ToList();

                if (Outlet.Count > 0)
                    OutletName = Outlet[0].OutletName;
            }
            else
                OutletName = "-- ALL --";

            x = GetCell(rSubtitle, cSubtitle);
            sheet.Cells[x].Value = "Nama Dealer : " + CompanyName;
            sheet.Cells[x].Style.Font.Size = 12;

            x = GetCell((rSubtitle + 1), cSubtitle);
            sheet.Cells[x].Value = "Nama Outlet : " + OutletName;
            sheet.Cells[x].Style.Font.Size = 12;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w1086, Value = "No VOR" },
                new ExcelCellItem { Column = 2, Width = w1086, Value = "Kode Dealer" },
                new ExcelCellItem { Column = 3, Width = w4000, Value = "Nama Dealer" },
                new ExcelCellItem { Column = 4, Width = w1800, Value = "Region" },
                new ExcelCellItem { Column = 5, Width = w1086, Value = "No Polisi" },
                new ExcelCellItem { Column = 6, Width = w1600, Value = "Tgl Masuk" },
                new ExcelCellItem { Column = 7, Width = w1086, Value = "Jam Masuk" },
                new ExcelCellItem { Column = 8, Width = w1600, Value = "Tgl Selesai" },
                new ExcelCellItem { Column = 9, Width = w1086, Value = "Status" },
                new ExcelCellItem { Column = 10, Width = w3000, Value = "Penyebab Pekerjaan Tertunda-1" },
                new ExcelCellItem { Column = 11, Width = w1600, Value = "Date" },
                new ExcelCellItem { Column = 12, Width = w3000, Value = "Penyebab Pekerjaan Tertunda-2" },
                new ExcelCellItem { Column = 13, Width = w1600, Value = "Date" },
                new ExcelCellItem { Column = 14, Width = w3000, Value = "Penyebab Pekerjaan Tertunda-3" },
                new ExcelCellItem { Column = 15, Width = w1600, Value = "Date" },
                new ExcelCellItem { Column = 16, Width = w3000, Value = "Penyebab Pekerjaan Tertunda-4" },
                new ExcelCellItem { Column = 17, Width = w1600, Value = "Date" },
                new ExcelCellItem { Column = 18, Width = w3000, Value = "Penyebab Pekerjaan Tertunda-5" },
                new ExcelCellItem { Column = 19, Width = w1600, Value = "Date" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            #endregion

            #region -- Data --

            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].ServiceNo },
                    new ExcelCellItem { Column = 2, Value = data[i].DealerCode },
                    new ExcelCellItem { Column = 3, Value = data[i].DealerName },
                    new ExcelCellItem { Column = 4, Value = data[i].AreaDealer },
                    new ExcelCellItem { Column = 5, Value = data[i].PoliceRegNo },
                    new ExcelCellItem { Column = 6, Value = data[i].CreatedDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 7, Value = data[i].CreatedDate, Format = "hh:mm:ss" },
                    new ExcelCellItem { Column = 8, Value = data[i].ClosedDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 9, Value = data[i].Status },
                    new ExcelCellItem { Column = 10, Value = data[i].JobDelayDesc1 },
                    new ExcelCellItem { Column = 11, Value = data[i].LastUpdateDate1, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 12, Value = data[i].JobDelayDesc2 },
                    new ExcelCellItem { Column = 13, Value = data[i].LastUpdateDate2, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 14, Value = data[i].JobDelayDesc3 },
                    new ExcelCellItem { Column = 15, Value = data[i].LastUpdateDate3, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 16, Value = data[i].JobDelayDesc4 },
                    new ExcelCellItem { Column = 17, Value = data[i].LastUpdateDate4, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 18, Value = data[i].JobDelayDesc5 },
                    new ExcelCellItem { Column = 19, Value = data[i].LastUpdateDate5, Format = "dd-MMM-YYYY" },
                };


                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.WrapText = true;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                    sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
            }

            #endregion
            return package;
        }

        private static ExcelPackage GenerateExcelMappingSrvGn(ExcelPackage package, List<MappingSrvGn> data, dynamic param)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            string z = "", x = "";
            const int
                rTitle = 1,
                rSubtitle = 3,
                rHdrFirst = 7,
                rDataStart = 9,

                cStart = 1,
                cSubtitle = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w1900 = GetTrueColWidth(19.00),
                w3000 = GetTrueColWidth(30.00),
                w4000 = GetTrueColWidth(40.00);
            #endregion
            var sheet = package.Workbook.Worksheets.Add("Mapping Marketing to Service");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Mapping Marketing To Service";
            sheet.Cells[x].Style.Font.Size = 20;

            string GroupNo = "", CompanyCode = "", BranchCode = "", CompanyName = "", OutletName = "", GroupArea = "";
            if (!param.GroupNoParam.Equals(""))
            {
                GroupNo = param.GroupNoParam;
                var Area = ctx.GroupAreas.Where(a => a.GroupNo == GroupNo).ToList();

                if (Area.Count > 0)
                    GroupArea = Area[0].AreaDealer;
            }
            else
                GroupArea = "-- ALL --";

            if (!param.CompanyCodeParam.Equals(""))
            {
                CompanyCode = param.CompanyCodeParam;
                var Company = ctx.GnMstDealerMappings.Where(a => a.DealerCode == CompanyCode && a.GroupNo.ToString() == GroupNo).ToList();

                if (Company.Count > 0)
                    CompanyName = Company[0].DealerName;
            }
            else
                CompanyName = "-- ALL --";

            if (!param.BranchCodeParam.Equals(""))
            {
                BranchCode = param.BranchCodeParam;
                var Outlet = ctx.GnMstDealerOutletMappings.Where(a => a.DealerCode == CompanyCode && a.OutletCode == BranchCode && a.GroupNo.ToString() == GroupNo).ToList();

                if (Outlet.Count > 0)
                    OutletName = Outlet[0].OutletName;
            }
            else
                OutletName = "-- ALL --";

            x = GetCell(rSubtitle, cSubtitle);
            sheet.Cells[x].Value = "Area : " + GroupArea;
            sheet.Cells[x].Style.Font.Size = 12;

            x = GetCell((rSubtitle + 1), cSubtitle);
            sheet.Cells[x].Value = "Dealer : " + CompanyName;
            sheet.Cells[x].Style.Font.Size = 12;

            x = GetCell((rSubtitle + 2), cSubtitle);
            sheet.Cells[x].Value = "Outlet : " + OutletName;
            sheet.Cells[x].Style.Font.Size = 12;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w1243, Value = "Marketing" },
                new ExcelCellItem { Column = 2, Width = w1900, Value = "" },
                new ExcelCellItem { Column = 3, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 4, Width = w3000, Value = "" },
                new ExcelCellItem { Column = 5, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 6, Width = w3000, Value = "" },
                new ExcelCellItem { Column = 7, Width = w1243, Value = "Service" },
                new ExcelCellItem { Column = 8, Width = w1900, Value = "" },
                new ExcelCellItem { Column = 9, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 10, Width = w3000, Value = "" },
                new ExcelCellItem { Column = 11, Width = w1243, Value = "" },
                new ExcelCellItem { Column = 12, Width = w3000, Value = "" },
            };

            // Colspan
            sheet.Cells[GetRange(rHdrFirst, 1, rHdrFirst, 6)].Merge = true;
            sheet.Cells[GetRange(rHdrFirst, 7, rHdrFirst, 12)].Merge = true;

            var headers2 = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w1243, Value = "Group No" },
                new ExcelCellItem { Column = 2, Width = w1900, Value = "Group Area" },
                new ExcelCellItem { Column = 3, Width = w1243, Value = "Dealer Code" },
                new ExcelCellItem { Column = 4, Width = w3000, Value = "Dealer Name" },
                new ExcelCellItem { Column = 5, Width = w1243, Value = "Outlet Code" },
                new ExcelCellItem { Column = 6, Width = w3000, Value = "Outlet Name" },
                new ExcelCellItem { Column = 7, Width = w1243, Value = "Group No" },
                new ExcelCellItem { Column = 8, Width = w1900, Value = "Group Area" },
                new ExcelCellItem { Column = 9, Width = w1243, Value = "Dealer Code" },
                new ExcelCellItem { Column = 10, Width = w3000, Value = "Dealer Name" },
                new ExcelCellItem { Column = 11, Width = w1243, Value = "Outlet Code" },
                new ExcelCellItem { Column = 12, Width = w3000, Value = "Outlet Name" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            foreach (var header in headers2)
            {
                x = GetCell(rHdrFirst + 1, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            #endregion

            #region -- Data --

            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].GroupNoGn },
                    new ExcelCellItem { Column = 2, Value = data[i].GroupAreaGn },
                    new ExcelCellItem { Column = 3, Value = data[i].CompanyCodeGn },
                    new ExcelCellItem { Column = 4, Value = data[i].CompanyNameGn },
                    new ExcelCellItem { Column = 5, Value = data[i].BranchCodeGn },
                    new ExcelCellItem { Column = 6, Value = data[i].BranchNameGn },
                    new ExcelCellItem { Column = 7, Value = data[i].GroupNoSrv },
                    new ExcelCellItem { Column = 8, Value = data[i].GroupAreaSrv },
                    new ExcelCellItem { Column = 9, Value = data[i].CompanyCodeSrv },
                    new ExcelCellItem { Column = 10, Value = data[i].CompanyNameSrv },
                    new ExcelCellItem { Column = 11, Value = data[i].BranchCodeSrv },
                    new ExcelCellItem { Column = 12, Value = data[i].BranchNameSrv },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.WrapText = true;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                    // sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
            }

            #endregion

            return package;
        }

        #endregion Generate Excels Area

        public FileContentResult DownloadExcelFile(string key, string filename)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            //Response.AppendHeader("content-disposition", "attachment; filename=" + filename + "_" + string.Format("{0}", DateTime.Now.ToString("dd-MMM-yyy HH.mm")) + ".xlsx");
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + "_" + string.Format("{0}", DateTime.Now.ToString("dd-MMM-yyy_HH.mm")) + ".xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }

        internal static string GetColName(int number)
        {
            return Helpers.EPPlusHelper.ExcelColumnNameFromNumber(number);
        }

        internal static int GetColNumber(string name)
        {
            return Helpers.EPPlusHelper.ExcelColumnNameToNumber(name);
        }

        internal static string GetCell(int row, int col)
        {
            return GetColName(col) + row.ToString();
        }

        internal static string GetRange(int row1, int col1, int row2, int col2)
        {
            return GetCell(row1, col1) + ":" + GetCell(row2, col2);
        }

        internal class ExcelCellItem
        {
            public int Column { get; set; }
            public double Width { get; set; }
            public object Value { get; set; }
            public string Format { get; set; }
        }

        internal static double GetTrueColWidth(double width)
        {
            //DEDUCE WHAT THE COLUMN WIDTH WOULD REALLY GET SET TO
            double z = 1d;
            if (width >= (1 + 2 / 3))
            {
                z = Math.Round((Math.Round(7 * (width - 1 / 256), 0) - 5) / 7, 2);
            }
            else
            {
                z = Math.Round((Math.Round(12 * (width - 1 / 256), 0) - Math.Round(5 * width, 0)) / 12, 2);
            }

            //HOW FAR OFF? (WILL BE LESS THAN 1)
            double errorAmt = width - z;

            //CALCULATE WHAT AMOUNT TO TACK ONTO THE ORIGINAL AMOUNT TO RESULT IN THE CLOSEST POSSIBLE SETTING 
            double adj = 0d;
            if (width >= (1 + 2 / 3))
            {
                adj = (Math.Round(7 * errorAmt - 7 / 256, 0)) / 7;
            }
            else
            {
                adj = ((Math.Round(12 * errorAmt - 12 / 256, 0)) / 12) + (2 / 12);
            }

            //RETURN A SCALED-VALUE THAT SHOULD RESULT IN THE NEAREST POSSIBLE VALUE TO THE TRUE DESIRED SETTING
            if (z > 0)
            {
                return width + adj;
            }

            return 0d;
        }

    }
}
