using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie
{
    public interface IGame
    {
        public Task Start(CancellationToken token);
        public Task PauseOrResume();
        public Task Reset();
    }
}
