using Battleship.Core;
using Battleship.MVVM.Model;
using System.IO;
using System.Windows;

namespace Battleship.MVVM.ViewModel
{
    public class ShipBuilderViewModel : ObservableObject
    {
        private readonly GameBoard _gb;
        private readonly ShipPainter _sp;

        private GameBoardViewModel _gameBoard;
        public GameBoardViewModel GameBoard
        {
            get { return _gameBoard; }
            set
            {
                _gameBoard = value;
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
                OnPropertyChanged();
            }
        }

        private Brushes _selectedBrush;
        public Brushes SelectedBrush
        {
            get
            {
                return _selectedBrush;
            }
            set
            {
                _selectedBrush = value;
                _sp.CurrentBrush = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SaveShipsCommand { get; }

        public ShipBuilderViewModel()
        {
            _gb = new GameBoard(10);
            GameBoard = new GameBoardViewModel(_gb);

            _sp = new ShipPainter(_gb);

            _sp.StartPainting();

            SaveShipsCommand = new RelayCommand(_ =>
            {
                if (File.Exists(Path.Combine(ShipService.FolderPath, $"{ShipSetName?.Trim()}.json")))
                {
                    MessageBox.Show("A ship set with this name already exists. Please choose a different name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ShipSetName = "";
                    return;
                }

                ShipService.SaveShips($"{ShipSetName?.Trim()}.json",
                _gb.DetectShips());
            },
                _ => !string.IsNullOrWhiteSpace(ShipSetName)
            );
        }
    }
}
