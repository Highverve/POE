using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.LightEngine.Types;
using Pilgrimage_Of_Embers.ParticleEngine.ParticleTypes;

namespace Pilgrimage_Of_Embers.CombatEngine.Projectiles.Types.Physical.Arrows
{
    public class FireArrow : PhysicalProjectile
    {
        public FireArrow(int ID, string Name, float ProjectileLength, Texture2D ProjectileIcon, float BaseSpeed, uint BaseDamage, int ItemID)
            : base(ID, Name, ProjectileLength, ProjectileIcon, BaseSpeed, BaseDamage, ItemID)
        {

        }

        protected override void Initialize()
        {
            particles = new FlameThrower(-1, Point.Zero, Vector2.Zero, Vector2.Zero, 30f, CurrentFloor);//new HiddenItem(-1, Point.Zero, new Vector2(0, 0), 30f, CurrentFloor);
            light = new FlickerLight(-1, "World/Objects/Lights/lightMask3", Point.Zero, Vector2.Zero, new Color(255, 200, 155, 150), .3f, 0f, .025f);

            base.Initialize();
        }

        private int burnUpTime = 0;
        protected override void UpdateBehavior(GameTime gt)
        {
            light.BaseSize = (heightDistance * .0075f) + .3f;
            base.UpdateBehavior(gt);
        }
        protected override void Terminate(GameTime gt)
        {
            burnUpTime += gt.ElapsedGameTime.Milliseconds;

            if (burnUpTime > 90000) //90 seconds
            {
                state = ProjectileState.Deactivated; //Remove projectile

                if (particles != null)
                    particles.IsActivated = false; //Deactivate particles

                if (light != null)
                    light.IsActivated = false;
            }
            base.Terminate(gt);
        }
        public override void OnEntityHit(GameTime gt)
        {
            if (hitEntity != null)
                hitEntity.STATUS_AddStatus(2, thrownBy);

            base.OnEntityHit(gt);
        }

        public override void OnColliderHit(GameTime gt)
        {
            base.OnColliderHit(gt);

            light.BaseSize = (heightDistance * .0075f) + .3f;
        }

        public override void ForceRemoveProjectile()
        {
            light.IsActivated = false;
            particles.IsActivated = false;

            base.ForceRemoveProjectile();
        }
    }
}
