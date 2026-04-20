using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BridgeWalker.Scripts.Infrastructure.Services
{
    public class JsonUtilityService
    {
        public async UniTask<T> ConvertJsonToAnyObjectAsync<T>(string addressablesJsonKey)
        {
            if (string.IsNullOrEmpty(addressablesJsonKey))
            {
                Debug.LogError($"[JsonUtilityService] Invalid addressable Json Key: {addressablesJsonKey}");
                return default;
            }

            var handle = Addressables.LoadAssetAsync<TextAsset>(addressablesJsonKey);
            var jsonAsset = await handle.Task;

            try
            {
                if (jsonAsset is null)
                {
                    Debug.LogError($"[JsonUtilityService] Failed to load Json asset with key: {addressablesJsonKey}");
                    return default;
                }

                var result = JsonUtility.FromJson<T>(jsonAsset.text);
                if (result is null)
                {
                    Debug.LogError($"[JsonUtilityService] Failed to parse Json asset with key: {addressablesJsonKey}");
                    return default;
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonUtilityService] Exception occured while converting Json to object with key: {addressablesJsonKey}");
                Debug.LogException(e);
                return default;
            }
            finally
            {
                Addressables.Release(handle);
            }
        }
        
        public string ConvertAnyObjectToJson<T>(T obj, bool prettyPrint = false)
        {
            if (obj is null)
            {
                Debug.LogError($"[JsonUtilityService] Cannot convert null object to Json");
                return string.Empty;
            }

            string result = JsonUtility.ToJson(obj, prettyPrint);
            return result;
        }
    }
}