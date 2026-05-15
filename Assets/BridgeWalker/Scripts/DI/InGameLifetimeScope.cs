using BridgeWalker.Scripts.Application.Interfaces;
using BridgeWalker.Scripts.Application.UseCases;
using BridgeWalker.Scripts.Infrastructure.Repositories;
using BridgeWalker.Scripts.Presentation;
using BridgeWalker.Scripts.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BridgeWalker.Scripts.DI
{
    public class InGameLifetimeScope : LifetimeScope
    {
        [SerializeField] private PlayerView playerView;

        protected override void Configure(IContainerBuilder builder)
        {
            // Repositories
            builder.Register<IPlayerCharacterRepository, PlayerCharacterRepository>(Lifetime.Scoped);
            builder.Register<IStageRepository, StageRepository>(Lifetime.Scoped);
            
            // UseCases
            builder.Register<PlayerCreationUseCase>(Lifetime.Scoped);
            builder.Register<PlayerTransformUseCase>(Lifetime.Scoped);
            builder.Register<StageCreationUseCase>(Lifetime.Scoped);
            
            // Views
            builder.RegisterComponent(playerView);
            
            // Presenters (EntryPoints)
            builder.RegisterEntryPoint<PlayerPresentation>();
        }
    }
}