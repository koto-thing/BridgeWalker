using UnityEngine;
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

        [SerializeField] public TextAsset StageData;
        
        private void Start()
        {
            CreateStage();
        }

        private void CreateStage()
        {
            StageData data = JsonUtility.FromJson<StageData>(StageData.text);
            for (int i = 0; i < data.width; i++)
            {
                for (int j = 0; j < data.height; j++)
                {
                    var cellData = data.cells.Find(c => c.x == i && c.y == j);
                    if (cellData is { cellType: "Bridge" })
                    {
                        Vector3 position = new Vector3(i * BridgeSize, 0, j * BridgeSize);
                        Instantiate(BridgePrefab, position, Quaternion.identity, StageParent);
                    }
                }
            }
        }
    }
}