using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics.Tracing;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace HelloWorld
{
    public class Advent17
    {
        public static void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent17.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<List<Node>> nodes = new List<List<Node>>();

            foreach (string line in lines) {
                List<Node> row = new List<Node>();
                foreach (char c in line) {
                    Node n = new Node();
                    n.Cost = int.Parse(new string(c, 1));
                    n.X = row.Count;
                    n.Y = nodes.Count;
                    row.Add(n);
                }
                nodes.Add(row);
            }

            int maxX = lines[0].Length;
            int maxY = lines.Length;

            for (int y=0; y < maxY; y++) {
                for (int x=0; x < maxX; x++) {
                    if (y != 0) {
                        nodes[y][x].Peers.Add(Direction.Up, nodes[y-1][x]);
                    }
                    if (y !=  maxY - 1) {
                        nodes[y][x].Peers.Add(Direction.Down, nodes[y+1][x]);
                    }
                    if (x != 0) {
                        nodes[y][x].Peers.Add(Direction.Left, nodes[y][x-1]);
                    }
                    if (x != maxX - 1) {
                        nodes[y][x].Peers.Add(Direction.Right, nodes[y][x+1]);
                    }
                }
            }

            //task1(nodes);
            task2(nodes);
        }

        private static void task1(List<List<Node>> nodes) {
            PathNode root = OriginalPathNode.CreateRoot(nodes[0][0], Direction.None);
            Node destination = nodes.Last().Last();
            destination.IsDestination = true;
            Queue<PathNode> paths = new Queue<PathNode>();
            paths.Enqueue(root);

            PathNode current;
            while (paths.TryDequeue(out current)) {
                if (current.Invalidated) {
                    continue;
                }
                foreach (PathNode p in current.newPaths()) { paths.Enqueue(p); }
            }

            Console.WriteLine(toString(nodes));
            // Console.WriteLine(toStringPaths(nodes));
            Console.WriteLine("Costs: " + string.Join(",", destination.Paths.Select(p => p.Value.CostAtNode.ToString())));
            Console.WriteLine("Min cost: " + destination.Paths.Select(p => p.Value.CostAtNode).Min());
            // Console.WriteLine("Costs: " + string.Join(",", destination.Paths.Select(p => p.CostAtNode.ToString())));
            // Console.WriteLine("Min cost: " + destination.Paths.Select(p => p.CostAtNode).Min());
        }

        private static void task2(List<List<Node>> nodes) {
            foreach (Node n in nodes.SelectMany(n => n)) {
                n.Reset();
            }

            PathNode root = UberPathNode.CreateRoot(nodes[0][0], Direction.None);
            Node destination = nodes.Last().Last();
            destination.IsDestination = true;

            Queue<PathNode> paths = new Queue<PathNode>();
            paths.Enqueue(root);

            PathNode current;
            while (paths.TryDequeue(out current)) {
                if (current.Invalidated) {
                    continue;
                }
                foreach (PathNode p in current.newPaths()) { paths.Enqueue(p); }
            }

            Console.WriteLine(toString(nodes));
            // Console.WriteLine(toStringPaths(nodes));

            foreach (PathNode p in destination.Paths.Values) {
            //foreach (PathNode p in nodes[131][140].Paths.Values) {
            //foreach (PathNode p in nodes[133][140].Paths.Values) {
                Console.WriteLine();
                Console.WriteLine("Cost: " + p.CostAtNode);
                Console.WriteLine(printPath(nodes, p));
            }
            //string.Join(System.Environment.NewLine, nodes[131][136].Paths.Values.Select(p => printPath(nodes, p)));

            Console.WriteLine("Costs: " + string.Join(",", destination.Paths.Select(p => p.Value.CostAtNode.ToString())));
            Console.WriteLine("Min cost: " + destination.Paths.Select(p => p.Value.CostAtNode).Min());
            // Console.WriteLine("Costs: " + string.Join(",", destination.Paths.Select(p => p.Value.CostAtNode)));
            // //Console.WriteLine("Min cost: " + destination.Paths.Select(p => p.Value.CostAtNode).Min());
        }

        private static string printPath(List<List<Node>> nodes, PathNode path) {
            List<List<string>> output = new List<List<string>>();

            foreach (List<Node> nList in nodes) {
                output.Add(new List<string>());
                foreach (Node n in nList) {
                    output.Last().Add(n.Cost.ToString());
                }
            }

            PathNode p = path;
            while (p != null) {
                output[p.N.Y][p.N.X] = 
                    p.Direction == Direction.Left ? "<" :
                    p.Direction == Direction.Right ? ">" :
                    p.Direction == Direction.Up ? "^" :
                    p.Direction == Direction.Down ? "v" :
                    p.N.Cost.ToString();
                p = p.Parent;
            }

            return string.Join(System.Environment.NewLine, output.Select(line => string.Concat(line)));
        }

        private static string toString(List<List<Node>> nodes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (List<Node> row in nodes) {
                foreach (Node n in row) {
                    sb.Append(n.Cost);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }


        public class Node {
            public int X,Y;
            public int Cost;
            public bool IsDestination = false;

            public Dictionary<Direction, Node> Peers = new Dictionary<Direction, Node>();

            public Dictionary<Tuple<Direction, int>, PathNode> Paths = new Dictionary<Tuple<Direction, int>, PathNode>();
            //public List<PathNode> Paths = new List<PathNode>();
            //public PathNode BestPath;

            public void Reset(){
                //this.Paths = new Dictionary<Tuple<Direction, int>, PathNode>();
                this.Paths.Clear();
            }

            internal Tuple<int, int> Coords()
            {
                return new Tuple<int, int>(Y, X);
            }

            public bool CanTravelDistance(Direction dir, int dist) {
                if (dist == 0) {
                    return true;
                }
                if (Peers.ContainsKey(dir)) {
                    return Peers[dir].CanTravelDistance(dir, dist - 1);
                }
                return false;
            }

        }

        public abstract class PathNode {
            public Node N;
            public int CostAtNode;
            public List<PathNode> Children;
            public PathNode Parent;
            public Direction Direction;
            protected HashSet<Tuple<int, int>> nodesInPath;

            public bool Invalidated { get; private set; } = false;

            protected void initNode(Node n, PathNode parent, Direction dir) {
                N = n;
                Parent = parent;
                Direction = dir;
                CostAtNode = parent.CostAtNode + N.Cost;
                nodesInPath = new HashSet<Tuple<int, int>>(parent.nodesInPath);
                nodesInPath.Add(N.Coords());
            }

            protected abstract PathNode createNode (Node n, PathNode parent, Direction dir);

            private int? bestCostToDestination;
            public IEnumerable<PathNode> newPaths() {
                if (this.N.IsDestination) {
                    if (bestCostToDestination > this.CostAtNode || bestCostToDestination == null) {
                        bestCostToDestination = this.CostAtNode;
                    }
                    return new List<PathNode>();
                }

                List<PathNode> children = new List<PathNode>();
                foreach (KeyValuePair<Direction, Node> peer in N.Peers) {
                    if (N.Y == 131 && N.X == 136 && this.CostAtNode == 928) {

                    }

                    if (!isPathValid(peer.Key)){
                        continue;
                    }

                    // if (!nodesInPath.Contains(peer.Value.Coords())){
                    //     PathNode child = createNode(peer.Value, this, peer.Key);
                    //     if (bestCostToDestination == null || child.CostAtNode < bestCostToDestination) {
                    //         peer.Value.Paths.Add(child);
                    //         children.Add(child);
                    //     }
                    // }


                    if (!nodesInPath.Contains(peer.Value.Coords())){
                        Tuple<Direction, int> travelKey = TravelKey(peer.Key);
                        if (! peer.Value.Paths.ContainsKey(travelKey)) {
                            PathNode child = createNode(peer.Value, this, peer.Key);
                            peer.Value.Paths[travelKey] = child;
                            children.Add(child);
                        }
                        else if (peer.Value.Paths[travelKey].CostAtNode > (this.CostAtNode + peer.Value.Cost)) {
                            PathNode child = createNode(peer.Value, this, peer.Key);
                            peer.Value.Paths[travelKey].Invalidate();
                            peer.Value.Paths[travelKey] = child;
                            children.Add(child);
                        }
                    }
                }
                this.Children = children;
                return children;
            }

            private void Invalidate()
            {
                if (this.N.X == 140 && this.N.Y == 134) {
                    
                }

                Queue<PathNode> toInvalidate = new Queue<PathNode>();

                toInvalidate.Enqueue(this);
                PathNode current;
                while (toInvalidate.TryDequeue(out current)) {
                    current.Invalidated = true;
                    if (current.Children != null) {
                        foreach (PathNode p in current.Children) {toInvalidate.Enqueue(p);}
                    }
                }
            }

            private void createNode() {

            }


            public Tuple<Direction, int> TravelKey (Direction dir) {

                PathNode current = this;
                int sameDirectionSteps=1;
                for (;; sameDirectionSteps++) {
                    if (current.Direction != dir) {
                        break;
                    }
                    if (current.Parent != null) {
                        current = current.Parent;
                    }
                }
                return new Tuple<Direction, int>(dir, sameDirectionSteps);

                // if (this.Direction == dir) {
                //     if (Parent != null && Parent.Direction == dir) {
                //         return new Tuple<Direction, int>(dir, 3);
                //     }
                //     return new Tuple<Direction, int>(dir, 2);
                // }
                // return new Tuple<Direction, int>(dir, 1);
            }


            protected abstract bool isPathValid(Direction nextDirection);
        }

        public class OriginalPathNode : PathNode {

            // public OriginalPathNode(Node n, PathNode parent, Direction dir)
            //                 : base(n, parent, dir) {}

            public static OriginalPathNode CreateRoot(Node n, Direction dir) {
                OriginalPathNode node = new OriginalPathNode();
                node.N = n;
                node.Parent = null;
                node.Direction = dir;
                node.CostAtNode = 0;
                node.nodesInPath = new HashSet<Tuple<int, int>>();
                node.nodesInPath.Add(n.Coords());
                return node;
            }

            protected override PathNode createNode(Node n, PathNode parent, Direction dir)
            {
                OriginalPathNode node = new OriginalPathNode();
                node.initNode(n, parent, dir);
                return node;
            }

            // public PathNode(Node n, PathNode parent, Direction dir) {
            //     N = n;
            //     Parent = parent;
            //     Direction = dir;
            //     CostAtNode = parent.CostAtNode + N.Cost;
            // }

            protected override bool isPathValid(Direction nextDirection)
            {
                // Is going straight for too long
                if ((Parent != null) &&
                    (Parent.Parent != null))
                {
                    if ((nextDirection == Direction) &&
                        (nextDirection == Parent.Direction) &&
                        (nextDirection == Parent.Parent.Direction)) {
                            return false;
                    }
                }

                // is reversing direction
                if ((Direction == Direction.Up && nextDirection == Direction.Down) ||
                    (Direction == Direction.Down && nextDirection == Direction.Up) ||
                    (Direction == Direction.Left && nextDirection == Direction.Right) ||
                    (Direction == Direction.Right && nextDirection == Direction.Left))
                {
                    return false;
                }

                return true;
            }

        }

        public class UberPathNode : PathNode {

            public static UberPathNode CreateRoot(Node n, Direction dir) {
                UberPathNode node = new UberPathNode();
                node.N = n;
                node.Parent = null;
                node.Direction = dir;
                node.CostAtNode = 0;
                node.nodesInPath = new HashSet<Tuple<int, int>>();
                node.nodesInPath.Add(n.Coords());
                return node;
            }

            protected override PathNode createNode(Node n, PathNode parent, Direction dir)
            {
                UberPathNode node = new UberPathNode();
                node.initNode(n, parent, dir);
                return node;
            }


            protected override bool isPathValid(Direction nextDirection)
            {
                PathNode current = this;
                int sameDirectionSteps=0;
                for (; sameDirectionSteps < 11; sameDirectionSteps++) {
                    if (current.Direction != this.Direction) {
                        break;
                    }
                    if (current.Parent != null) {
                        current = current.Parent;
                    }
                }

                // Can never reverse
                if (IsReverse(Direction, nextDirection)) {
                    return false;
                }

                // Must go same direction
                if (sameDirectionSteps <= 3) {
                    return nextDirection == this.Direction;
                }


                if (sameDirectionSteps > 3 && sameDirectionSteps < 10 && Direction == nextDirection) {
                    // Can continue
                    return true;
                }

                if (sameDirectionSteps >= 10 && Direction == nextDirection) {
                    // Cannot go forward
                    return false;
                }

                // here, must be turning. Ensure there is room to turn.
                if (N.CanTravelDistance(nextDirection, 4)) {
                    return true;
                }

                // if (N.Peers[nextDirection].Peers.ContainsKey(nextDirection) &&
                //     N.Peers[nextDirection].Peers[nextDirection].Peers.ContainsKey(nextDirection) &&
                //     N.Peers[nextDirection].Peers[nextDirection].Peers[nextDirection].Peers.ContainsKey(nextDirection)) {
                //     return true;
                // }

                return false;
            }

            private static bool IsReverse(Direction a, Direction b) {
                return ((a == Direction.Up && b == Direction.Down) ||
                    (a == Direction.Down && b == Direction.Up) ||
                    (a == Direction.Left && b == Direction.Right) ||
                    (a == Direction.Right && b == Direction.Left));
            }

        }

        public enum Direction {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3,
            None = 4
        }

    }
}
