using System;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Extensions.DebugView;
using Genbox.VelcroPhysics.MonoGame.DebugView;
using LyiarOwl.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LyiarOwl.Demo.Pong.Managers
{
    public class WorldManager
    {
        public static WorldManager Instance { get; private set; }
        private float _accumulator = 0f;

        public World World { get; private set; }
        public DebugView DebugView { get; private set; }
        public event Action FixedUpdate;
        public WorldManager(GraphicsDevice graphicsDevice, ContentManager content)
        {
            Instance = this;
            World = new World(Vector2.Zero);
            DebugView = new DebugView(World);
            DebugView.AppendFlags(DebugViewFlags.Shape);
            DebugView.LoadContent(graphicsDevice, content);
        }
        public void Update()
        {
            float frameTime = MathF.Min(Time.DeltaTime, 0.25f);
            _accumulator += frameTime;
            while (_accumulator >= Constants.TIME_STEP)
            {
                World.Step(Constants.TIME_STEP);
                Time.FixedDeltaTime = Constants.TIME_STEP;
                FixedUpdate?.Invoke();
                _accumulator -= Constants.TIME_STEP;
            }
        }
        public void Draw(OrthographicCamera2D camera)
        {
            DebugView.RenderDebugData(camera.Projection, camera.PhysicsDebugView);
        }
    }
}