using System;
using System.Collections.Generic;
using System.Text;

namespace LorisAngel.Games
{
    public abstract class Game
    {
        private int Turn;
        private ulong[] Players;
        public ulong GameId { get; private set; }
        public ulong RenderId { get; set; }
        private GameType Gamemode = GameType.UNSET;

        public Game(ulong id, ulong[] players)
        {
            Turn = 0;
            GameId = id;
            Players = players;
        }

        public abstract ulong CheckForWinner();
        public abstract void TakeTurn(int param1, int param2);

        public override string ToString() => $"Game: {Gamemode.ToString()}, GameId: {GameId}";
        public override bool Equals(object obj) => obj is Game game && GameId == game.GameId && Gamemode == game.Gamemode;
        public override int GetHashCode() => HashCode.Combine(GameId, Gamemode);
    }

    public enum GameType
    {
        UNSET, TICTACTOE, SNAKES, CONNECT, BLACKJACK
    }
}
