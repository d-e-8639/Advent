using System;

namespace HelloWorld.A2024
{
    public class Line {
        public Point Start, End;

        public Line (Point start, Point end) {
            Start = start;
            End = end;
        }

        public Line Reverse() {
            return new Line (End, Start);
        }

        public override string ToString()
        {
            return $"{Start}-{End}";
        }

        public bool IsHorizontal {
            get {return Start.X == End.X;}
        }

        public bool IsVertical {
            get {return Start.Y == End.Y;}
        }

        public double Length {
            get {
                return Math.Sqrt(Math.Pow(Start.X - End.X, 2) + Math.Pow(Start.Y - End.Y, 2));
            }
        }
    }
}