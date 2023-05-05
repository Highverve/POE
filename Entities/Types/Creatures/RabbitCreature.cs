using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.AI.Agents;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Entities.Factions;
using Pilgrimage_Of_Embers.Skills;

namespace Pilgrimage_Of_Embers.Entities.Types.Creatures
{
    public class RabbitCreature : BaseEntity
    {
        public RabbitCreature(int ID, string Name, AnimationState Animation, Skillset Skills, ObjectAttributes Attributes,
                              EntityLoot Loot, EntityStorage Storage, BaseFaction Faction, ObjectSenses Senses, EntityKin Kin,
                              float CircleRadius, float CenterOffset, float DepthOffset, float ShadowOffset, float InfoHeight,
                              BaseAgent AgentAI) : base(ID, Name, Animation, Skills, Attributes, Loot, Storage, Faction, Senses,
                                                        Kin, CircleRadius, CenterOffset, DepthOffset, ShadowOffset, InfoHeight, AgentAI)
        {
        }

        public override void Load(ContentManager cm)
        {
            base.Load(cm);
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}
