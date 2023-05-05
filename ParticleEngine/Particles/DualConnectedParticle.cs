using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrimage_Of_Embers.ParticleEngine.Particles
{
    public class DualConnectedParticle : Particle
    {
        private Particle connectedParticle, secondParticle;
        private Line lineOne, lineTwo;
        public int LineWidth { get; set; }

        private Texture2D pixel;
        private bool isDrawingParticle;

        public DualConnectedParticle() { }
        public DualConnectedParticle(Texture2D Texture, Texture2D Pixel, Vector2 Position, Vector2 Velocity, float Angle, Color Color, float Size, int TimeLeft, float Depth, Particle ConnectedParticle, Particle SecondParticle, int LineWidth, bool IsDrawingParticle = false)
            : base(Texture, Position, Velocity, Angle, Color, Size, TimeLeft, Depth)
        {
            pixel = Pixel;

            connectedParticle = ConnectedParticle;
            secondParticle = SecondParticle;

            this.LineWidth = LineWidth;
            isDrawingParticle = IsDrawingParticle;

            lineOne = new Line();
            lineTwo = new Line();
        }

        public override void Update(GameTime gt)
        {
            if (connectedParticle != null)
            {
                lineOne.locationA = Position;
                lineOne.locationB = connectedParticle.Position;

                if (connectedParticle.IsActive == false)
                    connectedParticle = null;
            }

            if (secondParticle != null)
            {
                lineTwo.locationB = Position;
                lineTwo.locationA = secondParticle.Position;

                if (secondParticle.IsActive == false)
                    secondParticle = null;
            }

            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (connectedParticle != null)
                lineOne.DrawLine(sb, pixel, Color, Depth, LineWidth);

            if (secondParticle != null)
                lineTwo.DrawLine(sb, pixel, Color, Depth, LineWidth);

            if (isDrawingParticle == true)
                base.Draw(sb);
        }

        public override void RecycleParticle()
        {
            connectedParticle = null;
            secondParticle = null;

            LineWidth = 0;
            isDrawingParticle = false;

            base.RecycleParticle();
        }
        public void SetRecycledVariables(Texture2D Texture, Texture2D Pixel, Vector2 Position, Vector2 Velocity, float Angle,
                                         Color Color, float Size, int TimeLeft, float Depth, Particle ConnectedParticle,
                                         Particle SecondParticle, int LineWidth, bool IsDrawingParticle = false)
        {
            SetMainRecycledVariables(Texture, Position, Velocity, Angle, Color, Size, TimeLeft, Depth);

            pixel = Pixel;

            connectedParticle = ConnectedParticle;
            secondParticle = SecondParticle;

            this.LineWidth = LineWidth;
            isDrawingParticle = IsDrawingParticle;

            lineOne = new Line();
            lineTwo = new Line();
        }
    }
}
