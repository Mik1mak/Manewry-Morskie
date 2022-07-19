using CellLib;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie
{
    public partial class TurnManager
    {
        private class SelectUnitAction : ICellAction
        {
            public string Name => "Wybierz jednostkę";
            public MarkOptions MarkMode => MarkOptions.Selectable;

            private readonly CellLocation locationToSelect;
            private readonly TurnManager parent;

            public SelectUnitAction(CellLocation location, TurnManager parent)
            {
                locationToSelect = location;
                this.parent = parent;
            }

            public async Task<bool> Execute(Move move, CancellationToken token)
            {
                ClearNonSelectableUnitActions();
                parent.selectedUnitLocation = locationToSelect;

                (MoveChecker? moveChecker, _)= parent.selectable[locationToSelect];

                foreach (CellLocation moveableLocation in moveChecker!.Moveable())
                    parent.selectable[moveableLocation].actions.Add(new MoveAction(moveableLocation, moveChecker));

                foreach (CellLocation minableLocation in moveChecker!.Minable())
                    parent.selectable[minableLocation].actions.Add(new SetMineAction(minableLocation, parent));

                foreach (CellLocation attackableOrDisarmableLcation in moveChecker!.AttackableOrDisarmable())
                {
                    AttackAction atkAction = new(
                        parent.map[parent.selectedUnitLocation!.Value].Unit!,
                        parent.map[attackableOrDisarmableLcation].Unit!,
                        parent.playerManager,
                        parent.selectable[attackableOrDisarmableLcation].moveChecker!,
                        attackableOrDisarmableLcation);

                    parent.selectable[attackableOrDisarmableLcation].actions.Add(atkAction);

                    if (parent.map[locationToSelect].Unit?.GetType() == typeof(Tralowiec))
                        parent.selectable[attackableOrDisarmableLcation].actions.Add(new DiasrmAction(atkAction));
                }

                await parent.PlayerUi.DisplayMessage("Wybierz działanie", MessageType.SideMessage);

                return await Task.FromResult(false);
            }

            private void ClearNonSelectableUnitActions()
            {
                foreach (var item in parent.selectable)
                {
                    ICellAction selectAction = item.Value.actions.First(o => o.GetType() == typeof(SelectUnitAction));
                    item.Value.actions.Clear();
                    item.Value.actions.Add(selectAction);
                }
            }
        }
    }
}
