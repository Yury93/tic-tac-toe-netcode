using Assets._Project.Scripts.Infrastructure.StateMachine;
using Assets._Project.Scripts.Infrastructure.StateMachine.States;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace Assets._Project.Scripts
{
    public class JoinLobbyWindow : MonoBehaviour
    {
        public Button joinLobbyButton;
        private LobbyService _lobbyService;
        private IStateMachine _stateMachine;
        [Inject]
        public void Construct(LobbyService lobbyService, IStateMachine stateMachine)
        {
            _lobbyService = lobbyService;
            _stateMachine = stateMachine;  
        }

        private void Start()
        {
            joinLobbyButton.onClick.AddListener(OnJoinLobby);
        }
        private async void OnJoinLobby()
        {
            joinLobbyButton.onClick.RemoveListener(OnJoinLobby);
            await _lobbyService.QuickJoinLobby();
            _stateMachine.Enter<GameState>();
        }
    }
}