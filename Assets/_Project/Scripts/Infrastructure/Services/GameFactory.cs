using Assets._Project.Scripts.GameLogic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Assets._Project.Scripts.Infrastructure.Services
{
    public interface IGameFactory
    {
        Task<GameObject> CreateCrossNetwork(Vector2 position, Color color);
        Task<GameObject> CreateLobbyWindow();
        Task<GameObject> CreateBoard();
    }

    public class GameFactory : IGameFactory
    {
        public readonly string CROSS = "CrossNetwork";
        public readonly string LOBBY = "LobbyCanvas";
        public readonly string BOARD = "Board";
        public readonly string SPAWN_CONTENT_TAG = "content";

        private readonly DiContainer _container;
        private readonly IAssetProvider _assetProvider;

        public GameFactory(DiContainer container,  IAssetProvider assetProvider)
        {
            _container = container;
            _assetProvider = assetProvider;
        }

        public async Task<GameObject> CreateCrossNetwork(Vector2 position, Color color)
        {
            var prefab = await _assetProvider.InstantiateAsync(CROSS, position);

            if (prefab == null)
            {
                Debug.LogError($"Prefab {CROSS} not found!");
                return null;
            }
            GameObject content = GameObject.FindGameObjectWithTag(SPAWN_CONTENT_TAG);

            var networkObject = prefab.GetComponent<NetworkObject>();
            networkObject.Spawn();
            networkObject.TrySetParent(content, false);

           
            ApplyColorToInstance(prefab, color);

            return prefab;
        }

        public async Task<GameObject> CreateLobbyWindow()
        {
            var prefab = await _assetProvider.InstantiateAsync (LOBBY);
            if (prefab == null)
            {
                Debug.LogError($"Prefab {LOBBY} not found!");
                return null;
            }
             
             
            _container.InjectGameObject(prefab);

            return prefab;
        }

        public async Task<GameObject> CreateBoard()
        {
            var prefab = await _assetProvider.InstantiateAsync(BOARD);
            if (prefab == null)
            {
                Debug.LogError($"Prefab {BOARD} not found!");
                return null;
            }
              
            _container.InjectGameObject(prefab);
            if (NetworkManager.Singleton.IsServer)
            {
                NetworkObject content = GameObject.FindGameObjectWithTag(SPAWN_CONTENT_TAG).GetComponent<NetworkObject>();

                content.Spawn();

            }

            return prefab;
        }

        private void ApplyColorToInstance(GameObject instance, Color color)
        {  
            var colorable = instance.GetComponent<IColorable>();
            if (colorable != null) 
                colorable.SetColor(color); 
        }
    } 
}