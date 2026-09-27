using System;
using Spark.TicTacToe.Content;
using Spark.TicTacToe.Model;
using Spark.TicTacToe.Multiplayer;
using Spark.TicTacToe.Network;
using Spark.TicTacToe.UI;
using VContainer.Unity;

namespace Spark.TicTacToe.Presenter
{
    public class MatchResultPresenter : IStartable, IDisposable
    {
        private readonly GameManager _gameManager;
        private readonly MultiplayerLobbyService _multiplayerService;
        private readonly ScreenSwitcher _screenSwitcher;
        private readonly MatchResultView _view;

        public MatchResultPresenter(
            GameManager gameManager,
            MultiplayerLobbyService multiplayerService,
            ScreenSwitcher screenSwitcher,
            MatchResultView view,
            TicTacToeStyle style)
        {
            _gameManager = gameManager;
            _multiplayerService = multiplayerService;
            _screenSwitcher = screenSwitcher;
            _view = view;
            
            _gameManager.Result.OnValueChanged += OnResultChanged;
            _gameManager.XWantsRematch.OnValueChanged += OnRematchStateChanged;
            _gameManager.OWantsRematch.OnValueChanged += OnRematchStateChanged;

            _multiplayerService.PlayerHasLeft += OnPlayerHasLeft;
            
            _view.Initialize(style);
            _view.RematchClicked += OnRematchClicked;
            _view.ExitClicked += OnExitClicked;
        }
        
        private void OnResultChanged(GameResult previous, GameResult current)
        {
            if (current == GameResult.None) return;
            
            _view.ShowResult(current, _gameManager.SelfMark);
            _screenSwitcher.ShowMatchResultScreen();
        }

        private void OnRematchStateChanged(bool previous, bool current)
        {
            _view.SetRematchStatus(_gameManager.XWantsRematch.Value, _gameManager.OWantsRematch.Value, _gameManager.SelfMark);
        }
        
        private void OnPlayerHasLeft(string playerId)
        {
            if (_gameManager.ConnectedPlayerCount.Value == 2)
            {
                _gameManager.StopTurnTimer();
                _view.ShowResult(GameResult.OpponentLeft, _gameManager.SelfMark);
                _screenSwitcher.ShowMatchResultScreen();
            }
        }
        
        private void OnRematchClicked()
        {
            _gameManager.RequestRematchRpc();
        }

        private async void OnExitClicked()
        {
            try
            {
                await _multiplayerService.LeaveSessionAsync();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            finally
            {
                _screenSwitcher.ShowMenuScreen();
            }
        }
        
        public void Start() { }

        public void Dispose()
        {
            _gameManager.Result.OnValueChanged -= OnResultChanged;
            _gameManager.XWantsRematch.OnValueChanged -= OnRematchStateChanged;
            _gameManager.OWantsRematch.OnValueChanged -= OnRematchStateChanged;
            
            _multiplayerService.PlayerHasLeft -= OnPlayerHasLeft;
            
            _view.RematchClicked -= OnRematchClicked;
            _view.ExitClicked -= OnExitClicked;
        }
    }
}