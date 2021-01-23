using System;
using System.Collections.Generic;
using System.Text;

namespace LorisAngel.Leaderboards
{
    public abstract class Leaderboard
    {
        private static List<LeaderboardRow> Rows;

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
