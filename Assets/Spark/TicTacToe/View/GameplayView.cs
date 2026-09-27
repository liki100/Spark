using System;
using UnityEngine;
using UnityEngine.UI;

namespace Spark.TicTacToe.View
{
    public class GameplayView : MonoBehaviour
    {
        [SerializeField] private BoardView _boardView;
        [SerializeField] private Button _surrenderButton;
        
        public BoardView BoardView => _boardView;
        
        public event Action SurrenderClicked;
        public event Action SettingsClicked;
        
        private void Awake()
        {
            _surrenderButton.onClick.AddListener(OnSurrenderClicked);
            //_settingsButton.onClick.AddListener(OnSettingsClicked);
        }

        private void OnDestroy()
        {
            _surrenderButton.onClick.RemoveListener(OnSurrenderClicked);
            //_settingsButton.onClick.RemoveListener(OnSettingsClicked);
        }

        private void OnSurrenderClicked() => SurrenderClicked?.Invoke();
        private void OnSettingsClicked() => SettingsClicked?.Invoke();
    }
}