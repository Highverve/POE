using Microsoft.Xna.Framework;
using System;

namespace Pilgrimage_Of_Embers.Entities.Classes
{
    public class FlameRenown
    {
        private float renown;
        public float Renown { get { return renown; } set { renown = value; } }
        
        public FlameRenown(float startingRenown = 0)
        {
            renown = startingRenown;
        }

        public float RenownMultiplier()
        {
            return 1 - Math.Abs((int)renown / 120);
        }
        public void AdjustRenown(float value)
        {
            Renown += value * RenownMultiplier();
        }

        private FlameRenownName renownName;
        public FlameRenownName RenownEnumName { get { VerifyName(); return renownName; } }
        public string RenownName() { return RenownEnumName.ToString() + " Flame"; }

        public enum FlameRenownName
        {
            Divine,
            Radiant,
            Shining,
            Glimmering,

            New,

            Faint,
            Gloaming,
            Cold,
            Dark,
        }
        private void VerifyName()
        {//0-20-50-90-100
            if (renown > 0 && renown < 20)
                renownName = FlameRenownName.Glimmering;
            if (renown >= 20 && renown < 50)
                renownName = FlameRenownName.Shining;
            if (renown > 50 && renown <= 90)
                renownName = FlameRenownName.Radiant;
            if (renown > 90)
                renownName = FlameRenownName.Divine;

            if (renown < 0 && renown > -20)
                renownName = FlameRenownName.Faint;
            if (renown <= -20 && renown > -50)
                renownName = FlameRenownName.Gloaming;
            if (renown < -50 && renown >= -90)
                renownName = FlameRenownName.Cold;
            if (renown < -95)
                renownName = FlameRenownName.Dark;

            if (renown == 0)
                renownName = FlameRenownName.New;
        }
    }
}
