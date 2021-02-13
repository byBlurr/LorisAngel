using Blurr;
using Blurr.Sql;
using LorisAngel.Common.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LorisAngel.Webpanel.Data
{
    public class LeaderboardService
    {
        public async Task<LoriLeaderboard> GetLeaderboardAsync(string lbName)
        {
            if (String.IsNullOrEmpty(lbName)) return null;

            string tableName = LoriLeaderboard.GetTableName(lbName);
            List<LeaderboardRow> rows = new List<LeaderboardRow>();


            // Connect to db
            SqlConnection connection = SqlConnection.Instance();
            connection.DatabaseName = "lorisangel";
            // connection.DatabasePassword = ""; Guess we gonna need a local file for this...

            try
            {
                string[] columns = { "id", "name", "score" };
                var sqlRows = SqlHelper.SelectDataFromTable<LeaderboardDBRow>(connection, tableName, columns);

                foreach (var row in sqlRows)
                {
                    rows.Add(new LeaderboardRow((ulong)row.id, row.name, row.score));
                }
            }
            catch (Exception ex)
            {
                Util.Log(LogType.Error, "MySql", ex.Message);
            }

            // Create leaderboard with rows
            LoriLeaderboard leaderboard = new LoriLeaderboard(lbName, rows);
            return leaderboard;
        }
    }

    public class LeaderboardDBRow
    {
        public long id { get; set; }
        public string name { get; set; }
        public int score { get; set; }
    }
}
