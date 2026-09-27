using System;
using System.Collections.Generic;
using Spark.Inventory.Content;
using Spark.Inventory.Model;

namespace Spark.Inventory.ViewModel
{
    public class InventoryViewModel : IDisposable
    {
        public IReadOnlyList<InventorySlotViewModel> Slots { get; }
        
        private readonly InventoryModel _model;
        private int? _selectedIndex;

        public InventoryViewModel(InventoryModel model, ItemLibrary library)
        {
            _model = model;
            
            var slots = new InventorySlotViewModel[_model.Slots.Count];
            for (var index = 0; index < slots.Length; index++)
            {
                slots[index] = new InventorySlotViewModel(model.Slots[index], library, OnSlotClicked, index);
            }
            Slots = slots;
        }
        
        public void Dispose()
        {
            foreach (var slot in Slots)
                slot.Dispose();
        }
        
        private void OnSlotClicked(int clickedIndex)
        {
            if (_selectedIndex.HasValue)
            {
                if (_selectedIndex.Value != clickedIndex)
                    _model.Swap(_selectedIndex.Value, clickedIndex);

                Slots[_selectedIndex.Value].SetSelected(false);
                _selectedIndex = null;
            }
            else
            {
                _selectedIndex = clickedIndex;
                Slots[clickedIndex].SetSelected(true);
            }
        }
    }
}