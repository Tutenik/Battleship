namespace Battleship.MVVM.Model
{
    public enum CellStatus
    {
        Empty,
        Ship,
        Hit,
        Missed
    }

    public class Cell(int x, int y)
    {
        /// <summary>
        /// Represents an action to be invoked when a cell changes.
        /// </summary>
        /// <remarks>Assign a handler to this delegate to respond to cell change events. The handler will
        /// be invoked whenever the associated cell changes state or value. This event does not provide additional
        /// context or event arguments; use with caution if more information about the change is required.</remarks>
        public event Action? CellChanged; //MVVM COTOJE AAAAAAAAAAHHHHHHHHHHHHHHHHHHHHHHH

        public event Action? CellEntered;
        public event Action? CellExited;
        public event Action<Cell>? CellClicked;


        private int _x = x;

        /// <summary>
        /// Gets or sets the X-coordinate value for the cell.
        /// </summary>
        /// <remarks>Setting this property raises the CellChanged event to notify listeners of the
        /// update.</remarks>
        public int X
        {
            get { return _x; }
            set 
            {
                _x = value;
                CellChanged?.Invoke();
            }
        }


        private int _y = y;

        /// <summary>
        /// Gets or sets the Y-coordinate value.
        /// </summary>
        /// <remarks>Setting this property raises the CellChanged event to notify listeners of the
        /// update.</remarks>
        public int Y
        {
            get { return _y; }
            set 
            { 
                _y = value;
                CellChanged?.Invoke();
            }
        }


        private CellStatus _status = CellStatus.Empty;

        /// <summary>
        /// Gets or sets the current status of the cell.
        /// </summary>
        /// <remarks>Setting this property raises the CellChanged event to notify listeners of the
        /// update.</remarks>
        public CellStatus Status
        {
            get { return _status; }
            set 
            {
                _status = value;
                CellChanged?.Invoke();
            }
        }

        public void OnCellEnter() => CellEntered?.Invoke();
        public void OnCellExit() => CellExited?.Invoke();
        public void OnCellClicked() => CellClicked?.Invoke(this);
    }
}