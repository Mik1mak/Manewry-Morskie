using CellLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie
{

    public class PawnsPlacingManager : IPlacingManager
    {
        private enum State
        {
            BeforePlacing,
            WaitingForChooseMode,
            WaitingForSelect,
            WaitingForChoosePawnsType,
            PawnChoosen,
            PawnsPlaced,
        }

        private readonly RectangleCellMap<MapField> map;
        private readonly PlayerManager players;
        private readonly Player currentPlayer;
        private readonly SemaphoreSlim semaphore = new(0, 1);

        private IEnumerable<CellLocation>? selectable;
        private CellLocation? selected;
        private Dictionary<Type, int> unitsToPlace = new(Fleet.UnitLimits);
        private State state = State.BeforePlacing;

        public PawnsPlacingManager(StandardMap map, PlayerManager players, Player player)
        {
            this.map = map;
            this.players = players;
            currentPlayer = player;
            unitsToPlace[typeof(Mina)] = 0;
        }

        public async Task PlacePawns(CancellationToken token)
        {
            await currentPlayer.UserInterface.DisplayMessage("Rozmieść swoje pionki na planszy", MessageType.SideMessage);
            await currentPlayer.UserInterface.DisplayOptionsMenu("Czy chcesz rozmieścić pionki ręcznie?", "Tak", "Nie");

            IEnumerable<CellLocation> entries = players.TopPlayer == currentPlayer ? 
                StandardMap.DefaultTopEnterences : StandardMap.DefaultBottomEnterences;

            foreach (CellLocation location in entries)
                foreach (Ways way in CellLib.Extensions.HorizontalDirections)
                    await PlaceUnit(location + way, new Bateria(), currentPlayer);

            state = State.WaitingForChooseMode;
            currentPlayer.UserInterface.ChoosenOptionId += ChoosePlacingOption;

            await semaphore.WaitAsync(token);
            token.ThrowIfCancellationRequested();
        }

        private async ValueTask PlaceUnit(CellLocation location, Unit unit, Player player)
        {
            player.Fleet.Set(unit);
            map[location].Unit = unit;
            unitsToPlace[unit.GetType()]--;

            bool isBattery = unit is Bateria;

            await player.UserInterface.PlacePawn(location, player.Color, isBattery, unit.ToString());
            await players.GetOpositePlayer(player).UserInterface.PlacePawn(location, player.Color, isBattery);
        }

        private async void ChoosePlacingOption(object sender, int e)
        {
            currentPlayer.UserInterface.ChoosenOptionId -= ChoosePlacingOption;

            if (e == 0)
            {
                //ręczne

                await currentPlayer.UserInterface.DisplayMessage("Rozmieść swoje pionki na planszy", MessageType.SideMessage);
                selectable = map.Keys.Where(l => map[l].Owner == currentPlayer && map[l].Unit == null);
                await currentPlayer.UserInterface.MarkCells(selectable, MarkOptions.Selectable);

                if(selectable.Any())
                {
                    if(state != State.PawnChoosen)
                        currentPlayer.UserInterface.ClickedLocation += SelectLocation;

                    state = State.WaitingForSelect;
                }
                else
                {
                    state = State.PawnsPlaced;
                    await currentPlayer.UserInterface.MarkCells(map.Keys, MarkOptions.None);
                    semaphore.Release();
                }
            }
            else
            {
                //TODO auto
                semaphore.Release();
            }
        }

        private async void SelectLocation(object sender, CellLocation e)
        {
            if (!selectable.Contains(e))
                return;

            selected = e;
            await currentPlayer.UserInterface.MarkCells(selectable!, MarkOptions.Selectable);
            await currentPlayer.UserInterface.MarkCells(selected!.Value, MarkOptions.Selected);
            await currentPlayer.UserInterface.DisplayMessage("Wybierz jednostkę jaką chcesz umieścić", MessageType.SideMessage);
            await currentPlayer.UserInterface.DisplayContextOptionsMenu(e, unitsToPlace.Select(vp => $"{vp.Key.Name} ({vp.Value})").ToArray());

            if(state != State.WaitingForChoosePawnsType)
            {
                currentPlayer.UserInterface.ChoosenOptionId += ChoosePawnType;
                state = State.WaitingForChoosePawnsType;
            }
        }

        private async void ChoosePawnType(object sender, int e)
        {
            Type chosenUnitType = unitsToPlace.Skip(e+1).First().Key;
            Unit newUnit = (Unit)Activator.CreateInstance(chosenUnitType);
            await PlaceUnit(selected!.Value, newUnit, currentPlayer);

            state = State.PawnChoosen;
            currentPlayer.UserInterface.ChoosenOptionId -= ChoosePawnType;
            currentPlayer.UserInterface.ChoosenOptionId += ChoosePlacingOption;
            ChoosePlacingOption(this, 0);
        }
    }
}
