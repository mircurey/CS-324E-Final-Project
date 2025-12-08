using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Final_Project_Pacman
{
    public class Pacman
    {
        private Dictionary<string, Texture2D[]> _frames;
        private Dictionary<string, Texture2D[]> _dyingFrames;
        private float _animTimer;
        private int _frameIndex;
        private float _animInterval = 100f; // ms per frame
        private SoundManager _sound;

        public Vector2 Position;
        public float Speed = 100f; // pixels/second
        public bool IsMoving { get; private set; }
        public bool IsDying { get; private set; }

        public enum Direction { Up, Down, Left, Right }
        public Direction CurrentDirection = Direction.Left;

        public enum AnimationState { Normal, Dying, Dead }
        public AnimationState CurrentAnimationState { get; private set; } = AnimationState.Normal;

        // collision size (approx)
        public int Size = 16;

        private float pacmanScale = 1.8f;

        public Pacman(SoundManager sound)
        {
            _frames = new Dictionary<string, Texture2D[]>();
            _dyingFrames = new Dictionary<string, Texture2D[]>();
            _sound = sound;
            IsDying = false;
            CurrentAnimationState = AnimationState.Normal;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            // normal animation (3 frames each direction)
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

            // dying animations (5 frames per direction) - filenames must match
            _dyingFrames["Right"] = new Texture2D[] {
                Content.Load<Texture2D>("assets/pacman-dying-right-1"),
                Content.Load<Texture2D>("assets/pacman-dying-right-2"),
                Content.Load<Texture2D>("assets/pacman-dying-right-3"),
                Content.Load<Texture2D>("assets/pacman-dying-right-4"),
                Content.Load<Texture2D>("assets/pacman-dying-right-5")
            };
            _dyingFrames["Left"] = new Texture2D[] {
                Content.Load<Texture2D>("assets/pacman-dying-left-1"),
                Content.Load<Texture2D>("assets/pacman-dying-left-2"),
                Content.Load<Texture2D>("assets/pacman-dying-left-3"),
                Content.Load<Texture2D>("assets/pacman-dying-left-4"),
                Content.Load<Texture2D>("assets/pacman-dying-left-5")
            };
            _dyingFrames["Up"] = new Texture2D[] {
                Content.Load<Texture2D>("assets/pacman-dying-up-1"),
                Content.Load<Texture2D>("assets/pacman-dying-up-2"),
                Content.Load<Texture2D>("assets/pacman-dying-up-3"),
                Content.Load<Texture2D>("assets/pacman-dying-up-4"),
                Content.Load<Texture2D>("assets/pacman-dying-up-5")
            };
            _dyingFrames["Down"] = new Texture2D[] {
                Content.Load<Texture2D>("assets/pacman-dying-down-1"),
                Content.Load<Texture2D>("assets/pacman-dying-down-2"),
                Content.Load<Texture2D>("assets/pacman-dying-down-3"),
                Content.Load<Texture2D>("assets/pacman-dying-down-4"),
                Content.Load<Texture2D>("assets/pacman-dying-down-5")
            };

            // initial position
            if (Position == Vector2.Zero)
                Position = new Vector2(20, 315);
        }

        public void ResetState()
        {
            IsDying = false;
            CurrentAnimationState = AnimationState.Normal;
            _frameIndex = 1;
            _animTimer = 0;
        }

        // Update now accepts MazeMap and DotManager to enforce rail movement
        public void Update(GameTime gameTime, MazeMap maze, DotManager dots)
        {
            if (CurrentAnimationState == AnimationState.Dead)
                return;

            if (CurrentAnimationState == AnimationState.Dying)
            {
                // animate dying to completion
                _animTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                float dieInterval = 120f;
                if (_animTimer >= dieInterval)
                {
                    _animTimer -= dieInterval;
                    _frameIndex++;
                    var dyingKey = CurrentDirection.ToString();
                    if (_frameIndex >= _dyingFrames[dyingKey].Length)
                    {
                        CurrentAnimationState = AnimationState.Dead;
                        IsDying = false;
                    }
                }
                return;
            }

            // Normal movement (grid-aligned using dot rails)
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 inputDir = Vector2.Zero;
            Direction? desiredDir = null;

            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Up)) { inputDir = new Vector2(0, -1); desiredDir = Direction.Up;  }
            else if (ks.IsKeyDown(Keys.Down)) { inputDir = new Vector2(0, 1); desiredDir = Direction.Down; }
            else if (ks.IsKeyDown(Keys.Left)) { inputDir = new Vector2(-1, 0); desiredDir = Direction.Left; }
            else if (ks.IsKeyDown(Keys.Right)) { inputDir = new Vector2(1, 0); desiredDir = Direction.Right; }

            // grid-aware movement: snap to nearest dot tile center and move only along tiles that exist in dot mask
            if (desiredDir != null)
            {
                // check if next tile along desired direction is a rail
                if (dots.CanMoveFromWorld(Position, desiredDir.Value, Speed, dt))
                {
                    // move in desired dir
                    Vector2 dirVec = DirectionToVector(desiredDir.Value);
                    Vector2 next = Position + dirVec * Speed * dt;
                    if (!CollidesWithWall(next, maze))
                    {
                        Position = next;
                        CurrentDirection = desiredDir.Value;
                        IsMoving = true;
                    }
                }
            }
            else
            {
                // continue moving in current direction if possible
                Vector2 dir = DirectionToVector(CurrentDirection);
                Vector2 next = Position + dir * Speed * dt;
                if (!CollidesWithWall(next, maze) && dots.IsRailAtWorld(next))
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

        public void StartDying()
        {
            if (CurrentAnimationState != AnimationState.Normal) return;
            CurrentAnimationState = AnimationState.Dying;
            IsDying = true;
            _frameIndex = 0;
            _animTimer = 0;
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
                _frameIndex = 1; // closed mouth frame
                _animTimer = 0;
                return;
            }

            _animTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_animTimer >= _animInterval)
            {
                _animTimer -= _animInterval;
                _frameIndex = (_frameIndex + 1) % _frames[CurrentDirection.ToString()].Length;

                // only play chomp if sfx enabled via SoundManager
                if (_frameIndex == 0 && _sound != null && !_sound.SfxMuted)
                    _sound.Chomp?.Play();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (CurrentAnimationState == AnimationState.Dying)
            {
                var dyingKey = CurrentDirection.ToString();
                Texture2D dt = _dyingFrames[dyingKey][_frameIndex];
                sb.Draw(dt, Position, null, Color.White, 0f, Vector2.Zero, pacmanScale, SpriteEffects.None, 0f);
                return;
            }

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

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, (int)(Size * pacmanScale), (int)(Size * pacmanScale));

        public void PlayEatDot()
        {
            if (_sound != null && !_sound.SfxMuted && _sound.Chomp != null)
                _sound.Chomp.Play();
        }

        public void PlayEatFruit()
        {
            if (_sound != null && !_sound.SfxMuted && _sound.EatFruit != null)
                _sound.EatFruit.Play();
        }

        public void PlayDeath()
        {
            if (_sound != null && !_sound.SfxMuted && _sound.Death != null)
                _sound.Death.Play();
        }

        public void PlayEatGhost()
        {
            if (_sound != null && !_sound.SfxMuted && _sound.EatGhost != null)
                _sound.EatGhost.Play();
        }
    }
}
