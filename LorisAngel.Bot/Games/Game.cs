using System;

namespace LorisAngel.Bot.Games
{
    public abstract class Game
    {
        public int Turn;
        public ulong[] Players { get; private set; }
        public ulong GameId { get; private set; }
        public ulong RenderId { get; set; }
        public GameType Gamemode = GameType.UNSET;


        public Game(ulong id, ulong[] players)
        {
            Turn = 0;
            GameId = id;
            Players = players;
        }

        public abstract ulong CheckForWinner();
        public abstract bool CheckForDraw();
        public abstract void TakeTurn(ulong player, int param1, int param2);
        public abstract string RenderGame();

        public override string ToString() => $"Game: {Gamemode.ToString()}, GameId: {GameId}";
        public override bool Equals(object obj) => obj is Game game && GameId == game.GameId && Gamemode == game.Gamemode;
        public override int GetHashCode() => HashCode.Combine(GameId, Gamemode);
    }

    public enum GameType
    {
        UNSET, TICTACTOE, SNAKES, CONNECT, BLACKJACK
    }
}
