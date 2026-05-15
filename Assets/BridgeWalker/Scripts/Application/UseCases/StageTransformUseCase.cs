using BridgeWalker.Scripts.Application.DTOs;
using UnityEngine;

namespace BridgeWalker.Scripts.Application.UseCases
{
    public class StageTransformUseCase
    {
        /// <summary>
        /// ワールド座標をグリッド座標に変換する
        /// </summary>
        /// <param name="worldPosition">取得したいグリッドのワールド座標</param>
        /// <returns>整数に丸めたステージ上のグリッド座標</returns>
        public Vector2Int ToGridPosition(Vector3 worldPosition, float bridgeSize)
        {
            if (bridgeSize <= 0) return Vector2Int.zero;
            
            return new Vector2Int(
                Mathf.RoundToInt(worldPosition.x / bridgeSize),
                Mathf.RoundToInt(worldPosition.z / bridgeSize)
            );
        }

        /// <summary>
        /// スタート地点の座標を取得する
        /// </summary>
        /// <param name="stageData">ステージのデータ</param>
        /// <returns>整数に丸めたステージのスタート地点の座標</returns>
        public Vector2Int GetStartPosition(StageData stageData)
        {
            int x = Mathf.Clamp((stageData.width - 1) / 2 + 1, 0, stageData.width - 1);
            int y = Mathf.Clamp(stageData.height - 1, 0, stageData.height - 1);
            return new Vector2Int(x, y);
        }
    }
}