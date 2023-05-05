using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.AI.Goal;
using Pilgrimage_Of_Embers.Entities.AI.Goal.Type;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using System.Linq;
using Pilgrimage_Of_Embers.Entities.Types.NPE;
using Pilgrimage_Of_Embers.TileEngine;

namespace Pilgrimage_Of_Embers.Entities.AI.Agents.Type
{
    public class AdvancedAgentTest : BaseAgent
    {
        private GlanceAround idleGlance;

        private Vector2 startTile;
        private bool hasHealingItem = false;

        private DesireHolder glanceDesire;

        private DesireHolder fleeDesire;
        private FleeAI flee;

        private DesireHolder useItemDesire;
        private UseItem useHealItem;

        private DesireHolder attackDesire;
        private UseWeapon useWeapon;

        private DesireHolder followEnemyDesire;
        private FollowPath followPath;

        private Point lastEnemyTile;

        public AdvancedAgentTest() : base() { }

        public override void Initialize()
        {
            currentEntity.SENSES_TurnSpeed = ObjectSenses.RotateSpeed.Medium;
            startTile = new Vector2(currentEntity.StartTile.X * TileMap.TileWidth, currentEntity.StartTile.Y * TileMap.TileHeight);

            hasHealingItem = currentEntity.STORAGE_Check("Potion", "Healing");

            currentEntity.EQUIPMENT_EquipWeapon((Weapon)currentEntity.STORAGE_GetItem(1000), 1);

            glanceDesire = new DesireHolder(0);
            fleeDesire = new DesireHolder(0);
            useItemDesire = new DesireHolder(0);
            attackDesire = new DesireHolder(0);
            followEnemyDesire = new DesireHolder(0);

            idleGlance = new GlanceAround("Glancing", currentEntity, glanceDesire, -2f, 2f, 1000, 2000);
            flee = new FleeAI("FleeFrom", currentEntity, fleeDesire, Vector2.Zero);
            useHealItem = new UseItem("UseHealingItem", currentEntity, useItemDesire, "Potion", "Healing", 1);
            useWeapon = new UseWeapon("Attack", currentEntity, attackDesire, true, 150, CombatEngine.CombatMove.Basic, CombatEngine.CombatMove.Power);
            followPath = new FollowPath("PathToEnemy", currentEntity, Point.Zero, true, followEnemyDesire);


            AddGoal(idleGlance);

            AddGoal(followPath);
            AddGoal(useHealItem);

            AddGoal(flee);
            AddGoal(useWeapon);

            base.Initialize();
        }

        public override void UpdateState(GameTime gt)
        {
            if (currentEntity.CurrentEnemies.Count == 0)
            {
                if (Vector2.Distance(currentEntity.Position, startTile) > 128)
                {
                    AddGoal(new FollowPath("GoToStartTile", currentEntity, new Point(currentEntity.StartTile.X * 2, currentEntity.StartTile.Y * 2), false, new DesireHolder(50)));
                    glanceDesire.Desire = 0;
                }
                else
                    glanceDesire.Desire = 100;

                attackDesire.Desire = 0;
                followEnemyDesire.Desire = 0;
            }

            if (currentEntity.CurrentEnemies.Count > 0)
            {
                glanceDesire.Desire = 0;
                attackDesire.Desire = 0f;
                followEnemyDesire.Desire = 0f;
                useItemDesire.Desire = 0f;
                fleeDesire.Desire = 0f;

                currentEntity.STEERING_LookAt(currentEntity.CurrentEnemies.FirstOrDefault().Position);

                if (currentEntity.Skills.health.CurrentHP < currentEntity.Skills.health.MaxHP * .25f && hasHealingItem == true)
                {
                    if (Vector2.Distance(currentEntity.Position, currentEntity.CurrentEnemies.FirstOrDefault().Position) < 500)
                    {
                        fleeDesire.Desire = 100f;
                        flee.FleeFrom = currentEntity.CurrentEnemies.FirstOrDefault().Position;
                    }

                    if (currentEntity.STEERING_State() == Steering_Behaviors.SteeringBehavior.MovementState.None)
                    {
                        hasHealingItem = currentEntity.STORAGE_Check("Potion", "Healing");
                        useItemDesire.Desire = 90f;

                        lastEnemyTile = Point.Zero;
                    }

                    currentEntity.STATE_SetAction("Walk");
                }

                if (currentEntity.Skills.health.CurrentHP > currentEntity.Skills.health.MaxHP * .25f || hasHealingItem == false)
                {
                    if (Vector2.Distance(currentEntity.Position, currentEntity.CurrentEnemies.FirstOrDefault().Position) < 150) //In attack range
                        attackDesire.Desire = 90f;
                    else
                    {
                        followEnemyDesire.Desire = 100f;
                        currentEntity.STATE_SetAction("Sprint");
                    }

                    if (lastEnemyTile != currentEntity.CurrentEnemies.FirstOrDefault().pathfindTile) //If enemy has moved tiles, reroute!
                    {
                        followPath.Goal = currentEntity.CurrentEnemies.FirstOrDefault().pathfindTile;
                        followPath.State = BaseGoal.GoalState.Inactive;
                    }

                    lastEnemyTile = currentEntity.CurrentEnemies.FirstOrDefault().pathfindTile;
                }
            }

            base.UpdateState(gt);
        }

        /*
if (currentEntity.MEMORY_ContainsShort("INVESTIGATE_PERIPHERAL"))
{
    AddGoal(investigatePoint);

    investigateDesire.Desire = 30;
    string memory = currentEntity.MEMORY_FetchShortMemory("INVESTIGATE_PERIPHERAL");

    if (!string.IsNullOrEmpty(memory))
    {
        investigatePoint.State = BaseGoal.GoalState.Inactive;

        string[] words = memory.Split(' ');

        Vector2 position = Vector2.Zero;

        float.TryParse(words[1], out position.X);
        float.TryParse(words[2], out position.Y);

        investigatePoint.Position = position;
    }
}*/
    }
}
