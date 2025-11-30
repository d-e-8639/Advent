using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Text;

namespace Advent.A2024
{
    public class Advent6
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent6.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            GridSquare[][] grid = new GridSquare[lines.Length][];

            int xStart=0, yStart=0;

            int y=0;
            foreach (string l in lines) {
                grid[y] = new GridSquare[l.Length];
                int x=0;
                foreach (char c in l) {
                    grid[y][x] = c == '#' ? new GridSquare(true) : new GridSquare(false);
                    if (c == '^') {
                        grid[y][x].IsStart = true;
                        xStart = x;
                        yStart = y;
                    }
                    x++;
                }
                y++;
            }

            task1(grid, xStart, yStart);
            task2(grid, xStart, yStart);
        }

        private static void task1(GridSquare[][] grid, int xStart, int yStart) {
            bool[][] visitGrid = new bool[grid.Length][];
            for (int i=0; i < visitGrid.Length; i++) { visitGrid[i] = new bool[grid[i].Length]; }
            visitGrid[yStart][xStart] = true;

            Guard g = new Guard(Direction.Up, xStart, yStart);
            while (g.Move(grid, visitGrid)) {}

            Console.WriteLine("Visited: " + visitGrid.SelectMany(row => row).Where(v => v).Count());
        }

        private static void task2(GridSquare[][] grid, int xStart, int yStart) {
            bool[][] interceptGrid = new bool[grid.Length][];
            for (int i=0; i < interceptGrid.Length; i++) { interceptGrid[i] = new bool[grid[i].Length]; }
            interceptGrid[yStart][xStart] = true;

            Guard gu = new Guard(Direction.Up, xStart, yStart);
            while (gu.Move(grid, interceptGrid)) {}


            int looped = 0;

            for (int y=0; y < interceptGrid.Length; y++) {
                for (int x=0; x < interceptGrid[0].Length; x++) {
                    GridSquare gs = grid[y][x];

                    if (gs.Stuff || gs.IsStart || !interceptGrid[y][x]) {
                        continue;
                    }

                    gs.Stuff = true;
                    HashSet<Tuple<int, int, int>> PastPositions = new HashSet<Tuple<int, int, int>>();

                    bool[][] visitGrid = new bool[grid.Length][];
                    for (int i=0; i < visitGrid.Length; i++) { visitGrid[i] = new bool[grid[i].Length]; }
                    visitGrid[yStart][xStart] = true;

                    Guard ghost = new Guard(Direction.Up, xStart, yStart);

                    bool loop = false;
                    while (ghost.Move(grid, visitGrid)) {
                        Tuple<int, int, int> pos = new Tuple<int, int, int>(ghost.X, ghost.Y, (int) ghost.D);
                        if (PastPositions.Contains(pos)) {
                            loop = true;
                            break;
                        }
                        PastPositions.Add(pos);
                    }

                    if (loop) {
                        looped ++;
                        //Console.WriteLine(ToString(grid, visitGrid));
                    }

                    gs.Stuff = false;
                }
            }
            Console.WriteLine("Loops: " + looped);

        }

        public static string ToString(GridSquare[][] grid, bool[][] visited) {
            StringBuilder sb = new StringBuilder();
            for (int y=0; y < grid.Length; y++) {
                for (int x=0; x < grid[0].Length; x++) {
                    if (grid[y][x].Stuff) {
                        sb.Append('#');
                    }
                    else if (grid[y][x].IsStart) {
                        sb.Append('^');
                    }
                    else if (visited[y][x]) {
                        sb.Append('0');
                    }
                    else {
                        sb.Append('.');
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public class GridSquare {
            public bool Stuff = false;
            public bool IsStart = false;

            public GridSquare(bool stuff) {
                Stuff = stuff;
            }
        }

        public class Guard {
            public Direction D;
            public int X, Y;

            public Guard(Direction d, int x, int y) {
                D = d;
                X = x;
                Y = y;
            }

            public bool Move (GridSquare[][] grid, bool[][] visitGrid) {
                int xDest = X, yDest = Y;

                if (D == Direction.Up) {
                    yDest--;
                }
                else if (D == Direction.Right) {
                    xDest++;
                }
                else if (D == Direction.Down) {
                    yDest++;
                }
                else if (D == Direction.Left) {
                    xDest--;
                }

                if (xDest < 0 || xDest >= grid[0].Length || yDest < 0 || yDest >= grid.Length) {
                    return false;
                }

                if (grid[yDest][xDest].Stuff) {
                    if (D == Direction.Up) {
                        D = Direction.Right;
                    }
                    else if (D == Direction.Right) {
                        D = Direction.Down;
                    }
                    else if (D == Direction.Down) {
                        D = Direction.Left;
                    }
                    else if (D == Direction.Left) {
                        D = Direction.Up;
                    }
                    return Move(grid, visitGrid);
                }

                this.X = xDest;
                this.Y = yDest;
                visitGrid[yDest][xDest] = true;

                return true;
            }

            public Guard Clone()
            {
                return new Guard(this.D, this.X, this.Y);
            }

        }

        public enum Direction {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3
        }

    }
}
