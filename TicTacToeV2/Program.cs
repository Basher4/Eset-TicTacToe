using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeV2
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "Tic-Tac-Toe Game";
			
			int gridSize;

			//Ask for grid size
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
			List<APlayer> players = new List<APlayer>(2);

			//create default players
			players.Add(new CPlayerHuman("Human", 'O'));
			//players.Add(new CPlayerAI("Computer - N00B", 'O', new CRandomAiController()));
			players.Add(new CPlayerAI("Computer - Better", 'X', new CBetterAiController()));

			game.AddPlayers(players);

			Console.Write("Should human begin [Y/n]? ");
			var key = Console.ReadKey().KeyChar;

			if (key != 'n' && key != 'N')
			{
				//user didn't write 'n', assume yes as answer -> human begins
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
