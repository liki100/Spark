using System.Collections.Generic;
using Spark.TicTacToe.Model;
using Unity.Netcode;
using UnityEngine;

namespace Spark.TicTacToe.Network
{
    public class BlindingRunRules : IGameRules
    {
        public string Debug => "Blinding Run Rules";
        
        private readonly GameManager _gameManager;

        public BlindingRunRules(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public float TurnDurationSeconds => 10f;

        public void Initialize() { }

        public void Update()
        {
            if (_gameManager.IsServer == false) return;
            if (_gameManager.Result.Value != GameResult.None) return;
            if (_gameManager.TurnDeadline < 0) return;
            if (_gameManager.NetworkManager.ServerTime.Time < _gameManager.TurnDeadline) return;

            var x = _gameManager.XPendingMark.Value;
            var o = _gameManager.OPendingMark.Value;

            _gameManager.SetCell(x.x, x.y, Mark.Draw);
            _gameManager.SetCell(x.x, x.y, Mark.None);
            _gameManager.XPendingMark.Value = BoardConstants.NoMove;
            
            _gameManager.SetCell(o.x, o.y, Mark.Draw);
            _gameManager.SetCell(o.x, o.y, Mark.None);
            _gameManager.OPendingMark.Value = BoardConstants.NoMove;

            if (x == BoardConstants.NoMove && o == BoardConstants.NoMove)
            {
                _gameManager.ResetTurnTimerRpc();
                return;
            }

            if (x == o)
            {
                var subBoardIndex = x.x;
                var freeCells = new List<int>();

                for (var index = 0; index < BoardConstants.CellCount; index++)
                {
                    if (_gameManager.GetCell(subBoardIndex, index) == Mark.None)
                        freeCells.Add(index);
                }

                if (freeCells.Count > 1)
                {
                    var randomIndex = Random.Range(0, freeCells.Count);
                    _gameManager.SetCell(subBoardIndex, freeCells[randomIndex], Mark.X);

                    freeCells.RemoveAt(randomIndex);

                    randomIndex = Random.Range(0, freeCells.Count);
                    _gameManager.SetCell(subBoardIndex, freeCells[randomIndex], Mark.O);
                }
                else
                {
                    _gameManager.SetCell(x.x, x.y, Mark.Draw);
                }

                UpdateSubBoardResult(subBoardIndex);
            }
            else
            {
                if (x != BoardConstants.NoMove) _gameManager.SetCell(x.x, x.y, Mark.X);
                if (o != BoardConstants.NoMove) _gameManager.SetCell(o.x, o.y, Mark.O);

                if (x != BoardConstants.NoMove) UpdateSubBoardResult(x.x);
                if (o != BoardConstants.NoMove && x.x != o.x) UpdateSubBoardResult(o.x);
            }

            UpdateBoardResult();

            if (_gameManager.Result.Value == GameResult.None) _gameManager.ResetTurnTimerRpc();
            else _gameManager.StopTurnTimerRpc();
        }

        public void RequestMove(int subBoardIndex, int cellIndex, RpcParams rpcParams = default)
        {
            if (_gameManager.Result.Value != GameResult.None) return;
            if (_gameManager.TryGetSenderMark(rpcParams, out var mark) == false) return;
            if (subBoardIndex < 0 || subBoardIndex >= BoardConstants.SubBoardCount) return;
            if (cellIndex < 0 || cellIndex >= BoardConstants.CellCount) return;
            if ((GameResult)_gameManager.SubBoardResults[subBoardIndex] != GameResult.None) return;
            if (_gameManager.GetCell(subBoardIndex, cellIndex) != Mark.None) return;

            if (mark == Mark.X) _gameManager.XPendingMark.Value = new Vector2Int(subBoardIndex, cellIndex);
            else _gameManager.OPendingMark.Value = new Vector2Int(subBoardIndex, cellIndex);
        }

        public void ResetBoard()
        {
            _gameManager.XPendingMark.Value = BoardConstants.NoMove;
            _gameManager.OPendingMark.Value = BoardConstants.NoMove;

            for (var index = 0; index < _gameManager.Cells.Count; index++)
                _gameManager.Cells[index] = (byte)Mark.None;

            for (var index = 0; index < _gameManager.SubBoardResults.Count; index++)
                _gameManager.SubBoardResults[index] = (byte)Mark.None;

            _gameManager.Result.Value = GameResult.None;

            _gameManager.XWantsRematch.Value = false;
            _gameManager.OWantsRematch.Value = false;

            _gameManager.ResetTurnTimerRpc();
        }

        private void UpdateSubBoardResult(int subBoardIndex)
        {
            for (var index = 0; index < BoardConstants.CellCount; index++)
                _gameManager.SubBoardCells[index] = _gameManager.GetCell(subBoardIndex, index);

            var subBoardResult = BoardWinChecker.GetResult(_gameManager.SubBoardCells);
            _gameManager.SubBoardResults[subBoardIndex] = (byte)subBoardResult;
        }

        private void UpdateBoardResult()
        {
            for (var index = 0; index < BoardConstants.SubBoardCount; index++)
            {
                _gameManager.BoardCells[index] = (GameResult)_gameManager.SubBoardResults[index] switch
                {
                    GameResult.XWins => Mark.X,
                    GameResult.OWins => Mark.O,
                    GameResult.Draw => Mark.Draw,
                    _ => Mark.None
                };
            }

            _gameManager.Result.Value = BoardWinChecker.GetResult(_gameManager.BoardCells);
        }

        public void Dispose() { }
    }
}