using Battleship.Core;
using Battleship.MVVM.Model;
using System.Collections.ObjectModel;

namespace Battleship.MVVM.ViewModel
{
    public class GameBoardViewModel : ObservableObject
    {
        private GameBoard _gameBoard;

        private readonly ObservableCollection<CellViewModel> _cells;
        public IEnumerable<CellViewModel> Cells => _cells; 

        private int _rows;

        public int Rows
        {
            get { return _rows; }
            set
            {
                _rows = value;
                OnPropertyChanged();
            }
        }

        private int _columns;

        public int Columns
        {
            get { return _columns; }
            set
            {
                _columns = value;
                OnPropertyChanged();
            }
        }


        public GameBoardViewModel(GameBoard gameBoard)
        {
            _gameBoard = gameBoard;
            Rows = gameBoard.Cells.GetLength(0);
            Columns = gameBoard.Cells.GetLength(1);

            _cells = new ObservableCollection<CellViewModel>();
            InitCells();
        }

        private void InitCells()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    _cells.Add(new CellViewModel(_gameBoard.GetCell(i, j)));
                }
            }
        }
    }
}