using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace Final_Project_Pacman
{
    public class Pacman
    {
        private Dictionary<string, Texture2D[]> _frames;
        private float _animTimer;
        private int _frameIndex;
        private float _animInterval = 100f; 
        private SoundManager _sound;

        public Vector2 Position;
        public float Speed = 100f; 
        public bool IsMoving { get; private set; }

        public enum Direction { Up, Down, Left, Right }
        public Direction CurrentDirection = Direction.Left;

        // collision size 
        public int Size = 16;

        private float pacmanScale = 1.8f;

        public Pacman(SoundManager sound)
        {
            _frames = new Dictionary<string, Texture2D[]>();
            _sound = sound;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            _frames["Right"] = new Texture2D[] {
                Content.Load<Texture2D>("assets/pacman-right-1"),
                Content.Load<Texture2D>("assets/pacman-right-2"),
                Content.Load<Texture2D>("assets/pacman-right-3")
            };
            _frames["Left"]  = new Texture2D[] {
                Content.Load<Texture2D>("assets/pacman-left-1"),
                Content.Load<Texture2D>("assets/pacman-left-2"),
                Content.Load<Texture2D>("assets/pacman-left-3")
            };
            _frames["Up"]    = new Texture2D[] {
                Content.Load<Texture2D>("assets/pacman-up-1"),
                Content.Load<Texture2D>("assets/pacman-up-2"),
                Content.Load<Texture2D>("assets/pacman-up-3")
            };
            _frames["Down"]  = new Texture2D[] {
                Content.Load<Texture2D>("assets/pacman-down-1"),
                Content.Load<Texture2D>("assets/pacman-down-2"),
                Content.Load<Texture2D>("assets/pacman-down-3")
            };

            if (Position == Vector2.Zero)
                Position = new Vector2(20, 315);
        }

        public void Update(GameTime gameTime, MazeMap maze)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 inputDir = Vector2.Zero;

            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Up)) inputDir = new Vector2(0, -1);
            else if (ks.IsKeyDown(Keys.Down)) inputDir = new Vector2(0, 1);
            else if (ks.IsKeyDown(Keys.Left)) inputDir = new Vector2(-1, 0);
            else if (ks.IsKeyDown(Keys.Right)) inputDir = new Vector2(1, 0);

            // only change direction if the new direction is not blocked
            if (inputDir != Vector2.Zero)
            {
                Vector2 nextInputPos = Position + inputDir * Speed * dt;
                if (!CollidesWithWall(nextInputPos, maze))
                {
                    inputDir.Normalize();
                    Position += inputDir * Speed * dt;
                    CurrentDirection = inputDir switch
                    {
                        var v when v == new Vector2(0, -1) => Direction.Up,
                        var v when v == new Vector2(0, 1) => Direction.Down,
                        var v when v == new Vector2(-1, 0) => Direction.Left,
                        var v when v == new Vector2(1, 0) => Direction.Right,
                        _ => CurrentDirection
                    };
                    IsMoving = true;
                }
            }
            else
            {
                // continue moving in current direction if path is clear
                Vector2 dir = CurrentDirection switch
                {
                    Direction.Up => new Vector2(0, -1),
                    Direction.Down => new Vector2(0, 1),
                    Direction.Left => new Vector2(-1, 0),
                    Direction.Right => new Vector2(1, 0),
                    _ => Vector2.Zero
                };

                Vector2 next = Position + dir * Speed * dt;
                if (!CollidesWithWall(next, maze))
                {
                    Position = next;
                    IsMoving = true;
                }
                else
                {
                    IsMoving = false;
                }
            }

            Animate(gameTime);
        }


        private bool CollidesWithWall(Vector2 worldPos, MazeMap maze)
        {
            float half = Size / 2f;
            var checks = new Vector2[]
            {
                new Vector2(worldPos.X + 1, worldPos.Y + 1),
                new Vector2(worldPos.X + Size - 1, worldPos.Y + 1),
                new Vector2(worldPos.X + 1, worldPos.Y + Size - 1),
                new Vector2(worldPos.X + Size - 1, worldPos.Y + Size - 1),
                new Vector2(worldPos.X + half, worldPos.Y + half)
            };

            foreach (var p in checks)
                if (maze.IsWallAtWorld(p))
                    return true;

            return false;
        }

        private void Animate(GameTime gameTime)
        {
            if (!IsMoving)
            {
                _frameIndex = 1;
                _animTimer = 0;
                return;
            }

            _animTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_animTimer >= _animInterval)
            {
                _animTimer -= _animInterval;
                _frameIndex = (_frameIndex + 1) % _frames[CurrentDirection.ToString()].Length;

                if (_frameIndex == 0)
                    PlayEatDot();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            var key = CurrentDirection.ToString();
            Texture2D tex = _frames[key][_frameIndex];

            sb.Draw(
                tex,
                Position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                pacmanScale,    
                SpriteEffects.None,
                0f
            );
        }

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Size, Size);

        public void PlayEatDot()
        {
            if (_sound != null && !_sound.IsMuted && _sound.Chomp != null)
                _sound.Chomp.Play();
        }

        public void PlayEatFruit()
        {
            if (_sound != null && !_sound.IsMuted && _sound.EatFruit != null)
                _sound.EatFruit.Play();
        }

        public void PlayDeath()
        {
            if (_sound != null && !_sound.IsMuted && _sound.Death != null)
                _sound.Death.Play();
        }

        public void PlayEatGhost()
        {
            if (_sound != null && !_sound.IsMuted && _sound.EatGhost != null)
                _sound.EatGhost.Play();
        }


    }
}

