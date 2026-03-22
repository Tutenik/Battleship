using Battleship.Core;

namespace Battleship.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand ShipBuilderCommand { get; set; }
        public RelayCommand SettingsViewCommand { get; set; }
        public RelayCommand AboutViewCommand { get; set; }

        public HomeViewModel HomeVM { get; set; }
        public ShipBuilderViewModel ShipBuilderVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }
        public AboutViewModel AboutVM { get; set; }

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            HomeVM = new HomeViewModel();
            ShipBuilderVM = new ShipBuilderViewModel();
            SettingsVM = new SettingsViewModel();
            AboutVM = new AboutViewModel();

            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o => { CurrentView = HomeVM; });
            ShipBuilderCommand = new RelayCommand(o => { CurrentView = ShipBuilderVM; });
            SettingsViewCommand = new RelayCommand(o => { CurrentView = SettingsVM; });
            AboutViewCommand = new RelayCommand(o => { CurrentView = AboutVM; });
        }
    }
}
