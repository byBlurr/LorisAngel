using System.Collections.Generic;
using System.Linq;

namespace LorisAngel.Leaderboards
{
    public class Leaderboard
    {
        public string Name { get; private set; }
        private List<LeaderboardRow> Rows;

        public Leaderboard(string name, List<LeaderboardRow> rows)
        {
            Name = name;
            Rows = rows;
        }

        public List<LeaderboardRow> GetTop(int count = 10)
        {
            List<LeaderboardRow> top = Rows.OrderByDescending(x => x.Score).ToList();
            if (top.Count > count) top.RemoveRange(count, top.Count - count);
            return top;
        }

        public List<LeaderboardRow> GetBottom(int count = 10)
        {
            List<LeaderboardRow> top = Rows.OrderBy(x => x.Score).ToList();
            if (top.Count > count) top.RemoveRange(count, top.Count - count);
            return top;
        }

        public LeaderboardRow GetUser(ulong user)
        {
            foreach (LeaderboardRow row in Rows) if (row.Id == user) return row;
            return null;
        }
    }

    public class LeaderboardRow
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public int Score { get; private set; }

        public LeaderboardRow(ulong id, string name, int score)
        {
            Id = id;
            Name = name;
            Score = score;
        }
    }
}
