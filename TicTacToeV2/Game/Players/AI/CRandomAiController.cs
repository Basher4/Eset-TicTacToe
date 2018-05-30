using System;

namespace TicTacToeV2
{
    /// <summary>
    /// Just random moves
    /// </summary>
    public class CRandomAiController : AAiController
    {
        private readonly Random _rand;

        public CRandomAiController() : base()
        {
            this._rand = new Random();
        }

        public override Point2D Move()
        {
            AssertInitialized();

            //Think(); 
            return new Point2D(_rand.Next(Game.GridSize), _rand.Next(Game.GridSize));
        }

        protected override void Think()
        {
            //No need to think...
        }
    }
}