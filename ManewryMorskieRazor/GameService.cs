using ManewryMorskie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskieRazor
{
    public class GameService
    {
        private IUserInterface ui;

        public GameService(UserInterface ui)
        {
            this.ui = ui;
        }

        public async Task RunGame()
        {
            Player playerOne = new(ui)
            {
                Color = 0,
            };

            Player playerTwo = new(ui)
            {
                Color = 1,
            };

            ManewryMorskieGame manewry = new(playerOne, playerTwo)
            {
                AsyncGame = false
            };

            await manewry.Start(CancellationToken.None);
        }
    }
}
