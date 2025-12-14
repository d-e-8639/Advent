using Advent.A2024;
using System;

namespace Advent.lib
{
    public class Point3D {
        public long X, Y, Z;
        public Point3D(long x, long y, long z) {
            X = x;
            Y = y;
            Z = z;
        }
        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }

        public Point3D Add(long x, long y, long z) {
            return new Point3D(X + x, Y + y, Z + z);
        }

        public double Distance(Point3D other)
        {
            double dist = Math.Sqrt(
                (other.X - this.X)*(other.X - this.X) +
                (other.Y - this.Y)*(other.Y - this.Y) +
                (other.Z - this.Z)*(other.Z - this.Z)
            );
            return dist;
        }

        public static bool operator ==(Point3D a, Point3D b) {
            return a.Equals(b);
        }

        public static bool operator !=(Point3D a, Point3D b) {
            return ! a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Point3D)) {
                return false;
            }

            Point3D p = (Point3D) obj;

            if ((p.X == X)&&(p.Y == Y)&&(p.Y == Y)) {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 31 + (int)X;
            hash = hash * 31 + (int)Y;
            hash = hash * 31 + (int)Z;
            return hash;
        }
    }
}
