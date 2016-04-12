using SimDms.Web;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(ElmahInitializer), "Initialize")]

namespace SimDms.Web
{
    using Elmah.SqlServer.EFInitializer;

    public static class ElmahInitializer
    {
        public static void Initialize()
        {
            //using (var context = new ElmahContext())
            //{
            //    context.Database.Initialize(true);
            //}
        }
    }
}

namespace SimDms.Web
{
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure.Interception;
    using System.Data.Entity.SqlServer;
    using System.Data.SqlClient;
    using System.Reflection;
    using System.Linq;
    using TracerX;
    using System.Web;

    public class CustomEFInterceptor : IDbCommandInterceptor
    {

        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
           // WriteLog(string.Format("Command Text: {1}", interceptionContext.IsAsync, command.CommandText));
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            string param = "", paramname = "";
            int n = command.Parameters.Count;
            if (n > 0)
            {
                paramname = command.Parameters[0].ParameterName + "   nvarchar(4000)";
                param = command.Parameters[0].ParameterName + " = N'" + command.Parameters[0].Value + "' ";
               
                for (var i = 1; i < n; i++)
                {
                    param += "," + command.Parameters[i].ParameterName + " = N'" + command.Parameters[i].Value + "' ";
                    paramname += "," + command.Parameters[i].ParameterName + "  nvarchar(4000)";
                }
            }
            WriteLog(string.Format("exec sp_executesql N'{0}',N'{1}',{2}",   command.CommandText, paramname, param ));
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
           // WriteLog(string.Format(" IsAsync: {0}, Command Text: {1}", interceptionContext.IsAsync, command.CommandText));
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            //WriteLog(string.Format(" IsAsync: {0}, Command Text: {1}", interceptionContext.IsAsync, command.CommandText));
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
           // WriteLog(string.Format(" IsAsync: {0}, Command Text: {1}", interceptionContext.IsAsync, command.CommandText));
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
           // WriteLog(string.Format(" IsAsync: {0}, Command Text: {1}", interceptionContext.IsAsync, command.CommandText));
        }

        private void WriteLog(string command)
        {

            if (MyLogger.DataString(1) == "true")
            {

                if (MyLogger.DataString(2) == HttpContext.Current.User.Identity.Name)
                {
                    MyLogger.Log.Info(command);
                }
            }

        }
    }
}