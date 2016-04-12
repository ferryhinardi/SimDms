using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using TracerX;

using System.Linq;

using System.Web;
using System.Web.Mvc;


namespace SimDms.General.Controllers.Api
{
    public class InquiryController : BaseController
    {

        public JsonResult Customers(bool AllowPeriod, DateTime StartDate, DateTime EndDate, string Branch)
        {

            try
            {
                string orderBy = string.Empty;
                string param = string.Empty;
                string docDate = string.Empty;
                string dtFirstDate, dtLastDate;
                string flag1 = AllowPeriod ? "1" : "0";

                dtFirstDate = StartDate.ToString("yyyy-MM-dd");
                dtLastDate = EndDate.ToString("yyyy-MM-dd");


                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "usprpt_QueryCustomerDealer2 '" + flag1 + "','" + dtFirstDate + "','" + dtLastDate + "','','','" + Branch + "'";
                cmd.CommandTimeout = 3600;

                MyLogger.Log.Info("Inquiry Customers: EXEC " + cmd.CommandText);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);
                var records = dt.Tables[0];
                var rows = records.Rows.Count;

                return Json(records);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        public JsonResult DealerInfo()
        {
            try
            {

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "SELECT TOP 1 Area,DealerName,DealerAbbreviation Abbr FROM gnMstDealerMapping";
                cmd.CommandTimeout = 3600;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);

                return Json(dt.Tables[0]);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }


    }
}
