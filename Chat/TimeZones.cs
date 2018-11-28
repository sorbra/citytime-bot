using System;
using System.Collections.Generic;
using System.Linq;

namespace mybot.Dialogflow
{
    class CityTimeZones
    {
        private List<CityTimeZone> _zones = new List<CityTimeZone>();

        public CityTimeZones()
        {
            _zones.Add(new CityTimeZone("Copenhagen","Europe/Copenhagen"));
            _zones.Add(new CityTimeZone("London","Europe/London"));
            _zones.Add(new CityTimeZone("New York","America/New_York"));
            _zones.Add(new CityTimeZone("Moscow","Europe/Moscow"));
            _zones.Add(new CityTimeZone("Bangalore","Asia/Calcutta"));
            _zones.Add(new CityTimeZone("Beijing","Asia/Shanghai"));
            _zones.Add(new CityTimeZone("Sydney","Australia/Sydney"));
        }

        public CityTimeZone GetCityTimeZone(string city) 
        {
            return _zones.FirstOrDefault(ctz => ctz.City.ToLower().Equals(city.ToLower()));
        }

        public TimeZoneInfo GetTimeZoneInfo(string city)
        {
            var zoneName = GetCityTimeZone(city)?.TimeZone;
            return string.IsNullOrEmpty(zoneName) ? null : TimeZoneInfo.FindSystemTimeZoneById(zoneName);
        }
    }

    class CityTimeZone
    {
        public CityTimeZone (string city, string timeZone)
        {
            City = city;
            TimeZone = timeZone;
        }
        public string City { get; set; }
        public string TimeZone { get; set; }
    }
}