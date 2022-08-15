using System.Threading.Tasks;

namespace ManewryMorskie.GameEndManagerComponents
{
    internal class DestroyedOkretyRakietoweGameEnd : IGameEnd
    {
        private readonly PlayerManager playerManager;
        private readonly StandardMap map;

        public DestroyedOkretyRakietoweGameEnd(PlayerManager playersManager, StandardMap map)
        {
            playerManager = playersManager;
            this.map = map;
        }

        public async Task Handle(Player? winner)
        {
            if (winner == null)
                await playerManager.WriteToPlayers("Remis! Okręty rakietowe zostały zniszczone");
            else
                await playerManager.WriteToPlayers(winner,
                msgToCurrent: "Zwycięstwo! Zniszczyłeś okręty rakietowe przeciwnika utrzymując obronę portu!",
                msgToOthers: "Porażka! Przeciwnik zniszczył Twóje okręty rakietowe utrzymując obronę portu!");
        }

        public (bool, Player?) IsGameEnded(int currentTurn)
        {
            Player current = playerManager.GetPlayerOfTurn(currentTurn);
            Player enemy = playerManager.GetOpositePlayer(current);

            if (enemy.Fleet.ActiveUnitsCount<OkretRakietowy>() == 0)
            {
                bool currentProtected = playerManager.TopPlayer == current ? map.TopEntrencesAreProtected : map.BottomEntrecesAreProtected;
                bool enemyProtected = playerManager.TopPlayer == enemy ? map.TopEntrencesAreProtected : map.BottomEntrecesAreProtected;

                if (currentProtected)
                {
                    if (current.Fleet.ActiveUnitsCount<OkretRakietowy>() == 0 && enemyProtected)
                        return (true, null);
                    else
                        return (true, current);
                }

            }

            return (false, null);
        }
    }
}
