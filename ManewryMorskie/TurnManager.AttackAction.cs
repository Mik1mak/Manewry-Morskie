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
        public class AttackAction : MoveAction, ICellAction
        {
            private readonly SemaphoreSlim semaphore = new(0, 1);
            private readonly TurnManager parent;
            private readonly Unit attacker;
            private readonly Unit target;

            private CellLocation attackLocation;
            private IEnumerable<CellLocation>? selectableEndLocations;

            public override string Name => "Atakuj";
            public override MarkOptions MarkMode => MarkOptions.Attackable;

            public AttackAction(Unit attacker, Unit target, TurnManager parent, MoveChecker checker, CellLocation attackLocation) : base((-1, -1), checker)
            {
                this.attacker = attacker;
                this.target = target;

                this.attackLocation = attackLocation;
                this.parent = parent;
            }

            protected AttackAction(AttackAction a)
            : this(a.attacker, a.target, a.parent, a.MoveChecker, a.attackLocation)
                    { }

            public override async Task<bool> Execute(Move move, CancellationToken token)
            {
                move.Attack = attackLocation;
                move.TargetUnitDescription = target.ToString();

                selectableEndLocations = MoveChecker.Moveable()
                    .Append(MoveChecker.From)
                    .Intersect(attackLocation.SquereRegion((int)attacker.AttackRange))
                    .Except(parent.internationalWaterManager.InternationalWaters);

                IUserInterface ui = parent.playerManager.CurrentPlayer.UserInterface;
                await ui.MarkCells(parent.map.Keys, MarkOptions.None);
                await ui.MarkCells(attackLocation, MarkOptions.Attacked);
                await ui.DisplayMessage("Wybierz pozycję końcową", MessageType.SideMessage);
                await ui.MarkCells(selectableEndLocations, MarkOptions.Moveable);

                parent.ActionSelectionActive = false;
                ui.ClickedLocation += Ui_ClickedLocation;
                await semaphore.WaitAsync(token);
                ui.ClickedLocation -= Ui_ClickedLocation;
                parent.ActionSelectionActive = true;

                await base.Execute(move, token);

                return await Task.FromResult(true);
            }

            private void Ui_ClickedLocation(object sender, CellLocation e)
            {
                if (!selectableEndLocations.Contains(e))
                    return;

                base.Destination = e;
                semaphore.Release();
            }
        }
    }
}
