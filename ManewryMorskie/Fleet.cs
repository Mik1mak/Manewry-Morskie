using System.Collections.Generic;
using System.Linq;

namespace ManewryMorskie
{
    public class Fleet
    {
        public HashSet<Unit> Units { get; set; } = new HashSet<Unit>();
        public HashSet<Unit> DestroyedUnits { get; set; } = new HashSet<Unit>();

        public int UnitsCount<T>() where T : Unit
        {
            return Units.Count(u => u.GetType() == typeof(T));
        }

        public int DestroyedUnitsCount<T>() where T : Unit
        {
            return DestroyedUnits.Count(u => u.GetType() == typeof(T));
        }

        public bool ContainUnit(Unit unit)
        {
            if (Units.Contains(unit))
                return true;

            return DestroyedUnits.Contains(unit);
        }

        public void Destroy(Unit unit)
        {
            Units.Remove(unit);
            DestroyedUnits.Add(unit);
        }
    }
}
