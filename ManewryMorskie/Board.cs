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
        public bool InternationalWater { get; set; } = false;
        public Ways Barriers { get; set; } = Ways.None;
        public Player? Owner { get; set; }
        public Unit? Unit { get; set; }
    }

    public class Board
    {
        private InternationalWaterManager internationalWaterManager;
        protected RectangleCellMap<MapField> map;

        public Board(RectangleCellMap<MapField> map)
        {
            this.map = map;
            internationalWaterManager = new InternationalWaterManager(map);
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
