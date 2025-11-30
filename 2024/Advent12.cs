using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using Advent.lib;

namespace Advent.A2024
{
    public class Advent12
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent12.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Grid<Plot> plots = new Grid<Plot>(lines.Select(l => l.Select(c => new Plot(c))));

            Stopwatch st1 = new Stopwatch();
            st1.Start();
            task1(plots);
            st1.Stop();


            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2(plots);
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );
        }

        private static void task1(Grid<Plot> plots) {
            List<List<GridItem<Plot>>> groupedPlots = GroupPlots(plots);


            List<int> lst = groupedPlots.Select(g =>
                g.Count * g.Select(p => p.Item.CountFences()).Sum())
                .ToList();

            int cost = groupedPlots.Select(g =>
                g.Count * g.Select(p => p.Item.CountFences()).Sum())
                .Sum();
            
            Console.WriteLine("Cost: " + cost);
        }

        private static void task2(Grid<Plot> plots) {
            verticalEdges(plots);
            horizontalEdges(plots);

            List<List<GridItem<Plot>>> groupedPlots = GroupPlots(plots);

            long total=0;

            foreach (IEnumerable<Plot> group in groupedPlots.Select(p => p.Select(x => x.Item))) {
                HashSet<long> edgeCount = new HashSet<long>();
                long itemCount=0;
                foreach(Plot p in group) {
                    itemCount++;
                    if (p.LeftEdge != null) {edgeCount.Add(p.LeftEdge.Id);}
                    if (p.RightEdge != null) {edgeCount.Add(p.RightEdge.Id);}
                    if (p.TopEdge != null) {edgeCount.Add(p.TopEdge.Id);}
                    if (p.BottomEdge != null) {edgeCount.Add(p.BottomEdge.Id);}
                }
                total += (edgeCount.Count * itemCount);
            }

            Console.WriteLine("Total with bulk discount: " + total);

            //printVertEdges(plots);
        }

        private static void printVertEdges(Grid<Plot> plots)
        {
            List<string> lines = Enumerable.Repeat("", plots.Height).ToList();

            long lastLeftId=-1, lastRightId=-1;
            char leftEdgeChar = '(';
            char rightEdgeChar = ')';

            //StringBuilder sb = new StringBuilder();
            for (int x=0; x < plots.Width; x++) {
                for (int y=0; y < plots.Height; y++){
                    Plot p = plots.GetItemAt(x, y);
                    if (p.LeftEdge != null) {
                        if (p.LeftEdge.Id != lastLeftId) {
                            leftEdgeChar = leftEdgeChar == '(' ? '[' : '(';
                            lastLeftId = p.LeftEdge.Id;
                        }
                        lines[y] = lines[y] + leftEdgeChar;
                    }
                    else {
                        lines[y] = lines[y] + ' ';
                    }

                    lines[y] = lines[y] + p.Plant;

                    if (p.RightEdge != null) {
                        if (p.RightEdge.Id != lastRightId) {
                            rightEdgeChar = rightEdgeChar == ')' ? ']' : ')';
                            lastRightId = p.RightEdge.Id;
                        }
                        lines[y] = lines[y] + rightEdgeChar;
                    }
                    else {
                        lines[y] = lines[y] + ' ';
                    }
                }
            }
            Console.Write(string.Join(System.Environment.NewLine, lines));
            Console.WriteLine();
        }


        private static void verticalEdges(Grid<Plot> plots) {
            for (int x=0; x <= plots.Width; x++) {
                Edge leftEdge = null;
                Edge rightEdge = null;

                for (int y=0; y < plots.Height; y++) {
                    // Left edge
                    Plot leftItem, rightItem;
                    leftItem = plots.GetItemAt(x-1, y, true);
                    rightItem = plots.GetItemAt(x, y, true);

                    if (isAnEdge(leftItem, rightItem)) {
                        if (leftEdge == null) {
                            leftEdge = new Edge(leftItem, rightItem);
                        }
                        if (!edgeContinues(leftEdge, leftItem)) {
                            leftEdge = new Edge(leftItem, rightItem);
                        }
                        leftItem.RightEdge = leftEdge;
                    }
                    else {
                        leftEdge = null;
                    }
                    // Right edge

                    if (isAnEdge(rightItem, leftItem)) {
                        if (rightEdge == null) {
                            rightEdge = new Edge(rightItem, leftItem);
                        }
                        if (!edgeContinues(rightEdge, rightItem)) {
                            rightEdge = new Edge(rightItem, leftItem);
                        }
                        rightItem.LeftEdge = rightEdge;
                    }
                    else {
                        rightEdge = null;
                    }

                }
            }
        }

        private static void horizontalEdges(Grid<Plot> plots) {
            for (int y=0; y <= plots.Height; y++) {
                Edge topEdge = null;
                Edge bottomEdge = null;

                for (int x=0; x < plots.Width; x++) {
                    Plot topItem, bottomItem;
                    topItem = plots.GetItemAt(x, y-1, true);
                    bottomItem = plots.GetItemAt(x, y, true);

                    if (isAnEdge(topItem, bottomItem)) {
                        if (topEdge == null) {
                            topEdge = new Edge(topItem, bottomItem);
                        }
                        if (!edgeContinues(topEdge, topItem)) {
                            topEdge = new Edge(topItem, bottomItem);
                        }
                        topItem.BottomEdge = topEdge;
                    }
                    else {
                        topEdge = null;
                    }
                    // Right edge

                    if (isAnEdge(bottomItem, topItem)) {
                        if (bottomEdge == null) {
                            bottomEdge = new Edge(bottomItem, topItem);
                        }
                        if (!edgeContinues(bottomEdge, bottomItem)) {
                            bottomEdge = new Edge(bottomItem, topItem);
                        }
                        bottomItem.TopEdge = bottomEdge;
                    }
                    else {
                        bottomEdge = null;
                    }

                }
            }
        }


        private static bool edgeContinues(Edge leftEdge, Plot insideItem)
        {
            if (insideItem.Plant == leftEdge.InsidePlot.Plant) {
                return true;
            }
            return false;
        }


        private static bool isAnEdge(Plot insideItem, Plot outsideItem)
        {
            if (insideItem == null) {
                return false;
            }
            if (outsideItem == null) {
                return true;
            }
            if (insideItem.Plant == outsideItem.Plant) {
                return false;
            }
            return true;
        }

        public static List<List<GridItem<Plot>>> GroupPlots(Grid<Plot> grid) {
            List<List<GridItem<Plot>>> plotGroups = new List<List<GridItem<Plot>>>();
            HashSet<int> groupedPlots = new HashSet<int>();

            foreach (GridItem<Plot> p in grid) {
                if (! groupedPlots.Contains(p.Item.Id)) {
                    List<GridItem<Plot>> g = new List<GridItem<Plot>>();
                    group(g, groupedPlots, p);
                    plotGroups.Add(g);
                }
            }
            return plotGroups;
        }

        private static void group(List<GridItem<Plot>> g, HashSet<int> groupedPlots, GridItem<Plot> p)
        {
            groupedPlots.Add(p.Item.Id);
            g.Add(p);

            foreach (GridItem<Plot> neighbor in p.Neighbors) {
                if ((! groupedPlots.Contains(neighbor.Item.Id))
                    &&(neighbor.Item.Plant == p.Item.Plant))
                {
                    group(g, groupedPlots, neighbor);
                }
            }
        }

        public class Plot : IGridRegisterable<Plot> {
            public char Plant;
            public int Id;

            private static object lockRef = new object();
            private static int idInc = 0;
            public Plot(char plant) {
                Plant = plant;
                lock(lockRef) {
                    Id = idInc++;
                }
            }

            public int CountFences() {
                int fenceCount=0;
                if (!gridRef.HasUp || gridRef.Up.Item.Plant != this.Plant) {
                    fenceCount++;
                }
                if (!gridRef.HasDown || gridRef.Down.Item.Plant != this.Plant) {
                    fenceCount++;
                }
                if (!gridRef.HasLeft || gridRef.Left.Item.Plant != this.Plant) {
                    fenceCount++;
                }
                if (!gridRef.HasRight || gridRef.Right.Item.Plant != this.Plant) {
                    fenceCount++;
                }
                return fenceCount;
            }

            private GridItem<Plot> gridRef = null;
            public void Register(GridItem<Plot> gridItem)
            {
                gridRef = gridItem;
            }

            public IEnumerable<Line> GetEdges()
            {
                List<Line> edges = new ();
                if (!gridRef.HasUp || gridRef.Up.Item.Plant != this.Plant) {
                    edges.Add(new Line(new Point(gridRef.X, gridRef.Y), new Point(gridRef.X + 1, gridRef.Y)));
                }
                if (!gridRef.HasDown || gridRef.Down.Item.Plant != this.Plant) {
                    edges.Add(new Line(new Point(gridRef.X, gridRef.Y + 1), new Point(gridRef.X + 1, gridRef.Y + 1)));
                }
                if (!gridRef.HasLeft || gridRef.Left.Item.Plant != this.Plant) {
                    edges.Add(new Line(new Point(gridRef.X, gridRef.Y), new Point(gridRef.X, gridRef.Y + 1)));
                }
                if (!gridRef.HasRight || gridRef.Right.Item.Plant != this.Plant) {
                    edges.Add(new Line(new Point(gridRef.X + 1, gridRef.Y), new Point(gridRef.X + 1, gridRef.Y + 1)));
                }
                return edges;
            }

            public Edge LeftEdge = null;
            public Edge RightEdge = null;
            public Edge TopEdge = null;
            public Edge BottomEdge = null;

        }

        public class Edge {
            public long Id;
            public Plot InsidePlot, OutsidePlot;

            private static object lockRef = new object();
            private static long idInc = 0;
            public Edge(Plot sideA, Plot sideB) {
                lock(lockRef) {
                    Id = idInc++;
                }

                InsidePlot = sideA;
                OutsidePlot = sideB;
            }

        }

    }
}
