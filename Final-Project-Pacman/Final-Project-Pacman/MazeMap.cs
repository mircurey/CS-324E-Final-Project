using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Final_Project_Pacman
{
    public class MazeMap
    {
        private int texWidth;
        private int texHeight;
        private Color[] pixelData;

        private Rectangle drawRect;
        private float scaleX;
        private float scaleY;

        private Texture2D debugPixel;

       
        private readonly Color wallColor = new Color(0, 162, 255);

        public MazeMap(Texture2D texture, Rectangle drawRectangle)
        {
            texWidth = texture.Width;
            texHeight = texture.Height;

            pixelData = new Color[texWidth * texHeight];
            texture.GetData(pixelData);

            drawRect = drawRectangle;
            scaleX = (float)drawRect.Width / texWidth;
            scaleY = (float)drawRect.Height / texHeight;
        }

        public void InitDebug(GraphicsDevice gd)
        {
            debugPixel = new Texture2D(gd, 1, 1);
            debugPixel.SetData(new[] { Color.White });
        }

      
        private int ColorDistanceSq(Color a, Color b)
        {
            int dr = a.R - b.R;
            int dg = a.G - b.G;
            int db = a.B - b.B;
            return dr * dr + dg * dg + db * db;
        }

        
        public bool IsWallAtWorld(Vector2 worldPos)
        {
            float lx = worldPos.X - drawRect.X;
            float ly = worldPos.Y - drawRect.Y;

            // wall
            if (lx < 0 || ly < 0 || lx >= drawRect.Width || ly >= drawRect.Height)
                return true;

            int tx = (int)(lx / scaleX);
            int ty = (int)(ly / scaleY);

            if (tx < 0 || ty < 0 || tx >= texWidth || ty >= texHeight)
                return true;

            Color c = pixelData[ty * texWidth + tx];

            
            bool rgbCheck =
                c.B > 200 &&      // bright blue
                c.G > 120 &&      // medium green
                c.R < 80;         // low red

            
            int distSq = ColorDistanceSq(c, wallColor);
            bool distanceCheck = distSq < 5000; 

            return rgbCheck || distanceCheck;
        }

       
        public void DrawDebug(SpriteBatch sb)
        {
            if (debugPixel == null) return;

            for (int ty = 0; ty < texHeight; ty++)
            {
                for (int tx = 0; tx < texWidth; tx++)
                {
                    Color c = pixelData[ty * texWidth + tx];

                    bool rgbCheck =
                        c.B > 200 &&
                        c.G > 120 &&
                        c.R < 80;

                    int distSq = ColorDistanceSq(c, wallColor);
                    bool distanceCheck = distSq < 5000;

                    if (!(rgbCheck || distanceCheck))
                        continue;

                    // Correct scaled drawing
                    int sx = drawRect.X + (int)(tx * scaleX);
                    int sy = drawRect.Y + (int)(ty * scaleY);
                    int sw = (int)Math.Ceiling(scaleX);
                    int sh = (int)Math.Ceiling(scaleY);

                    sb.Draw(
                        debugPixel,
                        new Rectangle(sx, sy, sw, sh),
                        Color.White * 0.45f
                    );
                }
            }
        }
    }
}
