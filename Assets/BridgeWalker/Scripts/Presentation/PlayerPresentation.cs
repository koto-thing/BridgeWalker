using System;
using BridgeWalker.Scripts.Application.UseCases;
using BridgeWalker.Scripts.View;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace BridgeWalker.Scripts.Presentation
{
    public class PlayerPresentation : IInitializable, ITickable, IDisposable
    {
        private readonly PlayerCreationUseCase _playerCreationUseCase;
        private readonly PlayerTransformUseCase _playerTransformUseCase;
        private readonly StageCreationUseCase _stageCreationUseCase;
        private readonly PlayerView _playerView;
        
        private DisposableBag _disposableBag = new();

        public PlayerPresentation(PlayerCreationUseCase playerCreationUseCase,
            PlayerTransformUseCase playerTransformUseCase,
            StageCreationUseCase stageCreationUseCase,
            PlayerView playerView)
        {
            _playerCreationUseCase = playerCreationUseCase;
            _playerTransformUseCase = playerTransformUseCase;
            _stageCreationUseCase = stageCreationUseCase;
            _playerView = playerView;
        }
        
        public async void Initialize()
        {
            await _stageCreationUseCase.LoadStage("Stage1");

            _playerView.PlayerGameObject = await _playerCreationUseCase.CreatePlayerGameObject("PlayerCharacterPrefab", _playerView.transform);
            
            _playerTransformUseCase.PlayerPosition
                .Subscribe(pos =>
                {
                    _playerView.PlayerGameObject.transform.position = pos;
                })
                .AddTo(ref _disposableBag);

            Vector2Int startPosition = _playerTransformUseCase.GetStartPosition();
            _playerTransformUseCase.SetInitialPosition(new Vector3(startPosition.x, 0, startPosition.y));
        }

        public void Tick()
        {
            if (Mouse.current is null || Camera.main is null)
                return;

            if (!Mouse.current.leftButton.wasPressedThisFrame)
                return;
            
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _playerView.BridgeLayer))
            {
                _playerTransformUseCase.MovePlayerCharacter(hit.collider.transform.position);
            }
        }

        public void Dispose()
        {
            _disposableBag.Dispose();
        }
    }
}
