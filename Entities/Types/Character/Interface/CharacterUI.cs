using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Entities.NPC;
using Pilgrimage_Of_Embers.Entities.Types.NPE.NPC;
using Pilgrimage_Of_Embers.ScreenEngine;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Interface
{
    public class CharacterUI
    {
        Controls controls = new Controls();

        ScreenManager screens;
        BaseEntity entity, controlledEntity;
        Camera camera;

        InteractionUI interactUI = new InteractionUI();
        ConversationManager conversations;

        public CharacterUI(string name)
        {
            conversations = new ConversationManager(name);
        }
        public void SetReferences(ScreenManager screens, CharacterEntity entity, Camera camera)
        {
            this.screens = screens;
            this.entity = entity;
            this.camera = camera;

            interactUI.SetReferences(screens, entity, camera, conversations);
            conversations.SetReferences(screens, entity);
        }
        public void SetControlledEntity(BaseEntity entity)
        {
            controlledEntity = entity;

            interactUI.SetControlledEntity(entity);
            conversations.SetControlledEntity(entity);
        }

        public void Load(ContentManager cm)
        {
            interactUI.Load(cm);
            conversations.Load(cm);
        }

        private bool isLerpSet = false;
        public void Update(GameTime gt, Vector2 position, bool canInteract)
        {
            float distanceToPlayer = Vector2.Distance(controlledEntity.Position, entity.Position);

            if (canInteract == true)
            {
                if (distanceToPlayer <= (entity.EntityCircle.radius * 5f) && controlledEntity.IsPlayerControlled == true)
                {
                    if (conversations.IsActive == false && interactUI.IsActive == false)
                        screens.ACTIVATEBOX_SetLines("Interact", entity.Name);

                    if (controlledEntity.IsActivatingObject == true && conversations.IsActive == false && controlledEntity != entity) //Prevent the entity from opening it's own UI (lol).
                        InvertInteractionUI();
                }

                if (interactUI.IsActive == true || conversations.IsActive == true)
                {
                    if (distanceToPlayer >= (entity.EntityCircle.radius * 7f))
                        CloseAllUI();
                }
            }

            interactUI.Update(gt, position);
            conversations.Update(gt, position);

            if (distanceToPlayer <= (entity.EntityCircle.radius * 20f))
            {
                if (interactUI.IsActive == true)
                {
                    if (isLerpSet == false)
                    {
                        camera.SetCameraState(Camera.CameraState.Cinematic);
                        camera.LookAt(entity.Position);
                        camera.DelaySpeed = 3f;

                        isLerpSet = true;
                    }
                }
                else if (interactUI.IsActive == false)
                {
                    if (isLerpSet == true)
                    {
                        camera.SetCameraState(Camera.CameraState.Current);
                        camera.DelaySpeed = 4f;

                        isLerpSet = false;
                    }
                }
            }
        }

        public void DrawShadow(SpriteBatch sb, float depth)
        {
            interactUI.DrawShadow(sb, depth);
        }
        public void Draw(SpriteBatch sb)
        {
            interactUI.Draw(sb);
            conversations.Draw(sb);
        }

        public void OpenInteractionUI() { interactUI.IsActive = true; }
        public void InvertInteractionUI()
        {
            interactUI.IsActive = !interactUI.IsActive;

            if (interactUI.IsActive == false)
                entity.CAPTION_SendImmediate(entity.CHAT_Retrieve("InteractClose"));
        }
        public void CloseAllUI()
        {
            conversations.IsActive = false;
            interactUI.IsActive = false;

            screens.BARTERING_End();
            screens.PURCHASING_End();

            conversations.Reset();
            entity.CAPTION_Reset();

            entity.CAPTION_SendImmediate(entity.CHAT_Retrieve("RudeWalkAway"));
        }

        public bool IsClickingUI()
        {
            return interactUI.IsClickingUI() || conversations.IsClickingUI();
        }
        public bool IsUIOpen()
        {
            return interactUI.IsActive || conversations.IsActive;
        }
        public void ForceCloseAll()
        {
            interactUI.IsActive = false;
            conversations.IsActive = false;
        }

        public StringBuilder SaveData(string tag)
        {
            //StringBuilder builder = new StringBuilder();
            //builder.AppendLine(conversations.SaveData(tag).ToString());

            return conversations.SaveData(tag);
        }
        public void LoadData(List<string> data)
        {
            conversations.LoadData(data);
        }
    }
}
