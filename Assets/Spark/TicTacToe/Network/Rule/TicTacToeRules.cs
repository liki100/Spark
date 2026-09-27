using Spark.TicTacToe.Model;
using Unity.Netcode;

namespace Spark.TicTacToe.Network
{
    public class TicTacToeRules : IGameRules
    {
        public string Debug => "TicTacToe";

        private readonly GameManager _gameManager;

        public TicTacToeRules(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public float TurnDurationSeconds => 30f;

        public void Initialize() { }

        public void Update()
        {
            if (_gameManager.IsServer == false) return;
            if (_gameManager.Result.Value != GameResult.None) return;
            if (_gameManager.TurnDeadline < 0) return;
            if (_gameManager.NetworkManager.ServerTime.Time < _gameManager.TurnDeadline) return;

            _gameManager.ResetTurnTimerRpc();

            _gameManager.Turn.Value = _gameManager.Turn.Value == Mark.X ? Mark.O : Mark.X;
            _gameManager.ActiveSubBoard.Value = -1;
        }

        public void RequestMove(int subBoardIndex, int cellIndex, RpcParams rpcParams = default)
        {
            if (_gameManager.Result.Value != GameResult.None) return;
            if (_gameManager.TryGetSenderMark(rpcParams, out var mark) == false) return;
            if (mark != _gameManager.Turn.Value) return;

            if (subBoardIndex < 0 || subBoardIndex >= BoardConstants.SubBoardCount) return;
            if (cellIndex < 0 || cellIndex >= BoardConstants.CellCount) return;
            if (_gameManager.ActiveSubBoard.Value != -1 && _gameManager.ActiveSubBoard.Value != subBoardIndex) return;
            if ((GameResult)_gameManager.SubBoardResults[subBoardIndex] != GameResult.None) return;
            if (_gameManager.GetCell(subBoardIndex, cellIndex) != Mark.None) return;

            _gameManager.SetCell(subBoardIndex, cellIndex, mark);

            for (var index = 0; index < BoardConstants.CellCount; index++)
                _gameManager.SubBoardCells[index] = _gameManager.GetCell(subBoardIndex, index);

            var subBoardResult = BoardWinChecker.GetResult(_gameManager.SubBoardCells);
            _gameManager.SubBoardResults[subBoardIndex] = (byte)subBoardResult;

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

            var boardResult = BoardWinChecker.GetResult(_gameManager.BoardCells);
            _gameManager.Result.Value = boardResult;

            _gameManager.ActiveSubBoard.Value = (GameResult)_gameManager.SubBoardResults[cellIndex] == GameResult.None ? cellIndex : -1;
            _gameManager.Turn.Value = mark == Mark.X ? Mark.O : Mark.X;

            if (_gameManager.Result.Value == GameResult.None) _gameManager.ResetTurnTimerRpc();
            else _gameManager.StopTurnTimerRpc();
        }

        public void ResetBoard()
        {
            for (var index = 0; index < _gameManager.Cells.Count; index++)
                _gameManager.Cells[index] = (byte)Mark.None;

            _gameManager.ActiveSubBoard.Value = -1;

            for (var index = 0; index < _gameManager.SubBoardResults.Count; index++)
                _gameManager.SubBoardResults[index] = (byte)Mark.None;

            _gameManager.Result.Value = GameResult.None;
            _gameManager.Turn.Value = Mark.X;

            _gameManager.XWantsRematch.Value = false;
            _gameManager.OWantsRematch.Value = false;

            _gameManager.StopTurnTimerRpc();
        }

        public void Dispose() { }
    }
}