using Pilgrimage_Of_Embers.Entities.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pilgrimage_Of_Embers.CombatEngine.AOE.Types
{
    public class TestAOE : BaseAOE
    {

        public TestAOE(int ID, string Name)
            : base(ID, Name)
        {

            SetValues(600f, 25f, 400f, 500);
        }

        public override void Load(ContentManager cm)
        {
            base.Load(cm);
        }

        protected override void UpdateBehavior(GameTime gt)
        {
            ChangeRadius(50f * (float)gt.ElapsedGameTime.TotalSeconds);


            base.UpdateBehavior(gt);
        }

        public override void Draw(SpriteBatch sb)
        {

            base.Draw(sb);
        }
        public override void DrawDebug(SpriteBatch sb)
        {
            sb.Draw(pixel, new Rectangle((int)position.X, (int)position.Y, (int)effectCircle.radius, 1), Color.White);
            sb.Draw(pixel, new Rectangle((int)position.X, (int)position.Y, 1, -(int)effectCircle.radius), Color.White);

            base.DrawDebug(sb);
        }

        protected override void ApplyEntityDamage(BaseEntity entity)
        {
            entity.HEALTH_Damage(1, KillerID);
        }
    }
}
