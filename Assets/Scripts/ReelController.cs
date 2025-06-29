using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReelController : MonoBehaviour
{
    [Header("Reels")]
    public ReelSpinner reel1;
    public ReelSpinner reel2;
    public ReelSpinner reel3;

    [Header("UI")]
    public Button spinButton;
    public TextMeshProUGUI prizeText;

    [Header("Account")]
    public AccountManager accountManager;  // bet=1, AddWin(mult)

    private void Start()
    {
        // Hook up the Spin button
        spinButton.onClick.AddListener(StartSpin);
        prizeText.text = "";
    }

    public void StartSpin()
    {
        if (!accountManager.PlaceBet())
        {
            prizeText.text = "Insufficient funds!";
            return;
        }

        spinButton.interactable = false;
        prizeText.text = "";
        StartCoroutine(SpinAllReels());
    }

    private IEnumerator SpinAllReels()
    {
        // Choose a random stop index on each reel (0..29)
        int stop1 = Random.Range(0, reel1.totalSymbols);
        int stop2 = Random.Range(0, reel2.totalSymbols);
        int stop3 = Random.Range(0, reel3.totalSymbols);

        // Staggered start for visual effect
        reel1.StartSpinAt(stop1);
        yield return new WaitForSeconds(0.3f);
        reel2.StartSpinAt(stop2);
        yield return new WaitForSeconds(0.3f);
        reel3.StartSpinAt(stop3);

        // Wait until all three reels have stopped
        yield return new WaitUntil(() => reel1.IsIdle && reel2.IsIdle && reel3.IsIdle);

        // Fetch the landed symbols
        SlotSymbol s1 = reel1.GetSymbolAt(stop1);
        SlotSymbol s2 = reel2.GetSymbolAt(stop2);
        SlotSymbol s3 = reel3.GetSymbolAt(stop3);

        // Calculate and display prize
        int multiplier = CalculatePrizeMultiplier(s1, s2, s3);
        if (multiplier > 0)
        {
            prizeText.text = $"You win {multiplier}×!";
            accountManager.AddWin(multiplier);
        }
        else
        {
            prizeText.text = "No Win";
        }

        spinButton.interactable = true;
    }

    /// <summary>
    /// Returns the 3-of-a-kind multiplier if all three symbols match; otherwise 0.
    /// (2-of-a-kind is ignored here since only 3-of-a-kind symbols pay.)
    /// </summary>
    private int CalculatePrizeMultiplier(SlotSymbol a, SlotSymbol b, SlotSymbol c)
    {
        // 3-of-a-kind
        if (a == b && b == c)
        {
            switch (a)
            {
                case SlotSymbol.Seven: return 1000;
                case SlotSymbol.Bar: return 500;
                case SlotSymbol.Diamond: return 250;
                case SlotSymbol.Bell: return 150;
                case SlotSymbol.Cherry: return 80;
                default: return 0;   // Lemon/Watermelon/Grapes/Orange
            }
        }

        // exactly 2-of-a-kind
        if (a == b || a == c || b == c)
        {
            var sym = (a == b || a == c) ? a : b;
            switch (sym)
            {
                case SlotSymbol.Seven: return 100;
                case SlotSymbol.Bar: return 75;
                case SlotSymbol.Diamond: return 40;
                case SlotSymbol.Bell: return 30;
                case SlotSymbol.Cherry: return 25;
                default: return 0;  // non-paying symbols
            }
        }

        return 0;
    }
}




public enum SlotSymbol
{
    Seven,
    Bar,
    Diamond,
    Bell,
    Cherry,
    Lemon,
    Watermelon,
    Grapes,
    Orange
}
