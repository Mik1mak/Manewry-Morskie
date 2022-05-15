using System;
using System.Collections.Generic;
using System.Text;
using CellLib;

namespace ManewryMorskie
{
    public static class Extensions
    {
        public static RectangleCellMap<MapField> MarkInternationalWaters(
            this RectangleCellMap<MapField> map,
            IEnumerable<CellLocation> internationalWaters,
            bool isInternational = true)
        {
            foreach (CellLocation location in internationalWaters)
                map[location].InternationalWater = isInternational;

            return map;
        }

        public static RectangleCellMap<MapField> MarkInternationalWaters(
            this RectangleCellMap<MapField> map,
            params CellLocation[] internationalWaters)
        {
            return map.MarkInternationalWaters(internationalWaters);
        }
    }
}
