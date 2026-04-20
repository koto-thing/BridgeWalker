using BridgeWalker.Scripts.Infrastructure.Services;
using VContainer;
using VContainer.Unity;

namespace BridgeWalker.Scripts.DI
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Audio関係
            builder.Register<FMODBGMService>(Lifetime.Singleton);
            builder.Register<FMODSEService>(Lifetime.Singleton);
            builder.Register<FMODVCAService>(Lifetime.Singleton);
            
            // File操作関係
            builder.Register<JsonFileCreationService>(Lifetime.Singleton);
            builder.Register<JsonUtilityService>(Lifetime.Singleton);
            
            // UnityのAPI関係
            builder.Register<SceneTransitionService>(Lifetime.Singleton);
        }
    }
}