using Netick;
using Netick.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace StinkySteak.PredictiveInventory
{
    public class MatchManager : NetworkEventsListener
    {
        [SerializeField] private NetworkObject _playerInventoryPrefab;

        public override void OnSceneLoaded(NetworkSandbox sandbox)
        {
            List<INetickSceneLoaded> listeners = sandbox.FindObjectsOfType<INetickSceneLoaded>();

            foreach (var listener in listeners)
            {
                listener.OnSceneLoaded(sandbox);
            }
        }

        public override void OnPlayerJoined(NetworkSandbox sandbox, NetworkPlayerId player)
        {
            if (!Sandbox.IsServer) return;

            Sandbox.NetworkInstantiate(_playerInventoryPrefab, player);
        }
    }
}