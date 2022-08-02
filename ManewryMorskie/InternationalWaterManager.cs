using System;
using System.Collections.Generic;
using System.Linq;
using CellLib;

namespace ManewryMorskie
{
    public class InternationalWaterManager
    {
        private readonly Dictionary<Unit, int> daysOnInternationalWater = new();
        private readonly RectangleCellMap<MapField> map;

        public IEnumerable<CellLocation> InternationalWaters { get; }
        public int TurnsOnInternationalWaterLimit { get; }

        public event EventHandler<Unit>? InternedUnit;

        public InternationalWaterManager(RectangleCellMap<MapField> map,
            int turnsOnInternationalWaterLimit = 6)
        {
            this.map = map;
            this.TurnsOnInternationalWaterLimit = turnsOnInternationalWaterLimit;
            InternationalWaters = map.Keys.Where(l => map[l].InternationalWater).ToList();
        }

        public void Iterate()
        {
            List<Unit> onInternationalWater = new();

            foreach (CellLocation point in InternationalWaters)
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
                if(daysOnInternationalWater[unit] > TurnsOnInternationalWaterLimit)
                {
                    daysOnInternationalWater.Remove(unit);
                    InternedUnit?.Invoke(this, unit);
                }
                    
        }
    }
}
