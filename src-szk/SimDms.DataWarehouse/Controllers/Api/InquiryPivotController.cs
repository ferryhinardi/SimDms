using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using GeLang;
using SimDms.DataWarehouse.Models;
using System.Data.Entity.Core.Objects;
using ClosedXML.Excel;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class InquiryPivotController : BaseController
    {
        public string ServiceTransaction()
        {
            string StartDate = Request["StartDate"] ?? "";
            string EndDate = Request["EndDate"] ?? "";
            string comp = Request["CompanyCode"] ?? "";
            string bran = Request["BranchCode"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = Request["PivotId"];

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@BranchCode", bran);
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
            string output2 = JsonConvert.SerializeObject(dt, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Formatting = Formatting.Indented });
            return output2;
        }
       
    }
}
