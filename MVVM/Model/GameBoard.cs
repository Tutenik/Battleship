using Battleship.MVVM.ViewModel;
namespace Battleship.MVVM.Model
{

    public class GameBoard
    {
        private readonly Random _rand = new Random();
        private List<Cell> _availableCells;

        /// <summary>
        /// Gets the two-dimensional array of cells contained in the grid.
        /// </summary>
        public Cell[,] Cells { get; }

        /// <summary>
        /// Gets the number of rows in the grid or matrix represented by the current instance.
        /// </summary>
        public int Rows => Cells.GetLength(1);

        /// <summary>
        /// Gets the number of columns in the grid.
        /// </summary>
        public int Columns => Cells.GetLength(0);

        /// <summary>
        /// Gets the collection of ships detected in the current context.
        /// </summary>
        public List<Ship> Ships => DetectShips();

        /// <summary>
        /// Initializes a new instance of the GameBoard class with the specified board size.
        /// </summary>
        /// <param name="size">The length of one side of the square game board. Must be a positive integer.</param>
        public GameBoard(int size)
        {
            Cells = InitCells(size);
        }

        /// <summary>
        /// Gets the cell at the specified row and column indices.
        /// </summary>
        /// <param name="row">The zero-based row index of the cell to retrieve. Must be within the valid range of rows.</param>
        /// <param name="column">The zero-based column index of the cell to retrieve. Must be within the valid range of columns.</param>
        /// <returns>The cell located at the specified row and column.</returns>
        public Cell GetCell(int row, int column)
        {
            return Cells[row, column];
        }

        /// <summary>
        /// Clears all cells on the board, resetting their status to empty.
        /// </summary>
        /// <remarks>Use this method to reset the board to its initial state before starting a new game or
        /// round.</remarks>
        public void ClearBoard()
        {
            foreach (var cell in Cells)
                cell.Status = CellStatus.Empty;
        }

        /// <summary>
        /// Initializes the collection of available cells based on the current set of cells.
        /// </summary>
        /// <remarks>Call this method to reset the available cells to match the current contents of the
        /// Cells collection. This is typically used to prepare the available cells for further operations that depend
        /// on their initial state.</remarks>
        public void InitializeAvailableCells()
        {
            _availableCells = new List<Cell>();

            foreach (var cell in Cells)
                _availableCells.Add(cell);
        }

        /// <summary>
        /// Selects and removes a random cell from the collection of available cells.
        /// </summary>
        /// <remarks>Each cell is removed from the collection after being returned, ensuring it will not
        /// be selected again. If called when no cells are available, the method may throw an exception.</remarks>
        /// <returns>A randomly selected cell from the available cells.</returns>
        public Cell GetRandomCell()
        {
            int index = _rand.Next(_availableCells.Count);
            var cell = _availableCells[index];

            // Remove so it won't be picked again
            _availableCells.RemoveAt(index);

            return cell;
        }

        /// <summary>
        /// Attempts to shoot the specified cell and updates its status to indicate a hit or miss.
        /// </summary>
        /// <param name="cell">The cell to shoot. The cell's status will be updated based on whether it contains a ship.</param>
        /// <returns>true if the cell contained a ship and is now marked as hit; otherwise, false.</returns>
        public static bool ShootCell(Cell cell)
        {
            if (cell.Status == CellStatus.Ship)
            {
                cell.Status = CellStatus.Hit;
                return true;
            }

            cell.Status = CellStatus.Miss;
            return false;
        }

        /// <summary>
        /// Attempts to place the specified ship on the board at the given starting row and column.
        /// </summary>
        /// <remarks>The method returns false if the ship would overlap with another ship or its adjacent
        /// cells, or if the placement would exceed the board boundaries.</remarks>
        /// <param name="selectedShip">The ship to place on the board. Cannot be null.</param>
        /// <param name="startRow">The zero-based row index where the ship's placement should begin. Must be within the bounds of the board.</param>
        /// <param name="startColumn">The zero-based column index where the ship's placement should begin. Must be within the bounds of the board.</param>
        /// <returns>true if the ship was successfully placed; otherwise, false.</returns>
        public bool PlaceShip(ShipViewModel selectedShip, int startRow, int startColumn)
        {
            if (selectedShip == null) return false;

            if (startRow < 0 || startColumn < 0 ||
                startRow + selectedShip.Rows > Rows ||
                startColumn + selectedShip.Columns > Columns)
                return false;

            foreach (var cell in selectedShip.Cells)
            {
                int row = startRow + cell.Row;
                int column = startColumn + cell.Column;

                if (GetCell(row, column).Status == CellStatus.Ship ||
                    GetCell(row, column).Status == CellStatus.ShipNeighbour)
                    return false;
            }

            var directions = new (int dx, int dy)[]
            {
                    (1, 0),    // right
                    (-1, 0),   // left
                    (0, 1),    // down
                    (0, -1),   // up
                    (1, 1),    // down-right
                    (-1, -1),  // up-left
                    (1, -1),   // up-right
                    (-1, 1)    // down-left
            };

            foreach (var cell in selectedShip.Cells)
            {
                int row = startRow + cell.Row;
                int column = startColumn + cell.Column;

                foreach (var (dx, dy) in directions)
                {
                    int nr = row + dx;
                    int nc = column + dy;

                    if (nr >= 0 && nr < Rows && nc >= 0 && nc < Columns)
                    {
                        var cellPeek = GetCell(nr, nc);
                        if (cellPeek.Status != CellStatus.Ship)
                            cellPeek.Status = CellStatus.ShipNeighbour;
                    }
                }

                GetCell(row, column).Status = CellStatus.Ship;
            }

            return true;
        }

        /// <summary>
        /// Removes the specified ship from the grid, clearing all cells occupied by the ship.
        /// </summary>
        /// <remarks>After removal, all cells previously occupied by the ship are set to empty, and their
        /// neighboring cells are cleared as well. This method does not throw an exception if the ship is not present on
        /// the grid.</remarks>
        /// <param name="ship">The ship to remove from the grid. Cannot be null.</param>
        public void RemoveShipFromGrid(Ship ship)
        {
            foreach (var cell in ship.Cells)
            {
                GetCell(cell.Row, cell.Column).Status = CellStatus.Empty;
                ClearNeighbours(cell.Row, cell.Column);
            }
        }

        /// <summary>
        /// Detects and returns the ship located at the specified starting cell, including all contiguous ship or hit
        /// cells connected orthogonally.
        /// </summary>
        /// <remarks>Only cells with a status of Ship or Hit are considered part of the detected ship.
        /// Detection is performed in four directions (up, down, left, right) from the starting cell.</remarks>
        /// <param name="startRow">The zero-based row index of the starting cell to begin ship detection. Must be within the bounds of the
        /// board.</param>
        /// <param name="startColumn">The zero-based column index of the starting cell to begin ship detection. Must be within the bounds of the
        /// board.</param>
        /// <returns>A Ship object representing all contiguous ship or hit cells connected to the starting cell. Returns an empty
        /// ship if the starting cell does not contain part of a ship.</returns>
        public Ship DetectShip(int startRow, int startColumn)
        {
            var result = new List<Cell>();
            var visited = new HashSet<(int, int)>();
            var queue = new Queue<(int, int)>();

            queue.Enqueue((startRow, startColumn));
            visited.Add((startRow, startColumn));

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                result.Add(new Cell(x, y));

                // 4-directional neighbors
                var neighbors = new (int, int)[]
                {
                    (x+1, y), (x-1, y),
                    (x, y+1), (x, y-1)
                };

                foreach (var (nx, ny) in neighbors)
                {
                    if (nx < 0 || ny < 0 || nx >= Rows || ny >= Columns)
                        continue;

                    if (visited.Contains((nx, ny)))
                        continue;

                    if (GetCell(nx, ny).Status != CellStatus.Ship && GetCell(nx, ny).Status != CellStatus.Hit)
                        continue;

                    queue.Enqueue((nx, ny));
                    visited.Add((nx, ny));
                }
            }

            return new Ship(result);
        }

        /// <summary>
        /// Identifies and returns all ships currently present on the game board.
        /// </summary>
        /// <remarks>A ship is defined as a contiguous group of cells with a status of <see
        /// cref="CellStatus.Ship"/> or <see cref="CellStatus.Hit"/>. Only ships that are fully or partially present on
        /// the board are included in the result.</remarks>
        /// <returns>A list of <see cref="Ship"/> objects representing each detected ship. The list is empty if no ships are
        /// found.</returns>
        public List<Ship> DetectShips()
        {
            int rows = Rows;
            int cols = Columns;

            bool[,] visited = new bool[rows, cols];
            List<Ship> ships = new();

            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (visited[i, j]) continue;
                    if (GetCell(i, j).Status != CellStatus.Ship && GetCell(i, j).Status != CellStatus.Hit) continue;

                    var cells = new List<Cell>();
                    var queue = new Queue<(int x, int y)>();

                    queue.Enqueue((i, j));
                    visited[i, j] = true;

                    while (queue.Count > 0)
                    {
                        var (x, y) = queue.Dequeue();
                        cells.Add(GetCell(x, y));

                        for (int d = 0; d < 4; d++)
                        {
                            int nx = x + dx[d];
                            int ny = y + dy[d];

                            if (nx < 0 || ny < 0 || nx >= rows || ny >= cols)
                                continue;

                            if (visited[nx, ny])
                                continue;

                            if (GetCell(nx, ny).Status != CellStatus.Ship && GetCell(nx, ny).Status != CellStatus.Hit)
                                continue;

                            visited[nx, ny] = true;
                            queue.Enqueue((nx, ny));
                        }
                    }

                    ships.Add(new Ship(cells));
                }
            }

            return ships;
        }


        /// <summary>
        /// Places all ships from the specified ship set randomly on the board.
        /// </summary>
        /// <remarks>Each ship is placed in a random position and orientation. The method attempts to
        /// place each ship up to a maximum number of times before moving to the next ship. This method is not
        /// thread-safe.</remarks>
        /// <param name="shipSet">The set of ships to be placed randomly on the board. Cannot be null.</param>
        public void PlaceShipsRandomly(ShipSet shipSet)
        {
            var rand = new Random();

            foreach (var ship in shipSet.Ships)
            {
                bool placed = false;
                int attempts = 0;
                var shipVM = new ShipViewModel(ship);


                while (!placed && attempts < 999)
                {
                    int x = rand.Next(0, Rows);
                    int y = rand.Next(0, Columns);

                    int rotations = rand.Next(0, 4);
                    for (int i = 0; i < rotations; i++)
                    {
                        Ship.RotateShip(shipVM.ToShip());

                        shipVM.Rows = shipVM.Cells.Max(c => c.Row) + 1;
                        shipVM.Columns = shipVM.Cells.Max(c => c.Column) + 1;
                    }

                    if (PlaceShip(shipVM, x, y))
                    {
                        placed = true;
                    }

                    attempts++;
                }
            }
        }

        /// <summary>
        /// Creates a new game board of the specified size and places the provided set of ships randomly on the board.
        /// </summary>
        /// <remarks>The random placement ensures that ship positions vary each time the method is called.
        /// This method is useful for initializing a new game with a randomized setup.</remarks>
        /// <param name="size">The size of the game board to create. Must be a positive integer representing both the width and height of
        /// the square board.</param>
        /// <param name="shipSet">The set of ships to place randomly on the board. Each ship in the set will be positioned according to the
        /// game's placement rules.</param>
        /// <returns>A new instance of the GameBoard class with ships placed at random positions.</returns>
        public static GameBoard CreateRandomizedBoard(int size, ShipSet shipSet)
        {
            var board = new GameBoard(size);
            board.PlaceShipsRandomly(shipSet);
            return board;
        }

        /// <summary>
        /// Reveals all neighboring cells for each cell occupied by the specified ship.
        /// </summary>
        /// <param name="ship">The ship whose occupied cells' neighbors will be revealed. Cannot be null.</param>
        public void RevealShip(Ship ship)
        {
            foreach (var cell in ship.Cells)
            {
                RevealNeighbours(cell.Row, cell.Column);
            }
        }

        private static Cell[,] InitCells(int size)
        {
            Cell[,] cells = new Cell[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    cells[i, j] = new Cell(i, j);
                }
            }

            return cells;
        }

        private bool HasAdjacentShip(int row, int column)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx + row < 0 || dx + row >= Rows || dy + column < 0 || dy + column >= Columns)
                        continue;

                    var neighbour = GetCell(row + dx, column + dy);
                    if (neighbour?.Status == CellStatus.Ship)
                        return true;
                }
            }

            return false;
        }

        private void ClearNeighbours(int row, int column)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = row + dx;
                    int ny = column + dy;

                    if (nx < 0 || nx >= Rows || ny < 0 || ny >= Columns)
                        continue;

                    var cell = GetCell(nx, ny);

                    if (cell.Status != CellStatus.ShipNeighbour)
                        continue;

                    if (!HasAdjacentShip(nx, ny))
                        cell.Status = CellStatus.Empty;
                }
            }
        }

        private void RevealNeighbours(int row, int column)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = row + dx;
                    int ny = column + dy;

                    if (nx < 0 || nx >= Rows || ny < 0 || ny >= Columns)
                        continue;

                    var cell = GetCell(nx, ny);

                    if (cell.Status != CellStatus.Hit)
                        cell.Status = CellStatus.Miss;
                }
            }
        }
    }
}
