using UnityEngine;
using UnityEngine.UI;

namespace Spark.TicTacToe.UI
{
    public class ConnectionPanelView : MonoBehaviour
    {
        [SerializeField] private ScreenSwitcher _screenSwitcher;
        [Space]
        [SerializeField] private Button _multiplayerButton;
        [SerializeField] private Button _exitButton;

        private void Awake()
        {
            _multiplayerButton.onClick.AddListener(OnMultiplayerClicked);
            _exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnDestroy()
        {
            _multiplayerButton.onClick.RemoveListener(OnMultiplayerClicked);
            _exitButton.onClick.RemoveListener(OnExitClicked);
        }

        private void OnMultiplayerClicked() => _screenSwitcher.ShowMultiplayerScreen();
        private void OnExitClicked() => Application.Quit();
    }
}
