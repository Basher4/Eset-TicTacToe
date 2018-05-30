using System;

namespace TicTacToeV2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Tic-Tac-Toe";
            
            int gridSize;

            //Ask for the grid size
            while (true)
            {
                do
                {
                    Console.Write("Set size of game grid: ");
                } while (!int.TryParse(Console.ReadLine(), out gridSize));

                if (gridSize >= 1)
                    break;

                Console.WriteLine($"No no no. Bad number. How do you expect to play with {gridSize} cells?");
            }

            //create game
            var game = new CTicTacToe2D(gridSize);

            //Get players' info
            Console.WriteLine("Player count: 2");	//can be more, why not

            //create default players
            game.AddPlayer(new CPlayerHuman("Human", 'O'));
            //game.AddPlayer(new CPlayerAI("Computer - N00B", 'O', new CRandomAiController()));
            game.AddPlayer(new CPlayerAI("Computer - Better", 'X', new CBetterAiController()));

            Console.Write("Should human begin [Y/n]? ");
            var key = Console.ReadKey().KeyChar;

            if (key != 'n' && key != 'N')
            {
                //user didn't write 'n', assume yes -> human begins
                game.SetStartPlayer(0);
            }
            else
            {
                //computer begins
                game.SetStartPlayer(1);
            }

            game.Start();
        }
    }
}
