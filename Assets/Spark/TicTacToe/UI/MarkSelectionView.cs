using Spark.TicTacToe.Model;
using Spark.TicTacToe.Network;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Spark.TicTacToe.UI
{
    public class MarkSelectionView : MonoBehaviour
    {
        [SerializeField] private Button _selectXButton;
        [SerializeField] private Button _selectOButton;

        private GameManager _gameManager;

        [Inject]
        public void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;

            _selectXButton.onClick.AddListener(() => _gameManager.RequestSelectMarkRpc(Mark.X));
            _selectOButton.onClick.AddListener(() => _gameManager.RequestSelectMarkRpc(Mark.O));
            
            _gameManager.XClientId.OnValueChanged += OnMarkChanged;
            _gameManager.OClientId.OnValueChanged += OnMarkChanged;
        }

        private void OnEnable()
        {
            UpdateButtons();
        }

        private void OnDestroy()
        {
            _gameManager.XClientId.OnValueChanged -= OnMarkChanged;
            _gameManager.OClientId.OnValueChanged -= OnMarkChanged;
        }

        private void OnMarkChanged(ulong previous, ulong current)
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            _selectXButton.interactable = _gameManager.XClientId.Value == BoardConstants.NoClient;
            _selectOButton.interactable = _gameManager.OClientId.Value == BoardConstants.NoClient;
        }
    }
}
