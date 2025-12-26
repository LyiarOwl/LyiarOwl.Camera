using System;
using Genbox.VelcroPhysics.Collision.ContactSystem;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using LyiarOwl.Demo.Snake.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LyiarOwl.Demo.Snake.Characters;

public class Fruit : PhysicalEntity2D
{
    private Color _color = new Color(255, 40, 88);
    private readonly RectangleF _fruitArea;

    // public Body Body;
    // public Vector2 Size;
    // public Vector2 Position
    // {
    //     get => Body.Position * WorldManager.PPM;
    //     set => Body.Position = value / WorldManager.PPM;
    // }
    // public Vector2 PhysicalPosition
    // {
    //     get => Body.Position;
    //     set => Body.Position = value;
    // }

    public Fruit(RectangleF fruitArea)
    {
        _fruitArea = fruitArea;
        World world = WorldManager.Instance.World;
        Body = BodyFactory.CreateRectangle(world, 0.75f, 0.75f, 1f,
            new Vector2(-10f, 0f), 0f, BodyType.Static);
        Body.FixtureList[0].IsSensor = true;
        Body.UserData = new UserData("Fruit");
        Size = new Vector2(32f);
        Body.OnCollision += OnCollisionEnter;
        RandomizePosition();
    }

    private void OnCollisionEnter(Fixture sender, Fixture other, Contact contact)
    {
        if (other.Body != null)
        {
            if (other.Body.UserData is UserData userData)
            {
                if (userData.Tag == "Player")
                {
                    RandomizePosition();
                }
            }
        }
    }

    public override void Draw(SpriteBatch batch)
    {
        batch.FillRectangle(
            Position - Size * 0.5f,
            Size,
            _color
        );
    }
    private void RandomizePosition()
    {
        var physicalFruitArea = new RectangleF(
            _fruitArea.Position / WorldManager.PPM,
            _fruitArea.Size / WorldManager.PPM
        );
        PhysicalPosition = new Vector2(
            MathF.Round(Random.Shared.NextSingle(physicalFruitArea.Left, physicalFruitArea.Right)),
            MathF.Round(Random.Shared.NextSingle(physicalFruitArea.Top, physicalFruitArea.Bottom))
        );
    }
}