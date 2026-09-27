using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Spark.TicTacToe.UI
{
    public class LoadingOverlay : MonoBehaviour
    {
        [SerializeField] private Image _spinnerImage;
        
        private Tween _tween;

        private void OnEnable() => _tween = Tween.EulerAngles(_spinnerImage.transform, startValue: Vector3.zero, endValue: new Vector3(0f, 0f, -360f), duration: 1f, ease: Ease.Linear, cycles: -1, cycleMode: CycleMode.Incremental);
        private void OnDisable() => _tween.Stop();
    }
}