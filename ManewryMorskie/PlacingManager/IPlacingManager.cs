using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie.PlacingManagerComponents
{
    public interface IPlacingManager : IDisposable
    {
        Task PlacePawns(CancellationToken token);
    }
}
