using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.MVVM.Model
{
    public class Ship
    {
        public int Size => Cells.Count;
        public List<Cell> Cells { get; set; }

        public Ship() { Cells = new List<Cell>(); }

        public Ship(List<Cell> cells)
        {
            Cells = cells;
        }

        public bool IsSunk()
        {
            return Cells.All(cell => cell.Status == CellStatus.Hit);
        }
    }
}
