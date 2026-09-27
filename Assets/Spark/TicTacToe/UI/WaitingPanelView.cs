using System;
using Spark.TicTacToe.Multiplayer;
using Spark.TicTacToe.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Spark.TicTacToe.UI
{
    public class WaitingPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject _joinCodeContainer;
        [SerializeField] private TMP_Text _joinCodeText;
        [SerializeField] private Button _copyJoinCodeButton;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private Button _leaveButton;

        private GameManager _gameManager;
        private MultiplayerLobbyService _multiplayerService;
        private ScreenSwitcher _screenSwitcher;

        [Inject]
        public void Construct(
            GameManager gameManager,
            MultiplayerLobbyService multiplayerService,
            ScreenSwitcher screenSwitcher)
        {
            _gameManager = gameManager;
            _multiplayerService = multiplayerService;
            _screenSwitcher = screenSwitcher;

            _copyJoinCodeButton.onClick.AddListener(CopyJoinCode);
            _leaveButton.onClick.AddListener(Leave);
            
            _gameManager.ConnectedPlayerCount.OnValueChanged += OnPlayerCountChanged;
        }
        
        private void OnEnable()
        {
            var joinCode = _multiplayerService.CurrentSession.Code;
            _joinCodeText.text = joinCode;
            _joinCodeContainer.SetActive(_multiplayerService.IsHost);
        }
        
        private void OnDestroy()
        {
            _gameManager.ConnectedPlayerCount.OnValueChanged -= OnPlayerCountChanged;
        }
        
        private void OnPlayerCountChanged(int previous, int current) => UpdateState();

        private void UpdateState()
        {
            _statusText.text = "Ожидание соперника...";
            
            if (_gameManager.ConnectedPlayerCount.Value >= 2)
                _screenSwitcher.ShowMarkSelectionScreen();
        }

        private void CopyJoinCode() => GUIUtility.systemCopyBuffer = _multiplayerService.CurrentSession.Code;
        
        private async void Leave()
        {
            try
            {
                await _multiplayerService.LeaveSessionAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                _screenSwitcher.ShowMenuScreen();
            }
        }
    }
}
