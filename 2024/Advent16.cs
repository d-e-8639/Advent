using Advent.lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Advent.A2024
{
    public class Advent16
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent16.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Grid<MazeRef> maze = new Grid<MazeRef>(lines.Select(l => l.Select(c => c == '#' ? MazeRef.Wall : c == '.' ? MazeRef.Tile : c == 'S' ? MazeRef.Start : MazeRef.End)));

            task1(maze);
            task2(maze);
        }


        public enum MazeRef
        {
            Wall,
            Tile,
            Start,
            End
        }
        //public class MazeRef : GridItem<MazeRef>//IGridRegisterable<MazeRef>
        //{
        //    //public void Register(GridItem<MazeRef> gridItem)
        //    //{
        //    //    throw new NotImplementedException();
        //    //}
        //}

        public enum ActionType {
            Start,
            Move,
            RotateClockwise,
            RotateCounterClockwise
        }

        public class Action {
            public GridItem<MazeRef> P;
            public ActionType T;
            public GridDirection D;

            public Action (GridItem<MazeRef> p, ActionType t, GridDirection d) {
                P = p;
                T = t;
                D = d;
            }
        }

        public class PathHashTable {
            // Location, direction, cost
            Dictionary<GridDirection, Path>[] map;
            Grid<MazeRef> grid;
            bool allowEqualCost;

            public PathHashTable (Grid<MazeRef> grid, bool allowEqualCost) {
                this.grid = grid;
                map = new Dictionary<GridDirection, Path>[grid.Width * grid.Height];
                this.allowEqualCost = allowEqualCost;
            }

            public bool existingBetterPath(Path p) {
                Action lastAction = p.Actions.Last();
                int index = (grid.Width * lastAction.P.Y) + lastAction.P.X;
                if (map[index] == null) {
                    map[index] = new Dictionary<GridDirection, Path>();
                }

                if (allowEqualCost) {
                    if (map[index].ContainsKey(lastAction.D) && map[index][lastAction.D].Cost < p.Cost) {
                        return true;
                    }
                    else {
                        map[index][lastAction.D] = p;
                        return false;
                    }
                }
                else {
                    if (map[index].ContainsKey(lastAction.D) && map[index][lastAction.D].Cost <= p.Cost) {
                        return true;
                    }
                    else {
                        map[index][lastAction.D] = p;
                        return false;
                    }
                }
            }
        }

        public class Path
        {
            public Path (List<Action> actions) {
                Actions = actions;
            }
            public Path(Path p, Action nextAction) {
                Actions = new List<Action>(p.Actions);
                Actions.Add(nextAction);
            }

            public List<Action> Actions = new List<Action>();

            private long? _cost;
            public long Cost
            {
                get
                {
                    if (_cost == null) {
                        _cost = Actions.Sum(a => a.T == ActionType.Move ? 1 : a.T == ActionType.Start ? 0 : 1000);
                    }
                    return _cost.Value;
                }
            }

            public bool IsAtEnd() {
                return (Actions.Last().P.Item == MazeRef.End);
            }

            public IEnumerable<Path> NextPaths() {
                Action lastAction = Actions.Last();
                List<Path> potentialPaths = new List<Path>();

                //straight ahead?
                {
                    GridItem<MazeRef> next = lastAction.P.Neighbor(lastAction.D);
                    if (next.Item != MazeRef.Wall && neverVisitedBefore(next)) {
                        potentialPaths.Add(new Path(this, new Action(lastAction.P.Neighbor(lastAction.D), ActionType.Move, lastAction.D)));
                    }
                }

                // Left?
                {
                    GridDirection turnLeft = lastAction.D == GridDirection.Up ? GridDirection.Left :
                                             lastAction.D == GridDirection.Left ? GridDirection.Down :
                                             lastAction.D == GridDirection.Down ? GridDirection.Right :
                                             GridDirection.Up;
                    GridItem<MazeRef> next = lastAction.P.Neighbor(turnLeft);
                    if (next.Item != MazeRef.Wall && neverVisitedBefore(next)) {
                        potentialPaths.Add(new Path(this, new Action(lastAction.P, ActionType.RotateCounterClockwise, turnLeft)));
                    }
                }

                // Right?
                {
                    GridDirection turnRight = lastAction.D == GridDirection.Up ? GridDirection.Right :
                                             lastAction.D == GridDirection.Right ? GridDirection.Down :
                                             lastAction.D == GridDirection.Down ? GridDirection.Left :
                                             GridDirection.Up;
                    GridItem<MazeRef> next = lastAction.P.Neighbor(turnRight);
                    if (next.Item != MazeRef.Wall && neverVisitedBefore(next)) {
                        potentialPaths.Add(new Path(this, new Action(lastAction.P, ActionType.RotateClockwise, turnRight)));
                    }
                }
                return potentialPaths;
            }

            private bool neverVisitedBefore(GridItem<MazeRef> next) {
                return !Actions.Any(a => a.P == next);
            }
        }

        private static void task1(Grid<MazeRef> maze) {
            PathHashTable pHash = new PathHashTable(maze, false);
            LinkedList<Path> allPaths = new LinkedList<Path>();
            allPaths.AddFirst(new Path([new Action(maze.First(i => i.Item == MazeRef.Start), ActionType.Start, GridDirection.Right)]));

            while (!allPaths.Any(p => p.IsAtEnd())) {
                Path shortestPath = allPaths.First.Value;
                allPaths.RemoveFirst();

                IEnumerable<Path> nextPaths = shortestPath.NextPaths();

                nextPaths = nextPaths.Where(p => !pHash.existingBetterPath(p)).ToList();

                if (!allPaths.Any()) {
                    foreach (Path p in nextPaths) {
                        allPaths.AddLast(p);
                    }
                }
                else {
                    LinkedListNode<Path> pin = allPaths.First;
                    foreach (Path p in nextPaths) {
                        while (pin != null && pin.Value.Cost < p.Cost) {
                            pin = pin.Next;
                        }
                        if (pin == null) {
                            allPaths.AddLast(p);
                        }
                        else {
                            allPaths.AddBefore(pin, p);
                        }
                    }

                }

            }

            Path success = allPaths.Single(p => p.IsAtEnd());
            Console.WriteLine(PathToString(maze, success));

            Console.WriteLine("Cheapest path: " + success.Cost);
        }

        public static string PathToString (Grid<MazeRef> maze, Path success) {
            StringBuilder sb = new StringBuilder();
            foreach (List<GridItem<MazeRef>> row in maze.Items) {
                foreach (GridItem<MazeRef> item in row) {
                    char c = '.';
                    if (item.Item == MazeRef.Wall) {
                        c = '#';
                    }
                    else {
                        foreach (Action a in success.Actions) {
                            if (object.ReferenceEquals(a.P, item)) {
                                if (a.T == ActionType.RotateClockwise || a.T == ActionType.RotateCounterClockwise) {
                                    c = 'O';
                                    break;
                                }
                                else if (a.D == GridDirection.Down) {
                                    c = 'v';
                                }
                                else if (a.D == GridDirection.Up) {
                                    c = '^';
                                }
                                else if (a.D == GridDirection.Left) {
                                    c = '<';
                                }
                                else if (a.D == GridDirection.Right) {
                                    c = '>';
                                }
                            }
                            if (c == 'O') { break; }
                        }
                    }
                    sb.Append(c);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private static List<Path> getAllBestPaths(Grid<MazeRef> maze) {
            PathHashTable pHash = new PathHashTable(maze, true);
            LinkedList<Path> allPaths = new LinkedList<Path>();
            allPaths.AddFirst(new Path([new Action(maze.First(i => i.Item == MazeRef.Start), ActionType.Start, GridDirection.Right)]));

            List<Path> completedPaths = new List<Path>();

            while (allPaths.Any()) {
                Path shortestPath = allPaths.First.Value;
                allPaths.RemoveFirst();

                if (completedPaths.Any()) {
                    // Discard any paths that are longer than the known shortest
                    if (shortestPath.Cost > completedPaths.First().Cost) {
                        continue;
                    }
                }

                if (shortestPath.IsAtEnd()) {
                    completedPaths.Add(shortestPath);
                    continue;
                }

                IEnumerable<Path> nextPaths = shortestPath.NextPaths();


                if (nextPaths.Any(p => p.Actions.Last().P.Y == 12 && p.Actions.Last().P.X == 5)) {
                    ;
                }

                if (nextPaths.Any(p => p.Actions.Last().P.Y == 7 && p.Actions.Last().P.X == 15)) {
                    ;
                }

                nextPaths = nextPaths.Where(p => !pHash.existingBetterPath(p)).ToList();


                if (!allPaths.Any()) {
                    foreach (Path p in nextPaths) {
                        allPaths.AddLast(p);
                    }
                }
                else {
                    LinkedListNode<Path> pin = allPaths.First;
                    foreach (Path p in nextPaths) {
                        while (pin != null && pin.Value.Cost < p.Cost) {
                            pin = pin.Next;
                        }
                        if (pin == null) {
                            allPaths.AddLast(p);
                        }
                        else {
                            allPaths.AddBefore(pin, p);
                        }
                    }

                }

            }
            return completedPaths;
        }

        private static void task2(Grid<MazeRef> maze) {
            List<Path> allBest = getAllBestPaths(maze);
            int n=0;

            foreach (Path p in allBest) {
                Console.Write(PathToString(maze, p));
                Console.WriteLine($"Path {++n} cost: " + p.Cost);
                Console.WriteLine();
            }

            HashSet<Point> pointInBestPaths = new HashSet<Point>();

            foreach (Path p in allBest) {
                foreach (Action a in p.Actions) {
                    pointInBestPaths.Add(new Point(a.P.X, a.P.Y));
                }
            }

            Console.WriteLine("Points on best paths: " + pointInBestPaths.Count);            

        }

    }
}
