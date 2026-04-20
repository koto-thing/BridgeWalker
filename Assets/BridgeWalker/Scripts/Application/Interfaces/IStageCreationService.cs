using BridgeWalker.Scripts.Domain.Entities;
using Cysharp.Threading.Tasks;

namespace BridgeWalker.Scripts.Application.Interfaces
{
    public interface IStageCreationService
    {
        public UniTask<Stage> LoadStage(string addressableKey);
    }
}