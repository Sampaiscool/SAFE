using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // For pointer events

public class App : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AppSO appData;  // Reference to the AppScriptableObject for app data
    public Image hoverBorder;  // Reference to the UI Image that represents the hover effect

    private AppManager appManager;

    void Start()
    {
        appManager = FindObjectOfType<AppManager>();  // Get the AppManager in the scene
    }

    // Called when the player clicks on the app
    public void OnAppClick()
    {
        // Pass the app data to the AppManager to handle opening
        appManager.OpenApp(appData, appData.appName);
    }

    // Fade effect for the hover border
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverBorder.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverBorder.gameObject.SetActive(false);
    }

}
