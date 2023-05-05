using System.Collections.Generic;
using System.Linq;
using Pilgrimage_Of_Embers.Entities.Status_Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities
{
    public class EntityStatus //Will be used for things such as poison, fire damage, regeneration, etc. (positive or negative)
    {
        Texture2D rect;

        public List<BaseStatus> statuses = new List<BaseStatus>();

        public EntityStatus(BaseEntity Entity)
        {
            targetEntity = Entity;
        }

        BaseEntity targetEntity;

        public void AddStatus(int ID)
        {
            for (int j = 0; j < statuses.Count; j++)
            {
                if (statuses[j].ID == ID)
                {
                    if (statuses[j].IsResetTimer == true)
                        statuses[j].ResetTimer();
                    if (statuses[j].IsIncreaseEffect == true)
                        statuses[j].IncreaseEffect();
                }
            }

            bool contains = statuses.Any(x => x.ID == ID);

            for (int i = 0; i < StatusDatabase.Statuses.Count; i++)
            {
                if (StatusDatabase.Statuses[i].ID == ID && contains == false)
                {
                    statuses.Add(StatusDatabase.Statuses[i].Copy(targetEntity));
                }
            }
        }
        public void RemoveStatus(int ID)
        {
            for (int i = 0; i < statuses.Count; i++)
            {
                if (statuses[i].ID == ID)
                {
                    statuses[i].ForceStop();
                }
            }
        }
        public void RemoveAll(bool isSoulgate)
        {
            for (int i = 0; i < statuses.Count; i++)
            {
                if (isSoulgate == true)
                {
                    if (statuses[i].IsRestCure == true)
                    {
                        statuses[i].ForceStop();
                        statuses.Remove(statuses[i]);
                    }
                }
                else
                {
                    statuses[i].ForceStop();
                    statuses.Remove(statuses[i]);
                }
            }
        }
        public void ForceClear()
        {
            statuses.Clear();
        }

        public bool CheckForStatus(int id)
        {
            for (int i = 0; i < statuses.Count; i++)
            {
                if (statuses[i].ID == id)
                {
                    return true;
                }
            }

            return false;
        }

        public int SecondsLeft(int ID)
        {
            for (int i = 0; i < statuses.Count; i++)
            {
                if (statuses[i].ID == ID)
                {
                    return statuses[i].TimeLeft;
                }
            }
            return -1;
        }

        public void LoadContent(ContentManager cm)
        {
            rect = cm.Load<Texture2D>("rect");
        }

        public void UpdateRow(Vector2 Offset)
        {
            int indexCount = 0;
            for (int i = 0; i < statuses.Count; i++)
            {
                if (statuses[i].IsInvisible == false)
                {
                    statuses[i].UpdateRect(indexCount, Offset, 32);
                    indexCount++;
                }
            }
        }

        public void Update(GameTime gt)
        {
            for (int i = 0; i < statuses.Count; i++)
            {
                statuses[i].Update(gt);

                if (statuses[i].IsActive == false)
                    statuses.RemoveAt(i);
            }
        }

        Color inside = Color.Lerp(new Color(.2f, .2f, .2f, 1f), Color.Transparent, .5f), outside = Color.Lerp(Color.Silver, Color.Transparent, .15f);

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < (int)MathHelper.Clamp(statuses.Count, 0, 6); i++)
            {
                if (statuses[i].IsInvisible == false)
                {
                    SpriteBatchHelper.DrawBoxBordered(sb, rect, new Rectangle(statuses[i].IconRect.X + 2, statuses[i].IconRect.Y + 2, statuses[i].IconRect.Width - 4, statuses[i].IconRect.Height), inside, outside);
                    sb.Draw(statuses[i].Icon, new Rectangle(statuses[i].IconRect.X - 1, statuses[i].IconRect.Y, statuses[i].IconRect.Width, statuses[i].IconRect.Height), Color.White);
                
                    if (statuses[i].IsInfinite == false) //don't display countdown meter when time is infinite
                        SpriteBatchHelper.DrawBoxBordered(sb, rect, new Rectangle(statuses[i].IconRect.X + 4, statuses[i].IconRect.Y + statuses[i].IconRect.Height, statuses[i].CountdownBar, 1), Color.White, Color.Black, 1);
                }
            }
        }
    }
}
