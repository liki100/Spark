using System;
using Spark.TicTacToe.Content;
using Spark.TicTacToe.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spark.TicTacToe.UI
{
    public class MatchResultView : MonoBehaviour
    {
        [SerializeField] private Image _resultImage;
        [SerializeField] private TMP_Text _resultText;
        [SerializeField] private TMP_Text _rematchStatusText;
        [SerializeField] private Button _rematchButton;
        [SerializeField] private Button _exitButton;
        
        public event Action RematchClicked;
        public event Action ExitClicked;
        
        private TicTacToeStyle _style;
        
        public void Initialize(TicTacToeStyle style)
        {
            _style = style;
            
            _rematchButton.onClick.AddListener(OnRematchButtonClicked);
            _exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        private void OnDestroy()
        {
            _rematchButton.onClick.RemoveListener(OnRematchButtonClicked);
            _exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }
        
        public void ShowResult(GameResult result, Mark selfMark)
        {
            _resultImage.gameObject.SetActive(true);
            
            _rematchStatusText.text = string.Empty;
            _rematchButton.interactable = true;

            switch (result)
            {
                case GameResult.XWins:
                    
                    _resultImage.sprite = _style.GetIcon(result);
                    _resultImage.color = _style.GetColor(result);
                    _resultText.text = Mark.X == selfMark ? "Победа!" : "Поражение";
                    
                    break;
                case GameResult.OWins:
                    
                    _resultImage.sprite = _style.GetIcon(result);
                    _resultImage.color = _style.GetColor(result);
                    _resultText.text = Mark.O == selfMark ? "Победа!" : "Поражение";
                    
                    break;
                case GameResult.Draw:

                    _resultImage.sprite = _style.GetIcon(result);
                    _resultImage.color = _style.GetColor(result);
                    _resultText.text = "Ничья";
                    
                    break;
                case GameResult.OpponentLeft:
                    
                    _resultImage.gameObject.SetActive(false);
                    _resultText.text = "Соперник покинул игру";
                    _rematchButton.interactable = false;
                    
                    break;
            }
        }
        
        public void SetRematchStatus(bool xWants, bool oWants, Mark selfMark)
        {
            _rematchStatusText.text = (xWants, oWants) switch
            {
                (true, false) => "X хочет реванш",
                (false, true) => "O хочет реванш",
                _ => string.Empty
            };

            _rematchButton.interactable = selfMark switch
            {
                Mark.X => xWants == false,
                Mark.O => oWants == false,
                _ => false
            };
        }

        private void OnRematchButtonClicked()
        {
            RematchClicked?.Invoke();
        }

        private void OnExitButtonClicked()
        {
            ExitClicked?.Invoke();
        }
    }
}