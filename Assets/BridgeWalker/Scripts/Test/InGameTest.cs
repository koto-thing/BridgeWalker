using UnityEngine;
using UnityEngine.InputSystem;
using BridgeWalker.Scripts.Application.DTOs;

namespace BridgeWalker.Scripts.Test
{
    public class InGameTest : MonoBehaviour
    {
        [Header("Stage Settings")]
        [SerializeField] public float BridgeSize = 1.0f;

        [Header("Stage Object Settings")]
        [SerializeField] public Transform StageParent;
        [SerializeField] public GameObject BridgePrefab;
        [SerializeField] public LayerMask BridgeLayer;

        [Header("Player Character Settings")] 
        [SerializeField] public GameObject PlayerCharacter;
        
        [SerializeField] public TextAsset StageData;

        public StageData createdStageData;
        private GameObject createdPlayerCharacter;
        
        private void Start()
        {
            CreateStage();
            CreatePlayerCharacter();
        }

        private void Update()
        {
            MovePlayerCharacter();
        }

        private void CreateStage()
        {
            createdStageData = JsonUtility.FromJson<StageData>(StageData.text);
            for (int i = 0; i < createdStageData.width; i++)
            {
                for (int j = 0; j < createdStageData.height; j++)
                {
                    var cellData = createdStageData.cells.Find(c => c.x == i && c.y == j);
                    if (cellData is { cellType: "Bridge" })
                    {
                        Vector3 position = new Vector3(i * BridgeSize, 0, j * BridgeSize);
                        Instantiate(BridgePrefab, position, Quaternion.identity, StageParent);
                    }
                }
            }
        }

        private void CreatePlayerCharacter()
        {
            Vector2Int startPosition = GetStartPosition(createdStageData);
            createdPlayerCharacter = Instantiate(PlayerCharacter, new Vector3(startPosition.x * BridgeSize, 0, startPosition.y * BridgeSize), Quaternion.identity);
        }
        
        private void MovePlayerCharacter()
        {
            if (Mouse.current is null || Camera.main is null)
                return;

            if (!Mouse.current.leftButton.wasPressedThisFrame)
                return;
            
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, BridgeLayer))
            {
                if (CheckPlayerMoveable(hit.collider.transform.position))
                {
                    Vector3 position = hit.collider.transform.position;
                    createdPlayerCharacter.transform.position = position;
                }
            }
        }

        private bool CheckPlayerMoveable(Vector3 targetWorldPosition)
        {
            if (createdPlayerCharacter is null || BridgeSize <= 0f)
            {
                return false;
            }

            Vector2Int currentPosition = ToGridPosition(createdPlayerCharacter.transform.position);
            Vector2Int targetPosition = ToGridPosition(targetWorldPosition);

            int dx = Mathf.Abs(currentPosition.x - targetPosition.x);
            int dy = Mathf.Abs(currentPosition.y - targetPosition.y);
            return dx + dy == 1;
        }

        private Vector2Int ToGridPosition(Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.RoundToInt(worldPosition.x / BridgeSize),
                Mathf.RoundToInt(worldPosition.z / BridgeSize)
            );
        }

        private Vector2Int GetStartPosition(StageData stageData)
        {
            int x = Mathf.Clamp((stageData.width - 1) / 2 + 1, 0, stageData.width - 1);
            int y = Mathf.Clamp(stageData.height - 1, 0, stageData.height - 1);
            return new Vector2Int(x, y);
        }
    }
}