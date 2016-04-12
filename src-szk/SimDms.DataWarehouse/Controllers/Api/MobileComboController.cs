using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class MobileComboController : BaseController
    {
        public ContentResult JsonpDealerList(string callback)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_JsonpDealerList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@DealerType", "");
            cmd.Parameters.AddWithValue("@LinkedModule", "");
            cmd.Parameters.AddWithValue("@GroupArea", "");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return Content(String.Format("{0}({1});",
                callback,
                new JavaScriptSerializer().Serialize(GetJson(dt))),
                "application/javascript");
        }

        public ContentResult JsonpBranchList(string callback)
        {
            string companyCode = Request["CompanyCode"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_JsonpBranchList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return Content(String.Format("{0}({1});",
                callback,
                new JavaScriptSerializer().Serialize(GetJson(dt))),
                "application/javascript");
        }
    }
}
