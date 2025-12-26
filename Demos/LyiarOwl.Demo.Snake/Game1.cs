using Microsoft.Xna.Framework;
using System;
using LyiarOwl.Demo.Snake.Managers;
using LyiarOwl.Demo.Snake.Characters;
using LyiarOwl.Camera;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LyiarOwl.Demo.Snake;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private InputManager _inputManager;
    private WorldManager _worldManager;
    private SpriteBatch _spriteBatch;

    /* Camera */
    private OrthographicCamera2D _camera;
    private BasicEffect _basicEffect;
    private FitViewport _viewport;

    /* Snake */
    private SnakeHead _snakeHead;

    /* Walls */
    private Wall[] _walls;

    /* Fruit */
    private Fruit _fruit;
    private RectangleF _fruitArea;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d); /* 1sec / 60 FPS */
        IsFixedTimeStep = true; /* limit the maximum 'fps' (maximum 'fps' is defined in TargetElapsedTime) */
        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        _inputManager = new InputManager();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        /* camera initialization */
        _basicEffect = OrthographicCamera2D.CreateDefaultEffect(GraphicsDevice);
        _viewport = new FitViewport(Window, GraphicsDevice, 1280, 720);
        _camera = new OrthographicCamera2D(GraphicsDevice, _basicEffect, _viewport)
        {
            Ppm = Constants.PPM,
            Zoom = 0.75f
        };
        _camera.Update(); /* apply PPM and Zoom changes */

        /* physics initialization */
        _worldManager = new WorldManager(GraphicsDevice, Content);
        _worldManager.FixedUpdate += FixedUpdate;

        /* snake initialization */
        _snakeHead = new SnakeHead();

        /* level walls */
        var world = _worldManager.World;
        _walls = [
            /* since size and position must be in pixels and the physics engine works in 
             * meters you need to convert from meters to pixels: 24 meters x 32px = 768px */
            new Wall(-24f * Constants.PPM, 0f, Constants.PPM, 25f * Constants.PPM),
            new Wall(24f * Constants.PPM, 0f, Constants.PPM, 25f * Constants.PPM),
            new Wall(0f, 12f * Constants.PPM, 48f * Constants.PPM, Constants.PPM),
            new Wall(0f, -12f * Constants.PPM, 48f * Constants.PPM, Constants.PPM),
        ];

        /* fruit */
        Vector2 fruitAreaSize = new Vector2(46f * Constants.PPM, 22f * Constants.PPM); /* size in pixels */
        _fruitArea = new RectangleF(-fruitAreaSize / 2f, fruitAreaSize);
        _fruit = new Fruit(_fruitArea);
    }

    protected override void Update(GameTime gameTime)
    {
        Time.DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _inputManager.Update();

        /* general update */
        _worldManager.Update();
        _camera.Update();

        base.Update(gameTime);
    }


    private void FixedUpdate()
    {
        _snakeHead.FixedUpdate();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Viewport = _viewport.Viewport;
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _basicEffect);

        _fruit.Draw(_spriteBatch);
        _snakeHead.Draw(_spriteBatch);

        foreach (var wall in _walls)
            wall.Draw(_spriteBatch);
        _spriteBatch.End();

        // _worldManager.Draw(_camera); /* uncomment to render the physics engine debug */

        base.Draw(gameTime);
    }
}
