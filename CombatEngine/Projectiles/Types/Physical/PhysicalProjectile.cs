using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.LightEngine;
using Pilgrimage_Of_Embers.ParticleEngine;
using Pilgrimage_Of_Embers.TileEngine;

namespace Pilgrimage_Of_Embers.CombatEngine.Projectiles.Types.Physical
{
    public class PhysicalProjectile : BaseProjectile
    {
        /*
        ----------
         To-Do
        ----------

            Projectiles properly reflect when hitting a shield

        */


        int itemID = -1;
        protected BaseEmitter particles;
        protected BaseLight light;

        public PhysicalProjectile(int ID, string Name, float ProjectileLength, Texture2D ProjectileTexture, float BaseSpeed, uint BaseDamage, int ItemID)
            : base(ID, Name, ProjectileLength, BaseSpeed, BaseDamage)
        {
            canHarmSelf = false;

            projTexture = ProjectileTexture;

            itemID = ItemID;
        }

        public override void Load(ContentManager cm)
        {
            heightDistance = 70f;
            gravity = 0f;

            base.Load(cm);
        }

        protected override void Initialize()
        {
            if (particles != null)
                tileMap.AddEmitter(particles);
            if (light != null)
            {
                light.TileLocation = Point.Zero;
                tileMap.AddLight(light);
            }
        }

        protected override void UpdateBehavior(GameTime gt)
        {
            gravity += 80f * (float)gt.ElapsedGameTime.TotalSeconds;
            heightDistance -= gravity * (float)gt.ElapsedGameTime.TotalSeconds;

            swoosh.Update(gt);
            swoosh.SetVariables(projectileLine.locationB, Depth - .0001f);

            if (particles != null)
                particles.Offset = projectileLine.locationB;
            if (light != null)
                light.Offset = projectileLine.locationB;
        }
        protected override void Terminate(GameTime gt)
        {
            if (itemID != -1)
            {
                if (projectileLine.Intersects(currentEntity.JumpingCircle))
                {
                    currentEntity.STORAGE_AddItem(itemID, 1, false, currentEntity.IsPlayerControlled); //If the player is controlling entity, add notification.
                    state = ProjectileState.Deactivated; //Remove projectile

                    if (particles != null)
                        particles.IsActivated = false;
                    if (light != null)
                        light.IsActivated = false;
                }
            }
        }

        private Texture2D pixel; private bool isAssigned = false;
        public override void Draw(SpriteBatch sb)
        {
            if (isAssigned == false)
            {
                pixel = new Texture2D(sb.GraphicsDevice, 1, 1);
                Color[] data = new Color[1] { Color.White }; //Pixel creating, the hard way.
                pixel.SetData(data);
                isAssigned = true;
            }

            sb.Draw(projTexture, projectileLine.locationA, Color.White, new Vector2(0, 64), rotation + angleOffset, 1f, Depth);

            if (state == ProjectileState.Update)
            {
                swoosh.Draw(sb);
                sb.Draw(projTexture, new Vector2(position.X, position.Y + (heightDistance)), WorldLight.ShadowColor, new Vector2(0, 64), rotation + angleOffset, 1f, Depth - .00001f);
            }

            if (GameSettings.IsDebugging == true)
                projectileLine.DrawLine(sb, pixel, Color.Black, 1f, 2);
        }

        public override void OnEntityHit(GameTime gt)
        {
            if (isShieldHit == false && hitEntity != null)
            {
                hitEntity.COMBAT_DamageEntity("[" + thrownBy + "]PhysicalProjectile", TotalDamage, 100, () => { }, "ProjectileDefense");

                if (itemID != -1)
                    hitEntity.LOOT_AddItem(itemID, 1, 100f);

                if (particles != null)
                    particles.IsActivated = false;
                if (light != null)
                    light.IsActivated = false;

                state = ProjectileState.Deactivated;
            }
        }

        private bool isInvertRotation = false, isShieldHit = false;
        public override void OnShieldHit(GameTime gt)
        {
            if (isInvertRotation == false)
            {
                if (intersectedShield != null)
                {
                    rotation = intersectedShield.shieldCross += random.NextFloat(-.5f, .5f);
                }

                gravity *= MathHelper.Clamp(30 * (heightDistance * .05f), 1f, 500f);
                speedMultiplier *= .75f;
                heightDistance *= .75f;

                isInvertRotation = true;
                isShieldHit = true;
            }
        }
        public override void OnColliderHit(GameTime gt)
        {
            heightDistance = 3;
            state = ProjectileState.Terminate;
        }
        public override void OnGroundHit(GameTime gt)
        {
            state = ProjectileState.Terminate;
        }

        public override BaseProjectile Copy(TileMap map)
        {
            PhysicalProjectile copy = (PhysicalProjectile)base.Copy(map);

            return copy;
        }
    }
}
