using Genbox.VelcroPhysics.Dynamics;
using LyiarOwl.Camera;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using LyiarOwl.Demo.Snake.Managers;
using Genbox.VelcroPhysics.Factories;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Genbox.VelcroPhysics.Collision.ContactSystem;

namespace LyiarOwl.Demo.Snake.Characters;

public class SnakeHead : PhysicalEntity2D
{
    private List<PhysicalEntity2D> _segments;
    public Vector2 Direction;
    // public Body Body;
    // public Vector2 Size;
    // public Vector2 Position
    // {
    //     get => Body.Position * _camera.Ppm;
    //     set => Body.Position = value / _camera.Ppm;
    // }
    // public Vector2 PhysicalPosition
    // {
    //     get => Body.Position;
    //     set => Body.Position = value;
    // }
    public SnakeHead()
    {
        World world = WorldManager.Instance.World;
        Size = new Vector2(32f, 32f);
        Body = BodyFactory.CreateRectangle(world, 0.75f, 0.75f, 1f, default, 0f, BodyType.Dynamic);
        Body.UserData = new UserData("Player"); /* this UserData goes to the body */
        Body.FixtureList[0].IsSensor = true;
        Body.OnCollision += OnCollisionEnter;

        _segments = [this];
    }

    private void OnCollisionEnter(Fixture fixtureA, Fixture fixtureB, Contact contact)
    {
        if (fixtureB.Body != null)
        {
            if (fixtureB.Body.UserData is UserData userData)
            {
                if (userData.Tag == "Fruit")
                {
                    Grow();
                }
                if (userData.Tag == "Obstacle")
                {
                    ResetState();
                }
            }
        }
    }

    public override void Update()
    {
        AssignDirection();
        
        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].PhysicalPosition = _segments[i - 1].PhysicalPosition;
        }
        
        PhysicalPosition = GetRoundedPosition(PhysicalPosition + Direction);
        Body.Awake = true;
        ContactManagerExt.FindNewContacts();
    }
    public override void Draw(SpriteBatch batch)
    {
        batch.FillRectangle(
            Position - Size * 0.5f,
            Size, 
            Color.White
        );

        foreach (var segment in _segments)
        {
            if (segment != this)
            {
                segment.Draw(batch);
            }
        }
    }
    private void Grow()
    {
        var segment = new SnakeSegment();
        segment.PhysicalPosition = GetRoundedPosition(_segments[_segments.Count - 1].PhysicalPosition);
        _segments.Add(segment);
    }
    private Vector2 GetRoundedPosition(Vector2 position) {
        return new Vector2(MathF.Round(position.X), MathF.Round(position.Y));
    }
    private void AssignDirection()
    {
        var keyboard = InputManager.Instance.Keyboard;
        if (keyboard.IsKeyDown(Keys.A))
            Direction = -Vector2.UnitX;
        if (keyboard.IsKeyDown(Keys.D))
            Direction = Vector2.UnitX;
        if (keyboard.IsKeyDown(Keys.W))
            Direction = -Vector2.UnitY;
        if (keyboard.IsKeyDown(Keys.S))
            Direction = Vector2.UnitY;
    }
    private void ResetState()
    {
        for (int i = 1; i < _segments.Count; i++)
        {
            WorldManager.Instance.World.DestroyBody(_segments[i].Body);
        }
        _segments.Clear();
        _segments.Add(this);
        PhysicalPosition = Vector2.Zero;
        Direction = Vector2.Zero;
    }
}