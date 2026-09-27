using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spark.TicTacToe.UI
{
    public class AuthenticationView : MonoBehaviour
    {
        [SerializeField] private LoadingOverlay _loadingOverlay;
        [Space]
        [SerializeField] private TMP_Text _errorText;
        [Space]
        [SerializeField] private TMP_InputField _usernameInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [Space]
        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _registerButton;
        [SerializeField] private Button _backButton;

        public event Action<string, string> LoginClicked;
        public event Action<string, string> RegisterClicked;
        public event Action BackClicked;
        
        private void Awake()
        {
            _loginButton.onClick.AddListener(OnLoginClicked);
            _registerButton.onClick.AddListener(OnRegisterClicked);
            _backButton.onClick.AddListener(OnBackClicked);
        }

        private void OnEnable() => ShowError(string.Empty);

        private void OnDestroy()
        {
            _loginButton.onClick.RemoveListener(OnLoginClicked);
            _registerButton.onClick.RemoveListener(OnRegisterClicked);
            _backButton.onClick.RemoveListener(OnBackClicked);
        }

        public void ShowError(string error) => _errorText.text = error;
        public void SetLoading(bool loading) => _loadingOverlay.gameObject.SetActive(loading);
        
        private void OnLoginClicked() => LoginClicked?.Invoke(_usernameInputField.text, _passwordInputField.text);
        private void OnRegisterClicked() => RegisterClicked?.Invoke(_usernameInputField.text, _passwordInputField.text);
        private void OnBackClicked() => BackClicked?.Invoke();
    }
}