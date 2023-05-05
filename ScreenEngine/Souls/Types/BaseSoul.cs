using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.TileEngine;

namespace Pilgrimage_Of_Embers.ScreenEngine.Souls.Types
{
    public class BaseSoul
    {
        private int id;
        public int ID { get { return id; } }

        private string name, description;
        public string Name { get { return name; } }
        public string Description { get { return description; } }

        private Texture2D icon;
        public Texture2D Icon { get { return icon; } }

        private Texture2D largeIcon;
        public Texture2D LargeIcon { get { return largeIcon; } }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public bool IsDisplay { get; set; }

        private bool isUnlimited = false;
        public bool IsUnlimited { get { return isUnlimited; } }

        protected uint soulCharges = 0; //Use for soul charge
        protected uint maxCharges;

        public uint SoulCharges { get { return soulCharges; } set { soulCharges = (uint)MathHelper.Clamp(value, 0, MaxCharges); } }
        public uint MaxCharges { get { return maxCharges; } }

        protected int effectTime, cooldown, delay = 0; //The delay between uses

        protected int Cooldown { get { return cooldown; } set { cooldown = (int)MathHelper.Clamp(value, 500, 300000); } } //Maximum value is 300,000ms = 5 min.
        public int CooldownTime { get { return cooldown; } }
        public int EffectTime { get { return effectTime; } }
        public int DelayTime { get { return delay; } }

        protected int soulLevel = 1;
        public int SoulLevel { get { return soulLevel; } set { soulLevel = (int)MathHelper.Clamp(value, 1, 10); } }

        public bool CanLevelUp { get { return soulLevel < 10; } }

        protected bool canActivate = true;
        public enum SoulState { None = 0, Effect = 1, Cooldown = 2 }
        SoulState currentState = SoulState.None;
        public SoulState CurrentState { get { return currentState; } }

        public bool IsSelected { get; set; }

        private bool isNew = true;
        public bool IsNew { get { return isNew; } set { isNew = value; } }

        protected TileMap tileMap;
        protected BaseEntity currentEntity;
        protected ScreenManager screens;

        public BaseSoul(int ID, string Name, string Description, Texture2D Icon, Texture2D LargeIcon, uint Cooldown, uint EffectTime, uint SoulCharges)
        {
            id = ID;
            name = Name;
            description = Description;
            icon = Icon;
            largeIcon = LargeIcon;

            effectTime = (int)EffectTime;
            this.Cooldown = (int)Cooldown;

            maxCharges = SoulCharges;
            soulCharges = SoulCharges;

            if (SoulCharges == 0)
                isUnlimited = true;
        }

        public void Update(GameTime gt)
        {
            UpdateSoulState(gt);
            UpdateBehavior(gt);
        }
        protected virtual void UpdateSoulState(GameTime gt)
        {
            if (isUnlimited == false)
            {
                if (soulCharges > 0)
                    canActivate = true;
                else
                    canActivate = false;
            }
            if (currentState == SoulState.Effect)
            {
                delay += gt.ElapsedGameTime.Milliseconds;

                if (delay >= effectTime)
                {
                    currentState = SoulState.Cooldown;
                    delay = 0;
                }
            }
            else if (currentState == SoulState.Cooldown)
            {
                delay += gt.ElapsedGameTime.Milliseconds;

                if (delay >= cooldown)
                {
                    currentState = SoulState.None;
                    delay = 0;
                }
            }
        }
        protected virtual void UpdateBehavior(GameTime gt)
        {
        }

        protected virtual void ApplyOnce(GameTime gt) { }
        protected virtual void ApplyConstant(GameTime gt) { }

        public void ResetDelay()
        {

        }
        public void ForceDelay()
        {

        }
        public void ForceDelay(int delayTime)
        {

        }
        public void ForceCharge()
        {
            soulCharges = maxCharges;
        }
        public void ForceDepleteCharge()
        {
            soulCharges = 0;
        }

        public void ActivateSoul()
        {
            if (canActivate == true)
            {
                if (currentState == SoulState.None)
                {
                    currentState = SoulState.Effect;

                    if (isUnlimited == false)
                        SoulCharges--;
                }
            }
        }
        
        public virtual BaseSoul DeepCopy(TileMap tileMap, BaseEntity currentEntity, ScreenManager screens)
        {
            BaseSoul soul = (BaseSoul)this.MemberwiseClone();

            soul.tileMap = tileMap;
            soul.currentEntity = currentEntity;
            soul.screens = screens;

            return soul;
        }

        protected const string space = " ";
        public virtual string SaveData()
        {
            return "Soul " + id.ToString() + space +
                   (int)currentState + space +
                   soulLevel.ToString() + space +
                   soulCharges.ToString() + space +
                   delay.ToString() + space +
                   isNew.ToString();
        }
        public virtual void LoadData(string data)
        {
            string[] words = data.Split(' ');

            currentState = (SoulState)int.Parse(words[2]);
            SoulLevel = int.Parse(words[3]);
            SoulCharges = uint.Parse(words[4]);
            delay = int.Parse(words[5]);
            isNew = bool.Parse(words[6]);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            BaseSoul soul = (BaseSoul)obj;
            return (this.id == soul.id);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
