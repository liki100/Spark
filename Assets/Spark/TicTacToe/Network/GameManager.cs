using System;
using Spark.TicTacToe.Model;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace Spark.TicTacToe.Network
{
    public class GameManager : NetworkBehaviour
    {
        public readonly NetworkList<byte> Cells = new();
        public readonly NetworkVariable<int> ActiveSubBoard = new(-1);
        public readonly NetworkList<byte> SubBoardResults = new();
        public readonly NetworkVariable<Mark> Turn = new(Mark.X);
        public readonly NetworkVariable<GameResult> Result = new();

        public readonly NetworkVariable<bool> XWantsRematch = new();
        public readonly NetworkVariable<bool> OWantsRematch = new();

        public readonly NetworkVariable<int> ConnectedPlayerCount = new();
        public readonly NetworkVariable<ulong> XClientId = new(BoardConstants.NoClient);
        public readonly NetworkVariable<ulong> OClientId = new(BoardConstants.NoClient);

        public readonly NetworkVariable<Vector2Int> XPendingMark = new(BoardConstants.NoMove);
        public readonly NetworkVariable<Vector2Int> OPendingMark = new(BoardConstants.NoMove);
        
        public double TurnDeadline { get; private set; } = -1;

        public readonly Mark[] SubBoardCells = new Mark[BoardConstants.CellCount];
        public readonly Mark[] BoardCells = new Mark[BoardConstants.SubBoardCount];

        private IGameRules _rules;
        
        public event Action Started;
        public event Action Restarted;
        
        public ulong LocalClientId => NetworkManager.LocalClientId;

        public double RemainingTurnTime => TurnDeadline >= 0
            ? Math.Max(0, TurnDeadline - NetworkManager.ServerTime.Time)
            : _rules.TurnDurationSeconds;

        public Mark SelfMark =>
            XClientId.Value == LocalClientId ? Mark.X :
            OClientId.Value == LocalClientId ? Mark.O :
            Mark.None;

        public Mark OpponentMark =>
            SelfMark == Mark.X ? Mark.O :
            SelfMark == Mark.O ? Mark.X :
            Mark.None;

        public void SetRules(ISession session)
        {
            var mode = session.Properties.TryGetValue("mode", out var property) == false
                ? GameMode.TicTacToe
                : BoardConstants.ParseMode(property.Value);
            
            _rules = mode switch
            {
                GameMode.TicTacToe => new TicTacToeRules(this),
                GameMode.BlindingRun => new BlindingRunRules(this),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                ConnectedPlayerCount.Value = 0;
                XClientId.Value = BoardConstants.NoClient;
                OClientId.Value = BoardConstants.NoClient;

                XPendingMark.Value = BoardConstants.NoMove;
                OPendingMark.Value = BoardConstants.NoMove;

                if (Cells.Count == 0)
                {
                    for (var index = 0; index < BoardConstants.TotalCellCount; index++)
                        Cells.Add((byte)Mark.None);
                }

                if (SubBoardResults.Count == 0)
                {
                    for (var index = 0; index < BoardConstants.SubBoardCount; index++)
                        SubBoardResults.Add((byte)Mark.None);
                }

                NetworkManager.OnClientConnectedCallback += OnClientConnected;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer) NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        }

        private void Update() => _rules?.Update();
        
        public Mark GetCell(int subBoardIndex, int cellIndex) =>
            (Mark)Cells[subBoardIndex * BoardConstants.CellCount + cellIndex];

        public void SetCell(int subBoardIndex, int cellIndex, Mark mark) =>
            Cells[subBoardIndex * BoardConstants.CellCount + cellIndex] = (byte)mark;

        public bool TryGetSenderMark(RpcParams rpcParams, out Mark mark)
        {
            var senderId = rpcParams.Receive.SenderClientId;

            if (XClientId.Value == senderId)
            {
                mark = Mark.X;
                return true;
            }

            if (OClientId.Value == senderId)
            {
                mark = Mark.O;
                return true;
            }

            mark = Mark.None;
            return false;
        }

        private void OnClientConnected(ulong clientId)
        {
            ConnectedPlayerCount.Value++;
        }
        
        [Rpc(SendTo.Server)]
        public void RequestSelectMarkRpc(Mark mark, RpcParams rpcParams = default)
        {
            var senderId = rpcParams.Receive.SenderClientId;
            var targetSeat = mark == Mark.X ? XClientId : OClientId;

            if (targetSeat.Value != BoardConstants.NoClient) return;

            if (XClientId.Value == senderId) XClientId.Value = BoardConstants.NoClient;
            if (OClientId.Value == senderId) OClientId.Value = BoardConstants.NoClient;

            targetSeat.Value = senderId;

            if (XClientId.Value == BoardConstants.NoClient || OClientId.Value == BoardConstants.NoClient) return;

            _rules.ResetBoard();
            StartedRpc();
        }

        [Rpc(SendTo.Server)]
        public void RequestMoveRpc(int subBoardIndex, int cellIndex, RpcParams rpcParams = default)
        {
            _rules.RequestMove(subBoardIndex, cellIndex, rpcParams);
        }

        [Rpc(SendTo.Server)]
        public void RequestSurrenderRpc(RpcParams rpcParams = default)
        {
            if (Result.Value != GameResult.None) return;
            if (TryGetSenderMark(rpcParams, out var mark) == false) return;

            Result.Value = mark == Mark.X ? GameResult.OWins : GameResult.XWins;
        }

        [Rpc(SendTo.Server)]
        public void RequestRematchRpc(RpcParams rpcParams = default)
        {
            if (Result.Value == GameResult.None) return;
            if (TryGetSenderMark(rpcParams, out var mark) == false) return;

            if (mark == Mark.X) XWantsRematch.Value = true;
            else OWantsRematch.Value = true;

            if (!XWantsRematch.Value || !OWantsRematch.Value) return;

            _rules.ResetBoard();
            RestartedRpc();
        }
        
        [Rpc(SendTo.Everyone)]
        public void ResetTurnTimerRpc() => ResetTurnTimer();

        [Rpc(SendTo.Everyone)]
        public void StopTurnTimerRpc() => StopTurnTimer();

        [Rpc(SendTo.Everyone)]
        private void StartedRpc() => Started?.Invoke();

        [Rpc(SendTo.Everyone)]
        private void RestartedRpc() => Restarted?.Invoke();
        
        public void StopTurnTimer() => TurnDeadline = -1;

        private void ResetTurnTimer()
        {
            TurnDeadline = NetworkManager.ServerTime.Time + _rules.TurnDurationSeconds;
        }
    }
}