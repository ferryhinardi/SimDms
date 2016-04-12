using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.IO;

namespace SimDms.General.Controllers.Api
{
    public class QueryController : BaseController
    {

        public JsonResult Analizer(string ssql)
        {
            DataTable dt = new DataTable();

            if (ssql == "" || !ssql.Trim().ToLower().StartsWith("select"))
                return Json(new { success = false, data = dt, msg = "invalid syntax" });
            if (ssql.ToLower().Contains("insert ")
                || ssql.ToLower().Contains("update ")
                || ssql.ToLower().Contains("delete ")
                || ssql.ToLower().Contains("create ")
                || ssql.ToLower().Contains("drop "))
            {
                //MessageBox.Show("Cannot execute syntax, dangerous syntax");
                return Json(new { success = false, data = dt, msg = "Cannot execute syntax, dangerous syntax" });
            }
            
            
            
            var conn = ctx.Database.Connection;
            var connectionState = conn.State;
            List<string> clmnnme=new List<string>();
            try
            {
                using (ctx)
                {
                    if (connectionState != ConnectionState.Open)
                        conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = ssql;
                        //cmd.CommandType = CommandType.TableDirect;
                        //cmd.Parameters.Add(new SqlParameter("jobCardId", 100525));
                        using (var reader = cmd.ExecuteReader())
                        {
                            
                            dt.Load(reader);

                            foreach(DataColumn dc in dt.Columns)
                            {
                                clmnnme.Add(dc.ColumnName);
                            }
                        }
                    }
                    return Json(new { success = true, hdr=clmnnme, data = dt,msg= string.Format("({0} row(s) affected)", dt.Rows.Count) });
                }

            }
            catch (Exception ex)
            {
                if (connectionState != ConnectionState.Closed)
                    conn.Close();
                return Json(new { success = false, data = dt ,msg=ex.Message});
            }
            
        }

        public string ExportXLS(string ssql)
        {
            try
            {
            DataTable dt = new DataTable();

            if (ssql == "" || !ssql.Trim().ToLower().StartsWith("select"))
                return "invalid syntax";
            if (ssql.ToLower().Contains("insert ")
                || ssql.ToLower().Contains("update ")
                || ssql.ToLower().Contains("delete ")
                || ssql.ToLower().Contains("create ")
                || ssql.ToLower().Contains("drop "))
            {                
                return "Cannot execute syntax, dangerous syntax";
            }
            

            DataSet ds = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;



            cmd.CommandText = ssql;
            

            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("QueryAnalizer");


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
             dt = ds.Tables[0];
            int x = 1;
            int y = 1;

            foreach (DataColumn clmn in dt.Columns)
            {
                ws.Cell(y, x).Value = clmn.ColumnName;
                x++;
            }
            x = 1;


            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];                    
                    
                    ws.Cell(y + 1, x).Value = val;
                    x++;
                }
                x = 1;
                y++;
            }
            

            MemoryStream ms = new MemoryStream();
            
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + "Query_"+DateTime.Now.ToString("yyyy-MM-dd")+".xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            
            workbook.SaveAs(ms);
            Response.BinaryWrite(ms.ToArray());
            Response.End();

            return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            

        }
    }
}
