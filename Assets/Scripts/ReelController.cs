using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using DG.Tweening;

public class ReelController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private SymbolDataSO symbolDataSO;
    [SerializeField] private Image symbolPrefab; 
    [SerializeField] private float symbolHeight = 75f;
    [SerializeField] private int totalSymbols = 5; // 3 visible + 1 top buffer + 1 bottom buffer
    [SerializeField] private float spinSpeed = 1000f; 

    private List<Image> _symbolImages = new List<Image>();
    private int[] _finalResultIds;
    private float _thresholdY; 

    private void Awake()
    {
        InitializeReel();
    }

    // Sets up the reel strip by instantiating symbols and positioning them.
    private void InitializeReel()
    {
        while (_symbolImages.Count < totalSymbols)
        {
            Image img = Instantiate(symbolPrefab, transform);
            _symbolImages.Add(img);
        }

        int middleIndex = totalSymbols / 2; 

        for (int i = 0; i < _symbolImages.Count; i++)
        {
            // Position symbols vertically centered around Y=0
            float y = (middleIndex - i) * symbolHeight;
            _symbolImages[i].rectTransform.anchoredPosition = new Vector2(0, y);
            
            // Assign initial random sprites
            int randomId = Random.Range(0, symbolDataSO.GetSymbolCount());
            _symbolImages[i].sprite = symbolDataSO.GetSymbolData(randomId).symbolSprite;
        }

        // Calculate the Y position where a symbol wraps around to the top
        _thresholdY = ((middleIndex - (totalSymbols - 1)) * symbolHeight) - (symbolHeight * 0.5f);
    }

    // Handles the spin animation sequence: Spin -> Move -> Stop
    public async UniTask Spin(int[] finalResultIds, float duration)
    {
        _finalResultIds = finalResultIds;

        // Reset visual state from previous spin
        foreach (var img in _symbolImages)
        {
            img.transform.localScale = Vector3.one;
            img.color = Color.white;
        }

        float elapsed = 0f;

        // Spin Loop: Move strip continuously for the duration
        while (elapsed < duration)
        {
            MoveStrip(Time.deltaTime);
            elapsed += Time.deltaTime;
            await UniTask.Yield();
        }

        // Stop Sequence: Snap to the final result
        await SnapToResult();
    }

    // Moves all symbols down and handles wrapping logic
    private void MoveStrip(float dt)
    {
        float moveAmount = spinSpeed * dt;

        for (int i = 0; i < _symbolImages.Count; i++)
        {
            Vector2 pos = _symbolImages[i].rectTransform.anchoredPosition;
            pos.y -= moveAmount;

            // Check if symbol has moved below the bottom threshold
            if (pos.y <= _thresholdY)
            {
                // Wrap to top
                pos.y += (totalSymbols * symbolHeight);
                UpdateSpriteOnRecycle(i);
            }

            _symbolImages[i].rectTransform.anchoredPosition = pos;
        }
    }

    // Assigns a random sprite when a symbol wraps to the top to simulate an infinite strip
    private void UpdateSpriteOnRecycle(int index)
    {
        int randomId = Random.Range(0, symbolDataSO.GetSymbolCount());
        _symbolImages[index].sprite = symbolDataSO.GetSymbolData(randomId).symbolSprite;
    }

    // Aligns the reel to show the final result symbols
    private async UniTask SnapToResult()
    {
        // Sort images by physical Y position (Top to Bottom)
        _symbolImages.Sort((a, b) => b.rectTransform.anchoredPosition.y.CompareTo(a.rectTransform.anchoredPosition.y));

        // Assign the determined result sprites to the visible slots (Indices 1, 2, 3)
        _symbolImages[1].sprite = symbolDataSO.GetSymbolData(_finalResultIds[0]).symbolSprite;
        _symbolImages[2].sprite = symbolDataSO.GetSymbolData(_finalResultIds[1]).symbolSprite;
        _symbolImages[3].sprite = symbolDataSO.GetSymbolData(_finalResultIds[2]).symbolSprite;
        
        // Randomize buffer symbols
        _symbolImages[0].sprite = symbolDataSO.GetSymbolData(Random.Range(0, symbolDataSO.GetSymbolCount())).symbolSprite;
        _symbolImages[4].sprite = symbolDataSO.GetSymbolData(Random.Range(0, symbolDataSO.GetSymbolCount())).symbolSprite;

        List<UniTask> tweens = new List<UniTask>();
        int middleIndex = totalSymbols / 2;
        
        // Animate symbols to their exact target positions with a bounce effect
        for (int i = 0; i < _symbolImages.Count; i++)
        {
            float targetY = (middleIndex - i) * symbolHeight;
            tweens.Add(_symbolImages[i].rectTransform.DOAnchorPosY(targetY, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask());
        }

        await UniTask.WhenAll(tweens);
    }

    // Highlights a specific symbol (Top, Mid, or Bot) for winning lines
    public void HighlightSymbol(int rowIndex)
    {
        float targetY = 0;
        if (rowIndex == 0) targetY = 75;
        if (rowIndex == 1) targetY = 0;
        if (rowIndex == 2) targetY = -75;

        // Find the symbol currently at the target Y position
        foreach (var img in _symbolImages)
        {
            if (Mathf.Abs(img.rectTransform.anchoredPosition.y - targetY) < 1f)
            {
                img.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f, 10, 1);
                img.DOColor(Color.yellow, 0.2f).SetLoops(2, LoopType.Yoyo);
                break;
            }
        }
    }
}
