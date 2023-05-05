using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ParticleEngine;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.ScreenEngine;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.Culture;

namespace Pilgrimage_Of_Embers.Entities.Effects
{
    public class BaseVisual
    {
        private int id;
        private string name;
        private Texture2D spriteSheet;

        private int frameTime = 0, lifeTime;
        private Point currentFrame, frameSize, sheetSize;
        protected int framesPassed = 0; //Used for stopping animations after X amount of frames has passed, or other manipulations.

        private float depth, scale;
        private bool isFrontside;

        private bool isActive, isStopping, isMultiple;

        private BaseEmitter emitter;

        private BasicAnimation animate = new BasicAnimation();
        private BaseEntity currentEntity;

        public int ID { get { return id; } }
        public string Name { get { return name; } }

        public bool IsActive { get { return isActive; } }
        public bool IsMultiple { get { return isMultiple; } }

        public enum VisualType
        {
            Animated,
            Emitter,
        }
        private VisualType type;
        public VisualType Type { get { return type; } }

        public BaseVisual(int ID, string Name, Texture2D Spritesheet, Point FrameSize, int FrameTime, float Scale, bool IsFrontside, int LifeTime = -1, bool IsMultiple = false)
        {
            id = ID;
            name = Name;
            spriteSheet = Spritesheet;

            frameSize = FrameSize;
            sheetSize = new Point(spriteSheet.Width / frameSize.X, spriteSheet.Height / frameSize.Y);

            frameTime = FrameTime;
            scale = Scale;
            isFrontside = IsFrontside;
            lifeTime = LifeTime;
            isMultiple = IsMultiple;

            type = VisualType.Animated;
            isActive = true;
        }
        public BaseVisual(int ID, string Name, BaseEmitter Emitter, bool IsFrontside, int LifeTime = -1, bool IsMultiple = false)
        {
            id = ID;
            name = Name;

            emitter = Emitter;

            isFrontside = IsFrontside;
            lifeTime = LifeTime;
            isMultiple = IsMultiple;

            type = VisualType.Emitter;
            isActive = true;
        }

        public virtual void Load(ContentManager cm)
        {
            if (emitter != null)
                emitter.Load(cm);
        }
        public virtual void Unload()
        {

        }
        public virtual void Update(GameTime gt)
        {
            if (isFrontside == true)
                depth = currentEntity.Depth + .00001f;
            else
                depth = currentEntity.Depth - .00001f;

            if (type == VisualType.Animated)
            {
                currentFrame = animate.FramePosition(gt, frameTime, sheetSize, true);

                if (isStopping == true)
                    isActive = false;
            }
            else
            {
                if (emitter != null)
                {
                    emitter.Depth = depth;
                    emitter.Offset = currentEntity.Position;
                    emitter.Update(gt);

                    if (isStopping == true)
                    {
                        emitter.IsActivated = false;

                        if (emitter.Particles.Count <= 0)
                            isActive = false;
                    }
                }
            }

            if (lifeTime != -1)
            {
                lifeTime -= gt.ElapsedGameTime.Milliseconds;

                if (lifeTime <= 0)
                    isStopping = true;
            }
        }
        public virtual void Draw(SpriteBatch sb)
        {
            if (type == VisualType.Animated)
            {
                sb.Draw(spriteSheet, currentEntity.Position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y),
                        Color.White, 0f, new Vector2(frameSize.X / 2, frameSize.Y / 2), scale, SpriteEffects.None, depth);
            }
            else
            {
                if (emitter != null)
                    emitter.Draw(sb);
            }
        }

        public void BeginStop()
        {
            isStopping = true;
        }
        public void ForceStop()
        {
            isActive = false;
        }

        public BaseVisual Copy(BaseEntity currentEntity, TileMap map, Camera camera, ScreenManager screens, PlayerEntity player, WeatherManager weather, CultureManager culture, BaseEntity controlledEntity, List<BaseEntity> entities)
        {
            BaseVisual copy = (BaseVisual)MemberwiseClone();

            copy.currentEntity = currentEntity;
            copy.animate = new BasicAnimation();

            if (emitter != null)
            {
                copy.emitter = emitter.Copy();
                copy.emitter.SetReferences(map, camera, screens, player, weather, culture, controlledEntity, entities);
                copy.emitter.IsActivated = true;
            }

            return copy;
        }

        //The following methods are being considered for removal
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            BaseVisual visual = (BaseVisual)obj;
            return (id == visual.id);
        }
        public override int GetHashCode()
        {
            return id;
        }
    }
}
