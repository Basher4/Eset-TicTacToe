namespace TicTacToeV2
{
	public class CPlayerAI : APlayer
	{
		public AAiController AiController { get; set; }

		public CPlayerAI(string name, char symbol, AAiController aiController) : base(name, symbol)
		{
			AiController = aiController;

			ShouldRedrawScreen = false;
		}

		/// <summary>
		/// Initialize APlayer and aiController
		/// </summary>
		/// <param name="game">Game instance</param>
		/// <param name="playerId">Player's ID</param>
		public override void PostGameInitialize(CTicTacToe2D game, int playerId)
		{
			base.PostGameInitialize(game, playerId);

			AiController.Initialize(game, playerId);
		}

		public override Point2D Move()
		{
			return AiController.Move();
		}
	}
}