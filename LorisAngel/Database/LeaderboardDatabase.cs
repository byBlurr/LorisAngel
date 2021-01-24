using Discord;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Sql;
using LorisAngel.Leaderboards;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngel.Database
{
    public class LeaderboardDatabase
    {
        // Check if user is on scoreboard
        public static async Task CheckAsync(ulong id, string name, string lbName)
        {
            string tableName;
            tableName = GetTableName(lbName);

            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;

            if (dbCon.IsConnect())
            {
                var cmd = new MySqlCommand($"SELECT 1 FROM {tableName} WHERE id = {id}", dbCon.Connection);
                var reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    var insertCmd = new MySqlCommand($"INSERT INTO {tableName} (id, name, score) VALUES (@id, @name, @score)", dbCon.Connection);
                    insertCmd.Parameters.Add("@id", MySqlDbType.UInt64).Value = id;
                    insertCmd.Parameters.Add("@name", MySqlDbType.String).Value = name;
                    insertCmd.Parameters.Add("@score", MySqlDbType.Int32).Value = 0;

                    try
                    {
                        await insertCmd.ExecuteNonQueryAsync();
                        insertCmd.Dispose();
                    }
                    catch (Exception ex)
                    {
                        await Util.Logger(new LogMessage(LogSeverity.Error, "Leaderboards", ex.Message));
                        insertCmd.Dispose();
                    }

                    cmd.Dispose();
                    reader.Dispose();
                }
                dbCon.Close();
            }
        }

        // Add score
        public static async Task AddScoreAsync(ulong userId, string lbName)
        {
            var user = CommandHandler.GetBot().GetUser(userId);
            string username = user.Username;
            await CheckAsync(userId, username, lbName);

            string tableName;
            tableName = GetTableName(lbName);

            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;

            if (dbCon.IsConnect())
            {
                var cmd = new MySqlCommand($"SELECT 1 FROM {tableName} WHERE id = {userId}", dbCon.Connection);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int newScore = reader.GetInt32(2) + 1;

                        var updateCmd = new MySqlCommand($"UPDATE {tableName} SET score = @score WHERE id = @id", dbCon.Connection);
                        updateCmd.Parameters.Add("@id", MySqlDbType.UInt64).Value = userId;
                        updateCmd.Parameters.Add("@score", MySqlDbType.Int32).Value = newScore;

                        try
                        {
                            await updateCmd.ExecuteNonQueryAsync();
                            updateCmd.Dispose();
                        }
                        catch (Exception ex)
                        {
                            await Util.Logger(new LogMessage(LogSeverity.Error, "Leaderboards", ex.Message));
                            updateCmd.Dispose();
                        }
                    }
                    cmd.Dispose();
                    reader.Dispose();
                }
                dbCon.Close();
            }
        }

        // Get leaderboard
        public static async Task<Leaderboard> GetLeaderboardAsync(string lbName, int count = 10)
        {


            return null;
        }

        // Helper method for getting table name from leaderboard name
        private static string GetTableName(string lbName)
        {
            string tableName;
            switch (lbName)
            {
                case "Connect 4":
                    tableName = "connect_leaderboard";
                    break;
                case "Tic Tac Toe":
                    tableName = "tictactoe_leaderboard";
                    break;
                default:
                    tableName = "bot_leaderboard";
                    break;
            }

            return tableName;
        }
    }
}
