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
    public class StageLifetimeScope : LifetimeScope
    {
        [SerializeField] private StageView stageView;

        protected override void Configure(IContainerBuilder builder)
        {
            // Repositories
            builder.Register<IStageRepository, StageRepository>(Lifetime.Scoped);

            // UseCases
            builder.Register<StageCreationUseCase>(Lifetime.Scoped);
            builder.Register<StageTransformUseCase>(Lifetime.Scoped);

            // Views
            builder.RegisterComponent(stageView);

            // Presenters (EntryPoints)
            builder.RegisterEntryPoint<StagePresenter>();
        }
    }
}