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

            // Get 10 rows in desc order
            for (int i = 0; i < 10; i++)
            {
                rows.Add(new LeaderboardRow(0L, "Steve " + i, i * 12));
            }

            // Create leaderboard with rows
            LoriLeaderboard leaderboard = new LoriLeaderboard(lbName, rows);

            return leaderboard;
        }
    }
}
