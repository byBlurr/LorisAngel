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
        public static async Task CheckAsync(string id, string name, string lbName)
        {
            string tableName;
            switch (lbName)
            {
                case "c4":
                    tableName = "connect_leaderboard";
                    break;
                case "ttt":
                    tableName = "tictactoe_leaderboard";
                    break;
                default:
                    tableName = "bot_leaderboard";
                    break;
            }


            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;

            if (dbCon.IsConnect())
            {
                var cmd = new MySqlCommand($"SELECT 1 FROM {tableName} WHERE id = {id}", dbCon.Connection);
                var reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    var insertCmd = new MySqlCommand($"INSERT INTO {tableName} (id, name, score) VALUES (@id, @name, @score)", dbCon.Connection);
                    cmd.Parameters.Add("@id", MySqlDbType.UInt64).Value = id;
                    cmd.Parameters.Add("@name", MySqlDbType.String).Value = name;
                    cmd.Parameters.Add("@score", MySqlDbType.Int32).Value = 0;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        cmd.Dispose();
                    }
                    catch (Exception ex)
                    {
                        await Util.Logger(new LogMessage(LogSeverity.Error, "Leaderboards", ex.Message));
                        cmd.Dispose();
                    }
                }
            }
        }

        // Add score
        public static async Task AddScoreAsync(ulong userId, string lbName)
        {
        }

        // Get leaderboard
        public static async Task<Leaderboard> GetLeaderboardAsync(string lbName, int count = 10)
        {


            return null;
        }
    }
}
