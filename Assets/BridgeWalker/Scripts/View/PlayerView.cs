using UnityEngine;

namespace BridgeWalker.Scripts.View
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] public GameObject PlayerGameObject;
        [SerializeField] public LayerMask BridgeLayer;
    }
}