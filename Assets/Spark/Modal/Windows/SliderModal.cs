using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Spark.Modal.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Spark.Modal.Windows
{
    public struct SliderModalArgs
    {
        public int Min;
        public int Max;
        public int Value;
    }

    public class SliderModal : ModalWindow<SliderModalArgs, int>
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        public override UniTask<ModalResult<int>> OpenAsync(SliderModalArgs args, CancellationToken cancellationToken)
        {
            _slider.minValue = args.Min;
            _slider.maxValue = args.Max;
            _slider.value = args.Value;

            var confirmed = _confirmButton
                .OnClickAsObservable()
                .Select(_ => ModalResult<int>.Confirmed(Mathf.RoundToInt(_slider.value)));

            var cancelled = _cancelButton
                .OnClickAsObservable()
                .Select(_ => ModalResult<int>.Cancelled());

            return Observable.Merge(confirmed, cancelled)
                .FirstAsync(cancellationToken: cancellationToken)
                .AsUniTask();
        }
    }
}
