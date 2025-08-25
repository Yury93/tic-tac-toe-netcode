using Unity.Netcode;
using UnityEngine;
using Assets._Project.Scripts;
using R3;
using Assets._Project.Scripts.Infrastructure;


namespace Assets._Project.Scripts
{
    public class NetworkService : NetworkBehaviour
    {
        public NetworkList<int> CellStates; // 0 = empty
        public NetworkVariable<int> CurrentPlayer = new NetworkVariable<int>(1); // 1 = player1, 2 = player2
        public NetworkVariable<bool> GameEnded = new NetworkVariable<bool>(false);
        public NetworkVariable<int> Winner = new NetworkVariable<int>(0); // 0 = None, 1 = PlayerXWins, 2 = PlayerOWins, 3 = Draw
        public Subject<bool> OnInit = new();
        public Subject<(Vector3, int)> OnMakeMoveServer = new();

        private void Awake()
        {
            CellStates = new NetworkList<int>(new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            OnInit.OnNext(true);
        }
        [ServerRpc]
        public void MakeMoveServerRpc(int cellIndex, int player,Vector3 position)
        {
            if (GameEnded.Value) return;
            if (CellStates[cellIndex] != 0) return;
            if (CurrentPlayer.Value != player) return;
            CellStates[cellIndex] = player;
            OnMakeMoveServer.OnNext((position, player));
            CheckWin();
            if (!GameEnded.Value)
            {
                CurrentPlayer.Value = CurrentPlayer.Value == 1 ? 2 : 1;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RestartGameServerRpc()
        {
            for (int i = 0; i < CellStates.Count; i++)
            {
                CellStates[i] = 0;
            }
            CurrentPlayer.Value = 1;
            GameEnded.Value = false;
            Winner.Value = 0;
        }
        private void CheckWin()
        {
            int[][] winCombos = {
            new[] {0,1,2}, new[] {3,4,5}, new[] {6,7,8}, // rows
            new[] {0,3,6}, new[] {1,4,7}, new[] {2,5,8}, // cols
            new[] {0,4,8}, new[] {2,4,6}                 // diagonals
        };

            foreach (var combo in winCombos)
            {
                int state1 = CellStates[combo[0]];
                int state2 = CellStates[combo[1]];
                int state3 = CellStates[combo[2]];

                if (state1 != 0 && state1 == state2 && state2 == state3)
                {
                    Winner.Value = state1; // 1 = PlayerXWins, 2 = PlayerOWins
                    GameEnded.Value = true;
                    return;
                }
            }

            bool isDraw = true;
            for (int i = 0; i < 9; i++)
            {
                if (CellStates[i] == 0)
                {
                    isDraw = false;
                    break;
                }
            }

            if (isDraw)
            {
                Winner.Value = 3; // Draw
                GameEnded.Value = true;
            }
        }
         
        public StateCell GetCellStateAsEnum(int index) => EnumConverter.IntToStateCell(CellStates[index]);  
        public StateCell GetCurrentPlayerAsEnum()=> EnumConverter.IntToStateCell(CurrentPlayer.Value); 
        public MatchResult GetMatchResult() => EnumConverter.GetMatchResultAsEnum(Winner.Value);
       
        public bool IsCellEmpty(int index)
        {
            return CellStates[index] == 0;
        } 
        public int GetCellState(int index)
        {
            return CellStates[index];
        }

        public int GetCurrentPlayerState()
        {
            return CurrentPlayer.Value;
        }

        public int GetGameResult()
        {
            return Winner.Value;
        }
    }
}