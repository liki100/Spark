using System.Collections.Generic;

namespace Spark.Inventory.Model
{
    public class InventoryModel
    {
        public IReadOnlyList<InventorySlotModel> Slots => _slots;
        
        private readonly IMaxStackProvider _maxStackProvider;
        private readonly InventorySlotModel[] _slots;
        
        public InventoryModel(int capacity, IMaxStackProvider maxStackProvider)
        {
            _maxStackProvider = maxStackProvider;
            
            _slots = new InventorySlotModel[capacity];
            for (var index = 0; index < capacity; index++)
                _slots[index] = new InventorySlotModel();
        }

        public bool TryAdd(string id, int amount)
        {
            var maxStack = GetMaxStack(id);
            var remaining = amount;
            var totalFreeCapacity = 0;
            
            foreach (var slot in _slots)
            {
                if (slot.IsEmpty) totalFreeCapacity += maxStack;
                else if (slot.Id == id) totalFreeCapacity += maxStack - slot.Amount;
            }

            if (totalFreeCapacity < amount) return false;
            
            foreach (var slot in _slots)
            {
                if (slot.Id != id) continue;
                
                var freeCount = maxStack - slot.Amount;
                if (freeCount <= 0) continue;
                    
                if (remaining <= freeCount)
                {
                    slot.AddToStack(remaining);
                    return true;
                }
                
                slot.AddToStack(freeCount);
                remaining -= freeCount;
            }

            if (remaining > 0)
            {
                foreach (var slot in _slots)
                {
                    if (slot.IsEmpty == false) continue;
                    
                    if (remaining >= maxStack)
                    {
                        slot.Set(id, maxStack);
                        remaining -= maxStack;
                    }
                    else
                    {
                        slot.Set(id, remaining);
                        remaining = 0;
                    }

                    if (remaining == 0) return true;
                }
            }
            
            return true;
        }

        public bool TryRemove(string id, int amount)
        {
            var totalOwnedAmount = 0;
            var remaining = amount;
            
            foreach (var slot in _slots)
            {
                if (slot.Id == id) totalOwnedAmount += slot.Amount;
            }

            if (totalOwnedAmount < amount) return false;

            foreach (var slot in _slots)
            {
                if (slot.Id == id)
                {
                    if (slot.Amount > remaining)
                    {
                        slot.RemoveFromStack(remaining);
                        return true;
                    }
                    
                    remaining -= slot.Amount;
                    slot.Clear();
                }
            }
            
            return true;
        }

        public void Swap(int fromSlotIndex, int toSlotIndex)
        {
            var fromSlot = _slots[fromSlotIndex];
            var toSlot = _slots[toSlotIndex];
            
            var fromSlotId = fromSlot.Id;
            var fromSlotAmount = fromSlot.Amount;
            
            var toSlotId = toSlot.Id;
            var toSlotAmount = toSlot.Amount;
            
            toSlot.Set(fromSlotId, fromSlotAmount);
            fromSlot.Set(toSlotId, toSlotAmount);
        }

        private int GetMaxStack(string id) => _maxStackProvider.GetMaxStack(id);
    }
}