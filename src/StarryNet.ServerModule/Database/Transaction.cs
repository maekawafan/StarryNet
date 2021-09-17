using MySqlConnector;

using System.Collections.Generic;
using StarryNet.StarryLibrary;

namespace StarryNet.ServerModule
{
    public class Transaction
    {
        public static object txLock = new object();
        private int currentCommandIndex = 0;
        private List<Command> commandList = new List<Command>();
        private MySqlTransaction transaction;
        private DB db;
        private int CommandCount { get { return commandList.Count; } }

        public Transaction(DB db)
        {
            this.db = db;
        }

        public void AddCommand(Command command)
        {
            commandList.Add(command);
        }

        public void ClearCommand()
        {
            commandList.Clear();
        }

        public async void Commit()
        {
            currentCommandIndex = 1;

            await db.RunTransaction(async () =>
            {
                using (transaction)
                {
                    foreach (var command in commandList)
                    {
                        if (await command.Execute(transaction) == false)
                        {
                            Log.Error($"트랜잭션 실패 ({currentCommandIndex}/{CommandCount}) {command?.ToString()}");
                            await transaction.RollbackAsync();
                            transaction = null;
                            return false;
                        }
                        else
                            currentCommandIndex++;
                    }
                    await transaction.CommitAsync();
                }
                transaction = null;
                return true;
            });
        }
    }
}