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
        private readonly StandardMap map;
        private readonly PlayerManager playerManager;
        private readonly SemaphoreSlim semaphore = new(0, 1);
        private readonly Dictionary<CellLocation, (MoveChecker? moveChecker, IList<ICellAction> actions)> selectable = new();
        private readonly Move result = new()
        {
            SetMines = new HashSet<CellLocation>()
        };

        private CellLocation? selectedUnitLocation;
        private CancellationToken? cancellationToken;
        
        private IUserInterface PlayerUi => playerManager.CurrentPlayer.UserInterface;

        public TurnManager(StandardMap map, PlayerManager playerManager)
        {
            this.map = map;
            this.playerManager = playerManager;
        }

        public async Task<Move> MakeMove(CancellationToken token)
        {
            selectable.Clear();
            cancellationToken = token;

            foreach (CellLocation unitLocation in map.LocationsWithPlayersUnits(playerManager.CurrentPlayer))
                selectable.Add(unitLocation, (new MoveChecker(map, playerManager, unitLocation), new List<ICellAction>()));

            foreach (var item in selectable.Where(kpv => kpv.Value.moveChecker?.UnitIsSelectable() ?? false))
                item.Value.actions.Add(new SelectUnitAction(item.Key, this));

            await PlayerUi.DisplayMessage("Wybierz jednostkę", MessageType.SideMessage);
            await UpdateMarks();


            PlayerUi.ClickedLocation += SelectedLocation;
            await semaphore.WaitAsync(token);
           
            PlayerUi.ChoosenOptionId -= PlayerUi_ChoosenOptionId;
            PlayerUi.ClickedLocation -= SelectedLocation;
            token.ThrowIfCancellationRequested();

            return result;
        }

        private async void SelectedLocation(object sender, CellLocation e)
        {
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

                    PlayerUi.ChoosenOptionId -= PlayerUi_ChoosenOptionId;
                    PlayerUi.ChoosenOptionId += PlayerUi_ChoosenOptionId;
                }
            }
        }

        private async void PlayerUi_ChoosenOptionId(object sender, int e)
        {
            if (selectable[selectedUnitLocation!.Value].actions.Count <= e)
                return;

            await RealiseAction(selectable[selectedUnitLocation!.Value].actions[e]);
            PlayerUi.ChoosenOptionId -= PlayerUi_ChoosenOptionId;
        }

        private async ValueTask RealiseAction(ICellAction action)
        {
            bool finishTurn = await action.Execute(result, cancellationToken!.Value);
            await UpdateMarks();

            if (finishTurn)
            {
                result.SourceUnitDescription = map[result.From].Unit!.ToString();
                semaphore.Release();
            }
        }

        private async ValueTask UpdateMarks()
        {
            await PlayerUi.MarkCells(map.Keys, MarkOptions.None);

            Dictionary<MarkOptions, HashSet<CellLocation>> buffer = new()
            {
                { MarkOptions.Selectable, new() },
                { MarkOptions.Moveable, new() },
                { MarkOptions.Attackable, new() },
                { MarkOptions.Minable, new() },
                { MarkOptions.Disarmable, new() },
            };

            foreach (var (location, actions) in selectable.Select(kpv => (kpv.Key, kpv.Value.actions)))
                foreach (ICellAction option in actions)
                    buffer[option.MarkMode].Add(location);
                    

            foreach (var item in buffer)
                await PlayerUi.MarkCells(item.Value, item.Key);

            if (selectedUnitLocation.HasValue)
                await PlayerUi.MarkCells(selectedUnitLocation.Value, MarkOptions.Selected);
        }
    }
}
