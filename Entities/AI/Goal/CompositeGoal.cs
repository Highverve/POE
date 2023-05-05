using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal
{
    public class CompositeGoal : BaseGoal
    {
        protected List<BaseGoal> subgoals;
        public List<BaseGoal> Subgoals { get { return subgoals; } }

        public CompositeGoal(string GoalContainerName, BaseEntity Entity, DesireHolder Desire) : base(GoalContainerName, Entity, Desire)
        {
            subgoals = new List<BaseGoal>();
        }

        public override void Initialize()
        {
        }
        public override void Update(GameTime gt)
        {
            UpdateSubgoals(gt);
        }
        public override void Terminate()
        {
        }

        public override void AddSubgoal(BaseGoal goal)
        {
            if (!ContainsGoal(goal))
            {
                subgoals.Insert(0, goal);
            }
        }

        float desireCheck;
        private void UpdateSubgoals(GameTime gt)
        {
            SortByDesire();

            for (int i = 0; i < subgoals.Count; i++)
            {
                if (i == 0)
                {
                    desireCheck = subgoals.FirstOrDefault().Desire.Desire;
                    subgoals[i].Update(gt);
                }
                else
                {
                    if (subgoals[i].Desire.Desire == desireCheck)
                        subgoals[i].Update(gt);
                    else
                        break;
                }
            }

            while (subgoals.Count != 0 && (subgoals.FirstOrDefault().State == GoalState.Completed || subgoals.FirstOrDefault().State == GoalState.Failed)) //remove completed or failed goals
            {
                subgoals.FirstOrDefault().Terminate();
                subgoals.Remove(subgoals.FirstOrDefault());
            }
            
            if (subgoals.Count > 1 && subgoals.FirstOrDefault().State == GoalState.Completed)
                state = GoalState.Active;
        }
        public void RemoveAllSubgoals()
        {
            for (int i = 0; i < subgoals.Count; i++)
            {
                subgoals[i].Terminate();
            }

            subgoals.Clear();
            state = GoalState.Completed;
        }

        public bool IsGoalPresent(BaseGoal goal)
        {
            if (subgoals.Count != 0) { return subgoals.FirstOrDefault() == goal; }

            return false;
        }
        public bool IsGoalPresent(string goalName)
        {
            if (subgoals.Count != 0) { return subgoals.FirstOrDefault().goalName.ToUpper() == goalName.ToUpper(); }

            return false;
        }

        public bool ContainsGoal(BaseGoal goal)
        {
            for (int i = 0; i < subgoals.Count; i++)
            {
                if (subgoals[i].Equals(goal))
                    return true;
            }

            return false;
        }
        public bool ContainsGoal(string goalName)
        {
            for (int i = 0; i < subgoals.Count; i++)
            {
                if (subgoals[i].goalName.ToUpper() == goalName.ToUpper())
                    return true;
            }

            return false;
        }

        public BaseGoal TargetGoal(BaseGoal goal)
        {
            for (int i = 0; i < subgoals.Count; i++)
            {
                if (subgoals[i] == goal)
                    return subgoals[i];
            }

            return null;
        }
        public BaseGoal TargetGoal(string goalName)
        {
            for (int i = 0; i < subgoals.Count; i++)
            {
                if (subgoals[i].goalName.ToUpper() == goalName.ToUpper())
                    return subgoals[i];
            }

            return null;
        }

        public override bool IsCompositeGoal() { return true; }

        private void SortByDesire()
        {
            subgoals.Sort((s1, s2) => s2.Desire.Desire.CompareTo(s1.Desire.Desire));
        }
    }
}
