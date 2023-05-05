using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pilgrimage_Of_Embers.ParticleEngine.Particles
{
    public class FloorParticle : Particle
    {
        int floor, bounceCount;
        public int PixelFloor { get { return floor; } }
        public int BounceCount { get { return bounceCount; } set { bounceCount = value; } }

        public FloorParticle() { }
        public FloorParticle(Texture2D Texture, Vector2 Position, Vector2 Velocity, float Angle, Color Color, float Size, int TimeLeft, float Depth, int FloorFromStart)
            : base (Texture, Position, Velocity, Angle, Color, Size, TimeLeft, Depth)
        {
            floor = (int)Position.Y + FloorFromStart;
        }
        public FloorParticle(Texture2D Texture, Point FrameSize, Vector2 Position, Vector2 Velocity, float Angle, Color Color, float Size, int TimeLeft, float Depth, int FloorFromStart)
            : base(Texture, FrameSize, Position, Velocity, Angle, Color, Size, TimeLeft, Depth)
        {
            floor = (int)Position.Y + FloorFromStart;
        }

        public override void Update(GameTime gt)
        {
            TimeLeft -= gt.ElapsedGameTime.Milliseconds;

            circle.Position += Velocity * (float)gt.ElapsedGameTime.TotalSeconds;
            circle.Y = MathHelper.Clamp(circle.Y, circle.Y - 500, floor + 1);
            Position = circle.Position;

            Size = MathHelper.Clamp(Size, 0f, 10f);

            circle.radius = Size * 2f;
        }

        public override void RecycleParticle()
        {
            floor = 0;
            bounceCount = 0;

            base.RecycleParticle();
        }
        public void SetRecycledVariables(Texture2D Texture, Vector2 Position, Vector2 Velocity, float Angle, Color Color, float Size, int TimeLeft, float Depth, int FloorFromStart)
        {
            SetMainRecycledVariables(Texture, Position, Velocity, Angle, Color, Size, TimeLeft, Depth);
            floor = (int)Position.Y + FloorFromStart;
        }
        public void SetRecycledVariables(Texture2D Texture, Point FrameSize, Vector2 Position, Vector2 Velocity, float Angle, Color Color, float Size, int TimeLeft, float Depth, int FloorFromStart)
        {
            SetMainRecycledVariables(Texture, FrameSize, Position, Velocity, Angle, Color, Size, TimeLeft, Depth);
            floor = (int)Position.Y + FloorFromStart;
        }
    }
}
