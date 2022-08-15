using System;
using System.Linq;
using System.Threading.Tasks;

namespace ManewryMorskie.GameEndManagerComponents
{
    internal class OkretDesantowyReachedEnemyFieldGameEnd : IGameEnd
    {
        private readonly StandardMap map;
        private readonly PlayerManager playersManager;
        private readonly MoveExecutor executor;

        public OkretDesantowyReachedEnemyFieldGameEnd(StandardMap map, PlayerManager playersManager, MoveExecutor executor)
        {
            this.map = map;
            this.playersManager = playersManager;
            this.executor = executor;
        }

        public async Task Handle(Player? winner)
        {
            await playersManager.WriteToPlayers(winner!,
                        msgToCurrent: "Zwycięstwo! Twój okręt desantowy wpłynął do portu przeciwnika!",
                        msgToOthers: "Porażka! Okręt desantowy przeciwnika wpłynął do Twojego portu!");
        }

        public (bool, Player?) IsGameEnded(int currentTurn)
        {
            Player current = playersManager.GetPlayerOfTurn(currentTurn);
            Player enemy = playersManager.GetOpositePlayer(current);

            MapField field = map[executor.LastExecuted!.To];
            if (field.Owner == enemy && field.Unit != null)
                if (field.Unit.GetType() == typeof(OkretDesantowy) && current.Fleet.Units.Contains(field.Unit))
                    return (true, current);

            return (false, null);
        }
    }
}
