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

        public bool ActiveClickInput { get; set; } = true;

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
            ActiveClickInput = true;
        }

        public void Click(CellLocation location)
        {
            if(ActiveClickInput)
                ClickedLocation?.Invoke(this, location);
        } 

        public async Task DisplayContextOptionsMenu(CellLocation location, params string[] options)
        {
            ActiveClickInput = false;
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
            ActiveClickInput = false;

            if(moveBuffer.Count != 0)
            {
                moveBuffer.Enqueue(mv);
                return;
            }

            while(true)
            {
                Pawn pawn = await boardService[mv.From].TakeOffPawn();
                BoardCellService toCell = boardService[mv.To];

                foreach (CellLocation l in mv.Path)
                {
                    await boardService[l].PlacePawn(pawn);
                    await Task.Delay(250);
                    await boardService[l].TakeOffPawn();
                }

                await toCell.PlacePawn(pawn);

                if (mv.Result != BattleResult.None)
                {
                    await toCell.PlacePawn(pawn.Copy(mv.SourceUnitDescription!));

                    BoardCellService targetCell = boardService[(mv.Attack ?? mv.Disarm!).Value];
                    await targetCell.PlacePawn(targetCell.Pawn!.Value.Copy(mv.TargetUnitDescription!));
                    await Task.Delay(2300);

                    int toPawnColor = toCell.Pawn!.Value.Color;

                    if (mv.Result.HasFlag(BattleResult.SourceDestroyed))
                        await toCell.TakeOffPawn();
                    else if (!mv.IsMyMove)
                        await toCell.TogglePawnLabel(toPawnColor);

                    if (mv.Result.HasFlag(BattleResult.TargetDestroyed))
                        await targetCell.TakeOffPawn();
                    else if(mv.IsMyMove)
                        await targetCell.TogglePawnLabel(toPawnColor);
                }

                await Task.Delay(200);

                if (moveBuffer.Any())
                    mv = moveBuffer.Dequeue();
                else
                    break;
            }

            ActiveClickInput = true;
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
                LabelIsHidden = string.IsNullOrEmpty(pawnDescription),
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
