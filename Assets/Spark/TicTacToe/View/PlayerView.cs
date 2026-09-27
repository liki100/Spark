using Spark.TicTacToe.Content;
using Spark.TicTacToe.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spark.TicTacToe.View
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Image _markImage;
        [SerializeField] private TMP_Text _statusText;

        private Mark _mark;
        private TicTacToeStyle _style;

        public void Initialize(TicTacToeStyle visuals)
        {
            _style = visuals;
        }
    
        public void Redraw(Mark mark)
        {
            _mark = mark;
            
            _titleText.color = _style.GetColor(mark);
            _markImage.color = _style.GetColor(mark);
            _markImage.sprite = _style.GetIcon(mark);
        }

        public void SetStatus(Mark mark)
        {
            if (_mark == mark)
            {
                _statusText.text = "В процессе";
                _statusText.color = _style.GetColor(_mark);
            }
            else
            {
                _statusText.text = "Ожидает ход";
                _statusText.color = _style.GetColor(GameResult.Draw);
            }
        }
    }

}