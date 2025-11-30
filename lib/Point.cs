namespace HelloWorld.lib
{
    public class Point {
        public int X, Y;
        public Point(int x, int y){
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public int xDist (Point p) {
            int d = X - p.X;
            if (d < 0) { return -d; }
            return d;
        }

        public int yDist (Point p) {
            int d = Y - p.Y;
            if (d < 0) { return -d; }
            return d;
        }

        public Point Add(int x,int y) {
            return new Point(X + x, Y + y);
        }

        public static bool operator ==(Point a, Point b) {
            return a.Equals(b);
        }

        public static bool operator !=(Point a, Point b) {
            return ! a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Point)) {
                return false;
            }

            Point p = (Point) obj;

            if ((p.X == X)&&(p.Y == Y)) {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 31 + X;
            hash = hash * 31 + Y;
            return hash;
        }
    }
}
