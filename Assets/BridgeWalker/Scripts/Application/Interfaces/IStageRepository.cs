using BridgeWalker.Scripts.Domain.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BridgeWalker.Scripts.Application.Interfaces
{
    public interface IStageRepository
    {
        public Stage CurrentStage { get; set; }
        
        /// <summary>
        /// 指定されたアドレスキーに基づいてステージデータを非同期に読み込み、CurrentStageプロパティに設定
        /// </summary>
        /// <param name="addressableKey">読み込むステージのアドレスキー</param>
        /// <returns></returns>
        public UniTask LoadStage(string addressableKey);
        public UniTask<GameObject> CreateStage(Transform stageParent);
    }
}