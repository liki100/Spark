using Spark.Inventory.ViewModel;
using UnityEngine;
using VContainer;

namespace Spark.Inventory.View
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private InventorySlotView _slotPrefab;
        [SerializeField] private Transform _slotsRoot;
        
        private InventorySlotView[] _slotViews;
        
        [Inject]
        public void Construct(InventoryViewModel viewModel)
        {
            ClearViews();
            
            _slotViews = new InventorySlotView[viewModel.Slots.Count];
            for (var index = 0; index < _slotViews.Length; index++)
            {
                var slotView = Instantiate(_slotPrefab, _slotsRoot);
                slotView.Bind(viewModel.Slots[index]);
                _slotViews[index] = slotView;
            }
        }

        private void ClearViews()
        {
            if (_slotViews == null) return;
            
            foreach (Transform child in _slotsRoot)
                Destroy(child.gameObject);
            
            _slotViews = null;
        }
    }
}