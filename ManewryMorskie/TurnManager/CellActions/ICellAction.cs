using System.Threading;
using System.Threading.Tasks;

namespace ManewryMorskie.TurnManagerComponents
{
    public interface ICellAction
    {
        public string Name { get; }
        public MarkOptions MarkMode { get; }


        /// <summary>
        /// returns true if turn should ends after execution
        /// </summary>
        /// <param name="move"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<bool> Execute(Move move, CancellationToken token);
    }
}
