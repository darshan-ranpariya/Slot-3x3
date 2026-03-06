using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public SlotMachine slotMachine;
    
    // Unity doesn't serialize 2D arrays, so we use a 1D array of size 9 (3x3)
    // Index = row * Cols + col
    public Text[] gridTexts; 
    
    public Button spinButton;
    public Text balanceText;
    public Text winText;
    public Text multiplierText;

    void Start()
    {
        if (slotMachine == null)
            slotMachine = FindObjectOfType<SlotMachine>();

        if (spinButton != null)
            spinButton.onClick.AddListener(slotMachine.Spin);
            
        if (slotMachine != null)
        {
            slotMachine.OnSpinFinished += UpdateUI;
            // Initial update if grid is ready
            if (slotMachine.Grid != null)
            {
                UpdateUI();
            }
        }
    }
    
    void OnDestroy()
    {
        if (slotMachine != null)
            slotMachine.OnSpinFinished -= UpdateUI;
    }

    void UpdateUI()
    {
        if (gridTexts == null || gridTexts.Length < SlotMachine.Rows * SlotMachine.Cols) return;

        string[,] grid = slotMachine.Grid;
        if (grid == null) return;
        
        for (int r = 0; r < SlotMachine.Rows; r++)
        {
            for (int c = 0; c < SlotMachine.Cols; c++)
            {
                int index = r * SlotMachine.Cols + c;
                if (index < gridTexts.Length && gridTexts[index] != null)
                {
                    gridTexts[index].text = grid[r, c];
                }
            }
        }

        if (balanceText != null)
            balanceText.text = "Balance: " + slotMachine.Balance;

        if (winText != null)
            winText.text = "Win: " + slotMachine.LastWin;

        if (multiplierText != null)
        {
            if (slotMachine.IsMultiplierActive)
                multiplierText.text = "Multiplier: " + slotMachine.CurrentMultiplier + "x";
            else
                multiplierText.text = "";
        }
    }
}
