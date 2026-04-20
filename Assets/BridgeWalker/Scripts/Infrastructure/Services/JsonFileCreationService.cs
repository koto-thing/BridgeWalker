using System.IO;
using UnityEngine;

namespace BridgeWalker.Scripts.Infrastructure.Services
{
    public class JsonFileCreationService
    {
        public void CreateTextToJsonFile(string filePath, string jsonText)
        {
            if (!CheckJsonText(jsonText))
            {
                Debug.LogError($"[JsonFileCreationService] Invalid Json Text: {jsonText}]");
                return;
            }

            File.WriteAllText(filePath, jsonText);
        }

        /// <summary>
        /// 文字列がJson形式かどうかをチェックする
        /// </summary>
        /// <param name="text">チェックする文字列</param>
        /// <returns>Json形式である: true</returns>
        private bool CheckJsonText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogError($"[JsonFileCreationService] Invalid Json Text: {text}");
                return false;
            }

            var trimmed = text.Trim();
            return trimmed.StartsWith("{") && trimmed.EndsWith("}") ||
                   trimmed.StartsWith("[") && trimmed.EndsWith("]");
        }
    }
}