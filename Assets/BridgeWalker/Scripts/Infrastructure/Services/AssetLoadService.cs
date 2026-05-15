using BridgeWalker.Scripts.Application.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BridgeWalker.Scripts.Infrastructure.Services
{
    public class AssetLoadService : IAssetLoadService
    {
        public async UniTask<T> LoadAssetAsync<T>(string key) where T : Object
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError($"[AssetLoadService] Asset key is null or empty.");
                return null;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            T asset = await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return asset;
            }

            Debug.LogError($"[AssetLoadService] Failed to load asset with key: {key}. Status: {handle.Status}");
            Addressables.Release(handle);
            return null;
        }

        public void ReleaseAsset<T>(T asset) where T : Object
        {
            if (asset == null)
            {
                return;
            }
            
            Addressables.Release(asset);
        }
    }
}