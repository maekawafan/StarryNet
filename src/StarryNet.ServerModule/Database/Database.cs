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

        public HashSet<MySqlConnection> connections = new HashSet<MySqlConnection>();

        public DB(string ip, int port, string id, string password, string dbName)
        {
            this.ip = ip;
            this.port = port;
            this.id = id;
            this.password = password;
            this.dbName = dbName;
            string connectKey = MakeConnectKey();
        }

        public int GetConnectionCount()
        {
            return connections.Count;
        }

        public void ConnectionClose(MySqlConnection connection)
        {
            connection.Close();
            connections.Remove(connection);
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
            ConnectionClose(connection);
        }

        public string MakeConnectKey()
        {
            return $"server={ip};port={port};uid={id};pwd={password};database={dbName};charset=utf8;ConvertZeroDateTime=True";
        }

        public MySqlConnection GetConnection()
        {
            MySqlConnection result = new MySqlConnection(MakeConnectKey());
            result.Open();
            if (result.State != ConnectionState.Open)
            {
                Log.Error($"DB Connection Fail server={ip} port={port} database={dbName}");
                return null;
            }
            connections.Add(result);
            return result;
        }

        public MySqlCommand GetCommand(string procedureName)
        {
            return new MySqlCommand(procedureName)
            {
                CommandType = CommandType.StoredProcedure,
            };
        }

        public void CloseAllConnection()
        {
            foreach (var connection in connections)
                connection.Clone();
            connections.Clear();
        }

        public async Task<bool> RunTransaction(Func<MySqlTransaction, Task<bool>> action)
        {
            MySqlConnection connection = GetConnection();
            bool result = false;
            try
            {
                using (MySqlTransaction tx = connection.BeginTransaction())
                {
                    result = await action(tx);
                }
            }
            finally
            {
                ConnectionClose(connection);
            }
            return result;
        }

        public void RunProcedure(string procedureName, params object[] values)
        {
            MySqlCommand command;
            MySqlConnection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - {e.Message}");
            }
            finally
            {
                ConnectionClose(connection);
            }
        }

        public void RunProcedure(string procedureName, Action<MySqlDataReader> successCallback, params object[] values)
        {
            MySqlCommand command;
            MySqlConnection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader?.Read() ?? false)
                        successCallback(reader);
                    else
                        successCallback(null);
                }
            }
            catch (Exception e)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - {e.Message}");
            }
            finally
            {
                ConnectionClose(connection);
            }
        }

        public void RunProcedure(string procedureName, Action failCallback, params object[] values)
        {
            MySqlCommand command;
            MySqlConnection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                command.ExecuteNonQuery();
            }
            catch
            {
                failCallback();
            }
            finally
            {
                ConnectionClose(connection);
            }
        }

        public void RunProcedure(string procedureName, Action<MySqlDataReader> successCallback, Action failCallback, params object[] values)
        {
            MySqlCommand command;
            MySqlConnection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader?.Read() ?? false)
                        successCallback(reader);
                    else
                        successCallback(null);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                failCallback();
            }
            finally
            {
                ConnectionClose(connection);
            }
        }

        public async Task RunProcedureAsync(string procedureName, params object[] values)
        {
            MySqlCommand command;
            MySqlConnection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - {e.Message}");
            }
            finally
            {
                ConnectionClose(connection);
            }
        }

        public async Task RunProcedureAsync(string procedureName, Action<MySqlDataReader> successCallback, params object[] values)
        {
            MySqlCommand command;
            MySqlConnection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader?.ReadAsync())
                        successCallback(reader);
                    else
                        successCallback(null);
                }
            }
            catch (Exception e)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - {e.Message}");
            }
            finally
            {
                ConnectionClose(connection);
            }
        }

        public async Task RunProcedureAsync(string procedureName, Action failCallback, params object[] values)
        {
            MySqlCommand command;
            MySqlConnection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch
            {
                failCallback();
            }
            finally
            {
                ConnectionClose(connection);
            }
        }

        public async Task RunProcedureAsync(string procedureName, Action<MySqlDataReader> successCallback, Action failCallback, params object[] values)
        {
            MySqlCommand command;
            MySqlConnection connection = GetConnection();
            if (Command.BuildCommand(connection, out command, procedureName, values) == false)
            {
                Log.Error($"프로시저[{procedureName}] 실행 실패 - 매개변수 오류");
                return;
            }

            try
            {
                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader?.ReadAsync())
                        successCallback(reader);
                    else
                        successCallback(null);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                failCallback();
            }
            finally
            {
                ConnectionClose(connection);
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