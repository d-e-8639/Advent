using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Advent.A2023
{
    public class Advent19
    {
        public static void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/2023/Advent19.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<List<string>> parts = SplitList.ByEmpty(lines);
            List<Workflow> ws = parts[0].Select(l => new Workflow(l)).ToList();
            Dictionary<string, Workflow> flows = ws.ToDictionary(w => w.Name);
            List<Rating> ratings = parts[1].Select(l => new Rating(l)).ToList();

            task1(flows, ratings);
        }

        private static void task1(Dictionary<string, Workflow> flows, List<Rating> ratings) {
            Console.WriteLine(
                "Sum: " +
                ratings.Where(r => flows["in"].Run(flows, r) == "A")
                    .Select(r => r.Values.Values.Sum()).Sum()
            );
        }

        private static void task2() {
        }

        public class Workflow {
            public string Name;
            public List<Assessment> Assessments;
            public string Default;

            public Workflow (string line) {
                string[] nameParts = line.Split('{');
                Name = nameParts[0];

                string[] workflowParts = nameParts[1].Trim('}').Split(',');
                Default = workflowParts.Last();
                Assessments = workflowParts.Take(workflowParts.Length - 1).Select(w => new Assessment(w)).ToList();
            }

            public string Run(Dictionary<string, Workflow> workflows, Rating r) {
                foreach (Assessment a in Assessments) {
                    string next = a.Assess(r);
                    if (next == "A" || next == "R") {
                        return next;
                    }
                    else if (next != null) {
                        return workflows[next].Run(workflows, r);
                    }
                }

                if (Default == "A" || Default == "R") {
                    return Default;
                }
                else {
                    return workflows[Default].Run(workflows, r);
                }
            }
        }

        public enum Operator{
            GT,
            LT
        }

        public class Assessment {
            public char Field;
            public Operator Op;
            public int Comparer;

            public string Destination;

            public Assessment(string w)
            {
                string[] parts = w.Split(':');
                Destination = parts[1];

                Field = parts[0][0];

                Op = parts[0][1] switch
                {
                    '<' => Operator.LT,
                    '>' => Operator.GT,
                    _ => throw new Exception()
                };

                Comparer = int.Parse(parts[0].Substring(2));
            }

            public string Assess(Rating r) {
                if (Op == Operator.GT) {
                    if (r.Values[Field] > Comparer) {
                        return Destination;
                    }
                    return null;
                }
                else if (Op == Operator.LT) {
                    if (r.Values[Field] < Comparer) {
                        return Destination;
                    }
                    return null;
                }
                throw new Exception();
            }
        }

        public class Rating {
            public Dictionary<char, int> Values;

            public Rating (string l) {
                string[] parts = l.Trim('{', '}').Split(',');

                Values = new Dictionary<char, int>();
                foreach (string part in parts) {
                    string[] bits = part.Split('=');
                    Values[bits[0][0]] = int.Parse(bits[1]);
                }
            }
        }

    }
}
