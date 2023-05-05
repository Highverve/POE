using System.Collections.Generic;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.TileEngine.Objects.Colliders;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.TileEngine;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Helper_Classes;
using Microsoft.Xna.Framework.Graphics;

namespace Pilgrimage_Of_Embers.CombatEngine.AOE
{
    public class BaseAOE
    {
        int id; string name;

        public int ID { get { return id; } }
        public string Name { get { return name; } }
        public string KillerID { get; set; }

        protected Vector2 position, positionOffset;

        protected Circle effectCircle;
        protected float effectRadius, minRadius, maxRadius; //This is only for determining if an entity is in range!
        protected int entityDamageSuspension;

        public enum EffectState
        {
            Initialize,
            Update,
            Terminate,
            Deactivated,
        }
        protected EffectState state = EffectState.Initialize;
        public EffectState State { get { return state; } }

        protected List<BaseEntity> entities;
        protected List<BaseEntity> closeEntities = new List<BaseEntity>();

        protected List<BaseCollider> colliders;
        protected List<BaseCollider> closeColliders = new List<BaseCollider>();

        protected TileMap tileMap;
        protected Camera camera;

        protected Texture2D pixel, circle;

        public BaseAOE(int ID, string Name)
        {
            id = ID;
            name = Name;

            effectCircle = new Circle();
        }

        public void SetReferences(List<BaseEntity> entities, List<BaseCollider> colliders, TileMap tileMap, Camera camera, Vector2 position)
        {
            this.entities = entities;
            this.colliders = colliders;
            this.tileMap = tileMap;
            this.camera = camera;
            positionOffset = position;
        }

        protected void SetValues(float effectRadius, float minRadius, float maxRadius, int entityDamageSuspension)
        {
            this.effectRadius = effectRadius;
            this.minRadius = minRadius;
            this.maxRadius = maxRadius;
            this.entityDamageSuspension = entityDamageSuspension;
        }

        public virtual void Load(ContentManager cm)
        {
            pixel = cm.Load<Texture2D>("rect");
        }

        protected virtual void Initialize()
        {

        }

        public void Update(GameTime gt)
        {
            position = positionOffset;
            effectCircle.Position = position;

            AddColliders(gt);
            SortEntities(gt);

            UpdateBehavior(gt);

            for (int i = 0; i < closeEntities.Count; i++)
            {
                if (IsEntityHit(closeEntities[i]))
                {
                    ApplyEntityDamage(closeEntities[i]);
                }
            }
        }
        protected virtual void UpdateBehavior(GameTime gt)
        {

        }
        protected virtual void Terminate()
        {

        }

        public virtual void Draw(SpriteBatch sb)
        {

        }
        public virtual void DrawDebug(SpriteBatch sb)
        {

        }

        private int entityTimer = 0;
        private void SortEntities(GameTime gt)
        {
            entityTimer += gt.ElapsedGameTime.Milliseconds;

            if (entityTimer >= 200)
            {
                closeEntities.Clear();
                for (int j = 0; j < entities.Count; j++)
                {
                    if ((Vector2.Distance(entities[j].Position, position) <= (effectRadius + (effectRadius / 5))))
                    {
                        bool isBehindCollider = false;
                        for (int i = 0; i < closeColliders.Count; i++)
                        {
                            if (closeColliders[i].IsBehind(position, entities[j].Position, entities[j].EntityCircle.radius) == true)
                            {
                                isBehindCollider = true; break;
                            }
                        }

                        if (isBehindCollider == false)
                            closeEntities.Add(entities[j]);
                    }
                }

                entityTimer = 0;
            }
        }

        private int colliderTimer = 0;
        private void AddColliders(GameTime gt)
        {
            colliderTimer += gt.ElapsedGameTime.Milliseconds;

            if (colliderTimer >= 200)
            {
                closeColliders.Clear();
                for (int i = 0; i < colliders.Count; i++)
                {
                    if (colliders[i].InRange(position, effectRadius))
                        closeColliders.Add(colliders[i]);
                }

                colliderTimer = 0;
            }
        }

        protected void ChangeRadius(float value)
        {
            effectCircle.radius += value;
            effectCircle.radius = MathHelper.Clamp(effectCircle.radius, minRadius, maxRadius);
        }
        protected void AssignRadius(float value)
        {
            effectCircle.radius = MathHelper.Clamp(value, minRadius, maxRadius);
        }

        protected void ChangePosition(float x, float y)
        {
            positionOffset.X += x;
            positionOffset.Y += y;
        }
        protected void AssignPosition(float x, float y)
        {
            positionOffset.X = x;
            positionOffset.Y = y;
        }

        protected virtual void ApplyEntityDamage(BaseEntity entity)
        {

        }

        protected bool IsEntityHit(BaseEntity entity)
        {
            return effectCircle.Intersects(entity.EntityCircle);
        }

        public virtual BaseAOE Copy()
        {
            BaseAOE area = (BaseAOE)this.MemberwiseClone();

            area.effectCircle = new Circle();

            return area;
        }
    }
}
