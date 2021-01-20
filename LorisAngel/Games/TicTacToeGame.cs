using LorisAngel.Rendering;

namespace LorisAngel.Games
{
    public class TicTacToeGame : Game
    {
        private GridRender GameGrid;

        public TicTacToeGame(ulong id, ulong[] players) : base(id, players)
        {
            GameGrid = new GridRender(id, "ttt", 3, 3, "free", 128);
            Turn = 0;
            Gamemode = GameType.TICTACTOE;
        }

        public override ulong CheckForWinner()
        {
            GridTile[,] Grid = GameGrid.GetGrid();
            string winner = "free";

            if (Grid[0, 0].State == Grid[1, 1].State && Grid[0, 0].State == Grid[2, 2].State) winner = Grid[1, 1].State;
            if (Grid[0, 2].State == Grid[1, 1].State && Grid[0, 2].State == Grid[2, 0].State) winner = Grid[1, 1].State;

            for (int x = 0; x < 3; x++)
                if (Grid[x, 0].State == Grid[x, 1].State && Grid[x, 0].State == Grid[x, 2].State) winner = Grid[x, 0].State;

            for (int y = 0; y < 3; y++)
                if (Grid[0, y].State == Grid[1, y].State && Grid[0, y].State == Grid[2, y].State) winner = Grid[0, y].State;

            ulong winningPlayer = 0L;
            if (winner == "cross") winningPlayer = Players[0];
            else if (winner == "naughts") winningPlayer = Players[1];

            return winningPlayer;
        }

        public override void TakeTurn(ulong player, int x, int y)
        {
            if (Players[Turn] != player) return;

            GridTile[,] Grid = GameGrid.GetGrid();

            x = x - 1;
            y = y - 1;

            if (Grid[x,y].State.Equals("free"))
            {
                if (Turn == 0)
                {
                    Grid[x, y].State = "cross";
                    Turn = 1;
                }
                else if (Turn == 1)
                {
                    Grid[x, y].State = "naughts";
                    Turn = 0;
                }
            }
        }

        public override string RenderGame() => GameGrid.Render();
    }
}
