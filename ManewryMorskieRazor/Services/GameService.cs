using ManewryMorskie.Network;
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
        private readonly UserInterface ui;
        private CancellationTokenSource? tokenSource;

        private IManewryMorskieClient? client;

        public GameService(UserInterface ui, BoardService boardService)
        {
            this.ui = ui;
            this.boardService = boardService;
        }

        public async ValueTask SetUpLocal()
        {
            await Clean();

            ManewryMorskieLocalClient localClient = new(ui);
            localClient.TurnChanged += Manewry_TurnChanged;
            client = localClient;
        }

        private async ValueTask Clean()
        {
            if(client is ManewryMorskieLocalClient localClient)
            {
                localClient.TurnChanged -= Manewry_TurnChanged;
                await client.DisposeAsync();
            }
        }

        public async Task RunGame()
        {
            if (client != null)
            {
                tokenSource?.Cancel();
                await Task.Delay(5);
                tokenSource = new();

                await client.RunGame(tokenSource.Token);
                await client.DisposeAsync();
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
