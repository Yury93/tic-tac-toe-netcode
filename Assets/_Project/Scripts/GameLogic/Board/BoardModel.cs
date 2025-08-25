using Assets._Project.Scripts.Assets._Project.Scripts;
using UnityEngine;
namespace Assets._Project.Scripts
{
    public interface IBoardModel
    {
        void CreateCell(int row, int col, Vector3 position);
        CellModel GetCell(int row, int col);
        void SetCellState(int row, int col,  StateCell state);
    }

    public class BoardModel : IBoardModel
    {
        private CellModel[,] _cells = new CellModel[Constants.BOARD_SIZE, Constants.BOARD_SIZE];

        public void CreateCell(int row, int col, Vector3 position)
        {
            _cells[row, col] = new CellModel(row, col, position);
        }

        public CellModel GetCell(int row, int col)
        {
            if (IsValidIndex(row, col))
                return _cells[row, col];
            return null;
        }

        public void SetCellState(int row, int col,  StateCell state)
        {
            var cell = GetCell(row, col);
            if (cell != null)
            {
                cell.stateCell = state;
            }
        }

        private bool IsValidIndex(int row, int col)
        {
            return row >= 0 && row < Constants.BOARD_SIZE &&
                   col >= 0 && col < Constants.BOARD_SIZE;
        }
    }
}