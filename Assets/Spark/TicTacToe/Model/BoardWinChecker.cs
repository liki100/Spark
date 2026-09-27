using System.Collections.Generic;
using UnityEngine;

namespace Spark.TicTacToe.Model
{
    public static class BoardWinChecker
    {
        private static readonly int[][] Lines =
        {
            new[] { 0, 1, 2 },
            new[] { 3, 4, 5 },
            new[] { 6, 7, 8 },
            new[] { 0, 3, 6 },
            new[] { 1, 4, 7 },
            new[] { 2, 5, 8 },
            new[] { 0, 4, 8 },
            new[] { 2, 4, 6 }
        };
        
        public static GameResult GetResult(IReadOnlyList<Mark> cells)
        {
            bool xWins = false;
            bool oWins = false;

            foreach (var line in Lines)
            {
                var a = cells[line[0]];
                var b = cells[line[1]];
                var c = cells[line[2]];

                if (a == Mark.X && b == Mark.X && c == Mark.X) xWins = true;
                if (a == Mark.O && b == Mark.O && c == Mark.O) oWins = true;
            }
            
            if (xWins && oWins)
                return Random.Range(0, 2) == 0
                    ? GameResult.XWins
                    : GameResult.OWins;

            if (xWins) return GameResult.XWins;

            if (oWins) return GameResult.OWins;

            foreach (var cell in cells)
            {
                if (cell == Mark.None)
                    return GameResult.None;
            }

            return GameResult.Draw;
        }
    }
}
