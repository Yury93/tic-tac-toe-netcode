using Assets._Project.Scripts.Infrastructure.Services;
using R3; 
using UnityEngine;
using Zenject;

namespace Assets._Project.Scripts.Infrastructure.StateMachine.States
{
    public class GameState : IState
    {
        private IGameFactory _gameFactory;
        private IStateMachine _stateMachine;
        private GameObject _board;
        private CompositeDisposable _dispose = new(); 
        [Inject]
        void Construct(IGameFactory gameFactory, IStateMachine stateMachine, NetworkService networkService )
        {
            _stateMachine = stateMachine;
            _gameFactory = gameFactory;
            networkService.OnMakeMoveServer 
                .Subscribe(move => CreateCross(move))
                .AddTo(_dispose);
        }

        private void CreateCross((Vector3, int) playerMove)
        {
          StateCell stateCell = EnumConverter.IntToStateCell( playerMove.Item2);
            if (stateCell == StateCell.Player1) _gameFactory.CreateCrossNetwork(playerMove.Item1,Color.black);
            else if (stateCell == StateCell.Player2) _gameFactory.CreateCrossNetwork(playerMove.Item1, Color.red);
        }

        public async void Enter()
        {
          _board = await _gameFactory.CreateBoard();
        }

        public void Exit()
        {
        }
    }
}