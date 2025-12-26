using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Genbox.VelcroPhysics.Dynamics;
using LyiarOwl.Demo.Pong.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LyiarOwl.Demo.Pong.Characters
{
    public class PhysicsEntity2D
    {
        protected World World => WorldManager.Instance.World;
        public Body Body;
        public Vector2 Size;
        public Vector2 PhysicalSize
        {
            get => Size / Constants.PPM;
            set => Size = value * Constants.PPM;
        }
        public Vector2 Position
        {
            get => Body.Position * Constants.PPM;
            set => Body.Position = value / Constants.PPM;
        }
        public Vector2 PhysicalPosition
        {
            get => Body.Position;
            set => Body.Position = value;
        }
        public PhysicsEntity2D(float width, float height)
        {
            Size = new Vector2(width, height);
        }

        public virtual void Update()
        {
        }
        public virtual void FixedUpdate()
        {
        }
        public virtual void Draw(SpriteBatch batch) {}
    }
}