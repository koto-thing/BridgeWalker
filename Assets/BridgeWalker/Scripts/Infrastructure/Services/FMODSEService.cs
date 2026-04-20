using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UIElements;

namespace BridgeWalker.Scripts.Infrastructure.Services
{
    public class FMODSEService : IDisposable
    {
        private readonly Dictionary<string, EventInstance> _seInstances = new Dictionary<string, EventInstance>();

        public void PlaySE(EventReference eventReference, string key = null)
        {
            if (eventReference.IsNull)
            {
                Debug.LogError($"[FMODSEService] Invalid Event Reference: {eventReference}");
                return;
            }

            key ??= eventReference.Guid.ToString();

            if (_seInstances.ContainsKey(key))
            {
                Debug.LogWarning($"[FMODSEService] SE Instance already exists for key: {key}");
                return;
            }

            try
            {
                var instance = RuntimeManager.CreateInstance(eventReference);
                instance.start();
                _seInstances[key] = instance;
            }
            catch (Exception e)
            {
                Debug.LogError($"[FMODSEService] Error playing SE: {eventReference}");
                Debug.LogException(e);
            }
        }

        public void PlayOneShot(EventReference eventReference, string key = null)
        {
            if (eventReference.IsNull)
            {
                Debug.LogError($"[FMODSEService] Invalid EventReference: {eventReference}");
                return;
            }

            try
            {
                RuntimeManager.PlayOneShot(eventReference);
            }
            catch (Exception e)
            {
                Debug.LogError($"[FMODSEService] Error playing one shot: {eventReference}");
                Debug.LogException(e);
            }
        }

        public void StopSE(string key, bool allowFadeOut = true)
        {
            if (string.IsNullOrEmpty(key) || !_seInstances.TryGetValue(key, out var instance))
            {
                Debug.LogWarning($"[FMODSEService] No SE instance found for key: {key}");
                return;
            }
            
            var stopMode = allowFadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE;
            instance.stop(stopMode);
            instance.release();
            instance.clearHandle();
            _seInstances.Remove(key);
        }

        public void StopAllSE(bool allowFadeOut = true)
        {
            var keys = new List<string>(_seInstances.Keys);
            foreach (var key in keys)
            {
                StopSE(key, allowFadeOut);
            }
        }

        public void PauseSE(string key)
        {
            if (string.IsNullOrEmpty(key) || !_seInstances.TryGetValue(key, out var instance))
            {
                Debug.LogWarning($"[FMODSEService] No SE instance found for key: {key}");
                return;
            }

            instance.setPaused(true);
        }

        public void ResumeSE(string key)
        {
            if (string.IsNullOrEmpty(key) || !_seInstances.TryGetValue(key, out var instance))
            {
                Debug.LogWarning($"[FMODSEService] No SE instance found for key: {key}");
                return;
            }

            instance.setPaused(false);
        }

        public void SetSEParameter(string key, string parameterName, float value)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError($"[FMODSEService] Invalid key: {key}");
                return;
            }

            if (string.IsNullOrEmpty(parameterName))
            {
                Debug.LogError($"[FMODSEService] Invalid parameter name: {parameterName}");
                return;
            }

            if (_seInstances.TryGetValue(key, out var instance))
            {
                instance.setParameterByName(parameterName, value);
            }
            else
            {
                Debug.LogWarning($"[FMODSEService] No SE instance found for key: {key}");
            }
        }

        public void SetSEVolume(string key, float volume)
        {
            if (string.IsNullOrEmpty(key) || !_seInstances.TryGetValue(key, out var instance))
            {
                Debug.LogWarning($"[FMODSEService] No SE instance found for key: {key}");
                return;
            }

            instance.setVolume(Mathf.Clamp01(volume));
        }

        public bool IsSEPlaying(string key)
        {
            if (string.IsNullOrEmpty(key) || !_seInstances.TryGetValue(key, out var instance))
            {
                Debug.LogWarning($"[FMODSEService] No SE instance found for key: {key}");
                return false;
            }

            if (!instance.isValid())
            {
                Debug.LogWarning($"[FMODSEService] Invalid SE instance: {key}");
                return false;
            }
            
            instance.getPlaybackState(out var playbackState);
            return playbackState != PLAYBACK_STATE.STOPPED;
        }
        
        public void Dispose()
        {
            StopAllSE();
        }
    }
}