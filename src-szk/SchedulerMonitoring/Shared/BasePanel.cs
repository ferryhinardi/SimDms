using EventScheduler;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
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

namespace SCHEMON
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
        public static readonly Logger Log = Logger.GetLogger("SCHEMON");

        // Just one way to initialize TracerX early.
        private static bool LogFileOpened = InitLogging();

        // Initialize the TracerX logging system.
        private static bool InitLogging()
        {

            Logger.DefaultBinaryFile.Close();
            Logger.DefaultBinaryFile.Name = "SchedulerMonitoring";
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


        public static void ProcessingEmail()
        {
            try
            {
               using ( SqlConnection con = MyShared.Conn)
               {
                    con.Open();

                    SqlCommand cmd = new SqlCommand("SdmsCis..uspfn_SpGetTaxTrDealers", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        SendMail(ds.Tables[0].Rows[i]["DealerId"].ToString(), ds.Tables[0].Rows[i]["Name"].ToString(), ds.Tables[0].Rows[i]["AreaName"].ToString());
                    }

                    con.Close();
               }

            }
            catch (Exception ex)
            {
                Log.Info(ex.Message);
            }
        }

        public static EmailDestModel getEmailDests(string dealerId)
        {
            using (SqlConnection con = MyShared.Conn)
            {

                con.Open();

                SqlCommand cmd = new SqlCommand("SdmsCis..uspfn_SpGetEmailDestination", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@dealerId", dealerId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                EmailDestModel listEmails = new EmailDestModel();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    listEmails.Emails = new List<string>();

                    listEmails.Subject = ds.Tables[0].Rows[i]["Subject"].ToString();
                    listEmails.Emails.Add(ds.Tables[0].Rows[i]["EmailDeptGA"].ToString());
                    listEmails.Emails.Add(ds.Tables[0].Rows[i]["EmailDeptTax"].ToString());
                }

                con.Close();

                return listEmails;
            }

        }

        public static List<TaxReminderModel> getTaxDetails(string dealerId)
        {
            using (SqlConnection con = MyShared.Conn)
            {

                con.Open();
                
                SqlCommand cmd = new SqlCommand("SdmsCis..uspfn_GetTaxDetails", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@dealerId", dealerId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                List<TaxReminderModel> listTaxDetails = new List<TaxReminderModel>();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    TaxReminderModel model = new TaxReminderModel();
                    model.OutletId = ds.Tables[0].Rows[i]["OutletId"].ToString();
                    model.OutletName = ds.Tables[0].Rows[i]["OutletName"].ToString();
                    model.ItemCode = ds.Tables[0].Rows[i]["ItemCode"].ToString();
                    model.DueDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["DueDate"]);

                    listTaxDetails.Add(model);
                }

                con.Close();

                return listTaxDetails;
            }
        }

        public static bool SendMail(string dealerid, string dealername, string areaname)
        {
            try
            {
                EmailDestModel dests = getEmailDests(dealerid);
                List<TaxReminderModel> listTaxDetails = getTaxDetails(dealerid);

                Log.Info("email dests have gotten!");

                MailMessage mail = new MailMessage();

                var sender = System.Configuration.ConfigurationManager.AppSettings["sender"].ToString();
                var testemail = System.Configuration.ConfigurationManager.AppSettings["testemail"].ToString();

                mail.From = new MailAddress(sender);
                mail.CC.Add(new MailAddress(sender));

                mail.Subject = dests.Subject;

                if (!string.IsNullOrEmpty(testemail))
                {
                    foreach (var item in testemail.Split(';'))
                    {
                        mail.To.Add(item);
                    }
                }
                else
                {
                    foreach (string item in dests.Emails)
                    {
                        mail.To.Add(item);
                    }
                }

                mail.IsBodyHtml = true;
                mail.Body += "<p>Kepada Yth.</p><p><strong>Owner dan Manager Operasional</strong></p>";
                mail.Body += "<ul type=\"disc\"><li>PT " + dealername + "</li><li>" + areaname + "</li></ul><p>Dengan hormat,</p>";
                mail.Body += "<p>Terima kasih dan apresiasi yang tinggi kami sampaikan atas standarisasi CI Suzuki yang telah Bapak dan Ibu laksanakan pada seluruh outlet, baik fasilitas 3S maupun 1S. Semoga Suzuki dan dealer Bapak/Ibu bertambah maju bersama-sama.</p>";
                mail.Body += "<p>Melalui surat elektronik otomatis ini, kami sampaikan <em>reminder</em> untuk mengingatkan kepada Bapak dan Ibu bahwa sebulan yang akan datang merupakan tenggat masa pajak dan izin reklame:</p>";
                mail.Body += "<table border=\"1\" style=\"border-collapse:collapse;width:\"100%\"\"><tbody><td nowrap=\"\" style=\"padding:3px;width:\"15%\"\" align=\"center\"><p>Outlet ID</p></td><td nowrap=\"\" style=\"padding:3px;width:\"55%\"\" align=\"center\"><p>Outlet</p></td><td style=\"padding:3px;width:\"15%\"\" align=\"center\"><p>Jenis Pajak</p></td><td  style=\"padding:3px;;width:\"15%\"\" align=\"center\"><p>Jatuh Tempo</p></td></tr>";

                foreach (TaxReminderModel item in listTaxDetails)
                {
                    mail.Body += "<tr><td><p style=\"color:green;padding:3px;\">" + item.OutletId + "</td><td><p style=\"color:green;padding:3px;\">" + item.OutletName + "</td><td><p style=\"color:green;padding:3px;\">" + item.ItemCode + "</p></td><td><p style=\"color:green;padding:3px;\">" + item.DueDate.ToString("dd-MMM-yyyy") + "</p></td></tr>";
                }

                mail.Body += "</tbody></table>";
                mail.Body += "<p>Kami sangat mengharapkan pembayaran tepat waktu kepada Dinas Pendapatan Daerah masing-masing demi menghindari denda akibat keterlambatan perpanjangan.</p>";
                mail.Body += "<p>Setelah Bapak/Ibu melaksanakan pembayaran perpanjangan pajak, mohon dapat dikirimkan copy dokumen berupa    <strong>SKPD, sticker peneng, dan foto obyek pajak</strong> terkait kepada kami. Alamat pengiriman: PT Suzuki Indomobil Sales, CIS-Section. Wisma Indomobil lantai 7. Jalan MT Haryono Kav.8, Cawang. Jakarta Timur 13330 atau melalui email <a href=\"mailto:kurnia.effendi@suzuki.co.id\">kurnia.effendi@suzuki.co.id</a> /    <a href=\"mailto:yulius.purwanto@suzuki.co.id\">yulius.purwanto@suzuki.co.id</a></p>";
                mail.Body += "<p>Atas perhatian Bapak/Ibu dan kerjasama yang baik, kami sampaikan terima kasih.</p>";
                mail.Body += "<br/><br/><p>Hormat kami,</p>";
                mail.Body += "<p><strong>Suzuki Corparate Identity Standard </strong></p>";
                mail.Body += "<p><strong>PT Suzuki Indomobil Sales</strong></p>";

                mail.Priority = MailPriority.High;

                var emailServer = System.Configuration.ConfigurationManager.AppSettings["emailserver"].ToString();
                SmtpClient client = new SmtpClient(emailServer);
                client.Send(mail);

                Log.Info("email sent! for " + dealerid);

                return true;
            }
            catch (Exception ex)
            {
                Log.Info("error : " + ex.Source.ToString());
                Log.Info("error : " + ex.Message.ToString());
                throw ex;
            }
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
                return "SCHEMON";
            }
        }

        public static string ServiceDesc
        {
            get
            {
                return "Schedule Monitoring";
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

        public static async Task BackupDaily()
        {
            try
            {
                using (SqlConnection CN = MyShared.Conn)
                {
                    await CN.OpenAsync();
                    var SQL = "EXEC uspfn_backup_daily";
                    SqlCommand cmd = new SqlCommand(SQL, CN);
                    cmd.CommandTimeout = 10800;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Error On BackupDaily: " + ex.Message);
            }
            finally
            {
                GC.Collect();
            }
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



        public static int BulkData(string DealerCode, SQLiteConnection db3, string filename, 
            string tablename, string dbConn)
        {

            string SQL = "SELECT * FROM " + tablename;
            string tempTableName = "#" + tablename + DateTime.Now.ToString("yyyyMMddHHmmss");
            var db = new SqlConnection(dbConn);
            bool AutoCreateTmpTable = true;
            bool NeedMerge = true;
            SqlTransaction transaction = null;
            SqlBulkCopy bulkCopy = null;

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
                return 0;
            }

            #region Check

            var cmdDB = db.CreateCommand();
            cmdDB.CommandText = "select * from simdms..sysdealeriotable where tablename='" + tablename + "'";
            var daIOT = new SqlDataAdapter(cmdDB);
            var dtIOT = new DataTable();
            daIOT.Fill(dtIOT);
            int n = dtIOT.Rows.Count;
            var destination = tablename;
            string listColumns = "";
            DataTable tmpDataTable = new DataTable();

            if (n == 1)
            {
                var sqlGetData = dtIOT.Rows[0]["Sql"].ToString();
                destination = dtIOT.Rows[0]["Destination"].ToString();
                if (sqlGetData != "MERGE")
                {
                    AutoCreateTmpTable = false;
                    NeedMerge = false;
                    SQL = sqlGetData;
                    tempTableName = destination;
                } 
            }

            #endregion
            
            #region Check Merge
            if (NeedMerge)
            {
                cmdDB.CommandText = "select top 0 * from " + destination;
                SqlDataReader reader = cmdDB.ExecuteReader();
                DataTable schema = reader.GetSchemaTable();

                foreach (DataRow row in schema.Rows)
                {
                    tmpDataTable.Columns.Add(new DataColumn(
                       row["ColumnName"].ToString(),
                       (Type)row["DataType"]));

                    if (listColumns == "")
                    {
                        listColumns = row["ColumnName"].ToString();
                    }
                    else
                    {
                        listColumns += "," + row["ColumnName"].ToString();
                    }
                }

                reader.Close();
                reader = null;

                SQL = "SELECT " + listColumns + " FROM " + tablename;
            }
            #endregion

            bool HasError = false, HasFatalError = false;

            StringBuilder errorMessage = new StringBuilder("Bulk copy failures:" + Environment.NewLine);

            using (SQLiteCommand cmd = new SQLiteCommand(SQL,db3))
            {
                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {

                        try
                        {
                            //transaction = db.BeginTransaction();
                            bulkCopy = new SqlBulkCopy(db);
                            bulkCopy.DestinationTableName = tempTableName;
                            int currentRow = 0;
                            
                            if (AutoCreateTmpTable)
                            {
                                //cmdDB.Transaction = transaction;
                                cmdDB.CommandText = "SELECT TOP 0 " + listColumns + " INTO " + tempTableName + " from " + tablename;
                                cmdDB.ExecuteNonQuery();
                            }

                            // create an object array to hold the data being transferred into tmpDataTable 
                            //in the loop below.
                            object[] values = new object[rdr.FieldCount];

                            // loop through the source data
                            while (rdr.Read())
                            {
                                // clear the temp DataTable from which the single-record bulk copy will be done
                                tmpDataTable.Rows.Clear();

                                // get the data for the current source row
                                rdr.GetValues(values);

                                // load the values into the temp DataTable
                                tmpDataTable.LoadDataRow(values, true);

                                // perform the bulk copy of the one row
                                try
                                {
                                    currentRow++;
                                    bulkCopy.WriteToServer(tmpDataTable);
                                }
                                catch (Exception ex)
                                {
                                    HasError = true;
                                    // an exception was raised with the bulk copy of the current row. 
                                    // The row that caused the current exception is the only one in the temp 
                                    // DataTable, so document it and add it to the error message.
                                    DataRow faultyDataRow = tmpDataTable.Rows[0];
                                    errorMessage.AppendFormat("Error on line #{0}: {1}{2}", currentRow, ex.Message, Environment.NewLine);
                                    //errorMessage.AppendFormat("Row data: {0}", Environment.NewLine);
                                    //foreach (DataColumn column in tmpDataTable.Columns)
                                    //{
                                    //    errorMessage.AppendFormat(
                                    //       "\tColumn {0} - [{1}]{2}",
                                    //       column.ColumnName,
                                    //       faultyDataRow[column.ColumnName].ToString(),
                                    //       Environment.NewLine);
                                    //}
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            HasFatalError = true;
                            Log.Debug(
                               "Unable to document SqlBulkCopy errors. See inner exceptions for details. " +
                               ex.Message);
                        }
                        finally
                        {

                            if (NeedMerge)
                            {
                                var cmdMgr = db.CreateCommand();
                                //cmdMgr.Transaction = transaction;
                                cmdMgr.CommandText = "EXEC uspfn_SysDealerMergeData '" + DealerCode +
                                    "','" + tablename + "','" + tempTableName + "','" + listColumns + "'";
                                Log.Info(cmdMgr.CommandText);
                                var dbaMerge = new SqlDataAdapter(cmdMgr);
                                var dtMerge = new DataTable();
                                dbaMerge.Fill(dtMerge);

                                //cmdDB.Transaction = transaction;
                                cmdDB.CommandText = dtMerge.Rows[0]["SQL"].ToString();
                                cmdDB.CommandTimeout = 3600;
                                Log.Info(cmdDB.CommandText);
                                cmdDB.ExecuteNonQuery();


                                cmdDB.CommandText = "select @@ROWCOUNT";
                                int rowChanges = Convert.ToInt32(cmdDB.ExecuteScalar());

                                Log.Info(rowChanges + " row(s) changed");

                            }

                            //if (transaction != null)
                            //{
                            //    if (HasFatalError)
                            //        transaction.Rollback();
                            //    else
                            //        transaction.Commit();
                            //}

                            if (HasError && db.State == ConnectionState.Open)
                            {
                                cmdDB.CommandTimeout = 3600;
                                cmdDB.CommandText = "uspfn_SysDealerLogErrorNotification2";
                                cmdDB.CommandType = CommandType.StoredProcedure;
                                cmdDB.Parameters.Clear();

                                cmdDB.Parameters.AddWithValue("@DealerCode", DealerCode);
                                cmdDB.Parameters.AddWithValue("@TableName", tablename);
                                cmdDB.Parameters.AddWithValue("@FileName", filename);
                                cmdDB.Parameters.AddWithValue("@ErrorDesc", errorMessage.ToString());

                                cmdDB.ExecuteNonQuery();
                            }

                        }

                        Log.Debug(errorMessage.ToString());

                    }
                }
            }

            return -1;

        }


        // <summary>
        /// Build an error message with the failed records and their related exceptions.
        /// </summary>
        /// <param name="connectionString">Connection string to the destination database</param>
        /// <param name="tableName">Table name into which the data will be bulk copied.</param>
        /// <param name="dataReader">DataReader to bulk copy</param>
        /// <returns>Error message with failed constraints and invalid data rows.</returns>
        public static string GetBulkCopyFailedData(
           string connectionString,
           string tableName,
           string tmpTableName,
           string DealerCode,
           string FileName,
           IDataReader dataReader, bool AutoCreateTmpTable = true)
        {

            StringBuilder errorMessage = new StringBuilder("Bulk copy failures:" + Environment.NewLine);
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            SqlBulkCopy bulkCopy = null;
            DataTable tmpDataTable = new DataTable();
            string listColumns = "";

            bool HasFatalError = false, HasError = false;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                transaction = connection.BeginTransaction();
                bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.CheckConstraints, transaction);
                bulkCopy.DestinationTableName = tmpTableName;
                int currentRow = 0;

                // create a datatable with the layout of the data.
                DataTable dataSchema = dataReader.GetSchemaTable();
                foreach (DataRow row in dataSchema.Rows)
                {
                    tmpDataTable.Columns.Add(new DataColumn(
                       row["ColumnName"].ToString(),
                       (Type)row["DataType"]));
                    if (listColumns == "")
                    {
                        listColumns = row["ColumnName"].ToString();
                    }
                    else
                    {
                        listColumns += "," + row["ColumnName"].ToString();
                    }
                }

                if (AutoCreateTmpTable)
                {
                    var cmdDB = connection.CreateCommand();
                    cmdDB.CommandText = "SELECT TOP 0 " + listColumns + " INTO " + tmpTableName + " from " + tableName;
                    Log.Info(cmdDB.CommandText);
                    cmdDB.ExecuteNonQuery();
                }

                // create an object array to hold the data being transferred into tmpDataTable 
                //in the loop below.
                object[] values = new object[dataReader.FieldCount];

                // loop through the source data
                while (dataReader.Read())
                {
                    // clear the temp DataTable from which the single-record bulk copy will be done
                    tmpDataTable.Rows.Clear();

                    // get the data for the current source row
                    dataReader.GetValues(values);

                    // load the values into the temp DataTable
                    tmpDataTable.LoadDataRow(values, true);

                    // perform the bulk copy of the one row
                    try
                    {
                        currentRow++;
                        bulkCopy.WriteToServer(tmpDataTable);
                    }
                    catch (Exception ex)
                    {
                        HasError = true;
                        // an exception was raised with the bulk copy of the current row. 
                        // The row that caused the current exception is the only one in the temp 
                        // DataTable, so document it and add it to the error message.
                        DataRow faultyDataRow = tmpDataTable.Rows[0];
                        errorMessage.AppendFormat("Error on line #{0}: {1}{2}", currentRow, ex.Message, Environment.NewLine);
                        errorMessage.AppendFormat("Row data: {0}", Environment.NewLine);
                        foreach (DataColumn column in tmpDataTable.Columns)
                        {
                            errorMessage.AppendFormat(
                               "\tColumn {0} - [{1}]{2}",
                               column.ColumnName,
                               faultyDataRow[column.ColumnName].ToString(),
                               Environment.NewLine);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HasFatalError = true;
                Log.Debug(
                   "Unable to document SqlBulkCopy errors. See inner exceptions for details. " +
                   ex.Message);
            }
            finally
            {
                if (transaction != null)
                {
                    if (HasFatalError)
                        transaction.Rollback();
                    else 
                        transaction.Commit();
                }

                if (HasError && connection.State == ConnectionState.Open)
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandTimeout = 3600;
                    cmd.CommandText = "uspfn_SysDealerLogErrorNotification2";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("@DealerCode", DealerCode);
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    cmd.Parameters.AddWithValue("@FileName", FileName);
                    cmd.Parameters.AddWithValue("@ErrorDesc", errorMessage.ToString());
                    cmd.ExecuteNonQuery();
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
            
            Log.Debug(errorMessage.ToString());
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
                string ConnTargetDB = (System.Configuration.ConfigurationManager.ConnectionStrings["SimDMS" + optDB].ToString());

                var db = new SqlConnection(ConnTargetDB);

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
                                    string listColumns = "";
                                    int CountUpdate = Convert.ToInt32(dt.Rows[i]["count"]);

                                    if (CountUpdate > 0)
                                    {
                                        tableName = dt.Rows[i]["name"].ToString();
                                        
                                        string tempTable = "#" +  tableName + DateTime.Now.ToString("yyyyMMddHHmmss");
                                        var cmdDB = db.CreateCommand();
                                        cmdDB.CommandText = "select * from simdms..sysdealeriotable where tablename='" + tableName + "'";
                                        var daIOT = new SqlDataAdapter(cmdDB);
                                        var dtIOT = new DataTable();
                                        daIOT.Fill(dtIOT);

                                        int n = dtIOT.Rows.Count;

                                        if (n == 1)
                                        {

                                            var DstTableName = dtIOT.Rows[0]["Destination"].ToString();
                                            var sqlGetData = dtIOT.Rows[0]["Sql"].ToString();

                                            if (sqlGetData == "MERGE")
                                            {
                                                var cmdT = db3.CreateCommand();
                                                cmdT.CommandText = "select * from " + tableName + " limit 0,1";
                                                var dbaX = new SQLiteDataAdapter(cmdT);

                                                var dtX = new DataTable();
                                                dbaX.Fill(dtX);


                                                listColumns = GenerateColumnsList(dtX);

                                                cmdDB.CommandText = "EXEC uspfn_SysDealerMergeData3 '" + DstTableName + "','" + tempTable + "','" + listColumns + "'";

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
                                                    cmdDB.CommandText = "SELECT TOP 0 " + listColumns + " INTO " + tempTable + " from " + DstTableName;
                                                    Log.Info("Create temporary table on target database  " + DstTableName);
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
                                                        Success = true;

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
                                                                HasError = true;
                                                                Log.Warn("ERROR >> " + ex.Message);
                                                            }
                                                            // Log.Info("Data has been copied (sqlite3 table to temp table)");
                                                        }

                                                        if (Success)
                                                        {
                                                            Log.Info("Start to mergering data from temp table to real table");

                                                            //try
                                                            //{
                                                            //    cmdDB.CommandTimeout = 1800;
                                                            //    Log.Info(MergeSQL);
                                                            //    cmdDB.CommandText = MergeSQL;

                                                            //    cmdDB.ExecuteNonQuery();

                                                            //    cmdDB.CommandText = "select @@ROWCOUNT";
                                                            //    int rowChanges = Convert.ToInt32(cmdDB.ExecuteScalar());

                                                            //    Log.Info(rowChanges + " row(s) changed");
                                                            //    totalRowChanges += rowChanges;

                                                            //}
                                                            //catch (Exception ExM)
                                                            //{
                                                            //    Success = false;
                                                            //    HasError = true;
                                                            //    Log.Warn("Error: " + ExM.Message);

                                                            //    cmdDB.CommandText = "DROP TABLE " + tempTable;
                                                            //    Log.Info(cmdDB.CommandText);
                                                            //    cmdDB.ExecuteNonQuery();

                                                            //    SendErrorLog("[Error Notification] Merger Data 3", "Dealer Code: " + DealerCode + Environment.NewLine + ExM.Message +
                                                            //        Environment.NewLine + Environment.NewLine + MergeSQL, DealerCode, tableName
                                                            //     );
                                                            //}

                                                        }

                                                        //if (Success){
                                                        //cmdDB.CommandText = "TRUNCATE TABLE " + tempTable;
                                                        //Log.Info(cmdDB.CommandText);
                                                        //cmdDB.ExecuteNonQuery();                                                       

                                                        //}

                                                    }

                                                    //if (Success)
                                                    //{
                                                        //cmdDB.CommandText = "Update SysDealer Set LastUpdate='" + dt.Rows[i]["lastupdate"].ToString() + "' "
                                                        //                    + "Where DealerCode='" + dt.Rows[i]["code"].ToString() + "' AND TableName='" + dt.Rows[i]["name"].ToString() + "'";
                                                        ////Log.Info(cmdDB.CommandText);
                                                        //cmdDB.ExecuteNonQuery();

                                                        //cmdDB.CommandText = "DROP TABLE " + tempTable;
                                                        //Log.Info(cmdDB.CommandText);
                                                        //cmdDB.ExecuteNonQuery();
                                                    //}

                                                    //Log.Info("Merge data finished");

                                                    cmdT.Dispose();
                                                    dbaX.Dispose();
                                                }

                                            }
                                            else 
                                            { 
                                                var cmdT = db3.CreateCommand();
                                                cmdT.CommandText = sqlGetData;
                                                var dbaX = new SQLiteDataAdapter(cmdT);
                                                var dtX = new DataTable();
                                                dbaX.Fill(dtX);

                                                using (SqlBulkCopy bcp = new SqlBulkCopy(db))
                                                {
                                                    Log.Info("Start copying data from sqlite3 table into destination table (" + DstTableName + ")");
                                                    bcp.DestinationTableName = DstTableName;
                                                    bcp.NotifyAfter = dtX.Rows.Count;
                                                    bcp.BatchSize = dtX.Rows.Count;
                                                    bcp.BulkCopyTimeout = 3600;
                                                    try
                                                    {
                                                        bcp.WriteToServer(dtX);
                                                    }
                                                    catch (Exception ex)
                                                    {                                                    
                                                        Log.Warn("ERROR >> " + ex.Message);
                                                    }                                                
                                                }
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                
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

                                                        cmdDB.CommandText = "TRUNCATE TABLE " + tempTable;
                                                        Log.Info(cmdDB.CommandText);
                                                        cmdDB.ExecuteNonQuery();
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

                    //cmdHst.CommandText = "EXEC [uspfn_SysDealerHistUpdStatus2] '" + uploadID + "',1";
                    //cmdHst.ExecuteNonQuery();

                }

                Log.Info("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * " + totalRowChanges + " row(s) affected * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * ");

                if (HasError)
                {
                    return -1;
                }

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

        public static async Task RunMergeProcess3(string id)
        {

            #region tryMerge
            try
            {

                using (SqlConnection CN = MyShared.Conn)
                {

                    await CN.OpenAsync();

                    var SQL = id;
                    SqlCommand cmd = new SqlCommand(SQL, CN);
                    cmd.CommandTimeout = 1800;
                    var dba = new SqlDataAdapter(cmd);

                    string ExtractDir = MyShared.extractDir + id;

                    if (!System.IO.Directory.Exists(extractDir))
                    {
                        System.IO.Directory.CreateDirectory(extractDir);
                    }

                    ExtractDir = ExtractDir + @"\";

                    var dt = new DataTable();
                    dba.Fill(dt);

                    int nRow = dt.Rows.Count;

                    if (nRow > 0)
                    {

                        Log.Info("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Log.Info("Merge All - Started...");
                        Log.Info("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");


                        SQL = "delete from sysdealerjobs where taskname='" + id + "';Insert into sysdealerjobs (TaskName, StartDate, JobTotal, Status) values ('" + id +
                              "',getdate()," + nRow.ToString() + ",0);";
                        cmd.CommandText = SQL;
                        cmd.ExecuteNonQuery();

                        for (int i = 0; i < nRow; i++)
                        {
                            //MergeProcess(CN, dt.Rows[i]);

                            string fileName = dt.Rows[i]["FileName"].ToString();
                            string filePath = dt.Rows[i]["FilePath"].ToString();
                            string filedb = fileName.Replace(".zip", ".db3");

                            try
                            {
                                string fileNameDb = ExtractDir + filedb;

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
                                MyShared.ExtractZipFile(MyShared.uploadDir + filePath, ExtractDir);
                                string optDB = fileName.Split('_')[1].ToString();

                                var nFirst = MyShared.MergeDatabase(ExtractDir + filedb, optDB, CN, dt.Rows[i]["UploadId"].ToString());
                                
                                if (nFirst == -1)
                                    MyShared.MergeDatabase3(ExtractDir + filedb, optDB, CN, dt.Rows[i]["UploadId"].ToString());
                            
                            }
                            catch (Exception ex)
                            {
                                Log.Warn("Error on extract or merge database: " + ex.Message + " >> " + filedb);
                            }


                            SQL = "update sysdealerjobs set lastupdate=getdate(), onProgress=" + (i+1).ToString() +
                                  " where taskname='" + id + "' and status=0";
                            cmd.CommandText = SQL;
                            cmd.ExecuteNonQuery();

                        }

                        Log.Info("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Log.Info("Merge All - Done!");
                        Log.Info("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

                        //cmd.CommandText = "EXEC master..sp_checkSIMDMSLogFile";
                        //cmd.ExecuteNonQuery();

                        //SQL = "EXEC [uspfn_SysDealerHistGet3] 10";
                        //cmd = new SqlCommand(SQL, CN);
                        //dba = new SqlDataAdapter(cmd);

                        //dt = new DataTable();
                        //dba.Fill(dt);

                        SQL = "update sysdealerjobs set status=1, lastupdate=getdate(), onProgress=" + nRow.ToString() +
                              " where taskname='" + id + "' and status=0";
                        cmd.CommandText = SQL;
                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Error On Run Merge Manual Process: " + ex.Message);
                SendErrorLog("[Error Notification] Merger Manual ", ex.Message, "", "");
            }
            finally
            {
                GC.Collect();
            }
            #endregion

        }

        public static int MergeDatabase3(string filedb, string optDB, SqlConnection historyDB, string uploadID)
        {
            int totalRowChanges = 0;
            string DealerCode = "";
            string tableName = "";
            bool HasError = false;

            using (SQLiteConnection db3 = new SQLiteConnection())
            {
                string ConnTargetDB = (System.Configuration.ConfigurationManager.ConnectionStrings["SimDMS" + optDB].ToString());

                var db = new SqlConnection(ConnTargetDB);

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
                                    tableName = dt.Rows[i]["name"].ToString();

                                    if (CountUpdate > 0)
                                    {
                                        var totalChanges = BulkData(DealerCode, db3, filedb, tableName, ConnTargetDB);
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
                    }

                    db.Close();
                    db = null;

                    cmdHst.CommandText = "EXEC [uspfn_SysDealerHistUpdStatus2] '" + uploadID + "',1";
                    cmdHst.ExecuteNonQuery();

                    DeleteFile(filedb);

                }

                Log.Info("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * " + totalRowChanges + " row(s) affected * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * ");
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

    public class EmailDestModel
    {
        public string Subject { get; set; }
        public List<string> Emails { get; set; }
    }

    public class TaxReminderModel
    {
        public string OutletId { get; set; }
        public string OutletName { get; set; }
        public string ItemCode { get; set; }
        public DateTime DueDate { get; set; }
    }

}
