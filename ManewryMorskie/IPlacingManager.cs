using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie
{
    public interface IPlacingManager
    {
        Task PlacePawns(CancellationToken token);
    }
}
