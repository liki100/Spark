using Spark.TicTacToe.Content;
using Spark.TicTacToe.Model;
using Spark.TicTacToe.Network;
using UnityEngine;
using VContainer;

namespace Spark.TicTacToe.View
{
    public class PlayerHud : MonoBehaviour
    {
        [SerializeField] private PlayerView _leftPlayerView;
        [SerializeField] private PlayerView _rightPlayerView;
        
        private GameManager _gameManager;
        private TicTacToeStyle _style;
        
        [Inject]
        public void Construct(GameManager gameManager, TicTacToeStyle style)
        {
            _gameManager = gameManager;
            _style = style;
            
            _leftPlayerView.Initialize(_style);
            _rightPlayerView.Initialize(_style);
            
            _gameManager.XClientId.OnValueChanged += OnMarkChanged;
            _gameManager.OClientId.OnValueChanged += OnMarkChanged;
            _gameManager.Turn.OnValueChanged += OnTurnChanged;
        }

        private void OnDestroy()
        {
            _gameManager.XClientId.OnValueChanged -= OnMarkChanged;
            _gameManager.OClientId.OnValueChanged -= OnMarkChanged;
            _gameManager.Turn.OnValueChanged -= OnTurnChanged;
        }
        
        private void OnMarkChanged(ulong previous, ulong current)
        {
            _leftPlayerView.Redraw(_gameManager.SelfMark);
            _rightPlayerView.Redraw(_gameManager.OpponentMark);
            
            UpdateTurn();
        }

        private void OnTurnChanged(Mark previous, Mark current)
        {
            UpdateTurn();
        }
        
        private void UpdateTurn()
        {
            var turn = _gameManager.Turn.Value;

            _leftPlayerView.SetStatus(turn);
            _rightPlayerView.SetStatus(turn);
        }
    }
}