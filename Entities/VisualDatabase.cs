using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pilgrimage_Of_Embers.Entities.Effects;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ParticleEngine.ParticleTypes;
using Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes;

namespace Pilgrimage_Of_Embers.Entities
{
    public class VisualDatabase
    {
        static List<BaseVisual> visuals = new List<BaseVisual>();
        public static List<BaseVisual> Visuals { get { return visuals; } }

        public static void LoadVisuals(ContentManager main, ContentManager map)
        {
            //visuals.Add(new BaseVisual(1, "TempOverlay", cm.Load<Texture2D>("Effects/regenerate_effect"), new Point(32, 32), 55, 4f, true));
            visuals.Add(new BaseVisual(1, "TempEmitter", new HiddenItem(), true));

            visuals.Add(new BaseVisual(500, "BloodSpew1", new BloodSpew(), true, 10, true));
            visuals.Add(new BaseVisual(510, "BloodDrip", new MetalSparks(), true, 100000));

            Load(map);
        }

        public static void Load(ContentManager mapContent)
        {
            for (int i = 0; i < visuals.Count; i++)
                visuals[i].Load(mapContent);
        }

        public static StringBuilder Output()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("    Visual Overlays (Total: " + visuals.Count + ")");
            builder.AppendLine("------------------------------------------------------------");

            visuals.OrderBy(x => x.ID);

            for (int i = 0; i < visuals.Count; i++)
            {
                builder.AppendLine(visuals[i].ID + " - " + visuals[i].Name + " [Type: " + visuals[i].Type.ToString() + "]");
            }

            return builder;
        }
    }
}
