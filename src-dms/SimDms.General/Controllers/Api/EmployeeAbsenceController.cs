using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SimDms.General.Controllers.Api
{
    public class EmployeeAbsenceController: BaseController
    {
        public JsonResult EmployeeAbsenceList(DateTime beginDate)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            cmd.CommandText = "uspfn_GnAbsenceList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@WorkingDate", beginDate);

            SqlDataAdapter daHdr = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dtHdr = new DataTable();
            daHdr.Fill(ds);
            var list = GetJson(ds.Tables[1]);

            return Json(new { success = true, data = list });
        }
    }
}
