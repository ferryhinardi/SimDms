using Newtonsoft.Json;
using SimDms.ConsoleApps.Models;
using SimDms.SUtility.Controllers;
using SimDms.SUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.ConsoleApps
{
    public class ProcessData
    {
        private DataContext ctx = new DataContext();
        private DataLogContext ctxLog = new DataLogContext();
        private DataDealerContext ctxDealer = new DataDealerContext();

        public void Process()
        {
            var list = ctxDealer.GnMstScheduleDatas.Where(p =>
                p.Status == "P" &&
                p.DataType == "EMPLY" &&
                p.Segment > 0).OrderBy(p => p.LastSendDate).Take(10).ToList();

            int i = 1;
            foreach (var data in list)
            {
                Console.Write("{0}  {1}   {2}", i.ToString().PadLeft(3, '0'), data.DataType, data.UniqueID);
                ProcessParsing(data.UniqueID);
                Console.WriteLine(" - Done");
                i++;
            }
        }

        public void ProcessLog()
        {
            var list = ctxDealer.GnMstScheduleDatas.Where(p =>
                p.Status == "P" &&
                p.DataType == "EMACH" &&
                p.Segment > 0).OrderBy(p => p.LastSendDate).Take(500).ToList();

            int i = 1;
            foreach (var data in list)
            {
                Console.Write("{0}  {1}   {2}", i.ToString().PadLeft(3, '0'), data.DataType, data.UniqueID);
                ProcessLog(data.UniqueID);
                Console.WriteLine(" - Done");
                i++;
            }
        }

        private void ProcessParsing(string pid)
        {
            var data = ctxDealer.GnMstScheduleDatas.Find(pid);
            if (data != null)
            {
                switch (data.DataType)
                {
                    case "EMPLY":
                        var lstem01 = JsonConvert.DeserializeObject<List<HrEmployee>>(data.Data);
                        foreach (var row in lstem01)
                        {
                            var model = ctx.HrEmployees.Find(row.CompanyCode, row.EmployeeID);
                            if (model == null) ctx.HrEmployees.Add(row);
                            else extend(model, row);
                        }
                        UpdateSchedule(pid);
                        break;
                    case "EMACH":
                        var lstem02 = JsonConvert.DeserializeObject<List<HrEmployeeAchievement>>(data.Data);
                        foreach (var row in lstem02)
                        {
                            var model = ctx.HrEmployeeAchievements.Find(row.CompanyCode, row.EmployeeID, row.AssignDate);
                            if (model == null) ctx.HrEmployeeAchievements.Add(row);
                            else extend(model, row);
                        }
                        UpdateSchedule(pid);
                        break;
                    case "EMUTA":
                        var lstem03 = JsonConvert.DeserializeObject<List<HrEmployeeMutation>>(data.Data);
                        foreach (var row in lstem03)
                        {
                            var model = ctx.HrEmployeeMutations.Find(row.CompanyCode, row.EmployeeID, row.MutationDate);
                            if (model == null) ctx.HrEmployeeMutations.Add(row);
                            else extend(model, row);
                        }
                        UpdateSchedule(pid);
                        break;
                    case "EMSFM":
                        var lstem04 = JsonConvert.DeserializeObject<List<HrEmployeeSales>>(data.Data);
                        foreach (var row in lstem04)
                        {
                            var model = ctx.HrEmployeeSales.Find(row.CompanyCode, row.EmployeeID);
                            if (model == null) ctx.HrEmployeeSales.Add(row);
                            else extend(model, row);
                        }
                        UpdateSchedule(pid);
                        break;

                    case "SVSPK":
                        var lstsv01 = JsonConvert.DeserializeObject<List<SvTrnService>>(data.Data);
                        foreach (var row in lstsv01)
                        {
                            var model = ctx.SvTrnServices.Find(row.CompanyCode, row.BranchCode, row.ProductType, row.ServiceNo);
                            if (model == null) ctx.SvTrnServices.Add(row);
                            else extend(model, row);
                        }
                        UpdateSchedule(pid);
                        break;
                    case "SVINV":
                        var lstsv02 = JsonConvert.DeserializeObject<List<SvTrnInvoice>>(data.Data);
                        foreach (var row in lstsv02)
                        {
                            var model = ctx.SvTrnInvoices.Find(row.CompanyCode, row.BranchCode, row.ProductType, row.InvoiceNo);
                            if (model == null) ctx.SvTrnInvoices.Add(row);
                            else extend(model, row);
                        }
                        UpdateSchedule(pid);
                        break;
                    case "SVMSI":
                        var lstsv03 = JsonConvert.DeserializeObject<List<SvHstSzkMsi>>(data.Data);
                        foreach (var row in lstsv03)
                        {
                            var model = ctx.SvHstSzkMsies.Find(row.CompanyCode, row.BranchCode, row.PeriodYear, row.PeriodMonth, row.SeqNo);
                            if (model == null) ctx.SvHstSzkMsies.Add(row);
                            else extend(model, row);
                        }
                        UpdateSchedule(pid);
                        break;

                    case "PMKDP":
                        var lstpm01 = JsonConvert.DeserializeObject<List<PmKdp>>(data.Data);
                        foreach (var row in lstpm01)
                        {
                            var model = ctx.PmKdps.Find(row.InquiryNumber, row.BranchCode, row.CompanyCode);
                            if (model == null) ctx.PmKdps.Add(row);
                            else extend(model, row);
                        }
                        UpdateSchedule(pid);
                        break;
                    case "PMSHS":
                        var lstpm02 = JsonConvert.DeserializeObject<List<PmStatusHistory>>(data.Data);
                        foreach (var row in lstpm02)
                        {
                            var model = ctx.PmStatusHistories.Find(row.InquiryNumber, row.CompanyCode, row.BranchCode, row.SequenceNo);
                            if (model == null) ctx.PmStatusHistories.Add(row);
                            else extend(model, row);
                        }
                        UpdateSchedule(pid);
                        break;
                    case "PMACT":
                        var lstpm03 = JsonConvert.DeserializeObject<List<PmActivity>>(data.Data);
                        foreach (var row in lstpm03)
                        {
                            var model = ctx.PmActivities.Find(row.CompanyCode, row.BranchCode, row.InquiryNumber, row.ActivityID);
                            if (model == null) ctx.PmActivities.Add(row);
                            else extend(model, row);
                        }
                        UpdateSchedule(pid);
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateSchedule(string pid)
        {
            var schedule = ctxDealer.GnMstScheduleDatas.Find(pid);
            schedule.Status = "P";
            schedule.UpdatedDate = DateTime.Now;
            try
            {
                ctx.SaveChanges();
                ctxDealer.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void extend(object object1, object object2)
        {
            var props = object2.GetType().GetProperties();
            foreach (var prop2 in props)
            {
                var prop1 = object1.GetType().GetProperties().Where(p => p.Name == prop2.Name).FirstOrDefault();
                if (prop1 != null)
                {
                    prop1.SetValue(object1, prop2.GetValue(object2, null), null);
                }
            }
        }

        private void ProcessLog(string pid)
        {
            var data = ctxDealer.GnMstScheduleDatas.Find(pid);
            if (data != null)
            {
                switch (data.DataType)
                {
                    case "EMPLY":
                        var lstem01 = JsonConvert.DeserializeObject<List<HrEmployee>>(data.Data);
                        foreach (var row in lstem01)
                        {
                            var model = new HrEmployeeLog { UnicID = Guid.NewGuid().ToString() };
                            extend(model, row);
                            ctxLog.HrEmployeeLogs.Add(model);
                            ctxLog.SaveChanges();
                        }
                        break;
                    case "EMACH":
                        var lstem02 = JsonConvert.DeserializeObject<List<HrEmployeeAchievement>>(data.Data);
                        foreach (var row in lstem02)
                        {
                            var model = new HrEmployeeAchievementLog { UnicID = Guid.NewGuid().ToString() };
                            extend(model, row);
                            ctxLog.HrEmployeeAchievementLogs.Add(model);
                            ctxLog.SaveChanges();
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
