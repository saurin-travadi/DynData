using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml;

namespace Service
{
    public class clsDB
    {

        public static void funcExecuteSQL(string sql, string connection, int commandTimeout=120)
        {
            using (var conn = new SqlConnection(connection))
            {
                conn.Open();
                using (var command = new SqlCommand(sql, conn))
                {
                    command.CommandTimeout = commandTimeout;
                    command.ExecuteNonQuery();
                }
            }
        }

        public static DataSet funcExecuteSQLDS(string sql, string connection, int commandTimeout=120)
        {
            using (var ds = new DataSet())
            {
                using (var conn = new SqlConnection(connection))
                {
                    conn.Open();
                    using (var command = new SqlCommand(sql, conn))
                    {
                        command.CommandTimeout = commandTimeout;
                        using (var da = new SqlDataAdapter(command))
                        {
                            da.Fill(ds);
                        }
                    }
                }
                return ds;
            }

        }

        public static object funcExecuteSQLScalar(string sql, string connection, int commandTimeout = 120)
        {
            using (var conn = new SqlConnection(connection))
            {
                conn.Open();
                using (var command = new SqlCommand(sql, conn))
                {
                    command.CommandTimeout = commandTimeout;
                    return command.ExecuteScalar();
                }
            }
        }
        
    }
}
