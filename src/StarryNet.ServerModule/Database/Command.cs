using MySqlConnector;

using System;
using System.Data;
using System.Threading.Tasks;

namespace StarryNet.ServerModule
{
    public class Command
    {
        public DB db { get; private set; }
        public string procedureName { get; private set; }
        private MySqlCommand command;
        private Action<MySqlDataReader> successCallback;
        private Action failCallback;

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

        internal static bool BuildCommand(DB.Connection connection, out MySqlCommand command, string procedureName, params object[] values)
        {
            command = new MySqlCommand(procedureName)
            {
                Connection = connection.connection,
                CommandType = CommandType.StoredProcedure,
            };

            if (values.Length % 2 != 0)
                return false;

            for (int i = 0; i < values.Length;)
                command.Parameters.AddWithValue(values[i++].ToString(), values[i++]);
            return true;
        }

        internal static bool BuildCommand(DB db, out MySqlCommand command, string procedureName, params object[] values)
        {
            command = db.GetCommand(procedureName);
            if (values.Length % 2 != 0)
                return false;

            for (int i = 0; i < values.Length;)
                command.Parameters.AddWithValue(values[i++].ToString(), values[i++]);
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
                        MySqlDataReader reader = await command.ExecuteReaderAsync();
                        using (reader)
                        {
                            if (await reader.ReadAsync())
                                successCallback(reader);
                        }
                    }
                }
            }
            catch
            {
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
            command.Transaction = tx;
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
                        MySqlDataReader reader = await command.ExecuteReaderAsync();
                        using (reader)
                        {
                            if (await reader.ReadAsync())
                                successCallback(reader);
                        }
                    }
                }
            }
            catch
            {
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
            return true;
        }
    }
}