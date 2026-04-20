using BridgeWalker.Scripts.Domain.ValueObjects;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BridgeWalker.Scripts.Infrastructure.Services
{
    public class SceneTransitionService
    {
        public async UniTaskVoid LoadSceneAsync(SceneLabel sceneLabel)
        {
            var handle = Addressables.LoadSceneAsync(sceneLabel.Value);
            try
            {
                await handle.Task;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SceneTransitionService] Exception occured while loading scene: {e.Message}");
            }
            finally
            {
                Addressables.Release(handle);
            }
        }
    }
}