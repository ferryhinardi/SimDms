using Newtonsoft.Json;
using SimDms.SUtility;
using SimDms.SUtility.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using System.Web.Script.Serialization;

namespace SimDms.SUtility.Controllers.Api
{
    public class InquiryController : BaseController
    {
        public JsonResult DataList()
        {
            string comp = Request["CompanyCode"] ?? "-";
            string type = Request["DataType"];
            string stus = Request["DataStatus"];

            var qry = from p in ctxDealer.GnMstScheduleDatas
                      where p.CompanyCode == comp
                      select new
                      {
                          p.UniqueID,
                          p.CompanyCode,
                          p.DataType,
                          p.Segment,
                          p.LastSendDate,
                          p.Status,
                          p.CreatedDate,
                          p.UpdatedDate
                      };

            if (!string.IsNullOrWhiteSpace(type)) qry = qry.Where(p => p.DataType == type);
            if (!string.IsNullOrWhiteSpace(stus)) qry = qry.Where(p => p.Status == stus);

            return Json(qry.KGrid());
        }

        public JsonResult LastUpdate()
        {
            string comp = Request["CompanyCode"] ?? "";
            string type = Request["DataType"] ?? "";
            string stus = Request["DataStatus"] ?? "";

            SqlCommand cmd = ctxDealer.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_UtlLastSend";
            cmd.CommandTimeout = 3600;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@DataType", type);
            cmd.Parameters.AddWithValue("@DataStatus", stus);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult GetData(string id = "")
        {
            var data = ctxDealer.GnMstScheduleDatas.Find(id);
            if (data != null)
            {
                switch (data.DataType)
                {
                    case "EMPLY":
                        return Json(new { success = true, type = data.DataType, data = JsonConvert.DeserializeObject<List<HrEmployee>>(data.Data) });
                    case "EMACH":
                        return Json(new { success = true, type = data.DataType, data = JsonConvert.DeserializeObject<List<HrEmployeeAchievement>>(data.Data) });
                    case "EMUTA":
                        return Json(new { success = true, type = data.DataType, data = JsonConvert.DeserializeObject<List<HrEmployeeMutation>>(data.Data) });
                    case "EMSFM":
                        return Json(new { success = true, type = data.DataType, data = JsonConvert.DeserializeObject<List<HrEmployeeSales>>(data.Data) });
                    case "SVSPK":
                        return Json(new { success = true, type = data.DataType, data = JsonConvert.DeserializeObject<List<SvTrnService>>(data.Data) });
                    case "SVINV":
                        return Json(new { success = true, type = data.DataType, data = JsonConvert.DeserializeObject<List<SvTrnInvoice>>(data.Data) });
                    case "SVMSI":
                        return Json(new { success = true, type = data.DataType, data = JsonConvert.DeserializeObject<List<SvHstSzkMsi>>(data.Data) });
                    case "PMKDP":
                        return Json(new { success = true, type = data.DataType, data = JsonConvert.DeserializeObject<List<PmKdp>>(data.Data) });
                    case "PMSHS":
                        return Json(new { success = true, type = data.DataType, data = JsonConvert.DeserializeObject<List<PmStatusHistory>>(data.Data) });
                    case "PMACT":
                        return Json(new { success = true, type = data.DataType, data = JsonConvert.DeserializeObject<List<PmActivity>>(data.Data) });
                    case "CS3DCALL":
                        return Json(new { success = true, type = data.DataType, data = JsonConvert.DeserializeObject<List<CsTDayCall>>(data.Data) });
                    default:
                        return Json(new { success = false, type = data.DataType, message = "data not registered yet" });
                }
            }
            else
            {
                return Json(new { success = false, type = data.DataType, message = "data not found" });
            }
        }

        public JsonResult SvInvoice()
        {
            string comp = Request["CompanyCode"] ?? "--";
            string bran = Request["BranchCode"] ?? "--";
            int year = Convert.ToInt32(string.IsNullOrWhiteSpace(Request["Year"]) ? "2100" : Request["Year"]);
            var qry = from p in ctx.SvTrnInvoices
                      where p.CompanyCode == comp && p.BranchCode == bran && p.InvoiceDate.Value.Year == year
                      select new
                      {
                          p.CompanyCode,
                          p.BranchCode,
                          p.InvoiceNo,
                          p.InvoiceDate,
                          p.JobOrderNo,
                          p.JobOrderDate,
                          p.JobType,
                          p.TotalSrvAmt
                      };

            return Json(qry.KGrid());
        }

        public JsonResult PartSalesFilter()
        {
            var json = Exec(new { query = "uspfn_vwPartSalesFilter" });
            return Json(json.Data);
        }
    }
}
