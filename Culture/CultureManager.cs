using System;
using System.Collections.Generic;
using System.Text;

namespace Pilgrimage_Of_Embers.Culture
{
    public class CultureManager
    {
        private Calendar calender;

        public CultureManager()
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            calender = new Calendar(676, r.Next(1, 4), r.Next(1, 12));
        }

        // [Encapsulation] Calendar
        public int CALENDAR_Hours { get { return calender.Hours; } set { calender.Hours = value; } }
        public int CALENDAR_Days { get { return calender.Days; } }
        public int CALENDAR_Aghtenes { get { return calender.Aghtene; } }
        public int CALENDAR_Passes { get { return calender.Passes; } }
        public int CALENDAR_TotalDays { get { return calender.Days + (calender.Passes * calender.DaysInAPass); } }

        public int CALENDAR_PassDays { get { return calender.DaysInAPass; } }
        public int CALENDAR_AghteneDays(string aghtene) { return calender.GetAghtene(aghtene).TotalDays; }
        public void CALENDAR_IncrementDays() { calender.Days++; }

        public Aghtene CALENDAR_Aghtene(string aghtene) { return calender.GetAghtene(aghtene); }
        public Aghtene CALENDAR_CurrentAghtene() { return calender.CurrentAghtene(); }

        public Holiday CALENDAR_NextHoliday() { return calender.NextHoliday(); }
        public Holiday CALENDAR_CurrentHoliday() { return calender.CurrentHoliday(); }

        public string DateStamp()
        {
            return CALENDAR_Passes + "/" + CALENDAR_Aghtenes + "/" + calender.DaysThisPass() + "/" + CALENDAR_Hours;
        }
        public Helper_Classes.Point4 ParseDateStamp(string stamp)
        {
            string[] integers = stamp.Split('/');

            Helper_Classes.Point4 date = new Helper_Classes.Point4();

            date.A = int.Parse(integers[0]);
            date.B = int.Parse(integers[1]);
            date.C = int.Parse(integers[2]);
            date.D = int.Parse(integers[3]);

            return date;
        }
        public Helper_Classes.Point4 CurrentDateStamp()
        {
            return new Helper_Classes.Point4(CALENDAR_Passes, CALENDAR_Aghtenes, calender.DaysThisPass(), CALENDAR_Hours);
        }
        public int DistanceInDays(Helper_Classes.Point4 oldStamp, Helper_Classes.Point4 newStamp)
        {
            int passesDistance = newStamp.A - oldStamp.A;
            int daysDistance = newStamp.C - oldStamp.C;

            return (passesDistance * CALENDAR_PassDays) + daysDistance; //Total days
        }

        public StringBuilder SaveData(string tag)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(tag + Environment.NewLine);
            builder.Append("Current " + CALENDAR_Passes + " " + CALENDAR_Aghtenes + " " + CALENDAR_Days + " " + CALENDAR_Hours + Environment.NewLine);
            builder.AppendLine(tag.Replace("[", "[/"));

            return builder;
        }
        public void LoadData(List<string> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].ToUpper().StartsWith("CURRENT"))
                {
                    string[] words = data[i].Split(' ');

                    calender.Passes = int.Parse(words[1]);
                    calender.Aghtene = int.Parse(words[2]);
                    calender.Days = int.Parse(words[3]);
                    calender.Hours = int.Parse(words[4]);
                }
            }
        }
    }
}
