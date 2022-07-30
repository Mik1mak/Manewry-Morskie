using CellLib;
using ManewryMorskie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskieRazor
{
    public class UserInterface : IUserInterface
    {
        private readonly BoardService boardService;
        private readonly DialogService dialogService;

        public UserInterface(BoardService board, DialogService dialogs)
        {
            (boardService, dialogService) = (board, dialogs);

            dialogService.OptionChoosed += DialogService_OptionChoosed;
        }

        private void DialogService_OptionChoosed(object? sender, int e)
        {
            ChoosenOptionId?.Invoke(this, e);
        }

        public event EventHandler<CellLocation>? ClickedLocation;
        public event EventHandler<int>? ChoosenOptionId;

        public void Click(CellLocation location) => ClickedLocation?.Invoke(this, location);

        public async Task DisplayContextOptionsMenu(CellLocation location, params string[] options)
        {
            await dialogService.DisplayOptions("Wybierz se coś", options);
        }

        public async Task DisplayMessage(string message, MessageType msgType = MessageType.Standard)
        {
            await dialogService.DisplayMessage(message, msgType);
        }

        public async Task DisplayOptionsMenu(string title, params string[] options)
        {
            await dialogService.DisplayOptions(title, options);
        }

        public Task ExecuteMove(Move mv)
        {
            throw new NotImplementedException();
        }

        public async Task MarkCells(IEnumerable<CellLocation> cells, MarkOptions mode)
        {
            foreach (CellLocation l in cells)
                await boardService[l].MarkCell(mode);
        }

        public async Task PlacePawn(CellLocation location, int playerColor, bool battery = false, string pawnDescription = "")
        {
            await boardService[location].PlacePawn(playerColor, battery, pawnDescription);
        }

        public async Task TakeOffPawn(CellLocation location)
        {
            await boardService[location].TakeOffPawn();
        }
    }
}
