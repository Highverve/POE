using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pilgrimage_Of_Embers.ParticleEngine
{
    public class ParticleValues
    {
        public Texture2D Texture { get; set; }

        public Vector2 position;
        public Vector2 velocity; //Direction particles travel

        public Vector2 spawnArea; //Area where the particles may spawn

        public float size, angle; //Size of particle

        public int respawnTime, respawn;   //Amount of time before next spawn
        public int spawnQuantity; //Amount of particles spawned at a time
        public int lifeTime;

        public Color color; //Color of particle

        public Vector2 offset, depthPos;

        public float depth;
        public int currentFloor;

        public ParticleValues(Texture2D Texture, Vector2 SpawnArea, float Size, int RespawnTime, int SpawnQuantity, int LifeTime)
            : this(Texture, Vector2.Zero, SpawnArea, Size, 0f, RespawnTime, SpawnQuantity, Color.White, Vector2.Zero, LifeTime)
        {
        }
        public ParticleValues(Texture2D Texture, Vector2 Velocity, Vector2 SpawnArea, float Size, float Angle, int RespawnTime, int SpawnQuantity, Color StartingColor, Vector2 Offset, int LifeTime)
        {
            this.Texture = Texture;
            velocity = Velocity;
            spawnArea = SpawnArea;
            size = Size;
            angle = Angle;
            respawnTime = RespawnTime;
            spawnQuantity = SpawnQuantity;
            color = StartingColor;
            offset = Offset;
            lifeTime = LifeTime;
        }
    }
}
