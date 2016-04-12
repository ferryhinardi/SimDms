using SimDms.Common.DcsWs;
using SimDms.Common.Models;
using SimDms.Sparepart.BLL;
using SimDms.Sparepart.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers.Api
{
    public class SendFileController : BaseController
    {
        private DcsWsSoapClient ws = new DcsWsSoapClient();

        public JsonResult Default()
        {
            return Json(new
            {
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year
            });
        }

        public JsonResult SendData(SendFile model)
        {
            string contents = "";
            string type = "";
            try
            {
                if (model.Daily)
                {
                    type = "PDLYR";
                    var dt = SalesPerformanceBLL.GetActivityReportData(CompanyCode, BranchCode, model.Month, model.Year);
                    contents = SalesPerformanceBLL.GenPDLYR(CompanyCode, model.StandardCode, BranchName, dt, model.Month, model.Year);
                    var PDLYR = new byte[contents.Length * sizeof(char)];
                    Buffer.BlockCopy(contents.ToCharArray(), 0, PDLYR, 0, PDLYR.Length);
                    Session.Add(type, PDLYR);

                    if (ws.IsValid())
                    {
                        return Json(new { success = true, contents = contents, type = type});
                    }
                    else
                    {
                        return Json(new { success = false, type = type });
                    }

                }
                if (model.PlanRealization)
                {
                    type = "PLRLD";
                    var dt = SalesPerformanceBLL.GetPlanRealization(CompanyCode, BranchCode, model.Month, model.Year);
                    contents = SalesPerformanceBLL.GenPLRLD(CompanyCode, BranchName, model.StandardCode, dt);
                    var PLRLD = new byte[contents.Length * sizeof(char)];
                    Buffer.BlockCopy(contents.ToCharArray(), 0, PLRLD, 0, PLRLD.Length);
                    Session.Add(type, PLRLD);
                    if (ws.IsValid())
                    {
                        return Json(new { success = true, contents = contents, type = type });
                    }
                    else
                    {
                        return Json(new { success = false, type = type });
                    }
                }
                if (model.Sales)
                {
                    type = "PMSPD";
                    var dt = SalesPerformanceBLL.GetUtilitySales(CompanyCode, BranchCode, model.Month, model.Year);
                    contents = SalesPerformanceBLL.GenPMSPD(CompanyCode, model.StandardCode, BranchName, dt);
                    var PMSPD = new byte[contents.Length * sizeof(char)];
                    Buffer.BlockCopy(contents.ToCharArray(), 0, PMSPD, 0, PMSPD.Length);
                    Session.Add(type, PMSPD);
                    if (ws.IsValid())
                    {
                        return Json(new { success = true, contents = contents, type = type });
                    }
                    else
                    {
                        return Json(new { success = false, type = type });
                    }
                }
                if (model.Stock)
                {
                    type = "PSTCK";
                    var dt = SalesPerformanceBLL.GetStockData(CompanyCode, BranchCode, model.Month, model.Year);
                    contents = SalesPerformanceBLL.GenPSTCK(CompanyCode, model.StandardCode, BranchName, dt, model.Month, model.Year);
                    var PSTCK = new byte[contents.Length * sizeof(char)];
                    Buffer.BlockCopy(contents.ToCharArray(), 0, PSTCK, 0, PSTCK.Length);
                    Session.Add(type, PSTCK);
                    if (ws.IsValid())
                    {
                        return Json(new { success = true, contents = contents, type = type });
                    }
                    else
                    {
                        return Json(new { success = false, type = type });
                    }
                }
                if (model.BackOrder)
                {
                    type = "PBORD";
                    var dt = SalesPerformanceBLL.GetBackOrderManifest(CompanyCode, BranchCode, model.Month, model.Year);
                    contents = SalesPerformanceBLL.GenPBORD(CompanyCode, model.StandardCode, BranchName, dt);
                    var PBORD = new byte[contents.Length * sizeof(char)];
                    Buffer.BlockCopy(contents.ToCharArray(), 0, PBORD, 0, PBORD.Length);
                    Session.Add(type, PBORD);
                    if (ws.IsValid())
                    {
                        return Json(new { success = true, contents = contents, type = type });
                    }
                    else
                    {
                        return Json(new { success = false, type = type });
                    }
                }
                if (model.LeadTime)
                {
                    type = "PMLTD";
                    var dt = SalesPerformanceBLL.GetUtilityLeadTime(CompanyCode, BranchCode, model.FirstPeriod, model.EndPeriod);
                    contents = SalesPerformanceBLL.GenPMLTD(CompanyCode, model.StandardCode, BranchName, dt);
                    var PMLTD = new byte[contents.Length * sizeof(char)];
                    Buffer.BlockCopy(contents.ToCharArray(), 0, PMLTD, 0, PMLTD.Length);
                    Session.Add(type, PMLTD);
                    if (ws.IsValid())
                    {
                        return Json(new { success = true, contents = contents, type = type });
                    }
                    else
                    {
                        return Json(new { success = false, type = type });
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return Json("");
        }

        public JsonResult SendFile(string type,string Contents)
        {
            //var file = Session[type] as byte[];
            string header = Contents.Split('\n')[0];

            var msg = "";
            
            Session.Clear();

            try
            {
                string result = ws.SendToDcs(type, CompanyCode, header, ProductType);
                if (result.StartsWith("FAIL")) return Json(new { success = false, message = result.Substring(5) });

                LogHeaderFile(type, CompanyCode, header, ProductType);
                msg = string.Format("{0} berhasil di upload", type);
                return Json(new { success = true, message = msg });
            }
            catch
            {
                msg = string.Format("{0} gagal digenerate", type);
                return Json(new { success = false, message = msg });
            }
        }

        public FileContentResult GetData(string type)
        {
            var file = Session[type] as byte[];

            Session.Clear();

            var ms = new MemoryStream(file);
            string contentType = "application/text";

            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("content-disposition", "attachment;filename=" + type + ".txt");
            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();

            return File(file, contentType, type + ".txt");
        }

        public JsonResult ValidateHeaderFile(string type, string Contents)
        {
            var result = false;
            var msg = "";

            string header = Contents.Split('\n')[0];
            
            string qry = string.Format("select * from gnDcsUploadFile where DataID = '{0}' and Header = '{1}'", type, header);
            var dt = ctx.Database.SqlQuery<GnDcsUploadFile>(qry);
            if (dt.Count() > 0)
            {
                result = false;
                msg = string.Format("Data {0} sudah pernah dikirim pada {1}, apakah akan dikirim ulang?", type, dt.FirstOrDefault().CreatedDate);
            }

            return Json(new { success = result, message = msg });
        }

        private void LogHeaderFile(string dataID, string custCode, string header, string prodType)
        {
            string query = "exec uspfn_spLogHeader @p0,@p1,@p2,@p3,@p4,@p5";
            object[] Parameters = { dataID, custCode, prodType, "SEND", DateTime.Now, header };
            ctx.Database.ExecuteSqlCommand(query, Parameters);
        }
    }
}
