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
        private IPathFinder? pathFinder;

        public CellLocation From { get; private set; }


        public MoveChecker(StandardMap map, PlayerManager playerMgr, CellLocation source)
        {
            this.map = map;
            this.playerMgr = playerMgr;
            this.From = source;

            pathFinder = new DijkstraPathFinder(map, From);
        }

        public void UpdatePaths()
        {
            if (map[From].Unit == null)
                pathFinder = new DijkstraPathFinder(map, From);
        }

        public IEnumerable<CellLocation> Moveable()
        {
            if (map[From].Unit == null)
                return Array.Empty<CellLocation>();

            return pathFinder!.CellsWhereDistanceFromSourceIsLowerThan(map[From].Unit!.Step + 1);
        }

        public IEnumerable<CellLocation> AttackableOrDisarmable()
        {
            Unit? unit = map[From].Unit;

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
            if (map[From].Unit == null || map[From].Unit!.GetType() != typeof(Tralowiec))
                return Array.Empty<CellLocation>();

            if(playerMgr.CurrentPlayer.Fleet.UsedMines >= Fleet.UnitLimits[typeof(Mina)])
                return Array.Empty<CellLocation>();

            return From.SquereRegion(1);
        }

        public IEnumerable<CellLocation> PathTo(CellLocation target)
        {
            if(pathFinder == null)
                return Array.Empty<CellLocation>();

            return pathFinder.ShortestPathTo(target);
        }

        public bool UnitIsSelectable()
        {
            if (map[From].Unit == null)
                return false;

            if (map[From].Unit!.GetType() == typeof(Mina))
                return false;

            return AttackableOrDisarmable().Any() || Moveable().Any() || Minable().Any();
        }
    }
}
