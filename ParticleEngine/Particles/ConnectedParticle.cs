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
    public class ConnectedParticle : Particle
    {
        private Particle connectedParticle;
        private Line line;
        public int LineWidth { get; set; }

        private Texture2D pixel;
        private bool isDrawingParticle;

        public ConnectedParticle() { }
        public ConnectedParticle(Texture2D Texture, Texture2D Pixel, Vector2 Position, Vector2 Velocity, float Angle, Color Color, float Size, int TimeLeft, float Depth, Particle ConnectedParticle, int LineWidth, bool IsDrawingParticle = false)
            : base(Texture, Position, Velocity, Angle, Color, Size, TimeLeft, Depth)
        {
            pixel = Pixel;

            connectedParticle = ConnectedParticle;
            this.LineWidth = LineWidth;
            isDrawingParticle = IsDrawingParticle;

            line = new Line();
        }

        public override void Update(GameTime gt)
        {
            if (connectedParticle != null)
            {
                line.locationA = Position;
                line.locationB = connectedParticle.Position;

                if (connectedParticle.IsActive == false)
                    connectedParticle = null;
            }

            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (connectedParticle != null)
            {
                line.DrawLine(sb, pixel, Color, Depth, LineWidth);
            }

            if (isDrawingParticle == true)
                base.Draw(sb);
        }

        public override void RecycleParticle()
        {
            connectedParticle = null;
            LineWidth = 0;
            isDrawingParticle = false;

            base.RecycleParticle();
        }
        public void SetRecycledVariables(Texture2D Texture, Texture2D Pixel, Vector2 Position, Vector2 Velocity,
                                         float Angle, Color Color, float Size, int TimeLeft, float Depth, Particle ConnectedParticle,
                                         int LineWidth, bool IsDrawingParticle = false)
        {
            SetMainRecycledVariables(Texture, Position, Velocity, Angle, Color, Size, TimeLeft, Depth);

            pixel = Pixel;

            connectedParticle = ConnectedParticle;
            this.LineWidth = LineWidth;
            isDrawingParticle = IsDrawingParticle;

            line = new Line();
        }
    }
}
