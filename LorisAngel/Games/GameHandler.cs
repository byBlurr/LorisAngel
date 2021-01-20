using System;
using System.Collections.Generic;
using System.Text;

namespace LorisAngel.Games
{
    public class GameHandler
    {
        private static List<Game> OngoingGames = new List<Game>();

        public static bool DoesGameExist(ulong gameId)
        {
            foreach (Game game in OngoingGames)
            {
                if (game.GameId == gameId)
                {
                    return true;
                }
            }

            return false;
        }

        public static void AddNewGame(Game game)
        {
            if (!OngoingGames.Contains(game)) OngoingGames.Add(game);
        }

        public static void EndGame(Game game)
        {
            if (OngoingGames.Contains(game)) OngoingGames.Remove(game);
        }

        public static ulong CheckForWinner(ulong gameId)
        {
            foreach (Game game in OngoingGames)
            {
                if (game.GameId == gameId)
                {
                    return game.CheckForWinner();
                }
            }

            return 0L;
        }

        public static void TakeTurn(ulong gameId, int param1, int param2)
        {
            foreach (Game game in OngoingGames)
            {
                if (game.GameId == gameId)
                {
                    game.TakeTurn(param1, param2);
                }
            }
        }
    }
}
