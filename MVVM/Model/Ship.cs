namespace Battleship.MVVM.Model
{
    public class Ship
    {
        public int Size => Cells.Count;
        public List<Cell> Cells { get; }

        public bool IsSunk => Cells.All(c => c.Status == CellStatus.Hit);

        public Ship() { Cells = new List<Cell>(); }

        public Ship(List<Cell> cells)
        {
            Cells = cells;
        }

        public void RotateShip()
        {

            foreach (var cell in Cells)
            {
                int row = cell.Row;
                cell.Row = cell.Column;
                cell.Column = -row;
            }

            int minRow = Cells.Min(c => c.Row);
            int minColumn = Cells.Min(c => c.Column);

            foreach (var cell in Cells)
            {
                cell.Row -= minRow;
                cell.Column -= minColumn;
            }
        }

        public List<Cell> GetRelativePositions()
        {
            if (Cells == null || Cells.Count == 0)
                return new List<Cell>();

            // Find top-left (min X, then min Y)
            var origin = Cells
                .OrderBy(c => c.Row)
                .ThenBy(c => c.Column)
                .First();

            return Cells
                .Select(c => new Cell
                (
                    c.Row - origin.Row,
                    c.Column - origin.Column
                ))
                .ToList();
        }

        public static void RotateShip(Ship ship)
        {
            foreach (var cell in ship.Cells)
            {
                int row = cell.Row;
                cell.Row = cell.Column;
                cell.Column = -row;
            }

            int minRow = ship.Cells.Min(c => c.Row);
            int minColumn = ship.Cells.Min(c => c.Column);

            foreach (var cell in ship.Cells)
            {
                cell.Row -= minRow;
                cell.Column -= minColumn;
            }
        }

        public static List<Cell> GetRelativePositions(Ship ship)
        {
            if (ship.Cells == null || ship.Cells.Count == 0)
                return new List<Cell>();

            // Find top-left (min X, then min Y)
            var origin = ship.Cells
                .OrderBy(c => c.Row)
                .ThenBy(c => c.Column)
                .First();

            return ship.Cells
                .Select(c => new Cell
                (
                    c.Row - origin.Row,
                    c.Column - origin.Column
                ))
                .ToList();
        }
    }
}
