using Genbox.VelcroPhysics.Dynamics;
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
    private readonly List<PhysicalEntity2D> _segments;
    private Vector2 _direction;
    public SnakeHead()
    {
        World world = WorldManager.Instance.World;
        Size = new Vector2(32f, 32f);
        Body = BodyFactory.CreateRectangle(
            world, 0.75f, 0.75f, 1f, default, 0f, BodyType.Dynamic
        );
        Body.UserData = Tags.Player; /* this UserData goes to the body */
        Body.FixtureList[0].IsSensor = true;
        Body.OnCollision += OnCollisionEnter;

        _segments = [this];
    }

    private void OnCollisionEnter(Fixture fixtureA, Fixture fixtureB, Contact contact)
    {
        if (fixtureB.Body != null)
        {
            if (fixtureB.Body.UserData != null && fixtureB.Body.UserData is string tag)
            {
                if (tag == Tags.Fruit)
                {
                    Grow();
                }
                if (tag == Tags.Obstacle)
                {
                    ResetState();
                }
            }
        }
    }

    public override void FixedUpdate()
    {
        AssignDirection();
        
        UpdateSegmentsPosition();
        
        PhysicalPosition = GetRoundedPosition(PhysicalPosition + _direction);
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
            _direction = -Vector2.UnitX;
        if (keyboard.IsKeyDown(Keys.D))
            _direction = Vector2.UnitX;
        if (keyboard.IsKeyDown(Keys.W))
            _direction = -Vector2.UnitY;
        if (keyboard.IsKeyDown(Keys.S))
            _direction = Vector2.UnitY;
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
        _direction = Vector2.Zero;
    }
    private void UpdateSegmentsPosition()
    {
        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].PhysicalPosition = _segments[i - 1].PhysicalPosition;
        }
    }
}