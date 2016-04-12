using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;
using SimDms.Common.DcsWs;
using SimDms.Common.Models;
using System.Text;
using System.IO;

namespace SimDms.Service.Controllers.Api
{
    public class InqMSIController : BaseController
    {
        private const string dataID = "WMSIA";
        private string msg = "";
        private DcsWsSoapClient ws = new DcsWsSoapClient();

        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year
            });
        }

        public JsonResult ListOfMonth()
        {
            List<Object> listObj = new List<Object>();
            string[] listMonth = new string[] { "Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "November", "Desember" };
            int idx = 1;
            foreach (var month in listMonth)
            {
                listObj.Add(new { value = idx, text = month });
                idx++;
            }
            return Json(listObj);
        }

        public JsonResult ListOfYear()
        {
            var year = DateTime.Now.Year;
            List<Object> listObj = new List<Object>();
            for (int i = year - 5; i <= year; i++)
            {
                listObj.Add(new { value = i.ToString(), text = i.ToString() });
            }
            return Json(listObj);
        }

        public JsonResult LoadData(int month, int year)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            cmd.CommandText = "usprpt_SvRpReport021V3";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@PeriodYear", year);
            cmd.Parameters.AddWithValue("@Month2", month);
            cmd.Parameters.AddWithValue("@UserId", CurrentUser.UserId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            var data = from a in dt.AsEnumerable().AsQueryable()
                       select new MSI
                       {
                           CompanyCode = a[0].ToString(),
                           BranchCode = a[1].ToString(),
                           PeriodYear = Convert.ToDecimal(a[2].ToString()),
                           SeqNo = Convert.ToInt32(a[3].ToString()),
                           MsiGroup = a[4].ToString(),
                           MsiDesc = a[5].ToString(),
                           Unit = a[6].ToString(),
                           Average = Convert.ToDecimal(a[7].ToString()),
                           Total = Convert.ToDecimal(a[8].ToString()),
                           Jan = Convert.ToDecimal(a[9].ToString()),
                           Feb = Convert.ToDecimal(a[10].ToString()),
                           Mar = Convert.ToDecimal(a[11].ToString()),
                           Apr = Convert.ToDecimal(a[12].ToString()),
                           May = Convert.ToDecimal(a[13].ToString()),
                           Jun = Convert.ToDecimal(a[14].ToString()),
                           Jul = Convert.ToDecimal(a[15].ToString()),
                           Aug = Convert.ToDecimal(a[16].ToString()),
                           Sep = Convert.ToDecimal(a[17].ToString()),
                           Oct = Convert.ToDecimal(a[18].ToString()),
                           Nov = Convert.ToDecimal(a[19].ToString()),
                           Dec = Convert.ToDecimal(a[20].ToString())
                       };

            return Json(data);
        }

        public JsonResult GenerateWMSIA(int year, int month)
        {
            var dt = GenWMSIA(year, month);
            var data = "";

            if (dt.Rows.Count > 1)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    data += dt.Rows[i][0].ToString() + " ";
                }
            }

            return Json(data);
        }

        public FileContentResult DownloadFile(int year, int month)
        {
            var dt = GenWMSIA(year, month);
            var data = "";
            StringBuilder sb = new StringBuilder();

            if (dt.Rows.Count > 1)
            {
                //data = dt.Rows[0][0].ToString();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    data += dt.Rows[i][0].ToString() + "\n";
                }
            }

            string[] vars = data.Split('\n');
            int len = vars.Length; int con = 1;
            foreach (string var in vars)
            {
                if (len == con)
                    sb.Append(var);
                else
                    sb.AppendLine(var);
                con++;
            }

            string WMSIA = sb.ToString();
            byte[] content = new byte[WMSIA.Length * sizeof(char)];
            System.Buffer.BlockCopy(WMSIA.ToCharArray(), 0, content, 0, content.Length);
            string contentType = "application/text";
            Response.Clear();
            MemoryStream ms = new MemoryStream(content);
            Response.ContentType = "application/text";
            Response.AddHeader("content-disposition", "attachment;filename=WMSIA.txt");
            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
            //Parameters to file are
            //1. The File Path on the File Server
            //2. The content type MIME type
            //3. The parameter for the file save by the browser
            return File(content, contentType, "WMSIA.txt");
        }

        public JsonResult SendFile(int year, int month)
        {
            var dt = GenWMSIA(year, month);
            var data = "";
            StringBuilder sb = new StringBuilder();

            string[] vars = data.Split('\n');

            for (int i = 0; i < vars.Length; i++)
            {
                if (i + 1 < vars.Length) sb.AppendLine(vars[i]);
                else sb.Append(vars[i]);
            }

            string header = sb.ToString().Split('\n')[0];
            try
            {

                string result = ws.SendToDcs(dataID, CompanyCode, data, ProductType);
                if (result.StartsWith("FAIL")) return Json(new { success = false, message = result.Substring(5) });

                LogHeaderFile(dataID, CompanyCode, header, ProductType);
                msg = string.Format("{0} berhasil di upload", dataID);
                return Json(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                msg = string.Format("{0} gagal digenerate : {1}", dataID, ex.Message.ToString());
                return Json(new { success = false, message = msg });
            }
        }

        public JsonResult ValidateHeaderFile(int year, int month)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dtHeader = new DataTable();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            string data = "";
            cmd.CommandText = "uspfn_SvMsiFlatFile";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@PeriodYear", year);
            cmd.Parameters.AddWithValue("@PeriodMonth", month);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dtHeader);

            if (dtHeader.Rows.Count > 1)
            {
                for (int i = 0; i < dtHeader.Rows.Count; i++)
                {
                    data += dtHeader.Rows[i][0].ToString() + "\n";
                }
            }

            var result = true;

            string[] vars = data.Split('\n');

            for (int i = 0; i < vars.Length; i++)
            {
                if (i + 1 < vars.Length) sb.AppendLine(vars[i]);
                else sb.Append(vars[i]);
            }

            var header = sb.ToString();


            string qry = string.Format("select * from gnDcsUploadFile where DataID = '{0}' and Header = '{1}'", dataID, header);
            var dt = ctx.Database.SqlQuery<GnDcsUploadFile>(qry);
            if (dt.Count() > 0)
            {
                result = false;
                msg = string.Format("Data {0} sudah pernah dikirim pada {1}, apakah akan dikirim ulang?", dataID, dt.FirstOrDefault().CreatedDate);
            }

            return Json(new { success = result, message = msg });
        }

        private void LogHeaderFile(string dataID, string custCode, string header, string prodType)
        {
            string query = "exec uspfn_spLogHeader @p0,@p1,@p2,@p3,@p4,@p5";
            object[] Parameters = { dataID, custCode, prodType, "SEND", DateTime.Now, header };
            ctx.Database.ExecuteSqlCommand(query, Parameters);
        }

        private DataTable GenWMSIA(int year, int month)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = new DataTable();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            string data = "";
            cmd.CommandText = "uspfn_SvMsiFlatFile";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@PeriodYear", year);
            cmd.Parameters.AddWithValue("@PeriodMonth", month);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

    }
}
