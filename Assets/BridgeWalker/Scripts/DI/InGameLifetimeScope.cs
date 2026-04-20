using BridgeWalker.Scripts.Infrastructure.Services;
using VContainer;
using VContainer.Unity;

namespace BridgeWalker.Scripts.DI
{
    public class InGameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<StageCreationService>(Lifetime.Scoped);
        }
    }
}