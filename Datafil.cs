using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Event_TidZoner
{
    public static class DataFil
    {
        private const string FilePath = @"C:\Users\user\source\repos\Event_TidZoner\Eventsinfo.json";
       

        // Save events to a JSON file
        public static void SparaData(List<Event> events)
        {
            try
            {
                string json = JsonSerializer.Serialize(events, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
                Console.WriteLine("Events saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        // Load events from a JSON file
        public static List<Event> LaddaData()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
                    return JsonSerializer.Deserialize<List<Event>>(json);
                }
                else
                {
                    Console.WriteLine("No data file found. Starting fresh.");
                    return new List<Event>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
                return new List<Event>();
            }
        }
    }
}
