using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.TileEngine;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Skills;
using Pilgrimage_Of_Embers.CombatEngine.Types;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.ScreenEngine.Spellbook.Types;
using Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes;
using Pilgrimage_Of_Embers.ParticleEngine;
using Pilgrimage_Of_Embers.VFX;

namespace Pilgrimage_Of_Embers.CombatEngine
{
    //Add to this if necessary. Deflectors block strikers, strikers are anything that inflict damage (Swords, shields, etc! Magic?)
    public enum CombatMove
    {
        None,
        Basic,
        Power,

        Jump,
        Roll,
        Sneak,
        Sprint,

        OffhandEmpty, //When the entity is holding only one weapon, click the offhand MB.
        BehindShield, //When the entity's offhand weapon is currently blocking.

        DeflectOther,
        DeflectThis,
    }

    /* Controls: (left/right - relative to weapon location) Keep controls out of this to make it compatible with entities.
     * Basic - left/right click
     * Combo - Basic, then mid-swing. Increment until at max, then reset to beginning.
     * Power - Hold left/right button on mouse
     * Jump - Space, then left/right click in mid-air
     * After Roll - roll, then left/right click
     * Sneak - while sneaking, left/right click
     * 
     * Offhand Empty - If opposite slot is empty, hold empty slot button.
     * Behind Shield - Hold shield button, then click weapon button. One-handed + Shield only.
     * Blocking - Hold empty slot button. Block with two-handed sword. Has a really small width, varies form weapon to weapon.
     */

    public enum CombatSlotType
    {
        OneHand,
        TwoHand
    }

    /* HasMove 

    public bool HasCombo { get { return hasCombo; } }
    public bool HasPower { get { return hasPower; } }
    public bool HasJump { get { return hasJump; } }
    public bool HasAfterRoll { get { return hasAfterRoll; } }
    public bool HasSneak { get { return hasSneak; } }
    public bool HasOneHand { get { return hasOneHand; } }
    public bool HasTwoHand { get { return hasTwoHand; } }
    public bool HasBehindShield { get { return hasBehindShield; } }

    //To help determine if a sword has that attack available. Set these values in the constructor in every sword.
    protected bool hasCombo, hasPower, hasJump, hasAfterRoll, hasSneak, hasOneHand, hasTwoHand, hasBehindShield;

    */

    public class WeaponCollision
    {
        private string name;

        public Line Line;

        public Vector2 PositionA { get { return Line.locationA; } set { Line.locationA = value; } }
        public Vector2 PositionB { get { return Line.locationB; } set { Line.locationB = value; } }

        public bool IsActive { get; set; }
        public bool IsBlocking { get; set; }

        public float BaseDamageMultiplier { get; private set; }
        public float DamageMultiplier { get; set; }

        public WeaponCollision(string Name, float DamageMultiplier = 1f)
        {
            name = Name;

            BaseDamageMultiplier = DamageMultiplier;
            this.DamageMultiplier = BaseDamageMultiplier;
            IsActive = false;

            Line = new Line();
        }

        public bool IsIntersecting(Line line)
        {
            return this.Line.Intersects(line);
        }
        public bool IsIntersecting(Circle circle)
        {
            return Line.Intersects(circle);
        }
        public bool IsIntersecting(Rectangle rect)
        {
            return Line.Intersects(rect);
        }

        public void DrawDebug(SpriteBatch sb, Texture2D pixel)
        {
            if (IsActive == true)
                Line.DrawLine(sb, pixel, Color.Red, .999f, 2);
            else if (IsBlocking == true)
                Line.DrawLine(sb, pixel, Color.SkyBlue, .999f, 2);
            else
                Line.DrawLine(sb, pixel, Color.White, .999f, 2);

            sb.Draw(pixel, new Rectangle((int)PositionA.X - 1, (int)PositionA.Y - 1, 3, 3), null, Color.Green, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            sb.Draw(pixel, new Rectangle((int)PositionB.X - 1, (int)PositionB.Y - 1, 3, 3), null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }

        public override string ToString()
        {
            return name + " " + Line.locationA.ToString() + " " + Line.locationB.ToString();
        }
    }

    public class BaseCombat //All things combat related are derived from this (Swords, shields, magic, bows, etc...)
    {
        protected Vector2 center, mouseWorld, directionPosition, handlePosition, endPosition, castingPosition, normalizedDirection, normalizedEnd;
        protected float baseStartingAngle, startingAngle, lastAngle, attackRange, shieldRadius, baseDepth, finalDepth, shadowOffset, baseShadowOffset;

        private float combatHeight;
        protected float weaponDistance, playerDistance, handleAngle, endAngle, weaponLength, reach, minReach, maxReach;
        protected float multiplier1, multiplier2, multiplier3;

        protected float CombatHeight { get { return combatHeight; } set { combatHeight = (float)MathHelper.Clamp(value, -256f, 256f); } }
        protected float FinalDistance { get { return weaponDistance + playerDistance; } }

        public Vector2 CastingPosition { get { return castingPosition; } }

        protected float scale, scaleLerp, baseScale, mouseDirection;
        public float MouseDirection { get { return mouseDirection; } }

        protected int dualWieldDirection = 1, currentFloor = 1, comboSwing = 1;
        private int currentSwing = 1;
        public string WeaponDamageName { get; private set; }

        protected bool isJumping, isRolling, isSneaking, isSprinting;
        protected bool isBasicClickOnce = true, isJumpClickOnce = true, isRollClickOnce = true, isSneakClickOnce = true,
                       isSprintClickOnce = true, isOffhandEmptyClickOnce = true, isDualHandClickOnce = true, isBehindShieldClickOnce = true;
        protected bool isMoveAnimPaused = false, isButtonHeld = false, isUpdateDirection = false;

        public bool IsEntitySetState()
        {
            if (combatMove != CombatMove.None)
                return isUpdateDirection;
            else
                return true;
        }

        protected WeaponCategory weaponCategory;
        protected CombatSlotType slotType;

        protected static Texture2D pixel;

        protected List<WeaponCollision> lines;
        public List<WeaponCollision> WeaponShape { get { return lines; } }

        protected float weaponSpeed = 1f, baseWeaponSpeed = 1f, agilitySpeed = 1f;
        public float WeaponSpeed { get { return baseWeaponSpeed * weaponSpeed * agilitySpeed; } }

        protected Point currentFrame, frameSize, sheetSize;

        protected Vector2 sparksPosition, sparksDirection, lastPositionA, lastPositionB;
        protected BaseEmitter sparks = new MetalSparks(Color.Beige, 50);
        protected MotionBlur blur;

        public Vector2 Center { set { center = value; } }
        public float ShadowOffset { set { shadowOffset = value; } }

        protected float directionAngle;
        public float DirectionAngle { get { return directionAngle; } }

        /// <summary>
        /// Determines how far a weapon's reach is. Used by AI.
        /// </summary>
        public float AttackRange { get; protected set; }

        public WeaponCategory WeaponCategory { get { return weaponCategory; } }
        public CombatSlotType SlotType { get { return slotType; } }

        protected CombatMove combatMove, lastCombatMove, queuedMove;
        public CombatMove CurrentAction { get { return combatMove; } }
        public CombatMove LastAction { get { return lastCombatMove; } }

        /// <summary>
        /// Returns true when a striker has hit a shield. If a shield attack hits a shield, the shield held up is knocked back, exposing the entity.
        /// </summary>
        public bool HitShield { get; set; }

        /// <summary>
        /// Returns true when a striker has hit an entity. If a striker hits a striker, the one with the lower strength level gets deflected.
        /// </summary>
        public bool HitEntity { get; set; }

        //Control-related
        protected Controls controls = new Controls();

        protected Controls.MouseButton mouseButton, antiButton;
        public Controls.MouseButton MouseButton { get { return mouseButton; } }
        public Controls.MouseButton AntiButton { get { return antiButton; } } //The opposite of the weapon button (if mouseButton = leftClick, this would equal to rightClick)

        //Texture flipping
        private SpriteEffects textureDirection = SpriteEffects.None;
        protected SpriteEffects TextureDirection
        {
            get { return textureDirection; }
            set
            {
                //Opposite hand has reversed values
                if (dualWieldDirection == -1)
                {
                    if (value == SpriteEffects.None)
                        textureDirection = SpriteEffects.FlipHorizontally;
                    else if (value == SpriteEffects.FlipHorizontally)
                        textureDirection = SpriteEffects.None;
                }
                else
                    textureDirection = value;
            }
        }
        /// <summary>
        /// For weapons of complex shapes, such as scythes. Multiply the WeaponCollision line positioning with this value.
        /// </summary>
        /// <returns>Either 1 or -1.</returns>
        protected int SpriteEffectMultiplier()
        {
            if (textureDirection == SpriteEffects.FlipHorizontally)
                return -1;

            return 1;
        }

        //References
        protected BaseEntity entity;
        protected BaseCombat offhand;

        protected Camera camera;
        protected Skillset Skillset { get { return entity.Skills; } }
        protected TileMap tileMap;

        protected Weapon weapon;
        protected Ammo ammo;
        protected BaseSpell spell;

        protected Random random;

        //Animation-related
        private List<CombatAction> actions = new List<CombatAction>();

        protected int combatTime = 0, maxCombatTime = 0;
        public int TimeLeft { get { return Math.Abs(maxCombatTime - combatTime); } }
        public int MaxCombatTime { get { return maxCombatTime; } }

        //Misc
        public bool IsUsable { get; set; } //If has two handed and one handed, one handed is unusable
        protected const float angleOffset = .78f;

        public BaseCombat(WeaponCategory Category, CombatSlotType Slot, float WeaponLength, float WeaponDistance)
        {
            weaponCategory = Category;
            slotType = Slot;

            weaponLength = WeaponLength;
            weaponDistance = WeaponDistance;
        }

        public void SetReferences(Camera camera, TileMap tileMap, BaseEntity entity)
        {
            this.camera = camera;
            this.tileMap = tileMap;
            this.entity = entity;
        }

        public static void LoadDebugTextures(ContentManager cm)
        {
            pixel = cm.Load<Texture2D>("rect");
        }
        public virtual void Load(ContentManager main, ContentManager map)
        {
            if (sparks != null)
            {
                sparks.Load(map);
                sparks.IsActivated = false;
                sparks.IsManualDepth = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gt"></param>
        /// <param name="checkControls">True = player-controlled; False = entity-controlled (with AI)</param>
        public virtual void Update(GameTime gt, bool checkControls, bool restrictControls)
        {
            lastCombatMove = combatMove;
            lastAngle = directionAngle + handleAngle;
            agilitySpeed = Skillset.agility.AttackSpeed;

            if (sparks != null)
            {
                sparks.Offset = sparksPosition;

                if (sparks is MetalSparks)
                    ((MetalSparks)sparks).SparkDirection = sparksDirection;
                
                if (sparks.Particles.Count > 0) //Only deactivate when there are particles.
                    sparks.IsActivated = false;
            }

            CalculateVectors();
            RetrieveEquipment();

            isForceStopped = false;

            UpdateActions(gt);
            UpdateControls(gt, checkControls, restrictControls);

            switch (combatMove)
            {
                case CombatMove.Basic: Basic(gt); break;
                case CombatMove.Power: Power(gt); break;

                case CombatMove.Jump: Jump(gt); break;
                case CombatMove.Roll: Roll(gt); break;
                case CombatMove.Sneak: Sneak(gt); break;
                case CombatMove.Sprint: Sprint(gt); break;

                case CombatMove.OffhandEmpty: Offhand(gt); break;
                case CombatMove.BehindShield: BehindShield(gt); break;

                case CombatMove.DeflectOther: DeflectOther(gt); break;
                case CombatMove.DeflectThis: DeflectThis(gt); break;
            }

            UpdateDepth();
            UpdateScale(gt);

            if (CurrentAction != CombatMove.None)
                blur.Update(gt, lastPositionA, lastPositionB, true);
        }
        private void RetrieveEquipment()
        {
            if (mouseButton == Controls.MouseButton.LeftClick)
                weapon = entity.EQUIPMENT_PrimaryWeapon();
            else if (mouseButton == Controls.MouseButton.RightClick)
                weapon = entity.EQUIPMENT_OffhandWeapon();

            ammo = entity.EQUIPMENT_PrimaryAmmo();
            spell = entity.EQUIPMENT_CurrentSpell();
        }
        private void CalculateVectors()
        {
            //Mouse-related
            mouseWorld = camera.ScreenToWorld(controls.MouseVector);
            mouseDirection = center.Direction(mouseWorld);

            //Clamping
            reach = MathHelper.Clamp(reach, minReach, maxReach);

            //Position calculating
            directionPosition = CalculateRelativeVector(directionAngle, center, FinalDistance, CombatHeight / 2, ref normalizedDirection);
            handlePosition = CalculateRelativeVector(directionAngle + handleAngle, center, FinalDistance + reach, CombatHeight / 2);
            endPosition = CalculateRelativeVector(directionAngle + endAngle, handlePosition, weaponLength, 0, ref normalizedEnd);
        }
        protected Vector2 CalculateRelativeVector(float angle, Vector2 center, float distance, float height)
        {
            //Get Vector2 from angle (radians)
            Vector2 normDirection = angle.ToVector2();

            //Normalize the Vector2
            if (normDirection != Vector2.Zero)
                normDirection.Normalize();

            //1. Multiply normDirection by distance
            //2. Add center to step 1 to get the offset
            //3. Subtract height from step 2's Y. Return.
            return (center + (normDirection * distance)) - new Vector2(0, height);
        }
        protected Vector2 CalculateRelativeVector(float angle, Vector2 center, float distance, float height, ref Vector2 direction)
        {
            //Get Vector2 from angle (radians)
            Vector2 normDirection = angle.ToVector2();

            //Normalize the Vector2
            if (normDirection != Vector2.Zero)
                normDirection.Normalize();

            direction = normDirection;

            //1. Multiply normDirection by distance
            //2. Add center to step 1 to get the offset
            //3. Subtract height from step 2's Y. Return.
            return (center + (normDirection * distance)) - new Vector2(0, height);
        }
        protected Vector2 CalculateShadow(Vector2 origin)
        {
            return origin + new Vector2(0, shadowOffset + CombatHeight + baseShadowOffset);
        }

        private void UpdateControls(GameTime gt, bool checkControls, bool restrictControls)
        {
            controls.UpdateCurrent();

            if (restrictControls == false && IsUsable == true)
            {
                if (checkControls == true)
                    CheckControls(gt);
                else
                    CheckEntityControls(gt); //This is unnecessary for BaseStriker. Implemented for BaseShooter control.

                if (queuedMove != CombatMove.None && CurrentAction == CombatMove.None)
                    RequestCombatMove(queuedMove, true);
            }

            controls.UpdateLast();
        }

        protected virtual void UpdateActions(GameTime gt)
        {
            if (combatTime > maxCombatTime || maxCombatTime == -1)
                ResetCombatState();

            if (CurrentAction != CombatMove.None)
            {
                if (isMoveAnimPaused == false)
                {
                    combatTime += gt.ElapsedGameTime.Milliseconds;

                    for (int i = 0; i < actions.Count; i++)
                    {
                        if (combatTime >= actions[i].minTime && combatTime <= actions[i].maxTime)
                            actions[i].action.Invoke();
                    }
                }
            }
            else
                ResetVariables();
        }
        protected void UpdateScale(GameTime gt)
        {
            if (CurrentAction != CombatMove.None)
                scaleLerp += 15f * (float)gt.ElapsedGameTime.TotalSeconds;
            else
                scaleLerp -= 15f * (float)gt.ElapsedGameTime.TotalSeconds;

            scaleLerp = MathHelper.Clamp(scaleLerp, 0f, 1f);
            scale = MathHelper.Lerp(0f, baseScale, scaleLerp);
        }

        protected virtual void CheckControls(GameTime gt)
        {
            isButtonHeld = false;

            //Offhand-oriented
            if (offhand != null)
            {
                if (offhand.IsBlocking())
                    CheckControl(mouseButton, CombatMove.BehindShield, isBehindShieldClickOnce);
            }
            if (offhand == null || slotType == CombatSlotType.TwoHand)
                CheckControl(antiButton, CombatMove.OffhandEmpty, isOffhandEmptyClickOnce);

            //Skill-oriented
            if (isJumping == true)
                CheckControl(mouseButton, CombatMove.Jump, isJumpClickOnce);
            if (isRolling == true)
                CheckControl(mouseButton, CombatMove.Roll, isRollClickOnce);
            if (isSneaking == true)
                CheckControl(mouseButton, CombatMove.Sneak, isSneakClickOnce);
            if (isSprinting == true)
                CheckControl(mouseButton, CombatMove.Sprint, isSprintClickOnce);

            //Basic/power
            if (IsActionless == true)
                CheckControl(mouseButton, CombatMove.Basic, isBasicClickOnce);
            if (IsActionless == true)
            {
                if (isBasicClickOnce == true) //Since they use the same button and state, if the basic click can be held down, automatically disabled the power move.
                {
                    if (controls.IsMouseHeld(gt, mouseButton, 200))
                        RequestCombatMove(CombatMove.Power);
                }
            }

            if (isButtonHeld == false) //For moves that have animation pauses
                isMoveAnimPaused = false;
        }
        private void CheckControl(Controls.MouseButton button, CombatMove move, bool isClickOnce)
        {
            if (isClickOnce == true)
            {
                if (controls.IsClickedOnce(button))
                {
                    RequestCombatMove(move);
                    isButtonHeld = true;
                }
            }
            else
            {
                if (controls.IsMouseHeld(button))
                {
                    RequestCombatMove(move);
                    isButtonHeld = true;
                }
            }
        }
        protected virtual void CheckEntityControls(GameTime gt) { }
        protected virtual void UpdateDepth() { }
        
        protected bool IsActionless { get { return ((isJumping == false) && (isRolling == false) && (isSneaking == false) && (isSprinting == false)); } }
        /// <summary>
        /// Prevent reassigning of the combatMove when the current attack is still in use.
        /// </summary>
        /// <param name="move"></param>
        public void RequestCombatMove(CombatMove move, bool isFromQueue = false)
        {
            if (IsUsable == true)
            {
                if (combatMove == CombatMove.None)
                    combatMove = move;
            }

            //If the action isn't from queue, attempt to queue it.
            if (isFromQueue == false)
            {
                if (queuedMove == CombatMove.None)
                {
                    if (combatTime > (maxCombatTime * .5f))
                    {
                        queuedMove = move;

                        //Only increase swing when queued move equals to last move.
                        if (queuedMove == lastCombatMove)
                            comboSwing++;
                    }
                }
            }
            else
                queuedMove = CombatMove.None;
        }
        public void RequestRandomMove(params CombatMove[] moves)
        {
            if (IsUsable == true)
            {
                CombatMove value = moves[random.Next(0, moves.Length)];
                RequestCombatMove(value);
            }
        }
        public CombatMove GetRandomMove(params CombatMove[] moves) { return moves[random.Next(0, moves.Length)]; }
        public void ForceCombatMove(CombatMove move)
        {
            ResetVariables();

            combatMove = move;
        }

        public void ResetCombatState()
        {
            combatMove = CombatMove.None;
        }
        protected virtual void ResetVariables()
        {
            ClearActions();

            combatTime = 0;
            maxCombatTime = 0;

            handleAngle = 0f;
            endAngle = 0f;
            reach = 0f;

            isAdded = false;
            isAssignedBeforeSwing = false;
            isUpdateDirection = false;

            multiplier1 = 0f;
            multiplier2 = 0f;
            multiplier3 = 0f;

            CombatHeight = 0f;

            TextureDirection = SpriteEffects.None;

            ShotCount = 1;
            CastCount = 1;
            currentSwing = 1;
            AssignDamageName();

            //If the queued move is not the same as the previous move.
            if (queuedMove != lastCombatMove)
                comboSwing = 1;

            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].IsActive = false;
                lines[i].DamageMultiplier = lines[i].BaseDamageMultiplier;

                if (LastAction != CombatMove.None)
                    lines[i].IsBlocking = false;
            }
        }

        #region Animation methods
        protected bool isAdded = false, isAssignedBeforeSwing;
        protected void AssignVariables(float angleOffset, int maxSwingTime,
                       float multiplier1 = 0f, float multiplier2 = 0f, float multiplier3 = 0f)
        {
            if (isAssignedBeforeSwing == false)
            {
                startingAngle = baseStartingAngle;
                startingAngle += (angleOffset * dualWieldDirection);

                maxCombatTime = (int)(maxSwingTime / WeaponSpeed);

                this.multiplier1 = multiplier1;
                this.multiplier2 = multiplier2;
                this.multiplier3 = multiplier3;

                isAssignedBeforeSwing = true;

                blur.Reset(lastPositionA, lastPositionB);
            }
        }
        protected void AddAction(Action action, int minTime, int maxTime)
        {
            actions.Add(new CombatAction(action, (int)(minTime / WeaponSpeed), (int)(maxTime / WeaponSpeed)));
        }
        protected void ClearActions()
        {
            actions.Clear();
        }

        protected void MoveHandle(GameTime gt, float velocity)
        {
            handleAngle += (velocity * WeaponSpeed) * (float)gt.ElapsedGameTime.TotalSeconds;
        }
        protected void MoveEnd(GameTime gt, float velocity)
        {
            endAngle += (velocity * WeaponSpeed) * (float)gt.ElapsedGameTime.TotalSeconds;
        }
        protected void MoveDirection(GameTime gt, float velocity)
        {
            directionAngle += (velocity * WeaponSpeed) * (float)gt.ElapsedGameTime.TotalSeconds;
        }
        protected void MoveReachLength(GameTime gt, float length)
        {
            reach += (length * WeaponSpeed) * (float)gt.ElapsedGameTime.TotalSeconds;
        }
        protected void MoveStrikerHeight(GameTime gt, float velocity)
        {
            CombatHeight += (velocity * WeaponSpeed) * (float)gt.ElapsedGameTime.TotalSeconds;
        }

        protected void Adjust(GameTime gt, ref float value, float speed)
        {
            value += (speed * WeaponSpeed) * (float)gt.ElapsedGameTime.TotalSeconds;
        }

        protected void IncreaseSwingInteger()
        {
            currentSwing++;
            AssignDamageName();
        }
        protected void AssignDamageName()
        {
            WeaponDamageName = "[" + entity.MapEntityID + "]" + weapon.Name + dualWieldDirection + currentSwing;
        }
        #endregion

        protected bool isForceStopped;
        public bool IsForceStopped { get { return isForceStopped; } }
        public virtual void ForceStop()
        {
            isMoveAnimPaused = false;
        }

        public void SetButtons(Controls.MouseButton mouseButton, Controls.MouseButton antiButton)
        {
            this.mouseButton = mouseButton;
            this.antiButton = antiButton;

            if (this.mouseButton == Controls.MouseButton.LeftClick)
                dualWieldDirection = 1; //Primary hand
            if (this.mouseButton == Controls.MouseButton.RightClick)
                dualWieldDirection = -1; //Offhand
        }
        public virtual void SetVariables(BaseEntity entity, BaseCombat offhand, float direction, int currentFloor, float shieldRadius, float depth, float playerDistance)
        {
            baseStartingAngle = direction;

            this.entity = entity;
            this.offhand = offhand;
            this.currentFloor = currentFloor;
            this.shieldRadius = shieldRadius * 2f;

            this.playerDistance = playerDistance;
            baseDepth = depth;

            if (isUpdateDirection == false)
            {
                if (scale <= 0) //Animation-based
                {
                    directionAngle = direction;
                    startingAngle = baseStartingAngle;
                }
            }
            else //Non-animation
            {
                directionAngle = direction;
                startingAngle = baseStartingAngle;
            }
        }
        public void SetStates(bool isJumping, bool isRolling, bool isSneaking, bool isSprinting)
        {
            this.isJumping = isJumping;
            this.isRolling = isRolling;
            this.isSneaking = isSneaking;
            this.isSprinting = isSprinting;
        }
        protected void SetClickBools(bool isBasicClickOnce, bool isJumpClickOnce, bool isRollClickOnce, bool isSneakClickOnce,
                       bool isSprintClickOnce, bool isOffhandEmptyClickOnce, bool isDualHandClickOnce, bool isBehindShieldClickOnce)
        {
            this.isBasicClickOnce = isBasicClickOnce;

            this.isJumpClickOnce = isJumpClickOnce;
            this.isRollClickOnce = isRollClickOnce;
            this.isSneakClickOnce = isSneakClickOnce;
            this.isSprintClickOnce = isSprintClickOnce;

            this.isOffhandEmptyClickOnce = isOffhandEmptyClickOnce;
            this.isDualHandClickOnce = isDualHandClickOnce;
            this.isBehindShieldClickOnce = isBehindShieldClickOnce;
        }

        protected void SetAnimationValues(Point frameSize, Point sheetSize)
        {
            this.frameSize = frameSize;
            this.sheetSize = sheetSize;
        }
        protected void SetCurrentFrame(int x, int y)
        {
            currentFrame.X = MathHelper.Clamp(x, 0, sheetSize.X - 1);
            currentFrame.Y = MathHelper.Clamp(y, 0, sheetSize.Y - 1);
        }

        #region Shooting Methods

        public int ArcheryDamage()
        {
            return (int)Skillset.strength.ArcheryDamage;
        }

        public int ShotCount { get; set; }
        public int CastCount { get; set; }

        public enum DecreaseProjectile { None, Ammo, Weapon, Spell }
        public void FireCustom(int id, Vector2 position, float angle, float accuracyOffset, float speedMultiplier, float damageMultiplier, DecreaseProjectile decrease, int shotCount)
        {
            if (ShotCount == shotCount) //directionAngle + accuracyOffset
            {
                Vector2 centerPos = camera.ScreenToWorld(controls.MouseVector);

                tileMap.AddProjectile(id, position, angle - random.NextFloat(-accuracyOffset, accuracyOffset), speedMultiplier, damageMultiplier, currentFloor, entity.MapEntityID, weapon.ProjectileDamage);
                ShotCount++;

                if (decrease == DecreaseProjectile.Ammo) //Ammo-based weapons, such as crossbows and bows.
                {
                    if (ammo != null)
                        ammo.CurrentAmount -= 1;
                }
                if (decrease == DecreaseProjectile.Weapon) //Thrown weapons
                {
                    if (ammo != null)
                        weapon.CurrentAmount -= 1;
                }
            }
        }

        #endregion

        public bool IsBlocking()
        {
            bool isBlocking = false;
            for (int i = 0; i < lines.Count; i++)
                if (lines[i].IsBlocking == true)
                    isBlocking = true;

            return isBlocking;
        }

        public bool IsColliding(Rectangle rect)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].IsActive == true)
                {
                    if (lines[i].IsIntersecting(rect))
                        return true;
                }
            }

            return false;
        }
        public bool IsColliding(Circle circle)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].IsActive == true)
                {
                    if (lines[i].IsIntersecting(circle))
                        return true;
                }
            }

            return false;
        }
        public bool IsColliding(Line line)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].IsActive == true)
                {
                    if (lines[i].IsIntersecting(line))
                        return true;
                }
            }

            return false;
        }

        public float CollidingDamageMultiplier(Rectangle rect)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].IsActive == true)
                {
                    if (lines[i].IsIntersecting(rect))
                        return lines[i].DamageMultiplier;
                }
            }

            return 1f;
        }
        public WeaponCollision CollidingLine(Rectangle rect)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].IsActive == true)
                {
                    if (lines[i].IsIntersecting(rect))
                        return lines[i];
                }
            }

            return null;
        }

        protected string[] attributeNames;
        public void OnEntityHit(BaseEntity enemy, WeaponCollision line)
        {
            enemy.COMBAT_DamageEntity(WeaponDamageName, (uint)(entity.COMBAT_PhysicalDamage(weapon) * line.DamageMultiplier), TimeLeft - (int)(TimeLeft * .25f), () =>
            {
                weapon.CurrentDurability--;
                enemy.Skills.resistance.CurrentStun++;

                if (hitAction != null)
                    hitAction(enemy, line);

                enemy.VISUAL_AddVisual(500); //Replace with a more customizable option? Put inside of BaseEntity.
            }, attributeNames);
        }
        protected Action<BaseEntity, WeaponCollision> hitAction;

        protected bool IsAngleMotionLeft()
        {
            if (directionAngle + handleAngle > lastAngle)
                return true;
            else
                return false;
        }
        protected int AngleMotionMultiplier()
        {
            if (IsAngleMotionLeft() == true)
                return 1;
            else
                return -1;
        }

        /// <summary>
        /// Input: Click primary 'button'.
        /// </summary>
        public virtual void Basic(GameTime gt) { ResetCombatState(); }
        /// <summary>
        /// Input: Hold primary 'button'.
        /// </summary>
        public virtual void Power(GameTime gt) { ResetCombatState(); }

        /// <summary>
        /// Input: Space(Jump button), while in mid-air click primary 'button'.
        /// </summary>
        public virtual void Jump(GameTime gt) { ResetCombatState(); }
        /// <summary>
        /// Input: LAlt(Dodge button), while rolling click primary 'button'.
        /// </summary>
        public virtual void Roll(GameTime gt) { ResetCombatState(); }
        /// <summary>
        /// Input: LCtrl(Sneak button), click primary 'button'.
        /// </summary>
        public virtual void Sneak(GameTime gt) { ResetCombatState(); }
        /// <summary>
        /// Input: LShift(Sprint button), while sprinting click primary 'button'
        /// </summary>
        public virtual void Sprint(GameTime gt) { ResetCombatState(); }

        /// <summary>
        /// If entity has one hand open, can use this. Input: click opposite 'button'.
        /// </summary>
        public virtual void Offhand(GameTime gt) { ResetCombatState(); }
        /// <summary>
        /// For attacks from behind a shield. If entity has a shield, can use this. Input: While shield is held, click non-shield 'button'.
        /// </summary>
        public virtual void BehindShield(GameTime gt) { ResetCombatState(); }

        /// <summary>
        /// What happens to strikers when they hit a shield.
        /// </summary>
        /// <param name="gt"></param>
        public virtual void DeflectThis(GameTime gt)
        {
            if (isAdded == false)
            {
                AssignVariables(0f, 600, .75f);
                isUpdateDirection = false;
                SetCurrentFrame(0, 0);

                //Update lerp
                AddAction(() => { multiplier1 += 5f * (float)gt.ElapsedGameTime.TotalSeconds; }, 0, 150);
                AddAction(() => { multiplier1 += 3f * (float)gt.ElapsedGameTime.TotalSeconds; }, 150, 300);
                AddAction(() => { multiplier1 += 1f * (float)gt.ElapsedGameTime.TotalSeconds; }, 300, 600);

                //Smoothly move sword to position
                AddAction(() => { directionAngle = MathHelper.SmoothStep(startingAngle + (.25f * AngleMotionMultiplier()), startingAngle + (1.5f * AngleMotionMultiplier()), multiplier1); }, 0, maxCombatTime);

                isAdded = true;
            }
        }
        /// <summary>
        /// What happens to a shield when a striker hits it.
        /// </summary>
        /// <param name="gt"></param>
        public virtual void DeflectOther(GameTime gt)
        {
            if (isAdded == false)
            {
                AssignVariables(0f, 250, 0f);
                isUpdateDirection = false;

                sparks.IsActivated = true;
                sparks.Depth = finalDepth + .001f;
                tileMap.AddEmitter(sparks);

                reach = -10f;

                //Update lerp
                AddAction(() => { multiplier1 += 7f * (float)gt.ElapsedGameTime.TotalSeconds; }, 0, maxCombatTime);

                //Smoothly move sword to position
                AddAction(() => { reach = MathHelper.SmoothStep(-10f, 0f, multiplier1); }, 0, maxCombatTime);

                isAdded = true;
            }
        }

        public virtual void DrawDebug(SpriteBatch sb)
        {
            sb.Draw(pixel, new Rectangle((int)directionPosition.X - 1, (int)directionPosition.Y - 1, 3, 3), Color.Red);

            for (int i = 0; i < lines.Count; i++)
                lines[i].DrawDebug(sb, pixel);
        }
        public virtual void Draw(SpriteBatch sb)
        {
        }

        public virtual BaseCombat Copy()
        {
            BaseCombat copy = (BaseCombat)MemberwiseClone();

            copy.entity = null;
            copy.lines = new List<WeaponCollision>();

            copy.random = new Random(Guid.NewGuid().GetHashCode());
            copy.actions = new List<CombatAction>();
            copy.controls = new Controls();

            copy.blur = new MotionBlur(5, Color.White);
            copy.sparks = sparks.Copy();

            return copy;
        }
    }
}
