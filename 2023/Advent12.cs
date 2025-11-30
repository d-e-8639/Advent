using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Data;

namespace HelloWorld
{
    public class Advent12
    {
        public static void Do()
        {
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent12.txt"))
            {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<Springs> springs = new List<Springs>();
            foreach (string line in lines){
                if (line == ""){
                    continue;
                }
                string[] parts = line.Split(' ');
                springs.Add(new Springs(parts[0], parts[1]));
            }

            task1(springs);
            task2(springs);
        }

        private static void task1(List<Springs> springs)
        {
            long validP = 0;
            foreach (Springs s in springs) {
                validP += s.Deep();
            }
            Console.WriteLine("Total perms:" + validP);
        }

        private static void task2(List<Springs> springs)
        {
            Stopwatch st = new Stopwatch();
            int count=0;
            long validP = 0;
            st.Start();
            foreach (Springs s in springs) {
                Springs unfolded = s.Unfold();
                Console.WriteLine(unfolded.ToString());
                validP += unfolded.MemDeep();
                Console.WriteLine("Processed line " + count++ + " Elapsed: " + st.ElapsedMilliseconds);
            }
            Console.WriteLine("Total perms:" + validP);
        }

        private class Springs
        {
            public string Positions;
            public List<int> Groups;

            public Springs(string positions, string groups){
                Positions = positions;
                Groups = groups.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)).ToList();
            }

            public Springs(string positions, List<int> groups){
                Positions = positions;
                Groups = groups;
            }

            public Springs Unfold() {
                return new Springs(string.Join("?", Enumerable.Repeat(Positions, 5)), Enumerable.Repeat(Groups, 5).SelectMany(x => x).ToList());
            }

            public long Deep() {
                long permuations=0;
                deep(0, ref permuations, 0);
                return permuations;
            }

            private bool canConsumeGroup(int depth, int groupLen) {
                int i;
                for (i=depth ; i < (depth + groupLen); i++){
                    if (i >= Positions.Length){
                        return false;
                    }
                    if (Positions[i] == '.'){
                        return false;
                    }
                }

                if (i >= Positions.Length) {
                    return true;
                }
                else if (Positions[i] != '#') {
                    return true;
                }
                return false;
            }

            private void deep(int depth, ref long validPermutations, int groupIndex) {
                if (depth >= Positions.Length) {
                    if (groupIndex == Groups.Count) {
                        validPermutations++;
                    }
                    return;
                }
                
                if (groupIndex == Groups.Count) {
                    for (int i=depth; i < Positions.Length; i++) {
                        // Consumed too many groups
                        if (Positions[i] == '#') {
                            return;
                        }

                    }
                }

                if (Positions[depth] == '#') {
                    if (groupIndex >= Groups.Count) {
                        // Generated more groups than there are
                        return;
                    }

                    if (canConsumeGroup(depth, Groups[groupIndex])) {
                        deep(depth + Groups[groupIndex] + 1, ref validPermutations, groupIndex + 1);
                    }
                    else {
                        // Bad path, group size *must* match
                        return;
                    }
                }
                else {

                    if (Positions[depth] == '?') {
                        // Try consuming the group
                        if ((groupIndex < Groups.Count) &&(canConsumeGroup(depth, Groups[groupIndex]))) {
                            deep(depth + Groups[groupIndex] + 1, ref validPermutations, groupIndex + 1);
                        }
                    }

                    // Don't consume the group
                    deep(depth + 1, ref validPermutations, groupIndex);
                }

            }


            public long MemDeep() {
                Dictionary<Tuple<int, int>, long> memo = new Dictionary<Tuple<int, int>, long>();
                long perms = memdeep(0, 0, memo);
                Console.WriteLine("cache size: " + memo.Count);
                return perms;
            }

            private long memdeep(int depth, int groupIndex, Dictionary<Tuple<int, int>, long> memo) {

                if (memo.TryGetValue(new Tuple<int, int>(depth, groupIndex), out long cache)){
                    return cache;
                }

                if (depth >= Positions.Length) {
                    if (groupIndex == Groups.Count) {
                        return 1;
                    }
                    return 0;
                }
                
                if (groupIndex == Groups.Count) {
                    for (int i=depth; i < Positions.Length; i++) {
                        // Consumed too many groups
                        if (Positions[i] == '#') {
                            return 0;
                        }

                    }
                }

                long perms = 0;

                if (Positions[depth] == '#') {
                    if (groupIndex >= Groups.Count) {
                        // Generated more groups than there are available
                        return 0;
                    }

                    if (canConsumeGroup(depth, Groups[groupIndex])) {
                        perms += memdeep(depth + Groups[groupIndex] + 1, groupIndex + 1, memo);
                    }
                    else {
                        // Bad path, group size *must* match
                        return 0;
                    }
                }
                else {
                    if (Positions[depth] == '?') {
                        // Try consuming the group
                        if ((groupIndex < Groups.Count) &&(canConsumeGroup(depth, Groups[groupIndex]))) {
                            perms += memdeep(depth + Groups[groupIndex] + 1, groupIndex + 1, memo);
                        }
                    }

                    // Don't consume the group
                    perms += memdeep(depth + 1, groupIndex, memo);
                }

                memo[new Tuple<int, int>(depth, groupIndex)] = perms;
                return perms;

            }

            public override string ToString()
            {
                return Positions + " " + string.Join(',', Groups);
            }
        }

    }
}
