using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using LyiarOwl.Demo.Snake.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LyiarOwl.Demo.Snake.Characters;

public class Wall
{
    private Color _color = new Color(64, 64, 64);

    public Body Body;
    public Vector2 Size;
    public Vector2 Position
    {
        get => Body.Position * WorldManager.PPM;
        set => Body.Position = value / WorldManager.PPM;
    }
    public Vector2 PhysicalPosition
    {
        get => Body.Position;
        set => Body.Position = value;
    }
    public Wall(float physicalX, float physicalY, float physicalWidth, float physicalHeight)
    {
        Size = new Vector2(physicalWidth * WorldManager.PPM, physicalHeight * WorldManager.PPM);
        Body = BodyFactory.CreateRectangle(WorldManager.Instance.World, physicalWidth, physicalHeight, 1f,
        new Vector2(physicalX, physicalY), 0f, BodyType.Static);
        Body.UserData = new UserData("Obstacle");
    }
    public void Draw(SpriteBatch batch)
    {
        batch.FillRectangle(
            Position - Size * 0.5f,
            Size,
            _color
        );
    }
}