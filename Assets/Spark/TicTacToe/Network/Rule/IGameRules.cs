using Unity.Netcode;

namespace Spark.TicTacToe.Network
{
    public interface IGameRules
    {
        public string Debug { get; }
        
        public float TurnDurationSeconds { get; }
        
        void Initialize();

        void Update();

        void RequestMove(int subBoardIndex, int cellIndex, RpcParams rpcParams);

        void ResetBoard();

        void Dispose();
    }
}