using TriInspector;
using UnityEngine;

namespace StinkySteak.PredictiveInventory
{
    public class SandboxHoverInventory : MonoBehaviour
    {
        private int _hoveredSlotIndex;

        public int HoveredSlotIndex => _hoveredSlotIndex - 1;

        public bool IsHoverValid => _hoveredSlotIndex > 0;

        public void SetHoveredSlotIndex(int hoveredSlotIndex)
            => _hoveredSlotIndex = hoveredSlotIndex + 1;

        public int Value;

        private void Update()
        {
            Value = HoveredSlotIndex;
        }
    }
}