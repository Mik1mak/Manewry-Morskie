using CellLib;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie
{

    public class PawnsPlacingManager : IPlacingManager, IDisposable
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
        private readonly ILogger? logger;


        private IEnumerable<CellLocation>? selectable;
        private CellLocation? selected;
        private Dictionary<Type, int> unitsToPlace = new(Fleet.UnitLimits);
        private State state = State.BeforePlacing;

        public PawnsPlacingManager(StandardMap map, PlayerManager players, Player player, ILogger? logger = null)
        {
            this.map = map;
            this.players = players;
            this.logger = logger;
            currentPlayer = player;
            unitsToPlace[typeof(Mina)] = 0;
        }

        public async Task PlacePawns(CancellationToken token)
        {
            logger?.LogInformation("Placing pawns started.");
            await currentPlayer.UserInterface.DisplayMessage("Rozmieść swoje pionki na planszy", MessageType.SideMessage);
            await currentPlayer.UserInterface.DisplayOptionsMenu("Czy chcesz rozmieścić pionki ręcznie?", "Tak", "Nie");

            IEnumerable<CellLocation> entries = players.TopPlayer == currentPlayer ? 
                StandardMap.DefaultTopEnterences : StandardMap.DefaultBottomEnterences;

            foreach (CellLocation location in entries)
                foreach (Ways way in CellLib.Extensions.HorizontalDirections)
                    await PlaceUnit(location + way, typeof(Bateria), currentPlayer);

            logger?.LogInformation("Batteries set.");

            state = State.WaitingForChooseMode;
            currentPlayer.UserInterface.ChoosenOptionId += ChoosePlacingOption;

            await semaphore.WaitAsync(token);
        }

        private async ValueTask PlaceUnit(CellLocation location, Type typeOfUnit, Player player)
        {
            Unit unit = (Unit)Activator.CreateInstance(typeOfUnit);

            player.Fleet.Set(unit);
            map[location].Unit = unit;
            unitsToPlace[unit.GetType()]--;

            bool isBattery = unit is Bateria;

            await player.UserInterface.PlacePawn(location, player.Color, isBattery, unit.ToString());

            if(players.GetOpositePlayer(player).UserInterface != player.UserInterface)
                await players.GetOpositePlayer(player).UserInterface.PlacePawn(location, player.Color, isBattery);
        }

        private async void ChoosePlacingOption(object sender, int e)
        {
            currentPlayer.UserInterface.ChoosenOptionId -= ChoosePlacingOption;
            logger?.LogInformation("Pawn placing mode choosen.");

            if (e == 0)
            {
                //ręczne
                logger?.LogInformation("Manual pawn placing.");

                await currentPlayer.UserInterface.DisplayMessage("Rozmieść swoje pionki na planszy", MessageType.SideMessage);

                if(selectable?.Any() ?? false)
                    await currentPlayer.UserInterface.MarkCells(selectable, MarkOptions.None);

                selectable = map.Keys.Where(l => map[l].Owner == currentPlayer && map[l].Unit == null).ToList();
                await currentPlayer.UserInterface.MarkCells(selectable, MarkOptions.Selectable);

                if(unitsToPlace.Any(x => x.Value != 0))
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
                logger?.LogInformation("Auto Placing.");

                while(unitsToPlace.Any(x => x.Value != 0))
                {
                    await PlaceUnit(map.Keys.First(l => map[l].Owner == currentPlayer && map[l].Unit == null),
                        unitsToPlace.First(x => x.Value != 0).Key, currentPlayer);
                }

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
            await currentPlayer.UserInterface.DisplayContextOptionsMenu(e, unitsToPlace.Where(v => v.Value != 0)
                .Select(vp => $"{vp.Key.Name} ({vp.Value})").ToArray());

            if(state != State.WaitingForChoosePawnsType)
            {
                currentPlayer.UserInterface.ChoosenOptionId += ChoosePawnType;
                state = State.WaitingForChoosePawnsType;
            }
        }

        private async void ChoosePawnType(object sender, int e)
        {
            Type chosenUnitType = unitsToPlace.Where(v => v.Value != 0).Skip(e).First().Key;
            
            await PlaceUnit(selected!.Value, chosenUnitType, currentPlayer);

            state = State.PawnChoosen;
            currentPlayer.UserInterface.ChoosenOptionId -= ChoosePawnType;
            currentPlayer.UserInterface.ChoosenOptionId += ChoosePlacingOption;
            ChoosePlacingOption(this, 0);
        }

        public void Dispose()
        {
            currentPlayer.UserInterface.ChoosenOptionId -= ChoosePawnType;
            currentPlayer.UserInterface.ChoosenOptionId -= ChoosePlacingOption;
            currentPlayer.UserInterface.ClickedLocation -= SelectLocation;
        }
    }
}
