using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.ScreenEngine.Souls.Types
{
    public class TestSoul : BaseSoul
    {
        public TestSoul(int ID, string Name, string Description, Texture2D Icon, Texture2D LargeIcon, uint EffectTime, uint Cooldown, uint SoulCharges)
            : base(ID, Name, Description, Icon, LargeIcon, Cooldown, EffectTime, SoulCharges)
        {

        }

        private int time = 0;
        protected override void UpdateBehavior(GameTime gt)
        {
            if (CurrentState == SoulState.Effect)
            {
                time += gt.ElapsedGameTime.Milliseconds;

                if (time >= 100)
                {
                    currentEntity.HEALTH_Restore((uint)soulLevel);
                    currentEntity.CAPTION_SendImmediate("Feeling better now.");
                    time = 0;
                }
            }

            base.UpdateBehavior(gt);
        }

        protected override void ApplyOnce(GameTime gt)
        {
        }
        protected override void ApplyConstant(GameTime gt)
        {
        }
    }
}
