using MySqlConnector;
using System;
using System.Data;
using System.Threading.Tasks;
using StarryNet.StarryLibrary;

namespace StarryNet.ServerModule
{
    public class Command
    {
        public DB db { get; private set; }
        public string procedureName { get; private set; }
        protected MySqlCommand command;
        protected Action<MySqlDataReader> successCallback;
        protected Action failCallback;

        public override string ToString()
        {
            return procedureName;
        }

        private bool IsNotAsync
        {
            get { return successCallback == null && failCallback == null; }
        }

        public Command() { }
        public Command(DB db)
        {
            this.db = db;
        }
        public Command(DB db, string procedureName)
        {
            this.db = db;
            this.procedureName = procedureName;
            BuildCommand(db, out command, procedureName);
        }
        public Command(DB db, string procedureName, params object[] values)
        {
            this.db = db;
            this.procedureName = procedureName;
            BuildCommand(db, out command, procedureName, values);
        }
        public Command(DB db, string procedureName, Action<MySqlDataReader> successCallback, params object[] values)
        {
            this.db = db;
            this.procedureName = procedureName;
            this.successCallback = successCallback;
            BuildCommand(db, out command, procedureName, values);
        }
        public Command(DB db, string procedureName, Action failCallback, params object[] values)
        {
            this.db = db;
            this.procedureName = procedureName;
            this.failCallback = failCallback;
            BuildCommand(db, out command, procedureName, values);
        }
        public Command(DB db, string procedureName, Action<MySqlDataReader> successCallback, Action failCallback, params object[] values)
        {
            this.db = db;
            this.procedureName = procedureName;
            this.successCallback = successCallback;
            this.failCallback = failCallback;
            BuildCommand(db, out command, procedureName, values);
        }

        public bool SetProcedure(string procedureName, params object[] values)
        {
            this.procedureName = procedureName;
            return BuildCommand(db, out command, procedureName, values);
        }

        internal static bool BuildCommand(MySqlConnection connection, out MySqlCommand command, string procedureName, params object[] values)
        {
            command = new MySqlCommand(procedureName)
            {
                Connection = connection,
                CommandType = CommandType.StoredProcedure,
            };

            if (values.Length % 2 != 0)
                return false;

            for (int i = 0; i < values.Length;)
            {
                if (values[i + 1] is byte[] array)
                {
                    var parameter = new MySqlParameter(values[i++].ToString(), MySqlDbType.Blob, array.Length);
                    parameter.Value = values[i++];
                    command.Parameters.Add(parameter);
                }
                else
                    command.Parameters.AddWithValue(values[i++].ToString(), values[i++]);
            }

            command.Parameters.Add("_error", MySqlDbType.Byte).Direction = ParameterDirection.Output;
            return true;
        }

        internal static bool BuildCommand(DB db, out MySqlCommand command, string procedureName, params object[] values)
        {
            command = db.GetCommand(procedureName);
            if (values.Length % 2 != 0)
                return false;

            for (int i = 0; i < values.Length;)
            {
                if (values[i + 1] is byte[] array)
                {
                    var parameter = new MySqlParameter(values[i++].ToString(), MySqlDbType.Blob, array.Length);
                    parameter.Value = values[i++];
                    command.Parameters.Add(parameter);
                }
                else
                    command.Parameters.AddWithValue(values[i++].ToString(), values[i++]);
            }

            command.Parameters.Add("_error", MySqlDbType.Byte).Direction = ParameterDirection.Output;
            return true;
        }

        public async void Run()
        {
            //if (db == null && db.connection == null && db.connection.State != ConnectionState.Open && command == null)
            //    return;
            try
            {
                if (IsNotAsync)
                    command.ExecuteNonQuery();
                else
                {
                    if (successCallback == null)
                        await command.ExecuteReaderAsync();
                    else
                    {
                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader?.ReadAsync())
                                successCallback(reader);
                            else
                                successCallback(null);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}\n{e.StackTrace}");
                if (failCallback != null)
                {
                    try
                    {
                        failCallback();
                    }
                    catch
                    {
                        return;
                    }
                }
                return;
            }
            return;
        }

        public async Task<bool> Execute(MySqlTransaction tx = null)
        {
            MySqlConnection tempConnection = null;
            command.Transaction = tx;
            if (tx != null)
                command.Connection = tx?.Connection;
            else
            {
                tempConnection = db.GetConnection();
                command.Connection = tempConnection;
            }
            //if (db == null && db.connection == null && db.connection.State != ConnectionState.Open && command == null)
            //    return false;
            try
            {
                if (IsNotAsync)
                    command.ExecuteNonQuery();
                else
                {
                    if (successCallback == null)
                        await command.ExecuteReaderAsync();
                    else
                    {
                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader?.ReadAsync())
                                successCallback(reader);
                            else
                                successCallback(null);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}\n{e.StackTrace}");
                if (failCallback != null)
                {
                    try
                    {
                        failCallback();
                    }
                    catch
                    {
                        return false;
                    }
                }
                return false;
            }
            finally
            {
                if (tempConnection != null)
                    db.ConnectionClose(tempConnection);
                command.Connection = null;
            }
            return true;
        }

        public async void ExecuteAsync(MySqlTransaction tx = null)
        {
            MySqlConnection tempConnection = null;
            command.Transaction = tx;
            if (tx != null)
                command.Connection = tx?.Connection;
            else
            {
                tempConnection = db.GetConnection();
                command.Connection = tempConnection;
            }
            //if (db == null && db.connection == null && db.connection.State != ConnectionState.Open && command == null)
            //    return false;
            try
            {
                if (IsNotAsync)
                    command.ExecuteNonQuery();
                else
                {
                    if (successCallback == null)
                        await command.ExecuteReaderAsync();
                    else
                    {
                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader?.ReadAsync())
                                successCallback(reader);
                            else
                                successCallback(null);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}\n{e.StackTrace}");
                if (failCallback != null)
                {
                    try
                    {
                        failCallback();
                    }
                    catch
                    {
                        return;
                    }
                }
                return;
            }
            finally
            {
                if (tempConnection != null)
                    db.ConnectionClose(tempConnection);
                command.Connection = null;
            }
        }
    }
}