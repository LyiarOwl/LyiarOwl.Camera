using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using LyiarOwl.Demo.Snake.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LyiarOwl.Demo.Snake.Characters;

public class Wall : PhysicalEntity2D
{
    private readonly Color _color = new Color(64, 64, 64);

    /// <param name="x">Position in pixels.</param>
    /// <param name="y">Position in pixels.</param>
    /// <param name="width">Size in pixels.</param>
    /// <param name="height">Size in pixels.</param>
    public Wall(float x, float y, float width, float height)
    {
        Size = new Vector2(width, height);
        Body = BodyFactory.CreateRectangle(
            WorldManager.Instance.World,
            PhysicalSize.X, PhysicalSize.Y,
            1f, default, 0f, BodyType.Static
        );
        Position = new Vector2(x, y);
        Body.UserData = Tags.Obstacle;
    }
    public override void Draw(SpriteBatch batch)
    {
        batch.FillRectangle(
            Position - Size * 0.5f,
            Size,
            _color
        );
    }
}