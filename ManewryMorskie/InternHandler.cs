using CellLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManewryMorskie
{
    public class InternHandler : IDisposable
    {
        private readonly InternationalWaterManager manager;
        private readonly RectangleCellMap<MapField> map;
        private readonly PlayerManager players;

        public InternHandler(InternationalWaterManager manager, RectangleCellMap<MapField> map, PlayerManager players)
        {
            this.manager = manager;
            this.map = map;
            this.players = players;
            manager.InternedUnit += Handle;
        }

        private async void Handle(object sender, Unit e)
        {
            foreach (CellLocation location in manager.InternationalWaters)
            {
                MapField field = map[location];

                if (field.Unit == e)
                {
                    Player player = players.First(p => p.Fleet.Units.Contains(e));
                    player.Fleet.Destroy(e);
                    field.Unit = null;

                    foreach (IUserInterface ui in players.UniqueInferfaces)
                    {
                        await ui.TakeOffPawn(location);
                        await ui.DisplayMessage($"Jednostka {e} została internowana, ponieważ przebywała przez " +
                            "4 tury na wodach międzynarodowych!");
                    }
                    return;
                }
            }
        }

        public void Dispose()
        {
            manager.InternedUnit -= Handle;
        }
    }
}
