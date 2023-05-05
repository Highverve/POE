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
    public class TrailLine
    {
        public Vector2 Position { get; set; }
        public Color Color { get; private set; }

        public float SpeedMultiplier { get; private set; }
        //public float TimeSpeedMultiplier { get; set; }
        private int lineWidth;
        private Line line = new Line();

        public TrailLine(Color Color, float SpeedMultiplier, int LineWidth)
        {
            this.Color = Color;
            this.SpeedMultiplier = SpeedMultiplier;
            lineWidth = LineWidth;
        }

        public void UpdatePosition(GameTime gt, Vector2 DirectionToPrevious, float DistanceToPrevious)
        {
            if (DistanceToPrevious > 0)
                Position += DirectionToPrevious * (DistanceToPrevious * SpeedMultiplier) * (float)gt.ElapsedGameTime.TotalSeconds;
        }

        public void DrawLine(SpriteBatch sb, Texture2D pixel, Vector2 lastPosition, float depth)
        {
            line.locationA = Position;
            line.locationB = lastPosition;

            line.DrawLine(sb, pixel, Color, depth, lineWidth);
        }
    }

    public class TrailParticle : Particle
    {
        private TrailLine[] trailVectors;

        private Texture2D pixel;
        private bool isDrawingParticle;

        public TrailParticle() { }
        public TrailParticle(Texture2D Texture, Texture2D Pixel, Vector2 Position, Vector2 Velocity, float Angle, Color Color, float Size, int TimeLeft, float Depth, TrailLine[] TrailLines, bool IsDrawingParticle = false)
            : base(Texture, Position, Velocity, Angle, Color, Size, TimeLeft, Depth)
        {
            pixel = Pixel;

            trailVectors = TrailLines;

            for (int i = 0; i < trailVectors.Length; i++)
            {
                if (i != 0)
                    trailVectors[i].Position = Position;
            }

            isDrawingParticle = IsDrawingParticle;
        }

        public override void Update(GameTime gt)
        {
            for (int i = 0; i < trailVectors.Length; i++)
            {
                if (i != 0)
                {
                    //trailVectors[i].TimeSpeedMultiplier = ((TimeLeft / Lifetime) * 4f) + 1;

                    Vector2 direction = trailVectors[i - 1].Position - trailVectors[i].Position;

                    if (direction != null)
                        direction.Normalize();

                    trailVectors[i].UpdatePosition(gt, direction, Vector2.Distance(trailVectors[i].Position, trailVectors[i - 1].Position));
                }
                else //This is the leader!
                    trailVectors[0].Position = Position;
            }

            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < trailVectors.Length; i++)
            {
                if (i != 0)
                    trailVectors[i].DrawLine(sb, pixel, trailVectors[i - 1].Position, Depth);
            }

            if (isDrawingParticle == true)
                base.Draw(sb);
        }

        public override void RecycleParticle()
        {
            trailVectors = new TrailLine[0];
            isDrawingParticle = false;

            base.RecycleParticle();
        }
        public void SetRecycledVariables(Texture2D Texture, Texture2D Pixel, Vector2 Position, Vector2 Velocity, float Angle, Color Color, float Size, int TimeLeft, float Depth, TrailLine[] TrailLines, bool IsDrawingParticle = false)
        {
            SetMainRecycledVariables(Texture, Position, Velocity, Angle, Color, Size, TimeLeft, Depth);

            pixel = Pixel;

            trailVectors = TrailLines;

            for (int i = 0; i < trailVectors.Length; i++)
                if (i != 0)
                    trailVectors[i].Position = Position;

            isDrawingParticle = IsDrawingParticle;
        }
    }
}
