using Netick;
using Netick.Unity;
using System;
using UnityEngine;

namespace StinkySteak.PredictiveInventory
{
    public class PlayerInventory : NetworkBehaviour
    {
        [SerializeField] private ItemEntryContainerSO _itemEntryContainerSO;
        [Networked] private StoredItem _onHandItem { get; set; }

        public StoredItem OnHandItem => _onHandItem;
        public bool IsOnHandItem => _onHandItem.IsValid;

        public const int InventorySize = 20;
        [Networked(size: InventorySize)] private NetworkArray<StoredItem> _storedItems = new NetworkArray<StoredItem>(InventorySize);

        private LocalPlayerManager _localPlayerManager;
        private SandboxHoverInventory _sandboxHoverInventory;

        public event Action OnHandItemChanged;
        public event Action OnInventoryChanged;
        public event Action<int> OnInventoryAtSlotChanged;

        public const int StoredItemMaxStack = 10;

        [OnChanged(nameof(_onHandItem))]
        private void OnChangedOnHandItem(OnChangedData onChangedData)
        {
            OnHandItemChanged?.Invoke();
        }

        [OnChanged(nameof(_storedItems))]
        private void OnChangedInventory(OnChangedData onChangedData)
        {
            int changedArrayIndex = onChangedData.GetArrayChangedElementIndex();

            OnInventoryChanged?.Invoke();
            OnInventoryAtSlotChanged?.Invoke(changedArrayIndex);
        }

        public override void NetworkStart()
        {
            _localPlayerManager = Sandbox.GetComponent<LocalPlayerManager>();
            _sandboxHoverInventory = Sandbox.GetComponent<SandboxHoverInventory>();

            if (IsInputSource)
            {
                _localPlayerManager.RegisterLocalPlayerInventory(this);
            }
        }

        public void QuickAddToInventory(StoredItem newItem)
        {
            int remainingAmount = newItem.Quantity;

            // Add to matching slot first, if there's remaining then add to empty slot

            for (int i = 0; i < InventorySize; i++)
            {
                if (remainingAmount <= 0) return;

                StoredItem storedItem = _storedItems[i];

                if (storedItem.IsValid && storedItem.ItemId == newItem.ItemId)
                {
                    bool isSlotFull = storedItem.Quantity >= StoredItemMaxStack;

                    if (!isSlotFull)
                    {
                        int spaceInSlot = StoredItemMaxStack - storedItem.Quantity;
                        int fillAmount = Mathf.Min(remainingAmount, spaceInSlot);

                        storedItem.Quantity += fillAmount;
                        _storedItems[i] = storedItem;

                        remainingAmount -= fillAmount;
                    }
                }
            }

            for (int i = 0; i < InventorySize; i++)
            {
                if (remainingAmount <= 0) return;

                StoredItem storedItem = _storedItems[i];

                if (!storedItem.IsValid)
                {
                    int spaceInSlot = StoredItemMaxStack - storedItem.Quantity;
                    int fillAmount = Mathf.Min(remainingAmount, spaceInSlot);

                    storedItem.ItemId = newItem.ItemId;
                    storedItem.Quantity += fillAmount;

                    _storedItems[i] = storedItem;

                    remainingAmount -= fillAmount;
                }
            }
        }

        public bool TryGetItemInfo(int index, out StoredItem item)
        {
            item = _storedItems[index];

            if (!item.IsValid)
            {
                return false;
            }

            return true;
        }

        public void PlaceOnHandItemToSlot(int toSlotIndex)
        {
            if (!IsOnHandItem)
                return;

            StoredItem storedItem = _storedItems[toSlotIndex];

            bool isSlotEmpty = !storedItem.IsValid;

            if (isSlotEmpty)
            {
                SetItem(toSlotIndex, _onHandItem);
                SetOnHandItem(StoredItem.None);
                return;
            }

            bool isCompatible = storedItem.ItemId == _onHandItem.ItemId;

            if (isCompatible)
            {
                int spaceInSlot = StoredItemMaxStack - storedItem.Quantity;
                bool canFillAll = spaceInSlot >= _onHandItem.Quantity;

                if (canFillAll)
                {
                    StoredItem updatedOnHandItem = storedItem;
                    updatedOnHandItem.Quantity += _onHandItem.Quantity;

                    SetItem(toSlotIndex, updatedOnHandItem);
                    SetOnHandItem(StoredItem.None);
                }
                else
                {
                    int fillAmount = Mathf.Min(spaceInSlot, OnHandItem.Quantity);
                    StoredItem updatedSlot = storedItem;
                    updatedSlot.Quantity += fillAmount;

                    StoredItem updatedOnHand = _onHandItem;
                    updatedOnHand.Quantity -= fillAmount;

                    // fil partially
                    SetItem(toSlotIndex, updatedSlot);
                    SetOnHandItem(updatedOnHand);
                }
            }
        }

        public void PlaceOnHandItemToSlot(int toSlotIndex, int placeQuantity)
        {
            if (!IsOnHandItem)
                return;

            StoredItem storedItem = _storedItems[toSlotIndex];

            bool isSlotEmpty = !storedItem.IsValid;

            if (isSlotEmpty)
            {
                StoredItem updatedOnHandItem = _onHandItem;
                updatedOnHandItem.Quantity -= placeQuantity;

                int slotItemUpdatedQuantity = placeQuantity;
                StoredItem slotItem = new StoredItem(_onHandItem.ItemId, slotItemUpdatedQuantity);

                SetItem(toSlotIndex, slotItem);
                SetOnHandItem(updatedOnHandItem);
                return;
            }

            bool isCompatible = storedItem.ItemId == _onHandItem.ItemId;

            if (isCompatible)
            {
                int spaceInSlot = StoredItemMaxStack - storedItem.Quantity;
                bool canFillAll = spaceInSlot >= _onHandItem.Quantity;

                if (canFillAll)
                {
                    StoredItem updatedOnHandItem = storedItem;
                    updatedOnHandItem.Quantity += _onHandItem.Quantity;

                    SetItem(toSlotIndex, updatedOnHandItem);
                    SetOnHandItem(StoredItem.None);
                }
                else
                {
                    int fillAmount = Mathf.Min(spaceInSlot, OnHandItem.Quantity);
                    StoredItem updatedSlot = storedItem;
                    updatedSlot.Quantity += fillAmount;

                    StoredItem updatedOnHand = _onHandItem;
                    updatedOnHand.Quantity -= fillAmount;

                    // fil partially
                    SetItem(toSlotIndex, updatedSlot);
                    SetOnHandItem(updatedOnHand);
                }
            }
        }

        public void PickSlotToOnHandItem(int fromSlotIndex)
        {
            if (TryGetItemInfo(fromSlotIndex, out StoredItem item))
            {
                SetOnHandItem(item);
                SetItem(fromSlotIndex, StoredItem.None);
            }
        }

        /// <param name="pickQuantityPercentage">Ranging from 0 to 1</param>
        public void PickSlotToOnHandItem(int fromSlotIndex, float pickQuantityPercentage)
        {
            if (TryGetItemInfo(fromSlotIndex, out StoredItem item))
            {
                int quantity = item.Quantity;
                int pickQuantity = Mathf.RoundToInt(pickQuantityPercentage * quantity);
                PickSlotToOnHandItem(fromSlotIndex, pickQuantity);
            }
        }

        public void PickSlotToOnHandItem(int fromSlotIndex, int pickQuantity)
        {
            if (TryGetItemInfo(fromSlotIndex, out StoredItem item))
            {
                StoredItem updatedOnHandItem = item;
                updatedOnHandItem.Quantity = pickQuantity;

                StoredItem updatedSlotItem = item;
                updatedSlotItem.Quantity -= pickQuantity;

                SetOnHandItem(updatedOnHandItem);
                SetItem(fromSlotIndex, updatedSlotItem);
            }
        }

        /// <param name="consumePercentage">Ranging from 0 to 1</param>
        public void ConsumeAllStoredItem(float consumePercentage)
        {
            for (int i = 0; i < InventorySize; i++)
            {
                StoredItem storedItem = _storedItems[i];

                if (storedItem.IsValid)
                {
                    int consumeAmount = Mathf.RoundToInt(consumePercentage * storedItem.Quantity);

                    storedItem.Quantity -= consumeAmount;
                    _storedItems[i] = storedItem;
                }
            }
        }

        [Rpc(RpcPeers.InputSource, RpcPeers.Owner, isReliable: true)]
        public void RPC_PickSlotToOnHandItem(int fromSlotIndex)
        {
            int pickQuantity = _storedItems[fromSlotIndex].Quantity;
            PickSlotToOnHandItem(fromSlotIndex, pickQuantity);
        }

        [Rpc(RpcPeers.InputSource, RpcPeers.Owner, isReliable: true)]
        public void RPC_PlaceOnHandItemToSlot(int toSlotIndex)
        {
            int placeQuantity = _onHandItem.Quantity;
            PlaceOnHandItemToSlot(toSlotIndex, placeQuantity);
        }

        [Rpc(RpcPeers.InputSource, RpcPeers.Owner, isReliable: true)]
        public void RPC_PickSlotToOnHandItemHalf(int fromSlotIndex)
        {
            int pickQuantity = Mathf.RoundToInt(_storedItems[fromSlotIndex].Quantity * 0.5f);
            PickSlotToOnHandItem(fromSlotIndex, pickQuantity);
        }

        [Rpc(RpcPeers.InputSource, RpcPeers.Owner, isReliable: true)]
        public void RPC_PlaceOnHandItemToSlotOneQuantity(int toSlotIndex)
        {
            PlaceOnHandItemToSlot(toSlotIndex, 1);
        }

        public StoredItem GetItemInfo(int index)
        {
            return _storedItems[index];
        }

        public void SetOnHandItem(StoredItem storedItem)
        {
            _onHandItem = storedItem;
        }

        public void SetItem(int index, StoredItem storedItem)
        {
            _storedItems[index] = storedItem;
        }

        public override void NetworkFixedUpdate()
        {
            if (FetchInput(out PlayerInventoryInput input))
            {
                if (input.ClickPrimary && input.IsHoveringInventorySlot)
                {
                    if (IsOnHandItem)
                    {
                        // place items on that slot
                        PlaceOnHandItemToSlot(input.HoveredInventorySlotNumber);
                    }
                    else
                    {
                        // take items if there is item
                        PickSlotToOnHandItem(input.HoveredInventorySlotNumber);
                    }
                }

                if (input.ClickSecondary && input.IsHoveringInventorySlot)
                {
                    if (IsOnHandItem)
                    {
                        // place items on that slot (put only 1 quantity)
                        PlaceOnHandItemToSlot(input.HoveredInventorySlotNumber, 1);
                    }
                    else
                    {
                        // take half of the items if there is item
                        PickSlotToOnHandItem(input.HoveredInventorySlotNumber, 0.5f);
                    }
                }
            }
        }

        public override void NetworkUpdate()
        {
            if (IsInputSource)
            {
                PlayerInventoryInput input = Sandbox.GetInput<PlayerInventoryInput>();

                input.HoveredInventorySlotNumber = _sandboxHoverInventory.HoveredSlotIndex;
                input.ClickPrimary |= Input.GetMouseButtonDown(0);
                input.ClickSecondary |= Input.GetMouseButtonDown(1);

                Sandbox.SetInput(input);
            }
        }
    }

    public struct PlayerInventoryInput : INetworkInput
    {
        public int HoveredInventorySlotNumber;
        public bool ClickPrimary;
        public bool ClickSecondary;

        public bool IsClickValid => ClickPrimary || ClickSecondary;
        public bool IsHoveringInventorySlot => HoveredInventorySlotNumber >= 0;
    }
}