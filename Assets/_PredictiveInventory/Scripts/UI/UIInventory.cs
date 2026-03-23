using Netick.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace StinkySteak.PredictiveInventory
{
    public class UIInventory : MonoBehaviour, INetickSceneLoaded
    {
        [SerializeField] private RectTransform _canvasRectTransform;
        [SerializeField] private UIListingInventory _uiListingPrefab;
        private UIListingInventory[] _uiListings;
        [SerializeField] private Transform _uiListingSlot;
        [SerializeField] private UIListingOnHandItem _uiListingOnHandItem;
        [SerializeField] private ItemEntryContainerSO _itemEntryContainerSO;

        private NetworkSandbox _networkSandbox;
        private PlayerInventory _localPlayerInventory;
        private LocalPlayerManager _localPlayerManager;
        private SandboxHoverInventory _sandboxHoverInventory;
        private SandboxInventorySettings _sandboxInventorySettings;

        public void Initialize(NetworkSandbox sandbox)
        {
            _uiListings = new UIListingInventory[PlayerInventory.InventorySize];

            for (int i = 0; i < PlayerInventory.InventorySize; i++)
            {
                UIListingInventory listing = Instantiate(_uiListingPrefab, _uiListingSlot);

                listing.UpdateProps(null, 0);
                listing.Initialize(this, i);

                _uiListings[i] = listing;
            }

            _networkSandbox = sandbox;
            _localPlayerManager = _networkSandbox.GetComponent<LocalPlayerManager>();
            _sandboxHoverInventory = _networkSandbox.GetComponent<SandboxHoverInventory>();
            _sandboxInventorySettings = _networkSandbox.GetComponent<SandboxInventorySettings>();

            if (_localPlayerManager.TryGetLocalPlayerInventory(out PlayerInventory playerInventory))
            {
                _localPlayerInventory = playerInventory;
                _localPlayerInventory.OnHandItemChanged += OnHandItemChanged;
                _localPlayerInventory.OnInventoryChanged += OnInventoryChanged;
            }
            else
            {
                _localPlayerManager.OnLocalPlayerInventoryRegistered += OnLocalPlayerInventoryRegistered;
            }
        }

        private void OnLocalPlayerInventoryRegistered()
        {
            if (_localPlayerManager.TryGetLocalPlayerInventory(out PlayerInventory playerInventory))
            {
                _localPlayerInventory = playerInventory;
                _localPlayerInventory.OnHandItemChanged += OnHandItemChanged;
                _localPlayerInventory.OnInventoryChanged += OnInventoryChanged;
            }
        }

        private void OnInventoryChanged()
        {
            UpdateAllListingProps();
        }

        private void UpdateAllListingProps()
        {
            for (int i = 0; i < _uiListings.Length; i++)
            {
                UIListingInventory listing = _uiListings[i];

                StoredItem storedItem = _localPlayerInventory.GetItemInfo(i);

                if (storedItem.IsValid)
                {
                    ItemEntrySO itemEntrySO = _itemEntryContainerSO.GetItem(storedItem.ItemId);

                    listing.UpdateProps(itemEntrySO.Icon, storedItem.Quantity);
                    continue;
                }

                listing.UpdateProps(null, 0);
            }
        }

        private void OnHandItemChanged()
        {
            if (_localPlayerInventory.IsOnHandItem)
            {
                ItemEntrySO itemEntrySO = _itemEntryContainerSO.GetItem(_localPlayerInventory.OnHandItem.ItemId);

                _uiListingOnHandItem.SetVisible(true);
                _uiListingOnHandItem.UpdateProps(itemEntrySO.Icon, _localPlayerInventory.OnHandItem.Quantity);
            }
            else
            {
                _uiListingOnHandItem.SetVisible(false);
            }
        }

        public void OnListingClickedPrimary(UIListingInventory uiListing)
        {
            if (_sandboxInventorySettings.InventoryNetworkMode != InventoryNetworkMode.RPC)
                return;

            if (_localPlayerInventory.IsOnHandItem)
                _localPlayerInventory.RPC_PlaceOnHandItemToSlot(uiListing.InventorySlotNumber);
            else
                _localPlayerInventory.RPC_PickSlotToOnHandItem(uiListing.InventorySlotNumber);
        }

        public void OnListingClickedSecondary(UIListingInventory uiListing)
        {
            if (_sandboxInventorySettings.InventoryNetworkMode != InventoryNetworkMode.RPC)
                return;

            if (_localPlayerInventory.IsOnHandItem)
                _localPlayerInventory.RPC_PlaceOnHandItemToSlotOneQuantity(uiListing.InventorySlotNumber);
            else
                _localPlayerInventory.RPC_PickSlotToOnHandItemHalf(uiListing.InventorySlotNumber);
        }

        public void OnPointerEnter(UIListingInventory uiListing)
        {
            if (_sandboxInventorySettings.InventoryNetworkMode != InventoryNetworkMode.Input)
                return;

            _sandboxHoverInventory.SetHoveredSlotIndex(uiListing.InventorySlotNumber);
        }
        public void OnPointerExit(UIListingInventory uiListing)
        {
            if (_sandboxInventorySettings.InventoryNetworkMode != InventoryNetworkMode.Input)
                return;

            _sandboxHoverInventory.SetHoveredSlotIndex(-1);
        }

        private void LateUpdate()
        {
            if (_localPlayerInventory == null) return;

            if (_localPlayerInventory.IsOnHandItem)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, Input.mousePosition, null, out Vector2 localPoint);

                Vector2 position = _canvasRectTransform.TransformPoint(localPoint);

                _uiListingOnHandItem.transform.position = position;
            }
        }

        public void OnSceneLoaded(NetworkSandbox sandbox)
        {
            Initialize(sandbox);
        }
    }
}