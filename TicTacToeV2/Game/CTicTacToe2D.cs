using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeV2
{
    /// <summary>
    /// Classic Tic Tac Toe game in 2D grid. This takes care of logic and rendering.
    /// </summary>
    public class CTicTacToe2D
    {
        public EGameState GameState { get; private set; } = EGameState.Unknown;

        /// <summary>
        /// Size of the game grid
        /// </summary>
        public int GridSize { get; private set; }


        private List<APlayer> _players;                     //collection of all players
        private int[,]        _gameGrid;                    //game grid - cell contains index of aPlayer, -1 is empty cell
        private int           _playerOnMove = -1;           //index of APlayer in players collection whose turn is now
        private int           _freeCellsLeft = -1;          //number of free cells left
        private int           _winnerId = -1;               //if of the winner; -1 = tie

        private bool _showNumbers = false;                  //show numbers along the axis, better user experience?

        /// <summary>
        /// Creates a Tic Tac Toe game with grid NxN and initializes the game
        /// </summary>
        /// <param name="size">Size of the grid</param>
        public CTicTacToe2D(int size)
        {
            _players = new List<APlayer>(2);
            _gameGrid = new int[size, size];

            GridSize = size;

            Initialize();
        }

        #region Game and player control methods
        /// <summary>
        /// Start the game loop
        /// </summary>
        public void Start()
        {
            //check for any undesired game states
            switch (GameState)
            {
                case EGameState.Unknown:
                    throw new GameException("Game must be initialized before calling Start()");
                case EGameState.Play:
                    throw new GameException("Game is already running on this instance.");
            }

            //check if game is not being played

            GameState = EGameState.Play;

            //check if we have at least two players
            if (_players.Count < 2)
            {
                throw new GameException("Game needs at least two players!");
            }

            //if we have more than two players and isn't specified otherwise,
            if (_playerOnMove == -1)
            {
                //set first aPlayer to start
                Debug.WriteLine("First player not set.");
                _playerOnMove = 0;
            }

            //post-initialize for all players
            for (int playerIndex = 0; playerIndex < _players.Count; playerIndex++)
            {
                _players[playerIndex].PostGameInitialize(this, playerIndex);
            }

            GameLoop();
        }

        /// <summary>
        /// Reset the game to its initial state
        /// </summary>
        public void Reset()
        {
            GameState = EGameState.Reset;

            Initialize();
        }

        /// <summary>
        /// Set which player makes the first move
        /// </summary>
        /// <param name="aPlayer"></param>
        public void SetStartPlayer(APlayer aPlayer)
        {
            _playerOnMove = _players.IndexOf(aPlayer);

            if (_playerOnMove == -1)
            {
                throw new GameException("No such player in the game");
            }
        }

        /// <summary>
        /// Set the index of a player which makes the first move
        /// </summary>
        /// <param name="index">Index of player in list</param>
        public void SetStartPlayer(int index)
        {
            if (index < 0 || index >= _players.Count)
            {
                throw new IndexOutOfRangeException();
            }

            _playerOnMove = index;
        }

        /// <summary>
        /// Add a player to the game
        /// </summary>
        /// <param name="aPlayer"></param>
        public void AddPlayer(APlayer aPlayer)
        {
            _players.Add(aPlayer);
        }

        /// <summary>
        /// Add a collection of players to the game
        /// </summary>
        /// <param name="players">Collection of players</param>
        public void AddPlayers(IEnumerable<APlayer> players)
        {
            foreach (var player in players)
            {
                _players.Add(player);
            }
        }

        /// <summary>
        /// Remove a player from the game if it's not playing
        /// </summary>
        /// <param name="aPlayer">Player to remove</param>
        /// <returns>True if the player was removed from list of players</returns>
        public bool RemovePlayer(APlayer aPlayer)
        {
            if (GameState == EGameState.Play)
            {
                throw new GameException("Cannot remove players while the game is playing");
            }

            return _players.Remove(aPlayer);
        }

        /// <summary>
        /// Remove players from the game if they are not playing
        /// </summary>
        /// <param name="players">Players to remove</param>
        public void RemovePlayers(IEnumerable<APlayer> players)
        {
            foreach (var player in players)
            {
                _players.Remove(player);
            }
        }

        /// <summary>
        /// Resize the game grid. Can be done only if the game is not running
        /// </summary>
        /// <param name="size"></param>
        public void ResizeGrid(int size)
        {
            if (GameState == EGameState.Play)
            {
                throw new GameException("Cannot resize while a game is running.");
            }

            GridSize = size;

            _gameGrid = new int[size, size];
        }
        #endregion

        private void Initialize()
        {
            GameState = EGameState.Initialize;

            //fill the game grid with empty 
            for (int x = 0; x < _gameGrid.GetLength(0); x++)
            {
                for (int y = 0; y < _gameGrid.GetLength(1); y++)
                {
                    _gameGrid[x, y] = -1;
                }
            }

            _freeCellsLeft = _gameGrid.GetLength(0) * _gameGrid.GetLength(1);
        }

        private void GameLoop()
        {
            while (GameState == EGameState.Play)
            {
                //Write game
                if (_players[_playerOnMove].ShouldRedrawScreen)
                {
                    Console.Clear();
                    WriteGame();
                }

                //Get move
                Point2D desiredMove;
                do
                {
                    desiredMove = _players[_playerOnMove].Move();
                } while (!IsValidMove(desiredMove));

                //Update grid and decrease free cells
                UpdateCell(desiredMove);
                _freeCellsLeft--;

                //update game state - check for winning
                UpdateGameState(desiredMove);

                //update playerOnMove
                _playerOnMove = (_playerOnMove + 1) % _players.Count;
            }

            WriteFinishGame();
        }

        #region Game state check stuff
        /// <summary>
        /// Checks if any player should win
        /// </summary>
        /// <param name="lastMove"></param>
        protected void UpdateGameState(Point2D lastMove)
        {
            var directions = GetPlayerOwnedLines(lastMove, _playerOnMove);

            if (directions.Any(i => i == GridSize))
            {
                GameState = EGameState.Finish;
                _winnerId = _playerOnMove;
            }

            //check if there are any free cells
            else if (_freeCellsLeft == 0)
            {
                GameState = EGameState.Finish;
                _winnerId = -1;                     //no winner found above -> tie
            }
        }

        /// <summary>
        /// <para>Returns length of lines of player's symbol starting prom <paramref name="position"/></para>
        /// 
        /// <para>
        /// Indexes in array:<para />
        ///     0 1 2<para />
        ///     3 P 3<para />
        ///     2 1 0
        /// </para>
        ///
        /// </summary>
        /// <param name="position">Point that lies on all lines</param>
        /// <param name="playerToCheckId">Current player's shape</param>
        /// <returns></returns>
        public int[] GetPlayerOwnedLines(Point2D position, int playerToCheckId)
        {
            if (playerToCheckId < 0 || playerToCheckId >= _players.Count)
            {
                throw new GameException("No such player in game");
            }

            int[] directions = { 1, 1, 1, 1 };    //one that was placed now
            var tasks = new Task[4];

            //count number of same shapes in all directions in parallel
            #region  Direction - 0
            tasks[0] = Task.Factory.StartNew(() =>
            {
                int x = position.X, y = position.Y;
                try
                {
                    while (_gameGrid[--x, --y] == playerToCheckId)
                    {
                        ++directions[0];
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    //When index is out of range, catch exception and position on
                }

                x = position.X;
                y = position.Y;
                try
                {
                    while (_gameGrid[++x, ++y] == playerToCheckId)
                    {
                        ++directions[0];
                    }
                }
                catch (IndexOutOfRangeException)
                {
                }
            });

            #endregion

            #region Direction - 1
            tasks[1] = Task.Factory.StartNew(() =>
            {
                int x = position.X, y = position.Y;
                try
                {
                    while (_gameGrid[x, --y] == playerToCheckId)
                    {
                        ++directions[1];
                    }
                }
                catch (IndexOutOfRangeException)
                {
                }

                x = position.X;
                y = position.Y;
                try
                {
                    while (_gameGrid[x, ++y] == playerToCheckId)
                    {
                        ++directions[1];
                    }
                }
                catch (IndexOutOfRangeException)
                {
                }
            });

            #endregion

            #region Direction - 2
            tasks[2] = Task.Factory.StartNew(() =>
            {
                int x = position.X, y = position.Y;
                try
                {
                    while (_gameGrid[++x, --y] == playerToCheckId)
                    {
                        ++directions[2];
                    }
                }
                catch (IndexOutOfRangeException)
                {
                }

                x = position.X; y = position.Y;
                try
                {
                    while (_gameGrid[--x, ++y] == playerToCheckId)
                    {
                        ++directions[2];
                    }
                }
                catch (IndexOutOfRangeException)
                {
                }
            });

            #endregion

            #region Direction - 3
            tasks[3] = Task.Factory.StartNew(() =>
            {
                int x = position.X, y = position.Y;
                try
                {
                    while (_gameGrid[++x, y] == playerToCheckId)
                    {
                        ++directions[3];
                    }
                }
                catch (IndexOutOfRangeException)
                {
                }

                x = position.X;
                y = position.Y;
                try
                {
                    while (_gameGrid[--x, y] == playerToCheckId)
                    {
                        ++directions[3];
                    }
                }
                catch (IndexOutOfRangeException)
                {
                }
            });

            #endregion

            //Wait for all tasks to finish
            Task.WaitAll(tasks);

            return directions;
        }
        #endregion

        /// <summary>
        /// Update cell with current player's id
        /// </summary>
        /// <param name="cell">Cell position</param>
        private void UpdateCell(Point2D cell) => _gameGrid[cell.X, cell.Y] = _playerOnMove;

        /// <summary>
        /// Get the content of a cell
        /// </summary>
        /// <param name="point">Cell position</param>
        /// <returns>Cell's content id</returns>
        public int CellAt(Point2D point) => _gameGrid[point.X, point.Y];

        /// <summary>
        /// Get the content of a cell
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>Cell's content id</returns>
        public int CellAt(int x, int y) => _gameGrid[x, y];

        private bool IsValidMove(Point2D point)
        {
            return (point.X >= 0 && point.X < GridSize) && (point.Y >= 0 && point.Y < GridSize) //valid position within grid
                    && CellAt(point) == -1;                                                     //cell is not marked
        }

        #region Output to console

        private void WriteFinishGame()
        {
            Console.Clear();

            WriteGrid();
            if (GameState == EGameState.Finish)
            {
                if (_winnerId == -1)
                {
                    Colorful.Console.WriteAscii($"   TIE!");
                }
                else
                {
                    Colorful.Console.WriteAscii($"   {_players[_winnerId].Name} WON!");
                }
            }
        }

        private void WriteGameUpdate(Point2D move, bool updateName = false)
        {
            if (updateName)
            {
                Console.SetCursorPosition(0, 0);
                WritePlayerOnMove();
            }

            Console.SetCursorPosition(move.X * 4 + 2, move.Y * 2 + 1);
        }

        /// <summary>
        /// Writes whole game to the Console
        /// </summary>
        private void WriteGame()
        {
            WritePlayerOnMove();
            WriteGrid();
        }

        /// <summary>
        /// Write grid to the Console
        /// </summary>
        private void WriteGrid()
        {
            for (int i = 0; i < GridSize; i++)
            {
                WriteSeparatorLine();
                WriteShapes(i);
            }

            WriteSeparatorLine();
        }

        /// <summary>
        /// Writes current player's name
        /// </summary>
        private void WritePlayerOnMove()
        {
            int spaceCount = Math.Max(GridSize * 2 - _players[_playerOnMove].Name.Length / 2, 0);   //X position to center player's name or 0

            Console.SetCursorPosition(spaceCount, 0);
            Console.WriteLine($"[{_players[_playerOnMove].Name}]");
        }

        /// <summary>
        /// Writes one line of the game grid
        /// </summary>
        /// <param name="line">line index</param>
        private void WriteShapes(int line)
        {
            Console.Write('|');
            for (int i = 0; i < GridSize; i++)
            {
                int value = _gameGrid[i, line];
                if (value == -1)
                {
                    //empty cell
                    Console.Write($"   |");
                }
                else
                {
                    Console.Write($" {_players[value].Symbol} |");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Writes separator
        /// +---+---+---+---+
        /// </summary>
        private void WriteSeparatorLine()
        {
            for (int i = 0; i < GridSize; i++)
            {
                Console.Write("+---");
            }
            Console.WriteLine('+');
        }
        #endregion

    }
}
