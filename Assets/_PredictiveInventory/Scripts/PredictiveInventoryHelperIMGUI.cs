using Netick.Unity;
using UnityEngine;

namespace StinkySteak.PredictiveInventory
{
    public class PredictiveInventoryHelperIMGUI : MonoBehaviour, INetickSceneLoaded
    {
        private const int PANEL_WIDTH = 250;
        private const int PANEL_HEIGHT = 300;
        private const int PADDING = 10;

        private NetworkSandbox _networkSandbox;
        private SandboxInventorySettings _sandboxInventorySettings;

        private void OnGUI()
        {
            if (_networkSandbox == null) return;

            if (!_networkSandbox.IsVisible) return;

            // Position panel to bottom left
            float x = PADDING;
            float y = Screen.height - PANEL_HEIGHT - PADDING;

            GUILayout.BeginArea(new Rect(x, y, PANEL_WIDTH, PANEL_HEIGHT), GUI.skin.box);

            // Current value label
            GUILayout.Label($"Inventory Mode: {_sandboxInventorySettings.InventoryNetworkMode}");

            GUILayout.Space(5);

            // Buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Mode to Input")) _sandboxInventorySettings.SetInventoryNetworkMode(InventoryNetworkMode.Input);
            if (GUILayout.Button("Set Mode to RPC")) _sandboxInventorySettings.SetInventoryNetworkMode(InventoryNetworkMode.RPC);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // Shortcuts label
            GUILayout.Label("Shortcuts:");
            GUILayout.Label("[Q] Add Consume 50% all items");
            GUILayout.Label("[W] Add 10x of each items");
            GUILayout.Label("[1] Add Item entry a");
            GUILayout.Label("[2] Add Item entry b");
            GUILayout.Label("[3] Add Item entry c");
            GUILayout.Label("[4] Add Item entry d");
            GUILayout.Label("[5] Add Item entry e");

            GUILayout.EndArea();
        }

        public void OnSceneLoaded(NetworkSandbox sandbox)
        {
            _networkSandbox = sandbox;
            _sandboxInventorySettings = _networkSandbox.GetComponent<SandboxInventorySettings>();
        }
    }
}