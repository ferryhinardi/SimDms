using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Sim.Dms.SendData.Model;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net;
using System.Configuration;
using System.Security.Principal;
using log4net;
using log4net.Config;

namespace Sim.Dms.SendData
{
    public class DataMgr
    {
        protected DataContext ctx = new DataContext();
        //protected DataContext ctx1 = new DataContext();

        private static readonly ILog logger = LogManager.GetLogger(typeof(DataMgr));

        static DataMgr()
        {
            DOMConfigurator.Configure();
        }

        //Get SDMS Update data order by priorty 
        public List<SendScheduleDms> GetSdmsUpdateData()
        {
            var listSchedule = ctx.SendScheduleDmss.OrderBy(m => m.Priority).ToList();
            return listSchedule;
        }

        public void ProcessUpdateData(SendScheduleDms scheduleData)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                DateTime lastDateData = DateTime.Now;
                string jsonString = "";
                string timeOut= ConfigurationSettings.AppSettings["CommandTimeOut"].ToString();
                int timeOutConnection = (string.IsNullOrWhiteSpace(timeOut)?int.Parse(timeOut):60);
                //Get Token Access from server and LastUpdateDateData from server
                DateTime lastSendDateData = DateTime.Now;
                var tokenAccess = GetTokenFromServer(scheduleData.CompanyCode, scheduleData.DataType);
                if (!string.IsNullOrWhiteSpace(tokenAccess.ResponseData))
                    lastSendDateData = Convert.ToDateTime(tokenAccess.ResponseData);
                else
                    lastSendDateData = Convert.ToDateTime("1990-01-01");

                //execute storeprocedure
                SqlConnection conn = (SqlConnection)ctx.Database.Connection;
                SqlCommand cmd = new SqlCommand();
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = scheduleData.SPName;
                cmd.Connection = conn;
                cmd.CommandTimeout = timeOutConnection;
                cmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime).Value = lastSendDateData;
                cmd.Parameters.Add("@Segment", SqlDbType.Int).Value = scheduleData.SegmentNo;
                conn.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
                conn.Close();

                if (ds.Tables[0].Rows.Count != 0)
                {
                    //filter all data from NULL value in LastUpdatDate COLUMN
                    logger.Info(string.Format("Processing Data = {0}, Total Row Of Data Processing= {1}", scheduleData.DataType, ds.Tables[0].Rows.Count));
                    lastDateData = ds.Tables[0].Rows.Cast<System.Data.DataRow>().OrderByDescending(m => m.Field<DateTime>(scheduleData.ColumnLastUpdate.ToString())).Select(m => m.Field<DateTime>(scheduleData.ColumnLastUpdate.ToString())).Take(1).FirstOrDefault();
                    var objJson = GetJson(ds.Tables[0]);
                    serializer.MaxJsonLength = int.MaxValue;
                    serializer.RecursionLimit = 100;
                    jsonString = serializer.Serialize(objJson);
                    if (tokenAccess.Success)
                    {
                        ///Send Schedule Data
                        logger.Info(string.Format("Send DMS Data to DMS Server. Process data={0}", ds.Tables[0].Rows.Count));
                        SendScheduleData(jsonString, scheduleData, lastDateData, tokenAccess.Message, ds.Tables[0].Rows.Count);
                        logger.Info(string.Format("DMS Data successfully send to Server, DataType={0}, Data Length={1}", scheduleData.DataType, scheduleData.SegmentNo));
                    }
                    else
                    {
                        throw new Exception("Error when processing function GetTokenFromServer, Message=" + tokenAccess.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error when processing function ProcessUpdateData, Message=" + ex.ToString());
            }
        }

        private CallbackResponse GetTokenFromServer(string companyName, string dataType)
        {
            try
            {
                string computerName = WindowsIdentity.GetCurrent().Name;
                string UrlGetToken = ConfigurationSettings.AppSettings[DMSConstant.UrlTokenAccess].ToString();
                byte[] callbackResponse;
                using (WebClientEx client = new WebClientEx())
                {
                    NameValueCollection reqParam = new NameValueCollection();
                    reqParam.Add(DMSConstant.UniqueID, Guid.NewGuid().ToString());
                    reqParam.Add(DMSConstant.CompanyCode, companyName);
                    reqParam.Add(DMSConstant.ComputerName, computerName);
                    reqParam.Add(DMSConstant.DataType, dataType);
                    callbackResponse = client.UploadValues(UrlGetToken, "POST", reqParam);
                };
                var response = ConvertCallBackResponse(callbackResponse);
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region "Send Schedule data"
        private void SendScheduleData(string jsonData, SendScheduleDms scheduleData, DateTime lastDataUpdate, string TokenID, int Segment)
        {
            try
            {
                string computerName = WindowsIdentity.GetCurrent().Name;
                byte[] callbackResponse;
                CallbackResponse response;
                string urlSendSchedule = ConfigurationSettings.AppSettings[DMSConstant.UrlSendDataSchedule].ToString();
                using (WebClient client = new WebClient())
                {
                    NameValueCollection reqParam = new NameValueCollection();
                    reqParam.Add(DMSConstant.UniqueID, Guid.NewGuid().ToString());
                    reqParam.Add(DMSConstant.CompanyCode, scheduleData.CompanyCode);
                    reqParam.Add(DMSConstant.DataType, scheduleData.DataType);
                    reqParam.Add(DMSConstant.Segment, Segment.ToString());
                    reqParam.Add(DMSConstant.Data, jsonData);
                    reqParam.Add(DMSConstant.ComputerName, computerName);
                    reqParam.Add(DMSConstant.TokenID, TokenID);
                    reqParam.Add(DMSConstant.LastSendDate, lastDataUpdate.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    callbackResponse = client.UploadValues(urlSendSchedule, "POST", reqParam);
                };
                response = ConvertCallBackResponse(callbackResponse);
                var entity = ctx.SendScheduleDmss.Find(scheduleData.CompanyCode, scheduleData.DataType);
                if (response.Success)
                {
                    if (entity != null)
                        entity.Status = DMSConstant.Success;
                    entity.LastSendDate = lastDataUpdate;
                }
                else
                {
                    if (entity != null)
                        entity.Status = DMSConstant.Fail;
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error when processing function SendScheduleData, Message=" + ex.ToString());
            }
            finally
            {
                ctx.SaveChanges();
            }
        }
        #endregion

        #region "Convert Callback Response"
        private CallbackResponse ConvertCallBackResponse(byte[] response)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var x = Encoding.UTF8.GetString(response);
            var s = x.Substring(1, x.ToString().Length - 3);
            var reponseObject = (CallbackResponse)serializer.Deserialize<CallbackResponse>(s);
            return reponseObject;
        }
        #endregion

        #region "Convert Json to List"
        protected List<Dictionary<string, object>> GetJson(DataTable dt)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row = null;

            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName.Trim(), dr[col]);
                }
                rows.Add(row);
            }
            return rows;
        }
        #endregion

    }

    public class WebClientEx : WebClient
    {
        public int Timeout { get; set; }

        public WebClientEx()
        {
            Timeout = 900000;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            request.Timeout = Timeout;
            return request;
        }
    }
}
