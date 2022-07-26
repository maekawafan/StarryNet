using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using StarryNet.StarryLibrary;

namespace StarryNet.ServerModule
{
    public class DatabaseMs
    {
        private SqlConnection connection;

        public void Open(string ip, string name, string id, string password)
        {
            string connectString = $"Data Source={ip};database={name};Persist Security Info=false;Integrated Security=SSPI;User ID={id};Password={password}enlist=true;";
            connection = new SqlConnection(connectString);
            Task openTask = connection.OpenAsync().ContinueWith((state) => { Log.Info($"DB 연결 - [{state.Status.ToString()}]"); });
        }

        public bool IsOpened()
        {
            return connection.State == ConnectionState.Open;
        }
    }
}
