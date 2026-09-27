using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Spark.Modal.Core;
using Spark.TicTacToe.Model;
using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace Spark.TicTacToe.UI
{
    public class CreateSessionModal : ModalWindow<Unit, SessionOptions>
    {
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private Toggle _privateToggle;
        [SerializeField] private TMP_Dropdown _modeDropdown;
        [Space]
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _backButton;

        private void Awake()
        {
            _modeDropdown.ClearOptions();
            _modeDropdown.AddOptions(BoardConstants.DisplayToMode.Select(pair => pair.Value).ToList());
        }

        public override UniTask<ModalResult<SessionOptions>> OpenAsync(Unit args, CancellationToken cancellationToken)
        {
            var confirmed = _confirmButton
                .OnClickAsObservable()
                .Select(_ =>
                {
                    var sessionOptions = new SessionOptions();
                    
                    if (string.IsNullOrWhiteSpace(_nameInputField.text) == false)
                        sessionOptions.Name = _nameInputField.text;
                    
                    sessionOptions.MaxPlayers = 2;
                    sessionOptions.IsPrivate = _privateToggle.isOn;
                    
                    var text = _modeDropdown.options[_modeDropdown.value].text;
                    var selected = BoardConstants.DisplayToMode.First(pair => pair.Value == text).Key;
                    sessionOptions.SessionProperties.TryAdd("mode", new SessionProperty(selected.ToString()));
                    
                    sessionOptions.WithRelayNetwork();
                    
                    return ModalResult<SessionOptions>.Confirmed(sessionOptions);
                });

            var cancelled = _backButton
                .OnClickAsObservable()
                .Select(_ => ModalResult<SessionOptions>.Cancelled());

            return Observable.Merge(confirmed, cancelled)
                .FirstAsync(cancellationToken: cancellationToken)
                .AsUniTask();
        }
    }
}