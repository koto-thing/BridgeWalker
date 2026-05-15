using BridgeWalker.Scripts.Application.Interfaces;
using UnityEngine;

namespace BridgeWalker.Scripts.Infrastructure.Services
{
    public class InstantiateService : IInstantiateService
    {
        /// <inheritdoc/>
        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null)
            {
                Debug.LogError("[InstantiateService] prefab is null.");
                return null;
            }

            return Object.Instantiate(prefab, position, rotation, parent);
        }

        /// <inheritdoc/>
        public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : Component
        {
            if (prefab == null)
            {
                Debug.LogError($"[InstantiateService] prefab of type {typeof(T).Name} is null.");
                return null;
            }

            return Object.Instantiate(prefab, position, rotation, parent);
        }

        /// <inheritdoc/>
        public void Destroy(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            Object.Destroy(gameObject);
        }
    }
}