using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.TileEngine;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.TileEngine.Objects.Colliders;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Physics.Swooshes;
using Pilgrimage_Of_Embers.Physics.Swooshes.Types;
using Pilgrimage_Of_Embers.CombatEngine.Types;

namespace Pilgrimage_Of_Embers.CombatEngine.Projectiles
{
    public abstract class BaseProjectile
    {
        int id; string name;
        public int ID { get { return id; } }
        public string Name { get { return name; } }
        protected Texture2D projTexture;
        public Texture2D ProjectileTexture { get { return projTexture; } }

        protected Vector2 position;
        protected Line projectileLine;
        protected float rotation, baseSpeed, speedMultiplier = 1f, damageMultiplier = 1f, length = 16f, heightDistance = 32; //heightDistance = the distance from the projectile and it's shadow. As this decreases, projectile's y is increased.
        protected float gravity, angleOffset = .78f;
        private uint baseDamage, weaponDamage;
        protected Vector2 refPosition;

        protected Random random;

        public Vector2 Position { get { return position; } }

        public float BaseSpeed { get { return baseSpeed; } }
        public float TotalSpeed { get { return (baseSpeed) * speedMultiplier; } }

        public uint BaseDamage { get { return baseDamage; } }
        public uint TotalDamage { get { return (uint)MathHelper.Clamp((baseDamage + weaponDamage) * damageMultiplier, 0, 50000); } }

        public enum ProjectileState
        {
            Initialize,
            Update,
            Terminate,
            Deactivated,
        }
        protected ProjectileState state = ProjectileState.Initialize;
        public ProjectileState State { get { return state; } }

        protected List<BaseEntity> entities;
        public List<BaseEntity> Entities { set { entities = value; } }
        protected List<BaseEntity> closeEntities = new List<BaseEntity>();

        protected List<BaseCollider> colliders;
        public List<BaseCollider> Colliders { set { colliders = value; } }
        protected List<BaseCollider> closeColliders = new List<BaseCollider>();

        protected TileMap tileMap;
        public TileMap Map { set { tileMap = value; } }

        protected Camera camera;
        public Camera Camera { set { camera = value; } }

        protected string thrownBy;
        public string ThrownBy { get { return thrownBy; } set { thrownBy = value; } }
        protected bool canHarmSelf = false;

        protected const string directory = "Combat/Projectiles/";

        private DepthFloor floor = new DepthFloor();
        protected float Depth { get { return floor.Depth; } }
        protected int CurrentFloor { get { return floor.CurrentFloor; } set { floor.CurrentFloor = value; } }

        protected BaseEntity currentEntity;

        protected BaseSwoosh swoosh;

        public BaseProjectile(int ID, string Name, float Length, float BaseSpeed, uint BaseDamage)
        {
            id = ID;
            name = Name;
            length = Length;
            baseSpeed = BaseSpeed;
            baseDamage = BaseDamage;

            swoosh = new AirSwoosh(Vector2.Zero, Vector2.Zero, 5, 0, 20f);
        }

        public virtual void Load(ContentManager cm)
        {
            swoosh.Load(cm);
        }

        protected Vector2 direction; private int swooshAssigned = 0;
        public virtual void Update(GameTime gt)
        {
            heightDistance = MathHelper.Clamp(heightDistance, 0f, 3000f);
            baseSpeed = MathHelper.Clamp(baseSpeed, 0f, 5000f);

            if (state == ProjectileState.Initialize)
            {
                Initialize();
                state = ProjectileState.Update;
            }

            if (state == ProjectileState.Update)
            {
                MoveProjectile(gt);
                UpdateBehavior(gt);

                if (IntersectsCollider())
                    OnColliderHit(gt);
                if (IntersectsEntity())
                    OnEntityHit(gt);
                if (IntersectsShield())
                    OnShieldHit(gt);
                if (heightDistance <= 2)
                    OnGroundHit(gt);

                floor.UpdateDepth(camera.WorldToScreen(position).Y);
            }

            if (swooshAssigned <= 3)
            {
                swoosh.ForceAllJointPosition(position);
                swooshAssigned++;
            }

            if (state == ProjectileState.Terminate)
            {
                Terminate(gt);
                floor.UpdateDepth(camera.WorldToScreen(position).Y - GameSettings.VectorCenter.Y);
            }

            AddObjects(gt);
        }

        protected virtual void Initialize()
        {
        }
        protected abstract void UpdateBehavior(GameTime gt);
        /// <summary>
        /// Do not forget to set state to "Deactivate". This is the end state. Terminate state is used for sticking the arrow in the ground, finalizing a magic spell's animation, etc.
        /// </summary>
        protected abstract void Terminate(GameTime gt);

        public abstract void Draw(SpriteBatch sb);

        public abstract void OnEntityHit(GameTime gt);
        public abstract void OnShieldHit(GameTime gt);
        public abstract void OnColliderHit(GameTime gt);
        public abstract void OnGroundHit(GameTime gt);

        int time = 160;
        private void AddObjects(GameTime gt)
        {
            time += gt.ElapsedGameTime.Milliseconds;

            if (time >= 150)
            {
                closeEntities.Clear();
                for (int i = 0; i < entities.Count; i++)
                {
                    if (DistanceTo(entities[i].Position) <= 400)
                        closeEntities.Add(entities[i]);
                }

                closeColliders.Clear();
                for (int c = 0; c < colliders.Count; c++)
                {
                    if (colliders[c].InRange(position, 200f))
                        closeColliders.Add(colliders[c]);
                }

                time = 0;
            }
        }

        private void MoveProjectile(GameTime gt)
        {
            direction = new Vector2((float)Math.Cos(rotation),
                                    (float)Math.Sin(rotation));

            if (direction != Vector2.Zero)
                direction.Normalize();

            float spd = TotalSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
            position += (direction * TotalSpeed) * (float)gt.ElapsedGameTime.TotalSeconds;

            projectileLine.locationA = position;
            projectileLine.locationB = position + (direction * length);

            refPosition = projectileLine.locationB;
        }
        public void SetReferences(Camera camera, TileMap map, List<BaseCollider> colliders, List<BaseEntity> entities, string thrower)
        {
            this.camera = camera;
            this.tileMap = map;
            this.colliders = colliders.ToList();
            this.entities = entities.ToList();
            this.thrownBy = thrower;

            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].MapEntityID.ToUpper() == thrownBy.ToUpper())
                {
                    currentEntity = entities[i];
                }
            }
        }
        public void SetValues(Vector2 position, float angle, int floor, float speedMultiplier, float damageMultiplier, int weaponDamage)
        {
            this.position = position;
            swoosh.ForceAllJointPosition(position);
            rotation = angle;
            CurrentFloor = floor;
            this.speedMultiplier = speedMultiplier;
            this.damageMultiplier = damageMultiplier;
            this.weaponDamage = (uint)weaponDamage;
        }

        protected BaseDeflector intersectedShield;

        protected bool IntersectsEntity()
        {
            for (int i = 0; i < closeEntities.Count; i++)
            {
                if (canHarmSelf == true)
                {
                    if (projectileLine.Intersects(closeEntities[i].Hitbox))
                    {
                        hitEntity = closeEntities[i];
                        return true;
                    }
                }
                else
                {
                    if (closeEntities[i].MapEntityID.ToUpper() != thrownBy.ToUpper()) //Skip over the entity who threw the projectile
                    {
                        if (projectileLine.Intersects(closeEntities[i].Hitbox))
                        {
                            hitEntity = closeEntities[i];
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        protected bool IntersectsShield()
        {
            for (int i = 0; i < closeEntities.Count; i++)
            {
                if (closeEntities[i].MapEntityID.ToUpper() != thrownBy.ToUpper()) //Skip over the entity who threw the projectile
                {
                    //if (projectileLine.Intersects(closeEntities[i].EntityCircle)) -- why is this here?
                    //    return true;

                    BaseDeflector primary = null, offhand = null;

                    if (closeEntities[i].EQUIPMENT_PrimaryCombat() != null && closeEntities[i].EQUIPMENT_PrimaryCombat() is BaseDeflector)
                        primary = (BaseDeflector)closeEntities[i].EQUIPMENT_PrimaryCombat();
                    if (closeEntities[i].EQUIPMENT_OffhandCombat() != null && closeEntities[i].EQUIPMENT_OffhandCombat() is BaseDeflector)
                        offhand = (BaseDeflector)closeEntities[i].EQUIPMENT_OffhandCombat();

                    if (primary != null && primary.IsDeflected(projectileLine))
                    {
                        if (primary.CurrentAction != CombatMove.None)
                        {
                            intersectedShield = primary;
                            return true;
                        }
                    }

                    if (offhand != null && offhand.IsDeflected(projectileLine))
                    {
                        if (offhand.CurrentAction != CombatMove.None)
                        {
                            intersectedShield = offhand;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        protected bool IntersectsCollider()
        {
            for (int i = 0; i < closeColliders.Count; i++)
            {
                if (closeColliders[i].Contains(projectileLine.locationB))
                    return true;
            }
            return false;
        }
        public bool IsOnScreen() { return camera.IsOnScreen(position); }

        protected BaseEntity hitEntity = null;
        [Obsolete("This has been replaced by a more efficient variable. Please use \"hitEntity\".", true)]
        protected BaseEntity EntityHit()
        {
            for (int i = 0; i < closeEntities.Count; i++)
            {
                if (projectileLine.Intersects(closeEntities[i].Hitbox))
                    return closeEntities[i];
            }
            return null;
        }

        protected bool IntersectsBlocker() //For physical projectiles, mostly
        {
            return false;
        }
        protected bool IntersectsStriker() //For the physical projectiles that can be deflected by strikers. Mostly darts and small thrown weapons. 
        {
            return false;
        }

        protected float DistanceTo(Vector2 position)
        {
            return Vector2.Distance(this.position, position);
        }

        public virtual void ForceRemoveProjectile()
        {

        }

        public override string ToString()
        {
            return "[" + this.GetType().Name + " of " + this.GetType().BaseType.Name + "]";
        }

        public virtual BaseProjectile Copy(TileMap map)
        {
            BaseProjectile projectile = (BaseProjectile)this.MemberwiseClone();
            projectile.projectileLine = new Line(Vector2.Zero, Vector2.Zero);
            projectile.floor = new DepthFloor();
            projectile.closeEntities = new List<BaseEntity>();
            projectile.closeColliders = new List<BaseCollider>();

            projectile.random = new Random(Guid.NewGuid().GetHashCode());

            projectile.swoosh = swoosh.Copy();

            return projectile;
        }
    }
}
