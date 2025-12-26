using System.Diagnostics;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Extensions.DebugView;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.MonoGame.DebugView;
using LyiarOwl.Camera;
using LyiarOwl.Demo.Snake;
using LyiarOwl.Demo.Snake.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Screens;

namespace LyiarOwl.Screns;

public class InGameScreen : GameScreen
{
    private Game1 _game => (Game1)base.Game;
    private OrthographicCamera2D _camera;
    private FitViewport _viewport;
    private BasicEffect _basicEffect;
    private SpriteBatch _spriteBatch;
    
    /* Snake */
    private Vector2 _segmentSize = Vector2.One * 32f;
    private Body _snakeHeadBody;


    public InGameScreen(Game1 game) : base(game)
    {
    }
    public override void LoadContent()
    {
        _basicEffect = OrthographicCamera2D.CreateDefaultEffect(GraphicsDevice);
        _viewport = new FitViewport(_game.Window, GraphicsDevice, 1280, 720);
        _camera = new OrthographicCamera2D(GraphicsDevice, _basicEffect, _viewport)
        {
            Ppm = 32f
        };
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        World world = WorldManager.Instance.World;
        _snakeHeadBody = BodyFactory.CreateRectangle(
            world, 1f, 1f, 1f, default, 0f, BodyType.Kinematic, new { tag = "Player" }
        );

        WorldManager.Instance.FixedUpdate += FixedUpdate;
    }
 
    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _camera.Update();

        LateUpdate();
    }

    public void FixedUpdate()
    {
        
    }

    public void LateUpdate()
    {
        
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Viewport = _viewport.Viewport;
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(effect: _basicEffect, samplerState: SamplerState.PointClamp);
        _spriteBatch.FillRectangle(
            _snakeHeadBody.Position * _camera.Ppm - _segmentSize * 0.5f,
            _segmentSize,
            Color.White
        );
        _spriteBatch.End();

        WorldManager.Instance.Draw(_camera);
    }

    public override void Dispose()
    {
        WorldManager.Instance.FixedUpdate -= FixedUpdate;
        base.Dispose();
    }
}
