using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CellLib
{
    public static class Extensions
    {
        private class DirectionsGroup : IEnumerable<Ways>
        {
            private readonly int step;
            public DirectionsGroup(int step) => this.step = step;

            public IEnumerator<Ways> GetEnumerator()
            {
                for (int i = 1; i <= (int)Ways.All; i <<= step)
                    yield return (Ways)i;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public static IEnumerable<Ways> AllDirections { get; } = new DirectionsGroup(1);
        public static IEnumerable<Ways> MainDirections { get; } = new DirectionsGroup(2);

        public static bool Contain(this Ways ways, Ways way) => (ways & way) == way;

        public static Ways NextWay(this Ways way, uint step)
        {
            int val = (int)way << (int)step;
            return (Ways)(val == 256 ? 1 : val);
        }

        public static IList<CellLocation> NextLocations(this CellLocation start, Ways ways, int length = 1)
        {
            List<CellLocation> list = new()
            {
                start
            };

            foreach (Ways way in AllDirections)
            {
                CellLocation last = start;

                if (ways.Contain(way))
                {
                    for (int j = 0; j < length; j++)
                    {
                        last += way;
                        list.Add(last);
                    }
                }
            }

            return list;
        }
        public static IList<CellLocation> NextLocations(this (int, int) start, Ways ways, int length = 1) 
            => NextLocations((CellLocation)start, ways, length);

        public static IList<CellLocation> Region(this CellLocation from, CellLocation to)
        {
            List<CellLocation> list = new();

            CellLocation fixedFrom = (Math.Min(from.Column, to.Column), Math.Min(from.Row, to.Row));
            CellLocation fixedTo = (Math.Max(from.Column, to.Column), Math.Max(from.Row, to.Row));

            for (int col = fixedFrom.Column; col <= fixedTo.Column; col++)
                for (int row = fixedFrom.Row; row <= fixedTo.Row; row++)
                    list.Add((col, row));

            return list;
        }
        public static IList<CellLocation> Region(this (int, int) from, CellLocation to) 
            => Region((CellLocation)from, to);
    }
}
