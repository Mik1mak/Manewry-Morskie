using CellLib;
using System.Collections.Generic;
using System;

namespace ManewryMorskie
{
    public class BoardFactory
    {
        private static CellLocation SimmetricTopLocation(CellLocation referLocation, int mapWidth, int mapHeight)
        {
            return new CellLocation(
                column: mapWidth - 1 - referLocation.Column,
                row: mapHeight - 1 - referLocation.Row);
        }

        public static Board StandardBoard(Player bottomPlayer, Player topPlayer)
        {
            int width = 12;
            int height = 18;

            RectangleCellMap<MapField> map = new(width, height);

            List<CellLocation> bottomTerritory = new();

            bottomTerritory.AddRange((0, 0).GetRegion((4,3)));
            bottomTerritory.AddRange((5, 0).GetRegion((11,1)));
            bottomTerritory.Add((10, 2));
            bottomTerritory.Add((11, 2));

            foreach (CellLocation location in bottomTerritory)
            {
                map[location].Owner = bottomPlayer;
                map[SimmetricTopLocation(location, width, height)].Owner = topPlayer;
            }

            HashSet<CellLocation> internationalWaters = new();
            internationalWaters.AddRange((5, 8).GetNext(Ways.All));
            internationalWaters.AddRange((6, 9).GetNext(Ways.All));
            internationalWaters.Add((6, 6));
            internationalWaters.Add((5, 11));

            return new Board(map, internationalWaters, Array.Empty<(CellLocation, CellLocation)>());
        }
    }
}
