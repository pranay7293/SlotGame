using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct SymbolSprite
{
    public SlotSymbol symbol;
    public Sprite sprite;
}

public class ReelSpinner : MonoBehaviour
{
    [Header("Reel Content & Prefab")]
    public RectTransform reelContent;           // masked viewport
    public GameObject symbolPrefab;             // UI→Image prefab (200×200)
    public List<SymbolSprite> spriteMappings;   // enum→sprite map

    [Header("Symbol Distribution (30 total)")]
    public List<SlotSymbol> symbolSequence;     // must be exactly 30 entries

    [Header("Spin Settings")]
    public float symbolHeight = 200f;           // must match prefab height
    public float spinSpeed = 2000f;             // units/sec
    public float decelerationTime = 1.0f;       // sec to ease out
    public int extraLoops = 3;                  // full loops before decel

    public bool IsIdle => isIdle;
    private bool isIdle = true;

    private float startX;
    private float oneReelHeight;    // 30×symbolHeight
    private float doubledHeight;    // 2×oneReelHeight
    public int totalSymbols;                // 30

    private void Awake()
    {
        totalSymbols = symbolSequence.Count;
        oneReelHeight = symbolHeight * totalSymbols;
        doubledHeight = oneReelHeight * 2f;

        // Make reelContent twice as tall so we can scroll seamlessly
        reelContent.pivot = new Vector2(0.5f, 1f); // top
        reelContent.sizeDelta = new Vector2(
            reelContent.sizeDelta.x,
            doubledHeight
        );
    }

    void Start()
    {
        startX = reelContent.anchoredPosition.x;

        PopulateReel();
    }

    void PopulateReel()
    {
        // clear old children
        foreach (Transform c in reelContent) Destroy(c.gameObject);

        // instantiate the sequence twice back-to-back
        for (int pass = 0; pass < 2; pass++)
        {
            foreach (var sym in symbolSequence)
            {
                var go = Instantiate(symbolPrefab, reelContent);
                var img = go.GetComponent<Image>();
                img.sprite = spriteMappings.Find(m => m.symbol == sym).sprite;

                var rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(200f, symbolHeight);
            }
        }

        // snap to top of first strip
        reelContent.anchoredPosition = new Vector2(startX, 0f);
    }

    public void StartSpinAt(int stopIndex)
    {
        if (isIdle)
            StartCoroutine(SpinRoutine(stopIndex));
    }

    private IEnumerator SpinRoutine(int stopIndex)
    {
        isIdle = false;

        // current displayed Y
        float displayY = reelContent.anchoredPosition.y;

        // compute how many symbol steps to go
        int currentIndex = Mathf.FloorToInt(displayY / symbolHeight) % totalSymbols;
        if (currentIndex < 0) currentIndex += totalSymbols;
        int steps = extraLoops * totalSymbols + ((stopIndex - currentIndex + totalSymbols) % totalSymbols);

        // timeline distances
        float startY = displayY;
        float targetY = startY + steps * symbolHeight;
        float decelDist = spinSpeed * decelerationTime;
        float fastEndY = targetY - decelDist;

        // FAST SPIN: constant speed until fastEndY
        while (displayY < fastEndY)
        {
            displayY += spinSpeed * Time.deltaTime;
            float wrapY = Mod(displayY, oneReelHeight);
            reelContent.anchoredPosition = new Vector2(startX, wrapY);
            yield return null;
        }

        // DECEL phase: cubic ease-out from fastEndY → targetY
        float elapsed = 0f;
        while (elapsed < decelerationTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / decelerationTime);
            float ease = 1f - Mathf.Pow(1f - t, 3);
            float lerpY = Mathf.Lerp(fastEndY, targetY, ease);
            float wrapY = Mod(lerpY, oneReelHeight);
            reelContent.anchoredPosition = new Vector2(startX, wrapY);
            yield return null;
        }

        // FINAL SNAP: exact payline position
        reelContent.anchoredPosition = new Vector2(startX, stopIndex * symbolHeight);
        isIdle = true;
    }

    // proper float mod (handles negatives)
    private float Mod(float x, float m) => ((x % m) + m) % m;

    // what symbol sits at this stop index
    public SlotSymbol GetSymbolAt(int index)
    {
        if (totalSymbols == 0)
        {
            Debug.LogError("ReelSpinner: symbolSequence.Count is zero in GetSymbolAt!");
            return default;
        }
        int i = ((index % totalSymbols) + totalSymbols) % totalSymbols;
        return symbolSequence[i];
    }
}
