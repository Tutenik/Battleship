using Battleship.Core;
using Battleship.MVVM.Model;
using System.Collections.ObjectModel;

namespace Battleship.MVVM.ViewModel
{
    public class ShipViewModel : ObservableObject
    {
        private Ship _ship;

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

        public ObservableCollection<CellViewModel> Cells { get; set; } = new ObservableCollection<CellViewModel>();

        public ShipViewModel(Ship ship)
        {
            _ship = ship;
            Rows = ship.Cells.Max(c => c.Row) + 1;
            Columns =  ship.Cells.Max(c => c.Column) + 1;

            if (Columns < Rows)
            {
                ship.RotateShip();

                int temp = Rows;
                Rows = Columns;
                Columns = temp;
            }

            Cells = new ObservableCollection<CellViewModel>();

            foreach (var cell in ship.Cells)
            {
                Cells.Add(new CellViewModel(cell));
            }

        }

        public ShipViewModel Clone()
        {
            var clone = new ShipViewModel(new Ship
            (
                Cells
                    .Select(c => new Cell(c.Row, c.Column ))
                    .ToList()
            ));

            return clone;
        }

        public Ship ToShip()
        {
            return _ship;
        }
    }
}
