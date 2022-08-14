using CellLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Selectable = System.Collections.Generic.Dictionary<CellLib.CellLocation, (ManewryMorskie.MoveChecker? moveChecker, System.Collections.Generic.IList<ManewryMorskie.ICellAction> actions)>;

namespace ManewryMorskie
{

    public partial class TurnManager : IDisposable
    {
        private readonly StandardMap map;
        private readonly PlayerManager playerManager;
        private readonly InternationalWaterManager internationalWaterManager;
        private readonly SemaphoreSlim semaphore = new(0, 1);
        private readonly Selectable selectable = new();
        private readonly CellMarker marker;
        private readonly Move result = new();

        private CellLocation? selectedUnitLocation;
        private CancellationToken? cancellationToken;

        public bool ActionSelectionActive { get; set; } = true;

        private IUserInterface PlayerUi => playerManager.CurrentPlayer.UserInterface;

        public TurnManager(StandardMap map, PlayerManager playerManager, InternationalWaterManager internationalWaterManager)
        {
            this.map = map;
            this.playerManager = playerManager;
            this.internationalWaterManager = internationalWaterManager;
            this.marker = new CellMarker(this);
        }

        public async Task<Move> MakeMove(CancellationToken token)
        {
            selectable.Clear();
            result.Clear();
            cancellationToken = token;
            
            foreach (CellLocation unitLocation in map.LocationsWithPlayersUnits(playerManager.CurrentPlayer))
                selectable.Add(unitLocation, 
                    (new MoveChecker(map, playerManager, unitLocation, internationalWaterManager),
                    new List<ICellAction>()));

            foreach (var item in selectable.Where(kpv => kpv.Value.moveChecker?.UnitIsSelectable() ?? false))
                item.Value.actions.Add(new SelectUnitAction(item.Key, this));

            await PlayerUi.DisplayMessage("Wybierz jednostkę", MessageType.SideMessage);
            await marker.UpdateMarks();

            PlayerUi.ClickedLocation += SelectedLocation;

            await semaphore.WaitAsync(token);
            token.ThrowIfCancellationRequested();

            marker.LastMove = new(result);
            await marker.ClearAndMarkLastMove(playerManager.UniqueInferfaces);

            await PlayerUi.DisplayMessage("Poczekaj aż przeciwnik wykona ruch", MessageType.SideMessage);
            return result;
        }

        private async void SelectedLocation(object sender, CellLocation e)
        {
            if (!ActionSelectionActive)
                return;

            if (selectable.TryGetValue(e, out var value))
            {
                selectedUnitLocation = e;

                if (value.actions.Count == 1)
                {
                    await RealiseAction(value.actions.First());
                }
                else if (value.actions.Count > 1)
                {
                    //wyświetl listę i pozwól wybrać
                    await PlayerUi.DisplayContextOptionsMenu(e, value.actions.Select(o => o.Name).ToArray());

                    PlayerUi.ChoosenOptionId -= ChooseOption;
                    PlayerUi.ChoosenOptionId += ChooseOption;
                }
            }
        }

        private async void ChooseOption(object sender, int e)
        {
            if (selectable[selectedUnitLocation!.Value].actions.Count <= e)
                return;

            PlayerUi.ChoosenOptionId -= ChooseOption;
            await RealiseAction(selectable[selectedUnitLocation!.Value].actions[e]);
        }

        private async ValueTask RealiseAction(ICellAction action)
        {
            bool finishTurn = await action.Execute(result!, cancellationToken!.Value);
            await marker.UpdateMarks();

            if (finishTurn)
            {
                PlayerUi.ClickedLocation -= SelectedLocation;
                PlayerUi.ChoosenOptionId -= ChooseOption;
                selectedUnitLocation = null;
                result!.SourceUnitDescription = map[result.From].Unit!.ToString();
                semaphore.Release();
            }
        }

        public void Dispose()
        {
            PlayerUi.ChoosenOptionId -= ChooseOption;
            PlayerUi.ClickedLocation -= SelectedLocation;
        }
    }
}
