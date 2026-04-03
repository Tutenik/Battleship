using Battleship.Core;

namespace Battleship.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; }
        public RelayCommand ShipBuilderCommand { get; }
        public RelayCommand SettingsViewCommand { get; }
        public RelayCommand AboutViewCommand { get; }

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
            HomeVM = new HomeViewModel(this);
            ShipBuilderVM = new ShipBuilderViewModel();
            SettingsVM = new SettingsViewModel();
            AboutVM = new AboutViewModel();

            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o =>
            {
                if (HomeVM.PrepGameVM == null) { CurrentView = HomeVM; }
                else if (HomeVM.PrepGameVM.GameVM == null) { CurrentView = HomeVM.PrepGameVM; }
                else CurrentView = HomeVM.PrepGameVM.GameVM;
            });
            ShipBuilderCommand = new RelayCommand(o => { CurrentView = ShipBuilderVM; });
            SettingsViewCommand = new RelayCommand(o => { CurrentView = SettingsVM; });
            AboutViewCommand = new RelayCommand(o => { CurrentView = AboutVM; });
        }

        public void ChangeCurrentView(object newView)
        {
            CurrentView = newView;
        }
    }
}
