using CellLib;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace ManewryMorskie.PlacingManagerComponents
{

    public class ManualPlacingManager : PlacingMangerBase, IPlacingManager, IDisposable
    {
        private readonly Dictionary<Type, string> unitsLabels = new();
        private readonly Player currentPlayer;

        private IUserInterface Ui => currentPlayer.UserInterface;

        public ManualPlacingManager(Dictionary<Type, int> unitsToPlace, RectangleCellMap<MapField> map,
            PlayerManager players, Player currentPlayer)
            : base(unitsToPlace, map, players)
        {
            this.currentPlayer = currentPlayer;

            foreach (Type unitType in unitsToPlace.Keys)
                unitsLabels.Add(unitType, unitType.Name);
            unitsLabels[typeof(Krazownik)] = "Krążownik";
            unitsLabels[typeof(OkretDesantowy)] = "Okręt Desantowy";
            unitsLabels[typeof(OkretPodwodny)] = "Okręt Podwodny";
            unitsLabels[typeof(OkretRakietowy)] = "Okręt Rakietowy";
            unitsLabels[typeof(Tralowiec)] = "Trałowiec";
        }

        public async Task PlacePawns(CancellationToken token)
        {
            await PlaceDefaultBatteries(currentPlayer);

            LocationSelectionHandler selectionHandler = new(Ui);
            OptionsHandler optionsHandler = new(Ui);
            List<CellLocation>? selectable = default;

            selectable = map.Keys.Where(l => map[l].Owner == currentPlayer && map[l].Unit == null).ToList();
            await Ui.MarkCells(selectable, MarkOptions.Selectable);

            do
            {
                await Ui.DisplayMessage("Rozmieść swoje pionki na planszy", MessageType.SideMessage);

                CellLocation selected = await selectionHandler.WaitForCorectSelection(selectable, token);
                await Ui.MarkCells(selected, MarkOptions.Selected);
                await Ui.DisplayMessage("Wybierz jednostkę jaką chcesz umieścić", MessageType.SideMessage);

                Type? chosenUnitType = await optionsHandler.ChooseOption(
                    options: unitsToPlace.Where(v => v.Value != 0)
                        .Select(vp => new KeyValuePair<string, Type>($"{unitsLabels[vp.Key]} ({vp.Value})", vp.Key))
                        .ToList(),
                     context: selected,
                     token: token);

                await PlaceUnit(selected, chosenUnitType!, currentPlayer);
                selectable.Remove(selected);
                await currentPlayer.UserInterface.MarkCells(selected, MarkOptions.None);
            } while (unitsToPlace.Any(x => x.Value != 0));

            await currentPlayer.UserInterface.MarkCells(map.Keys, MarkOptions.None);
            await currentPlayer.UserInterface.DisplayMessage("Zaczekaj aż przeciwnik ustawi pionki", MessageType.SideMessage);
        }

        public void Dispose() { }
    }

}
