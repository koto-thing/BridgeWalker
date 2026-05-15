using BridgeWalker.Scripts.Domain.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BridgeWalker.Scripts.Application.Interfaces
{
    public interface IPlayerCharacterRepository
    {
        public PlayerCharacter CurrentPlayerCharacter { get; set; }
        
        /// <summary>
        /// 指定されたアドレスキーに基づいてプレイヤーキャラクターのゲームオブジェクトを非同期に作成
        /// </summary>
        /// <param name="addressableKey">プレイヤーキャラクターのPrefabのアドレスキー</param>
        /// <returns>インスタンス化されたプレイヤーキャラクター</returns>
        public UniTask<GameObject> CreatePlayer(string addressableKey, Transform parent);
    }
}