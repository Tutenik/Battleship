namespace Battleship.MVVM.Model
{
    public class ShipPainter
    {
        public enum Brushes
        {
            ShipBrush,
            None
        }

        private GameBoard _board;
        private Brushes _currentBrush;

        public Brushes CurrentBrush
        {
            get { return _currentBrush; }
            set { _currentBrush = value; }
        }

        public ShipPainter(GameBoard board, Brushes currentBrush)
        {
            _board = board;
            CurrentBrush = currentBrush;
        }

        public void PaintCell()
        {
            for (int i = 0; i < _board.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < _board.Cells.GetLength(1); j++)
                {
                    _board.Cells[i, j].CellClicked -= ActivateCell;
                    _board.Cells[i, j].CellClicked += ActivateCell;
                }
            }
        }

        public void StopPaintingCell()
        {
            for (int i = 0; i < _board.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < _board.Cells.GetLength(1); j++)
                {
                    _board.Cells[i, j].CellClicked -= ActivateCell;
                }
            }
        }

        private void ActivateCell(Cell cell)
        {
            cell.Status = CurrentBrush switch
            {
                Brushes.ShipBrush => CellStatus.Ship,
                _ => CellStatus.Empty,
            };
        }
    }
}
