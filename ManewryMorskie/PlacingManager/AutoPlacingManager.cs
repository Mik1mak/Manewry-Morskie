using CellLib;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace ManewryMorskie.PlacingManagerComponents
{
    public class AutoPlacingManager : PlacingMangerBase, IPlacingManager, IDisposable
    {
        private readonly Player currentPlayer;

        public AutoPlacingManager(Dictionary<Type, int> unitsToPlace, RectangleCellMap<MapField> map, PlayerManager players,
            Player currentPlayer)
            : base(unitsToPlace, map, players)
        {
            this.currentPlayer = currentPlayer;
        }

        public async Task PlacePawns(CancellationToken token)
        {
            while (unitsToPlace.Any(x => x.Value != 0))
            {
                await PlaceUnit(map.Keys.First(l => map[l].Owner == currentPlayer && map[l].Unit == null),
                    unitsToPlace.First(x => x.Value != 0).Key, currentPlayer);
            }
        }

        public void Dispose() { }
    }

}
