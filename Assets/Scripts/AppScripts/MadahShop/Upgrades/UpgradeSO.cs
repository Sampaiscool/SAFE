using UnityEngine;

[CreateAssetMenu(menuName = "MadahShop/Upgrades", fileName = "NewUpgrade")]
public class UpgradeSO : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
    public int cost;

    // What kind of upgrade this is
    public UpgradeType type;

    [Header("No Glitch Time Settings")]
    public float freezeDuration;  // how long glitches stay frozen

    public enum UpgradeType
    {
        NoGlitchTime,
        InstantSolveKFlipped,
        OpenAllApplications,
    }
}
