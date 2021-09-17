using MySqlConnector;

using System.Collections.Generic;
using System.Threading.Tasks;
using StarryNet.StarryLibrary;

namespace StarryNet.ServerModule
{
    public class Transaction
    {
        public static object txLock = new object();
        private int currentCommandIndex = 0;
        private List<Command> commandList = new List<Command>();
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

        public async Task<bool> Commit()
        {
            currentCommandIndex = 1;

            return await db.RunTransaction(async (MySqlTransaction tx) =>
            {
                foreach (Command command in commandList)
                {
                    if (await command.Execute(tx) == false)
                    {
                        Log.Error($"트랜잭션 실패 ({currentCommandIndex}/{CommandCount}) {command?.ToString()}");
                        await tx.RollbackAsync();
                        tx = null;
                        return false;
                    }
                    else
                        currentCommandIndex++;
                }
                await tx.CommitAsync();
                return true;
            });
        }


        public async void CommitAsync()
        {
            currentCommandIndex = 1;

            await db.RunTransaction(async (MySqlTransaction tx) =>
            {
                foreach (var command in commandList)
                {
                    if (await command.Execute(tx) == false)
                    {
                        Log.Error($"트랜잭션 실패 ({currentCommandIndex}/{CommandCount}) {command?.ToString()}");
                        await tx.RollbackAsync();
                        tx = null;
                        return false;
                    }
                    else
                        currentCommandIndex++;
                }
                await tx.CommitAsync();
                return true;
            });
        }
    }
}