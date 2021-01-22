using LorisAngel.Rendering;
using System;

namespace LorisAngel.Games
{
    public class ConnectGame : Game
    {
        private const string FREE_STATE = "free";
        private const string PLAYER1_STATE = "yellow";
        private const string PLAYER2_STATE = "red";

        private GridRender GameGrid;
        private bool Filled;

        public ConnectGame(ulong id, ulong[] players) : base(id, players)
        {
            GameGrid = new GridRender(id, "connect", 7, 6, FREE_STATE, 128);
            Turn = 0;
            Gamemode = GameType.CONNECT;
            Filled = false;
            Random rnd = new Random();
            Turn = rnd.Next(0, 1);
        }

        public override ulong CheckForWinner()
        {
            bool filled = true;
            var Board = GameGrid.GetGrid();

            for (int x = 0; x < Board.GetLength(0); x++)
            {
                for (int y = 0; y < Board.GetLength(1); y++)
                {
                    string toFind = Board[x, y].State;

                    if (toFind.Equals(FREE_STATE)) filled = false;

                    // Check horizontal
                    int connects = 0;
                    int toCheckFrom = x - 3;
                    int toCheckTo = x + 3;
                    if (toCheckFrom < 0) toCheckFrom = 0;
                    if (toCheckTo >= Board.GetLength(0)) toCheckTo = Board.GetLength(0) - 1;
                    for (int toCheck = toCheckFrom; toCheck <= toCheckTo; toCheck++)
                    {
                        if (Board[toCheck, y].State.Equals(toFind)) connects++;
                        else connects = 0;

                        if (connects == 4 && !toFind.Equals(FREE_STATE))
                        {
                            if (toFind.Equals(PLAYER1_STATE)) return Players[0];
                            else if (toFind.Equals(PLAYER2_STATE)) return Players[1];
                        }
                    }

                    // Check vertical
                    connects = 0;
                    toCheckFrom = y - 3;
                    toCheckTo = y + 3;
                    if (toCheckFrom < 0) toCheckFrom = 0;
                    if (toCheckTo >= Board.GetLength(1)) toCheckTo = Board.GetLength(1) - 1;
                    for (int toCheck = toCheckFrom; toCheck <= toCheckTo; toCheck++)
                    {
                        if (Board[x, toCheck].State.Equals(toFind)) connects++;
                        else connects = 0;

                        if (connects == 4 && !toFind.Equals(FREE_STATE))
                        {
                            if (toFind.Equals(PLAYER1_STATE)) return Players[0];
                            else if (toFind.Equals(PLAYER2_STATE)) return Players[1];
                        }
                    }



                    // Check diag \
                    connects = 0;
                    int toCheckFromX = x - 3;
                    int toCheckFromY = y + 3;
                    int toCheckToX = x + 3;
                    int toCheckToY = y - 3;

                    if (toCheckFromX < 0) toCheckFromX = 0;
                    if (toCheckFromY >= Board.GetLength(1)) toCheckFromY = Board.GetLength(1) - 1;
                    if (toCheckToX >= Board.GetLength(0)) toCheckToX = Board.GetLength(0) - 1;
                    if (toCheckToY < 0) toCheckToY = 0;

                    for (int i = 0; i < 7; i++)
                    {
                        if (toCheckFromX + i <= toCheckToX && toCheckFromY - i >= toCheckToY)
                        {
                            if (Board[toCheckFromX + i, toCheckFromY - i].State.Equals(toFind)) connects++;
                            else connects = 0;

                            if (connects == 4 && !toFind.Equals(FREE_STATE))
                            {
                                if (toFind.Equals(PLAYER1_STATE)) return Players[0];
                                else if (toFind.Equals(PLAYER2_STATE)) return Players[1];
                            }
                        }
                    }

                    // Check diag /
                    connects = 0;
                    toCheckFromX = x - 3;
                    toCheckFromY = y - 3;
                    toCheckToX = x + 3;
                    toCheckToY = y + 3;

                    if (toCheckFromX < 0) toCheckFromX = 0;
                    if (toCheckFromY < 0) toCheckFromY = 0;
                    if (toCheckToX >= Board.GetLength(0)) toCheckToX = Board.GetLength(0) - 1;
                    if (toCheckToY >= Board.GetLength(1)) toCheckToY = Board.GetLength(1) - 1;

                    for (int i = 0; i < 7; i++)
                    {
                        if (toCheckFromX + i <= toCheckToX && toCheckFromY + i <= toCheckToY)
                        {
                            if (Board[toCheckFromX + i, toCheckFromY + i].State.Equals(toFind)) connects++;
                            else connects = 0;

                            if (connects == 4 && !toFind.Equals(FREE_STATE))
                            {
                                if (toFind.Equals(PLAYER1_STATE)) return Players[0];
                                else if (toFind.Equals(PLAYER2_STATE)) return Players[1];
                            }
                        }
                    }
                }
            }

            Filled = filled;
            return 0L;
        }

        public bool CheckForDraw() => Filled;

        public override string RenderGame() => GameGrid.Render();

        public override void TakeTurn(ulong player, int slot, int _)
        {
            var Board = GameGrid.GetGrid();
            if (slot >= 0 && slot < Board.GetLength(0))
            {
                int currentY = Board.GetLength(1) - 1;
                if (Board[slot, currentY].State.Equals(FREE_STATE))
                {
                    string newState = PLAYER1_STATE;
                    if (Turn == 1)
                    {
                        newState = PLAYER2_STATE;
                        Turn = 0;
                    }
                    else Turn = 1;

                    bool foundPosition = false;
                    while (!foundPosition)
                    {
                        if (currentY - 1 < 0) break;

                        if (Board[slot, currentY - 1].State == FREE_STATE) currentY--;
                        else foundPosition = true;
                    }

                    Board[slot, currentY].State = newState;
                }
            }
        }
    }
}
