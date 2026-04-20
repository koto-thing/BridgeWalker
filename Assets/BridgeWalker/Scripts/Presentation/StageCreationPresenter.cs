using BridgeWalker.Scripts.Application.UseCases;
using VContainer.Unity;

namespace BridgeWalker.Scripts.Presentation
{
    public class StageCreationPresenter : IInitializable
    {
        private readonly StageCreationUseCase _useCase;

        public StageCreationPresenter(StageCreationUseCase useCase)
        {
            _useCase = useCase;
        }
        
        public async void Initialize()
        {
            await _useCase.LoadStage("Stage1");
        }
    }
}