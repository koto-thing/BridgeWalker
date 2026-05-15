using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BridgeWalker.Scripts.Application.Interfaces
{
    public interface IAssetLoadService
    {
        /// <summary>
        /// 指定されたキーに基づいてアセットを非同期にロード
        /// </summary>
        /// <param name="key">アセットのキー</param>
        /// <typeparam name="T">ロードするアセットの型</typeparam>
        /// <returns>ロードされたアセットのオブジェクト</returns>
        UniTask<T> LoadAssetAsync<T>(string key) where T : Object;
        
        /// <summary>
        /// ロードされたアセットを解放
        /// </summary>
        /// <param name="asset">開放するアセット</param>
        /// <typeparam name="T">開放するアセットの型</typeparam>
        void ReleaseAsset<T>(T asset) where T : Object;
    }
}
