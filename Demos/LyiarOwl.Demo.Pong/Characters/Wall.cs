using Genbox.VelcroPhysics.Factories;
using LyiarOwl.Demo.Pong.Managers;
using Microsoft.Xna.Framework;

namespace LyiarOwl.Demo.Pong.Characters
{
    public class Wall : PhysicsEntity2D
    {
        public static string Tag;
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
            Tag = tag;
        }
    }
}