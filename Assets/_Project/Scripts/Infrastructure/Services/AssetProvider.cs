using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets._Project.Scripts.Infrastructure.Services
{
    public interface IAssetProvider
    {
        Task<GameObject> InstantiateAsync(string name);
        Task<GameObject> InstantiateAsync(string name, Vector3 position); 
        Task<GameObject> InstantiateNetworkObjectAsync(string name, bool checkForNetworkIdentity = true);
        Task<T> LoadAsync<T>(string name) where T : class;
    }
    public class AssetProvider : IAssetProvider
    {

        public Dictionary<string, AsyncOperationHandle> _handleResources = new Dictionary<string, AsyncOperationHandle>();
        public Dictionary<string, AsyncOperationHandle> _cachedResources = new Dictionary<string, AsyncOperationHandle>();
        public AssetProvider()
        {
            Addressables.InitializeAsync();
        }

        public async Task<GameObject> InstantiateAsync(string name)
        {
            var task = LoadAsync<GameObject>(name);
            await task;
            var result = task.Result;
            var go = GameObject.Instantiate(result);
            return go;
        }
        public async Task<GameObject> InstantiateAsync(string name, Vector3 position)
        {
            var task = LoadAsync<GameObject>(name);
            await task;
            var result = task.Result;
            var go = GameObject.Instantiate(result);
            go.transform.position = position;
            return go;
        }
      

        public async Task<T> LoadAsync<T>(string name) where T : class
        {
            if (_cachedResources.ContainsKey(name))
            {
                return _cachedResources[name].Result as T;
            }
            if (_handleResources.ContainsKey(name))
            { 
                return _handleResources[name].Result as T;
            }
            var locations = await Addressables.LoadResourceLocationsAsync(name).Task;
            if (locations == null || locations.Count == 0)
            {
                Debug.LogError($"error {name} ");
                return null;
            }



            var handle = Addressables.LoadAssetAsync<T>(name);
            _handleResources[name] = handle;
            handle.Completed += (asset) =>
            {
                _cachedResources[name] = asset;
            };
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {

                return handle.Result;
            }
            else
            {
                Debug.LogError($" Async oper load asset {name} = failed");
                return null;
            }
        }
        public async Task<GameObject> InstantiateNetworkObjectAsync(string name, bool checkForNetworkIdentity = true)
        {
            GameObject prefab = await LoadAsync<GameObject>(name);

            if (prefab == null)
            {
                Debug.LogError($" {name} = null");
                return null;
            }

            if (checkForNetworkIdentity)
            {
                NetworkObject networkObject = prefab.GetComponent<NetworkObject>();
                if (networkObject == null)
                {
                    
                    return null;
                }
            }

            GameObject instance = GameObject.Instantiate(prefab);
             
            return instance;
        }
    }
}