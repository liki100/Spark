using Spark.TicTacToe.Model;
using UnityEngine;

namespace Spark.TicTacToe.Content
{
    // Таблица "какую картинку и какой цвет показывать" для метки и для итога игры —
    // одно место, откуда все View берут визуал, вместо своего switch в каждом.
    [CreateAssetMenu(fileName = "TicTacToeStyle", menuName = "Spark/TicTacToe/Style")]
    public class TicTacToeStyle : ScriptableObject
    {
        [SerializeField] private Sprite _xMarkIcon;
        [SerializeField] private Color _xMarkColor;
        [Space]
        [SerializeField] private Sprite _oMarkIcon;
        [SerializeField] private Color _oMarkColor;
        [Space]
        [SerializeField] private Sprite _drawIcon;
        [SerializeField] private Color _drawColor;
        [Space]
        [SerializeField] private Sprite _noneIcon;
        [SerializeField] private Color _noneColor;

        public Sprite GetIcon(Mark mark)
        {
            return mark switch
            {
                Mark.X => _xMarkIcon,
                Mark.O => _oMarkIcon,
                Mark.Draw => _drawIcon,
                _ => _noneIcon
            };
        }

        public Sprite GetIcon(GameResult result)
        {
            return result switch
            {
                GameResult.XWins => _xMarkIcon,
                GameResult.OWins => _oMarkIcon,
                GameResult.Draw => _drawIcon,
                _ => _noneIcon
            };
        }

        public Color GetColor(Mark mark)
        {
            return mark switch
            {
                Mark.X => _xMarkColor,
                Mark.O => _oMarkColor,
                Mark.Draw => _drawColor,
                _ => _noneColor
            };
        }

        public Color GetColor(GameResult result)
        {
            return result switch
            {
                GameResult.XWins => _xMarkColor,
                GameResult.OWins => _oMarkColor,
                GameResult.Draw => _drawColor,
                _ => _noneColor
            };
        }
    }
}
