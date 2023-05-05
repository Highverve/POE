using System.Collections.Generic;
using Pilgrimage_Of_Embers.Entities.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.ScreenEngine;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.ParticleEngine;
using Pilgrimage_Of_Embers.Culture;
using Pilgrimage_Of_Embers.TileEngine;
using System.Linq;

namespace Pilgrimage_Of_Embers.Entities
{
    public class EntityVisual //Will be used to display effects around an entity (I.E., fire swoosh, teleport, etc).
    {
        List<BaseVisual> visuals = new List<BaseVisual>();

        protected TileMap map;
        protected Camera camera;
        protected ScreenManager screens;
        protected PlayerEntity player;
        protected WeatherManager weather;
        protected CultureManager culture;
        protected BaseEntity controlledEntity;
        protected List<BaseEntity> entities;

        public EntityVisual() { }

        public void SetReferences(TileMap map, Camera camera, ScreenManager screens, PlayerEntity player, WeatherManager weather, CultureManager culture, BaseEntity controlledEntity, List<BaseEntity> entities)
        {
            this.map = map;
            this.camera = camera;
            this.screens = screens;
            this.player = player;
            this.weather = weather;
            this.culture = culture;
            this.controlledEntity = controlledEntity;
            this.entities = entities;
        }

        public void AddVisual(int ID, BaseEntity entity)
        {
            for (int i = 0; i < VisualDatabase.Visuals.Count; i++)
            {
                if (VisualDatabase.Visuals[i].ID == ID)
                {
                    if (VisualDatabase.Visuals[i].IsMultiple == false)
                    {
                        if (visuals.Count(v => v.ID == ID) == 0)
                        {
                            visuals.Add(VisualDatabase.Visuals[i].Copy(entity, map, camera, screens, player, weather, culture, controlledEntity, entities));
                        }
                    }
                    else
                    {
                        visuals.Add(VisualDatabase.Visuals[i].Copy(entity, map, camera, screens, player, weather, culture, controlledEntity, entities));
                    }
                }
            }
        }
        public void StopVisual(int ID)
        {
            for (int i = 0; i < visuals.Count; i++)
            {
                if (visuals[i].ID == ID)
                    visuals[i].BeginStop();
            }
        }
        public void StopAllVisuals()
        {
            for (int i = 0; i < VisualDatabase.Visuals.Count; i++)
                visuals[i].BeginStop();
        }

        public void Update(GameTime gt)
        {
            for (int i = 0; i < visuals.Count; i++)
            {
                visuals[i].Update(gt);

                if (visuals[i].IsActive == false)
                    visuals.RemoveAt(i); //If effect is no longer active, remove it.
            }
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < visuals.Count; i++)
            {
                visuals[i].Draw(sb);
            }
        }

        public void Clear()
        {
            visuals.Clear();
        }
    }
}
