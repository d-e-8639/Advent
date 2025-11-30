using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace HelloWorld.A2023
{
    public static class SplitList
    {
        public static List<List<string>> ByEmpty(IEnumerable<string> input) {
            List<List<string>> parts = new List<List<string>>();
            List<string> latest = new List<string>();
            parts.Add(latest);

            foreach (string s in input) {
                if (s == "") {
                    latest = new List<string>();
                    parts.Add(latest);
                    continue;
                }
                latest.Add(s);
            }

            return parts;
        }

    }
}