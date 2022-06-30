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

        public Task DisplayOptionsMenu(IList<string> options, CellLocation location);
        public Task MarkCells(IEnumerable<CellLocation> cells, MarkOptions mode);
        public Task ExecuteMove(Move mv);
        public Task DisplayMessage(string message);
    }

}
