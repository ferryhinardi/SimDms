using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SimDms.General.Controllers.Api
{
    public class PostingController : BaseController
    {
        public JsonResult GetValue(string id)
        {
            return Json(new { id = id, Date = new DateTime(), Data = GetSessionValue(id) });
        }

        public JsonResult PostingDate()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_GnGetPostingDate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("CompanyCode", CompanyCode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt.Rows[0]));
        }


        public JsonResult PostingStatus()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_gnCheckPostingStatus";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            
            string retVal = "1", retVal2 = "1", retVal3 = "1";
            string info = "Unknown Error";

            try
            {
                da.Fill(dt);
                if (dt !=null && dt.Rows.Count ==1)
                {
                    retVal = dt.Rows[0][0].ToString();
                    retVal2 = dt.Rows[0][1].ToString();
                    retVal3 = dt.Rows[0][1].ToString();
                    info = dt.Rows[0][2].ToString();
                }
            } catch(Exception ex)
            {

            }

            return Json(new { success = true, spsrv = retVal, sales=retVal2, info = info, tax = retVal3 });
        }

        public JsonResult AllowPosting()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_checkdailyaccess '" + CurrentUser.UserId + "'";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            string retVal = "0";

            if (dt != null && dt.Rows.Count == 1)
            {
                retVal = dt.Rows[0][0].ToString();
            }

            return Json(new { success = true, allow = retVal });
        }

        public JsonResult DailyPosting()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_GnDailyPosting";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("UserId", CurrentUser.UserId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt.Rows[0]));
        }



        public JsonResult DailyPostingMultiCompany(string PostingDate)
        {

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "usprpt_PostingMultiCompanyMainProcess";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("PostingDate", DateTime.Now); //PostingDate);
            cmd.Parameters.AddWithValue("UserId", CurrentUser.UserId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            string success = "0";
            string info = "Terjadi kesalahan saat posting";

            if (dt != null && dt.Rows.Count == 1)
            {
                success = dt.Rows[0][0].ToString();
                info = dt.Rows[0][1].ToString();
            }

            return Json(new { success=success, info=info });
        }

        //public JsonResult getServerClock()
        //{
        //    DateTime svrClock

        //    return Json(new { success=true, data= });
        //}

    }
}
