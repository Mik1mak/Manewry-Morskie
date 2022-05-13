using CellLib;
using System.Collections;
using System.Collections.Generic;

namespace ManewryMorskie
{
    public class BarrierManager : IEnumerable<(CellLocation, CellLocation)>
    {
        private readonly List<(CellLocation, CellLocation)> barriers = new();
        public void Add((CellLocation, CellLocation) barrier) => barriers.Add(barrier);
        public void AddRange(IEnumerable<(CellLocation, CellLocation)> values) => barriers.AddRange(values);

        public bool IsBarrierBetween(CellLocation a, CellLocation b)
        {
            foreach (var barrier in barriers)
                if(barrier == (a, b) || barrier == (b, a))
                    return true;
            return false;
        }

        public Ways GetBarriers(CellLocation location)
        {
            Ways result = Ways.None;

            foreach (Ways way in Extensions.MainDirections)
                if (IsBarrierBetween(location, location + way))
                    result |= way;

            return result;
        }

        public IEnumerator<(CellLocation, CellLocation)> GetEnumerator() => barriers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => barriers.GetEnumerator();
    }
}
