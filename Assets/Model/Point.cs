namespace CellularAutomaton.Controller
{
    public class Point
    {
        public int x;
        public int y;

        public Point()
        {
            x = y = 0;
        }
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(Point))
                return false;
            Point c = obj as Point;
            return (this.x == c.x && this.y == c.y);
        }
    }
}