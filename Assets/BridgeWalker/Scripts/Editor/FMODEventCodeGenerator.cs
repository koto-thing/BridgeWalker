using System.Collections.Generic;
using System.IO;
using System.Text;
using FMODUnity;
using UnityEditor;
using UnityEngine;

namespace BridgeWalker.Scripts.Editor
{
    public class FMODEventCodeGenerator : EditorWindow
    {
        private const string OUTPUT_SCRIPT_PATH = "Assets/BridgeWalker/Scripts/Domain/ValueObjects/FMODEventPaths.cs";
        private const string OUTPUT_ASSET_PATH = "Assets/BridgeWalker/SO/FMODEventTable.asset";
        private const string NAMESPACE = "BridgeWalker.Scripts.Domain.ValueObjects";

        private static int _lastEventCount = -1; // 前回のイベント数を記録する
        private static int _lastEventHash = 0;  // 前回のイベントパスのハッシュ

        static FMODEventCodeGenerator()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        [MenuItem("Tools/FMOD/Generate AudioEventTable")]
        private static void GenerateManual()
        {
            Generate();
        }

        private static void OnEditorUpdate()
        {
            // FMODが初期化されていない場合はスキップ
            if (!EventManager.IsInitialized)
                return;

            var events = EventManager.Events;
            var currentCount = events.Count;
            var currentHash = ComputeEventsHash(events);
            
            // イベント数またはハッシュ値が変化していたら再生成
            if (currentCount == _lastEventCount && currentHash == _lastEventHash)
                return;

            _lastEventCount = currentCount;
            _lastEventHash = currentHash;
            
            Debug.Log("[FMODEventCodeGenerator] Events changed. Regenerating...");
            Generate();
        }

        private static int ComputeEventsHash(IEnumerable<EditorEventRef> events)
        {
            var hash = 0;
            foreach (var e in events)
            {
                hash ^= e.Path.GetHashCode();
            }

            return hash;
        }
        
        private static void Generate()
        {
            // FMODのイベントパスをすべて取得
            var eventPaths = GetAllFMODEventPaths();
            if (eventPaths.Count == 0)
            {
                Debug.LogError("[FMODEventCodeGenerator] No FMOD event paths found.");
                return;
            }

            GenerateScript(eventPaths);
            AssetDatabase.Refresh();

            EditorApplication.delayCall += GenerateAssetIfNotExists;
            Debug.Log($"[FMODEventCodeGenerator] Generated {eventPaths.Count} events.");
        }

        private static List<string> GetAllFMODEventPaths()
        {
            var paths = new List<string>();
            
            foreach (var e in EventManager.Events)
            {
                paths.Add(e.Path);
            }
            return paths;
        }

        private static void GenerateScript(List<string> eventPaths)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// THIS FILE IS AUTO-GENERATED. DO NOT EDIT MANUALLY.");
            sb.AppendLine($"// Generated at: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();
            sb.AppendLine("using FMODUnity;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using FMOD.Studio;");
            sb.AppendLine();
            sb.AppendLine($"namespace {NAMESPACE}");
            sb.AppendLine("{");
            sb.AppendLine("    [CreateAssetMenu(fileName = \"AudioEventTable\", menuName = \"Audio/AudioEventTable\")]");
            sb.AppendLine("    public class FMODEventPaths : ScriptableObject");
            sb.AppendLine("    {");

            foreach (var path in eventPaths)
            {
                var fieldName = PathToFieldName(path);
                sb.AppendLine($"        public EventReference {fieldName} = new EventReference() {{ Path = \"{path}\" }};");
                sb.AppendLine();
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            // 出力ディレクトリがなければ作成する
            var dir = Path.GetDirectoryName(OUTPUT_SCRIPT_PATH);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir!);
            }
            
            File.WriteAllText(OUTPUT_SCRIPT_PATH, sb.ToString(), Encoding.UTF8);
            Debug.Log($"[FMODEventCodeGenerator] Generated script: {OUTPUT_SCRIPT_PATH}");
        }

        private static void GenerateAssetIfNotExists()
        {
            if (File.Exists(OUTPUT_ASSET_PATH))
            {
                Debug.LogError("[FMODEventCodeGenerator] Asset already exists: " + OUTPUT_ASSET_PATH);
                return;
            }

            var asset = ScriptableObject.CreateInstance("AudioEventTable");
            if (asset is null)
            {
                Debug.LogError("[FMODEventCodeGenerator] Failed to create AudioEventTable asset.");
                return;
            }

            var dir = Path.GetDirectoryName(OUTPUT_ASSET_PATH);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir!);
            }
            
            AssetDatabase.CreateAsset(asset, OUTPUT_ASSET_PATH);
            AssetDatabase.SaveAssets();
            Debug.Log($"[FMODEventCodeGenerator] Generated asset: {OUTPUT_ASSET_PATH}");
        }

        private static string PathToFieldName(string eventPath)
        {
            var withoutPrefix = eventPath.Replace("event:/", "");
            
            var snakeCase = System.Text.RegularExpressions.Regex.Replace(
                withoutPrefix,
                @"(?<=[a-z0-9])(?=[A-Z])",
                "_"
            );

            return snakeCase
                .Replace("/", "_")
                .Replace(" ", "_")
                .Replace("-", "_")
                .ToUpper();
        }
    }
}