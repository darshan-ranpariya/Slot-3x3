using System.Collections.Generic;
using UnityEngine;

public class WinEvaluator : IWinEvaluator
{
    private readonly SymbolDataSO _symbolData;

    public WinEvaluator(SymbolDataSO symbolData)
    {
        _symbolData = symbolData;
    }

    public WinResult Evaluate(int[][] grid, int currentBet, int minBet, int multiplier)
    {
        WinResult result = new WinResult
        {
            TotalWin = 0,
            Multiplier = multiplier,
            WinningLineIndices = new List<int>()
        };

        // Check Horizontal Lines
        for (int row = 0; row < 3; row++)
        {
            if (grid[0][row] == grid[1][row] && grid[1][row] == grid[2][row])
            {
                int symbolId = grid[0][row];
                SymbolData data = _symbolData.GetSymbolData(symbolId);
                if (data != null)
                {
                    // Calculate Payout: Base Value * Bet Multiplier * Feature Multiplier
                    int lineWin = (data.payoutValue * (currentBet / minBet)) * multiplier;
                    result.TotalWin += lineWin;
                    result.WinningLineIndices.Add(row);
                }
            }
        }

        // Check Diagonal 1 (Top-Left -> Bottom-Right)
        if (grid[0][0] == grid[1][1] && grid[1][1] == grid[2][2])
        {
            int symbolId = grid[1][1];
            SymbolData data = _symbolData.GetSymbolData(symbolId);
            if (data != null)
            {
                int lineWin = (data.payoutValue * (currentBet / minBet)) * multiplier;
                result.TotalWin += lineWin;
                result.WinningLineIndices.Add(3); 
            }
        }

        // Check Diagonal 2 (Top-Right -> Bottom-Left)
        if (grid[2][0] == grid[1][1] && grid[1][1] == grid[0][2])
        {
            int symbolId = grid[1][1];
            SymbolData data = _symbolData.GetSymbolData(symbolId);
            if (data != null)
            {
                int lineWin = (data.payoutValue * (currentBet / minBet)) * multiplier;
                result.TotalWin += lineWin;
                result.WinningLineIndices.Add(4);
            }
        }

        return result;
    }
}
