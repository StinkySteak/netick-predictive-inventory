using UnityEngine;

namespace StinkySteak.PredictiveInventory
{
    public class PlayerInventoryDevCheats : MonoBehaviour
    {
        [SerializeField] private PlayerInventory _playerInventory;
        [SerializeField] private ItemEntrySO[] _itemEntries;

        private void QuickAdd(int entryIndex, int amount = 1)
        {
            ItemEntrySO entrySO = _itemEntries[entryIndex];

            StoredItem item = new StoredItem();
            item.ItemId = entrySO.Id;
            item.Quantity = amount;

            _playerInventory.QuickAddToInventory(item);
        }

        private void Update()
        {
            if (!_playerInventory.IsServer) return;

            int defaultAddAmout = 5;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                QuickAdd(0, defaultAddAmout);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                QuickAdd(1, defaultAddAmout);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                QuickAdd(2, defaultAddAmout);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                QuickAdd(3, defaultAddAmout);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                QuickAdd(4, defaultAddAmout);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                QuickAdd(0, 10);
                QuickAdd(1, 10);
                QuickAdd(2, 10);
                QuickAdd(3, 10);
                QuickAdd(4, 10);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                _playerInventory.ConsumeAllStoredItem(0.5f);
            }
        }
    }
}