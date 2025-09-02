using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ShopItemUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("UI References")]
    public Image background;
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text costText;
    public Button buyButton;

    [Header("Colors")]
    public Color baseColor = Color.white;

    private Color hoverColor;
    private Color pressedColor;
    private Color disabledColor;

    private UpgradeSO upgrade;
    private System.Action<UpgradeSO> onBuyClicked;

    private bool affordable = true;

    void Awake()
    {
        // Auto-generate variations
        hoverColor = baseColor * 0.9f;  // slightly darker
        pressedColor = baseColor * 0.75f; // darker
        disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);
    }

    public void Setup(UpgradeSO data, System.Action<UpgradeSO> onBuyClicked, bool canAfford = true)
    {
        upgrade = data;
        this.onBuyClicked = onBuyClicked;

        nameText.text = data.upgradeName;
        costText.text = data.cost.ToString();
        iconImage.sprite = data.icon;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuyClicked(upgrade));

        SetAffordable(canAfford);

        if (canAfford)
            SetBackgroundInstant(baseColor);
    }

    public void SetAffordable(bool canAfford)
    {
        affordable = canAfford;
        buyButton.interactable = canAfford;

        if (!canAfford)
        {
            SetBackground(disabledColor);
            iconImage.color = new Color(1f, 1f, 1f, 0.5f);
            nameText.color = new Color(1f, 1f, 1f, 0.5f);
            costText.color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            SetBackground(baseColor);
            iconImage.color = Color.white;
            nameText.color = Color.white;
            costText.color = Color.white;
        }
    }

    // Pointer events
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (affordable) SetBackground(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (affordable) SetBackground(baseColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (affordable) SetBackground(pressedColor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!affordable) return;

        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
            SetBackground(hoverColor);
        else
            SetBackground(baseColor);
    }

    // --- Helpers ---
    private void SetBackgroundInstant(Color c)
    {
        background.color = c;
    }

    private void SetBackground(Color target)
    {
        background.color = target;
    }
}
