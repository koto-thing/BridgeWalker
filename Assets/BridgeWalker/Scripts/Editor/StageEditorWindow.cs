using UnityEditor;
using UnityEngine;
using System.IO;
using BridgeWalker.Scripts.Application.DTOs;
using System.Collections.Generic;

namespace BridgeWalker.Scripts.Editor
{
    public class StageEditorWindow : EditorWindow
    {
        private StageData _stageData;
        private string _filePath = "Assets/BridgeWalker/TextAsset/stage_001.json";
        private Vector2 _scrollPosition;

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

            if (_stageData == null)
            {
                EditorGUILayout.HelpBox("Please load a stage or create a new one.", MessageType.Info);
                if (GUILayout.Button("Create New Stage"))
                {
                    _stageData = new StageData { width = 5, height = 5, stageId = "new_stage", stageName = "New Stage" };
                    InitializeCells();
                }
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("Stage Information", EditorStyles.boldLabel);
            _stageData.stageId = EditorGUILayout.TextField("Stage ID", _stageData.stageId);
            _stageData.stageName = EditorGUILayout.TextField("Stage Name", _stageData.stageName);

            EditorGUI.BeginChangeCheck();
            _stageData.width = EditorGUILayout.IntField("Width", _stageData.width);
            _stageData.height = EditorGUILayout.IntField("Height", _stageData.height);
            if (EditorGUI.EndChangeCheck())
            {
                ResizeCells();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Cells", EditorStyles.boldLabel);

            DrawGrid();

            EditorGUILayout.EndScrollView();
        }

        private void DrawGrid()
        {
            if (_stageData.cells == null) return;

            // Simple grid layout
            float cellSize = 40f;
            float padding = 5f;

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
        }
    }
}