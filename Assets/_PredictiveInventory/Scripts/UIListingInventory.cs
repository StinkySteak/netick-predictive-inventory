using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StinkySteak.PredictiveInventory
{
    public class UIListingInventory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TMP_Text _quantity;

        private int _inventorySlotNumber;
        public int InventorySlotNumber => _inventorySlotNumber;

        private UIInventory _uiInventory;

        public void Initialize(UIInventory uiInventory, int slotNumber)
        {
            _uiInventory = uiInventory;
            _inventorySlotNumber = slotNumber;
        }

        public void UpdateProps(Sprite itemIcon, int quantity)
        {
            _itemIcon.sprite = itemIcon;

            bool isValid = quantity > 0;

            _itemIcon.gameObject.SetActive(isValid);
            _quantity.gameObject.SetActive(isValid);

            if (isValid)
            {
                _quantity.SetText("{0}", quantity);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _uiInventory.OnPointerExit(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _uiInventory.OnPointerEnter(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                _uiInventory.OnListingClickedPrimary(this);
            if (eventData.button == PointerEventData.InputButton.Right)
                _uiInventory.OnListingClickedSecondary(this);
        }
    }
}