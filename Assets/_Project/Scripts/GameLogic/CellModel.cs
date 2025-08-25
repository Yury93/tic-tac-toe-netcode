using UnityEngine;


namespace Assets._Project.Scripts
{
    public enum StateCell : int
    {
        Empty = 0,
        Player1 = 1,
        Player2 = 2
    }
    public enum MatchResult 
    {
        None,
        Player1Wins,
        Player2Wins ,
        Draw  
    }
    public class CellModel
    { 
        public readonly int row;
        public readonly int col;
        public readonly Vector3 position;
        public StateCell stateCell = StateCell.Empty;

        public CellModel(int row, int col, Vector3 position)
        {
            this.row = row;
            this.col = col;
            this.position = position;
        }
    }
}