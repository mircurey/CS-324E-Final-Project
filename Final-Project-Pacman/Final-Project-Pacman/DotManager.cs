using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Final_Project_Pacman; 

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

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            _dotTex = Content.Load<Texture2D>("assets/dot");
            _fruitTex = Content.Load<Texture2D>("assets/strawberry");
        }

        public void GenerateDots(bool[,] walls)
        {
            _dots.Clear();

            for (int y = 0; y < 31; y++)
            {
                for (int x = 0; x < 28; x++)
                {
                    if (!walls[y, x])
                    {
                        _dots.Add(new Rectangle(x * 20 + 8, y * 20 + 58, 4, 4));
                    }
                }
            }

            _fruitRect = new Rectangle(260, 300, 16, 16);
        }

        public int Update(Pacman pac)
        {
            int score = 0;

            // dot eating
            for (int i = _dots.Count - 1; i >= 0; i--)
            {
                if (pac.Bounds.Intersects(_dots[i]))
                {
                    score += ScoreValue;
                    _dots.RemoveAt(i);
                }
            }

            // fruit eating
            if (_fruitRect != Rectangle.Empty && pac.Bounds.Intersects(_fruitRect))
            {
                score += FruitValue;
                _fruitRect = Rectangle.Empty;
            }

            return score;
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var dot in _dots)
                sb.Draw(_dotTex, dot, Color.White);

            if (_fruitRect != Rectangle.Empty)
                sb.Draw(_fruitTex, _fruitRect, Color.White);
        }
    }
}
