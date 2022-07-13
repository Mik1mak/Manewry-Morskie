using CellLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie
{
    public class SingleTurnManager
    {
        private readonly StandardMap map;
        private readonly PlayerManager playerManager;
        private readonly SemaphoreSlim semaphore = new(0, 1);

        private readonly Dictionary<CellLocation, MoveChecker> moveCheckers = new();

        private IEnumerable<CellLocation>? selectableUnitsLocations;
        private CellLocation? selectedLocation;

        private IEnumerable<CellLocation>? minableLocations;
        private CellLocation? minedLocation;

        private IEnumerable<CellLocation>? attackableLocations;
        private CellLocation? attackedLocation;

        private IEnumerable<CellLocation>? moveableLocations;

        public SingleTurnManager(StandardMap map, PlayerManager playerManager)
        {
            this.map = map;
            this.playerManager = playerManager;
        }

        public async Task MakeTurn(CancellationToken token)
        {
            moveCheckers.Clear();

            foreach (CellLocation unitLocation in map.LocationsWithPlayersUnits(playerManager.CurrentPlayer))
                moveCheckers.Add(unitLocation, new MoveChecker(map, playerManager, unitLocation));

            selectableUnitsLocations = moveCheckers.Where(kpv => kpv.Value.UnitIsSelectable()).Select(kpv => kpv.Key);

            IUserInterface playerUi = playerManager.CurrentPlayer.UserInterface;
            await playerUi.MarkCells(selectableUnitsLocations, MarkOptions.Selectable);
            await playerUi.DisplayMessage("Wybierz jednostkę", MessageType.SideMessage);

            playerUi.ClickedLocation += SelectUnit;
            await semaphore.WaitAsync(token);
            token.ThrowIfCancellationRequested();

            playerUi.ClickedLocation -= SelectUnit;
        }

        private async void SelectUnit(object sender, CellLocation e)
        {
            if (!selectableUnitsLocations.Contains(e))
                return;

            IUserInterface playerUi = playerManager.CurrentPlayer.UserInterface;

            if(selectedLocation.HasValue)
            {
                await playerUi.MarkCells(map.Keys, MarkOptions.None);
                await playerUi.MarkCells(selectableUnitsLocations!, MarkOptions.Selectable);
            }
                    
            selectedLocation = e;
            await playerUi.MarkCells(selectedLocation.Value, MarkOptions.Selected);

            minableLocations = moveCheckers[selectedLocation.Value].Minable();
            attackableLocations = moveCheckers[selectedLocation.Value].AttackableOrDisarmable();
            moveableLocations = moveCheckers[selectedLocation.Value].Moveable();

            await playerUi.MarkCells(moveableLocations, MarkOptions.Moveable);
            await playerUi.MarkCells(attackableLocations, MarkOptions.Attackable);
            await playerUi.MarkCells(minableLocations, MarkOptions.Minable);

            if (minableLocations.Any())
                await playerUi.DisplayMessage("Wybierz ruch lub zaminowanie albo atak lub rozbrojenie", MessageType.SideMessage);
            else
                await playerUi.DisplayMessage("Wybierz ruch albo atak", MessageType.SideMessage);

            playerUi.ClickedLocation -= Action;
            playerUi.ClickedLocation += Action;
        }

        private void Action(object sender, CellLocation e)
        {
            //TODO

            if (moveableLocations.Contains(e))
            {
                if(minableLocations.Contains(e))
                {
                    //do poruszenia się lub zaminowania
                }
                else
                {
                    //tylko do poruszania się
                    //wykonanie ruchu
                }
            }
            else if(attackableLocations.Contains(e))
            {
                //do zaatakowania lub rozminowania
            }
            else if(minableLocations.Contains(e))
            {
                //tylko do zaminowania
            }
        }
    }
}
