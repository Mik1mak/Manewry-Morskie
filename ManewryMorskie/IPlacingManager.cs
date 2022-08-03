using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie
{
    public interface IPlacingManager : IDisposable
    {
        Task PlacePawns(CancellationToken token);
    }
}
