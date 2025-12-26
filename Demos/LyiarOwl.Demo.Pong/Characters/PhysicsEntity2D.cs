using Genbox.VelcroPhysics.Dynamics;
using LyiarOwl.Demo.Pong.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LyiarOwl.Demo.Pong.Characters
{
    public class PhysicsEntity2D
    {
        protected World World => WorldManager.Instance.World;
        public Body Body;
        /// <summary>
        /// Size in pixels;
        /// </summary>
        public Vector2 Size;
        /// <summary>
        /// Size in meters.
        /// </summary>
        public Vector2 PhysicalSize
        {
            get => Size / Constants.PPM;
            set => Size = value * Constants.PPM;
        }
        /// <summary>
        /// Position in pixels.
        /// </summary>
        public Vector2 Position
        {
            get => Body.Position * Constants.PPM;
            set => Body.Position = value / Constants.PPM;
        }
        /// <summary>
        /// Position in meters.
        /// </summary>
        public Vector2 PhysicalPosition
        {
            get => Body.Position;
            set => Body.Position = value;
        }
        /// <param name="width">Size in pixels.</param>
        /// <param name="height">Size in pixels.</param>
        public PhysicsEntity2D(float width, float height)
        {
            Size = new Vector2(width, height);
        }

        public virtual void Update()
        {
        }
        public virtual void FixedUpdate()
        {
        }
        public virtual void Draw(SpriteBatch batch) {}
    }
}