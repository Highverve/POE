using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.LightEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrimage_Of_Embers.CombatEngine.Types.Deflectors
{
    class VillagekeepersShield : BaseDeflector
    {
        public VillagekeepersShield()
            : base(CombatSlotType.OneHand, .4f, 20f)
        {

        }

        public override void Load(ContentManager main, ContentManager map)
        {
            shieldTexture = main.Load<Texture2D>("Combat/Deflectors/villagekeepersShield");
            baseScale = 2f;
            baseShadowOffset = 30f;

            SetClickBools(false, false, true, true, true, true, true, true);

            base.Load(main, map);
        }

        public override void Basic(GameTime gt)
        {
            isUpdateDirection = true;

            deflector.IsBlocking = true;
            deflectorSpike.IsBlocking = true;
        }
        public override void Sneak(GameTime gt)
        {
            //deflector.IsBlocking = true;
            //deflectorSpike.IsBlocking = true;

            if (isAdded == false)
            {
                AssignVariables(-1.5f, 1000, 0f, .3f);

                //Multi's
                AddAction(() => Adjust(gt, ref multiplier1, 4f), 0, 300);
                AddAction(() => Adjust(gt, ref multiplier1, -2f), 300, maxCombatTime);

                AddAction(() => { if (multiplier2 <= .8f) Adjust(gt, ref multiplier2, 5f); }, 300, maxCombatTime);
                AddAction(() => { if (multiplier2 > .8f && multiplier2 <= .9f) Adjust(gt, ref multiplier2, 2f); }, 300, maxCombatTime);
                AddAction(() => { if (multiplier2 > .9f) Adjust(gt, ref multiplier2, .25f); }, 300, maxCombatTime);
                AddAction(() => Adjust(gt, ref multiplier2, -1.5f), 0, 300);

                //Moving smoothly
                AddAction(() => { reach = MathHelper.SmoothStep(0f, 15f, multiplier1); }, 0, maxCombatTime);
                AddAction(() => { directionAngle = MathHelper.SmoothStep(startingAngle, startingAngle + (3.5f * dualWieldDirection), multiplier2); }, 0, maxCombatTime);

                //Animation
                AddAction(() => SetCurrentFrame(0, 1), 0, 50);
                AddAction(() => SetCurrentFrame(1, 1), 50, 100);
                AddAction(() => SetCurrentFrame(2, 1), 100, 150);
                AddAction(() => SetCurrentFrame(3, 1), 150, 200); //Glowing fully

                AddAction(() => SetCurrentFrame(2, 1), 650, 700);
                AddAction(() => SetCurrentFrame(1, 1), 700, 750);
                AddAction(() => SetCurrentFrame(0, 1), 750, 800);

                //Damage range
                AddAction(() =>
                {
                    deflectorSpike.IsActive = true;
                    deflector.IsActive = true;
                }, 450, 650);
                AddAction(() =>
                {
                    deflectorSpike.IsActive = false;
                    deflector.IsActive = false;
                }, 650, 700);

                isAdded = true;
            }
        }
        public override void Sprint(GameTime gt)
        {

            base.Basic(gt);
        }
        public override void Jump(GameTime gt)
        {
            isUpdateDirection = true;

            deflector.IsBlocking = true;
            deflectorSpike.IsBlocking = true;

            base.Jump(gt);
        }

        public override void DeflectOther(GameTime gt)
        {
            //sparks.IsActivated = true;
            //tileMap.AddEmitter(sparks);

            base.DeflectOther(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(shieldTexture, shieldCenter, new Rectangle(spriteDirection.X * frameSize.X, spriteDirection.Y * spriteDirection.Y, frameSize.X, frameSize.Y),
                    Color.White, 0f, new Vector2(frameSize.X / 2, frameSize.Y / 2), scale, SpriteEffects.None, finalDepth);

            sb.Draw(shieldTexture, CalculateShadow(shieldCenter), new Rectangle(spriteDirection.X * frameSize.X, spriteDirection.Y * spriteDirection.Y, frameSize.X, frameSize.Y),
                    WorldLight.ShadowColor, 0f, new Vector2(frameSize.X / 2, frameSize.Y / 2), scale, SpriteEffects.FlipVertically, finalDepth - .001f);

            base.Draw(sb);
        }
    }
}
