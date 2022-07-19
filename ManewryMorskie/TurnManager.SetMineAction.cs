using CellLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie
{
    public partial class TurnManager
    {
        private class SetMineAction : ICellAction
        {
            public string Name => "Ustaw minę";
            public MarkOptions MarkMode => MarkOptions.Minable;

            private readonly CellLocation setMineLocation;
            private readonly TurnManager parent;


            public SetMineAction(CellLocation setMineLocation, TurnManager parent)
            {
                this.setMineLocation = setMineLocation;
                this.parent = parent;
            }

            public async Task<bool> Execute(Move move, CancellationToken token)
            {
                move.SetMines.Add(setMineLocation);
                Unit mine = new Mina();

                parent.playerManager.CurrentPlayer.Fleet.Set(mine);
                parent.map[setMineLocation].Unit = mine;

                foreach (Player player in parent.playerManager)
                    await player.UserInterface.PlacePawn(setMineLocation, player.Color, mine.ToString());

                foreach ((MoveChecker? moveChecker, IList<ICellAction> actions) in parent.selectable.Values)
                {
                    moveChecker!.UpdatePaths();
                    actions.Clear();

                    if (moveChecker!.UnitIsSelectable())
                        actions.Add(new SelectUnitAction(moveChecker.From, parent));
                }

                return await Task.FromResult(false);
            }
        }
    }
    
}
