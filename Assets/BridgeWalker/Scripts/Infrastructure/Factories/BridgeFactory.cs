using System;
using BridgeWalker.Scripts.Domain.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace BridgeWalker.Scripts.Infrastructure.Factories
{
    public class BridgeFactory : IDisposable
    {
        private const string BridgePrefabKey = "Bridge";

        private GameObject _bridgePrefab;

        public BridgeFactory()
        {
            InitializeAsync().Forget();
        }
        
        public async UniTask InitializeAsync()
        {
            var loadHandle = Addressables.LoadAssetAsync<GameObject>(BridgePrefabKey);
            await loadHandle.ToUniTask();
            _bridgePrefab = loadHandle.Result;
        }

        public GameObject Create(Bridge bridge, Transform parent)
        {
            if (_bridgePrefab == null)
            {
                Debug.LogError("[BridgeFactory] Bridge prefab is not loaded. Call InitializeAsync() before creating bridges.");
                return null;
            }
            
            Vector3 position = new Vector3(bridge.X * bridge.BridgeSize, 0, bridge.Y * bridge.BridgeSize);
            return Object.Instantiate(_bridgePrefab, position, Quaternion.identity, parent);
        }

        public void Dispose()
        {
            if (_bridgePrefab != null)
            {
                Addressables.Release(_bridgePrefab);
                _bridgePrefab = null;
            }
        }
    }
}