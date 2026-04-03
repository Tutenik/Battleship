using System.IO;

namespace Battleship.MVVM.Model
{
    public class ShipSet
    {
        /// <summary>
        /// Name of the shipset
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Lie path to ship set.
        /// </summary>
        public string ShipSetPath { get; set; }

        /// <summary>
        /// Ships in ship set
        /// </summary>
        public List<Ship> Ships { get; }

        public ShipSet(string path)
        {
            Ships = ShipService.LoadShips(path);
            Name = Path.GetFileNameWithoutExtension(path);
            ShipSetPath = path;
        }
    }
}
