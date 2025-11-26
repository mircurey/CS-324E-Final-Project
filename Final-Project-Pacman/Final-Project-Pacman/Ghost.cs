using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project_Pacman;

public class Ghost
{
    private Texture2D _texture;
    private Texture2D _pixel; // used for pupils
    
    public Vector2 Position;
    public string Name;
    public int Speed = 2;
        
    public enum Direction { Up, Down, Left, Right }
    public Direction CurrentDirection;

    public Ghost(Texture2D texture, GraphicsDevice graphicsDevice, Vector2 startPosition, string name)
    {
        _texture = texture;
        Position = startPosition;
        Name = name;
        CurrentDirection = Direction.Left;

        // create 1x1 white pixel texture for drawing eyes
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void Update(GameTime gameTime)
    {
        // ghost movement
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // draw ghost body (parent)
        spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, 28, 28), Color.White);

        // draw eyes (child)
        DrawEyes(spriteBatch);
    }

    private void DrawEyes(SpriteBatch spriteBatch)
    {
        Vector2 leftEyePos = Position + new Vector2(6, 8);
        Vector2 rightEyePos = Position + new Vector2(16, 8);

        // pupil offset based on looking direction
        Vector2 lookOffset = Vector2.Zero;
        switch (CurrentDirection)
        {
            case Direction.Up: lookOffset = new Vector2(0, -2); break;
            case Direction.Down: lookOffset = new Vector2(0, 2); break;
            case Direction.Left: lookOffset = new Vector2(-2, 0); break;
            case Direction.Right: lookOffset = new Vector2(2, 0); break;
        }

        // draw whites of eyes
        spriteBatch.Draw(_pixel, new Rectangle((int)leftEyePos.X, (int)leftEyePos.Y, 6, 6), Color.White);
        spriteBatch.Draw(_pixel, new Rectangle((int)rightEyePos.X, (int)rightEyePos.Y, 6, 6), Color.White);

        // draw pupils
        spriteBatch.Draw(_pixel, new Rectangle((int)(leftEyePos.X + 1 + lookOffset.X), (int)(leftEyePos.Y + 1 + lookOffset.Y), 4, 4), Color.Black);
        spriteBatch.Draw(_pixel, new Rectangle((int)(rightEyePos.X + 1 + lookOffset.X), (int)(rightEyePos.Y + 1 + lookOffset.Y), 4, 4), Color.Black);
    }    
}