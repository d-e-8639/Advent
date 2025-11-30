using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime expire = new DateTime(2025, 04, 30, 0, 5, 0, DateTimeKind.Utc);
            DateTime thirtyBefore = expire.AddDays(-30);
            TimeZoneInfo sydneyTZ = TimeZoneInfo.GetSystemTimeZones().Where(tz => tz.DisplayName.Contains("Sydney")).Single();
            DateTime sydneyRenew = TimeZoneInfo.ConvertTime(thirtyBefore, sydneyTZ);
            
            Console.WriteLine(sydneyRenew.ToString("o", CultureInfo.InvariantCulture));


            //A2024.Advent15.Do("C:/src/HelloWorld/2024/");
            //A2023.Advent19.Do();
        }
    }
}
// https://github.com/d-e-8639/Advent/