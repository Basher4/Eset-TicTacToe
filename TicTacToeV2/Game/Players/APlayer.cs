namespace TicTacToeV2
{
	public abstract class APlayer
	{
		/// <summary>
		/// Player's display name
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		/// Symbol displayed on the grid
		/// </summary>
		public char Symbol { get; protected set; }

		public bool ShouldRedrawScreen { get; protected set; }

		//instance of game player belongs to
		private CTicTacToe2D _game = null;
		private int _playerId = -1;

		public APlayer(string name, char symbol)
		{
			Name   = name;
			Symbol = symbol;
			ShouldRedrawScreen = true;
		}

		/// <summary>
		/// Called before the game starts after initialization
		/// </summary>
		/// <param name="game">Instance of a game that contains this player</param>
		/// <param name="playerId">Player ID in a game</param>
		public virtual void PostGameInitialize(CTicTacToe2D game, int playerId)
		{
			if (_game != null)
			{
				throw new GameException("This player is already in this or different game.");
			}

			_game = game;
			_playerId = playerId;
		}

		/// <summary>
		/// Get location player wants to select. This should be validated afterwards.
		/// </summary>
		/// <returns></returns>
		public abstract Point2D Move();
	}
}