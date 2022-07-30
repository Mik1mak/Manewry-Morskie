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
        public async Task PlacePawn(int pawnColor, bool isBattery, string label)
        {
            if (PawnPlaced != null)
                await PawnPlaced.Invoke(pawnColor, isBattery, label);
        }

        public async Task MarkCell(MarkOptions option)
        {
            if (CellMarked != null)
                await CellMarked.Invoke(option);
        }

        public async Task TakeOffPawn()
        {
            if (PawnTakenOff != null)
                await PawnTakenOff.Invoke();
        }

        public event Func<int, bool, string, Task>? PawnPlaced;
        public event Func<MarkOptions, Task>? CellMarked;
        public event Func<Task>? PawnTakenOff;
    }
}
