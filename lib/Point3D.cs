using System;

namespace Advent.lib
{
    public class Point3D {
        public int X, Y, Z;
        public Point3D(int x, int y, int z){
            X = x;
            Y = y;
            Z = z;
        }
        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }

        public Point Add(int x,int y) {
            return new Point(X + x, Y + y);
        }

        public double Distance(Point3D other)
        {
            double dist = Math.Sqrt(
                (long)(other.X - this.X)*(long)(other.X - this.X) +
                (long)(other.Y - this.Y)*(long)(other.Y - this.Y) +
                (long)(other.Z - this.Z)*(long)(other.Z - this.Z)
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
            hash = hash * 31 + X;
            hash = hash * 31 + Y;
            hash = hash * 31 + Z;
            return hash;
        }
    }
}
