using ManewryMorskie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskieRazor
{
    public class GameService
    {
        public event Func<string, Task>? TurnPause;

        private readonly BoardService boardService;
        private readonly Player playerOne;
        private readonly Player playerTwo;

        private CancellationTokenSource tokenSource = new();
        public bool GameIsOngoing { get; private set; } = false;

        public GameService(UserInterface ui, BoardService boardService)
        {
            playerOne = new(ui)
            {
                Color = 1,
            };

            playerTwo = new(ui)
            {
                Color = 0,
            };

            this.boardService = boardService;
        }

        public async Task RunGame()
        {
            if(GameIsOngoing)
            {
                tokenSource.Cancel();
                await Task.Delay(5);
                tokenSource = new();
            }

            playerOne.Fleet.Clear();
            playerTwo.Fleet.Clear();

            GameIsOngoing = true;
            await using ManewryMorskieGame manewry = new(playerOne, playerTwo) { AsyncGame = false };
            CancellationToken token = tokenSource.Token;
            manewry.TurnChanged += Manewry_TurnChanged;

            try
            {
                Console.WriteLine("Game started");
                await manewry.Start(token);
            }
            catch(OperationCanceledException)
            {
                Console.WriteLine("Game terminated");
            }
            finally
            {
                manewry.TurnChanged -= Manewry_TurnChanged;
                GameIsOngoing = false;
                Console.WriteLine("Game finished");
            }
        }

        private async void Manewry_TurnChanged(object? sender, int e)
        {
            if(TurnPause != null)
                await TurnPause.Invoke(string.Empty);
            
            foreach (BoardCellService bcs in boardService)
                if(bcs.Pawn.HasValue)
                    await bcs.TogglePawnLabel(e);
        }
    }
}
