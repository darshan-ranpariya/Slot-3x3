using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SlotMachine : MonoBehaviour
{
    // 3x3 grid
    public const int Rows = 3;
    public const int Cols = 3;

    // Symbols available (High to Low value)
    // "Seven", "Bar", "Bell", "Plum", "Orange", "Lemon", "Cherry"
    public readonly string[] symbols = { "Seven", "Bar", "Bell", "Plum", "Orange", "Lemon", "Cherry" };

    // Paytable (Symbol -> Base Win for 3 in a row)
    private Dictionary<string, int> paytable = new Dictionary<string, int>() {
        { "Seven", 100 },
        { "Bar", 50 },
        { "Bell", 20 },
        { "Plum", 10 },
        { "Orange", 5 },
        { "Lemon", 3 },
        { "Cherry", 2 }
    };

    // Game State
    public string[,] Grid { get; private set; }
    public int Balance { get; private set; } = 1000; // Starting balance
    public int CurrentBet { get; private set; } = 10;
    public int LastWin { get; private set; }
    
    // Feature: Voltage Multiplier
    public bool IsMultiplierActive { get; private set; }
    public int CurrentMultiplier { get; private set; } = 1;

    // Events
    public event Action OnSpinStart;
    public event Action OnSpinFinished;

    void Awake()
    {
        Grid = new string[Rows, Cols];
    }

    void Start()
    {
        // Initial random fill without logic so the grid isn't empty
        FillRandomGrid(); 
    }

    public void Spin()
    {
        if (Balance < CurrentBet)
        {
            Debug.Log("Not enough balance!");
            return;
        }

        // Deduct bet
        Balance -= CurrentBet;
        LastWin = 0;
        
        // Voltage Multiplier Logic (approx 1 in 50 chance)
        // We use 0-49 range; if 0, trigger feature.
        IsMultiplierActive = UnityEngine.Random.Range(0, 50) == 0;
        
        if (IsMultiplierActive)
        {
            // Multiplier can be 2x, 3x, or 5x
            int[] mults = { 2, 3, 5 };
            CurrentMultiplier = mults[UnityEngine.Random.Range(0, mults.Length)];
        }
        else
        {
            CurrentMultiplier = 1;
        }

        if (OnSpinStart != null) OnSpinStart.Invoke();

        // In a real game, we might wait for animation here. 
        // For now, we calculate immediately.
        FillRandomGrid();
        CalculateWin();
        
        // Add win to balance
        Balance += LastWin;

        PrintGrid(); // Debug log

        if (OnSpinFinished != null) OnSpinFinished.Invoke();
    }

    void FillRandomGrid()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                Grid[r, c] = symbols[UnityEngine.Random.Range(0, symbols.Length)];
            }
        }
    }

    void CalculateWin()
    {
        int roundWin = 0;

        // Check Horizontal Rows
        for (int r = 0; r < Rows; r++)
        {
            if (Grid[r, 0] == Grid[r, 1] && Grid[r, 1] == Grid[r, 2])
            {
                string symbol = Grid[r, 0];
                if (paytable.ContainsKey(symbol))
                {
                    int win = paytable[symbol];
                    roundWin += win;
                    Debug.Log($"Win on Row {r}: {symbol} (+{win})");
                }
            }
        }
        
        // Check Diagonal 1 (Top-Left to Bottom-Right)
        if (Grid[0, 0] == Grid[1, 1] && Grid[1, 1] == Grid[2, 2])
        {
             string symbol = Grid[1, 1];
             if (paytable.ContainsKey(symbol)) 
             {
                 int win = paytable[symbol];
                 roundWin += win;
                 Debug.Log($"Win on Diagonal 1: {symbol} (+{win})");
             }
        }
        
        // Check Diagonal 2 (Top-Right to Bottom-Left)
        if (Grid[0, 2] == Grid[1, 1] && Grid[1, 1] == Grid[2, 0])
        {
             string symbol = Grid[1, 1];
             if (paytable.ContainsKey(symbol)) 
             {
                 int win = paytable[symbol];
                 roundWin += win;
                 Debug.Log($"Win on Diagonal 2: {symbol} (+{win})");
             }
        }

        // Apply Multiplier
        if (IsMultiplierActive && roundWin > 0)
        {
            Debug.Log($"Voltage Multiplier Active! Win multiplied by {CurrentMultiplier}x");
        }
        
        LastWin = roundWin * CurrentMultiplier;
    }

    void PrintGrid()
    {
        string output = "Slot Grid:\n";
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                output += Grid[r, c] + "\t";
            }
            output += "\n";
        }
        Debug.Log(output);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Spin();
        }
    }
}
