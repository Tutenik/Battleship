using Battleship.Core;
using Battleship.MVVM.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace Battleship.MVVM.ViewModel
{
    public class PrepGameViewModel : ObservableObject
    {
        private MainViewModel _mainViewModel;
        private GameBoard _gameBoard;
        private ShipSet _shipSet;

        private GameBoardViewModel _gameBoardVM;

        public GameBoardViewModel GameBoardVM
        {
            get { return _gameBoardVM; }
            set
            {
                _gameBoardVM = value;
                OnPropertyChanged();
            }
        }

        private ShipViewModel _selectedShip;
        public ShipViewModel? SelectedShip
        {
            get
            {
                return _selectedShip;
            }
            set
            {
                _selectedShip = value;
                OnPropertyChanged();

                if (_selectedShip != null)
                    HoverShip = _selectedShip.Clone();
            }
        }

        private ShipViewModel _hoverShip;
        public ShipViewModel? HoverShip
        {
            get => _hoverShip;
            set
            {
                _hoverShip = value;
                OnPropertyChanged();
            }
        }

        private double _leftCanvasPosizion;
        public double LeftCanvasPosition
        {
            get
            {
                return _leftCanvasPosizion;
            }
            set
            {
                _leftCanvasPosizion = value;
                OnPropertyChanged(nameof(LeftCanvasPosition));
            }
        }

        private double _topCanvasPosition;
        public double TopCanvasPosition
        {
            get
            {
                return _topCanvasPosition;
            }
            set
            {
                _topCanvasPosition = value;
                OnPropertyChanged(nameof(TopCanvasPosition));
            }
        }

        private int _gridX;
        public int GridRow
        {
            get
            {
                return _gridX;
            }
            set
            {
                _gridX = value;
                OnPropertyChanged(nameof(GridRow));
            }
        }

        private int _gridY;
        public int GridColumn
        {
            get
            {
                return _gridY;
            }
            set
            {
                _gridY = value;
                OnPropertyChanged(nameof(GridColumn));
            }
        }

        private Point _mousePosition;
        public Point MousePosition
        {
            get => ClampToBoard(
                Offset(QuantizeToGrid(_mousePosition, 40), HoverShip), 
                HoverShip?.Columns ?? 0, 
                HoverShip?.Rows ?? 0, 
                _gameBoard.Cells.GetLength(1), 
                _gameBoard.Cells.GetLength(0));
            set
            {
                _mousePosition = value;
                OnPropertyChanged();
            }
        }

        public GameViewModel GameVM { get; set; }
        public ObservableCollection<ShipViewModel> Ships { get; set; }

        public RelayCommand RotateShipCommand { get; set; }
        public RelayCommand MouseMoveCommand { get; set; }
        public RelayCommand MouseLeftButtonUpCommand { get; set; }
        public RelayCommand MouseRightButtonDownCommand { get; set; }
        public RelayCommand RandomizeCommand { get; set; }
        public RelayCommand ReadyCommand { get; set; }

        public PrepGameViewModel(MainViewModel mainViewModel, ShipSet selectedShipSet)
        {
            _mainViewModel = mainViewModel;
            _gameBoard = new GameBoard(10);
            GameBoardVM = new GameBoardViewModel(_gameBoard);

            _shipSet = selectedShipSet;

            Ships = new ObservableCollection<ShipViewModel>();
            foreach (var ship in selectedShipSet.Ships)
            {
                Ships.Add(new ShipViewModel(ship));
            }

            Ships = new ObservableCollection<ShipViewModel>(Ships.OrderByDescending(c => c.Cells.Count));

            RotateShipCommand = new RelayCommand(_ => OnRotate());
            MouseMoveCommand = new RelayCommand(_ => OnMouseMove());
            MouseLeftButtonUpCommand = new RelayCommand(_ => OnShipPlace());
            MouseRightButtonDownCommand = new RelayCommand(_ => OnShipPick());
            RandomizeCommand = new RelayCommand(_ => OnRandomize());
            ReadyCommand = new RelayCommand(_ => OnReady(), _ => Ships.Count == 0);
        }


        private void OnRotate()
        {
            if (HoverShip == null) return;

            Ship.RotateShip(new Ship(
                    HoverShip.Cells.Select(c => new Cell(c.Row, c.Column)).ToList()
                ));

            int newRows = HoverShip.Cells.Max(c => c.Column) + 1;
            int newCols = HoverShip.Cells.Max(c => c.Row) + 1;

            HoverShip.Rows = newRows;
            HoverShip.Columns = newCols;

            foreach (var cell in HoverShip.Cells)
            {
                int temp = cell.Row;
                cell.Row = cell.Column;
                cell.Column = temp;
            }
        }

        private void OnMouseMove()
        {
            TopCanvasPosition = MousePosition.Y * 40;
            LeftCanvasPosition = MousePosition.X * 40;
            GridRow = (int)MousePosition.Y;
            GridColumn = (int)MousePosition.X;
        }

        private void OnShipPlace()
        {

            bool placed = _gameBoard.PlaceShip(HoverShip, GridRow, GridColumn);

            if (placed)
            {
                Ships.Remove(SelectedShip);
                SelectedShip = null;
                HoverShip = null;
            }
        }

        private void OnShipPick()
        {
            if (SelectedShip != null) return;

            var pickedUpShip = PickUpShip(GridRow, GridColumn);

            if (pickedUpShip == null) return;

            SelectedShip = pickedUpShip;
            Ships.Add(pickedUpShip);
        }
        private void OnRandomize()
        {
            HoverShip = null;
            SelectedShip = null;
            _gameBoard.ClearBoard();
            _gameBoard.PlaceShipsRandomly(_shipSet);
            Ships.Clear();
        }
        private void OnReady()
        {
            GameVM = new GameViewModel(_gameBoard, CreateEnemyBoard());
            _mainViewModel.ChangeCurrentView(GameVM);
        }

        private GameBoard CreateEnemyBoard()
        {
            var enemyBoard = new GameBoard(10);
            enemyBoard.PlaceShipsRandomly(_shipSet);
            return enemyBoard;
        }

        private Point QuantizeToGrid(Point position, double cellSize)
        {
            int x = (int)(position.X / cellSize);
            int y = (int)(position.Y / cellSize);

            return new Point(x, y);
        }

        private Point Offset(Point position, ShipViewModel ship)
        {
            if (ship == null) return new Point(position.X, position.Y);
            return new Point(position.X - ship.Columns / 2, position.Y - ship.Rows / 2);
        }

        private Point ClampToBoard(
                                Point gridPos,
                                int shipWidth,
                                int shipHeight,
                                int boardRows,
                                int boardColumns)
        {
            int x = (int)gridPos.X;
            int y = (int)gridPos.Y;

            if (x < 0)
                x = 0;
            else if (x + shipWidth > boardColumns)
                x = boardColumns - shipWidth;

            if (y < 0)
                y = 0;
            else if (y + shipHeight > boardRows)
                y = boardRows - shipHeight;

            return new Point(x, y);
        }

        private ShipViewModel? PickUpShip(int row, int column)
        {
            if (_gameBoard.GetCell(row, column).Status != CellStatus.Ship)
                return null;


            var oldCells = _gameBoard.DetectShip(row, column);

            _gameBoard.RemoveShipFromGrid(new Ship(oldCells));

            var shipCells = Ship.GetRelativePositions(new Ship(oldCells));

            return new ShipViewModel(new Ship(shipCells));
        }
    }
}
