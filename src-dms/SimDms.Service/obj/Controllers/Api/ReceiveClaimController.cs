using SimDms.Service.BLL;
using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class ReceiveClaimController : BaseController
    {
        #region Public JsonResult
        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (!file.FileName.ToLower().EndsWith("txt"))
                    throw new Exception("File harus berupa text document (.txt)");
                var textFile = new StreamReader(file.InputStream);
                var content = textFile.ReadToEnd();

                return Json(new { success = true, Content = content, FileName = file.FileName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult ReceiveFile(string Content, WarrantyClaimReceiveHdr model)
        {
            try
            {
                string[] lines = Content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                var oWClaimHdrFile = p_ValidateFile(lines, model);
                var oWClaimDtlFiles = p_WClaimDtlFiles(lines);

                string msg = string.Empty;
                var oWarrantyClaimBLL = WarrantyClaimBLL.Instance(CurrentUser.UserId);
                var isReceive = oWarrantyClaimBLL.Receive(oWClaimHdrFile, oWClaimDtlFiles, out msg);
                oWarrantyClaimBLL = null;
                if (isReceive)
                {
                    var paymentDate = oWClaimHdrFile.ReimbursementDate.ToString("dd-MMM-yyyy");
                    var totalTicket = oWClaimHdrFile.TotalItem.ToString("n0");

                    return Json(new { success = true, PaymentDate = paymentDate, TotalTicket = totalTicket });
                }
                else
                {
                    return Json(new { success = false, message = msg });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult GenerateWClaimFile(SvTrnClaimJudgement model)
        {
            try
            {
                string customerAbbrName = string.Empty;
                string msg = string.Empty;
                bool isSuccess = false;

                var billTypes = ctx.svMstBillTypes.Where(q => q.CompanyCode == CompanyCode && q.BillType == "W").FirstOrDefault();
                if (billTypes != null)
                {
                    var customers = ctx.Customers.Where(p => p.CompanyCode == CompanyCode
                    && p.CustomerCode == billTypes.CompanyCode).FirstOrDefault();

                    if (customers != null)
                    {
                        customerAbbrName = customers.CustomerAbbrName;
                    }
                }

                if (!string.IsNullOrWhiteSpace(customerAbbrName))
                {
                    var oWarrantyClaimBLL = WarrantyClaimBLL.Instance(CurrentUser.UserId);
                    var rcvDealer = oWarrantyClaimBLL.GetReceiveDealer(model.SuzukiRefferenceNo, model.ReceivedDate);
                    if (rcvDealer.Count() == 0)
                    {
                        throw new Exception("Data tidak ditemukan!!");
                    }

                    string fileName = "WCMRB"; 
                    List<string> lines = new List<string>();
                    foreach (Claim claim in rcvDealer)
                    {
                        fileName = claim.SenderDealerCode;
                        WClaimRcvHdrFile headerText = oWarrantyClaimBLL.GetWRcvClaimHdrFile("WCMRB", model.SuzukiRefferenceNo, claim.SenderDealerCode, CompanyName, model.ReceivedDate);
                        List<WClaimRcvDtlFile> dtlFiles = oWarrantyClaimBLL.GetWRcvClaimDtlFile(model.SuzukiRefferenceNo, claim.SenderDealerCode, model.ReceivedDate);
                        lines.Add(headerText.Text);
                        foreach (WClaimRcvDtlFile dtlFile in dtlFiles)
                        {
                            lines.Add(dtlFile.Text);
                            foreach (WClaimRcvPartFile partFile in dtlFile.ListPartFiles)
                            {
                                lines.Add(partFile.Text);
                            }
                        }
                    }
                    isSuccess = true;
                    msg = "Generate File Sukses !!";
                    p_GenerateFile(fileName, lines);
                }
                else
                {
                    msg = "Sub Dealer tidak bisa melakukan proses Generate ini !!";
                }

                return Json(new { success = isSuccess, message = msg});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        public JsonResult SvRpTrn014Summary(string SuzukiRefferenceNo)
        {
            using (var sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand)
            {
                string sql = "usprpt_SvRpTrn014_Summary";
                string tableName = "RcvClaimSum";
    
                sqlCmd.CommandText = sql;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                sqlCmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                sqlCmd.Parameters.AddWithValue("@ProductType", ProductType);
                sqlCmd.Parameters.AddWithValue("@SuzukiRefferenceNo", SuzukiRefferenceNo);
                SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
                DataTable dtReportSummary = new DataTable(tableName);
                sqlDA.Fill(dtReportSummary);

                return GenerateReportXls(dtReportSummary, tableName, tableName);
            }
        }

        public JsonResult SvRpTrn014Detail(string SuzukiRefferenceNo)
        {
            using (var sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand)
            {
                string sql = "usprpt_SvRpTrn014_Summary";
                string tableName = "RcvClaimDtl";

                sqlCmd.CommandText = sql;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                sqlCmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                sqlCmd.Parameters.AddWithValue("@ProductType", ProductType);
                sqlCmd.Parameters.AddWithValue("@SuzukiRefferenceNo", SuzukiRefferenceNo);
                SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
                //DataTable dtReportDetail = new DataTable(tableName);
                DataSet dtReportDetail = new DataSet(tableName);
                sqlDA.Fill(dtReportDetail);

                var sheet = new List<string> { };
                sheet.Add(tableName);

                return GenerateReportXls(dtReportDetail, sheet, tableName);
            }
        }
        #endregion

        #region Private Method
        private ActionResult p_GenerateFile(string fileName, List<string> lines)
        {
            StringBuilder data = new StringBuilder();
            int counter = 1;
            foreach (string str in lines)
            {
                if (counter == lines.Count)
                    data.Append(str.ToString());
                else
                    data.AppendLine(str.ToString());
                counter++;
            }

            var bytesFile = Encoding.UTF8.GetBytes(data.ToString());
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".txt");
            using (MemoryStream MyMemoryStream = new MemoryStream(bytesFile))
            {
                MyMemoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
                Response.Close();

                return File(Response.OutputStream, Response.ContentType);
            }
        }

        private WClaimRcvHdrFile p_ValidateFile(string[] lines, WarrantyClaimReceiveHdr model)
        {
            var WClaimRcvHdrFile = new WClaimRcvHdrFile(lines[0]);

            string message = string.Empty;
            //  Data ID harus “WCMRB”
            if (WClaimRcvHdrFile.DataID != "WCMRB")
            {
                message = "Data Header tidak sesuai dengan Warranty Claim!";
                throw new Exception(message + ",File Tidak Valid");
                
            }

            //  Dealer Code harus sama dengan Kode Pengirim
            if (WClaimRcvHdrFile.DealerCode != model.CustomerCode)
            {
                message = "Kode Dealer pada File tidak sesuai dengan yang Anda masukkan !";
                throw new Exception(message + ",File Tidak Valid");
            }

            //  Received Dealer Code harus sama dengan Company Code sesuai dengan user login
            if (WClaimRcvHdrFile.RcvDealerCode != CompanyCode)
            {
                message = "Kode Dealer Penerima pada File tidak sesuai dengan Kode Dealer Anda !";
                throw new Exception(message + ",File Tidak Valid");
                
            }

            //  Total Number of Item harus sama dengan jumlah record di detail
            if (WClaimRcvHdrFile.TotalItem != p_GetTotalItem(lines))
            {
                message = "Jumlah Record di Detail Tidak Valid dengan Informasi di Header!";
                throw new Exception(message + ",File Tidak Valid");
                
            }

            //  Payment No harus sama dengan Nomor Pembayaran
            if (WClaimRcvHdrFile.ReimbursementNo != model.PaymentNo)
            {
                message = "No Kwitansi pada File tidak sesuai dengan No Kwitansi yang Anda Masukkan !";
                throw new Exception(message + ",File Tidak Valid");
            }

            //  Product Type harus sama dengan Product Type sesuai dengan user login
            if (WClaimRcvHdrFile.ProductType != (
                    ProductType == "2W" ? "A" :
                    ProductType == "4W" ? "B" :
                    ProductType == "OB" ? "C" : ProductType))
                throw new Exception("Product Type Pada File Tidak Sesuai Dengan Product Type Dealer Anda !");

            return WClaimRcvHdrFile;
        }

        private List<WClaimRcvDtlFile> p_WClaimDtlFiles(string[] lines)
        {
            var oWClaimDtlFiles = new List<WClaimRcvDtlFile>();
            WClaimRcvDtlFile oWClaimDtlFile = null;
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("1"))
                {
                    oWClaimDtlFile = new WClaimRcvDtlFile(lines[i]);
                    oWClaimDtlFiles.Add(oWClaimDtlFile);
                }
                if (lines[i].StartsWith("2"))
                {
                    if (oWClaimDtlFile != null)
                    {
                        WClaimRcvPartFile partfile = new WClaimRcvPartFile(lines[i]);
                        oWClaimDtlFile.AddPartFile(partfile);
                    }
                }
            }
           
            return oWClaimDtlFiles;
        }

        private int p_GetTotalItem(string[] lines)
        {
            int i = 0;
            foreach (string line in lines)
            {
                if (line.StartsWith("1")) i++;
            }
            return i;
        }

        #endregion
    }
}
