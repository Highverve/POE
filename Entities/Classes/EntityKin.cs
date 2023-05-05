namespace Pilgrimage_Of_Embers.Entities
{
    /// <summary>
    /// E.G, entity is of type "Creature", and is a "Squirrel".
    /// </summary>
    public class EntityKin
    {
        private string type, kin;

        public string Type { get { return type; } } //The subtype of the type of entity. E.G, a creature has squirrels, horses, dogs, ...
        public string Kin { get { return kin; } } //To help narrow down further if necessary.

        public EntityKin(string Type) : this(Type, "") { }
        public EntityKin(string Type, string Kin)
        {
            type = Type;
            kin = Kin;

            if (string.IsNullOrEmpty(type))
            {
                //Complain to error window

            }
        }

        public bool CompareType(string type) { return (this.type.ToUpper().Equals(type.ToUpper())); }
        public bool CompareKin(string kin)
        {
            if (string.IsNullOrEmpty(kin) || string.IsNullOrEmpty(this.kin))
                return false;

            return (kin.ToUpper().Equals(kin.ToUpper()));
        }

        public override string ToString()
        {
            string temp = "";

            if (!string.IsNullOrEmpty(type))
                temp += type + " of ";

            if (!string.IsNullOrEmpty(kin))
                temp += kin;
            else
                temp += "foreigner";

            return temp;
        }

        public EntityKin Copy()
        {
            EntityKin copy = (EntityKin)this.MemberwiseClone();

            return copy;
        }
    }
}
