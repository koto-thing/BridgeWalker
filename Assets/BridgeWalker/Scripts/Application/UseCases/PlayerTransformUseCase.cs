using BridgeWalker.Scripts.Application.DTOs;
using BridgeWalker.Scripts.Application.Interfaces;
using BridgeWalker.Scripts.Domain.Entities;
using R3;
using UnityEngine;

namespace BridgeWalker.Scripts.Application.UseCases
{
    public class PlayerTransformUseCase
    {
        private readonly IPlayerCharacterRepository _playerCharacterRepository;
        private readonly IStageRepository _stageRepository;

        public PlayerTransformUseCase(IPlayerCharacterRepository playerCharacterRepository, IStageRepository stageRepository)
        {
            _playerCharacterRepository = playerCharacterRepository;
            _stageRepository = stageRepository;
        }

        public Observable<Vector3> PlayerPosition => _playerCharacterRepository.CurrentPlayerCharacter.CurrentPosition;

        /// <summary>
        /// プレイヤーキャラの初期位置を取得する
        /// </summary>
        /// <returns>初期位置</returns>
        public Vector2Int GetStartPosition()
        {
            Stage stageData = _stageRepository?.CurrentStage;
            if (stageData == null)
            {
                Debug.LogError("[PlayerTransformUseCase] Stage is not loaded. Cannot determine start position.");
                return Vector2Int.zero;
            }

            int x = Mathf.Clamp((stageData.Width - 1) / 2 + 1, 0, stageData.Width - 1);
            int y = Mathf.Clamp(stageData.Height - 1, 0, stageData.Height - 1);
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// 初期位置を設定する
        /// </summary>
        /// <param name="startPosition"></param>
        public void SetInitialPosition(Vector3 startPosition)
        {
            if (_playerCharacterRepository.CurrentPlayerCharacter is null)
                return;
            
            _playerCharacterRepository.CurrentPlayerCharacter.CurrentPosition.Value = startPosition;
        }
        
        /// <summary>
        /// プレイヤーキャラが指定位置に移動できるかを確認
        /// </summary>
        /// <param name="targetWorldPosition"></param>
        /// <returns>true: 移動できる</returns>
        public bool CheckPlayerMoveable(Vector3 targetWorldPosition)
        {
            if (_playerCharacterRepository.CurrentPlayerCharacter is null)
                return false;

            Vector2Int currentPosition = ToGridPosition(_playerCharacterRepository.CurrentPlayerCharacter.CurrentPosition.Value);
            Vector2Int targetPosition = ToGridPosition(targetWorldPosition);

            int dx = Mathf.Abs(currentPosition.x - targetPosition.x);
            int dy = Mathf.Abs(currentPosition.y - targetPosition.y);
            return dx + dy == 1;
        }

        /// <summary>
        /// プレイヤーキャラクターを移動させる
        /// </summary>
        /// <param name="targetWorldPosition">移動先のワールド座標</param>
        public void MovePlayerCharacter(Vector3 targetWorldPosition)
        {
            if (_playerCharacterRepository.CurrentPlayerCharacter is null)
                return;

            if (CheckPlayerMoveable(targetWorldPosition))
            {
                _playerCharacterRepository.CurrentPlayerCharacter.CurrentPosition.Value = targetWorldPosition;
            }
        }

        /// <summary>
        /// ワールド座標をグリッド座標に変換する
        /// ブリッジのサイズを考慮して、ワールド座標をブリッジのサイズで割り、四捨五入して整数に変換する
        /// </summary>
        /// <param name="worldPosition">ワールド座標</param>
        /// <param name="bridgeSize">ブリッジの大きさ</param>
        /// <returns>ステージのグリッド座標</returns>
        private Vector2Int ToGridPosition(Vector3 worldPosition, float bridgeSize = 1.0f)
        {
            return new Vector2Int(
                Mathf.RoundToInt(worldPosition.x / bridgeSize),
                Mathf.RoundToInt(worldPosition.z / bridgeSize)
            );
        }
    }
}