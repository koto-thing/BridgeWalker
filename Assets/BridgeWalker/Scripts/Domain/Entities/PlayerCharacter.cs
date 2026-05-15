using R3;
using UnityEngine;

namespace BridgeWalker.Scripts.Domain.Entities
{
    public class PlayerCharacter
    {
        public ReactiveProperty<Vector3> CurrentPosition { get; } = new();
    }
}