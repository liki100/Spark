using System;
using Spark.TicTacToe.Content;
using Spark.TicTacToe.Model;
using Spark.TicTacToe.Network;
using Spark.TicTacToe.UI;
using Spark.TicTacToe.View;
using Unity.Netcode;
using UnityEngine;
using VContainer.Unity;

namespace Spark.TicTacToe.Presenter
{
    public class GameplayPresenter : IStartable, IDisposable
    {
        private readonly GameManager _gameManager;
        private readonly ScreenSwitcher _screenSwitcher;
        private readonly GameplayView _gameplayView;
        private readonly BoardView _boardView;

        public GameplayPresenter(GameManager gameManager, ScreenSwitcher screenSwitcher, GameplayView gameplayView, TicTacToeStyle style)
        {
            _gameManager = gameManager;
            _screenSwitcher = screenSwitcher;
            _gameplayView = gameplayView;
            _boardView = gameplayView.BoardView;
            
            _gameManager.Cells.OnListChanged += OnCellsChanged;
            _gameManager.ActiveSubBoard.OnValueChanged += OnActiveSubBoardChanged;
            _gameManager.SubBoardResults.OnListChanged += OnSubBoardResultsChanged;
            _gameManager.Turn.OnValueChanged += OnTurnChanged;
            _gameManager.XPendingMark.OnValueChanged += OnXPendingChanged;
            _gameManager.OPendingMark.OnValueChanged += OnOPendingChanged;
            _gameManager.Started += OnStarted;
            _gameManager.Restarted += OnRestarted;
            
            _gameplayView.SurrenderClicked += OnSurrenderClicked;
            _gameplayView.SettingsClicked += OnSettingsClicked;
            
            _boardView.Initialize(style);
            _boardView.CellClicked += OnCellClicked;
        }

        private void OnCellsChanged(NetworkListEvent<byte> change)
        {
            if (change.Type != NetworkListEvent<byte>.EventType.Value) return;

            var subBoardIndex = change.Index / BoardConstants.CellCount;
            var cellIndex = change.Index % BoardConstants.CellCount;
            
            _boardView.SetCell(subBoardIndex, cellIndex, (Mark)change.Value);
        }
        
        private void OnActiveSubBoardChanged(int previous, int current)
        {
            _boardView.SetActiveSubBoard(_gameManager.ActiveSubBoard.Value, _gameManager.Turn.Value);
        }

        private void OnSubBoardResultsChanged(NetworkListEvent<byte> change)
        {
            if (change.Type != NetworkListEvent<byte>.EventType.Value) return;
            
            _boardView.SetSubBoardResult(change.Index, (GameResult)change.Value);
        }

        private void OnTurnChanged(Mark previous, Mark current)
        {
            _boardView.SetActiveSubBoard(_gameManager.ActiveSubBoard.Value, _gameManager.Turn.Value);
        }

        private void OnXPendingChanged(Vector2Int previous, Vector2Int current)
        {
            if (_gameManager.SelfMark != Mark.X) return;
            if (current == BoardConstants.NoMove) return;
            
            if (previous != BoardConstants.NoMove)
                _boardView.SetCell(previous.x, previous.y, Mark.None);

            if (current != BoardConstants.NoMove)
                _boardView.SetCell(current.x, current.y, Mark.X);
        }
        
        private void OnOPendingChanged(Vector2Int previous, Vector2Int current)
        {
            if (_gameManager.SelfMark != Mark.O) return;
            if (current == BoardConstants.NoMove) return;
            
            if (previous != BoardConstants.NoMove)
                _boardView.SetCell(previous.x, previous.y, Mark.None);

            if (current != BoardConstants.NoMove)
                _boardView.SetCell(current.x, current.y, Mark.O);
        }

        private void OnStarted()
        {
            for (var index = 0; index < BoardConstants.TotalCellCount; index++)
            {
                var subBoardIndex = index / BoardConstants.CellCount;
                var cellIndex = index % BoardConstants.CellCount;
                
                _boardView.SetCell(subBoardIndex, cellIndex, _gameManager.GetCell(subBoardIndex, cellIndex));
            }
            
            _boardView.SetActiveSubBoard(_gameManager.ActiveSubBoard.Value, _gameManager.Turn.Value);

            for (var index = 0; index < BoardConstants.SubBoardCount; index++)
            {
                _boardView.SetSubBoardResult(index, (GameResult)_gameManager.SubBoardResults[index]);
            }
            
            _screenSwitcher.ShowBoardScreen();
        }

        private void OnRestarted()
        {
            _screenSwitcher.ShowBoardScreen();
        }
        
        private void OnSurrenderClicked()
        {
            _gameManager.RequestSurrenderRpc();
        }
        
        private void OnSettingsClicked()
        {
            // TEMP
        }
        
        private void OnCellClicked(int subBoardIndex, int cellIndex)
        {
            _gameManager.RequestMoveRpc(subBoardIndex, cellIndex);
        }
        
        public void Start() { }

        public void Dispose()
        {
            _gameManager.Cells.OnListChanged -= OnCellsChanged;
            _gameManager.ActiveSubBoard.OnValueChanged -= OnActiveSubBoardChanged;
            _gameManager.SubBoardResults.OnListChanged -= OnSubBoardResultsChanged;
            _gameManager.Turn.OnValueChanged -= OnTurnChanged;
            _gameManager.XPendingMark.OnValueChanged -= OnXPendingChanged;
            _gameManager.OPendingMark.OnValueChanged -= OnOPendingChanged;
            _gameManager.Started -= OnStarted;
            _gameManager.Restarted -= OnRestarted;
            
            _gameplayView.SurrenderClicked -= OnSurrenderClicked;
            _gameplayView.SettingsClicked -= OnSettingsClicked;
            
            _boardView.CellClicked -= OnCellClicked;
        }
    }
}