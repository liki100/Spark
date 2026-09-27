using System;
using PrimeTween;
using Spark.TicTacToe.Content;
using Spark.TicTacToe.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Spark.TicTacToe.View
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _markImage;
        
        public event Action Clicked;
        
        private TicTacToeStyle _style;
        private Mark _currentMark = Mark.None;
        
        private void Awake()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }
        
        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }
        
        public void Initialize(TicTacToeStyle style)
        {
            _style = style;
        }
        
        public void SetMark(Mark mark)
        {
            var isNewPlacement = _currentMark == Mark.None && mark != Mark.None;
            _currentMark = mark;

            _markImage.sprite = _style.GetIcon(mark);
            _markImage.color = _style.GetColor(mark);

            if (isNewPlacement)
                Tween.Scale(_markImage.transform, 0f, 1f, 0.25f, Ease.OutBack);
        }
        
        private void OnButtonClicked()
        {
            Clicked?.Invoke();
        }
    }
}
