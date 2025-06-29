using UnityEngine;
using TMPro;

public class AccountManager : MonoBehaviour
{
    [Header("Account Settings")]
    [Tooltip("Starting balance for the player")]
    public int startingBalance = 100;
    [Tooltip("Amount to deduct per spin")]
    public int betAmount = 1;

    [Header("UI")]
    [Tooltip("Text component to display current balance")]
    public TextMeshProUGUI balanceText;

    private int balance;

    void Start()
    {
        // Initialize and show the starting balance
        balance = startingBalance;
        UpdateUI();
    }

    /// <summary>
    /// Attempt to place the standard bet.
    /// Returns true if the bet was placed, false if insufficient funds.
    /// </summary>
    public bool PlaceBet()
    {
        if (balance < betAmount)
            return false;

        balance -= betAmount;
        UpdateUI();
        return true;
    }

    /// <summary>
    /// Credit the player's account by (multiplier × betAmount).
    /// E.g. if multiplier=100 and betAmount=1, adds 100.
    /// </summary>
    public void AddWin(int multiplier)
    {
        int winAmount = multiplier * betAmount;
        balance += winAmount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (balanceText != null)
            balanceText.text = $"Balance: {balance}";
    }
}
