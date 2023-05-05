using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.ParticleEngine
{
    public class Particle
    {
        protected Texture2D Texture;
        public Vector2 Origin { get; set; }
        public Vector2 Position;
        public Vector2 Velocity;
        public float Angle { get; set; }
        public Color Color;
        public Color StartingColor { get; set; }
        public float Size { get; set; }
        public int TimeLeft { get; set; }
        public int Lifetime { get; set; }

        public int Floor { get; set; }

        public float Depth { get; set; }

        protected Circle circle = new Circle();
        public Circle Circle { get { return circle; } }

        private bool isActive;
        public bool IsActive { get { return isActive; } }

        private Point currentFrame, frameSize, sheetSize;

        public static int CurrentParticles, RecycledParticles;

        public Particle() { }
        public Particle(Texture2D Texture, Vector2 Position, Vector2 Velocity, float Angle, Color Color, float Size, int TimeLeft, float Depth)
        {
            this.Texture = Texture;
            this.Position = Position;
            this.Velocity = Velocity;
            this.Angle = Angle;
            this.Color = Color;
            this.StartingColor = Color;
            this.Size = Size;

            this.TimeLeft = TimeLeft;
            Lifetime = TimeLeft;

            this.Depth = Depth;

            circle.Position = this.Position;

            isActive = true;

            CurrentParticles++;

            currentFrame = new Point(0, 0);

            if (Texture != null)
            {
                frameSize = new Point(Texture.Width, Texture.Height);
                Origin = Texture.Center();
            }

            sheetSize = new Point(1, 1);
        }
        public Particle(Texture2D Texture, Point FrameSize, Vector2 Position, Vector2 Velocity, float Angle, Color Color, float Size, int TimeLeft, float Depth)
        {
            this.Texture = Texture;
            this.Position = Position;
            this.Velocity = Velocity;
            this.Angle = Angle;
            this.Color = Color;
            this.StartingColor = Color;
            this.Size = Size;

            this.TimeLeft = TimeLeft;
            Lifetime = TimeLeft;

            this.Depth = Depth;

            circle.Position = this.Position;

            isActive = true;

            CurrentParticles++;

            currentFrame = new Point(0, 0);
            frameSize = FrameSize;
            sheetSize = new Point(Texture.Width / frameSize.X, Texture.Height / frameSize.Y);

            Origin = new Vector2(frameSize.X / 2, frameSize.Y / 2);
        }

        public virtual void Update(GameTime gt)
        {
            TimeLeft -= gt.ElapsedGameTime.Milliseconds;

            circle.Position += Velocity * (float)gt.ElapsedGameTime.TotalSeconds;
            Position = circle.Position;

            Size = MathHelper.Clamp(Size, 0f, 10f);

            circle.radius = Size * 2f;
        }
        public virtual void Draw(SpriteBatch sb)
        {
            if (Texture != null)
                sb.Draw(Texture, Position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color, Angle, Origin, Size, SpriteEffects.None, Depth);
        }

        // [Methods]
        public void SetCurrentFrame(Point frame)
        {
            currentFrame.X = MathHelper.Clamp(frame.X, 0, sheetSize.X - 1);
            currentFrame.Y = MathHelper.Clamp(frame.Y, 0, sheetSize.Y - 1);
        }
        public void DestroyParticle()
        {
            isActive = false;
            CurrentParticles--;
        }

        public virtual void RecycleParticle()
        {
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Angle = 0f;
            Color = Color.Transparent;
            StartingColor = Color.Transparent;
            Size = 1f;

            TimeLeft = 0;
            Lifetime = 0;

            Depth = 0f;

            circle.Position = Vector2.Zero;

            currentFrame = Point.Zero;
            frameSize = Point.Zero;
            sheetSize = Point.Zero;

            Origin = Vector2.Zero;

            isActive = false;
            CurrentParticles--;
        }
        public void SetMainRecycledVariables(Texture2D Texture, Vector2 Position, Vector2 Velocity,
                                             float Angle, Color Color, float Size, int TimeLeft, float Depth)
        {
            this.Texture = Texture;
            this.Position = Position;
            this.Velocity = Velocity;
            this.Angle = Angle;
            this.Color = Color;
            this.StartingColor = Color;
            this.Size = Size;

            this.TimeLeft = TimeLeft;
            Lifetime = TimeLeft;

            this.Depth = Depth;

            circle.Position = this.Position;

            currentFrame = new Point(0, 0);

            if (Texture != null)
            {
                frameSize = new Point(Texture.Width, Texture.Height);
                Origin = Texture.Center();
            }

            sheetSize = new Point(1, 1);

            isActive = true;
            CurrentParticles++;
        }
        public void SetMainRecycledVariables(Texture2D Texture, Point FrameSize, Vector2 Position, Vector2 Velocity,
                                 float Angle, Color Color, float Size, int TimeLeft, float Depth)
        {
            this.Texture = Texture;
            this.Position = Position;
            this.Velocity = Velocity;
            this.Angle = Angle;
            this.Color = Color;
            this.StartingColor = Color;
            this.Size = Size;

            this.TimeLeft = TimeLeft;
            Lifetime = TimeLeft;

            this.Depth = Depth;

            circle.Position = this.Position;

            currentFrame = new Point(0, 0);
            frameSize = FrameSize;
            sheetSize = new Point(Texture.Width / frameSize.X, Texture.Height / frameSize.Y);

            Origin = new Vector2(frameSize.X / 2, frameSize.Y / 2);

            isActive = true;
            CurrentParticles++;
        }
    }
}
