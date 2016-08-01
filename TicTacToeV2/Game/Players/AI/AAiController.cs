using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeV2
{
	public abstract class AAiController
	{
		#region Protected variables
		protected CTicTacToe2D Game;

		protected bool IsInitialized = false;
		protected int PlayerId = -1;
		#endregion

		/// <summary>
		/// Initialize aiController
		/// </summary>
		/// <param name="game">Game instance</param>
		/// <param name="playerId">Controller's owner player id</param>
		public virtual void Initialize(CTicTacToe2D game, int playerId)
		{
			if (Game == null)
			{
				Game = game;
			}

			System.Diagnostics.Debug.WriteLineIf(IsInitialized, "AiController initialized more than once");
			IsInitialized = true;
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
			if (!IsInitialized)
			{
				throw new GameException(message);
			}
		}
	}
}
