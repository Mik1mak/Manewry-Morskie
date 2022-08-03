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
        private readonly Player playerOne;
        private readonly Player playerTwo;

        private Task? game;
        private CancellationTokenSource tokenSource = new();
        private bool gameIsOngoing = false;

        public GameService(UserInterface ui)
        {
            playerOne = new(ui)
            {
                Color = 0,
            };

            playerTwo = new(ui)
            {
                Color = 1,
            };
        }

        public async Task RunGame()
        {
            if(gameIsOngoing)
            {
                tokenSource.Cancel();
                await Task.Delay(3);
                tokenSource = new();
            }

            playerOne.Fleet.Clear();
            playerTwo.Fleet.Clear();

            gameIsOngoing = true;
            await using ManewryMorskieGame manewry = new(playerOne, playerTwo) { AsyncGame = false };
            CancellationToken token = tokenSource.Token;

            try
            {
                Console.WriteLine("Game started");
                await manewry.Start(token);
            }
            catch(OperationCanceledException)
            {
                Console.WriteLine("Game terminated");
            }
            finally
            {
                gameIsOngoing = false;
                Console.WriteLine("Game finished");
            }
        }
    }
}
