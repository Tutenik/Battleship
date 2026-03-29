using System.IO;
using System.Text.Json;
using System.Windows;

namespace Battleship.MVVM.Model
{
    public static class ShipService
    {
        public static string FolderPath => Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources",
                "ShipTemplates"
            );

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
                Positions = ship.GetRelativePositions()
                    .Select(p => new PositionDto { X = p.Row, Y = p.Column })
                    .ToList()
            }).ToList();

            var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(Path.Combine(FolderPath, path), json);
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
