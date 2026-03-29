using Battleship.MVVM.Model;
using Battleship.Core;
using System.IO;

namespace Battleship.MVVM.ViewModel
{
    public class ShipBuilderViewModel : ObservableObject
    {
        public GameBoardViewModel GameBoardVM { get; set; }

        private object _gridView;
        public object GridView
        {
            get { return _gridView; }
            set
            {
                _gridView = value;
                OnPropertyChanged();
            }
        }

        private string _shipSetName;
        public string ShipSetName
        {
            get
            {
                return _shipSetName;
            }
            set
            {
                _shipSetName = value;
                OnPropertyChanged(nameof(ShipSetName));
            }
        }

        public RelayCommand PaintCommand { get; set; }
        public RelayCommand StopPaintingCommand { get; set; }
        public RelayCommand SaveShipsCommand { get; set; }

        public GameBoard gab;

        public ShipPainter sp;

        public ShipBuilderViewModel()
        {
            gab = new(10);
            GameBoardVM = new GameBoardViewModel(gab);
            GridView = GameBoardVM;

            sp = new ShipPainter(gab, ShipPainter.Brushes.ShipBrush);

            PaintCommand = new RelayCommand(_ => sp.PaintCell());
            StopPaintingCommand = new RelayCommand(_ => sp.StopPaintingCell());

            SaveShipsCommand = new RelayCommand(_ => 
                ShipService.SaveShips($"{ShipSetName?.Trim()}.json", 
                gab.DetectShips()),
                _ => !string.IsNullOrWhiteSpace(ShipSetName)
            );
        }
    }
}
