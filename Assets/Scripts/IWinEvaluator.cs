using System.Collections.Generic;

public interface IWinEvaluator
{
    WinResult Evaluate(int[][] grid, int currentBet, int minBet, int multiplier);
}

public struct WinResult
{
    public int TotalWin;
    public int Multiplier;
    public List<int> WinningLineIndices;
}
