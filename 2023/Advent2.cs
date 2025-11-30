using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HelloWorld
{
    public class Advent2
    {
        public void Task1(){
            string gameDoc;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent2.txt")) {
                gameDoc = sr.ReadToEnd();
            }

            List<Game> games = new List<Game>();
            foreach (string line in gameDoc.Split('\n', StringSplitOptions.RemoveEmptyEntries)) {
                games.Add(Game.FromString(line));

            }

            Console.WriteLine("Possible Id sum: " + getPossibleIdSum(games));

            Console.WriteLine("Minimum set cube power: " + getMinSetCubePower(games));
        }

        private int getMinSetCubePower(List<Game> games){
            int powerSum=0;
            foreach (Game g in games) {
                int maxRed=0, maxGreen=0, maxBlue=0;
                foreach(Subset s in g.Subsets){
                    maxRed = Math.Max(maxRed, s.Red);
                    maxGreen = Math.Max(maxGreen, s.Green);
                    maxBlue = Math.Max(maxBlue, s.Blue);
                }
                int power = maxRed * maxGreen * maxBlue;
                powerSum += power;
            }
            return powerSum;
        }

        private int getPossibleIdSum(List<Game> games){
            int maxRed = 12;
            int maxGreen = 13;
            int maxBlue = 14;

            int possibleIdSum = 0;
            foreach (Game g in games) {
                bool possible = true;
                foreach (Subset s in g.Subsets){
                    if ((s.Red > maxRed)||(s.Green > maxGreen)||(s.Blue > maxBlue)) {
                        possible = false;
                    }
                }
                if (possible) {
                    possibleIdSum += g.GameNumber;
                }
            }
            return possibleIdSum;
        }

// Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
// Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
// Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
// Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
// Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
        public class Game {
            public int GameNumber;
            public List<Subset> Subsets = new List<Subset>();
            public Game(int number) {
                GameNumber = number;
            }

            public static Game FromString(string line){
                int colon = line.IndexOf(':');
                string game = line.Substring(0, colon);
                string subsets = line.Substring(colon + 1);

                Game g = new Game(int.Parse(game.Substring(5)));
                foreach(string subset in subsets.Split(';', StringSplitOptions.RemoveEmptyEntries)){
                    g.Subsets.Add(Subset.FromString(subset));
                }
                return g;
            }
        }

        public class Subset {
            public int Blue = 0;
            public int Green = 0;
            public int Red = 0;

            public static Subset FromString(string subset){

                Regex r =  new Regex("([0-9]+) ([a-z]+)");
                Subset s = new Subset();

                foreach(string countColour in subset.Split(',', StringSplitOptions.RemoveEmptyEntries)){
                    Match m = r.Match(countColour.Trim());
                    int count = int.Parse(m.Groups[1].Value);
                    string colour = m.Groups[2].Value;
                    if (colour == "red"){
                        s.Red = count;
                    }
                    else if (colour == "green"){
                        s.Green = count;
                    }
                    else if (colour == "blue"){
                        s.Blue = count;
                    }
                }

                return s;
            }
        }
    }

}
