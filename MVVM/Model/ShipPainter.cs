namespace Battleship.MVVM.Model
{
    public enum Brushes
    {
        ShipBrush,
        EraserBrush,
        None
    }

    public class ShipPainter
    {
        private GameBoard _board;

        public Brushes CurrentBrush { get; set;  }

        public ShipPainter(GameBoard board)
        {
            _board = board;
        }

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
