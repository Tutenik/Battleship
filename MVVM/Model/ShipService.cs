using System.IO;
using System.Text.Json;
using System.Windows;


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

                    string jsonPath = Path.Combine(path, "Default.json");

                    if (!File.Exists(jsonPath))
                    {
                        File.WriteAllText(jsonPath, GetDefaultJson());
                    }

                    MessageBox.Show(
                                $"A new ship set file has been created at:\n{path}\n\n" +
                                $"If you previously had custom ship sets, they may not be included.\n" +
                                $"Make sure the Resources folder is located in the same directory as the application (.exe).\n\n" +
                                $"To restore your old ship sets, replace the newly created Resources folder with your original one.",
                                "Information",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
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

            try
            {
                File.WriteAllText(Path.Combine(FolderPath, path), json);
                MessageBox.Show("Ship set saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving ships: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private static string GetDefaultJson()
        {
            return @"[
  {
    ""Positions"": [
      { ""X"": 0, ""Y"": 0 },
      { ""X"": 0, ""Y"": 1 },
      { ""X"": 0, ""Y"": 2 },
      { ""X"": 0, ""Y"": 3 }
    ]
  },
  {
    ""Positions"": [
      { ""X"": 0, ""Y"": 0 },
      { ""X"": 1, ""Y"": 0 },
      { ""X"": 2, ""Y"": 0 },
      { ""X"": 3, ""Y"": 0 },
      { ""X"": 4, ""Y"": 0 },
      { ""X"": 5, ""Y"": 0 }
    ]
  },
  {
    ""Positions"": [
      { ""X"": 0, ""Y"": 0 },
      { ""X"": 0, ""Y"": 1 },
      { ""X"": 0, ""Y"": 2 }
    ]
  },
  {
    ""Positions"": [
      { ""X"": 0, ""Y"": 0 },
      { ""X"": 0, ""Y"": 1 }
    ]
  },
  {
    ""Positions"": [
      { ""X"": 0, ""Y"": 0 },
      { ""X"": 0, ""Y"": 1 }
    ]
  },
  {
    ""Positions"": [
      { ""X"": 0, ""Y"": 0 },
      { ""X"": 1, ""Y"": 0 }
    ]
  },
  {
    ""Positions"": [
      { ""X"": 0, ""Y"": 0 }
    ]
  }
]";
        }
    }

}
