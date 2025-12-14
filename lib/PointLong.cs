namespace Advent.lib
{
    public class PointLong {
        public long X, Y;
        public PointLong(long x, long y){
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public long xDist (PointLong p) {
            long d = X - p.X;
            if (d < 0) { return -d; }
            return d;
        }

        public long yDist (PointLong p) {
            long d = Y - p.Y;
            if (d < 0) { return -d; }
            return d;
        }

        public PointLong Add(long x,long y) {
            return new PointLong(X + x, Y + y);
        }

        public static bool operator ==(PointLong a, PointLong b) {
            if (object.ReferenceEquals(null, a))
                return object.ReferenceEquals(null, b);
            
            return a.Equals(b);
        }

        public static bool operator !=(PointLong a, PointLong b) {
            if (object.ReferenceEquals(null, a))
                return !object.ReferenceEquals(null, b);

            return ! a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PointLong)) {
                return false;
            }

            PointLong p = (PointLong) obj;

            if ((p.X == X)&&(p.Y == Y)) {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 31 + (int)X;
            hash = hash * 31 + (int)Y;
            return hash;
        }
    }
}
