using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.Entities.Actions
{
    /// <summary>
    /// Marked for removal! Or replace with existing GameObject code to tidy up that class.
    /// </summary>
    public class Jumping
    {
        bool canJump = true;
        bool isCollidable = true;
        public bool IsCollidable { get { return isCollidable; } }

        public enum JumpState { Ground, Air, Landed }
        private JumpState jumpState = JumpState.Ground;
        public JumpState CurrentState { get { return jumpState; } }

        private Circle jumpCircle, baseCircle;
        public Circle JumpCircle { get { return jumpCircle; } }

        const float gravity = 9f;
        float velocity, mass, offset = -0.001f;
        public float Offset { get { return offset; } }
        public int JumpHeightFloor { get { return -((int)(offset / 100f)); } }

        int timesJumped = 0, prevTimesJumped;
        public int TimesJumped { get { return timesJumped; } }

        public Jumping(float CircleRadius, float Mass)
        {
            baseCircle = new Circle(Vector2.Zero, CircleRadius);
            jumpCircle = baseCircle;

            mass = Mass;
            velocity = MathHelper.Clamp(-300f, 0f, 1000f);
        }

        public void UpdateTemp()
        {
            prevTimesJumped = timesJumped;
        }
        public void Update(GameTime gt, Vector2 center)
        {
            jumpCircle.Position = center;

            if (jumpState == JumpState.Landed)
                ResetValues();

            if (jumpState == JumpState.Air)
                UpdateJump(gt);
        }
        private void UpdateJump(GameTime gt)
        {
            jumpCircle = Circle.Empty;

            if (offset < -50f)
                isCollidable = false;
            else
                isCollidable = true;

            if (offset < 0f) //If entity is still in air
                AccelerateJump(gt);
            else
                jumpState = JumpState.Landed;
        }

        private void AccelerateJump(GameTime gt)
        {
            velocity += (gravity * mass) * (float)gt.ElapsedGameTime.TotalSeconds;
            offset += velocity * (float)gt.ElapsedGameTime.TotalSeconds;

            //offset = MathHelper.Clamp(offset, -2048f, 0f);
        }
        private void ResetValues()
        {
            jumpCircle = baseCircle;
            offset = -0.001f;

            timesJumped++;

            jumpState = JumpState.Ground;
        }

        public void SetHeight(float offsetHeight)
        {
            velocity = 0f;
            offset = offsetHeight;
            jumpState = JumpState.Air;
        }
        public void SetVelocity(float velocity)
        {
            this.velocity = velocity;
        }
        public void SetMass(float mass)
        {
            this.mass = mass;
        }

        public void Jump(float jumpHeight)
        {
            if (jumpState == JumpState.Ground)
            {
                velocity = -jumpHeight;//MathHelper.Clamp(jumpHeight, 0f, 6f);
                jumpState = JumpState.Air;
            }
        }
        public void ForceJump()
        {
            jumpState = JumpState.Air;
        }
        public void Bounce(int jumpCount, float jumpForce)
        {
            if (TimesJumped == jumpCount && prevTimesJumped == (jumpCount - 1))
                Jump(jumpForce);
        }

        public void ForceFall()
        {

        }

        public Jumping Copy()
        {
            return new Jumping(JumpCircle.radius, mass);
        }
    }
}
