using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// Manages the core game loop, economy, and win calculations for the slot machine.
/// </summary>
public class SlotMachineManager : MonoBehaviour
{
    private int balance;
    private int currentBet;
    private int betStep = 10; 
    [Header("Economy Settings")]
    [SerializeField] private int minBet = 10;
    [SerializeField] private int maxBet = 100;

    [Header("Win Ratio Settings")]
    [Range(0f, 1f)]
    public float winProbability = 0.3f; 

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI betText;
    [SerializeField] private TextMeshProUGUI winText; 
    [SerializeField] private TextMeshProUGUI multiplierText; 
    [SerializeField] private RectTransform plusBtnTransform;
    [SerializeField] private RectTransform minusBtnTransform;
    [SerializeField] private RectTransform spinBtnTransform; 

    [Header("Game Components")]
    public ReelController[] reels; 
    public SymbolDataSO symbolDataSO; 

    private bool isSpinning = false;
    private IWinEvaluator _winEvaluator;
    private int consecutiveWins = 0; 

    void Start()
    {
        // SERVER INTEGRATION: Replace PlayerPrefs with API call to get user balance
        balance = PlayerPrefs.GetInt("PlayerBalance", 1000); 
        currentBet = PlayerPrefs.GetInt("CurrentBet", 10);
        consecutiveWins = PlayerPrefs.GetInt("ConsecutiveWins", 0); 
        
        _winEvaluator = new WinEvaluator(symbolDataSO);
        
        UpdateUI();
    }

    public async void OnSpinClick()
    {
        if (isSpinning) return; 
        if (balance < currentBet) return; 

        StartSpinSequence();

        // 1. Deduct Bet
        DeductBet();

        // 2. Generate Result
        // SERVER INTEGRATION: Replace local RNG with API call to get spin result
        int[][] results = GenerateResults();

        // 3. Start Spins
        await SpinReels(results);

        // 4. Handle Win/Loss
        HandleSpinResult(results);
        
        EndSpinSequence();
    }

    private void StartSpinSequence()
    {
        isSpinning = true;
        winText.text = ""; 
        winText.alpha = 1f; 
        if (multiplierText != null) multiplierText.text = ""; 

        if (spinBtnTransform != null)
        {
            spinBtnTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 10, 1);
        }

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySpinStart();
    }

    private void DeductBet()
    {
        // SERVER INTEGRATION: Send bet amount to server
        balance -= currentBet;
        SaveData();
        UpdateUI();
    }

    private async UniTask SpinReels(int[][] results)
    {
        UniTask[] spinTasks = new UniTask[reels.Length];
        for (int i = 0; i < reels.Length; i++)
        {
            // Staggered spin duration: 2.3s, 2.8s, 3.3s
            spinTasks[i] = reels[i].Spin(results[i], 2.3f + (i * 0.5f));
        }

        await UniTask.WhenAll(spinTasks); 
    }

    private void HandleSpinResult(int[][] results)
    {
        // Determine Multiplier based on consecutive wins
        int multiplier = 1;
        
        // Check if this spin is a win (without multiplier) to update streak
        var tempResult = _winEvaluator.Evaluate(results, currentBet, minBet, 1);
        
        if (tempResult.TotalWin > 0)
        {
            consecutiveWins++;
            multiplier = Mathf.Min(consecutiveWins, 5); // Cap multiplier at 5x
        }
        else
        {
            consecutiveWins = 0; 
        }

        // Evaluate final win with the calculated multiplier
        var winResult = _winEvaluator.Evaluate(results, currentBet, minBet, multiplier);
        
        if (winResult.TotalWin > 0)
        {
            ProcessWin(winResult);
        }
        
        UpdateUI();
    }

    private void ProcessWin(WinResult winResult)
    {
        // SERVER INTEGRATION: Validate win amount with server response
        balance += winResult.TotalWin;
        winText.text = $"WIN: {winResult.TotalWin}";
        
        winText.alpha = 1f;
        winText.DOFade(0f, 1f).SetDelay(3f);

        if (winResult.Multiplier > 1 && multiplierText != null)
        {
            multiplierText.text = $"{winResult.Multiplier}x!";
            multiplierText.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 1f, 10, 1);
        }

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayWin();

        SaveData();
        
        foreach (int lineIndex in winResult.WinningLineIndices)
        {
            HighlightLine(lineIndex);
        }
    }

    private void EndSpinSequence()
    {
        isSpinning = false;
    }

    // Simulates Server RNG
    private int[][] GenerateResults()
    {
        int symbolCount = symbolDataSO.GetSymbolCount();
        int[][] results = new int[3][];
        for (int i = 0; i < 3; i++)
        {
            results[i] = new int[3];
        }

        bool forceWin = Random.value <= winProbability;

        if (forceWin)
        {
            // Pick a random winning line
            int winType = Random.Range(0, 5); 
            int winningSymbol = Random.Range(0, symbolCount);

            // Fill with noise
            for (int c = 0; c < 3; c++)
            {
                for (int r = 0; r < 3; r++)
                {
                    results[c][r] = Random.Range(0, symbolCount);
                }
            }

            // Force winning line
            if (winType < 3) // Horizontal
            {
                int row = winType;
                results[0][row] = winningSymbol;
                results[1][row] = winningSymbol;
                results[2][row] = winningSymbol;
            }
            else if (winType == 3) // Diagonal 1
            {
                results[0][0] = winningSymbol;
                results[1][1] = winningSymbol;
                results[2][2] = winningSymbol;
            }
            else if (winType == 4) // Diagonal 2
            {
                results[2][0] = winningSymbol;
                results[1][1] = winningSymbol;
                results[0][2] = winningSymbol;
            }
        }
        else
        {
            // Ensure losing grid
            do
            {
                for (int c = 0; c < 3; c++)
                {
                    for (int r = 0; r < 3; r++)
                    {
                        results[c][r] = Random.Range(0, symbolCount);
                    }
                }
            } while (_winEvaluator.Evaluate(results, currentBet, minBet, 1).TotalWin > 0);
        }

        return results;
    }

    private void HighlightLine(int lineIndex)
    {
        if (lineIndex < 3) // Horizontal Rows
        {
            reels[0].HighlightSymbol(lineIndex);
            reels[1].HighlightSymbol(lineIndex);
            reels[2].HighlightSymbol(lineIndex);
        }
        else if (lineIndex == 3) // Diagonal 1
        {
            reels[0].HighlightSymbol(0);
            reels[1].HighlightSymbol(1);
            reels[2].HighlightSymbol(2);
        }
        else if (lineIndex == 4) // Diagonal 2
        {
            reels[2].HighlightSymbol(0);
            reels[1].HighlightSymbol(1);
            reels[0].HighlightSymbol(2);
        }
    }

    public void IncreaseBet()
    {
        if (isSpinning) return;
        if (currentBet + betStep <= maxBet)
        {
            currentBet += betStep;
            UpdateUI();
            
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayButtonClick();

            if (plusBtnTransform != null)
            {
                plusBtnTransform.DORotate(new Vector3(0, 0, -360), 0.5f, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutBack);
            }
        }
    }

    public void DecreaseBet()
    {
        if (isSpinning) return;
        if (currentBet - betStep >= minBet)
        {
            currentBet -= betStep;
            UpdateUI();
            
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayButtonClick();

            if (minusBtnTransform != null)
            {
                minusBtnTransform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutBack);
            }
        }
    }
    
    void UpdateUI()
    {
        balanceText.text = $"{balance}";
        betText.text = $"{currentBet}";
    }
    
    void SaveData()
    {
        // SERVER INTEGRATION: Sync state with server
        PlayerPrefs.SetInt("PlayerBalance", balance);
        PlayerPrefs.SetInt("CurrentBet", currentBet);
        PlayerPrefs.SetInt("ConsecutiveWins", consecutiveWins);
        PlayerPrefs.Save();
    }
}