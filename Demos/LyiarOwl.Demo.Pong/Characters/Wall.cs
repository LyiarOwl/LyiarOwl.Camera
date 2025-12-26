using Genbox.VelcroPhysics.Factories;
using Microsoft.Xna.Framework;

namespace LyiarOwl.Demo.Pong.Characters
{
    public class Wall : PhysicsEntity2D
    {
        /// <param name="x">Position in pixels.</param>
        /// <param name="y">Position in pixels.</param>
        /// <param name="width">Size in pixels.</param>
        /// <param name="height">Size in pixels.</param>
        public Wall(float x, float y, float width, float height, string tag = null) : base(width, height)
        {
            Body = BodyFactory.CreateRectangle(
                World,
                PhysicalSize.X, PhysicalSize.Y,
                1f,
                default
            );
            Position = new Vector2(x, y);
            Body.UserData = tag;
        }
    }
}