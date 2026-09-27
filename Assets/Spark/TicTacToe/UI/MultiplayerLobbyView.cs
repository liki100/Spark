using System;
using System.Collections.Generic;
using Spark.TicTacToe.View;
using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace Spark.TicTacToe.UI
{
    public class MultiplayerLobbyView : MonoBehaviour
    {
        [SerializeField] private LoadingOverlay _loadingOverlay;
        [Space]
        [SerializeField] private Transform _sessionsContainer;
        [SerializeField] private SessionView _sessionViewPrefab;
        [SerializeField] private Color _rowColorPrimary;
        [SerializeField] private Color _rowColorSecondary;
        [Space]
        [SerializeField] private Button _refreshButton;
        [SerializeField] private Button _quickJoinButton;
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _joinButton;
        [SerializeField] private TMP_InputField _joinCodeInput;
        [SerializeField] private Button _exitButton;

        public event Action Activated;
        public event Action RefreshClicked;
        public event Action QuickJoinClicked;
        public event Action CreateClicked;
        public event Action<string> JoinClicked;
        public event Action ExitClicked;
        public event Action<string> ConnectClicked;
        
        private List<SessionView> _currentSessions = new();
        
        private void Awake()
        {
            _refreshButton.onClick.AddListener(OnRefreshClicked);
            _quickJoinButton.onClick.AddListener(OnQuickJoinClicked);
            _createButton.onClick.AddListener(OnCreateClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
            _exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnEnable()
        {
            Activated?.Invoke();
        }

        private void OnDestroy()
        {
            _refreshButton.onClick.RemoveListener(OnRefreshClicked);
            _quickJoinButton.onClick.RemoveListener(OnQuickJoinClicked);
            _createButton.onClick.RemoveListener(OnCreateClicked);
            _joinButton.onClick.RemoveListener(OnJoinClicked);
            _exitButton.onClick.RemoveListener(OnExitClicked);
        }

        public void SetSessions(IList<ISessionInfo> sessions)
        {
            foreach (var session in _currentSessions)
            {
                session.ConnectClicked -= OnConnectClicked;
                Destroy(session.gameObject);
            }

            _currentSessions.Clear();

            for (var index = 0; index < sessions.Count; index++)
            {
                var sessionView = Instantiate(_sessionViewPrefab, _sessionsContainer);
                sessionView.Initialize(
                    sessions[index],
                    index % 2 == 0 ? _rowColorPrimary : _rowColorSecondary,
                    index % 2 == 0 ? _rowColorSecondary : _rowColorPrimary);
                sessionView.ConnectClicked += OnConnectClicked;
                _currentSessions.Add(sessionView);
            }
        }

        public void SetLoading(bool loading)
        {
            _loadingOverlay.gameObject.SetActive(loading);
        }
        
        private void OnRefreshClicked() => RefreshClicked?.Invoke();
        private void OnQuickJoinClicked() => QuickJoinClicked?.Invoke();
        private void OnCreateClicked() => CreateClicked?.Invoke();
        private void OnJoinClicked() => JoinClicked?.Invoke(_joinCodeInput.text);
        private void OnExitClicked() => ExitClicked?.Invoke();
        private void OnConnectClicked(string sessionId) => ConnectClicked?.Invoke(sessionId);
    }
}