using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SimDms.Service.BLL;
using SimDms.Common.DcsWs;

namespace SimDms.Service.Controllers.Api
{
    public class GenerateClaimController : BaseController
    {
        private const string DataId = "WCLAM";
        private DcsWsSoapClient ws = new DcsWsSoapClient();
        public JsonResult Default()
        {
            return Json(new
            {

                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BatchNo = "BAT/XX/YYYYYY",
                ReceiptDate = DateTime.Now,
                FPJDate = DateTime.Now,
                ProductType = ProductType,
                UserFullName = CurrentUser.FullName
            });
        }

        public JsonResult Save(GenerateClm model, string GenerateNo)
        {
            if (model.FPJNo == null || model.FPJNo == "null")
            {
                model.FPJNo = "-";
                model.FPJDate = DateTime.Now;
            }
            if (model.FPJGovNo == null || model.FPJGovNo == "null")
            {
                model.FPJGovNo = "-";
            }

            string msg = string.Empty;
            bool stat = false;
            string batch = string.Empty;
            using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
            new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                try
                {
                    batch = (string)ctx.Database.SqlQuery<string>("uspfn_SvSaveClaimBatch @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10",
                        CompanyCode, BranchCode, ProductType, model.ReceiptNo, model.ReceiptDate, model.FPJNo, model.FPJDate, model.FPJGovNo, GenerateNo, CurrentUser.UserId, model.LockingBy).FirstOrDefault();

                    ctx.SaveChanges();
                    tranScope.Complete();
                    msg = "Data has been Saved";
                    stat = true;
                }
                catch (Exception ex)
                {
                    string innerEx = (ex.InnerException == null) ? ex.Message :
                    (ex.InnerException.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message;
                    msg = (ex.InnerException == null) ? ex.Message : innerEx;
                }
            }

            return Json(new { success = stat, message = msg, data = new { Batch = batch } });
        }

        public JsonResult DeleteData(string BatchNo)
        {
            string msg = string.Empty;
            bool stat = false;
            using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
            new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                try
                {
                    stat = (bool)ctx.Database.SqlQuery<bool>("uspfn_SvUtlKsgClaimDelete @p0, @p1, @p2, @p3, @p4",
                        CompanyCode, BranchCode, "CLM", BatchNo, CurrentUser.UserId).FirstOrDefault();

                    ctx.SaveChanges();
                    tranScope.Complete();
                    msg = "Data has been Deleted";
                }
                catch (Exception ex)
                {
                    string innerEx = (ex.InnerException == null) ? ex.Message :
                    (ex.InnerException.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message;
                    msg = (ex.InnerException == null) ? ex.Message : innerEx;
                }
            }

            return Json(new { success = stat, message = msg });
        }

        public JsonResult getDataQuery(string BatchNo)
        {
            DataSet dt = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvGetClaimBatch";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@BatchNo", BatchNo);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            var list = GetJson(dt.Tables[1]);

            if (list != null)
            {
                return Json(list);
            }
            else
            {
                return Json(new { success = false }); ;
            }
        }

        public JsonResult GetClaimBatchQuery()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvGetClaimBatchQuery";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult GnMstLookUpDtl()
        {
            var record = ctx.GnMstLookUpDtls.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.CodeID == "SRV_FLAG" && a.LookUpValue == "CLM_HOLDING");
            return Json(record);
        }

        public JsonResult Generate(string BatchNo)
        {
            return Json(new { data = p_GenerateFile(BatchNo) });
        }

        public ActionResult SaveFile()
        {
            var BatchNo = string.IsNullOrEmpty(Request["BatchNo"]) ? "" : Request["BatchNo"].ToString();
            var bytesFile = Encoding.UTF8.GetBytes(p_GenerateFile(BatchNo));
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.AddHeader("content-disposition", "attachment;filename=WCLAM.txt");
            using (MemoryStream MyMemoryStream = new MemoryStream(bytesFile))
            {
                MyMemoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
                Response.Close();

                return File(Response.OutputStream, Response.ContentType);
            }
        }

        public JsonResult WsStatus()
        {
            return Json(new { status = ws.IsValid() ? "Online" : "Offline" });
        }

        public JsonResult SendFile(string Contents)
        {
            var msg = "";
            bool rsl = false;
            string header = Contents.Split('\n')[0];

            var checkOnline = ws.IsValid() ? "online" : "offline";

            if (checkOnline == "online")
            {
                try
                {
                    string result = ws.SendToDcs(DataId, CompanyCode, Contents, ProductType);
                    if (result.StartsWith("FAIL")) return Json(new { success = false, message = result.Substring(5) });

                    LogHeaderFile(DataId, CompanyCode, header, ProductType);
                    msg = string.Format("{0} berhasil di upload (dcs :" + checkOnline +")", DataId);
                    rsl = true;
                }
                catch
                {
                    msg = string.Format("{0} gagal digenerate (dcs :" + checkOnline +")", DataId);
                    rsl = false;
                }
            }
            else
            {
                rsl = false;
                msg = string.Format("{0} gagal digenerate (dcs :" + checkOnline + ") ", DataId);
            }

            return Json(new { success = rsl, message = msg });

        }

        #region Private Method

        private string p_GenerateFile(string BatchNo)
        {
            //StringBuilder data = new StringBuilder();
            //List<string> dataList = p_GenWClaimFile(BatchNo);
            //int counter = 1;
            //foreach (string str in dataList)
            //{
            //    if (counter == dataList.Count)
            //        data.Append(str.ToString());
            //    else
            //        data.AppendLine(str.ToString());
            //    data.Append(str.ToString());

            //    counter++;
            //}
            //return data.ToString();
           
            string contents = "";
            List<string> dataList = p_GenWClaimFile(BatchNo);
            int counter1 = 1;

            foreach (string str in dataList)
            {
                if (counter1 == dataList.Count)
                {
                    contents = contents + str.ToString();
                }                    
                else
                {
                    contents = contents + str.ToString() + '\n';
                }
                counter1++;
            }

            return contents;
        }

        private List<string> p_GenWClaimFile(string batchNo)
        {
            return p_GenWClaimFile("WCLAM", batchNo);
        }

        private List<string> p_GenWClaimFile(string dataID, string batchNo)
        {
            WarrantyClaimBLL oWarrantyClaimBLL = WarrantyClaimBLL.Instance(CurrentUser.UserId);
            WarrantyClaimHdrFile header = oWarrantyClaimBLL.GetWClaimHdrFile(dataID, batchNo);
            List<WarrantyClaimDtlFile> details = p_GetWClaimDtlFilesTuning(batchNo);

            List<string> lines = new List<string>();
            lines.Add(header.Text);
            foreach (WarrantyClaimDtlFile detail in details)
            {
                lines.Add(detail.Text);
                foreach (WarrantyClaimPartFile partfile in detail.ListPartFiles)
                {
                    lines.Add(partfile.Text);
                }
                if (detail.RepairDescription != null && detail.RepairDescription.Length > 0)
                {
                    string information3 = "3" + detail.RepairDescription.PadRight(154, ' ');
                    if (information3.Length > 156)
                        information3 = information3.Substring(0, 155);

                    lines.Add(information3);
                }
            }

            return lines;
        }

        private List<WarrantyClaimDtlFile> p_GetWClaimDtlFilesTuning(string batchNo)
        {
            SqlCommand sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            sqlCmd.CommandText = "uspfn_SvGetFlatFileClaimDtl";
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            sqlCmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            sqlCmd.Parameters.AddWithValue("@ProductType", ProductType);
            sqlCmd.Parameters.AddWithValue("@BatchNo", batchNo);
            DataTable dt = new DataTable();
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
            sqlDA.Fill(dt);

            List<WarrantyClaimDtlFile> files = new List<WarrantyClaimDtlFile>();
            foreach (DataRow row in dt.Rows)
            {
                WarrantyClaimDtlFile file = new WarrantyClaimDtlFile();
                file.IssueNo = row["IssueNoCust"].ToString();
                file.ServiceBookNo = row["ServiceBookNo"].ToString();
                #region ** ChassisCode+ChassisNo dan EngineCode+EngineNo **
                //file.ChassisCode = row["ChassisCode"].ToString();
                //file.ChassisNo = Convert.ToString(row["ChassisNo"]);
                //file.EngineCode = row["EngineCode"].ToString();
                //file.EngineNo = row["EngineNo"].ToString();

                file.ChassisCodeChasisNo = row["ChassisCode"].ToString() + Convert.ToString(row["ChassisNo"]);
                file.EngineCodeEngineNo = row["EngineCode"].ToString() + row["EngineNo"].ToString();

                #endregion
                file.BasicModel = row["BasicModel"].ToString();
                file.TechnicalModel = row["TechnicalModel"].ToString();
                file.IssueDate = Convert.ToDateTime(row["IssueDate"].ToString());
                file.RegisteredDate = Convert.ToDateTime(row["RegisteredDate"].ToString());
                file.RepairedDate = Convert.ToDateTime(row["RepairedDate"].ToString());
                file.Odometer = Convert.ToDecimal(row["Odometer"].ToString());
                file.TroubleCode = row["TroubleCode"].ToString();
                file.KdOemCode = row["OemCode"].ToString();
                file.ClaimCategoryCode = row["CategoryCode"].ToString();
                file.OperationNumber = row["OperationNo"].ToString();
                file.ActualLaborTime = row["ActualLaborTime"].ToString();
                file.SubletWorkTime = row["SubletWorkTime"].ToString();
                file.RegisteredDate = Convert.ToDateTime(row["RegisteredDate"].ToString());
                file.RepairDescription = row["TroubleDescription"].ToString().Replace("\r", "").Replace("\n", ", ");
                files.Add(file);
                p_SetWClaimPartFiles(file, row["BranchCode"].ToString(), row["GenerateNo"], row["GenerateSeq"]);
            }

            dt = null;
            sqlDA = null;
            sqlCmd = null;

            return files;
        }

        private void p_SetWClaimPartFiles(WarrantyClaimDtlFile detail, string branchCode, object generateNo, object generateSeq)
        {
            SqlCommand sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            sqlCmd.CommandText = "uspfn_SvInqCausalPart";
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            sqlCmd.Parameters.AddWithValue("@BranchCode", branchCode);
            sqlCmd.Parameters.AddWithValue("@InvoiceNo", generateNo);
            sqlCmd.Parameters.AddWithValue("@InvoiceSeq", generateSeq);
            sqlCmd.Parameters.AddWithValue("@CausalPartNo", "");
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
            DataTable dt = new DataTable();
            sqlDA.Fill(dt);

            int i = 0;
            WarrantyClaimPartFile partfile = new WarrantyClaimPartFile();
            foreach (DataRow reader in dt.Rows)
            {
                i++;
                WarrantyClaimPartFile.WarrantyClaimPart part = new WarrantyClaimPartFile.WarrantyClaimPart();
                part.CausalPartCode = Convert.ToBoolean(reader["IsCausalPart"]) ? "X" : " ";
                part.PartNo = Convert.ToString(reader["PartNo"]);
                part.Qty = Convert.ToInt32(reader["PartQty"]);
                partfile.AddPart(part);

                if (i == 1) detail.AddPartFile(partfile);
                if (i >= 7)
                {
                    i = 0;
                    partfile = new WarrantyClaimPartFile();
                }
            }

            dt = null;
            sqlDA = null;
            sqlCmd = null;
        }

        private void LogHeaderFile(string dataID, string custCode, string header, string prodType)
        {
            string query = "exec uspfn_spLogHeader @p0,@p1,@p2,@p3,@p4,@p5";
            object[] Parameters = { dataID, custCode, prodType, "SEND", DateTime.Now, header };
            ctx.Database.ExecuteSqlCommand(query, Parameters);
        }

        #endregion
    }
}
