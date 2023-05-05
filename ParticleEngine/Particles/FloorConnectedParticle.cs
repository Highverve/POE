using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrimage_Of_Embers.ParticleEngine.Particles
{
    public class FloorConnectedParticle : ConnectedParticle
    {
        int floor, bounceCount;
        public int PixelFloor { get { return floor; } }
        public int BounceCount { get { return bounceCount; } set { bounceCount = value; } }

        public FloorConnectedParticle() { }
        public FloorConnectedParticle(Texture2D Texture, Texture2D Pixel, Vector2 Position, Vector2 Velocity, float Angle, Color Color, float Size, int TimeLeft, float Depth,
                  Particle ConnectedParticle, int LineWidth, int Floor, bool IsDrawingParticle = false)
            : base (Texture, Pixel, Position, Velocity, Angle, Color, Size, TimeLeft, Depth, ConnectedParticle, LineWidth, IsDrawingParticle)
        {
            floor = (int)Position.Y + Floor;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            circle.Y = MathHelper.Clamp(circle.Y, circle.Y - 500, floor + 1);
            Position = circle.Position;
        }

        public override void RecycleParticle()
        {
            floor = 0;

            base.RecycleParticle();
        }
        public void SetRecycledVariables(Texture2D Texture, Texture2D Pixel, Vector2 Position, Vector2 Velocity, float Angle,
                                         Color Color, float Size, int TimeLeft, float Depth, Particle ConnectedParticle,
                                         int LineWidth, int Floor, bool IsDrawingParticle = false)
        {
            SetRecycledVariables(Texture, Pixel, Position, Velocity, Angle, Color, Size, TimeLeft, Depth, ConnectedParticle, LineWidth, IsDrawingParticle);

            floor = (int)Position.Y + Floor;
        }
    }
}
