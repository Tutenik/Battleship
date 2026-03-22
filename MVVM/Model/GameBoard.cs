using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.MVVM.Model
{
    public class GameBoard
    {
        public Cell[,] Cells { get; set; }
        private int _size;

        public GameBoard(int size)
        {
            _size = size;
            Cells = InitCells(size);
        }

        private Cell[,] InitCells(int size)
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

        public Cell GetCell(int x, int y)
        {
            return Cells[x, y];
        }

        public bool PlaceShip(Ship ship, int x, int y, int rotation)
        {
            if (x + ship.Size > _size) return false;

            // Check if cells are empty
            for (int i = 0; i < ship.Size; i++)
            {
                if (Cells[x + i, y].Status != CellStatus.Empty)
                    return false;
            }

            // Place ship in cells
            for (int i = 0; i < ship.Size; i++)
            {
                Cells[x + i, y].Status = CellStatus.Ship;
                ship.Cells.Add(Cells[x + i, y]);
            }

            return true;
        }

        public void RemoveShip(Ship ship)
        {
            foreach (var cell in ship.Cells)
            {
                cell.Status = CellStatus.Empty;
            }
            ship.Cells.Clear();
        }

        public bool GetFireShot(int x, int y)
        {
            if (Cells[x, y].Status == CellStatus.Ship)
            {
                Cells[x, y].Status = CellStatus.Hit;
                return true; // Hit
            }
            else
            {
                Cells[x, y].Status = CellStatus.Missed;
                return false; // Missed
            }
        }

        internal void Reset()
        {
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Cells[i, j].Status = CellStatus.Empty;
                }
            }
        }
    }
}
