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

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content,
                                GraphicsDevice graphicsDevice)
        {
            _fruitTex = content.Load<Texture2D>("assets/strawberry");

            _dotTex = new Texture2D(graphicsDevice, 1, 1);
            _dotTex.SetData(new[] { Color.White });
        }

        public void GenerateDots(MazeMap maze)
        {
            _dots.Clear();

            int pelletSize = 6;
            int tile = 20;

            int offsetX = 0;
            int offsetY = 50;
            int shiftY = -3;       
            int extraShift = 7;   

            string[] mask =
            {
                "0000000000000000000000000000",
                "0000000000000000000000000000", // row 1
                "0111111111111001111111111110", // row 2
                "0100001000001001000001000010", // row 3
                "0100001000001001000001000010", // row 4
                "0111111111111111111111111110", // row 5  
                "0000000000000000000000000000", // row 6
                "0100001001000000001001000010", // row 7
                "0111111001111001111001111110", // row 8
                "0000001000001001000001000000", // row 9
                "0000001000001001000001000000", // row 10
                "0000001001111111111001000000", // row 11
                "0000001001000000001001000000", // row 12
                "0000011111000000001111111111", // row 13 
                "0000000000000000000000000000", // row 14
                "0000001001000000001001000000", // row 15
                "0000001001111111111001000000", // row 16
                "0000001001000000001001000000", // row 17
                "0000001001000000001001000000", // row 18
                "0111111111111001111111111110", // row 19
                "0100001000001001000001000010", // row 20
                "0111001111111111111111001110", // row 21 
                "0000000000000000000000000000", // row 22
                "0001001001000000001001001000", // row 23
                "0111111001111001111001111110", // row 24
                "0100000000001001000000000010", // row 25
                "0100000000001001000000000010", // row 26
                "0111111111111111111111111110"  // row 27
            };

            int rows = mask.Length;
            int cols = mask[0].Length;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (mask[row][col] != '1')
                        continue;

                    int px = offsetX + col * tile + tile / 2;
                    int py = offsetY + row * tile + tile / 2 + shiftY;

                    if (row == 5 || row == 13 || row == 21)
                        py += extraShift;

                    _dots.Add(new Rectangle(
                        px - pelletSize / 2,
                        py - pelletSize / 2,
                        pelletSize,
                        pelletSize
                    ));
                }
            }

            _fruitRect = new Rectangle(270, 300 + shiftY + 20, 16, 16);
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

            return score;
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var dot in _dots)
            {
                var bigDot = new Rectangle(dot.X - 2, dot.Y - 2, dot.Width + 6, dot.Height + 6);
                sb.Draw(_dotTex, dot, Color.Yellow);
            }

            if (_fruitRect != Rectangle.Empty && _fruitTex != null)
            {
                sb.Draw(_fruitTex, _fruitRect, Color.White);
            }
        }
    }
}
