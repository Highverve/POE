using System;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.CombatEngine.Types;
using Pilgrimage_Of_Embers.CombatEngine;

namespace Pilgrimage_Of_Embers.ScreenEngine.Spellbook.Types
{
    public class BaseSpell
    {
        private int id;
        private string name, description;
        private Texture2D icon;

        public int ID { get { return id; } }
        public string Name { get { return name; } }
        public string Description { get { return description; } }
        public Texture2D Icon { get { return icon; } }

        public bool IsNew { get; set; }
        public bool IsSelected { get; set; }

        public Point gridLocation { get; set; }
        public Rectangle spellRect { get; set; }

        public enum TabType { Destructive, Cursings, Blessings, Invocations, Sundries }
        private TabType type;

        public TabType Type { get { return type; } }

        private int charges, maxCharges;
        public int Charges { get { return charges; } set { charges = value; } }
        public int MaxCharges { get { return charges; } }
        public void Recharge() { charges = maxCharges; }

        protected BaseEntity currentEntity;
        protected TileMap tileMap;

        protected string buttonOneText = "Equip Spell", buttonTwoText = "Unequip Spell", buttonThreeText, buttonFourText;
        public string ButtonOneText { get { return buttonOneText; } }
        public string ButtonTwoText { get { return buttonTwoText; } }
        public string ButtonThreeText { get { return buttonThreeText; } }
        public string ButtonFourText { get { return buttonFourText; } }

        public BaseSpell(int ID, string Name, string Description, Texture2D Icon, TabType Type, int MaxCharges, Action<BaseSpell, BaseCombat> Basic, Action<BaseSpell, BaseCombat> Power, Action<BaseSpell, BaseCombat> Jump,
                         Action<BaseSpell, BaseCombat> AfterRoll, Action<BaseSpell, BaseCombat> Sneak, Action<BaseSpell, BaseCombat> Sprint, Action<BaseSpell, BaseCombat> OffhandEmpty, Action<BaseSpell, BaseCombat> DualHand, Action<BaseSpell, BaseCombat> BehindShield)
        {
            id = ID;

            name = Name;
            description = Description;

            icon = Icon;
            type = Type;

            maxCharges = MaxCharges;
            Recharge();

            BasicAction = Basic;
            PowerAction = Power;
            JumpAction = Jump;
            AfterRollAction = AfterRoll;
            SneakAction = Sneak;
            SprintAction = Sprint;
            OneHandAction = OffhandEmpty;
            DualHandAction = DualHand;
            BehindShieldAction = BehindShield;

            IsNew = true;
        }

        public void UpdateValues(Vector2 gridOffset, int itemSize, int scrollPosition)
        {
            spellRect = new Rectangle((gridLocation.X * itemSize) + (int)gridOffset.X,
                                      (gridLocation.Y * itemSize) + scrollPosition + (int)gridOffset.Y,
                                      itemSize,
                                      itemSize);
        }

        public virtual void ButtonOne()
        {

        }
        public virtual void ButtonTwo()
        {

        }
        public virtual void ButtonThree()
        {

        }
        public virtual void ButtonFour()
        {

        }

        public void CastSpell(BaseCombat caster, CombatEngine.CombatMove action)
        {
            if (charges > 0)
            {
                switch(action)
                {
                    case CombatMove.Basic: if (BasicAction != null) BasicAction.Invoke(this, caster); break;
                    case CombatMove.Power: if (PowerAction != null) PowerAction.Invoke(this, caster); break;
                    case CombatMove.Jump:  if (JumpAction != null) JumpAction.Invoke(this, caster); break;
                    case CombatMove.Roll: if (AfterRollAction != null) AfterRollAction.Invoke(this, caster); break;
                    case CombatMove.Sneak: if (SneakAction != null) SneakAction.Invoke(this, caster); break;
                    case CombatMove.Sprint: if (SprintAction != null) SprintAction.Invoke(this, caster); break;
                    case CombatMove.OffhandEmpty: if (OneHandAction != null) OneHandAction.Invoke(this, caster); break;
                    case CombatMove.BehindShield: if (BehindShieldAction != null) BehindShieldAction.Invoke(this, caster); break;
                }
            }
        }

        public Action<BaseSpell, BaseCombat> BasicAction, PowerAction, JumpAction, AfterRollAction,
                                                     SneakAction, SprintAction, OneHandAction, DualHandAction,
                                                     BehindShieldAction;

        public override bool Equals(object obj)
        {
            var temp = (BaseSpell)obj;

            if (temp == null)
                return false;

            return this.id == temp.id;
        }

        public override int GetHashCode()
        {
            return id;
        }

        public virtual BaseSpell Copy()
        {
            BaseSpell spell = (BaseSpell)MemberwiseClone();

            return spell;
        }

        protected const string space = " ";
        public StringBuilder SaveData()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Spell " + ID.ToString() + " " + charges + " " + IsNew.ToString() + " //" + name);

            return builder;
        }
        public void LoadData(string data)
        {
            string[] words = data.Split(' ');

            charges = int.Parse(words[2]);
            IsNew = bool.Parse(words[3]);
        }
    }
}
