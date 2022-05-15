using System;
using System.Collections.Generic;
using System.Linq;
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
            int turnsOnInternationalWaterLimit = 3)
        {
            this.map = map;
            this.turnsOnInternationalWaterLimit = turnsOnInternationalWaterLimit;
            internationalWaters = map.Keys.Where(l => map[l].InternationalWater).ToList();
        }

        public void Iterate()
        {
            List<Unit> onInternationalWater = new();

            foreach (CellLocation point in internationalWaters)
            {
                Unit? unit = map[point].Unit;

                if (unit == null)
                    continue;

                onInternationalWater.Add(unit);

                if(!daysOnInternationalWater.TryAdd(unit, 1))
                    daysOnInternationalWater[unit]++;
            }

            foreach (var item in daysOnInternationalWater)
                if (!onInternationalWater.Contains(item.Key))
                    daysOnInternationalWater[item.Key] = 0;

            foreach (Unit unit in onInternationalWater)
                if(daysOnInternationalWater[unit] > turnsOnInternationalWaterLimit)
                {
                    daysOnInternationalWater.Remove(unit);
                    InternedUnit?.Invoke(this, unit);
                }
                    
        }
    }
}
