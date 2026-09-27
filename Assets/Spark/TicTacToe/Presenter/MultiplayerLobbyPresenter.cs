using System;
using Spark.Modal.Core;
using Spark.TicTacToe.Multiplayer;
using Spark.TicTacToe.Network;
using Spark.TicTacToe.UI;
using Unity.Services.Multiplayer;
using VContainer.Unity;

namespace Spark.TicTacToe.Presenter
{
    public class MultiplayerLobbyPresenter : IStartable, IDisposable
    {
        private readonly MultiplayerLobbyService _service;
        private readonly MultiplayerLobbyView _view;
        private readonly ModalService _modalService;
        private readonly GameManager _gameManager;
        private readonly ScreenSwitcher _screenSwitcher;

        public MultiplayerLobbyPresenter(
            MultiplayerLobbyService service,
            MultiplayerLobbyView view,
            ModalService modalService,
            GameManager gameManager,
            ScreenSwitcher screenSwitcher)
        {
            _service = service;
            _view = view;
            _modalService = modalService;
            _gameManager = gameManager;
            _screenSwitcher = screenSwitcher;
            
            _view.Activated += OnActivated;
            _view.RefreshClicked += OnRefreshClicked;
            _view.QuickJoinClicked += OnQuickJoinClicked;
            _view.CreateClicked += OnCreateClicked;
            _view.JoinClicked += OnJoinClicked;
            _view.ExitClicked += OnExitClicked;
            _view.ConnectClicked += OnConnectClicked;
        }
        
        private async void OnActivated()
        {
            try
            {
                _view.SetLoading(true);
                await _service.InitializeAsync();

                try
                {
                    var sessions = await _service.GetSessionsAsync();
                    _view.SetSessions(sessions);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
            catch (Exception)
            {
                _screenSwitcher.ShowMenuScreen();
            }
            finally
            {
                _view.SetLoading(false);
            }
        }

        private async void OnRefreshClicked()
        {
            try
            {
                _view.SetLoading(true);
                var sessions = await _service.GetSessionsAsync();
                _view.SetSessions(sessions);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            finally
            {
                _view.SetLoading(false);
            }
        }

        private async void OnQuickJoinClicked()
        {
            try
            {
                _view.SetLoading(true);
                var session = await _service.QuickJoinAsync();
                _gameManager.SetRules(session);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            finally
            {
                _view.SetLoading(false);
            }
        }
        
        private async void OnCreateClicked()
        {
            try
            {
                var result = await _modalService.OpenAsync<CreateSessionModal, SessionOptions>();
                if (result.IsConfirmed == false) return;
                
                _view.SetLoading(true);
                var session = await _service.CreateSessionAsync(result.Value);
                _gameManager.SetRules(session);
                _screenSwitcher.ShowWaitingScreen();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            finally
            {
                _view.SetLoading(false);
            }
        }

        private async void OnJoinClicked(string joinCode)
        {
            try
            {
                _view.SetLoading(true);
                var session = await _service.JoinSessionByCodeAsync(joinCode);
                _gameManager.SetRules(session);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            finally
            {
                _view.SetLoading(false);
            }
        }

        private void OnExitClicked()
        {
            _screenSwitcher.ShowMenuScreen();
        }
        
        private async void OnConnectClicked(string sessionId)
        {
            try
            {
                _view.SetLoading(true);
                var session = await _service.JoinSessionByIdAsync(sessionId);
                _gameManager.SetRules(session);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            finally
            {
                _view.SetLoading(false);
            }
        }
        
        public void Start() { }
        
        public void Dispose()
        {
            _view.Activated -= OnActivated;
            _view.RefreshClicked -= OnRefreshClicked;
            _view.QuickJoinClicked -= OnQuickJoinClicked;
            _view.CreateClicked -= OnCreateClicked;
            _view.JoinClicked -= OnJoinClicked;
            _view.ExitClicked -= OnExitClicked;
            _view.ConnectClicked -= OnConnectClicked;
        }
    }
}