using CellLib;
using ManewryMorskie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskieRazor
{
    public class BoardService : RectangleCellMap<BoardCellService>
    {
        public BoardService() : base(12, 18) { }
    }

    public class BoardCellService
    {
        public Pawn? Pawn { get; private set; }

        public async Task PlacePawn(Pawn pawn)
        {
            Pawn = pawn;

            if (PawnChanged != null)
                await PawnChanged.Invoke(Pawn);
        }

        public async Task MarkCell(MarkOptions option)
        {
            if (CellMarked != null)
                await CellMarked.Invoke(option);
        }

        public async Task<Pawn> TakeOffPawn()
        {
            if (!Pawn.HasValue)
                throw new InvalidOperationException("There is no pawn on this cell.");

            Pawn result = Pawn.Value;
            Pawn = null;

            if (PawnChanged != null)
                await PawnChanged.Invoke(null);

            return result;
        }

        public event Func<Pawn?, Task>? PawnChanged;
        public event Func<MarkOptions, Task>? CellMarked;
    }
}
