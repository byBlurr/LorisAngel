using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace LorisAngel.Rendering
{
    public class GridRender
    {
        private ulong Id;
        private string Name;
        private int TileSize;
        private GridTile[,] Grid;

        public GridRender(ulong id, string name, int width, int height, string defaultState, int tileSize = 64)
        {
            Grid = new GridTile[width, height];
            for (int x = 0; x < Grid.GetLength(0); x++)
                for (int y = 0; y < Grid.GetLength(1); y++)
                    Grid[x, y] = new GridTile(defaultState);

            Id = id;
            Name = name;
            TileSize = tileSize;
        }

        public string Render()
        {
            int width = (Grid.GetLength(0) * TileSize) + TileSize;
            int height = (Grid.GetLength(1) * TileSize) + TileSize;

            Bitmap editedBitmap = new Bitmap(width, height);
            Graphics graphicImage = Graphics.FromImage(editedBitmap);
            graphicImage.SmoothingMode = SmoothingMode.AntiAlias;

            // Only render background if the file exists, not all games have background
            string backgroundPath = Path.Combine(AppContext.BaseDirectory, $"textures/background_{Name}.png");
            if (File.Exists(backgroundPath))
            {
                Bitmap backTexture = new Bitmap(backgroundPath);
                graphicImage.DrawImage(backTexture, 0, 0, width, height);
            }

            for (int x = 0; x < Grid.GetLength(0); x++)
            {
                for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    string tileTexturePath = Path.Combine(AppContext.BaseDirectory, $"textures/tile_{Name}_{Grid[x,y].State}.png");
                    Bitmap tileTexture = new Bitmap(tileTexturePath);

                    graphicImage.DrawImage(tileTexture, (x * TileSize) + (TileSize / 2), (y * TileSize) + (TileSize / 2), TileSize, TileSize);
                    tileTexture.Dispose();
                }
            }

            // Save board
            string path = Path.Combine(AppContext.BaseDirectory, $"textures/grid_{Name}_{Id}.jpg");
            editedBitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);

            graphicImage.Dispose();
            editedBitmap.Dispose();

            // Return path to board
            return path;
        }

        public void SetTileState(string state, int x, int y)
        {
            Grid[x, y].State = state;
        }

        public GridTile[,] GetGrid()
        {
            return Grid;
        }
    }

    public class GridTile
    {
        public string State { get; set; }
        public GridTile(string state)
        {
            State = state;
        }
    }
}
