using BridgeWalker.Scripts.Application.Interfaces;
using BridgeWalker.Scripts.Domain.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BridgeWalker.Scripts.Infrastructure.Repositories
{
    public class PlayerCharacterRepository : IPlayerCharacterRepository
    {
        private readonly IAssetLoadService _assetLoadService;
        private readonly IInstantiateService _instantiateService;
        
        public PlayerCharacter CurrentPlayerCharacter { get; set; }

        public PlayerCharacterRepository(IAssetLoadService assetLoadService, IInstantiateService instantiateService)
        {
            _assetLoadService = assetLoadService;
            _instantiateService = instantiateService;
        }
        
        /// <inheritdoc />
        public async UniTask<GameObject> CreatePlayer(string addressableKey, Transform parent)
        {
            CurrentPlayerCharacter = new PlayerCharacter();
            GameObject playerCharacterObject = await _assetLoadService.LoadAssetAsync<GameObject>(addressableKey);
            return _instantiateService.Instantiate(playerCharacterObject, Vector3.zero, Quaternion.identity, parent);
        }
    }
}