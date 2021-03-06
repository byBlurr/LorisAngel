﻿using System.Collections.Generic;
using System.Linq;

namespace LorisAngel.Common.Objects
{
    public class LoriLeaderboard
    {
        public string Name { get; private set; }
        private List<LeaderboardRow> Rows;

        public LoriLeaderboard(string name, List<LeaderboardRow> rows)
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

        public int GetPosition(ulong user)
        {
            List<LeaderboardRow> top = Rows.OrderByDescending(x => x.Score).ToList();
            for (int i = 0; i < top.Count; i++)
            {
                if (top[i].Id == user) return i + 1;
            }
            return Rows.Count + 1;
        }

        public string GetPositionAsString(ulong user)
        {
            return $"#{GetPosition(user)} of {GetSize()}";
        }

        public int GetSize()
        {
            return Rows.Count;
        }

        // Helper method for getting table name from leaderboard name
        public static string GetTableName(string lbName)
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

        // Helper method for getting leaderboard name from table name
        public static string GetLeaderboardName(string tableName)
        {
            string lbName;
            switch (tableName)
            {
                case "connect_leaderboard":
                    lbName = "Connect 4";
                    break;
                case "tictactoe_leaderboard":
                    lbName = "Tic Tac Toe";
                    break;
                default:
                    lbName = "Leaderboard";
                    break;
            }

            return lbName;
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
