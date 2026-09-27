using System;
using Spark.TicTacToe.Model;
using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace Spark.TicTacToe.View
{
    public class SessionView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _modeText;
        [SerializeField] private Button _joinButton;
        [Space]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _frameImage;
        
        private ISessionInfo _sessionInfo;
        
        public event Action<string> ConnectClicked;

        public void Initialize(
            ISessionInfo sessionInfo,
            Color backgroundColor,
            Color frameColor)
        {
            _sessionInfo = sessionInfo;
            
            _titleText.text = _sessionInfo.Name;
            _modeText.text = _sessionInfo.Properties.TryGetValue("mode", out var property) 
                ? BoardConstants.DisplayToMode[BoardConstants.ParseMode(property.Value)] 
                : "";
            
            _backgroundImage.color = backgroundColor;
            _frameImage.color = frameColor;
        }

        private void Awake() => _joinButton.onClick.AddListener(OnJoinClicked);
        private void OnDestroy() => _joinButton.onClick.RemoveListener(OnJoinClicked);
        private void OnJoinClicked() => ConnectClicked?.Invoke(_sessionInfo.Id);
    }
}