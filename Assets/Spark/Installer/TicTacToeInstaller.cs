using Spark.TicTacToe.Content;
using Spark.TicTacToe.Multiplayer;
using Spark.TicTacToe.Network;
using Spark.TicTacToe.Presenter;
using Spark.TicTacToe.UI;
using Spark.TicTacToe.View;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Spark.Installer
{
    public class TicTacToeInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private TicTacToeStyle _style;
        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private UnityTransport _transport;
        [SerializeField] private WaitingPanelView _waitingPanelView;
        [SerializeField] private MarkSelectionView _markSelectionView;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private GameplayView _gameplayView;
        [SerializeField] private MatchResultView _matchResultView;
        [SerializeField] private PlayerHud _playerHud;
        [SerializeField] private TurnTimerView _turnTimerView;
        [SerializeField] private MultiplayerLobbyView _multiplayerLobbyView;
        [SerializeField] private ScreenSwitcher _screenSwitcher;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponent(_style);
            builder.RegisterComponent(_networkManager);
            builder.RegisterComponent(_transport);
            builder.Register<MultiplayerLobbyService>(Lifetime.Singleton);
            builder.RegisterComponent(_multiplayerLobbyView);
            builder.RegisterEntryPoint<MultiplayerLobbyPresenter>();
            builder.RegisterComponent(_screenSwitcher);
            builder.RegisterComponent(_waitingPanelView);
            builder.RegisterComponent(_markSelectionView);
            builder.RegisterComponent(_gameManager);
            builder.RegisterComponent(_gameplayView);
            builder.RegisterEntryPoint<GameplayPresenter>();
            builder.RegisterComponent(_matchResultView);
            builder.RegisterEntryPoint<MatchResultPresenter>();
            builder.RegisterComponent(_playerHud);
            builder.RegisterComponent(_turnTimerView);
        }
    }
}
