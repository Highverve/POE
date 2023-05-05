using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Skills;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.Entities.Factions;
using Pilgrimage_Of_Embers.ScreenEngine;
using Pilgrimage_Of_Embers.TileEngine.Objects.Colliders;
using System.Text.RegularExpressions;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.Debugging;
using Pilgrimage_Of_Embers.Entities.Actions;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.CombatEngine;
using Pilgrimage_Of_Embers.CombatEngine.Types;
using Pilgrimage_Of_Embers.ScreenEngine.Souls.Types;
using Pilgrimage_Of_Embers.Entities.Classes;
using Pilgrimage_Of_Embers.Entities.Types.Player;
using Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.TakeImageTypes;
using Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes;
using Pilgrimage_Of_Embers.Entities.NPC;
using Pilgrimage_Of_Embers.Culture;
using Pilgrimage_Of_Embers.Performance;
using Pilgrimage_Of_Embers.ScreenEngine.Spellbook.Types;
using Pilgrimage_Of_Embers.AudioEngine;
using Pilgrimage_Of_Embers.ParticleEngine;
using Pilgrimage_Of_Embers.Entities.AI.Agents;
using Pilgrimage_Of_Embers.Entities.Steering_Behaviors;
using Pilgrimage_Of_Embers.Entities.AI.Goal;
using System.IO;

namespace Pilgrimage_Of_Embers.Entities.Entities
{
    public enum EntityType { Player, Character, Monster }

    public class BaseEntity : GameObject
    {
        // To-Do:
        // 1. Replace merchant dialogue with a master dialogue. This will be used for everything the character or player does, including merchant dialogue stuff.

        private int id;
        public int ID { get { return id; } }

        private string mapEntityID = ""; //for multiple versions of entities with the same "id" in the same map.
        public string MapEntityID { get { return mapEntityID; } }

        protected string name; //name to be displayed above entity, when mouse is hovering on said entity
        public string Name { get { return name; } set { name = value; } }

        public int BaseCurrentFloor { get { return depthFloor.CurrentFloor; } set { depthFloor.CurrentFloor = value; } }
        public int JumpHeightFloor { get { return -((int)(heightOffset / 100f)); } }
        public int CombinedFloor { get { return depthFloor.CurrentFloor + JumpHeightFloor; } }
        public new float Depth { get { return depthFloor.Depth; } }
        protected float centerOffset, depthOffset, shadowYOrigin, infoHeight;

        protected Random random;

        private float speed;
        public float Speed { get { return speed; } set { speed = value; } }

        protected Vector2 mouseToWorld;
        public Vector2 refPosition;

        public Point pathfindTile;
        protected float mouseDirection;

        protected Skillset skillset;
        public Skillset Skills { get { return skillset; } }

        protected AnimationState animation;
        protected Color entityColor, entityTargetColor;
        protected float colorLerp = 1f;

        protected EntityStatus status;
        protected EntityVisual visualOverlay;
        protected ObjectAttributes attributes;
        protected ObjectStatistics statistics;

        protected EntityCaption caption;
        protected EntityChat chat;

        protected EntityInfo info;
        protected ObjectSenses senses;
        protected BaseFaction faction;
        protected EntityKin kin;
        protected ObjectMemory memory;

        protected EntityEquipment equipment;
        protected EntityStorage storage;
        protected EntityLoot loot;

        protected Merchant merchant;

        private Texture2D jumpBar, jumpFiller;
        private float spaceBarJump = 0;
        private bool displayJumpBar = false;

        public bool CanType { get; set; }
        TypingInterface typing = new TypingInterface();
        
        public BaseFaction Faction { get { return faction; } }
        public EntityEquipment Equipment { get { return equipment; } }

        public Rectangle baseEntityHitbox, entityHitbox; //interactRect for NPC --- Replace with circles
        protected Circle entityCircle, baseEntityCircle, particleCircle, jumpCircle, baseJumpCircle; //For all things map-to-entity interaction : for particle-to-entity interaction

        public Rectangle Hitbox { get { return entityHitbox; } }
        public Circle EntityCircle { get { return entityCircle; } }
        public Circle ParticleCircle { get { return particleCircle; } }
        public Circle JumpingCircle { get { return jumpCircle; } }

        protected BaseAgent agentAI;
        public SteeringBehavior steering;
        protected Pathfinder pathfinder;

        protected Controls controls = new Controls();

        protected List<BaseEntity> mapEntities = new List<BaseEntity>(), closeEntities = new List<BaseEntity>();
        protected List<BaseEntity> validEnemies = new List<BaseEntity>(), currentEnemies = new List<BaseEntity>(); //These are entities that the entity will kill if spotted.
        protected List<BaseEntity> validAllies = new List<BaseEntity>(), currentAllies = new List<BaseEntity>(); //These are entities that the entity will work together with if found.
        protected List<BaseEntity> neutralList = new List<BaseEntity>(); //neutrals can attack entity if provoked via attack.

        public BaseEntity enemyTarget { get; set; }
        public BaseEntity allyTarget { get; set; }

        public List<BaseEntity> MapEntities { get { return mapEntities; } }
        public List<BaseEntity> ValidEnemies { get { return validEnemies; } }
        public List<BaseEntity> ValidAllies { get { return validAllies; } }
        public List<BaseEntity> CurrentEnemies { get { return currentEnemies; } }
        public List<BaseEntity> CurrentAllies { get { return currentAllies; } }

        public List<BaseCollider> totalColliders = new List<BaseCollider>();
        protected List<BaseCollider> nearColliders = new List<BaseCollider>();
        public List<BaseCollider> entityColliders = new List<BaseCollider>();

        protected List<EntityDisposition> dispositions = new List<EntityDisposition>();
        protected List<BaseEntity> companions = new List<BaseEntity>();
        public List<BaseEntity> Companions { get { return companions; } }

        protected bool canMakeCompanion = true;
        public bool CanMakeCompanion { get { return canMakeCompanion; } }
        public bool HasCompanionLeader { get { return (companionLeader != null); } }
        public bool IsCompanionLeader(BaseEntity entity)
        {
            if (HasCompanionLeader)
                return entity.MapEntityID == companionLeader.MapEntityID;
            else
                return false;
        }
        protected BaseEntity companionLeader;
        protected PlayerEntity playerEntity;

        protected TakeImage deathImage;
        protected JumpCircle jumpDust;

        protected TileMap tileMap;
        protected WorldManager worldManager;
        protected EntityManager entityManager;

        protected EntityType entityType;
        public EntityType Type { get { return entityType; } }

        protected bool isSavable;
        public bool IsSavable { get { return isSavable; } }

        private ActionPackage currentAction = new ActionPackage();
        public ActionPackage CurrentAction() { return currentAction; }
        public void SetAction(string action) { currentAction.SetAction(action); }

        private int restCount = 0;
        public int RestCount { get { return restCount; } }

        FlameRenown renown = new FlameRenown(0); //Realistically, this will only be used by the player, and slightly by characters.

        protected Texture2D pixel;
        protected bool isDrawCalled = true;

        public BaseEntity() : base(-1, 0, 0) { }
        public BaseEntity(int ID, string Name, AnimationState Animation, Skillset Skillset, ObjectAttributes Attributes, EntityLoot Drops, EntityStorage Storage, BaseFaction Faction,
                          ObjectSenses Senses, EntityKin Kin, float CircleRadius, float CenterOffset, float DepthOffset, float ShadowOffset, float InfoHeight, BaseAgent AgentAI) : base(-1, -1, 0f)
        {
            id = ID;
            name = Name;

            depthFloor = new DepthFloor();
            animation = Animation;
            skillset = Skillset;
            attributes = Attributes;
            skillset.SetReferences(attributes);

            equipment = new EntityEquipment();
            loot = Drops;
            storage = Storage;

            status = new EntityStatus(this);
            visualOverlay = new EntityVisual();
            info = new EntityInfo(name, skillset.health.MaxHP);
            statistics = new ObjectStatistics(null, true);

            chat = new EntityChat(name);

            faction = Faction;
            senses = Senses;
            kin = Kin;
            memory = new ObjectMemory();

            baseEntityCircle = new Circle(CircleRadius);
            entityCircle = new Circle(CircleRadius);
            particleCircle = new Circle(CircleRadius);
            baseJumpCircle = new Circle(CircleRadius);
            jumpCircle = new Circle(CircleRadius);
            selectionCircle = new Circle(CircleRadius * 2);

            centerOffset = CenterOffset;
            depthOffset = DepthOffset;
            shadowYOrigin = ShadowOffset;
            infoHeight = InfoHeight;

            agentAI = AgentAI;

            random = new Random(Guid.NewGuid().GetHashCode());
            isUseTileLocation = false;
            IsObjectMovable = true;

            baseEntityHitbox = new Rectangle(0, 0, 16, 16);
            entityHitbox = baseEntityHitbox;
        }

        public void SetReferences(Camera Camera, TileMap Map, DebugManager Debug, WorldManager WorldManager, ScreenManager Screens, EntityManager Entities, CultureManager Culture, PlayerEntity Player, WeatherManager weather)
        {
            camera = Camera;
            tileMap = Map;
            debug = Debug;
            worldManager = WorldManager;
            screens = Screens;
            entityManager = Entities;
            culture = Culture;
            playerEntity = Player;
            this.weather = weather;

            equipment.SetReferences(screens, storage, this);
            loot.SetReferences(screens, tileMap, this, debug, camera);
            typing.SetReferences(this, new Vector2(GameSettings.VectorCenter.X, GameSettings.VectorResolution.Y - (GameSettings.VectorCenter.Y / 4)));
            visualOverlay.SetReferences(map, camera, screens, player, weather, culture, controlledEntity, entities);

            storage.SetReferences(screens, tileMap, this, camera);

            Initialize();
        }
        public virtual void SetControlledEntity(BaseEntity controlledEntity)
        {
            this.controlledEntity = controlledEntity;
        }

        public virtual void SetMapInfo(PathfindMap pathfindMap) { }

        public virtual void Initialize()
        {
            maxBounce = 1;
            bounceHeight = .35f;

            agentAI.SetReferences(this, tileMap);
        }

        public override void Load(ContentManager cm)
        {
            status.LoadContent(cm);
            info.Load(cm);
            senses.Load(cm); typing.Load(cm);

            caption = new EntityCaption(new Vector2(0, -55));
            caption.Load(cm);

            jumpBar = cm.Load<Texture2D>("Interface/jumpBar");
            jumpFiller = cm.Load<Texture2D>("Interface/jumpBarFiller");

            deathImage.Load(cm);

            pixel = cm.Load<Texture2D>("rect");
        }

        protected Point startTile; private int startFloor; private float lookDirection;
        public Point StartTile { get { return startTile; } }
        public void SetValues(int objectID, int currentFloor, Point tile, float lookDirection, bool isSavable)
        {
            base.id = objectID;
            depthFloor.CurrentFloor = currentFloor;

            startFloor = currentFloor;
            this.lookDirection = lookDirection;

            startTile = tile;
            SetTile(tile);

            SENSES_SightDirection = lookDirection;
            SENSES_CurrentDirection = lookDirection;

            this.isSavable = isSavable;
        }

        private bool isDeathReset = false, isBodyDisappear = false, isDeathParticleAdded = false;
        private int deathWaitTime = 0;
        public override void Update(GameTime gt)
        {
            UpdateDepth(depthOffset);
            //depthFloor.UpdateDepth(position + depthOffset);

            controls.UpdateLast();
            controls.UpdateCurrent();

            mouseToWorld = camera.ScreenToWorld(controls.MouseVector);
            mouseDirection = Position.Direction(mouseToWorld);

            animation.Update(gt);

            currentAction.Update(gt);

            if (IsDead == true) //Entity is dead, set values
                UpdateDeath(gt);
            else
                UpdateAlive(gt);

            UpdateCombat(gt, SENSES_CurrentDirection, IsPlayerControlled);

            caption.Update(gt);
            storage.Update(gt);

            memory.Update(gt);

            ManageMessages(gt);
            SuspensionManager(gt);

            UpdateCompanions(gt);
            UpdateColorLerp(gt);

            base.Update(gt);

            entityHitbox.X = (int)Position.X - (entityHitbox.Width / 2);
            entityHitbox.Y = (int)Position.Y - (entityHitbox.Height);
            entityCircle.Position = position; //Position offset
            particleCircle.Position = position; //Position offset
            jumpCircle.Position = Position; //Position offset plus heightOffset!

            deathImage.Offset = Position - new Vector2(animation.FrameSize.X, animation.FrameSize.Y);
            deathImage.Update(gt);

            jumpDust.Offset = Position; //Center

            refPosition = Position;
        }

        // --- Death-related ---
        protected virtual void UpdateDeath(GameTime gt)
        {
            if (isDeathReset == false)
            {
                InitiateDeath();
                isDeathReset = true;
            }

            animation.Look = AnimationState.Direction.Down;
            STATE_SetAction("Dead");

            if (ANIMATION_IsFinished == true) //Death animation is complete, pause the animation at final frame
            {
                ANIMATION_Pause();
                FinalizeDeath();

                if (deathWaitTime >= 500)
                {
                    isBodyDisappear = true;
                    entityCircle.radius = 0f;
                    jumpCircle.radius = 0f;

                    if (isDeathParticleAdded == false)
                    {
                        deathImage.IsAdded = false;
                        isDeathParticleAdded = true;
                    }
                }
                else
                    deathWaitTime += gt.ElapsedGameTime.Milliseconds;
            }
        }
        protected virtual void UpdateAlive(GameTime gt)
        {
            skillset.UpdateSkillset(gt);
            status.Update(gt);

            //CAPTION_SendImmediate(EQUIPMENT_ArmorWeight() + " " + skillset.defense.PhysicalDefenseMultiplier + " " + skillset.defense.ProjectileDefenseMultiplier + " " + skillset.defense.MagicalDefenseMultiplier);

            RegenerateStamina(gt);

            visualOverlay.Update(gt);
            equipment.Update(gt);

            UpdateControlType(gt); //Update movement of entity (both player and monster)
            UpdateCollision(gt);

            CheckWeaponBehavior(gt, EQUIPMENT_PrimaryWeapon());
            CheckWeaponBehavior(gt, EQUIPMENT_OffhandWeapon());

            ProcessDamage(gt);

            if (MERCHANT_IsMerchant() == true)
                merchant.UpdateRemoval();

            SearchNearEntities(gt);
            CheckChatOptions(gt);
        }
        private void UpdateColorLerp(GameTime gt)
        {
            colorLerp += 2.5f * (float)gt.ElapsedGameTime.TotalSeconds;
            colorLerp = MathHelper.Clamp(colorLerp, -1f, 1f);

            entityColor = Color.Lerp(entityTargetColor, Color.White, colorLerp);
        }

        // --- Companion-related ---
        private CallLimiter limitCompanionCheck = new CallLimiter(1000);
        protected virtual void UpdateCompanions(GameTime gt)
        {
            if (limitCompanionCheck.IsCalling(gt))
            {
                companions.Clear();
                for (int i = 0; i < mapEntities.Count; i++)
                {
                    if (mapEntities[i].IsCompanionLeader(this))
                    {
                        companions.Add(mapEntities[i]);
                    }
                }
            }
        }

        protected bool isPlayerControlled = false;
        public bool IsPlayerControlled { get { return isPlayerControlled; } }
        public void SetPlayerControlled(bool value)
        {
            isPlayerControlled = value;

            if (value == true)
            {
                screens.INVENTORY_SetItemData(this, storage, equipment);
                screens.SOULS_SetData(equipment, storage);
                screens.Container_SetEntityData(this, storage);
                screens.SKILLS_SetData(skillset, this);
                screens.HUD_Set(this);

                tileMap.SetControlledEntity(this);
            }
        }

        protected void UpdateControlType(GameTime gt)
        {
            CheckSelectedObjects(gt);

            if (IsPlayerControlled == true)
                UpdatePlayerControls(gt);

            if (SUSPENSION_Movement == Suspension.SuspendState.None)
                STATE_SetLook(MathHelper.ToDegrees(senses.CurrentDirection));

            STATE_SetMovement(lastMotion);

            UpdateSpeed(gt);
            UpdateEntityMovement(gt);

            if (!animation.IsPerforming("Dodge"))
                movementMotion = Vector2.Zero;
            controlledMotion = Vector2.Zero;

            if (IsPlayerControlled && camera.state == Camera.CameraState.Current)
                camera.LookAt(Position + new Vector2(0, centerOffset));
        }

        protected void UpdatePlayerControls(GameTime gt)
        {
            typing.Update(gt);
            CheckCompanionCommandControls(gt);
            equipment.UpdateShortcuts(gt, controls);

            if (IsAirborne() == false && !animation.IsPerforming("Dodge")) //Prevent entity from changing direction mid-flight
            {
                if (controls.CurrentKey.IsKeyDown(controls.CurrentControls.MoveUp))
                    SetMotionY(-1);
                else if (controls.CurrentKey.IsKeyDown(controls.CurrentControls.MoveDown))
                    SetMotionY(1);

                if (controls.CurrentKey.IsKeyDown(controls.CurrentControls.MoveLeft))
                    SetMotionX(-1);
                else if (controls.CurrentKey.IsKeyDown(controls.CurrentControls.MoveRight))
                    SetMotionX(1);

                IsActivatingObject = false; //Temporary. See below comment.
                if (controls.IsKeyPressedOnce(controls.CurrentControls.Activate) && SUSPENSION_Action == Suspension.SuspendState.None)
                {
                    UseSelectedObject();
                    ActivateObject(); //Temporary, until all GameObjects are converted to the "UseSelectedObject()" style method(s).
                }

                if (controls.IsKeyPressedOnce(controls.CurrentControls.SwapWeaponUse))
                    isPrimaryUse = !isPrimaryUse;
            }

            if (SUSPENSION_Action == Suspension.SuspendState.None && camera.state != Camera.CameraState.Debug)
            {
                if (controls.IsKeyPressedOnce(controls.CurrentControls.Roll) && isStaminaWaiting == false)
                {
                    Vector2 dir = camera.ScreenToWorld(controls.MouseVector) - position;
                    if (dir != Vector2.Zero)
                        dir.Normalize();

                    Dodge(dir);
                }
            }

            if (SUSPENSION_Action == Suspension.SuspendState.None && COMBAT_IsStateSetting())
            {
                UpdatePlayerState(gt);

                SENSES_CurrentDirection = (Position + new Vector2(0, centerOffset)).Direction(mouseToWorld);
                SENSES_SightDirection = SENSES_CurrentDirection;
            }
        }
        private void CheckCompanionCommandControls(GameTime gt)
        {
            if (controls.IsKeyPressedOnce(controls.CurrentControls.Command0))
            {
                //Open companion interface
            }
            if (controls.IsKeyPressedOnce(controls.CurrentControls.Command1))
            {
                CAPTION_SendImmediate("Follow me!");
                SendMessage(new MessageHolder(this, null, this.faction, "Companions", "Follow", 750, 100));
            }
            if (controls.IsKeyPressedOnce(controls.CurrentControls.Command2))
            {
                CAPTION_SendImmediate("Go over there!");
                SendMessage(new MessageHolder(this, null, this.faction, "Companions", "GoOver", 750, 100));
            }
            if (controls.IsKeyPressedOnce(controls.CurrentControls.Command3))
            {
                CAPTION_SendImmediate("Stand guard!");
                SendMessage(new MessageHolder(this, null, this.faction, "Companions", "Guard", 750, 100));
            }

            if (controls.IsKeyPressedOnce(controls.CurrentControls.Command4))
            {
                CAPTION_SendImmediate("Retreat!");
            }
            if (controls.IsKeyPressedOnce(controls.CurrentControls.Command5))
            {
                CAPTION_SendImmediate("Teleport away!");
            }
            if (controls.IsKeyPressedOnce(controls.CurrentControls.Command6))
            {
                CAPTION_SendImmediate("I need you to do something.");
                SendMessage(new MessageHolder(this, null, this.faction, "Companions", "Activate", 750, 100));
            }

            if (controls.IsKeyPressedOnce(controls.CurrentControls.Command7))
            {
                CAPTION_SendImmediate("Use the item!");
            }
            if (controls.IsKeyPressedOnce(controls.CurrentControls.Command8))
            {
                CAPTION_SendImmediate("Go collect that item over there.");
            }
            if (controls.IsKeyPressedOnce(controls.CurrentControls.Command9))
            {
                CAPTION_SendImmediate("Let me assist you.");
                SendMessage(new MessageHolder(this, null, this.faction, "Companions", "Control", 750, 100));
            }
        }

        private void UpdatePlayerState(GameTime gt)
        {
            if (!animation.IsPerforming("Dodge"))
            {
                if (controls.CurrentKey.IsKeyDown(controls.CurrentControls.Sprint) && isStaminaWaiting == false)
                {
                    if (IsInputMoving())
                    {
                        STATE_SetAction("Sprint");
                        STAMINA_Damage((10f + (EQUIPMENT_ArmorWeight() * .1f)) * (float)gt.ElapsedGameTime.TotalSeconds);
                    }
                }
                else if (controls.CurrentKey.IsKeyDown(controls.CurrentControls.Sneak))
                {
                    if (IsInputMoving())
                    {
                        STATE_SetAction("Sneak");
                        STAMINA_Restore(5f * (float)gt.ElapsedGameTime.TotalSeconds); //Restore stamina at half-rate while sneaking
                    }
                    else
                        STATE_SetAction("SneakIdle");
                }
                else if (IsInputMoving() == true)
                    STATE_SetAction("Walk");
                else
                    STATE_SetAction("Idle");
            }

            UpdateJump(gt);
            UpdateDodge(gt);
        }

        private void UpdateJump(GameTime gt)
        {
            displayJumpBar = false;

            if (IsAirborne() == false)
            {
                if (controls.CurrentKey.IsKeyDown(controls.CurrentControls.Jump))
                {
                    spaceBarJump += 1f * (float)gt.ElapsedGameTime.TotalSeconds;
                    spaceBarJump = MathHelper.Clamp(spaceBarJump, 0f, 1f);

                    if (spaceBarJump > .2f)
                        displayJumpBar = true;
                }
                else if (spaceBarJump > 0f && spaceBarJump < .2f)
                {
                    spaceBarJump = 1f;
                    Jump(skillset.agility.jumpHeight, spaceBarJump);
                    spaceBarJump = 0f;

                    jumpDust.Velocity = 300f;
                }
                else if (spaceBarJump != 0f)
                {
                    jumpDust.Velocity = 300f * spaceBarJump;

                    Jump(MathHelper.Clamp(skillset.agility.jumpHeight * spaceBarJump, skillset.agility.jumpHeight / 2, skillset.agility.jumpHeight), spaceBarJump);
                    spaceBarJump = 0f;
                }
            }

            /*jumpDust.IsActivated = false;
            if (jumping.CurrentState == Jumping.JumpState.Landed)
            {
                if (jumpDust.Velocity >= 100f)
                {
                    //tileMap.AddEmitter(jumpDust);
                    //jumpDust.IsActivated = true;
                }
            }*/
        }
        private void UpdateDodge(GameTime gt)
        {
            if (animation.IsPerforming("Dodge"))
            {
                entityHitbox.Width = 0;
                entityHitbox.Height = 0;

                if (Vector2.Distance(position, dodgeStart) >= (skillset.agility.DodgeDistance * EQUIPMENT_WeightSpeedMultiplier))
                {
                    STATE_SetAction("Idle");

                    entityHitbox.Width = baseEntityHitbox.Width;
                    entityHitbox.Height = baseEntityHitbox.Height;
                }
            }
        }

        private float maxSpeed, speedLerp, newSpeed;
        public float MaxSpeed { get { return maxSpeed; } set { maxSpeed = MathHelper.Clamp(value, 0, skillset.agility.SprintSpeed); } }
        public void ResetMovement() { lastMotion = Vector2.Zero; MaxSpeed = 0f; }

        private float slipperiness = 1f, acceleration = 1f;
        /// <summary>
        /// 1f = default, .5f = slippery, .1f = very slippery
        /// </summary>
        public float Slipperiness { get { return slipperiness; } set { slipperiness = MathHelper.Clamp(value, .05f, 1f); } }
        /// <summary>
        /// 1f = default, .5f = slow acceleration, .1f = very slow acceleration
        /// </summary>
        public float Acceleration { get { return acceleration; } set { acceleration = MathHelper.Clamp(value, .1f, 1f); } }
        protected void UpdateSpeed(GameTime gt)
        {
            if (IsAirborne() == false) //Prevent entity from changing jump speed mid-flight
            {
                if (IsInputMoving() && SUSPENSION_Movement == Suspension.SuspendState.None && COMBAT_IsStateSetting())
                    speedLerp += (7f * acceleration) * (float)gt.ElapsedGameTime.TotalSeconds;
                else
                    speedLerp -= (5f * slipperiness) * (float)gt.ElapsedGameTime.TotalSeconds;

                speedLerp = MathHelper.Clamp(speedLerp, 0f, 1f);

                CalculateMaxSpeed(AGILITY_WalkSpeed(), "Walk");
                CalculateMaxSpeed(AGILITY_SprintSpeed(), "Sprint");
                CalculateMaxSpeed(AGILITY_SneakSpeed(), "Sneak");
                CalculateMaxSpeed(skillset.agility.DodgeSpeed * EQUIPMENT_WeightSpeedMultiplier, "Dodge");

                maxSpeed = MathHelper.Lerp(0f, newSpeed, speedLerp);
                Speed = MaxSpeed;
            }
        }
        protected void CalculateMaxSpeed(float newSpeed, string action)
        {
            if (animation.IsPerforming(action))
            {
                this.newSpeed = newSpeed;
                CalculateLerpSpeed(MaxSpeed, newSpeed);
            }
        }
        private bool isStateChanged = false;
        private void CalculateLerpSpeed(float lastAgilSpeed, float maxAgilSpeed)
        {
            if (animation.LastAction != animation.CurrentAction)
                isStateChanged = true;

            if (isStateChanged == true)
            {
                if (lastAgilSpeed < maxAgilSpeed)
                    speedLerp = (lastAgilSpeed / maxAgilSpeed);

                isStateChanged = false;
            }
        }

        Vector2 finalMotion, movementMotion, controlledMotion;
        Vector2 outputMotion, lastMotion;
        public Vector2 FinalMotion { get { return finalMotion; } set { finalMotion = value; } }
        public Vector2 MovementMotion
        {
            get { return movementMotion; }
        }

        /// <summary>
        /// For entity movement input only! For weapons and other controlling factors, add velocity to ControlMotion(...)
        /// </summary>
        /// <param name="x"></param>
        public void SetMotionX(float x)
        {
            if (SUSPENSION_Movement == Suspension.SuspendState.None)
                movementMotion.X += x;
        }
        public void SetMotionY(float y)
        {
            if (SUSPENSION_Movement == Suspension.SuspendState.None)
                movementMotion.Y += y;
        }
        public void SetMovementMotion(Vector2 motion)
        {
            if (SUSPENSION_Movement == Suspension.SuspendState.None)
            {
                movementMotion.X += motion.X;
                movementMotion.Y += motion.Y;
            }
        }

        /// <summary>
        /// For other controlling factors.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="speedAmplifier"></param>
        public void ControlMotion(float x, float y, bool isArmorWeightMultiplied = false)
        {
            if (isArmorWeightMultiplied == true)
            {
                controlledMotion.X += x * EQUIPMENT_WeightSpeedMultiplier;
                controlledMotion.Y += y * EQUIPMENT_WeightSpeedMultiplier;
            }
            else
            {
                controlledMotion.X += x;
                controlledMotion.Y += y;
            }
        }

        public Vector2 OutputMotion { get { return outputMotion; } }
        public bool IsInputMoving() { return !(outputMotion == Vector2.Zero); }

        protected void UpdateEntityMovement(GameTime gt)
        {
            if (movementMotion != Vector2.Zero)
            {
                movementMotion.Normalize();
                lastMotion = movementMotion;
            }
            outputMotion = movementMotion;

            finalMotion = (lastMotion * Speed) + controlledMotion;

            position += finalMotion * (float)gt.ElapsedGameTime.TotalSeconds;
            pathfindTile = new Point((int)(position.X / Pathfinder.TileSize.X), (int)(position.Y / Pathfinder.TileSize.Y));
        }

        // --- Entity collision detection ---
        int addNew = 200;
        protected void UpdateCollision(GameTime gt)
        {
            for (int i = 0; i < nearColliders.Count; i++)
            {
                //If the character's height offset is outside of the jump-able range, push them out
                if (heightOffset <= nearColliders[i].MinJumpHeight || heightOffset >= nearColliders[i].MaxJumpHeight)
                {
                    if (nearColliders[i].Intersects(entityCircle))
                        position -= nearColliders[i].PushOutCircle(entityCircle.Position, entityCircle.radius);
                }
            }

            AddColliders(gt);
        }
        private void AddColliders(GameTime gt)
        {
            addNew += gt.ElapsedGameTime.Milliseconds;
            if (addNew >= 200)
            {
                nearColliders.Clear();
                for (int i = 0; i < totalColliders.Count; i++)
                {
                    if (totalColliders[i].InRange(entityCircle.Position, entityCircle.radius) == true &&
                        IsTouchable(totalColliders[i].CurrentFloor) == true)
                        nearColliders.Add(totalColliders[i]);
                }
                nearColliders = nearColliders.OrderBy(x => x.DistanceTo(position)).ToList();
                addNew = 0;
            }
        }
        public void PushEntity(BaseEntity entity)
        {
            if (!IsDead && !entity.IsDead)
            {
                position += entity.EntityCircle.Repel(position, .5f);
                entity.position -= EntityCircle.Repel(position, .5f);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (isBodyDisappear == false)
                animation.DrawSprite(sb, new Vector2(Position.X, Position.Y + centerOffset), Vector2.Zero, entityColor, Depth, shadowHeightOffset + shadowYOrigin);

            deathImage.Draw(sb);

            visualOverlay.Draw(sb);
            caption.Draw(sb, Position - new Vector2(0, infoHeight - 15));

            if (displayJumpBar == true && IsPlayerControlled)
            {
                sb.Draw(jumpBar, position + new Vector2(0, 48), Color.White, jumpBar.Center(), 0f, 1f);
                sb.Draw(jumpFiller, position + new Vector2(0, 48), new Rectangle(0, 0, (int)(jumpFiller.Width * spaceBarJump), jumpFiller.Height), Color.Beige, 0f, jumpFiller.Center(), 1f, SpriteEffects.None, 1f);
            }

            if (IsDead == false)
            {
                senses.Depth = Depth - .0001f;
                senses.Position = Position + new Vector2(0, centerOffset);
                senses.Draw(sb);

                if (EQUIPMENT_PrimaryCombat() != null)
                    EQUIPMENT_PrimaryCombat().Draw(sb);

                if (EQUIPMENT_OffhandCombat() != null)
                    EQUIPMENT_OffhandCombat().Draw(sb);

                if (GameSettings.IsDebugging == true)
                {
                    if (EQUIPMENT_PrimaryCombat() != null)
                        EQUIPMENT_PrimaryCombat().DrawDebug(sb);

                    if (EQUIPMENT_OffhandCombat() != null)
                        EQUIPMENT_OffhandCombat().DrawDebug(sb);
                }
            }
        }
        public override void DrawUI(SpriteBatch sb)
        {
            if (IsPlayerControlled == true)
            {
                if (screens.EFFECTS_IsWidescreen() == false)
                {
                    status.UpdateRow(new Vector2(screens.HUD_Offset.X + MathHelper.SmoothStep(30, 188, screens.HUD_MainOpacity + .5f), screens.HUD_Offset.Y + 52));
                    status.Draw(sb);
                }

                typing.Draw(sb);

                /*
                if (SUSPENSION_Action == Suspension.SuspendState.Suspended)
                {
                    Vector2 barPosition = camera.WorldToScreen(position + new Vector2(entityCircle.radius * 2.5f, -64));

                    sb.DrawBoxBordered(pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, 2, 50), Color.Transparent, ColorHelper.Charcoal);

                    int barHeight = (int)(50 * ((float)actionSuspension.CurrentTime / actionSuspension.MaxTime));
                    sb.Draw(pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y - barHeight + 50, 2, barHeight), Color.DarkOrange);
                }*/
            }

            if (GameSettings.IsDebugging == true)
            {
                Vector2 tempPos = camera.WorldToScreen(Position);

                sb.DrawBoxBordered(pixel, new Rectangle((int)tempPos.X - 1, (int)tempPos.Y - 1, 3, 3), Color.White, Color.Black, 1f);
                sb.DrawBoxBordered(pixel, new Rectangle((int)tempPos.X - 1, (int)(tempPos.Y - 1) + (int)centerOffset, 3, 3), Color.SkyBlue, Color.Black, 1f);
                sb.DrawBoxBordered(pixel, new Rectangle((int)tempPos.X - 1, (int)(tempPos.Y - 1) + (int)depthOffset, 3, 3), Color.Yellow, Color.Black, 1f);

                Vector2 rectPos = camera.WorldToScreen(new Vector2(Hitbox.Location.X, Hitbox.Location.Y));
                sb.DrawBoxBordered(pixel, new Rectangle((int)rectPos.X, (int)rectPos.Y, Hitbox.Width, Hitbox.Height), Color.Transparent, Color.Red);
            }
        }
        protected void DrawInfo(SpriteBatch sb)
        {
            if (isBodyDisappear == false)
            {
                if (caption.IsTalking)
                    info.Offset = infoHeight + 25;
                else
                    info.Offset = infoHeight;

                info.CurrentHealth = skillset.health.CurrentHP;
                if (IsPlayerControlled == false)
                    info.SetOpacity(Vector2.Distance(camera.ScreenToWorld(controls.MouseVector), position), entityCircle.radius * 1.5f);
                else
                    info.SetOpacity(1f, 2f);

                info.Draw(sb, Position + new Vector2(0, centerOffset));
            }
        }

        public void AssignMapID(int entityMapID)
        {
            mapEntityID = name + entityMapID.ToString();
            info.EntityID = mapEntityID;
        }
        public void AddEntities(List<BaseEntity> entities)
        {
            mapEntities.Clear();
            dispositions.Clear();

            for (int i = 0; i < entities.Count; i++)
            {
                if (!entities[i].Equals(this)) //Exclude the current class.
                {
                    mapEntities.Add(entities[i]); //Get the reference, still.
                    dispositions.Add(new EntityDisposition(entities[i]));
                }
            }

            SelectValidTargets();

            deathImage.SetReferences(tileMap, camera, screens, null, null, null, null, mapEntities);
        }

        private void SelectValidTargets()
        {
            for (int i = 0; i < mapEntities.Count; i++)
            {
                if (FACTION_IsEnemy(mapEntities[i].Faction)) //Entity has found a target that is of an opposing faction. Add it to the validEnemies list.
                    validEnemies.Add(mapEntities[i]);
                else if (FACTION_IsAlly(mapEntities[i].Faction))
                    validAllies.Add(mapEntities[i]);
            }

            for (int i = 0; i < dispositions.Count; i++)
            {
                //if (FACTION_IsEnemy(dispositions[i].Entity.Faction))
                    dispositions[i].SetDisposition(-50);
                //if (FACTION_IsAlly(dispositions[i].Entity.Faction))
                //    dispositions[i].SetDisposition(30);
            }
        }

        private CallLimiter limitSearchNearEntities = new CallLimiter(200);
        private void SearchNearEntities(GameTime gt)
        {
            if (limitSearchNearEntities.IsCalling(gt))
            {
                for (int i = 0; i < validEnemies.Count; i++)
                {
                    if (!currentEnemies.Contains(validEnemies[i]))
                    {
                        ObjectSenses.SightType type = SENSES_IsInSight(validEnemies[i].Position);

                        if (type == ObjectSenses.SightType.Spotted && validEnemies[i].IsDead == false)
                            currentEnemies.Add(validEnemies[i]);
                    }
                }

                for (int i = 0; i < currentEnemies.Count; i++)
                {
                    if (Vector2.Distance(currentEnemies[i].Position, position) >= SENSES_SightLength * 2f || currentEnemies[i].IsDead == true)
                        currentEnemies.RemoveAt(i);
                }

                for (int i = 0; i < validAllies.Count; i++)
                {
                    if (SENSES_IsInSight(validAllies[i].Position) == ObjectSenses.SightType.Spotted)
                        currentAllies.Add(validAllies[i]);
                }

                currentEnemies.Sort((e1, e2) => Vector2.Distance(e1.Position, Position).CompareTo(Vector2.Distance(e2.Position, Position))); //Sort by closest target
            }
        }

        #region [Encapsulation] Suspension

        private Suspension actionSuspension = new Suspension(), //Actions includes stuff like consuming items or swapping weapons
                           movementSuspension = new Suspension(), //Self-explanatory.
                           messageSuspension = new Suspension(); //... Not in use?

        private void SuspensionManager(GameTime gt)
        {
            actionSuspension.ManageSuspension(gt);
            movementSuspension.ManageSuspension(gt);
            messageSuspension.ManageSuspension(gt);
        }

        public Suspension.SuspendState SUSPENSION_Action { get { return actionSuspension.SuspensionState; } }
        public Suspension.SuspendState SUSPENSION_Movement { get { return movementSuspension.SuspensionState; } }
        public Suspension.SuspendState SUSPENSION_Message { get { return messageSuspension.SuspensionState; } }

        public void SUSPEND_Action(int milliseconds) { actionSuspension.Suspend(milliseconds); }
        public void SUSPEND_Movement(int milliseconds) { movementSuspension.Suspend(milliseconds); }
        public void SUSPEND_Message(int milliseconds) { messageSuspension.Suspend(milliseconds); }

        #endregion

        #region [Encapsulation] Message Managing

        private List<MessageHolder> unprocessedMessages = new List<MessageHolder>();
        private List<MessageHolder> lastProcessedMessages = new List<MessageHolder>();

        public void AddMessage(MessageHolder message)
        {
            int notEquals = 0;
            for (int i = 0; i < lastProcessedMessages.Count; i++)
            {
                if (lastProcessedMessages[i].Message.ToUpper() != message.Message.ToUpper() &&
                    lastProcessedMessages[i].SubMessage.ToUpper() != message.SubMessage.ToUpper()) //If the message has not been received recently, add it to the list. This will help prevent echo-messages
                {
                    notEquals++;
                }
            }

            if (notEquals == lastProcessedMessages.Count)
                unprocessedMessages.Add(message);
        }
        private void ManageMessages(GameTime gt)
        {
            if (SUSPENSION_Message == Suspension.SuspendState.None)
            {
                for (int i = 0; i < unprocessedMessages.Count; i++)
                {
                    unprocessedMessages[i].Timer -= gt.ElapsedGameTime.Milliseconds;

                    if (unprocessedMessages[i].Timer <= 0)
                    {
                        HandleMessage(unprocessedMessages[i]); //Send message along its way

                        lastProcessedMessages.Add(unprocessedMessages[i].Copy()); //Add the message to the latest processed messages list

                        unprocessedMessages.RemoveAt(i); i--; //Remove the message from the unprocessed messages.
                    }
                }

                for (int j = 0; j < lastProcessedMessages.Count; j++)
                {
                    lastProcessedMessages[j].Cooldown -= gt.ElapsedGameTime.Milliseconds;

                    if (lastProcessedMessages[j].Cooldown <= 0) //If time has passed, remove the processed message
                        lastProcessedMessages.RemoveAt(j);

                    if (lastProcessedMessages.Count >= 20) //Or if there are too many processed messages, remove the first one added.
                        lastProcessedMessages.Remove(lastProcessedMessages.First());
                }
            }
        }

        public void SendMessage(MessageHolder message)
        {
            for (int i = 0; i < mapEntities.Count; i++)
            {
                if (Vector2.Distance(position, mapEntities[i].Position) <= senses.VoiceRadius)
                    mapEntities[i].AddMessage(message);
            }
        }
        public void SendMessage(MessageHolder message, float forceVoiceRadius)
        {
            for (int i = 0; i < mapEntities.Count; i++)
            {
                if (Vector2.Distance(position, mapEntities[i].Position) <= forceVoiceRadius)
                    mapEntities[i].AddMessage(message);
            }
        }
        public void SendMessage(object sender, object baggage, BaseFaction faction, int time, int cooldown, string message, string subMessage, bool isRandomResponseTime = false, int minTime = 1000, int maxTime = 2500, bool isRandomCooldown = false, int minCooldown = 500, int maxCooldown = 1000)
        {
            for (int i = 0; i < mapEntities.Count; i++)
            {
                if (Vector2.Distance(position, mapEntities[i].Position) <= senses.VoiceRadius)
                {
                    if (isRandomResponseTime == true)
                        time = random.Next(minTime, maxTime);
                    if (isRandomCooldown == true)
                        cooldown = random.Next(minCooldown, maxCooldown);

                    MessageHolder m = new MessageHolder(sender, baggage, faction, message, subMessage, time, cooldown);
                    mapEntities[i].AddMessage(m);
                }
            }
        }

        protected virtual void HandleMessage(MessageHolder message)
        {
            if (message.Message.ToUpper().Equals("CHAT"))
            {
                CAPTION_Queue(chat.RetrieveRandom(message.SubMessage));
            }

            if (message.Message.ToUpper().Equals("SAY"))
            {
                string[] msgSubMsg = message.SubMessage.Split(new char[] { ':' }, 2);

                string m = msgSubMsg[0];
                string subMessage = "";

                if (msgSubMsg.Length > 1)
                    subMessage = msgSubMsg[1];

                CAPTION_Queue(message.SubMessage);
                SendMessage(new MessageHolder(this, null, faction, m, subMessage, 1000, 1500));
            }

            if (message.Message.ToUpper().Equals("JUMP"))
                Jump();
        }

        #endregion

        #region [Encapsulation] Status Managing
        public void STATUS_AddStatus(int id, string entityID)
        {
            status.AddStatus(id);

            if (status.statuses.Count > 0)
                status.statuses.Last().KillerID = entityID;
        }
        public void STATUS_RemoveStatus(int id)
        {
            status.RemoveStatus(id);
        }
        public bool STATUS_CheckForStatus(int id)
        {
            return status.CheckForStatus(id);
        }
        public int STATUS_SecondsLeft(int id)
        {
            return status.SecondsLeft(id);
        }
        public void STATUS_ClearEffects()
        {
            status.ForceClear();
        }
        #endregion

        #region [Encapsulation] Visual Managing
        public void VISUAL_AddVisual(int id) { visualOverlay.AddVisual(id, this); }
        public void VISUAL_BeginStop(int id) { visualOverlay.StopVisual(id); }
        public void VISUAL_StopAll() { visualOverlay.StopAllVisuals(); }
        public void VISUAL_Clear() { visualOverlay.Clear(); }
        #endregion

        #region [Encapsulation] Attribute Managing

        public float ATTRIBUTE_GetMultiplier(string name, float nullDefault = 1f)
        {
            return attributes.GetMultiplier(name, nullDefault);
        }
        public void ATTRIBUTE_SetMultiplier(string name, float value)
        {
            attributes.SetMultiplier(name, value);
        }
        public void ATTRIBUTE_AdjustMultiplier(string name, float value)
        {
            attributes.AdjustMultiplier(name, value);
        }

        #region Attribute Names
        public static readonly string[] ATTRIBUTE_CombatPhysicalMultipliers = new string[]
        {
            ATTRIBUTE_EquipmentPhysicalDefense,
            ATTRIBUTE_SkillsPhysicalDefense,
            ATTRIBUTE_SkillsPhysicalDefenseMultiplier
        };
        public static readonly string[] ATTRIBUTE_CombatProjectileMultipliers = new string[]
        {
            ATTRIBUTE_EquipmentProjectileDefense,
            ATTRIBUTE_SkillsProjectileDefense,
            ATTRIBUTE_SkillsProjectileDefenseMultiplier
        };
        public static readonly string[] ATTRIBUTE_CombatMagicMultipliers = new string[]
        {
            ATTRIBUTE_EquipmentMagicalDefense,
            ATTRIBUTE_SkillsMagicalDefense,
            ATTRIBUTE_SkillsMagicalDefenseMultiplier
        };

        public const string ATTRIBUTE_EquipmentPhysicalDefense = "EquipmentPhysicalDefense";
        public const string ATTRIBUTE_EquipmentProjectileDefense = "EquipmentProjectileDefense";
        public const string ATTRIBUTE_EquipmentMagicalDefense = "EquipmentMagicDefense";
        public const string ATTRIBUTE_EquipmentEffectAmplifier = "EquipmentEffectAmplifier";
        public const string ATTRIBUTE_EquipmentEffectResistance = "EquipmentEffectResistance";
        public const string ATTRIBUTE_EquipmentWeight = "EquipmentWeight";

        public const string ATTRIBUTE_SkillsPhysicalDefense = "SkillsPhysicalDefense";
        public const string ATTRIBUTE_SkillsProjectileDefense = "SkillsProjectileDefense";
        public const string ATTRIBUTE_SkillsMagicalDefense = "SkillsMagicalDefense";
        public const string ATTRIBUTE_SkillsPhysicalDefenseMultiplier = "SkillsPhysicalDefenseMultiplier";
        public const string ATTRIBUTE_SkillsProjectileDefenseMultiplier = "SkillsProjectileDefenseMultiplier";
        public const string ATTRIBUTE_SkillsMagicalDefenseMultiplier = "SkillsMagicalDefenseMultiplier";
        #endregion

        #region Attribute Methods
        public float EQUIPMENT_PhysicalDefense() { return ATTRIBUTE_GetMultiplier(ATTRIBUTE_EquipmentPhysicalDefense, 0); }
        public float EQUIPMENT_ProjectileDefense() { return ATTRIBUTE_GetMultiplier(ATTRIBUTE_EquipmentProjectileDefense, 0); }
        public float EQUIPMENT_MagicalDefense() { return ATTRIBUTE_GetMultiplier(ATTRIBUTE_EquipmentMagicalDefense, 0); }
        public float EQUIPMENT_EffectAmplifier() { return ATTRIBUTE_GetMultiplier(ATTRIBUTE_EquipmentEffectAmplifier); }
        public float EQUIPMENT_EffectResistance() { return ATTRIBUTE_GetMultiplier(ATTRIBUTE_EquipmentEffectResistance); }
        public float EQUIPMENT_ArmorWeight() { return ATTRIBUTE_GetMultiplier(ATTRIBUTE_EquipmentWeight, 0) * skillset.agility.ArmorWeightMultiplier; }
        #endregion

        #endregion

        #region [Encapsulation] Statistic Managing

        public void STATISTIC_SetStat(string name, int value)
        {
            statistics.SetStat(name, value);
        }
        public void STATISTIC_SetStat(string name, float value)
        {
            statistics.SetStat(name, value);
        }
        public void STATISTIC_SetStat(string name, string value)
        {
            statistics.SetStat(name, value);
        }

        public void STATISTIC_AdjustStat(string name, int value)
        {
            statistics.AdjustStat(name, value);
        }
        public void STATISTIC_AdjustStat(string name, float value)
        {
            statistics.AdjustStat(name, value);
        }
        public void STATISTIC_AdjustStat(string name, string value)
        {
            statistics.AdjustStat(name, value);
        }

        public int STATISTIC_GetStatInteger(string name)
        {
            return statistics.GetStatInteger(name);
        }
        public float STATISTIC_GetStatFloat(string name)
        {
            return statistics.GetStatFloat(name);
        }
        public string STATISTIC_GetStatString(string name)
        {
            return statistics.GetStatString(name);
        }

        public string STATISTICS_SaveData(string tag)
        {
            return statistics.SaveData(tag).ToString();
        }
        public void STATISTICS_LoadData(List<string> data)
        {
            statistics.LoadData(data);
        }

        #endregion

        #region [Encapsulation] Skills Managing
        public int SKILL_Embers() { return skillset.ExperiencePoints; }
        public float SKILL_EmberMultiplier { get { return skillset.ExperienceMultiplier; } set { skillset.ExperienceMultiplier = value; } }
        public void SKILL_AddEmbers(int value)
        {
            skillset.AddEXP(value);
            screens.INTEGERDISPLAY_AddInteger("+" + (int)(value * SKILL_EmberMultiplier) + " Embers", Position, Color.Orange, BaseCurrentFloor + 1, false);

            STATISTIC_AdjustStat("embers_collected", value);
            if (STATISTIC_GetStatInteger("most_embers_held") < skillset.ExperiencePoints)
                STATISTIC_SetStat("most_embers_held", skillset.ExperiencePoints);
        }
        public void SKILL_RemoveEmbers(int value) { skillset.RemoveEXP(value); screens.INTEGERDISPLAY_AddInteger("-" + value + " Embers", Position, Color.Orange, BaseCurrentFloor + 1, true); }
        public int SKILL_TakeEmbers(int value)
        {
            int removal = Math.Min(value, skillset.ExperiencePoints);
            SKILL_RemoveEmbers(removal);

            return removal;
        }
        public void SKILL_ClearEmbers() { skillset.ExperiencePoints = 0; }
        public int SKILL_GetLevel(string skill)
        {
            int returnValue = -1;

            switch (skill.ToUpper())
            {
                case "HEALTH": returnValue = skillset.health.Level; break;
                case "ENDURANCE": returnValue = skillset.endurance.Level; break;
                case "AGILITY": returnValue = skillset.agility.Level; break;
                case "ARCHERY": returnValue = skillset.archery.Level; break;
                case "STRENGTH": returnValue = skillset.strength.Level; break;
                case "MAGIC": returnValue = skillset.wisdom.Level; break;
                case "INTELLIGENCE": returnValue = skillset.intelligence.Level; break;
                case "TRAPPING": returnValue = skillset.trapping.Level; break;
                case "LOOTING": returnValue = skillset.looting.Level; break;
                case "CONCEALMENT": returnValue = skillset.concealment.Level; break;
                case "AWARENESS": returnValue = skillset.awareness.Level; break;
                case "DEFENSE": returnValue = skillset.resistance.Level; break;
            }

            return returnValue;
        }

        public void HEALTH_Damage(uint value, string entityID, bool sendDigit = true)
        {
            skillset.health.Damage(value);
            killerID = entityID;

            if (sendDigit == true)
            {
                if (value >= 1)
                    screens.INTEGERDISPLAY_AddInteger("-" + value.ToString(), Position, new Color(190, 80, 85, 255), BaseCurrentFloor + 1, true);
                else if (value == 0)
                    screens.INTEGERDISPLAY_AddInteger("-" + value.ToString(), Position, new Color(85, 80, 190, 255), BaseCurrentFloor + 1, true);
            }

            if (value < skillset.health.MaxHP / 20) //5%
            {
                entityTargetColor = Color.Lerp(Color.White, Color.Red, .25f);
                CAPTION_SendImmediate(chat.RetrieveRandom("HPLowDamage"));
            }
            else if (value < skillset.health.MaxHP / 10) //10%
            {
                entityTargetColor = Color.Lerp(Color.White, Color.Red, .5f);
                CAPTION_SendImmediate(chat.RetrieveRandom("HPMediumDamage"));
            }
            else if (value < skillset.health.MaxHP / 5) //20%
            {
                entityTargetColor = Color.Lerp(Color.White, Color.Red, .75f);
                CAPTION_SendImmediate(chat.RetrieveRandom("HPHighDamage"));
            }
            else //Greater than 20%
            {
                entityTargetColor = Color.Lerp(Color.White, Color.Red, 1f);
                CAPTION_SendImmediate(chat.RetrieveRandom("HPMassiveDamage"));
            }

            colorLerp = 0f;
        }
        public void HEALTH_Damage(uint value, string entityID, Color flashColor, bool sendDigit = true)
        {
            skillset.health.Damage(value);
            killerID = entityID;

            if (sendDigit == true)
                screens.INTEGERDISPLAY_AddInteger("-" + value.ToString(), Position + new Vector2(0, centerOffset), new Color(190, 80, 85, 255), BaseCurrentFloor + 1, true);

            if (value < skillset.health.MaxHP / 20) //5%
                CAPTION_SendImmediate(chat.RetrieveRandom("HPLowDamage"));
            else if (value < skillset.health.MaxHP / 10) //10%
                CAPTION_SendImmediate(chat.RetrieveRandom("HPMediumDamage"));
            else if (value < skillset.health.MaxHP / 5) //20%
                CAPTION_SendImmediate(chat.RetrieveRandom("HPHighDamage"));
            else //Greater than 20%
                CAPTION_SendImmediate(chat.RetrieveRandom("HPMassiveDamage"));

            entityTargetColor = flashColor;

            colorLerp = 0f;
        }

        public void HEALTH_Restore(uint value, bool sendDigit = true)
        {
            skillset.health.Restore(value);

            if (sendDigit == true)
                screens.INTEGERDISPLAY_AddInteger("+" + value.ToString(), Position, new Color(100, 220, 100, 255), BaseCurrentFloor + 1, false);
        }

        public void STAMINA_Damage(float value)
        {
            value = MathHelper.Clamp(value, 0f, 10000);

            skillset.endurance.DamageStamina(value);
        }
        public void STAMINA_Restore(float value)
        {
            value = MathHelper.Clamp(value, 0f, 10000);

            skillset.endurance.RestoreStamina(value);
        }

        public void MAGIC_Deplete(uint value)
        {
            skillset.wisdom.UseCharge(value);
        }
        public void MAGIC_Restore(uint value)
        {
            skillset.wisdom.RestoreCharge(value);
        }
        public int MAGIC_SpellSlots() { return skillset.wisdom.SpellSlots(); }
        public float MAGIC_NegativePotionEffect() { return skillset.wisdom.NegativePotionEffect; }
        public float MAGIC_PositivePotionEffect() { return skillset.wisdom.PositivePotionMultiplier; }
        public float MAGIC_NegativePotionMultiplier
        {
            get { return skillset.wisdom.NegativePotionMultiplier; }
            set { skillset.wisdom.NegativePotionMultiplier = value; }
        }

        public float DEFENSE_MaximumStun() { return skillset.resistance.MaxStun + EQUIPMENT_WeightStunMultiplier; }

        public float ARCHERY_BaseDamage() { return skillset.strength.ArcheryDamage; }
        public float ARCHERY_Accuracy() { return skillset.archery.Accuracy; }

        public float AGILITY_SneakSpeed() { return skillset.agility.SneakSpeed * EQUIPMENT_WeightSpeedMultiplier; }
        public float AGILITY_WalkSpeed() { return skillset.agility.WalkSpeed * EQUIPMENT_WeightSpeedMultiplier; }
        public float AGILITY_SprintSpeed() { return skillset.agility.SprintSpeed * EQUIPMENT_WeightSpeedMultiplier; }

        private int staminaEmptyWaitTime = 0, flashColor = 0; bool isStaminaWaiting = false;
        protected void RegenerateStamina(GameTime gt)
        {
            bool isRegeneratingStamina = true;

            if (EQUIPMENT_PrimaryCombat() != null)
            {
                if (EQUIPMENT_PrimaryCombat().CurrentAction != CombatMove.None)
                    isRegeneratingStamina = false;
            }

            if (EQUIPMENT_OffhandCombat() != null)
            {
                if (EQUIPMENT_OffhandCombat().CurrentAction != CombatMove.None)
                    isRegeneratingStamina = false;
            }

            if (STATE_IsSprinting() == true || STATE_IsSneaking() == true || IsAirborne() == true)
                isRegeneratingStamina = false;

            if (skillset.endurance.CurrentStamina <= 0f)
                isStaminaWaiting = true;

            if (isStaminaWaiting == true)
            {
                staminaEmptyWaitTime += gt.ElapsedGameTime.Milliseconds;
                flashColor += gt.ElapsedGameTime.Milliseconds;

                if (flashColor > 450)
                {
                    entityTargetColor = Color.Lerp(Color.White, Color.Transparent, .5f);
                    colorLerp = 0f;
                    flashColor = 0;
                }

                if (staminaEmptyWaitTime > 2000)
                {
                    staminaEmptyWaitTime = 0;
                    flashColor = 0;
                    isStaminaWaiting = false;
                }
            }

            if (isRegeneratingStamina == true)
                STAMINA_Restore((skillset.endurance.RegenerationMultiplier * EQUIPMENT_WeightSpeedMultiplier) * (float)gt.ElapsedGameTime.TotalSeconds);
        }
        #endregion

        #region[Encapsulation] State Managing

        public AnimationState.AnimateState ANIMATION_State { get { return animation.CurrentState; } }
        public void ANIMATION_ResetValues() { animation.ResetCurrentValues(); }

        /// <summary>
        /// Check if current animation has completed it's cycle
        /// </summary>
        public bool ANIMATION_IsFinished { get { return animation.IsAnimationFinished; } }
        public void ANIMATION_Set(int time) { animation.SetTimePerFrame(time); }
        public Texture2D ANIMATION_Spritesheet { get { return animation.AnimationSheet; } }

        public void ANIMATION_Pause() { animation.PauseAnimation(); }
        public void ANIMATION_Pause(uint milliseconds) { animation.PauseAnimation(milliseconds); }
        public void ANIMATION_Play() { animation.PlayAnimation(); }

        public void STATE_SetAction(string action)
        {
            animation.SetAction(action);
        }

        /// <summary>
        /// In degrees
        /// </summary>
        /// <param name="angle"></param>
        protected void STATE_SetLook(float angle)
        {
            animation.SetLookDirection(angle);
        }
        protected void STATE_SetMovement(Vector2 moveDirection)
        {
            animation.SetMoveDirection(moveDirection);
        }

        public bool STATE_IsSneaking() { return animation.IsPerforming("SNEAK"); }
        public bool STATE_IsWalking() { return animation.IsPerforming("WALK"); }
        public bool STATE_IsSprinting() { return animation.IsPerforming("SPRINT"); }

        public int STATE_SizeSquared() { return (animation.FrameSize.X * animation.FrameSize.Y) * (int)MathHelper.Clamp(animation.Scale, 1, animation.Scale); }

        #endregion

        #region [Encapsulation] Sense Managing
        public float SENSES_CurrentDirection { get { return senses.CurrentDirection; } set { senses.CurrentDirection = value; } }
        public float SENSES_SightDirection { get { return senses.SightDirection; } set { senses.SightDirection = value; } }
        public float SENSES_SightLength { get { return senses.SightLength; } set { senses.SightLength = value; } }
        public ObjectSenses.RotateSpeed SENSES_TurnSpeed { get { return senses.TurnSpeed; } set { senses.TurnSpeed = value; } }

        public ObjectSenses.SightType SENSES_IsInSight(Vector2 target) { return senses.IsInSight(target); }
        public bool SENSES_IsHearingRange(Vector2 target) { return senses.IsHearingRange(target); }

        #endregion

        #region [Encapsulation] Animation Managing

        public void ANIMATION_SetTimePerFrame(int milliseconds)
        {
            animation.SetTimePerFrame(milliseconds);
        }
        public Point ANIMATION_CurrentFrame { get { return animation.CurrentFrame; } }
        public int ANIMATION_TimePerFrame { get { return animation.BaseTimePerFrame; } }
        public bool ANIMATION_IsAnimationFinished { get { return animation.IsAnimationFinished; } }

        #endregion

        #region [Encapsulation] Caption Managing

        public void CAPTION_SendImmediate(string message, bool isInfiniteTime = false) { if (IsDead == false) caption.SendCaptionImmediate(message, isInfiniteTime); }
        public void CAPTION_Queue(string message)
        {
            if (IsDead == false)
                caption.QueueCaption(message);
        }
        public string CAPTION_CurrentCaption { get { return caption.CurrentMessage; } }
        public void CAPTION_Reset() { caption.Reset(); }

        #endregion

        #region [Encapsulation] Faction Managing

        public bool FACTION_IsAlly(BaseFaction faction)
        {
            if (this.faction.GetDisposition(faction.ID) == BaseFaction.Disposition.Like ||
                this.faction.GetDisposition(faction.ID) == BaseFaction.Disposition.Fond ||
                this.faction.GetDisposition(faction.ID) == BaseFaction.Disposition.Love)
            {
                return true;
            }

            return false;
        }
        public bool FACTION_IsEnemy(BaseFaction faction)
        {
            if (this.faction.GetDisposition(faction.ID) == BaseFaction.Disposition.Despise)
                return true;

            return false;
        }
        public bool FACTION_IsUnknown(BaseFaction faction)
        {
            if (this.faction.GetDisposition(faction.ID) == BaseFaction.Disposition.Neutral)
                return true;

            return false;
        }

        #endregion

        #region [Encapsulation] Storage Managing

        public void STORAGE_AddItem(int id, int quantity, bool isProtected = false, bool displayNotification = false) { storage.AddItem(id, quantity, isProtected, displayNotification); }
        public int STORAGE_AddItemGetDifference(int id, int quantity, bool isProtected = false, bool displayNotification = false)
        {
            return storage.AddItemGetDifference(id, quantity, isProtected, displayNotification);
        }
        public void STORAGE_AddItem(BaseItem item, bool displayNotification = false) { storage.AddItem(item, displayNotification); }
        public void STORAGE_RemoveItem(int id, int quantity) { storage.RemoveItem(id, quantity); }
        public void STORAGE_ClearItem(int id, int removeAllButX = 0) { storage.ClearItem(id, removeAllButX); }

        public void STORAGE_RepairAll()
        {
            for (int i = 0; i < storage.TotalItems().Count; i++)
            {
                //If the item is broken ...
                if (storage.TotalItems()[i].IsBroken() == true)
                {
                    //And the item can be repaired on rest if broken ...
                    if (storage.TotalItems()[i].IsRestRepairsBroken == true)
                        storage.TotalItems()[i].RepairItem(); //Repair it!
                }
                else
                {
                    //If the item is not broken, check if the item can be repaired on rest.
                    if (storage.TotalItems()[i].IsRestRepairsUnbroken == true)
                        storage.TotalItems()[i].RepairItem(); //If yes, repair it!
                }
            }
        }

        public void STORAGE_UseItemButton(int id, int button) { storage.UseItemButton(id, button); }

        public int STORAGE_Search(string itemType) { return storage.SearchStorage(itemType); }
        public int STORAGE_Search(string itemType, string itemSubType) { return storage.SearchStorage(itemType, itemSubType); }

        public BaseItem STORAGE_SearchForItem(string itemType) { return storage.SearchStorageByItem(itemType); }
        public BaseItem STORAGE_SearchForItem(string itemType, string itemSubType) { return storage.SearchStorageByItem(itemType, itemSubType); }
        public List<BaseItem> STORAGE_ReturnItems(string itemType) { return storage.SearchForItem(itemType); }
        public List<BaseItem> STORAGE_ReturnItems(string itemType, string itemSubType) { return storage.SearchForItem(itemType, itemSubType); }
        public BaseItem STORAGE_GetItem(int id) { return storage.GetItem(id); }

        public bool STORAGE_Check(string itemType) { return storage.CheckStorage(itemType); }
        public bool STORAGE_Check(string itemType, string itemSubType) { return storage.CheckStorage(itemType, itemSubType); }

        public bool STORAGE_Check(int id) { return storage.CheckForItem(id); }
        public bool STORAGE_Check(int id, int quantity) { return storage.CheckForItem(id, quantity); }

        public bool STORAGE_IsMax(int id) { return storage.GetItem(id).IsQuantityMax; }

        public void STORAGE_AddSpell(int id) { storage.AddSpell(id); }

        public void STORAGE_Gift(BaseEntity gifter, int id, int quantity)
        {
            STORAGE_AddItem(id, quantity);
            BaseItem i = ItemDatabase.Item(id);

            string response = chat.RetrieveRandom("GiftItem [ID:" + id + "]");
            if (string.IsNullOrEmpty(response))
                response = chat.RetrieveRandom("GiftItem");

            CAPTION_Queue(response);

            AddMessage(new MessageHolder(gifter, i, null, "Gift", quantity.ToString(), 100, 100));
        }
        public void STORAGE_ThrowItem(int id, int quantity)
        {
            tileMap.AddContainer(new TileEngine.Objects.ContainerTypes.SingleItem(-1, BaseCurrentFloor, 0f, id, quantity, position, false));
        }

        #endregion

        #region [Encapsulation] Equipment Managing
        public void EQUIPMENT_EquipWeapon(Weapon weapon, int index) { equipment.EquipWeapon(index, weapon); }

        public void EQUIPMENT_SwitchPrimary(EntityEquipment.WeaponSlot slot) { equipment.SwitchPrimary(slot); SUSPEND_Action(250); }
        public void EQUIPMENT_SwitchOffhand(EntityEquipment.WeaponSlot slot) { equipment.SwitchOffhand(slot); SUSPEND_Action(250); }

        public Weapon EQUIPMENT_PrimaryWeapon() { return equipment.CurrentPrimary(); }
        public Weapon EQUIPMENT_OffhandWeapon() { return equipment.CurrentOffhand(); }

        public Ammo EQUIPMENT_PrimaryAmmo() { return equipment.CurrentPrimaryAmmo(); }

        public BaseCombat EQUIPMENT_PrimaryCombat() { return equipment.CurrentPrimaryWeapon(); }
        public BaseCombat EQUIPMENT_OffhandCombat() { return equipment.CurrentOffhandWeapon(); }
        public BaseCombat EQUIPMENT_CombatWeapon(bool isPrimary)
        {
            if (isPrimary == true)
                return equipment.CurrentPrimaryWeapon();
            else
                return equipment.CurrentOffhandWeapon();
        }

        public void COMBAT_RequestMove(BaseCombat combat, CombatMove move) { if (combat != null) combat.RequestCombatMove(move); }
        public void COMBAT_RequestMoves(BaseCombat combat, params CombatMove[] moves) { if (combat != null) combat.RequestRandomMove(moves); }

        public void EQUIPMENT_EquipArmor(Armor armor) { equipment.EquipArmor(armor); }
        public void EQUIPMENT_EquipAmmo(Ammo ammo, int index) { equipment.EquipAmmo(index, ammo); }
        public void EQUIPMENT_SoulEquip(BaseSoul soul, EntityEquipment.SoulSlot slot) { equipment.EquipSoul(soul, slot); }

        public bool EQUIPMENT_IsItemEquipped(int id) { return equipment.IsItemEquipped(id); }
        public bool EQUIPMENT_IsQuickslotEquipped(int id) { return equipment.IsQuickslotEquipped(id); }
        public bool EQUIPMENT_IsSoulEquipped(int id) { return equipment.IsSoulEquipped(id); }

        public int EQUIPMENT_PrimaryWeaponIndex { get { return equipment.PrimaryWeaponIndex; } }
        public int EQUIPMENT_OffhandWeaponIndex { get { return equipment.OffhandWeaponIndex; } }

        public float EQUIPMENT_WeightSpeedMultiplier { get { return (1f - (EQUIPMENT_ArmorWeight() / 200) * .75f); } }
        public float EQUIPMENT_WeightStunMultiplier { get { return (EQUIPMENT_ArmorWeight() / 2); } }

        public BaseSpell EQUIPMENT_CurrentSpell() { return equipment.CurrentSpell(); }

        #endregion

        #region [Encapsulation] Merchant Trading

        public bool MERCHANT_IsMerchant() { return (merchant != null); }
        public void MERCHANT_AddItem(int id, int quantity) { if (merchant != null) { merchant.AddItem(id, quantity, false, false); } }
        public bool MERCHANT_HasItem(int id)
        {
            if (merchant != null)
                return merchant.MerchantHasItem(id);
            else
                return false;
        }
        public float MERCHANT_GetItemDesire(BaseItem item)
        {
            if (merchant != null)
                return merchant.GetItemDesire(item);
            else
                return 0f;
        }
        public BaseItem MERCHANT_GetTradeItem(int priceDifference)
        {
            if (merchant != null)
                return merchant.GetTradeItem(priceDifference);
            else
                return null;
        }
        public float MERCHANT_TradeMarkdown { get { if (merchant != null) return merchant.TradeMarkdown; else return 0f; } }
        public float MERCHANT_QuicksellMarkdown { get { if (merchant != null) return merchant.QuickSellMarkdown; else return 0f; } }
        public float MERCHANT_PurchaseMarkup { get { if (merchant != null) return merchant.PurchaseMarkup; else return 0f; } }
        public int MERCHANT_ItemCount { get { return merchant.StockCount; } }
        public int MERCHANT_ItemStock { get { return merchant.TotalItems; } }
        public bool MERCHANT_IsAcceptableTrade(int merchantPrice, int traderPrice) { return merchant.IsAcceptableTrade(merchantPrice, traderPrice); }
        public bool MERCHANT_IsAcceptableItem(int id) { return merchant.IsAcceptableItem(id); }

        public void MERCHANT_SimulateTradeEconomy(int iterations, Merchant.EconomyHealth health) { merchant.SimulateTradeEconomy(iterations, health); }

        public List<BaseItem> MERCHANT_Stock() { if (merchant != null) return merchant.Stock; else return new List<BaseItem>(); }

        #endregion

        #region [Encapsulation] Combat Managing

        protected void UpdateCombat(GameTime gt, float direction, bool isPlayerControlled)
        {
            ValidateCombatCombination();

            UpdateWeaponry(gt, EQUIPMENT_PrimaryCombat(), EQUIPMENT_OffhandCombat(), direction, isPlayerControlled);
            UpdateWeaponry(gt, EQUIPMENT_OffhandCombat(), EQUIPMENT_PrimaryCombat(), direction, isPlayerControlled);
        }
        private void UpdateWeaponry(GameTime gt, BaseCombat combat, BaseCombat offhand, float direction, bool isPlayerControlled)
        {
            if (combat != null)
            {
                //If the character runs out of stamina
                if (isStaminaWaiting == true)
                    combat.ForceStop();

                combat.Center = Position + new Vector2(0, centerOffset);
                combat.ShadowOffset = Math.Abs((centerOffset * 2f) - (shadowHeightOffset));

                combat.SetStates(IsAirborne(),
                                 animation.IsPerforming("Dodge"),
                                 animation.IsPerforming("Sneak") || animation.IsPerforming("SneakIdle"),
                                 animation.IsPerforming("Sprint") && IsInputMoving());

                bool restrictAttacking = false;

                if (IsPlayerControlled == true)
                    restrictAttacking = screens.IsUIOpen || isStaminaWaiting || IsDead;
                else
                    restrictAttacking = isStaminaWaiting || IsDead;

                combat.SetVariables(this, offhand, direction, CombinedFloor, entityCircle.radius, Depth, entityCircle.radius * 2);
                combat.Update(gt, isPlayerControlled, restrictAttacking);
            }
        }
        protected void CheckWeaponBehavior(GameTime gt, Weapon weapon)
        {
            if (weapon != null)
            {
                BaseCombat combat = weapon.CombatWeapon;

                CheckStrikerBehavior(gt, combat, weapon);
                CheckDeflectorBehavior(gt, weapon);
            }
        }
        protected void CheckStrikerBehavior(GameTime gt, BaseCombat combat, Weapon weapon)
        {
            for (int i = 0; i < mapEntities.Count; i++)
            {
                if (mapEntities[i].IsTouchable(CombinedFloor) == true && !mapEntities[i].IsCompanionLeader(this)) //IsCompanionLeader() prevents friendly fire
                {
                    if (combat.CurrentAction != combat.LastAction)
                        mapEntities[i].COMBAT_RemoveProcessedDamage("[" + mapEntityID + "]" + weapon.Name + weapon.UniqueID);

                    if (combat.IsForceStopped == true)
                        mapEntities[i].COMBAT_RemoveUnprocessedDamage("[" + mapEntityID + "]" + weapon.Name + weapon.UniqueID);

                    WeaponCollision weaponLine = combat.CollidingLine(mapEntities[i].Hitbox);
                    if (weaponLine != null)
                    {
                        if (combat.IsForceStopped == false)
                            combat.OnEntityHit(mapEntities[i], weaponLine);
                    }
                }
            }
        }

        private void CheckDeflectorBehavior(GameTime gt, Weapon weapon)
        {
            //If the weapon is not doing anything, then... don't do anything here!
            if (weapon.CombatWeapon.CurrentAction != CombatMove.None)
            {
                for (int i = 0; i < mapEntities.Count; i++)
                {
                    //Don't check for monsters on a different floor
                    if (mapEntities[i].IsTouchable(CombinedFloor))
                    {
                        //EXPERIMENTAL!
                        CheckWeaponDeflect(weapon, mapEntities[i].EQUIPMENT_PrimaryWeapon(), mapEntities[i], mapEntities[i].Skills.strength.Level);
                        CheckWeaponDeflect(weapon, mapEntities[i].EQUIPMENT_OffhandWeapon(), mapEntities[i], mapEntities[i].Skills.strength.Level);
                    }
                }
            }
        }
        private void CheckWeaponDeflect(Weapon currentWeapon, Weapon enemyWeapon, BaseEntity enemy, int enemyStrength)
        {
            //TO-DO: Add in stamina depletion for "on shield hit" and "your weapon deflected"

            if (enemyWeapon != null) //If the monster has a weapon in hand
            {
                if (enemyWeapon.CombatWeapon.CurrentAction != CombatMove.None) //If the monster is swinging the weapon
                {
                    for (int j = 0; j < enemyWeapon.CombatWeapon.WeaponShape.Count; j++) //Iterate through the enemy's weapon shape
                    {
                        if (currentWeapon.CombatWeapon.IsColliding(enemyWeapon.CombatWeapon.WeaponShape[j].Line)) // and if the active weapon shape is colliding with any of the current weapon's active shapes
                        {
                            //Experimental!
                            if (enemyWeapon.CombatWeapon.WeaponShape[j].IsBlocking == true)
                            {
                                //CAPTION_SendImmediate("My sword hit your shield!");
                                currentWeapon.CombatWeapon.ForceCombatMove(CombatMove.DeflectThis);
                                enemyWeapon.CombatWeapon.ForceCombatMove(CombatMove.DeflectOther);

                                enemy.COMBAT_RemoveUnprocessedDamage("[" + mapEntityID + "]" + currentWeapon.Name + currentWeapon.UniqueID);
                                //The weapon hit a shield shape, so the weapon should bounce off.
                            }
                            else if (enemyWeapon.CombatWeapon is BaseDeflector && currentWeapon.CombatWeapon is BaseDeflector) //Old if statement. Change to check for both shield active... stuff.
                            {
                                //The shield hit a shield. Knock down opponent's shield.
                                CAPTION_SendImmediate("I bashed my shield against yours!");
                            }
                            else if (enemyWeapon.CombatWeapon.WeaponShape[j].IsActive == true) //If the weapon shape is active
                            {
                                //The weapon hit another weapon, so compare strength levels. Highest strength level wins.
                                if (Skills.strength.Level > enemyStrength)
                                {
                                    //CAPTION_SendImmediate("I deflected your sword!");
                                    enemyWeapon.CombatWeapon.ForceCombatMove(CombatMove.DeflectThis);
                                    //Deflect the mapEntities[i] weapon.
                                }
                                else if (Skills.strength.Level < enemyStrength)
                                {
                                    //CAPTION_SendImmediate("You deflected my sword!");
                                    currentWeapon.CombatWeapon.ForceCombatMove(CombatMove.DeflectThis);
                                    //Deflect my weapon
                                }
                                else
                                {
                                    //CAPTION_SendImmediate("Same strength level!");
                                    currentWeapon.CombatWeapon.ForceCombatMove(CombatMove.DeflectThis);
                                    enemyWeapon.CombatWeapon.ForceCombatMove(CombatMove.DeflectThis);
                                    //Strength levels are equal, so deflect both weapons!
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void COMBAT_RequestMove(bool isPrimaryHand, CombatMove move)
        {
            if (isPrimaryHand == true)
                EQUIPMENT_PrimaryCombat().RequestCombatMove(move);
            else
                EQUIPMENT_OffhandCombat().RequestCombatMove(move);
        }

        private bool isPrimaryUse = true;
        protected void ValidateCombatCombination()
        {
            int count = 0;

            if (EQUIPMENT_PrimaryCombat() != null)
            {
                if (EQUIPMENT_PrimaryCombat().SlotType == CombatSlotType.OneHand)
                    count += 1; //One-handed weapon
                else
                    count += 2; //Two-handed weapon
            }

            if (EQUIPMENT_OffhandCombat() != null)
            {
                if (EQUIPMENT_OffhandCombat().SlotType == CombatSlotType.OneHand)
                    count += 1; //One-handed weapon
                else
                    count += 2; //Two-handed weapon
            }

            if (count > 2)
            {
                if (isPrimaryUse == true)
                {
                    EQUIPMENT_PrimaryCombat().IsUsable = true;
                    EQUIPMENT_OffhandCombat().IsUsable = false;
                }
                else
                {
                    EQUIPMENT_PrimaryCombat().IsUsable = false;
                    EQUIPMENT_OffhandCombat().IsUsable = true;
                }
            }
            else //If count is less than 3... both are one-handed, so both are usable!
            {
                if (EQUIPMENT_PrimaryCombat() != null) { EQUIPMENT_PrimaryCombat().IsUsable = true; }
                if (EQUIPMENT_OffhandCombat() != null) { EQUIPMENT_OffhandCombat().IsUsable = true; }
            }
        }

        protected void COMBAT_ApplyDamage(uint damage, string killerID, string[] attributeNames)
        {
            float current = damage;

            for (int i = 0; i < attributeNames.Length; i++)
            {
                float multi = ATTRIBUTE_GetMultiplier(attributeNames[i]);
                if (multi > 0)
                    current *= multi;
            }

            HEALTH_Damage((uint)current, killerID);
            tileMap.SLASH_Add(Position + new Vector2(0, centerOffset), Color.Lerp(Color.White, Color.Transparent, .15f), Color.Transparent, 6f, entityCircle.radius * 2f);
        }
        public uint COMBAT_PhysicalDamage(Weapon weapon)
        {
            return (uint)skillset.strength.GetPhysicalDamage((int)(weapon.PhysicalDamage * weapon.RequirementPercentage()));
        }
        public uint COMBAT_ProjectileDamage(Weapon weapon)
        {
            return (uint)skillset.strength.GetProjectileDamage((int)(weapon.ProjectileDamage * weapon.RequirementPercentage()));
        }
        public float COMBAT_BlockingPercentage(Weapon weapon)
        {
            return weapon.BlockingPercentage;
        }

        private Dictionary<string, Tuple<uint, int, string[]>> unprocessedDamage = new Dictionary<string, Tuple<uint, int, string[]>>();
        private Dictionary<string, int> processedDamage = new Dictionary<string, int>();
        public void COMBAT_DamageEntity(string damageID, uint damage, int resetTime, Action action, params string[] attributeMultipliers)
        {
            if (!processedDamage.ContainsKey(damageID) && !unprocessedDamage.ContainsKey(damageID))
            {
                unprocessedDamage.Add(damageID, Tuple.Create(damage, resetTime, attributeMultipliers));
                action();
            }
        }
        public void COMBAT_RemoveUnprocessedDamage(string damageID)
        {
            if (unprocessedDamage.ContainsKey(damageID))
                unprocessedDamage.Remove(damageID);
        }
        /// <summary>
        /// Each weapon is responsible for clearing their own processed damage after each swing!
        /// </summary>
        /// <param name="damageID"></param>
        public void COMBAT_RemoveProcessedDamage(string damageID)
        {
            if (processedDamage.ContainsKey(damageID))
            {
                processedDamage.Remove(damageID);
            }
        }

        private int unprocessedDamageTime = 0, processedDamageTime;
        public void ProcessDamage(GameTime gt)
        {
            if (unprocessedDamage.Count > 0)
            {
                unprocessedDamageTime += gt.ElapsedGameTime.Milliseconds;

                if (unprocessedDamageTime >= 80) //At 60 FPS, this takes 2 update steps to apply damage. This should leave time for other things to remove damage, just in case.
                {
                    COMBAT_ApplyDamage(unprocessedDamage.FirstOrDefault().Value.Item1, unprocessedDamage.FirstOrDefault().Key.FromWithin('[', ']', 1), unprocessedDamage.FirstOrDefault().Value.Item3);

                    processedDamage.Add(unprocessedDamage.FirstOrDefault().Key, unprocessedDamage.FirstOrDefault().Value.Item2); //Once damage has been applied, add to processedDamage and remove the item from unprocessedDamage
                    unprocessedDamage.Remove(unprocessedDamage.FirstOrDefault().Key);

                    unprocessedDamageTime = 0;
                }
            }
            
            if (processedDamage.Count > 0)
            {
                processedDamageTime += gt.ElapsedGameTime.Milliseconds;

                if (processedDamageTime > processedDamage.FirstOrDefault().Value)
                {
                    processedDamage.Remove(processedDamage.FirstOrDefault().Key); //Remove the first processed damage in the dictionary every 1 second. This is backup in case someone forgets to clear in COMBAT_RemoveProcessedDamage()
                    processedDamageTime = 0;
                }
            }
        }

        protected bool COMBAT_IsStateSetting()
        {
            bool isSetStatePrimary = EQUIPMENT_PrimaryCombat() != null ? EQUIPMENT_PrimaryCombat().IsEntitySetState() : true;
            bool isSetStateOffhand = EQUIPMENT_OffhandCombat() != null ? EQUIPMENT_OffhandCombat().IsEntitySetState() : true;

            return isSetStatePrimary && isSetStateOffhand;
        }

        public bool COMBAT_IsInCombat { get; protected set; }
        #endregion

        #region [Encapsulation] Kin Managing

        public bool KIN_CompareSubType(string subType) { return kin.CompareType(subType); }
        public bool KIN_Compare(string kinType) { return kin.CompareKin(kinType); }

        #endregion

        #region [Encapsulation] Companion methods

        public string EmotionalDisposition(BaseEntity entity)
        {
            string value = string.Empty;

            EntityDisposition disposition = GetDisposition(entity);

            if (disposition != null)
            {
                if (disposition.Disposition == 0)
                    value = "Unsure";
                if (disposition.Disposition > 0 && disposition.Disposition <= 15)
                    value = "Liked";
                if (disposition.Disposition > 15 && disposition.Disposition <= 30)
                    value = "Pleasing";
                if (disposition.Disposition > 30 && disposition.Disposition <= 45)
                    value = "Amicable";
                if (disposition.Disposition > 45 && disposition.Disposition <= 60)
                    value = "Compeer";
                if (disposition.Disposition > 60 && disposition.Disposition <= 75)
                    value = "Exceptional";
                if (disposition.Disposition > 75 && disposition.Disposition <= 90)
                    value = "Kindred Spirit";
                if (disposition.Disposition > 90 && disposition.Disposition <= 100)
                    value = "Legendary";

                //Negative
                if (disposition.Disposition < 0 && disposition.Disposition >= -15)
                    value = "Cautious";
                if (disposition.Disposition < -15 && disposition.Disposition >= -30)
                    value = "Distant";
                if (disposition.Disposition < -30 && disposition.Disposition >= -45)
                    value = "Annoyed";
                if (disposition.Disposition < -45 && disposition.Disposition >= -60)
                    value = "Vexed";
                if (disposition.Disposition < -60 && disposition.Disposition >= -75)
                    value = "Resentful";
                if (disposition.Disposition < -75 && disposition.Disposition >= -90)
                    value = "Loathed";
                if (disposition.Disposition < -90 && disposition.Disposition >= -100)
                    value = "Enraged";
            }

            return value;
        }
        public void MakeCompanion(BaseEntity companionLeader)
        {
            if (canMakeCompanion == true)
            {
                this.companionLeader = companionLeader;
                chat.RetrieveRandom("CompanionLeader");
            }
        }
        public void MakeCompanion(BaseEntity companionLeader, int minimumDisposition)
        {
            minimumDisposition = (int)MathHelper.Clamp(minimumDisposition, -100, 100);

            EntityDisposition disposition = GetDisposition(companionLeader);

            if (disposition != null)
            {
                if (disposition.Disposition > minimumDisposition)
                {
                    if (canMakeCompanion == true)
                    {
                        this.companionLeader = companionLeader;
                        chat.RetrieveRandom("CompanionLeader");
                    }
                }
            }
        }
        public virtual void UnmakeCompanion()
        {
            companionLeader = null;
        }
        public void AdjustDisposition(BaseEntity entity, int value)
        {
            EntityDisposition disp = GetDisposition(entity);

            if (disp != null)
            {
                disp.AdjustDisposition(value);

                if (HasCompanionLeader)
                {
                    if (entity.MapEntityID == companionLeader.MapEntityID)
                    {
                        if (disp.Disposition <= 20) //If the entity's disposition to the companion leader is small, unmake companion leader
                            UnmakeCompanion();
                    }
                }
            }
        }
        protected EntityDisposition GetDisposition(BaseEntity entity)
        {
            EntityDisposition disp = null;

            for (int i = 0; i < dispositions.Count; i++)
            {
                if (dispositions[i].Entity.MapEntityID == entity.MapEntityID)
                    disp = dispositions[i];
            }

            return disp;
        }

        #endregion

        #region [Encapsulation] Steering

        public virtual Vector2 STEERING_Direction() { return Vector2.Zero; }
        public virtual Vector2 STEERING_Cross() { return Vector2.Zero; }

        #endregion

        #region [Encapsulation] Movement Abilities

        protected override void UpdateFall(GameTime gt)
        {
            if (IsAirborne() == true)
            {
                if (isPlayerControlled && heightOffset <= -200)
                {
                    camera.DelaySpeed = 3f;
                    camera.SmoothZoom(1f, 2f, true, 0);
                }
                else if (isPlayerControlled && heightOffset <= -400)
                    camera.DelaySpeed = 4f;
                else if (isPlayerControlled && heightOffset <= -600)
                    camera.DelaySpeed = 5f;
                else if (isPlayerControlled && heightOffset <= -800)
                    camera.DelaySpeed = 6f;
                else if (isPlayerControlled && heightOffset > -800)
                    camera.DelaySpeed = 7f;

                camera.Zoom = MathHelper.Lerp(1f, .75f, MathHelper.Clamp(heightOffset / -1200, 0, -1200));

                STATE_SetAction("Jump");
                jumpCircle.radius = 0f;
            }
        }
        protected override void GroundHit()
        {
            if (currentBounce == 1)
            {
                lastMotion *= .5f;
            }
            if (gravityVelocity >= 6f)
            {
                uint damage = (uint)((gravityVelocity - 6f) * 30f);
                HEALTH_Damage(damage, "FallDamage", true);
            }

            //Reset camera values
            if (isPlayerControlled == true)
            {
                camera.DelaySpeed = 3f;
                camera.SmoothZoom(1f, 2f, true, 0);
                jumpCircle.radius = baseJumpCircle.radius;
            }
        }
        public void Jump(float jumpHeight, float staminaDamage)
        {
            gravityVelocity -= (jumpHeight) * MathHelper.Clamp(EQUIPMENT_WeightSpeedMultiplier * 1.5f, .35f, 1f);
            STAMINA_Damage((25f + (EQUIPMENT_ArmorWeight() * .1f)) * staminaDamage);
        }
        public void Jump()
        {
            gravityVelocity -= (skillset.agility.jumpHeight) * MathHelper.Clamp(EQUIPMENT_WeightSpeedMultiplier * 1.5f, .35f, 1f);
            STAMINA_Damage(25f + (EQUIPMENT_ArmorWeight() * .1f));
        }
        public bool JUMP_IsCollidable() { return heightOffset > -20f; }

        private Vector2 dodgeStart;
        public void Dodge(Vector2 direction)
        {
            SUSPEND_Action((int)((skillset.agility.DodgeDistance / (skillset.agility.DodgeSpeed * EQUIPMENT_WeightSpeedMultiplier)) * 1000));

            dodgeStart = position;
            movementMotion = direction;

            STATE_SetAction("Dodge");
            STAMINA_Damage((35f + (EQUIPMENT_ArmorWeight() * .1f)));
        }

        #endregion

        #region [Encapsulation] Depth Floor Methods

        public bool IsSameFloor(int floor) { return depthFloor.IsSameFloor(floor); }
        public bool IsCloseFloor(int floor) { return depthFloor.IsCloseFloor(floor); }
        public bool IsTouchable(int floor) { return (depthFloor.IsSameFloor(floor)); }

        #endregion

        #region [Encapsulation] Flame Renown

        public float RENOWN_Value { get { return renown.Renown; } set { renown.Renown = value; } }
        public FlameRenown.FlameRenownName RENOWN_Name() { return renown.RenownEnumName; }
        public string RENOWN_Title() { return renown.RenownName(); }
        public void RENOWN_Adjust(float value) { renown.AdjustRenown(value); }

        #endregion

        #region [Encapsulation] Looting

        public void LOOT_AddItem(int id, int quantity, float rarityPct) { loot.AddItem(id, quantity, rarityPct); }
        public int LOOT_Experience() { return loot.experiencePoints; }
        public int LOOT_FlameID() { return loot.soulID; }
        protected virtual void DropLoot() { loot.DropLoot(BaseCurrentFloor, "World/Objects/Containers/ClothSack2", position + new Vector2(50, 16)); }

        #endregion

        #region [Encapsulation] Chatter

        public string CHAT_Retrieve(string phrase)
        {
            return chat.RetrieveRandom(phrase);
        }
        public bool CHAT_Check(string phrase)
        {
            return chat.CheckKey(phrase);
        }
        public string CHAT_VariableInjection(string line, string inject, string variable)
        {
            return line.Replace(inject, variable);
        }

        #endregion

        #region [Encapsulation] Map Interation

        public bool IsActivatingObject { get; protected set; }

        public GameObject SelectedObject { get; protected set; }
        private Circle selectionCircle;

        private List<GameObject> selectionGroupObjects = new List<GameObject>();

        public void CheckSelectedObjects(GameTime gt)
        {
            selectionGroupObjects.Clear();

            selectionCircle.Position = position + (mouseDirection.ToVector2() * MathHelper.Clamp(Vector2.Distance(position, mouseToWorld), 8, 64));

            for (int i = 0; i < usableObjects.Count; i++)
            {
                if (selectionCircle.Contains(usableObjects[i].Position))
                    selectionGroupObjects.Add(usableObjects[i]);
            }

            float lastDist = 100000;
            for (int i = 0; i < selectionGroupObjects.Count; i++)
            {
                if (usableObjects[i].IsObjectUsable == true)
                {
                    float temp = 0;
                    if ((temp = Vector2.Distance(selectionGroupObjects[i].Position, selectionCircle.Position)) < lastDist)
                    {
                        SelectedObject = selectionGroupObjects[i];
                        lastDist = temp;
                    }
                }
            }

            if (selectionGroupObjects.Count == 0)
                SelectedObject = null;

            if (SelectedObject != null && SelectedObject.IsObjectUsable && SelectedObject.CurrentFloor == CurrentFloor)
                 SelectedObject.SelectObject(gt);
        }
        public void UseSelectedObject()
        {
            if (SelectedObject != null &&
                SelectedObject.IsObjectUsable &&
                SelectedObject.CurrentFloor == CurrentFloor)
                SelectedObject.UseObject(this);
        }

        #endregion

        //Below code wil typically only be used by NPEs.

        #region [Encapsulation] Memory

        public void MEMORY_AddShort(MemoryPacket packet)
        {
            this.memory.SHORT_Add(packet);
        }
        public void MEMORY_AddShort(string key, string memory, int lifeTime)
        {
            this.memory.SHORT_Add(key, memory, lifeTime);
        }

        public void MEMORY_AddLong(MemoryPacket packet)
        {
            memory.LONG_Add(packet);
        }
        public void MEMORY_AddLong(string key, string memory, int lifeTime, int priority)
        {
            this.memory.LONG_Add(key, memory, lifeTime, priority);
        }

        public string MEMORY_FetchShortMemory(string keyword)
        {
            return memory.SHORT_FetchMemory(keyword);
        }
        public MemoryPacket MEMORY_FetchShortMemoryPacket(string keyword)
        {
            return memory.SHORT_FetchMemoryPacket(keyword);
        }

        public string MEMORY_FetchLongMemory(string keyword)
        {
            return memory.LONG_FetchMemory(keyword);
        }
        public MemoryPacket MEMORY_FetchLongMemoryPacket(string keyword)
        {
            return memory.LONG_FetchMemoryPacket(keyword);
        }

        public void MEMORY_RemoveLong(MemoryPacket packet)
        {
            memory.LONG_Remove(packet);
        }
        public void MEMORY_RemoveLong(string keyword)
        {
            memory.LONG_Remove(keyword);
        }

        public bool MEMORY_ContainsShort(string keyword)
        {
            return memory.SHORT_ContainsMemory(keyword);
        }
        public bool MEMORY_ContainsLong(string keyword)
        {
            return memory.LONG_ContainsMemory(keyword);
        }

        public StringBuilder MEMORY_SaveData()
        {
            return memory.SaveData();
        }
        public void MEMORY_LoadData(List<string> data)
        {
            memory.LoadData(data);
        }

        #endregion

        #region [Encapsulate] Movement Managing

        public void STEERING_SetTargetPosition(Vector2 position) { steering.SetTargetPosition(position); }
        public void STEERING_SetTargetTile(Point goal) { steering.SetTargetTile(goal); }
        public void STEERING_ResetPathfind() { steering.ResetPath(); }
        public void STEERING_LookAt(Vector2 position) { steering.LookAt(position); }

        public void STEERING_SetState(SteeringBehavior.MovementState state) { steering.SetState(state); }
        public void STEERING_ForceStop() { steering.ForceStop(); }

        public SteeringBehavior.MovementState STEERING_State() { return steering.State; }

        #endregion

        #region [Encapsulate] AI methods

        public void AddGoal(BaseGoal goal) { agentAI.AddGoal(goal); }
        public void RemoveAllGoals() { agentAI.RemoveAllGoals(); }
        public BaseGoal TargetGoal(string goalName) { return agentAI.TargetGoal(goalName); }
        public bool IsGoalPresent(string goalName) { return agentAI.IsGoalPresent(goalName); }

        public void SetEnemyTarget(BaseEntity target)
        {
            enemyTarget = target;
        }
        public void SetAllyTarget(BaseEntity target)
        {
            allyTarget = target;
        }

        #endregion

        #region [Encapsulate] Pathfinding

        public Vector2 PathfindPosition { get { return pathfinder.CurrentTilePosition; } }
        public Point PathfindTile() { return pathfinder.CurrentTile(); }
        public void RemoveCurrentTile() { pathfinder.RemoveCurrentTile(); }
        public void RemoveLastTile() { pathfinder.RemoveLastTile(); }
        public bool IsClose(float minDistance) { return pathfinder.IsClose(position, minDistance); }
        public bool IsPathCompleted() { return pathfinder.CurrentPath.Count <= 0; }

        public bool IsPathFailed() { return pathfinder.IsPathFailed; }
        public void CheckPathFailure(GameTime gt) { pathfinder.CheckPathFailure(gt); }

        public void Pathfind(Point start, Point goal)
        {
            pathfinder.RegenerateMap(BaseCurrentFloor, start, goal);
            pathfinder.GeneratePath(BaseCurrentFloor, start, goal);
        }

        #endregion

        private int lowHP = 10000;
        protected virtual void CheckChatOptions(GameTime gt)
        {
            if (skillset.health.CurrentHP < (float)(skillset.health.MaxHP / 8))
            {
                lowHP += gt.ElapsedGameTime.Milliseconds;

                if (lowHP >= 10000)
                {
                    if (HasCompanionLeader == true)
                        CAPTION_SendImmediate(chat.RetrieveRandom("CompanionLowHP"));
                    else
                        CAPTION_SendImmediate(chat.RetrieveRandom("LowHP"));

                    lowHP = 0;
                }
            }
            else
                lowHP = 9750;
        }

        // [Methods] TileMap
        public void ThrowProjectile(int id) { tileMap.AddProjectile(id, position, mouseDirection, 1f, 1f, CombinedFloor, mapEntityID, 0); }
        public void ThrowProjectile(int id, float speedMultiplier, float damageMultiplier, float accuracyDifference, int baseDamage)
        {
            float direction = mouseDirection - random.NextFloat(-accuracyDifference, accuracyDifference);
            tileMap.AddProjectile(id, Position, direction, speedMultiplier, damageMultiplier, CombinedFloor, mapEntityID, baseDamage);
        }
        public void AddSound(SoundEffect2D sfx) { tileMap.AddSound(sfx, ref refPosition); }

        // Miscellaneous Methods
        public void SetTile(Point tileLocation, bool forceCameraFocus = true)
        {
            position = new Vector2(tileLocation.X * TileMap.TileWidth + (TileMap.TileWidth / 2), tileLocation.Y * TileMap.TileHeight + (TileMap.TileHeight / 2));

            if (isPlayerControlled == true)
            {
                if (forceCameraFocus == true)
                    camera.ForceLookAt(position);
            }
        }
        public void ActivateObject() { IsActivatingObject = true; }
        public virtual void SOULGATE_Rest()
        {
            HEALTH_Restore((uint)controlledEntity.Skills.health.MaxHP);
            MAGIC_Restore((uint)controlledEntity.Skills.wisdom.MaxEnergy);
            STAMINA_Restore(controlledEntity.Skills.endurance.MaxStamina);

            STORAGE_RepairAll(); //Repairs all non-broken items.
            storage.ForceChargeAll();

            status.RemoveAll(true);
            visualOverlay.Clear();

            if (this is PlayerEntity)
            {
                entityManager.ReviveMonsters(true);
                Revive();
            }
        }

        #region [Methods] Death 

        protected void ForceDeath()
        {
            ANIMATION_Pause();
            HEALTH_Damage(10000000, "LORDLY SPIRIT", false);

            isDeathReset = true;
            isBodyDisappear = true;
            isDeathParticleAdded = true;
            deathWaitTime = 1000;

            entityCircle.radius = 0f;
            jumpCircle.radius = 0f;
        }

        protected string killerID;
        public virtual void InitiateDeath()
        {
            ANIMATION_ResetValues();

            if (EQUIPMENT_PrimaryCombat() != null) EQUIPMENT_PrimaryCombat().ForceStop();
            if (EQUIPMENT_OffhandCombat() != null) EQUIPMENT_OffhandCombat().ForceStop();

            if (!(this is PlayerEntity))
            {
                DropLoot();
                SendXP();
                SKILL_ClearEmbers();
                restCount = playerEntity.SoulgateRestCount; //Assign rest count value to monster when killed.
            }

            if (this is PlayerEntity)
            {
                worldManager.ForceSavePlayerData();
                tileMap.EMBER_AddPile(CurrentFloor, Position, this, skillset.ExperiencePoints);
            }

            if (statistics != null)
                STATISTIC_AdjustStat("total_deaths", 1);

            status.RemoveAll(true);
            visualOverlay.Clear();
        }
        public virtual void FinalizeDeath()
        {
        }
        public bool IsDead { get { return skillset.health.IsDead; } }
        public void Revive()
        {
            ANIMATION_Play();

            status.RemoveAll(true);
            skillset.RestoreSkills();

            isDeathReset = false;
            isBodyDisappear = false;
            isDeathParticleAdded = false;
            deathWaitTime = 0;

            entityCircle.radius = baseEntityCircle.radius;
            jumpCircle.radius = baseJumpCircle.radius;

            STATE_SetAction("Idle");

            if (!(this is PlayerEntity) && isPlayerControlled == false)
                SetTile(startTile, false);
        }
        public void SendXP()
        {
            for (int i = 0; i < mapEntities.Count; i++)
            {
                if (killerID != null)
                {
                    if (killerID.ToUpper() != mapEntityID) //Prevent cheating to get XP by killing yourself. That would... not send a good message.
                    {
                        if (mapEntities[i].MapEntityID.ToUpper().Equals(killerID.ToUpper()))
                        {
                            mapEntities[i].SKILL_AddEmbers(LOOT_Experience() + skillset.ExperiencePoints);

                            if (mapEntities[i].statistics != null)
                                mapEntities[i].STATISTIC_AdjustStat("beings_slain", 1);
                            if (statistics != null)
                            {
                                if (STATISTIC_GetStatInteger("most_embers_lost") < skillset.ExperiencePoints)
                                    STATISTIC_SetStat("most_embers_lost", skillset.ExperiencePoints);
                            }

                            Skills.ExperiencePoints = 0;
                            deathImage.KillerEntity = mapEntities[i];

                            break;
                        }
                    }
                }
            }
        }

        #endregion

        public virtual BaseEntity DeepCopy(TileMap map, Camera camera)
        {
            BaseEntity copy = (BaseEntity)this.MemberwiseClone();

            copy.depthFloor = new DepthFloor();
            copy.animation = animation.Copy();
            copy.skillset = skillset.Copy();
            copy.attributes = attributes.Copy();
            copy.skillset.SetReferences(copy.attributes);

            copy.visualOverlay = new EntityVisual();
            copy.visualOverlay.SetReferences(map, camera, screens, player, weather, culture, controlledEntity, entities);
            copy.status = new EntityStatus(copy);
            copy.info = info.Copy();
            copy.statistics = statistics.Copy(); ;

            copy.caption = new EntityCaption(new Vector2(0, -55));
            copy.chat = new EntityChat(copy.name);

            copy.equipment = equipment.Copy();
            copy.storage = storage.Copy(screens, map, copy, camera);
            copy.loot = loot.Copy(screens, map, copy, debug);

            if (copy.agentAI != null)
                copy.agentAI = agentAI.Copy(copy, map);

            copy.steering = new SteeringBehavior(copy);

            copy.kin = kin.Copy();
            copy.faction = Faction;

            copy.mapEntities = new List<BaseEntity>();
            copy.validEnemies = new List<BaseEntity>();
            copy.validAllies = new List<BaseEntity>();
            copy.currentEnemies = new List<BaseEntity>();
            copy.currentAllies = new List<BaseEntity>();

            copy.selectionCircle = new Circle(this.selectionCircle.radius);
            copy.selectionGroupObjects = new List<GameObject>();

            copy.senses = senses.Copy();
            copy.memory = new ObjectMemory();
            
            copy.entityCircle = new Circle(entityCircle.radius);
            copy.particleCircle = new Circle(particleCircle.radius);
            copy.jumpCircle = new Circle(jumpCircle.radius);

            copy.unprocessedMessages = new List<MessageHolder>();
            copy.lastProcessedMessages = new List<MessageHolder>();
            copy.dispositions = new List<EntityDisposition>();

            copy.processedDamage = new Dictionary<string, int>();
            copy.unprocessedDamage = new Dictionary<string, Tuple<uint, int, string[]>>();

            copy.deathImage = new EmberImage(15, animation.AnimationSheet, animation.DeathTexture, 2f);
            copy.jumpDust = new JumpCircle(Color.Lerp(Color.Transparent, ColorHelper.Charcoal, .75f), 150f, 1000);

            copy.enemyTarget = null;
            copy.allyTarget = null;

            copy.random = new Random(Guid.NewGuid().GetHashCode());

            return copy;
        }

        public new string SaveData()
        {
            if (isSavable == true)
                return "Entity " + mapEntityID.Replace(' ', '_') + " " + IsDead + " " + restCount;
            else
                return string.Empty;
        }
        public virtual void LoadData(List<string> data, int playerRestCount)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].ToUpper().StartsWith("ENTITY"))
                {
                    string[] words = data[i].Split(' ');
                    words[1] = words[1].Replace('_', ' ');

                    if (words[1].ToUpper().Equals(mapEntityID.ToUpper()))
                    {
                        bool isDead = bool.Parse(words[2]);
                        restCount = int.Parse(words[3]);

                        //The monster is dead and the player has not rested at a soulgate since!
                        if (restCount == playerRestCount && isDead == true)
                            ForceDeath();

                        //Monster may or may not be dead. But none of that matters now, because the player rested at a soulgate after their deaths!
                        if (restCount != playerRestCount)
                            restCount = playerRestCount;
                    }
                }
            }
        }

        public void ReloadStateFiles()
        {
            senses.ReloadState();
            skillset.ReloadState();
            animation.ReloadState();
            storage.ReloadState();
        }
        //Variables (circleRadius, hitbox, etc...)

        public override string ToString()
        {
            return "[" + this.GetType().Name + " of " + this.GetType().BaseType.Name + "] - ID:" + id.ToString() + " Floor:" + BaseCurrentFloor.ToString() + " MapID:" + mapEntityID;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            BaseEntity entity = (BaseEntity)obj;
            return (this.mapEntityID == entity.mapEntityID);
        }
        public override int GetHashCode()
        {
            int result = int.Parse(Regex.Replace(mapEntityID, "[0-9]", ""));
            return result;
        }

        //GameObject Overrides
        public override void InitializeSuggestLine()
        {
            objectType = TileEngine.Map.Editor.AutoSuggestionObject.ObjectType.Entities;
            suggestLines.Add("Monster int MonsterID, int ObjectID, int CurrentFloor, Point TileLocation, float LookDirection");
        }
        public override string MapOutputLine()
        {
            //MONSTER id objectID currentFloor startTile lookDirection
            return "Monster " + id + " " + base.id + " " + startFloor + " " + startTile.X + " " + startTile.Y + " " + lookDirection;
        }

        //void AnimationSetTimePerFrame(uint milliseconds), Point _AnimationCurrentFrame_(), uint _AnimationGetTimePerFrame_(), bool _IsAnimationFinished_()

        public override void SetDisplayVariables()
        {
            //Some of these have been stacked in one line, as I can see the total methods getting very crowded, very quickly. Tooltips will go offscreen.
            displayVariables.AppendLine("Point StartTile (" + startTile.X + ", " + startTile.Y + ")");
            displayVariables.AppendLine("void AddStatus(int id), void RemoveStatus(int id), bool _CheckForStatus_(int id)");
            displayVariables.AppendLine("int _StatusSecondsLeft_(int id), void ClearAllStatuses(), void AddVisual(int id)");
            displayVariables.AppendLine("void StopVisual(int id), void StopAllVisuals(int id), void ClearAllVisuals()");
            displayVariables.AppendLine("void AddXP(int xp), void RemoveXP(int xp), void ClearXP()");
            displayVariables.AppendLine("void DamageHP(uint value, bool sendDigits), void RestoreHP(uint value, bool sendDigits)");
            displayVariables.AppendLine("void DamageStamina(float value), void RestoreStamina(float value)");
            displayVariables.AppendLine("void UseMagic(uint value), void RestoreMagic(uint value), int _SpellSlots_()");
            displayVariables.AppendLine("int _SkillLevel_(string skill), void AnimationSetTimePerFrame(uint milliseconds)");
            displayVariables.AppendLine("Point _AnimationCurrentFrame_(), uint _AnimationGetTimePerFrame_()");
            displayVariables.AppendLine("bool _IsAnimationFinished_(), SendCaptionImmediate(string message), QueueCaption(string message)");
            displayVariables.AppendLine("string _CurrentCaption_(), void ResetCaption()");
            displayVariables.AppendLine("void MoveDown(int value) [Temporary]");
            displayVariables.AppendLine("");
            displayVariables.AppendLine("");
            displayVariables.AppendLine("");
            displayVariables.AppendLine("");

            displayVariables.AppendLine("void Kill()");
            displayVariables.AppendLine("--------------------");
            base.SetDisplayVariables();
        }
        public override void ParseEdit(string line, string[] words)
        {
            try
            {
                if (line.ToUpper().StartsWith("KILL"))
                    skillset.health.CurrentHP = 0;
                if (line.ToUpper().StartsWith("ADDSTATUS"))
                    STATUS_AddStatus(int.Parse(words[1]), "LordlySpirit");
                if (line.ToUpper().StartsWith("STARTTILE"))
                {
                    Point tile = new Point().Parse(words[1], words[2]);

                    startTile = tile;
                    SetTile(tile);
                }
                if (line.ToUpper().StartsWith("MOVEDOWN"))
                    position.Y += int.Parse(words[1]);
            }
            catch
            {
            }

            base.ParseEdit(line, words);
        }

        public override string RetrieveVariable(string name)
        {
            //if (name.ToUpper().StartsWith("ITEMID"))
            //    return itemID.ToString();
            return string.Empty;
        }
    }
}
