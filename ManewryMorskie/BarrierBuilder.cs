using CellLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ManewryMorskie
{
    public class BarrierBuilder : IEnumerable<(CellLocation, CellLocation)>
    {
        private RectangleCellMap<MapField> map;

        public BarrierBuilder(RectangleCellMap<MapField> map)
        {
            this.map = map;
        }

        private readonly List<(CellLocation, CellLocation)> barriers = new();
        public BarrierBuilder Add((CellLocation, CellLocation) barrier)
        {
            barriers.Add(barrier);
            return this;
        }
        public BarrierBuilder AddRange(IEnumerable<(CellLocation, CellLocation)> values)
        {
            barriers.AddRange(values);
            return this;
        }

        private CellLocation SimmetricLocation(CellLocation location)
        {
            var result = new CellLocation(
                column: map.Width - 1 - location.Column,
                row: map.Height - 1 - location.Row);
            return result;
        }

        public BarrierBuilder AddSimmetricBarriers()
        {
            var simmetric = barriers
                .Select(t => (SimmetricLocation(t.Item1), SimmetricLocation(t.Item2)))
                .ToList();

            barriers.AddRange(simmetric);

            return this;
        }

        private bool IsBarrierBetween(CellLocation a, CellLocation b)
        {
            foreach (var barrier in barriers)
                if(barrier == (a, b) || barrier == (b, a))
                    return true;
            return false;
        }

        public Ways GetBarriers(CellLocation location)
        {
            Ways result = Ways.None;

            foreach (Ways way in CellLib.Extensions.MainDirections)
            {
                if (IsBarrierBetween(location, location + way))
                {
                    result |= way;

                    CellLocation neighbourCell = location + way.NextWay(2);

                    if (IsBarrierBetween(location, neighbourCell)
                        || IsBarrierBetween(neighbourCell, neighbourCell + way))
                        result |= way.NextWay(1);

                    neighbourCell = location + way.NextWay(6);

                    if (IsBarrierBetween(neighbourCell, neighbourCell + way))
                        result |= way.NextWay(7);
                }
            }


            return result;
        }

        private void SetBorder(IEnumerable<CellLocation> where, Ways border)
        {
            foreach (CellLocation location in where)
                Add((location, location + border));
        }

        public void BuildBarriers()
        {
            int maxX = map.Width - 1;
            int maxY = map.Height - 1;

            SetBorder((0, 0).NextLocations(Ways.Right, maxX), Ways.Bottom);
            SetBorder((0, 0).NextLocations(Ways.Top, maxY), Ways.Left);
            SetBorder((maxX, maxY).NextLocations(Ways.Left, maxX), Ways.Top);
            SetBorder((maxX, maxY).NextLocations(Ways.Bottom, maxY), Ways.Right);

            foreach (CellLocation location in map.Keys)
                map[location].Barriers |= GetBarriers(location);
        }

        public IEnumerator<(CellLocation, CellLocation)> GetEnumerator() => barriers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => barriers.GetEnumerator();
    }
}
