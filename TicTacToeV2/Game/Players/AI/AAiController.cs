using System.Diagnostics;

namespace TicTacToeV2
{
    public abstract class AAiController
    {
        private bool _isInitialized;

        protected CTicTacToe2D Game;
        protected int PlayerId = -1;

        /// <summary>
        /// Initialize AiController
        /// </summary>
        /// <param name="game">Game instance</param>
        /// <param name="playerId">Controller's owner player id</param>
        public virtual void Initialize(CTicTacToe2D game, int playerId)
        {
            if (Game == null)
            {
                Game = game;
            }

            Debug.WriteLineIf(_isInitialized, "AiController initialized more than once");
            _isInitialized = true;
            PlayerId = playerId;
        }

        /// <summary>
        /// Get AI's move
        /// </summary>
        /// <returns></returns>
        public abstract Point2D Move();

        /// <summary>
        /// Update all structures
        /// </summary>
        protected abstract void Think();

        /// <summary>
        /// Check if Controller is initialized. If not throw a GameException
        /// </summary>
        /// <param name="message">Message of exception</param>
        protected void AssertInitialized(string message = "AiController must be initialized before use")
        {
            if (!_isInitialized)
            {
                throw new GameException(message);
            }
        }
    }
}
