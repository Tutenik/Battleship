using Battleship.MVVM.AIStrategies;

namespace Battleship.MVVM.Model
{
    public class GameSession
    {
        private CancellationTokenSource _ctsGameLoop;
        private CancellationTokenSource _ctsTimer;

        private readonly int _turnTime = 10000;
        private readonly Random _rand = new Random();
        private readonly IAIStrategy _aiStrategy;
        private Cell _lastClickedCell;

        /// <summary>
        /// Gets the game board for player 1.
        /// </summary>
        public GameBoard Player1Board { get; }

        /// <summary>
        /// Gets the game board for player 2.
        /// </summary>
        public GameBoard Player2Board { get; }

        /// <summary>
        /// Gets the current status message for the operation or process.
        /// </summary>
        public bool IsPlayer1Turn { get; private set; } = true;

        /// <summary>
        /// Gets the percentage of the resource that remains available.
        /// </summary>
        public double RemainingPercentage { get; private set; } = 1.0;

        /// <summary>
        /// Gets the current status message for the operation or process.
        /// </summary>
        public string StatusMessage { get; private set; } = "";

        /// <summary>
        /// Occurs when the active player changes during gameplay.
        /// </summary>
        /// <remarks>Subscribe to this event to be notified whenever the current player's turn ends and
        /// control passes to another player. This event is typically used to update UI elements or trigger game logic
        /// that depends on the active player.</remarks>
        public event Action PlayerTurnChanged;

        /// <summary>
        /// Occurs when the remaining percentage value changes.
        /// </summary>
        /// <remarks>Subscribe to this event to be notified whenever the remaining percentage is updated.
        /// This event is typically raised after the value has changed.</remarks>
        public event Action RemainingPrecentageChanged;

        /// <summary>
        /// Occurs when the status message is updated.
        /// </summary>
        /// <remarks>Subscribe to this event to be notified whenever the status message changes. Event
        /// handlers are invoked on the thread that raises the event; ensure thread safety when updating UI
        /// elements.</remarks>
        public event Action StatusMessageChanged;

        /// <summary>
        /// Initializes a new instance of the GameSession class with the specified player boards and AI strategy.
        /// </summary>
        /// <param name="player1Board">The game board assigned to player 1. Cannot be null.</param>
        /// <param name="player2Board">The game board assigned to player 2. Cannot be null.</param>
        /// <param name="aiStrategy">The AI strategy used to control the computer opponent. Cannot be null.</param>
        public GameSession(GameBoard player1Board, GameBoard player2Board, IAIStrategy aiStrategy)
        {
            Player1Board = player1Board;
            Player2Board = player2Board;

            _aiStrategy = aiStrategy;

            Player1Board.InitializeAvailableCells();
            SubscribeToClickEvents();
            PlayerTurnChanged += HandleTimer;

            _ = GameLoop();
            _ = StartTimer(_turnTime);
        }

        /// <summary>
        /// Resets the player's board to display only ship positions, hiding all other cell statuses.
        /// </summary>
        /// <remarks>Use this method to prepare the player's board for display by ensuring that only ship
        /// locations are visible. This is typically called when updating the player's view to prevent revealing
        /// additional information about the board state.</remarks>
        public void ShowOwnBoard()
        {
            foreach (var cell in Player1Board.Cells)
            {
                cell.Status = cell.Status switch
                {
                    CellStatus.Ship => CellStatus.Ship,
                    _ => CellStatus.Empty
                };
            }
        }

        /// <summary>
        /// Stops the turn timer.
        /// </summary>
        /// <remarks>Call this method to cancel the active turn timer, preventing it from triggering a move skip.</remarks>
        public void StopTimer()
        {
            _ctsTimer?.Cancel();
        }

        private void HandleTimer()
        {
            StopTimer();

            _ = StartTimer(_turnTime);
        }

        private void SubscribeToClickEvents()
        {
            foreach (var cell in Player2Board.Cells)
            {
                cell.CellClicked += OnEnemyCellClicked;
            }

        }

        private void UnsubscribeFromClickEvents()
        {
            foreach (var cell in Player2Board.Cells)
            {
                cell.CellClicked -= OnEnemyCellClicked;
            }
        }

        private void OnEnemyCellClicked(Cell cell)
        {
            if (_lastClickedCell == cell) return;
            if (!IsPlayer1Turn) return;

            _lastClickedCell = cell;
            cell.Status = cell.Status switch
            {
                CellStatus.Empty => CellStatus.Miss,
                CellStatus.ShipNeighbour => CellStatus.Miss,
                CellStatus.Ship => CellStatus.Hit,
                _ => cell.Status
            };

            if (cell.Status != CellStatus.Hit)
            {
                IsPlayer1Turn = false;
            }
            PlayerTurnChanged?.Invoke();
            foreach (var ship in Player2Board.Ships.Where(ship => ship.IsSunk))
            {
                Player2Board.RevealShip(ship);
            }

            CheckGameOver();
        }

        private async Task GameLoop()
        {
            _ctsGameLoop = new CancellationTokenSource();
            var token = _ctsGameLoop.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (!IsPlayer1Turn)
                    {
                        do
                        {
                            await Task.Delay(_rand.Next((int)(_turnTime * 0.02f), (int)(_turnTime * 0.55f)), token);
                            PlayerTurnChanged?.Invoke();
                        }
                        while (_aiStrategy.MakeMove(Player1Board));

                        IsPlayer1Turn = true;
                        PlayerTurnChanged?.Invoke();
                    }

                    CheckGameOver();
                    await Task.Delay(50, token);
                }
            }
            catch (TaskCanceledException)
            {
                // expected when stopping loop
            }
        }

        private async Task StartTimer(int durationMs)
        {
            _ctsTimer = new CancellationTokenSource();
            var token = _ctsTimer.Token;

            RemainingPercentage = 1.0;

            int interval = 50;
            double steps = durationMs / interval;
            double decrement = 1 / steps;

            try
            {
                while (RemainingPercentage > 0 && !token.IsCancellationRequested)
                {
                    await Task.Delay(interval, token);

                    RemainingPercentage -= decrement;

                    if (RemainingPercentage < 0)
                    {
                        RemainingPercentage = 0;
                        StopTimer();
                    }
                    RemainingPrecentageChanged?.Invoke();
                }

                SkipMove();
            }
            catch (TaskCanceledException) { }
        }

        private void SkipMove()
        {
            IsPlayer1Turn = !IsPlayer1Turn;
            PlayerTurnChanged?.Invoke();
        }

        private void CheckGameOver()
        {
            if (Player1Board.Ships.All(s => s.IsSunk))
            {
                EndGame("You Lost");
            }

            if (Player2Board.Ships.All(s => s.IsSunk))
            {
                EndGame("You Won!");
            }
        }

        private void EndGame(string statusMessage)
        {
            UnsubscribeFromClickEvents();
            StopTimer();
            _ctsGameLoop?.Cancel();

            StatusMessage = statusMessage;
            StatusMessageChanged?.Invoke();
        }
    }
}
