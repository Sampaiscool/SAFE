using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClockApp : MonoBehaviour
{
    public TMP_Text timeDisplayText;  // The Text component to display the time
    public Button CloseButton;
    public GameManager gameManager;

    void Start()
    {
        // Start showing the time immediately when the scene loads (it now gets the time from GameManager)
        StartClock();

        CloseButton.onClick.AddListener(CloseApp); // Handle close button click
    }

    // Starts the clock immediately on scene load
    void StartClock()
    {
        // Start updating the clock immediately (every 1 second)
        InvokeRepeating("UpdateClock", 0f, 1f);  // Update the clock every 1 second
    }

    // Update the clock every second
    void UpdateClock()
    {
        // Get current time from GameManager
        float currentTimeInSeconds = GameManager.Instance.currentTimeInSeconds;

        // Convert the total time in seconds to hours and minutes
        int hours = Mathf.FloorToInt(currentTimeInSeconds / 3600);
        int minutes = Mathf.FloorToInt((currentTimeInSeconds % 3600) / 60);

        // Format the time string
        string timeString = string.Format("{0:D2}:{1:D2}", hours, minutes);

        // Update the text display with the current time
        timeDisplayText.text = "Time: " + timeString;
    }

    // Close the app when the close button is clicked
    void CloseApp()
    {
        gameObject.SetActive(false); // Disable the entire app panel
    }
}
