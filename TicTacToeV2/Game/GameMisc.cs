using System;
using System.Runtime.Serialization;

namespace TicTacToeV2
{
    /// <summary>
    /// All possible states of a game
    /// </summary>
    public enum EGameState
    {
        Unknown,
        Initialize,
        Play,
        Reset,
        Finish
    }

    /// <summary>
    /// Specialized exception class for game error messages
    /// </summary>
    public class GameException : Exception
    {
        public GameException()
        {
        }

        public GameException(string message) : base(message)
        {
        }

        public GameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// X,Y coordinates
    /// </summary>
    public struct Point2D
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Parses X,Y coordinates divided by comma or space from <paramref name="text"/>
        /// </summary>
        /// <param name="text">String containing text to be parsed</param>
        /// <param name="output">Output structure containing coordinates in <paramref name="text"/>. If return value is false, content is invalid.</param>
        /// <returns>True if conversion was successful</returns>
        public static bool TryParse(string text, out Point2D output)
        {
            try
            {
                int[] coords = Array.ConvertAll(text.Split(',', ' '), int.Parse);

                //if conversion succeeded, no exception is thrown
                output = new Point2D(coords[0], coords[1]);
            }
            catch (Exception)
            {
                //exception occurred - invalid text
                output = new Point2D();

                return false;
            }

            return true;
        }
    }
}