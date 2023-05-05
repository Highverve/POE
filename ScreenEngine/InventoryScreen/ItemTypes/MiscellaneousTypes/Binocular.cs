using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.MiscellaneousTypes
{
    public class Binocular : Miscellaneous
    {
        public bool IsLooking { get; set; }
        public bool IsMovementLocked { get; set; }
        public float Distance { get; set; }
        private float currentDistance, lerp;

        public Binocular(Texture2D Icon, int ID, string Name, string Description, int MaxQuantity, int MaxDurability, bool IsDisposable, Requirements ItemRequirements, int SellPrice, string Type, string Subtype)
            : base(Icon, ID, Name, Description, MaxQuantity, MaxDurability, IsDisposable, ItemRequirements, SellPrice, Type, Subtype)
        {
        }

        public override void Load(ContentManager main, ContentManager map)
        {
            IsRestRepairsBroken = true;
            IsRestRepairsUnbroken = true;
            base.Load(main, map);
        }

        public override void RefreshAttributeText()
        {
            AttributeText = string.Empty;
            AttributeText += "Hold the \"Sneak\" key.";
        }

        public override void UpdateItem(GameTime gt)
        {
            if (IsLooking == true)
            {
                if (Distance != 0)
                {
                    lerp += 5f * (float)gt.ElapsedGameTime.TotalSeconds;
                    lerp = MathHelper.Clamp(lerp, 0f, 1f);
                    currentDistance = MathHelper.Lerp(0f, Distance, lerp);

                    Vector2 dir = currentEntity.SENSES_SightDirection.ToVector2();
                    if (dir != Vector2.Zero)
                        dir.Normalize();

                    camera.LookAt(currentEntity.Position + (dir * currentDistance));
                }

                if (currentEntity.STATE_IsSneaking() == false)
                {
                    camera.SetCameraState(Camera.CameraState.Current);
                    camera.SmoothZoom(1f, 1f, true, 0);

                    lerp = 0f;
                    IsLooking = false;
                    IsMovementLocked = false;
                }

                if (IsMovementLocked == true)
                {
                    currentEntity.SUSPEND_Movement(10);
                    //currentEntity.SUSPEND_Action(10);
                }
            }

            base.UpdateItem(gt);
        }
    }
}
