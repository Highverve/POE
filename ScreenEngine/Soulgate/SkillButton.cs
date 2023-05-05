using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Skills;

namespace Pilgrimage_Of_Embers.ScreenEngine.Soulgate
{
    public class SkillButton
    {
        int order;
        private string name;
        private Texture2D icon;

        public int Order { get { return order; } }
        public string Name { get { return name; } }
        public Texture2D Icon { get { return icon; } }

        public Rectangle RowRect { get; set; }
        public Rectangle ButtonRect { get; set; }
        public bool IsHover { get; set; }
        public bool IsRowHover { get; set; }

        private Action<Skills.Skillset> onFortifyClick;
        public Action<Skills.Skillset> OnFortifyClick { get { return onFortifyClick; } }

        private BaseSkill skill;
        public BaseSkill Skill { get { return skill; } }

        public SkillButton(int Order, string Name, Texture2D Icon, Action<Skills.Skillset> OnFortifyClick, BaseSkill Skill)
        {
            order = Order;
            name = Name;
            icon = Icon;

            skill = Skill;

            onFortifyClick = OnFortifyClick;
        }

        public void SetReferences(BaseSkill skill)
        {
            this.skill = skill;
        }
    }
}
