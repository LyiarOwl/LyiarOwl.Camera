using Genbox.VelcroPhysics.Dynamics;
using LyiarOwl.Demo.Snake.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LyiarOwl.Demo.Snake.Characters;

public class PhysicalEntity2D
{
    public Body Body;
    public Vector2 Size;
    public Vector2 PhysicalSize
    {
        get => Size / WorldManager.PPM;
        set => Size = value * WorldManager.PPM;
    }
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
    public virtual void Update()
    {
        
    }
    public virtual void Draw(SpriteBatch batch)
    {
        
    }
}