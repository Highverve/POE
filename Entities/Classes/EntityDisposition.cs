using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.Classes
{
    public class EntityDisposition
    {
        private int disposition;
        public int Disposition { get { return disposition; } }

        private BaseEntity entity;
        public BaseEntity Entity { get { return entity; } }

        public EntityDisposition(BaseEntity Entity)
        {
            entity = Entity;
            disposition = 0;
        }

        public void AdjustDisposition(int amount)
        {
            disposition += amount;
            disposition = (int)MathHelper.Clamp(disposition, -100, 100);
        }
        public void SetDisposition(int amount)
        {
            disposition = amount;
            disposition = (int)MathHelper.Clamp(disposition, -100, 100);
        }
    }
}
