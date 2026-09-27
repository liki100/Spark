using R3;
using Spark.Inventory.ViewModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Spark.Inventory.View
{
    public class InventorySlotView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _amountText;

        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _deselectedColor;
        [SerializeField] private Image _background;
        
        private InventorySlotViewModel _viewModel;
        
        public void Bind(InventorySlotViewModel viewModel)
        {
            _viewModel = viewModel;
            
            _viewModel.Icon.Subscribe(sprite =>
            {
                _icon.enabled = sprite != null;
                _icon.sprite = sprite;
            }).AddTo(this);
            
            _viewModel.Amount.Subscribe(amount =>
            {
                _amountText.text = amount > 0 ? amount.ToString() : "";
            }).AddTo(this);
            
            _viewModel.IsSelected.Subscribe(selected =>
            {
                _background.color = selected ? _selectedColor : _deselectedColor;
            }).AddTo(this);
        }
        
        public void OnPointerClick(PointerEventData eventData) => _viewModel.Click();
    }
}