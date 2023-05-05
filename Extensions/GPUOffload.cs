using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pilgrimage_Of_Embers.Extensions
{
    public class GPUOffload
    {
        private Effect effect;

        public GPUOffload(Effect Effect)
        {
            effect = Effect;
        }

        public void Update(GameTime gt)
        {
            for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
                effect.CurrentTechnique.Passes[i].Apply();
        }

        public int RetrieveInteger(string name)
        {
            return effect.Parameters[name].GetValueInt32();
        }
        public float RetrieveFloat(string name)
        {
            return effect.Parameters[name].GetValueSingle();
        }
        public bool RetrieveBoolean(string name)
        {
            return effect.Parameters[name].GetValueBoolean();
        }
        public Vector2 RetrieveVector2(string name)
        {
            return effect.Parameters[name].GetValueVector2();
        }
        public Vector3 RetrieveVector3(string name)
        {
            return effect.Parameters[name].GetValueVector3();
        }
        public Vector4 RetrieveVector4(string name)
        {
            return effect.Parameters[name].GetValueVector4();
        }
        public string RetrieveString(string name)
        {
            return effect.Parameters[name].GetValueString();
        }

        public void SetValue(string name, int value)
        {
            effect.Parameters[name].SetValue(value);
        }
        public void SetValue(string name, float value)
        {
            effect.Parameters[name].SetValue(value);
        }
        public void SetValue(string name, bool value)
        {
            effect.Parameters[name].SetValue(value);
        }
        public void SetValue(string name, Vector2 value)
        {
            effect.Parameters[name].SetValue(value);
        }
        public void SetValue(string name, Vector3 value)
        {
            effect.Parameters[name].SetValue(value);
        }
        public void SetValue(string name, Vector4 value)
        {
            effect.Parameters[name].SetValue(value);
        }
    }
}
