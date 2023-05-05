using System.Text;
using Pilgrimage_Of_Embers.Entities.AI.Goal;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Types.NPE;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.Performance;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.AI.Agents
{
    public class BaseAgent
    {
        public enum AgentState { Idle, Combat,  }

        protected CompositeGoal currentAI;
        protected BaseEntity currentEntity;
        protected TileMap map;
        protected DesireManager desire;

        private CallLimiter limitAICheck = new CallLimiter(30); //Only check AI state every *time*

        protected bool isRemoveGoals = false, isInitialized = false;

        public BaseAgent()
        {
        }
        public void SetReferences(BaseEntity entity, TileMap map)
        {
            currentEntity = entity;
            this.map = map;

            currentAI = new CompositeGoal(entity.Name, entity, new DesireHolder(0)); //Desire shouldn't matter here.
            desire = new DesireManager(entity);
        }

        private StringBuilder builder = new StringBuilder();

        public virtual void Initialize() { }
        public void Update(GameTime gt)
        {
            if (isInitialized == false)
            {
                Initialize();
                isInitialized = true;
            }

            currentAI.Update(gt);

            UpdateState(gt);

            /*
            if (currentAI.Subgoals.Count > 0 && GameSettings.IsDebugging == true)
            {
                builder.Clear();
                for (int i = 0; i < currentAI.Subgoals.Count; i++)
                    builder.AppendLine(currentAI.Subgoals[i].goalName + " " + currentAI.Subgoals[i].Desire.Desire);

                currentEntity.CAPTION_SendImmediate(builder.ToString());
            }*/

            if (isRemoveGoals == true)
            {
               RemoveAllGoals();
               isRemoveGoals = false;
            }
        }
        public virtual void UpdateState(GameTime gt)
        {

        }

        public void AddGoal(BaseGoal goal) { currentAI.AddSubgoal(goal); }
        public void RemoveAllGoals() { currentAI.RemoveAllSubgoals(); }

        public bool IsGoalPresent(BaseGoal goal) { return currentAI.IsGoalPresent(goal); }
        public bool IsGoalPresent(string goalName) { return currentAI.IsGoalPresent(goalName); }

        public bool ContainsGoal(BaseGoal goal) { return currentAI.ContainsGoal(goal); }
        public bool ContainsGoal(string goalName) { return currentAI.ContainsGoal(goalName); }

        public BaseGoal TargetGoal(BaseGoal goal) { return currentAI.TargetGoal(goal); }
        public BaseGoal TargetGoal(string goalName) { return currentAI.TargetGoal(goalName); }

        public virtual BaseAgent Copy(BaseEntity entity, TileMap map)
        {
            BaseAgent copy = (BaseAgent)this.MemberwiseClone();
            copy.SetReferences(entity, map);

            return copy;
        }
    }
}
