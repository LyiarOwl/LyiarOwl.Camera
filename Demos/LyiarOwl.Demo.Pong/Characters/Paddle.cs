using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LyiarOwl.Demo.Pong.Characters
{
    public class Paddle : PhysicsEntity2D
    {
        private Vector2 _lockPhysicalPosition;
        protected float Speed = 10f;
        public Paddle(float x, float y, string tag = null) : base(20f, 150f)
        {
            Body = BodyFactory.CreateRectangle(
                World,
                PhysicalSize.X, PhysicalSize.Y,
                1f,
                default,
                0f,
                BodyType.Dynamic
            );
            Body.AngularDamping = 0f;
            Body.LinearDamping = 2f;
            Body.FixedRotation = true;
            Body.Friction = 0;
            Body.UserData = tag;
            Position = new Vector2(x, y);
            _lockPhysicalPosition = PhysicalPosition;
        }
        public override void Update() {}
        public override void FixedUpdate()
        {
            LockX();
        }
        public override void Draw(SpriteBatch batch)
        {
            batch.FillRectangle(
                Position - Size * 0.5f,
                Size,
                Color.White
            );
        }
        private void LockX()
        {
            /* lock x position */
            var physicalPosition = PhysicalPosition;
            physicalPosition.X = _lockPhysicalPosition.X;
            PhysicalPosition = physicalPosition;
        }
        public void ResetState()
        {
            var position = PhysicalPosition;
            position.Y = 0f;
            PhysicalPosition = position;
            Body.LinearVelocity = Vector2.Zero;
        }
    }
}