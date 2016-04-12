using EventScheduler;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using SVNMON.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using TracerX;

namespace SVNMON
{
    public delegate void MyServiceHandler(int code);
    public delegate void MySMSEventHandler(int code, string Message);
    public delegate void StartingImportEventHandler(string TableName, int Count);

    public class BasePanel
    {
        public string Name { get; set; }
        public Object ObjectPtr { get; set; }
    }

    public class MyShared
    {

        public static Boolean SvcState { get; set; }
        public static List<Boolean> lSharedState = new List<bool>();
        public static List<string> Worker = new List<string>();


        // Declare a Logger instance for use by this class.
        public static readonly Logger Log = Logger.GetLogger("SVNMON");

        // Just one way to initialize TracerX early.
        private static bool LogFileOpened = InitLogging();

        
        // Initialize the TracerX logging system.
        private static bool InitLogging()
        {

            Logger.DefaultBinaryFile.Close();
            Logger.DefaultBinaryFile.Name = "SvnMonitoring";
            Logger.DefaultBinaryFile.Directory = "%EXEDIR%/Logs";
            Logger.DefaultBinaryFile.Use_00 = true;
            Logger.DefaultBinaryFile.Archives = 3;
            Logger.DefaultBinaryFile.CircularStartDelaySeconds = 0;
            Logger.DefaultBinaryFile.CircularStartSizeKb = 1;
            Logger.DefaultBinaryFile.MaxSizeMb = 2;
            Logger.DefaultBinaryFile.AppendIfSmallerThanMb = 0;

            // Open the output file.
            return Logger.DefaultBinaryFile.Open();
        }

        public static string RunExternalExe(string workdir,  string filename, string arguments = null)
        {
            var process = new Process();

            process.StartInfo.WorkingDirectory = workdir;
            process.StartInfo.FileName = filename;

            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            var stdOutput = new StringBuilder();
            process.OutputDataReceived += (sender, args) => stdOutput.Append(args.Data);

            string stdError = null;
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                stdError = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception("OS error while executing " + Format(filename, arguments) + ": " + e.Message, e);
            }

            if (process.ExitCode == 0)
            {
                return stdOutput.ToString();
            }
            else
            {
                var message = new StringBuilder();

                if (!string.IsNullOrEmpty(stdError))
                {
                    message.AppendLine(stdError);
                }

                if (stdOutput.Length != 0)
                {
                    message.AppendLine("Std output:");
                    message.AppendLine(stdOutput.ToString());
                }

                throw new Exception(Format(filename, arguments) + " finished with exit code = " + process.ExitCode + ": " + message);
            }
        }

        static string Format(string filename, string arguments)
        {
            return "'" + filename +
                ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
                "'";
        }


        public static void DebugInfo(string info)
        {
            Log.Info(info);
        }

        public static void InitVars()
        {
            if (lSharedState.Count == 0)
            {
                lSharedState.Add(false);
                lSharedState.Add(false);
            }
        }


        private static Expression ApplyFilter(string opr, Expression left, Expression right)
        {
            Expression InnerLambda = null;
            switch (opr)
            {
                case "==":
                case "=":
                case "eq":
                    InnerLambda = Expression.Equal(left, right);
                    break;
                case "<":
                case "lt":
                    InnerLambda = Expression.LessThan(left, right);
                    break;
                case ">":
                case "gt":
                    InnerLambda = Expression.GreaterThan(left, right);
                    break;
                case ">=":
                case "gte":
                    InnerLambda = Expression.GreaterThanOrEqual(left, right);
                    break;
                case "<=":
                case "lte":
                    InnerLambda = Expression.LessThanOrEqual(left, right);
                    break;
                case "!=":
                case "ne":
                    InnerLambda = Expression.NotEqual(left, right);
                    break;
                case "&&":
                case "and":
                    InnerLambda = Expression.And(left, right);
                    break;
                case "||":
                case "or":
                    InnerLambda = Expression.Or(left, right);
                    break;
                case "like":
                    InnerLambda = Expression.Call(left, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), right);
                    break;
                case "notlike":
                    InnerLambda = Expression.Not(Expression.Call(left, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), right));
                    break;
            }
            return InnerLambda;
        }

        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        static readonly double MaxUnixSeconds = (DateTime.MaxValue - UnixEpoch).TotalSeconds;

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dtValue = unixTimeStamp > MaxUnixSeconds
               ? UnixEpoch.AddMilliseconds(unixTimeStamp)
               : UnixEpoch.AddSeconds(unixTimeStamp);

            return dtValue.ToLocalTime();
        }

        public static IQueryable<T> FilteringHelper<T>(IQueryable<T> source, SearchViewModel paramRequest)
        {

            var filterParam = paramRequest.BuildFilter();

            if (filterParam != null)
            {
                IQueryable<T> queryable = source;
                ParameterExpression expression = Expression.Parameter(typeof(T), string.Empty);
                List<Expression> list = new List<Expression>();
                for (int i = 0; i < filterParam.Count; i++)
                {
                    var param = filterParam[i];

                    if (!string.IsNullOrWhiteSpace(param.value))
                    {
                        try
                        {
                            string value = param.value;
                            PropertyInfo propertyInfo = source.First().GetType().GetProperty(param.property);
                            MemberExpression instance = Expression.Property(expression, propertyInfo);
                            var dataType = propertyInfo.PropertyType.FullName.ToLower();

                            if (dataType.Contains("string"))
                            {
                                list.Add(ApplyFilter(param.op, instance, Expression.Constant(value)));
                            }
                            else if (dataType.Contains("datetime"))
                            {
                                list.Add(ApplyFilter(param.op, instance, Expression.Constant((UnixTimeStampToDateTime(Convert.ToDouble(value))))));
                            }
                            else if (dataType.Contains("system.int"))
                            {
                                if (value.ToLower() == "true")
                                {
                                    value = "1";
                                }
                                else if (value.ToLower() == "false")
                                {
                                    value = "0";
                                }

                                list.Add(ApplyFilter(param.op, instance, Expression.Constant(Convert.ToInt32(value))));
                            }
                            else if (dataType.Contains("bool"))
                            {
                                list.Add(ApplyFilter(param.op, instance, Expression.Constant(Convert.ToBoolean(value))));
                            }
                            else
                            {
                                list.Add(ApplyFilter(param.op, instance, Expression.Constant(Convert.ToDouble(value))));
                            }

                        }
                        catch (Exception ex)
                        {
                            MyShared.DebugInfo(ex.Message);
                        }

                    }
                }
                if (list.Count < 1)
                {
                    return queryable;
                }

                Expression[] expressionArray = list.ToArray();
                Expression left = expressionArray[0];
                for (int j = 1; j < expressionArray.Length; j++)
                {
                    left = Expression.And(left, expressionArray[j]);
                }
                Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>(left, new ParameterExpression[] { expression });
                return queryable.Where<T>(predicate);
            }

            return source;

        }
        
        public static GridResult<T> OrderingHelper<T>(IQueryable<T> source, SearchViewModel paramRequest, bool anotherLevel = false)
        {
            var sortParams = paramRequest.BuildSort();

            Sorting sortParam = sortParams[0];
            ParameterExpression param = Expression.Parameter(typeof(T), string.Empty); // I don't care about some naming
            MemberExpression property = Expression.PropertyOrField(param, sortParam.property);
            LambdaExpression sort = Expression.Lambda(property, param);
            bool descending = sortParam.direction == "DESC" ? true : false;

            IQueryable<T> queryable = FilteringHelper<T>(source, paramRequest);

            int TotalCount = queryable.Count<T>();

            MethodCallExpression call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                queryable.Expression,
                Expression.Quote(sort));

            var data = (IOrderedQueryable<T>)queryable.Provider.CreateQuery<T>(call);

            if (sortParams.Count > 1)
            {
                for (int i = 1; i < sortParams.Count; i++)
                {
                    sortParam = sortParams[i];
                    property = Expression.PropertyOrField(param, sortParam.property);
                    sort = Expression.Lambda(property, param);
                    descending = sortParam.direction == "DESC" ? true : false;

                    call = Expression.Call(
                        typeof(Queryable), ("ThenBy") + (descending ? "Descending" : string.Empty),
                        new[] { typeof(T), property.Type }, data.Expression,
                        Expression.Quote(sort));

                    data = (IOrderedQueryable<T>)data.Provider.CreateQuery<T>(call);
                }
            }

            return new GridResult<T> { total = TotalCount, data = data.Skip(paramRequest.start).Take(paramRequest.limit) };
        }
        
        public static async Task SendErrorLog(string subject, string Description, string DealerCode = "", string TableName = "", string fileName = "")
        {

            string EmailServer = System.Configuration.ConfigurationManager.AppSettings["emailserver"].ToString();
            string UserId = System.Configuration.ConfigurationManager.AppSettings["emailuserid"].ToString();
            string Password = System.Configuration.ConfigurationManager.AppSettings["emailpassword"].ToString();
            string Domain = System.Configuration.ConfigurationManager.AppSettings["domain"].ToString();
            string ReportTo = System.Configuration.ConfigurationManager.AppSettings["reportTo"].ToString();
            string emailFrom = System.Configuration.ConfigurationManager.AppSettings["emailFrom"].ToString();
            
            var Dest = ReportTo.Split(';');

            if (DealerCode != "" && TableName != ""){
                using (SqlConnection CN = MyShared.Conn)
                {
                    CN.Open();
                    SqlCommand cmd = CN.CreateCommand();
                    cmd.CommandTimeout = 3600;
                    cmd.CommandText = "uspfn_SysDealerLogErrorNotification";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("@DealerCode", DealerCode);
                    cmd.Parameters.AddWithValue("@TableName", TableName);
                    cmd.Parameters.AddWithValue("@ErrorDesc", Description);

                    cmd.ExecuteNonQuery();
                }
            }

            MailMessage message = new MailMessage();

            try
            {
                foreach (var sendTo in Dest)
                {
                    message.To.Add(sendTo);
                }

                message.Subject = subject;
                message.Body = Description;

                if (fileName != "")
                {

                    var htmlView = AlternateView.CreateAlternateViewFromString(Description + Environment.NewLine, null, "text/html");
                    message.AlternateViews.Add(htmlView);

                    message.Attachments.Add(new Attachment(fileName));   
                    message.IsBodyHtml = true;
                                 
                }

                message.From = new MailAddress(emailFrom);
                SmtpClient smtp = new SmtpClient(EmailServer);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(UserId, Password, Domain);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                smtp.Dispose();
                message.Dispose();
            }
            catch (Exception ex)
            {
                Log.Info(ex.Message);
            }

        }
        
        public static bool State
        {
            get
            {
                InitVars();
                return lSharedState[0];
            }
            set
            {
                InitVars();
                lSharedState[0] = value;
            }
        }

        public static bool State2
        {
            get
            {
                InitVars();
                return lSharedState[1];
            }
            set
            {
                InitVars();
                lSharedState[1] = value;
            }
        }

        public static string ServiceName
        {
            get
            {
                return "SVNMON";
            }
        }

        public static string ServiceDesc
        {
            get
            {
                return "Svn Monitoring";
            }
        }      

        public static SqlConnection Conn
        {
            get
            {
                return new SqlConnection((System.Configuration.ConfigurationManager.ConnectionStrings["SimDMS"].ToString()));
            }
        }

        public static string extractDir
        {
            get
            {
                string Url = System.Configuration.ConfigurationManager.AppSettings["extractdir"].ToString();
                Url = Url.Replace(@"{APPPATH}", Environment.CurrentDirectory);
                return Url;
            }
        }

        public static string uploadDir
        {
            get
            {
                string Url = System.Configuration.ConfigurationManager.AppSettings["uploaddir"].ToString();
                Url = Url.Replace(@"{APPPATH}", Environment.CurrentDirectory);
                return Url;
            }
        }


        public static async Task<Boolean> MergeProcess(SqlConnection CN,  DataRow row)
        {
            string UploadID = row["UploadID"].ToString();
            string fileName = row["FileName"].ToString();
            string filePath = row["FilePath"].ToString();
            string filedb = fileName.Replace(".zip", ".db3");

            var b = fileName.Split('_');

            try
            {
                string tablename = b[3];
                string optDB = b[1];
                bool IsWIP = false;
                string sql = "select tablename from SysDealerTWIP where tablename='" + tablename + "'";

                var db = new SqlConnection((System.Configuration.ConfigurationManager.ConnectionStrings["SimDMS" + optDB].ToString()));
                SqlCommand cmd = null;

                try
                {
                    db.Open();                    
                    cmd = new SqlCommand(sql, db);
                    var obj = cmd.ExecuteScalar();

                    if (obj == null)
                    {
                        sql = "select uploadid from SysDealerWIP where uploadid='" + UploadID + "'";
                        cmd.CommandText = sql;
                        var obj2 = cmd.ExecuteScalar();

                        if (obj2 == null)
                        {
                            if (Worker.Count < 10)
                            {
                                sql = "insert into SysDealerTWIP(tablename) values('" + tablename + "')";
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();

                                sql = "insert into SysDealerWIP(uploadid) values('" + UploadID + "')";
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();

                                Worker.Add(UploadID);
                            }
                            else
                            {
                                IsWIP = true;
                            }
                        }
                        else
                        {
                            IsWIP = true;
                        }

                    }
                    else
                    {
                        IsWIP = true;
                    }                    
                }
                catch (Exception ex)
                {
                    Log.Info("Error when connect to storage db: " + ex.Message);
                    return false;
                }



                if (IsWIP)
                {
                    try
                    {
                        db.Close();
                        db.Dispose();
                    } catch(Exception ex)
                    {
                        Log.Info("Error when closing db: " + ex.Message);
                    }
                    return false;
                }

                string fileNameDb = MyShared.extractDir + filedb;

                if (File.Exists(fileNameDb))
                {
                    GC.Collect();
                    try
                    {
                        File.Delete(fileNameDb);
                        MyShared.Log.Info("File Exists. Delete existing file successfull >> " + fileNameDb);
                    }
                    catch (Exception ex)
                    {
                        MyShared.Log.Warn("File Exists. Could not delete a file >> " + fileNameDb + " >> " + ex.Message);
                    }
                }

                MyShared.ExtractZipFile(MyShared.uploadDir + filePath, MyShared.extractDir);
               
                MyShared.MergeDatabase(MyShared.extractDir + filedb, optDB, CN, UploadID);

                sql = "delete from SysDealerWIP where uploadid = '" + UploadID + "'";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                sql = "delete from SysDealerTWIP where tablename = '" + tablename + "'";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                Worker.RemoveAt(0);

                if (db.State == ConnectionState.Open)
                {
                    db.Close();
                }

                db.Dispose();

                RunMergeProcess();

            }
            catch (Exception ex)
            {
                Log.Warn("Error on extract or merge database: " + ex.Message + " >> " + filedb);
                return false;
            }

            return true;
        }


        public static async Task RunMergeProcess2()
        {

            int nPending = 0;

            try
            {

                Log.Info("Function RunMergeProcess - Called");

                if (State2)
                {
                    Log.Info("Merge process already run");
                    return;
                }

                await Task.Delay(5000);

                if (State2)
                {
                    Log.Info("Merge process already run");
                    return;
                }

                State2 = true;
                
                using (SqlConnection CN = MyShared.Conn)
                {

                    await CN.OpenAsync();

                    var SQL = "EXEC [uspfn_SysDealerHistGet32] 22";
                    SqlCommand cmd = new SqlCommand(SQL, CN);
                    var dba = new SqlDataAdapter(cmd);

                    var dt = new DataTable();
                    dba.Fill(dt);

                    int nRow = dt.Rows.Count;

                    if (nRow > 0)
                    {

                        Log.Info("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Log.Info("Merge All - Started...");
                        Log.Info("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

                        for (int i = 0; i < nRow; i++)
                        {
                            //MergeProcess(CN, dt.Rows[i]);

                            string fileName = dt.Rows[i]["FileName"].ToString();
                            string filePath = dt.Rows[i]["FilePath"].ToString();
                            string filedb = fileName.Replace(".zip", ".db3");

                            try
                            {
                                string fileNameDb = MyShared.extractDir + filedb;
                                if (File.Exists(fileNameDb))
                                {
                                    GC.Collect();
                                    try
                                    {
                                        File.Delete(fileNameDb);
                                        MyShared.Log.Info("File Exists. Delete existing file successfull >> " + fileNameDb);
                                    }
                                    catch (Exception ex)
                                    {
                                        MyShared.Log.Warn("File Exists. Could not delete a file >> " + fileNameDb + " >> " + ex.Message);
                                    }
                                }
                                MyShared.ExtractZipFile(MyShared.uploadDir + filePath, MyShared.extractDir);
                                string optDB = fileName.Split('_')[1].ToString();
                                MyShared.MergeDatabase(MyShared.extractDir + filedb, optDB, CN, dt.Rows[i]["UploadId"].ToString());
                            }
                            catch (Exception ex)
                            {
                                Log.Warn("Error on extract or merge database: " + ex.Message + " >> " + filedb);
                            }
                        }

                        Log.Info("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Log.Info("Merge All - Done!");
                        Log.Info("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

                        cmd.CommandText = "EXEC master..sp_checkSIMDMSLogFile";
                        cmd.ExecuteNonQuery();

                        SQL = "EXEC [uspfn_SysDealerHistGet32] 10";
                        cmd = new SqlCommand(SQL, CN);
                        dba = new SqlDataAdapter(cmd);

                        dt = new DataTable();
                        dba.Fill(dt);

                        nPending = dt.Rows.Count;

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Error On Run Merge Process: " + ex.Message);
                SendErrorLog("[Error Notification] Merger Proccess", ex.Message , "", "");
            }
            finally
            {
                GC.Collect();
                State2 = false;
                if (nPending>0)
                {
                    RunMergeProcess2();
                }
            }

        }

        public static async Task PostSvnUpdateInfo(FormUrlEncodedContent data)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = System.Configuration.ConfigurationManager.AppSettings["updatesvninfo"].ToString();
                MyShared.Log.Info("Goto " + url);
                HttpResponseMessage response = await client.PostAsync(new Uri(url), data);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                MyShared.Log.Info(responseBody);
            }
            catch (Exception ex)
            {
                MyShared.Log.Info(ex.Message);
            }
        }

        public static async Task RunSvnUpdate()
        {
            try
            {

                Log.Info("Function RunMergeProcess - Called");

                await Task.Delay(1000);
                
                if (State)
                {
                    Log.Info("Merge process already run");
                    return;
                }

                await Task.Delay(2000);

                State = true;

                SimDMSDB db = new SimDMSDB();

                Log.Info("Get list of repository");
                var data = db.SysRepositories.ToList();

                Log.Info("number of repository: " + data.Count.ToString());

                foreach (var item in data)
                {

                    string fileNameDb = item.PathName + @".svn\wc.db";
                    Log.Info("db svn: " + fileNameDb);
                    if (File.Exists(fileNameDb))
                    {
                        try
                        {
                            using (SQLiteConnection db3 = new SQLiteConnection())
                            {
                                 db3.ConnectionString = "Data Source=" + fileNameDb;
                                 db3.Open();
                                 using (SQLiteCommand cmd = db3.CreateCommand())
                                 {
                                     cmd.CommandText = "delete from wc_lock";
                                     cmd.ExecuteNonQuery();
                                 }
                                 db3.Close();                                    
                            }
                        }
                        catch (Exception ex)
                        {
                            MyShared.Log.Warn(ex.Message);
                        }
                    }


                    try
                    {
                        Log.Info("starting call svn update on: " + item.PathName);
                        string msg = MyShared.RunExternalExe(item.PathName, "svn", "update --username sdms --password sdms --trust-server-cert --non-interactive");
                        MyShared.Log.Info(item.PathName + " svn update");
                        string revision = msg.Substring(msg.LastIndexOf(' ') + 1);
                        item.LastMessage = msg;
                        item.Revision = revision;
                        item.LastUpdate = DateTime.Now;
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        Log.Info("save svn update info # " + revision);
                        db.SaveChanges();

                        var pairs = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("dealercode", item.CompanyCode),
                            new KeyValuePair<string, string>("lastupdate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                            new KeyValuePair<string, string>("revisi", revision.Replace(".","")),
                            new KeyValuePair<string, string>("message", msg)
                        };

                        var content = new FormUrlEncodedContent(pairs);

                        PostSvnUpdateInfo(content);

                    }
                    catch (Exception ex)
                    {
                        MyShared.Log.Info(ex.Message);
                    }
                }
             
            } catch (Exception ex)
            {
                Log.Info("Error on Run SVN Update: " + ex.Message);
                
            }

            State = false;
        }

        public static async Task RunMergeProcess()
        {
            int nPending = 0;

            try
            {

                Log.Info("Function RunMergeProcess - Called");

                if (State)
                {
                    Log.Info("Merge process already run");
                    return;
                }

                await Task.Delay(5000);

                if (State)
                {
                    Log.Info("Merge process already run");
                    return;
                }

                State = true;

                using (SqlConnection CN = MyShared.Conn)
                {

                    await CN.OpenAsync();

                    var SQL = "EXEC [uspfn_SysDealerHistGet3] 22";
                    SqlCommand cmd = new SqlCommand(SQL, CN);
                    var dba = new SqlDataAdapter(cmd);

                    var dt = new DataTable();
                    dba.Fill(dt);

                    int nRow = dt.Rows.Count;

                    if (nRow > 0)
                    {

                        Log.Info("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Log.Info("Merge All - Started...");
                        Log.Info("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

                        for (int i = 0; i < nRow; i++)
                        {
                            //MergeProcess(CN, dt.Rows[i]);

                            string fileName = dt.Rows[i]["FileName"].ToString();
                            string filePath = dt.Rows[i]["FilePath"].ToString();
                            string filedb = fileName.Replace(".zip", ".db3");

                            try
                            {
                                string fileNameDb = MyShared.extractDir + filedb;
                                if (File.Exists(fileNameDb))
                                {
                                    GC.Collect();
                                    try
                                    {
                                        File.Delete(fileNameDb);
                                        MyShared.Log.Info("File Exists. Delete existing file successfull >> " + fileNameDb);
                                    }
                                    catch (Exception ex)
                                    {
                                        MyShared.Log.Warn("File Exists. Could not delete a file >> " + fileNameDb + " >> " + ex.Message);
                                    }
                                }
                                MyShared.ExtractZipFile(MyShared.uploadDir + filePath, MyShared.extractDir);
                                string optDB = fileName.Split('_')[1].ToString();
                                MyShared.MergeDatabase(MyShared.extractDir + filedb, optDB, CN, dt.Rows[i]["UploadId"].ToString());
                            }
                            catch (Exception ex)
                            {
                                Log.Warn("Error on extract or merge database: " + ex.Message + " >> " + filedb);
                            }
                        }

                        Log.Info("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Log.Info("Merge All - Done!");
                        Log.Info("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

                        cmd.CommandText = "EXEC master..sp_checkSIMDMSLogFile";
                        cmd.ExecuteNonQuery();

                        SQL = "EXEC [uspfn_SysDealerHistGet3] 10";
                        cmd = new SqlCommand(SQL, CN);
                        dba = new SqlDataAdapter(cmd);

                        dt = new DataTable();
                        dba.Fill(dt);

                        nPending = dt.Rows.Count;

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Error On Run Merge Process: " + ex.Message);
                SendErrorLog("[Error Notification] Merger Proccess", ex.Message, "", "");
            }
            finally
            {
                GC.Collect();
                State = false;
                if (nPending > 0)
                {
                    RunMergeProcess();
                }
            }

        }

        private static string GenerateColumnsList(DataTable dtX)
        {
            string listColumns = "";
            int nColumns = dtX.Columns.Count;

            for (int iCol = 0; iCol < nColumns; iCol++)
            {
                if (listColumns == "")
                    listColumns += dtX.Columns[iCol].ColumnName;
                else
                    listColumns += "," + dtX.Columns[iCol].ColumnName;
            }
            return listColumns;
        }

        public static int MergeDatabase(string filedb, string optDB, SqlConnection historyDB, string uploadID)
        {
            int totalRowChanges = 0;
            string DealerCode = "";
            string tableName = "";
            bool HasError = false;

            using (SQLiteConnection db3 = new SQLiteConnection())
            {
                var db = new SqlConnection((System.Configuration.ConfigurationManager.ConnectionStrings["SimDMS" + optDB].ToString()));

                try
                {
                    db.Open();
                }
                catch (Exception ex)
                {
                    Log.Info("Error when connect to storage db: " + ex.Message);
                }

                if (db.State != ConnectionState.Open)
                {
                    return totalRowChanges;
                }

                var cmdHst = historyDB.CreateCommand();

                try
                {

                    Log.Info("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * ");
                    Log.Info("Starting Merge Data from " + filedb);


                    cmdHst.CommandText = "EXEC [uspfn_SysDealerHistUpdStatus2] '" + uploadID + "',0";
                    cmdHst.ExecuteNonQuery();

                    if (db3 != null)
                    {
                        db3.ConnectionString = "Data Source=" + filedb + ";BinaryGuid=true";
                        db3.Open();
                        using (SQLiteCommand cmd = db3.CreateCommand())
                        {
                            cmd.CommandText = "select * from BACKUP_INFO order by name";
                            var dba = new SQLiteDataAdapter(cmd);
                            var dt = new DataTable();
                            dba.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    Log.Info("Dealer Code: " + dt.Rows[i]["code"].ToString() + ", Table Name: " + dt.Rows[i]["name"].ToString() + " , Count: " + dt.Rows[i]["count"].ToString() + " , Last Update: " + dt.Rows[i]["lastupdate"].ToString());
                                    DealerCode = dt.Rows[i]["code"].ToString();

                                    int CountUpdate = Convert.ToInt32(dt.Rows[i]["count"]);
                                    if (CountUpdate > 0)
                                    {
                                        tableName = dt.Rows[i]["name"].ToString();



                                        string tempTable = "#" + tableName + DateTime.Now.ToString("yyyyMMddHHmmss");
                                        var cmdDB = db.CreateCommand();

                                        try
                                        {
                                            string listColumns = "";

                                            var cmdT = db3.CreateCommand();
                                            if ("OmHstInquirySales" == tableName)
                                            {
                                                cmdT.CommandText = "update OmHstInquirySales set AfterDiscTotal=0 where AfterDiscTotal IS NULL";
                                                cmdT.ExecuteNonQuery();

                                                string strGetDoubleId = "select group_concat(substr(id,instr(id,',') + 1)) id from (SELECT count(*) A, group_concat(rowid) id, [Year] ,[Month] ,[CompanyCode] ,[BranchCode] ,[BranchHeadID] ,[SoNo] ,[SODate] ,[InvoiceNo] ,[InvoiceDate] , [ChassisCode] ,[ChassisNo] ,[EngineCode] ,[EngineNo] , [ColourCode] ,[AfterDiscTotal] , [LastUpdateDate] FROM [OmHstInquirySales] group by [Year] ,[Month] ,[CompanyCode] ,[BranchCode] ,[BranchHeadID] ,[SoNo] ,[SODate] ,[InvoiceNo] ,[InvoiceDate] , [ChassisCode] ,[ChassisNo] ,[EngineCode] ,[EngineNo] , [ColourCode] ,[AfterDiscTotal] , [LastUpdateDate] ) a where a > 1 --";
                                                cmdT.CommandText = strGetDoubleId;
                                                string listId = cmdT.ExecuteScalar().ToString();

                                                if (!string.IsNullOrEmpty(listId))
                                                {
                                                    cmdT.CommandText = "select count(*) from OmHstInquirySales";
                                                    Log.Info("Before process : " + cmdT.ExecuteScalar());

                                                    Log.Info("List of double Id " + listId);

                                                    cmdT.CommandText = "delete from OmHstInquirySales where rowid in (" + listId + ")";
                                                    cmdT.ExecuteNonQuery();

                                                    cmdT.CommandText = "select count(*) from OmHstInquirySales";
                                                    Log.Info("After process : " + cmdT.ExecuteScalar());
                                                }

                                            }
                                            else if (tableName == "spMstItems")
                                            {
                                                cmdT.CommandText = "update spMstItems set PurcDiscPct=0 where PurcDiscPct IS NULL";
                                                cmdT.ExecuteNonQuery();
                                            }

                                            cmdT.CommandText = "select * from " + tableName + " limit 0,1";
                                            var dbaX = new SQLiteDataAdapter(cmdT);
                                            var dtX = new DataTable();
                                            dbaX.Fill(dtX);


                                            listColumns = GenerateColumnsList(dtX);

                                            cmdDB.CommandText = "EXEC uspfn_SysDealerMergeData '" + dt.Rows[i]["code"].ToString() + "','" + tableName + "','" + tempTable + "','" + listColumns + "'";

                                            //Log.Info(cmdDB.CommandText);
                                            var dbaMerge = new SqlDataAdapter(cmdDB);
                                            var dtMerge = new DataTable();
                                            dbaMerge.Fill(dtMerge);

                                            if (dtMerge.Rows.Count == 1)
                                            {

                                                Log.Info("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                                                Log.Info("Start processing table data of " + tableName);

                                                string MergeSQL = dtMerge.Rows[0]["SQL"].ToString();
                                                listColumns = dtMerge.Rows[0][1].ToString();
                                                cmdDB.CommandTimeout = 1800;
                                                cmdDB.CommandText = "SELECT TOP 0 " + listColumns + " INTO " + tempTable + " from " + tableName;
                                                Log.Info("Create temporary table on target database  " + tempTable);
                                                Log.Info(cmdDB.CommandText);
                                                cmdDB.ExecuteNonQuery();

                                                Log.Info("Get all data from table on backup file (sqlite3 database)");

                                                //dtX.Columns[0].DataType = typeof(Guid);
                                                bool Success = true;

                                                int nRowCapacity = 1000;
                                                int nPageSize = CountUpdate / nRowCapacity;

                                                var DefaultColumns = listColumns;

                                                for (int nPage = 0; nPage <= nPageSize; nPage++)
                                                {

                                                    cmdT.CommandText = "select " + DefaultColumns + " from " + tableName + " limit " + (nPage * nRowCapacity).ToString() + ", " + nRowCapacity.ToString();
                                                    dbaX = new SQLiteDataAdapter(cmdT);

                                                    if (dtX != null)
                                                    {
                                                        dtX.Columns.Clear();
                                                        dtX = null;
                                                    }
                                                    dtX = new DataTable();
                                                    dbaX.Fill(dtX);
                                                    Log.Info(cmdT.CommandText);

                                                    using (SqlBulkCopy bcp = new SqlBulkCopy(db))
                                                    {
                                                        Log.Info("Start copying data from sqlite3 table into temp table (" + tempTable + ")");
                                                        bcp.DestinationTableName = tempTable;
                                                        bcp.NotifyAfter = dtX.Rows.Count;
                                                        bcp.BatchSize = dtX.Rows.Count;
                                                        bcp.BulkCopyTimeout = 3600;
                                                        try
                                                        {
                                                            bcp.WriteToServer(dtX);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Success = false;

                                                            Log.Warn("ERROR >> " + ex.Message);

                                                            if (ex.Message.ToLower().Contains("uniqueidentifier") || tableName.ToLower() == "sphstpartsales")
                                                            {
                                                                Log.Info("change data type to uniqueidentifier");
                                                                DataColumn newcolumn = new DataColumn(dtX.Columns[0].Caption, typeof(Guid));
                                                                dtX.Columns.Remove(dtX.Columns[0].Caption);
                                                                dtX.Columns.Add(newcolumn);
                                                                var listColumns2 = GenerateColumnsList(dtX);
                                                                cmdDB.CommandTimeout = 1800;
                                                                cmdDB.CommandText = "DROP TABLE " + tempTable + "; SELECT TOP 0 " + listColumns2 + " INTO " + tempTable + " from " + tableName;
                                                                cmdDB.ExecuteNonQuery();

                                                                DataTable dtC = new DataTable();
                                                                dbaX.Fill(dtC);

                                                                int nRowsCopy = dtC.Rows.Count;

                                                                for (int iRow = 0; iRow < nRowsCopy; iRow++)
                                                                {
                                                                    dtX.Rows[iRow][newcolumn.Caption] = dtC.Rows[iRow][newcolumn.Caption];
                                                                }

                                                                Success = true;

                                                                try
                                                                {
                                                                    bcp.WriteToServer(dtX);
                                                                }
                                                                catch (Exception ex2)
                                                                {
                                                                    Success = false;
                                                                    HasError = true;
                                                                    Log.Warn(ex.HResult.ToString() + " >> " + ex2.Message);
                                                                    SendErrorLog("[Error Notification] Write Bulk Copy 2", "Dealer Code: " + DealerCode + Environment.NewLine + ex2.Message, DealerCode, tableName);
                                                                }

                                                            }
                                                            else
                                                            {
                                                                Success = false;
                                                                HasError = true;
                                                                Log.Warn(ex.HResult.ToString() + " >> " + ex.Message);
                                                                SendErrorLog("[Error Notification] Write Bulk Copy 1", "Dealer Code: " + DealerCode + Environment.NewLine + "Table Name: " + tableName + Environment.NewLine + ex.Message, DealerCode, tableName);
                                                            }
                                                        }
                                                        // Log.Info("Data has been copied (sqlite3 table to temp table)");
                                                    }



                                                    if (Success)
                                                    {
                                                        Log.Info("Start to mergering data from temp table to real table");

                                                        try
                                                        {
                                                            cmdDB.CommandTimeout = 1800;
                                                            cmdDB.CommandText = MergeSQL;
                                                            cmdDB.ExecuteNonQuery();
                                                            //Log.Info(MergeSQL);

                                                            cmdDB.CommandText = "select @@ROWCOUNT";
                                                            int rowChanges = Convert.ToInt32(cmdDB.ExecuteScalar());

                                                            Log.Info(rowChanges + " row(s) changed");
                                                            totalRowChanges += rowChanges;

                                                        }
                                                        catch (Exception ExM)
                                                        {
                                                            Success = false;
                                                            HasError = true;
                                                            Log.Warn("Error: " + ExM.Message);

                                                            cmdDB.CommandText = "DROP TABLE " + tempTable;
                                                            Log.Info(cmdDB.CommandText);
                                                            cmdDB.ExecuteNonQuery();

                                                            SendErrorLog("[Error Notification] Merger Data", "Dealer Code: " + DealerCode + Environment.NewLine + ExM.Message +
                                                                Environment.NewLine + Environment.NewLine + MergeSQL, DealerCode, tableName
                                                             );
                                                        }

                                                    }
                                                }

                                                if (Success)
                                                {

                                                    cmdDB.CommandText = "Update SysDealer Set LastUpdate='" + dt.Rows[i]["lastupdate"].ToString() + "' "
                                                                        + "Where DealerCode='" + dt.Rows[i]["code"].ToString() + "' AND TableName='" + dt.Rows[i]["name"].ToString() + "'";
                                                    //Log.Info(cmdDB.CommandText);
                                                    cmdDB.ExecuteNonQuery();

                                                    cmdDB.CommandText = "DROP TABLE " + tempTable;
                                                    Log.Info(cmdDB.CommandText);
                                                    cmdDB.ExecuteNonQuery();
                                                }



                                                //Log.Info("Merge data finished");

                                                cmdT.Dispose();
                                                dbaX.Dispose();

                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            HasError = true;
                                            Log.Warn("Error on merge data: " + ex.Message);
                                            SendErrorLog("[Error Notification] Create Temp Table ", "Dealer Code: " + DealerCode + Environment.NewLine + "Table Name: " + tableName + Environment.NewLine + ex.Message, DealerCode, tableName);
                                        }

                                        Log.Info("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");

                                    }
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("Error On Merge Database: " + ex.Message);
                    HasError = true;
                    SendErrorLog("[Error Notification] Merger Database", ex.Message, DealerCode, tableName);
                }
                finally
                {
                    if (db3 != null)
                    {
                        db3.Close();
                        db3.Dispose();
                        GC.Collect();
                        if (!HasError)
                        {
                            DeleteFile(filedb);
                        }
                    }

                    db.Close();
                    db = null;

                    cmdHst.CommandText = "EXEC [uspfn_SysDealerHistUpdStatus2] '" + uploadID + "',1";
                    cmdHst.ExecuteNonQuery();


                }

                Log.Info("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * " + totalRowChanges + " row(s) affected * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * ");
                return totalRowChanges;
            }
        }

        public static int MergeDatabaseDummy(string filedb, string optDB, SqlConnection historyDB, string uploadID, out string SQLMerge)
        {
            int totalRowChanges = 0;
            string DealerCode = "";
            string tableName = "";
            bool HasError = false;
            SQLMerge = "";

            using (SQLiteConnection db3 = new SQLiteConnection())
            {
                var db = new SqlConnection((System.Configuration.ConfigurationManager.ConnectionStrings["SimDMS" + optDB].ToString()));

                try
                {
                    db.Open();
                }
                catch (Exception ex)
                {
                    Log.Info("Error when connect to storage db: " + ex.Message);
                }

                if (db.State != ConnectionState.Open)
                {
                    return totalRowChanges;
                }

                var cmdHst = historyDB.CreateCommand();

                try
                {

                    Log.Info("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * ");
                    Log.Info("Starting Merge Data from " + filedb);


                    cmdHst.CommandText = "EXEC [uspfn_SysDealerHistUpdStatus2] '" + uploadID + "',0";
                    cmdHst.ExecuteNonQuery();

                    if (db3 != null)
                    {
                        db3.ConnectionString = "Data Source=" + filedb + ";BinaryGuid=true";
                        db3.Open();
                        using (SQLiteCommand cmd = db3.CreateCommand())
                        {
                            cmd.CommandText = "select * from BACKUP_INFO order by name";
                            var dba = new SQLiteDataAdapter(cmd);
                            var dt = new DataTable();
                            dba.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    Log.Info("Dealer Code: " + dt.Rows[i]["code"].ToString() + ", Table Name: " + dt.Rows[i]["name"].ToString() + " , Count: " + dt.Rows[i]["count"].ToString() + " , Last Update: " + dt.Rows[i]["lastupdate"].ToString());
                                    DealerCode = dt.Rows[i]["code"].ToString();

                                    int CountUpdate = Convert.ToInt32(dt.Rows[i]["count"]);
                                    if (CountUpdate > 0)
                                    {
                                        tableName = dt.Rows[i]["name"].ToString();



                                        string tempTable = "DUMMY_" + tableName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                                        var cmdDB = db.CreateCommand();

                                        try
                                        {
                                                string listColumns = "";

                                                var cmdT = db3.CreateCommand();
                                                if ("OmHstInquirySales" == tableName)
                                                {
                                                    cmdT.CommandText = "update OmHstInquirySales set AfterDiscTotal=0 where AfterDiscTotal IS NULL";
                                                    cmdT.ExecuteNonQuery();

                                                    string strGetDoubleId = "select group_concat(substr(id,instr(id,',') + 1)) id from (SELECT count(*) A, group_concat(rowid) id, [Year] ,[Month] ,[CompanyCode] ,[BranchCode] ,[BranchHeadID] ,[SoNo] ,[SODate] ,[InvoiceNo] ,[InvoiceDate] , [ChassisCode] ,[ChassisNo] ,[EngineCode] ,[EngineNo] , [ColourCode] ,[AfterDiscTotal] , [LastUpdateDate] FROM [OmHstInquirySales] group by [Year] ,[Month] ,[CompanyCode] ,[BranchCode] ,[BranchHeadID] ,[SoNo] ,[SODate] ,[InvoiceNo] ,[InvoiceDate] , [ChassisCode] ,[ChassisNo] ,[EngineCode] ,[EngineNo] , [ColourCode] ,[AfterDiscTotal] , [LastUpdateDate] ) a where a > 1 --";
                                                    cmdT.CommandText = strGetDoubleId;
                                                    string listId = cmdT.ExecuteScalar().ToString();

                                                    if (!string.IsNullOrEmpty(listId))
                                                    {
                                                        cmdT.CommandText = "select count(*) from OmHstInquirySales";
                                                        Log.Info("Before process : " + cmdT.ExecuteScalar());

                                                        Log.Info("List of double Id " + listId);

                                                        cmdT.CommandText = "delete from OmHstInquirySales where rowid in (" + listId + ")";
                                                        cmdT.ExecuteNonQuery();

                                                        cmdT.CommandText = "select count(*) from OmHstInquirySales";
                                                        Log.Info("After process : " + cmdT.ExecuteScalar());
                                                    }

                                                }
                                                else if (tableName == "spMstItems")
                                                {
                                                    cmdT.CommandText = "update spMstItems set PurcDiscPct=0 where PurcDiscPct IS NULL";
                                                    cmdT.ExecuteNonQuery();
                                                }

                                                cmdT.CommandText = "select * from " + tableName + " limit 0,1";
                                                var dbaX = new SQLiteDataAdapter(cmdT);
                                                var dtX = new DataTable();
                                                dbaX.Fill(dtX);

                                                
                                                listColumns = GenerateColumnsList(dtX);                                                

                                                cmdDB.CommandText = "EXEC uspfn_SysDealerMergeData '" + dt.Rows[i]["code"].ToString() + "','" + tableName + "','" + tempTable + "','" + listColumns + "'";
                                            
                                            //Log.Info(cmdDB.CommandText);
                                            var dbaMerge = new SqlDataAdapter(cmdDB);
                                            var dtMerge = new DataTable();
                                            dbaMerge.Fill(dtMerge);

                                            if (dtMerge.Rows.Count == 1)
                                            {

                                                Log.Info("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                                                Log.Info("Start processing table data of " + tableName);

                                                SQLMerge = dtMerge.Rows[0]["SQL"].ToString();
                                                listColumns = dtMerge.Rows[0][1].ToString();
                                                cmdDB.CommandTimeout = 1800;
                                                cmdDB.CommandText = "SELECT TOP 0 " + listColumns + " INTO " + tempTable + " from " + tableName;
                                                Log.Info("Create temporary table on target database  " + tempTable);
                                                Log.Info(cmdDB.CommandText);
                                                cmdDB.ExecuteNonQuery();

                                                Log.Info("Get all data from table on backup file (sqlite3 database)");

                                                //dtX.Columns[0].DataType = typeof(Guid);
                                                bool Success = true;

                                                int nPageSize = CountUpdate / 50000;

                                                var DefaultColumns = listColumns;

                                                for (int nPage = 0; nPage <= nPageSize; nPage++)
                                                {

                                                    cmdT.CommandText = "select " + DefaultColumns + " from " + tableName + " limit " + (nPage * 50000).ToString() + ", 50000";
                                                    dbaX = new SQLiteDataAdapter(cmdT);

                                                    if (dtX != null)
                                                    {
                                                        dtX.Columns.Clear();
                                                        dtX = null;
                                                    }                                                    
                                                    dtX = new DataTable();
                                                    dbaX.Fill(dtX);
                                                    Log.Info(cmdT.CommandText);

                                                    using (SqlBulkCopy bcp = new SqlBulkCopy(db))
                                                    {
                                                        Log.Info("Start copying data from sqlite3 table into temp table (" + tempTable + ")");
                                                        bcp.DestinationTableName = tempTable;
                                                        bcp.NotifyAfter = dtX.Rows.Count;
                                                        bcp.BatchSize = dtX.Rows.Count;
                                                        bcp.BulkCopyTimeout = 3600;
                                                        try
                                                        {
                                                            bcp.WriteToServer(dtX);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Success = false;

                                                            Log.Warn("ERROR >> " + ex.Message);

                                                            if (ex.Message.ToLower().Contains("uniqueidentifier") || tableName.ToLower() == "sphstpartsales")
                                                            {
                                                                Log.Info("change data type to uniqueidentifier");
                                                                DataColumn newcolumn = new DataColumn(dtX.Columns[0].Caption, typeof(Guid));
                                                                dtX.Columns.Remove(dtX.Columns[0].Caption);
                                                                dtX.Columns.Add(newcolumn);
                                                                var listColumns2 = GenerateColumnsList(dtX);
                                                                cmdDB.CommandTimeout = 1800;
                                                                cmdDB.CommandText = "DROP TABLE " + tempTable + "; SELECT TOP 0 " + listColumns2 + " INTO " + tempTable + " from " + tableName;
                                                                cmdDB.ExecuteNonQuery();

                                                                DataTable dtC = new DataTable();
                                                                dbaX.Fill(dtC);

                                                                int nRowsCopy = dtC.Rows.Count;

                                                                for (int iRow = 0; iRow < nRowsCopy; iRow++)
                                                                {
                                                                    dtX.Rows[iRow][newcolumn.Caption] = dtC.Rows[iRow][newcolumn.Caption];
                                                                }

                                                                Success = true;

                                                                try
                                                                {
                                                                    bcp.WriteToServer(dtX);
                                                                }
                                                                catch (Exception ex2)
                                                                {
                                                                    Success = false;
                                                                    HasError = true;
                                                                    Log.Warn(ex.HResult.ToString() + " >> " + ex2.Message);
                                                                    SendErrorLog("[Error Notification] Write Bulk Copy 2", "Dealer Code: " + DealerCode + Environment.NewLine + ex2.Message, DealerCode, tableName);
                                                                }

                                                            }
                                                            else
                                                            {
                                                                Success = false;
                                                                HasError = true;
                                                                Log.Warn(ex.HResult.ToString() + " >> " + ex.Message);
                                                                SendErrorLog("[Error Notification] Write Bulk Copy 1", "Dealer Code: " + DealerCode + Environment.NewLine + "Table Name: " + tableName + Environment.NewLine + ex.Message, DealerCode, tableName);
                                                            }
                                                        }
                                                        // Log.Info("Data has been copied (sqlite3 table to temp table)");
                                                    }
                                                    
                                                }
                                                
                                                //Log.Info("Merge data finished");

                                                cmdT.Dispose();
                                                dbaX.Dispose();

                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            HasError = true;
                                            Log.Warn("Error on merge data: " + ex.Message);
                                            SendErrorLog("[Error Notification] Create Temp Table ", "Dealer Code: " + DealerCode + Environment.NewLine + "Table Name: " +  tableName + Environment.NewLine + ex.Message, DealerCode , tableName );
                                        }

                                        Log.Info("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");

                                    }
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("Error On Merge Database: " + ex.Message);
                    HasError = true;
                    SendErrorLog("[Error Notification] Merger Database", ex.Message, DealerCode , tableName );
                }
                finally
                {
                    if (db3 != null)
                    {
                        db3.Close();
                        db3.Dispose();
                        GC.Collect();
                        if (!HasError)
                        {
                            DeleteFile(filedb);
                        }
                    }

                    db.Close();
                    db = null;

                    cmdHst.CommandText = "EXEC [uspfn_SysDealerHistUpdStatus2] '" + uploadID + "',1";
                    cmdHst.ExecuteNonQuery();
                    

                }

                Log.Info("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * " + totalRowChanges  + " row(s) affected * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * ");
                return totalRowChanges;
            }
        }

        private static async Task DeleteFile(string fileName)
        {
            GC.Collect();

            await Task.Delay(15000);
            try
            {
                System.IO.File.Delete(fileName);
            }
            catch (Exception ex)
            {
                Log.Warn("Error 1 when deleting filedb: " + ex.Message);
                Task.Delay(15000).Wait();
                try
                {
                    System.IO.File.Delete(fileName);
                }
                catch (Exception eRR)
                {
                    Log.Warn("Error 2 when deleting filedb: " + eRR.Message);
                }
            }
        }

        public static void ExtractZipFile(string archiveFilenameIn, string outFolder, string password = "")
        {
            ZipFile zf = null;
            try
            {
                FileStream fs = File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);
                if (!String.IsNullOrEmpty(password))
                {
                    zf.Password = password;     // AES encrypted entries are handled automatically
                }
                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;           // Ignore directories
                    }
                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    byte[] buffer = new byte[4096];     // 4K is optimum
                    Stream zipStream = zf.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    String fullZipToPath = Path.Combine(outFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
        }

        public static string CreateDummyTable(string filedb, string optDB, string tableName)
        {
            string totalRowChanges = "";
            string DealerCode = "";
            bool HasError = false;
            int Rows = 0;


            using (SQLiteConnection db3 = new SQLiteConnection())
            {
                var db = new SqlConnection((System.Configuration.ConfigurationManager.ConnectionStrings["SimDMS" + optDB].ToString()));

                try
                {
                    db.Open();
                }
                catch (Exception ex)
                {
                    Log.Info("Error when connect to storage db: " + ex.Message);
                }

                if (db.State != ConnectionState.Open)
                {
                    return totalRowChanges;
                }


                try
                {                    

                    if (db3 != null)
                    {
                        db3.ConnectionString = "Data Source=" + filedb;
                        db3.Open();

                        using (SQLiteCommand cmd = db3.CreateCommand())
                        {
                            cmd.CommandText = "select * from " + tableName;
                            var dba = new SQLiteDataAdapter(cmd);
                            var dtX = new DataTable();
                            dba.Fill(dtX);


                            string tempTable = "AAA" + tableName + DateTime.Now.ToString("yyyyMMddHHmmss");
                            var cmdDB = db.CreateCommand();
                            cmdDB.CommandText = "SELECT TOP 0 * INTO " + tempTable + " from " + tableName;
                            cmdDB.ExecuteNonQuery();

                            Rows = dtX.Rows.Count;

                            using (SqlBulkCopy bcp = new SqlBulkCopy(db))
                            {                                
                                bcp.DestinationTableName = tempTable;
                                bcp.NotifyAfter = dtX.Rows.Count;
                                bcp.BatchSize = dtX.Rows.Count;
                                bcp.BulkCopyTimeout = 600;
                                try
                                {
                                    bcp.WriteToServer(dtX);
                                }
                                catch (Exception ex)
                                {                                    
                                    Log.Warn(ex.HResult.ToString() + " >> " + ex.Message);
                                }                                                    
                            }

                            string listColumns = GenerateColumnsList(dtX);
                            var dlCode = filedb.Split('_')[0];

                            cmdDB.CommandText = "EXEC uspfn_SysDealerMergeData '" + dlCode + "','" + tableName + "','" + tempTable + "','" + listColumns + "'";
                            //Log.Info(cmdDB.CommandText);
                            var dbaMerge = new SqlDataAdapter(cmdDB);
                            var dtMerge = new DataTable();
                            dbaMerge.Fill(dtMerge);

                            if (dtMerge.Rows.Count == 1)
                            {
                                return dtMerge.Rows[0]["SQL"].ToString();
                            }                           

                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("Error On Merge Database: " + ex.Message);
                    HasError = true;
                }
                finally
                {
                    if (db3 != null)
                    {
                        db3.Close();
                        db3.Dispose();
                        GC.Collect();
                    }

                    db.Close();
                    db = null;

                }

                return totalRowChanges;
            }
        }


    }
    
    public class DataGridBinding
    {
        private DataTable dt = null;
        private SqlDataAdapter dba = null;
        private BindingSource bindingSource1 = null;

        public string SQL { get; set; }

        public DataGridBinding(string sql)
        {
            SQL = sql;
            BuildData();
        }

        public void BuildData()
        {
            SqlCommand cmd = new SqlCommand(SQL, MyShared.Conn);
            dba = new SqlDataAdapter(cmd);
            dba.UpdateCommand = new SqlCommandBuilder(dba).GetUpdateCommand();
            dt = new DataTable();
            dba.Fill(dt);

            bindingSource1 = new BindingSource();
            bindingSource1.DataSource = dt;
        }

        public BindingSource SourceDataBinding
        {
            get
            {
                return bindingSource1;
            }
        }

        public void SaveData()
        {
            dba.Update(dt);
            MessageBox.Show("Save data finished!");
        }
         
    }

    public class Sorting
    {
        public string property { set; get; }
        public string direction { set; get; }
    }

    public class Filtering
    {
        public string op { get; set; }
        public string value { get; set; }
        public string property { get; set; }
    }

    public class GridResult<T>
    {
        public IEnumerable<T> data { get; set; }
        public int total { get; set; }
    }

    public class SearchViewModel
    {
        public int start { get; set; }
        public int limit { get; set; }
        public string sort { get; set; }
        public string filter { get; set; }
        public string group { get; set; }
        public int page { get; set; }

        public List<Sorting> BuildSort()
        {
            if (string.IsNullOrEmpty(this.sort)) return null;
            return JsonConvert.DeserializeObject<List<Sorting>>(sort);
        }

        public List<Filtering> BuildFilter()
        {
            if (string.IsNullOrEmpty(this.filter)) return null;
            string strFilter = this.filter.Replace("\"operator\":\"", "\"op\":\"");
            return JsonConvert.DeserializeObject<List<Filtering>>(strFilter);
        }

    }

}
