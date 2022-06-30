using CellLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManewryMorskie
{
    public class ManewryMorskieGame
    {
        private readonly InternationalWaterManager internationalWaterManager;
        private readonly RectangleCellMap<MapField> map;
        private readonly GameEndDetector endDetector;

        private readonly TurnCounter turnManager = new();
        private readonly PlayerManager playerManager;

        public ManewryMorskieGame(Player player1, Player player2)
        {
            playerManager = new(turnManager);
            playerManager.AddPlayer(player1).AddPlayer(player2);

            map = MapSetups.Standard(player1, player2);

            internationalWaterManager = new InternationalWaterManager(map);

            turnManager.TurnChanging += TurnCounter_TurnChanging;
            internationalWaterManager.InternedUnit += InternationalWaterManager_InternedUnit;

            endDetector = new GameEndDetector(map, turnManager, playerManager);
            endDetector.GameEnded += EndDetector_GameEnded;
        }

        private void EndDetector_GameEnded(object sender, GameEnd e)
        {
            throw new NotImplementedException();

            switch(e.GameEndReason)
            {
                case GameEndReason.DestroyedOkretDesantowy:
                    playerManager.WriteToPlayers(e.Winner!,
                        msgToCurrent: "Zwycięstwo! Zniszczyłeś okręt desantowy przeciwnika!",
                        msgToOthers: "Porażka! Przeciwnik zniszczył Twój okręt desantowy!");
                    break;
                case GameEndReason.DestroyedBatteries:

                    break;
                case GameEndReason.OkretDesantowyReachedEnemyField:

                    break;
            }
        }

        private void TurnCounter_TurnChanging(object sender, int e)
        {
            internationalWaterManager.Iterate();
        }

        private void InternationalWaterManager_InternedUnit(object sender, Unit e)
        {
            foreach (MapField field in internationalWaterManager.InternationalWaters.Select(location => map[location]))
                if (field.Unit == e)
                {
                    field.Unit = null;
                    break;
                }

            foreach (Player player in playerManager)
                if(player.Fleet.Units.Contains(e))
                {
                    player.Fleet.Destroy(e);
                    return;
                }
        }
    }
}
