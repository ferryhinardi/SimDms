using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.BLL
{
    public class XLogger
    {
        DataTable dtXlogger = new DataTable("dtXlogger");

        public XLogger(){
            DataTable dtXLogger = new DataTable("dtXlogger");
            DataColumn colType = new DataColumn("LoggerType");
            DataColumn colMessage = new DataColumn("LoggerMessage");
            dtXLogger.Columns.AddRange(new DataColumn[] { colType, colMessage });
        }

        /// <summary>
        /// Add Logger Data
        /// </summary>
        /// <param name="loggerType"></param>
        /// <param name="loggerMessage"></param>
        public void AddMessage(LoggerType loggerType, string loggerMessage){
            DataRow dr = dtXlogger.NewRow();
            dr["LoggerType"] = (LoggerType)loggerType;
            dr["LoggerMessage"] = loggerMessage;
            dtXlogger.Rows.Add(dr);
            dtXlogger.AcceptChanges();
        }

        /// <summary>
        /// Get Logger Data Record(s)
        /// </summary>
        /// <returns>DataTable Logger data</returns>
        public DataTable ShowInformation()
        {
            return dtXlogger;
        }
    }
}
