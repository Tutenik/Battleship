using Battleship.Core;
using Battleship.MVVM.Model;
using System.Collections.ObjectModel;

namespace Battleship.MVVM.ViewModel
{
    public class PrepGameViewModel : ObservableObject
    {
		private object _gameBoard;

		public object GameBoard
		{
			get { return _gameBoard; }
			set 
			{ 
				_gameBoard = value;
				OnPropertyChanged();
			}
		}

		private ShipViewModel _selectedShip;
		public ShipViewModel SelectedShip
		{
			get
			{
				return _selectedShip;
			}
			set
			{
				_selectedShip = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<ShipViewModel> Ships { get; set; }

		public PrepGameViewModel(ShipSet selectedShipSet)
		{
			GameBoard = new GameBoardViewModel(new GameBoard(10));

			var tempList = new ObservableCollection<ShipViewModel>();
			foreach (var ship in selectedShipSet.Ships)
			{
				tempList.Add(new ShipViewModel(ship));
			}

			Ships = new ObservableCollection<ShipViewModel>(tempList.OrderByDescending(c => c.Cells.Count));
		}

	}
}
