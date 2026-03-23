using UnityEngine;

namespace StinkySteak.PredictiveInventory
{
    public class SandboxInventorySettings : MonoBehaviour
    {
        private InventoryNetworkMode _inventoryNetworkMode;
        public InventoryNetworkMode InventoryNetworkMode => _inventoryNetworkMode;

        public void SetInventoryNetworkMode(InventoryNetworkMode inventoryNetworkMode)
            => _inventoryNetworkMode = inventoryNetworkMode;
    }
}