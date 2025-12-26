using Genbox.VelcroPhysics.Dynamics;
using LyiarOwl.Demo.Snake.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LyiarOwl.Demo.Snake.Characters;

public class PhysicalEntity2D
{
    public Body Body;
    /// <summary>
    /// Size in pixels.
    /// </summary>
    public Vector2 Size;
    /// <summary>
    /// <para>Size in meters.</para>
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
    public virtual void FixedUpdate()
    {
        
    }
    public virtual void Draw(SpriteBatch batch)
    {
        
    }
}