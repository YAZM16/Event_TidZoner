using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event_TidZoner
{
   class Program
   {
        static void Main()
        {
            Console.WriteLine("Welcome to the Event Planner!");
            var events = DataFil.LaddaData() ?? new List<Event>();

            bool fortsätt = true;
            int choice = 0;
            while (fortsätt)
            {
                Console.Clear();
                ShowMeny(choice);

                ConsoleKeyInfo knapp = Console.ReadKey(true);
                switch (knapp.Key)
                {
                    case ConsoleKey.UpArrow:
                        choice = choice == 0 ? 7 : choice - 1; // Gå upp, wrap runt
                        break;

                    case ConsoleKey.DownArrow:
                        choice = (choice + 1) % 7; // Gå ner, wrap runt
                        break;

                    case ConsoleKey.Enter:
                        Console.Clear();
                        switch (choice)
                        {
                            case 0:
                                events.Add(CreateEvent());
                                break;
                            case 1:
                                ShowEventInTimeZones(events);
                                break;
                            case 2:
                                CheckForConflicts(events);
                                break;
                            case 3:
                                CountdownToEvent(events);
                                break;
                            case 4:
                                CreateRecurringEvent();
                                break;
                            case 5:
                                Console.WriteLine("Goodbye!");
                                DataFil.SparaData(events);
                                Console.WriteLine("Data saved.");
                                fortsätt = false;
                                break;
                            default:
                                Console.WriteLine("Invalid option, try again.");
                                break;
                        }
                        break;
                    
                }
            }
        }  

        static void ShowMeny(int choice)
        {
            string[] menychoice =
            {
                "Create an event",
                "Show event in multiple time zones",
                "Check for conflicts",
                "Countdown to event",
                "Create recurring event",
                "Exit and save data",
            };
            Console.WriteLine("--- Welcome to the Event Planner!---");
            for (int i = 0; i < menychoice.Length; i++)
            {
                if (i == choice)
                {
                    Console.WriteLine($"--> {menychoice[i]}");
                }
                else
                {
                    Console.WriteLine($"    {menychoice[i]}");
                }
            }

        }



        static Event CreateEvent()
        {
            Console.Write("Enter event name: ");
            string name = Console.ReadLine();

            Console.Write("Enter event start date and time (yyyy-MM-dd HH:mm): ");
            DateTime startTime = DateTime.ParseExact(Console.ReadLine(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

            Console.Write("Enter duration (e.g., 72:00 for 72 hours and 0 minutes): ");
            string[] durationParts = Console.ReadLine().Split(':');
            if (durationParts.Length != 2 ||
                !int.TryParse(durationParts[0], out int hours) ||
                !int.TryParse(durationParts[1], out int minutes))
            {
                Console.WriteLine("Invalid duration format. Please use HH:mm format.");
                return null;
            }
            TimeSpan duration = new TimeSpan(hours, minutes, 0);

            Console.Write("Enter time zone (e.g., Eastern Standard Time or UTC+05:30): ");
            string timeZoneInput = Console.ReadLine();
            TimeZoneInfo timeZone = ParseTimeZone(timeZoneInput);

            var newEvent = new Event
            {
                Name = name,
                StartTime = TimeZoneInfo.ConvertTimeToUtc(startTime, timeZone),
                Duration = duration,
                TimeZone = timeZone
            };

            Console.WriteLine($"Event created: {newEvent}");
            return newEvent;
        }

        static void ShowEventInTimeZones(List<Event> events)
            {
                if (events.Count == 0)
                {
                    Console.WriteLine("No events available.");
                    return;
                }

                Console.WriteLine("Select an event to view:");
                for (int i = 0; i < events.Count; i++)
                    Console.WriteLine($"{i + 1}. {events[i].Name}");

                if (!int.TryParse(Console.ReadLine(), out int eventIndex) || eventIndex < 1 || eventIndex > events.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    return;
                }

                var selectedEvent = events[eventIndex - 1];

                Console.WriteLine("Enter three time zones to view the event (e.g., UTC, Pacific Standard Time):");
                for (int i = 0; i < 3; i++)
                {
                    Console.Write($"Time Zone {i + 1}: ");
                    string timeZoneInput = Console.ReadLine();
                    TimeZoneInfo timeZone = ParseTimeZone(timeZoneInput);

                    var localTime = TimeZoneInfo.ConvertTimeFromUtc(selectedEvent.StartTime, timeZone);
                    Console.WriteLine($"Event in {timeZone.Id}: {localTime}");
                }
            }


            static void CheckForConflicts(List<Event> events)
            {
               Console.WriteLine("Create a new event to check for conflicts:");
               var newEvent = CreateEvent();

                foreach (var existingEvent in events)
                {
                   if (newEvent.StartTime < existingEvent.EndTime && newEvent.EndTime > existingEvent.StartTime)
                   {
                      Console.WriteLine($"Conflict detected with: {existingEvent}");
                      return;
                   }
                }
                      Console.WriteLine("No conflicts found.");
            }

            static void CountdownToEvent(List<Event> events)
            {
                if (events.Count == 0)
                {
                     Console.WriteLine("No events available.");
                     return;
                }

                     Console.WriteLine("Select an event for countdown:");
                for (int i = 0; i < events.Count; i++)
                    Console.WriteLine($"{i + 1}. {events[i].Name}");

                  int eventIndex = int.Parse(Console.ReadLine()) - 1;
                  var selectedEvent = events[eventIndex];

                  TimeSpan timeLeft = selectedEvent.StartTime - DateTime.UtcNow;
                  if (timeLeft.TotalSeconds <= 0)
                     Console.WriteLine("The event has already started or passed.");
                  else
                     Console.WriteLine($"Time left to the event: {timeLeft.Days} days, {timeLeft.Hours} hours, {timeLeft.Minutes} minutes, {timeLeft.Seconds} seconds.");
            }

            static void CreateRecurringEvent()
            {
               Console.Write("Enter event name: ");
               string name = Console.ReadLine();

               Console.Write("Enter the day of the week (e.g., Monday): ");
               DayOfWeek dayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), Console.ReadLine(), true);
 
               Console.Write("Enter the time of the day (HH:mm): ");
               TimeSpan timeOfDay = TimeSpan.Parse(Console.ReadLine());

               Console.WriteLine("The next 5 occurrences are:");
               DateTime today = DateTime.Today;
               for (int i = 0, occurrences = 0; occurrences < 5; i++)
               {
                   var date = today.AddDays(i);
                  if (date.DayOfWeek == dayOfWeek)
                  {
                     Console.WriteLine($"{date + timeOfDay:yyyy-MM-dd HH:mm}");
                     occurrences++;
                  }
               }
            }

            static TimeZoneInfo ParseTimeZone(string timeZoneInput)
            {
                try
                {
                    if (timeZoneInput.StartsWith("UTC", StringComparison.OrdinalIgnoreCase))
                    {
                        string offset = timeZoneInput.Replace("UTC", "").Trim();
                        if (TimeSpan.TryParse(offset, out TimeSpan utcOffset))
                        {
                            return TimeZoneInfo.CreateCustomTimeZone(timeZoneInput, utcOffset, timeZoneInput, timeZoneInput);
                        }
                    }
                    return TimeZoneInfo.FindSystemTimeZoneById(timeZoneInput);
                }
                catch (TimeZoneNotFoundException)
                {
                    Console.WriteLine($"Time zone '{timeZoneInput}' not found. Please try again.");
                }
                catch (InvalidTimeZoneException)
                {
                    Console.WriteLine($"Invalid time zone '{timeZoneInput}'. Please use a valid identifier.");
                }

                Console.WriteLine("Falling back to UTC time zone.");
                return TimeZoneInfo.Utc;
            

            }
        
   }
}
