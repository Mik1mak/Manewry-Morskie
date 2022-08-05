namespace ManewryMorskie.Network
{
    public interface IManewryMorskieClient : IAsyncDisposable
    {
        public event Func<string?, Task>? GameClosed;

        public Task RunGame(CancellationToken ct = default);
    }
}