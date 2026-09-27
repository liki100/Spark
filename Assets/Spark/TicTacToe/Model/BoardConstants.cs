using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spark.TicTacToe.Model
{
    public static class BoardConstants
    {
        public static readonly Vector2Int NoMove = new(-1, -1);
        
        public const ulong NoClient = ulong.MaxValue;
        
        public const int SubBoardCount = 9;
        public const int CellCount = 9;
        public const int TotalCellCount = SubBoardCount * CellCount;
        
        public static readonly Dictionary<GameMode, string> DisplayToMode = new()
        {
            { GameMode.TicTacToe, "Классический" }, 
            { GameMode.BlindingRun, "Альтернативный" }
        };
        
        public static GameMode ParseMode(string value)
        {
            return Enum.TryParse<GameMode>(value, out var mode)
                ? mode
                : GameMode.TicTacToe;
        }
    }
}