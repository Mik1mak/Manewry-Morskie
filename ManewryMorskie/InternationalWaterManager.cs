using System;
using System.Collections.Generic;

namespace ManewryMorskie
{
    public class InternationalWaterManager
    {
        private readonly Dictionary<Unit, int> daysOnInternationalWater = new();
        private readonly MapPoint[] internationalWaters;
        private readonly int turnsOnInternationalWaterLimit;
        private readonly Map map;

        public event EventHandler<Unit>? InternedUnit;

        public InternationalWaterManager(Map map,
            MapPoint[] internationalWaters,
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

            foreach (MapPoint point in internationalWaters)
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
