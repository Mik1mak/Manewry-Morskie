using System.Threading.Tasks;

namespace ManewryMorskie.GameEndManagerComponents
{
    internal class OkretDesantowyDestroyedGameEnd : IGameEnd
    {
        private readonly PlayerManager playersManager;

        public OkretDesantowyDestroyedGameEnd(PlayerManager playersManager)
        {
            this.playersManager = playersManager;
        }

        public async Task Handle(Player? winner)
        {
            if (winner == null)
                await playersManager.WriteToPlayers("Remis! Okręty desantowe zostały pokonane!");
            else
                await playersManager.WriteToPlayers(winner,
                    msgToCurrent: "Zwycięstwo! Zniszczyłeś okręt desantowy przeciwnika!",
                    msgToOthers: "Porażka! Przeciwnik zniszczył Twój okręt desantowy!");
        }

        public (bool, Player?) IsGameEnded(int currentTurn)
        {
            Player current = playersManager.GetPlayerOfTurn(currentTurn);
            Player enemy = playersManager.GetOpositePlayer(current);

            if (enemy.Fleet.ActiveUnitsCount<OkretDesantowy>() == 0)
            {
                if (current.Fleet.ActiveUnitsCount<OkretDesantowy>() == 0)
                    return (true, null);
                else
                    return (true, current);
            }

            return (false, null);
        }
    }
}
