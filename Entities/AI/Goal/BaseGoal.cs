using System;
using Pilgrimage_Of_Embers.Entities.Entities;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.Types.NPE;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal
{
    public class BaseGoal
    {
        public enum GoalState { Inactive, Active, Completed, Failed }
        protected GoalState state = GoalState.Inactive;
        public GoalState State { get { return state; } set { state = value; } }

        public string goalName = "Empty!";

        private int failedCount = 0, failTolerance = 3;

        private DesireHolder desire;
        public DesireHolder Desire { get { return desire; } }

        protected BaseEntity entity, targetEntity;

        protected Random random;

        public BaseGoal(string GoalName, BaseEntity Entity, DesireHolder Desire, int FailTolerance = 3)
        {
            goalName = GoalName;
            entity = Entity;
            desire = Desire;
            failTolerance = FailTolerance;

            random = new Random(Guid.NewGuid().GetHashCode());
        }

        public virtual void Initialize() { } //runs on goal activation
        public virtual void Update(GameTime gt) { ActivateIfInactive(); } //runs each update step
        public virtual void Terminate() { } //runs on goal termination

        public void ActivateIfInactive()
        {
            if (state == GoalState.Inactive)
            {
                state = GoalState.Active;
                Initialize();
            }
        }

        public void ReactivateIfFailed()
        {
            if (failedCount < failTolerance)
            {
                Initialize();
                state = GoalState.Active;

                failedCount++;
            }
        }

        public void TerminateIfCompletedOrFailed()
        {
            if (state == GoalState.Completed || failedCount >= failTolerance)
            {
                Terminate();
            }
        }

        public virtual void AddSubgoal(BaseGoal goal) { }
        public virtual bool IsCompositeGoal() { return false; }
        public void ForceTerminate() { state = GoalState.Failed; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            BaseGoal goal = (BaseGoal)obj;
            return (this.goalName == goal.goalName);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
