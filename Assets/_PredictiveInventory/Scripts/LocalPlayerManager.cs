using System;
using UnityEngine;

namespace StinkySteak.PredictiveInventory
{
    public class LocalPlayerManager : MonoBehaviour
    {
        private PlayerInventory _localPlayerInventory;
        public PlayerInventory LocalPlayerInventory => _localPlayerInventory;

        public event Action OnLocalPlayerInventoryRegistered;

        public void RegisterLocalPlayerInventory(PlayerInventory playerInventory)
        {
            _localPlayerInventory = playerInventory;
            OnLocalPlayerInventoryRegistered?.Invoke();
        }

        public bool TryGetLocalPlayerInventory(out PlayerInventory playerInventory)
        {
            if (_localPlayerInventory == null)
            {
                playerInventory = null;
                return false;
            }

            playerInventory = _localPlayerInventory;
            return true;
        }
    }
}