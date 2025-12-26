using System;
using Genbox.VelcroPhysics.Collision.ContactSystem;
using Genbox.VelcroPhysics.Dynamics;
using LyiarOwl.Camera;
using LyiarOwl.Demo.Pong.Characters;
using LyiarOwl.Demo.Pong.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LyiarOwl.Demo.Pong;

public class Game1 : Game
{
    /* general */
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private WorldManager _worldManager;
    private InputsManager _inputsManager;

    /* camera */
    private FitViewport _viewport;
    private BasicEffect _basicEffect;
    private OrthographicCamera2D _camera;

    /* walls */
    private Wall _topWall;
    private Wall _bottomWall;
    private Wall _leftWall;
    private Wall _rightWall;

    /* paddles */
    private Paddle _playerPaddle;
    private Paddle _cpuPaddle;

    /* background */
    private RenderTarget2D _dashedLine;
    
    /* ball */
    private Ball _ball;

    /* UI */
    private int _cpuScore;
    private int _playerScore;
    private SpriteFont _gameFont;
    private Vector2 _playerScoreStringSize;
    private Vector2 _playerScoreStringOrigin;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(Constants.TARGET_ELAPSED_TIME);
        Time.DeltaTime = (float)Constants.TARGET_ELAPSED_TIME;

        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        _inputsManager = new InputsManager();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _worldManager = new WorldManager(GraphicsDevice, Content);
        _worldManager.FixedUpdate += FixedUpdate;

        /* camera */
        _basicEffect = OrthographicCamera2D.CreateDefaultEffect(GraphicsDevice);
        _viewport = new FitViewport(Window, GraphicsDevice, 1280, 720);
        _camera = new OrthographicCamera2D(GraphicsDevice, _basicEffect, _viewport)
        {
            Ppm = Constants.PPM,
            Zoom = 0.725f
        };
        _camera.Update(); 
        /* this call is necessary because we are changing Zoom and PPM and PreRenderDashedDivider needs
         updated data from the camera */
        PreRenderDashedDivider();

        /* walls */
        _topWall = new Wall(0f, -550f, 2000f, 100f, Tags.TopWall);
        _bottomWall = new Wall(0f, 550f, 2000f, 100f, Tags.BottomWall);
        _leftWall = new Wall(-950f, 0f, 100f, 1200f, Tags.LeftWall);
        _rightWall = new Wall(950f, 0f, 100f, 1200f, Tags.RightWall);

        /* ball */
        _ball = new Ball();
        _ball.Body.OnCollision += HandleScore;

        /* paddles */
        _playerPaddle = new PlayerPaddle(-800f, 0f);
        _cpuPaddle = new CPUPaddle(800f, 0f, _ball);

        /* UI */
        _gameFont = Content.Load<SpriteFont>("GameFont");
        AdjustPlayerScoreText();
    }

    protected override void Update(GameTime gameTime)
    {
        Time.DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _worldManager.Update();
        _inputsManager.Update();
        _camera.Update();
        _playerPaddle.Update();
        _cpuPaddle.Update();

        base.Update(gameTime);
    }

    private void FixedUpdate()
    {
        _playerPaddle.FixedUpdate();
        _cpuPaddle.FixedUpdate();
        _ball.FixedUpdate();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Viewport = _viewport.Viewport;
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _basicEffect);
        _spriteBatch.Draw(_dashedLine, new Vector2(-2f, _camera.Top - 13f), Color.White);
        _playerPaddle.Draw(_spriteBatch);
        _cpuPaddle.Draw(_spriteBatch);
        _ball.Draw(_spriteBatch);
        DrawUI();
        _spriteBatch.End();
        
        _worldManager.Draw(_camera);

        base.Draw(gameTime);
    }
    private void DrawUI()
    {
        /* player score */
        _spriteBatch.DrawString(
            _gameFont, 
            _playerScore.ToString(),
            new Vector2(-50f, _camera.Top + 50f),
            Color.White,
            0f,
            _playerScoreStringOrigin,
            1f,
            SpriteEffects.None,
            0f
        );
        /* cpu score */
        _spriteBatch.DrawString(
            _gameFont,
            _cpuScore.ToString(),
            new Vector2(65f, _camera.Top + 50f),
            Color.White
        );
    }
    private void PreRenderDashedDivider(float thickness = 8f)
    {
        const float lineHeight = 30f;
        const float gap = 60f;
        int lines = (int)(_camera.Height / lineHeight);
        _dashedLine = new RenderTarget2D(GraphicsDevice, 4, (int)lineHeight * lines + (int)gap * lines);
        
        GraphicsDevice.SetRenderTarget(_dashedLine);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        for (int i = 0; i < lines; i++)
        {
            float y = (lineHeight + gap) * i;
            _spriteBatch.DrawLine(0f, y, 0f, y + lineHeight, Color.White, thickness);
        }
        _spriteBatch.End();
        GraphicsDevice.SetRenderTarget(null);
    }
    private void HandleScore(Fixture a, Fixture other, Contact contact)
    {
        if (other.Body != null)
        {
            if (other.Body.UserData != null && other.Body.UserData is string tag)
            {
                if (tag == Tags.LeftWall)
                {
                    _cpuScore++;
                    _ball.ResetState();
                    _cpuPaddle.ResetState();
                }
                if (tag == Tags.RightWall)
                {
                    _playerScore++;
                    _ball.ResetState();
                    _playerPaddle.ResetState();
                    AdjustPlayerScoreText();
                }
            }
        }
    }
    private void AdjustPlayerScoreText()
    {
        _playerScoreStringSize = _gameFont.MeasureString(_playerScore.ToString());
        _playerScoreStringOrigin.X = _playerScoreStringSize.X;
    }
}
