using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // --------------------- Singleton ---------------------
    public static GameManager Instance { get; private set; }

    // --------------------- Time System ---------------------
    [Header("Time Settings")]
    public float currentTimeInSeconds = 12 * 3600f; // Start at 12:00 (in seconds)
    public float timeIncrement = 60f;               // Increment time by 1 min (60s)

    // --------------------- Heat System ---------------------
    [Header("Heat Settings")]
    public int minimalHeat = 30;
    public int currentHeat;
    public int maxHeat = 100;

    // --------------------- Economy ---------------------
    [Header("Economy Settings")]
    public int startCoins;
    public int currentCoins;

    // --------------------- Difficulty ---------------------
    [Header("Difficulty Settings")]
    public Difficulties difficulty = Difficulties.Medium;
    private float eventInterval;

    // --------------------- Exit Code ---------------------
    [Header("Exit.exe Code")]
    public string exitCode = "";  // Player’s winning code (20 chars)
    private string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    // --------------------- Applications ---------------------
    [Header("Applications")]
    public ApplicationData[] allApplications;
    public Dictionary<ApplicationData, string> SessionNames = new Dictionary<ApplicationData, string>();

    // --------------------- Core Managers ---------------------
    [Header("Managers & Systems")]
    public GlitchManager glitchManager;
    public SessionManager sessionManager;
    public SettingsManager settingsManager;
    public AudioManager audioManager;

    // --------------------- Apps & Minigames ---------------------
    [Header("Apps & Minigames")]
    public SystemDevices systemDevices;
    public MadahShop madahShop;
    public Ghost ghost;
    public EyeControl EyeControl;
    public Notes Notes;
    public Jenuve jenuve;
    public GridFlip kFlipped;

    // --------------------- Game State ---------------------
    [Header("Game State")]
    public AppNames CurrentControl = AppNames.None;
    public DeathReason LostDueTo = DeathReason.None;
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

        difficulty = PlayerSettings.chosenDifficulty;
        SetEventInterval();

        if (isNewGame)
        {
            sessionManager.StartNewSession();
            sessionManager.InitializeApplicationsForGame();
            // Reset unlocked applications at the start of a new game
            ResetAllUnlockedApplications();
            isNewGame = false; // Ensure this only happens once

            ResetAllApplicationIds();

            // Optionally generate the exit code on first start or if not present
            if (string.IsNullOrEmpty(exitCode))
            {
                GenerateExitCode();
            }

            //Set the Start Coins amount
            switch (difficulty)
            {
                case Difficulties.Easy:
                    CoinsChange(20, false);
                    break;
                case Difficulties.Medium:
                    CoinsChange(15, false);
                    break;
                case Difficulties.Hard:
                    CoinsChange(10, false);
                    break;
                case Difficulties.Crazy:
                    CoinsChange(0, false);
                    break;
                default:
                    break;
            }
            currentCoins += startCoins;
        }

        currentHeat = minimalHeat;

        // Start the clock
        StartClock();

        // Start the timed event
        InvokeRepeating("TimedEvent", eventInterval, eventInterval);  // Adjusted based on difficulty
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CurrentControl = AppNames.None;

            settingsManager.ToggleSettings();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            // --- Managers ---
            glitchManager = FindObjectOfType<GlitchManager>();
            sessionManager = FindObjectOfType<SessionManager>();
            settingsManager = FindObjectOfType<SettingsManager>();
            audioManager = FindObjectOfType<AudioManager>();

            // --- Apps ---
            systemDevices = FindObjectOfType<SystemDevices>();
            madahShop = FindObjectOfType<MadahShop>();
            EyeControl = FindObjectOfType<EyeControl>();
            Notes = FindObjectOfType<Notes>();
            jenuve = FindObjectOfType<Jenuve>();
            kFlipped = FindObjectOfType<GridFlip>();
            ghost = FindObjectOfType<Ghost>();

            Debug.Log("GameManager re-linked scene references!");
        }
    }

    // Generates a random 4-digit ID for the applications
    public int GenerateRandomID(int min, int max)
    {
        return Random.Range(min, max);
    }

    // Method to generate and assign IDs to all applications
    public void ResetAllApplicationIds()
    {
        // Delete old Prefs

        // generate New Id's
        foreach (var app in allApplications)
        {
            app.SaveIds();
        }
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
        glitchManager.GlitchTimedEvent(null);
    }

    public void HeatChange(int amount)
    {
        if (currentHeat > maxHeat)
        {
            systemDevices.HeatAndCorruptionCheck();
        }

        currentHeat += amount;
    }

    /// <summary>
    /// Changes the coins amount
    /// </summary>
    /// <param name="amount">The amount that will be added / removed</param>
    /// <param name="IsPurchase">true = amount will be removed, false = amount will be added</param>
    public void CoinsChange(int amount, bool IsPurchase)
    {
        if (IsPurchase == true)
        {
            currentCoins -= amount;
            madahShop.UpdateCoinText(-amount);
        }
        else
        {
            currentCoins += amount;
            madahShop.UpdateCoinText(+amount);
        }

        
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
        StopAllGameSystems();

        LostDueTo = reason;
        UnityEngine.SceneManagement.SceneManager.LoadScene("LostScene");

        switch (reason)
        {
            case DeathReason.SystemDevices_CorruptionAndHeat:
                Debug.Log("Player lost due to heat and corruption.");
                break;
            case DeathReason.Jenuve_IncorrectCell:
                Debug.Log("Player lost due to being on the incorrect cell inside Jenuva.");
                break;
            case DeathReason.Jenuve_GlitchMiniGameFailed:
                Debug.Log("Player lost due to Failing to fix the glitch in time inside Jenuva.");
                break;
            case DeathReason.KFlipped_NotFinished:
                Debug.Log("Player lost due to not finishing the KFlipped minigame.");
                break;
            case DeathReason.Debug_Death:
                Debug.Log("Player lost due to debug death.");
                break;
        }
    }
    public void StopAllGameSystems()
    {
        CancelInvoke();            // stop all InvokeRepeating, including UpdateClock + TimedEvent
    }

    public void ResetGame()
    {
        Debug.Log("Player has reset the game after death");

        // Reset coins/heat/time/difficulty
        difficulty = PlayerSettings.chosenDifficulty;
        currentHeat = minimalHeat;
        currentTimeInSeconds = 12 * 3600f;
        
        switch (difficulty)
        {
            case Difficulties.Easy:
                currentCoins = 20;
                break;
            case Difficulties.Medium:
                currentCoins = 15;
            break;
            case Difficulties.Hard:
                currentCoins = 10;
                break;
            case Difficulties.Crazy:
                currentCoins = 0;
                break;
            default:
                break;
        }

        LostDueTo = DeathReason.None;

        // Reset unlocked apps
        ResetAllUnlockedApplications();
        ResetAllApplicationIds();

        // Optionally regenerate exit code
        GenerateExitCode();

        // Restart systems
        SetEventInterval();
        StartClock();
        InvokeRepeating("TimedEvent", eventInterval, eventInterval);
    }
}
