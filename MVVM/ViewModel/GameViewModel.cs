using Battleship.Core;
using Battleship.MVVM.AIStrategies;
using Battleship.MVVM.Model;
using System.Windows;

namespace Battleship.MVVM.ViewModel
{
    public class GameViewModel : ObservableObject
    {
		private GameSession _gameSession;

        private bool _isPlayerTurn = true;
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

		private double _percantage = 1.0;
		public double Percentage
		{
			get
			{
				return _percantage;
			}
			set
			{
				_percantage = value;
				OnPropertyChanged(nameof(Percentage));
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
			_gameSession = new GameSession(gameBoard, enemyGameBoard, new EasyAi());

            GameBoardVM = new GameBoardViewModel(_gameSession.Player1Board);
			EnemyGameBoardVM = new GameBoardViewModel(_gameSession.Player2Board);

			_gameSession.ShowOwnBoard();

            _gameSession.PlayerTurnChanged += UpdatePlayerTurn;
			_gameSession.RemainingPrecentageChanged += UpdateRemainingPercentage;
        }

		private void UpdatePlayerTurn()
		{
			IsPlayerTurn = _gameSession.IsPlayer1Turn;
        }

		private void UpdateRemainingPercentage()
		{
			Percentage = _gameSession.RemainingPercentage;
        }
    }
}
