using Battleship.Core;
using Battleship.MVVM.Model;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        private ImageSource _imagePath;
        public ImageSource ImagePath
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

            _imagePath = new BitmapImage(new Uri("/Resources/Images/boykis.png", UriKind.Relative));

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
                CellStatus.Hit => new BitmapImage( new Uri("/Resources/Images/HitCell.png", UriKind.Relative)),
                CellStatus.Missed => new BitmapImage(new Uri("/Resources/Images/MissedCell.png", UriKind.Relative)),
                CellStatus.Ship =>  new BitmapImage(new Uri("/Resources/Images/ShipCell.png", UriKind.Relative)),
                _ => new BitmapImage(new Uri("/Resources/Images/boykis.png", UriKind.Relative)),
            };
        }
    }
}