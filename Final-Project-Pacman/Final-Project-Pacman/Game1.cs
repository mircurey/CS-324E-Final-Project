using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Final_Project_Pacman;

public enum GameState
{
    MainMenu,
    Playing,
    InfoScreen
}

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    private SpriteFont _font;
    private Texture2D _mazeTexture;
    private Texture2D _blinkyTexture, _pinkyTexture, _inkyTexture, _clydeTexture;
    
    private List<Ghost> _ghosts;
        
    private GameState _currentState;
    private KeyboardState _previousKState;
    private HighScore highScore;
    private int currentScore = 0;

    private MazeMap _mazeMap;
    private Pacman _pacman;
    private SoundManager _sound;
    private DotManager _dotManager;
    private GameTimer _timer;
    private Texture2D _muteTex;
    private Texture2D _volumeTex;
    private Rectangle _soundButtonRect;
    private MouseState _prevMouse;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        _graphics.PreferredBackBufferWidth = 560;
        _graphics.PreferredBackBufferHeight = 620;
    }

    protected override void Initialize()
    {
        _currentState = GameState.MainMenu;
        _ghosts = new List<Ghost>();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _font = Content.Load<SpriteFont>("fonts/PressStart2P");
        _mazeTexture = Content.Load<Texture2D>("assets/pacmaze");
            
        _blinkyTexture = Content.Load<Texture2D>("assets/blinky");
        _pinkyTexture = Content.Load<Texture2D>("assets/pinky");
        _inkyTexture = Content.Load<Texture2D>("assets/inky");
        _clydeTexture = Content.Load<Texture2D>("assets/clyde");

        _muteTex   = Content.Load<Texture2D>("assets/MuteVolume");
        _volumeTex = Content.Load<Texture2D>("assets/VolumeOn");
        int btnX = _graphics.PreferredBackBufferWidth - _muteTex.Width - 10;
        int btnY = 40;  
        _soundButtonRect = new Rectangle(btnX, btnY, _muteTex.Width, _muteTex.Height);

        highScore = new HighScore();

        _sound = new SoundManager();
        _sound.LoadContent(Content);
        _sound.SetMuted(false);

        Rectangle mazeDrawRect = new Rectangle(0, 50, 560, 570);
        _mazeMap = new MazeMap(_mazeTexture, mazeDrawRect);

        _pacman = new Pacman(_sound);
        _pacman.LoadContent(Content);

        _dotManager = new DotManager();
        _dotManager.LoadContent(Content, GraphicsDevice);
        _dotManager.GenerateDots(_mazeMap);

        _sound.PlayBeginning();
        _timer = new GameTimer(60);

    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState currentKState = Keyboard.GetState();

        MouseState mouse = Mouse.GetState();
        if (_currentState == GameState.Playing)   
        {
            if (mouse.LeftButton == ButtonState.Pressed &&
                _prevMouse.LeftButton == ButtonState.Released &&
                _soundButtonRect.Contains(mouse.Position))
            {
                bool newMuted = !_sound.IsMuted;
                _sound.SetMuted(newMuted);
            }
        }
        _prevMouse = mouse;

        if (currentKState.IsKeyDown(Keys.M) && _previousKState.IsKeyUp(Keys.M))
        {
            _currentState = GameState.MainMenu;
            currentScore = 0;       // reset score
            _sound.PlayBeginning();     // restart intro music
        }

        // ESC: Quit from menu, Back to menu from game
        if (currentKState.IsKeyDown(Keys.Escape) && _previousKState.IsKeyUp(Keys.Escape))
        {
            if (_currentState == GameState.MainMenu)
                Exit();
            else
            {
                _currentState = GameState.MainMenu;
                _sound.PlayBeginning();
            }
        }

        switch (_currentState)
        {
            case GameState.MainMenu:
                
                if (currentKState.IsKeyDown(Keys.Space) && _previousKState.IsKeyUp(Keys.Space))
                {
                    _currentState = GameState.Playing;
                    currentScore = 0;        
                    _timer.Start(); 
                }

                if (currentKState.IsKeyDown(Keys.I) && _previousKState.IsKeyUp(Keys.I))
                {
                    _currentState = GameState.InfoScreen;
                }
                break;

            case GameState.Playing:
                _pacman.Update(gameTime, _mazeMap);
                int gained = _dotManager.Update(_pacman);
                if (gained > 0)
                    currentScore += gained;
                _timer.Update(gameTime);   // 🔵 UPDATE TIMER

                if (!_timer.IsRunning && _timer.RemainingSeconds == 0)
                {
                    highScore.Save(currentScore);
                    _sound.PlayBeginning();
                    _currentState = GameState.InfoScreen;
                }
                break;
        }

        _previousKState = currentKState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        if (_currentState == GameState.MainMenu)
        {
            DrawTextCentered("PAC-MAN PROJECT", 100, Color.Yellow);
            DrawTextCentered("Press SPACE to Start", 300, Color.White);
            DrawTextCentered("Press 'I' for Info & Score", 350, Color.Cyan);
        }
        else if (_currentState == GameState.InfoScreen)
        {
            DrawTextCentered("HOW TO PLAY", 50, Color.Yellow);
            DrawTextCentered("[will add game controls,", 150, Color.White);
            DrawTextCentered("details, and objective]", 175, Color.White);
            DrawTextCentered("HIGH SCORE: " + highScore.Value.ToString("D4"), 300, Color.Red);
        }
        else if (_currentState == GameState.Playing)
        {
            _spriteBatch.DrawString(_font, "SCORE: " + currentScore.ToString(),
                new Vector2(10, 10), Color.White);

            _spriteBatch.DrawString(_font, "HIGH: " + highScore.Value.ToString(),
                new Vector2(370, 10), Color.White);
            string timeText = "TIME: " + _timer.RemainingSeconds.ToString();
            _spriteBatch.DrawString(_font, timeText, new Vector2(210, 10), Color.White);


            _spriteBatch.Draw(_mazeTexture, new Rectangle(0, 50, 560, 570), Color.White);
            
            _dotManager.Draw(_spriteBatch);
            _mazeMap.DrawDebug(_spriteBatch);

            _pacman.Draw(_spriteBatch);

            foreach (var g in _ghosts)
                g.Draw(_spriteBatch);
            if (_sound != null)
            {
                Texture2D icon = _sound.IsMuted ? _muteTex : _volumeTex;

                float scale = 0.10f;  

                _spriteBatch.Draw(
                    icon,
                    new Vector2(_soundButtonRect.X + 590, _soundButtonRect.Y - 23), 
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    scale,       
                    SpriteEffects.None,
                    0f
                );

            }

        }
        _spriteBatch.End();
        base.Draw(gameTime);
    }
    
    private void DrawTextCentered(string text, int y, Color color)
    {
        if (_font == null) return;
        Vector2 textSize = _font.MeasureString(text);
        _spriteBatch.DrawString(_font, text,
            new Vector2((_graphics.PreferredBackBufferWidth - textSize.X) / 2, y), color);
    }
}
