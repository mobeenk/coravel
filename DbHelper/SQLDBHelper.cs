
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TBCIntegration.Helper;

namespace AttendanceNotification.Helper
{
    public class Ref<T>
    {
        public Ref() { }
        public Ref(T value) { Value = value; }
        public T Value { get; set; }
        public override string ToString()
        {
            T value = Value;
            return value == null ? "" : value.ToString();
        }
        public static implicit operator T(Ref<T> r) { return r.Value; }
        public static implicit operator Ref<T>(T value) { return new Ref<T>(value); }
    }
    public class SQLDBHelper
    {
        public SQLDBHelper(string constr)
        {

            ConnStr = constr;
        }

        private string ConnStr { get; set; }

        internal SqlConnection TheConnection => new SqlConnection(ConnStr);

        internal SqlCommand getCommand(string command, SqlConnection conn)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandTimeout = conn.ConnectionTimeout;
            cmd.CommandText = command;
            return cmd;
        }
        private SqlParameter GetReturnParameter()
        {
            SqlParameter retpara = new SqlParameter
            {
                ParameterName = "ReturnValue",
                Direction = ParameterDirection.ReturnValue
            };
            return retpara;
        }
        internal void Dispose()
        {
            if (TheConnection != null)
            {
                TheConnection.Dispose();
            }
        }
        internal async Task<object> ExecuteScaler(string SpName, SqlParameter[] sqlpara = null)
        {
            // ConnectionState connectionState = Connection.State;
            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getCommand(SpName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await conn.OpenAsync();

                        if (sqlpara != null)
                        {
                            foreach (SqlParameter para in sqlpara)
                            {
                                if (para.Value == null)
                                {
                                    para.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(para);
                            }
                        }
                        return await cmd.ExecuteScalarAsync();
                    }
                }

            }
            catch (SqlException sqlex)
            {
                throw new Exception("ExecuteScaler SPname=" + SpName + Environment.NewLine, sqlex);
            }
            finally
            {

            }
        }
        internal async Task<object> ExecuteScaler(string CommandText)
        {

            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getCommand(CommandText, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        await conn.OpenAsync();

                        return await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("ExecuteScaler SPname=" + CommandText + Environment.NewLine, sqlex);
            }
            finally
            {

            }
        }
        internal async Task<int> ExecuteNonQuery(string SpName, SqlParameter[] sqlpara = null, bool returnvalue = false)
        {
            SqlParameter retpara = null;

            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getCommand(SpName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await conn.OpenAsync();
                        if (sqlpara != null)
                        {
                            foreach (SqlParameter para in sqlpara)
                            {
                                if (para.Value == null)
                                {
                                    para.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(para);
                            }
                        }
                        if (returnvalue)
                        {
                            retpara = GetReturnParameter();
                            cmd.Parameters.Add(retpara);
                        }
                        int i = await cmd.ExecuteNonQueryAsync();
                        if (returnvalue)
                        {
                            return retpara.Value.ToInt();
                        }
                        else
                        {
                            return i;
                        }
                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("ExecuteNonQuery SPname=" + SpName + Environment.NewLine + sqlex.Message, sqlex);
            }
            finally
            {
                retpara = null;

            }
        }
        internal async Task<long> ExecuteNonQuerywithLong(string SpName, SqlParameter[] sqlpara = null)
        {
            SqlParameter retpara = null;
            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getCommand(SpName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await conn.OpenAsync();
                        if (sqlpara != null)
                        {
                            foreach (SqlParameter para in sqlpara)
                            {
                                if (para.Value == null)
                                {
                                    para.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(para);
                            }
                        }
                        retpara = GetReturnParameter();
                        cmd.Parameters.Add(retpara);
                        int i = await cmd.ExecuteNonQueryAsync();
                        return retpara.Value.ToLong();
                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("ExecuteNonQuerywithLong SPname=" + SpName + Environment.NewLine + sqlex.Message, sqlex);
            }
            finally
            {
                retpara = null;

            }
        }
        internal async Task<int> ExecuteCommand(string CommandText, SqlParameter[] sqlpara = null)
        {
            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getCommand(CommandText, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        await conn.OpenAsync();
                        if (sqlpara != null)
                        {
                            foreach (SqlParameter para in sqlpara)
                            {
                                if (para.Value == null)
                                {
                                    para.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(para);
                            }
                        }
                        int i = await cmd.ExecuteNonQueryAsync();
                        return i;
                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("ExecuteNonQuery CmmandText=" + CommandText + Environment.NewLine, sqlex);
            }
            finally
            {

            }

        }
        internal async Task<List<T>> ExecuteToDataTable<T>(string SpName, SqlParameter[] sqlpara = null) where T : class, new()
        {


            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getCommand(SpName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await conn.OpenAsync();

                        if (sqlpara != null)
                        {
                            foreach (SqlParameter para in sqlpara)
                            {
                                if (para.Value == null)
                                {
                                    para.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(para);
                            }
                        }

                        using (DbDataReader dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            DataTable dt = new DataTable();
                            dt.Load(dataReader);
                            cmd.Parameters.Clear();
                            return dt.MapTo<T>();
                        }
                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("ExecuteToDataTable SpName=" + SpName + Environment.NewLine, sqlex);
            }
            finally
            {


            }
        }
        internal async Task<T> ExecuteSelectOne<T>(string SpName, SqlParameter[] sqlpara = null) where T : class, new()
        {


            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getCommand(SpName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await conn.OpenAsync();

                        if (sqlpara != null)
                        {
                            foreach (SqlParameter para in sqlpara)
                            {
                                if (para.Value == null)
                                {
                                    para.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(para);
                            }
                        }

                        using (DbDataReader dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            DataTable dt = new DataTable();
                            dt.Load(dataReader);
                            cmd.Parameters.Clear();
                            List<T> d = dt.MapTo<T>();
                            return d.SingleOrDefault();
                        }
                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("ExecuteToDataTable SpName=" + SpName + Environment.NewLine, sqlex);
            }
            finally
            {


            }
        }
        internal async Task<DataTable> ExecuteToDataTableAsync(string SpName, SqlParameter[] sqlpara = null)
        {
            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getCommand(SpName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await conn.OpenAsync();

                        if (sqlpara != null)
                        {
                            foreach (SqlParameter para in sqlpara)
                            {
                                if (para.Value == null)
                                {
                                    para.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(para);
                            }
                        }

                        using (DbDataReader dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            DataTable dt = new DataTable();
                            dt.Load(dataReader);
                            cmd.Parameters.Clear();
                            return dt;
                        }
                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("ExecuteToDataTable SpName=" + SpName + Environment.NewLine, sqlex);
            }
            finally
            {


            }
        }
        internal async Task<List<T>> ExecuteCommandToDataTable<T>(string CmmandText) where T : class, new()
        {

            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getCommand(CmmandText, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        await conn.OpenAsync();

                        using (DbDataReader dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            DataTable dt = new DataTable();
                            dt.Load(dataReader);
                            cmd.Parameters.Clear();
                            return dt.MapTo<T>();
                        }
                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("ExecuteCommandToDataTable Command=" + CmmandText + Environment.NewLine, sqlex);
            }
            finally
            {
            }

        }
        internal async Task<List<T>> ExecuteToDataTableWithReturnValue<T>(string SpName, Ref<object> returnvalue, SqlParameter[] sqlpara = null) where T : class, new()
        {
            SqlParameter retpara = null;
            returnvalue = null;

            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getCommand(SpName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await conn.OpenAsync();

                        if (sqlpara != null)
                        {
                            foreach (SqlParameter para in sqlpara)
                            {
                                if (para.Value == null)
                                {
                                    para.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(para);
                            }
                        }

                        retpara = GetReturnParameter();
                        cmd.Parameters.Add(retpara);

                        using (DbDataReader dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            returnvalue = new Ref<object>
                            {
                                Value = retpara.Value
                            };
                            DataTable dt = new DataTable();
                            dt.Load(dataReader);
                            cmd.Parameters.Clear();
                            return dt.MapTo<T>();
                        }
                    }
                }
            }

            catch (SqlException sqlex)
            {
                throw new Exception("ExecuteToDataTableWithReturnValue SPname=" + SpName + Environment.NewLine, sqlex);
            }
            finally
            {

            }

        }
        internal async Task<DataSet> ExecuteToDataSet(string SpName, SqlParameter[] sqlpara = null)
        {

            try
            {

                using (SqlConnection conn = TheConnection)
                {

                    using (SqlCommand cmd = getCommand(SpName, conn))

                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        await conn.OpenAsync();
                        if (sqlpara != null)
                        {
                            foreach (SqlParameter para in sqlpara)
                            {
                                if (para.Value == null)
                                {
                                    para.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(para);
                            }
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return ds;
                        }

                    }
                }
            }

            catch (SqlException sqlex)
            {
                throw new Exception("ExecuteToDataSet SPname=" + SpName + Environment.NewLine, sqlex);

                //ErrorLog.LogError("ExecuteToDataSet SPName=" + SpName + " " + sqlex.ToString(), fileid);
                //return null;


            }
        }
        internal async Task<bool> InsertBulkCopyAsync(DataTable dt)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(ConnStr, SqlBulkCopyOptions.Default |
                       SqlBulkCopyOptions.UseInternalTransaction))
            {
                bulkCopy.BulkCopyTimeout = GetConnectionTimeOut();
                bulkCopy.DestinationTableName = dt.TableName;
                bulkCopy.BatchSize = 500;
                foreach (DataColumn dc in dt.Columns)
                {

                    bulkCopy.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                }
                try
                {
                    // Write from the source to the destination.
                    await bulkCopy.WriteToServerAsync(dt);

                }
                catch (SqlException ex)
                {
                    throw new Exception("InsertBulkCopy " + Environment.NewLine, ex);
                    //ErrorLog.LogError("InsertBulkCopy:tablename:" + dt.TableName + ex.ToString(), fileid);

                    //return false;
                }
                finally
                {
                    if (dt != null)
                    {
                        dt.Dispose();
                    }

                    dt = null;
                }


            }
            return true;
        }

        private int GetConnectionTimeOut()
        {
            return 600;
        }
    }
}
