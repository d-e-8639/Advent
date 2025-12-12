using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.IO;

namespace Advent
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentDir = Directory.GetCurrentDirectory();
            //currentDir = currentDir.Substring(0, currentDir.LastIndexOf("bin\\"));
            //A2024.Advent16.Do(currentDir + "2024/");
            //A2023.Advent19.Do();
            Y2025.Advent8.Do(currentDir+"/Y2025/");
        }
    }
}
// https://github.com/d-e-8639/Advent/