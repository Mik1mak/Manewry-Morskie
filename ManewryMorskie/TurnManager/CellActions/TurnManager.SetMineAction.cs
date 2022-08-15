﻿using CellLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie.TurnManagerComponents
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
                Player currentPlayer = parent.playerManager.CurrentPlayer;

                move.SetMines.Add(setMineLocation);
                Unit mine = new Mina();

                currentPlayer.Fleet.Set(mine);
                parent.map[setMineLocation].Unit = mine;

                await currentPlayer.UserInterface.PlacePawn(setMineLocation, currentPlayer.Color, false, mine.ToString());

                if(currentPlayer.UserInterface != parent.playerManager.GetOpositePlayer().UserInterface)
                    await parent.playerManager.GetOpositePlayer().UserInterface.PlacePawn(setMineLocation, currentPlayer.Color);

                foreach ((MoveChecker? moveChecker, IList<ICellAction> actions) in parent.selectable.Values)
                {
                    actions.Clear();

                    if(moveChecker?.UnitIsSelectable() ?? false)
                    {
                        moveChecker.UpdatePaths();

                        if(moveChecker.UnitIsSelectable())
                            actions.Add(new SelectUnitAction(moveChecker.From, parent));
                    }
                }

                parent.selectedUnitLocation = null;

                return await Task.FromResult(false);
            }
        }
    }
    
}
