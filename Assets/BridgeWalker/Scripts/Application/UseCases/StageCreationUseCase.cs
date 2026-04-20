using BridgeWalker.Scripts.Application.Interfaces;
using BridgeWalker.Scripts.Domain.Entities;
using Cysharp.Threading.Tasks;

namespace BridgeWalker.Scripts.Application.UseCases
{
    public class StageCreationUseCase
    {
        private readonly IStageCreationService _stageCreationService;

        public StageCreationUseCase(IStageCreationService stageCreationService)
        {
            _stageCreationService = stageCreationService;
        }
        
        public async UniTask LoadStage(string addressableKey)
        {
            var stage = await _stageCreationService.LoadStage(addressableKey);
        }
    }
}