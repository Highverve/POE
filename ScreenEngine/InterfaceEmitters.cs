using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ParticleEngine;
using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.ScreenEngine
{
    public class InterfaceEmitters
    {
        private List<BaseEmitter> emitters = new List<BaseEmitter>();
        
        public void AddEmitterReference(BaseEmitter emitter)
        {
            if (!emitters.Contains(emitter))
            {
                emitter.Load(localContent);
                emitters.Add(emitter);
            }
        }

        private ContentManager localContent;
        public void Load(ContentManager cm)
        {
            localContent = cm;
        }

        public void Update(GameTime gt)
        {
            for (int i = 0; i < emitters.Count; i++)
                emitters[i].Update(gt);
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < emitters.Count; i++)
                emitters[i].Draw(sb);
        }
    }
}
