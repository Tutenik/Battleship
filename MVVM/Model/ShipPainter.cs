namespace Battleship.MVVM.Model
{
    /// <summary>
    /// Types of brushes for painting.
    /// </summary>
    public enum Brushes
    {
        ShipBrush,
        EraserBrush,
        None
    }

    /// <summary>
    /// Used to paint on grid.
    /// </summary>
    public class ShipPainter
    {
        private readonly GameBoard _board;

        /// <summary>
        /// Gets current brush.
        /// </summary>
        public Brushes CurrentBrush { get; set; }

        /// <summary>
        /// BLU BLU BLU BLA BLA BLA BLE BLE BLE
        /// </summary>
        /// <param name="board"></param>
        public ShipPainter(GameBoard board)
        {
            _board = board;
        }

        /// <summary>
        /// Start painting.
        /// </summary>
        public void StartPainting()
        {
            for (int i = 0; i < _board.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < _board.Cells.GetLength(1); j++)
                {
                    _board.Cells[i, j].CellClicked -= Paint;
                    _board.Cells[i, j].CellClicked += Paint;
                }
            }
        }

        /// <summary>
        /// Stop painting
        /// </summary>
        public void StopPaintingCell()
        {
            for (int i = 0; i < _board.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < _board.Cells.GetLength(1); j++)
                {
                    _board.Cells[i, j].CellClicked -= Paint;
                }
            }
        }

        private void Paint(Cell cell)
        {
            cell.Status = CurrentBrush switch
            {
                Brushes.ShipBrush => CellStatus.Ship,
                Brushes.EraserBrush => CellStatus.Empty,
                _ => cell.Status,
            };
        }
    }
}
