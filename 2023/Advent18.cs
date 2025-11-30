using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace HelloWorld
{
    public class Advent18
    {
        public static void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/2023/Advent18.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<PlanRow> plan = lines.Select(l => PlanRow.FromString(l)).ToList();

            task1(plan);

            List<PlanRow> pLines = lines.Select(l => PlanRow.FromString2(l)).ToList();
            //List<PlanRow> pLines = lines.Select(l => PlanRow.FromString(l)).ToList();

            task2(pLines);
        }

        private static void task1(List<PlanRow> plan) {
            // SparseGrid grid = new SparseGrid(plan);
            FlatGrid grid = new FlatGrid();
            grid.Walk(plan);

            Console.WriteLine(grid.ToString());
            Console.WriteLine();

            grid.FillGrid();

            Console.WriteLine(grid.ToString());
            Console.WriteLine("Fills: " + grid.countFills());
        }

        private static void task2(List<PlanRow> lines) {
            CircularEnumerator<PlanRow> cc = new CircularEnumerator<PlanRow>(lines);

            List<Trench> trenches = new List<Trench>();
            Point current = new Point(0, 0);
            while (cc.MoveNext() && cc.Loops < 1) {
                Point nxt = cc.Current.OffsetPoint(current);
                Trench t = new Trench(current, nxt, cc.Current.Direction == Direction.R || cc.Current.Direction == Direction.L);
                if (cc.Prev.Direction == cc.Next.Direction) {
                    t.Flip = true;
                }
                else {
                    t.Flip = false;
                }
                trenches.Add(t);
                current = nxt;
            }

            // Get Y value for each row in the grid which has a corner
            List<int> allCornerHeights = trenches.SelectMany(t => new int[]{t.A.Y, t.B.Y}).Distinct().Order().ToList();
            TrenchCollection tc = new TrenchCollection(trenches);

            long sum=0;
            int prevCornerHeight = allCornerHeights.First();
            foreach (int y in allCornerHeights) {
                
                // Add the sum for all previous rows between the last corner and this corner
                // sum += prevVolume * (y - prevCornerHeight - 1);

                // Calculate the value of this row, which has one or more horizontal sections
                sum += tc.HorizontalVolume(y);

                // Calculate value of the previous row, and multiply it by the number of rows there are
                long prevRowSum = tc.HorizontalVolume(y - 1);
                prevRowSum *= y - prevCornerHeight - 1;
                sum += prevRowSum;

                prevCornerHeight = y;
            }
            Console.WriteLine("Fast total area: " + sum);
        }

        public class Trench {
            public Point A, B;
            public bool Horizontal;
            public bool? Flip;

            public Trench (Point a, Point b, bool horizontal) {
                A = a;
                B = b;
                Horizontal = horizontal;
            }

            public int LeftMostX {
                get { return Math.Min(A.X, B.X);}
            }

            public int RightMostX {
                get { return Math.Max(A.X, B.X);}
            }

        }

        public class TrenchCollection {
            List<Trench> Trenches;

            public TrenchCollection(List<Trench> trenches) {
                Trenches = trenches;
            }

            public IEnumerable<Trench> HorizontalCrossings(int y) {
                return Trenches.Where(t =>
                                        (t.Horizontal == true && t.A.Y == y)||
                                        (t.Horizontal == false && between(y, t.A.Y, t.B.Y))
                                    );
            }

            // public IEnumerable<Trench> VerticalWithinHeight(int y) {
            //     return Trenches.Where(t => t.Horizontal == false && between(y, t.A.Y, t.B.Y));
            // }

            private static bool between (int x, int a, int b) {
                return ((a < x) && (x < b)) || ((b < x)&&(x < a));
            }

            public long HorizontalVolume (int y) {
                long currentRowSum = 0;
                bool inside = false;
                long lastStartX = int.MinValue;
                foreach (Trench t in HorizontalCrossings(y).OrderBy(t => t.A.X)) {
                    if (t.Horizontal && !t.Flip.Value) {
                        if (! inside) {
                            currentRowSum += Math.Abs(t.B.X - t.A.X) + 1;
                        }
                    }
                    else {
                        if (inside) {
                            currentRowSum += t.RightMostX - lastStartX + 1;
                        }
                        else if (inside == false){
                            lastStartX = t.LeftMostX;
                        }
                        inside = !inside;
                    }

                }
                return currentRowSum;
            }

        }



        public enum Direction {
            U,
            R,
            D,
            L
        }

        public class PlanRow {
            public Direction Direction;
            public int Distance;

            public PlanRow (Direction direction, int distance) {
                Direction = direction;
                Distance = distance;
            }

            public static PlanRow FromString (string line) {
                string[] parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                Direction d = parts[0] == "U" ? Direction.U : parts[0] == "R" ? Direction.R : parts[0] == "D" ? Direction.D : parts[0] == "L" ? Direction.L : throw new Exception();

                return new PlanRow(d, int.Parse(parts[1]));
            }

            public static PlanRow FromString2 (string line) {
                string[] parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                string hex = parts[2].Trim('(', ')', '#');

                int dist = Convert.ToInt32(hex.Substring(0, 5), 16);
                Direction dir = hex[5] == '0' ? Direction.R : hex[5] == '1' ? Direction.D : hex[5] == '2' ?  Direction.L : hex[5] == '3' ? Direction.U : throw new Exception();

                return new PlanRow(dir, dist);
            }

            public Point OffsetPoint(Point p) {
                switch (this.Direction) {
                    case Direction.U:
                        return new Point(p.X, p.Y - this.Distance);
                    case Direction.D:
                        return new Point(p.X, p.Y + this.Distance);
                    case Direction.L:
                        return new Point(p.X - this.Distance, p.Y);
                    case Direction.R:
                        return new Point(p.X + this.Distance, p.Y);
                    default:
                        throw new Exception();
                }
            }
        }

        public class SparseGrid {
            private Dictionary<int, Dictionary<int, bool>> grid = new Dictionary<int, Dictionary<int, bool>>();

            public SparseGrid(List<PlanRow> plan)
            {
                int x=0, y=0;
                Set(x, y, true);
                foreach (PlanRow p in plan) {
                    for (int i=0; i < p.Distance; i++) {
                        if (p.Direction == Direction.U) {
                            y--;
                        }
                        else if (p.Direction == Direction.D) {
                            y++;
                        }
                        else if (p.Direction == Direction.L) {
                            x--;
                        }
                        else if (p.Direction == Direction.R) {
                            x++;
                        }
                        Set(x, y, true);
                    }
                }
            }

            public bool Get(int x, int y) {
                Dictionary<int, bool> row;

                if (grid.TryGetValue(y, out row)) {
                    bool val;
                    if (row.TryGetValue(x, out val)) {
                        return val;
                    }
                }

                return false;
            }

            public void Set(int x, int y, bool val) {
                if (!grid.ContainsKey(y)) {
                    grid[y] = new Dictionary<int, bool>();
                }
                grid[y][x] = val;
            }

            public void GetBorders(out int minX, out int maxX, out int minY, out int maxY) {
                minY = maxY = grid.First().Key;
                minX = maxX = grid.First().Value.First().Key;

                foreach (KeyValuePair<int, Dictionary<int, bool>> row in grid) {
                    minY = Math.Min(minY, row.Key);
                    maxY = Math.Max(maxY, row.Key);
                    foreach (KeyValuePair<int, bool> xRow in row.Value) {
                        minX = Math.Min(minX, xRow.Key);
                        maxX = Math.Max(maxX, xRow.Key);
                    }
                }
            }

            public override string ToString()
            {
                int minX, maxX, minY, maxY;
                GetBorders(out minX, out maxX, out minY, out maxY);

                char[][] output = new char[maxY - minY + 1][];
                for(int y=minY; y <= maxY; y++){
                    output[y - minY] = new char[maxX - minX + 1];
                    for (int x=minX; x <= maxX; x++) {
                        output[y - minY][x - minX] = '.';
                    }
                }

                foreach (KeyValuePair<int, Dictionary<int, bool>> row in grid) {
                    foreach (KeyValuePair<int, bool> point in row.Value) {
                        output[row.Key - minY][point.Key - minX] = point.Value ? '#' : '.';
                    }
                }

                return string.Join(System.Environment.NewLine, output.Select(row => new string(row)));
            }

        }

        public class FlatGrid {
            List<List<bool>> grid = new List<List<bool>>();
            public int yOffset, xOffset;

            public FlatGrid() {
                //g.GetBorders(out xOffset, out yOffset, out _, out _);

            }

            public void Walk(List<PlanRow> plan) {
                int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;

                int x=0, y=0;
                foreach (PlanRow p in plan) {
                    for (int i=0; i < p.Distance; i++) {
                        if (p.Direction == Direction.U) {
                            y--;
                        }
                        else if (p.Direction == Direction.D) {
                            y++;
                        }
                        else if (p.Direction == Direction.L) {
                            x--;
                        }
                        else if (p.Direction == Direction.R) {
                            x++;
                        }
                    }
                    if (x < minX) {
                        minX = x;
                    }
                    if (y < minY) {
                        minY = y;
                    }
                    if (x > maxX) {
                        maxX = x;
                    }
                    if (y > maxY) {
                        maxY = y;
                    }
                }

                xOffset = -minX;
                yOffset = -minY;

                
                grid = new List<List<bool>>();
                for (int i=0; i < (maxY - minY + 1); i++) {
                    grid.Add(new List<bool>(Enumerable.Repeat(false, maxX - minY + 1)));
                }

                grid[yOffset][xOffset] = true;
                foreach (PlanRow p in plan) {
                    for (int i=0; i < p.Distance; i++) {
                        if (p.Direction == Direction.U) {
                            y--;
                        }
                        else if (p.Direction == Direction.D) {
                            y++;
                        }
                        else if (p.Direction == Direction.L) {
                            x--;
                        }
                        else if (p.Direction == Direction.R) {
                            x++;
                        }
                        grid[y + yOffset][x + xOffset] = true;
                    }
                }

            }


            private void enqueueIfFalse (Queue<Point> toDig, int x, int y) {
                if (! grid[y][x]) {
                    grid[y][x] = true;
                    toDig.Enqueue(new Point(x, y));
                }
            }

            public void FillGrid()
            {
                Queue<Point> toDig = new Queue<Point>();
                for (int j=0; j < grid[0].Count; j++) {
                    if (grid[0][j] == true && grid[1][j] == false) {
                        toDig.Enqueue(new Point(j, 1));
                        break;
                    }
                }

                while (toDig.Any()) {
                    Point next = toDig.Dequeue();
                    grid[next.Y][next.X] = true;

                    enqueueIfFalse(toDig, next.X, next.Y - 1);
                    enqueueIfFalse(toDig, next.X, next.Y + 1);
                    enqueueIfFalse(toDig, next.X - 1, next.Y);
                    enqueueIfFalse(toDig, next.X + 1, next.Y);
                }


                // for (int i=0; i < grid.Count; i++) {
                //     bool inside = false;
                //     bool prev = false;
                //     bool entryTop = false;

                //     for (int j=0; j < grid[i].Count; j++) {
                //         bool val = grid[i][j];

                //         if (val && prev) {
                //             if (i > 0 && grid[i-1][j-1]) {
                //                 entryTop = true;
                //             }
                //             else {
                //                 entryTop = false;
                //             }
                //             while (grid[i][j]) {
                //                 j++;
                //             }
                //             if (i > 0 && j < grid[i].Count) {
                //                 if (entryTop != grid[i-1][j-1])
                //             }
                //         }
                //         else if (val && (!prev)) {

                //             inside = !inside;
                //         }
                //         else if (inside) {
                //             grid[i][j] = true;
                //         }
                //         prev = val;
                //     }
                // }
            }
            public override string ToString()
            {
                return string.Join(Environment.NewLine, grid.Select(row => new string(row.Select(p => p ? '#' : '.').ToArray())));
            }

            public int countFills() {
                return grid.SelectMany(row => row).Select(v => v ? (int)1 : (int) 0).Sum();
            }

        }

        public class Point {
            public int X, Y;
            public Point(int x, int y){
                X = x;
                Y = y;
            }
            public override string ToString()
            {
                return $"({Y},{X})";
            }
        }


    }
}
