using CellLib;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;

namespace ManewryMorskie
{
    public class MapField
    {
        public Player? Owner { get; set; }
        public Unit? Unit { get; set; }
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
            barrierManager.AddRange(barriers);
        }

        public void Place(Unit unit, CellLocation location)
        {
            throw new NotImplementedException();
            map[location].Unit = unit;
        }

        public Unit Take(CellLocation location)
        {
            throw new NotImplementedException();
            return map[location].Unit;
        }
    }
}
