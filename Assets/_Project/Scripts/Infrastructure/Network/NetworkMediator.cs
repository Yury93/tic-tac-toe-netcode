using Assets._Project.Scripts; 
using R3; 
using System;
using UnityEngine;


namespace Assets._Project.Scripts
{
    public class NetworkMediator : IDisposable
    {
        private NetworkService _networkService;
        public Subject<(int index, StateCell state)> OnCellStateChanged = new();
        public Subject<StateCell> OnCurrentPlayerChanged = new();
        public Subject<MatchResult> OnGameEnded = new();
        private CompositeDisposable _disposables = new();
        public NetworkMediator(NetworkService networkService)
        {
            this._networkService = networkService;
            this._networkService.OnInit.Subscribe(_ => SubscribeToNetworkServiceEvents()).AddTo(_disposables);
        }

        public void SubscribeToNetworkServiceEvents()
        {
            _networkService.CellStates.OnListChanged += (changeEvent) =>
            {
                var state = _networkService.GetCellStateAsEnum(changeEvent.Index);
                OnCellStateChanged.OnNext((changeEvent.Index, state));
            };

            _networkService.CurrentPlayer.OnValueChanged += (old, newPlayer) =>
            {
                var playerState = _networkService.GetCurrentPlayerAsEnum();
                OnCurrentPlayerChanged.OnNext(playerState);
            };

            _networkService.GameEnded.OnValueChanged += (old, ended) =>
            {
                if (ended)
                {
                    var result = _networkService.GetMatchResult();
                    OnGameEnded.OnNext(result);
                }
            };
        }

        public void MakeMove(int cellIndex, StateCell player,Vector3 position)
        {
            int playerStateInt;
            switch (player)
            {
                case StateCell.Player1:
                    playerStateInt = 1;
                    break;
                case StateCell.Player2:
                    playerStateInt = 2;
                    break;
                default:
                    playerStateInt = 0;
                    break;
            }
            _networkService.MakeMoveServerRpc(cellIndex, playerStateInt,position);
        }
        public void ClickRestartButton()
        {
            this._networkService.RestartGameServerRpc();
        }

        public StateCell GetCurrentPlayer()
        {
            return _networkService.GetCurrentPlayerAsEnum();
        }

        public bool IsGameEnded()
        {
            return _networkService.GameEnded.Value;
        }

        public bool IsCellEmpty(int index)
        {
            return _networkService.IsCellEmpty(index);
        }

        public void Dispose()
        {
            OnCellStateChanged?.Dispose();
            OnCurrentPlayerChanged?.Dispose();
            OnGameEnded?.Dispose();
        }
    }
}