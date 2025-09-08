using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MadahShop : MonoBehaviour
{
    public List<UpgradeSO> availableUpgrades;
    public GameObject shopItemPrefab; 
    public Transform contentParent;
    public TMP_Text CurrentCoinsText;

    public GameObject floatingTextPrefab; // prefab met een TMP text
    public Transform floatingTextSpawnPoint; // waar de tekst spawnt (bv. naast je coin UI)

    private float passivegainInterval = 15f;

    void Start()
    {
        PopulateShop();

        InvokeRepeating("PassiveCoinGain", passivegainInterval, passivegainInterval);
    }

    public void PassiveCoinGain()
    {
        GameManager.Instance.CoinsChange(1, false);
    }

    void PopulateShop()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject); // clear old items

        foreach (var upgrade in availableUpgrades)
        {
            GameObject itemGO = Instantiate(shopItemPrefab, contentParent);
            ShopItemUI ui = itemGO.GetComponent<ShopItemUI>();
            ui.Setup(upgrade, TryPurchase);
        }
    }
    public void UpdateCoinText(int changeAmount)
    {
        // Update de main counter
        CurrentCoinsText.text = "Coins: " + GameManager.Instance.currentCoins;

        // Spawn het kleine effectje
        if (floatingTextPrefab != null)
        {
            GameObject ft = Instantiate(floatingTextPrefab, floatingTextSpawnPoint.position, Quaternion.identity, floatingTextSpawnPoint);

            TextMeshProUGUI tmp = ft.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = (changeAmount >= 0 ? "+" : "") + changeAmount.ToString();
                tmp.color = changeAmount >= 0 ? Color.green : Color.red;
            }
        }
    }

    void TryPurchase(UpgradeSO upgrade)
    {
        if (GameManager.Instance.currentCoins < upgrade.cost)
        {
            //Not enough Money
            return;
        }

        GameManager.Instance.CoinsChange(upgrade.cost, true);
        ApplyUpgrade(upgrade);
    }

    void ApplyUpgrade(UpgradeSO upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeSO.UpgradeType.NoGlitchTime:
                GameManager.Instance.glitchManager.FreezeForDuration(upgrade.freezeDuration);
                break;
            case UpgradeSO.UpgradeType.InstantSolveKFlipped:
                GameManager.Instance.kFlipped.InstantSolveOneRound();
                break;
            case UpgradeSO.UpgradeType.StopGhost:
                GameManager.Instance.ghost.FreezeForDuration(upgrade.stopDuration);

                break;
        }
    }
}
