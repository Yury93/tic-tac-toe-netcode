using Assets._Project.Scripts;
using Assets._Project.Scripts.Infrastructure.Services;
using Assets._Project.Scripts.Infrastructure.StateMachine;
using Assets._Project.Scripts.Infrastructure.StateMachine.States;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using Zenject;


namespace Assets._Project.Scripts
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private NetworkService _networkService;
        [SerializeField] private BoardView _boardView;
        public override void InstallBindings()
        {
            Container.Bind<LobbyService>().FromNew().AsSingle().NonLazy();
            Container.Bind<NetworkService>().FromInstance(_networkService).AsSingle();
            Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();
            Container.Bind<IGameFactory>().To<GameFactory>().AsSingle();
            Container.Bind<NetworkMediator>().AsSingle().WithArguments(_networkService); 
            BindStateMachine();
            BindStates(); 
            Container.Bind<IBoardModel>().To<BoardModel>().AsTransient();
            Container.Bind<IBoardViewModel>().To<BoardViewModel>().AsTransient();
            Container.Bind<IBoardView>().FromInstance(_boardView).AsSingle(); 
        }
        private void BindStateMachine()
        {
            Container.Bind<IStateFactory>().To<StateFactory>().AsSingle();
            Container.Bind<IStateMachine>().To< StateMachine>().AsSingle();
        } 
        private void BindStates()
        {
            Container.Bind<CreateMenuState>().AsSingle();
            Container.Bind<GameState>().AsSingle();
        } 
    }
}