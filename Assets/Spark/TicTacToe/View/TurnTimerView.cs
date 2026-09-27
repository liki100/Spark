using PrimeTween;
using Spark.TicTacToe.Network;
using TMPro;
using UnityEngine;
using VContainer;

namespace Spark.TicTacToe.View
{
    public class TurnTimerView : MonoBehaviour
    {
        private const float UrgentThresholdSeconds = 5f;
        private const float PulseScale = 1.2f;
        private const float PulseDuration = 0.35f;

        [SerializeField] private TMP_Text _secondsText;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _urgentColor;
        
        private GameManager _gameManager;
        private bool _isUrgent;

        private Tween _tween;

        [Inject]
        public void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;
        }
        
        private void Update()
        {
            var remainingSeconds = _gameManager.RemainingTurnTime;
            _secondsText.text = Mathf.Max(0, Mathf.CeilToInt((float)remainingSeconds)).ToString();

            SetUrgent(remainingSeconds <= UrgentThresholdSeconds);
        }

        private void SetUrgent(bool isUrgent)
        {
            if (isUrgent == _isUrgent) return;

            _isUrgent = isUrgent;

            if (isUrgent)
            {
                _secondsText.color = _urgentColor;
                
                _secondsText.transform.localScale = Vector3.one;
                _tween = Tween.Scale(_secondsText.transform, 1f, PulseScale, PulseDuration, Ease.InOutSine, cycles: -1, cycleMode: CycleMode.Yoyo);
            }
            else
            {
                _tween.Stop();
                _secondsText.transform.localScale = Vector3.one;
                
                _secondsText.color = _normalColor;
            }
        }
    }
}
