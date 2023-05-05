using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrimage_Of_Embers.CombatEngine.Projectiles.Types.Physical
{
    public class StatusProjectile : PhysicalProjectile
    {
        int statusID;

        public StatusProjectile(int ID, string Name, float ProjectileLength, Texture2D ProjectileIcon, float BaseSpeed, uint BaseDamage, int ItemID, int StatusID)
            : base(ID, Name, ProjectileLength, ProjectileIcon, BaseSpeed, BaseDamage, ItemID)
        {
            statusID = StatusID;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void UpdateBehavior(GameTime gt)
        {
            base.UpdateBehavior(gt);
        }
        protected override void Terminate(GameTime gt)
        {
            state = ProjectileState.Deactivated; //Remove projectile
            base.Terminate(gt);
        }
        public override void OnEntityHit(GameTime gt)
        {
            if (hitEntity != null)
                hitEntity.STATUS_AddStatus(statusID, thrownBy);

            base.OnEntityHit(gt);
        }

        public override void OnColliderHit(GameTime gt)
        {
            base.OnColliderHit(gt);
        }

        public override void ForceRemoveProjectile()
        {

            base.ForceRemoveProjectile();
        }
    }
}
