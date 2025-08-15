using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton Instance
    public static GameManager Instance { get; private set; }

    public float currentTimeInSeconds = 12 * 3600f; // Start time at 12:00 in seconds (12 * 3600)
    public float timeIncrement = 60f; // Increment time by 1 minute (60 seconds)

    public int minimalHeat = 30;
    public int currentHeat;
    public int maxHeat = 100;

    public Difficulties difficulty = Difficulties.Medium; // Default is Medium
    private float eventInterval;

    // Store the exit code here
    public string exitCode = "";  // To hold the entire exit.exe code (20 characters)

    // Define the characters to be used in the exit code (Uppercase, lowercase, numbers)
    private string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    // Array to hold all your ApplicationData instances (predefined)
    public ApplicationData[] allApplications;

    public GlitchManager glitchManager;
    public SystemDevices systemDevices;

    // Flag to track if the game is new
    public bool isNewGame = true;

    // Called on the first frame when the object is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make sure GameManager persists across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameManager if it exists
        }

        SetEventInterval();

        if (isNewGame)
        {
            // Reset unlocked applications at the start of a new game
            ResetAllUnlockedApplications();
            isNewGame = false; // Ensure this only happens once

            // Optionally generate the exit code on first start or if not present
            if (string.IsNullOrEmpty(exitCode))
            {
                GenerateExitCode();
            }
        }

        currentHeat = minimalHeat;

        // Start the clock
        StartClock();

        // Start the timed event
        InvokeRepeating("TimedEvent", eventInterval, eventInterval);  // Adjusted based on difficulty
    }

    // Generates a random 4-digit ID for the applications
    public int GenerateRandomID(int min, int max)
    {
        return Random.Range(min, max);
    }

    // Method to generate and assign IDs to all applications
    public void GenerateAppIds()
    {
        foreach (var app in allApplications)
        {
            // Generate random IDs for each application
            app.applicationId = GenerateRandomID(1000, 9999);  // 4-digit ID
            app.QuickFlashId = GenerateRandomID(10, 99);       // 2-digit ID

            // Save these IDs to PlayerPrefs (optional, if you want to persist them across sessions)
            PlayerPrefs.SetInt(app.applicationName + "_AppId", app.applicationId);
            PlayerPrefs.SetInt(app.applicationName + "_QuickFlashId", app.QuickFlashId);
        }

        // Save changes to PlayerPrefs
        PlayerPrefs.Save();
    }

    void SetEventInterval()
    {
        switch (difficulty)
        {
            case Difficulties.Easy:
                eventInterval = Random.Range(120f, 125f); ;  // 2 minutes
                break;
            case Difficulties.Medium:
                eventInterval = Random.Range(60f, 65f); ;   // 1 minute
                break;
            case Difficulties.Hard:
                eventInterval = Random.Range(30f, 35f);   // 30 seconds
                break;
            case Difficulties.Crazy:
                eventInterval = Random.Range(5f, 10f); // Random between 5-10 seconds
                break;
        }
    }
    public void GenerateExitCode()
    {
        exitCode = "";  // Reset the code before regenerating it

        // Generate a random 20-character code
        for (int i = 0; i < 20; i++)
        {
            exitCode += characters[Random.Range(0, characters.Length)];
        }

        Debug.Log("Exit Code Generated: " + exitCode);
    }

    /// <summary>
    /// Get a certain lenght of the code
    /// </summary>
    /// <param name="startIndex">start index</param>
    /// <param name="length">total charaters to get</param>
    /// <returns>The exit code for those numbers</returns>
    public string GetExitCodePart(int startIndex, int length)
    {
        if (startIndex + length <= exitCode.Length)
        {
            return exitCode.Substring(startIndex, length);
        }
        else
        {
            return "";  // Return an empty string if the range is invalid
        }
    }

    // Starts the clock immediately
    public void StartClock()
    {
        InvokeRepeating("UpdateClock", 0f, 1f); // Update every 1 second
    }

    // Update the clock every second
    private void UpdateClock()
    {
        currentTimeInSeconds += timeIncrement; // Increment the time by 1 minute (60 seconds)
    }

    // Reset unlocked applications for the current session
    public void ResetAllUnlockedApplications()
    {
        // Loop through all the predefined ApplicationData instances and reset them
        foreach (var app in allApplications)
        {
            // Reset each app to the locked state
            app.isUnlocked = false;

            // Reset PlayerPrefs for this app (lock the app)
            PlayerPrefs.SetInt(app.applicationName + "_Unlocked", 0);
        }

        // Save changes to PlayerPrefs after resetting
        PlayerPrefs.Save();
    }

    public void TimedEvent()
    {
        glitchManager.GlitchTimedEvent();
    }

    public void HeatChange(int amount)
    {
        if (currentHeat > maxHeat)
        {
            systemDevices.HeatAndCorruptionCheck();
        }

        currentHeat += amount;
    }
    public void ResetHeat()
    {
        currentHeat = minimalHeat;
    }

    /// <summary>
    /// Called when the player loses
    /// </summary>
    /// <param name="reason">The Reason the player lost (enum)</param>
    public void PlayerLost(DeathReason reason)
    {
        switch (reason)
        {
            case DeathReason.SystemDevices_CorruptionAndHeat:
                Debug.Log("Player lost due to heat and corruption.");
                break;
            case DeathReason.Jenuve_IncorrectCell:
                Debug.Log("Player lost due to being on the incorrect cell inside Jenuva");
                break;
            default:
                break;
        }
    }
}
