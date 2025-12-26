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

    public Fruit(RectangleF fruitArea)
    {
        _fruitArea = fruitArea;
        World world = WorldManager.Instance.World;
        Body = BodyFactory.CreateRectangle(world, 0.75f, 0.75f, 1f,
            new Vector2(-10f, 0f), 0f, BodyType.Static);
        Body.FixtureList[0].IsSensor = true;
        Body.UserData = Tags.Fruit;
        Size = new Vector2(Constants.PPM);
        Body.OnCollision += OnCollisionEnter;
        RandomizePosition();
    }

    private void OnCollisionEnter(Fixture sender, Fixture other, Contact contact)
    {
        if (other.Body != null)
        {
            if (other.Body.UserData != null && other.Body.UserData is string tag)
            {
                if (tag == Tags.Player)
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
            _fruitArea.Position / Constants.PPM,
            _fruitArea.Size / Constants.PPM
        );
        PhysicalPosition = new Vector2(
            MathF.Round(Random.Shared.NextSingle(physicalFruitArea.Left, physicalFruitArea.Right)),
            MathF.Round(Random.Shared.NextSingle(physicalFruitArea.Top, physicalFruitArea.Bottom))
        );
    }
}