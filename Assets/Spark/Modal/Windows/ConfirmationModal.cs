using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Spark.Modal.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spark.Modal.Windows
{
    public struct ConfirmationModalArgs
    {
        public string Title;
        public string Description;
    }
    
    public class ConfirmationModal : ModalWindow<ConfirmationModalArgs, Unit>
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        public override UniTask<ModalResult<Unit>> OpenAsync(ConfirmationModalArgs args, CancellationToken cancellationToken)
        {
            _titleText.text = args.Title;
            _descriptionText.text = args.Description;
            
            var confirmed = _confirmButton
                .OnClickAsObservable()
                .Select(_ => ModalResult.Confirmed());

            var cancelled = _cancelButton
                .OnClickAsObservable()
                .Select(_ => ModalResult.Cancelled());

            return Observable.Merge(confirmed, cancelled)
                .FirstAsync(cancellationToken: cancellationToken)
                .AsUniTask();
        }
    }
}
