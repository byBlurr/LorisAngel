using System.Collections.Generic;

namespace LorisAngel.Games
{
    public class GameHandler
    {
        private static List<Game> OngoingGames = new List<Game>();

        public static bool DoesGameExist(ulong gameId, GameType gamemode)
        {
            foreach (Game game in OngoingGames)
            {
                if (game.GameId == gameId && game.Gamemode == gamemode)
                {
                    return true;
                }
            }

            return false;
        }

        public static Game GetGame(ulong gameId, GameType gamemode)
        {
            foreach (Game game in OngoingGames)
            {
                if (game.GameId == gameId && game.Gamemode == gamemode)
                {
                    return game;
                }
            }

            return null;
        }

        public static void AddNewGame(Game game)
        {
            if (!OngoingGames.Contains(game)) OngoingGames.Add(game);
        }

        public static void EndGame(Game game)
        {
            if (OngoingGames.Contains(game)) OngoingGames.Remove(game);
        }

        public static ulong CheckForWinner(ulong gameId, GameType gamemode)
        {
            foreach (Game game in OngoingGames)
            {
                if (game.GameId == gameId && game.Gamemode == gamemode)
                {
                    return game.CheckForWinner();
                }
            }

            return 0L;
        }

        public static bool CheckForDraw(ulong gameId, GameType gamemode)
        {
            foreach (Game game in OngoingGames)
            {
                if (game.GameId == gameId && game.Gamemode == gamemode)
                {
                    return game.CheckForDraw();
                }
            }

            return false;
        }

        public static void TakeTurn(ulong gameId, GameType gamemode, ulong userid, int param1, int param2 = 0)
        {
            foreach (Game game in OngoingGames)
            {
                if (game.GameId == gameId && game.Gamemode == gamemode)
                {
                    game.TakeTurn(userid, param1, param2);
                }
            }
        }
    }
}
