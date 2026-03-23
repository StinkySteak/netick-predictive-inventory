using UnityEngine;

namespace StinkySteak.PredictiveInventory
{
    [CreateAssetMenu(fileName = nameof(ItemEntryContainerSO))]
    public class ItemEntryContainerSO : ScriptableObject
    {
        [SerializeField] private ItemEntrySO[] _items;

        public ItemEntrySO GetItem(int itemId)
        {
            foreach (var itemEntry in _items)
            {
                if (itemEntry.Id == itemId)
                {
                    return itemEntry;
                }
            }

            return null;
        }
    }
}