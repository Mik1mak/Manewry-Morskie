using CellLib;
using System.Collections.Generic;

namespace ManewryMorskie
{
    public class BarrierManager
    {
        public List<(CellLocation, CellLocation)> Barriers { get; private set; } = new();

        public bool IsBarrierBetween(CellLocation a, CellLocation b)
        {
            foreach (var barrier in Barriers)
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
    }
}
