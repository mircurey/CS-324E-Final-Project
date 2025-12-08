using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Final_Project_Pacman
{
    public class DotManager
    {
        private Texture2D _dotTex;     
        private Texture2D _fruitTex;

        private List<Rectangle> _dots = new();
        private Rectangle _fruitRect;

        public int ScoreValue = 10;
        public int FruitValue = 100;

        private Texture2D _appleTex;
        private Rectangle _apple1Rect;
        private Rectangle _apple2Rect;

        public int AppleValue = 50;

        // grid info used as rails
        private string[] mask;
        private int rows;
        private int cols;
        private int tile = 20;
        private int offsetX = 0;
        private int offsetY = 50;
        private int pelletSize = 6;
        private bool[,] rail; // true if tile is a rail (dot present)

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content,
                                GraphicsDevice graphicsDevice)
        {
            _fruitTex = content.Load<Texture2D>("assets/strawberry");

            _dotTex = new Texture2D(graphicsDevice, 1, 1);
            _dotTex.SetData(new[] { Color.White });

            _appleTex = content.Load<Texture2D>("assets/apple");
        }

        public void GenerateDots(MazeMap maze)
        {
            _dots.Clear();

            tile = 20;
            offsetX = 0;
            offsetY = 50;
            pelletSize = 6;

            mask =
            new string[]
            {
                "0000000000000000000000000000",
                "0000000000000000000000000000",
                "0111111111111001111111111110",
                "0100001000001001000001000010",
                "0100001000001001000001000010",
                "0111111111111111111111111110",
                "0000000000000000000000000000",
                "0100001001000000001001000010",
                "0111111001111001111001111110",
                "0000001000001001000001000000",
                "0000001000001001000001000000",
                "0000001001111111111001000000",
                "0000001001000000001001000000",
                "0000011111000000001111111111",
                "0000000000000000000000000000",
                "0000001001000000001001000000",
                "0000001001111111111001000000",
                "0000001001000000001001000000",
                "0000001001000000001001000000",
                "0111111111111001111111111110",
                "0100001000001001000001000010",
                "0111001111111111111111001110",
                "0000000000000000000000000000",
                "0001001001000000001001001000",
                "0111111001111001111001111110",
                "0100000000001001000000000010",
                "0100000000001001000000000010",
                "0111111111111111111111111110"
            };

            rows = mask.Length;
            cols = mask[0].Length;
            rail = new bool[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (mask[row][col] != '1') continue;

                    int px = offsetX + col * tile + tile / 2;
                    int py = offsetY + row * tile + tile / 2;

                    // tiny per-row offsets used earlier
                    int shiftY = -3;
                    int extraShift = 7;
                    if (row == 5 || row == 13 || row == 21) py += extraShift;
                    py += shiftY;

                    _dots.Add(new Rectangle(px - pelletSize / 2, py - pelletSize / 2, pelletSize, pelletSize));
                    rail[row, col] = true;
                }
            }

            _fruitRect = new Rectangle(270, 300, 16, 16);
            _apple1Rect = new Rectangle(500, 92, 16, 16);
            _apple2Rect = new Rectangle(260, 479, 16, 16);
        }

        // returns true if the provided grid cell is a rail
        public bool IsRailAtGrid(int row, int col)
        {
            if (rail == null) return false;
            if (row < 0 || col < 0 || row >= rail.GetLength(0) || col >= rail.GetLength(1)) return false;
            return rail[row, col];
        }

        // convert world position (pixels) to grid cell
        public Point WorldToGrid(Vector2 world)
        {
            int cx = (int)world.X;
            int cy = (int)world.Y;

            // reverse the shifts used when generating to find nearest cell approx
            int localY = cy - offsetY;
            if (localY < 0) localY = 0;

            int row = localY / tile;
            int col = (cx - offsetX) / tile;
            if (row < 0) row = 0;
            if (col < 0) col = 0;
            if (row >= (rail?.GetLength(0) ?? 1)) row = (rail?.GetLength(0) ?? 1) - 1;
            if (col >= (rail?.GetLength(1) ?? 1)) col = (rail?.GetLength(1) ?? 1) - 1;

            return new Point(row, col);
        }

        // returns true if a world position lands on a rail tile
        public bool IsRailAtWorld(Vector2 world)
        {
            var p = WorldToGrid(world);
            return IsRailAtGrid(p.X, p.Y);
        }

        // helper to get center world position of a grid cell (for snapping)
        public Vector2 GridToWorldCenter(int row, int col)
        {
            int sx = offsetX + col * tile + tile / 2;
            int sy = offsetY + row * tile + tile / 2;
            int shiftY = -3;
            if (row == 5 || row == 13 || row == 21) sy += 7;
            sy += shiftY;
            return new Vector2(sx, sy);
        }

        // Determine if Pacman can move from current world position in the requested direction
        // by checking whether the next tile in that direction contains a rail.
        public bool CanMoveFromWorld(Vector2 worldPos, Pacman.Direction desiredDir, float speed, float dt)
        {
            Point grid = WorldToGrid(worldPos);
            int row = grid.X;
            int col = grid.Y;

            int nr = row;
            int nc = col;
            switch (desiredDir)
            {
                case Pacman.Direction.Up: nr = row - 1; break;
                case Pacman.Direction.Down: nr = row + 1; break;
                case Pacman.Direction.Left: nc = col - 1; break;
                case Pacman.Direction.Right: nc = col + 1; break;
            }

            // If next cell is rail -> allowed
            if (IsRailAtGrid(nr, nc)) return true;

            // fallback: if the exact world pixel ahead is not a wall (color detection), allow it
            Vector2 dirVec = desiredDir switch
            {
                Pacman.Direction.Up => new Vector2(0, -1),
                Pacman.Direction.Down => new Vector2(0, 1),
                Pacman.Direction.Left => new Vector2(-1, 0),
                Pacman.Direction.Right => new Vector2(1, 0),
                _ => Vector2.Zero
            };

            Vector2 probe = worldPos + dirVec * speed * dt;
            return ! /* will be checked by caller: MazeMap.IsWallAtWorld(probe) */ false;
        }

        public int Update(Pacman pac)
        {
            int score = 0;

            for (int i = _dots.Count - 1; i >= 0; i--)
            {
                if (pac.Bounds.Intersects(_dots[i]))
                {
                    score += ScoreValue;
                    pac.PlayEatDot();
                    _dots.RemoveAt(i);
                }
            }

            if (_fruitRect != Rectangle.Empty && pac.Bounds.Intersects(_fruitRect))
            {
                score += FruitValue;
                pac.PlayEatFruit();
                _fruitRect = Rectangle.Empty;
            }

            if (_apple1Rect != Rectangle.Empty && pac.Bounds.Intersects(_apple1Rect))
            {
                score += AppleValue;
                pac.PlayEatFruit();
                _apple1Rect = Rectangle.Empty;
            }

            if (_apple2Rect != Rectangle.Empty && pac.Bounds.Intersects(_apple2Rect))
            {
                score += AppleValue;
                pac.PlayEatFruit();
                _apple2Rect = Rectangle.Empty;
            }

            return score;
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var dot in _dots)
            {
                sb.Draw(_dotTex, dot, Color.Yellow);
            }

            if (_appleTex != null)
            {
                if (_apple1Rect != Rectangle.Empty)
                    sb.Draw(_appleTex, _apple1Rect, Color.White);

                if (_apple2Rect != Rectangle.Empty)
                    sb.Draw(_appleTex, _apple2Rect, Color.White);
            }

            if (_fruitRect != Rectangle.Empty && _fruitTex != null)
            {
                sb.Draw(_fruitTex, _fruitRect, Color.White);
            }
        }
    }
}
