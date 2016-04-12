using SimDms.Common.DcsWs;
using SimDms.Common.Models;
using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class SendSMRController : BaseController
    {
        private string CodeID = "WSMRR";
        private DcsWsSoapClient ws = new DcsWsSoapClient();
        private string msg = "";

        public JsonResult Default()
        {
            int year = DateTime.Today.Year;
            int month = DateTime.Today.Month;

            return Json(new
            {
                FirstPeriod = new DateTime(year, month, 1),
                EndPeriod = new DateTime(year, month, 1).AddMonths(1).AddDays(-1),
            });
        }

        public JsonResult Inquiry(DateTime FirstPeriod, DateTime EndPeriod)
        {
            SqlCommand sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            sqlCmd.CommandText = "uspfn_SvGenSMR";
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            sqlCmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            sqlCmd.Parameters.AddWithValue("@Date1", FirstPeriod);
            sqlCmd.Parameters.AddWithValue("@Date2", EndPeriod);
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
            DataSet ds = new DataSet();
            sqlDA.Fill(ds);
            WSMRR smr = new WSMRR(ds);

            return Json(smr);
        }

        public FileContentResult SaveFile(string Contents)
        {
            string[] lines = null;

            lines = Regex.Split(Contents, "\n");

            byte[] content = lines.Select(s => Convert.ToByte(s, 16)).ToArray();
            string contentType = "application/text";

            MemoryStream ms = new MemoryStream(content);
            Response.ContentType = contentType;
            Response.AddHeader("content-disposition", "attachment;filename=" + CodeID + ".txt");
            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();

            return File(content, contentType, CodeID + ".txt");
        }

        public JsonResult SendFile(string Contents)
        {
            string[] lines = null;

            lines = Regex.Split(Contents, "\n");

            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i + 1 < lines.Length) sb.AppendLine(lines[i]);
                    else sb.Append(lines[i]);
                }

                string data = sb.ToString();
                string customerCode;

                LookUpDtl recDtl = ctx.LookUpDtls.Find(CompanyCode, CodeID, "1");
                if (recDtl != null && recDtl.ParaValue == "1")
                {
                    string cust = ctx.CoProfileServices.Find(CompanyCode, BranchCode).LockingBy;
                    if (string.IsNullOrEmpty(cust))
                        customerCode = BranchCode;
                    else
                        customerCode = cust;
                }
                else
                    customerCode = CompanyCode;

                string result = ws.SendToDcs(CodeID, customerCode, data, ProductType);
                if (result.StartsWith("FAIL")) return Json(new { success = false, message = result.Substring(5) });
                msg = string.Format("{0} berhasil di upload", CodeID);
                return Json(new { success = true, message = msg });
            }
            catch
            {
                msg = string.Format("{0} gagal digenerate", CodeID);
                return Json(new { success = false, message = msg });
            }
        }
    }
}