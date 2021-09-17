using System;
using MySqlConnector;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using StarryNet.StarryLibrary;

namespace StarryNet.ServerModule
{
    public class DB
    {
        public readonly string ip;
        public readonly int port;
        public readonly string id;
        public readonly string password;
        public readonly string dbName;
        public List<Connection> connectionList = new List<Connection>();

        public int usableConnectionCount
        {
            get { return connectionList.FindAll((connection) => { return connection.usable; }).Count; }
        }
        public int unusableConnectionCount
        {
            get { return connectionList.FindAll((connection) => { return !connection.usable; }).Count; }
        }

        public DB(string ip, int port, string id, string password, string dbName)
        {
            this.ip = ip;
            this.port = port;
            this.id = id;
            this.password = password;
            this.dbName = dbName;
            string connectKey = MakeConnectKey();
        }

        public async void ConnectionTest()
        {
            MySqlConnection connection = new MySqlConnection(MakeConnectKey());
            try
            {
                await connection.OpenAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            if (connection.State == ConnectionState.Open)
                Log.Info($"DB[{dbName}] Connect Success!");
            else
                Log.Error($"DB[{dbName}] Connect Fail!");
            connection.Close();
        }

        public string MakeConnectKey()
        {
            return $"server={ip};port={port};uid={id};pwd={password};database={dbName};charset=utf8";
        }

        public int GetConnectionCount()
        {
            return connectionList.Count;
        }

        private Connection GetConnection()
        {
            foreach (Connection connection in connectionList)
            {
                if (connection.usable)
                {
                    connection.usable = false;
                    connection.connection.Open();
                    return connection;
                }
            }
            Connection result = new Connection(new MySqlConnection(MakeConnectKey()));
            result.usable = false;
            result.connection.Open();
            if (result.connection.State != ConnectionState.Open)
            {
                Log.Error($"DB Connection Fail server={ip} port={port} database={dbName}");
                return null;
            }
            connectionList.Add(result);
            return result;
        }

        public MySqlCommand GetCommand(string procedureName)
        {
            return new MySqlCommand(procedureName)
            {
                Connection = GetConnection().connection,
                CommandType = CommandType.StoredProcedure,
            };
        }

        public void CloseAllConnection()
        {
            for (int i = 0; i < connectionList.Count; i++)
            {
                if (connectionList[i].usable)
                {
                    connectionList[i].connection.Close();
                    connectionList.RemoveAt(i);
                    i--;
                }
            }
        }

        public int ClearIdleConnection(double surplusTime = 60.0f)
        {
            int clearCount = 0;
            for (int i = 0; i < connectionList.Count; i++)
            {
                Connection connection = connectionList[i];
                if (connection.usable && connection.lastUseTime.AddSeconds(surplusTime) < DateTime.UtcNow)
                {
                    connection.connection.Close();
                    connectionList.RemoveAt(i);
                    i--;
                    clearCount++;
                }
            }
            return clearCount;
        }

        public async Task RunTransaction(Func<Task<bool>> action)
        {
            Connection connection = GetConnection();
            await connection.BeginTransaction();
            await action();
            connection.EndTransaction();
        }

        public void RunProcedure(string procedureName, params object[] values)
        {
            MySqlCommand command;
            DB.Connection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                connection.Lock();
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - {e.Message}");
            }
            finally
            {
                connection.Unlock();
            }
        }

        public void RunProcedure(string procedureName, Action<MySqlDataReader> successCallback, params object[] values)
        {
            MySqlCommand command;
            DB.Connection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                connection.Lock();
                MySqlDataReader reader = command.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        try
                        {
                            successCallback(reader);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - {e.Message}");
            }
            finally
            {
                connection.Unlock();
            }
        }

        public void RunProcedure(string procedureName, Action failCallback, params object[] values)
        {
            MySqlCommand command;
            DB.Connection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                connection.Lock();
                MySqlDataReader reader = command.ExecuteReader();
                using (reader)
                {
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                failCallback();
            }
            finally
            {
                connection.Unlock();
            }
        }

        public void RunProcedure(string procedureName, Action<MySqlDataReader> successCallback, Action failCallback, params object[] values)
        {
            MySqlCommand command;
            DB.Connection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                connection.Lock();
                MySqlDataReader reader = command.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        try
                        {
                            successCallback(reader);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                failCallback();
            }
            finally
            {
                connection.Unlock();
            }
        }

        public async Task RunProcedureAsync(string procedureName, params object[] values)
        {
            MySqlCommand command;
            DB.Connection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                connection.Lock();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - {e.Message}");
            }
            finally
            {
                connection.Unlock();
            }
        }

        public async Task RunProcedureAsync(string procedureName, Action<MySqlDataReader> successCallback, params object[] values)
        {
            MySqlCommand command;
            DB.Connection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                connection.Lock();
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                using (reader)
                {
                    if (reader.Read())
                    {
                        try
                        {
                            successCallback(reader);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - {e.Message}");
            }
            finally
            {
                connection.Unlock();
            }
        }

        public async Task RunProcedureAsync(string procedureName, Action failCallback, params object[] values)
        {
            MySqlCommand command;
            DB.Connection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                connection.Lock();
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                using (reader)
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch
            {
                failCallback();
            }
            finally
            {
                connection.Unlock();
            }
        }

        public async Task RunProcedureAsync(string procedureName, Action<MySqlDataReader> successCallback, Action failCallback, params object[] values)
        {
            MySqlCommand command;
            DB.Connection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                connection.Lock();
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                using (reader)
                {
                    if (await reader.ReadAsync())
                    {
                        try
                        {
                            successCallback(reader);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                failCallback();
            }
            finally
            {
                connection.Unlock();
            }
        }


        ~DB()
        {
            foreach (Connection conn in connectionList)
                conn.connection.Close();
            //if (connection != null)
            //    connection.Close();
        }

        public class Connection
        {
            public MySqlConnection connection;
            public bool usable;
            public DateTime lastUseTime = DateTime.MinValue;

            public Connection(MySqlConnection connection)
            {
                this.connection = connection;
                usable = true;
            }

            public async Task<MySqlTransaction> BeginTransaction()
            {
                if (connection == null)
                {
                    Log.Error("null인 커넥션의 트랜잭션을 시작했습니다.");
                    return null;
                }
                usable = false;
                return await connection.BeginTransactionAsync();
            }

            public void EndTransaction()
            {
                if (connection == null)
                {
                    Log.Error("null인 커넥션의 트랜잭션을 종료했습니다.");
                    return;
                }
                usable = true;
                lastUseTime = DateTime.UtcNow;
            }

            public void Lock()
            {
                if (connection == null)
                {
                    Log.Error("null인 커넥션에 대해 Lock()를 호출했습니다.");
                    return;
                }
                usable = false;
            }

            public void Unlock()
            {
                if (connection == null)
                {
                    Log.Error("null인 커넥션에 대해 Unlock()를 호출했습니다.");
                    return;
                }
                connection.Close();
                usable = true;
                lastUseTime = DateTime.UtcNow;
            }

            public void Erase()
            {
                if (connection != null)
                {
                    connection.Close();
                    connection = null;
                }
                usable = false;
            }
        }
    }

    public static class DBEx
    {
        public static bool NextSelect(this MySqlDataReader reader)
        {
            if (!reader.NextResult())
                return false;
            reader.Read();
            return true;
        }
    }
}