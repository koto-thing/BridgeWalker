using UnityEditor;
using UnityEngine;
using System.IO;
using BridgeWalker.Scripts.Application.DTOs;
using System.Collections.Generic;

namespace BridgeWalker.Scripts.Editor
{
    public class StageEditorWindow : EditorWindow
    {
        private const string PreviewRootName = "[Stage Preview]";
        private const string StartMarkerName = "[Start]";
        private StageData _stageData;
        private string _filePath = "Assets/BridgeWalker/TextAsset/stage_001.json";
        private Vector2 _scrollPosition;
        private bool _previewRefreshRequested;
        private GameObject _bridgePrefab;
        private float _cellSize = 1f;
        private bool _drawScenePreview = true;

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.delayCall -= DelayedRefreshPreview;
            _previewRefreshRequested = false;
            ClearPreviewInHierarchy();
            SceneView.RepaintAll();
        }

        [MenuItem("Window/BridgeWalker/Stage Editor")]
        public static void ShowWindow()
        {
            GetWindow<StageEditorWindow>("Stage Editor");
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            _filePath = EditorGUILayout.TextField("File Path", _filePath);
            if (GUILayout.Button("Load", GUILayout.Width(60)))
            {
                LoadData();
            }
            if (GUILayout.Button("Save", GUILayout.Width(60)))
            {
                SaveData();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            _bridgePrefab = (GameObject)EditorGUILayout.ObjectField("Bridge Prefab", _bridgePrefab, typeof(GameObject), false);
            _cellSize = EditorGUILayout.FloatField("Cell Size", _cellSize);
            _drawScenePreview = EditorGUILayout.Toggle("Scene Preview", _drawScenePreview);
            if (EditorGUI.EndChangeCheck())
            {
                RequestPreviewRefresh();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh Hierarchy Preview"))
            {
                RequestPreviewRefresh();
            }
            if (GUILayout.Button("Clear Preview", GUILayout.Width(120)))
            {
                ClearPreviewInHierarchy();
            }
            EditorGUILayout.EndHorizontal();

            if (_stageData == null)
            {
                EditorGUILayout.HelpBox("Please load a stage or create a new one.", MessageType.Info);
                if (GUILayout.Button("Create New Stage"))
                {
                    _stageData = new StageData { width = 5, height = 5, stageId = "new_stage", stageName = "New Stage" };
                    InitializeCells();
                    RequestPreviewRefresh();
                }
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("Stage Information", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            _stageData.stageId = EditorGUILayout.TextField("Stage ID", _stageData.stageId);
            _stageData.stageName = EditorGUILayout.TextField("Stage Name", _stageData.stageName);
            if (EditorGUI.EndChangeCheck())
            {
                RequestPreviewRefresh();
            }

            EditorGUI.BeginChangeCheck();
            _stageData.width = EditorGUILayout.IntField("Width", _stageData.width);
            _stageData.height = EditorGUILayout.IntField("Height", _stageData.height);
            if (EditorGUI.EndChangeCheck())
            {
                ResizeCells();
                RequestPreviewRefresh();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Cells", EditorStyles.boldLabel);

            DrawGrid();

            EditorGUILayout.Space();
            DrawStartGuideRow();

            EditorGUILayout.EndScrollView();
        }

        private void DrawGrid()
        {
            if (_stageData.cells == null) return;

            // Simple grid layout
            float cellSize = 40f;

            // UI grid: normal order (top of UI corresponds to lower z in Scene, actual Scene placement is inverted below)
            for (int y = 0; y < _stageData.height; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < _stageData.width; x++)
                {
                    var cell = GetCell(x, y);
                    if (cell == null) continue;

                    Color defaultColor = GUI.backgroundColor;
                    GUI.backgroundColor = GetCellColor(cell.cellType);

                    if (GUILayout.Button(GetCellAbbreviation(cell.cellType), GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                    {
                        CycleCellType(cell);
                    }
                    GUI.backgroundColor = defaultColor;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private StageCellDto GetCell(int x, int y)
        {
            return _stageData.cells.Find(c => c.x == x && c.y == y);
        }

        private Color GetCellColor(string type)
        {
            switch (type)
            {
                case "Bridge": return Color.cyan;
                case "Goal": return Color.green;
                case "Empty": return Color.gray;
                default: return Color.white;
            }
        }

        private string GetCellAbbreviation(string type)
        {
            if (string.IsNullOrEmpty(type)) return "?";
            return type.Substring(0, 1);
        }

        private void CycleCellType(StageCellDto cell)
        {
            switch (cell.cellType)
            {
                case "Empty": cell.cellType = "Bridge"; break;
                case "Bridge": cell.cellType = "Goal"; break;
                case "Goal": cell.cellType = "Empty"; break;
                default: cell.cellType = "Empty"; break;
            }

            RequestPreviewRefresh();
        }

        private void DrawStartGuideRow()
        {
            if (_stageData == null || _stageData.width <= 0 || _stageData.height <= 0)
            {
                return;
            }

            var startPosition = GetStartCellPosition();
            EditorGUILayout.LabelField($"Start Guide - bottom row is fixed at Z=0, start cell is ({startPosition.x}, {startPosition.y})", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < _stageData.width; x++)
            {
                var isStart = x == startPosition.x;
                var previousEnabled = GUI.enabled;
                GUI.enabled = false;
                var previousColor = GUI.backgroundColor;
                GUI.backgroundColor = isStart ? new Color(1f, 0.75f, 0.15f) : Color.black;

                var label = isStart ? "Start" : string.Empty;
                GUILayout.Button(label, GUILayout.Width(40f), GUILayout.Height(28f));

                GUI.backgroundColor = previousColor;
                GUI.enabled = previousEnabled;
            }

            EditorGUILayout.EndHorizontal();
        }

        private Vector2Int GetStartCellPosition()
        {
            // Keep the rule: center-right preference as before: (width-1)/2 + 1
            var x = Mathf.Clamp(((_stageData.width - 1) / 2) + 1, 0, _stageData.width - 1);
            var y = Mathf.Clamp(_stageData.height - 1, 0, _stageData.height - 1);
            return new Vector2Int(x, y);
        }

        private void InitializeCells()
        {
            _stageData.cells = new List<StageCellDto>();
            for (int y = 0; y < _stageData.height; y++)
            {
                for (int x = 0; x < _stageData.width; x++)
                {
                    _stageData.cells.Add(new StageCellDto { x = x, y = y, cellType = "Empty" });
                }
            }
        }

        private void ResizeCells()
        {
            var newCells = new List<StageCellDto>();
            for (int y = 0; y < _stageData.height; y++)
            {
                for (int x = 0; x < _stageData.width; x++)
                {
                    var existing = GetCell(x, y);
                    if (existing != null)
                    {
                        newCells.Add(existing);
                    }
                    else
                    {
                        newCells.Add(new StageCellDto { x = x, y = y, cellType = "Empty" });
                    }
                }
            }
            _stageData.cells = newCells;
        }

        private void LoadData()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                _stageData = JsonUtility.FromJson<StageData>(json);
                Debug.Log($"Loaded stage: {_stageData.stageName}");
                RequestPreviewRefresh();
            }
            else
            {
                Debug.LogError($"File not found: {_filePath}");
            }
        }

        private void SaveData()
        {
            if (_stageData == null) return;
            string json = JsonUtility.ToJson(_stageData, true);
            File.WriteAllText(_filePath, json);
            AssetDatabase.ImportAsset(_filePath);
            Debug.Log($"Saved stage to: {_filePath}");
            RequestPreviewRefresh();
        }

        private void RequestPreviewRefresh()
        {
            if (_previewRefreshRequested)
            {
                return;
            }

            _previewRefreshRequested = true;
            EditorApplication.delayCall += DelayedRefreshPreview;
        }

        private void DelayedRefreshPreview()
        {
            EditorApplication.delayCall -= DelayedRefreshPreview;
            _previewRefreshRequested = false;

            if (this == null)
            {
                return;
            }

            try
            {
                RefreshPreviewInHierarchy();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void RefreshPreviewInHierarchy()
        {
            if (_stageData == null)
            {
                ClearPreviewInHierarchy();
                return;
            }

            ClearPreviewInHierarchy();

            var rootName = $"{PreviewRootName} {_stageData.stageName}".Trim();
            var root = new GameObject(rootName);
            Undo.RegisterCreatedObjectUndo(root, "Create Stage Preview");

            // Create rows and populate. For Scene/Game placement we invert Y -> Z mapping (visual arrangement differs from UI).
            for (int y = 0; y < _stageData.height; y++)
            {
                var rowObject = new GameObject($"Row {y}");
                Undo.RegisterCreatedObjectUndo(rowObject, "Create Stage Preview Row");
                rowObject.transform.SetParent(root.transform, false);
                // Place rows so that UI y=0 corresponds to top in hierarchy, but Scene placement is inverted below.
                rowObject.transform.localPosition = new Vector3(0f, 0f, (_stageData.height - 1 - y) * _cellSize);

                for (int x = 0; x < _stageData.width; x++)
                {
                    var cell = GetCell(x, y);
                    if (cell == null || cell.cellType == "Empty")
                    {
                        continue;
                    }

                    if (_bridgePrefab != null && cell.cellType == "Bridge")
                    {
                        var prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(_bridgePrefab);
                        if (prefabInstance != null)
                        {
                            Undo.RegisterCreatedObjectUndo(prefabInstance, "Create Stage Preview Bridge");
                            prefabInstance.name = $"Bridge ({x},{y})";
                            prefabInstance.transform.SetParent(rowObject.transform, false);
                            // Local X is straightforward; rowObject Z already accounts for inverted mapping
                            prefabInstance.transform.localPosition = new Vector3(x * _cellSize, 0f, 0f);
                        }
                    }
                    else
                    {
                        var cellObject = new GameObject($"{cell.cellType} ({x},{y})");
                        Undo.RegisterCreatedObjectUndo(cellObject, "Create Stage Preview Cell");
                        cellObject.transform.SetParent(rowObject.transform, false);
                        cellObject.transform.localPosition = new Vector3(x * _cellSize, 0f, 0f);
                    }
                }
            }

            var startPosition = GetStartCellPosition();
            var startObject = new GameObject(StartMarkerName);
            Undo.RegisterCreatedObjectUndo(startObject, "Create Stage Start Marker");
            startObject.transform.SetParent(root.transform, false);
            // Inverse mapping for scene: UI y -> scene z = (height - 1 - y)
            startObject.transform.localPosition = new Vector3(startPosition.x * _cellSize, 0f, (_stageData.height - 1 - startPosition.y) * _cellSize);

            Selection.activeGameObject = root;
            SceneView.RepaintAll();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!_drawScenePreview || _stageData == null)
            {
                return;
            }

            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

            for (int y = 0; y < _stageData.height; y++)
            {
                for (int x = 0; x < _stageData.width; x++)
                {
                    var cell = GetCell(x, y);
                    var cellType = cell != null ? cell.cellType : "Empty";
                    DrawSceneCellBox(x, y, cellType);
                }
            }

            DrawSceneStartMarker();
        }

        private void DrawSceneCellBox(int x, int y, string cellType)
        {
            // Scene placement: invert UI y -> scene z
            var center = new Vector3(x * _cellSize, 0f, (_stageData.height - 1 - y) * _cellSize);
            var half = _cellSize * 0.5f;
            var points = new[]
            {
                new Vector3(center.x - half, 0.02f, center.z - half),
                new Vector3(center.x - half, 0.02f, center.z + half),
                new Vector3(center.x + half, 0.02f, center.z + half),
                new Vector3(center.x + half, 0.02f, center.z - half),
            };

            var fillColor = GetSceneFillColor(cellType);
            var outlineColor = GetCellColor(cellType);
            Handles.DrawSolidRectangleWithOutline(points, fillColor, outlineColor);
        }

        private void DrawSceneStartMarker()
        {
            var startPosition = GetStartCellPosition();
            var center = new Vector3(startPosition.x * _cellSize, 0.04f, (_stageData.height - 1 - startPosition.y) * _cellSize);
            var half = _cellSize * 0.5f;
            var points = new[]
            {
                new Vector3(center.x - half, 0.04f, center.z - half),
                new Vector3(center.x - half, 0.04f, center.z + half),
                new Vector3(center.x + half, 0.04f, center.z + half),
                new Vector3(center.x + half, 0.04f, center.z - half),
            };

            Handles.DrawSolidRectangleWithOutline(points, new Color(1f, 0.72f, 0f, 0.22f), new Color(1f, 0.72f, 0f, 1f));
            Handles.Label(center + Vector3.up * 0.15f, $"Start ({startPosition.x}, {startPosition.y})");
        }

        private Color GetSceneFillColor(string type)
        {
            switch (type)
            {
                case "Bridge": return new Color(0f, 1f, 1f, 0.25f);
                case "Goal": return new Color(0f, 1f, 0f, 0.25f);
                case "Empty": return new Color(0.5f, 0.5f, 0.5f, 0.12f);
                default: return new Color(1f, 1f, 1f, 0.12f);
            }
        }

        private void ClearPreviewInHierarchy()
        {
            var previewCandidates = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            var targets = new List<GameObject>();

            foreach (var candidate in previewCandidates)
            {
                if (candidate == null)
                {
                    continue;
                }

                if (!candidate.name.StartsWith(PreviewRootName))
                {
                    continue;
                }

                targets.Add(candidate);
            }

            foreach (var target in targets)
            {
                if (target != null)
                {
                    Undo.DestroyObjectImmediate(target);
                }
            }

            SceneView.RepaintAll();
        }
    }
}