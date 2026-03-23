using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StinkySteak.PredictiveInventory
{
    public class UIListingOnHandItem : MonoBehaviour
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TMP_Text _quantity;

        private UIInventory _uiInventory;

        public void Initialize(UIInventory uiInventory)
        {
            _uiInventory = uiInventory;
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void UpdateProps(Sprite itemIcon, int quantity)
        {
            _itemIcon.sprite = itemIcon;
            _quantity.SetText("{0}", quantity);
        }
    }
}