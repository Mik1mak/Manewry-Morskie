using CellLib;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie.TurnManagerComponents
{
    internal class MoveAction : ICellAction
    {
        public virtual string Name => "Przemieść się";
        public virtual MarkOptions MarkMode => MarkOptions.Moveable;

        protected MoveChecker MoveChecker { get; private set; }
        protected CellLocation Destination { get; set; }

        public MoveAction(CellLocation destination, MoveChecker checker)
        {
            MoveChecker = checker;
            Destination = destination;
        }

        public async virtual Task<bool> Execute(Move move, CancellationToken token)
        {
            move.To = Destination;
            move.From = MoveChecker.From;
            move.Path = MoveChecker.PathTo(Destination);

            return await Task.FromResult(true);
        }
    }
}
