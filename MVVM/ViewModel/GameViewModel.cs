using Battleship.Core;
using Battleship.MVVM.Model;

namespace Battleship.MVVM.ViewModel
{
    public class GameViewModel : ObservableObject
    {
        private GameBoard _gameBoard;
        private GameBoard _enemyGameBoard;

		private bool _isPlayerTurn;
		public bool IsPlayerTurn
		{
			get
			{
				return _isPlayerTurn;
			}
			set
			{
				_isPlayerTurn = value;
				OnPropertyChanged(nameof(IsPlayerTurn));
			}
		}

		private GameBoardViewModel _gameBoardVM;
		public GameBoardViewModel GameBoardVM
		{
			get
			{
				return _gameBoardVM;
			}
			set
			{
				_gameBoardVM = value;
				OnPropertyChanged();
			}
		}

		private GameBoardViewModel _enemyGameBoardVM;

        public GameBoardViewModel EnemyGameBoardVM
		{
			get
			{
				return _enemyGameBoardVM;
			}
			set
			{
				_enemyGameBoardVM = value;
				OnPropertyChanged(nameof(EnemyGameBoardVM));
			}
		}

		public GameViewModel(GameBoard gameBoard, GameBoard enemyGameBoard)
        {
			_gameBoard = gameBoard;
            _enemyGameBoard = enemyGameBoard;

            GameBoardVM = new GameBoardViewModel(gameBoard);
			EnemyGameBoardVM = new GameBoardViewModel(enemyGameBoard);

			foreach (var cell in _enemyGameBoard.Cells)
			{
				cell.CellClicked += OnEnemyCellClicked;
            }
        }

        private void OnEnemyCellClicked(Cell cell)
        {
            cell.Status = cell.Status switch
			{
				CellStatus.Empty => CellStatus.Missed,
				CellStatus.ShipNeighbour => CellStatus.Missed,
                CellStatus.Ship => CellStatus.Hit,
				_ => cell.Status
			};
        }
    }
}
