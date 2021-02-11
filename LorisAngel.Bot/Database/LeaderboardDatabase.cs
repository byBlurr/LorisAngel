using Discord;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Sql;
using LorisAngel.Common.Objects;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LorisAngel.Bot.Database
{
    public class LeaderboardDatabase
    {
        // Check if user is on scoreboard
        public static async Task CheckAsync(ulong id, string name, string lbName)
        {
            string tableName;
            tableName = LoriLeaderboard.GetTableName(lbName);

            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;

            if (dbCon.IsConnect())
            {
                var cmd = new MySqlCommand($"SELECT 1 FROM {tableName} WHERE id = {id}", dbCon.Connection);
                var reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    while (LCommandHandler.Saving) await Task.Delay(50);
                    LCommandHandler.Saving = true;

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
                        await Util.LoggerAsync(new LogMessage(LogSeverity.Error, "Leaderboards", ex.Message));
                        insertCmd.Dispose();
                    }
                    LCommandHandler.Saving = false;

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
            tableName = LoriLeaderboard.GetTableName(lbName);

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
                        while (LCommandHandler.Saving) await Task.Delay(50);
                        LCommandHandler.Saving = true;

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
                            await Util.LoggerAsync(new LogMessage(LogSeverity.Error, "Leaderboards", ex.Message));
                            updateCmd.Dispose();
                        }
                        LCommandHandler.Saving = false;
                    }
                    cmd.Dispose();
                    reader.Dispose();
                }
                dbCon.Close();
            }
        }

        // Get leaderboard
        public static async Task<LoriLeaderboard> GetLeaderboardAsync(string lbName, int count = 10)
        {
            LoriLeaderboard lb = null;

            string tableName;
            tableName = LoriLeaderboard.GetTableName(lbName);

            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;

            if (dbCon.IsConnect())
            {
                var cmd = new MySqlCommand($"SELECT * FROM {tableName}", dbCon.Connection);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    List<LeaderboardRow> rows = new List<LeaderboardRow>();

                    while (reader.Read())
                    {
                        LeaderboardRow row = new LeaderboardRow(reader.GetUInt64(0), reader.GetString(1), reader.GetInt32(2));
                        rows.Add(row);
                    }

                    lb = new LoriLeaderboard(LoriLeaderboard.GetLeaderboardName(tableName), rows);
                    cmd.Dispose();
                    reader.Dispose();
                }
                dbCon.Close();
            }

            return lb;
        }
    }
}
