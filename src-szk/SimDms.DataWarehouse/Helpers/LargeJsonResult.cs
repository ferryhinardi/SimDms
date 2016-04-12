using Microsoft.Reporting.WebForms;
using SimDms.DataWarehouse.Controllers;
using SimDms.DataWarehouse.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SimDms.DataWarehouse.Helpers
{
    
    //to handle json limit max length in mvc3
    public class LargeJsonResult : JsonResult
    {
        const string JsonResult_GetNotAllowed = "This request has been blocked because of sensitive infomation and Get request will not be allowed..";
        public LargeJsonResult()
        {
            MaxJsonLength = 1024000;
            RecursionLimit = 100;
        }

        public int MaxJsonLength { get; set; }
        public int RecursionLimit { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet && String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(JsonResult_GetNotAllowed);
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!string.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                JavaScriptSerializer ser = new JavaScriptSerializer() { MaxJsonLength = MaxJsonLength, RecursionLimit = RecursionLimit };
                response.Write(ser.Serialize(Data));
            }
        }

    }


    public class ElmahCommandInterceptor : IDbCommandInterceptor
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void NonQueryExecuting(
            DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            LogIfNonAsync(command, interceptionContext);
        }

        public void NonQueryExecuted(
            DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            LogIfError(command, interceptionContext);
        }

        public void ReaderExecuting(
            DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            LogIfNonAsync(command, interceptionContext);
        }

        public void ReaderExecuted(
            DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            LogIfError(command, interceptionContext);
        }

        public void ScalarExecuting(
            DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            LogIfNonAsync(command, interceptionContext);
        }

        public void ScalarExecuted(
            DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            LogIfError(command, interceptionContext);
        }

        private void LogIfNonAsync<TResult>(
            DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
            if (!interceptionContext.IsAsync)
            {
                //Logger.Warn("Non-async command used: {0}", command.CommandText);
            }
        }

        private void LogIfError<TResult>(
            DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
            if (interceptionContext.Exception != null)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(interceptionContext.Exception);
            }
        }
    }


    public class MyHelpers
    {
        public static DataTable GetTable(DbContext ctx, string sSQL)
        {
            try
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600;  cmd.CommandText = sSQL;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static DataSet GetDataSet(DbContext ctx, string sSQL)
        {
            try
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600;  cmd.CommandText = sSQL;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string GetConnString(string cfgName)
        {
            //string MyAppPath = HttpContext.Current.Request.ApplicationPath.ToString();

            //if (MyAppPath.Length > 1)
            //{
            //    var IsMultipleApp = System.Configuration.ConfigurationManager.AppSettings["MultipleApp"] ?? "0";
            //    if (Convert.ToBoolean(IsMultipleApp))
            //    {
            //        cfgName += MyAppPath.Replace(@"/", "_");
            //    }
            //}

            string cnStr = System.Configuration.ConfigurationManager.ConnectionStrings[cfgName].ToString();
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                cnStr += "-" + HttpContext.Current.User.Identity.Name;
            }

            //MyLogger.Info("Conn String: " + cnStr);
            return cnStr;
        }

        public static byte[] GenerateSQLReport(MyReportParameter param,ReportParameter[] paramsReport = null)
        {
            LocalReport localReport = new LocalReport();
            var CN = new SqlConnection((System.Configuration.ConfigurationManager.ConnectionStrings["DataContext"].ToString()));
            var SQL = param.sql;
            var reportFilename = param.filename;
            localReport.ReportPath = reportFilename;

            SqlCommand cmd = new SqlCommand(SQL, CN);
            var dba = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            dba.Fill(dt);

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = param.Name;
            reportDataSource.Value = dt;
            localReport.DataSources.Add(reportDataSource);

            if (paramsReport != null)
            {
                localReport.SetParameters(paramsReport);
            }

            string mimeType;
            string encoding;
            string fileNameExtension;

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            //Render the report            
            return localReport.Render(param.reportType, null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
        }

        public static byte[] GenerateSQLReportPdf(MyReportParameter param, ReportParameter[] paramsReport = null)
        {
            LocalReport localReport = new LocalReport();
            var CN = new SqlConnection((System.Configuration.ConfigurationManager.ConnectionStrings["DataContext"].ToString()));
            var SQL = param.sql;
            var reportFilename = param.filename;
            localReport.ReportPath = reportFilename;

            SqlCommand cmd = new SqlCommand(SQL, CN);
            var dba = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            dba.Fill(dt);

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = param.Name;
            reportDataSource.Value = dt;
            localReport.DataSources.Add(reportDataSource);

            if (paramsReport != null)
            {
                localReport.SetParameters(paramsReport);
            }

            string mimeType;
            string encoding;
            string fileNameExtension;

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            string devinfo = "<DeviceInfo><ColorDepth>32</ColorDepth><DpiX>350</DpiX><DpiY>350</DpiY><OutputFormat>PDF</OutputFormat>" +
                            "  <PageWidth>9in</PageWidth>" +
                            "  <PageHeight>11.69in</PageHeight>" +
                            "  <MarginTop>0.5in</MarginTop>" +
                            "  <MarginLeft>0.2in</MarginLeft>" +
                            "  <MarginRight>0in</MarginRight>" +
                            "  <MarginBottom>0in</MarginBottom>" +
                            "</DeviceInfo>";

            //Render the report            
            return localReport.Render(param.reportType, devinfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
        }

        

    }


    public class MyReportParameter
    {
        public string filename { get; set; }
        public string sql { get; set; }
        public string reportType { get; set; }
        public string Name { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int OrderCount { get; set; }
        public DateTime? LastPurchase { get; set; }
        public int UnitsInStock { get; set; }
        public bool? LowStock
        {
            get { return UnitsInStock < 300; }
        }
    }

    public static class DoddleProductRepository
    {
        public static List<Product> GetAll()
        {
            var rand = new Random();
            return Enumerable.Range(1, 200)
                .Select(i => new Product
                {
                    Id = i,
                    Name = "Product" + i,
                    Description =
                        "This is an example description showing long text in some of the items. Here is some UTF text €",
                    Price = rand.NextDouble() * 100,
                    OrderCount = rand.Next(1000),
                    LastPurchase = DateTime.Now.AddDays(rand.Next(1000)),
                    UnitsInStock = rand.Next(0, 1000)
                })
                .ToList();
        }

    }
}