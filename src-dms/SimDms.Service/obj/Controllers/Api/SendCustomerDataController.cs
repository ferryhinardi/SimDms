using SimDms.Common.DcsWs;
using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class SendCustomerDataController : BaseController
    {
        private DcsWsSoapClient ws = new DcsWsSoapClient();

        private WSMRDHeaderFlat GetFlatCustomerHdr(bool isLocked)
        {
            var header = new WSMRDHeaderFlat();

            var query = "uspfn_SvGetFlatFileCustHdr @p0, @p1, @p2";
            var result = ctx.Database.SqlQuery<WSMRDHeader>(query, CompanyCode, BranchCode, isLocked).FirstOrDefault();
            if (result == null) return null;

            header.DealerCode = result.DealerCode;
            header.DealerName = result.DealerName;
            header.ProductType = result.ProductType;
            header.ReceivingDealerCode = result.ReceivingDealerCode;
            header.SendDate = result.SendDate;
            header.TotalNumberOfItem = result.TotalItem.ToString();

            return header;
        }

        private List<WSMRDDetailFlat> GetFlatCustomerDtls(bool isLocked)
        {
            var details = new List<WSMRDDetailFlat>();

            var query = "uspfn_SvGetFlatFileCustDtl @p0, @p1, @p2";
            var result = ctx.Database.SqlQuery<WSMRDDetail>(query, CompanyCode, BranchCode, isLocked);
            if (result.Count() == 0) return null;

            foreach (var item in result)
            {
                var detail = new WSMRDDetailFlat();
                detail.Address = item.Address;
                detail.BirthDate = item.BirthDate.Value;
                detail.CustomerName = item.CustomerName;
                detail.EmailAddress = item.Email;
                detail.Gender = item.Gender;
                detail.MobileNo = item.HPNo != null ? (Regex.IsMatch(item.HPNo, @"^\d*$") == true) ? item.HPNo : "-" : "";
                detail.PhoneNo = item.PhoneNo != null ? (Regex.IsMatch(item.PhoneNo, @"^\(?\d*\)?[\s\-]?\d*$") == true) ? item.PhoneNo : "-" : "";
                detail.PoliceRegNo = item.PoliceRegNo;
                detail.Remarks = item.Remarks;
                detail.VinNo = item.VIN;
                details.Add(detail);
            }

            return details;
        }

        private List<string> GetCustomerFlat(bool isLocked)
        {
            var header = GetFlatCustomerHdr(isLocked);
            var details = GetFlatCustomerDtls(isLocked);
            if (header == null || details == null) return null;

            var lines = new List<string>();
            lines.Add(header.Text);
            foreach (var detail in details)
            {
                lines.Add(detail.Text);
            }
            return lines;
        }

        public JsonResult Inquiry(bool isLocked)
        {
            var dataList = GetCustomerFlat(isLocked);
            if (dataList == null) return Json(null);
            var data = new StringBuilder();
            var counter = 1;
            foreach (var str in dataList)
            {
                if (dataList.Count > 1)
                {
                    if (counter == dataList.Count) data.Append(str);
                    else data.AppendLine(str);
                    counter++;
                }
                else break;
            }
            return Json(new { result = data.ToString() });
        }

        public JsonResult PrepareFile(string text)
        {
            var sessionName = "downloadCustomerData";
            var content = new byte[text.Length * sizeof(char)];
            Buffer.BlockCopy(text.ToCharArray(), 0, content, 0, content.Length);
            TempData.Add(sessionName, content);
            return Json(new { sessionName = sessionName });
        }

        public FileContentResult DownloadFile(string sessionName)
        {
            var content = TempData[sessionName] as byte[];
            TempData.Clear();
            var stream = new MemoryStream(content);
            var contentType = "application/text";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("content-disposition", "attachment;filename=PORDS.txt");
            Response.Buffer = true;
            stream.WriteTo(Response.OutputStream);
            Response.End();
            return File(content, contentType, "");
        }

        public JsonResult SendToDCS(string text, string filter)
        {
            var message = "";
            const string dataId = "WSMRD";
            var lines = text.Split('\n');
            var sb = new StringBuilder();
            
            foreach (var line in lines) sb.AppendLine(line);
            var data = sb.ToString();
            var customerCode = "";

            try
            {
                var recDtl = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode
                && x.CodeID == dataId && x.LookUpValue == "1");
                if (recDtl != null && recDtl.ParaValue == "1")
                {
                    var copro = ctx.CoProfileServices.FirstOrDefault(x => x.CompanyCode == CompanyCode
                        && x.BranchCode == BranchCode);
                    customerCode = copro == null ? BranchCode : (copro.LockingBy == "" ? BranchCode : copro.LockingBy);
                }
                else customerCode = CompanyCode;

                var result = ws.SendToDcs(dataId, customerCode, data, ProductType);
                if (result.StartsWith("FAIL")) throw new Exception(result.Substring(5));

                var query = "exec uspfn_gnCustUpdateIsLock @p0, @p1, @p2";
                ctx.Database.ExecuteSqlCommand(query, CompanyCode, BranchCode, filter);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new { message = message });
        }
    }
}
