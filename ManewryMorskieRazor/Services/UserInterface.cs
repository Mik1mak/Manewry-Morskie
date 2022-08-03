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

        public bool ActiveInput { get; set; } = true;

        public event EventHandler<CellLocation>? ClickedLocation;
        public event EventHandler<int>? ChoosenOptionId;


        public UserInterface(BoardService board, DialogService dialogs)
        {
            (boardService, dialogService) = (board, dialogs);
            dialogService.OptionChoosed += DialogService_OptionChoosed;
        }

        private void DialogService_OptionChoosed(object? sender, int e)
        {
            if(ActiveInput)
                ChoosenOptionId?.Invoke(this, e);
        }


        public void Click(CellLocation location)
        {
            if(ActiveInput)
                ClickedLocation?.Invoke(this, location);
        } 

        public async Task DisplayContextOptionsMenu(CellLocation location, params string[] options)
        {
            await dialogService.DisplayOptions("Wybierz działanie", options);
        }

        public async Task DisplayMessage(string message, MessageType msgType = MessageType.Standard)
        {
            await dialogService.DisplayMessage(message, msgType);
        }

        public async Task DisplayOptionsMenu(string title, params string[] options)
        {
            await dialogService.DisplayOptions(title, options);
        }

        public async Task ExecuteMove(Move mv)
        {
            Pawn pawn = await boardService[mv.From].TakeOffPawn();

            if (!mv.Result.HasFlag(BattleResult.SourceDestroyed))
                await boardService[mv.To].PlacePawn(pawn);

            if (mv.Result.HasFlag(BattleResult.TargetDestroyed))
                await boardService[mv.Attack.HasValue ? mv.Attack.Value : mv.Disarm!.Value].TakeOffPawn();

            //TODO animacje
        }

        public async Task MarkCells(IEnumerable<CellLocation> cells, MarkOptions mode)
        {
            foreach (CellLocation l in cells)
                await boardService[l].MarkCell(mode);
        }

        public async Task PlacePawn(CellLocation location, int playerColor, bool battery = false, string pawnDescription = "")
        {
            await boardService[location].PlacePawn(new()
            {
                Color = playerColor,
                IsBattery = battery,
                Label = pawnDescription,
            });
        }

        public async Task TakeOffPawn(CellLocation location)
        {
            await boardService[location].TakeOffPawn();
        }
    }
}
