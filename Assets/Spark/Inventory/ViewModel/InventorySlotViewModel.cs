using System;
using R3;
using Spark.Inventory.Content;
using Spark.Inventory.Model;
using UnityEngine;

namespace Spark.Inventory.ViewModel
{
    public class InventorySlotViewModel : IDisposable
    {
        private readonly ReactiveProperty<bool> _isSelected = new(false);
        private readonly ItemLibrary _library;
        private readonly Action<int> _onClick;
        private readonly int _index;
        
        public ReadOnlyReactiveProperty<Sprite> Icon { get; private set; }
        public ReadOnlyReactiveProperty<int> Amount { get; private set; }
        public ReadOnlyReactiveProperty<bool> IsSelected => _isSelected;

        public InventorySlotViewModel(InventorySlotModel model, ItemLibrary library, Action<int> onClick, int index)
        {
            _library = library;
            _onClick = onClick;
            _index = index;
            
            Amount = model.AmountObservable.ToReadOnlyReactiveProperty();
            Icon = model.IdObservable
                .Select(id => string.IsNullOrEmpty(id) ? null : ResolveIcon(id))
                .ToReadOnlyReactiveProperty();
        }
        
        public void Dispose()
        {
            IsSelected?.Dispose();
            Icon?.Dispose();
            Amount?.Dispose();
        }
        
        public void Click() => _onClick(_index);

        internal void SetSelected(bool selected) => _isSelected.Value = selected;
        
        private Sprite ResolveIcon(string id) => _library.TryGet(id, out var definition) ? definition.Icon : null;
    }
}