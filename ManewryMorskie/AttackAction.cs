using CellLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie
{
    public class AttackAction : MoveAction, ICellAction
    {
        private readonly SemaphoreSlim semaphore = new(0, 1);
        private readonly PlayerManager players;
        private readonly Unit attacker;
        private readonly Unit target;
        
        private CellLocation attackLocation;
        private IEnumerable<CellLocation>? selectableEndLocations;

        public override string Name => "Atakuj";
        public override MarkOptions MarkMode => MarkOptions.Attackable;

        public AttackAction(Unit attacker, Unit target, PlayerManager players, MoveChecker checker, CellLocation attackLocation) : base((-1,-1), checker)
        {
            this.attacker = attacker;
            this.target = target;

            this.attackLocation = attackLocation;
            this.players = players;
        }

        protected AttackAction(AttackAction a) 
            : this(a.attacker, a.target, a.players, a.MoveChecker, a.attackLocation)
        {}

        public override async Task<bool> Execute(Move move, CancellationToken token)
        {
            //Move = move;
            move.Attack = attackLocation;
            move.TargetUnitDescription = target.ToString();

            selectableEndLocations = MoveChecker.Moveable()
                .Intersect(attackLocation.SquereRegion((int)attacker.AttackRange));

            IUserInterface ui = players.CurrentPlayer.UserInterface;
            await ui.MarkCells(attackLocation, MarkOptions.Attacked);
            await ui.DisplayMessage("Wybierz pozycję końcową", MessageType.SideMessage);
            await ui.MarkCells(selectableEndLocations, MarkOptions.Moveable);

            ui.ClickedLocation += Ui_ClickedLocation;
            await semaphore.WaitAsync(token);
            ui.ClickedLocation -= Ui_ClickedLocation;

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
