using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Culture
{
    public class Holiday
    {
        private string name;
        public string Name { get { return name; } }

        private string description;
        public string Description { get { return description; } }

        private int day;
        public int Day { get { return day; } }

        public Holiday(string Name, string Description, int Day)
        {
            name = Name;
            description = Description;
            day = Day;
        }
    }
    public class Aghtene
    {
        private string name;
        public string Name { get { return name; } }

        private string description;
        public string Description { get { return description; } }

        private int totalDays;
        public int TotalDays { get { return totalDays; } }

        private List<Holiday> holidays = new List<Holiday>();
        public List<Holiday> Holidays { get { return holidays; } }

        public Aghtene(string Name, string Description, int TotalDays)
        {
            name = Name;
            description = Description;
            totalDays = TotalDays;
        }

        public void AddHoliday(string Name, string Description, int Day)
        {
            if (string.IsNullOrEmpty(Name)) Name = "Holly's Day";
            if (string.IsNullOrEmpty(Description)) Description = "A day to celebrate the souls and their kind.";
            Day = (int)MathHelper.Clamp(Day, 1, totalDays);

            holidays.Add(new Holiday(Name, Description, Day));

            holidays.Sort((h1, h2) => h1.Day.CompareTo(h2.Day));
        }

        public Holiday CurrentHoliday(int currentDay)
        {
            for (int i = 0; i < holidays.Count; i++)
            {
                if (holidays[i].Day == currentDay)
                    return holidays[i];
            }

            return null;
        }
        public Holiday NextHoliday(int currentDay)
        {
            for (int i = 0; i < holidays.Count; i++)
            {
                if (holidays[i].Day >= currentDay)
                    return holidays[i];
            }

            return null;
        }
    }

    public class Calendar
    {
        private int hours;
        public int Hours { get { return hours; } set { hours = value; } }

        private int days;
        public int Days { get { return days; } set { days = value; VerifyDays(); } }

        private int aghtene;
        public int Aghtene { get { return aghtene; } set { aghtene = value; VerifyAghtenes(); } }

        private int passes;
        public int Passes { get { return passes; } set { passes = value; } }

        private void VerifyDays()
        {
            if (days > CurrentAghtene().TotalDays)
            {
                days = 1;
                Aghtene++;
            }
        }
        private void VerifyAghtenes()
        {
            if (aghtene > aghtenes.Count)
            {
                aghtene = 1;
                passes++;
            }
        }

        private List<Aghtene> aghtenes = new List<Aghtene>();

        public Calendar(int StartingPass, int StartingAghtene, int StartingDay)
        {
            LoadAghtenes();

            Passes = StartingPass;
            Aghtene = StartingAghtene;
            Days = StartingDay;
        }
        private void LoadAghtenes()
        {
            aghtenes.Add(new Aghtene("Renewings", "Snow and ice are still around, however it is usually warmer. In contrast to the last aghtene, this is said to be \"a new beginning for all good living persons and creatures.\"", 60));
            aghtenes.Add(new Aghtene("Calming & Growth", "Snow and ice starts to melt, and is gone by aghtene's half. All greenery is usually dry at this time after the melting of snow and ice, the wind is calm yet wistful, the temperature is not hot nor cold, and smells are fair. Planters should place seed in the ground before the end of this season. It is also said that you can see trees and other foliage stretching upward as if growing ever slowly", 90));
            aghtenes.Add(new Aghtene("Gusts", "The weather is very windy, and it blows from the south. Cloudy, and yet the rain comes in the next aghtene.", 90));
            aghtenes.Add(new Aghtene("Rainfall", "With great winds before brings clouds of rain, however rarely thunderous. Gatherer's should collect the planted seed, wild herbs and berries between this season and the next.", 120));
            aghtenes.Add(new Aghtene("Heat", "The sun is closest to the earth, and rarely cloudy during the day.", 60));
            aghtenes.Add(new Aghtene("Change", "The earth begins to cool, and the leaves change color and eventually fall. Logs for fires should be gathered at this time for the cold.", 60));
            aghtenes.Add(new Aghtene("Frost", "Gray clouds roll in from the north, and the air begins to chill. Frost covers the ground in the morning.", 60));
            aghtenes.Add(new Aghtene("Endings", "Ponds and rivers begin to freeze, snow starts to fall, and creatures of snow have the advantage. Those who do not seek shelter will surely die in this final pass. In old tales, the final aghtene symbolizes the death of the kind, the loyal, and the unprepared.", 180));

            for (int i = 0; i < aghtenes.Count; i++)
                daysInAPass += aghtenes[i].TotalDays;

            AddHolidays();
        }

        // [Methods] Holidays
        private void AddHolidays()
        {
            AddHoliday("Renewings", "New Pass", "A day to celebrate the beginning of the next pass.", 1);
        }
        private void AddHoliday(string aghteneName, string holidayName, string holidayDescription, int day)
        {
            Aghtene ag = GetAghtene(aghteneName);

            if (ag != null)
                ag.AddHoliday(holidayName, holidayDescription, day);
        }
        public Holiday NextHoliday()
        {
            return aghtenes[aghtene - 1].NextHoliday(days);
        }
        public Holiday CurrentHoliday()
        {
            return aghtenes[aghtene - 1].CurrentHoliday(days);
        }

        // [Methods] Aghtenes
        public Aghtene GetAghtene(string name)
        {
            for (int i = 0; i < aghtenes.Count; i++)
            {
                if (aghtenes[i].Name.ToUpper().Equals(name.ToUpper()))
                    return aghtenes[i];
            }

            return null;
        }
        public Aghtene CurrentAghtene()
        {
            return aghtenes[aghtene - 1];
        }

        private int daysInAPass;
        public int DaysInAPass
        {
            get
            {
                return daysInAPass;
            }
        }

        public int DaysThisPass()
        {
            int days = 0;

            for (int i = 0; i < aghtene - 1; i++)
                days += aghtenes[i].TotalDays;

            days += this.days;

            return days;
        }
    }
}
