using TriInspector;
using UnityEngine;
namespace StinkySteak.PredictiveInventory
{
    [CreateAssetMenu(fileName = nameof(ItemEntrySO))]
    public class ItemEntrySO : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField][PreviewObject] private Sprite _icon;
        [SerializeField] private string _displayName;
        [SerializeField] private string _key;

        public int Id => _id;
        public Sprite Icon => _icon;
        public string DisplayName => _displayName;
        public string Key => _key;
    }
}