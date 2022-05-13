using System;
using System.Collections.Generic;
using CellLib;

namespace ManewryMorskie
{
    public class InternationalWaterManager
    {
        private readonly Dictionary<Unit, int> daysOnInternationalWater = new();
        private readonly IEnumerable<CellLocation> internationalWaters;
        private readonly int turnsOnInternationalWaterLimit;
        private readonly RectangleCellMap<MapField> map;

        public event EventHandler<Unit>? InternedUnit;

        public InternationalWaterManager(RectangleCellMap<MapField> map,
            IEnumerable<CellLocation> internationalWaters,
            int turnsOnInternationalWaterLimit = 3)
        {
            this.map = map;
            this.internationalWaters = internationalWaters;
            this.turnsOnInternationalWaterLimit = turnsOnInternationalWaterLimit;
        }

        public void Track(IEnumerable<Unit> units)
        {
            foreach (Unit unit in units)
                daysOnInternationalWater[unit] = 0;
        }

        public void Track(params Unit[] units) => Track(units);
        public void Untrack(Unit unit) => daysOnInternationalWater.Remove(unit);

        public void Iterate()
        {
            List<Unit> onInternationalWater = new();

            foreach (CellLocation point in internationalWaters)
            {
                Unit? unit = map[point].Unit;

                if (unit == null)
                    continue;

                onInternationalWater.Add(unit);
                daysOnInternationalWater[unit]++;
            }

            foreach (Unit unit in onInternationalWater)
            {
                if(daysOnInternationalWater[unit] > turnsOnInternationalWaterLimit)
                {
                    Untrack(unit);
                    InternedUnit?.Invoke(this, unit);
                }
            }
        }
    }
}
