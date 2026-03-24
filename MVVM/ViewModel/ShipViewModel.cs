using Battleship.Core;
using Battleship.MVVM.Model;
using System.Collections.ObjectModel;

namespace Battleship.MVVM.ViewModel
{
    public class ShipViewModel : ObservableObject
    {
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

        public ObservableCollection<CellViewModel> Cells { get; set; }
        public ShipViewModel(Ship ship)
        {
            Rows = ship.Cells.Max(c => c.X) + 1;
            Columns =  ship.Cells.Max(c => c.Y) + 1;

            if (Columns < Rows)
            {
                ShipService.Rotate90Clockwise(ship.Cells);

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
    }
}
