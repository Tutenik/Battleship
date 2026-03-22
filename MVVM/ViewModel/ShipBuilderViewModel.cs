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

        public static GameBoard gab = new(10);

        public ShipPainter sp;

        public ShipBuilderViewModel()
        {
            GameBoardVM = new GameBoardViewModel(gab);
            GridView = GameBoardVM;

            sp = new ShipPainter(gab, ShipPainter.Brushes.ShipBrush);

            PaintCommand = new RelayCommand(_ => sp.PaintCell());
            StopPaintingCommand = new RelayCommand(_ => sp.StopPaintingCell());

            //PaintCommand = new RelayCommand();
            string folder = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "ShipTemplates"
            );

            SaveShipsCommand = new RelayCommand(_ => 
                ShipService.SaveShips(Path.Combine(folder, $"{ShipSetName?.Trim()}.json"), 
                ShipService.DetectShips(gab.Cells)),
                _ => !string.IsNullOrWhiteSpace(ShipSetName)
            );
        }
    }
}
