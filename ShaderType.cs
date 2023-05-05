using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Pilgrimage_Of_Embers
{
    public class ShaderType
    {
        public enum State { Started, End }

        private string name;
        private int order;
        private string effectDirectory;
        private bool isActive = false;
        private State currentState = State.End;
        private Effect effect;

        public string Name { get { return name; } }
        public int Order { get { return order; } }
        public bool IsActive { get { return isActive; } set { isActive = value; } }
        public State CurrentState { get { return currentState; } set { currentState = value; } }
        public Effect Effect { get { return effect; } set { effect = value; } }
        public RenderTarget2D renderTarget;

        public ShaderType(string Name, int Order, string EffectDirectory, Action<Effect> ApplyShaderValues)
        {
            name = Name;
            order = Order;

            effectDirectory = EffectDirectory;

            continueShading = ApplyShaderValues;
        }
        public void Load(ContentManager cm)
        {
            effect = cm.Load<Effect>(effectDirectory);
        }
        public void AssignRenderTarget(GraphicsDevice graphicsDevice, int x, int y, SurfaceFormat format)
        {
            renderTarget = new RenderTarget2D(graphicsDevice, x, y, true, format, DepthFormat.None);
        }

        Action<Effect> continueShading;
        /// <summary>
        /// What to do continuously
        /// </summary>
        public Action<Effect> ContinueShading { get { return continueShading; } }

    }
}
