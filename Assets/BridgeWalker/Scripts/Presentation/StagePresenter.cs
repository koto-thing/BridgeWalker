using BridgeWalker.Scripts.Application.UseCases;
using BridgeWalker.Scripts.View;
using VContainer.Unity;

namespace BridgeWalker.Scripts.Presentation
{
    public class StagePresenter : IInitializable
    {
        private readonly StageCreationUseCase _useCase;
        private readonly StageTransformUseCase _stageTransformUseCase;
        private readonly StageView _stageView;

        public StagePresenter(StageCreationUseCase useCase,
            StageTransformUseCase stageTransformUseCase,
            StageView stageView)
        {
            _useCase = useCase;
            _stageTransformUseCase = stageTransformUseCase;
            _stageView = stageView;
        }
        
        public async void Initialize()
        {
            await _useCase.LoadStage("Stage1");
            await _useCase.CreateStage(_stageView.StageRoot);
        }
    }
}