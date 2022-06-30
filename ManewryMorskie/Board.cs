using CellLib;
using System.Collections;
using System.Text;
using System.Linq;
using System;

namespace ManewryMorskie
{

    public class Board
    {
        
        private readonly TurnCounter turnCounter;
        protected RectangleCellMap<MapField> map;

        public Board(RectangleCellMap<MapField> map, TurnCounter turnCounter)
        {
            this.map = map;
            this.turnCounter = turnCounter;
        }

        public void Place(Unit unit, CellLocation location)
        {
            map[location].Unit = unit;
            //map[location].Owner = turnCounter.CurrentPlayer;
        }

        public Unit? Take(CellLocation location)
        {
            //map[location].Owner = null;
            return map[location].Unit;
        }
    }
}
