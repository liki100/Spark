using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Spark.Modal.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spark.Modal.Windows
{
    public struct InfoModalArgs
    {
        public string Title;
        public string Description;
        public Sprite Icon;
    }
    
    public class InfoModal : ModalWindow<InfoModalArgs, Unit>
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _closeButton;

        public override UniTask<ModalResult<Unit>> OpenAsync(InfoModalArgs args, CancellationToken cancellationToken)
        {
            _titleText.text = args.Title;
            _descriptionText.text = args.Description;
            _icon.sprite = args.Icon;
            
            return _closeButton.OnClickAsObservable()
                .Select(_ => ModalResult.Confirmed())
                .FirstAsync(cancellationToken: cancellationToken)
                .AsUniTask();
        }
    }
}
