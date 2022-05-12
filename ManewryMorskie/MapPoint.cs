namespace ManewryMorskie
{
    public struct MapPoint
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public MapPoint() => Row = Column = 0;

        public MapPoint(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public MapPoint(Ways ways) : this()
        {
            if ((ways & Ways.Top) != Ways.None)
                Row++;
            if ((ways & Ways.Bottom) != Ways.None)
                Row--;
            if ((ways & Ways.Right) != Ways.None)
                Column++;
            if ((ways & Ways.Left) != Ways.None)
                Column--;
        }

        public static MapPoint operator +(MapPoint a, MapPoint b) => new(a.Row + b.Row, a.Column + b.Column);
        public static MapPoint operator +(MapPoint a, Ways way) => a + new MapPoint(way);
    }
}
