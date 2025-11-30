using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics.Metrics;

namespace Advent
{
    public class Advent16
    {
        public static void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent16.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<List<Tile>> contraption = new List<List<Tile>>();
            foreach (string line in lines) {
                List<Tile> row = new List<Tile>();
                foreach (char c in line) {
                    row.Add(Tile.Factory(c, row.Count, contraption.Count));
                }
                contraption.Add(row);
            }

            task1(contraption);
            task2(contraption);
        }

        private static void task1(List<List<Tile>> contraption) {
            int energized = countEnergize(contraption, new BeamStep(Direction.Right, 0, 0));
            Console.WriteLine("Energized: " + energized);
        }

        private static void task2(List<List<Tile>> contraption) {
            int maxEner = 0;
            for (int y=0; y < contraption.Count; y++) {
                maxEner = Math.Max(maxEner, countEnergize(contraption, new BeamStep(Direction.Right, 0, y)));
                maxEner = Math.Max(maxEner, countEnergize(contraption, new BeamStep(Direction.Left, contraption[y].Count - 1, y)));
            }
            for (int x=0; x < contraption[0].Count; x++) {
                maxEner = Math.Max(maxEner, countEnergize(contraption, new BeamStep(Direction.Right, x, 0)));
                maxEner = Math.Max(maxEner, countEnergize(contraption, new BeamStep(Direction.Left, x, contraption.Count - 1)));
            }
            Console.Write("Max energize: " + maxEner);
        }


        private static int countEnergize (List<List<Tile>> contraption, BeamStep initialBeam) {
            foreach(Tile t in contraption.SelectMany(r => r)){
                t.Reset();
            }

            Queue<BeamStep> beams = new Queue<BeamStep>();
            beams.Enqueue(initialBeam);

            BeamStep current;
            while (beams.TryDequeue(out current)) {
                if (current.Y < 0 || current.Y >= contraption.Count) {
                    // Out out bounds in Y axis
                    continue;
                }
                if (current.X < 0 || current.X >= contraption[0].Count) {
                    // out of bounds in X axis
                    continue;
                }

                contraption[current.Y][current.X].GenerateNextSteps(current.Direction, beams.Enqueue);
            }

            // printContraption(contraption, false, false);
            // Console.WriteLine();
            // printContraption(contraption, false, true);

            return contraption.SelectMany(row => row).Sum(tile => tile.Illuminated ? 1 : 0);
        }

        private static void printContraption(List<List<Tile>> contraption, bool illPaths, bool illuminate) {
            StringBuilder sb = new StringBuilder();
            for (int y=0; y < contraption.Count; y++ ) {
                for (int x=0; x < contraption[y].Count; x++) {
                    if (illPaths) {
                    }
                    else if (illuminate) {
                        if (contraption[y][x].Illuminated) {
                            sb.Append('*');
                        }
                        else {
                        sb.Append(contraption[y][x].ToString());
                        }
                    }
                    else {
                        sb.Append(contraption[y][x].ToString());
                    }
                }
                sb.AppendLine();
            }
            Console.WriteLine(sb.ToString());
        }


        public enum Direction {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3
        }
        public class BeamStep {
            public Direction Direction;
            public int X, Y;
            public BeamStep (Direction dir, int x, int y) {
                X = x;
                Y = y;
                Direction = dir;
            }
            public Tuple<int, int, int> GetHashKey() {
                return new Tuple<int, int, int>((int) Direction, X, Y);
            }
        }

        public abstract class Tile {
            public int X, Y;

            public static Tile Factory (char c, int x, int y) {
                Tile nuTile;

                if (c == '\\') {
                    nuTile = new BackwardMirror();
                }
                else if (c == '/') {
                    nuTile = new ForwardMirror();
                }
                else if (c == '|') {
                    nuTile = new VertSplitter();
                }
                else if (c == '-') {
                    nuTile = new HorzSplitter();
                }
                else if (c == '.') {
                    nuTile = new Empty();
                }
                else {
                    throw new NotImplementedException();
                }

                nuTile.Y = y;
                nuTile.X = x;
                return nuTile;
            }

            public bool Illuminated {
                get { return visitedDirections.Any(visDir => visDir == true); }
            }

            protected BeamStep nextCoords(Direction d) {
                return new BeamStep(d,
                     d == Direction.Left ? X - 1 : d == Direction.Right ? X + 1 : X,
                     d == Direction.Up ? Y - 1 : d == Direction.Down ? Y + 1 : Y);
            }

            private bool[] visitedDirections = [false, false, false, false];
            public void GenerateNextSteps(Direction d, Action<BeamStep> enqueue) {
                if (visitedDirections[(int)d]) {
                    // Already handled a beam from this direction.
                    return;
                }
                visitedDirections[(int)d] = true;
                pushNextSteps(d, enqueue);
            }

            protected abstract void pushNextSteps(Direction d, Action<BeamStep> enqueue);

            public void Reset(){
                visitedDirections = [false, false, false, false];
            }
        }

        public class Empty : Tile
        {
            protected override void pushNextSteps(Direction d, Action<BeamStep> enqueue)
            {
                enqueue(nextCoords(d));
            }

            public override string ToString()
            {
                return ".";
            }
        }

        public abstract class Splitter : Tile {}
        public class VertSplitter : Splitter {
            protected override void pushNextSteps(Direction d, Action<BeamStep> enqueue)
            {
                if (d == Direction.Up || d == Direction.Down) {
                    enqueue(nextCoords(d));
                }
                else {
                    enqueue(nextCoords(Direction.Up));
                    enqueue(nextCoords(Direction.Down));
                }
            }
            public override string ToString()
            {
                return "|";
            }
        }
        public class HorzSplitter : Splitter
        {
            protected override void pushNextSteps(Direction d, Action<BeamStep> enqueue)
            {
                if (d == Direction.Left || d == Direction.Right) {
                    enqueue(nextCoords(d));
                }
                else {
                    enqueue(nextCoords(Direction.Left));
                    enqueue(nextCoords(Direction.Right));
                }
            }
            public override string ToString()
            {
                return "-";
            }
        }

        public abstract class Mirror : Tile {}
        public class ForwardMirror : Mirror
        {
            private static Direction[] directionMap = new Direction[4];
            static ForwardMirror() {
                directionMap[(int) Direction.Right] = Direction.Up;
                directionMap[(int) Direction.Left] = Direction.Down;
                directionMap[(int) Direction.Up] = Direction.Right;
                directionMap[(int) Direction.Down] = Direction.Left;
            }

            protected override void pushNextSteps(Direction d, Action<BeamStep> enqueue)
            {
                enqueue(nextCoords(directionMap[(int)d]));
            }

            public override string ToString()
            {
                return "/";
            }
        }
        public class BackwardMirror : Mirror
        {
            private static Direction[] directionMap = new Direction[4];
            static BackwardMirror() {
                directionMap[(int)Direction.Right] = Direction.Down;
                directionMap[(int)Direction.Left] = Direction.Up;
                directionMap[(int)Direction.Up] = Direction.Left;
                directionMap[(int)Direction.Down] = Direction.Right;
            }

            protected override void pushNextSteps(Direction d, Action<BeamStep> enqueue)
            {
                enqueue(nextCoords(directionMap[(int)d]));
            }

            public override string ToString()
            {
                return "\\";
            }
        }
    }
}
