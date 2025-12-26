using System.Numerics;

namespace LyiarOwl.Demo.Pong.Characters
{
   public class CPUPaddle : Paddle
    {
        private readonly Ball _ball;
        /// <param name="x">Position in pixels.</param>
        /// <param name="y">Position in pixels.</param>
        public CPUPaddle(float x, float y, Ball ball) : base(x, y, Tags.CPUPaddle)
        {
            _ball = ball;
        }

        public override void FixedUpdate()
        {
            /* move towards ball */
            if (_ball.Body.LinearVelocity.X > 0f)
            {
                if (Position.Y > _ball.Position.Y) /* ball is below this paddle */
                    Body.ApplyForce(-Vector2.UnitY * Speed);
                else
                    Body.ApplyForce(Vector2.UnitY * Speed);
            }
            /* move towards center */
            else
            {
                if (Position.Y > 0f) /* this paddle is below the center */
                    Body.ApplyForce(-Vector2.UnitY * Speed);
                else
                    Body.ApplyForce(Vector2.UnitY * Speed);
            }

            base.FixedUpdate();
        }
    }
}