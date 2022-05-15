using CellLib;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ManewryMorskie
{
    public class MapSetups
    {
        public static RectangleCellMap<MapField> Standard(Player bottomPlayer, Player topPlayer)
        {
            int width = 12;
            int height = 18;

            RectangleCellMap<MapField> map = new(width, height);

            BarrierBuilder barierBuilder = new(map);
            barierBuilder
                .AddRange((0, 3).NextLocations(Ways.Right).Select(l => (l, l + Ways.Top)))
                .AddRange((3, 3).NextLocations(Ways.Right).Select(l => (l, l + Ways.Top)))
                .AddRange((5, 1).NextLocations(Ways.Right, 3).Select(l => (l, l + Ways.Top)))
                .AddRange((10, 2).NextLocations(Ways.Right).Select(l => (l, l + Ways.Top)))
                .AddRange((1, 3).NextLocations(Ways.Right).Select(l => (l, l + Ways.Right)))
                .AddRange((4, 3).NextLocations(Ways.Bottom).Select(l => (l, l + Ways.Right)))
                .AddRange((10, 2).NextLocations(Ways.Bottom).Select(l => (l, l + Ways.Left)))
                .Add(((8, 1), (9, 1)))
                .AddSimmetricBarriers()
                .BuildBarriers();

            List<CellLocation> bottomTerritory = new();
            bottomTerritory.AddRange((0, 0).Region((4, 3)));
            bottomTerritory.AddRange((5, 0).Region((11, 1)));
            bottomTerritory.Add((10, 2));
            bottomTerritory.Add((11, 2));

            foreach (CellLocation location in bottomTerritory)
            {
                map[location].Owner = bottomPlayer;
                map.MiddleSimmetricElement(location).Owner = topPlayer;
            }

            map.MarkInternationalWaters((5, 8).NextLocations(Ways.All))
                .MarkInternationalWaters((6, 9).NextLocations(Ways.All))
                .MarkInternationalWaters((6, 6), (5, 11));

            return map;
        }
    }
}
