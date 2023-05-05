using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers.Entities
{
    /// <summary>
    /// For display simple entity info above said entity. Do not use for bosses.
    /// </summary>
    public class EntityInfo
    {
        //Only draw health bar above entity!

        string entityName, entityID;
        int currentHealth, maxHealth, currentCap, barWidth, currentHPBar, currentCapBar;

        bool displayInfo = true;
        float opacity;

        public int CurrentHealth { set { currentHealth = value; } } //Set to skillset.health.currentHP
        public int CurrentCap { set { currentCap = value; } } //Set to skillset.health.currentCap
        public float Opacity { set { opacity = MathHelper.Clamp(value, 0f, 1f); } } //This should equate to distance from mouse to entity.
        public bool DisplayInfo { set { displayInfo = value; } } //If entity has not spotted player/controlled entity, set to false.
        public string EntityID { set { entityID = value; } }
        public string EntityName { get { return entityName; } }

        Texture2D goldBar, barFiller;
        Color color, barColor, textColor;
        public Color TextColor { get { return textColor; } set { textColor = value; } }

        Vector2 origin;

        float offset;
        public float Offset { set { offset = value; } }

        SpriteFont font;

        public EntityInfo(string Name, int MaxHealth)
        {
            entityName = Name;
            maxHealth = MaxHealth;

            textColor = new Color(.95f, .55f, .25f, .95f);
        }

        public void Load(ContentManager cm)
        {
            goldBar = cm.Load<Texture2D>("Interface/Entities/Meters/entityGoldBar");
            barFiller = cm.Load<Texture2D>("Interface/Entities/Meters/entityBarBackground");
            barWidth = goldBar.Width;

            font = cm.Load<SpriteFont>("Fonts/boldOutlined");

            origin = new Vector2(barWidth / 2, 0);
        }
        public void Unload()
        {
            goldBar.Dispose();
            barFiller.Dispose();
        }

        public void Draw(SpriteBatch sb, Vector2 position)
        {
            if (displayInfo == true)
            {
                CalculateBar();

                sb.Draw(barFiller, position - new Vector2(0, offset), new Rectangle(0, 0, currentHPBar, barFiller.Height), barColor, 0f, origin, 1f, SpriteEffects.None, .95f);
                sb.Draw(goldBar, position - new Vector2(0, offset), new Rectangle(0, 0, goldBar.Width, goldBar.Height), color, 0f, origin, 1f, SpriteEffects.None, .96f);

                SpriteBatchHelper.DrawString(sb, font, entityName, position - new Vector2(0, offset) + new Vector2(0, -12), new Vector2(font.MeasureString(entityName).X / 2, 0),
                                             Color.Lerp(textColor, Color.Transparent, opacity), 1f, .97f);

                if (GameSettings.IsDebugging == true)
                    SpriteBatchHelper.DrawString(sb, font, entityID, position - new Vector2(0, offset) + new Vector2(0, 100), new Vector2(font.MeasureString(entityID).X / 2, 0), Color.Gray, 1f, 1f);

                //SpriteBatchHelper.DrawStringBordered(sb, font, currentHealth.ToString() + @"/" + maxHealth.ToString(), position + offset + new Vector2(0, -30), Color.White, Color.Black);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intensity">Between -3 and 3.</param>
        public void CalculateTextColor(int intensity)
        {
            intensity = (int)MathHelper.Clamp(intensity, -3, 3);

            //Greens
            if (intensity == 1)
                textColor = new Color(149, 229, 162, 255);
            if (intensity == 2)
                textColor = new Color(76, 153, 89, 255);
            if (intensity == 3)
                textColor = new Color(28, 114, 43, 255);

            //Reds
            if (intensity == -1)
                textColor = Color.Lerp(Color.Beige, Color.Yellow, .35f);
            if (intensity == -2)
                textColor = Color.Lerp(Color.Orange, Color.White, .25f);
            if (intensity == -3)
                textColor = Color.Lerp(Color.DarkRed, Color.White, .25f);
        }
        private void CalculateBar()
        {
            color = Color.Lerp(Color.White, Color.Transparent, opacity);
            barColor = Color.Lerp(new Color(142, 60, 53, 255), Color.Transparent, opacity);
            currentHPBar = barWidth * currentHealth / maxHealth;
        }
        public void SetOpacity(float currentDistance, float maxDistance)
        {
            if (currentDistance < maxDistance)
                opacity = 0f;
            else
                opacity = 1f; //(currentDistance / maxDistance);
        }

        public EntityInfo Copy()
        {
            EntityInfo copy = (EntityInfo)MemberwiseClone();

            return copy;
        }
    }
}
