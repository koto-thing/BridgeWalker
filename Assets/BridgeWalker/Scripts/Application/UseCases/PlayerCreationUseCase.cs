using BridgeWalker.Scripts.Application.Interfaces;
using BridgeWalker.Scripts.Domain.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BridgeWalker.Scripts.Application.UseCases
{
    public class PlayerCreationUseCase
    {
        private readonly IPlayerCharacterRepository _playerCharacterRepository;

        public PlayerCreationUseCase(IPlayerCharacterRepository playerCharacterRepository)
        {
            _playerCharacterRepository = playerCharacterRepository;
        }
        
        public async UniTask<GameObject> CreatePlayerGameObject(string addressableKey, Transform parent)
        {
            return await _playerCharacterRepository.CreatePlayer(addressableKey, parent);
        }
    }
}