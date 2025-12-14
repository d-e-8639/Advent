using System;
using System.Diagnostics;

namespace Advent.lib
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
            get {return Start.Y == End.Y;}
        }

        public bool IsVertical {
            get {return Start.X == End.X;}
        }

        private static bool between (long a, long b, long pin) {
            if (pin >= Math.Min(a,b) && pin <= Math.Max(a, b)) {
                return true;
            }
            return false;
        }

        public bool Intersect(Line other) {
            if ((this.IsHorizontal)&&(other.IsHorizontal)) {
                if (between(this.Start.X, this.End.X, other.Start.X) ||
                    between(this.Start.X, this.End.X, other.End.X) ||
                    between(other.Start.X, other.End.X, this.Start.X) ||
                    between(other.Start.X, other.End.X, this.End.X)) {
                    return true;
                }
                return false;
            }
            else if ((this.IsVertical)&&(other.IsVertical)) {
                if (between(this.Start.Y, this.End.Y, other.Start.Y) ||
                    between(this.Start.Y, this.End.Y, other.End.Y) ||
                    between(other.Start.Y, other.End.Y, this.Start.Y) ||
                    between(other.Start.Y, other.End.Y, this.End.Y)) {
                    return true;
                }
                return false;
            }
            else if (Crossing(other)) {
                return true;
            }
            //else if ((this.IsHorizontal)&&(other.IsVertical)) {
            //    //other start x is above and end x is below OR
            //    // other start x is below and end x is above
            //    return (
            //        between(other.Start.Y, other.End.Y, this.Start.Y) &&
            //        between(this.Start.X, this.End.X, other.Start.X)
            //    );
            //}
            //else if ((this.IsVertical)&&(other.IsHorizontal)) {
            //    return (
            //        between(other.Start.X, other.End.X, this.Start.X) &&
            //        between(this.Start.Y, this.End.Y, other.Start.Y)
            //    );
            //}
            else {
                throw new NotImplementedException("I didn't write anything to cover this case :(");
            }

        }

        public bool Crossing(Line other) {
            if ((this.IsHorizontal && other.IsHorizontal)||
                (this.IsVertical && other.IsVertical)) {
                // If these lines are paralell, they don't cross. 
                return false;
            }

            if ((this.IsHorizontal) && (other.IsVertical)) {
                //other start x is above and end x is below OR
                // other start x is below and end x is above
                return (
                    between(other.Start.Y, other.End.Y, this.Start.Y) &&
                    between(this.Start.X, this.End.X, other.Start.X)
                );
            }
            else if ((this.IsVertical) && (other.IsHorizontal)) {
                return (
                    between(other.Start.X, other.End.X, this.Start.X) &&
                    between(this.Start.Y, this.End.Y, other.Start.Y)
                );
            }
            else {
                throw new NotImplementedException("I didn't write anything to cover this case :(");
            }
        }

        public bool IsEquivalentTrajectory (Line other) {
            if (this.IsHorizontal && other.IsHorizontal) {
                return this.Start.Y == other.Start.Y;
            }
            else if (this.IsVertical && other.IsVertical) {
                return this.Start.X == other.Start.X;
            }
            else {
                return false;
            }
        }

        public bool PointIsOn(Point p) {
            if (IsHorizontal) {
                return ((p.Y == Start.Y) &&
                    between(Start.X, End.X, p.X));

            }
            else if (IsVertical) {
                return ((p.X == Start.X) &&
                    between(Start.Y, End.Y, p.Y));
            }
            else {
                throw new NotSupportedException();
            }
        }

        public double Length {
            get {
                return Math.Sqrt(Math.Pow(Start.X - End.X, 2) + Math.Pow(Start.Y - End.Y, 2));
            }
        }


        public static void _testIntersect() {

            // Horizontal lines
            Debug.Assert(new Line(new Point(11, 1), new Point(11, 10)).Intersect(new Line(new Point(11, 2), new Point(11, 9))));
            Debug.Assert(new Line(new Point(11, 3), new Point(11, 7)).Intersect(new Line(new Point(11, 1), new Point(11, 10))));

            Debug.Assert(new Line(new Point(11, 1), new Point(11, 7)).Intersect(new Line(new Point(11, 6), new Point(11, 10))));
            Debug.Assert(new Line(new Point(11, 1), new Point(11, 7)).Intersect(new Line(new Point(11, 7), new Point(11, 10))));

            Debug.Assert(new Line(new Point(11, 1), new Point(11, 7)).Intersect(new Line(new Point(11, 6), new Point(11, -10))));
            Debug.Assert(new Line(new Point(11, 1), new Point(11, 7)).Intersect(new Line(new Point(11, 1), new Point(11, -10))));

            Debug.Assert(new Line(new Point(11, 1), new Point(11, 10)).Intersect(new Line(new Point(11, 1), new Point(11, 10))));

            Debug.Assert(false == new Line(new Point(11, 1), new Point(11, 10)).Intersect(new Line(new Point(11, 11), new Point(11, 20))));
            Debug.Assert(false == new Line(new Point(11, 1), new Point(11, 10)).Intersect(new Line(new Point(11, -1), new Point(11, -20))));


            // Vertical lines
            Debug.Assert(new Line(new Point(1, 11), new Point(10, 11)).Intersect(new Line(new Point(2, 11), new Point(9, 11))));
            Debug.Assert(new Line(new Point(3, 11), new Point(7, 11)).Intersect(new Line(new Point(1, 11), new Point(10, 11))));

            Debug.Assert(new Line(new Point(1, 11), new Point(7, 11)).Intersect(new Line(new Point(6, 11), new Point(10, 11))));
            Debug.Assert(new Line(new Point(1, 11), new Point(7, 11)).Intersect(new Line(new Point(7, 11), new Point(10, 11))));

            Debug.Assert(new Line(new Point(1, 11), new Point(7, 11)).Intersect(new Line(new Point(6, 11), new Point(-10, 11))));
            Debug.Assert(new Line(new Point(1, 11), new Point(7, 11)).Intersect(new Line(new Point(1, 11), new Point(-10, 11))));

            Debug.Assert(new Line(new Point(1, 11), new Point(10, 11)).Intersect(new Line(new Point(1, 11), new Point(10, 11))));

            Debug.Assert(false == new Line(new Point(1, 11), new Point(10, 11)).Intersect(new Line(new Point(11, 11), new Point(20, 11))));
            Debug.Assert(false == new Line(new Point(1, 11), new Point(10, 11)).Intersect(new Line(new Point(-1, 11), new Point(-20, 11))));

            // Horizontal crosses vertical
            Debug.Assert(new Line(new Point(11, 1), new Point(11, 10)).Intersect(new Line(new Point(1, 2), new Point(15, 2))));
            Debug.Assert(false == new Line(new Point(11, 1), new Point(11, 10)).Intersect(new Line(new Point(-11, 2), new Point(-1, 2))));
            Debug.Assert(false == new Line(new Point(11, 1), new Point(11, 10)).Intersect(new Line(new Point(12, 3), new Point(15, 3))));

            // vertical crosses horizontal
            Debug.Assert(new Line(new Point(1, 11), new Point(10, 11)).Intersect(new Line(new Point(2, 1), new Point(2, 15))));
            Debug.Assert(false == new Line(new Point(1, 11), new Point(10, 11)).Intersect(new Line(new Point(2, -11), new Point(2, -1))));
            Debug.Assert(false == new Line(new Point(1, 11), new Point(10, 11)).Intersect(new Line(new Point(3, 12), new Point(3, 15))));

        }

    }
}