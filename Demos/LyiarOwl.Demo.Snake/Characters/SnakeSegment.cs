using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using LyiarOwl.Demo.Snake.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LyiarOwl.Demo.Snake.Characters;

public class SnakeSegment : PhysicalEntity2D
{
    public SnakeSegment()
    {
        Size = new Vector2(20f, 20f);
        Body = BodyFactory.CreateRectangle(WorldManager.Instance.World, 0.75f, 0.75f, 
            1f, default, 0f, BodyType.Dynamic);
        Body.UserData = Tags.Obstacle;
    }
    public override void Draw(SpriteBatch batch)
    {
        batch.FillRectangle(
            Position - Size * 0.5f,
            Size,
            Color.White
        );
    }
}