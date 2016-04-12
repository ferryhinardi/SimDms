
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.General.Controllers.Api
{
    public class BackupController : BaseController
    {
        public JsonResult BackupDB(string suffix)
        {
         
            string msg = "";

            object obj = null;
            string[] param = { "BACKUP_DBNAME", "BACKUP_FOLDERPATH", "BACKUP_FILENAME", "BACKUP_FILEEXT" };
            string paramSql = "usputl_backupdb";
            int path = paramSql.Length;

            var conn = ctx.Database.Connection;
            var connectionState = conn.State;

            for (int x = 0; x < param.Length; x++)
            {

                var recParam = ctx.SysParameters.Find(param[x].ToString());
                if (recParam == null)
                {
                    msg = "Parameter untuk backup database belum tersedia \nSilahkan setting parameter terlebih dahulu";
                    return Json(new { success = false, message = msg });
                }
                else
                {
                    if (string.IsNullOrEmpty(recParam.ParamValue.ToString()))
                    {
                        msg = "Parameter untuk backup database belum tersedia \nSilahkan setting parameter terlebih dahulu";
                        return Json(new { success = false, message = msg });
                    }
                }
                paramSql += " '" + recParam.ParamValue + "',";
            }

            try            
            {
   
                paramSql = paramSql.Substring(0, paramSql.Length - 1);
                paramSql += ",'0','" + suffix + "'";                

                using (ctx)
                {
                    if (connectionState != ConnectionState.Open)
                        conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                                        
                        cmd.CommandText = paramSql;
                        //cmd.CommandType = CommandType.TableDirect;
                        //cmd.Parameters.Add(new SqlParameter("jobCardId", 100525));
                        cmd.CommandTimeout = 36000;
                        obj=cmd.ExecuteScalar();

                    }
                    
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
                return Json(new { success = false, message = msg });
            }
            if (connectionState != ConnectionState.Closed)
                conn.Close();

            if (obj != null)
            {
                msg = string.Format("{0} {1}", "Sukses backup database ke server path\n", obj);
                return Json(new { success = true, message = msg });
            }
            else
            {
                msg = "Sukses backup database ke server path " + paramSql.Substring(path, paramSql.Length - path);
                return Json(new { success = true, message = msg });
            }            
            
        }
    }
}
