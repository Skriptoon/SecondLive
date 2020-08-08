using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using SecondLive.Core.Logging;

namespace SecondLive.Core
{
    class MySQL
    {
        private static string SERVER = "185.231.153.63";
        private static string DATABASE = "ragemp";
        private static string UID = "ragemp";
        private static string PASSWORD = "8dggn3ODKPcJurqf";
        private static string CHARSET = "UTF8";
        private static string MIN_POOL = "10";
        private static string MAX_POOL = "500";
        private static string POOLING = "True";

        private static string CONNSTRING = "";
        private static nLog Log = new nLog("MySQL");

        public static void Set(string srv, string name, string uid, string pass)
        {
            SERVER = srv;
            DATABASE = name;
            UID = uid;
            PASSWORD = pass;

            Log.Debug($"Cred: {srv}:{name}:{uid}");

            CONNSTRING = $"SERVER={SERVER};DATABASE={DATABASE};UID={UID};PASSWORD={PASSWORD};CHARSET={CHARSET};Min Pool Size={MIN_POOL};Max Pool Size={MAX_POOL};Pooling={POOLING}";
            //POOL.thread.Start();
        }

        public static bool Test()
        {
            Log.Debug("Testing connection...");
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(CONNSTRING);
                conn.Open();
                Log.Debug("Connection is successful!", nLog.Type.Success);
                return true;
            }
            catch (ArgumentException ae)
            {
                Log.Write($"Сonnection string contains an error\n{ae.ToString()}", nLog.Type.Error);
                return false;
            }
            catch (MySqlException me)
            {
                switch (me.Number)
                {
                    case 1042:
                        Log.Write("Unable to connect to any of the specified MySQL hosts", nLog.Type.Error);
                        break;
                    case 0:
                        Log.Write("Access denied", nLog.Type.Error);
                        break;
                    default:
                        Log.Write($"Unknown error ({me.Number})", nLog.Type.Error);
                        break;
                }
                return false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        // Send query function //
        public static void Query(string Query, bool showQuery = false)
        {
            using (MySqlConnection conn = new MySqlConnection(CONNSTRING))
            {
                try
                {
                    conn.OpenAsync();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = Query;

                    if (showQuery) {
                        Log.Debug("Query to DB: " + Query);
                    }

                    cmd.ExecuteNonQueryAsync();
                    conn.CloseAsync();
                }
                catch (MySqlException e)
                {
                    Log.Write($"Error: " + e, nLog.Type.Error);
                }
            }
            //POOL.queue.Enqueue(new MySqlCommand(Query));
        }
        public static void Query(MySqlCommand cmd)
        {
            using (MySqlConnection conn = new MySqlConnection(CONNSTRING))
            {
                try
                {
                    conn.OpenAsync();
                    cmd.Connection = conn;
                    cmd.ExecuteNonQueryAsync();

                    conn.CloseAsync();
                }
                catch (MySqlException e)
                {
                    Log.Write($"Error: " + e, nLog.Type.Error);
                }
            }
            //POOL.queue.Enqueue(cmd);
        }
        public static async Task QueryAsync(string Query, bool showQuery = false)
        {
            using (MySqlConnection conn = new MySqlConnection(CONNSTRING))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = Query;

                    if (showQuery) {
                        Log.Debug("Query to DB: " + Query);
                    }

                    await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
                catch (MySqlException e)
                {
                    Log.Write($"Error: " + e, nLog.Type.Error);
                }
            }
            //POOL.queue.Enqueue(new MySqlCommand(Query));
        }
        public static async Task QueryAsync(MySqlCommand cmd)
        {
            using (MySqlConnection conn = new MySqlConnection(CONNSTRING))
            {
                try
                {
                    await conn.OpenAsync();
                    cmd.Connection = conn;
                    await cmd.ExecuteNonQueryAsync();

                    await conn.CloseAsync();
                }
                catch (MySqlException e)
                {
                    Log.Write($"Error: " + e, nLog.Type.Error);
                }
            }
            //POOL.queue.Enqueue(cmd);
        }

        // Query with return function //
        public static DataTable QueryResult(string Query, bool showQueryResult = false)
        {
            using (MySqlConnection conn = new MySqlConnection(CONNSTRING))
            {
                try
                {
                    conn.OpenAsync();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = Query;

                    if (showQueryResult) {
                        Log.Debug("Query to DB: " + Query);
                    }

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        reader.Close();
                        conn.Close();
                        return null;
                    }

                    DataTable result = new DataTable();
                    result.Load(reader);
                    reader.Close();
                    conn.CloseAsync();

                    if (showQueryResult) {
                        Log.Debug("Result from DB: " + result);
                    }

                    return result;
                }
                catch (MySqlException e)
                {
                    // If connection abort, print to console message
                    Log.Write($"Error: " + e, nLog.Type.Error);

                    return null;
                }
            }
        }

        public static DataTable QueryResult(MySqlCommand cmd)
        {
            using (MySqlConnection conn = new MySqlConnection(CONNSTRING))
            {
                try
                {
                    conn.OpenAsync();
                    cmd.Connection = conn;
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        reader.Close();
                        conn.Close();
                        return null;
                    }

                    DataTable result = new DataTable();

                    result.Load(reader);

                    reader.Close();
                    conn.CloseAsync();

                    return result;
                }
                catch (MySqlException e)
                {
                    Log.Write($"Error: " + e, nLog.Type.Error);
                    return null;
                }
            }
        }
        public static async Task<DataTable> QueryResultAsync(string Query, bool showQueryResult = false)
        {
            using (MySqlConnection conn = new MySqlConnection(CONNSTRING))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = Query;

                    if (showQueryResult) {
                        Log.Debug("Query to DB: " + Query);
                    }

                    DbDataReader reader = await cmd.ExecuteReaderAsync();

                    if (!reader.HasRows)
                    {
                        reader.Close();
                        await conn.CloseAsync();
                        return null;
                    }

                    DataTable result = new DataTable();
                    result.Load(reader);
                    reader.Close();
                    await conn.CloseAsync();

                    if (showQueryResult) {
                        Log.Debug("Result from DB: " + result);
                    }

                    return result;
                }
                catch (MySqlException e)
                {
                    await Log.WriteAsync($"Error: " + e, nLog.Type.Error);
                    return null;
                }
            }
        }
        public static async Task<DataTable> QueryResultAsync(MySqlCommand cmd)
        {
            using (MySqlConnection conn = new MySqlConnection(CONNSTRING))
            {
                try
                {
                    await conn.OpenAsync();
                    cmd.Connection = conn;
                    DbDataReader reader = await cmd.ExecuteReaderAsync();

                    if (!reader.HasRows)
                    {
                        reader.Close();
                        await conn.CloseAsync();
                        return null;
                    }

                    DataTable result = new DataTable();

                    result.Load(reader);

                    reader.Close();
                    await conn.CloseAsync();

                    return result;
                }
                catch (MySqlException e)
                {
                    await Log.WriteAsync($"Error: " + e, nLog.Type.Error);
                    return null;
                }
            }
        }

        public static string ConvertTime(DateTime Date)
        {
            return Date.ToString("s");
        }

        #region MySQL Query Pool
        private class POOL
        {
            public static Thread thread = new Thread(new ThreadStart(Handler));
            public static Queue<MySqlCommand> queue = new Queue<MySqlCommand>();

            private static void Handler()
            {
                try
                {
                    Log.Write("Queue thread started.", nLog.Type.Success);
                    while (true)
                    {
                        if (queue.Count < 1) continue;
                        using (MySqlConnection conn = new MySqlConnection(CONNSTRING))
                        {
                            conn.OpenAsync();
                            MySqlCommand cmd = queue.Dequeue();
                            cmd.Connection = conn;
                            cmd.ExecuteNonQueryAsync();
                            conn.CloseAsync();
                            //Log.Debug("Queue completed!", nLog.Type.Success);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        #endregion
    }
}
