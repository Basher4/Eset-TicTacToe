﻿using System;
using Priority_Queue;

namespace TicTacToeV2
{
    /// <summary>
    /// Defensive AI controller - works correctly with two players <para/>
    /// Not the best, but better than random.
    /// </summary>
    public class CBetterAiController : AAiController
    {
        /// <summary>
        /// Class for use in priority queue
        /// </summary>
        private class PriorityQueuePointNode : FastPriorityQueueNode
        {
            public Point2D Point { get; set; }
        }

        private class Streak
        {
            public bool IsMyStreak { get; set; }
            public int StreakLength { get; set; }

            public Streak(bool isMyStreak, int streakLength)
            {
                IsMyStreak = isMyStreak;
                StreakLength = streakLength;
            }
        }

        private int _gridSize;
        private double[,] _scoreGrid;
        private FastPriorityQueue<PriorityQueuePointNode> _scoreQueue;

        public CBetterAiController() : base()
        {
        }

        public override void Initialize(CTicTacToe2D game, int playerId)
        {
            base.Initialize(game, playerId);

            _gridSize = game.GridSize;
            _scoreGrid = new double[_gridSize, _gridSize];
            _scoreQueue = new FastPriorityQueue<PriorityQueuePointNode>(_gridSize * _gridSize);
        }

        public override Point2D Move()
        {
            Think();

            //select a cell with best score
            return _scoreQueue.Dequeue().Point;
        }

        protected override void Think()
        {
            _scoreQueue.Clear();

            //update grid scores
            for (int y = 0; y < _gridSize; y++)
            {
                for (int x = 0; x < _gridSize; x++)
                {
                    //assess one cell
                    var score = CalculateScoreForCell(x, y) * -1;   //lowest number = highest priority


                    var node = new PriorityQueuePointNode
                    {
                        Point = new Point2D(x, y),
                        Priority = score
                    };

                    {
                        _scoreQueue.Enqueue(node, score);
                        _scoreGrid[x, y] = score;
                    }
                }
            }
        }

        //Calculates score for cell - higher score = better cell
        private double CalculateScoreForCell(int x, int y)
        {
            /*
             * I can win     -> double.MaxValue
             * Enemy can win -> double.MaxValue
             * otherwise:
             *      enemy streak * ENEMY_STREAK_MULTIPLIER
             *      my stream * MY_STREAK_MULTIPLIER
             */
            //x If no one can win in every direction -> score = 0

            //Calculate values in all directions
            var streaks = CalcStreaksInEveryDirection(x, y);

            if (streaks == null)
            {
                //we're not on empty cell => skip
                return 0;
            }

            //check if I can win
            for (int i = 0; i < 4; i++)
            {
                if (streaks[i].IsMyStreak && streaks[i].StreakLength == _gridSize - 1 ||
                    streaks[i + 1].IsMyStreak && streaks[i + 1].StreakLength == _gridSize - 1)
                {
                    return double.MaxValue;
                }

                if (streaks[i].IsMyStreak && streaks[i + 4].IsMyStreak)
                {
                    if (streaks[i].StreakLength + streaks[i + 4].StreakLength == _gridSize - 1)
                    {
                        return double.MaxValue;
                    }
                }
            }

            var best = streaks[0];

            //Get streak with best score
            foreach (var streak in streaks)
            {
                var bestScore = GetStreakScore(best);
                var streakScore = GetStreakScore(streak);

                if (bestScore < streakScore)
                {
                    best = streak;
                }
            }

            return GetStreakScore(best);

            
            int GetStreakScore(Streak streak)
            {
                // if I would want to pay more attention to my or enemy streaks,
                // I'd change these multipliers

                const int myStreakMultiplier = 1;
                const int enemyStreakMultiplier = 1;

                var streakMultiplier = streak.IsMyStreak ?
                    myStreakMultiplier : enemyStreakMultiplier;
                return streak.StreakLength * streakMultiplier;
            }
        }

        /// <summary>
        /// Calculate length of streak in every direction from an empty cell
        /// </summary>
        /// <param name="x">X position of center</param>
        /// <param name="y">Y position of center</param>
        /// <returns>BOOL - my streak; INT - streak length</returns>
        private Streak[] CalcStreaksInEveryDirection(int x, int y)
        {
            if (Game.CellAt(x, y) != -1)
            {
                //Not an empty cell
                return null;
            }

            var streakLengths = new int[8];
            var myStreaks = new bool[8];

            FCalcLine(0, -1, -1);   //left  up
            FCalcLine(1, +0, -1);   //      up
            FCalcLine(2, +1, -1);   //right up
            FCalcLine(3, +1, +0);   //right
            FCalcLine(4, +1, +1);   //right down
            FCalcLine(5, +0, +1);   //      down
            FCalcLine(6, -1, +1);   //left  down
            FCalcLine(7, -1, +0);   //left

            var tuples = new Streak[8];
            for (int i = 0; i < 8; i++)
            {
                tuples[i] = new Streak(myStreaks[i], streakLengths[i]);
            }

            return tuples;

            //function to calculate streak length and save it to the array
            void FCalcLine(int index, int dx, int dy)
            {
                int _x = x + dx, _y = y + dy;
                if (!(_x >= 0 && _x < _gridSize && _y >= 0 && _y < _gridSize)) return;

                int id = Game.CellAt(_x, _y);
                if (id == -1)
                {
                    streakLengths[index] = 0;
                    myStreaks[index] = true;
                    return;
                }

                myStreaks[index] = (id == PlayerId);

                while (_x >= 0 && _x < _gridSize && _y >= 0 && _y < _gridSize)
                {
                    if (Game.CellAt(_x, _y) == id)
                    {
                        streakLengths[index]++;
                    }

                    _x += dx;
                    _y += dy;
                }
            }
        }
    }
}