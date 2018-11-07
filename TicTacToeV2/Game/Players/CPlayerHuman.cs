using Colorful;

namespace TicTacToeV2
{
    public class CPlayerHuman : APlayer
    {
        public CPlayerHuman(string name, char symbol, bool shouldRedrawScreen = true)
            : base(name, symbol, shouldRedrawScreen)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets player input from console
        /// </summary>
        /// <returns></returns>
        public override Point2D Move()
        {
            Point2D target;
            do
            {
                Console.Write("Write X,Y coordinates of the cell" +
                              "you want to mark (0,0 - top left):\n > ");
            } while (!Point2D.ReadFromConsole(out target));
            return target;
        }
    }
}