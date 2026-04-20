using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace BridgeWalker.Scripts.Infrastructure.Services
{
    public class FMODBGMService : IDisposable
    {
        private readonly Dictionary<string, EventInstance> _bgmInstances = new Dictionary<string, EventInstance>();

        public void PlayBGM(EventReference eventReference, string key = null)
        {
            if (eventReference.IsNull)
            {
                Debug.LogError($"[FMODBGMService] Invalid Event Reference: {eventReference}");
                return;
            }

            key ??= eventReference.Guid.ToString();

            if (_bgmInstances.ContainsKey(key))
            {
                Debug.LogWarning($"[FMODBGMService] BGM Instance already exists for key: {key}");
                return;
            }

            try
            {
                var instance = RuntimeManager.CreateInstance(eventReference);
                instance.start();
                _bgmInstances[key] = instance;
            }
            catch (Exception e)
            {
                Debug.LogError($"[FMODBGMService] Error playing BGM: {eventReference}");
                Debug.LogException(e);
            }
        }

        public void StopBGM(string key, bool allowFadeOut = true)
        {
            if (string.IsNullOrEmpty(key) || !_bgmInstances.TryGetValue(key, out var instance))
            {
                Debug.LogWarning($"[FMODBGMService] No BGM instance found for key: {key}");
                return;
            }
            
            var stopMode = allowFadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE;
            instance.stop(stopMode);
            instance.release();
            instance.clearHandle();
            _bgmInstances.Remove(key);
        }

        public void StopAllBGM(bool allowFadeOut = true)
        {
            var keys = new List<string>(_bgmInstances.Keys);
            foreach (var key in keys)
            {
                StopBGM(key, allowFadeOut);
            }
        }

        public void PauseBGM(string key)
        {
            if (string.IsNullOrEmpty(key) || !_bgmInstances.TryGetValue(key, out var instance))
            {
                Debug.LogWarning($"[FMODBGMService] No BGM instance found for key: {key}");
                return;
            }
            
            instance.setPaused(true);
        }

        public void ResumeBGM(string key)
        {
            if (string.IsNullOrEmpty(key) || !_bgmInstances.TryGetValue(key, out var instance))
            {
                Debug.LogWarning($"[FMODBGMService] No BGM instance found for key: {key}");
                return;
            }
            
            instance.setPaused(false);
        }

        public void SwitchBGM(string oldKey, EventReference newEventReference, bool allowFadeOut = true)
        {
            StopBGM(oldKey, allowFadeOut);
            PlayBGM(newEventReference, oldKey);
        }

        public void SetBGMParameter(string key, string parameterName, float value)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError($"[FMODBGMService] Invalid key: {key}");
                return;
            }
            
            if (string.IsNullOrEmpty(parameterName))
            {
                Debug.LogError($"[FMODBGMService] Invalid parameter name: {parameterName}");
                return;
            }

            if (_bgmInstances.TryGetValue(key, out var instance))
            {
                instance.setParameterByName(parameterName, value);
            }
            else
            {
                Debug.LogError($"[FMODBGMService] No BGM instance found for key: {key}");
            }
        }

        public bool IsBGMPlaying(string key)
        {
            if (string.IsNullOrEmpty(key) || !_bgmInstances.TryGetValue(key, out var instance))
            {
                return false;
            }

            if (!instance.isValid())
            {
                return false;
            }        
            
            instance.getPlaybackState(out var playbackState);
            return playbackState != PLAYBACK_STATE.STOPPED;
        }
        
        public void Dispose()
        {
            StopAllBGM();
        }
    }
}