using CellLib;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie
{
    public class DiasrmAction : AttackAction, ICellAction
    {
        public override string Name => "Rozbrój";

        public override MarkOptions MarkMode => MarkOptions.Disarmable;

        public DiasrmAction(Unit attacker, Unit target, PlayerManager players, MoveChecker checker, CellLocation attackLocation) 
            : base(attacker, target, players, checker, attackLocation)
        {}

        public DiasrmAction(AttackAction attackAction) : base(attackAction)
        {}

        public override async Task<bool> Execute(Move move, CancellationToken token)
        {
            await base.Execute(move, token);

            move.Disarm = move.Attack;
            move.Attack = null;

            return await Task.FromResult(true); 
        }
    }
}
