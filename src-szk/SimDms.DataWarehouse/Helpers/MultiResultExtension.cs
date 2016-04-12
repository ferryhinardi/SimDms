using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace SimDms.DataWarehouse.Helpers
{
    public static class DbContextExtensions
    {
        public static MultiResultSetReader MultiResultSetSqlQuery(
            this DbContext ctx, string query, params SqlParameter[] parameters)
        {
            return new MultiResultSetReader(ctx, query, parameters);
        }
    }

    public class MultiResultSetReader : IDisposable
    {
        private readonly DbContext ctx;
        private readonly DbCommand cmd;
        private bool _connectionNeedsToBeClosed;
        private DbDataReader reader;

        public MultiResultSetReader(DbContext ctx, string query, SqlParameter[] parameters)
        {
            this.ctx = ctx;
            cmd = ctx.Database.Connection.CreateCommand();
            cmd.CommandText = query;
            if (parameters != null && parameters.Any()) cmd.Parameters.AddRange(parameters);
        }

        public void Dispose()
        {
            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }

            if (_connectionNeedsToBeClosed)
            {
                ctx.Database.Connection.Close();
                _connectionNeedsToBeClosed = false;
            }
        }

        public ObjectResult<T> ResultSetFor<T>()
        {
            if (reader == null)
            {
                reader = GetReader();
            }
            else
            {
                reader.NextResult();
            }

            var objContext = ((IObjectContextAdapter)ctx).ObjectContext;
            return objContext.Translate<T>(reader);
        }

        private DbDataReader GetReader()
        {
            if (ctx.Database.Connection.State != ConnectionState.Open)
            {
                ctx.Database.Connection.Open();
                _connectionNeedsToBeClosed = true;
            }

            return cmd.ExecuteReader();
        }
    }
}