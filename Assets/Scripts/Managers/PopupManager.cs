using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject popup;
    public Button popupButton;
    public AppManager appManager;

    private Coroutine closePopupCoroutine; // To hold the coroutine so we can stop it if needed

    /// <summary>
    /// Shows the popup that happens when an app gets glitched.
    /// </summary>
    /// <param name="glitchedPopupName">The app that's getting glitched</param>
    public void ShowPopup(AppNames glitchedPopupName)
    {
        popup.SetActive(true);
        AssignButton(glitchedPopupName);

        // Start the auto-close coroutine
        if (closePopupCoroutine != null)
        {
            StopCoroutine(closePopupCoroutine); // Stop previous coroutine if popup was shown again
        }
        closePopupCoroutine = StartCoroutine(AutoClosePopup(5f)); // Set 5 seconds timer
    }

    /// <summary>
    /// Assigns the button action based on the glitched app.
    /// </summary>
    public void AssignButton(AppNames glitchedPopupName)
    {
        popupButton.onClick.RemoveAllListeners(); // Clear any previous listeners

        popupButton.onClick.AddListener(() => ClosePopup());
        // Add listener based on the app name
        if (glitchedPopupName == AppNames.SystemDevices)
        {
            
            popupButton.onClick.AddListener(() => appManager.OpenSystemDevices());
        }
    }

    /// <summary>
    /// Closes the popup when the button is clicked.
    /// </summary>
    public void ClosePopup()
    {
        popup.SetActive(false); // Hide the popup
    }

    /// <summary>
    /// Automatically closes the popup after a specified delay (in seconds).
    /// </summary>
    private IEnumerator AutoClosePopup(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified time
        ClosePopup(); // Close the popup after delay
    }
}
