using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers.Entities.Status_Effects
{
    public class BaseStatus
    {
        private int id, lifetime, startTime, timer, effectTimer = 0;
        private Texture2D icon;
        private string name, displayName;
        private int maxStack;
        private bool isActive, isInfinite, isInvisible, isRestCure;
        private Rectangle iconRect;
        private int countdownBar;
        protected int effectIterator = 1, durationIncrease = 1, effectTimeIntensity;

        public int ID { get { return id; } }
        public Texture2D Icon { get { return icon; } }
        public string Name { get { return name; } }
        public string DisplayName { get { return displayName; } }
        public string KillerID { get; set; }
        public Rectangle IconRect { get { return iconRect; } }

        public int CountdownBar { get { return countdownBar; } }
        public int TimeLeft { get { return timer; } }
        public int LifeTime { get { return lifetime; } }
        public int EffectTimeIntensity { get { return effectTimeIntensity; } }
        public float EffectTimeMultiplier { get; set; }

        public bool IsActive { get { return isActive; } }
        public bool IsInfinite { get { return isInfinite; } }
        public bool IsInvisible { get { return isInvisible; } }
        public bool IsRestCure { get { return isRestCure; } }

        public bool IsResetTimer { get; protected set; }
        public bool IsIncreaseEffect { get; protected set; }

        public enum StackType
        {
            IncreaseDuration,
            IncreaseEffect,
            IncreaseBoth,
            None
        }
        public StackType stackType = StackType.IncreaseDuration;

        protected BaseEntity targetEntity;

        public BaseStatus(int ID, Texture2D Icon, string Name, int Lifetime, int EffectTimeIntensity, bool IsStackingResetTimer, bool IsStackingAmplifyEffect, int MaxStack,
                          Action<BaseEntity, BaseStatus> ApplyEffect, Action<BaseEntity, BaseStatus> Terminate, bool IsInfinite, bool IsInvisible, bool IsRestCure)
        {
            id = ID;
            icon = Icon;
            name = Name;

            lifetime = Lifetime;
            startTime = Lifetime;

            effectTimeIntensity = EffectTimeIntensity;
            effectTimer = EffectTimeIntensity;

            IsResetTimer = IsStackingResetTimer;
            IsIncreaseEffect = IsStackingAmplifyEffect;
            maxStack = MathHelper.Clamp(MaxStack, 1, 10);

            ApplyCustomEffect = ApplyEffect;
            this.Terminate = Terminate;

            isInfinite = IsInfinite;
            isInvisible = IsInvisible;
            isRestCure = IsRestCure;

            isActive = true;

            EffectTimeMultiplier = 1f;
        }

        public void Load(ContentManager cm)
        {
        }

        public virtual void Update(GameTime gt)
        {
            if (isActive == true)
            {
                if (isInfinite == true)
                    timer = 0;
                else
                    IncreaseTimer(gt);

                UpdateEffectTimer(gt);
            }
        }
        private void UpdateEffectTimer(GameTime gt)
        {
            effectTimer += gt.ElapsedGameTime.Milliseconds;
            if (effectTimer >= effectTimeIntensity * EffectTimeMultiplier)
            {
                for (int i = 0; i < effectIterator; i++)
                {
                    if (ApplyCustomEffect != null)
                        ApplyCustomEffect.Invoke(targetEntity, this);
                }

                effectTimer = 0;
            }
        }
        public void IncreaseTimer(GameTime gt)
        {
            timer += gt.ElapsedGameTime.Milliseconds;

            if (timer >= lifetime)
            {
                if (Terminate != null)
                    Terminate(targetEntity, this);

                isActive = false;
            }
        }

        Controls controls = new Controls();
        public void UpdateRect(int LocX, Vector2 Offset, int Spacing)
        {
            iconRect = new Rectangle(LocX * Spacing + (int)Offset.X, (int)Offset.Y, icon.Width, icon.Height);
         
            displayName = name + " (Multiplier x" + effectIterator + ", Lifetime x" + durationIncrease + ")";
            countdownBar = (iconRect.Width - 7) * timer / lifetime; // HP = hp.Width * TargetHandler.player.Health.CurrentHP / TargetHandler.player.Health.BaseHP;

            controls.UpdateCurrent();
            if (iconRect.Contains(controls.MousePosition))
                ToolTip.RequestStringAssign(displayName);
        }

        public void ResetTimer()
        {
            timer = 0;
        }
        /*public void IncreaseDuration()
        {
            if (effectIterator < maxStack)
            {
                lifetime += startTime;
                durationIncrease = lifetime / startTime;
            }
        }*/
        public void IncreaseEffect()
        {
            if (effectIterator < maxStack)
            {
                effectIterator++;
            }
        }

        public void ForceStop()
        {
            isActive = false;
        }

        public BaseStatus Copy(BaseEntity target)
        {
            BaseStatus copy = (BaseStatus)MemberwiseClone();
            copy.targetEntity = target;

            return copy;
        }

        public Action<BaseEntity, BaseStatus> ApplyCustomEffect { get; protected set; }
        public Action<BaseEntity, BaseStatus> Terminate { get; protected set; }
    }
}
