using System.Collections;
using System.IO;

namespace Battleship.MVVM.Model
{
    public class ShipSet
    {
        public string Name { get; set; }
        public string ShipSetPath { get; set; }

        private readonly List<Ship> _ships;
        public List<Ship> Ships => _ships;

        public ShipSet(string path)
        {
            _ships = ShipService.LoadShips(path);
            Name = Path.GetFileNameWithoutExtension(path);
            ShipSetPath = path;
        }
    }
}
