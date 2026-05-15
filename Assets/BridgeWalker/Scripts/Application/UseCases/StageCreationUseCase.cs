using BridgeWalker.Scripts.Application.Interfaces;
using BridgeWalker.Scripts.Domain.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BridgeWalker.Scripts.Application.UseCases
{
    public class StageCreationUseCase
    {
        private readonly IStageRepository _stageRepository;

        public StageCreationUseCase(IStageRepository stageRepository)
        {
            _stageRepository = stageRepository;
        }
        
        public async UniTask LoadStage(string addressableKey)
        {
            await _stageRepository.LoadStage(addressableKey);
        }

        public async UniTask CreateStage(Transform stageParent)
        {
            await _stageRepository.CreateStage(stageParent);
        }
    }
}