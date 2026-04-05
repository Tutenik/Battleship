using Battleship.Core;
using Battleship.MVVM.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Battleship.MVVM.ViewModel
{
    public class HomeViewModel : ObservableObject
    {
        private FileSystemWatcher _watcher;

        private readonly ObservableCollection<ShipSet> _shipSets;
        public ObservableCollection<ShipSet> ShipSets => _shipSets;

        public PrepGameViewModel PrepGameVM { get; set; }

        private ShipSet _celectedItem;
        public ShipSet SelectedItem
        {
            get
            {
                return _celectedItem;
            }
            set
            {
                _celectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public RelayCommand PlayGameCommand { get; }

        public HomeViewModel(MainViewModel mainViewModel)
        {
            _shipSets = new ObservableCollection<ShipSet>();
            InitShipSets();
            SelectedItem = _shipSets.First(ss => ss.Name == "Default");
            SetupWatcher();

            PlayGameCommand = new RelayCommand(_ =>
            {
                PrepGameVM = new PrepGameViewModel(mainViewModel, SelectedItem);
                mainViewModel.ChangeCurrentView(PrepGameVM);
            });
        }

        private void InitShipSets()
        {
            var filePaths = Directory.GetFiles(ShipService.FolderPath, "*.json");
            foreach (var file in filePaths)
            {
                _shipSets.Add(new ShipSet(file));
            }
        }

        private void SetupWatcher()
        {
            _watcher = new FileSystemWatcher(ShipService.FolderPath, "*.json");

            _watcher.Created += OnFileCreated;
            _watcher.Deleted += OnFileDeleted;

            _watcher.EnableRaisingEvents = true;
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _shipSets.Add(new ShipSet(e.FullPath));
            });
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var item = _shipSets.FirstOrDefault(s => s.ShipSetPath == e.FullPath);
                if (item != null)
                    _shipSets.Remove(item);
            });
        }
    }
}
