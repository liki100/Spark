using PrimeTween;
using UnityEngine;

namespace Spark.TicTacToe.UI
{
    public class ScreenSwitcher : MonoBehaviour
    {
        private const float FadeDuration = 0.25f;

        [SerializeField] private CanvasGroup _menuScreen;
        [SerializeField] private CanvasGroup _multiplayerScreen;
        [SerializeField] private CanvasGroup _connectionScreen;
        [SerializeField] private CanvasGroup _waitingScreen;
        [SerializeField] private CanvasGroup _markSelectionScreen;
        [SerializeField] private CanvasGroup _boardScreen;
        [SerializeField] private CanvasGroup _matchResultScreen;

        private CanvasGroup[] _allScreens;
        private CanvasGroup _currentScreen;

        private void Awake()
        {
            _allScreens = new[] { _menuScreen, _multiplayerScreen, _connectionScreen, _waitingScreen, _markSelectionScreen, _boardScreen, _matchResultScreen };
            
            _currentScreen = _connectionScreen;
            
            foreach (var screen in _allScreens)
            {
                var isActive = screen == _currentScreen;

                screen.alpha = isActive ? 1f : 0f;
                screen.interactable = isActive;
                screen.blocksRaycasts = isActive;
                screen.gameObject.SetActive(isActive);
            }
        }
        
        public void ShowMenuScreen() => ShowOnly(_menuScreen);
        public void ShowMultiplayerScreen() => ShowOnly(_multiplayerScreen);
        public void ShowWaitingScreen() => ShowOnly(_waitingScreen);
        public void ShowMarkSelectionScreen() => ShowOnly(_markSelectionScreen);
        public void ShowBoardScreen() => ShowOnly(_boardScreen);
        public void ShowMatchResultScreen() => ShowOnly(_matchResultScreen);

        private void ShowOnly(CanvasGroup target)
        {
            if (_currentScreen == target)
                return;

            _currentScreen = target;
            
            foreach (var screen in _allScreens)
            {
                var showing = screen == target;
                if (showing == false && screen.gameObject.activeSelf == false) continue;
                
                screen.interactable = showing;
                screen.blocksRaycasts = showing;
                
                if (showing) screen.gameObject.SetActive(true);

                Tween.StopAll(screen);
                var tween = Tween.Alpha(screen, showing ? 1f : 0f, FadeDuration);
                if (showing == false) tween.OnComplete(screen, s => s.gameObject.SetActive(false));
            }
        }
    }
}
