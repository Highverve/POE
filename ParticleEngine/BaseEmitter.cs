using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.ParticleEngine.Particles;
using System.Linq;

namespace Pilgrimage_Of_Embers.ParticleEngine
{
    public class BaseEmitter : GameObject
    {
        protected List<Particle> particles = new List<Particle>(), recycle = new List<Particle>();
        public List<Particle> Particles { get { return particles; } }

        protected Particle NextRecycle()
        {
            Particle value = null;

            if (recycle.Count > 0)
            {
                value = recycle.First(); //Return recycled particle
                recycle.Remove(recycle.First());
                Particle.RecycledParticles--;
            }

            if (value == null)
                return new Particle(); //If there are no recycled particles, create a new particle.

            if (this is ParticleTypes.HiddenItem)
                Debugging.DebugManager.info.SetVariable("Regular Recycled: " + recycle.Count, 1);

            return value;
        }
        protected TrailParticle NextTrailRecycle()
        {
            TrailParticle value = null;

            if (recycle.Count > 0)
            {
                for (int i = 0; i < recycle.Count; i++)
                {
                    if (recycle[i] is TrailParticle)
                    {
                        value = (TrailParticle)recycle[i]; //Return recycled particle
                        recycle.Remove(recycle[i]);
                        Particle.RecycledParticles--;

                        break;
                    }
                }
            }

            Debugging.DebugManager.info.SetVariable("Trail Recycled: " + recycle.Count, 2);
            
            if (value == null)
                return new TrailParticle(); //If there are no recycled particles, create a new particle.

            return value;
        }
        protected DualConnectedParticle NextDualConnectedRecycle()
        {
            DualConnectedParticle value = null;

            if (recycle.Count > 0)
            {
                for (int i = 0; i < recycle.Count; i++)
                {
                    if (recycle[i] is DualConnectedParticle)
                    {
                        value = (DualConnectedParticle)recycle[i]; //Return recycled particle
                        recycle.Remove(recycle[i]);
                        Particle.RecycledParticles--;

                        break;
                    }
                }
            }

            if (value == null)
                return new DualConnectedParticle(); //If there are no recycled particles, create a new particle.

            return value;
        }
        protected FloorParticle NextFloorRecycle()
        {
            FloorParticle value = null;

            if (recycle.Count > 0)
            {
                for (int i = 0; i < recycle.Count; i++)
                {
                    if (recycle[i] is FloorParticle)
                    {
                        value = (FloorParticle)recycle[i]; //Return recycled particle
                        recycle.Remove(recycle[i]);
                        Particle.RecycledParticles--;

                        break;
                    }
                }
            }

            if (value == null)
                return new FloorParticle(); //If there are no recycled particles, create a new particle.

            return value;
        }
        protected FloorConnectedParticle NextFloorConnectedRecycle()
        {
            FloorConnectedParticle value = null;

            if (recycle.Count > 0)
            {
                for (int i = 0; i < recycle.Count; i++)
                {
                    if (recycle[i] is FloorConnectedParticle)
                    {
                        value = (FloorConnectedParticle)recycle[i]; //Return recycled particle
                        recycle.Remove(recycle[i]);
                        Particle.RecycledParticles--;

                        break;
                    }
                }
            }

            if (value == null)
                return new FloorConnectedParticle(); //If there are no recycled particles, create a new particle.

            return value;
        }
        protected ConnectedParticle NextConnectedRecycle()
        {
            ConnectedParticle value = null;

            if (recycle.Count > 0)
            {
                for (int i = 0; i < recycle.Count; i++)
                {
                    if (recycle[i] is ConnectedParticle)
                    {
                        value = (ConnectedParticle)recycle[i]; //Return recycled particle
                        recycle.Remove(recycle[i]);
                        Particle.RecycledParticles--;

                        break;
                    }
                }
            }

            if (value == null)
                return new ConnectedParticle(); //If there are no recycled particles, create a new particle.

            return value;
        }

        protected ParticleValues values;
        public ParticleValues Values { get { return values; } }

        public BaseEmitter(int ID, int CurrentFloor, float DepthOffset) : base(ID, CurrentFloor, DepthOffset) { trueRandom = new Random(Guid.NewGuid().GetHashCode()); }

        public override void Update(GameTime gt)
        {
            UpdateParticles(gt);
        }

        public void SpawnParticles(GameTime gt, ParticleValues values)
        {
            UpdateDepth(depthOrigin); values.depth = Depth;

            values.respawn -= gt.ElapsedGameTime.Milliseconds;
            if (values.respawn <= 0)
            {
                for (int i = 0; i < values.spawnQuantity; i++)
                {
                    ApplyRandomValues();

                    if (isActivated == true && IsOnScreen())
                    {
                        particles.Add(new Particle(values.Texture, values.position, values.velocity, values.angle, values.color, values.size, values.lifeTime, Depth));
                    }

                    if (!IsOnScreen())
                    {
                        if (particles.Count > 0)
                            ForceRecycleAll();
                    }
                }

                values.respawn = values.respawnTime;
            }
        }

        protected int spawnTimer;
        public void SpawnParticles(GameTime gt, int RespawnTime, int SpawnCount, Action SpawnCall, bool CheckOnScreen)
        {
            bool isOnScreen = true;
            if (CheckOnScreen == true)
                isOnScreen = IsOnScreen();

            if (isActivated == true && isOnScreen == true)
            {
                UpdateDepth(depthOrigin);

                spawnTimer += gt.ElapsedGameTime.Milliseconds;
                if (spawnTimer >= RespawnTime)
                {
                    for (int i = 0; i < SpawnCount; i++)
                    {
                        ApplyRandomValues();
                        SpawnCall();
                    }

                    if (CheckOnScreen == true)
                    {
                        if (!IsOnScreen())
                        {
                            if (particles.Count > 0)
                                ForceRecycleAll();
                        }
                    }

                    spawnTimer = 0;
                }
            }
        }
        protected virtual void ApplyRandomValues() { }

        private void UpdateParticles(GameTime gt)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                if (particles[i].IsActive == true)
                {
                    particles[i].Update(gt);

                    ApplyParticleBehavior(gt, i);

                    if (GameSettings.ParticleInteraction == true)
                        ApplyInteractionBehavior(gt, i);

                    if (particles[i].TimeLeft <= 0)
                    {
                        particles[i].RecycleParticle(); //Recycle the particle's variables
                        recycle.Add(particles[i]); //Add the particle to the recycle list
                        particles.Remove(particles[i]); //Remove from current particle list
                        Particle.RecycledParticles++;
                    }
                }
            }
        }
        protected virtual void ApplyParticleBehavior(GameTime gt, int i) { }
        protected virtual void ApplyInteractionBehavior(GameTime gt, int i) { }

        public override void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                //if (particles[i].IsActive == true)
                    particles[i].Draw(sb);
            }
        }

        public override void SetDisplayVariables()
        {
            displayVariables.AppendLine("int Recycled (" + recycle.Count + ")");

            base.SetDisplayVariables();
        }
        public override void ParseEdit(string line, string[] words)
        {
            if (line.ToUpper().StartsWith("DEDUPE"))
            {
                for (int i = 0; i < recycle.Count; i++)
                {
                    for (int j = i + 1; j < recycle.Count; j++)
                    {
                        if (recycle[i].Equals(recycle[j]))
                            recycle.Remove(recycle[j]);
                    }
                }
            }

            base.ParseEdit(line, words);
        }

        protected int LimitSpawnQuantity(int quantity)
        {
            float temp = quantity;

            temp *= GameSettings.LimitPercentage();
            if (temp < .5f)
                temp = 1f;

            return (int)temp;
        }

        public void ForceRemoveAll()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles.RemoveAt(i);
                Particle.CurrentParticles--;
            }

            for (int i = 0; i < recycle.Count; i++)
            {
                recycle.RemoveAt(i);
                Particle.RecycledParticles--;
            }
        }
        public void ForceRecycleAll()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].RecycleParticle(); //Recycle the particle's variables
                recycle.Add(particles[i]); //Add the particle to the recycle list
                particles.Remove(particles[i]); //Remove from current particle list
                Particle.RecycledParticles++;
            }
        }

        public virtual BaseEmitter Copy()
        {
            BaseEmitter copy = (BaseEmitter)MemberwiseClone();

            copy.particles = new List<Particle>();
            copy.isManualDepth = true;

            return copy;
        }
    }
}
