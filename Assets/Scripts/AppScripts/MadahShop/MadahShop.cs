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
    public GridFlip KFlipped;
    
    public GlitchManager glitchManager;


    void Start()
    {
        PopulateShop();
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
    public void UpdateCoinText()
    {
        CurrentCoinsText.text = "Coins: " + GameManager.Instance.currentCoins.ToString();
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
                glitchManager.FreezeForDuration(upgrade.freezeDuration);
                break;
            case UpgradeSO.UpgradeType.InstantSolveKFlipped:
                KFlipped.InstantSolveOneRound();
                break;
            case UpgradeSO.UpgradeType.StopGhost:
                GameManager.Instance.ghost.FreezeForDuration(upgrade.stopDuration);

                break;
        }
    }
}
