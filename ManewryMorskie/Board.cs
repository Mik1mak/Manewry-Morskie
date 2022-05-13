using CellLib;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;

namespace ManewryMorskie
{
    public abstract class Player
    {
        public string Name { get; set; } = nameof(Player);
    }

    public class MapField
    {
        public Player? Owner { get; set; }
        public Unit? Unit { get; set; }
    }

    public class BoardFactory
    {
        private static CellLocation SimmetricTopLocation(CellLocation referLocation, int mapWidth, int mapHeight)
        {
            return new CellLocation(
                column: mapWidth - 1 - referLocation.Column,
                row: mapHeight - 1 - referLocation.Row);
        }

        public static Board StandardBoard()
        {
            int width = 12;
            int height = 18;

            RectangleCellMap<MapField> map = new(width, height);

            HashSet<CellLocation> internationalWaters = new();
            internationalWaters.AddRange(new CellLocation(5, 8).GetNext(Ways.All));
            internationalWaters.AddRange(new CellLocation(6, 9).GetNext(Ways.All));
            internationalWaters.Add(new CellLocation(6, 6));
            internationalWaters.Add(new CellLocation(5, 11));

            return new Board(map, internationalWaters, Array.Empty<(CellLocation, CellLocation)>());
        }
    }

    public class Board
    {
        private InternationalWaterManager internationalWaterManager;
        private BarrierManager barrierManager;
        protected RectangleCellMap<MapField> map;

        public Board(RectangleCellMap<MapField> map, 
            IEnumerable<CellLocation> internationalWaters, 
            IEnumerable<(CellLocation, CellLocation)> barriers)
        {
            this.map = map;
            internationalWaterManager = new InternationalWaterManager(map, internationalWaters);
            barrierManager = new BarrierManager();
            barrierManager.Barriers.AddRange(barriers);
        }

        public void Place(Unit unit, CellLocation location)
        {
            internationalWaterManager.Track(unit);
            map[location].Unit = unit;
        }

        public Unit Take(CellLocation location)
        {
            internationalWaterManager.Untrack(map[location].Unit);
            return map[location].Unit;
        }
    }
}
