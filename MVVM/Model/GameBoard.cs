using Battleship.MVVM.ViewModel;
namespace Battleship.MVVM.Model
{
    public class GameBoard
    {
        public Cell[,] Cells { get; set; }
        private int _size;

        private Random r = new Random();
        private List<Cell> _availableCells;

        public int Rows => Cells.GetLength(1);
        public int Columns => Cells.GetLength(0);

        public List<Ship> Ships => DetectShips();

        public GameBoard(int size)
        {
            _size = size;
            Cells = InitCells(size);
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

        public void ClearBoard()
        {
            foreach (var cell in Cells)
                cell.Status = CellStatus.Empty;
        }

        public Cell GetCell(int row, int column)
        {
            return Cells[row, column];
        }

        public void InitializeAvailableCells()
        {
            _availableCells = new List<Cell>();

            foreach (var cell in Cells)
                _availableCells.Add(cell);
        }

        public Cell GetRandomCell()
        {
            int index = r.Next(_availableCells.Count);
            var cell = _availableCells[index];

            // Remove so it won't be picked again
            _availableCells.RemoveAt(index);

            return cell;
        }

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

        public void RemoveShipFromGrid(Ship ship)
        {
            foreach (var cell in ship.Cells)
            {
                GetCell(cell.Row, cell.Column).Status = CellStatus.Empty;
                ClearNeighbours(cell.Row, cell.Column);
            }
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

        private bool HasAdjacentShip(int row, int column)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx < 0 || dx >= Rows || dy < 0 || dy >= Rows)
                        continue;

                    var neighbour = GetCell(row + dx, column + dy);
                    if (neighbour?.Status == CellStatus.Ship)
                        return true;
                }
            }

            return false;
        }

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
                        Ship.RotateShip(new Ship(
                            shipVM.Cells.Select(c => new Cell(c.Row, c.Column)).ToList()
                        ));

                        int newRows = shipVM.Cells.Max(c => c.Column) + 1;
                        int newCols = shipVM.Cells.Max(c => c.Row) + 1;

                        shipVM.Rows = newRows;
                        shipVM.Columns = newCols;

                        foreach (var cell in shipVM.Cells)
                        {
                            int temp = cell.Row;
                            cell.Row = cell.Column;
                            cell.Column = temp;
                        }
                    }

                    if (PlaceShip(shipVM, x, y))
                    {
                        placed = true;
                    }

                    attempts++;
                }
            }
        }

        public static GameBoard CreateRandomizedBoard(int size, ShipSet shipSet)
        {
            var board = new GameBoard(size);
            board.PlaceShipsRandomly(shipSet);
            return board;
        }

        public void RevealShip(Ship ship)
        {
            foreach (var cell in ship.Cells)
            {
                RevealNeighbours(cell.Row, cell.Column);
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
