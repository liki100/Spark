using System;
using Spark.TicTacToe.UI;
using Unity.Services.Authentication;
using UnityEngine;

namespace Spark.TicTacToe.Presenter
{
    public class AuthenticationPresenter : IDisposable
    {
        private readonly AuthenticationView _view;
        private readonly ScreenSwitcher _screenSwitcher;

        public AuthenticationPresenter(
            AuthenticationView view,
            ScreenSwitcher screenSwitcher)
        {
            _view = view;
            _screenSwitcher = screenSwitcher;
            
            _view.LoginClicked += OnLoginClicked;
            _view.RegisterClicked += OnRegisterClicked;
            _view.BackClicked += OnBackClicked;
        }

        private async void OnLoginClicked(string username, string password)
        {
            try
            {
                _view.ShowError(string.Empty);
                _view.SetLoading(true);

                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                
                _screenSwitcher.ShowMultiplayerScreen();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _view.ShowError("Неправильный логин или пароль");
            }
            finally
            {
                _view.SetLoading(false);
            }
        }
        
        private async void OnRegisterClicked(string username, string password)
        {
            try
            {
                _view.ShowError(string.Empty);
                _view.SetLoading(true);

                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                
                _screenSwitcher.ShowMultiplayerScreen();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _view.ShowError("Ошибка регистрации");
            }
            finally
            {
                _view.SetLoading(false);
            }
        }

        private void OnBackClicked()
        {
            _screenSwitcher.ShowMenuScreen();
        }
        
        public void Dispose()
        {
            _view.LoginClicked -= OnLoginClicked;
            _view.RegisterClicked -= OnRegisterClicked;
            _view.BackClicked -= OnBackClicked;
        }
    }
}