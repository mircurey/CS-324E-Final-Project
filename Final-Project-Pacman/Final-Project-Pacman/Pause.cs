using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project_Pacman
{
    public class Pause
    {
        private Texture2D _pauseTex;

        public Rectangle ButtonRect { get; private set; }
        public bool IsPaused { get; private set; }

        private const float SCALE = 0.10f;
        private const int X_POS = 430;
        private const int Y_POS = 30;

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content,
                                GraphicsDevice graphicsDevice)
        {
            _pauseTex = content.Load<Texture2D>("assets/Pause");

            int width = (int)(_pauseTex.Width * SCALE);
            int height = (int)(_pauseTex.Height * SCALE);

            ButtonRect = new Rectangle(X_POS, Y_POS, width, height);
        }

        public void TogglePause()
        {
            IsPaused = !IsPaused;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_pauseTex, ButtonRect, Color.White);
        }
    }
}
