using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Final_Project_Pacman
{
    public class Ghost
    {
        private Texture2D _texture;
        private Texture2D _pixel;
        public Vector2 Position;
        public string Name;
        public float Speed = 80f;

        public enum Direction { Up, Down, Left, Right }
        public Direction CurrentDirection;

        private Random _rng = new Random();

        public bool IsReleased = false;   
        public bool IsMovingOutOfHouse = false;

        private const int Size = 28;

        public Ghost(Texture2D texture, GraphicsDevice gd, Vector2 startPos, string name)
        {
            _texture = texture;
            Position = startPos;
            Name = name;

            CurrentDirection = Direction.Left;

            _pixel = new Texture2D(gd, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime, MazeMap maze)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsReleased)
                return;

            if (IsMovingOutOfHouse)
            {
                MoveOutOfGhostHouse(dt);
                return;
            }

            MoveNormal(dt, maze);
        }

        private void MoveNormal(float dt, MazeMap maze)
        {
            Vector2 dir = DirectionToVector(CurrentDirection);
            Vector2 next = Position + dir * Speed * dt;

            if (CollidesWithWall(next, maze))
            {
                CurrentDirection = (Direction)_rng.Next(4);
                return;
            }

            Position = next;
        }

        private void MoveOutOfGhostHouse(float dt)
        {
            float exitY = 210; // ghost house exit
            Position += new Vector2(0, -Speed * dt);

            if (Position.Y <= exitY)
            {
                Position.Y = exitY;
                IsMovingOutOfHouse = false;
                CurrentDirection = Direction.Left;
            }
        }

        private Vector2 DirectionToVector(Direction d)
        {
            return d switch
            {
                Direction.Up => new Vector2(0, -1),
                Direction.Down => new Vector2(0, 1),
                Direction.Left => new Vector2(-1, 0),
                Direction.Right => new Vector2(1, 0),
                _ => Vector2.Zero
            };
        }

        private bool CollidesWithWall(Vector2 worldPos, MazeMap maze)
        {
            var checks = new Vector2[]
            {
                new Vector2(worldPos.X+2, worldPos.Y+2),
                new Vector2(worldPos.X+Size-2, worldPos.Y+2),
                new Vector2(worldPos.X+2, worldPos.Y+Size-2),
                new Vector2(worldPos.X+Size-2, worldPos.Y+Size-2)
            };

            foreach (var c in checks)
                if (maze.IsWallAtWorld(c))
                    return true;

            return false;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, Size, Size), Color.White);
            DrawEyes(sb);
        }

        private void DrawEyes(SpriteBatch sb)
        {
            Vector2 leftEye = Position + new Vector2(6, 8);
            Vector2 rightEye = Position + new Vector2(16, 8);

            Vector2 offset = CurrentDirection switch
            {
                Direction.Up => new Vector2(0, -2),
                Direction.Down => new Vector2(0, 2),
                Direction.Left => new Vector2(-2, 0),
                Direction.Right => new Vector2(2, 0),
                _ => Vector2.Zero
            };

            sb.Draw(_pixel, new Rectangle((int)leftEye.X, (int)leftEye.Y, 6, 6), Color.White);
            sb.Draw(_pixel, new Rectangle((int)rightEye.X, (int)rightEye.Y, 6, 6), Color.White);

            sb.Draw(_pixel, new Rectangle((int)(leftEye.X + 1 + offset.X), (int)(leftEye.Y + 1 + offset.Y), 4, 4), Color.Black);
            sb.Draw(_pixel, new Rectangle((int)(rightEye.X + 1 + offset.X), (int)(rightEye.Y + 1 + offset.Y), 4, 4), Color.Black);
        }
    }
}
