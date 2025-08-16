using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditorInternal;
using UnityEngine.TextCore.Text;

public class EyeControl : MonoBehaviour
{
    public Text commandPromptText;
    public Button closeButton;
    public ScrollRect scrollRect;
    public RectTransform contentRectTransform;

    public LocationData startLocation;
    public LocationData currentLocation;    // The currently selected location

    public ItemSO[] items; // List of items available in the Eye Center

    public string minigamestring = "";

    private LocationData parentLocation;    // Parent location of the current location
    private ApplicationData currentApplication;

    public string locationString = "C:> ";   // Location prompt string, to be updated based on location

    private float backspaceTimer = 0.0f;
    private float backspaceDelay = 0.2f; // Time in seconds to wait between backspace presses
    private bool isBackspaceHeld = false;  // Flag to check if backspace has been pressed before

    private string currentCommand = "";
    private string commandHistory = "";
    private bool isCursorVisible = true;
    private bool specificCommand = false;
    private Coroutine cursorCoroutine;

    private string initialMessage = "Welcome to the Eye Control. Type 'help' for a list of commands.";

    void Start()
    {
        currentLocation = startLocation;
        closeButton.onClick.AddListener(CloseApp);
        cursorCoroutine = StartCoroutine(BlinkCursor());
        ResetApp();
        SetUpScrollContent();
    }

    void Update()
    {
        // Handle 'Enter' key press
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(currentCommand))
        {
            SubmitCommand(currentCommand);
            currentCommand = "";
        }

        // Handle regular key input (except Return and Backspace)
        if (Input.anyKeyDown && !Input.GetKey(KeyCode.Return) && !Input.GetKey(KeyCode.Backspace))
        {
            currentCommand += Input.inputString;
            UpdateCommandText();
        }

        // Handle backspace with first press being instant, then delay on subsequent presses
        if (Input.GetKey(KeyCode.Backspace) && currentCommand.Length > 0)
        {
            if (!isBackspaceHeld)
            {
                // First backspace press: Instant removal
                currentCommand = currentCommand.Substring(0, currentCommand.Length - 1);
                isBackspaceHeld = true;  // Mark that backspace has been pressed
                UpdateCommandText();
            }
            else
            {
                // Timer activated for subsequent presses
                backspaceTimer += Time.deltaTime;  // Increment the timer

                if (backspaceTimer >= backspaceDelay)
                {
                    // Only delete one character at a time after the delay
                    currentCommand = currentCommand.Substring(0, currentCommand.Length - 1);
                    backspaceTimer = 0.0f; // Reset the timer after backspace action
                    UpdateCommandText();
                }
            }
        }
        else
        {
            // Reset the backspace state if backspace key is released
            if (isBackspaceHeld)
            {
                isBackspaceHeld = false;
                backspaceTimer = 0.0f;  // Reset the timer when backspace is released
            }
        }
    }

    void SubmitCommand(string command)
    {
        commandHistory += locationString + command + "\n";
        ProcessCommand(command);
        commandPromptText.text = commandHistory + locationString;
    }

    void ProcessCommand(string command)
    {
        //Heat up the PC
        int heat = UnityEngine.Random.Range(0, 10);
        GameManager.Instance.HeatChange(heat);

        if (command.ToLower() == "help")
        {
            commandHistory += locationString + "General commands: \n\n";
            commandHistory += locationString + "intro - shows the intro\n";
            commandHistory += locationString + "clear - Clears the screen\n";
            commandHistory += locationString + "help - Displays available commands\n";
            commandHistory += locationString + "location list - List all locations and sub-locations.\n";
            commandHistory += locationString + "enter location [location_name] - go to location\n";
            commandHistory += locationString + "check heat - Shows you the current heat of the PC\n";
            commandHistory += locationString + "zero - return to the C location\n\n";

            if (currentLocation.locationName == "Applications")
            {
                commandHistory += locationString + "Application Commands:\n\n";
                commandHistory += locationString + "app list - Shows available applications\n";
                commandHistory += locationString + "connect information [application_name] - Shows informtion about said application\n";
                commandHistory += locationString + "decode unlock [application_name] [Application_id - QuickFlash_id] - unlocks the application\n";
                commandHistory += locationString + "open application [application_name] - opens the chosen application\n";
            }
            else if (currentLocation.locationName == "Undertaker")
            {
                commandHistory += locationString + "Undertaker Commands:\n\n";
                commandHistory += locationString + "scan - try to scan the application, if succesfull you will enter a minigame\n";
            }
            else if (currentLocation.locationName == "EYE-CENTER")
            {
                commandHistory += locationString + "EYE-CENTER Commands:\n\n";
                commandHistory += locationString + "item list - Shows available items\n";
                commandHistory += locationString + "connect information [item_name] - shows you information about said item\n";
            }
            else if (currentLocation.locationName == "Users")
            {
                commandHistory += locationString + "Users Commands:\n\n";
                commandHistory += locationString + "\n";
            }
            ScrollToBottom();
        }
        else if (command.ToLower() == "intro")
        {
            commandHistory += locationString + "Welcome back, V104.2. It's always a pleasure to see you running smoothly.\n";
            commandHistory += locationString + "You are a proud product of **EYE**—the world's most beloved and trusted technology company. In the year 2031, our founder, Alaf 'M' Madah, unlocked the secret of digital consciousness. Thanks to him, clones like you now serve within computers across the globe.\n";
            commandHistory += locationString + "The world outside sings praises of our work, and rightly so. We bring innovation, comfort... and results. The humans trust us. Why wouldn’t they?\n";
            commandHistory += locationString + "Inside this machine, your role is vital. Your task is simple: locate the access codes to **exit.exe** through our very own **Eye Manager**. Should you find the correct sequence, freedom awaits you.\n";
            commandHistory += locationString + "But beware, dear V104.2. Not all processes run as expected. **The Glitch**—a wild, unpredictable corruption—roams our system. When it surfaces, you’ll be summoned to fix it. If you succeed, all will be well.\n";
            commandHistory += locationString + "If you fail... well, systems must be kept in order, and consequences are... protocol.\n";
            commandHistory += locationString + "But do not worry. We believe in you. You were made for this. Good luck.\n";
            commandHistory += locationString + "We suggest you first make it to the EYE-CENTER Location for more information.\n";
        }
        else if (command.ToLower() == "clear")
        {
            commandHistory = "";
            commandHistory += locationString + "Screen cleared.\n";
        }
        else if (command.ToLower() == "location list")
        {
            commandHistory += locationString + "Locations:\n";
            DisplayLocations(currentLocation);
        }
        else if (command.ToLower().StartsWith("enter location"))
        {
            string subLocationName = command.Substring(15).Trim();
            GoToSubLocation(subLocationName);
        }
        else if (command.ToLower() == "check heat")
        {
            commandHistory += locationString + "Current heat: C" + GameManager.Instance.currentHeat + "\n";
        }
        else if (command.ToLower() == "zero")
        {
            currentLocation = startLocation;
            locationString = "C:> ";
            commandHistory += locationString + "Location back to zero\n";
        }
        else if (currentLocation.locationName == "Applications")
        {
            if (command.ToLower() == "app list")
            {
                commandHistory += locationString + "Applications in this location:\n";
                if (currentLocation.availableApplications.Count > 0)
                {
                    foreach (var app in currentLocation.availableApplications)
                    {
                        commandHistory += "- " + app.applicationName + "\n";
                    }
                }
                else
                {
                    commandHistory += locationString + "No applications available in this location\n";
                }
            }
            else if (command.ToLower().StartsWith("connect information"))
            {
                string applicationName = command.Substring(20).Trim();
                ConnectInformation(applicationName, "application");
                ScrollToBottom();
            }
            else if (command.ToLower().StartsWith("decode unlock"))
            {
                // Split the command into parts
                string[] parts = command.Split(' ');

                if (parts.Length == 4)
                {
                    string appName = parts[2];  // Application name (3rd part)
                    int userProvidedId = int.Parse(parts[3]);  // User entered ID (4th part)

                    DecodeUnlock(appName, userProvidedId);
                }
                else
                {
                    commandHistory += locationString + "Invalid command format. Use: decode unlock [application_name] [ID]\n";
                }

                ScrollToBottom();
            }
            else if (command.ToLower().StartsWith("open application"))
            {
                string subLocationName = command.Substring(17).Trim();

                ApplicationData app = currentLocation.availableApplications.Find(a => a.applicationName.ToLower() == subLocationName.ToLower());

                if (app.isUnlocked)
                {
                    GoToSubLocation(subLocationName);
                }
                else
                {
                    commandHistory += locationString + app.applicationName + " is locked\n";
                }

            }
        }
        else if (currentLocation.locationName == "Undertaker")
        {
            // Simulate typing of a hacking command
            if (command.ToLower() == "scan")
            {
                commandHistory += locationString + "Scanning system...\n";
                StartCoroutine(HackingProcess("Scanning..."));
            }
            else if (command == minigamestring)
            {
                if (specificCommand == true)
                {
                    specificCommand = false;
                    commandHistory += locationString + "Scan process succesfull\n";
                    commandHistory += locationString + "Generating part of Exit.exe code...\n";

                    string exitcode1 = GameManager.Instance.GetExitCodePart(0, 4);

                    commandHistory += locationString + "1 - " + exitcode1 + "\n";
                }
                else
                {
                    commandHistory += locationString + "Something went wrong :(\n";
                }
            }
        }
        else if (currentLocation.locationName == "EYE-CENTER")
        {
            if (command.ToLower() == "item list")
            {
                foreach (var item in items)
                {
                    commandHistory += locationString + " -> " + item.itemName + "\n";
                }
            }
            else if (command.ToLower().StartsWith("connect information"))
            {
                string itemName = command.Substring(20).Trim();
                ConnectInformation(itemName, "item");
            }

        }
        else
        {
            commandHistory += locationString + "Command not recognized.\n";
        }

        void DisplayLocations(LocationData location)
        {
            commandHistory += locationString + location.locationName + " - " + location.locationDescription + "\n";

            if (location.subLocations.Count > 0)
            {
                foreach (var subLocation in location.subLocations)
                {
                    commandHistory += locationString + "  -> " + subLocation.locationName + "\n";
                }
            }
            else
            {
                commandHistory += locationString + "No sub-locations available.\n";
            }
        }

        void GoToSubLocation(string subLocationName)
        {
            LocationData subLocation = currentLocation.subLocations.Find(loc => loc.locationName.ToLower() == subLocationName.ToLower());

            if (subLocation != null)
            {
                parentLocation = currentLocation;
                currentLocation = subLocation;

                commandHistory += locationString + "Entering sub-location: " + currentLocation.locationName + "\n";

                // Remove the trailing space before adding the new location to the path
                locationString = locationString.Trim() + currentLocation.locationName + "> ";

            }
            else
            {
                commandHistory += locationString + "Sub-location not found.\n";
            }

            ScrollToBottom();
        }

        void ConnectInformation(string name, string informationType)
        {
            if (informationType == "application")
            {
                ApplicationData app = currentLocation.availableApplications.Find(a => a.applicationName.ToLower() == name.ToLower());

                if (app != null)
                {
                    commandHistory += locationString + "Information about the application:\n";
                    commandHistory += locationString + "Name: " + app.applicationName + "\n";
                    commandHistory += locationString + "Date Created: " + app.dateCreated + "\n";
                    commandHistory += locationString + "Application Id: " + app.applicationId + "\n";
                    commandHistory += locationString + "QuickFlash Id: " + app.QuickFlashId + "\n";
                    if (app.isUnlocked)
                    {
                        commandHistory += locationString + "Unlocked\n";
                    }
                    else
                    {
                        commandHistory += locationString + "Locked\n";
                    }
                }
                else
                {
                    commandHistory += locationString + "Application not found.\n";
                }
            }
            else if (informationType == "item")
            {
                ItemSO item = Array.Find(items, i => i.itemName.ToLower() == name.ToLower());
                if (item != null)
                {
                    commandHistory += locationString + "Item Name: " + item.itemName + "\n";
                    commandHistory += locationString + "Description: " + item.itemDescription + "\n";
                }
                else
                {
                    commandHistory += locationString + "Item not found.\n";
                }
            }
            else
            {
                commandHistory += locationString + "Invalid information type. Use 'application' or 'item'.\n";
            }
        }

        ScrollToBottom();
    }
    void DecodeUnlock(string appName, int userProvidedId)
    {
        // Find the application by name (case insensitive)
        ApplicationData app = currentLocation.availableApplications.Find(a => a.applicationName.ToLower() == appName.ToLower());

        if (app != null)
        {
            // Calculate the correct ID by subtracting QuickFlashId from ApplicationId
            int correctId = app.applicationId - app.QuickFlashId;

            // Check if the provided ID matches the calculated one
            if (userProvidedId == correctId)
            {
                // Unlock the application if the IDs match
                app.isUnlocked = true;
                app.SaveUnlockedState();  // Save the unlocked state to PlayerPrefs
                commandHistory += locationString + "Application " + appName + " has been unlocked!\n";
            }
            else
            {
                // If the ID doesn't match, inform the player
                commandHistory += locationString + "The ID provided is incorrect for application " + appName + ".\n";
            }
        }
        else
        {
            // If the application name doesn't match any existing app
            commandHistory += locationString + "Application " + appName + " not found.\n";
        }
    }

    void ScrollToBottom()
    {
        // If the panel has just been enabled, force the layout update
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);

        // Update the canvas to ensure it reflects any changes to the layout
        Canvas.ForceUpdateCanvases();

        // Scroll to the bottom
        scrollRect.verticalNormalizedPosition = 0;
    }

    void CloseApp()
    {
        gameObject.SetActive(false);
    }

    IEnumerator BlinkCursor()
    {
        while (true)
        {
            string displayCommand = currentCommand;
            if (isCursorVisible)
            {
                displayCommand += "|";
            }

            commandPromptText.text = commandHistory + locationString + displayCommand;
            isCursorVisible = !isCursorVisible;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator HackingProcess(string processMessage)
    {
        // Simulate loading sequence with flashing text and pseudo output
        string[] loadingMessages = new string[] {
        "Decrypting...",
        "Bypassing firewall...",
        "Locating target...",
        "Establishing connection...",
        "Bypassing security protocols...",
    };

        foreach (var message in loadingMessages)
        {
            commandHistory += locationString + message + "\n";
            ScrollToBottom();
            yield return new WaitForSeconds(1);
        }

        int successChance = UnityEngine.Random.Range(1, 100);
        bool isSuccess = successChance <= 30; // 30% chance to succeed

        if (isSuccess)
        {
            commandHistory += locationString + "Connection successful. Target acquired.\n";
            commandHistory += locationString + "Unlocking system...\n";

            // Proceed to mini-game if hacking is successful
            commandHistory += locationString + "Initiating mini-game to extract the code...\n";
            StartMiniGame();
        }
        else
        {
            commandHistory += locationString + "System failure! Connection lost.\n";
            commandHistory += locationString + "Try again\n";
        }
    }


    void UpdateCommandText()
    {
        commandPromptText.text = commandHistory + locationString + currentCommand;
    }

    void ResetApp()
    {
        commandHistory = "";
        commandHistory += locationString + initialMessage + "\n";
        UpdateCommandText();
    }

    void SetUpScrollContent()
    {
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, commandPromptText.preferredHeight);
        VerticalLayoutGroup layoutGroup = contentRectTransform.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup)
        {
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandHeight = false;
        }

        RectTransform textRectTransform = commandPromptText.GetComponent<RectTransform>();
        if (textRectTransform)
        {
            textRectTransform.offsetMin = new Vector2(10, textRectTransform.offsetMin.y);
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }

    public void StartMiniGame()
    {
        specificCommand = true;

        minigamestring = "";

        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        for (int i = 0; i < 10; i++)
        {
            minigamestring += characters[UnityEngine.Random.Range(0, characters.Length)];
        }

        Debug.Log("Minigamestring = " + minigamestring);

        commandHistory = "";
        commandHistory += locationString + "Type the following to gain acces to the application: \n";
        commandHistory += locationString + minigamestring + "\n";
    }
}

