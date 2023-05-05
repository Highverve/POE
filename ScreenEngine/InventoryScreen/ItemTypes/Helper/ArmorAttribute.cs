using Pilgrimage_Of_Embers.Entities.Entities;
using System;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper
{
    public class ItemAttribute
    {
        private float physicalDefense, projectileDefense, magicDefense, effectResistance, effectAmplifier, weight;

        /// <summary>
        /// Reduces damage taken from close-combat weaponry.
        /// </summary>
        public float BasePhysicalDefense { get { return physicalDefense; } }
        /// <summary>
        /// Reduces damage taken from projectiles.
        /// </summary>
        public float BaseProjectileDefense { get { return projectileDefense; } }
        /// <summary>
        /// Reduces damage taken from spells.
        /// </summary>
        public float BaseMagicDefense { get { return magicDefense; } }
        /// <summary>
        /// Reduces the movement speeds of the entity, but improves stun defense.
        /// </summary>
        public float BaseWeight { get { return weight; } }

        /// <summary>
        /// Reduces the potency of negative effects.
        /// </summary>
        public float BaseEffectResistance { get { return effectResistance; } }
        /// <summary>
        /// Increases the potency of positive effects.
        /// </summary>
        public float BaseEffectAmplifier { get { return effectAmplifier; } }

        public float PhysicalDefense(int currentReinforcement, float reinforceMultiplier)
        {
            return physicalDefense + (int)(currentReinforcement * reinforceMultiplier);
        }
        public float ProjectileDefense(int currentReinforcement, float reinforceMultiplier)
        {
            return projectileDefense + (int)(currentReinforcement * reinforceMultiplier);
        }
        public float MagicDefense(int currentReinforcement, float reinforceMultiplier)
        {
            return magicDefense + (int)(currentReinforcement * reinforceMultiplier);
        }

        public float Weight(int currentReinforcement, float reinforceMultiplier)
        {
            return weight + (int)(currentReinforcement * reinforceMultiplier);
        }

        public float EffectResistance(int currentReinforcement, float reinforceMultiplier)
        {
            return effectResistance + (int)(currentReinforcement * reinforceMultiplier);
        }
        public float EffectAmplifier(int currentReinforcement, float reinforceMultiplier)
        {
            return effectAmplifier + (int)(currentReinforcement * reinforceMultiplier);
        }

        public float HealthModifier(int currentReinforcement, float reinforceMultiplier)
        {
            return healthModifier + (int)(currentReinforcement * reinforceMultiplier);
        }
        public float StaminaModifier(int currentReinforcement, float reinforceMultiplier)
        {
            return staminaModifier + (int)(currentReinforcement * reinforceMultiplier);
        }
        public float MagicModifier(int currentReinforcement, float reinforceMultiplier)
        {
            return magicModifier + (int)(currentReinforcement * reinforceMultiplier);
        }

        public float PhysicalDamageModifier(int currentReinforcement, float reinforceMultiplier)
        {
            return physicalDamageModifier + (int)(currentReinforcement * reinforceMultiplier);
        }
        public float ProjectileDamageModifier(int currentReinforcement, float reinforceMultiplier)
        {
            return projectileDamageModifier + (int)(currentReinforcement * reinforceMultiplier);
        }
        public float SpellDamageModifier(int currentReinforcement, float reinforceMultiplier)
        {
            return spellDamageModifier + (int)(currentReinforcement * reinforceMultiplier);
        }

        public float AccuracyAmplifier(int currentReinforcement, float reinforceMultiplier)
        {
            return accuracyAmplifier + (currentReinforcement * reinforceMultiplier);
        }
        public float ConcealmentAmplifier(int currentReinforcement, float reinforceMultiplier)
        {
            return concealmentAmplifier + (currentReinforcement * reinforceMultiplier);
        }
        public float AwarenessAmplifier(int currentReinforcement, float reinforceMultiplier)
        {
            return awarenessAmplifier + (currentReinforcement * reinforceMultiplier);
        }
        public float SpeedAmplifier(int currentReinforcement, float reinforceMultiplier)
        {
            return speedAmplifier + (currentReinforcement * reinforceMultiplier);
        }

        private float healthModifier = 0, staminaModifier = 0, magicModifier = 0, physicalDamageModifier = 0,
                      projectileDamageModifier = 0, spellDamageModifier = 0, accuracyAmplifier = 0, concealmentAmplifier = 0,
                      awarenessAmplifier = 0, speedAmplifier = 0;

        public float BaseHealthModifier { get { return healthModifier; } }
        public float BaseStaminaModifier { get { return staminaModifier; } }
        public float BaseMagicModifier { get { return magicModifier; } }

        public float BasePhysicalDamageModifier { get { return physicalDamageModifier; } }
        public float BaseProjectileDamageModifier { get { return projectileDamageModifier; } }
        public float BaseSpellDamageModifier { get { return spellDamageModifier; } }

        public float BaseAccuracyAmplifier { get { return accuracyAmplifier; } }
        public float BaseConcealmentAmplifier { get { return concealmentAmplifier; } }
        public float BaseAwarenessAmplifier { get { return awarenessAmplifier; } }
        public float BaseSpeedAmplifier { get { return speedAmplifier; } }

        private Action<BaseEntity, int> action = null;
        public Action<BaseEntity, int> Action { get { return action; } }

        public ItemAttribute() { }
        public ItemAttribute(Action<BaseEntity, int> Action)
        {
            action = Action;
        }
        public ItemAttribute(float PhysicalDefense, float ProjectileDefense, float MagicDefense, float Weight, Action<BaseEntity, int> Action) : this (Action)
        {
            physicalDefense = PhysicalDefense;
            projectileDefense = ProjectileDefense;
            magicDefense = MagicDefense;
            weight = Weight;
        }
        public ItemAttribute(float PhysicalDefense, float ProjectileDefense, float MagicDefense, float Weight, float EffectResistance, float EffectAmplifier, float HealthModifier, float StaminaModifier,
                              float MagicModifier, float PhysicalDamageModifier, float ProjectileDamageModifier, float SpellDamageModifier, Action<BaseEntity, int> Action)
            : this(PhysicalDefense, ProjectileDefense, MagicDefense, Weight, Action)
        {
            effectResistance = EffectResistance;
            effectAmplifier = EffectAmplifier;

            healthModifier = HealthModifier;
            staminaModifier = StaminaModifier;
            magicModifier = MagicModifier;
            physicalDamageModifier = PhysicalDamageModifier;
            projectileDamageModifier = ProjectileDamageModifier;
            spellDamageModifier = SpellDamageModifier;
        }
        public ItemAttribute(float PhysicalDefense, float ProjectileDefense, float MagicDefense, float Weight, float EffectResistance, float EffectAmplifier, float HealthModifier, float StaminaModifier,
                      float MagicModifier, float PhysicalDamageModifier, float ProjectileDamageModifier, float SpellDamageModifier, float ArcheryAccuracyAmplifier, float ConcealmentAmplifier,
                      float AwarenessAmplifier, float SpeedAmplifier, Action<BaseEntity, int> Action)
            : this(PhysicalDefense, ProjectileDefense, MagicDefense, Weight, EffectResistance, EffectAmplifier, HealthModifier, StaminaModifier, MagicModifier, PhysicalDamageModifier,
                  ProjectileDamageModifier, SpellDamageModifier, Action)
        {
            accuracyAmplifier = ArcheryAccuracyAmplifier;
            concealmentAmplifier = ConcealmentAmplifier;
            awarenessAmplifier = AwarenessAmplifier;
            speedAmplifier = SpeedAmplifier;

            action = Action;
        }
    }
}
