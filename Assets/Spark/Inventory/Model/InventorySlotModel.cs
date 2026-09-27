using R3;

namespace Spark.Inventory.Model
{
    public class InventorySlotModel
    {
        private readonly ReactiveProperty<string> _id = new(null);
        private readonly ReactiveProperty<int> _amount = new(0);

        public ReadOnlyReactiveProperty<string> IdObservable => _id;
        public ReadOnlyReactiveProperty<int> AmountObservable => _amount;

        public string Id => _id.CurrentValue;
        public int Amount => _amount.CurrentValue;
        
        public bool IsEmpty => _amount.CurrentValue == 0 || string.IsNullOrEmpty(_id.CurrentValue);
        
        internal void Set(string id, int amount = 1)
        {
            _id.Value = id;
            _amount.Value = amount;
        }
        
        internal void AddToStack(int amount = 1) => _amount.Value += amount;
        
        internal void RemoveFromStack(int amount = 1) => _amount.Value -= amount;
        
        internal void Clear()
        {
            _id.Value = null;
            _amount.Value = 0;
        }
    }
}