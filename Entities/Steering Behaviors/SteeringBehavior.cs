using System;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.Steering_Behaviors
{
    public class SteeringBehavior
    {
        protected Vector2 direction, cross;
        public Vector2 Direction { get { return direction; } }
        public Vector2 Cross { get { return cross; } }

        protected BaseEntity currentEntity;
        private Vector2 targetPosition;
        private Point goal;

        public enum MovementState
        {
            None,
            Arrive,
            Flee,
            Flank,
            Pathfind,
        }
        public MovementState State { get; private set; }
        private Random random;

        public SteeringBehavior(BaseEntity CurrentEntity)
        {
            currentEntity = CurrentEntity;

            random = new Random(Guid.NewGuid().GetHashCode());
        }

        public void Update(GameTime gt)
        {
            direction = currentEntity.SENSES_CurrentDirection.ToVector2();
            cross = direction.Cross();

            DefaultSpeed();

            switch (State)
            {
                case MovementState.Arrive: currentEntity.SetMovementMotion(Arrive(targetPosition, true, currentEntity.EntityCircle.radius * 1.5f)); break;
                case MovementState.Flee: currentEntity.SetMovementMotion(Arrive(targetPosition, false, currentEntity.EntityCircle.radius * 10f)); break;
                case MovementState.Flank: currentEntity.SetMovementMotion(Flank(targetPosition, true)); break;
                case MovementState.Pathfind: currentEntity.SetMovementMotion(Pathfind(gt, currentEntity.pathfindTile, goal, true)); break;
            }
        }

        public Vector2 Arrive(Vector2 target, bool isForward = true, float safeDistance = 50)
        {
            Vector2 DesiredVelocity;
            
            if (isForward)
                DesiredVelocity = Vector2.Normalize(target - currentEntity.Position);
            else
                DesiredVelocity = Vector2.Normalize(currentEntity.Position - target);

            float distance = Vector2.Distance(currentEntity.Position, target);

            if (isForward == true) //Arriving
            {
                if (distance <= safeDistance)
                    State = MovementState.None;
            }
            else //Fleeing
            {
                if (distance >= safeDistance)
                    State = MovementState.None;
            }

            return DesiredVelocity;
        }
        public Vector2 Flank(Vector2 position, bool isLeft)
        {
            Vector2 targetPosition, exactCross;

            exactCross = currentEntity.Position.Direction(position).ToVector2();
            exactCross.Normalize();
            exactCross = exactCross.Cross();

            if (isLeft == true)
                targetPosition = currentEntity.Position + (exactCross * 50f);
            else
                targetPosition = currentEntity.Position + (exactCross * 50f);

            return Arrive(targetPosition, true, 10);
        }

        private Point lastTile = Point.Zero; private bool isPathInitialized;
        public Vector2 Pathfind(GameTime gt, Point start, Point goal, bool removeLastTile = false)
        {
            if (isPathInitialized == false)
            {
                InitializePathfind(start, goal, removeLastTile);
                isPathInitialized = true;
            }

            if (currentEntity.IsPathCompleted() == false)
            {
                LookAt(currentEntity.PathfindPosition);

                if (currentEntity.IsClose(8f))
                    currentEntity.RemoveCurrentTile();

                return Arrive(currentEntity.PathfindPosition, safeDistance: 5);
            }
            else
            {
                State = MovementState.None;
                ResetPath();
                currentEntity.STATE_SetAction("Idle");
            }

            currentEntity.CheckPathFailure(gt);

            return Vector2.Zero;
        }
        private void InitializePathfind(Point start, Point goal, bool removeLastTile)
        {
            currentEntity.Pathfind(start, goal);

            if (currentEntity.IsPathCompleted() == false) //If the path is not empty ... remove one
            {
                currentEntity.RemoveCurrentTile(); //Remove the first tile to prevent going backwards.

                if (removeLastTile == true)
                    currentEntity.RemoveLastTile();
            }

            lastTile = new Point(goal.X, goal.Y);
        }
        public void ResetPath() { isPathInitialized = false; }

        /*Old Protect Code
        public void Protect(float relaxRadius)
        {
            Vector2 point = enemyEntity.Position.Midpoint(allyEntity.Position);

            float distance = Vector2.Distance(currentEntity.Position, point);

            if (distance >= relaxRadius)
            {
                currentEntity.SENSES_SightDirection = currentEntity.Position.Direction(point);
                return Arrive(point, true, 32);
            }
            else
            {
                currentEntity.SENSES_SightDirection = currentEntity.Position.Direction(enemyEntity.Position);
                return Vector2.Zero;
            }
        }
        */
        /*Old Wander Code
        private int time = 0, stopTime = 0, increment = 0;
        private bool isStopped = false;
        public Vector2 Wander(GameTime gt, float wanderRadius, int minTime, int maxTime, int stoppingTime)
        {
            Vector2 projection = currentEntity.Position;

            if (isStopped == false)
            {
                time -= gt.ElapsedGameTime.Milliseconds;
                if (time <= 0)
                {
                    stopTime = stoppingTime;
                    currentEntity.SENSES_SightDirection += random.NextFloat(-2f, 2f);
                    increment++;
                    isStopped = true;
                }
                else
                {
                    projection = new Vector2((float)Math.Cos(currentEntity.SENSES_SightDirection) * wanderRadius + currentEntity.Position.X,
                                             (float)Math.Sin(currentEntity.SENSES_SightDirection) * wanderRadius + currentEntity.Position.Y);

                    if (increment % 2 == 0)
                        currentEntity.SENSES_SightDirection += 1f * (float)gt.ElapsedGameTime.TotalSeconds;
                    else
                        currentEntity.SENSES_SightDirection -= 1f * (float)gt.ElapsedGameTime.TotalSeconds;
                }
            }
            else if (isStopped == true)
            {
                stopTime -= gt.ElapsedGameTime.Milliseconds;
                if (stopTime <= 0)
                {
                    time = random.Next(minTime, maxTime);
                    isStopped = false;
                }
            }

            return Arrive(projection);
        }
        */


        public void DefaultSpeed()
        {
            currentEntity.Speed = currentEntity.MaxSpeed;
        }

        public void LookAt(Vector2 position)
        {
            currentEntity.SENSES_SightDirection = currentEntity.Position.Direction(position);
        }
        public void SetTargetPosition(Vector2 position)
        {
            targetPosition = position;
        }
        public void SetTargetTile(Point tile)
        {
            goal = tile;
        }

        public void SetState(MovementState movement)
        {
            State = movement;
        }
        public void ForceStop()
        {
            targetPosition = currentEntity.Position;
            State = MovementState.None;
        }
    }
}
