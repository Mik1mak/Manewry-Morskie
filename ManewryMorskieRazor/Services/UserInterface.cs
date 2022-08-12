using CellLib;
using ManewryMorskie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskieRazor
{
    public class UserInterface : IUserInterface, IAsyncDisposable
    {
        private readonly BoardService boardService;
        private readonly DialogService dialogService;
        private readonly Queue<Move> moveBuffer = new();

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
            ChoosenOptionId?.Invoke(this, e);
            ActiveInput = true;
        }

        public void Click(CellLocation location)
        {
            if(ActiveInput)
                ClickedLocation?.Invoke(this, location);
        } 

        public async Task DisplayContextOptionsMenu(CellLocation location, params string[] options)
        {
            ActiveInput = false;
            await boardService[location].DisplayContextMenu(options);
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
            if(moveBuffer.Count != 0)
            {
                moveBuffer.Enqueue(mv);
                return;
            }

            while(true)
            {
                Pawn pawn = await boardService[mv.From].TakeOffPawn();
                await boardService[mv.To].PlacePawn(pawn);

                if (mv.Result != BattleResult.None)
                {
                    BoardCellService bcs = boardService[(mv.Attack ?? mv.Disarm!).Value];
                    await bcs.TogglePawnLabel(bcs.Pawn!.Value.Color);
                    await Task.Delay(2000);
                }

                if (mv.Result.HasFlag(BattleResult.SourceDestroyed))
                    await boardService[mv.To].TakeOffPawn();

                if (mv.Result.HasFlag(BattleResult.TargetDestroyed))
                    await boardService[(mv.Attack ?? mv.Disarm!).Value].TakeOffPawn();

                await Task.Delay(500);

                if (moveBuffer.Any())
                    mv = moveBuffer.Dequeue();
                else
                    break;
            }
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

        public async ValueTask DisposeAsync()
        {
            foreach (CellLocation l in boardService.Keys)
            {
                if (boardService[l].Pawn.HasValue)
                    await TakeOffPawn(l);
                await boardService[l].DisplayContextMenu(Array.Empty<string>());
            }

            await MarkCells(boardService.Keys, MarkOptions.None);
            await DisplayMessage(string.Empty, MessageType.Empty);
        }
    }
}
