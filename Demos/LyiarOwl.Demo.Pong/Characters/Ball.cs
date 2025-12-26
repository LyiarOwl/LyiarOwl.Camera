using System;
using Genbox.VelcroPhysics.Collision.ContactSystem;
using Genbox.VelcroPhysics.Collision.Handlers;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LyiarOwl.Demo.Pong.Characters
{
    public class Ball : PhysicsEntity2D
    {
        private Vector2 _baseVelocity;
        private float _currentSpeed;
        private const float _BASE_SPEED = 5f;
        public Ball() : base(25, 25)
        {
            Body = BodyFactory.CreateRectangle(
                World,
                PhysicalSize.X, PhysicalSize.Y,
                1f,
                default,
                0f,
                BodyType.Dynamic
            );
            Body.Restitution = 1f;
            Body.Friction = 0f;
            Body.FixedRotation = true;

            Body.OnCollision += OnCollision;
            _currentSpeed = _BASE_SPEED;

            AddStartingForce();
        }

        private void OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.Body != null)
            {
                if (fixtureB.Body.UserData != null && fixtureB.Body.UserData is string tag)
                {
                    if (tag == Tags.TopWall || tag == Tags.BottomWall)
                    {
                        IncreaseSpeed(0.25f);
                    }
                    if (tag == Tags.PlayerPaddle || tag == Tags.CPUPaddle)
                    {
                        IncreaseSpeed(0.5f);
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            /* clamp velocity */
            var velocity = Body.LinearVelocity;
            if (velocity.X > 0f) velocity.X = _baseVelocity.X;
            else velocity.X = -_baseVelocity.X;
            if (velocity.Y > 0f) velocity.Y = _baseVelocity.Y;
            else velocity.Y = -_baseVelocity.Y;
            Body.LinearVelocity = velocity;
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.FillRectangle(
                Position - Size * 0.5f,
                Size,
                Color.White
            );

        }

        public void AddStartingForce()
        {
            float x = FastRandom.Shared.NextSingle() < 0.5f ? -1f : 1f;
            float y = FastRandom.Shared.NextSingle() < 0.5f ? 
                FastRandom.Shared.NextSingle(-1f, -0.5f) : FastRandom.Shared.NextSingle(0.5f, 1f);
            var direction = new Vector2(x, y);
            Body.LinearVelocity = direction * _currentSpeed;
            UpdateBaseVelocity();
        }
        private void IncreaseSpeed(float speed)
        {
            _currentSpeed += speed;
            var direction = Body.LinearVelocity.NormalizedCopy();
            Body.LinearVelocity = direction * _currentSpeed;
            UpdateBaseVelocity();
        }
        private void UpdateBaseVelocity()
        {
            _baseVelocity = new Vector2()
            {
                X = MathF.Abs(Body.LinearVelocity.X),
                Y = MathF.Abs(Body.LinearVelocity.Y),
            };
        }
        public void ResetState()
        {
            PhysicalPosition = Vector2.Zero;
            Body.LinearVelocity = Vector2.Zero;
            _currentSpeed = _BASE_SPEED;
            AddStartingForce();
        }
    }
}