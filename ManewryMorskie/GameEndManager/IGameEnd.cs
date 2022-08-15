using System.Threading.Tasks;

namespace ManewryMorskie.GameEndManagerComponents
{
    internal interface IGameEnd
    {
        public (bool, Player?) IsGameEnded(int currentTurn);

        public Task Handle(Player? winner);
    }
}
