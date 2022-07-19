using CellLib;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManewryMorskie
{

    public interface IUserInterface
    {
        public event EventHandler<CellLocation>? ClickedLocation;
        public event EventHandler<int>? ChoosenOptionId;

        public Task DisplayOptionsMenu(string title, params string[] options);
        public Task DisplayContextOptionsMenu(CellLocation location, params string[] options);
        public Task MarkCells(IEnumerable<CellLocation> cells, MarkOptions mode);
        public Task ExecuteMove(Move mv);
        public Task DisplayMessage(string message, MessageType msgType = MessageType.Standard);

        public Task TakeOffPawn(CellLocation location);
        public Task PlacePawn(CellLocation location, int playerColor, bool battery = false, string pawnDescription = "");
    }

}
