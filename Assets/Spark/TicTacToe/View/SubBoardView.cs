using System;
using PrimeTween;
using Spark.TicTacToe.Content;
using Spark.TicTacToe.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Spark.TicTacToe.View
{
    public class SubBoardView : MonoBehaviour
    {
        [SerializeField] private CellView[] _cellViews;
        [Space]
        [SerializeField] private Image _highlightFrame;
        [SerializeField] private Color _defaultHighlightColor;
        [Space]
        [SerializeField] private GameObject _resultContainer;
        [SerializeField] private Image _resultImage;

        public event Action<int> CellClicked;

        private TicTacToeStyle _style;
        private GameResult _currentResult = GameResult.None;
        
        public void Initialize(TicTacToeStyle style)
        {
            _style = style;

            for (var index = 0; index < _cellViews.Length; index++)
            {
                var cellIndex = index;

                _cellViews[cellIndex].Initialize(style);
                _cellViews[cellIndex].Clicked += () =>
                    OnCellClicked(cellIndex);
            }
        }

        public void SetCell(int cellIndex, Mark mark)
        {
            _cellViews[cellIndex].SetMark(mark);
        }

        public void SetHighlight(Mark turn)
        {
            _highlightFrame.color = turn == Mark.None ? _defaultHighlightColor : _style.GetColor(turn);
        }

        public void SetResult(GameResult result)
        {
            var isNewResult = _currentResult == GameResult.None && result != GameResult.None;
            _currentResult = result;

            var hasResult = result != GameResult.None;
            _resultContainer.SetActive(hasResult);

            if (isNewResult)
            {
                _resultImage.sprite = _style.GetIcon(result);
                _resultImage.color = _style.GetColor(result);
                Tween.Scale(_resultContainer.transform, 0f, 1f, 0.5f, Ease.OutBack);
            }
        }
        
        private void OnCellClicked(int cellIndex)
        {
            CellClicked?.Invoke(cellIndex);
        }
    }
}
