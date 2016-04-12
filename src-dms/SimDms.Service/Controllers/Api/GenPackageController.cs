using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class GenPackageController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new {
                CompanyCode = CompanyCode,
                BranchCode = BranchCode,
                ReceiptDate = DateTime.Now,
                FPJDate = DateTime.Now
            });
        }

        public JsonResult GetBatch(string batchNo)
        {
            try
            {
                var cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = string.Format("exec uspfn_SvTrnPackageBatchGet '{0}', '{1}', '{2}'",
                    CompanyCode, BranchCode, batchNo);
                var ds = new DataSet();
                new SqlDataAdapter(cmd).Fill(ds);
                var table1 = GetJson(ds.Tables[0]);
                var table2 = GetJson(ds.Tables[1]);
                return Json(new { message = "", header = table1, details = table2 });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult Validate()
        {
            var msg = DateTransValidation(DateTime.Now);
            return Json(new { message = msg });
        }

        public JsonResult Query()
        {
            try
            {
                var sql = string.Format("exec uspfn_SvTrnPackageList '{0}', '{1}', '{2}'",
                    CompanyCode, BranchCode, "A");
                var data = ctx.Database.SqlQuery<PackageSP>(sql);
                return Json(new { message = "", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult Save(BatchPackageSP header, List<PackageSP> details)
        {
            try
            {
                var generateNo = "";
                foreach (var dtl in details)
                {
                    generateNo += string.Format(",'{0}'", dtl.GenerateNo);
                }

                if (generateNo.Length > 0)
                {
                    generateNo = generateNo.Substring(1);
                }

                var sql = string.Format(
                    "exec uspfn_SvTrnPackageBatchSave '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}'",
                    CompanyCode, BranchCode, header.ReceiptNo, header.ReceiptDate, header.FpjNo, 
                    header.FpjDate, generateNo, CurrentUser.UserId);
                var result = ctx.Database.SqlQuery<string>(sql);

                return Json(new { message = "", batchNo = result });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult GenerateFile(string batchNo)
        {
            try
            {
                var text = GetGeneratedText(batchNo);
                return Json(new { message = "", file = text }); 
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public FileResult SaveGeneratedFile(string batchNo)
        {
            var text = GetGeneratedText(batchNo);
            var buffer = Encoding.UTF8.GetBytes(text);
            var stream = new System.IO.MemoryStream(buffer, 0, buffer.Length);
            stream.Position = 0;
            return File(stream, "text/plain", "WPACK.txt");
        }

        private string GetGeneratedText(string batchNo)
        {
            var sql = string.Format("exec uspfn_SvTrnPackageBatchFile '{0}', '{1}' ,'{2}'",
                        CompanyCode, BranchCode, batchNo);
            var batch = ctx.Database.SqlQuery<PackageBatchFile>(sql).ToList();
            var text = string.Empty;
            for (int i = 0; i < batch.Count; i++)
            {
                if (i == 0) text += batch[i].FlatData;
                else text += "\n" + batch[i].FlatData;
            }
            return text;
        }

        public class PackageBatchFile
        {
            public string FlatData { get; set; }
            public string SeqData { get; set; }
        }
    }
}
