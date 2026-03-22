using System.IO;
using System.Text.Json;
using System.Windows;

namespace Battleship.MVVM.Model
{
    public static class ShipService
    {
        public static List<Ship> DetectShips(Cell[,] board)
        {
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);

            bool[,] visited = new bool[rows, cols];
            List<Ship> ships = new();

            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (visited[i, j]) continue;
                    if (board[i, j].Status != CellStatus.Ship) continue;

                    var ship = new Ship();
                    var queue = new Queue<(int x, int y)>();

                    queue.Enqueue((i, j));
                    visited[i, j] = true;

                    while (queue.Count > 0)
                    {
                        var (x, y) = queue.Dequeue();
                        ship.Cells.Add(new Cell(x, y));

                        for (int d = 0; d < 4; d++)
                        {
                            int nx = x + dx[d];
                            int ny = y + dy[d];

                            if (nx < 0 || ny < 0 || nx >= rows || ny >= cols)
                                continue;

                            if (visited[nx, ny])
                                continue;

                            if (board[nx, ny].Status != CellStatus.Ship)
                                continue;

                            visited[nx, ny] = true;
                            queue.Enqueue((nx, ny));
                        }
                    }

                    ships.Add(ship);
                }
            }

            return ships;
        }

        public class ShipDto
        {
            public List<PositionDto> Positions { get; set; } = new();
        }

        public class PositionDto
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public static void SaveShips(string path, List<Ship> ships)
        {
            var dto = ships.Select(ship => new ShipDto
            {
                Positions = ship.Cells
                    .Select(p => new PositionDto { X = p.X, Y = p.Y })
                    .ToList()
            }).ToList();

            var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            MessageBox.Show(path);

            File.WriteAllText(path, json);
        }

        public static List<Ship> LoadShips(string path)
        {
            if (!File.Exists(path))
                return new List<Ship>();

            var json = File.ReadAllText(path);

            var dto = JsonSerializer.Deserialize<List<ShipDto>>(json);

            return [.. dto.Select(d => new Ship([.. d.Positions.Select(p => new Cell(p.X, p.Y))])
         )];
        }
    }
}
