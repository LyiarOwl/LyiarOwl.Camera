using System;
using Genbox.VelcroPhysics.Factories;
using LyiarOwl.Demo.Pong.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LyiarOwl.Demo.Pong.Characters
{
    public class PlayerPaddle : Paddle
    {
        private float _yAxis;
        /// <param name="x">Position in pixels.</param>
        /// <param name="y">Position in pixels.</param>
        public PlayerPaddle(float x, float y) : base(x, y, Tags.PlayerPaddle)
        {
        }

        public override void Update()
        {
            _yAxis = GetYInputAxis();
        }
        public override void FixedUpdate()
        {
            if (_yAxis != 0f)
            {
                Body.ApplyForce(new Vector2(0f, _yAxis) * Speed);
            }

            base.FixedUpdate();
        }
        private float GetYInputAxis()
        {
            var keyboard = InputsManager.Instance.Keyboard;
            float up = keyboard.IsKeyDown(Keys.W) ? -1f : 0f;
            float down = keyboard.IsKeyDown(Keys.S) ? 1f : 0f;
            return up + down;
        }
    }
}