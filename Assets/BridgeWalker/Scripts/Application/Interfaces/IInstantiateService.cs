using UnityEngine;

namespace BridgeWalker.Scripts.Application.Interfaces
{
    public interface IInstantiateService
    {
        /// <summary>
        /// prefab から GameObject を生成して返す
        /// </summary>
        GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null);

        /// <summary>
        /// prefab から型付きコンポーネントを生成して返す
        /// </summary>
        T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : Component;

        /// <summary>
        /// 生成した GameObject を破棄する
        /// </summary>
        void Destroy(GameObject gameObject);
    }
}