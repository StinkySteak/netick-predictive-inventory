using Netick.Unity;

namespace StinkySteak.PredictiveInventory
{
    public interface INetickSceneLoaded
    {
        void OnSceneLoaded(NetworkSandbox sandbox);
    }
}