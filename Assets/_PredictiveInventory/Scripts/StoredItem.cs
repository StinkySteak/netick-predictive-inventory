namespace StinkySteak.PredictiveInventory
{
    public struct StoredItem
    {
        public int ItemId;
        public int Quantity;

        public StoredItem(int itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }

        public bool IsValid => ItemId > 0 && Quantity > 0;
        public static StoredItem None => default;
    }
}