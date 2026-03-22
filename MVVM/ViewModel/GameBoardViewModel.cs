using Battleship.Core;
using Battleship.MVVM.Model;
using System.Collections.ObjectModel;

namespace Battleship.MVVM.ViewModel
{
    public class CellViewModel : ObservableObject
    {
        private readonly Cell _cell;

        private int _row;
        public int Row
        {
            get
            {
                return _row;
            }
            set
            {
                _row = value;
                OnPropertyChanged(nameof(Row));
            }
        }

        private int _column;
        public int Column
        {
            get
            {
                return _column;
            }
            set
            {
                _column = value;
                OnPropertyChanged(nameof(Column));
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                _imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        public RelayCommand CellEnterCommand { get; set; }
        public RelayCommand CellLeaveCommand { get; set; }
        public RelayCommand CellClickCommand { get; set; }

        public CellViewModel(Cell cell)
        {
            _cell = cell;

            _column = cell.Y;
            _row = cell.X;
            _imagePath = "/Resources/Images/boykis.png";

            _cell.CellChanged += UpdateCell;

            CellEnterCommand = new RelayCommand(_ => _cell.OnCellEnter());
            CellLeaveCommand = new RelayCommand(_ => _cell.OnCellExit());
            CellClickCommand = new RelayCommand(_ => _cell.OnCellClicked());
        }

        private void UpdateCell()
        {
            Row = _cell.X;
            Column = _cell.Y;
            ImagePath = _cell.Status switch
            {
                CellStatus.Hit => "/Resources/Images/HitCell.png",
                CellStatus.Missed => "/Resources/Images/MissedCell.png",
                CellStatus.Ship => "/Resources/Images/ShipCell.png",
                _ => "/Resources/Images/boykis.png",
            };
        }
    }

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
            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    _cells.Add(new CellViewModel(_gameBoard.GetCell(j, i)));
                }
            }
        }
    }
}