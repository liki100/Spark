using UnityEngine;

namespace Spark.Inventory.Content
{
    public class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _maxStack;
        
        public string Id => _id;
        public Sprite Icon => _icon;
        public int MaxStack => _maxStack;
    }
}