using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Extractor
{
    public class MyExtractor
    {
        private DataContext ctx = new DataContext();

        public MyExtractor()
        {

        }

        public void Run()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SysDealerHistGet2";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            try
            {
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
