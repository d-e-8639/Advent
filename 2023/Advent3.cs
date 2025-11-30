using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Advent
{
    public class Advent3
    {
        public void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent3.txt")) {
                file = sr.ReadToEnd();
            }

            string[] schematic = file.Split(new string[]{"\r", "\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            task1(schematic);
            task2(schematic);
        }

        void task1(string[] schematic) {
            HashSet<string> alreadyFound = new HashSet<string>();
            List<int> partNumbers = new List<int>();

            for (int y=0; y < schematic.Length; y++){
                for (int x=0; x < schematic[y].Length; x++){
                    if (!Char.IsNumber(schematic[y][x]) && (schematic[y][x] != '.')){
                        checkPartNumbers(schematic, y, x, alreadyFound, partNumbers);
                    }
                }
            }

            Console.WriteLine("Part number sum: " + partNumbers.Sum());
        }

        void task2(string[] schematic) {
            long gearRatioTotal=0;

            for (int y=0; y < schematic.Length; y++){
                for (int x=0; x < schematic[y].Length; x++){
                    if (schematic[y][x] == '*'){
                        HashSet<string> alreadyFound = new HashSet<string>();
                        List<int> partNumbers = new List<int>();

                        checkPartNumbers(schematic, y, x, alreadyFound, partNumbers);

                        if (partNumbers.Count == 2) {
                            gearRatioTotal += partNumbers[0] * partNumbers[1];
                        }
                    }
                }
            }

            Console.WriteLine("gear ratio total: " + gearRatioTotal);
        }

        private void checkPartNumbers(string[] schematic, int y, int x, HashSet<string> alreadyFound, List<int> partNumbers) {

            if (schematic[y][x] == '*'){

            }

            for (int yy = Math.Max(y - 1, 0); yy <= Math.Min(y + 1, schematic.Length); yy++){
                for (int xx = Math.Max(x - 1, 0); xx <= Math.Min(x + 1, schematic[yy].Length); xx++){
                    if (Char.IsNumber(schematic[yy][xx])) {
                        int partNumberStart = getNumStart(schematic[yy], xx);
                        int partNumberEnd = getNumEnd(schematic[yy], xx);

                        // Check it hasn't already been added
                        string coordinates = yy.ToString() + ":" + partNumberStart.ToString();
                        if (alreadyFound.Contains(coordinates)){
                            break;
                        }

                        string partNumber = schematic[yy].Substring(partNumberStart, (partNumberEnd - partNumberStart + 1));
                        partNumbers.Add(int.Parse(partNumber));
                        alreadyFound.Add(coordinates);
                    }
                }
            }
        }

        private int getNumStart(string line, int xx){
            int firstNum = xx;
            for (int i = xx; i >= 0; i --) {
                if (Char.IsNumber(line[i])) {
                    firstNum = i;
                }
                else {
                    break;
                }
            }
            return firstNum;
        }

        private int getNumEnd(string line, int xx){
            int lastNum = xx;
            for (int i = xx; i < line.Length; i ++) {
                if (Char.IsNumber(line[i])) {
                    lastNum = i;
                }
                else {
                    break;
                }
            }
            return lastNum;
        }

    }
}
