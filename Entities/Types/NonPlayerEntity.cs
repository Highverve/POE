using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Entities.AI.Goal;
using Pilgrimage_Of_Embers.Entities.Steering_Behaviors;
using Pilgrimage_Of_Embers.Skills;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Entities.AI.Goal.Type;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Factions;
using Pilgrimage_Of_Embers.Entities.AI.Agents;
using Pilgrimage_Of_Embers.Entities.Classes;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.Performance;

namespace Pilgrimage_Of_Embers.Entities.Types
{
    public class NonPlayerEntity : BaseEntity
    {
        public enum CombatType
        {
            Melee,
            Archery,
            Magic,
            Flaskmaster,

            MeleeArchery,
            MeleeMagic,
            MeleeFlaskmaster,

            ArcheryMagic,
            ArcheryFlaskmaster,

            MagicFlaskmaster,

            Hybrid, //Melee, Archery, Magic
            Quabrid, //Melee, Archery, Magic, Flaskmaster
            Distancer //Archery, Magic, Flaskmaster
        }
        public enum RespawnFrequency { None, OncePerAghtene, Always }

        protected Vector2 lastTargetPos;
        public Vector2 LastTargetPos { get { return lastTargetPos; } set { lastTargetPos = value; } }

        public enum CombatTemperament
        {
            Flee = 0,
            Passive = 1,
            Neutral = 2,
            Aggressive = 3
        }
        protected CombatTemperament temperament = CombatTemperament.Neutral;
        private Texture2D dispositionGradient;

        public NonPlayerEntity(int ID, string Name, AnimationState Animation, Skillset Skills, ObjectAttributes Attributes, EntityLoot Drops, EntityStorage Storage, BaseFaction Faction, ObjectSenses Senses, EntityKin Kin, float CircleRadius, float CenterOffset, float DepthOffset, float ShadowOffset, float InfoHeight, BaseAgent AgentAI)
            : base(ID, Name, Animation, Skills, Attributes, Drops, Storage, Faction, Senses, Kin, CircleRadius, CenterOffset, DepthOffset, ShadowOffset, InfoHeight, AgentAI)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void SetMapInfo(PathfindMap map)
        {
            pathfinder = new Pathfinder(map);
        }

        public override void Load(ContentManager cm)
        {
            dispositionGradient = cm.Load<Texture2D>("Effects/Gradients/dispositionColor");

            //osPoint = new ScreenEngine.OffscreenPoint(null, ScreenEngine.OffscreenPoint.Red);
            //screens.OFFSCREEN_AddPoint(osPoint);

            base.Load(cm);
        }

        private CallLimiter limitDispositionColor = new CallLimiter(5000);
        private EntityDisposition controlledEntityDisposition;
        public override void Update(GameTime gt)
        {
            if (IsDead == false)
            {
                if (isPlayerControlled == false)
                {
                    if (SUSPENSION_Action == Suspension.SuspendState.None)
                    {
                        agentAI.Update(gt);
                        steering.Update(gt);
                    }
                }
            }

            UpdateDisposition(gt);

            base.Update(gt);
        }
        protected void UpdateDisposition(GameTime gt)
        {
            for (int i = 0; i < dispositions.Count; i++)
            {
                if (dispositions[i].Entity.IsPlayerControlled == true)
                    controlledEntityDisposition = dispositions[i];
            }

            if (IsPlayerControlled == false)
            {
                if (limitDispositionColor.IsCalling(gt))
                    info.TextColor = dispositionGradient.SelectColor(controlledEntityDisposition.Disposition + 100);
            }
            else
                info.TextColor = new Color(144, 185, 219, 255);
        }

        public override void Draw(SpriteBatch sb)
        {
            DrawInfo(sb);

            base.Draw(sb);
        }
        public override void DrawUI(SpriteBatch sb)
        {

            base.DrawUI(sb);
        }

        protected override void HandleMessage(MessageHolder message)
        {
            if (message.Message.ToUpper().Equals("GIFT"))
            {
                //BaseEntity entity = (BaseEntity)message.Sender;
                //BaseItem i = (BaseItem)message.Baggage;
            }

            if (message.Message.ToUpper().Equals("COMPANIONS"))
            {
                BaseEntity leader = (BaseEntity)message.Sender;

                if (HasCompanionLeader == true)
                {
                    if (leader.MapEntityID == companionLeader.MapEntityID)
                    {
                        //Do something
                    }
                }
            }

            base.HandleMessage(message);
        }

        public override Vector2 STEERING_Direction() { return steering.Direction; }
        public override Vector2 STEERING_Cross() { return steering.Cross; }
    }
}
