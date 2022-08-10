using ManewryMorskie;
using ManewryMorskie.Network;

namespace ManewryMorskieRazor
{
    public class ManewryMorskieLocalClient : IManewryMorskieClient
    {
        private ManewryMorskieGame game;
        private IUserInterface ui;

        public event Func<string?, Task>? GameClosed;

        public event EventHandler<int> TurnChanged
        {
            add => game.TurnChanged += value;
            remove => game.TurnChanged -= value;
        }

        public ManewryMorskieLocalClient(IUserInterface ui)
        {
            this.ui = ui;

            Player playerOne = new(ui)
            {
                Color = 1,
            };

            Player playerTwo = new(ui)
            {
                Color = 0,
            };

            game = new(playerOne, playerTwo);
        }

        public async Task RunGame(CancellationToken ct = default)
        {
            try
            {
                await game.Start(ct);
            }
            finally
            {
                GameClosed?.Invoke(string.Empty);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if(ui is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
        }
    }
}
