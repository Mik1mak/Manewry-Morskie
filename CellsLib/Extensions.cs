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

        public static bool Contain(this Ways ways, Ways way) => (ways & way) != Ways.None;

        public static Ways NextWay(this Ways way, int step)
        {
            int val = (int)way << step;
            return (Ways)(val == 256 ? 1 : val);
        }

        public static IList<CellLocation> GetNext(this CellLocation start, Ways ways, int length = 1)
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

        public static void AddRange<T>(this ICollection<T> target, ICollection<T> source)
        {
            foreach (T item in source)
                target.Add(item);
        }
    }
}
