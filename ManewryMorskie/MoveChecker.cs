using CellLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManewryMorskie
{
    public class MoveChecker
    {
        private StandardMap map;
        private PlayerManager playerMgr;
        private CellLocation source;
        private IPathFinder? pathFinder;

        public MoveChecker(StandardMap map, PlayerManager playerMgr, CellLocation source)
        {
            this.map = map;
            this.playerMgr = playerMgr;
            this.source = source;

            if (map[source].Unit == null)
                pathFinder = new DijkstraPathFinder(map, source);
        }

        public IEnumerable<CellLocation> Moveable()
        {
            if (map[source].Unit == null)
                return Array.Empty<CellLocation>();

            return pathFinder!.CellsWhereDistanceFromSourceIsLowerThan(map[source].Unit!.Step + 1);
        }

        public IEnumerable<CellLocation> AttackableOrDisarmable()
        {
            Unit? unit = map[source].Unit;

            if (unit == null)
                return Array.Empty<CellLocation>();

            IEnumerable<CellLocation> moveable = Moveable();

            //lokacje pól zajętych przez przeciwnika w zasięgu ruchu i ataku jednostki
            return moveable
                .SelectMany(l => l.SquereRegion((int)unit.AttackRange))
                .Intersect(map.LocationsWithPlayersUnits(playerMgr.GetOpositePlayer()));
        }

        public IEnumerable<CellLocation> Minable()
        {
            if (map[source].Unit == null || map[source].Unit!.GetType() != typeof(Tralowiec))
                return Array.Empty<CellLocation>();

            if(playerMgr.CurrentPlayer.Fleet.UsedMines >= Fleet.UnitLimits[typeof(Mina)])
                return Array.Empty<CellLocation>();

            return pathFinder!.CellsWhereDistanceFromSourceIsLowerThan(map[source].Unit!.Step + 2);
        }

        public IEnumerable<CellLocation> PathTo(CellLocation target)
        {
            if(pathFinder == null)
                return Array.Empty<CellLocation>();

            return pathFinder.ShortestPathTo(target);
        }

        public bool UnitIsSelectable()
        {
            if (map[source].Unit == null)
                return false;

            return AttackableOrDisarmable().Count() > 0 || Moveable().Count() > 0 || Minable().Count() > 0;
        }
    }
}
