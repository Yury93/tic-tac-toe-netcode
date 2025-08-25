using Assets._Project.Scripts.Infrastructure.Services; 
using UnityEngine;
using Zenject;

namespace Assets._Project.Scripts.Infrastructure.StateMachine.States
{
    public class CreateMenuState : IState
    {
        private IGameFactory _gameFactory;
        private IStateMachine _stateMachine;
        private GameObject _lobbyWindow;
        [Inject]
        void Construct(IGameFactory gameFactory, IStateMachine stateMachine )
        {
            _stateMachine = stateMachine;
            _gameFactory = gameFactory; 
        }
        public async void Enter()
        {
          _lobbyWindow = await _gameFactory.CreateLobbyWindow();
        } 

        public void Exit()
        { 
            GameObject.Destroy(_lobbyWindow);
        }
    }
}