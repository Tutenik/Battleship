using Battleship.MVVM.AIStrategies;
using System.Windows;
using System.Linq;

namespace Battleship.MVVM.Model
{
    public class GameSession
    {
        private CancellationTokenSource _ctsGameLoop;
        private CancellationTokenSource _ctsTimer;

        private Random r = new Random();
        private Cell _lastClickedCell;
        public GameBoard Player1Board { get; }
        public GameBoard Player2Board { get; }

        IAIStrategy AIStrategy { get; }

        public event Action PlayerTurnChanged;
        public bool IsPlayer1Turn { get; private set; } = true;

        private int _turnTime = 10000;

        public event Action RemainingPrecentageChanged;
        public double RemainingPercentage { get; private set; } = 1.0;

        private GameSession(GameBoard player1Board, GameBoard player2Board)
        {
            Player1Board = player1Board;
            Player2Board = player2Board;

            SubscribeToClickEvents();
        }

        public GameSession(GameBoard player1Board, GameBoard player2Board, IAIStrategy aiStrategy)
        {
            Player1Board = player1Board;
            Player2Board = player2Board;

            AIStrategy = aiStrategy;

            ShowOwnBoard();
            Player1Board.InitializeAvailableCells();
            SubscribeToClickEvents();

            _ = GameLoop();
            PlayerTurnChanged += HandleTimer;
            _ = StartTimer(_turnTime);
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

            // invoked because of timer, and this is the easiest option
            PlayerTurnChanged?.Invoke();

            if (cell.Status != CellStatus.Hit)
            {
                IsPlayer1Turn = false;
                PlayerTurnChanged?.Invoke();
            }

            foreach (var ship in Player2Board.Ships.Where(ship => ship.IsSunk))
            {
                Player2Board.RevealShip(ship);
            }

            CheckGameOver();
        }

        public async Task GameLoop()
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
                            await Task.Delay(r.Next((int)(_turnTime * 0.02f), (int)(_turnTime * 0.55f)), token);
                            // The same as before, rhis is besause of timer and this is the easiest way to update it
                            PlayerTurnChanged?.Invoke();
                        }
                        while (AIStrategy.MakeMove(Player1Board));

                        IsPlayer1Turn = true;
                        PlayerTurnChanged?.Invoke();
                    }
                    CheckGameOver();

                    await Task.Delay(200, token);
                }
            }
            catch (TaskCanceledException)
            {
                // expected when stopping loop
            }
        }

        public async Task StartTimer(int durationMs)
        {
            _ctsTimer = new CancellationTokenSource();
            var token = _ctsTimer.Token;

            RemainingPercentage = 1.0;

            int interval = 50; // update every 50ms
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
        public void StopTimer()
        {
            _ctsTimer?.Cancel();
        }

        private void SkipMove()
        {
            IsPlayer1Turn = !IsPlayer1Turn;
            PlayerTurnChanged?.Invoke();
        }

        private void CheckGameOver()
        {
            if (Player1Board.Ships.All(s => s.IsSunk) || Player2Board.Ships.All(s => s.IsSunk))
            {
                EndGame();
            }
        }

        private void EndGame()
        {
            UnsubscribeFromClickEvents();
            StopTimer();
            _ctsGameLoop?.Cancel();
        }
    }
}
