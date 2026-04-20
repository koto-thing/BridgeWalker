using System.IO;
using BridgeWalker.Scripts.Infrastructure.DTOs;
using BridgeWalker.Scripts.Infrastructure.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BridgeWalker.Scripts.Infrastructure.Repositories
{
    public class FMODSettingsRepository
    {
        private readonly JsonUtilityService _jsonUtilityProvider;
        private readonly JsonFileCreationService _jsonCreator;
        
        private const string SettingsFileName = "FMODSettings.json";
        private string filePath = Path.Combine(UnityEngine.Application.persistentDataPath, SettingsFileName);
        
        public float MasterVolume;
        public float BgmVolume;
        public float SeVolume;

        public FMODSettingsRepository(
            JsonUtilityService jsonUtilityProvider,
            JsonFileCreationService jsonCreator)
        {
            _jsonUtilityProvider = jsonUtilityProvider;
            _jsonCreator = jsonCreator;
            
            LoadSettings().Forget();
        }

        /// <summary>
        /// 現在の設定値をjsonファイルにセーブ
        /// </summary>
        public void SaveSettings()
        {
            var settingsData = new FMODSettingsData
            {
                MasterVolume = MasterVolume,
                BGMVolume = BgmVolume,
                SEVolume = SeVolume
            };

            string jsonText = _jsonUtilityProvider.ConvertAnyObjectToJson(settingsData);
            _jsonCreator.CreateTextToJsonFile(filePath, jsonText);
        }

        /// <summary>
        /// jsonファイルから設定値をロードしてプロパティに反映
        /// </summary>
        public async UniTaskVoid LoadSettings()
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[FMODSettingsRepository] Settings file not found: {filePath}");
                return;
            }

            try
            {
                string jsonText = File.ReadAllText(filePath);
                var settingsData = await _jsonUtilityProvider.ConvertJsonToAnyObjectAsync<FMODSettingsData>(jsonText);
                
                MasterVolume = settingsData.MasterVolume;
                BgmVolume = settingsData.BGMVolume;
                SeVolume = settingsData.SEVolume;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[FMODSettingsRepository] Failed to load settings from file: {filePath}");
                Debug.LogError(e);
            }
        }
    }
}