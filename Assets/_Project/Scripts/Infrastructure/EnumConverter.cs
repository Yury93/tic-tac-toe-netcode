using UnityEditor;
using UnityEngine;

namespace Assets._Project.Scripts.Infrastructure
{
    public class EnumConverter  
    {
        public static StateCell IntToStateCell(int value)
        {
            return value switch
            {
                0 => StateCell.Empty,
                1 => StateCell.Player1,
                2 => StateCell.Player2,
                _ => StateCell.Empty
            };
        }
        public static MatchResult GetMatchResultAsEnum(int value)
        {
            return value switch
            {
                0 => MatchResult.None,
                1 => MatchResult.Player1Wins,
                2 => MatchResult.Player2Wins,
                3 => MatchResult.Draw,
                _ => MatchResult.None
            };
        }
    }
}