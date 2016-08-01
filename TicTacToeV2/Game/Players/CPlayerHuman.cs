using Colorful;

namespace TicTacToeV2
{
	public class CPlayerHuman : APlayer
	{
		public CPlayerHuman(string name, char symbol) : base(name, symbol)
		{
		}

		public override Point2D Move()
		{
			//TODO: Maybe more interactive selection with arrow keys

			//Get X,Y coordinates from stdin
			Point2D point;
			string input;

			do
			{
				Console.Write("Write X,Y coordinates of cell you want to mark (0,0 - top left):\n > ");
				input = Console.ReadLine();
			} while (!Point2D.TryParse(input, out point));	//repeat unless valid point is provided

			return point;
		}
	}
}