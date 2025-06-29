using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour
{
    [Header("Reels (Logic Only)")]
    public ReelSpinner reel1;
    public ReelSpinner reel2;
    public ReelSpinner reel3;

    [Header("Default Simulation")]
    [Tooltip("Default number of spins to run on Start()")]
    public int spinsToRun = 100000;

    [Header("Custom Simulation UI")]
    [Tooltip("Enter number of spins here")]
    public TMP_InputField spinCountInput;
    [Tooltip("Click to run a custom simulation")]
    public Button runCustomButton;

    [Header("Bet Settings")]
    public float betPerSpin = 1f;
    [Tooltip("How many spins to process per frame for UI responsiveness")]
    public int batchSize = 10000;

    [Header("Results UI")]
    public TextMeshProUGUI totalBetText;
    public TextMeshProUGUI totalPayoutText;
    public TextMeshProUGUI rtpText;

    void Awake()
    {
        // Wire up the custom-run button
        runCustomButton.onClick.AddListener(OnRunCustomClicked);
    }

    void Start()
    {
        // Run the default simulation on start
        StartCoroutine(RunSimulationCoroutine(spinsToRun));
    }

    private void OnRunCustomClicked()
    {
        // Parse the player's input
        if (int.TryParse(spinCountInput.text, out int customSpins) && customSpins > 0)
        {
            // Disable the button while sim is running
            runCustomButton.interactable = false;
            StartCoroutine(RunSimulationCoroutine(customSpins, reenableButton: true));
        }
        else
        {
            // Invalid input feedback
            spinCountInput.textComponent.color = Color.red;
            Invoke(nameof(ResetInputColor), 1f);
        }
    }

    private void ResetInputColor()
    {
        spinCountInput.textComponent.color = Color.black;
    }

    private IEnumerator RunSimulationCoroutine(int targetSpins, bool reenableButton = false)
    {
        double sumReturns = 0.0, sumSquares = 0.0, totalBet = 0.0, totalPayout = 0.0;
        int spinsDone = 0;
        int n1 = reel1.totalSymbols, n2 = reel2.totalSymbols, n3 = reel3.totalSymbols;
        if (n1 == 0 || n2 == 0 || n3 == 0) { Debug.LogError("One reel is empty."); yield break; }

        while (spinsDone < targetSpins)
        {
            int batch = Math.Min(batchSize, targetSpins - spinsDone);
            for (int i = 0; i < batch; i++)
            {
                totalBet += betPerSpin;
                // random stops
                int s1 = UnityEngine.Random.Range(0, n1);
                int s2 = UnityEngine.Random.Range(0, n2);
                int s3 = UnityEngine.Random.Range(0, n3);
                // center symbols
                var c1 = reel1.GetSymbolAt(s1);
                var c2 = reel2.GetSymbolAt(s2);
                var c3 = reel3.GetSymbolAt(s3);
                int mult = GetPrizeMultiplier(c1, c2, c3);
                double ret = betPerSpin * mult;
                totalPayout += ret;
                sumReturns += ret;
                sumSquares += ret * ret;
            }
            spinsDone += batch;

            // update UI + 95% CI
            double mean = sumReturns / spinsDone;
            double var = (sumSquares / spinsDone) - (mean * mean);
            double se = Math.Sqrt(var / spinsDone);
            double ci95 = 1.96 * se * 100.0;
            double rtp = mean * 100.0;

            totalBetText.text = $"Total Bet: {totalBet:F0}";
            totalPayoutText.text = $"Total Payout: {totalPayout:F0}";
            rtpText.text = $"{rtp:F2}% ± {ci95:F2}% (95% CI)";

            yield return null;
        }

        if (reenableButton)
            runCustomButton.interactable = true;

        Debug.Log($"Simulation ({targetSpins} spins) done: RTP ≈ {sumReturns / targetSpins * 100.0:F2}% ± {1.96 * Math.Sqrt(((sumSquares / targetSpins) - (sumReturns / targetSpins) * (sumReturns / targetSpins)) / targetSpins) * 100.0:F2}%");
    }

    private int GetPrizeMultiplier(SlotSymbol a, SlotSymbol b, SlotSymbol c)
    {
        // identical to your paytable logic
        if (a == b && b == c)
        {
            switch (a)
            {
                case SlotSymbol.Seven: return 1000;
                case SlotSymbol.Bar: return 500;
                case SlotSymbol.Diamond: return 250;
                case SlotSymbol.Bell: return 150;
                case SlotSymbol.Cherry: return 80;
                default: return 0;
            }
        }
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
                default: return 0;
            }
        }
        return 0;
    }
}
