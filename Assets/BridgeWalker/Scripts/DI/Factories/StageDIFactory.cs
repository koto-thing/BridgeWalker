using VContainer;
using VContainer.Unity;

namespace BridgeWalker.Scripts.DI.Factories
{
    /// <summary>
    /// DIコンテナを使用してステージ（LifetimeScope）を生成するファクトリ
    /// </summary>
    public class StageDIFactory
    {
        private readonly IObjectResolver _resolver;
        private readonly StageLifetimeScope _stagePrefab;
        
        public StageDIFactory(IObjectResolver resolver, StageLifetimeScope stagePrefab)
        {
            _resolver = resolver;
            _stagePrefab = stagePrefab;
        }

        public StageLifetimeScope Create()
        {
            return _resolver.Instantiate(_stagePrefab);
        }
    }
}