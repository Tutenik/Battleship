using System.IO;
using System.Text.Json;


namespace Battleship.MVVM.Model
{
    /// <summary>
    /// used to load and save ships to json files.
    /// </summary>
    public static class ShipService
    {
        /// <summary>
        /// FolderPath for ship sets
        /// </summary>
        public static string FolderPath
        {
            get
            {
                string path = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Resources",
                    "ShipTemplates");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }

        /// <summary>
        /// Used for data transfer.
        /// </summary>
        public class ShipDto
        {
            public List<PositionDto> Positions { get; set; } = new();
        }

        /// <summary>
        /// used for data transfer.
        /// </summary>
        public class PositionDto
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        /// <summary>
        /// Save ship to json file.
        /// </summary>
        /// <param name="path">Use <see cref="FolderPath"/> + name.json</param>
        /// <param name="ships"></param>
        public static void SaveShips(string path, List<Ship> ships)
        {
            var dto = ships.Select(ship => new ShipDto
            {
                Positions = Ship.GetRelativePositions(ship)
                    .Select(p => new PositionDto { X = p.Row, Y = p.Column })
                    .ToList()
            }).ToList();

            var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(Path.Combine(FolderPath, path), json);
        }

        /// <summary>
        /// Used to load ships from json file.
        /// </summary>
        /// <param name="path">Use <see cref="FolderPath"/> + name.json</param>
        /// <returns>List of ships from json.</returns>
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
