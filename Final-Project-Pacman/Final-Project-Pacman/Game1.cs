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
        // TODO: Add your initialization logic here
        _currentState = GameState.MainMenu;
        _ghosts = new List<Ghost>();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _font = Content.Load<SpriteFont>("fonts/PressStart2P");
        _mazeTexture = Content.Load<Texture2D>("assets/pacmaze"); 
            
        _blinkyTexture = Content.Load<Texture2D>("assets/blinky");
        _pinkyTexture = Content.Load<Texture2D>("assets/pinky");
        _inkyTexture = Content.Load<Texture2D>("assets/inky");
        _clydeTexture = Content.Load<Texture2D>("assets/clyde");
        highScore = new HighScore();

        _sound = new SoundManager();
        _sound.LoadContent(Content);

        // the rectangle you use when drawing the maze (must match Draw call)
        Rectangle mazeDrawRect = new Rectangle(0, 50, 560, 570);
        _mazeMap = new MazeMap(_mazeTexture, mazeDrawRect);
        //_mazeMap.InitDebug(GraphicsDevice);


        _pacman = new Pacman(_sound);
        _pacman.LoadContent(Content);

        _sound.Beginning?.Play();

    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState currentKState = Keyboard.GetState();

        // check for single press of escape key
        if (currentKState.IsKeyDown(Keys.Escape) && _previousKState.IsKeyUp(Keys.Escape))
        {
            // if currently in main menu, exit application
            if (_currentState == GameState.MainMenu)
            {
                Exit();
            }
            // if in other state, return to main menu
            else
            {
                _currentState = GameState.MainMenu;
            }
        }
        
        switch (_currentState)
        {
            case GameState.MainMenu:
                // switch state if key is pressed (single press)
                if (currentKState.IsKeyDown(Keys.Space) && _previousKState.IsKeyUp(Keys.Space))
                {
                    _currentState = GameState.Playing;
                }
                if (currentKState.IsKeyDown(Keys.I) && _previousKState.IsKeyUp(Keys.I))
                {
                    _currentState = GameState.InfoScreen;
                }
                
                break;

            case GameState.Playing:
                // game updates to be added in future
                _pacman.Update(gameTime, _mazeMap);
                break;
        }
        _previousKState = currentKState; 

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here
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

            // temporary high score in place of real high score to be added later
            DrawTextCentered("HIGH SCORE: " + highScore.Value.ToString("D4"), 300, Color.Red);

        }
        else if (_currentState == GameState.Playing)
        {
            // GUI header, add update functionality
            _spriteBatch.DrawString(_font, "SCORE: " + currentScore.ToString(), 
                new Vector2(10, 10), Color.White);

            _spriteBatch.DrawString(_font, "HIGH: " + highScore.Value.ToString(), 
                new Vector2(400, 10), Color.White);

            // draw maze and make room at top for score
            _spriteBatch.Draw(_mazeTexture, new Rectangle(0, 50, 560, 570), Color.White);
            
            // Draw maze (scaled)
            _spriteBatch.Draw(_mazeTexture, new Rectangle(0, 50, 560, 570), Color.White);

            // DEBUG: draw wall mask (unscaled)
            _mazeMap.DrawDebug(_spriteBatch);

            _pacman.Draw(_spriteBatch);

            foreach (var g in _ghosts)
                g.Draw(_spriteBatch);
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
    
    // helper function to draw text centered
    private void DrawTextCentered(string text, int y, Color color)
    {
        if (_font == null) return;
        Vector2 textSize = _font.MeasureString(text);
        _spriteBatch.DrawString(_font, text, new Vector2((_graphics.PreferredBackBufferWidth - textSize.X) / 2, y), color);
    }
}