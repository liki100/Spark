using System;
using Spark.TicTacToe.Content;
using Spark.TicTacToe.Model;
using UnityEngine;

namespace Spark.TicTacToe.View
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private SubBoardView[] _subBoardViews;
        
        public event Action<int, int> CellClicked;

        public void Initialize(TicTacToeStyle style)
        {
            for (var index = 0; index < _subBoardViews.Length; index++)
            {
                var subBoardIndex = index;
                
                _subBoardViews[subBoardIndex].Initialize(style);
                _subBoardViews[subBoardIndex].CellClicked += cellIndex =>
                    OnCellClicked(subBoardIndex, cellIndex);
            }
        }
        
        public void SetCell(int subBoardIndex, int cellIndex, Mark mark)
        {
            _subBoardViews[subBoardIndex].SetCell(cellIndex, mark);
        }

        public void SetActiveSubBoard(int activeSubBoardIndex, Mark turn)
        {
            for (var subBoardIndex = 0; subBoardIndex < 9; subBoardIndex++)
            {
                if (activeSubBoardIndex == -2) _subBoardViews[subBoardIndex].SetHighlight(Mark.None);
                else if (activeSubBoardIndex == -1) _subBoardViews[subBoardIndex].SetHighlight(turn);
                else _subBoardViews[subBoardIndex].SetHighlight(activeSubBoardIndex == subBoardIndex ? turn : Mark.None);
            }
        }

        public void SetSubBoardResult(int subBoardIndex, GameResult result)
        {
            _subBoardViews[subBoardIndex].SetResult(result);
        }

        private void OnCellClicked(int subBoardIndex, int cellIndex)
        {
            CellClicked?.Invoke(subBoardIndex, cellIndex);
        }
    }
}
