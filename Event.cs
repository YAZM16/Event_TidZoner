
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Event_TidZoner
{
    public class Event 
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeZoneInfo TimeZone { get; set; }

        public DateTime EndTime => StartTime.Add(Duration); // Computed property

        public override string ToString()
        {
            return $"{Name} starts at {StartTime:yyyy-MM-dd HH:mm} and lasts {Duration} (TimeZone: {TimeZone.Id})";
        }
    }
}
