namespace Battleship.MVVM.Model
{
    /// <summary>
    /// Creates new ship.
    /// </summary>
    /// <param name="cells">The cells of the ship.</param>
    public class Ship
    {
        /// <summary>
        /// Gets the list of cells of the ship.
        /// </summary>
        public List<Cell> Cells { get; }

        /// <summary>
        /// Returns if the ship is sunk.
        /// </summary>
        public bool IsSunk => Cells.All(c => c.Status == CellStatus.Hit);

        public Ship(List<Cell> cells)
        {
            Cells = cells;
        }

        /// <summary>
        /// Rotates the ship 90 degrees.
        /// </summary>
        /// <param name="ship">The rotated ship</param>
        /// <remarks>Modifies the ship values</remarks>
        public static void RotateShip(Ship ship)
        {
            int maxRow = ship.Cells.Max(c => c.Row);

            foreach (var cell in ship.Cells)
            {
                int oldRow = cell.Row;

                cell.Row = cell.Column;
                cell.Column = maxRow - oldRow;
            }
        }

        /// <summary>
        /// Gets relative positions of the ship.
        /// </summary>
        /// <param name="ship">The ship in question.</param>
        /// <returns>Cells with relative positions.</returns>
        public static List<Cell> GetRelativePositions(Ship ship)
        {
            if (ship.Cells == null || ship.Cells.Count == 0)
                return new List<Cell>();

            int minRow = ship.Cells.Min(c => c.Row);
            int minColumn = ship.Cells.Min(c => c.Column);

            return ship.Cells
                .Select(c => new Cell
                (
                    c.Row - minRow,
                    c.Column - minColumn
                ))
                .ToList();
        }
    }
}
