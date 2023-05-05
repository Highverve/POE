using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes
{
    public class TakeImage : BaseEmitter
    {
        Texture2D rawImage, imageFile, pixel;
        Rectangle clipping;

        public BaseEntity KillerEntity { get; set; }

        protected float scale;

        public TakeImage(int ID, int CurrentFloor, int DepthCenterY, Texture2D Image, float Scale)
            : base(ID, CurrentFloor, DepthCenterY)
        {
            imageFile = Image;
            scale = Scale;

            isAdded = true;
        }

        public TakeImage(int ID, int CurrentFloor, int DepthCenterY, Texture2D Image, Rectangle Clipping, float Scale)
            : base(ID, CurrentFloor, DepthCenterY)
        {
            rawImage = Image;
            clipping = Clipping;

            scale = Scale;

            fixedRandom = new Random(ID + CurrentFloor * DepthCenterY + (Image.Width * Image.Height) -
                               (int)(Image.Center().Y + Image.Center().X));

            isAdded = true;
        }

        public override void Load(ContentManager cm)
        {
            pixel = cm.Load<Texture2D>("rect");
            base.Load(cm);
        }

        bool isAdded = false;
        public bool IsAdded { set { isAdded = value; } }

        public override void Update(GameTime gt)
        {
            if (isActivated == true)
            {
                depthFloor.UpdateDepth(Position.Y);

                if (isAdded == false)
                {
                    AddParticles();
                    isAdded = true;
                }
            }
            base.Update(gt);
        }
        protected virtual void AddParticles()
        {
        }

        bool isAssigned = false;
        public override void Draw(SpriteBatch sb)
        {
            if (isAssigned == false)
            {
                if (imageFile == null) //If the image file has not been assigned a value (E.G, if the second constructor was chosen rather than the first one)
                    imageFile = rawImage.Crop(sb.GraphicsDevice, clipping);

                isAssigned = true;
            }

            base.Draw(sb);
        }

        public void ResetImage(Rectangle clipping)
        {
            this.clipping = clipping;
            imageFile = null;
            isAssigned = false;
        }
        public void ResetImage(Texture2D texture, Rectangle clipping)
        {
            rawImage = texture;
            this.clipping = clipping;
            isAssigned = false;
        }

        protected void AddPixels(int minLifeTime, int maxLifeTime, Vector2 minVelocity, Vector2 maxVelocity)
        {
            Color[] data = new Color[imageFile.Width * imageFile.Height];
            imageFile.GetData(data);

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].A > 0) //if it is not transparent
                {
                    float x, y; //Experimental code!
                    y = i / imageFile.Height; //i = 769, length = 32 x 32 = 1024, height = 32   :   769 / 32 = 24.03125
                    x = i - (y * imageFile.Width); //24.03125 % 24 = .03125 * 32(width) = 1
                    //x = 1, y = 24

                    particles.Add(new Particle(pixel, new Vector2(Position.X + (x * scale), Position.Y + (y * scale)),
                                  new Vector2(trueRandom.NextFloat(minVelocity.X, maxVelocity.X), trueRandom.NextFloat(minVelocity.Y, maxVelocity.Y)), 0f, data[i], scale, trueRandom.Next(minLifeTime, maxLifeTime), 1f));
                }
            }
        }

        protected void AddPixels(int minLifeTime, int maxLifeTime, Vector2 minVelocity, Vector2 maxVelocity, int relativeMaxFloorY)
        {
            Color[] data = new Color[imageFile.Width * imageFile.Height];
            imageFile.GetData(data);

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].A > 0) //if it is not transparent
                {
                    float x, y;
                    y = i / imageFile.Height; //i = 769, length = 32 x 32 = 1024, height = 32   :   769 / 32 = 24.03125
                    x = i - (y * imageFile.Width); //24.03125 % 24 = .03125 * 32(width) = 1
                    //x = 1, y = 24

                    Particle particle = NextRecycle();
                    particle.SetMainRecycledVariables(pixel, new Vector2(Position.X + (x * scale), Position.Y + (y * scale)),
                                  new Vector2(trueRandom.NextFloat(minVelocity.X, maxVelocity.X), trueRandom.NextFloat(minVelocity.Y, maxVelocity.Y)), 0f, data[i], scale, trueRandom.Next(minLifeTime, maxLifeTime), 1f);

                    particles.Add(particle);
                    particles.Last().Floor = (int)(Position.Y + (y * scale) + relativeMaxFloorY);
                }
            }
        }
    }
}
